using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    
    public class HurtBoxHandler : MonoBehaviour
    {

        private Animator animator;
        [SerializeField] private GameObject hurtBoxEmpty;
        [SerializeField] private GameObject defaultHurtBoxSet;
        private List<GameObject> animationList = new List<GameObject>();
        private List<HitHurtBoxStruct> moveStructures = new List<HitHurtBoxStruct>();

        private List<GameObject> animationHurtBoxes = new List<GameObject>() { };
        public BootlegCharacterController characterController;

        private int frameIndex = 0;

        public bool isHitStunned;

        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
            animator = GetComponent<Animator>();
            for (int i = 0; i < hurtBoxEmpty.transform.childCount; i++)
            {
                animationList.Add(hurtBoxEmpty.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < animationList.Count; i++)
            {
                HitHurtBoxStruct newStruct = new HitHurtBoxStruct(animationList[i].name);
                for (int x = 0; x < animationList[i].transform.childCount; x++)
                {
                    GameObject currentAttack = animationList[i];
                    newStruct.BoxList.Add(currentAttack.transform.GetChild(x).gameObject);
                }
                moveStructures.Add(newStruct);
            }
            defaultHurtBoxSet.SetActive(true);
        }

        private void FixedUpdate()
        {
            
        }

        public void AnimationHurtBoxTrigger()
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            foreach (HitHurtBoxStruct animation in moveStructures)
            {
                if (animation.Name == animationName)
                {
                    foreach (GameObject frame in animation.BoxList)
                    {
                        if (frameIndex > 0)
                        {
                            if (frame.name == animationName + (frameIndex - 1))
                            {
                                animationHurtBoxes.Clear();
                                frame.gameObject.SetActive(false);
                            }
                        }
                        if (frame.name == animationName + frameIndex)
                        {
                            frame.gameObject.SetActive(true);
                            for (int i = 0; i < frame.transform.childCount; i++)
                            {
                                animationHurtBoxes.Add(frame.transform.GetChild(i).gameObject);
                            }

                        }
                    }
                }
            }
            frameIndex++;
        }

        public void ResetFrameIndex()
        {
            frameIndex = 0;
            
            foreach (HitHurtBoxStruct animation in moveStructures)
            {
                foreach (GameObject frame in animation.BoxList)
                {
                    frame.gameObject.SetActive(false);
                }
            }
            animationHurtBoxes.Clear();
        }


        

    }
}
