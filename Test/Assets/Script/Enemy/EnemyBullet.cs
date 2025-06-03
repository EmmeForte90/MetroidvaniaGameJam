using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameObject Player;
    public float speed = 5f; // Velocità del proiettile
    private Vector3 targetPosition; // Posizione bersaglio (ultima posizione del player)
    private int orderBehind = 5; 
    private int orderInFront = -5; 
    public Renderer skeletonRenderer;
    public GameObject VFXExplode;
    public bool arrowrainbow = false;
    private Vector3 launchVelocity;
    private float flightTime = 0f;
    public float gravity = -9.81f;
    private bool useArc = false;
    private Quaternion startRotation;

    private void OnEnable() {Player = GameManager.instance.Player;}

    private void FacePlayer()
    {
        Vector3 scale = transform.localScale;

        if (Player.transform.position.x > transform.position.x)
            scale.x = 1f;
        else
            scale.x = 1f;

        transform.localScale = scale;
    }

    private void OrderLayer()
    {if (Player.transform.position.z > transform.position.z)
    {skeletonRenderer.sortingOrder  = orderBehind;}else{skeletonRenderer.sortingOrder  = orderInFront;}}

    

    public void OnTriggerEnter(Collider collision)
    {
    if (collision.gameObject.CompareTag("Player")){StartCoroutine(CollisionF());}
    }
     IEnumerator CollisionF()
    {
        yield return new WaitForSeconds(0.2f);
        // Distrugge il proiettile dopo 5 secondi per evitare spreco di risorse
        Instantiate(VFXExplode, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
    public void Initialize(Vector3 playerLastPosition)
{
    targetPosition = playerLastPosition;
    FacePlayer();

    if (arrowrainbow)
    {
        useArc = true;

        // Calcola la distanza orizzontale e il tempo di volo desiderato
        float arcHeight = 3f; // Altezza massima dell’arco
        flightTime = Vector3.Distance(transform.position, targetPosition) / speed;

        // Calcola velocità iniziale per creare l'arco
        Vector3 toTarget = targetPosition - transform.position;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float yOffset = toTarget.y;

        float Vy = (4 * arcHeight) / flightTime;
        float Vxz = toTargetXZ.magnitude / flightTime;

        Vector3 result = toTargetXZ.normalized * Vxz;
        result.y = Vy;

        launchVelocity = result;
        startRotation = transform.rotation;
    }
}


    void Update()
{
    OrderLayer();

    if (!useArc)
    {
        // Movimento normale
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Instantiate(VFXExplode, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    else
    {
        // Movimento ad arco
        float deltaTime = Time.deltaTime;
        launchVelocity.y += gravity * deltaTime;
        transform.position += launchVelocity * deltaTime;

        // Ruota per seguire la direzione
        if (launchVelocity != Vector3.zero)
        {
            float angle = Mathf.Atan2(launchVelocity.y, launchVelocity.x) * Mathf.Rad2Deg;

            // Se flippato, ruota la sprite anche in senso opposto
            float direction = Mathf.Sign(transform.localScale.x);
            transform.rotation = Quaternion.Euler(0f, 0f, angle * direction);
        }

        // Distruggi se scende troppo
        if (transform.position.y < -5f || Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Instantiate(VFXExplode, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

}