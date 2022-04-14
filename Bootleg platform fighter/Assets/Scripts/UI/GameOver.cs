using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace BootlegPlatformFighter
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField]
        private Animator crossFadeAnimator;

        [SerializeField]
        private TextMeshProUGUI winText;
        [SerializeField]
        private TextMeshProUGUI statsText;

        // Start is called before the first frame update
        void Start()
        {
            winText.text = GameManagerData.LastWinner.name + " Won!";
            statsText.text = $"Damage Caused: {GameManagerData.LastWinner.damageCaused}%\nDamage Taken: {GameManagerData.LastWinner.damageTaken}";
        }

        public void QuitToCharSelect()
        {
            crossFadeAnimator.SetTrigger("Start");
            StartCoroutine(LoadCharSelectAfterSec());
        }

        private IEnumerator LoadCharSelectAfterSec()
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
