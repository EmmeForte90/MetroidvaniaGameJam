using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    public float lifeTime = 0;
    public GameObject title;
    private void OnEnable() {StartCoroutine(Destroy());}
    IEnumerator Destroy(){yield return new WaitForSeconds(lifeTime); Destroy(title);}
}
