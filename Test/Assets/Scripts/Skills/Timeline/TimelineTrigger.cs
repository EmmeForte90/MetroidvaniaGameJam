using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    //public TimelineManager.filmati filmati;
    private PlayableDirector timeline;
    public int id;
    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player") //&& !TimelineManager.TimelineStart(id))
        {
            c.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            timeline.Play();
            this.enabled = false;
            TimelineManager.TimelineEnd(id);
        }
    }
}
