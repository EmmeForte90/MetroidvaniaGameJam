using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class CharacterController2D : MonoBehaviour
{
    [Header("Move")]

    [SerializeField] public float moveSpeed = 5f; // velocità di movimento
    [SerializeField] public float jumpForce = 5f; // forza del salto
    [SerializeField] public float runMultiplier = 2f; // moltiplicatore di velocità per la corsa
    private int jumpCounter = 0;
    private int maxJumps = 2;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;
    Vector2 playerPosition;
    Vector2 HitPosition;
    public GameObject Hit;

    [Header("HP")]
    [SerializeField]public float health = 100f; // salute del personaggio
    PlayerHealth Less;


    [Header("Respawn")]
    //[HideInInspector]
    private Transform respawnPoint; // il punto di respawn del giocatore
    public string sceneName; // il nome della scena in cui si trova il punto di respawn
    
    [Header("Animations")]
    private Animator anim; // componente Animator del personaggio
    private int state = 0;

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

    public Transform slashpoint;

    [Header("VFX")]
    // Variabile per il gameobject del proiettile
    [SerializeField] GameObject blam;
    [SerializeField] public Transform gun;
    [SerializeField] GameObject Circle;
    [SerializeField] public Transform circlePoint;

    [Header("Abilitations")]
    [SerializeField] public GameplayManager gM;
    private bool IsKnockback = false;
    private bool stopInput = false;
    private bool isGrounded = false; // vero se il personaggio sta saltando
    private bool isJumping = false; // vero se il personaggio sta saltando
    private bool isHurt = false; // vero se il personaggio sta saltando
    private bool isLoop = false; // vero se il personaggio sta saltando
    private bool isAttacking = false; // vero se il personaggio sta attaccando
    private bool isLanding = false; // vero se il personaggio sta attaccando
    private bool isRunning = false; // vero se il personaggio sta correndo
    private float currentSpeed; // velocità corrente del personaggio
    private Rigidbody2D rb; // componente Rigidbody2D del personaggio
    [SerializeField] public static bool playerExists;
    [SerializeField] public bool blockInput = false;
   
    public SkeletonMecanim skeletonM;
    public float moveX;
[Header("Audio")]
[SerializeField] AudioSource SwSl;
[SerializeField] AudioSource Smagic;
[SerializeField] AudioSource SRun;



public static CharacterController2D instance;
public static CharacterController2D Instance
        {
            //Se non trova il componente lo trova in automatico
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<CharacterController2D>();
                return instance;
            }
        }

    void Start()
    {
        isGrounded = true; // vero se il personaggio sta saltando
        playerPosition = transform.position;
        HitPosition = Hit.transform.position;
        Less = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        if (gM == null)
        {
            gM = GetComponent<GameplayManager>();
        }
        SetState(0);
        anim = GetComponent<Animator>();
        currentCooldown = attackCooldown;
        
        if (playerExists) {
        Destroy(gameObject);
    }
    else {
        playerExists = true;
        DontDestroyOnLoad(gameObject);
    }


    }

    void Update()
    {
         

        if (Less.currentHealth <= 0)
        {
            Respawn();
        }

        if(!gM.PauseStop || IsKnockback)
        {

#region  Move
        moveX = Input.GetAxis("Horizontal");

        if (state == 0)
        {
        if (moveX != 0)
        { 
        SetState(1);
        }else if (moveX == 0)
        {
        anim.Play("Gameplay/idle");
        SetState(0);

        }
        }
        
        currentSpeed = moveSpeed;
        if (isRunning && !isAttacking)
        {
            if (moveX != 0)
    { 
    if (isRunning)
    {
        currentSpeed = moveSpeed * runMultiplier;
        SetState(2);
    }
    else
    {
        currentSpeed = moveSpeed;
        SetState(1);
    }
    }
    else
    {
    currentSpeed = 0;
    anim.Play("Gameplay/idle");
    SetState(0);
    }

        }
        if (isAttacking || isLanding)
        {   
        rb.velocity = new Vector2(0f, 0f);
        }else
        {
        rb.velocity = new Vector2(moveX * currentSpeed, rb.velocity.y);
        }

        if (moveX < 0)
        {
            moveX = -1;
        transform.localScale = new Vector2(-1f, 1f);
        }
        else if (moveX > 0)
        {
            moveX = 1;
        transform.localScale = new Vector2(1f, 1f);
        }
#endregion


// gestione dell'input dello sparo
if (Input.GetButtonDown("Fire2"))
{
Blast();   
}


        // gestione dell'input del salto
  if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
{
    SetState(3);
    isJumping = true;
    isGrounded = false; // vero se il personaggio sta saltando
    rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    jumpCounter++;
    if(jumpCounter == 2)
    {
        Smagic.Play();
        Instantiate(Circle, circlePoint.position, transform.rotation);
    }
}

if (isJumping)
{

    if (Input.GetButtonDown("Fire1"))
    {
        Attack();
        isAttacking = true;
        isJumping = false;
    }
    else
    {
        SetState(4);
        isLoop = true;
    }
   
    }

    if(!isGrounded &&!isAttacking && !isJumping)
    {
        SetState(12);
        isLoop = true;
    }



        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                isAttacking= false;
                shootTimer = 0.5f;
            }
        // gestione del timer della combo
        if (comboCounter > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                isAttacking= false;
                comboCounter = 0;
                comboTimer = 0.5f;
            }
        }

        // gestione del cooldown dell'attacco
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        // gestione dell'input della corsa
        if (Input.GetButton("Fire3"))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        // gestione dell'animazione del personaggio
        UpdateAnimation();
        }

        
