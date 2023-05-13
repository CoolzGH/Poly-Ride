using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotController : MonoBehaviour
{
    private Camera mainCamera;
    private Transform plotTransform;
    private float distanceToDestroy = 15f;

    private void Start()
    {
        mainCamera = Camera.main;
        plotTransform = transform;
    }

    private void Update()
    {
        if(mainCamera.transform.position.z - plotTransform.position.z > distanceToDestroy)
        {
            Destroy(gameObject);
        }
    }
}