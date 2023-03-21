using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    
    public float[] position;

    public PlayerData(Move Player)
    {
//Tutti i dati del character
position = new float[3];
position[0] = Player.transform.position.x;
position[1] = Player.transform.position.y;
position[2] = Player.transform.position.z;
    }
}
