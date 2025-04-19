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
/// Excel模块和对话模块的桥接，负责将存储Excel数据的ScriptObject进一步转化为对话使用的SO数据
/// </summary>
public class DialogueDataLoader : SerializedMonoBehaviour
{
    public Dictionary<string,NarrationCharacter> characterDic = new Dictionary<string,NarrationCharacter>();
    private static string targetFolderPath = "Assets/Excel/SOData";

    [SerializeField] 
    [ListDrawerSettings(ShowFoldout = true)]
    private List<ScriptableObject> listeners = new List<ScriptableObject>();

    [Button("收集 ScriptableObjects")]
    [DisableInPlayMode] // 仅在 Edit 模式下可用
    public void GatherScriptableObjects()
    {
#if UNITY_EDITOR // 确保只在编辑器下执行
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("此功能只能在 Edit 模式下使用！");
            return;
        }

        listeners.Clear();

        Debug.Log($"开始从文件夹 {targetFolderPath} 收集 ScriptableObjects...");

        // 获取所有 ScriptableObject
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { targetFolderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (obj != null && obj.name != "ExcelRefData" && obj.name != "TestExcelDataView")
            {
                if (listeners.Contains(obj))
                {
                    Debug.Log("已经包含" + obj.name);
                    continue;
                }
                else listeners.Add(obj);
                CreateAndSaveScriptableObject(ExtractAllTest1(obj), obj.name);
                Debug.Log($"找到并添加 ScriptableObject: {path}");
            }
        }

        Debug.Log($"ScriptableObjects 收集完成，共找到 {listeners.Count} 个对象");

        // 标记对象为脏以便保存
        EditorUtility.SetDirty(this);
#endif
    }
    public void CreateAndSaveScriptableObject(List<Test1> texts,string _assetName)
    {
#if UNITY_EDITOR // 确保只在编辑器下执行
        // 创建ScriptableObject实例
        DialogueSO dialogue = ScriptableObject.CreateInstance<DialogueSO>();
        // 设置你想要保存的路径
        // 注意：路径应该相对于Resources文件夹
        string resourcesSubPath = "ScriptableObject/Dialogue/DialogueData"; // Resources下的子文件夹
        string assetName = _assetName + ".asset";

        // 确保路径存在
        string fullResourcesPath = Application.dataPath + "/Resources/" + resourcesSubPath;
        if (!System.IO.Directory.Exists(fullResourcesPath))
        {
            System.IO.Directory.CreateDirectory(fullResourcesPath);
        }

        // 保存asset
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
                    Debug.Log("nextNodeIndex不为空");
                    if (text.nextNodeIndex.Count > 0)
                    {
                        Debug.Log("nextNodeIndex长度不为0");
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

        // 选中新创建的对象
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

        // 获取RawDataList的值
        object rawDataList = rawDataListField.GetValue(so);
        if (rawDataList is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                // 获取item的所有字段
                FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    // 检查字段类型是否是Test1
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
