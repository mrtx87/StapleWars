using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishHandling : MonoBehaviour {


    public float duration;
    public float timeCounter = 0;
    public bool isFinished = false;
    public bool runTimer = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CountTime();

        if(checkIfFinished())
        {
            
        }

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

    private void startTimer()
    {
        runTimer = true;
    }

    public bool checkIfFinished()
    {
        return isFinished & !runTimer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("GroundedBlock"))
        {
            startTimer();
        }
    }
}
