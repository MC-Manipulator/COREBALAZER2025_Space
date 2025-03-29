using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class RoomExit : MonoBehaviour
    {
        public GameObject entrance;

        public void EnterOtherRoom()
        {
            if (entrance != null)
                PlayerControl.player.transform.position = entrance.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            EnterOtherRoom();
        }
    }
}