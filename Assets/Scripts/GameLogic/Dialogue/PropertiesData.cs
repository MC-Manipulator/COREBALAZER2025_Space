using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Dialogue/PropertiesData")]
public class PropertiesData : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "������", ValueLabel = "����ֵ")]
    public List<Stat> customProperties = new List<Stat>();

    [Button("���Bool����")]
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

    [Button("���Int����")]
    private void AddIntProperty(string _name, int defaultValue = 0)
    {
        IntStat intStat = ScriptableObject.CreateInstance<IntStat>();
        intStat.name = _name;
        AssetDatabase.AddObjectToAsset(intStat, this);
        customProperties.Add(intStat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button("���Float����")]
    private void AddFloatProperty(string _name, float defaultValue = 0f)
    {
        FloatStat floattat = ScriptableObject.CreateInstance<FloatStat>();
        floattat.name = _name;
        AssetDatabase.AddObjectToAsset(floattat, this);
        customProperties.Add(floattat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [Button("ɾ������"), HorizontalGroup("Actions")]
    public void RemoveStat(Stat statToRemove)
    {
        if (statToRemove == null || !customProperties.Contains(statToRemove))
            return;
        customProperties.Remove(statToRemove);
        Object.DestroyImmediate(statToRemove, true); 
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this); 
    }

    [Button("ɾ����������", ButtonHeight = 30)]
    [GUIColor(1, 0.4f, 0.4f)] // ��ɫ��ť��ʾɫ
    public void RemoveAllStats()
    {
        // ��ȫȷ�϶Ի���
        if (!EditorUtility.DisplayDialog(
            "Σ�ղ�����",
            "ȷ��Ҫɾ�����������𣿴˲������ɳ�����",
            "ȫ��ɾ��",
            "ȡ��"))
        {
            return;
        }

        // ������ʱ�б�����޸�ʱ��������
        List<Stat> statsToRemove = new List<Stat>(customProperties);

        foreach (Stat stat in statsToRemove)
        {
            RemoveStat(stat);
        }

        Debug.Log($"��ɾ���������ԣ��� {statsToRemove.Count} ��");
    }

}
