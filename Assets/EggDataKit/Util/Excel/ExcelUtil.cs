#region

//文件创建者：Egg
//创建时间：10-30 03:44

#endregion

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EggFramework.Util.Res;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Util.Excel
{
    public static class ExcelUtil
    {
        public static IEnumerable<string> ExcelTypes
        {
            get
            {
                var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
                var ret     = setting.Configs.Select(c => c.TypeName).ToList();
                ret.AddRange(TypeUtil.DefaultTypes);
                ret.AddRange(TypeUtil.ResTypes);
                ret.AddRange(TypeUtil.UnityStructTypes);
                return ret;
            }
        }

        public const string TABLE_HEAD_SIGN   = "*";
        public const string LIST_SPLIT_SIGN   = "|";
        public const string VECTOR_SPLIT_SIGN = "-";

        private static bool _inited;

        private static readonly Dictionary<Type, Func<string, object>> _switchFuncs = new(); //

        private static void RegisterTypeHandle(Type type, Func<string, object> switchFunc)
        {
            _switchFuncs.TryAdd(type, switchFunc);
        }

        private static void SetData(object target, FieldInfo memberField, List<string> datas,
            ref int headDataIndex)
        {
            MakeSureInited();

            if (typeof(IList).IsAssignableFrom(memberField.FieldType))
            {
                var eleType = memberField.FieldType.GenericTypeArguments[0];
                var list    = (IList)Activator.CreateInstance(memberField.FieldType);
                try
                {
                    var data = datas[headDataIndex];
                    headDataIndex++;
                    var eleRaw = data.Split(LIST_SPLIT_SIGN);
                    foreach (var se in eleRaw)
                    {
                        if (_switchFuncs.TryGetValue(eleType, out var func))
                        {
                            var ret = func?.Invoke(se);
                            if (ret != null)
                                list!.Add(ret);
                        }
                    }

                    memberField.SetValue(target, list);
                }
                catch (Exception e)
                {
                    Debug.LogError("配置信息与实际信息冲突，请重新生成表格文件");
                    throw;
                }
            }
            else
            {
                if (_switchFuncs.TryGetValue(memberField.FieldType, out var func))
                {
                    try
                    {
                        memberField.SetValue(target, func?.Invoke(datas[headDataIndex]));
                        headDataIndex++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("配置信息与实际信息冲突，请重新生成表格文件");
                        throw;
                    }
                }
                else
                {
                    var fieldStruct = Activator.CreateInstance(memberField.FieldType);
                    var type        = fieldStruct.GetType();
                    var fields      = type.GetSerializeFieldInfos();
                    foreach (var fieldInfo in fields)
                    {
                        SetData(fieldStruct, fieldInfo, datas, ref headDataIndex);
                    }

                    memberField.SetValue(target, fieldStruct);
                }
            }
        }

        private static void MakeSureInited() //
        {
            if (_inited) return;
            RegisterTypeHandle(typeof(int),    se => int.TryParse(se.Trim(), out var intValue) ? intValue : 0);
            RegisterTypeHandle(typeof(float),  se => float.TryParse(se.Trim(), out var floatValue) ? floatValue : 0);
            RegisterTypeHandle(typeof(double), se => double.TryParse(se.Trim(), out var doubleValue) ? doubleValue : 0);
            RegisterTypeHandle(typeof(string), se => se.Trim());
            RegisterTypeHandle(typeof(bool),   se => se.Trim() == "1");
            RegisterTypeHandle(typeof(Vector3), se =>
            {
                var raw          = se.Trim();
                var digitStrings = raw.Split(VECTOR_SPLIT_SIGN);
                var digit1 = digitStrings.Length > 0
                    ? (float.TryParse(digitStrings[0].Trim(), out var d1) ? d1 : 0)
                    : 0;
                var digit2 = digitStrings.Length > 1
                    ? (float.TryParse(digitStrings[1].Trim(), out var d2) ? d2 : 0)
                    : 0;
                var digit3 = digitStrings.Length > 2
                    ? (float.TryParse(digitStrings[2].Trim(), out var d3) ? d3 : 0)
                    : 0;
                return new Vector3(digit1, digit2, digit3);
            });
            RegisterTypeHandle(typeof(Vector2), se =>
            {
                var raw          = se.Trim();
                var digitStrings = raw.Split(VECTOR_SPLIT_SIGN);
                var digit1 = digitStrings.Length > 0
                    ? (float.TryParse(digitStrings[0].Trim(), out var d1) ? d1 : 0)
                    : 0;
                var digit2 = digitStrings.Length > 1
                    ? (float.TryParse(digitStrings[1].Trim(), out var d2) ? d2 : 0)
                    : 0;
                return new Vector2(digit1, digit2);
            });
            RegisterTypeHandle(typeof(Color), se => ColorUtil.ParseHexColor(se));

            var resRefTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
            foreach (var resRefType in resRefTypes)
            {
                var refData  = GetCacheResRefData(resRefType);
                var baseType = refData.GetType().BaseType;
                var dataType = baseType!.GenericTypeArguments[0];
                RegisterTypeHandle(dataType, se =>
                {
                    var fieldInfo = baseType!.GetField("ResRefs");
                    var list      = (IList)fieldInfo.GetValue(refData);
                    foreach (var o in list)
                    {
                        var type = o.GetType();
                        var name = (string)type.GetField("Name").GetValue(o);
                        var data = type.GetField("Data").GetValue(o);
                        if (name == se) return data;
                    }

                    return null;
                });
            }


            _inited = true;
        }
        
        public static void ReadDataByExcelEntitySOType(Type soType)
        {
            var refData = ResUtil.GetAsset<ExcelRefData>();
            if (refData == null)
            {
                Debug.LogError("没有生成Excel引用文件，请先点击·生成csv文件和代码数据类·");
            }

            var excelEntityTypes = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            var targetType     = excelEntityTypes.Find(type => soType.Name == type.Name + "SO");
            var dataRef          = refData.ExcelDataRefs.Find(df => df.Name == targetType.Name);
            if (string.IsNullOrEmpty(dataRef.Name))
            {
                Debug.LogError("没有找到对应的数据引用");
            }
            else
            {
                var list = ReadData(dataRef.TextAsset, targetType);
                var data = GetOrCreateExcelSOData(soType);
                soType.GetField("RawDataList").SetValue(data, list);
                EditorUtility.SetDirty(data);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ReadDataByExcelEntityType(Type type)
        {
            var refData = ResUtil.GetAsset<ExcelRefData>();
            if (refData == null)
            {
                Debug.LogError("没有生成Excel引用文件，请先点击·生成csv文件和代码数据类·");
            }

            var excelEntitySOTypes = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            var targetSOType       = excelEntitySOTypes.Find(soType => soType.Name == type.Name + "SO");
            var dataRef            = refData.ExcelDataRefs.Find(df => df.Name == type.Name);
            if (string.IsNullOrEmpty(dataRef.Name))
            {
                Debug.LogError("没有找到对应的数据引用");
            }
            else
            {
                var list = ReadData(dataRef.TextAsset, type);
                var data = GetOrCreateExcelSOData(targetSOType);
                targetSOType.GetField("RawDataList").SetValue(data, list);
                EditorUtility.SetDirty(data);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ReadData()
        {
            var refData = ResUtil.GetAsset<ExcelRefData>();
            if (refData == null)
            {
                Debug.LogError("没有生成Excel引用文件，请先点击·生成csv文件和代码数据类·");
            }

            var excelEntityTypes   = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            var excelEntitySOTypes = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            foreach (var targetType in excelEntityTypes)
            {
                var targetSOType = excelEntitySOTypes.Find(type => type.Name == targetType.Name + "SO");
                var dataRef      = refData.ExcelDataRefs.Find(df => df.Name == targetType.Name);
                if (string.IsNullOrEmpty(dataRef.Name))
                {
                    Debug.LogError("没有找到对应的数据引用");
                }
                else
                {
                    var list = ReadData(dataRef.TextAsset, targetType);
                    var data = GetOrCreateExcelSOData(targetSOType);
                    targetSOType.GetField("RawDataList").SetValue(data, list);
                    EditorUtility.SetDirty(data);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Object GetOrCreateExcelSOData(Type type)
        {
            var setting = StorageUtil.LoadFromSettingFile("ExcelSetting", new ExcelSetting());
            var path    = $"{setting.DataPathRoot}/{type.Name}.asset";
            var data    = ResUtil.GetOrCreateAsset(path, type);
            ResUtil.AddAssetToGroup("ExcelEntitySO", path, true, true);
            return data;
        }

        private static readonly Dictionary<Type, Object> _cacheResRefDatas = new();

        private static Object GetCacheResRefData(Type resRefType)
        {
            if (_cacheResRefDatas.TryGetValue(resRefType, out var data))
            {
                if (data != null)
                    return data;
            }

            data = ResUtil.GetAsset(resRefType);
            if (data == null)
            {
                ResUtil.RefreshResRef();
                data = ResUtil.GetAsset(resRefType);
            }

            _cacheResRefDatas[resRefType] = data;
            return _cacheResRefDatas[resRefType];
        }

        public static List<string> ParseDataFromLine(string data)
        {
            var ret     = new List<string>();
            var builder = new StringBuilder();
            var ignore  = false;
            foreach (var c in data)
            {
                switch (c)
                {
                    case '\"':
                        ignore = !ignore;
                        break;
                    case ',':
                        if (ignore)
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            ret.Add(builder.ToString());
                            builder.Clear();
                        }

                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            ret.Add(builder.ToString());
            return ret;
        }

        public static IEnumerable ReadData(TextAsset textAsset, Type type)
        {
            ResUtil.RefreshResRef(false);
            var text       = textAsset.text;
            var entityList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            var dataLines  = text.Split("\n");
            foreach (var t in dataLines)
            {
                var dataLine = t.Trim();
                if (string.IsNullOrEmpty(dataLine)) continue;
                var datas = ParseDataFromLine(dataLine);
                if (datas.Any(se => se.StartsWith(TABLE_HEAD_SIGN))) continue;
                var entity         = Activator.CreateInstance(type);
                var fields         = type.GetSerializeFieldInfos().ToList();
                var tableHeadIndex = 0;
                foreach (var fieldInfo in fields)
                {
                    SetData(entity, fieldInfo, datas, ref tableHeadIndex);
                }

                entityList.Add(entity);
            }

            return entityList;
        }

        public static List<T> ReadData<T>(TextAsset textAsset) where T : IExcelEntity
        {
            var text       = textAsset.text;
            var entityList = new List<T>();
            var dataLines  = text.Split("\n");
            foreach (var t in dataLines)
            {
                var dataLine = t.Trim();
                if (dataLine.StartsWith(TABLE_HEAD_SIGN)) continue; //跳过表头
                if (string.IsNullOrEmpty(dataLine)) continue;
                var datas          = dataLine.Split(",").ToList();
                var entity         = Activator.CreateInstance<T>();
                var fields         = typeof(T).GetSerializeFieldInfos().ToList();
                var tableHeadIndex = 0;
                foreach (var fieldInfo in fields)
                {
                    SetData(entity, fieldInfo, datas, ref tableHeadIndex);
                }

                entityList.Add(entity);
            }

            return entityList;
        }
    }
}

#endif