using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public AudioSettingBoard audioSettingBoard;
    public SaveBoard saveBoard;
    public GameObject blackMask;

    private void Start()
    {
        AudioManager.Instance.PlayMusic("TestBGM");
    }

    public void StartGame()
    {
        AudioManager.Instance.PlaySound("TestSFX");
        SceneManager.LoadScene(1);
        Debug.Log("StartGame");
    }

    public void OpenSaveBoard()
    {
        saveBoard.gameObject.SetActive(true);
    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame");

    }

    public void OpenAudioSettingBoard()
    {
        blackMask.SetActive(true);
        audioSettingBoard.Open();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("ExitGame");
    }
}
