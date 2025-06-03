using System.Collections;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.UI;
using TMPro;
using System.Data.SqlTypes;
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public string TypeEnemy = "Normal";
    public GameObject ThisEnemy;
    public Transform player;
    public CharacterController controller;
    [Header("Gravità")]
    public float gravity = 9.81f;
    private float verticalVelocity = 0f;
    private bool canChase = true;
    private bool wasInAttackRange = false;
    [Header("Reward")]
    public GameObject[] Money;
    private Transform rewardSpawnPoint; 
    public bool CanReawrd = true;

    [Header("Settings")]
    public GameObject UiInterface;
    //public GameObject IconMinimap;
    public Scrollbar EnergyBar;
    public GameObject DefenceBarOBJ;
    public Scrollbar DefenceBar;
    public int EnemyAtk;
    public float HP_Cur;
    public float HP_Max = 100;
    public float Def_Cur;  
    public float Def_Max = 100;
    //public TextMeshProUGUI HPC_T,HPM_T,DC_T;
    public GameObject VFXDamage;
    public GameObject DamageCaster;
    public GameObject[] VFXSlash;
    public GameObject[] Bullet;
    private int IntBullet = 0;
    public Transform firePoint;
    public GameObject VFXDie;
    public GameObject VFXMoney;
    public GameObject DamageCount;
    public float chaseSpeed = 5f;   // Velocità di inseguimento
    public float attackRange = 2f;  // Distanza per attaccare il player
    public float waitTime = 1f;     // Tempo di attesa prima di riprendere l'inseguimento
    public float detectionRange = 10f; // Distanza massima per iniziare l'inseguimento
    public float durationKnockback = 0.3f; // Distanza massima per iniziare l'inseguimento
    public float durationKnockbackSkill = 1f; // Distanza massima per iniziare l'inseguimento
    public float ForceKnocback = 10f; // Distanza massima per iniziare l'inseguimento
    public float ForceKnocbackSkill = 1f; // Distanza massima per iniziare l'inseguimento
    private bool isAttack = false;
    private bool isKnockedBack = false; 
    public bool canKnock = true;
    private bool VFX = true;
    private bool isDie = false;
    bool diecount = false;
    bool startcount = false;

    [Header("OrderLayer")]
    public int orderBehind = -5;  
    public int orderInFront = 5;  

    private string currentAnimationName;
    public SkeletonAnimation _skeletonAnimation;
    public Spine.AnimationState _spineAnimationState;
    public Spine.Skeleton _skeleton;    
    public Renderer skeletonRenderer;
    Spine.EventData eventData;
    [Header("Animations")]
    [SpineAnimation]public  string idle;
    [SpineAnimation]public  string walk;
    [SpineAnimation]public  string run;
    [SpineAnimation]public  string guard;
    [SpineAnimation]public  string attack;
    [SpineAnimation]public  string die;
    [SpineAnimation]public  string hurt;



    private void OnEnable() 
    {
        CanReawrd = true;
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (_skeletonAnimation == null) {Debug.LogError("Componente SkeletonAnimation non trovato!");}  
        _spineAnimationState = GetComponent<Spine.Unity.SkeletonAnimation>().AnimationState;
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.skeleton;   
        controller = GetComponent<CharacterController>();
        VFXDamage.SetActive(false); DamageCount.SetActive(false);VFXDie.SetActive(false);VFXMoney.SetActive(false);//IconMinimap.SetActive(true);
        HP_Cur = HP_Max; 
        if(TypeEnemy == "Warrior"){ Def_Cur = Def_Max;DefenceBarOBJ.SetActive(true);}else if(TypeEnemy != "Warrior"){ Def_Cur = 0;DefenceBarOBJ.SetActive(false);}
        diecount = true;
        isDie = false;
        //if(!startcount){GameManager.instance.N_Enemy++;startcount = true;}
        StartCoroutine(recognizePlayer());
        UiInterface.SetActive(true);
        BulletID();
    }
    IEnumerator recognizePlayer()
    {
        yield return new WaitForSeconds(1);
        //player = GameManager.instance.ActivatePet.transform;
        StopCoroutine(recognizePlayer());
    }
    public void BulletID()
    { 
        switch (TypeEnemy)
                {
                    case "Normal":IntBullet = 0;break;
                    case "Warrior":IntBullet = 0;break;
                    case "Bow":IntBullet = 1;break;
                    case "Mage":IntBullet = 2;break;
                }    
    }

    void Update()
    {
        // Gravità
        verticalVelocity -= gravity * Time.deltaTime;

         if(!VFX){StartCoroutine(RestoreVFX());}
        if(!isDie)
        {
        //FacePlayer(); 
        HP();UIInterfaceFlip(); //OrderLayer();
        if (!isKnockedBack){Chase();}}else if(isDie){Die();
        }
    }
    void Shoot(int bulletID)
{
    if (player != null)
    {
        Vector3 lastPlayerPosition = player.transform.position; // Memorizza l'ultima posizione del player
        GameObject bullet = Instantiate(Bullet[bulletID], firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().Initialize(lastPlayerPosition);
    }
}

   public void Chase()
{
    float distance = Vector3.Distance(transform.position, player.position);

    // Controllo se il player era dentro il raggio di attacco l'ultimo frame
    bool isInAttackRange = distance <= attackRange;

    // Se il player esce dal raggio d'attacco, parte la pausa
    if (wasInAttackRange && !isInAttackRange)
    {
        OnPlayerExitAttackRange();
    }

    wasInAttackRange = isInAttackRange;

    // Se il player è almeno 3 unità più in alto del nemico, il nemico si ferma completamente
    if (player.position.y - transform.position.y >= 3f)
    {
        PlayAnimationLoop(idle);
        FacePlayer(); 
        return; // Blocca tutto, inclusi movimento e flip
    }

    if (!canChase)
    {
        // Sta aspettando, non insegue
        return;
    }

    if (distance > detectionRange)
    {
        PlayAnimationLoop(idle);
        FacePlayer(); 
    }
    else if (distance > attackRange)
    {
        Debug.Log("Enemy is chasing the player.");

        PlayAnimationLoop(run);

        // Movimento solo sull'asse X
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Flip verso il player
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        controller.Move(direction * chaseSpeed * Time.deltaTime);
    }
    else // dentro il raggio di attacco
    {
        if (!isAttack)
        {
            Attack();
            FacePlayer();
            StartCoroutine(RestoreAtk());
        }
    }
}

public void OnPlayerExitAttackRange()
{
    if (canChase)
        StartCoroutine(WaitBeforeChasing());
}

private IEnumerator WaitBeforeChasing()
{
    canChase = false;
    PlayAnimationLoop(idle);
    FacePlayer(); 
    yield return new WaitForSeconds(1f);
    canChase = true;
}

    void Attack()
    {
        Debug.Log("Enemy is attacking the player!");
        PlayAnimationLoop(attack);
        GameManager.instance.EnemyAtk = EnemyAtk;
    }

    IEnumerator RestoreAtk()
    {
        isAttack = true;
        yield return new WaitForSeconds(waitTime);
        isAttack = false;
        //StopCoroutine(RestoreAtk());
    }
    IEnumerator RestoreDef()
    {
        yield return new WaitForSeconds(5f);
        Def_Cur = Def_Max;
        StopCoroutine(RestoreDef());
    }
    public void HP()
    {
    EnergyBar.size = Mathf.RoundToInt(HP_Cur) / (float)Mathf.RoundToInt(HP_Max);
    EnergyBar.size = Mathf.Clamp(EnergyBar.size, 0.01f, 1f);
    if(TypeEnemy == "Warrior"){DefenceBar.size = Mathf.RoundToInt(Def_Cur) / (float)Mathf.RoundToInt(Def_Max);
    DefenceBar.size = Mathf.Clamp(DefenceBar.size, 0.01f, 1f);}
    if(TypeEnemy == "Warrior"){if(Def_Cur <= 0){StartCoroutine(RestoreDef());}}
    //HPC_T.text = HP_Cur.ToString();HPM_T.text = HP_Max.ToString();
    if(HP_Cur <= 0){HP_Cur = 0; isDie = true;} else if(HP_Cur >= HP_Max){HP_Cur = HP_Max;}
    }
     private void Flip()
    {
        if (player.transform.localScale.x > 0f){transform.localScale = new Vector3(-1, 1,1);}
        else if (player.transform.localScale.x < 0f){transform.localScale = new Vector3(1, 1,1);}
    }
    public void UIInterfaceFlip()
    {
        if (transform.localScale.x > 0f){UiInterface.transform.localScale = new Vector3(-1, 1,1);}
        else if (transform.localScale.x < 0f){UiInterface.transform.localScale = new Vector3(1, 1,1);}
    }
    private void FacePlayer()
    {
        if (player != null)
        {
            if (player.transform.position.x > transform.position.x){transform.localScale = new Vector3(1, 1, 1);}
            else{transform.localScale = new Vector3(-1, 1, 1);}
        }
    }
      /*private void OrderLayer()
    {
        if (Player.transform.position.z > transform.position.z)
        {skeletonRenderer.sortingOrder  = orderBehind;}
        else{skeletonRenderer.sortingOrder  = orderInFront;}
    }*/
     #region Damage

    public void OnTriggerEnter(Collider collision)
{
    if (collision.gameObject.CompareTag("PDamage")){ damageMele();}
    else if (collision.gameObject.CompareTag("PSkill")){damageSkill();}
    else if (collision.gameObject.CompareTag("PDamageDown")){damageSkill();GameManager.instance.BrainPlayer.Bump();}
}

public void damageSkill()
{
     if(Def_Cur <= 0){
        if(canKnock)
        {
        // Calcola la direzione lungo l'asse X tra il nemico e il player
        float diffX = transform.position.x - player.transform.position.x;
        // Usa Mathf.Sign per ottenere la direzione corretta (1 per destra, -1 per sinistra)
        Vector3 knockbackDirection = new Vector3(Mathf.Sign(diffX), 0, 0);
        Knockback(knockbackDirection * ForceKnocbackSkill, durationKnockbackSkill);
        PlayAnimationB(hurt);
        }
        VFXDamage.SetActive(true);
        DamageCount.SetActive(true);
        HP_Cur -= GameManager.instance.skillDamage; 
        //DC_T.text = skillDamage.ToString(); 
        TemporaryChangeColor(Color.red);
        }
        else if(Def_Cur > 0)
        {
            
        PlayAnimationB(guard);
         VFXDamage.SetActive(true);
         Def_Cur -= 20;
        } 
}

public void damageMele()
{
if(Def_Cur <= 0){
        if(canKnock)
        {
        // Calcola la direzione lungo l'asse X tra il nemico e il player
        float diffX = transform.position.x - player.transform.position.x;
        // Usa Mathf.Sign per ottenere la direzione corretta (1 per destra, -1 per sinistra)
        Vector3 knockbackDirection = new Vector3(Mathf.Sign(diffX), 0, 0);
        Knockback(knockbackDirection * ForceKnocback, durationKnockback);
        PlayAnimationB(hurt);
        }
        VFXDamage.SetActive(true);
        DamageCount.SetActive(true);
        HP_Cur -= GameManager.instance.Atk;
        //DC_T.text = GameManager.instance.DB.Atk.ToString();
        TemporaryChangeColor(Color.red);
        }else if(Def_Cur > 0)
        {
         PlayAnimationB(guard);
         VFXDamage.SetActive(true);
         Def_Cur -= 20;
    }
}

public void Knockback(Vector3 force, float duration)
{
    if (!isKnockedBack)
    {
        Debug.Log("Enemy received knockback!");
        StartCoroutine(ApplyKnockbackCoroutine(force, duration));
    }
}

private IEnumerator ApplyKnockbackCoroutine(Vector3 force, float duration)
{
    isKnockedBack = true;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        controller.Move(force * Time.deltaTime);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    isKnockedBack = false;
}
private void Die(){StartCoroutine(TimeDire());}
private IEnumerator TimeDire()
{
    PlayAnimationB(die);
    UiInterface.SetActive(false);
    VFXDie.SetActive(true);
    SpawnReward();
    yield return new WaitForSeconds(1f);
    ChangeColorTransparent();
    //IconMinimap.SetActive(false);
    yield return new WaitForSeconds(0.5f);
    //if(diecount){GameManager.instance.N_Enemy--;
    //GameManager.instance.Count_Enemy++;
    VFXMoney.SetActive(true);
    //GameManager.instance.Count_Money += Money;
    diecount = false;startcount = false;
    ThisEnemy.SetActive(false);
    StopCoroutine(TimeDire());
}
public void SpawnReward()
{
    if (CanReawrd)
    {
        int count = Random.Range(1, 6); // genera da 1 a 5 inclusi

        for (int i = 0; i < count; i++)
        {
            if (Money.Length == 0) return;

            GameObject rewardPrefab = Money[Random.Range(0, Money.Length)];

            Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), 0.2f, Random.Range(-0.2f, 0.2f));
            Vector3 spawnPos = rewardSpawnPoint != null 
                ? rewardSpawnPoint.position + offset 
                : transform.position + offset;

            Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
        }
        CanReawrd = false;
    }
}

