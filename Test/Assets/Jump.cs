using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Jump : MonoBehaviour
{
    [SerializeField] public float jumpForce = 5f; // forza del salto
    private int jumpCounter = 0;
    private int maxJumps = 2;
    private float jumpDuration = 2f;
    public float fallMultiplier = 2.5f;
    [SerializeField] public float jumpHeight = 2f;
    float coyoteTime = 0.1f;
    float coyoteCounter = 0f;
    bool isGrounded = true;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("HP")]
    [SerializeField]public float health = 100f; // salute del personaggio
    PlayerHealth Less;
    Attacks Atk;

    [Header("Animations")]
    private SkeletonAnimation skeletonAnimation;
    private string currentAnimationName;
    public string idleAnimationName;
    public string jump_startAnimationName;
    public string jumpAnimationName;
    public string fallAnimationName;
    public string landingAnimationName;


    [Header("VFX")]
    // Variabile per il gameobject del proiettile
    [SerializeField] GameObject Circle;
    [SerializeField] GameObject dust;
    [SerializeField] GameObject DustImp;

    [SerializeField] public Transform circlePoint;
    [Header("Abilitations")]
    [SerializeField] public GameplayManager gM;
    private bool IsKnockback = false;
    private bool stopInput = false;
    private bool isJumping = false; // vero se il personaggio sta saltando
    private bool isFall = false; // vero se il personaggio sta saltando
    private bool isHurt = false; // vero se il personaggio sta saltando
    private bool isLoop = false; // vero se il personaggio sta saltando
    private bool isAttacking = false; // vero se il personaggio sta attaccando
    private bool isCrouching = false; // vero se il personaggio sta attaccando
    public bool isLanding = false; // vero se il personaggio sta attaccando
    private bool isRunning = false; // vero se il personaggio sta correndo
    public Rigidbody2D rb; // componente Rigidbody2D del personaggio
    [SerializeField] public static bool playerExists;
    [SerializeField] public bool blockInput = false;

    [Header("Audio")]
    [SerializeField] AudioSource Smagic;
    
    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true; // vero se il personaggio sta saltando
        rb = GetComponent<Rigidbody2D>();
        if (gM == null)
        {
            gM = GetComponent<GameplayManager>();
        }
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        currentAnimationName = idleAnimationName;
    }

    // Update is called once per frame
    void Update()
{
    if (!gM.PauseStop)
    {
        // Utilizzare un raycast per determinare se il personaggio è su un terreno solido o meno
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        // Se il personaggio non è più su un terreno solido, aumentare il contatore coyote
        if (!isGrounded)
        {
            coyoteCounter += Time.deltaTime;
        }else
        {
            isFall = false;
            isJumping = false;
            
        }

        // Se il pulsante di salto viene premuto e il personaggio ha ancora salti disponibili o sta ancora entro il tempo di coyote, saltare
        if (Input.GetButtonDown("Jump") && (jumpCounter < maxJumps || coyoteCounter < coyoteTime))
        {
            isJumping = true;
            StartCoroutine(Jumpstart());
            jumpCounter++;
            rb.velocity = Vector2.up * Mathf.Sqrt(jumpHeight * -2f * Physics2D.gravity.y);
            
            if (jumpCounter == 2)
            {
                Smagic.Play();
                Instantiate(Circle, circlePoint.transform.position, transform.rotation);
            }

            StartCoroutine(JumpDurationCoroutine(jumpDuration));
        }

        // Se la velocità verticale del personaggio è negativa, attivare la modalità di caduta
        if (rb.velocity.y < 0)
        {
            isFall = true;
            skeletonAnimation.AnimationName = fallAnimationName;
            isLoop = true;
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Se viene premuto il pulsante di attacco e il personaggio sta saltando, attaccare
        if (Input.GetButtonDown("Fire1") && isJumping)
        {
            Atk.isAttacking = true;
            isJumping = false;
            skeletonAnimation.AnimationName = fallAnimationName;
            isFall = true;
            isLoop = true;
        }

    }
}

#region Gizmos
private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }
#endregion

void OnCollisionEnter2D(Collision2D collision) 
{
    if(collision.gameObject.tag == "Ground")
    {
        jumpCounter = 0;
        skeletonAnimation.AnimationName = landingAnimationName;
        StartCoroutine(stopPlayer());
    }
}

    IEnumerator stopPlayer()
{
isLanding = true; 
skeletonAnimation.AnimationName = landingAnimationName;
yield return new WaitForSeconds(0.5f);
skeletonAnimation.AnimationName = idleAnimationName;
isLanding = false;    
}
    
    IEnumerator Jumpstart()
{
skeletonAnimation.AnimationName = jump_startAnimationName;
yield return new WaitForSeconds(0.5f);
skeletonAnimation.AnimationName = jumpAnimationName;
}

    private IEnumerator JumpDurationCoroutine(float duration)
{
    float timer = 0f;
    while (timer < duration)
    {
        timer += Time.deltaTime;
        yield return null;
    }

    isJumping = false;
}

            

            

}
