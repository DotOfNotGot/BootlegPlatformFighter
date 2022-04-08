using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BootlegPlatformFighter
{
    public class UISelector : MonoBehaviour
    {
        [SerializeField]
        private List<string> Options;

        string currentOption = "";
        int currentIndex = 0;

        private void Start()
        {
            currentOption = Options[currentIndex];
            GetComponentInChildren<TextMeshProUGUI>().text = currentOption;

            if (Options.Count == 1)
            {
                // Disable buttons when they're useless
                var buttons = GetComponentsInChildren<Button>();
                foreach (var butt in buttons)
                {
                    butt.interactable = false;
                }
            }
        }

        public void NextOption()
        {
            if (currentIndex + 1 > Options.Count - 1)
                currentIndex = 0;
            else
                currentIndex++;
            currentOption = Options[currentIndex];
            GetComponentInChildren<TextMeshProUGUI>().text = currentOption;
        }

        public void PreviousOption()
        {
            if (currentIndex - 1 < 0)
                currentIndex = Options.Count - 1;
            else
                currentIndex--;
            currentOption = Options[currentIndex];
            GetComponentInChildren<TextMeshProUGUI>().text = currentOption;
        }
    }
}
