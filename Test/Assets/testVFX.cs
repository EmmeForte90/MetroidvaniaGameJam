using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class testVFX : MonoBehaviour
{

    [SerializeField] GameObject vert;
    [SerializeField] GameObject lunge;
    [SerializeField] GameObject hori;
    [SerializeField] public Transform slashpoint;
    private bool vfx = false;
    private float vfxTimer = 0.5f;
    public SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Spine.Skeleton _skeleton;
    Spine.EventData eventData;

void Start()
    {
        _spineAnimationState = GetComponent<Spine.Unity.SkeletonAnimation>().AnimationState;
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.skeleton;
    _spineAnimationState.Event += HandleEvent;
    }



    void Update()
    {
         if(vfx)
        {vfxTimer -= Time.deltaTime; //decrementa il timer ad ogni frame
        if (vfxTimer <= 0f) {
        vfx = false;
        lunge.gameObject.SetActive(false);
        vert.gameObject.SetActive(false);
        hori.gameObject.SetActive(false);
        }}
    _spineAnimationState.Event += HandleEvent;
    }

    void HandleEvent (TrackEntry trackEntry, Spine.Event e) {

if (e.Data.Name == "ver") {
    if(!vfx)
    {
        // Inserisci qui il codice per gestire l'evento.
        lunge.gameObject.SetActive(false);
        vert.gameObject.SetActive(true);
        hori.gameObject.SetActive(false);        
        vfx = true;
    }
    }

if (e.Data.Name == "lun") {
        if(!vfx)
    {
        // Inserisci qui il codice per gestire l'evento.
        lunge.gameObject.SetActive(true);
        vert.gameObject.SetActive(false);
        hori.gameObject.SetActive(false);        
        vfx = true;
    }
    }

if (e.Data.Name == "hor") {
    if(!vfx)
    {
        // Inserisci qui il codice per gestire l'evento.
        lunge.gameObject.SetActive(false);
        vert.gameObject.SetActive(false);
        hori.gameObject.SetActive(true);        
        vfx = true;
    }
    }

}
}


