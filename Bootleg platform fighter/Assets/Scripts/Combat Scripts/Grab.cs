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

        [Header("Collider")]
        [SerializeField] public float grabAreaRadius;


        void Start()
        {
            hitboxHandler = character.GetComponent<HitBoxHandler>();
            characterController = character.GetComponent<BootlegCharacterController>();
            fightingScript = character.GetComponent<Fighting>();
        }

        private void FixedUpdate()
        {
            
        }

        public void FindEnemyInArea()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, grabAreaRadius, characterLayers);

            if (hitEnemies.Length > 0)
            {
                foreach (Collider2D enemy in hitEnemies)
                {
                    Fighting enemyFighting = enemy.gameObject.GetComponent<Fighting>();


                    if (enemy.gameObject.GetComponent<BootlegCharacterController>().playerIndex != gameObject.GetComponent<BootlegCharacterController>().playerIndex)
                    {
                        if (enemyFighting.canBeHit)
                        {
                            TurnOffMovement(enemy);
                        }
                    }
                }
            }
        }

        public void TurnOffMovement(Collider2D enemy)
        {
            BootlegCharacterController enemyController = enemy.gameObject.GetComponent<BootlegCharacterController>();
            enemyController.canMove = false;
            characterController.canMove = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, grabAreaRadius);
        }

    }
}
    
