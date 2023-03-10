using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class Move : MonoBehaviour
{
    
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private float horDir;
    private float vertDir;

    public float runSpeedThreshold = 5f; // or whatever value you want

    [Header("Dash")]
    public float dashForce = 50f;
    public float dashDuration = 0.5f;
    private float dashTime;
    private bool dashing;
    private bool Atkdashing;
    private float dashForceAtk = 40f;
    private bool attackNormal;
    public float dashCoolDown = 1f;
    private float coolDownTime;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    bool canDoubleJump = false;
    public float groundDelay = 0.1f; // The minimum time before the player can jump again after touching the ground
    bool isTouchingWall = false;
    public LayerMask wallLayer;         // layer del muro
    public float wallJumpForce = 7f;    // forza del walljump
    public float wallSlideSpeed = 1f;   // velocità di scivolamento lungo il muro
    public float wallDistance = 0.5f;   // distanza dal muro per effettuare il walljump
    public bool canWallJump = false;
    bool wallJumped = false;


    
    float coyoteCounter = 0f;

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
    
    [Header("Abilitazioni")]
    public bool unlockWalljump = false;
    public bool unlockDoubleJump = false;
    public bool unlockDash = false;
    //private bool isDashing;



    [Header("VFX")]
    // Variabile per il gameobject del proiettile
    [SerializeField] GameObject blam;
    [SerializeField] public Transform gun;
    [SerializeField] GameObject Circle;
    [SerializeField] public Transform circlePoint;
    [SerializeField] public Transform slashpoint;
    [SerializeField] GameObject attack;
    [SerializeField] GameObject attack_h;
    [SerializeField] GameObject attack_l;
    [SerializeField] GameObject attack_a;
    [SerializeField] GameObject pesante;
    [SerializeField] GameObject charge;



    
   
    [Header("Animations")]
    [SpineAnimation][SerializeField] private string idleAnimationName;
    [SpineAnimation][SerializeField] private string walkAnimationName;
    [SpineAnimation][SerializeField] private string runAnimationName;
    [SpineAnimation][SerializeField] private string jumpAnimationName;
    [SpineAnimation][SerializeField] private string jumpDownAnimationName;
    [SpineAnimation][SerializeField] private string attackAnimationName;
    [SpineAnimation][SerializeField] private string attack_lAnimationName;
    [SpineAnimation][SerializeField] private string attack_hAnimationName;
    [SpineAnimation][SerializeField] private string attack_aAnimationName;
    [SpineAnimation][SerializeField] private string chargeAnimationName;
    [SpineAnimation][SerializeField] private string blastAnimationName;
    [SpineAnimation][SerializeField] private string landingAnimationName;
    [SpineAnimation][SerializeField] private string walljumpAnimationName;
    [SpineAnimation][SerializeField] private string walljumpdownAnimationName;
    [SpineAnimation][SerializeField] private string dashAnimationName;
    [SpineAnimation][SerializeField] private string pesanteAnimationName;
    [SpineAnimation][SerializeField] private string RestAnimationName;
    [SpineAnimation][SerializeField] private string UpAnimationName;
    [SpineAnimation][SerializeField] private string respawnAnimationName;
    [SpineAnimation][SerializeField] private string upatkjumpAnimationName;
    [SpineAnimation][SerializeField] private string downatkjumpAnimationName;


private string currentAnimationName;

//private CharacterState currentState = CharacterState.Idle;
private int comboCount = 0;
     

    [Header("Attacks")]
    [SerializeField] public int comboCounter = 0; // contatore delle combo
    [SerializeField] float nextAttackTime = 0f;
    [SerializeField] float attackRate = 0.5f;
    [SerializeField] public float shootTimer = 2f; // tempo per completare una combo
    [SerializeField] private GameObject bullet;
    // Dichiarazione delle variabili
    public int Damage;
    public int currentTime;
    public int timeLimit = 3; // Tempo massimo per caricare l'attacco
    public int maxDamage = 50; // Danno massimo dell'attacco caricato
    public int minDamage = 10; // Danno minimo dell'attacco non caricato
    private float timeSinceLastAttack = 0f;
    public bool isCharging;
    private bool touchGround;
    private bool isDashing;
    public bool isAttacking = false; // vero se il personaggio sta attaccando
    public bool isAttackingAir = false; // vero se il personaggio sta attaccando
    public bool isBlast = false; // vero se il personaggio sta attaccando

    public bool stopInput = false;

    public int facingDirection = 1; // La direzione in cui il personaggio sta guardando: 1 per destra, -1 per sinistra
    PlayerHealth Less;
    [SerializeField] public GameplayManager gM;

    [Header("Audio")]
    public float basePitch = 1f;
    public float randomPitchOffset = 0.1f;
    [SerializeField] AudioSource SwSl;
    [SerializeField] AudioSource Smagic;
    [SerializeField] AudioSource Swalk;
    [SerializeField] AudioSource Srun;
    [SerializeField] AudioSource Scharge;
    [SerializeField] AudioSource Sdash;


    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Spine.Skeleton _skeleton;
    Spine.EventData eventData;



    private Rigidbody2D rb;

public static Move instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
_skeletonAnimation = GetComponent<SkeletonAnimation>();
if (_skeletonAnimation == null) {
    Debug.LogError("Componente SkeletonAnimation non trovato!");
}        rb = GetComponent<Rigidbody2D>();
       
        if (gM == null)
        {
            gM = GetComponent<GameplayManager>();
        }
        Less = GetComponent<PlayerHealth>();
        _spineAnimationState = GetComponent<Spine.Unity.SkeletonAnimation>().AnimationState;
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.skeleton;
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
       // Debug.DrawLine(transform.position + raycastColliderOffset, transform.position + raycastColliderOffset + Vector3.down * distanceFromGroundRaycast, Color.red);
       // Debug.DrawLine(transform.position - raycastColliderOffset, transform.position - raycastColliderOffset + Vector3.down * distanceFromGroundRaycast, Color.red);
       // Debug.DrawLine(transform.position, transform.position + Vector3.down * distanceFromGroundRaycast, Color.red);

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if(!gM.PauseStop || !stopInput)
        {
        horDir = Input.GetAxisRaw("Horizontal");
        vertDir = Input.GetAxisRaw("Vertical");

        if (isGrounded())
        {
            //Debug.Log("isGrounded(): " + isGrounded());
            lastTimeGround = coyoteTime; 
            isAttackingAir = false;
            canDoubleJump = true;
        
            rb.gravityScale = 1;
        }
        else
        {
            lastTimeGround -= Time.deltaTime;
            modifyPhysics();
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
 // Controllo se il personaggio è a contatto con un muro
 if(unlockWalljump)
 {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
isTouchingWall = Physics2D.Raycast(transform.position, direction, wallDistance, wallLayer);
 }

if (Input.GetButtonDown("Jump"))
{
            lastTimeJump = Time.time + jumpDelay;

        //Pre-interrupt jump if button released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            lastTimeGround = 0; //Avoid spam button
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);   
        }
    
    if (canDoubleJump && unlockDoubleJump)
    {
        // Double jump
        lastTimeJump = Time.time + jumpDelay;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        
        if(isTouchingWall)
        {
        canDoubleJump = true;
        wallSlide();
        }else 
        {
        canDoubleJump = false;
        }
    }
}



// Wallslide
        if (isTouchingWall && !isGrounded() && rb.velocity.y < 0 && unlockWalljump)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            wallSlidedown();
        }
        

        // Walljump
        if (Input.GetButtonDown("Jump") && isTouchingWall && unlockWalljump)
        {
           float horizontalVelocity = Mathf.Sign(transform.localScale.x) * wallJumpForce;
            rb.velocity = new Vector2(horizontalVelocity, jumpForce);
            wallJumped = true;
            canDoubleJump = true;
            Invoke("SetWallJumpedToFalse", 0.5f);
        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // controlla se il player è in aria e preme il tasto di attacco e il tasto direzionale basso
         if (!isGrounded() && Input.GetButtonDown("Fire1") && vertDir < 0)
        {
            isAttackingAir = true;
            DownAtk();

        } else  if (!isGrounded() && Input.GetButtonDown("Fire1") && vertDir > 0)
        {
            isAttackingAir = true;
            UpAtk();

        }      
                   
                    
                
                
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////             
               
// gestione dell'input dello sparo
if (Input.GetButtonDown("Fire2") && isBlast && Time.time >= nextAttackTime)
{
    //if (Less.currentMana > 0)
    //{
        //Animazione
        useMagic();   
        Stop();
    //}
    isBlast = false;
    nextAttackTime = Time.time + 1f / attackRate;
}

// ripristina la possibilità di attaccare dopo il tempo di attacco
if (!isBlast && Time.time >= nextAttackTime)
{
    isBlast = true;
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Scelta della skill dal menu rapido

if (Input.GetButtonDown("SlotUp"))
{
   UpdateMenuRapido.Instance.Selup();
}else if (Input.GetButtonDown("SlotRight"))
{
      UpdateMenuRapido.Instance.Selright();

}else if (Input.GetButtonDown("SlotLeft"))
{
      UpdateMenuRapido.Instance.Selleft();

}else if (Input.GetButtonDown("SlotBottom"))
{
      UpdateMenuRapido.Instance.Selbottom();

}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

        if (Input.GetButtonDown("Fire1"))
        {
            if(!isAttackingAir)
            {
            //Se non sta facendo un attacco caricato
            if(!isCharging)
            {
            isAttacking = true;
            AddCombo();
            if(comboCount == 5)
            { comboCount = 0;}
            }
            }

        }
    
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

 if (Input.GetButtonDown("Fire3") && !isCharging && Time.time - timeSinceLastAttack > attackRate)
    {
        isCharging = true;
        AnimationCharge();
        Stop();
        // Inizializza il timer al tempo massimo
        currentTime = timeLimit;
        InvokeRepeating("CountDown", 1f, 1f);
    }

    if (Input.GetButtonDown("Fire3") && isCharging)
    {
        Stop();
        // Decrementa il timer di un secondo
        currentTime--;
        // Aggiorna il danno dell'attacco in base al tempo rimanente
        Damage = minDamage + (maxDamage - minDamage) * currentTime / timeLimit;
    }

    if (Input.GetButtonUp("Fire3") && isCharging)
    {
        if (currentTime == 0)
        {
            Damage = maxDamage;
        }
        else
        {
            Damage = minDamage + (maxDamage - minDamage) * currentTime / timeLimit;
        }
        AnimationChargeRelease();
        isCharging = false;
        Debug.Log("Charge ratio: " + (float)currentTime / timeLimit + ", Damage: " + Damage);
        timeSinceLastAttack = Time.time;
        CancelInvoke("CountDown");
    }

    if (isCharging)
    {
        Stop();
    }
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

if (Input.GetButton("Dash")&& !dashing && coolDownTime <= 0 && unlockDash)
        {
            dashing = true;
            coolDownTime = dashCoolDown;
            dashTime = dashDuration;
            dashAnm();
        }

        if (coolDownTime > 0)
        {
            coolDownTime -= Time.deltaTime;
        }

 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

        }
        else if (gM.PauseStop || stopInput)
        {//Bloccato
        }

        // gestione dell'input del Menu 
        if (Input.GetButtonDown("Pause") && !stopInput)
        {
            gM.Pause();
            StopinputTrue();
            //InventoryManager.Instance.ListItems();
            Stop();
        }
        else if(Input.GetButtonDown("Pause") && stopInput)
        {
            gM.Resume();
            StopinputFalse();
        }

        checkFlip();
        moving();
    }

 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

    private void FixedUpdate()
    {
        if(!gM.PauseStop || !isAttacking || !isCharging || !touchGround || !isDashing)
        {
        float playerSpeed = horDir * speed;
        float accelRate = Mathf.Abs(playerSpeed) > 0.01f? acceleration : deceleration;
        rb.AddForce((playerSpeed - rb.velocity.x) * accelRate * Vector2.right);
        rb.velocity = new Vector2(Vector2.ClampMagnitude(rb.velocity, speed).x, rb.velocity.y); //Limit velocity
        
        if (lastTimeJump > Time.time && lastTimeGround > 0)
           { 
            jump();
           }
        if (dashing || Atkdashing)
        {
            if (horDir < 0)
        {

           rb.AddForce(-transform.right * dashForce, ForceMode2D.Impulse);
            dashTime -= Time.deltaTime;
        }
        else if (horDir > 0)
        {
            //anim.SetTrigger("Dash");

            rb.AddForce(transform.right * dashForce, ForceMode2D.Impulse);
            dashTime -= Time.deltaTime;
        }
        else if (horDir == 0)
        {
            if (rb.transform.localScale.x == -1)
        {

           rb.AddForce(-transform.right * dashForce, ForceMode2D.Impulse);
            dashTime -= Time.deltaTime;
        }
        else if (rb.transform.localScale.x == 1)
        {
            //anim.SetTrigger("Dash");

            rb.AddForce(transform.right * dashForce, ForceMode2D.Impulse);
            dashTime -= Time.deltaTime;
        }
                //dashing = false;
                //Atkdashing = false;
        }

            if (dashTime <= 0)
            {
                dashing = false;
                Atkdashing = false;

            }
        }

        if (attackNormal)
        {
            if (horDir < 0)
        {

           rb.AddForce(-transform.right * dashForceAtk, ForceMode2D.Impulse);
            dashTime -= Time.deltaTime;
        }
        else if (horDir > 0)
        {
            //anim.SetTrigger("Dash");

            rb.AddForce(transform.right * dashForceAtk, ForceMode2D.Impulse);
            dashTime -= Time.deltaTime;
        }
        else if (horDir == 0)
        {
                dashing = false;
                attackNormal = false;
        }

            if (dashTime <= 0)
            {
                dashing = false;
                attackNormal = false;

            }
        }





    }
    }

