using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTask : MonoBehaviour {

    public float duration;
    public float timeCounter = 0;
    public bool isFinished = false;
    public bool runTimer = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CountTime();
		
	}

    private void CountTime()
    {
        if (runTimer)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= duration)
            {
                isFinished = true;
                runTimer = false;
            }
        }
        
    }

    public bool checkIsFinished()
    {
        return isFinished & !runTimer;
    }
}
