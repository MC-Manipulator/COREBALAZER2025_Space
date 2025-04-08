using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class StatData : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "������", ValueLabel = "����ֵ")]
    public Dictionary<string, Stat> customProperties = new Dictionary<string, Stat>();

    //[Button("���Bool����")]
    //private void AddBoolProperty(string name, bool defaultValue = false)
    //{
    //    new BoolStat 
    //    customProperties[name] = defaultValue;
    //}

    //[Button("���Int����")]
    //private void AddIntProperty(string name, int defaultValue = 0)
    //{
    //    customProperties[name] = defaultValue;
    //}

    //[Button("���Float����")]
    //private void AddFloatProperty(string name, float defaultValue = 0f)
    //{
    //    customProperties[name] = defaultValue;
    //}
}

public class Stat { }

public class IntStat : Stat { public int value; }
public class FloatStat : Stat { public float value; }
public class BoolStat : Stat { public bool value; }
