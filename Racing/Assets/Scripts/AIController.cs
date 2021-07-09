using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;

    [Header("AICar Sensitivites")]
    public float steeringSensitivity = 0.01f;
    public float brakeSensitivity = 3f;
    public float accelSensitivity = 0.3f;

    Drive ds;
    GameObject tracker;

    int currentTrackerWp = 0;
    float lookAhead = 10f;
    float lastTimeMoving = 0;
    float finishSteer;

    CheckpointManager cpm;

    void Start()
    {
        if(circuit==null)
        {
            circuit = GameObject.FindGameObjectWithTag("Circuit").GetComponent<Circuit>();
        }

        ds = GetComponent<Drive>();

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        tracker.GetComponent<MeshRenderer>().enabled = false;
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.transform.position = ds.rb.transform.position;
        tracker.transform.rotation = ds.rb.transform.rotation;

        GetComponent<Ghost>().enabled = false;
        finishSteer = Random.Range(-1, 1);
    }

    void Update()
    {
        if (!RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }

        if (cpm == null)
            cpm = ds.rb.GetComponent<CheckpointManager>();

        if(cpm.lap==RaceMonitor.totalLaps+1)
        {
            ds.highAccel.Stop();
            ds.Go(0, finishSteer, 0);
            return;
        }

        ProgressTracker();
        Vector3 localTarget;

        if(ds.rb.velocity.magnitude>1)
        {
            lastTimeMoving = Time.time;
        }

        if(Time.time>lastTimeMoving+4 || ds.rb.gameObject.transform.position.y<-5)
        {
            ds.rb.gameObject.transform.position = cpm.lastCP.transform.position + transform.up * 2;
            ds.rb.gameObject.transform.rotation = cpm.lastCP.transform.rotation;
            tracker.transform.position = cpm.lastCP.transform.position;
            ds.rb.gameObject.layer = 8;
            GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if(Time.time<ds.rb.GetComponent<AvoidDetector>().avoidTime)
        {
            localTarget = tracker.transform.right * ds.rb.GetComponent<AvoidDetector>().avoidPath;
        }
        else
        {
            localTarget = ds.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1)*Mathf.Sign(ds.currentSpeed);

        float speedFactor = ds.currentSpeed / ds.maxSpeed;
        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);

        float cornerFactor = corner / 90f;

        float brake = 0;
        if(corner>10 && speedFactor>0.1f)
        {
            brake = Mathf.Lerp(0, 1 + speedFactor * brakeSensitivity, cornerFactor);
        }

        float accel = 1;
        if(corner>20 && speedFactor>0.2f)
        {
            accel = Mathf.Lerp(0, 1 * accelSensitivity, 1 - cornerFactor);
        }

        float prevTorque = ds.torque;
        if(speedFactor<0.3 && ds.rb.gameObject.transform.forward.y>0.1f)
        {
            ds.torque *= 3f;
            accel = 1;
            brake = 0;
        }
      
        ds.Go(accel, steer, brake);
        ds.CheckForSkid();
        ds.CalculateEngineSound();

        ds.torque = prevTorque;
    }

    void ProgressTracker()
    {
        Debug.DrawLine(ds.rb.transform.position, tracker.transform.position);

        if (Vector3.Distance(ds.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) return;

        tracker.transform.LookAt(circuit.waypoints[currentTrackerWp].transform.position);
        tracker.transform.Translate(0, 0, 1.0f); //speed of tracker

        if(Vector3.Distance(tracker.transform.position,circuit.waypoints[currentTrackerWp].transform.position)<1)
        {
            currentTrackerWp++;
            if(currentTrackerWp>=circuit.waypoints.Length)
            {
                currentTrackerWp = 0;
            }
        }
    }

    void ResetLayer()
    {
        ds.rb.gameObject.layer = 0;
        GetComponent<Ghost>().enabled = false;
    }
}
