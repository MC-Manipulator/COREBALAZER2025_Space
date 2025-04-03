using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public ItemType itemName;
    public delegate void SelectedItemButtonAction(ItemType itemType);
    public SelectedItemButtonAction OnItemButtonSelected;



    public void SelectItem()
    {
        OnItemButtonSelected.Invoke(itemName);
    }
}