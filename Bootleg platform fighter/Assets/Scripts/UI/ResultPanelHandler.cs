using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BootlegPlatformFighter
{
    public class ResultPanelHandler : MonoBehaviour
    {
        [SerializeField]
        private bool _open;
        [SerializeField]
        private float _animationTime = 0.5f;
        [SerializeField]
        private float minHeight = 287f;
        [SerializeField]
        private float maxHeight = 535f;


        public bool IsOpen => _open;
        public void OnClick()
        {
            if (!_open)
            {
                MaximizeCard();
                // Loops through all existing cards and minimize the ones that are open, except for this.
                var cards = transform.parent.GetComponentsInChildren<ResultPanelHandler>();
                for (int i = 0; i < cards.Length; i++)
                {
                    var card = cards[i];
                    if (card == this)
                        continue;
                    if (card.IsOpen) {
                        card.MinimizeCard();
                    }
                }
            }
            else
            {
                MinimizeCard();
            }
        }

        public void MinimizeCard()
        {
            var defX = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(defX, minHeight), _animationTime);
            _open = false;
        }

        public void MaximizeCard()
        {
            var defX = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(defX, maxHeight), _animationTime);
            _open = true;
        }
    }
}
