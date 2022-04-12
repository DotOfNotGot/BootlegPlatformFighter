using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class SceneAnimator : MonoBehaviour
    {
        [SerializeField]
        private Animator crossFadeAnimator;

        public void FadeOut()
        {
            crossFadeAnimator.SetTrigger("Start");
        }
    }
}
