using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    struct HitBoxStruct
    {
        public string Name;
        public List<GameObject> HitboxList;

        public HitBoxStruct(string name)
        {
            Name = name;
            HitboxList = new List<GameObject>();
        }
    }
    public class HitBoxHandler : MonoBehaviour
    {
        
        private Animator animator;
        [SerializeField] private GameObject hitBoxEmpty;

        private List<GameObject> AttackList = new List<GameObject>();

        private List<HitBoxStruct> attackStructures = new List<HitBoxStruct>();

        private int attackIndex = 0;
        // Start is called before the first frame update
        void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            for (int i = 0; i < hitBoxEmpty.transform.childCount; i++)
            {
                AttackList.Add(hitBoxEmpty.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < AttackList.Count; i++)
            {
                HitBoxStruct newStruct = new HitBoxStruct(AttackList[i].name);
                for (int x = 0; x < AttackList[i].transform.childCount; x++)
                {
                    GameObject currentAttack = AttackList[i];
                    newStruct.HitboxList.Add(currentAttack.transform.GetChild(x).gameObject);
                }
                attackStructures.Add(newStruct);
            }

        }


        public void AnimationHitboxTrigger()
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            foreach (HitBoxStruct attack in attackStructures)
            {
                if (attack.Name == animationName)
                {
                    foreach (GameObject frame in attack.HitboxList)
                    {
                        if (attackIndex > 0)
                        {
                            if (frame.name == animationName + (attackIndex - 1))
                            {
                                frame.gameObject.SetActive(false);
                            }
                        }
                        if (frame.name == animationName + attackIndex)
                        {
                            frame.gameObject.SetActive(true);
                            
                        }
                    }
                }
            }
            attackIndex++;
        }

        public void ResetAttackIndex()
        {
            attackIndex = 0;
            foreach (HitBoxStruct attack in attackStructures)
            {
                foreach (GameObject frame in attack.HitboxList)
                {
                    frame.gameObject.SetActive(false);
                }
            }
        }
    }
}