#endregion
    #region Animations
    private void SetSkeletonColor(Color color) => _skeletonAnimation.Skeleton.SetColor(color);
public void TemporaryChangeColor(Color color){SetSkeletonColor(color);Invoke(nameof(ResetColor), 0.5f);}
public void ChangeColorP() => SetSkeletonColor(Color.green);
public void ChangeColorR() => SetSkeletonColor(new Color32(193, 155, 26, 255));
public void ChangeColorTransparent() => SetSkeletonColor(new Color32(0, 0, 0, 0));
public void ChangeColorNormal() => SetSkeletonColor(new Color32(255, 255, 255, 255));
public void ResetColor() => SetSkeletonColor(Color.white);
private void PlayAnimationInternal(string animationName, bool loop, Spine.AnimationState.TrackEntryDelegate completeCallback)
{
    if (currentAnimationName != animationName)
    {
        _skeletonAnimation.state.SetAnimation(0, animationName, loop);
        currentAnimationName = animationName;
        _spineAnimationState.Event += HandleEvent;
    }
    if (completeCallback != null){_skeletonAnimation.state.GetCurrent(0).Complete += completeCallback;}
}

public void PlayAnimation(string animationName){PlayAnimationInternal(animationName, false, OnAttackAnimationComplete);}
public void PlayAnimationB(string animationName){PlayAnimationInternal(animationName, false, OnAttackBAnimationComplete);}
public void PlayAnimationStop(string animationName){PlayAnimationInternal(animationName, false, null);}
public void PlayAnimationLoop(string animationName){PlayAnimationInternal(animationName, true, null);}
public void ClearAnm(){_skeletonAnimation.state.ClearTrack(0);currentAnimationName = null;}
private void OnExploreAnimationComplete(Spine.TrackEntry trackEntry){trackEntry.Complete -= OnExploreAnimationComplete;RestoreANM();}
private void OnAttackAnimationComplete(Spine.TrackEntry trackEntry){trackEntry.Complete -= OnAttackAnimationComplete;RestoreANM();}
private void OnAttackBAnimationComplete(Spine.TrackEntry trackEntry){trackEntry.Complete -= OnAttackBAnimationComplete;RestoreANM();}
public void RestoreANM()
{ 
    _skeletonAnimation.state.SetAnimation(0, idle, true);
}

    void HandleEvent (TrackEntry trackEntry, Spine.Event e) {
    //Normal VFX
    if (e.Data.Name == "atk" && VFX){Shoot(IntBullet);print("BulletStart");VFX = false;}
    if (e.Data.Name == "atk_1" && VFX){VFXSlash[0].SetActive(true);DamageCaster.SetActive(true);print("damageCaster");VFX = false;}
   
    }
    IEnumerator RestoreVFX(){yield return new WaitForSeconds(0.5f);VFX = true;StopCoroutine(RestoreVFX());}
    #endregion
#region gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    #endregion
}