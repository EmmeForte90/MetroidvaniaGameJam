using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class AiEnemysword : Health, IDamegable
{
 [Header("Enemy")]
[SerializeField] GameObject Brain;
private Health health;
private Transform player;
[SerializeField] LayerMask playerlayer;

[Header("Move")]
public float moveSpeed = 2f; // velocità di movimento
public float atckForward = 5; // velocità di movimento
private float pauseDuration = 0.5f; // durata della pausa
private float pauseTimer; // timer per la pausa
[SerializeField] private Vector3[] positions;
private int id_positions;
private float horizontal;
private bool direction_x = true;
private Rigidbody2D rb;

[Header("Attack")]
public float chaseSpeed = 4f; // velocità di inseguimento
public float WaitAfterAtk = 2f;
public float attackDamage = 10; // danno d'attacco
public float sightRadius = 5f; // raggio di vista del nemico
public float chaseThreshold = 2f; // soglia di distanza per iniziare l'inseguimento
public float attackrange = 2f;
public float attackCooldown = 2f; // durata del cooldown dell'attacco
private float attackTimer;


[Header("Abilitations")]
private bool isChasing = false; // indica se il nemico sta inseguendo il player
private bool isMove = false;
private bool isAttacking = false;
private bool isDie = false;
private bool pauseAtck = false;
private bool canAttack = true;
private bool firstattack = true;
private bool isPlayerInAttackRange = false;
private bool activeActions = true;

private float waitTimer = 0f;
private float waitDuration = 2f;

[Header("Knockback")]
    private bool kb = false;
    public float knockbackForce; // la forza del knockback
    public float knockbackTime; // il tempo di knockback
    public float jumpHeight; // l'altezza del salto
    public float fallTime; // il tempo di caduta

 [Header("Audio")]
    [HideInInspector] public float basePitch = 1f;
    [HideInInspector] public float randomPitchOffset = 0.1f;
    [SerializeField] AudioSource SwSl;
    [SerializeField] AudioSource Swalk;

[Header("VFX")]
    // Variabile per il gameobject del proiettile
    //[SerializeField] GameObject blam;
    //[SerializeField] public Transform gun;
    [SerializeField] public Transform slashpoint;
    [SerializeField] GameObject attack;
    [SerializeField] GameObject attack_h;



[Header("Animations")]
    [SpineAnimation][SerializeField] private string idleAnimationName;
    [SpineAnimation][SerializeField] private string walkAnimationName;
    [SpineAnimation][SerializeField] private string runAnimationName;
    [SpineAnimation][SerializeField] private string attackAnimationName;
    [SpineAnimation][SerializeField] private string hurtAnimationName;
    [SpineAnimation][SerializeField] private string diebackAnimationName;
    [SpineAnimation][SerializeField] private string diefrontAnimationName;
    
    private string currentAnimationName;
    public SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Spine.Skeleton _skeleton;
    Spine.EventData eventData;

private enum State { Move, Chase, Attack, Knockback, Dead, Hurt, Wait }
private State currentState;

public static AiEnemysword instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player").transform;
        _spineAnimationState = GetComponent<Spine.Unity.SkeletonAnimation>().AnimationState;
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.skeleton;
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (_skeletonAnimation == null) {
            Debug.LogError("Componente SkeletonAnimation non trovato!");
        }       
        rb = GetComponent<Rigidbody2D>();
       
    }
