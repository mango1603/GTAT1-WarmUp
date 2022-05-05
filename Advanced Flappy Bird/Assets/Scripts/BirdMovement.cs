using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public float life = 20;

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0) Destroy(gameObject);
        else transform.Translate(0, 0, 10 * Time.deltaTime);
    }
}