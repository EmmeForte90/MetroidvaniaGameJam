using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameplayManager : MonoBehaviour
{
    public static bool playerExists;
    
private GameObject player; // Variabile per il player

    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
virtualCamera.Follow = player.transform;
virtualCamera.LookAt = player.transform;



        // Verifica se un'istanza del GameObject esiste già e distruggila se necessario
        if (playerExists) 
        {
            Destroy(gameObject);
        }
        else 
        {
            playerExists = true;
            DontDestroyOnLoad(gameObject);
            
        }
    }
}





