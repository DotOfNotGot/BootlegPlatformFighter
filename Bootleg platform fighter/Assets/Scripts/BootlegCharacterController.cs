using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BootlegPlatformFighter
{
    public class BootlegCharacterController : MonoBehaviour
    {
        public int playerIndex;

        public struct Controls
        {
            public float horizontalInput;
            public float verticalInput;
            public bool jumpButton;
            public bool airdashButton;
            public float airdashAxis;

            // State change variables
            public bool jumpButtonPressed;
            public bool airdashButtonPressed;
            public void SetStateChangeVariables(Controls previousControls)
            {
                jumpButtonPressed = !previousControls.jumpButton && jumpButton;
                airdashButtonPressed = !previousControls.airdashButton && airdashButton;
            }
        }

        // Constant values
        private const float deadZone = 0.1f;
        [SerializeField] private bool isInHorizontalDeadZone;
        [SerializeField] private bool isInVerticalDeadZone;
        private bool previousIsInHorizontalDeadZone;
        private bool previousIsInVerticalDeadZone;
        private const float walkZone = 0.5f;

        // Character specific values
        [SerializeField] private float speed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float shortHopForce;
        [SerializeField] private float doubleJumpForce;
        [SerializeField] private float gravityModifier;
        [SerializeField] private float fallSpeed;
        [SerializeField] private float maxFallSpeed;
        [SerializeField] private float fastFallSpeed;
        [SerializeField] private float maxFastFallSpeed;
        [SerializeField] private int dashLength;
        [SerializeField] private float airControl;


        // State variables
        public PlayerState playerState;
        public PlayerState previousPlayerState;
        private Animator characterAnimation;

        public enum PlayerState
        {
            GroundIdling,
            GroundJumpSquatting,
            GroundWalking,
            GroundDashing,
            GroundRunning,
            GroundCrouching,
            GroundBlocking,
            Airdashing,
            Jumping,
            Airborne,
            LandingLag,
            HitStun,
            BlockStun,
            Tumble

        }

        public LayerMask groundLayer;

        private int groundIdlingWalkCounter = 0;
        private int groundJumpSquattingCounter = 0;
        private int framesJumpButtonPressed = 0;
        private int groundWalkingIdleCounter = 0;
        private int groundRunningIdleCounter = 0;
        public int groundDashingCounter = 0;
        private int airdashCounter = 0;
        private int airborneCounter = 0;
        private int landingLagCounter = 0;

        [SerializeField] private float debugAxis;

        private Controls previousControls;

        private Rigidbody2D playerRb;
        private BoxCollider2D playerCollider;
        private int airdashTime = 10;
        private float airdashForce = 30.0f;

        private float jumpSquatStartVelocity;
        private float velocityXNew;
        private float velocityYOld;

        public float jumpHorizontalVelocityModifier;
        public float airJumpHorizontalVelocityModifier;

        public Vector2 moveVector;
        [SerializeField] private float angle;
        private float airborneTrajectory;
        private float airdashStartHorizontalInput;
        private float airdashStartVerticalInput;
        [SerializeField] private float dashStartHorizontalInput;
        

        // Bools for checking states
        public bool isOnGround;
        public bool hasAirJump;
        public bool hasAirDash;
        public bool isFastFalling;
        public bool isFacingLeft;

        public bool debugPlayerColissionOff;

        // Start is called before the first frame update
        void Start()
        {
            characterAnimation = GetComponent<Animator>();
            playerCollider = GetComponent<BoxCollider2D>();
            playerRb = GetComponent<Rigidbody2D>();
            playerRb.gravityScale *= gravityModifier;
        }

        public void ProcessUpdate(Controls controls)
        {
            controls.SetStateChangeVariables(previousControls);
            moveVector.x = controls.horizontalInput;
            moveVector.y = controls.verticalInput;

            angle = Vector2.Angle(moveVector, Vector2.right);

            if (controls.horizontalInput < -deadZone && isOnGround)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                isFacingLeft = true;
            }
            else if (controls.horizontalInput > deadZone && isOnGround)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);

                isFacingLeft = false;
            }

            if (controls.horizontalInput < deadZone && controls.horizontalInput > -deadZone)
            {
                isInHorizontalDeadZone = true;
            }
            else
            {
                isInHorizontalDeadZone = false;
            }

            if (controls.verticalInput > -deadZone)
            {
                isInVerticalDeadZone = true;
            }
            else
            {
                isInVerticalDeadZone = false;
            }

            // Handles state changes
            GroundCheck();
            PlayerCollisionCheck();
            UpdatePlayerState(controls);
            HandlePlayerState(controls);

            // Store current input for next frame.
            previousControls = controls;
            previousIsInHorizontalDeadZone = isInHorizontalDeadZone;
            previousIsInVerticalDeadZone = isInVerticalDeadZone;
        }

        private void UpdatePlayerState(Controls controls)
        {
            // State machine, including all states in regions.
            switch (playerState)
            {
                #region GROUND_IDLING
                case PlayerState.GroundIdling:

                    bool groundIdlingWalkCounterShouldIncrease = false;

                    // Changes state to GroundCrouching.
                    if (controls.verticalInput < -deadZone)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundCrouching;
                    }

                    // Changes state to GroundDashing.
                    if ((controls.horizontalInput > walkZone || controls.horizontalInput < -walkZone) && (previousIsInHorizontalDeadZone))
                    {

                        if (previousPlayerState != PlayerState.LandingLag)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundDashing;
                        }
                        else
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundRunning;
                        }

                    }
                    // Changes state to GroundWalking.
                    else if (!isInHorizontalDeadZone)
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
                    if (controls.jumpButtonPressed)
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

                    if (!isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                    }

                    break;
                #endregion
                #region GROUND_JUMPSQUATTING
                case PlayerState.GroundJumpSquatting:
                    bool groundJumpSquattingCounterShouldIncrease = false;

                    if (groundJumpSquattingCounter == 0)
                    {
                        jumpSquatStartVelocity = playerRb.velocity.x;
                    }

                    // Checks how many frames the jump button was pressed, gonna be used for differentiating shorthopping and normal jumps later.
                    if (controls.jumpButton)
                    {
                        framesJumpButtonPressed++;
                    }

                    // Changes state to airdashing
                    if ((controls.airdashButtonPressed || controls.airdashAxis > 0) && hasAirDash)
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


                    // Changes state to GroundCrouching.
                    if (controls.verticalInput < -deadZone)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundCrouching;
                    }

                    // Changes state to GroundJumpSquatting.
                    if (controls.jumpButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundJumpSquatting;
                    }

                    // Changes state to GroundIdling.
                    if (isInHorizontalDeadZone)
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

                    if (!isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                    }

                    break;
                #endregion
                #region GROUND_DASHING
                case PlayerState.GroundDashing:

                    characterAnimation.SetBool("isDashing", true);


                    if (groundDashingCounter == 0)
                    {
                        dashStartHorizontalInput = controls.horizontalInput;
                    }

                    // Changes state to GroundJumpSquatting
                    if (controls.jumpButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundJumpSquatting;
                    }

                    else if (groundDashingCounter == dashLength)
                    {
                        // Changes state to GroundRunning if dashTimer reaches max.
                        if (controls.horizontalInput > walkZone || controls.horizontalInput < -walkZone)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundRunning;
                        }
                        // Changes state to GroundIdling if dashTimer reaches max.
                        else
                        {
                            previousPlayerState = playerState;

                            playerState = PlayerState.GroundIdling;
                        }
                    }

                    // If GroundDashingCounter hasn't reached dashLength yet and a dash is inputted in opposite direction, then dash that direction. THIS IS ALSO A THING IM CHANGING SO IT WORKS PROPERLY
                    if ((dashStartHorizontalInput > walkZone && controls.horizontalInput < -walkZone) || (dashStartHorizontalInput < -walkZone && controls.horizontalInput > walkZone))
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
                        dashStartHorizontalInput = 0;
                    }
                    else
                    {
                        if (groundDashingCounter == 0)
                        {
                            dashStartHorizontalInput = controls.horizontalInput;
                        }
                        groundDashingCounter++;
                    }



                    break;
                #endregion
                #region GROUND_RUNNING
                case PlayerState.GroundRunning:

                    characterAnimation.SetBool("isRunning", true);

                    // Changes state to GroundCrouching.
                    if (controls.verticalInput < -deadZone)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundCrouching;
                    }

                    // Changes state to GroundJumpSquatting
                    if (controls.jumpButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundJumpSquatting;
                    }
                    // Changes state to GroundIdling
                    else if (isInHorizontalDeadZone)
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
                #region GROUND_CROUCHING
                case PlayerState.GroundCrouching:

                    characterAnimation.SetBool("isCrouching", true);

                    if (controls.jumpButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundJumpSquatting;
                    }

                    if (controls.verticalInput > -deadZone && isInHorizontalDeadZone)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundIdling;
                    }
                    else if (controls.verticalInput > -deadZone && !isInHorizontalDeadZone && previousIsInHorizontalDeadZone)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundDashing;
                    }
                    else if (controls.verticalInput > -deadZone && !isInHorizontalDeadZone && !previousIsInHorizontalDeadZone)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundWalking;
                    }

                    if (!isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                    }

                    break;
                #endregion
                #region GROUND_BLOCKING
                case PlayerState.GroundBlocking:
                    break;
                #endregion
                #region AIRDASHING
                case PlayerState.Airdashing:

                    hasAirDash = false;

                    // Sets playerState to Airborne if airdashCounter has reached airdashTime.
                    if (airdashCounter == airdashTime)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                    }

                    // Sets playerState to GroundIdling if player is on ground.
                    if (isOnGround && airdashCounter > 5)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
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
                    if ((controls.airdashButtonPressed || controls.airdashAxis > 0) && hasAirDash)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airdashing;
                    }
                    // Goes into airborne state.
                    else
                    {
                        previousPlayerState = playerState;
                        airborneTrajectory = controls.horizontalInput;
                        playerState = PlayerState.Airborne;
                    }

                    break;
                #endregion
                #region AIRBORNE
                case PlayerState.Airborne:


                    // Airdash state check
                    if ((controls.airdashButtonPressed || controls.airdashAxis > 0) && hasAirDash)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airdashing;
                    }

                    // Jump state check(Since current state airborne it checks for airjump)
                    if ((!previousControls.jumpButton && controls.jumpButton) && hasAirJump)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Jumping;
                        hasAirJump = false;
                    }

                    // GroundIdling state check
                    if (isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
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
                #region LANDINGLAG
                case PlayerState.LandingLag:

                    landingLagCounter++;
                    if (landingLagCounter > 4)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundIdling;
                    }

                    break;
                #endregion
                #region HITSTUN
                case PlayerState.HitStun:
                    break;
                #endregion
                #region BLOCKSTUN
                case PlayerState.BlockStun:
                    break;
                #endregion
                #region TUMBLE
                case PlayerState.Tumble:
                    break;
                #endregion
                default:
                    break;
            }


            characterAnimation.SetFloat("PlayerVelocityX", playerRb.velocity.x);

            if (playerState != PlayerState.GroundDashing)
            {
                characterAnimation.SetBool("isDashing", false);
            }
            if (playerState != PlayerState.GroundRunning)
            {
                characterAnimation.SetBool("isRunning", false);
            }
            if (playerState != PlayerState.GroundCrouching)
            {
                characterAnimation.SetBool("isCrouching", false);
            }
        }

        private void HandlePlayerState(Controls controls)
        {
            switch (playerState)
            {
                #region GROUND_IDLING
                case PlayerState.GroundIdling:
                    break;
                #endregion
                #region GROUND_JUMPSQUATTING
                case PlayerState.GroundJumpSquatting:
                    break;
                #endregion
                #region GROUND_WALKING
                case PlayerState.GroundWalking:

                    playerRb.velocity = new Vector2(controls.horizontalInput * speed * 0.75f, playerRb.velocity.y);

                    break;
                #endregion
                #region GROUND_DASHING
                case PlayerState.GroundDashing:

                    playerRb.velocity = new Vector2(dashStartHorizontalInput, playerRb.velocity.y).normalized * speed;

                    break;
                #endregion
                #region GROUND_RUNNING
                case PlayerState.GroundRunning:

                    playerRb.velocity = new Vector2(controls.horizontalInput * speed, playerRb.velocity.y);

                    break;
                #endregion
                #region GROUND_CROUCHING
                case PlayerState.GroundCrouching:
                    break;
                #endregion
                #region GROUND_BLOCKING
                case PlayerState.GroundBlocking:
                    break;
                #endregion
                #region AIRDASHING
                case PlayerState.Airdashing:

                    if (airdashCounter == 0)
                    {
                        playerRb.gravityScale = 0;
                        airdashStartHorizontalInput = controls.horizontalInput;
                        airdashStartVerticalInput = controls.verticalInput;

                        Vector2 airdashDirection = new Vector2(airdashStartHorizontalInput, airdashStartVerticalInput).normalized;

                        playerRb.velocity = airdashDirection * airdashForce;
                    }

                    break;
                #endregion
                #region JUMPING
                case PlayerState.Jumping:

                    if (previousPlayerState == PlayerState.GroundJumpSquatting)
                    {
                        if (framesJumpButtonPressed <= 3)
                        {
                            playerRb.velocity = new Vector2(jumpSquatStartVelocity * jumpHorizontalVelocityModifier, shortHopForce);
                            framesJumpButtonPressed = 0;
                        }
                        else if (framesJumpButtonPressed > 3)
                        {
                            playerRb.velocity = new Vector2(jumpSquatStartVelocity * jumpHorizontalVelocityModifier, jumpForce);
                            framesJumpButtonPressed = 0;
                        }

                    }
                    else if (!hasAirJump)
                    {
                        velocityXNew = (controls.horizontalInput * 0.5f) * airControl;
                        playerRb.velocity = new Vector2(velocityXNew * airJumpHorizontalVelocityModifier, doubleJumpForce);
                    }

                    break;
                #endregion
                #region AIRBORNE
                case PlayerState.Airborne:

                    // If the first frame of airborne and the previous state was airdash then reset velocity to 0
                    if (airborneCounter == 0 && previousPlayerState == PlayerState.Airdashing)
                    {
                        playerRb.velocity = new Vector2(0, 0);
                    }

                    velocityXNew = Mathf.Clamp(playerRb.velocity.x + controls.horizontalInput * airControl, -20, 20);

                    if (controls.verticalInput < -0.3 && playerRb.velocity.y < 0 && (previousIsInVerticalDeadZone || isFastFalling))
                    {
                        isFastFalling = true;
                        velocityYOld = playerRb.velocity.y;
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * 2, -maxFastFallSpeed, maxFastFallSpeed));
                    }
                    else if (playerRb.velocity.y < 0)
                    {
                        velocityYOld = playerRb.velocity.y;
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * fallSpeed, -maxFallSpeed, maxFallSpeed));
                    }

                    if (isInVerticalDeadZone)
                    {
                        isFastFalling = false;
                    }


                    playerRb.velocity = new Vector2(velocityXNew, playerRb.velocity.y);

                    break;
                #endregion
                #region LANDINGLAG
                case PlayerState.LandingLag:
                    break;
                #endregion
                #region HITSTUN
                case PlayerState.HitStun:
                    break;
                #endregion
                #region BLOCKSTUN
                case PlayerState.BlockStun:
                    break;
                #endregion
                #region TUMBLE
                case PlayerState.Tumble:
                    break;
                #endregion
                default:
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void GroundCheck()
        {

            var contactPoints = new List<ContactPoint2D>();
            playerCollider.GetContacts(contactPoints);
            isOnGround = false;
            foreach (var contactPoint in contactPoints)
            {
                if (contactPoint.normal == Vector2.up)
                {
                    isOnGround = true;
                    hasAirDash = true;
                    hasAirJump = true;
                    return;
                }
            }
        }

        private void PlayerCollisionCheck()
        {
            if ((playerRb.velocity.x > 10 || playerRb.velocity.x < -10) || playerRb.velocity.y != 0 || debugPlayerColissionOff)
            {
                Physics2D.IgnoreLayerCollision(6, 6, true);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(6, 6, false);
            }
        }
    }
}
