using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] float shotgunBullet = 3f;
    [SerializeField] float bombSpeed = 10f;
    //Variabile della velocità del proiettile
    [SerializeField] GameObject Explode;
    [SerializeField] Transform prefabExp;
    private float lifeTime = 0.5f;
    //Riservato allo shotgun

    Rigidbody2D myRigidbody;
    //Il corpo rigido
    CharacterController2D player;
    Health Health;
    //Attribuscie una variabile allo script di movimento del player
    //Per permettere al proiettile di emularne l'andamento
    float xSpeed;
    float shotgunSpeed;
    public Vector3 LaunchOffset;
    public string targetTag = "Enemy";
    private GameObject target;
    //Riservato alla bomb

    [Header("Che tipo di bullet")]
    [SerializeField] bool isNormal;
    [SerializeField] bool isRapid;
    [SerializeField] bool isBomb;
    [SerializeField] bool isTarget;
    [SerializeField] bool isShotgun;
    [SerializeField] bool rightFace;


   
        void Start()
    {
                target = GameObject.FindWithTag(targetTag);
        myRigidbody = GetComponent<Rigidbody2D>();
        //Recupera i componenti del rigidbody
        player = FindObjectOfType<CharacterController2D>();
        Health = FindObjectOfType<Health>();
        //Recupera i componenti dello script
        xSpeed = player.transform.localScale.x * bulletSpeed;
        shotgunSpeed = player.transform.localScale.x * shotgunBullet;
        //La variabile è uguale alla scala moltiplicata la velocità del proiettile
        //Se il player si gira  anche lo spawn del proittile farà lo stesso
        
        if(isBomb)
        {        
            if(player.transform.localScale.x > 0)
            {
            var direction = transform.right + Vector3.up;
            GetComponent<Rigidbody2D>().AddForce(direction * bombSpeed, ForceMode2D.Impulse);
            }
            else if(player.transform.localScale.x < 0)
            {
            var direction = -transform.right + Vector3.up;
            GetComponent<Rigidbody2D>().AddForce(direction * bombSpeed, ForceMode2D.Impulse);
            }
        }
        transform.Translate(LaunchOffset);

        if(isTarget)
        {        
            if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            transform.Translate(direction.normalized * xSpeed * Time.deltaTime, Space.World);
        }
    }
        }
        
    




#region Update
    void Update()
    {
        if(isNormal || isRapid && !isShotgun && !isBomb)
        {
         myRigidbody.velocity = new Vector2 (xSpeed, 0f);
        }
        else if(!isNormal && !isRapid && isShotgun && !isBomb)
        {
        myRigidbody.velocity = new Vector2 (shotgunSpeed, 0f);
        }
        else if(!isNormal && !isRapid && !isShotgun && isBomb)
        {

        }
        //La velocità e la direzione del proiettile
        FlipSprite();
        
    }
#endregion
 

#region  FlipSprite
    void FlipSprite()
    {
        bool bulletHorSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        //se il player si sta muovendo le sue coordinate x sono maggiori di quelle e
        //di un valore inferiore a 0

        if (bulletHorSpeed) //Se il player si sta muovendo
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
            //La scala assume un nuovo vettore e il rigidbody sull'asse x 
            //viene modificato mentre quello sull'asse y no. 
        }
        
        
    }

#endregion


    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Enemy")
        //Se il proiettile tocca il nemico
        {            
            Instantiate(Explode, transform.position, transform.rotation);
            if(isNormal && !isRapid )
            //Se è un proiettile normale e non rapido
            {
            Destroy(gameObject);
            //Il nemico subisce danno
            Health.TakeDamage(10f);
            //IDamegable hit = other.GetComponent<IDamegable>();
           // hit.Damage();
            //Viene distrutto quando colpisce il nemico

            }
            else if(!isNormal && isRapid)
            //Quando è un proiettile rapido e non normale
            {
                Health.TakeDamage(5f);
            //IDamegable hit = other.GetComponent<IDamegable>();
            //hit.Damage();
            //Non viene distrutto
            }
            else if(isBomb)
            //Quando è un proiettile rapido e non normale
            {
            Destroy(gameObject);
            //CinemachineShake.instance.ShakeCamera(5f, .1f);
            Health.TakeDamage(50f);
            //IDamegable hit = other.GetComponent<IDamegable>();
            //hit.Damage();
            //Non viene distrutto
            }
            else if(isShotgun)
            //Quando è un proiettile rapido e non normale
            {
            Health.TakeDamage(25f);
            //Non viene distrutto
            }
        }

        if(other.gameObject.tag == "Ground" && !isShotgun)
        //Se il proiettile tocca il nemico
        {            
            Instantiate(Explode, transform.position, transform.rotation);
            Destroy(gameObject);
            //Viene distrutto
        }
        else if(isShotgun)
        {
            Invoke("Destroy", lifeTime);
        }
        
    }

    void Destroy()
    {
        Destroy(gameObject); 
        if(!isShotgun)
        {
        Instantiate(Explode, transform.position, transform.rotation);
        }
  
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(!isShotgun)
        {
        Instantiate(Explode, transform.position, transform.rotation);
        }
        Destroy(gameObject); 

        //Se il proiettile tocca una superficie viene distrutto 
    }

}
