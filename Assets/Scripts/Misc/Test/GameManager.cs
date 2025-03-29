using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
