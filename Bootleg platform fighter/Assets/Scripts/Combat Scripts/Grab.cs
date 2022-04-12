using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Grab : MonoBehaviour
    {


        [SerializeField] private GameObject character;
        private BootlegCharacterController characterController;
        private AnimationHandler animationHandler;
        [SerializeField] private LayerMask hurtBoxLayer;
        private HitBoxHandler hitboxHandler;

        private BootlegCharacterController enemyController;
        private Knockback enemyKnockback;

        [Header("Collider")]
        [SerializeField] public float grabAreaRadius;

        [Header("Damage")]
        [SerializeField] public float damage;
        [SerializeField] public float baseKnockback;
        [SerializeField] public float knockbackScaling = 0.1f;


        void Start()
        {
            hitboxHandler = character.GetComponent<HitBoxHandler>();
            characterController = character.GetComponent<BootlegCharacterController>();
            animationHandler = GetComponent<AnimationHandler>();
        }

        private void FixedUpdate()
        {
            if (enemyController != null)
            {
                if (enemyController.playerState == BootlegCharacterController.PlayerState.Grabbed && characterController.playerState == BootlegCharacterController.PlayerState.Grab)
                {
                    if (characterController.moveVector.x != 0)
                    {
                        enemyKnockback.KnockBack(new Vector2(characterController.moveVector.x, 0), baseKnockback, knockbackScaling, damage);
                        ResetEnemy();
                        animationHandler.ExitAnimation();

                    }
                    else if (characterController.moveVector.y != 0)
                    {
                        enemyKnockback.KnockBack(new Vector2(0,characterController.moveVector.y), baseKnockback, knockbackScaling, damage);
                        ResetEnemy();
                        animationHandler.ExitAnimation();
                    }
                }
                else if (characterController.playerState == BootlegCharacterController.PlayerState.Grab)
                {
                    ResetEnemy();

                }
            }
        }

        public void FindEnemyInArea()
        {
            List<Collider2D> hitHurtBoxes = Physics2D.OverlapCircleAll(transform.position, grabAreaRadius, hurtBoxLayer).ToList();

            if (hitHurtBoxes.Count > 0)
            {
                foreach (Collider2D hurtBox in hitHurtBoxes)
                {
                    HurtBox hurtScript = hurtBox.gameObject.GetComponent<HurtBox>();

                    if (enemyController.characterIndex != characterController.characterIndex)
                    {
                        if (/*enemyFighting.canBeHit &&*/ hurtScript.canBeGrabbed)
                        {
                            SetEnemyState(BootlegCharacterController.PlayerState.Grabbed);
                            break;
                        }
                    }
                }
            }
        }

        private void ResetEnemy()
        {
            enemyController = null;
            enemyKnockback = null;
        }

        public void SetEnemyState(BootlegCharacterController.PlayerState playerState)
        {
            
            enemyController.playerState = playerState;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, grabAreaRadius);
        }

    }
}
    
