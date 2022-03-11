using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Grab : MonoBehaviour
    {

        private Fighting fightingScript;

        [SerializeField] private GameObject character;
        private BootlegCharacterController characterController;
        [SerializeField] private LayerMask characterLayers;
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
            fightingScript = character.GetComponent<Fighting>();
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
                        characterController.ExitAnimation();

                    }
                    else if (characterController.moveVector.y != 0)
                    {
                        enemyKnockback.KnockBack(new Vector2(0,characterController.moveVector.y), baseKnockback, knockbackScaling, damage);
                        ResetEnemy();
                        characterController.ExitAnimation();
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
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, grabAreaRadius, characterLayers);

            if (hitEnemies.Length > 0)
            {
                foreach (Collider2D enemy in hitEnemies)
                {
                    Fighting enemyFighting = enemy.gameObject.GetComponent<Fighting>();
                    enemyKnockback = enemy.gameObject.GetComponent<Knockback>();
                    enemyController = enemy.gameObject.GetComponent<BootlegCharacterController>();

                    if (enemyController.characterIndex != characterController.characterIndex)
                    {
                        if (enemyFighting.canBeHit)
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
    
