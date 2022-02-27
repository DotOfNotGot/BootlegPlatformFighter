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
        [SerializeField] private LayerMask characterLayers;
        private List<GameObject> moveList = new List<GameObject>();
        private List<HitBoxStruct> moveStructures = new List<HitBoxStruct>();

        private List<GameObject> attackHitBoxes = new List<GameObject>() {};

        [SerializeField] private int hitLag = 2;


        private int attackIndex = 0;
        // Start is called before the first frame update
        void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            for (int i = 0; i < hitBoxEmpty.transform.childCount; i++)
            {
                moveList.Add(hitBoxEmpty.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < moveList.Count; i++)
            {
                HitBoxStruct newStruct = new HitBoxStruct(moveList[i].name);
                for (int x = 0; x < moveList[i].transform.childCount; x++)
                {
                    GameObject currentAttack = moveList[i];
                    newStruct.HitboxList.Add(currentAttack.transform.GetChild(x).gameObject);
                }
                moveStructures.Add(newStruct);
            }


        }

        #region HANDLE ANIMATION TRIGGER
        public void AnimationHitboxTrigger()
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            foreach (HitBoxStruct attack in moveStructures)
            {
                if (attack.Name == animationName)
                {
                    foreach (GameObject frame in attack.HitboxList)
                    {
                        if (attackIndex > 0)
                        {
                            if (frame.name == animationName + (attackIndex - 1))
                            {
                                attackHitBoxes.Clear();
                                frame.gameObject.SetActive(false);
                            }
                        }
                        if (frame.name == animationName + attackIndex)
                        {
                            frame.gameObject.SetActive(true);
                            for (int i = 0; i < frame.transform.childCount; i++)
                            {
                                attackHitBoxes.Add(frame.transform.GetChild(i).gameObject);
                            }
                            
                        }
                    }
                }
            }
            attackIndex++;
        }

        public void FixedUpdate()
        {
            if (attackHitBoxes.Count > 0)
            {
                for (int i = 0; i < attackHitBoxes.Count; i++)
                {
                    GameObject hitbox = attackHitBoxes[i];
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitbox.transform.position, hitbox.GetComponent<Hitbox>().attackAreaRadius, characterLayers);

                    if (hitEnemies.Length > 0)
                    {
                        foreach (Collider2D enemy in hitEnemies)
                        {
                            Fighting enemyFighting = enemy.gameObject.GetComponent<Fighting>();
                            if (enemy.gameObject.GetComponent<BootlegCharacterController>().playerIndex != gameObject.GetComponent<BootlegCharacterController>().playerIndex)
                            {
                                if (enemyFighting.canBeHit)
                                {
                                    enemyFighting.canBeHit = false;
                                    enemyFighting.StartCoroutine(enemyFighting.HitLag(hitLag));
                                    hitbox.GetComponent<Hitbox>().SendToKnockback(hitEnemies);
                                }
                            }
                        }
                        i = attackHitBoxes.Count;
                    }

                }
            }
           
        }

        public void ResetAttackIndex()
        {
            attackIndex = 0;
            foreach (HitBoxStruct attack in moveStructures)
            {
                foreach (GameObject frame in attack.HitboxList)
                {
                    frame.gameObject.SetActive(false);
                }
            }
        }
        #endregion

    
    }
}