private void Update()
    {
        if (!GameplayManager.instance.PauseStop)
        {
            CheckState();

            switch (currentState)
            {
                case State.Move:
                    Move();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Attack:
                    Attack();
                    FacePlayer();
                    if(firstattack)
        {
        attackTimer = 0;
        firstattack = false;
        }
                    break;
                case State.Knockback:
                    Knockback();
                    break;
                case State.Dead:
                    break;
                case State.Hurt:
                    break;
                case State.Wait:
                Wait();
                    break;
            }
        }
    }


    private void CheckState()
{
    
//Hp finiti, muore
    if (health.currentHealth == 0)
    {
        isChasing = false;
        isAttacking = false;
        isMove = false;
        isDie = true;
        currentState = State.Dead;
        return;
    }
//Hp finiti, Indietreggia
    if (IsKnockback())
    {
        isChasing = false;
        isAttacking = false;
        isMove = false;
        isDie = false;
        currentState = State.Knockback;
        return;
    }

//Distanza per attaccare il player è dentro il raggio
    if (Vector2.Distance(transform.position, player.position) < attackrange)
    {
        isChasing = false;
        isAttacking = true;
        isMove = false;
        isDie = false;
        isPlayerInAttackRange = true;
        currentState = State.Attack;
        return;
    }

//Il player è appena uscito dal raggio e si avvia il timer di attesa prima che il nemico torni a inseguirlo
      if (Vector2.Distance(transform.position, player.position) > attackrange && isPlayerInAttackRange)
    {
        isChasing = false;
        isAttacking = false;
        isMove = false;
        isDie = false;
        activeActions = false;
        currentState = State.Wait;
        StartCoroutine(waitChase());
    }

//Distanza per inseguire
if(!isPlayerInAttackRange)
{
    if (Vector2.Distance(transform.position, player.position) < chaseThreshold)
    {
        isChasing = true;
        isAttacking = false;
        isMove = false;
        isDie = false;
        firstattack = true;
        currentState = State.Chase;
        return;
    }
}

//Il nemico entra in pausa se il player è troppo in alto
    if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.up), 5f, playerlayer) )
    {
        isChasing = false;
        isAttacking = false;
        isMove = false;
        currentState = State.Wait;
        return;
    }
//Altrimenti si muove in autonomia
    if(activeActions)
    {
    isMove = true;
    isChasing = false;
    isAttacking = false;
    isDie = false;
    currentState = State.Move;
    }
}

IEnumerator waitChase()
    {
        yield return new WaitForSeconds(WaitAfterAtk);
        isPlayerInAttackRange = false;
        activeActions = true;

    }

private void Wait()
{
    rb.velocity = new Vector3(0, 0);
    IdleAnm();
}

private void Move()
{
    // Controlla se l'oggetto deve essere in movimento
    if (isMove && !isAttacking)
    {
        // Controlla se l'oggetto deve spostarsi verso destra o sinistra
        if (transform.position.x < positions[id_positions].x)
        {
            horizontal = 1;
        }
        else 
        {
            horizontal = -1;
        }

        // Controlla se l'oggetto è arrivato alla posizione obiettivo corrente
        if (transform.position == positions[id_positions])
        {
            // Se è l'ultima posizione obiettivo, torna alla prima posizione
            if (id_positions == positions.Length - 1)
            {
                id_positions = 0;
            } 
            else
            {
                // Vai alla prossima posizione obiettivo
                id_positions++;
            }
        }

        // Controlla se è necessario fare una pausa
        if (pauseTimer > 0)
        {
            // Ferma l'animazione di movimento e attendi
            IdleAnm();
            pauseTimer -= Time.deltaTime;
            return;
        }

        // Sposta gradualmente l'oggetto verso la posizione obiettivo
        transform.position = Vector2.MoveTowards(transform.position, positions[id_positions], moveSpeed * Time.deltaTime);

        // Controlla se l'oggetto è arrivato alla posizione obiettivo corrente
        if (Vector2.Distance(transform.position, positions[id_positions]) < 0.1f)
        {
            // Imposta il timer di pausa
            pauseTimer = pauseDuration;
        }
    }
    else if (!isMove && !isAttacking) // Se l'oggetto non deve essere in movimento
    {
        // Ferma l'animazione di movimento e attiva l'animazione di idle
        IdleAnm();
    }

    // Inverte l'orientamento dell'oggetto in base alla sua direzione di movimento
    Flip();

    // Esegui l'animazione di movimento
    MovingAnm();
}

    
    

private void Flip()
    {
        if (direction_x && horizontal < 0f || !direction_x && horizontal > 0f)
        {
            direction_x = !direction_x;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }


private void Chase()
{
    
    if(isChasing && !isAttacking)
    {
    // inseguimento del giocatore
    if (player.transform.position.x > transform.position.x)
    {
        transform.localScale = new Vector2(1f, 1f);
    }
    else if (player.transform.position.x < transform.position.x)
    {
        transform.localScale = new Vector2(-1f, 1f);
    }
    Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

    transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

    ChaseAnm();
    }
}

private void Attack()
{
    if (isAttacking)
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            canAttack = false;
            return;
        }
        else
        {
            canAttack = true;
        }

        if (canAttack && Vector2.Distance(transform.position, player.position) < attackrange)
        {
            AttackAnm();
            //player.GetComponent<PlayerHealth>().Damage(attackDamage);
            attackTimer = attackCooldown;
        }
    }
}


