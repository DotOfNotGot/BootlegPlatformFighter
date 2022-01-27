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


        public Transform attackPoint;
        public float attackRange = 0.5f;
        public LayerMask characterLayers;

        // Start is called before the first frame update
        void Start()
        {
            characterController = gameObject.GetComponent<BootlegCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            /*This gets the character's direction into 2 variables, so that can be used to
             * determine the Vector2 of the attackPoint.*/
            horizontalInput = GetComponent<BootlegCharacterController>().moveVector.x;
            verticalInput = GetComponent<BootlegCharacterController>().moveVector.y;
            UpdateAttackPoint(new Vector2(horizontalInput, verticalInput));
            
            HandleAttackInput();

        }
        public void Attack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, characterLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Knockback>().KnockBack(new Vector2(attackPoint.position.x - transform.position.x, attackPoint.position.y - transform.position.y), 100.0f, 0.1f, 1.0f);
            }
        }

        public void UpdateAttackPoint(Vector2 direction, float xOffset = 0.85f, float yOffset = 1.5f)
        {
            if (direction.x != 0)
            {
                lastHorizontalInput = direction.x;
            }
            if (!(direction.x == 0 && direction.y == 0))
            {
                attackPoint.localPosition = new Vector2(xOffset * direction.x, yOffset * direction.y);

            }
            else
            {
                attackPoint.localPosition = new Vector2(xOffset * lastHorizontalInput, yOffset * direction.y);
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

        private void HandleAttackInput()
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
                #region GROUND_IDLING
                case BootlegCharacterController.PlayerState.GroundIdling:
                    if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.W))
                    {
                        Debug.Log("ww");
                    }
                    else if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.S))
                    {
                        Debug.Log("ss");
                    }
                    else if (Input.GetKeyDown(KeyCode.F))
                    {
                        Attack();
                    }
                    break;
                #endregion
                default:
                    break;

            }
            
        }


        void BasicAttack(float knockbackVelocity, float direction, float baseDamage, float hitStun, int damageMultiplier = 1)
        {

        }
    }
}
