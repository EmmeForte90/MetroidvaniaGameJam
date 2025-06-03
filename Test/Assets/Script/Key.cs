using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    [Header("Setting")]
    public string TypeKey = "Normal";
    public GameObject ThisKey;
    public int IdKey = 0;
    public int SpriteKey = 0;
    [Header("VFX")]
    public GameObject VFXExplode;
    private bool take = false;

    private void OnEnable(){StartCoroutine(DataChest());}
    private IEnumerator DataChest()
    {
        yield return new WaitForSeconds(0.2f);
        if (GameManager.instance.HaveKey[IdKey])
        {ThisKey.SetActive(false);}
        else{ThisKey.SetActive(true);}
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            take = true;
            Instantiate(VFXExplode, transform.position, Quaternion.identity);
            GameManager.instance.KeyUI.sprite = GameManager.instance.Key[SpriteKey];
            GameManager.instance.HaveKey[IdKey] = true;
            Destroy(gameObject);
        }
    }
}