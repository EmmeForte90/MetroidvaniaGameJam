using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

    public class TimelineController : MonoBehaviour 
    {
    private PlayableDirector _director;
    

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        //Press F to next dialogue in timeline//
        if ((Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && _director.time != 0)
        { 
            StartTimeline();
        }
    }

    public void StartTimeline()
    {  
        _director.time = _director.time;
        _director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    public void StopTimeline()
    {
        _director.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    
}
