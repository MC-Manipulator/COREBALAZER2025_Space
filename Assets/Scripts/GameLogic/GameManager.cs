using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{
    public GameObject menuPanel;

    public void OpenMenuPanel()
    {
        menuPanel.SetActive(true);
    }

    public void CloseMenuPanel()
    {
        menuPanel.SetActive(false);
    }

    public void BackMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
