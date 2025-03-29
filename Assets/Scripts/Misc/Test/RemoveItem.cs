using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class RemoveItem : MonoBehaviour
    {
        public int id = 0;

        public void RemoveItemToBackpack()
        {
            BackpackSystem.instance.RemoveItem(id);
        }
    }

}