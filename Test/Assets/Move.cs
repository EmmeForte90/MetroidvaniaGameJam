using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class Move : MonoBehaviour
{
    
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    public float horDir;
    public float runSpeedThreshold = 5f; // or whatever value you want


    [Header("Jump")]
    [SerializeField] private float jumpForce;

    //COYOTE TIME: can jump for a short time after leave ground
    [SerializeField] private float coyoteTime;
    private float lastTimeGround;
    
    //JUMP DELAY: avoid jump only when touch ground
    [SerializeField] private float jumpDelay;
    private float lastTimeJump;

    [SerializeField] private float gravityOnJump;
    [SerializeField] private float gravityOnFall;
    
    private readonly Vector3 raycastColliderOffset = new (0.25f, 0, 0);
    private const float distanceFromGroundRaycast = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
  
  
    
   
    [Header("Animations")]
    [SpineAnimation][SerializeField] private string idleAnimationName;
    [SpineAnimation][SerializeField] private string walkAnimationName;
    [SpineAnimation][SerializeField] private string runAnimationName;
    [SpineAnimation][SerializeField] private string jumpAnimationName;
    [SpineAnimation][SerializeField] private string jumpDownAnimationName;
    [SpineAnimation][SerializeField] private string attackAnimationName;
    [SpineAnimation][SerializeField] private string attack_lAnimationName;
    [SpineAnimation][SerializeField] private string attack_hAnimationName;
    [SpineAnimation][SerializeField] private string blastAnimationName;
    //[SpineAnimation][SerializeField] private string attackAnimationName;

    [Header("Attacks")]
    private float currentCooldown; // contatore del cooldown attuale
    [SerializeField] float nextAttackTime = 0f;
    [SerializeField] float attackRate = 2f;
    [SerializeField] public float attackCooldown = 0.5f; // tempo di attesa tra gli attacchi
    [SerializeField] public float comboTimer = 2f; // tempo per completare una combo
    [SerializeField] public int comboCounter = 0; // contatore delle combo
    [SerializeField] public int maxCombo = 3; // numero massimo di combo
    [SerializeField] public float shootTimer = 2f; // tempo per completare una combo
    [SerializeField] private GameObject bullet;
    public float maxChargeTime = 3f; // Tempo massimo di carica in secondi
    public float maxDamage = 30f; // Danno massimo dell'attacco
    private float chargeTime;
    private bool isCharging;
    public Transform slashpoint;
    public int facingDirection = 1; // La direzione in cui il personaggio sta guardando: 1 per destra, -1 per sinistra
    PlayerHealth Less;
    [SerializeField] public GameplayManager gM;



    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Spine.Skeleton _skeleton;

    private string currentAnimationName;


    private Rigidbody2D rb;

    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        rb = GetComponent<Rigidbody2D>();
        if (gM == null)
        {
            gM = GetComponent<GameplayManager>();
        }
        Less = GetComponent<PlayerHealth>();
        currentCooldown = attackCooldown;
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.skeleton;
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        horDir = Input.GetAxisRaw("Horizontal");

        if (isGrounded())
        {
            lastTimeGround = coyoteTime;   
            rb.gravityScale = 1;
        }
        else
        {
            lastTimeGround -= Time.deltaTime;
            modifyPhysics();
        }

        if (Input.GetButtonDown("Jump"))
            lastTimeJump = Time.time + jumpDelay;

        //Pre-interrupt jump if button released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            lastTimeGround = 0; //Avoid spam button
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);   
        }

        
        checkFlip();

        selectAnimation();
    }

    private void FixedUpdate()
    {
        float playerSpeed = horDir * speed;
        float accelRate = Mathf.Abs(playerSpeed) > 0.01f? acceleration : deceleration;
        rb.AddForce((playerSpeed - rb.velocity.x) * accelRate * Vector2.right);
        rb.velocity = new Vector2(Vector2.ClampMagnitude(rb.velocity, speed).x, rb.velocity.y); //Limit velocity
        
        if (lastTimeJump > Time.time && lastTimeGround > 0)
            jump();
    }

    private void jump()
    {
        lastTimeJump = 0;
        lastTimeGround = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce), ForceMode2D.Impulse);
    }

    private void modifyPhysics()
    {
        if (rb.velocity.y > 0)
            rb.gravityScale = gravityOnJump;
        else if (rb.velocity.y < 0)
            rb.gravityScale = gravityOnFall;
    }


    private bool isGrounded()
    {
        //DOUBLE RAYCAST FOR GROUND: check if you touch the ground even with just one leg 
        return (
                Physics2D.Raycast(transform.position + raycastColliderOffset, Vector3.down, distanceFromGroundRaycast, groundLayer)
                ||
                Physics2D.Raycast(transform.position - raycastColliderOffset, Vector3.down, distanceFromGroundRaycast, groundLayer)
            );
    }
    
    private void checkFlip()
    {
        if (horDir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horDir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void selectAnimation()
    {
        switch (rb.velocity.y)
        {
            case 0:
                    float speed = Mathf.Abs(rb.velocity.x);
                    if (speed == 0)
                    {
                        // Player is jumping or falling
                        if (currentAnimationName != "idle")
                        {
                            _spineAnimationState.SetAnimation(0, idleAnimationName, false);
                            currentAnimationName = "idle"; 
                        }
                    }
                    else if (speed > runSpeedThreshold)
                    {
                        // Player is running
                        if (currentAnimationName != "run")
                        {
                            _spineAnimationState.SetAnimation(0, runAnimationName, true);
                            currentAnimationName = "run"; 
                        }
                    }
                    else
                    {
                        // Player is walking
                        if (currentAnimationName != "walk")
                        {
                            _spineAnimationState.SetAnimation(0, walkAnimationName, true);
                            currentAnimationName = "walk"; 
                        }
                    }                
                    break;

            case > 0:
                if (currentAnimationName != "jump")
                {
                    _spineAnimationState.SetAnimation(0, jumpAnimationName, false);
                    currentAnimationName = "jump"; 
                }
                break;
            case < 0:
                if (currentAnimationName != "jump_down")
                {
                    _spineAnimationState.SetAnimation(0, jumpDownAnimationName, false);
                    currentAnimationName = "jump_down"; 
                }
                break;
        }
    }
    
#if(UNITY_EDITOR)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + raycastColliderOffset, transform.position + raycastColliderOffset + Vector3.down * distanceFromGroundRaycast);
        Gizmos.DrawLine(transform.position - raycastColliderOffset, transform.position - raycastColliderOffset + Vector3.down * distanceFromGroundRaycast);
    }
#endif
}


