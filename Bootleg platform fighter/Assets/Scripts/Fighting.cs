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

        [SerializeField] private Vector2 direction;

        [Header("Jab")]
        [SerializeField] private float jabDamage;
        [SerializeField] private float jabBaseKnockback;
        [SerializeField] private float jabKnockbackScaling = 0.1f;
        [SerializeField] [Range(-90, 90)] private float jabAngle;

        [Header("Forward Tilt")]
        [SerializeField] private float forwardTiltDamage;
        [SerializeField] private float forwardTiltBaseKnockback;
        [SerializeField] private float forwardTiltKnockbackScaling = 0.1f;

        [Header("Up Tilt")]
        [SerializeField] private float upTiltDamage;
        [SerializeField] private float upTiltBaseKnockback;
        [SerializeField] private float upTiltKnockbackScaling = 0.1f;

        [Header("Down Tilt")]
        [SerializeField] private float downTiltDamage;
        [SerializeField] private float downTiltBaseKnockback;
        [SerializeField] private float downTiltKnockbackScaling = 0.1f;

        [Header("Smash")]
        [SerializeField] private float forwardSmashDamage;
        [SerializeField] private float upSmashDamage;
        [SerializeField] private float downSmashDamage;

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

            direction = new Vector2(Mathf.Cos(jabAngle * Mathf.Deg2Rad), Mathf.Sin(jabAngle * Mathf.Deg2Rad));
            Debug.DrawRay(transform.position, new Vector2((direction.x * 5) * horizontalInput, direction.y * 5), Color.green);
            horizontalInput = GetComponent<BootlegCharacterController>().moveVector.x;
            verticalInput = GetComponent<BootlegCharacterController>().moveVector.y;
            UpdateAttackPoint(new Vector2(horizontalInput, verticalInput));
            HandleAttackInput(controls);

        }
        public void Attack(float baseKnockback, float knockbackScaling, float baseDamage)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, characterLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.gameObject.GetComponent<BootlegCharacterController>().playerIndex != characterController.playerIndex)
                {
                    enemy.GetComponent<Knockback>().KnockBack(new Vector2(attackPoint.position.x - transform.position.x, 1) * direction, baseKnockback, knockbackScaling, baseDamage, jabAngle);

                }
            }
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
                    }
                    break;
                #endregion
                #region GROUND DASH & GROUND RUN
                case BootlegCharacterController.PlayerState.GroundDashing:
                case BootlegCharacterController.PlayerState.GroundRunning:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Attack(forwardTiltBaseKnockback, forwardTiltKnockbackScaling, forwardTiltDamage);
                    }
                    break;
                #endregion
                case BootlegCharacterController.PlayerState.GroundCrouching:
                    if (Input.GetKeyDown(KeyCode.E) || controls.normalAttackButton)
                    {
                        Debug.Log("DOWNTILT");
                        Attack(downTiltBaseKnockback, downTiltKnockbackScaling, downTiltDamage);
                    }
                    break;
                default:
                    break;

            }

        }


        void BasicAttack(float knockbackVelocity, float direction, float baseDamage, float hitStun, int damageMultiplier = 1)
        {

        }
    }
}
