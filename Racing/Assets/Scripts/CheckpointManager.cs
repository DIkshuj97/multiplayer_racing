using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject lastCP;

    public int lap = 0;
    public int checkPoint = -1;
    public float timeEntered = 0;
    int checkpointCount;
    int nextCheckPoint;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpointCount = cps.Length;
        foreach(GameObject c in cps)
        {
            if(c.name=="0")
            {
                lastCP = c;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Checkpoint")
        {
            int thisCPNumber = int.Parse(col.gameObject.name);
            if(thisCPNumber==nextCheckPoint)
            {
                lastCP = col.gameObject;
                checkPoint = thisCPNumber;
                timeEntered = Time.time;
                if (checkPoint == 0) lap++;

                nextCheckPoint++;
                if(nextCheckPoint>=checkpointCount)
                {
                    nextCheckPoint = 0;
                }
            }
        }
    }
}
