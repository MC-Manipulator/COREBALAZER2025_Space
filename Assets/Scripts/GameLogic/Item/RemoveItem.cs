using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class RemoveItem : MonoBehaviour
    {
        public ItemType id;

        public void RemoveItemToBackpack()
        {
            //BackpackSystem.instance.RemoveItem(id);
            BackpackMgr.GetInstance().RemoveItem(id);
        }
    }

}