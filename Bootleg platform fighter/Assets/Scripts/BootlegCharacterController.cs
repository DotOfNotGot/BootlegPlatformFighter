using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using DG.Tweening;

namespace BootlegPlatformFighter
{
    public class BootlegCharacterController : MonoBehaviour
    {
        //public ParticleSystem dust;
        public int characterIndex;

        public int GetPlayerIndex()
        {
            return characterIndex;
        }

        public struct Controls
        {
            public float movementHorizontalInput;
            public float movementVerticalInput;
            public float macroHorizontalInput;
            public float macroVerticalInput;
            public bool jumpButton;
            public bool airdashButton;
            public bool normalAttackButton;
            public bool specialAttackButton;
            public bool grabButton;
            public bool pauseButton;
            public bool jumpButtonPressed;
            public bool airdashButtonPressed;
            public bool normalAttackButtonPressed;
            public bool specialAttackButtonPressed;
            public bool grabButtonPressed;
            public bool pauseButtonPressed;

            public void SetStateChangeVariables(Controls previousControls)
            {
                jumpButtonPressed = !previousControls.jumpButton && jumpButton;
                airdashButtonPressed = !previousControls.airdashButton && airdashButton;
                normalAttackButtonPressed = !previousControls.normalAttackButton && normalAttackButton;
                specialAttackButtonPressed = !previousControls.specialAttackButton && specialAttackButton;
                grabButtonPressed = !previousControls.grabButton && grabButton;
                pauseButtonPressed = !previousControls.pauseButton && pauseButton;
            }
            
        }

        Controls controls;
        Controls previousControls;
        public void SetNewControls(Controls newControls, Controls newPreviousControls)
        {

            controls = newControls;
            previousControls = newPreviousControls;

        }



        private GameManager gameManager;
        [SerializeField] private Animator crossFadeAnimator;

        // Constant values
        private const float deadZone = 0.2f;
        [SerializeField] private bool isInHorizontalDeadZone;
        [SerializeField] private bool isInVerticalDeadZone;
        [SerializeField] private bool isInWalkZone;
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
        [SerializeField] private int dashLength;
        [SerializeField] private float airControl;

        [SerializeField] GameObject smallPlatform;
        [SerializeField] int goDownIndexMax;

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
            BlockBreak,
            Airdashing,
            Jumping,
            Airborne,
            LandingLag,
            TumbleLand,
            HitStun,
            BlockStun,
            Tumble,
            Grabbed,
            Jab,
            ForwardTilt,
            UpTilt,
            DownTilt,
            ForwardStrong,
            UpStrong,
            DownStrong,
            NeutralAir,
            ForwardAir,
            UpAir,
            DownAir,
            NeutralSpecial,
            SideSpecial,
            UpSpecial,
            DownSpecial,
            Grab,
            UpThrow,
            DownThrow,
            BackThrow,
            ForwardThrow
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
        private int tumbleLandCounter = 0;
        private int crouchParryCounter = 0;
        private int tumbleTimeCounter = 0;


        [SerializeField] private float debugAxis;


        private Rigidbody2D playerRb;
        private BoxCollider2D playerCollider;
        [SerializeField] private int airdashTime = 10;
        [SerializeField] private float airdashForce = 30.0f;

        private float jumpSquatStartVelocity;
        private float velocityXNew;


        public float jumpHorizontalVelocityModifier;
        public float airJumpHorizontalVelocityModifier;

        public Vector2 moveVector;
        [SerializeField] private float angle;
        private float airdashStartHorizontalInput;
        private float airdashStartVerticalInput;
        [SerializeField] private float dashStartHorizontalInput;


        // Bools for checking states
        public bool isOnGround;
        public bool hasAirJump;
        public bool hasAirDash;
        public bool isFastFalling;
        public bool isFacingLeft;
        public bool canMove;
        public bool isParrying;

        public bool canBeHit = true;
        private int invulnerableTimer = -1;

        public bool debugPlayerColissionOff;

        public UnityEvent grabEvent;

        public float damageTakenPercent = 0.0f;

        private AnimationHandler animationHandler;
        private GameObject mainObject;

        [SerializeField] private int blockTimerDefault = 10;
        private int blockTimer;

        [SerializeField] private int blockBreakTimerDefault = 10;
        private int blockBreakTimer;


