using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class Eivir_Boss : MonoBehaviour, IDamegable
{
   [Header("Sistema Di HP")]
   // Enemy enemy;
    public float maxHealth = 500f;
    public float currentHealth = 500f;
    public Color originalColor;
    public float colorChangeDuration;

    [Header("Enemy")]
    [SerializeField] GameObject Brain;
    private Transform player;
    [SerializeField] LayerMask playerlayer;
    private float timeBeforeDestroying = 3f;

    [Header("Moving")]
    public float moveSpeed = 2f; // velocità di movimento
    [SerializeField] float atckForward = 5; // velocità di movimento
    [SerializeField] float pauseDuration = 0.5f; // durata della pausa
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
    public float InvincibleTime = 1f;

    [Header("Abilitations")]
    private bool isMove = false;
    private bool isJump = false;
    private bool isBlast = false;
    private bool isBlastJump = false;
    private bool isDie = false;
    private bool isRun = false;
    private bool isAttacking = false;
    private bool activeActions = true;
    private bool pauseAtck = false;
    private bool canAttack = true;
    private bool firstattack = true;
    private bool isPlayerInAttackRange = false;
    private bool isHurt = false;

    [Header("Audio")]
    [HideInInspector] public float basePitch = 1f;
    [HideInInspector] public float randomPitchOffset = 0.1f;
    [SerializeField] public AudioClip[] listmusic; // array di AudioClip contenente tutti i suoni che si vogliono riprodurre
    private AudioSource[] bgm; // array di AudioSource che conterrà gli oggetti AudioSource creati
    public AudioMixer SFX;
    
    [Header("VFX")]
    [SerializeField] public Transform slashpoint;
    [SerializeField] public Transform hitpoint;
    [SerializeField] GameObject attack;
    [SerializeField] GameObject attack_h;
    [SerializeField] GameObject attack_B;
    [SerializeField] GameObject Sdeng;

    [Header("Animations")]
    [SpineAnimation][SerializeField] private string idleAnimationName;
    [SpineAnimation][SerializeField] private string walkAnimationName;
    [SpineAnimation][SerializeField] private string runAnimationName;
    [SpineAnimation][SerializeField] private string dashAnimationName;
    [SpineAnimation][SerializeField] private string comboAnimationName;
    [SpineAnimation][SerializeField] private string blastAnimationName;
    [SpineAnimation][SerializeField] private string jumpAnimationName;
    [SpineAnimation][SerializeField] private string jumpblastAnimationName;
    [SpineAnimation][SerializeField] private string tiredAnimationName;
    [SpineAnimation][SerializeField] private string attackAnimationName;
    [SpineAnimation][SerializeField] private string parrytAnimationName;
    [SpineAnimation][SerializeField] private string dietAnimationName;
    [SpineAnimation][SerializeField] private string hurtAnimationName;
    [SpineAnimation][SerializeField] private string JumpBlastLandingAnimationName;
    [SpineAnimation][SerializeField] private string JumpattackAnimationName;

    private string currentAnimationName;
    public SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Spine.Skeleton _skeleton;
    Spine.EventData eventData;

    private enum State { moving, dash, run, jump, combo, attack, dead, tired, blast, wait }
    private State currentState;

    public static Eivir_Boss instance;


    private void Awake()
    {
    if (instance == null)
    {
        instance = this;
    }
    rb = GetComponent<Rigidbody2D>();
    _skeletonAnimation = GetComponent<SkeletonAnimation>();
    if (_skeletonAnimation == null) {
        Debug.LogError("Componente SkeletonAnimation non trovato!");
    }
    _spineAnimationState = _skeletonAnimation.AnimationState;
    _skeleton = _skeletonAnimation.skeleton;
    player = GameObject.FindWithTag("Player").transform;
    
    currentHealth = maxHealth;

    bgm = new AudioSource[listmusic.Length]; // inizializza l'array di AudioSource con la stessa lunghezza dell'array di AudioClip
    for (int i = 0; i < listmusic.Length; i++) // scorre la lista di AudioClip
    {
        bgm[i] = gameObject.AddComponent<AudioSource>(); // crea un nuovo AudioSource come componente del game object attuale (quello a cui è attaccato lo script)
        bgm[i].clip = listmusic[i]; // assegna l'AudioClip corrispondente all'AudioSource creato
    }
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

private void Update()
    {
        if (!GameplayManager.instance.PauseStop)
        {
            CheckState();

            #region testDanno
            if(Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("Il pulsante è stato premuto!");
            Damage(10);
            }
            #endregion
            
            switch (currentState)
            {
                case State.moving:
                //Moving();
                    break;
                case State.dash:
               // Chase();
                    break;
                case State.attack:
                Attack();
                FacePlayer();
                    if(firstattack)
                    {
                    attackTimer = 0;
                    firstattack = false;
                    }
                    break;
                case State.combo:
              //  Combo();
                    break;
                case State.run:
              //  Run();
                    break;
                case State.jump:
             //   Jump();
                    break;
                case State.blast:
             //   Blast();
                    break;
                case State.tired:
                Wait();
                    break;
                case State.wait:
                Wait();
                    break;
            }
        }
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

private void CheckState()
{
    if (Move.instance.isDeath) 
    {
return; // esce immediatamente se il personaggio è morto
}

if (currentHealth <= 0) { // controlla se il personaggio è morto
    isRun = false;
    isAttacking = false;
    isMove = false;
    isBlast = false;
    activeActions = false;
    isDie = true;
    Die();
    currentState = State.dead;

} else if (Vector2.Distance(transform.position, player.position) < attackrange) { 
    // controlla se il personaggio è dentro il raggio d'attacco
    isRun = false;
    isAttacking = true;
    isMove = false;
    isPlayerInAttackRange = true;
    currentState = State.attack;
} else if (Vector2.Distance(transform.position, player.position) > attackrange && isPlayerInAttackRange) { 
    // controlla se il personaggio è appena uscito dal raggio e si avvia il timer di attesa
    isRun = false;
    isAttacking = false;
    isMove = false;
    isBlast = false;
    activeActions = false;
    currentState = State.wait;
    StartCoroutine(waitChase());
} else if (Vector2.Distance(transform.position, player.position) > attackrange && !isPlayerInAttackRange) { 
    // controlla se il personaggio è nel raggio di inseguimento per sparare
    isAttacking = false;
    isMove = false;
    isBlast = true;
    firstattack = true;
    currentState = State.blast;
} else if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.up), 5f, playerlayer)) { 
    // controlla se il personaggio è bloccato da un ostacolo
    isAttacking = false;
    isMove = false;
    isBlast = false;
    currentState = State.wait;
} else if (activeActions) { 
    // controlla se il personaggio può muoversi in autonomia
    isMove = true;
    isAttacking = false;
    isBlast = false;
    currentState = State.moving;
}


}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

