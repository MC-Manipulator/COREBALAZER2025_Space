using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class Item
    {
        public int id;
        public Sprite image;
        public string itemName;
        public string itemDescription;

        public string GetName()
        {
            return itemName;
        }

        public string GetDescription()
        {
            return itemDescription;

        }
    }
}