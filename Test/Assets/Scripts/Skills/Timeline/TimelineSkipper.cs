using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TimelineSkipper : MonoBehaviour
{
    private PlayableDirector timeline = null;
    [SerializeField] 
    private float skipToSecond;
    public GameObject warningMes;
    private bool againPress = false;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Pause") && !againPress)
        {
            warningMes.gameObject.SetActive(true);
            againPress = true;
        }
        if(Input.GetButtonDown("Pause") && timeline.time != 0 && againPress)
        {
            timeline.time = skipToSecond;
            warningMes.gameObject.SetActive(true);
            StartCoroutine(skipOneFrame());
        }
    }

    IEnumerator skipOneFrame()
    {
        yield return null;
        warningMes.gameObject.SetActive(false);
        againPress = false;
        timeline.time = timeline.time;
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
}
