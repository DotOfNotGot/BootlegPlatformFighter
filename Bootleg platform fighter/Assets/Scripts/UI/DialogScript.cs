using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BootlegPlatformFighter
{
    public class DialogScript : MonoBehaviour
    {
        // https://forum.unity.com/threads/how-to-select-a-callback-function-for-your-script-from-the-editor.295550/
        [SerializeField]
        private EventTrigger.TriggerEvent yesCallback;
        [SerializeField]
        private EventTrigger.TriggerEvent noCallback;

        public void Yes()
        {
            yesCallback.Invoke(new BaseEventData(EventSystem.current));
        }

        public void No()
        {
            noCallback.Invoke(new BaseEventData(EventSystem.current));
        }
    }
}
