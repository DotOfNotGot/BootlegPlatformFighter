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
        private AudioManager audioManager;

        private List<GameObject> attackHitBoxes = new List<GameObject>() { };

        [SerializeField] private int hitLagFrames = 60;

        private int blockStun = 100;

        private int remainingLagFrames = 0;

        public bool hitTarget = false;

        private HurtBox hurtScript;
        private GameObject hitbox;

        private BootlegCharacterController bootlegCharacterController;

        private int attackIndex = 0;

        public bool visualizeHitboxes = false;

        // Start is called before the first frame update
        void Start()
        {
            audioManager = GetComponent<AudioManager>();
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
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                visualizeHitboxes = !visualizeHitboxes;
            }
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
                    hitbox = attackHitBoxes[i];
                    List<Collider2D> hurtBoxes = Physics2D.OverlapCircleAll(hitbox.transform.position, hitbox.GetComponent<Hitbox>().attackAreaRadius, hurtBoxLayers).ToList();
                    List<Collider2D> tempHurtBoxes = new List<Collider2D>();
                    foreach (Collider2D hurtBox in hurtBoxes)
                    {
                        if (hurtBox.gameObject.GetComponent<HurtBox>().characterIndex != bootlegCharacterController.characterIndex)
                        {
                            tempHurtBoxes.Add(hurtBox);
                        }
                    }
                    hurtBoxes = tempHurtBoxes;
                    if (hurtBoxes.Count > 0 && !hitTarget)
                    {
                        foreach (Collider2D hurtBox in hurtBoxes)
                        {

                            hurtScript = hurtBox.gameObject.GetComponent<HurtBox>();
                            Debug.Log(hurtScript.characterIndex);

                            if (hurtScript.characterIndex != bootlegCharacterController.characterIndex && hurtScript.characterController.canBeHit)
                            {
                                remainingLagFrames = hitLagFrames;
                                //Debug.Log("Hit " + hurtBox.name + " Belonging to " + hurtScript.character.name + " with " + hitbox.name + " belonging to " + gameObject.name);
                                if (hurtScript.characterController.playerState == BootlegCharacterController.PlayerState.GroundCrouching)
                                {
                                    //GetComponent<Knockback>().StartHitStun(blockStun);
                                    break;
                                }
                                else if (!hitTarget)
                                {
                                    audioManager.HitTriggerSound();
                                    hitbox.GetComponent<Hitbox>().SendToHitStun(hurtScript.character);
                                    hitTarget = true;
                                    break;
                                }

                            }
                        }
                    }
                    i = attackHitBoxes.Count;
                }



                
            }
            if (hurtScript != null)
            {
                if (hurtScript.characterIndex == bootlegCharacterController.characterIndex)
                {
                    hurtScript = null;
                }
                else if (hitTarget && !hurtScript.character.gameObject.GetComponent<Knockback>().isHitStunned)
                {
                   
                    hitTarget = false;
                    hurtScript = null;
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
