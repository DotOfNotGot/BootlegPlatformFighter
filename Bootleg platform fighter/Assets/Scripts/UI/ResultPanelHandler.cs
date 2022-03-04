using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BootlegPlatformFighter
{
    public class ResultPanelHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        public void OnClick()
        {
            var defX = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(defX, 535f), 3f);
        }
    }
}
