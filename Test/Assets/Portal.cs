using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject PointTransfer;
    public string Area;

    public void OnTriggerEnter(Collider collision){if (collision.gameObject.CompareTag("Player")){StartCoroutine(Transfer());}}

    IEnumerator Transfer()
    {
        GameManager.instance.BrainPlayer.StopInput = true;
        GameManager.instance.BrainPlayer.currentState = "Stop";
        GameManager.instance.BrainPlayer.horizontalInput = 0f;
        GameManager.instance.BrainPlayer.PlayAnimationLoop("Gameplay/idle");
        GameManager.instance.FadeClosed.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.instance.Area = Area;
        GameManager.instance.Player.transform.position = PointTransfer.transform.position;
        GameManager.instance.FadeClosed.SetActive(false);
        GameManager.instance.FadeOpen.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.instance.BrainPlayer.StopInput = false;
    }
}
