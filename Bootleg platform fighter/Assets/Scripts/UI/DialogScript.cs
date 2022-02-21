using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BootlegPlatformFighter
{
    public class DialogScript : MonoBehaviour
    {
        // CANNOT BE USED DYNAMICALLY.. YET

        // https://forum.unity.com/threads/how-to-select-a-callback-function-for-your-script-from-the-editor.295550/
        // Place callback here
        [SerializeField]
        EventTrigger.TriggerEvent callback;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Yes()
        {
            callback.Invoke(new BaseEventData(EventSystem.current));
        }

        public void No()
        {

        }
    }
}
