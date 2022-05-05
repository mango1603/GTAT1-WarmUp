using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBounces : MonoBehaviour
{
    public float defaultYSpeed;
    [SerializeField] private float ySpeed;
    public float yTarget;
    public GameObject soundBounce;

    private void Awake()
    {
        ySpeed = defaultYSpeed;
    }

    void Update()
    {
        transform.Translate(0, ySpeed, 0);
        ySpeed = Mathf.Lerp(ySpeed, yTarget, 0.025f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(soundBounce, transform.position, transform.rotation);
            ySpeed = defaultYSpeed;
        }
    }
}