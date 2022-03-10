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
        private HitBoxHandler hitboxHandler;
        private Vector2 direction;

        [Header("Collider")]
        [SerializeField] public float attackAreaRadius;

        [Header("Damage")]
        [SerializeField] public float damage;
        [SerializeField] public float baseKnockback;
        [SerializeField] public float knockbackScaling = 0.1f;
        [SerializeField] [Range(-180, 180)] public float angle;
        [SerializeField] [Range(1, 10)] private int lineLengthDEBUG = 2;



        void Start()
        {
            hitboxHandler = character.GetComponent<HitBoxHandler>();
            characterController = character.GetComponent<BootlegCharacterController>();
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        void FixedUpdate()
        {
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
        }

        public void SendToKnockback(Collider2D hitEnemy)
        {
            hitEnemy.GetComponent<Knockback>().KnockBack(new Vector2(transform.position.x - character.transform.position.x, 1) * direction, baseKnockback, knockbackScaling, damage);
        }


        private void OnDrawGizmosSelected()
        {
            
            Gizmos.DrawWireSphere(transform.position, attackAreaRadius);
            Debug.DrawLine(transform.position, new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * lineLengthDEBUG + transform.position.x, Mathf.Sin(angle * Mathf.Deg2Rad) * lineLengthDEBUG + transform.position.y), Color.red);
        }
    }
}
