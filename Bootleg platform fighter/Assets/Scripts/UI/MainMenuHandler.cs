using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace BootlegPlatformFighter
{
    public class MainMenuHandler : MonoBehaviour
    {

        private PlayerInput playerInput;

        [SerializeField]
        private GameObject menuPanel;
        [SerializeField]
        private GameObject characterSelectPanel;

        // charscreen
        [SerializeField]
        private TMP_InputField player1Name;
        [SerializeField]
        private TMP_InputField player2Name;

        [SerializeField]
        private PlayerInputManager playerManager;

        private void Start()
        {
            if (GameManagerData.setupControllers)
                return;
            for (int i = 0; i < InputSystem.devices.Count; i++)
            {
                //var c = InputSystem.devices[i];
                //Debug.Log(c.description.capabilities);
                playerManager.JoinPlayer(i, -1, "Controller"); // idk why this works tbh, thought i had to link the device directly
            }
            GameManagerData.setupControllers = true;
        }

        public void LoadMainMenu()
        {
            menuPanel.SetActive(true);
            characterSelectPanel.SetActive(false);
        }

        public void LoadCharacterSelect()
        {
            menuPanel.SetActive(false);
            characterSelectPanel.SetActive(true);
        }

        public void LoadFightScene()
        {
            var playerInputObjects = GameObject.FindGameObjectsWithTag("PlayerInputPrefab");
            foreach (GameObject playerInputObject in playerInputObjects)
            {
                DontDestroyOnLoad(playerInputObject);
            }
            // a bit ghetto but for now I think having it simple and static is best
            GameManagerData.Players.Clear();
            GameManagerData.Players.Add(0, new Player_t(player1Name.text));
            GameManagerData.Players.Add(1, new Player_t(player2Name.text));
            StartCoroutine(FightScene());
        }

        private IEnumerator FightScene()
        {
            GameManagerData.GamePaused = false;
            yield return new WaitForSeconds(1f); // wait for anim
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }
    }
}