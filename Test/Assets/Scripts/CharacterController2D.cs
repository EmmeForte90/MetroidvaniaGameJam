using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;


public class CharacterController2D : MonoBehaviour
{
   public float moveSpeed = 5f; // velocità di movimento
    public float jumpForce = 5f; // forza del salto
    public float attackDamage = 10f; // danno dell'attacco
    public float runMultiplier = 2f; // moltiplicatore di velocità per la corsa
    private int jumpCounter = 0;
    private int maxJumps = 2;
    public float health = 100f; // salute del personaggio
private bool isGrounded;
public LayerMask LayerMask;
public static bool playerExists;
public bool blockInput = false;
public float attackCooldown = 0.5f; // tempo di attesa tra gli attacchi
    public float comboTimer = 2f; // tempo per completare una combo
    public int comboCounter = 0; // contatore delle combo
    public int maxCombo = 3; // numero massimo di combo
        public float shootTimer = 2f; // tempo per completare una combo

    PlayerHealth Less;

    private Animator anim; // componente Animator del personaggio
    private float currentCooldown; // contatore del cooldown attuale
    [SerializeField] float nextAttackTime = 0f;
    [SerializeField] float attackRate = 2f;
[Header("VFX")]
    [SerializeField] private GameObject bullet;
    // Variabile per il gameobject del proiettile
    [SerializeField] GameObject blam;
        [SerializeField] public Transform gun;

    [SerializeField] GameObject Circle;
    [SerializeField] public Transform circlePoint;
    public GameplayManager gM;
    private bool stopInput = false;
    private bool isJumping = false; // vero se il personaggio sta saltando
    private bool isAttacking = false; // vero se il personaggio sta attaccando
    private bool isLanding = false; // vero se il personaggio sta attaccando
    private bool isRunning = false; // vero se il personaggio sta correndo
    private float currentSpeed; // velocità corrente del personaggio
    private Rigidbody2D rb; // componente Rigidbody2D del personaggio
    public GameObject Slash;
    public GameObject Slash1;
    public Transform slashpoint;
    public SkeletonMecanim skeletonM;
    public float moveX;

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
        Less = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        if (gM == null)
        {
            gM = GetComponent<GameplayManager>();
        }
        
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
        if(!gM.PauseStop)
        {
        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if(isGrounded)
        {
//            anim.SetTrigger("Isground");
        }
        // gestione dell'input del movimento
        moveX = Input.GetAxis("Horizontal");
        currentSpeed = moveSpeed;
        if (isRunning && !isAttacking)
        {
            currentSpeed *= runMultiplier;
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



// gestione dell'input dello sparo
        if (Input.GetButtonDown("Fire2"))
{
Blast();
    
}


        // gestione dell'input del salto
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
{
    isJumping = true;
    rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    jumpCounter++;
    if(jumpCounter == 2)
    {
        Instantiate(Circle, circlePoint.position, transform.rotation);

    }
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
        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("IsJumping", isJumping);
//        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("IsRunning", isRunning);
        }

        // gestione dell'input del Menu

        
    if (Input.GetKeyDown(KeyCode.Escape) && !stopInput)
    {
        gM.Pause();
        stopInput = true;
        //myAnimator.SetTrigger("idle");
        //SFX.Play(0);
        rb.velocity = new Vector2(0f, 0f);
    }
    else if(Input.GetKeyDown(KeyCode.Escape) && stopInput)
    {
        gM.Resume();
        stopInput = false;
        //SFX.Play(0);
    }

    }

    
void Blast()
{
    if(Less.currentMana > 0)
        {
if (Time.time > nextAttackTime)
        {
        isAttacking = true;
        nextAttackTime = Time.time + 1f / attackRate;
        anim.SetTrigger("isShoot");
        //AudioManager.instance.PlaySFX(1);
        Instantiate(blam, gun.position, transform.rotation);
        Instantiate(bullet, gun.position, transform.rotation);
        //PlayerBulletCount.instance.removeOneBullet();
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
            anim.SetInteger("ComboCounter", comboCounter);
            anim.SetTrigger("Attack1");
            if (comboCounter == 1)
            {
                slash();
            }else if (comboCounter == 2)
            {
                cutting();
            }else if (comboCounter == 3)
            {

            }
            currentCooldown = attackCooldown;
            comboTimer = 0.5f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            jumpCounter = 0;
            StartCoroutine(stopPlayer());

        }
    }

IEnumerator stopPlayer()
{
isLanding = true;    
yield return new WaitForSeconds(0.5f);
isLanding = false;    
}

public void slash()
{
        Instantiate(Slash, slashpoint.transform.position, transform.rotation);

} 
public void cutting()
{
        Instantiate(Slash1, slashpoint.transform.position, transform.rotation);

} 
    

public void TakeDamage(float damage)
    {
        // sottrai danno dalla salute del personaggio
        health -= damage;
        // attiva un'animazione di danno (se presente)
        anim.SetTrigger("TakeDamage");

        // controlla se la salute è minore o uguale a 0 e gestisce la morte del personaggio
        if (health <= 0f)
        {
            // attiva un'animazione di morte (se presente)
            anim.SetTrigger("Die");
            // distrugge il personaggio
            Destroy(gameObject);
        }
    }

#region CambioArma
    public void SetBulletPrefab(GameObject newBullet)
    //Funzione per cambiare arma
    {
       bullet = newBullet;
    }    
    
#endregion

}



           


