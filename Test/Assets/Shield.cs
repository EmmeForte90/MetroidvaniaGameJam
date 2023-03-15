using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
     public float rotationSpeed = 50f;
    public float shieldDuration = 5f;
    private float timer;
    [SerializeField] GameObject Explode;
    [SerializeField] Transform prefabExp;
    void Start()
    {
        Move.instance.Evocation();
        Move.instance.Stop();
        timer = shieldDuration;
    }

    void Update()
    {
        // Fluttua attorno al giocatore
        transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(Mathf.Sin(Time.time * rotationSpeed), Mathf.Cos(Time.time * rotationSpeed)) * 2f;

        // Timer di durata scudo
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Instantiate(Explode, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}