// gestione dell'input del Menu 
        if (Input.GetButtonDown("Pause") && !stopInput)
        {
            gM.Pause();
            stopInput = true;
            //myAnimator.SetTrigger("idle");
            //SFX.Play(0);
            rb.velocity = new Vector2(0f, 0f);
        }
        else if(Input.GetButtonDown("Pause") && stopInput)
        {
            gM.Resume();
            stopInput = false;
        }
    }

    void UpdateAnimation()
    {
        switch (state)
        {
            case 0:
                anim.Play("Gameplay/idle");
                break;
            case 1:
                anim.Play("Gameplay/walk");
                break;
            case 2:
                anim.Play("Gameplay/run");
                break;
            case 3:
                anim.Play("Gameplay/jump_start");
                //state = 4;
                break;
            case 4:
                anim.Play("Gameplay/jump");
                break;
            case 5:
                anim.Play("Gameplay/landing");
                //state = 0;
                break;
            case 6:
                anim.Play("Gameplay/blast");
                break;
            case 7:
                anim.Play("CS/attack");
                break;
            case 8:
                anim.Play("CS/attack_h");
                break;
            case 9:
                anim.Play("CS/attack_l");
                break;
            case 10:
                anim.Play("Gameplay/hurt");
                break;
            case 11:
                anim.Play("Gameplay/die");
                break;
            case 12:
                anim.Play("Gameplay/fall");
                break;
        }
        var currentAnimInfo = anim.GetCurrentAnimatorStateInfo(0);
    if(!isLoop)
    {
    if (currentAnimInfo.normalizedTime >= 1)
    {
        state = 0;
    }
    }
    }

 public void SetState(int newState)
    {
        state = newState;
    }

void Blast()
{
    if(Less.currentMana > 0)
        {
if (Time.time > nextAttackTime)
        {
        isAttacking = true;
        nextAttackTime = Time.time + 1f / attackRate;
        Smagic.Play();
        SetState(6);
        Instantiate(blam, gun.position, transform.rotation);
        Instantiate(bullet, gun.position, transform.rotation);
        }
        
}
}
    void Attack()
    {
        if (currentCooldown <= 0)
        {
            isAttacking = true;
            comboCounter++;
            if (comboCounter > maxCombo)
            {
                comboCounter = 1;
            }
            if (comboCounter == 1)
            {
                        SetState(7);
            }else if (comboCounter == 2)
            {
                        SetState(8);
            }else if (comboCounter == 3)
            {
                        SetState(9);
            }
            currentCooldown = attackCooldown;
            comboTimer = 0.5f;
            
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // vero se il personaggio sta saltando
            isJumping = false;
            jumpCounter = 0;
            SetState(5);
            StartCoroutine(stopPlayer());

        }

    }

private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            RespawnObject respawnObject = collision.GetComponent<RespawnObject>();
            if (respawnObject != null)
            {
                respawnPoint = respawnObject.respawnPoint;
                sceneName = respawnObject.sceneName;
            }
        }


//Test per gestire il respawn
        if (collision.CompareTag("EditorOnly"))
        {
           Respawn();
        }
    }



IEnumerator stopPlayer()
{
isLanding = true; 
yield return new WaitForSeconds(0.5f);
isLoop = false;
isLanding = false;    
}

public void AnmHurt()
{
            SetState(10);
}

#region CambioMagia
    public void SetBulletPrefab(GameObject newBullet)
    //Funzione per cambiare arma
    {
       bullet = newBullet;
    }    
    
#endregion



    public void SoundSlash()
    {
        SwSl.Play();
    } 



public void Respawn()
{
    // Cambia la scena
    SceneManager.LoadScene(sceneName);

    // Aspetta che la nuova scena sia completamente caricata
    StartCoroutine(WaitForSceneLoad());
}

IEnumerator WaitForSceneLoad()
{
    yield return new WaitForSeconds(0);

    // Trova l'oggetto con il tag "respawn" nella nuova scena
    GameObject respawnPoint = GameObject.FindWithTag("Respawn");

    // Teletrasporta il giocatore alla posizione dell'oggetto "respawn"
    transform.position = respawnPoint.transform.position;
}
}



           


