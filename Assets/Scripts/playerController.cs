using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour
{
    public PlayerState playerState = PlayerState.FALLING;
    bool moving = false;
    public bool movingLeft = false;
    public bool movingRight = false;
    bool dead = false;
    public SpriteRenderer spriteDisplay;

    public float jumpBurst = 0.1f;
    public float jumpTrail = 0.1f;
    bool jumping;
    public float wallBurst = 0.1f;
    public float jumpLimit = .30f;

    public float maxSpeedWalk = 1;
    public float maxSpeedRun = 2;
    public float maxJumpSpeed = 1;
    public float walkSpeed = 0.5f;
    public float maxSpeedFall = 1f;
    public float maxSpeedWallGrab = 0.1f;
    public float maxSpeedWallJump = 1f;

    public float runSpeed = 0.75f;
    public float wallKickSpeed = 1f;
    float maxMovement = 120;
    float maxMovementThisFrame = 0;

    public float horizontalSpeed = 0;
    public float verticalSpeed = 0;

    public float damage = 0;

    float friction = 50;
    public float gravity = 0.1f;
    float minSpeed = 2;
    [HideInInspector]
    public float currentSpeedx;
    [HideInInspector]
    public float currentSpeedy;
    bool wallKickFromLeft = false;
    bool wallKickFromRight = false;
    float wallKickCounter = 0;
    //float directionCorrect = 68.51f;
    float jumpCounter;
    //public WallCollisionLeft wallLeft;
    [HideInInspector]
    public WallCollisionRight wallCollision;
    [HideInInspector]
    public GroundCollision grounded;
    bool facingRight = true;
    [HideInInspector]
    public bool landing = false;
    [HideInInspector]
    public float groundCounter = 0;
    public PortalController Portal;
    public int damageSound;

    public bool fastFall = true;
    bool noJump = false;
    public bool inputEnabled = true;
    float deathCounter = 0;
    public PlayerAttacks attack = 0;

    public float attackCounter = 0;

    public int attack1Counter = 0;
    [HideInInspector]
    public bool attack1CounterOn = false;
    [HideInInspector]
    public bool holdingAttack = false;
    [HideInInspector]
    public int crouchCounter = 0;
    private bool doubleJumpItem;
    private bool doubleJumpFlag;
    public bool attack3Pound = false;

    public ParticleSystem WalkParticles;
    public ParticleSystem WallParticlesLeft;
    public ParticleSystem WallParticlesRight;
    public ParticleSystem doubleJumpParticles;
    public ParticleSystem BloodParticles;


    public AudioSource JumpSound;
    public AudioSource AttackSource;
    public AudioClip Attack3Sound;
    public AudioClip[] walkingAudioClips = new AudioClip[7];
    public AudioSource walkingAudioSource;
    public AudioSource LandingAudioSource;
    public AudioClip fastFallLandingAudioClip;
    public AudioClip landingAudioClip;
    public AudioClip coinAudioClip;
    public AudioClip Attack1AudioClip1;
    public AudioClip Attack1AudioClip2;
    public AudioClip Runetone1;
    public AudioClip Runetone2;
    public AudioClip Runetone3;
    public AudioClip Runetone4;
    public AudioClip Runetone5;
    public AudioClip Damage1;
    public AudioClip Damage2;
    public AudioClip Damage3;
    public AudioSource CoinAudioSource;
    public AudioSource RuneAudioSource1;
    public AudioSource RuneAudioSource2;
    public AudioSource RuneAudioSource3;
    public AudioSource RuneAudioSource4;
    public AudioSource RuneAudioSource5;

    private int runeCounter = 0;
    public int coinCount = 0;
    private HashSet<string> runesCollected;
    private List<string> runeNames;

    public Collider2D attackGroundPoundHitbox;
    public Collider2D attackPunchHitbox;
    public Collider2D attackPunchSndHitbox;

    Animator anim;
    RaycastHit2D[] raycastHits = new RaycastHit2D[1];
    BoxCollider2D boxCollider;
    CircleCollider2D circleCollider;
    int groundLayerMask = 0;

    Dialog dialogComponent;

    Rigidbody2D player;

    public enum PlayerState
    {
        IDLE,
        WALKING,
        RUNNING,
        DIALOG,
        ATTACKING,
        FALLING,
        WALL_JUMP,
        WALL_GRAB,
        JUMPING,
        DOUBLE_JUMP,
        LANDING,
        CROUCHING
    };

    public enum PlayerAttacks
    {
        NO_ATTACK = 0,
        PUNCH = 1,
        PUNCH_SND = 2,
        GROUND_POUND = 3
    };

    public enum Direction
    {
        LEFT,
        RIGHT,
        OPPOSITE
    }

    // Use this for initialization
    void Start()
    {
        runesCollected = new HashSet<string>();
        runeNames = new List<string>();
        runeNames.Add("Tree");
        runeNames.Add("P");
        runeNames.Add("House");
        runeNames.Add("Cross");
        runeNames.Add("Fish");
        player = GetComponent<Rigidbody2D>();
        spriteDisplay = gameObject.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        groundLayerMask = LayerMask.GetMask("ground");
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hazard")
        {
            damage += 15;
            damageSound = Random.RandomRange(0, 3);
            if (damageSound == 0)
            {
                AttackSource.PlayOneShot(Damage1);
            }
            else if (damageSound == 1)
            {
                AttackSource.PlayOneShot(Damage2);
            }
            else if (damageSound == 2)
            {
                AttackSource.PlayOneShot(Damage3);
            }



        }
    }



    void OnTriggerStay2D(Collider2D other)
    {
        AudioSource audioSource;
        SpriteRenderer spriteRenderer;
        if (other.tag == "Hazard")
        {
            damage += 1;
        }
        else if (damage > 0)
        {
            damage -= 1f;
        }

        if (other.tag == "Collectable")
        {
            if (other.gameObject.name == "gem_1")
            {
                maxSpeedWalk += .05f;
                maxSpeedRun += .05f;
                maxJumpSpeed += .05f;
                walkSpeed += .05f;


                Destroy(GameObject.Find("gem_1"));
            }

            if (other.gameObject.name == "doublejump")
            {
                doubleJumpItem = true;
                Destroy(GameObject.Find("doublejump"));
            }

            if (other.gameObject.name == "coin")
            {
                coinCount++;
                CoinAudioSource.PlayOneShot(coinAudioClip);
                Destroy(other.gameObject);
            }
            else if (other.gameObject.name.EndsWith("RuneCollectable"))
            {
                if (!runesCollected.Contains(other.gameObject.name))
                {
                    runesCollected.Add(other.gameObject.name);
                    audioSource = other.gameObject.GetComponent<AudioSource>();
                    spriteRenderer = other.gameObject.GetComponent<SpriteRenderer>();
                    GameObject.Find(other.gameObject.name.Replace("Collectable", "")).GetComponent<SpriteRenderer>().enabled = true;
                    audioSource.PlayOneShot(audioSource.clip);
                    spriteRenderer.enabled = false;
                }
            }
        }
    }

    // Update is called once per frame 
    void Update()
    {
        float wallKickDirection;
        float rayX;
        float rayY;
        float rayXEnd;
        float rayYEnd;

        currentSpeedx = player.velocity.x;
        currentSpeedy = player.velocity.y;
        moving = false;
        maxMovementThisFrame = maxMovement;

        landing = false;

        anim.SetBool("WallGrab", false);


        //damage system-----------------------------------------------------------------------------------
        //Debug.Log(damage);
        BloodParticles.emissionRate = (damage * 100);
        if (damage > 250)
        {
            dead = true;
            anim.SetBool("Dead", true);
        }




        if (Input.GetKeyDown("j"))
        {
            runeCounter++;
        }

        if (Mathf.Abs(horizontalSpeed) < 0.000001) horizontalSpeed = 0;

        anim.SetBool("Up", false);
        anim.SetBool("Down", false);

        attackGroundPoundHitbox.enabled = false;
        attackPunchHitbox.enabled = false;
        attackPunchSndHitbox.enabled = false;

        switch (playerState)
        {
            case PlayerState.DIALOG:

                player.velocity = Vector2.zero;
                anim.SetBool("Dialog", true);
                inputEnabled = false;
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    dialogComponent.IncrementMessage();
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    dialogComponent.DecrementMessage();
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("space"))
                {
                    dialogComponent.SelectDialogOption();
                }
                horizontalSpeed = 0;
                verticalSpeed = 0;
                break;

            case PlayerState.IDLE:
                horizontalSpeed = Mathf.Lerp(horizontalSpeed, 0, 0.3f);
                verticalSpeed = 0;
                if ((!grounded.grounded) && (playerState != PlayerState.ATTACKING))
                {
                    playerState = PlayerState.FALLING;
                }
                break;

            case PlayerState.JUMPING:
                //jumping state
                anim.SetBool("Up", true);

                rayX = boxCollider.bounds.center.x;
                rayY = boxCollider.bounds.center.y;
                rayXEnd = boxCollider.bounds.center.x;
                rayYEnd = boxCollider.bounds.center.y + boxCollider.bounds.extents.y + 1;
                if (CastLine(rayX, rayY, rayXEnd, rayYEnd, groundLayerMask))
                {
                    playerState = PlayerState.FALLING;
                    transform.position -= new Vector3(0, 1);
                }
                break;
            case PlayerState.FALLING:
                //falling state
                anim.SetBool("Down", true);

                //verticalSpeed -= gravity;
                //verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeedFall, maxSpeedFall);

                horizontalSpeed = Mathf.Lerp(horizontalSpeed, 0, Time.deltaTime * 0.5f);

                /*
                if ((wallCollision.wallLeft == true) || (wallCollision.wallRight == true))
                {
                    anim.SetBool("WallGrab", true);
                    playerState = PlayerState.WALL_GRAB;
                }
                */

                rayX = boxCollider.bounds.center.x;
                rayY = boxCollider.bounds.center.y;
                rayXEnd = boxCollider.bounds.center.x + boxCollider.bounds.size.x * (facingRight ? 1 : -1);
                rayYEnd = boxCollider.bounds.center.y;
                if (CastLine(rayX, rayY, rayXEnd, rayYEnd, groundLayerMask) && (Input.GetKey("d") || Input.GetKey("a")))
                {
                    anim.SetBool("WallGrab", true);
                    playerState = PlayerState.WALL_GRAB;
                }

                rayX = boxCollider.bounds.center.x;
                rayY = boxCollider.bounds.center.y;
                rayXEnd = boxCollider.bounds.center.x;
                rayYEnd = boxCollider.bounds.center.y - boxCollider.bounds.size.y;
                if (CastLine(rayX, rayY, rayXEnd, rayYEnd, groundLayerMask))
                {
                    playerState = PlayerState.IDLE;
                }
                rayX = boxCollider.bounds.center.x;
                rayY = boxCollider.bounds.center.y;
                rayXEnd = boxCollider.bounds.center.x;
                rayYEnd = boxCollider.bounds.center.y + boxCollider.bounds.size.y;
                if (CastLine(rayX, rayY, rayXEnd, rayYEnd, groundLayerMask))
                {
                    transform.position -= new Vector3(0, 1);
                    verticalSpeed = 0;
                }
                break;

            case PlayerState.WALL_GRAB:
                anim.SetBool("WallGrab", true);

                //verticalSpeed -= gravity;
                //verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeedWallGrab, maxSpeedWallGrab);
                verticalSpeed = (verticalSpeed / 2);
                /*
                rayX = boxCollider.bounds.center.x;
                rayY = boxCollider.bounds.center.y;
                rayXEnd = boxCollider.bounds.center.x + boxCollider.bounds.size.x * (facingRight ? 1 : -1);
                rayYEnd = boxCollider.bounds.center.y;
                if (CastLine(rayX, rayY, rayXEnd, rayYEnd, groundLayerMask))
                {
                    player.transform.position = new Vector2(player.transform.position.x + (raycastHits[0].point.x - boxCollider.transform.position.x) + boxCollider.bounds.extents.x,
                                                            player.transform.position.y);
                }
                */

                if (wallCollision.wallLeft)
                {
                    WallParticlesLeft.emissionRate = 250;
                    //Debug.Log("wall particles should be playing");

                }
                else if (wallCollision.wallRight)
                {
                    WallParticlesRight.emissionRate = 250;
                    //Debug.Log("wall particles should be playing");
                }
                else
                {
                    playerState = PlayerState.FALLING;
                    WallParticlesRight.emissionRate = 0;
                    WallParticlesLeft.emissionRate = 0;
                }
                break;


         


            case PlayerState.RUNNING:
                if ((!grounded.grounded) && (playerState != PlayerState.ATTACKING))
                {
                    playerState = PlayerState.FALLING;
                }
                break;

            case PlayerState.WALKING:
                if (!Input.GetKey("a") && !Input.GetKey("d"))
                {
                    playerState = PlayerState.IDLE;
                    horizontalSpeed = 0;
                }
                break;

            case PlayerState.ATTACKING:
                switch (attack)
                {
                    case PlayerAttacks.GROUND_POUND:
                        attackGroundPoundHitbox.enabled = true;
                        break;
                    case PlayerAttacks.PUNCH:
                        attackPunchHitbox.enabled = true;
                        break;
                    case PlayerAttacks.PUNCH_SND:
                        attackPunchSndHitbox.enabled = true;
                        break;
                    case PlayerAttacks.NO_ATTACK:
                        playerState = PlayerState.IDLE;
                        break;
                }
                break;

            default:
                break;
        }

        //double jump stuff-----------------------------------------------------------------------------------
        if ((doubleJumpItem == true) && (grounded.grounded == false) && (doubleJumpFlag == false) && (Input.GetKeyDown("w")))
        {
            playerState = PlayerState.DOUBLE_JUMP;
            jumping = true;
            jumpCounter = 0;
            verticalSpeed += jumpBurst;
            //player.velocity = new Vector2(player.velocity.x, 0);
            doubleJumpFlag = true;
            doubleJumpParticles.emissionRate = 5000;
        }
        else
        {
            doubleJumpParticles.emissionRate = 0;
        }
        //setting movement speed

        //jumping check
        if ((grounded.grounded == true) && (noJump == false) && (inputEnabled == true) && (Portal.portalActivated == false) && (playerState != PlayerState.DIALOG) && (playerState != PlayerState.ATTACKING) && (((Input.GetKeyDown("w")) || (Input.GetKeyDown("space")))))
        {
            JumpSound.Play();
            jumping = true;
            jumpCounter = 0;
            if (crouchCounter > 0)
            {
                verticalSpeed += (jumpBurst + 2000);
            }
            else verticalSpeed += jumpBurst;
            playerState = PlayerState.JUMPING;
        }

        if (playerState == PlayerState.JUMPING || playerState == PlayerState.DOUBLE_JUMP)
        {
            if (Input.GetKeyUp("w") || (Input.GetKeyUp("space")))
            {
                jumpCounter = 0;
                jumping = false;
                playerState = PlayerState.FALLING;
            }
            else
            {
                jumpCounter += Time.deltaTime;
                verticalSpeed += jumpTrail;
                //if (playerState != PlayerState.DOUBLE_JUMP)playerState = PlayerState.JUMPING;
                verticalSpeed = Mathf.Clamp(verticalSpeed, -maxJumpSpeed, maxJumpSpeed);
            }
        }
        


        if (jumpCounter > jumpLimit)
        {
            jumping = false;
            jumpCounter = 0;
            playerState = PlayerState.FALLING;
        }

        if ((grounded.landing == true) && (!wallCollision.wallLeft) && (!wallCollision.wallRight) && (Portal.portalActivated == false))
        {
            if (fastFall)
            {
                LandingAudioSource.PlayOneShot(fastFallLandingAudioClip);
            }
            else
            {
                LandingAudioSource.PlayOneShot(landingAudioClip);
            };
        }

        if (grounded.grounded == false)
        {
            grounded.landing = false;
        }

        //add gravity
        if (!grounded.grounded)
        {
            verticalSpeed -= gravity;
        }
        if (grounded.grounded == true)
        {
            //gravity = 10;
            doubleJumpFlag = false;
        }

        //less control in air

        //movement input
        if ((Input.GetKey("a")) && !(movingRight) && (inputEnabled == true))
        {
            moving = true;
            movingLeft = true;
        }
        else
        {
            movingLeft = false;
        }

        if ((Input.GetKey("d")) && !(movingLeft) && (inputEnabled == true))
        {
            moving = true;
            movingRight = true;
        }
        else
        {
            movingRight = false;
        }

        //TODO move to walking state, move rate = 0 to top of function
        if (grounded.grounded == true)
        {
            if ((movingLeft) || (movingRight))
            {
                WalkParticles.emissionRate = 1000;
                if (movingLeft)
                {
                    WalkParticles.transform.rotation = Quaternion.Euler(300, 0, 0);
                }
                else WalkParticles.transform.rotation = Quaternion.Euler(240, 0, 0);

                if (!walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.PlayOneShot(walkingAudioClips[Random.Range(0, walkingAudioClips.Length)]);
                    if (Input.GetKey(KeyCode.LeftShift))
                        walkingAudioSource.pitch = 1.25f;
                    else walkingAudioSource.pitch = 1.0f;
                }
            }
            else WalkParticles.emissionRate = 0;
        }
        else WalkParticles.emissionRate = 0;


        int moveDirection = 0;
        if (movingLeft)
        {
            moveDirection = -1;
        }
        if (movingRight)
        {
            moveDirection = 1;
        }

        if (playerState == PlayerState.ATTACKING && attack == PlayerAttacks.GROUND_POUND && grounded.grounded)
        {
            moveDirection = 0;
        }

        if (inputEnabled && (movingLeft || movingRight))
        {
            if (grounded.grounded && !jumping && playerState != PlayerState.ATTACKING)
            {
                playerState = PlayerState.WALKING;
            }

            if (Input.GetKey(KeyCode.LeftShift) && (movingLeft || movingRight))
            {
                if (playerState != PlayerState.ATTACKING && grounded.grounded && !jumping) playerState = PlayerState.RUNNING;

                if (grounded.grounded == true)
                {
                    horizontalSpeed += moveDirection * runSpeed * Time.deltaTime;
                    horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeedRun, maxSpeedRun);
                }
                else
                {
                    horizontalSpeed += moveDirection * runSpeed * Time.deltaTime;
                    horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeedRun, maxSpeedRun);
                }
            }
            else
            {
                if (grounded.grounded == false)
                {
                    horizontalSpeed += moveDirection * walkSpeed * Time.deltaTime;
                    horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeedWalk, maxSpeedWalk);
                }
                else
                {
                    horizontalSpeed += moveDirection * walkSpeed * Time.deltaTime;
                    horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeedWalk, maxSpeedWalk);
                }
            }
        }

        if (Input.GetKeyUp("a") && (inputEnabled == true))
        {
            movingLeft = false;
        }

        if (Input.GetKeyUp("d") && (inputEnabled == true))
        {
            movingRight = false;
        }

        if (!(Input.GetKey("a")) && !(Input.GetKey("d")))
        {
            moving = false;
        }

        //wall jumping---------------------------------------------------------------------------------------------------------------


        wallKickDirection = wallCollision.wallNearRight ? -1 : 1;
        if ((wallCollision.wallNearRight || wallCollision.wallNearLeft) && (!grounded.grounded) && (Input.GetKeyDown("w") || Input.GetKeyDown("space")))
        {
            playerState = PlayerState.WALL_JUMP;
            inputEnabled = false;
            Flip();
            horizontalSpeed = wallKickDirection * wallBurst * Time.deltaTime;
            horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeedWallJump, maxSpeedWallJump);
            jumping = true;
            noJump = true;
            jumpCounter = 0;
            wallKickFromRight = wallCollision.wallNearRight;
            wallKickFromLeft = wallCollision.wallNearLeft;
            verticalSpeed = 0;

            StartCoroutine(wallCollision.Deactivate());
        }

        //wall kick pulling, like in N
        if (wallKickFromLeft || wallKickFromRight)
        {
            wallKickCounter += Time.deltaTime;
            wallKickDirection = wallKickFromRight ? -1 : 1;

            if (wallKickCounter < 0.6f)
            {
                //Debug.Log("Kicking")
                //Debug.Log("input disabled");
                horizontalSpeed += wallKickDirection * wallKickSpeed * Time.deltaTime;
                verticalSpeed += wallKickSpeed * Time.deltaTime;
                /*
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    verticalSpeed += jumpTrail * Time.deltaTime;
                    horizontalSpeed += wallKickDirection * (jumpTrail * 0.9f) * Time.deltaTime;
                }
                else
                {
                    verticalSpeed += jumpTrail * Time.deltaTime;
                    horizontalSpeed += wallKickDirection * (jumpTrail * 0.7f) * Time.deltaTime;
                }
                */
                horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeedWallJump, maxSpeedWallJump);
            }

            if (wallKickCounter > 0.2f)
            {
                inputEnabled = true;
            }

            if (wallKickCounter >= 0.4f)
            {
                //Debug.Log("End Kicking");
                wallKickFromLeft = false;
                wallKickFromRight = false;
                inputEnabled = true;
                wallKickCounter = 0;

                playerState = PlayerState.FALLING;
            }
        }

        if (((wallCollision.wallLeft && Input.GetKeyDown("d")) || (wallCollision.wallRight && Input.GetKeyDown("a"))) &&
            (player.velocity.y > 0) && (grounded.grounded == false))
        {
            StartCoroutine(wallCollision.Deactivate());
            //player.velocity = new Vector2(0, 0);
            // horizontalSpeed += wallFall;
            //player.AddForce(new Vector2(0, 500));
            maxMovementThisFrame = maxMovement;
            playerState = PlayerState.FALLING;
        }

        if (((wallCollision.wallLeft) || (wallCollision.wallRight)) && (!Input.GetKey("a")) && (!Input.GetKey("d")))
        {
            StartCoroutine(wallCollision.Deactivate());
            maxMovementThisFrame = maxMovement;
        }

        if (Input.GetKey("r"))
        {
            playerState = PlayerState.IDLE;
            Application.LoadLevel("Scene_VerticalSlice");
        }

        if ((grounded.grounded == true) && (moving == false))
        {
            // player.velocity -= (new Vector2(Mathf.Sign(player.velocity.x) * friction, 0) * Time.deltaTime);
            if (Mathf.Abs(player.velocity.x) < minSpeed)
            {
                // player.velocity = new Vector2(0, 0);
            }
        }

        if (wallCollision.wallLeft || wallCollision.wallRight)
        {
            // player.velocity -= new Vector2(0, wallFriction);
            maxMovementThisFrame = maxMovement / 1000;
        }


        if ((grounded.grounded == false) && ((wallCollision.wallLeft == true) || (wallCollision.wallRight == true)))
        {
            horizontalSpeed = 0;
        }

        //player can't move if in the landing state
        if ((groundCounter > 0) && (groundCounter < 45) & (fastFall == true))
        {
            horizontalSpeed = 0;
            noJump = true;
        }
        else
        {
            noJump = false;
        }
        //Debug.Log("ground counter:" + groundCounter);


        //add velocity to player

        //Check Above Head.
        if (CastLine(player.transform.position.x, player.transform.position.y, player.transform.position.x, player.transform.position.y + 2, groundLayerMask))
        {
            verticalSpeed = 0;
        }
        //Check out in front.
        bool wallCheck = CastLine(player.transform.position.x,
                                  player.transform.position.y,
                                  player.transform.position.x + Mathf.Sign(horizontalSpeed),
                                  player.transform.position.y, groundLayerMask);
        if (playerState != PlayerState.JUMPING && wallCheck)
        {
            horizontalSpeed = 0;
        }
        if (horizontalSpeed != 0 || verticalSpeed != 0)
        {
            player.transform.position += new Vector3(horizontalSpeed, verticalSpeed, 0);
        }
        rayX = boxCollider.bounds.center.x;
        rayY = boxCollider.bounds.center.y;
        rayXEnd = boxCollider.bounds.center.x;
        rayYEnd = boxCollider.bounds.center.y - boxCollider.bounds.size.y;
        if (!AirState(playerState) && CastLine(rayX, rayY, rayXEnd, rayYEnd, groundLayerMask))
        {
            //Debug.Log("rayX " + rayX + ", rayY " + rayY + ", rayXEnd " + rayXEnd + ", rayYEnd " + rayYEnd);
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y +
                                        (raycastHits[0].point.y - boxCollider.transform.position.y) + boxCollider.bounds.extents.y);
        }
        //player.AddForce(new Vector2(horizontalSpeed, verticalSpeed));




        if ((wallCollision.wallNearRight == true) && (Input.GetKey("d")) && !facingRight)
        {
            Flip();
        }

        if ((wallCollision.wallNearLeft == true) && (Input.GetKey("a")) && facingRight)
        {
            Flip();
        }

        if (attack != 0)
        {
            horizontalSpeed = 0;
        }


        anim.SetBool("Sliding", playerState == PlayerState.IDLE && horizontalSpeed != 0);
    }

    public bool AirState(PlayerState state)
    {
        return (state == PlayerState.JUMPING) ||
               (state == PlayerState.DOUBLE_JUMP) ||
               (state == PlayerState.WALL_GRAB) ||
               (state == PlayerState.WALL_JUMP);
    }

    public bool CastLine(float x, float y, float endX, float endY, int mask)
    {
        int numRaycastHits = 0;
        Vector2 startLine = new Vector2(x, y);
        Vector2 endLine = new Vector2(endX, endY);
        Debug.DrawLine(startLine, endLine, Color.red);
        numRaycastHits = Physics2D.LinecastNonAlloc(startLine,
                                                    endLine,
                                                    raycastHits,
                                                    mask);
        return numRaycastHits > 0;
    }

    public bool CastLinePolar(float x, float y, float length, float angle, int mask)
    {
        float endX = Mathf.Cos(angle) * length;
        float endY = Mathf.Sin(angle) * length;
        return CastLine(x, y, endX, endY, mask);
    }

    /* animation code-------------------------------------------------------------------------------------------------------------*/

    void FixedUpdate()
    {
        if (dead == true)
        {
            inputEnabled = false;
            deathCounter++;
        }

        if (deathCounter > 70)
        {
            Application.LoadLevel(0);
        }

        switch (playerState)
        {
            case PlayerState.DIALOG:
                break;

            case PlayerState.IDLE:
                if ((grounded.grounded == true) && (Input.GetKey("s")) && (moving == false))
                {
                    playerState = PlayerState.CROUCHING;
                }
                break;

            case PlayerState.CROUCHING:
                //Crouching--------------------------------------------------------------------------------
                anim.SetBool("Crouch", true);

                if ((grounded.grounded == true) && (Input.GetKey("s")) && (moving == false))
                {
                    crouchCounter++;
                }
                else
                {
                    crouchCounter = 0;
                }

                if (crouchCounter > 17)
                {
                    anim.SetBool("Crouch Hold", true);
                    anim.SetBool("Crouch", false);
                }

                if (crouchCounter >= 18)
                {
                    crouchCounter = 18;
                }

                if (crouchCounter == 0)
                {
                    anim.SetBool("Crouch", false);
                    anim.SetBool("Crouch Hold", false);
                    Debug.Log("Crounch to idle");
                    playerState = PlayerState.IDLE;
                }
                break;
            case PlayerState.ATTACKING:
                attackCounter++;

                if (grounded.grounded == true)
                {
                    inputEnabled = false;
                }

                if ((!Input.GetKey(KeyCode.LeftArrow)) && (!Input.GetKey(KeyCode.RightArrow)) && (!Input.GetKey(KeyCode.DownArrow)))
                {
                    holdingAttack = false;
                }


                anim.SetFloat("AttackCounter", attackCounter);
                if (attack == PlayerAttacks.PUNCH)
                {
                    if ((attackCounter == 29 || attackCounter == 85) && !holdingAttack)
                    {
                        attack = PlayerAttacks.NO_ATTACK;
                        playerState = PlayerState.IDLE;
                        inputEnabled = true;
                        attackCounter = 0;
                    }
                    else if (attackCounter > 90)
                    {
                        attack = PlayerAttacks.PUNCH_SND;
                    }
                }
                else if (attack == PlayerAttacks.PUNCH_SND)
                {
                    if (attackCounter > 169)
                    {

                        if (holdingAttack)
                        {
                            attack = PlayerAttacks.PUNCH;
                            attackCounter = 0;
                        }
                        else
                        {
                            attack = PlayerAttacks.NO_ATTACK;
                            inputEnabled = true;
                            playerState = PlayerState.IDLE;
                            attackCounter = 0;
                        }
                    }
                }
                else if (attack == PlayerAttacks.GROUND_POUND)
                {
                    if ((attackCounter > 12) && (anim.GetBool("Grounded") == true) && (anim.GetBool("Attack3 Pound") == false))
                    {
                        anim.SetBool("Attack3 Pound", true);
                        attackCounter = 0;
                        //AttackSource.PlayOneShot(Attack3Sound);
                        attack3Pound = true;
                    }
                    if (attackCounter > 75 && (anim.GetBool("Attack3 Pound")))
                    {
                        attack = PlayerAttacks.NO_ATTACK;
                        playerState = PlayerState.IDLE;
                        attackCounter = 0;
                        attack3Pound = false;
                        inputEnabled = true;
                        anim.SetBool("Attack3 Pound", false);
                    }
                }

                anim.SetInteger("Attack", (int)attack);

                if (inputEnabled == false)
                {
                    //Debug.Log("Input is disabled");
                }
                break;

            default:
                break;
        }

        //moving variable for animator, to make sure that the player is trying to move, not just sliding down a slope
        anim.SetBool("Moving", moving);

        //VerticalSpeed is calculated using thresholds in the fall animation blend tree.
        anim.SetFloat("VerticalSpeed", 0.5f * (65 / maxSpeedFall) * verticalSpeed);

        if (grounded.grounded == true)
        {
            anim.SetBool("Landing", false);
        }
        else
        {
            groundCounter = 0;
        }
        anim.SetBool("Grounded", grounded.grounded);

        /*
        if (grounded.grounded && !moving && playerState != PlayerState.CROUCHING && !jumping && (playerState != PlayerState.ATTACKING) && (playerState != PlayerState.DIALOG))
        {
            Debug.Log("crounced to idle2");
            playerState = PlayerState.IDLE;

        }
        */

        //landing state
        if ((grounded.grounded == true) && (groundCounter < 47))
        {
            groundCounter++;
        }

        //falling fast enough for a landing animation
        if (verticalSpeed < -2)
        {
            fastFall = true;
        }

        if (groundCounter == 45)
        {
            fastFall = false;
        }

        if ((groundCounter > 0) && (groundCounter < 47))
        {
            landing = true;
        }

        if ((groundCounter == 0) || (groundCounter == 47))
        {
            landing = false;
        }

        anim.SetBool("Landing", landing);


        if ((Mathf.Abs(currentSpeedx) > 0) && (grounded.grounded == true))
        {
            anim.SetFloat("Speed", Mathf.Abs(currentSpeedx));
        }

        if ((Mathf.Abs(currentSpeedx) <= 0) && (grounded.grounded == true))
        {
            anim.SetFloat("Speed", Mathf.Abs(currentSpeedx));
        }

        if (fastFall == false)
        {
            anim.SetBool("FastFall", false);
        }

        if ((horizontalSpeed > 0.001) && !facingRight)
        {
            Flip();
        }
        else if ((horizontalSpeed < -0.001) && (facingRight))
        {
            Flip();
        }

        if (fastFall == true)
        {
            anim.SetBool("FastFall", true);
        }

        /*Attacking animations---------------------------------------------------------------*/

        if ((Input.GetKeyDown(KeyCode.LeftArrow)) && facingRight)
        {
            Flip();
        }

        if ((Input.GetKeyDown(KeyCode.RightArrow)) && !facingRight)
        {
            Flip();
        }

        if (playerState != PlayerState.ATTACKING && playerState != PlayerState.DIALOG)
        {
            anim.SetBool("Attack3 Pound", false);
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                attack = PlayerAttacks.GROUND_POUND;
                playerState = PlayerState.ATTACKING;
                holdingAttack = true;
            }
            else if ((Input.GetKeyDown(KeyCode.LeftArrow)) || (Input.GetKeyDown(KeyCode.RightArrow)))
            {
                attack = PlayerAttacks.PUNCH;
                playerState = PlayerState.ATTACKING;
                holdingAttack = true;
            }
        }

        //sliding code----------------------------------------------------------------------------------------
        //if ((Mathf.Abs(horizontalSpeed.x) > 0.5) && (moving == false) && (grounded.grounded == true))
        //{
        // anim.SetBool("Sliding", true);
        //  Debug.Log("Sliding is true");
        // }
        // else anim.SetBool("Sliding", false);

    }

    public void Flip(Direction direction = Direction.OPPOSITE)
    {
        if (direction == Direction.OPPOSITE)
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        else if (direction == Direction.LEFT)
        {
            facingRight = false;
            Vector3 theScale = transform.localScale;
            if (Mathf.Sign(theScale.x) != -1)
            {
                theScale.x *= -1;
                transform.localScale = theScale;
            }
        }
        else
        {
            facingRight = true;
            Vector3 theScale = transform.localScale;
            if (Mathf.Sign(theScale.x) != 1)
            {
                theScale.x *= -1;
                transform.localScale = theScale;
            }
        }
    }

    public bool StartDialog(Dialog dialogComponent)
    {
        bool ready = false;
        if (grounded.grounded)
        {
            ready = true;
            playerState = PlayerState.DIALOG;
            this.dialogComponent = dialogComponent;
        }
        return ready;
    }

    public void EndDialog()
    {
        playerState = PlayerState.IDLE;
        this.dialogComponent = null;
        anim.SetBool("Dialog", false);
        inputEnabled = true;
    }

    void LateUpdate()
    {
        //clamp velocity
        player.velocity = Vector2.ClampMagnitude(player.velocity, maxMovementThisFrame);

        if (playerState != PlayerState.WALL_GRAB)
        {
            WallParticlesLeft.emissionRate = 0;
            WallParticlesRight.emissionRate = 0;

        }

    }
}
