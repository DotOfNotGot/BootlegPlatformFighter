using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighting : MonoBehaviour
{
    private BoxCollider2D attackBox;
    private float horizontalInput;
    private float verticalInput;

    bool facingLeft;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask characterLayers;

    // Start is called before the first frame update
    void Start()
    {
        //attackBox = attackBoxObject.gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = gameObject.GetComponent<PlayerController>().horizontalInput;
        verticalInput = gameObject.GetComponent<PlayerController>().verticalInput;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }


        void Attack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, characterLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Knockback>().KnockBack(new Vector2(attackPoint.position.x - transform.position.x,attackPoint.position.y - transform.position.y), 1000.0f);
            }
        }
    }

    public void UpdateAttackPoint(Vector2 direction, float xOffset = 0.85f, float yOffset = 0)
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void BasicAttack(float knockbackVelocity, float direction, float baseDamage, float hitStun, int damageMultiplier = 1)
    {

    }
}
