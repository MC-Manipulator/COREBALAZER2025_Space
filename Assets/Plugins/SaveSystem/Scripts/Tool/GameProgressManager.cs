using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager instance;

    public int day = 0;

    public float gameTime = 0f;
    public float hour = 0f;
    public float minute = 0f;

    public TMP_Text dayText;
    public TMP_Text gameTimeText;


    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
        else
            Destroy(gameObject);
        AVGSaveSystem.instance.onSave.AddListener(OnSave);
        AVGSaveSystem.instance.onLoad.AddListener(OnLoad);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;

        minute = (int)(gameTime / 60f);
        hour = (int)(gameTime / 3600f);

        gameTimeText.text = gameTime.ToString() + "s";
    }

    public void NextDay()
    {
        day++;
        SetDay();
    }

    public void SetDay()
    {
        dayText.text = "Day " + day.ToString();
    }

    public void OnSave()
    {
        ES3.Save("day", day);
        ES3.Save("gameTime", gameTime);
    }

    public void OnLoad()
    {
        day = ES3.Load<int>("day", 0);
        gameTime = ES3.Load<float>("gameTime", 0f);
        SetDay();
    }
}
