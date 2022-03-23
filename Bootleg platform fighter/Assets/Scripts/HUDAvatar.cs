using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BootlegPlatformFighter
{
    public class HUDAvatar : MonoBehaviour
    {
        [SerializeField]
        private int _CharacterIndex = -1; // CharacterIndex it's displaying data for

        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _healthText;
        private GameObject _lifePanel;

        private bool hasBeenInitialized = false;
        private void Start()
        {
            if (!hasBeenInitialized)
            {
                _nameText = transform.Find("NamePanel").Find("NameText").GetComponent<TextMeshProUGUI>();
                _healthText = transform.Find("DataPanel").Find("HealthText").GetComponent<TextMeshProUGUI>();
                _lifePanel = transform.Find("DataPanel").Find("LifePanel").gameObject;
                hasBeenInitialized = true;
            }
        }
        private TextMeshProUGUI getHealthText()
        {
            if (!_healthText)
                Start();
            return _healthText;
        }
        private TextMeshProUGUI getNameText()
        {
            if (!_nameText)
                Start();
            return _nameText;
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
            if (!_lifePanel)
                Start();
            return _lifePanel.transform.childCount;
        }
        public void RemoveOneHeart()
        {
            if (getHealthCount() > 0)
                Destroy(_lifePanel.transform.GetChild(getHealthCount() - 1).gameObject);
        }
        public void SetName(string text)
        {
            getNameText().text = text;
        }
        public void SetHealth(float percentage)
        {
            getHealthText().text = percentage + "%";
        }
    }
}
