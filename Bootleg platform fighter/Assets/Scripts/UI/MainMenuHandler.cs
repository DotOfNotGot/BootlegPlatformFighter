using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace BootlegPlatformFighter
{
    public class MainMenuHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject menuPanel;
        [SerializeField]
        private GameObject characterSelectPanel;

        // charscreen
        [SerializeField]
        private TMP_InputField player1Name;
        [SerializeField]
        private TMP_InputField player2Name;

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
            // a bit ghetto but for now I think having it simple and static is best
            GameManagerData.Players.Clear();
            GameManagerData.Players.Add(new Player_t(player1Name.text));
            GameManagerData.Players.Add(new Player_t(player2Name.text));
            StartCoroutine(FightScene());
        }

        private IEnumerator FightScene()
        {
            yield return new WaitForSeconds(1f); // wait for anim
            SceneManager.LoadSceneAsync("WinterStage - Akandesh", LoadSceneMode.Single);
        }
    }
}