using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BootlegPlatformFighter
{
    struct HitHurtBoxStruct
    {
        public string Name;
        public List<GameObject> BoxList;

        public HitHurtBoxStruct(string name)
        {
            Name = name;
            BoxList = new List<GameObject>();
        }
    }
    public class HitBoxHandler : MonoBehaviour
    {

        private Animator animator;
        [SerializeField] private GameObject hitBoxEmpty;
        [SerializeField] private LayerMask hurtBoxLayers;
        private List<GameObject> moveList = new List<GameObject>();
        private List<HitHurtBoxStruct> moveStructures = new List<HitHurtBoxStruct>();

        private List<GameObject> attackHitBoxes = new List<GameObject>() { };

        [SerializeField] private int hitLagFrames = 60;

        private int remainingLagFrames = 0;

        private BootlegCharacterController bootlegCharacterController;

        private int attackIndex = 0;
        // Start is called before the first frame update
        void Start()
        {
            bootlegCharacterController = GetComponentInParent<BootlegCharacterController>();
            animator = gameObject.GetComponent<Animator>();
            for (int i = 0; i < hitBoxEmpty.transform.childCount; i++)
            {
                moveList.Add(hitBoxEmpty.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < moveList.Count; i++)
            {
                HitHurtBoxStruct newStruct = new HitHurtBoxStruct(moveList[i].name);
                for (int x = 0; x < moveList[i].transform.childCount; x++)
                {
                    GameObject currentAttack = moveList[i];
                    newStruct.BoxList.Add(currentAttack.transform.GetChild(x).gameObject);
                }
                moveStructures.Add(newStruct);
            }


        }

        #region HANDLE ANIMATION TRIGGER
        public void AnimationHitboxTrigger()
        {
            string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            foreach (HitHurtBoxStruct attack in moveStructures)
            {
                if (attack.Name == animationName)
                {
                    foreach (GameObject frame in attack.BoxList)
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
            if (remainingLagFrames > 0) {
                --remainingLagFrames;
                return;
            }
            
            
            if (attackHitBoxes.Count > 0) {
                for (int i = 0; i < attackHitBoxes.Count; i++)
                {
                    GameObject hitbox = attackHitBoxes[i];
                    List<Collider2D> hurtBoxes = Physics2D.OverlapCircleAll(hitbox.transform.position, hitbox.GetComponent<Hitbox>().attackAreaRadius, hurtBoxLayers).ToList();

                    if (hurtBoxes.Count > 0)
                    {
                        foreach (Collider2D hurtBox in hurtBoxes)
                        {
                            HurtBox hurtScript = hurtBox.gameObject.GetComponent<HurtBox>();

                            if (hurtScript.characterIndex != bootlegCharacterController.characterIndex)
                            {
                                remainingLagFrames = hitLagFrames;
                                //Debug.Log("Hit " + hurtBox.name + " Belonging to " + hurtScript.character.name + " with " + hitbox.name + " belonging to " + gameObject.name);
                                hitbox.GetComponent<Hitbox>().SendToKnockback(hurtScript.character);
                            }
                        }
                    }
                    i = attackHitBoxes.Count;
                }
            }
        }

        public void ResetAttackIndex()
        {
            attackIndex = 0;
            foreach (HitHurtBoxStruct attack in moveStructures)
            {
                foreach (GameObject frame in attack.BoxList)
                {
                    frame.gameObject.SetActive(false);
                }
            }
            attackHitBoxes.Clear();
        }
        #endregion


    }
}