//Aspetta
private void Wait()
{
    rb.velocity = new Vector3(0, 0);
    IdleAnm();
}



//Il nemico torna a inseguirlo dopo tot tempo
IEnumerator waitChase()
    {
        yield return new WaitForSeconds(WaitAfterAtk);
        isPlayerInAttackRange = false;
        activeActions = true;
    }

// Controlla se l'oggetto deve essere in movimento, il rigedbody è in KInematic
private void Moving()
{
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
        if (Vector2.Distance(transform.position, positions[id_positions]) < 0.0001f)
        {
            // Imposta il timer di pausa
            pauseTimer = pauseDuration;
            ChanceAtk();
        }
    }
    else if (!isMove && !isAttacking) // Se l'oggetto non deve essere in movimento
    {
        // Ferma l'animazione di movimento e attiva l'animazione di idle
        IdleAnm();
    }

    // Inverte l'orientamento dell'oggetto in base alla sua direzione di movimento
    FacePlayer();

    // Esegui l'animazione di movimento
    MovingAnm();
}


void ChanceAtk()
{
    int randomChance = Random.Range(1, 5); // Genera un numero casuale compreso tra 1 e 5
    isAttacking = true;
    if (randomChance == 1) // Se il numero casuale è  1 
    {
        //Esegue il dash
    }
    else if (randomChance == 2)// Se il numero casuale è 2
    {
        //Esegue una combo
    }
    else if (randomChance == 3)// Se il numero casuale è 3
    {
        //Salta
    }
    else if (randomChance == 4)// Se il numero casuale è 4
    {
        //Blast
    }
    else if (randomChance == 5)// Se il numero casuale è 5
    {
        // Si muove sul punto successivo
    }

}
    

