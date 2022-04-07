using System.Text.RegularExpressions;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class AnimationHandler : MonoBehaviour
    {
        private BootlegCharacterController characterController;
        private Animator characterAnimation;
        private HurtBoxHandler hurtBoxHandler;
        // Start is called before the first frame update
        void Start()
        {
            characterAnimation = GetComponent<Animator>();
            characterController = GetComponentInParent<BootlegCharacterController>();
            hurtBoxHandler = GetComponent<HurtBoxHandler>();
        }

        public string GetAnimationName()
        {

            string animationName = characterAnimation.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            string animationRegex = @".*_(\D*)";
            Match animationMatch = Regex.Match(animationName, animationRegex);
            string matchResult = animationMatch.Groups[1].Value;
            return "is" + matchResult + "ing";
        }

        public void ExitAnimation(string returnToIdle = null)
        {
            string animationBoolName = GetAnimationName();
            characterAnimation.SetBool(animationBoolName, false);
            AudioManager audioManager = GetComponent<AudioManager>();
            audioManager.audioIndex = 0;

            //Get which state to enter into
            if (returnToIdle != null)
            {
                characterController.playerState = BootlegCharacterController.PlayerState.GroundIdling;
                EnterNewAnimation("Huldra_Idle");
            }

        }

        public void EnterNewAnimation(string newAnim)
        {
            characterAnimation.Play(newAnim);
        }

        public void CancelAnimation(string newAnim)
        {
            ExitAnimation();
            EnterNewAnimation(newAnim);
            hurtBoxHandler.ResetFrameIndex();
            GetComponent<HitBoxHandler>().ResetAttackIndex();
            string animationBoolName = GetAnimationName();
            characterAnimation.SetBool(animationBoolName, true);
        }

    }
}
