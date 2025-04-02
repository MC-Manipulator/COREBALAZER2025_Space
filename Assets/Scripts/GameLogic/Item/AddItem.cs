using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class AddItem : MonoBehaviour
    {
        public ItemSO item;

        public void AddItemToBackpack()
        {
            //BackpackSystem.instance.AddItem(item);
            BackpackMgr.GetInstance().AddItem(item);
        }
    }
}