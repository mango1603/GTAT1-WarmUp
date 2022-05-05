using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdObstacles : MonoBehaviour
{
    public float sendTimer = 1;
    public float frequency = 6;
    public float position;
    public GameObject myObstacle;
    public GameObject mainCharacter;

    void Update()
    {
        sendTimer -= Time.deltaTime;
        if (sendTimer <= 0)
        {
            position = Random.Range(-20, -40);
            transform.position = new Vector3(0, position, 0);
            Instantiate(myObstacle, transform.position, transform.rotation);
            sendTimer = frequency;
        }

        // if (mainCharacter != null) Time.timeScale = 1;
        // else Time.timeScale = 0;
    }
}