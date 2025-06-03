using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointShrine : MonoBehaviour
{
    public GameObject Shrine;
       
    public void OnTriggerEnter(Collider collision){if (collision.gameObject.CompareTag("Player")){RestoreandCheckPoint();}}
    public void RestoreandCheckPoint()
    {
        GameManager.instance.SavedCheck = Shrine;
        GameManager.instance.E_cur = GameManager.instance.E_max;
        GameManager.instance.M_cur = GameManager.instance.M_max;
        GameManager.instance.BrainPlayer.isDie = false;
    }

}
