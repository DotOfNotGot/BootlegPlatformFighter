using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{

    // Constant values
    private const float deadZone = 0.1f;
    private const float walkZone = 0.5f;

    // Character specific values
    public float speed;
    public float jumpForce;
    public float shortHopForce;
    public float airControl;
    public float doubleJumpForce;
    public float gravityModifier;
    public float dashForce;
    public int dashLength;
    // State variables
    public PlayerState playerState;
    [SerializeField] private PlayerState previousPlayerState;

    public enum PlayerState
    {
        GroundIdling,
        GroundJumpSquatting,
        GroundWalking,
        GroundDashing,
        GroundRunning,
        Airdashing,
        Jumping,
        Airborne
    }

    private int groundIdlingWalkCounter = 0;
    private int groundJumpSquattingCounter = 0;
    private int framesJumpButtonPressed = 0;
    private int groundWalkingIdleCounter = 0;
    private int groundRunningIdleCounter = 0;
    public int groundDashingCounter = 0;
    private int airdashCounter = 0;
    private int airborneCounter = 0;

    private float previousHorizontalInput;
    private bool jumpButtonStatePreviousFrame;
    private bool airdashButtonStatePreviousFrame;

    private Rigidbody2D playerRb;
    private int airdashTime = 10;
    private float airdashForce = 50.0f;
    public float horizontalInput;
    public float verticalInput;
    private bool jumpButton;
    private bool airdashButton;
    private Vector2 moveVector;
    public float angle;
    private float airborneTrajectory;
    private float jumpStartHorizontalInput;
    private float jumpsquatStartHorizontalInput;
    private float airdashStartHorizontalInput;
    private float airdashStartVerticalInput;

    // Bools for checking states
    public bool isOnGround;
    public bool hasAirJump;
    public bool hasAirDash;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        Physics2D.gravity *= gravityModifier;

    }

    private void FixedUpdate()
    {
        // Read all input
        previousHorizontalInput = horizontalInput;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveVector.x = horizontalInput;
        moveVector.y = verticalInput;
        angle = Vector2.Angle(moveVector, Vector2.right);

        jumpButtonStatePreviousFrame = jumpButton;
        jumpButton = Input.GetButton("Jump");

        airdashButtonStatePreviousFrame = airdashButton;
        airdashButton = Input.GetButton("Airdash");


        // Handles state changes
        UpdatePlayerState();

        if (playerState == PlayerState.Jumping)
        {


            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);

            if (hasAirJump)
            {

                playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                playerRb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
                //// Jump arc


                //// Checking which direction you can influence jump arc
                //if (jumpStartHorizontalInput > 0 && horizontalInput < 0)
                //{
                //    transform.Translate((Vector2.right * jumpStartHorizontalInput * Time.deltaTime * speed) * horizontalInput / 2);
                //}
                //else if (jumpStartHorizontalInput < 0 && horizontalInput > 0)
                //{
                //    transform.Translate((Vector2.right * jumpStartHorizontalInput * Time.deltaTime * speed) * -horizontalInput / 2);
                //}

            }
        }

        if (playerState == PlayerState.Airborne)
        {


            if (airborneCounter == 0 && previousPlayerState == PlayerState.Airdashing)
            {
                    playerRb.velocity = new Vector2(0, 0);
            }

            if (previousPlayerState == PlayerState.Jumping)
            {
                if (hasAirJump)
                {
                    airborneTrajectory = jumpsquatStartHorizontalInput;
                }
                if (!hasAirJump)
                {
                    airborneTrajectory = jumpStartHorizontalInput;
                }
                
            }

            if (previousPlayerState == PlayerState.Airdashing)
            {
                airborneTrajectory = horizontalInput * airControl;
            }

            transform.Translate((Vector2.right * airborneTrajectory * Time.deltaTime * speed));
        }

        if (playerState == PlayerState.Airdashing)
        {
            if (airdashCounter == 0)
            {
                playerRb.gravityScale = 0;
                airdashStartHorizontalInput = horizontalInput;
                airdashStartVerticalInput = verticalInput;
                playerRb.velocity = new Vector2(0, 0);
                playerRb.AddForce(new Vector2(airdashStartHorizontalInput * airdashForce, airdashStartVerticalInput * airdashForce), ForceMode2D.Impulse);
            }

            if (airdashCounter < airdashTime * 0.75)
            {
                playerRb.velocity = new Vector2(airdashStartHorizontalInput * airdashForce, airdashStartVerticalInput * airdashForce);
            }


        }

        if (playerState == PlayerState.GroundDashing)
        {
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            playerRb.AddForce(Vector2.right * (dashForce * horizontalInput), ForceMode2D.Impulse);
        }

        if (playerState == PlayerState.GroundRunning || playerState == PlayerState.GroundWalking)
        {
            transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * speed);
        }
    }

    private void UpdatePlayerState()
    {

        switch (playerState)
        {
            #region GROUND_IDLING
            case PlayerState.GroundIdling:
                bool groundIdlingWalkCounterShouldIncrease = false;
                if (horizontalInput > walkZone || horizontalInput < -walkZone)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundDashing;
                }
                else if (horizontalInput > deadZone || horizontalInput < -deadZone)
                {
                    if (groundIdlingWalkCounter == 5)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundWalking;
                    }
                    else
                    {
                        groundIdlingWalkCounterShouldIncrease = true;
                    }
                }

                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                    groundIdlingWalkCounterShouldIncrease = false;
                }

                if (groundIdlingWalkCounterShouldIncrease)
                {
                    groundIdlingWalkCounter++;
                }
                else
                {
                    groundIdlingWalkCounter = 0;
                }

                break;
            #endregion
            #region GROUND_JUMPSQUATTING
            case PlayerState.GroundJumpSquatting:
                bool groundJumpSquattingCounterShouldIncrease = false;

                if (jumpButton)
                {
                    framesJumpButtonPressed++;
                }

                if (!airdashButtonStatePreviousFrame && airdashButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airdashing;
                }
                else if (groundJumpSquattingCounter == 5)
                {
                    jumpsquatStartHorizontalInput = horizontalInput;
                    previousPlayerState = playerState;
                    playerState = PlayerState.Jumping;
                }
                else
                {
                    groundJumpSquattingCounterShouldIncrease = true;
                }

                if (groundJumpSquattingCounterShouldIncrease)
                {
                    groundJumpSquattingCounter++;
                }
                else
                {
                    groundJumpSquattingCounter = 0;
                }

                break;
            #endregion
            #region GROUND_WALKING
            case PlayerState.GroundWalking:

                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                }

                if (horizontalInput > walkZone || horizontalInput < -walkZone)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundDashing;
                }

                if ((horizontalInput < deadZone && horizontalInput > -deadZone))
                {
                    groundWalkingIdleCounter++;
                    if (groundWalkingIdleCounter == 5)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundIdling;
                        groundWalkingIdleCounter = 0;
                    }
                }
                else
                {
                    groundWalkingIdleCounter = 0;
                }

                break;
            #endregion
            #region GROUND_DASHING
            case PlayerState.GroundDashing:

                bool groundDashingCounterShouldIncrease = true;
                if (isOnGround && previousPlayerState != PlayerState.Airborne)
                {
                    if (!jumpButtonStatePreviousFrame && jumpButton)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundJumpSquatting;
                        groundDashingCounterShouldIncrease = false;
                    }
                    else if (groundDashingCounter == dashLength)
                    {
                        if ((horizontalInput > walkZone || horizontalInput < -walkZone))
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundRunning;
                        }

                        if ((horizontalInput < deadZone && horizontalInput > -deadZone))
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundIdling;
                        }
                        groundDashingCounterShouldIncrease = false;
                    }

                    if ((previousHorizontalInput > deadZone && horizontalInput < -deadZone) || (previousHorizontalInput < -deadZone && horizontalInput > deadZone))
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundDashing;
                        groundDashingCounterShouldIncrease = false;
                    }

                    if (groundDashingCounterShouldIncrease)
                    {
                        groundDashingCounter++;
                    }
                    else
                    {
                        groundDashingCounter = 0;
                    }
                }
                else if (isOnGround && previousPlayerState == PlayerState.Airborne)
                {
                    playerState = PlayerState.GroundRunning;
                }
                else if (!isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airborne;
                    groundDashingCounter = 0;
                }
                break;
            #endregion
            #region GROUND_RUNNING
            case PlayerState.GroundRunning:
                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                }
                else if ((horizontalInput < deadZone && horizontalInput > -deadZone))
                {
                    groundRunningIdleCounter++;
                    if (groundRunningIdleCounter == 5)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundIdling;
                        groundRunningIdleCounter = 0;
                    }
                }
                else
                {
                    groundRunningIdleCounter = 0;
                }

                if (!isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airborne;
                }

                break;
            #endregion
            #region AIRDASHING
            case PlayerState.Airdashing:

                if (airdashCounter == airdashTime)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airborne;
                }

                if (isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundIdling;
                }

                if (playerState != PlayerState.Airdashing)
                {
                    airdashCounter = 0;
                    playerRb.gravityScale = 1;
                }
                else
                    airdashCounter++;

                break;
            #endregion
            #region JUMPING
            case PlayerState.Jumping:

                previousPlayerState = playerState;
                playerState = PlayerState.Airborne;

                break;
            #endregion
            #region AIRBORNE
            case PlayerState.Airborne:

                if ((!airdashButtonStatePreviousFrame && airdashButton) && hasAirDash)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airdashing;
                    hasAirDash = false;
                }

                if ((!jumpButtonStatePreviousFrame && jumpButton) && hasAirJump)
                {
                    jumpStartHorizontalInput = horizontalInput;
                    previousPlayerState = playerState;
                    playerState = PlayerState.Jumping;
                    hasAirJump = false;
                }

                if (isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundIdling;
                }
                airborneCounter++;
                if (playerState != PlayerState.Airborne)
                {
                    airborneCounter = 0;
                }
                break;
            #endregion
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
}
