using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BootlegPlatformFighter
{
    [System.Serializable]
    public class Audio
    {
        public string Name;
        [Range(0.0f,1.0f)] public float Volume = 1.0f;
        public AudioClip Clip;
    }

    public class AudioManager : MonoBehaviour
    {
        private AudioSource source;
        public Audio[] audioArray;
        public int audioIndex = 0;


        private Animator animator;
        private BootlegCharacterController characterController;


        private void Start()
        {
            source = gameObject.AddComponent<AudioSource>();
            animator = GetComponent<Animator>();
            characterController = GetComponent<BootlegCharacterController>();
        }

        public void AnimationTriggerSound()
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            string regex = @".*_(\D*)";

            foreach (Audio audio in audioArray)
            {
                Match audioMatch = Regex.Match(animationName, regex);
                if (audioMatch.Success)
                {
                    if (audio.Name.ToLower() == audioMatch.Groups[1].Value.ToLower())
                    {
                        PlaySound(audio);
                    }
                    else if (audio.Name.ToLower() == audioMatch.Groups[1].Value.ToLower() + audioIndex)
                    {
                        PlaySound(audio);
                        audioIndex++;
                    }
                }
            }
            
        }


        private void PlaySound(Audio clip)
        {
            source.volume = clip.Volume;
            source.PlayOneShot(clip.Clip);
        }
    }
}
