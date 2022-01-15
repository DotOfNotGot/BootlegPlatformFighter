using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float jumpForce;
    public float airControl;
    public float doubleJumpForce;
    public float gravityModifier;

    private Rigidbody2D playerRb;
    private float airdashForce = 20.0f;
    public float horizontalInput;
    public float verticalInput;
    private Vector2 moveVector;
    public float angle;
    private float oldHorizontalInput;

    // Bools for checking states
    public bool isOnGround;
    public bool hasAirJump;
    public bool hasAirDash;


    // Start is called before the first frame update
    void Start()
    {
        isOnGround = false;
        playerRb = GetComponent<Rigidbody2D>();
        Physics2D.gravity *= gravityModifier;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        moveVector.x = horizontalInput;
        moveVector.y = verticalInput;
        angle = Vector2.Angle(moveVector, Vector2.right);
        if (isOnGround)
        {
            // Movement left and right
            transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * speed);


            // Grounded jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                oldHorizontalInput = horizontalInput;
                playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isOnGround = false;
            }
        }
        else if (!isOnGround)
        {
            // Jump arc
            transform.Translate(Vector2.right * oldHorizontalInput * Time.deltaTime * speed);

            // Checking which direction you can influence jump arc
            if (oldHorizontalInput > 0 && horizontalInput < 0)
            {
                transform.Translate((Vector2.right * oldHorizontalInput * Time.deltaTime * speed) * horizontalInput);
            }
            else if (oldHorizontalInput < 0 && horizontalInput > 0)
            {
                transform.Translate((Vector2.right * oldHorizontalInput * Time.deltaTime * speed) * -horizontalInput);
            }

            // Midair/Double jump
            if (Input.GetKeyDown(KeyCode.Space) && hasAirJump)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);

                transform.Translate((Vector2.right * oldHorizontalInput/2 * Time.deltaTime * speed));
                oldHorizontalInput = horizontalInput / 4;
                hasAirJump = false;
            }

            if (Input.GetKeyDown(KeyCode.K) && hasAirDash)
            {
                playerRb.AddForce(moveVector * airdashForce, ForceMode2D.Impulse);
                hasAirDash = false;
            }
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && playerRb.velocity.y == 0)
        {
            isOnGround = true;
            hasAirJump = true;
            hasAirDash = true;
        }
    }
}
