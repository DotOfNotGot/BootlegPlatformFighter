using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Fighting : MonoBehaviour
    {
        public BootlegCharacterController.Controls controls;

        private BootlegCharacterController characterController;
        private BoxCollider2D attackBox;
        private float horizontalInput;
        private float verticalInput;
        private float lastHorizontalInput = 1;

        [SerializeField] Collider2D playerCollider;

        public Transform attackPoint;
        public float attackRange = 0.5f;
        public LayerMask characterLayers;

        public bool canBeHit = true;

        [SerializeField] private Vector2 direction;


        // Start is called before the first frame update
        void Start()
        {
            characterController = gameObject.GetComponent<BootlegCharacterController>();
            attackPoint.localScale = new Vector3(8 * attackRange, 8 * attackRange, 8 * attackRange);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            /*This gets the character's direction into 2 variables, so that can be used to
             * determine the Vector2 of the attackPoint.*/

            Debug.DrawRay(transform.position, new Vector2((direction.x * 5) * horizontalInput, direction.y * 5), Color.green);
            horizontalInput = GetComponent<BootlegCharacterController>().moveVector.x;
            verticalInput = GetComponent<BootlegCharacterController>().moveVector.y;
            UpdateAttackPoint(new Vector2(horizontalInput, verticalInput));
            HandleAttackInput(controls);

        }

        public void UpdateAttackPoint(Vector2 direction, float xOffset = 0.85f, float yOffset = 1.5f)
        {
            if (direction.y != 0)
            {
                attackPoint.localPosition = new Vector2(0, yOffset * direction.y);
            }
            else
            {
                attackPoint.localPosition = new Vector2(xOffset, 0);
            }

        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
            {
                return;
            }
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        public void HandleAttackInput(BootlegCharacterController.Controls controls)
        {
            /*
             Depending on the current playerState in BootlegCharacterController.cs, 
            and which attack inputs are being pressed, different variables will be 
            used for the different attacks.

            KeyCode as input is Placeholder only, should be replaced with
            a way that allows for customizability.
             */
            switch (characterController.playerState)
            {
                #region GROUND IDLING
                case BootlegCharacterController.PlayerState.GroundIdling:

<<<<<<< HEAD
                    if (controls.normalAttackButtonPressed)
                    {

                    }

                    if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.W) || (controls.normalAttackButtonPressed && controls.verticalInput > 0))
                    {
                        
                    }
                    else if ((Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.S)) || (controls.normalAttackButtonPressed && controls.verticalInput < 0))
                    {
                        
                    }
                    else if (Input.GetKeyDown(KeyCode.E) || controls.normalAttackButtonPressed)
                    {
                        characterController.GetComponent<Animator>().SetBool("isJabbing", true);
                        //Attack(jabBaseKnockback, jabKnockbackScaling, jabDamage);
=======

                    if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.W) || (controls.normalAttackButtonPressed && controls.verticalInput > 0))
                    {
                        Debug.Log("UPTILT");
                        Attack(upTiltBaseKnockback, upTiltKnockbackScaling, upTiltDamage);
                    }
                    else if ((Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.S)) || (controls.normalAttackButtonPressed && controls.verticalInput < 0))
                    {
                        Debug.Log("DOWNTILT");
                        Attack(downTiltBaseKnockback, downTiltKnockbackScaling, downTiltDamage);
                    }
                    else if (Input.GetKeyDown(KeyCode.E) || controls.normalAttackButtonPressed)
                    {
                        Debug.Log("JAB");
                        Attack(jabBaseKnockback, jabKnockbackScaling, jabDamage);
>>>>>>> main
                    }
                    break;
                #endregion
                #region GROUND DASH & GROUND RUN
                case BootlegCharacterController.PlayerState.GroundDashing:
                case BootlegCharacterController.PlayerState.GroundRunning:
                    if (Input.GetKeyDown(KeyCode.E))
                    {

                    }
                    break;
                #endregion
                case BootlegCharacterController.PlayerState.GroundCrouching:
                    if (Input.GetKeyDown(KeyCode.E) || controls.normalAttackButton)
                    {

                    }
                    break;
                default:
                    break;

            }

        }


        public IEnumerator HitLag(int frameCount)
        {
            yield return StartCoroutine(WaitFor.Frames(frameCount));
            canBeHit = true;
        }
    }

    public static class WaitFor
    {
        public static IEnumerator Frames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;

            }
        }
    }
}
