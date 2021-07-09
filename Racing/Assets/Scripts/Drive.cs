using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drive : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider[] WCs;
    public GameObject[] Wheels;

    public GameObject brakeLight;

    [Header("Car Audio")]
    public AudioSource skidSound;
    public AudioSource highAccel;

    [Header("Wheels Effect")]
    public Transform skidTrailPrefab;
    public ParticleSystem smokePrefab;

    [Header("Gear System")]
    public Rigidbody rb;
    public float gearLength=3f;
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public int numGears = 5;
    public float maxSpeed = 200f;

    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    float rpm;
    int currentGear;
    float currentGearPerc;

    Transform[] skidTrails = new Transform[4];
    ParticleSystem[] skidSmoke = new ParticleSystem[4];

    [Header("Car Engine Properties")]
    public float torque=200f;
    public float maxSteerAngle = 30f;
    public float maxBrakeTorque = 500f;

    [Header("Player Name")]
    public GameObject playerNamePrefab;
    public Renderer jeepMesh;

    public string networkName = "";
    string[] aiNames = { "Adrin", "Lee", "Penny", "Merlin","John" };

    public void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel,-1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1)*maxBrakeTorque;

        if(brake!=0)
        {
            brakeLight.SetActive(true);
        }
        else
        {
            brakeLight.SetActive(false);
        }
        
        float thrustTorque = 0;
        if(currentSpeed<maxSpeed)
        {
            thrustTorque = accel * torque;
        }

        for (int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;
            if(i<2)
            {
                WCs[i].steerAngle = steer;
            }
            else
            {
                WCs[i].brakeTorque = brake;
            }
            
            Quaternion quat;
            Vector3 position;

            WCs[i].GetWorldPose(out position, out quat);

            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quat;
        }
    }

    public void CheckForSkid()
    {
        int numSkidding = 0;

        for(int i=0;i<4;i++)
        {
            WheelHit wheelHit;

            WCs[i].GetGroundHit(out wheelHit);

            if(Mathf.Abs(wheelHit.forwardSlip)>=0.4f || Mathf.Abs(wheelHit.sidewaysSlip)>=0.4f)
            {
                numSkidding++;
                if(!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                // StartSkidTrail(i);
                skidSmoke[i].transform.position = WCs[i].transform.position - WCs[i].transform.up * WCs[i].radius;
                skidSmoke[i].Emit(1);
            }

            else
            {
                //EndSkidTrail(i);
            }
        }

        if(numSkidding==0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }

    void StartSkidTrail(int i)
    {
        if(skidTrails[i]==null)
        {
            skidTrails[i] = Instantiate(skidTrailPrefab);
        }

        skidTrails[i].parent = WCs[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WCs[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if(skidTrails[i]==null)
        {
            return;
        }

        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }

    public void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
            Mathf.Abs(currentSpeed / maxSpeed));

        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);
        var gearNumFactor = currentGear / (float)numGears;

        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearmax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * (currentGear);

        if(currentGear>0 && speedPercentage<downGearMax)
        {
            currentGear--;
        }

        if(speedPercentage>upperGearmax && currentGear<numGears-1)
        {
            currentGear++;
        }

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    void Start()
    {
        for(int i=0;i<4;i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
        brakeLight.SetActive(false);

        GameObject playerName = Instantiate(playerNamePrefab);
        playerName.GetComponent<NameUIController>().target=rb.gameObject.transform;

        if(GetComponent<AIController>().enabled)
        {
            if(networkName!="")
            {
                playerName.GetComponent<Text>().text = networkName;
            }
            else
            {
                playerName.GetComponent<Text>().text = aiNames[Random.Range(0, aiNames.Length)];
            }
        }
        else
        {
            playerName.GetComponent<Text>().text = PlayerPrefs.GetString("PlayerName");
        }
        playerName.GetComponent<NameUIController>().carRend = jeepMesh;
    }
}
