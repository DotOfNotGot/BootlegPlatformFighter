using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BootlegPlatformFighter
{
    public class GameOver_old : MonoBehaviour
    {
        [SerializeField]
        private Animator crossFadeAnimator;
        [SerializeField]
        GameObject resultsCardPrefab;
        // Start is called before the first frame update
        void Start()
        {
            foreach (var player in GameManagerData.Players)
            {
                Instantiate(resultsCardPrefab, transform);
            }
        }

        public void LoadMainMenu()
        {
            crossFadeAnimator.SetTrigger("Start");
            StartCoroutine(LoadMainMenuAfterSec());
        }
        private IEnumerator LoadMainMenuAfterSec()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }

        public void RestartStage()
        {
            crossFadeAnimator.SetTrigger("Start");
            StartCoroutine(LoadStageAfterSec());
        }
        private IEnumerator LoadStageAfterSec()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single); // IMPORTANT: Has to be changed when we add more stages
        }
    }
}
