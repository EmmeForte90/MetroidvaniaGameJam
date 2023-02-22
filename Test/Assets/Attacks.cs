using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Attacks : MonoBehaviour
{
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

    [Header("VFX")]
    // Variabile per il gameobject del proiettile
    [SerializeField] GameObject blam;
    [SerializeField] public Transform gun;
    [SerializeField] GameObject Circle;
    [SerializeField] public Transform circlePoint;
    
    public string idleAnimationName;
    public string attackAnimationName;
    public string attack_lAnimationName;
    public string attack_hAnimationName;
    public string blastAnimationName;
    
    private Rigidbody2D rb;
    private SkeletonAnimation skeletonAnimation;
    [SerializeField] public GameplayManager gM;

    [Header("Audio")]
    [SerializeField] AudioSource SwSl;
    [SerializeField] AudioSource Smagic;
    private string currentAnimationName;
    public bool isAttacking = false; // vero se il personaggio sta attaccando

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (gM == null)
        {
            gM = GetComponent<GameplayManager>();
        }
        Less = GetComponent<PlayerHealth>();
        currentCooldown = attackCooldown;
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        currentAnimationName = idleAnimationName;
    }

    // Update is called once per frame
    void Update()
    {
        // gestione dell'input dello sparo
        if (Input.GetButtonDown("Fire2"))
        {
        Blast();   
        }

    // Se il personaggio sta saltando o facendo il Wall Jump, aggiornare lo stato
    if (Input.GetButtonDown("Fire1"))
    {
        Attack();
        isAttacking = true;
        
    
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

        if (Input.GetButtonDown("Fire3") && !isCharging)
        {
            isCharging = true;
            chargeTime = 0f;

            //animator.Play(chargeAnimation.name);
        }

        if (Input.GetButtonDown("Fire3") && isCharging)
        {
            chargeTime += Time.deltaTime;

            if (chargeTime > maxChargeTime)
            {
                chargeTime = maxChargeTime;
            }
        }

        if (Input.GetButtonUp("Fire3") && isCharging)
        {
            float chargeRatio = chargeTime / maxChargeTime;
            float damage = maxDamage * chargeRatio;
            Debug.Log("Charge ratio: " + chargeRatio + ", Damage: " + damage);

            //animator.Play(attackAnimation.name);
            isCharging = false;
        }



        // gestione dell'animazione del personaggio
        selectAnimation();
    
}

private void selectAnimation()
    {
        switch (comboCounter)
        {
            case 0:
            if (comboCounter == 0)
            {
                skeletonAnimation.AnimationName = idleAnimationName;
            }
            else if (comboCounter == 1)
            {
                skeletonAnimation.AnimationName = attackAnimationName;

            }else if (comboCounter == 2)
            {
                skeletonAnimation.AnimationName = attack_hAnimationName;

            }else if (comboCounter == 3)
            {
                skeletonAnimation.AnimationName = attack_lAnimationName;

            }
            break;
        }
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

void Blast()
{
    if(Less.currentMana > 0)
        {
if (Time.time > nextAttackTime)
        {
        isAttacking = true;
        nextAttackTime = Time.time + 1f / attackRate;
        skeletonAnimation.AnimationName = blastAnimationName;
        Smagic.Play();
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


            }else if (comboCounter == 2)
            {


            }else if (comboCounter == 3)
            {

            }
            currentCooldown = attackCooldown;
            comboTimer = 0.5f;
            
        }
        
    }

}
