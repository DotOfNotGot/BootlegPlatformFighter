using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BootlegPlatformFighter
{
    public class InGameMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject Menu;
        [SerializeField]
        private GameObject Tint;
        [SerializeField]
        private GameObject QuitDialog;
        [SerializeField]
        private GameObject OptionsMenu;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Menu.SetActive(!Menu.activeSelf);
            }
        }

        public void ResumeGame()
        {
            Menu.SetActive(false);
        }

        public void ShowOptions()
        {
            Tint.SetActive(true);
            OptionsMenu.SetActive(true);
        }

        public void QuitGame()
        {
            Tint.SetActive(true);
            QuitDialog.SetActive(true);
        }
        public void QuitCallback(BaseEventData dat)
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }
}
