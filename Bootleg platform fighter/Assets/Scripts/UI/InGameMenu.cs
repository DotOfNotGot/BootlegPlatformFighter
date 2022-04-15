using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace BootlegPlatformFighter
{
    public class InGameMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject Menu;
        [SerializeField]
        private GameObject QuitTint;
        [SerializeField]
        private GameObject QuitDialog;
        [SerializeField]
        private GameObject RestartTint;
        [SerializeField]
        private GameObject RestartDialog;
        [SerializeField]
        private GameObject OptionsMenu;
        [SerializeField]
        private GameObject ResumeButton;

        [SerializeField]
        private Animator CrossFadeAnimator;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            GameManagerData.GamePaused = Menu.activeSelf;
            if (Input.GetKeyDown(KeyCode.Escape) || Gamepad.current.startButton.wasPressedThisFrame)
            {
                Menu.SetActive(!Menu.activeSelf);


                // breaks scale animation :(
                //if (Menu.activeSelf)
                //    Time.timeScale = 0f;
                //else
                //    Time.timeScale = 1f;
            }
        }

        // Button
        public void ResumeGame()
        {
            Menu.SetActive(false);
        }
        // Button
        public void QuitGame()
        {
            QuitTint.SetActive(true);
            QuitDialog.SetActive(true);
        }
        // Button
        public void RestartGame()
        {
            RestartTint.SetActive(true);
            RestartDialog.SetActive(true);
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

        public void LoadMainMenuCallback(BaseEventData dat)
        {
            CrossFadeAnimator.SetTrigger("Start");
            StartCoroutine(loadMainMenuAfterSecond());
        }

        public void RestartGameCallback(BaseEventData dat)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            ResumeGame();
        }

        private IEnumerator loadMainMenuAfterSecond()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }

        public void CancelQuitCallback(BaseEventData dat)
        {
            QuitTint.SetActive(false);
            RestartTint.SetActive(false);
            QuitDialog.SetActive(false);
            RestartDialog.SetActive(false);
            EventSystem.current.SetSelectedGameObject(ResumeButton);
        }
    }
}
