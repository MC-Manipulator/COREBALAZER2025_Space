using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageButton : MonoBehaviour
{
    public int pageNumber;
    public SaveBoard sb;
    public SpriteRenderer sr;

    private void OnEnable()
    {
        this.GetComponent<Button>().onClick.AddListener(ChangePage);
    }

    private void OnDisable()
    {
        this.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void ChangePage()
    {
        sb.ChangePage(pageNumber);
    }

    public void Selected()
    {
        sr.color = Color.green;
    }

    public void Deselected()
    {

        sr.color = Color.white;
    }
}
