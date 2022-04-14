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
            if (GameManagerData.Players.Count - 1 >= idx)
                SetName(GameManagerData.Players[idx].name);
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
            if (getHealthCount() > 0)
            {
                var badheart = _lifePanel.transform.GetChild(getHealthCount() - 1);
                // Scale to twice size then back to 0
                badheart.DOScale(2f, 0.3f).SetEase(Ease.InOutBack).onComplete = () =>
                    badheart.DOScale(0f, 0.5f).onComplete = () =>
                        Destroy(badheart.gameObject);
            }
        }
        public void SetName(string text)
        {
            NameText.text = text;
            GameManagerData.Players[_CharacterIndex].name = text;
        }
        public void SetHealth(float percentage)
        {
            // health animation when changed
            if (percentage != 0f)
            {
                var healthTransform = HealthText.GetComponentInParent<Transform>();
                healthTransform.DOScale(1.5f, 0.2f).onComplete = () =>
                    healthTransform.DOScale(1f, 0.3f);
            }
            GameManagerData.Players[_CharacterIndex].damageTaken = percentage;
            HealthText.text = percentage + "%";
        }
    }
}
