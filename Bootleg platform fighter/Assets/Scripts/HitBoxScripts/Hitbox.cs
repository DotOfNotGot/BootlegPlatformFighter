using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour fightingScript;

        [SerializeField] private GameObject character;
        private BootlegCharacterController characterController;
        private Vector2 direction;
        private HitBoxHandler hitboxHandler;

        [Header("Collider")]
        [SerializeField] public float colliderRadius;

        [Header("Damage")]
        [SerializeField] public float damage;
        [SerializeField] public float baseKnockback;
        [SerializeField] public float knockbackScaling = 0.1f;
        [SerializeField] [Range(-90, 90)] public float angle;

        public int priorityIndex;
        // Start is called before the first frame update
        void Start()
        {
            hitboxHandler = character.GetComponent<HitBoxHandler>();
            characterController = character.GetComponent<BootlegCharacterController>();
            Match regexMatch = Regex.Match(name, @"\d");
            priorityIndex = int.Parse(regexMatch.Value);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, colliderRadius, characterLayers);

            /*foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.gameObject.GetComponent<BootlegCharacterController>().playerIndex != characterController.playerIndex)
                {
                    Debug.Log("Hit");
                    hitboxHandler.CalculateHitBoxPriority(gameObject, hitEnemies);
                }
            }*/
        }

        public void SendToKnockback(Collider2D[] hitEnemies)
        {
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Knockback>().KnockBack(new Vector2(transform.position.x - character.transform.position.x, 1) * direction, baseKnockback, knockbackScaling, damage, angle);
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, colliderRadius);
        }
    }
}
