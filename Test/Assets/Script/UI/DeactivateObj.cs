using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObj : MonoBehaviour
{
    public float lifeTime = 0;
    public GameObject title;
    private void OnEnable() {StartCoroutine(Destroy());}
    IEnumerator Destroy(){yield return new WaitForSeconds(lifeTime); title.gameObject.SetActive(false);}
}