        // Start is called before the first frame update
        void Start()
        {
            characterAnimation = GetComponentInChildren<Animator>();
            playerCollider = GetComponentInChildren<BoxCollider2D>();
            playerRb = GetComponent<Rigidbody2D>();
            playerRb.gravityScale *= gravityModifier;
            gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
            mainObject = transform.GetChild(0).gameObject;
            animationHandler = mainObject.GetComponent<AnimationHandler>();
            blockTimer = blockTimerDefault;

            // you can use mainObject as first param - akandesh
            gameManager.InitializeCameraTargets(transform.GetChild(0).gameObject, characterIndex);
        }

        private void FixedUpdate()
        {
            if (!canBeHit)
            {
                if (invulnerableTimer > 0)
                {
                    invulnerableTimer--;
                }
                else if (invulnerableTimer == 0)
                {
                    SetInvulnerable(true);
                }
            }
            moveVector.x = controls.movementHorizontalInput;
            moveVector.y = controls.movementVerticalInput;

            angle = Vector2.Angle(moveVector, Vector2.right);

            if (controls.movementHorizontalInput < walkZone && controls.movementHorizontalInput > -walkZone)
            {
                isInWalkZone = true;
            }
            else
            {
                isInWalkZone = false;
            }

            if (controls.movementHorizontalInput < deadZone && controls.movementHorizontalInput > -deadZone)
            {
                isInHorizontalDeadZone = true;
            }
            else
            {
                isInHorizontalDeadZone = false;
            }

            if (controls.movementVerticalInput > -deadZone)
            {
                isInVerticalDeadZone = true;
            }
            else
            {
                isInVerticalDeadZone = false;
            }

            if (!isInVerticalDeadZone)
            {
                IgnoreSmallPlatforms(true);
            }
            else
            {
                IgnoreSmallPlatforms(false);
            }

            // Handles state changes
            GroundCheck();
            PlayerCollisionCheck();
            UpdatePlayerState(controls, previousControls);
            HandlePlayerState(controls);

            if (playerState == PlayerState.GroundCrouching)
            {
                BlockCountdown();
            }
            else if (blockTimer < blockTimerDefault)
            {
                BlockCountUp();
            }

            if (playerState != PlayerState.Airdashing)
            {
                playerRb.gravityScale = 1 * gravityModifier;
            }

            if (playerState != PlayerState.Tumble && previousPlayerState == PlayerState.Tumble)
            {
                if (isFacingLeft)
                {
                    transform.GetChild(0).rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
                }
            }

            // Store current input for next frame.
            previousIsInHorizontalDeadZone = isInHorizontalDeadZone;
            previousIsInVerticalDeadZone = isInVerticalDeadZone;
        }

        private void UpdatePlayerState(Controls controls, Controls previousControls)
        {

            // State machine, including all states in regions.
            switch (playerState)
            {
                #region GROUND_IDLING
                case PlayerState.GroundIdling:

                    bool groundIdlingWalkCounterShouldIncrease = false;

                    // Changes state to ForwardStrong
                    if (controls.specialAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.ForwardStrong;
                    }

                    // Changes state to Jab
                    if (controls.normalAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Jab;
                    }


                    // Changes state to GroundCrouching.
                    if (controls.movementVerticalInput < -deadZone && blockTimer >= (blockTimerDefault / 4))
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundCrouching;
                        
                    }

                    // Changes state to GroundDashing.
                    if ((controls.movementHorizontalInput > walkZone || controls.movementHorizontalInput < -walkZone) && (previousIsInHorizontalDeadZone))
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundDashing;
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
                    if (controls.airdashButtonPressed && hasAirDash)
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


                    // Changes state to Jab
                    if (controls.normalAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Jab;
                    }

                    // Changes state to ForwardStrong.
                    if (controls.specialAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.ForwardStrong;
                    }

                    // Changes state to GroundCrouching.
                    if (controls.movementVerticalInput < -deadZone && blockTimer >= (blockTimerDefault / 4))
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

                    if ((previousControls.movementHorizontalInput > walkZone && controls.movementHorizontalInput < -walkZone) || (previousControls.movementHorizontalInput < -walkZone && controls.movementHorizontalInput > walkZone))
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.GroundDashing;
                        groundDashingCounter = 0;
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

                    characterAnimation.SetInteger("DashTimer", groundDashingCounter);

                    if (groundDashingCounter == 0)
                    {
                        dashStartHorizontalInput = controls.movementHorizontalInput;

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
                        if (controls.movementHorizontalInput > walkZone || controls.movementHorizontalInput < -walkZone)
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
                    // If GroundDashingCounter hasn't reached dashLength yet and a dash is inputted in opposite direction, then dash that direction.
                    else if ((dashStartHorizontalInput > walkZone && controls.movementHorizontalInput < -walkZone) || (dashStartHorizontalInput < -walkZone && controls.movementHorizontalInput > walkZone))
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
                            dashStartHorizontalInput = controls.movementHorizontalInput;
                        }
                        groundDashingCounter++;
                    }


