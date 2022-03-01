using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] public float attackAreaRadius;

        [Header("Damage")]
        [SerializeField] public float damage;
        [SerializeField] public float baseKnockback;
        [SerializeField] public float knockbackScaling = 0.1f;
        [SerializeField] [Range(-90, 90)] public float angle;


        void Start()
        {
            hitboxHandler = character.GetComponent<HitBoxHandler>();
            characterController = character.GetComponent<BootlegCharacterController>();
        }

        void FixedUpdate()
        {
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
        }

        public void SendToKnockback(Collider2D hitEnemy)
        {
            hitEnemy.GetComponent<Knockback>().KnockBack(new Vector2(transform.position.x - character.transform.position.x, 1) * direction, baseKnockback, knockbackScaling, damage, angle);
            
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, attackAreaRadius);
        }
    }
}
