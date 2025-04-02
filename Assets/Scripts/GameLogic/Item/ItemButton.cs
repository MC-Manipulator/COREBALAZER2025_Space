using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class ItemButton : MonoBehaviour
    {
        public ItemType id;
        public delegate void SelectItemAction(ItemType id);
        public event SelectItemAction OnItemSelected;

        public void SelectItem()
        {
            //BackpackSystem.instance.SelectItem(id);
            OnItemSelected?.Invoke(id);
        }
    }
}