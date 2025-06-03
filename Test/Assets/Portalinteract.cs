using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portalinteract : MonoBehaviour
{
    public GameObject PointTransfer;
    public GameObject Baloon;
    private bool cantouch = false;
    public string Area;

    void Awake(){Baloon.SetActive(false);}

    public void OnTriggerEnter(Collider collision){if (collision.gameObject.CompareTag("Player")){cantouch = true;Baloon.SetActive(true);}}
    public void OnTriggerExit(Collider collision){if (collision.gameObject.CompareTag("Player")){cantouch = false;Baloon.SetActive(false);}}

    void LateUpdate(){if(cantouch){if (Input.GetButtonDown("TastoX")){StartCoroutine(Transfer());}}}

    IEnumerator Transfer()
    { 
        GameManager.instance.BrainPlayer.StopInput = true;
        GameManager.instance.BrainPlayer.currentState = "Stop";
        GameManager.instance.BrainPlayer.horizontalInput = 0f;
        GameManager.instance.BrainPlayer.PlayAnimationLoop("Gameplay/idle");
        GameManager.instance.FadeClosed.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.instance.Area = Area;
        cantouch = false; Baloon.SetActive(false);
        GameManager.instance.Player.transform.position = PointTransfer.transform.position;
        GameManager.instance.FadeClosed.SetActive(false);
        GameManager.instance.FadeOpen.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.instance.BrainPlayer.StopInput = false;
    }

    
}
