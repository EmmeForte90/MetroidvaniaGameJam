using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TimelineSkipper : MonoBehaviour
{
    private PlayableDirector timeline = null;
    [SerializeField] 
    private float skipToSecond;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if(Input.GetButton("Fire1") && timeline.time != 0)
        {
            timeline.time = skipToSecond;
            StartCoroutine(skipOneFrame());
        }
    }

    IEnumerator skipOneFrame()
    {
        yield return null;
        timeline.time = timeline.time;
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
}
