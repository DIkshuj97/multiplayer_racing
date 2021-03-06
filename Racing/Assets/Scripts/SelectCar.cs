using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCar : MonoBehaviour
{
    public GameObject[] cars;
    int currentCar = 0;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerCar"))
        {
           currentCar = PlayerPrefs.GetInt("PlayerCar");
        }
        transform.LookAt(cars[currentCar].transform.position);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCar++;
            if(currentCar>cars.Length-1)
            {
                currentCar = 0;
            }

            PlayerPrefs.SetInt("PlayerCar", currentCar);
        }

        Quaternion lookDir = Quaternion.LookRotation(cars[currentCar].transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime);
    }
}
