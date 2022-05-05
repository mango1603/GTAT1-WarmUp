using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    public GameObject deathExplosion;
    public GameObject checkpointSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "environment")
        {
            GameObject deathObj = Instantiate(deathExplosion, transform.position, transform.rotation) as GameObject;
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "checkpoint")
        {
            Instantiate(checkpointSound, transform.position, transform.rotation);
        }
    }
}
