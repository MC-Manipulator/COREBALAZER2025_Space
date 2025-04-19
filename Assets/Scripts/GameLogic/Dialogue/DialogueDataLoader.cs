using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;
using Test;
using System.Linq;
using EggFramework.Generator;
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

            if (obj != null && obj.name != "ExcelRefData" && obj.name != "TestExcelDataView")
            {
                if (listeners.Contains(obj))
                {
                    Debug.Log("�Ѿ�����" + obj.name);
                    continue;
                }
                else listeners.Add(obj);
                CreateAndSaveScriptableObject(ExtractAllTest1(obj), obj.name);
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
        // ��������Ҫ�����·��
        // ע�⣺·��Ӧ�������Resources�ļ���
        string resourcesSubPath = "ScriptableObject/Dialogue/DialogueData"; // Resources�µ����ļ���
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
        foreach (Test1 text in texts)
        {
            //Debug.Log(text.text);
            NarrationCharacter narrationCharacter = characterDic[text.m_name];
            Sprite illustration;
            if (narrationCharacter.IllustrationOfCharacter.ContainsKey(text.Illustration))
            {
                illustration = narrationCharacter.IllustrationOfCharacter[text.Illustration];
            }
            else
            {
                illustration = narrationCharacter.IllustrationOfCharacter.First().Value;
            }

            if (text.type == "Choice")
            {
                ChoiceDialogueNode dialogueNode = ScriptableObject.CreateInstance<ChoiceDialogueNode>();
                dialogueNode = new ChoiceDialogueNode(text.text, narrationCharacter, illustration);
                Debug.Log(text.text);
                dialogueNode.choices = new DialogueChoice[text.nextNodeIndex.Count];
                for (int i = 0; i < text.nextNodeIndex.Count; i++)
                {
                    dialogueNode.choices[i] = new DialogueChoice(text.choicePreview[i], text.nextNodeIndex[i]);
                }
                //Debug.Log(text.text);
                dialogueNode.name = dialogueNode.m_text;
                dialogue.nodes.Add(dialogueNode);
                AssetDatabase.AddObjectToAsset(dialogueNode, dialogue);
            }
            else if (text.type == "Branch")
            {
                BranchDialogueNode dialogueNode = ScriptableObject.CreateInstance<BranchDialogueNode>();
                dialogueNode = new BranchDialogueNode(text.text, narrationCharacter, illustration);
                dialogueNode.branchs = new DialogueBranch[text.nextNodeIndex.Count];
                for (int i = 0; i < text.nextNodeIndex.Count; i++)
                {
                    dialogueNode.branchs[i] = new DialogueBranch(text.nextNodeIndex[i]);
                }
                //Debug.Log(text.text);
                dialogueNode.name = dialogueNode.m_text;
                dialogue.nodes.Add(dialogueNode);
                AssetDatabase.AddObjectToAsset(dialogueNode, dialogue);
            }
            else
            {
                Debug.Log("111");
                BasicDialogueNode dialogueNode = ScriptableObject.CreateInstance<BasicDialogueNode>();
                dialogueNode = new BasicDialogueNode(text.text, narrationCharacter, illustration);
                if(text.nextNodeIndex != null)
                {
                    Debug.Log("nextNodeIndex��Ϊ��");
                    if (text.nextNodeIndex.Count > 0)
                    {
                        Debug.Log("nextNodeIndex���Ȳ�Ϊ0");
                        if (text.nextNodeIndex[0] == 0)
                        {
                            dialogueNode.nextNodeIndex = dialogue.nodes.Count + 1;
                            Debug.Log(dialogue.nodes.Count + 1);
                        }
                        else
                        {
                            dialogueNode.nextNodeIndex = text.nextNodeIndex[0];
                        }
                    }
                    else
                    {
                        dialogueNode.nextNodeIndex = dialogue.nodes.Count + 1;
                    }
                }

                else
                {
                    dialogueNode.nextNodeIndex = dialogue.nodes.Count + 1;
                    Debug.Log(dialogue.nodes.Count + 1);
                }
                dialogueNode.name = dialogueNode.m_text;
                dialogue.nodes.Add(dialogueNode);
                AssetDatabase.AddObjectToAsset(dialogueNode, dialogue);
            }
        }
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