                    break;
                #endregion
                #region GROUND_RUNNING
                case PlayerState.GroundRunning:

                    // Changes state to GroundCrouching.
                    if (controls.movementVerticalInput < -deadZone && blockTimer >= (blockTimerDefault / 4))
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
                    else if (isInWalkZone)
                    {
                        groundRunningIdleCounter++;
                        if (groundRunningIdleCounter == 5)
                        {
                            previousPlayerState = playerState;
                            if (isInHorizontalDeadZone)
                            {
                                playerState = PlayerState.GroundIdling;
                            }
                            else
                            {
                                playerState = PlayerState.GroundWalking;
                            }

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

                    // Changes state to Jab
                    if (controls.normalAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Jab;
                    }

                    // Changes state to ForwardStrong
                    if (controls.specialAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.ForwardStrong;
                    }

                    break;
                #endregion
                #region GROUND_CROUCHING
                case PlayerState.GroundCrouching:

                    if (blockTimer == 0)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.BlockBreak;
                        characterAnimation.SetBool("isCrouching", false);

                    }

                    if (crouchParryCounter <= 2)
                    {
                        isParrying = true;
                    }
                    else if (crouchParryCounter > 2)
                    {
                        // Changes state to GroundBlocking
                        if (controls.airdashButton)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundBlocking;
                        }

                        if (controls.jumpButtonPressed)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundJumpSquatting;
                        }

