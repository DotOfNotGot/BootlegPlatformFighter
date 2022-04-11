using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

namespace BootlegPlatformFighter
{
    public class OptionsHandler : MonoBehaviour
    {
        [SerializeField]
        AudioSource mainAudio;

        [SerializeField]
        private GameObject Tint;
        [SerializeField]
        private GameObject OptionsMenu;
        [SerializeField]
        private Slider fpsSlider;
        [SerializeField]
        private Slider tbdVolumeSlider;
        [SerializeField]
        private Toggle vsyncToggle;
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;
        [SerializeField]
        private Toggle fullscreenToggle;

        // Start is called before the first frame update
        void Start()
        {
            ResetToDefault(); // We don't save configs yet
            InitializeResolutions();
        }
        public void ResetToDefault()
        {
            Application.targetFrameRate = -1; // Default per platform, uncapped for standalone platforms
            QualitySettings.vSyncCount = 0; // Vsync off
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            fpsSlider.value = Application.targetFrameRate == -1 ? 0 : Application.targetFrameRate;

            if (mainAudio)
            {
                mainAudio.volume = 1; // 0-1f
                tbdVolumeSlider.value = mainAudio.volume * 100f;
            }
        }

        private void InitializeResolutions()
        {
            resolutionDropdown.ClearOptions();
            int selectedResolution = 0;
            List<string> optionStrings = new List<string>();
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                var resolution = Screen.resolutions[i];
                if (resolution.height == Screen.height &&
                    resolution.width == Screen.width)
                {
                    selectedResolution = i;
                }
                string option = resolution.width + "x" + resolution.height;
                optionStrings.Add(option);
            }
            resolutionDropdown.AddOptions(optionStrings);
            resolutionDropdown.value = selectedResolution;
        }

        public void ShowOptions()
        {
            Tint.SetActive(true);
            OptionsMenu.SetActive(true);
        }

        public void SaveOptions()
        {
            // Save the values here
            Debug.Log("VOLUME: " + tbdVolumeSlider.value);
            Application.targetFrameRate = (int)fpsSlider.value;
            QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;

            int gameWidth = int.Parse(resolutionDropdown.options[resolutionDropdown.value].text.Split('x')[0]);
            int gameHeight = int.Parse(resolutionDropdown.options[resolutionDropdown.value].text.Split('x')[1]);
            if (Screen.width != gameWidth || gameHeight != Screen.height)
            {
                Screen.SetResolution(gameWidth, gameHeight, fullscreenToggle.isOn);
                Debug.Log("Changed resolution");
                Debug.Log(Screen.width + "x" + Screen.height);
                Debug.Log(gameWidth + "x" + gameHeight);
            }


            // then close
            CloseOptions();
        }

        public void CloseOptions()
        {
            Tint.SetActive(false);
            OptionsMenu.SetActive(false);
        }
    }
}
