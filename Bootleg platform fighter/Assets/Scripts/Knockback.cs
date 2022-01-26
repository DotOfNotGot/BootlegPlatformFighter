using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KnockBack(Vector2 direction, float knockbackVelocity)
    {
        direction = new Vector2(direction.x * knockbackVelocity, direction.y * knockbackVelocity);
        rigidBody.AddForce(direction);
    }
}
