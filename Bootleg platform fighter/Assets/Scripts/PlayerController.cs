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
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float shortHopForce;
    [SerializeField] private float airControl;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float gravityModifier;
    [SerializeField] private float dashForce;
    [SerializeField] private int dashLength;

    // State variables
    [SerializeField] private PlayerState playerState;
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
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    private bool jumpButton;
    private bool airdashButton;
    private Vector2 moveVector;
    [SerializeField] private float angle;
    private float airborneTrajectory;
    private float jumpStartHorizontalInput;
    private float jumpsquatStartHorizontalInput;
    private float airdashStartHorizontalInput;
    private float airdashStartVerticalInput;

    // Bools for checking states
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool hasAirJump;
    [SerializeField] private bool hasAirDash;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerRb.gravityScale *= gravityModifier;
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


        // Handles physics of jumping state .
        if (playerState == PlayerState.Jumping)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);

            if (previousPlayerState == PlayerState.GroundJumpSquatting)
            {
                playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else if (!hasAirJump)
            {
                playerRb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            }
        }

        // Handles physics and movement of airborne state.
        if (playerState == PlayerState.Airborne)
        {

            // If the first frame of airborne and the previous state was airdash then reset velocity to 0
            if (airborneCounter == 0 && previousPlayerState == PlayerState.Airdashing)
            {
                playerRb.velocity = new Vector2(0, 0);
            }

            if (previousPlayerState == PlayerState.Jumping)
            {
            }

            if (previousPlayerState == PlayerState.Airdashing)
            {
                airborneTrajectory = horizontalInput * airControl;
            }

            transform.Translate((Vector2.right * airborneTrajectory * Time.deltaTime * speed));
        }

        // Handles physics and movement of airdashing state.
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
        }

        // Handles physics and movement of grounddashing state.
        if (playerState == PlayerState.GroundDashing)
        {
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            playerRb.AddForce(Vector2.right * (dashForce * horizontalInput), ForceMode2D.Impulse);
        }

        // Handles movement of groundrunning state.
        if (playerState == PlayerState.GroundRunning || playerState == PlayerState.GroundWalking)
        {
            transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * speed);
        }
    }

    private void UpdatePlayerState()
    {
        // State machine, including all states in regions.
        switch (playerState)
        {
            #region GROUND_IDLING
            case PlayerState.GroundIdling:
                bool groundIdlingWalkCounterShouldIncrease = false;

                // Changes state to GroundDashing.
                if (horizontalInput > walkZone || horizontalInput < -walkZone)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundDashing;
                }
                // Changes state to GroundWalking.
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

                // Changes state to GroundJumpSquatting.
                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                    groundIdlingWalkCounterShouldIncrease = false;
                }

                // Counter for shifting from idle to walk.
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

                // Checks how many frames the jump button was pressed, gonna be used for differentiating shorthopping and normal jumps later.
                if (jumpButton)
                {
                    framesJumpButtonPressed++;
                }

                // Changes state to airdashing
                if (!airdashButtonStatePreviousFrame && airdashButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airdashing;
                }
                // Changes state to jumping(This state is active for 1 frame and only adds the force of the jump).
                else if (groundJumpSquattingCounter == 5)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Jumping;
                }
                // How long the player has been in jumpsquat.
                else
                {
                    groundJumpSquattingCounterShouldIncrease = true;
                }

                // Increases jumpsquatting counter.
                if (groundJumpSquattingCounterShouldIncrease)
                {
                    groundJumpSquattingCounter++;
                }
                // Sets jumpsquatting counter to 0.
                else
                {
                    groundJumpSquattingCounter = 0;
                }

                break;
            #endregion
            #region GROUND_WALKING
            case PlayerState.GroundWalking:

                // Changes state to GroundJumpSquatting.
                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                }

                // Changes state to GroundDashing.
                if (horizontalInput > walkZone || horizontalInput < -walkZone)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundDashing;
                }

                // Changes state to GroundIdling.
                if ((horizontalInput < deadZone && horizontalInput > -deadZone))
                {
                    groundWalkingIdleCounter++;
                    // Checks how long stick has been in deadzone to put player in idle.
                    if (groundWalkingIdleCounter == 5)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundIdling;
                        groundWalkingIdleCounter = 0;
                    }
                }
                // Sets counter to 0 for next time in state.
                else
                {
                    groundWalkingIdleCounter = 0;
                }

                break;
            #endregion
            #region GROUND_DASHING
            case PlayerState.GroundDashing:

                // Changes state to GroundJumpSquatting
                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                }

                else if (groundDashingCounter == dashLength)
                {

                    // Changes state to GroundRunning if dashTimer reaches max.
                    if ((horizontalInput > walkZone || horizontalInput < -walkZone))
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundRunning;
                    }

                    // Changes state to GroundIdling if dashTimer reaches max.
                    if ((horizontalInput < deadZone && horizontalInput > -deadZone))
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundIdling;
                    }
                }

                // If GroundDashingCounter hasn't reached dashLength yet and a dash is inputted in opposite direction, then dash that direction. THIS IS ALSO A THING IM CHANGING SO IT WORKS PROPERLY
                if ((previousHorizontalInput > deadZone && horizontalInput < -deadZone) || (previousHorizontalInput < -deadZone && horizontalInput > deadZone))
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundDashing;
                    groundDashingCounter = 0;
                }

                //Checks if player is still on ground, if not sets playerstate to airborne.
                if (!isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airborne;
                }

                // Checks if the player is still in GroundDashing if not sets groundDashingCounter to 0.
                if (playerState != PlayerState.GroundDashing)
                {
                    groundDashingCounter = 0;
                }
                else
                {
                    groundDashingCounter++;
                }

                
                break;
            #endregion
            #region GROUND_RUNNING
            case PlayerState.GroundRunning:

                // Changes state to GroundJumpSquatting
                if (!jumpButtonStatePreviousFrame && jumpButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundJumpSquatting;
                }
                // Changes state to GroundIdling
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

                // Checks if player is still on ground if not sets playerState to airborne.
                if (!isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airborne;
                }

                break;
            #endregion
            #region AIRDASHING
            case PlayerState.Airdashing:

                // Sets playerState to Airborne if airdashCounter has reached airdashTime.
                if (airdashCounter == airdashTime)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airborne;
                }

                // Sets playerState to GroundIdling if player is on ground.
                if (isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundIdling;
                }

                // Checks if playerState is still airdashing, if true resets gravityscale to normal again. Probably moving this in a bit.
                if (playerState != PlayerState.Airdashing)
                {
                    airdashCounter = 0;
                    playerRb.gravityScale = 1;
                    playerRb.gravityScale *= gravityModifier;
                }
                // Increases airdashCounter.
                else
                {
                    airdashCounter++;
                }

                break;
            #endregion
            #region JUMPING
            case PlayerState.Jumping:

                // Sets playerState to airdashing if button pressed.
                if (!airdashButtonStatePreviousFrame && airdashButton)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airdashing;
                }
                // Goes into airborne state.
                else
                {
                    previousPlayerState = playerState;
                    airborneTrajectory = horizontalInput;
                    playerState = PlayerState.Airborne;
                }

                break;
            #endregion
            #region AIRBORNE
            case PlayerState.Airborne:


                // Airdash state check
                if ((!airdashButtonStatePreviousFrame && airdashButton) && hasAirDash)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Airdashing;
                    hasAirDash = false;
                }

                // Jump state check(Since current state airborne it checks for airjump)
                if ((!jumpButtonStatePreviousFrame && jumpButton) && hasAirJump)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.Jumping;
                    hasAirJump = false;
                }

                // GroundIdling state check
                if (isOnGround)
                {
                    previousPlayerState = playerState;
                    playerState = PlayerState.GroundIdling;
                }

                // Used only to check the first frame so velocity gets reset to 0 after an airdash
                airborneCounter++;

                // Resets airborneCounter after changing state.
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
        // Checks if on ground THIS IS INCREDIBLY POOR, REPLACING WITH RAYCASTING OR SOMETHING LATER
        if (collision.gameObject.CompareTag("Ground") && playerRb.velocity.y == 0)
        {
            isOnGround = true;
            hasAirJump = true;
            hasAirDash = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Checks if not on ground, also getting replaced.
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
}
