using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Dialogue/PropertiesData")]
public class PropertiesData : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "属性名", ValueLabel = "属性值")]
    public List<Stat> customProperties = new List<Stat>();

    [Button("添加Bool属性")]
    private void AddBoolProperty(string _name, bool defaultValue = false)
    {
        BoolStat boolStat = ScriptableObject.CreateInstance<BoolStat>();
        boolStat = new BoolStat();
        boolStat.name = _name;
        AssetDatabase.AddObjectToAsset(boolStat, this);
        customProperties.Add(boolStat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button("添加Int属性")]
    private void AddIntProperty(string _name, int defaultValue = 0)
    {
        IntStat intStat = ScriptableObject.CreateInstance<IntStat>();
        intStat.name = _name;
        AssetDatabase.AddObjectToAsset(intStat, this);
        customProperties.Add(intStat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button("添加Float属性")]
    private void AddFloatProperty(string _name, float defaultValue = 0f)
    {
        FloatStat floattat = ScriptableObject.CreateInstance<FloatStat>();
        floattat.name = _name;
        AssetDatabase.AddObjectToAsset(floattat, this);
        customProperties.Add(floattat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [Button("删除属性"), HorizontalGroup("Actions")]
    public void RemoveStat(Stat statToRemove)
    {
        if (statToRemove == null || !customProperties.Contains(statToRemove))
            return;
        customProperties.Remove(statToRemove);
        Object.DestroyImmediate(statToRemove, true); 
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this); 
    }

    [Button("删除所有属性", ButtonHeight = 30)]
    [GUIColor(1, 0.4f, 0.4f)] // 红色按钮警示色
    public void RemoveAllStats()
    {
        // 安全确认对话框
        if (!EditorUtility.DisplayDialog(
            "危险操作！",
            "确定要删除所有属性吗？此操作不可撤销！",
            "全部删除",
            "取消"))
        {
            return;
        }

        // 创建临时列表避免修改时遍历错误
        List<Stat> statsToRemove = new List<Stat>(customProperties);

        foreach (Stat stat in statsToRemove)
        {
            RemoveStat(stat);
        }

        Debug.Log($"已删除所有属性，共 {statsToRemove.Count} 个");
    }

}
