using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using System.Linq;

public class TriggerOrdalia : MonoBehaviour
{

    public float TimeStart;
    public GameObject[] Arena;
    public GameObject Enemy;
    public GameObject[] EnemyPrefab;
    public Transform[] SpawnPoints; // array di punti di spawn
    private CinemachineVirtualCamera virtualCamera; //riferimento alla virtual camera di Cinemachine
    public GameObject Camera;
    private GameObject player; // Variabile per il player
    public GameObject Actor;
    public BoxCollider2D trigger;
    private int enemiesDestroyedCount = 0;
    private bool generateWaves = true;
    private bool StartOndata = false;
    private int waveCount = 0; // contatore delle ondate
    private int COnde;
    private int EnemyCount = 0;
    public int MaxEnemy = 0;
    public float SpawnInterval = 2f;
    public float WaveInterval = 2f;

    public int[] waves = {3, 5, 7, 9};//Numerp di nemici generati per ondate
    private int lastSpawnIndex = -1;
           

    void Start()
    {
    virtualCamera = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>(); //ottieni il riferimento alla virtual camera di Cinemachine
    player = GameObject.FindWithTag("Player");
    EnemyCount = MaxEnemy;
    }

  void Update()
    {

        if (Enemy == null)
        {
            if(!StartOndata)
            {
            StartCoroutine(GeneratEnemy());
            StartOndata = true;
            }
        }   
 

    }



public void EndOrdalia()
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

IEnumerator GeneratEnemy()
{     
    
    COnde = waves.Length;

while (EnemyPrefab.Length > 0 && waveCount < COnde) // finché ci sono ancora nemici nell'array e non abbiamo raggiunto il numero di ondate
{
    int EnemiesPerWave = waves[waveCount];
    waveCount++; // diminuisce il contatore delle ondate
    for (int i = 0; i < EnemiesPerWave; i++) // per ogni nemico per ondata
    {
        lastSpawnIndex++;
        if (lastSpawnIndex >= SpawnPoints.Length)
        {
            lastSpawnIndex = 0;
        }
        GameObject enemyToSpawn = EnemyPrefab[0]; // prendi il primo nemico dell'array
        Instantiate(enemyToSpawn, SpawnPoints[lastSpawnIndex].position, transform.rotation); // spawn il nemico nella prossima posizione di spawn
        AiEnemysword.instance.chaseThreshold = 10f; // soglia di distanza per iniziare l'inseguimento
        EnemyPrefab = EnemyPrefab.Where((enemy, index) => index != 0).ToArray();
        EnemyCount--;
        yield return new WaitForSeconds(SpawnInterval);
    }
    if (waveCount < COnde) // se non è l'ultima ondata
    {
        yield return new WaitForSeconds(WaveInterval); // aspetta l'intervallo tra le ondate
    }
}
if (EnemyCount <= 0 && waveCount >= COnde) // se tutti i nemici sono stati sconfitti e abbiamo raggiunto l'ultimo livello di ondate
{
    EndOrdalia(); // chiama la funzione EndOrdalia
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

    }


}