private void Attack()
{
    if (isAttacking) // Se il personaggio sta attaccando...
    {
        if (attackTimer > 0) // ...e l'attacco non è ancora disponibile...
        {
            attackTimer -= Time.deltaTime; // ...decrementa il timer dell'attacco...
            canAttack = false; // ...e imposta la variabile "canAttack" su "false".
            return; // Esci dalla funzione.
        }
        else // Altrimenti...
        {
            canAttack = true; // ...imposta la variabile "canAttack" su "true".
        }

        if (canAttack && Vector2.Distance(transform.position, player.position) < attackrange) // Se l'attacco è disponibile e il personaggio è abbastanza vicino al giocatore...
        {
            Attack1Anm(); // ...esegui l'animazione dell'attacco...
            attackTimer = attackCooldown; // ...e reimposta il timer dell'attacco.
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


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public void Damage(int damage)
{
    if (isDie) return;

    currentHealth -= damage;
    TemporaryChangeColor(Color.red);
    Instantiate(Sdeng, hitpoint.position, transform.rotation);
    PlayMFX(1);

    if (!isHurt)
    {
        StartCoroutine(WaitForHurt());
    }
}

private IEnumerator WaitForHurt()
{
    isHurt = true;
    yield return new WaitForSeconds(InvincibleTime);
    isHurt = false;
    activeActions = true;
}

public void TemporaryChangeColor(Color color)
{
    _skeletonAnimation.Skeleton.SetColor(color);
    Invoke(nameof(ResetColor), colorChangeDuration);
}

private void ResetColor()
{
    _skeletonAnimation.Skeleton.SetColor(originalColor);
}



public void Die()
{
    PlayMFX(2);
    //Drop
    //EssenceGive();
    DieAnm();
    StartCoroutine(DestroyAfterDeath());
}

private IEnumerator DestroyAfterDeath()
{
    yield return new WaitForSeconds(timeBeforeDestroying);
    //Destroy(gameObject);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//ANIMATIONS


public void DieAnm()
{
    if (currentAnimationName != dietAnimationName)
                {    
                    _spineAnimationState.ClearTrack(2);
                    _spineAnimationState.SetAnimation(1, dietAnimationName, true);
                    currentAnimationName = dietAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(1).Complete += OnAttackAnimationComplete;
}



public void JumpAtk()
{
    if (currentAnimationName != JumpattackAnimationName)
                {    
                    _spineAnimationState.ClearTrack(2);
                    _spineAnimationState.SetAnimation(1, JumpattackAnimationName, true);
                    currentAnimationName = JumpattackAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(1).Complete += OnAttackAnimationComplete;
}


public void JumpBlast()
{
    if (currentAnimationName != jumpblastAnimationName)
                {    
                    _spineAnimationState.ClearTrack(2);
                    _spineAnimationState.SetAnimation(1, jumpblastAnimationName, true);
                    currentAnimationName = jumpblastAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(1).Complete += OnAttackAnimationComplete;
}

public void JumpBlastLanding()
{
    if (currentAnimationName != JumpBlastLandingAnimationName)
                {    
                    _spineAnimationState.ClearTrack(2);
                    _spineAnimationState.SetAnimation(1, JumpBlastLandingAnimationName, false);
                    currentAnimationName = JumpBlastLandingAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
                //_spineAnimationState.GetCurrent(1).Complete += OnAttackAnimationComplete;
}


public void Attack1Anm()
{
            
   
    if (currentAnimationName != attackAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, attackAnimationName, false);
                    currentAnimationName = attackAnimationName;
                    _spineAnimationState.Event += HandleEvent;
                    //attack.gameObject.SetActive(true);
                    // StartCoroutine(VFXCont());
                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
               _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
    
}

public void HurtAnm()
{
            
    if (currentAnimationName != hurtAnimationName)
                {
                    TemporaryChangeColor(Color.red);
                    _spineAnimationState.SetAnimation(2, hurtAnimationName, false);
                    currentAnimationName = hurtAnimationName;
                    _spineAnimationState.Event += HandleEvent;

                   // Debug.Log("Combo Count: " + comboCount + ", Playing Animation: combo_1");
                }
                // Add event listener for when the animation completes
               _spineAnimationState.GetCurrent(2).Complete += OnAttackAnimationComplete;
    
}
public void RunAnm()
{
    if (currentAnimationName != runAnimationName)
                {
                    _spineAnimationState.SetAnimation(2, runAnimationName, true);
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
public void PlayMFX(int soundToPlay)
    {
        bgm[soundToPlay].Stop();
        // Imposta la pitch dell'AudioSource in base ai valori specificati.
        bgm[soundToPlay].pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        bgm[soundToPlay].Play();
    }


IEnumerator VFXCont()
{   
    yield return new WaitForSeconds(0.5f);
    attack_h.gameObject.SetActive(false);
    attack_B.gameObject.SetActive(false);
    attack.gameObject.SetActive(false);

}


void HandleEvent (TrackEntry trackEntry, Spine.Event e) {

if (e.Data.Name == "VFXslash") {
        // Inserisci qui il codice per gestire l'evento.
        //Instantiate(attack, slashpoint.position, transform.rotation);
        attack.gameObject.SetActive(true);
                    StartCoroutine(VFXCont());
       PlayMFX(0);
    }

if (e.Data.Name == "VFXslash_h") {
        // Inserisci qui il codice per gestire l'evento.
        attack_h.gameObject.SetActive(true);
                    StartCoroutine(VFXCont());
        PlayMFX(0);
    }
if (e.Data.Name == "VFXSlashB") {
        // Inserisci qui il codice per gestire l'evento.
        attack_B.gameObject.SetActive(true);
                    StartCoroutine(VFXCont());
        PlayMFX(0);
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

   
}


}
