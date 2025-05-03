using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    public DialogueSO Dialogue;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EventCenter.GetInstance().EventTrigger("∂‘ª∞«Î«Û", Dialogue);
        }
    }
}
