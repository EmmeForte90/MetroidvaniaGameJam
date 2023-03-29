using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SkillItem : MonoBehaviour
{
   [Header("Skill")]
    public Skill Skill;

public int id;

 [Header("Audio")]
 [HideInInspector] public float basePitch = 1f;
    [HideInInspector] public float randomPitchOffset = 0.1f;
[SerializeField] public AudioClip[] listmusic; // array di AudioClip contenente tutti i suoni che si vogliono riprodurre
private AudioSource[] bgm; // array di AudioSource che conterrà gli oggetti AudioSource creati
   public AudioMixer SFX; 


    [Header("VFX")]
    [SerializeField] GameObject VFX;
  

public void PlayMFX(int soundToPlay)
    {
        // Imposta la pitch dell'AudioSource in base ai valori specificati.
        bgm[soundToPlay].pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset); 
        bgm[soundToPlay].Play();
    }


    public void Pickup()
    {
       GameplayManager.instance.SkillAc(id);
      // PlayMFX(0);
        Destroy(gameObject);
    }

public void SkillDosentExist()
    {
 Destroy(gameObject);
 }

 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        Pickup();
        Instantiate(VFX, transform.position, transform.rotation);
       // take.Play();

        }
    }
}

