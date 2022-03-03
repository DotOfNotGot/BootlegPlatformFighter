using System.Collections;
using System.
    Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class ResultPanelHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject ExpandStatsObject;
        // Start is called before the first frame update
        void Start()
        {
            ExpandStatsObject.transform.localScale = new Vector3(1f, 0f);
        }

        public void OnClick()
        {
            ExpandStatsObject.LeanScale(Vector3.one, 3f);
        }
    }
}