void FacePlayer()
    {
        if (player != null)
        {
            if (player.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
private IEnumerator JumpBackCo(Rigidbody2D rb)
    {

        if (rb != null)
        {
            kb = true;
            Vector2 knockbackDirection = new Vector2(0f, jumpHeight); // direzione del knockback verso l'alto
            if (rb.transform.position.x < player.transform.position.x) // se la posizione x del nemico è inferiore a quella del player
                knockbackDirection = new Vector2(-1, jumpHeight); // la direzione del knockback è verso sinistra
            else if (rb.transform.position.x > player.transform.position.x) // se la posizione x del nemico è maggiore a quella del player
                knockbackDirection = new Vector2(1, jumpHeight); // la direzione del knockback è verso destra
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse); // applica il knockback
            yield return new WaitForSeconds(knockbackTime); // aspetta il tempo di knockback
            kb = false;
        }
    }


    
private void Knockback()
{
    // gestione del knockback
    // utilizzare la fisica di Unity per gestire la forza e il tempo di knockback
}

private bool IsKnockback()
{
    // controllo se il nemico è in knockback
    return kb;
}

#region Gizmos
private void OnDrawGizmos()
    {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, chaseThreshold);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, attackrange);
        //Debug.DrawRay(transform.position, new Vector3(chaseThreshold, 0), Color.red);
    }
#endregion


   public void Damage(int damage)
    {
        health.currentHealth -= damage;
        
    }

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//ANIMATIONS

public void AttackAnm()
{
            
   
    if (currentAnimationName != attackAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, attackAnimationName, false);
                    currentAnimationName = attackAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
               _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
    
}

public void ChaseAnm()
{
    if (currentAnimationName != runAnimationName)
                {
                    _spineAnimationState.SetAnimation(1, runAnimationName, true);
                    currentAnimationName = runAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(1).Complete += OnAttackAnimationComplete;
}

public void MovingAnm()
{
    
    if (currentAnimationName != walkAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, walkAnimationName, true);
                    currentAnimationName = walkAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
}

public void IdleAnm()
{
    if (currentAnimationName != idleAnimationName)
                {
                    _spineAnimationState.SetAnimation(1, idleAnimationName, true);
                    currentAnimationName = idleAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
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
    //isAttacking = false;
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//EVENTS
//Non puoi giocare di local scale sui vfx perché sono vincolati dal localscale del player PERò puoi giocare sulla rotazione E ottenere gli
//stessi effetti
void HandleEvent (TrackEntry trackEntry, Spine.Event e) {

if (e.Data.Name == "VFXslash") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(attack, slashpoint.position, transform.rotation);
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

if (e.Data.Name == "VFXslash_h") {
        // Inserisci qui il codice per gestire l'evento.
        Instantiate(attack_h, slashpoint.position, transform.rotation);
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

    if (e.Data.Name == "attack") {
        // Inserisci qui il codice per gestire l'evento.
        if(horizontal == 1)
        {
        transform.position += transform.right * atckForward * Time.deltaTime; //sposta il nemico in avanti
        } else if(horizontal == 1)
        {
        transform.position += transform.right * -atckForward * Time.deltaTime; //sposta il nemico in avanti
        } 

    }

    if (e.Data.Name == "walk") {
        // Inserisci qui il codice per gestire l'evento.
         if (Swalk == null) {
            Debug.LogError("AudioSource non trovato");
            return;
        }
        // Assicurati che l'oggetto contenente l'AudioSource sia attivo.
        if (!Swalk.gameObject.activeInHierarchy) {
            Swalk.gameObject.SetActive(true);
        }
        // Imposta la pitch dell'AudioSource in base ai valori specificati.
        Swalk.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        // Assegna la clip audio all'AudioSource e avviala.
        Swalk.Play();
        
    }
}


}
