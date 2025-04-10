#region

//文件创建者：Egg
//创建时间：09-19 10:26

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EggFramework.Util.Res;
using Sirenix.OdinInspector;

namespace EggFramework.Util
{
    public static class TypeUtil
    {
#if UNITY_EDITOR
        public static IEnumerable<string> ResTypes
        {
            get
            {
                var types = GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
                return types.Select(type => type!.BaseType!.GenericTypeArguments[0].Name);
            }
        }

        public static Type GetResTypeByTypeName(string name)
        {
            var types = GetDerivedClassesFromGenericClass(typeof(ResRefData<>))
                .Select(type => type!.BaseType!.GenericTypeArguments[0]);
            return types.FirstOrDefault(tp => tp.Name == name);
        }
#endif

        public static IEnumerable<string> UnityStructTypes =>
            new[] { "Vector2", "Vector3", "Color" };

        public static IEnumerable<string> DefaultTypes =>
            new[] { "Single", "Int32", "String", "Boolean", "Double" };

        public static IEnumerable<PropertyInfo> GetSerializePropertyInfos(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static IEnumerable<FieldInfo> GetSerializeFieldInfos(this Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        public static List<Assembly> CurrentAssemblies
        {
            get
            {
                if (!_loaded)
                {
                    LoadAssemblies();
                }

                return _currentAssemblies;
            }
        }

        private static void LoadAssemblies()
        {
            _loaded            = true;
            _currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        }

        private static bool _loaded;

        private static List<Assembly> _currentAssemblies;

        public static List<Type> GetDerivedClassesFromGenericInterfaces(Type interfaceType)
        {
            var ret = new List<Type>();
            foreach (var currentAssembly in CurrentAssemblies)
            {
                var types = currentAssembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && type.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedClassesFromGenericClass(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && !type.IsAbstract &&
                        type.BaseType is { IsGenericType: true } &&
                        targetType.IsAssignableFrom(type.BaseType.GetGenericTypeDefinition()))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedClasses(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && !type.IsAbstract && targetType.IsAssignableFrom(type))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedInterfaces(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsInterface && !type.IsAbstract && targetType.IsAssignableFrom(type))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }

        public static List<Type> GetDerivedStructs(Type targetType)
        {
            var ret = new List<Type>();
            foreach (var assembly in CurrentAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsValueType && !type.IsAbstract && targetType.IsAssignableFrom(type))
                    {
                        ret.Add(type);
                    }
                }
            }

            return ret;
        }
        
        public static string GetLabelTextTag(Type type)
        {
            var customAttributes = type.GetCustomAttributes(typeof(LabelTextAttribute), false);
            if (customAttributes.Length <= 0)
            {
                return type.Name;
            }

            return ((LabelTextAttribute)customAttributes[0]).Text;
        }

        public static IEnumerable<string> GetEnumNames(Type type)
        {
            var list = new List<string>();
            foreach (var value in Enum.GetValues(type))
            {
                list.Add(value.ToString());
            }

            return list;
        }

        public static IList CastListElements(IList list, Type targetType)
        {
            var ret = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));
            
            foreach (var o in list)
            {
                ret.Add(o);
            }

            return ret;
        }
    }
}