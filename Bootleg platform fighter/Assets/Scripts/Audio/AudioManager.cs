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
        public AudioClip clip;
    }

    public class AudioManager : MonoBehaviour
    {
        private AudioSource source;
        public Audio[] whooshArray;
        public Audio[] hitArray;
        public int audioIndex = 0;

        public bool hasHit = false;

        private HitBoxHandler hitBoxHandler;

        private Animator animator;
        private BootlegCharacterController characterController;


        private void Start()
        {
            source = gameObject.AddComponent<AudioSource>();
            animator = GetComponent<Animator>();
            characterController = GetComponent<BootlegCharacterController>();
            hitBoxHandler = GetComponent<HitBoxHandler>();
        }

        public void AnimationTriggerSound()
        {
            if (!hasHit)
            {
                string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                string regex = @".*_(\D*)";

                foreach (Audio audio in whooshArray)
                {
                    Match audioMatch = Regex.Match(animationName, regex);
                    if (audioMatch.Success)
                    {
                        if (audio.Name.ToLower() == audioMatch.Groups[1].Value.ToLower())
                        {
                            PlaySound(audio.clip);
                        }
                        else if (audio.Name.ToLower() == audioMatch.Groups[1].Value.ToLower() + audioIndex)
                        {
                            PlaySound(audio.clip);
                            audioIndex++;
                        }
                    }
                }

            }
        }
        public void HitTriggerSound()
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            string regex = @".*_(\D*)";

            foreach (Audio audio in hitArray)
            {
                Match audioMatch = Regex.Match(animationName, regex);
                if (audioMatch.Success)
                {
                    if (audio.Name.ToLower() == audioMatch.Groups[1].Value.ToLower())
                    {
                        PlaySound(audio.clip);
                    }
                    else if (audio.Name.ToLower() == audioMatch.Groups[1].Value.ToLower() + audioIndex)
                    {
                        PlaySound(audio.clip);
                        audioIndex++;
                    }
                }
            }
            hasHit = true;
            
        }


        private void PlaySound(AudioClip clip)
        {
            source.PlayOneShot(clip);
        }
    }
}
