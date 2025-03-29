using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueSystem : MonoBehaviour
{
    public static ClueSystem Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject cluePanel;
    [SerializeField] private Transform clueContent;
    [SerializeField] private ClueEntry clueEntryPrefab;
    [SerializeField] private ClueDetailPanel detailPanel;

    [Header("Clue Data")]
    [SerializeField] private ClueData clueDatabase;

    private Dictionary<string, Clue> obtainedClues = new Dictionary<string, Clue>();
    public event Action OnClueUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        AVGSaveSystem.instance.onLoad.AddListener(OnLoad);
        AVGSaveSystem.instance.onSave.AddListener(OnSave);
    }
    // 添加新线索
    public void AddClue(string clueID)
    {
        if (clueDatabase.TryGetClue(clueID, out Clue newClue))
        {
            if (!obtainedClues.ContainsKey(clueID))
            {
                Debug.Log("clueID");
                obtainedClues.Add(clueID, newClue);
                UpdateClueUI();
                OnClueUpdated?.Invoke();
            }
        }
    }

    // 更新UI显示
    private void UpdateClueUI()
    {
        // 清空现有条目
        foreach (Transform child in clueContent)
        {
            Destroy(child.gameObject);
        }

        // 生成新条目
        foreach (var clue in obtainedClues.Values)
        {
            ClueEntry entry = Instantiate(clueEntryPrefab, clueContent);
            entry.Initialize(clue, () => ShowDetail(clue));
        }
    }

    // 显示详细信息
    private void ShowDetail(Clue clue)
    {
        detailPanel.Show(clue);
    }

    // 保存/加载
    /*
    private void SaveClues()
    {
        List<string> clueIDs = new List<string>(obtainedClues.Keys);
        PlayerPrefs.SetString("ObtainedClues", string.Join(",", clueIDs));
    }

    private void LoadClues()
    {
        string savedClues = PlayerPrefs.GetString("ObtainedClues");
        if (!string.IsNullOrEmpty(savedClues))
        {
            string[] clueIDs = savedClues.Split(',');
            foreach (string id in clueIDs)
            {
                AddClue(id);
            }
        }
    }*/


    public void OnSave()
    {
        string[] clueIDList = new string[obtainedClues.Count];
        int it = 0;
        foreach (Clue i in obtainedClues.Values)
        {
            clueIDList[it++] = i.clueID;
        }
        ES3.Save("Clue", clueIDList);
    }

    public void OnLoad()
    {
        ClearClues();

        string[] clueIDList = ES3.Load("Clue", new string[0]);
        foreach (string i in clueIDList)
        {
            AddClue(i);
        }
    }

    private void ClearClues()
    {
        obtainedClues.Clear();
        PlayerPrefs.SetString("ObtainedClues", "");
        UpdateClueUI();
    }

    // UI控制
    public void ToggleCluePanel()
    {
        cluePanel.SetActive(!cluePanel.activeSelf);
        if (cluePanel.activeSelf) UpdateClueUI();
    }
}
