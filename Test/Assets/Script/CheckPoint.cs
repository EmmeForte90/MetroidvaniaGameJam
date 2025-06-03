using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject CheckPointP;

    public void OnTriggerEnter(Collider collision)
    {if (collision.gameObject.CompareTag("Player")){GameManager.instance.CheckPoint = CheckPointP;}}
}