// Metodo per ripristinare il valore di wallJumped dopo 0.5 secondi
    void SetWallJumpedToFalse()
    {
        wallJumped = false;
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

//Se può fare walljump annulla la gravità
    if (canWallJump)
    {
        rb.velocity = new Vector2(horDir, 0f);
        rb.gravityScale = 0f; // disattiva la gravità durante il wall jump
    }
    else if (!canWallJump)
    {
        rb.gravityScale = gravityOnFall;

    }
}

public void StopinputTrue()
{
   stopInput = true;   
}
public void StopinputFalse()
{
    stopInput = false;   
}

    private bool isGrounded()
{
    //TRIPLE RAYCAST FOR GROUND: check if you touch the ground even with just one leg 
    return (
        
            Physics2D.Raycast(transform.position + raycastColliderOffset, Vector3.down, distanceFromGroundRaycast, groundLayer)
            ||
            Physics2D.Raycast(transform.position - raycastColliderOffset, Vector3.down, distanceFromGroundRaycast, groundLayer)
            ||
            Physics2D.Raycast(transform.position, Vector3.down, distanceFromGroundRaycast, groundLayer)
            
        );
}
    
    private void checkFlip()
    {
        if (horDir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horDir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

 public void Stop()
    {
        rb.velocity = new Vector2(0f, 0f);
        horDir = 0;

    }


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public void wallJump()
{
    if (currentAnimationName != walljumpAnimationName)
                {
                    _spineAnimationState.SetAnimation(1, walljumpAnimationName, true);
                    currentAnimationName = walljumpAnimationName;
                                        _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
            _spineAnimationState.GetCurrent(1).Complete += OnJumpAnimationComplete;
    
}

public void UpAtk()
{
    if (currentAnimationName != upatkjumpAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, upatkjumpAnimationName, true);
                    currentAnimationName = upatkjumpAnimationName;
                                        _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
            _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
    
}

public void DownAtk()
{
    if (currentAnimationName != downatkjumpAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, downatkjumpAnimationName, true);
                    currentAnimationName = downatkjumpAnimationName;
                                        _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
            _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
    
}


private void wallSlide()
    {
        if (currentAnimationName != walljumpAnimationName) {
            _spineAnimationState.SetAnimation(1, walljumpAnimationName, true);
            currentAnimationName = walljumpAnimationName;
        }
    }

private void wallSlidedown()
    {
        if (currentAnimationName != walljumpdownAnimationName) {
            _spineAnimationState.SetAnimation(1, walljumpdownAnimationName, true);
            currentAnimationName = walljumpdownAnimationName;
        }
    } 

private void notWallSlide()
{
    if (currentAnimationName == jumpDownAnimationName || currentAnimationName == jumpAnimationName) {
        _spineAnimationState.SetAnimation(0, jumpDownAnimationName, false);
        currentAnimationName = jumpDownAnimationName;
        var currentAnimation = _spineAnimationState.GetCurrent(1);
        if (currentAnimation != null) {
            currentAnimation.Complete += OnJumpAnimationComplete;
        }
    }            
}

private void OnJumpAnimationComplete(Spine.TrackEntry trackEntry)
{
    // Remove the event listener
    trackEntry.Complete -= OnJumpAnimationComplete;

    // Clear the track 1 and reset to the idle animation
    _spineAnimationState.ClearTrack(1);
    _spineAnimationState.SetAnimation(1, idleAnimationName, true);
    currentAnimationName = idleAnimationName;

     // Reset the attack state
    isAttacking = false;
    isAttackingAir = false;
}


public void AnimationCharge()
{
    if (currentAnimationName != chargeAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, chargeAnimationName, true);
                    currentAnimationName = chargeAnimationName;
                         _spineAnimationState.Event += HandleEvent;
                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
               // _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

public void dashAnm()
{
    if (currentAnimationName != dashAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, dashAnimationName, false);
                    currentAnimationName = dashAnimationName;
                                        _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
            _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

public void useMagic()
{
    if (currentAnimationName != blastAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, blastAnimationName, false);
                    currentAnimationName = blastAnimationName;
                    Blast();
                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}




public void AnimationChargeRelease()
{
    if (currentAnimationName != pesanteAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, pesanteAnimationName, false);
                    currentAnimationName = pesanteAnimationName;
                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

void CountDown()
{
    currentTime--;
    if (currentTime == 0)
    {
        Damage = maxDamage;
        AnimationChargeRelease();
        isCharging = false;
        Debug.Log("Charge ratio: 1.0, Damage: " + Damage);
        timeSinceLastAttack = Time.time;
        CancelInvoke("CountDown");
    }
}

public void AddCombo()
{
    //Se sta attaccando
    if (isAttacking)
    {
        //Il contatore aumenta ogni volta che si preme il tasto
        comboCount++;

        switch (comboCount)
        {
            //Setta lo stato d'animazione ed esegue l'animazione in base al conto della combo
            case 1:
                if (currentAnimationName != attackAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, attackAnimationName, false);
                    currentAnimationName = attackAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            case 2:
                if (currentAnimationName != attack_hAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, attack_hAnimationName, false);
                    currentAnimationName = attack_hAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            case 3:
                if (currentAnimationName != attack_lAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, attack_lAnimationName, false);
                    currentAnimationName = attack_lAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            case 4:
                if (currentAnimationName != attack_aAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, attack_aAnimationName, false);
                    currentAnimationName = attack_aAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            default:
                break;
        }
    }
}
private void OnAttackAnimationComplete(Spine.TrackEntry trackEntry)
{
    // Remove the event listener
    trackEntry.Complete -= OnAttackAnimationComplete;

    // Clear the track 1 and reset to the idle animation
    _spineAnimationState.ClearTrack(2);
    _spineAnimationState.SetAnimation(1, idleAnimationName, true);
    currentAnimationName = idleAnimationName;

     // Reset the attack state
    isAttacking = false;
    isAttackingAir = false;

}


public void AnimationRest()
{
    if (currentAnimationName != RestAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, RestAnimationName, false);
                    currentAnimationName = RestAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}
public void animationWakeup()
{
    if (currentAnimationName != UpAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, UpAnimationName, false);
                    currentAnimationName = UpAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

public void respawnWakeup()
{
    if (currentAnimationName != respawnAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, respawnAnimationName, false);
                    currentAnimationName = respawnAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

private void moving() {
    if(!isTouchingWall)
    {
    switch (rb.velocity.y) {
        case 0:
            float speed = Mathf.Abs(rb.velocity.x);
            if (speed == 0) {
                // Player is not moving
                if (currentAnimationName != idleAnimationName) {
                    _spineAnimationState.SetAnimation(1, idleAnimationName, true);
                    currentAnimationName = idleAnimationName;
                }
            } else if (speed > runSpeedThreshold) {
                // Player is running
                if (currentAnimationName != runAnimationName) {
                    _spineAnimationState.SetAnimation(1, runAnimationName, true);
                    currentAnimationName = runAnimationName;
                }
            } else {
                // Player is walking
                if (currentAnimationName != walkAnimationName) {
                    _spineAnimationState.SetAnimation(1, walkAnimationName, true);
                    currentAnimationName = walkAnimationName;
                }
            }
            break;

        case > 0:
            // Player is jumping
            
            if (currentAnimationName != jumpAnimationName) {
                _spineAnimationState.SetAnimation(1, jumpAnimationName, true);
                currentAnimationName = jumpAnimationName;
            }
            
            break;

        case < 0:
            // Player is falling
            
            if (currentAnimationName != jumpDownAnimationName) {
                _spineAnimationState.SetAnimation(1, jumpDownAnimationName, true);
                currentAnimationName = jumpDownAnimationName;
            }
            
            break;
    }
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void SetBulletPrefab(GameObject newBullet)
    //Funzione per cambiare arma
    {
       bullet = newBullet;
    }    
    
void Blast()
{
        isBlast = true;
        Debug.Log("il blast è partito");
        //nextAttackTime = Time.time + 1f / attackRate;
        Smagic.Play();
        Instantiate(blam, gun.position, transform.rotation);
        Instantiate(bullet, gun.position, transform.rotation);
        
        
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


void HandleEvent (TrackEntry trackEntry, Spine.Event e) {

if (e.Data.Name == "VFXpesante") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(pesante, slashpoint.position, transform.rotation);
    }


    if (e.Data.Name == "SoundSlash") {     
    // Controlla se la variabile "SwSl" è stata inizializzata correttamente.
        if (SwSl == null) {
            Debug.LogError("AudioSource non trovato");
            return;
        }
        // Assicurati che l'oggetto contenente l'AudioSource sia attivo.
        if (!SwSl.gameObject.activeInHierarchy) {
            SwSl.gameObject.SetActive(true);
        }
        // Imposta la pitch dell'AudioSource in base ai valori specificati.
        SwSl.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        // Assegna la clip audio all'AudioSource e avviala.
        SwSl.Play();
    }
    
    if (e.Data.Name == "VFXSlash") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(attack, slashpoint.position, transform.rotation);
    }

if (e.Data.Name == "VFXSlash_h") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(attack_h, slashpoint.position, transform.rotation);
        // Controlla se la variabile "SwSl" è stata inizializzata correttamente.
    }
if (e.Data.Name == "VFXSlash_l") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(attack_l, slashpoint.position, transform.rotation);
    }
    if (e.Data.Name == "VFXSlash_a") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(attack_a, slashpoint.position, transform.rotation);
    }

if (e.Data.Name == "soundWalk") {
        // Imposta la pitch dell'AudioSource in base ai valori specificati.
        Swalk.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        // Assegna la clip audio all'AudioSource e avviala.
        Swalk.Play();
    }
if (e.Data.Name == "soundRun") {
        // Imposta la pitch dell'AudioSource in base ai valori specificati.
        Srun.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        // Assegna la clip audio all'AudioSource e avviala.
        Srun.Play();
    }
if (e.Data.Name == "SoundCharge") {
            
        Instantiate(charge, transform.position, transform.rotation);

        // Imposta la pitch dell'AudioSource in base ai valori specificati.

        Scharge.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        // Assegna la clip audio all'AudioSource e avviala.
        Scharge.Play();
    }
if (e.Data.Name == "dash") {
            
        Sdash.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        // Assegna la clip audio all'AudioSource e avviala.
        Sdash.Play();
    }
}

    #region Gizmos
private void OnDrawGizmos()
    {
    Gizmos.color = Color.red;
    // disegna un Gizmo che rappresenta il Raycast
    Gizmos.DrawLine(transform.position, transform.position + new Vector3(transform.localScale.x, 0, 0) * wallDistance);
    }
#endregion

#if(UNITY_EDITOR)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + raycastColliderOffset, transform.position + raycastColliderOffset + Vector3.down * distanceFromGroundRaycast);
        Gizmos.DrawLine(transform.position - raycastColliderOffset, transform.position - raycastColliderOffset + Vector3.down * distanceFromGroundRaycast);
    }
#endif
}


