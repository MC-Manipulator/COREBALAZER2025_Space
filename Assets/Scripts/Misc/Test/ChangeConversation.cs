using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Test
{
    public class ChangeConversation : MonoBehaviour
    {
        public GameObject target;
        public DialogueSystemTrigger trigger;
        public DialogueSystemController controller;
        public string changeConversation;

        public void ChangeConversationTest()
        {
            Debug.Log("ChangeConversationTest");

            if (target == null)
            {
                Debug.Log("target is null");
                return;
            }

            if (!target.TryGetComponent<DialogueSystemTrigger>(out trigger))
            {
                Debug.Log("trigger is null");
                return;
            }

            trigger.conversation = changeConversation;
        }
    }
}