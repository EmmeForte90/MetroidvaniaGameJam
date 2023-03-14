using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globo : MonoBehaviour
{
   public float speed = 10f; // velocità del proiettile
    //Enemy Enemy;
    [SerializeField] GameObject Explode;
    [SerializeField] Transform prefabExp;
    [SerializeField] int damage = 50;
    public float rotationSpeed = 2500f;

    [SerializeField] float lifeTime = 0.5f;
    Rigidbody2D rb;    

    // Start is called before the first frame update
    void Start()
    {
        //Recupera i componenti del rigidbody
        rb = GetComponent<Rigidbody2D>();
        //Recupera i componenti dello script
        if(Move.instance.transform.localScale.x > 0)
    {
        rb.velocity = transform.right * speed;
    } 
    else if(Move.instance.transform.localScale.x < 0)
    {
        rb.velocity = -transform.right * speed;
    }
        //La variabile è uguale alla scala moltiplicata la velocità del proiettile
        //Se il player si gira  anche lo spawn del proittile farà lo stesso
       
    // Update is called once per frame
    }
private void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.tag == "Enemy")
    {  
        Instantiate(Explode, transform.position, transform.rotation);
        IDamegable hit = other.GetComponent<IDamegable>();
        hit.Damage(damage);
        Destroy(gameObject);

    }

    if (other.gameObject.tag == "Ground")
    { 
        Invoke("Destroy", lifeTime);
    }
}
}

