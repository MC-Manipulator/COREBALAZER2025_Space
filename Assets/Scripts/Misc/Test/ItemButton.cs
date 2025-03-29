using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class ItemButton : MonoBehaviour
    {
        public int id;

        public void SelectItem()
        {
            BackpackSystem.instance.SelectItem(id);
        }
    }
}