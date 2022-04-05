using System.Text.RegularExpressions;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class AnimationHandler : MonoBehaviour
    {
        private BootlegCharacterController characterController;
        private Animator characterAnimation;
        // Start is called before the first frame update
        void Start()
        {
            characterAnimation = GetComponent<Animator>();
            characterController = GetComponentInParent<BootlegCharacterController>();
        }

        public string GetAnimationName()
        {

            string animationName = characterAnimation.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            string animationRegex = @".*_(\D*)";
            Match animationMatch = Regex.Match(animationName, animationRegex);
            string matchResult = animationMatch.Groups[1].Value;
            return "is" + matchResult + "ing";
        }

        public void ExitAnimation()
        {
            string animationBoolName = GetAnimationName();
            characterAnimation.SetBool(animationBoolName, false);
            characterController.playerState = BootlegCharacterController.PlayerState.GroundIdling;

            AudioManager audioManager = GetComponent<AudioManager>();
            audioManager.audioIndex = 0;
        }

        public void CancelAnimation(string newAnim)
        {
            ExitAnimation();
            characterAnimation.Play(newAnim);
            characterController.GetComponent<HurtBoxHandler>().ResetFrameIndex();
            characterController.GetComponent<HitBoxHandler>().ResetAttackIndex();
            string animationBoolName = GetAnimationName();
            characterAnimation.SetBool(animationBoolName, true);

        }

    }
}
