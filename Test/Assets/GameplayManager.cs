using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameplayManager : MonoBehaviour
{
    public static bool playerExists;
    


    private void Awake()
    {
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





