using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TriggerOrdalia : MonoBehaviour
{

    public float TimeStart;
    public GameObject[] Arena;
    public GameObject Enemy;
    private CinemachineVirtualCamera virtualCamera; //riferimento alla virtual camera di Cinemachine
    public GameObject Camera;
    private GameObject player; // Variabile per il player
    public GameObject Actor;



    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); //ottieni il riferimento alla virtual camera di Cinemachine
        player = GameObject.FindWithTag("Player");
    }

  void Update()
    {
        if (Enemy == null)
        {
            virtualCamera.Follow = player.transform;   
            foreach (GameObject arenaObject in Arena)
        {
            arenaObject.SetActive(false);
            Destroy(gameObject);
        }
        }     

    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
         foreach (GameObject arenaObject in Arena)
        {
            arenaObject.SetActive(true);
            virtualCamera.Follow = Camera.transform;
            StartCoroutine(StartOrdalia());
        }
        }
    }

    IEnumerator StartOrdalia()
    {
        ActorOrdalia.Instance.Standup();
        yield return new WaitForSeconds(TimeStart); 
        Actor.gameObject.SetActive(false);
        Enemy.gameObject.SetActive(true);


    }

}

