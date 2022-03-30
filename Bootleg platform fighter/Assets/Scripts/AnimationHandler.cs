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

        public void ExitAnimation()
        {
            string animationName = characterAnimation.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            string animationRegex = @".*_(\D*)";
            Match animationMatch = Regex.Match(animationName, animationRegex);
            string matchResult = animationMatch.Groups[1].Value.ToLower();
            char[] matchLetters = matchResult.ToCharArray();
            matchLetters[0] = char.ToUpper(matchLetters[0]);
            matchResult = new string(matchLetters);

            string animationBoolName = "is" + matchResult + "ing";
            characterAnimation.SetBool(animationBoolName, false);
            characterController.playerState = BootlegCharacterController.PlayerState.GroundIdling;

            AudioManager audioManager = GetComponent<AudioManager>();
            audioManager.audioIndex = 0;
        }
    }
}
