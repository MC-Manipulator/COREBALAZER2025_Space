#region

//文件创建者：Egg
//创建时间：10-30 03:44

#endregion

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EggFramework.Util.Res;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Util.Excel
{
    public static partial class ExcelUtil
    {
        private static readonly Dictionary<Type, Type> _type2RefTypeDic      = new();
        private static readonly Dictionary<Type, Type> _type2DataViewTypeDic = new();

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
        public const string VECTOR_SPLIT_SIGN = ":";

        private static bool _inited;

        private static readonly Dictionary<Type, Func<string, object>> _parserFuncs    = new();
        private static readonly Dictionary<Type, Func<object, string>> _serializeFuncs = new();

        public static ExcelStructConfig GetExcelStructConfigByName(string configName)
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            return setting.Configs.Find(config => config.TypeName == configName);
        }

        private static List<ExcelStructConfig> _cachedStructConfig;
        private static List<ExcelTableConfig>  _cachedTableConfig;

        public static void CacheExcelStructConfig()
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            _cachedStructConfig = setting.Configs;
        }

        public static void CacheExcelTableConfig()
        {
            var configs = StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
            _cachedTableConfig = configs;
        }

        public static ExcelTableConfig GetCachedExcelTableConfigByName(string configName)
        {
            if (_cachedTableConfig != null)
            {
                return _cachedTableConfig.Find(config => config.ConfigName == configName);
            }

            return new ExcelTableConfig();
        }

        public static ExcelStructConfig GetCachedExcelStructConfigByName(string configName)
        {
            if (_cachedStructConfig != null)
            {
                return _cachedStructConfig.Find(config => config.TypeName == configName);
            }

            return GetExcelStructConfigByName(configName);
        }

        public static Dictionary<string, int> GenerateConfigColDic(ExcelTableConfig tableConfig,
            Func<string, ExcelStructConfig> structConfigGetter)
        {
            if (structConfigGetter == null) return null;
            var colIndex           = 0;
            var colReloadConfigDic = new Dictionary<string, int>();
            foreach (var excelColConfig in tableConfig.Columns)
            {
                if (excelColConfig.IsStruct)
                {
                    if (excelColConfig.IsList)
                    {
                        throw new Exception("不支持结构体List");
                    }

                    var structConfig = structConfigGetter.Invoke(excelColConfig.ExcelColType);
                    foreach (var structConfigColumn in structConfig.Columns)
                    {
                        if (structConfigColumn.IsStruct)
                        {
                            throw new Exception("不支持结构体嵌套");
                        }

                        colReloadConfigDic.Add(
                            $"{excelColConfig.Name}_{excelColConfig.ExcelColType}->{structConfigColumn.Name}_{(structConfigColumn.IsList ? $"List<{structConfigColumn.ExcelColType}>" : structConfigColumn.ExcelColType)}",
                            colIndex);
                        colIndex++;
                    }
                }
                else
                {
                    colReloadConfigDic.Add(
                        $"{excelColConfig.Name}_{(excelColConfig.IsList ? $"List<{excelColConfig.ExcelColType}>" : excelColConfig.ExcelColType)}",
                        colIndex);
                    colIndex++;
                }
            }

            return colReloadConfigDic;
        }

        private static void RegisterParserHandle(Type type, Func<string, object> switchFunc)
        {
            _parserFuncs.TryAdd(type, switchFunc);
        }

        private static void RegisterSerializeHandle(Type type, Func<object, string> switchFunc)
        {
            _serializeFuncs.TryAdd(type, switchFunc);
        }

        public static object ParseData(Type targetType, string value)
        {
            MakeSureInited();

            if (typeof(IList).IsAssignableFrom(targetType))
            {
                var ret     = (IList)Activator.CreateInstance(targetType);
                var eleType = targetType.GenericTypeArguments[0];
                var datas   = value.Split("|");
                foreach (var data in datas)
                {
                    var path = data.Trim();
                    if (!string.IsNullOrEmpty(path))
                        ret.Add(ParseSingleData(eleType, path));
                }

                return ret;
            }


            return ParseSingleData(targetType, value);
        }

        private static object ParseSingleData(Type targetType, string value)
        {
            if (_parserFuncs.TryGetValue(targetType, out var func))
            {
                try
                {
                    return func?.Invoke(value);
                }
                catch (Exception e)
                {
                    Debug.LogError("配置信息与实际信息冲突，请重新生成表格文件");
                    throw;
                }
            }

            Debug.LogError("非法数据类型");
            return null;
        }

        public static string SerializeData<T>(T value)
        {
            return SerializeData(value, typeof(T));
        }

        public static string SerializeData(object value, Type type)
        {
            MakeSureInited();
            if (typeof(IList).IsAssignableFrom(type))
            {
                var ret     = new StringBuilder();
                var eleType = type.GenericTypeArguments[0];
                for (var index = 0; index < ((IList)value).Count; index++)
                {
                    var o = ((IList)value)[index];
                    ret.Append(SerializeSingleData(o, eleType));
                    if (index != ((IList)value).Count - 1)
                        ret.Append("|");
                }

                return ret.ToString();
            }

            return SerializeSingleData(value, type);
        }

        private static string SerializeSingleData(object value, Type type)
        {
            if (_serializeFuncs.TryGetValue(type, out var func))
            {
                try
                {
                    return func?.Invoke(value);
                }
                catch (Exception e)
                {
                    Debug.LogError("配置信息与实际信息冲突，请重新生成表格文件");
                    throw;
                }
            }

            Debug.LogError("非法数据类型");
            return null;
        }

        public static T ParseData<T>(string value)
        {
            return (T)ParseData(typeof(T), value);
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
                    if (eleRaw.Length == 1 && string.IsNullOrEmpty(eleRaw[0])) return;
                    foreach (var se in eleRaw)
                    {
                        if (_parserFuncs.TryGetValue(eleType, out var func))
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
                if (_parserFuncs.TryGetValue(memberField.FieldType, out var func))
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
            RegisterParserHandle(typeof(int), se => int.TryParse(se.Trim(), out var intValue) ? intValue : 0);
            RegisterSerializeHandle(typeof(int), val => val.ToString());
            RegisterParserHandle(typeof(float), se => float.TryParse(se.Trim(), out var floatValue) ? floatValue : 0);
            RegisterSerializeHandle(typeof(float), val => val.ToString());
            RegisterParserHandle(typeof(double),
                se => double.TryParse(se.Trim(), out var doubleValue) ? doubleValue : 0);
            RegisterSerializeHandle(typeof(double),
                val => val.ToString());
            RegisterParserHandle(typeof(string), se => se.Trim());
            RegisterSerializeHandle(typeof(string), se => se.ToString());
            RegisterParserHandle(typeof(bool), se => se.Trim() == "1");
            RegisterSerializeHandle(typeof(bool), val => (bool)val ? "1" : "0");
            RegisterParserHandle(typeof(Vector3), se =>
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
            RegisterSerializeHandle(typeof(Vector3), val =>
            {
                var vec = (Vector3)val;
                return $"{vec.x}:{vec.y}:{vec.z}";
            });
            RegisterParserHandle(typeof(Vector2), se =>
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
            RegisterSerializeHandle(typeof(Vector2), val =>
            {
                var vec = (Vector2)val;
                return $"{vec.x}:{vec.y}";
            });
            RegisterParserHandle(typeof(Color), se => ColorUtil.ParseHexColor(se));
            RegisterSerializeHandle(typeof(Color), val => ColorUtil.Color2Hex((Color)val));

            var resRefTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
            foreach (var resRefType in resRefTypes)
            {
                var refData  = GetCacheResRefData(resRefType);
                var baseType = refData.GetType().BaseType;
                var dataType = baseType!.GenericTypeArguments[0];
                RegisterParserHandle(dataType, se =>
                {
                    if (dataType != typeof(Sprite)) return AssetDatabase.LoadAssetAtPath(se, dataType);
                    if (!se.Contains("_$_")) return AssetDatabase.LoadAssetAtPath<Sprite>(se);
                    var parts      = se.Split(new[] { "_$_" }, StringSplitOptions.None);
                    var assetPath  = parts[0];
                    var spriteName = parts[1];
                    var subAssets  = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                    foreach (var obj in subAssets)
                    {
                        if (obj is Sprite sprite && sprite.name == spriteName)
                        {
                            return sprite;
                        }
                    }

                    Debug.LogError($"找不到SubAsset Sprite: {se}");
                    return null;
                });
                RegisterSerializeHandle(dataType, val =>
                {
                    if (val is Sprite sprite)
                    {
                        if (AssetDatabase.IsMainAsset(sprite)) return AssetDatabase.GetAssetPath(sprite);
                        return AssetDatabase.GetAssetPath(sprite) + "_$_" + sprite.name;
                    }

                    return AssetDatabase.GetAssetPath((Object)val);
                });
            }

            var refTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ExcelEntityRef<>));
            foreach (var refType in refTypes)
            {
                _type2RefTypeDic[refType.BaseType.GenericTypeArguments[0]] = refType;
            }

            var dataViewTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ExcelDataView<,>));
            foreach (var dataViewType in dataViewTypes)
            {
                _type2DataViewTypeDic[dataViewType.BaseType.GenericTypeArguments[1]] = dataViewType;
            }

            _inited = true;
        }

        public static void ReadDataByExcelEntitySOType(Type targetSOType)
        {
            var refData = ResUtil.GetAsset<ExcelRefData>();
            if (refData == null)
            {
                Debug.LogError("没有生成Excel引用文件，请先点击·生成csv文件和代码数据类·");
            }

            var excelEntityTypes         = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            var targetType               = excelEntityTypes.Find(type => targetSOType.Name == type.Name + "SO");
            var excelEntityDataViewTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ExcelDataView<,>));
            var targetDataViewType =
                excelEntityDataViewTypes.Find(type => type.Name == targetType.Name + "DataView");
            var dataRef = refData.ExcelDataRefs.Find(df => df.Name == targetType.Name);
            if (string.IsNullOrEmpty(dataRef.Name))
            {
                Debug.LogError("没有找到对应的数据引用");
            }
            else
            {
                var list    = ReadData(dataRef.TextAsset, targetType);
                var refList = CreateViewRef(dataRef.TextAsset, targetType);
                MiddleWare(list, targetType);
                var data     = GetOrCreateExcelSOData(targetSOType);
                var dataView = GetOrCreateExcelDataView(targetDataViewType);
                targetSOType.GetField("RawDataList").SetValue(data, list);
                targetDataViewType.GetField("DataViews").SetValue(dataView, refList);
                EditorUtility.SetDirty(data);
            }
        }

        private static void MiddleWare(IEnumerable list, Type targetType)
        {
            var middleWareTypes = TypeUtil.GetDerivedClassesFromGenericInterfaces(typeof(IExcelMiddleware<>));
            foreach (var middleWareType in middleWareTypes)
            {
                if (middleWareType!.GetInterfaces().First().GetGenericArguments()[0] == targetType)
                {
                    var inst = Activator.CreateInstance(middleWareType);
                    foreach (var o in list)
                    {
                        middleWareType!.GetInterfaces().First().GetMethod("Process")!.Invoke(inst, new[] { o });
                    }

                    return;
                }
            }
        }

        public static void ReadDataByExcelEntityType(Type targetType)
        {
            var refData = ResUtil.GetAsset<ExcelRefData>();
            if (refData == null)
            {
                Debug.LogError("没有生成Excel引用文件，请先点击·生成csv文件和代码数据类·");
            }

            var excelEntitySOTypes       = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            var targetSOType             = excelEntitySOTypes.Find(soType => soType.Name == targetType.Name + "SO");
            var dataRef                  = refData.ExcelDataRefs.Find(df => df.Name == targetType.Name);
            var excelEntityDataViewTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ExcelDataView<,>));
            var targetDataViewType =
                excelEntityDataViewTypes.Find(type => type.Name == targetType.Name + "DataView");
            if (string.IsNullOrEmpty(dataRef.Name))
            {
                Debug.LogError("没有找到对应的数据引用");
            }
            else
            {
                var list    = ReadData(dataRef.TextAsset, targetType);
                var refList = CreateViewRef(dataRef.TextAsset, targetType);
                MiddleWare(list, targetType);
                var data     = GetOrCreateExcelSOData(targetSOType);
                var dataView = GetOrCreateExcelDataView(targetDataViewType);
                targetSOType.GetField("RawDataList").SetValue(data, list);
                targetDataViewType.GetField("DataViews").SetValue(dataView, refList);
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

            var excelEntityTypes         = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            var excelEntitySOTypes       = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            var excelEntityDataViewTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ExcelDataView<,>));
            foreach (var targetType in excelEntityTypes)
            {
                var targetSOType = excelEntitySOTypes.Find(type => type.Name == targetType.Name + "SO");
                var targetDataViewType =
                    excelEntityDataViewTypes.Find(type => type.Name == targetType.Name + "DataView");
                var dataRef = refData.ExcelDataRefs.Find(df => df.Name == targetType.Name);
                if (string.IsNullOrEmpty(dataRef.Name))
                {
                    Debug.LogError("没有找到对应的数据引用");
                }
                else
                {
                    var list    = ReadData(dataRef.TextAsset, targetType);
                    var refList = CreateViewRef(dataRef.TextAsset, targetType);
                    MiddleWare(list, targetType);
                    var data     = GetOrCreateExcelSOData(targetSOType);
                    var dataView = GetOrCreateExcelDataView(targetDataViewType);
                    targetSOType.GetField("RawDataList").SetValue(data, list);
                    targetDataViewType.GetField("DataViews").SetValue(dataView, refList);
                    EditorUtility.SetDirty(data);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Object GetOrCreateExcelSOData(Type type)
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            var path    = $"{setting.DataPathRoot}/{type.Name}.asset";
            var data    = ResUtil.GetOrCreateAsset(path, type);
            ResUtil.AddAssetToGroup("ExcelEntitySO", path, true, true);
            return data;
        }

        private static Object GetOrCreateExcelDataView(Type type)
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            DirectoryUtil.MakeSureDirectory($"{setting.DataPathRoot}/DataView");
            var path = $"{setting.DataPathRoot}/DataView/{type.Name}.asset";
            var data = ResUtil.GetOrCreateAsset(path, type);
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

        public static IEnumerable CreateViewRef(TextAsset textAsset, Type type)
        {
            var text      = textAsset.text;
            var refList   = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(_type2RefTypeDic[type]));
            var dataLines = text.Split("\n");
            var dataCount = 0;
            foreach (var t in dataLines)
            {
                var dataLine = t.Trim();
                if (string.IsNullOrEmpty(dataLine)) continue;
                var datas                     = ParseDataFromLine(dataLine);
                if (dataCount == 0) dataCount = datas.Count;
                if (dataCount != 0 && datas.Count < dataCount)
                {
                    continue;
                }

                if (datas.Any(se => se.StartsWith(TABLE_HEAD_SIGN))) continue;
                var refEntity = Activator.CreateInstance(_type2RefTypeDic[type]);
                _type2RefTypeDic[type].GetField("RawData").SetValue(refEntity, datas);
                refList.Add(refEntity);
            }

            return refList;
        }

        private static List<List<string>> ReadData(string text)
        {
            var dataLines = text.Split("\n");
            var dataCount = 0;
            var ret       = new List<List<string>>();
            foreach (var t in dataLines)
            {
                var dataLine = t.Trim();
                if (string.IsNullOrEmpty(dataLine)) continue;
                var datas                     = ParseDataFromLine(dataLine);
                if (dataCount == 0) dataCount = datas.Count;
                if (dataCount != 0 && datas.Count < dataCount)
                {
                    continue;
                }

                if (datas.Any(se => se.StartsWith(TABLE_HEAD_SIGN))) continue;
                ret.Add(datas);
            }

            return ret;
        }

        private static IEnumerable ReadData(TextAsset textAsset, Type type)
        {
            var text       = textAsset.text;
            var entityList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            var dataLines  = text.Split("\n");
            var dataCount  = 0;
            foreach (var t in dataLines)
            {
                var dataLine = t.Trim();
                if (string.IsNullOrEmpty(dataLine)) continue;
                var datas                     = ParseDataFromLine(dataLine);
                if (dataCount == 0) dataCount = datas.Count;
                if (dataCount != 0 && datas.Count < dataCount)
                {
                    continue;
                }

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

        internal static List<List<string>> LoadFromCustomFile(string filePath)
        {
            var text = File.ReadAllText(filePath);
            return ReadData(text);
        }

        internal static List<List<string>> PickDataByIndex(List<List<string>> rawDataBefore,
            Dictionary<int, int> pickIndexDic, int count)
        {
            var ret = new List<List<string>>();
            foreach (var list in rawDataBefore)
            {
                var newList = Enumerable.Repeat(string.Empty, count).ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    if (pickIndexDic.TryGetValue(i, out var pickIndex))
                    {
                        newList[pickIndex] = list[i];
                    }
                }

                ret.Add(newList);
            }

            return ret;
        }

        internal static void WriteTableData(ExcelTableConfig config, List<List<string>> rawDataAfter)
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            var path    = $"{setting.ExcelDataPathRoot}/{config.ConfigName}/{config.ConfigName}.csv";
            var builder = new StringBuilder();
            var header  = GetTableHeader(config);
            builder.Append(header);
            foreach (var data in rawDataAfter)
            {
                foreach (var se in data)
                {
                    builder.Append(se);
                    builder.Append(',');
                }

                builder.Append('\n');
            }

            File.WriteAllText(path, builder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }

        internal static Dictionary<int, int> DiffWithColDic(Dictionary<string, int> reload, Dictionary<string, int> now,
            ExcelStructRenameConfig config)
        {
            var result = new Dictionary<int, int>();
            foreach (var (originalKey, reloadValue) in reload)
            {
                var modifiedKey = originalKey;

                if (originalKey.Contains("->"))
                {
                    var structParamSplit = originalKey.Split(new[] { "->" }, StringSplitOptions.None);
                    if (structParamSplit.Length != 2)
                        continue;

                    var structPart = structParamSplit[0].Trim();
                    var paramPart  = structParamSplit[1].Trim();

                    var nameTypeSplit = structPart.Split('_');
                    if (nameTypeSplit.Length < 2)
                        continue;

                    var structName = nameTypeSplit[0];
                    var structType = string.Join("_", nameTypeSplit.Skip(1));
                    
                    if (!string.IsNullOrEmpty(config.OriginName) && structType == config.OriginName)
                    {
                        var newStructPart = $"{structName}_{config.NewName}";
                        modifiedKey = $"{newStructPart}->{paramPart}";
                    }
                }

                if (now.TryGetValue(modifiedKey, out var nowValue))
                {
                    result[reloadValue] = nowValue;
                }
            }

            return result;
        }
    }
}

#endif