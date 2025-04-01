using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;
using Test;

/// <summary>
/// Excelģ��ͶԻ�ģ����Žӣ����𽫴洢Excel���ݵ�ScriptObject��һ��ת��Ϊ�Ի�ʹ�õ�SO����
/// </summary>
public class DialogueDataLoader : SerializedMonoBehaviour
{
    public Dictionary<string,NarrationCharacter> characterDic = new Dictionary<string,NarrationCharacter>();
    private static string targetFolderPath = "Assets/Excel/SOData";

    [SerializeField] 
    [ListDrawerSettings(ShowFoldout = true)]
    private List<ScriptableObject> listeners = new List<ScriptableObject>();

    [Button("�ռ� ScriptableObjects")]
    [DisableInPlayMode] // ���� Edit ģʽ�¿���
    public void GatherScriptableObjects()
    {
#if UNITY_EDITOR // ȷ��ֻ�ڱ༭����ִ��
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("�˹���ֻ���� Edit ģʽ��ʹ�ã�");
            return;
        }

        listeners.Clear();

        Debug.Log($"��ʼ���ļ��� {targetFolderPath} �ռ� ScriptableObjects...");

        // ��ȡ���� ScriptableObject
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { targetFolderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (obj != null && obj.name != "ExcelRefData")
            {
                CreateAndSaveScriptableObject(ExtractAllTest1(obj),obj.name);
                listeners.Add(obj);
                Debug.Log($"�ҵ������ ScriptableObject: {path}");
            }
        }

        Debug.Log($"ScriptableObjects �ռ���ɣ����ҵ� {listeners.Count} ������");

        // ��Ƕ���Ϊ���Ա㱣��
        EditorUtility.SetDirty(this);
#endif
    }
    public void CreateAndSaveScriptableObject(List<Test1> texts,string _assetName)
    {
#if UNITY_EDITOR // ȷ��ֻ�ڱ༭����ִ��
        // ����ScriptableObjectʵ��
        DialogueSO dialogue = ScriptableObject.CreateInstance<DialogueSO>();

        foreach(Test1 text in texts)
        {
            dialogue.nodes.Add(new BasicDialogueNode(text.text, characterDic[text.m_name]));
        }
        // ��������Ҫ�����·��
        // ע�⣺·��Ӧ�������Resources�ļ���
        string resourcesSubPath = "ScriptObject/DialogueData"; // Resources�µ����ļ���
        string assetName = _assetName + ".asset";

        // ȷ��·������
        string fullResourcesPath = Application.dataPath + "/Resources/" + resourcesSubPath;
        if (!System.IO.Directory.Exists(fullResourcesPath))
        {
            System.IO.Directory.CreateDirectory(fullResourcesPath);
        }

        // ����asset
        string assetPath = "Assets/Resources/" + resourcesSubPath + "/" + assetName;
        AssetDatabase.CreateAsset(dialogue, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // ѡ���´����Ķ���
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = dialogue;
    }
#endif
    public List<Test1> ExtractAllTest1(ScriptableObject so)
    {
        List<Test1> result = new List<Test1>();

        FieldInfo rawDataListField = so.GetType().GetField("RawDataList", BindingFlags.Public | BindingFlags.Instance);
        if (rawDataListField == null)
        {
            Debug.LogError($"ScriptableObject {so.GetType().Name} does not contain RawDataList field");
            return result;
        }

        // ��ȡRawDataList��ֵ
        object rawDataList = rawDataListField.GetValue(so);
        if (rawDataList is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                // ��ȡitem�������ֶ�
                FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    // ����ֶ������Ƿ���Test1
                    if (field.FieldType == typeof(Test1))
                    {
                        Test1 test1Value = (Test1)field.GetValue(item);
                        if (test1Value.text != null)
                        {
                            result.Add(test1Value);
                        }
                    }
                }
            }
        }

        return result;
    }
}
