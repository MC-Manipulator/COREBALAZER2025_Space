using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class StatData : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "属性名", ValueLabel = "属性值")]
    public Dictionary<string, Stat> customProperties = new Dictionary<string, Stat>();

    //[Button("添加Bool属性")]
    //private void AddBoolProperty(string name, bool defaultValue = false)
    //{
    //    new BoolStat 
    //    customProperties[name] = defaultValue;
    //}

    //[Button("添加Int属性")]
    //private void AddIntProperty(string name, int defaultValue = 0)
    //{
    //    customProperties[name] = defaultValue;
    //}

    //[Button("添加Float属性")]
    //private void AddFloatProperty(string name, float defaultValue = 0f)
    //{
    //    customProperties[name] = defaultValue;
    //}
}

public class Stat { }

public class IntStat : Stat { public int value; }
public class FloatStat : Stat { public float value; }
public class BoolStat : Stat { public bool value; }
