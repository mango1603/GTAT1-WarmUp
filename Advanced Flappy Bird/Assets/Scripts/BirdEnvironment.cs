using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnvironment : MonoBehaviour
{
    public float sendTimer = 0;

    public float frequency = 9.93f;

    public GameObject floor;
    
    void Update()
    {
        sendTimer -= Time.deltaTime;
        if (sendTimer <= 0)
        {
            Instantiate(floor, new Vector3(0, 0, -10), transform.rotation);
            sendTimer = frequency;
        }
    }
}
