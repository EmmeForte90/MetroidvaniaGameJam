using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

   [Header("Music")]
    [SerializeField] public AudioClip[] listmusic; // array di AudioClip contenente tutti i suoni che si vogliono riprodurre
    private AudioSource[] bgm; // array di AudioSource che conterrà gli oggetti AudioSource creati
   
    [SerializeField] public AudioClip[] SoundDestroy; // array di AudioClip contenente tutti i suoni che si vogliono riprodurre
    private AudioSource[] sgm; // array di AudioSource che conterrà gli oggetti AudioSource creati


    public AudioMixer MSX;
    public AudioMixer SFX;
 
public static AudioManager instance;


private void Awake() 
{ 
    if (instance == null)
        {
            instance = this;
        }
 bgm = new AudioSource[listmusic.Length]; // inizializza l'array di AudioSource con la stessa lunghezza dell'array di AudioClip
    for (int i = 0; i < listmusic.Length; i++) // scorre la lista di AudioClip
    {
        bgm[i] = gameObject.AddComponent<AudioSource>(); // crea un nuovo AudioSource come componente del game object attuale (quello a cui è attaccato lo script)
        bgm[i].clip = listmusic[i]; // assegna l'AudioClip corrispondente all'AudioSource creato
    }

sgm = new AudioSource[SoundDestroy.Length]; // inizializza l'array di AudioSource con la stessa lunghezza dell'array di AudioClip
    for (int i = 0; i < SoundDestroy.Length; i++) // scorre la lista di AudioClip
    {
        sgm[i] = gameObject.AddComponent<AudioSource>(); // crea un nuovo AudioSource come componente del game object attuale (quello a cui è attaccato lo script)
        sgm[i].clip = SoundDestroy[i]; // assegna l'AudioClip corrispondente all'AudioSource creato
    }

}

 public void SetVolume(float volume)
    {
        MSX.SetFloat("Volume", volume);

    }


     public void SetSFX(float volume)
    {
        SFX.SetFloat("Volume", volume);

    }

    public void PlayMFX(int soundToPlay)
    {
        bgm[soundToPlay].Stop();
        bgm[soundToPlay].pitch = Random.Range(.9f, 1.1f);
        bgm[soundToPlay].Play();
    }

    public void StopMFX(int soundToPlay)
    {
        bgm[soundToPlay].Stop();
    }

 public void PlaySFX(int soundToPlay)
    {
        sgm[soundToPlay].Play();
    }



  public void CrossFadeINAudio(int soundToPlay)
    {        
        StartCoroutine(FadeIn(bgm[soundToPlay], 1f));

    }


      public void CrossFadeOUTAudio(int soundToPlay)
    {
        StartCoroutine(FadeOut(bgm[soundToPlay], 1f));

    }



public  IEnumerator FadeOut(AudioSource bgm, float FadeTime)
    {
        float startVolume = bgm.volume;
 
        while (bgm.volume > 0)
        {
            bgm.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        bgm.Stop();
        bgm.volume = startVolume;
    }
 
    public  IEnumerator FadeIn(AudioSource bgm, float FadeTime)
    {
        float startVolume = 0.2f;
 
        bgm.volume = 0;
        bgm.Play();
 
        while (bgm.volume < 1.0f)
        {
            bgm.volume += startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        bgm.volume = 0.3f;
    }

}


