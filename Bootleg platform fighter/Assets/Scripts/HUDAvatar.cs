using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace BootlegPlatformFighter
{
    public class HUDAvatar : MonoBehaviour
    {
        [SerializeField]
        private int _CharacterIndex = -1; // CharacterIndex it's displaying data for

        private GameObject _lifePanel;
        private TextMeshProUGUI HealthText { get; set; }
        private TextMeshProUGUI NameText { get; set; }
        private void Awake()
        {
            DOTween.Init();
            DOTween.defaultEaseType = Ease.Linear;
            NameText = transform.Find("NamePanel").Find("NameText").GetComponent<TextMeshProUGUI>();
            HealthText = transform.Find("DataPanel").Find("HealthText").GetComponent<TextMeshProUGUI>();
            _lifePanel = transform.Find("DataPanel").Find("LifePanel").gameObject;
        }
        public void setCharacterIndex(int idx)
        {
            _CharacterIndex = idx;
        }
        public int getCharacterIndex()
        {
            return _CharacterIndex;
        }
        public int getHealthCount()
        {
            return _lifePanel.transform.childCount;
        }
        public void RemoveOneHeart()
        {
            if (getHealthCount() > 0) {
                var badheart = _lifePanel.transform.GetChild(getHealthCount() - 1);
                badheart.DOScale(0f, 1f).onComplete = () => Destroy(badheart.gameObject);
            }
        }
        public void SetName(string text)
        {
            NameText.text = text;
        }
        public void SetHealth(float percentage)
        {
            HealthText.text = percentage + "%";
        }
    }
}
