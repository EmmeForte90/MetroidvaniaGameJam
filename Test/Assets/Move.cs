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
    public bool isWallSliding;
    public bool canJumpAgain;
    public float wallJumpForce;
    private Vector2 wallJumpDirection;
    public float wallCheckDistance = 0.4f;
    //public LayerMask wallLayer;
    public bool canWallJump = false;



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
    [SpineAnimation][SerializeField] private string dashAnimationName;
    [SpineAnimation][SerializeField] private string pesanteAnimationName;







    public enum CharacterState {
    Idle,
    Walking,
    Running,
    Jumping,
    Falling,
    Attacking,
    Attacking1,
    Attacking2,
    Attacking3,
    Attacking4,
    Attacking5,
    Blasting,
    wallJumping,
    Landing,
    Dash,

}

private CharacterState currentState = CharacterState.Idle;
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
    public bool isBlast = false; // vero se il personaggio sta attaccando

    private bool stopInput = false;

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

    private string currentAnimationName;


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

        if(!gM.PauseStop)
        {
        horDir = Input.GetAxisRaw("Horizontal");

        if (isGrounded())
        {
            //Debug.Log("isGrounded(): " + isGrounded());
            lastTimeGround = coyoteTime; 
            canDoubleJump = true;
            rb.gravityScale = 1;
        }
        else
        {
            lastTimeGround -= Time.deltaTime;
            modifyPhysics();
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
if (Input.GetButtonDown("Jump"))
{
   if (lastTimeGround + groundDelay > Time.time)
    {
        // Regular jump
        lastTimeJump = Time.time + jumpDelay;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    else if (canDoubleJump)
    {
        // Double jump
        lastTimeJump = Time.time + jumpDelay;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canDoubleJump = false;
    }
}
else if (Input.GetButtonDown("Jump") && canWallJump)
{
    rb.velocity = new Vector2(-wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce);
    canJumpAgain = true;
    isWallSliding = false;
}



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // gestione dell'input dello sparo
if (Input.GetButtonDown("Fire2") && isBlast && Time.time >= nextAttackTime && !canWallJump)
{
    if (Less.currentMana > 0)
    {
        useMagic();   
        Stop();
    }
    isBlast = false;
    nextAttackTime = Time.time + 1f / attackRate;
}

// ripristina la possibilità di attaccare dopo il tempo di attacco
if (!isBlast && Time.time >= nextAttackTime)
{
    isBlast = true;
}
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

        if (Input.GetButtonDown("Fire1") && !canWallJump)
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
    
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

 if (Input.GetButtonDown("Fire3") && !isCharging && Time.time - timeSinceLastAttack > attackRate && !canWallJump)
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

if (Input.GetButton("Dash")&& !dashing && coolDownTime <= 0)
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

        // gestione dell'input del Menu 
        if (Input.GetButtonDown("Pause") && !stopInput)
        {
            gM.Pause();
            stopInput = true;
            Stop();
        }
        else if(Input.GetButtonDown("Pause") && stopInput)
        {
            gM.Resume();
            stopInput = false;
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

RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), wallCheckDistance, groundLayer);
        if (hit.collider != null && hit.collider.tag == "Ground" && !isGrounded()) {
            wallSlide();
            canWallJump = true;
        } else if (hit.collider != null && hit.collider.tag != "Ground" && !isGrounded()) {
            notWallSlide();
            canWallJump = false;
        } else {
            notWallSlide();
            canWallJump = false;
        }



    }
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

 private void Stop()
    {
        rb.velocity = new Vector2(0f, 0f);
        horDir = 0;

    }


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public void wallJump()
{
    if (currentState != CharacterState.wallJumping)
                {
                    _spineAnimationState.SetAnimation(1, walljumpAnimationName, true);
                    currentState = CharacterState.wallJumping;
                                        _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
            _spineAnimationState.GetCurrent(1).Complete += OnJumpAnimationComplete;
    
}

private void wallSlide()
    {
        if (currentState != CharacterState.wallJumping) {
            _spineAnimationState.SetAnimation(1, walljumpAnimationName, true);
            currentState = CharacterState.wallJumping;
        }
    }

   
private void notWallSlide()
{
    if (currentState == CharacterState.Falling || currentState == CharacterState.Jumping) {
        _spineAnimationState.SetAnimation(0, jumpDownAnimationName, false);
        currentState = CharacterState.Falling;
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
    currentState = CharacterState.Idle;

     // Reset the attack state
    isAttacking = false;
}


public void AnimationCharge()
{
    if (currentState != CharacterState.Attacking5)
                {
                    _spineAnimationState.SetAnimation(2, chargeAnimationName, true);
                    currentState = CharacterState.Attacking;
                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
               // _spineAnimationState.GetCurrent(1).Complete += OnAttackAnimationComplete;
}

public void dashAnm()
{
    if (currentState != CharacterState.Dash)
                {
                    _spineAnimationState.SetAnimation(2, dashAnimationName, false);
                    currentState = CharacterState.Dash;
                                        _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
            _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

public void useMagic()
{
    if (currentState != CharacterState.Blasting)
                {
                    _spineAnimationState.SetAnimation(2, blastAnimationName, false);
                    currentState = CharacterState.Blasting;
                    Blast();
                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}




public void AnimationChargeRelease()
{
    if (currentState != CharacterState.Attacking4)
                {
                    _spineAnimationState.SetAnimation(1, pesanteAnimationName, false);
                    currentState = CharacterState.Attacking4;
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
                if (currentState != CharacterState.Attacking)
                {
                    Spine.TrackEntry trackEntry = _spineAnimationState.SetAnimation(2, attackAnimationName, false);
                    currentState = CharacterState.Attacking;
                    _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            case 2:
                if (currentState != CharacterState.Attacking1)
                {
                    _spineAnimationState.SetAnimation(2, attack_hAnimationName, false);
                    currentState = CharacterState.Attacking1;
                                        _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            case 3:
                if (currentState != CharacterState.Attacking2)
                {
                    _spineAnimationState.SetAnimation(2, attack_lAnimationName, false);
                    currentState = CharacterState.Attacking2;
                                        _spineAnimationState.Event += HandleEvent;

                    //Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
                break;
            case 4:
                if (currentState != CharacterState.Attacking3)
                {
                    _spineAnimationState.SetAnimation(2, attack_aAnimationName, false);
                    currentState = CharacterState.Attacking3;
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
    currentState = CharacterState.Idle;

     // Reset the attack state
    isAttacking = false;
}




private void moving() {
    if(!canWallJump)
    {
    switch (rb.velocity.y) {
        case 0:
            float speed = Mathf.Abs(rb.velocity.x);
            if (speed == 0) {
                // Player is not moving
                if (currentState != CharacterState.Idle) {
                    _spineAnimationState.SetAnimation(1, idleAnimationName, true);
                    currentState = CharacterState.Idle;
                }
            } else if (speed > runSpeedThreshold) {
                // Player is running
                if (currentState != CharacterState.Running) {
                    _spineAnimationState.SetAnimation(1, runAnimationName, true);
                    currentState = CharacterState.Running;
                }
            } else {
                // Player is walking
                if (currentState != CharacterState.Walking) {
                    _spineAnimationState.SetAnimation(1, walkAnimationName, true);
                    currentState = CharacterState.Walking;
                }
            }
            break;

        case > 0:
            // Player is jumping
            
            if (currentState != CharacterState.Jumping) {
                _spineAnimationState.SetAnimation(1, jumpAnimationName, true);
                currentState = CharacterState.Jumping;
            }
            
            break;

        case < 0:
            // Player is falling
            
            if (currentState != CharacterState.Falling) {
                _spineAnimationState.SetAnimation(1, jumpDownAnimationName, true);
                currentState = CharacterState.Falling;
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
Gizmos.DrawLine(transform.position, transform.position + new Vector3(transform.localScale.x, 0, 0) * wallCheckDistance);
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