                        if (controls.movementVerticalInput > -deadZone && isInHorizontalDeadZone)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundIdling;
                        }
                        else if (controls.movementVerticalInput > -deadZone && !isInHorizontalDeadZone && previousIsInHorizontalDeadZone)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundDashing;
                        }
                        else if (controls.movementVerticalInput > -deadZone && !isInHorizontalDeadZone && !previousIsInHorizontalDeadZone)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.GroundWalking;
                        }

                        if (!isOnGround)
                        {
                            previousPlayerState = playerState;
                            playerState = PlayerState.Airborne;
                        }
                        isParrying = false;
                    }

                    if (playerState != PlayerState.GroundCrouching)
                    {
                        crouchParryCounter = 0;
                    }
                    else
                    {
                        crouchParryCounter++;
                    }

                    break;
                #endregion
                #region BLOCKBREAK
                case PlayerState.BlockBreak:

                    if (blockBreakTimer == blockBreakTimerDefault)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
                        blockBreakTimer = 0;
                        canMove = true;
                    }
                    else
                    {
                        blockBreakTimer++;
                    }

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
                    if (isOnGround && airdashCounter > 1)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
                    }

                    if (controls.normalAttackButtonPressed)
                    {
                        previousPlayerState = playerState;

                        playerState = PlayerState.NeutralAir;

                    }
                    if (controls.specialAttackButtonPressed)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.ForwardAir;
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
                    if ((controls.airdashButtonPressed) && hasAirDash)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airdashing;
                    }
                    // Goes into airborne state.
                    else
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                    }


                    break;
                #endregion
                #region AIRBORNE
                case PlayerState.Airborne:


                    // Airdash state check
                    if ((controls.airdashButtonPressed) && hasAirDash)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airdashing;
                    }

                    // Jump state check(Since current state airborne it checks for airjump)
                    if (controls.jumpButtonPressed && hasAirJump)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Jumping;
                        hasAirJump = false;
                        if (playerRb.velocity.y > 1)
                        {
                            characterAnimation.Play("Huldra_JumpPeak");
                            //animationHandler.CancelAnimation("Huldra_Jump");

                        }
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
                    if (controls.normalAttackButtonPressed)
                    {
                        previousPlayerState = playerState;

                        playerState = PlayerState.NeutralAir;

                    }

                    if (controls.specialAttackButtonPressed)
                    {
                        previousPlayerState = playerState;

                        playerState = PlayerState.ForwardAir;
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
                        landingLagCounter = 0;
                    }

                    break;
                #endregion
                #region TumbleLand
                case PlayerState.TumbleLand:

                    tumbleLandCounter++;
                    if (tumbleLandCounter > 20)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
                        tumbleLandCounter = 0;
                    }

                    break;
                #endregion
                #region TUMBLE
                case PlayerState.Tumble:

                    if (isOnGround && tumbleTimeCounter > 10)
                    {
                        tumbleTimeCounter = 0;
                        previousPlayerState = playerState;
                        playerState = PlayerState.TumbleLand;
                    }
                    else if (tumbleTimeCounter > 30)
                    {
                        tumbleTimeCounter = 0;
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                    }
                    tumbleTimeCounter++;
                    break;
                #endregion
                #region JAB
                case PlayerState.Jab:

                    if (!isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                        animationHandler.CancelAnimation("Huldra_Fall");
                    }

                    break;
                #endregion
                #region FORWARDSTRONG
                case PlayerState.ForwardStrong:

                    if (!isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.Airborne;
                        animationHandler.CancelAnimation("Huldra_Fall");
                    }

                    break;
                #endregion
                #region NEUTRALAIR
                case PlayerState.NeutralAir:

                    if (isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
                        animationHandler.CancelAnimation("Huldra_LandingLag");
                    }

                    break;
                #endregion
                #region FORWARDAIR
                case PlayerState.ForwardAir:

                    if (isOnGround)
                    {
                        previousPlayerState = playerState;
                        playerState = PlayerState.LandingLag;
                        animationHandler.CancelAnimation("Huldra_LandingLag");
                    }

                    break;
                #endregion
                default:
                    break;
            }

            characterAnimation.SetFloat("PlayerVelocityX", playerRb.velocity.x);
            characterAnimation.SetFloat("PlayerVelocityY", playerRb.velocity.y);

            if (playerState != PlayerState.GroundIdling)
            {
                characterAnimation.SetBool("isIdling", false);
            }

            if (playerState != PlayerState.TumbleLand)
            {
                characterAnimation.SetBool("isTumbleLanding", false);
            }

            if (playerState != PlayerState.Tumble)
            {
                characterAnimation.SetBool("isTumbleing", false);
            }

            if (playerState != PlayerState.HitStun)
            {
                characterAnimation.SetBool("isHitStunning", false);
            }

            if (playerState != PlayerState.GroundDashing)
            {
                characterAnimation.SetBool("isDashing", false);
            }

            if (playerState != PlayerState.GroundRunning)
            {
                characterAnimation.SetBool("isRunning", false);
            }

            if (playerState != PlayerState.GroundCrouching && !isParrying)
            {
                characterAnimation.SetBool("isCrouching", false);
            }

            if (playerState != PlayerState.GroundWalking)
            {
                characterAnimation.SetBool("isWalking", false);
            }

            if (playerState != PlayerState.GroundJumpSquatting)
            {
                characterAnimation.SetBool("isJumpSquatting", false);
            }

            if (playerState != PlayerState.Airdashing)
            {
                characterAnimation.SetBool("isAirDashing", false);
            }

            if (playerState != PlayerState.BlockBreak)
            {
                characterAnimation.SetBool("isBlockBreaking", false);
            }

            if (playerState != PlayerState.Jab)
            {
                characterAnimation.SetBool("isJabing", false);
            }

            if (playerState != PlayerState.ForwardStrong)
            {
                characterAnimation.SetBool("isForwardStronging", false);
            }

            if (playerState != PlayerState.NeutralAir)
            {
                characterAnimation.SetBool("isNeutralAiring", false);
            }
            if (playerState != PlayerState.ForwardAir)
            {
                characterAnimation.SetBool("isForwardAiring", false);
            }
        }

        private void HandlePlayerState(Controls controls)
        {
            switch (playerState)
            {
                #region GROUND_IDLING
                case PlayerState.GroundIdling:

                    characterAnimation.SetBool("isIdling", true);

                    break;
                #endregion
                #region GROUND_JUMPSQUATTING
                case PlayerState.GroundJumpSquatting:

                    characterAnimation.SetBool("isJumpSquatting", true);

                    break;
                #endregion
                #region GROUND_WALKING
                case PlayerState.GroundWalking:

                    characterAnimation.SetBool("isWalking", true);
                    TurnAround(controls);
                    playerRb.velocity = new Vector2(controls.movementHorizontalInput * speed * 0.5f, playerRb.velocity.y);

                    break;
                #endregion
                #region GROUND_DASHING
                case PlayerState.GroundDashing:

                    characterAnimation.SetBool("isDashing", true);

                    if (groundDashingCounter == 1)
                    {
                        TurnAround(controls);
                    }

                    playerRb.velocity = new Vector2(dashStartHorizontalInput, playerRb.velocity.y).normalized * speed;

                    break;
                #endregion
                #region GROUND_RUNNING
                case PlayerState.GroundRunning:

                    characterAnimation.SetBool("isRunning", true);
                    TurnAround(controls);
                    if (!isInHorizontalDeadZone)
                    {
                        playerRb.velocity = new Vector2(controls.movementHorizontalInput, playerRb.velocity.y).normalized * speed;
                    }

                    break;
                #endregion
                #region GROUND_CROUCHING
                case PlayerState.GroundCrouching:

                    characterAnimation.SetBool("isCrouching", true);
                    

                    break;
                #endregion
                #region BLOCKBREAK
                case PlayerState.BlockBreak:

                    canMove = false;
                    characterAnimation.SetBool("isBlockBreaking", true);

                    break;
                #endregion
                #region AIRDASHING
                case PlayerState.Airdashing:

                    characterAnimation.SetBool("isAirDashing", true);

                    if (airdashCounter == 0)
                    {
                        playerRb.gravityScale = 0;
                        airdashStartHorizontalInput = controls.movementHorizontalInput;
                        airdashStartVerticalInput = controls.movementVerticalInput;

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
                        velocityXNew = (controls.movementHorizontalInput * 0.5f) * airControl;
                        playerRb.velocity = new Vector2(velocityXNew * airJumpHorizontalVelocityModifier, doubleJumpForce);
                    }

                    break;
                #endregion
                #region AIRBORNE
                case PlayerState.Airborne:

                    // If the first frame of airborne and the previous state was airdash then reset velocity to 0
                    if (airborneCounter == 0 && previousPlayerState == PlayerState.Airdashing)
                    {
                        //playerRb.velocity = new Vector2(0, 0);
                    }

                    velocityXNew = Mathf.Clamp(playerRb.velocity.x + controls.movementHorizontalInput * airControl, -20, 20);

                    if (controls.movementVerticalInput < -0.3 && playerRb.velocity.y <= 0 && (previousIsInVerticalDeadZone || isFastFalling))
                    {
                        isFastFalling = true;
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * maxFallSpeed, -maxFallSpeed, maxFallSpeed));
                    }
                    else if (playerRb.velocity.y < 0)
                    {
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * fallSpeed, -maxFallSpeed, maxFallSpeed));
                    }

                    if (isInVerticalDeadZone)
                    {
                        isFastFalling = false;
                    }


                    playerRb.velocity = new Vector2(velocityXNew, playerRb.velocity.y);

                    break;
                #endregion
                #region HITSTUN
                case PlayerState.HitStun:
                    characterAnimation.SetBool("isHitStunning", true);
                    break;
                #endregion
                #region TUMBLE
                case PlayerState.Tumble:
                    characterAnimation.SetBool("isTumbleing", true);
                    if (damageTakenPercent >= 50)
                    {
                        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, Random.Range(15, 360));
                    }
                    break;
                #endregion
                #region TumbleLand
                case PlayerState.TumbleLand:

                    characterAnimation.SetBool("isTumbleLanding", true);

                    break;
                #endregion
                #region JAB
                case PlayerState.Jab:

                    characterAnimation.SetBool("isJabing", true);


                    break;
                #endregion
                #region FORWARDSTRONG
                case PlayerState.ForwardStrong:

                    characterAnimation.SetBool("isForwardStronging", true);

                    break;
                #endregion
                #region NEUTRALAIR
                case PlayerState.NeutralAir:

                    characterAnimation.SetBool("isNeutralAiring", true);

                    velocityXNew = Mathf.Clamp(playerRb.velocity.x + controls.movementHorizontalInput * airControl, -20, 20);

                    if (controls.movementVerticalInput < -0.3 && playerRb.velocity.y <= 0 && (previousIsInVerticalDeadZone || isFastFalling))
                    {
                        isFastFalling = true;
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * maxFallSpeed, -maxFallSpeed, maxFallSpeed));
                    }
                    else if (playerRb.velocity.y < 0)
                    {
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * fallSpeed, -maxFallSpeed, maxFallSpeed));
                    }

                    if (isInVerticalDeadZone)
                    {
                        isFastFalling = false;
                    }

                    break;
                #endregion
                #region FORWARDAIR
                case PlayerState.ForwardAir:

                    characterAnimation.SetBool("isForwardAiring", true);

                    velocityXNew = Mathf.Clamp(playerRb.velocity.x + controls.movementHorizontalInput * airControl, -20, 20);

                    if (controls.movementVerticalInput < -0.3 && playerRb.velocity.y <= 0 && (previousIsInVerticalDeadZone || isFastFalling))
                    {
                        isFastFalling = true;
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * maxFallSpeed, -maxFallSpeed, maxFallSpeed));
                    }
                    else if (playerRb.velocity.y < 0)
                    {
                        playerRb.velocity = new Vector2(velocityXNew, Mathf.Clamp(playerRb.velocity.y * fallSpeed, -maxFallSpeed, maxFallSpeed));
                    }

                    if (isInVerticalDeadZone)
                    {
                        isFastFalling = false;
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
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void IgnoreSmallPlatforms(bool shouldIgnore)
        {
            Physics2D.IgnoreCollision(smallPlatform.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>(), playerCollider, shouldIgnore);
            Physics2D.IgnoreCollision(smallPlatform.transform.GetChild(1).gameObject.GetComponent<BoxCollider2D>(), playerCollider, shouldIgnore);
            
        }

        private void GroundCheck()
        {
            var contactPoints = new List<ContactPoint2D>();
            playerCollider.GetContacts(contactPoints);
            isOnGround = false;
            foreach (var contactPoint in contactPoints)
            {
                if (contactPoint.normal == Vector2.up && playerRb.velocity.y < 1)
                {
                    isOnGround = true;
                    characterAnimation.SetBool("isOnGround", isOnGround);
                    hasAirDash = true;
                    hasAirJump = true;
                    return;
                }
            }
            characterAnimation.SetBool("isOnGround", isOnGround);
        }

        private bool PlayerCollisionCheck()
        {
            if ((playerRb.velocity.x > 10 || playerRb.velocity.x < -10) || playerRb.velocity.y != 0 || debugPlayerColissionOff)
            {
                Physics2D.IgnoreLayerCollision(6, 6, true);
                return true;
            }
            Physics2D.IgnoreLayerCollision(6, 6, false);
            return false;
        }

        public IEnumerator DelayRespawn()
        {
            gameManager.RemoveHeartFromPlayer(characterIndex);
            yield return new WaitForSeconds(3);
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
            gameManager.RespawnPlayer(gameObject, characterIndex);
            damageTakenPercent = 0;
        }

        private IEnumerator GameOverSceneLoader()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("GameOver");
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("Collided with " + collision.transform.name);
            if (collision.transform.name.Contains("DeathZone"))
            {
                for (int i = 0; i < transform.childCount; i++)
                    transform.GetChild(i).gameObject.SetActive(false);
                Instantiate(gameManager.ExplosionPrefab, transform.position, gameManager.ExplosionPrefab.transform.rotation);
                if (gameManager.FindHUDAvatarByIndex(characterIndex).getHealthCount() > 1)
                {
                    StartCoroutine(DelayRespawn());
                }
                else
                {
                    // Player had 1 heart. Now he will have 0.

                    // When adding support for multiple players we need to store index of those eliminated
                    foreach (var t in GameManagerData.Players)
                    {
                        if (t.Key != characterIndex)
                        {
                            GameManagerData.LastWinner = t.Value;
                            break;
                        }
                    }
                    crossFadeAnimator.SetTrigger("Start");
                    StartCoroutine(GameOverSceneLoader());
                }
            }
        }

        void TurnAround(Controls controls)
        {
            if (controls.movementHorizontalInput < -deadZone && isOnGround)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                isFacingLeft = true;
            }
            else if (controls.movementHorizontalInput > deadZone && isOnGround)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                isFacingLeft = false;
            }
        }

        public void CallGrabScript()
        {
            grabEvent.Invoke();
        }

        public void BlockCountdown()
        {
            blockTimer--;
        }

        public void BlockCountUp()
        {
            blockTimer++;
        }


        public void SetInvulnerable(bool isVulnerable, int frames = 0)
        {
            canBeHit = isVulnerable;
            if (!canBeHit)
            {
                invulnerableTimer = frames;
            }
            else
            {
                invulnerableTimer = -1;
            }
        }
       
    }
}
