using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

    public class TimelineController : MonoBehaviour 
    {
    private PlayableDirector _director;
    public GameObject Cutscene;
    public int ID;


    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        
    }

    private void Update()
    {
        if ((Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && _director.time != 0)
        { 
            StartTimeline();
        }
    }

public  void TimelineRepeat()
{
    CutsceneManager.Instance.TimelineStart(ID);
}

public  void TimelineDontRepeat()
{
    CutsceneManager.Instance.TimelineEnd(ID);
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
