using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class AddItem : MonoBehaviour
    {
        public int id = 0;
        public string itemName;
        public string itemDescription;
        public Sprite sprite;

        public void AddItemToBackpack()
        {
            BackpackSystem.instance.AddItem(id, itemName, itemDescription, sprite);
        }
    }
}