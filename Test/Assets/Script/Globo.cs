using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globo : MonoBehaviour
{
    public bool GloboB = true;
    public float speed = 10f; // Velocità del proiettile
    private Vector3 direction; // Direzione del proiettile
    public GameObject VFXExplode;
    public void Initialize(Vector3 playerScale)
    {
        // Se la scala X del player è positiva, spara a destra, altrimenti a sinistra
        direction = playerScale.x > 0 ? Vector3.right : Vector3.left;
        StartCoroutine(DestroyF());
    }
    public void OnTriggerEnter(Collider collision)
    {
    if (collision.gameObject.CompareTag("Enemy")){StartCoroutine(CollisionF());}
    }
    IEnumerator CollisionF()
    {
        yield return new WaitForSeconds(0.2f);
        // Distrugge il proiettile dopo 5 secondi per evitare spreco di risorse
        Instantiate(VFXExplode, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        if(GloboB){Destroy(gameObject);}else if(!GloboB){Destroy(gameObject, 5f);}
    }
    IEnumerator DestroyF()
    {
        yield return new WaitForSeconds(5f);
        // Distrugge il proiettile dopo 5 secondi per evitare spreco di risorse
        Instantiate(VFXExplode, transform.position, Quaternion.identity);
        if(GloboB){Destroy(gameObject);}
    }
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime; // Muove il proiettile
    }
}