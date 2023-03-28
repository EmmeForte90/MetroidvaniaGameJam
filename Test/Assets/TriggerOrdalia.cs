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
    public BoxCollider2D trigger;



    void Start()
    {
       virtualCamera = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>(); //ottieni il riferimento alla virtual camera di Cinemachine
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
            AudioManager.instance.CrossFadeOUTAudio(2);
            AudioManager.instance.CrossFadeINAudio(1);
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

    trigger.enabled = false;
    ActorOrdalia.Instance.Standup();
    AudioManager.instance.CrossFadeOUTAudio(1);
    AudioManager.instance.CrossFadeINAudio(2);
    yield return new WaitForSeconds(2);
    ActorOrdalia.Instance.idle();
    yield return new WaitForSeconds(TimeStart);
    Actor.gameObject.SetActive(false);
    Enemy.gameObject.SetActive(true);
    //Instantiate(Enemy, Actor.transform.position, transform.rotation);
    AiEnemysword.instance.chaseThreshold = 9f; // soglia di distanza per iniziare l'inseguimento

    }


}

