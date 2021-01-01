using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    //refrence pointer to a method, pass method as a variable for a callback
    public delegate void Callback();

    private class TimedEvent
    {
        public float TimeToExecute;
        public Callback Method;
    }

    //declare a list of TimedEvent object
    private List<TimedEvent> events;

	void Awake () {
        events = new List<TimedEvent>();
	}

    //add a TimedEvent object to list
    public void add(Callback method, float inSeconds)
    {
        events.Add(new TimedEvent { TimeToExecute = inSeconds + Time.time, Method = method });
    }

	void Update () {
        if (events.Count == 0) //if events list is empty
            return;

        for (int I = 0; I < events.Count; I++)
        {
            var timedEvent = events[I];
            if(timedEvent.TimeToExecute <= Time.time)
            {
                timedEvent.Method();
                events.Remove(timedEvent);
            }
        }
	}
}
