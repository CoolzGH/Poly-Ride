using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSpawner : MonoBehaviour
{
    private const float plotSize = 30f;
    private const float xPosLeft = -37.5f;
    private const float xPosRight = 37.5f;
    private const int initAmount = 6;

    private float lastPosZ = -22.5f;

    [SerializeField] private List<GameObject> plots;

    private void Start()
    {
        for(int i = 0; i < initAmount; i++)
        {
            SpawnPlot();
        }
    }

    public void SpawnPlot()
    {
        GameObject plotLeft = plots[Random.Range(0, plots.Count)];
        GameObject plotRight = plots[Random.Range(0, plots.Count)];

        float zPos = lastPosZ + plotSize;

        GameObject plotL = Instantiate(plotLeft, new Vector3(xPosLeft, 0f, zPos), plotLeft.transform.rotation);
        GameObject plotR = Instantiate(plotRight, new Vector3(xPosRight, 0f, zPos), new Quaternion(0, 180, 0, 0));

        plotL.AddComponent<PlotController>();
        plotR.AddComponent<PlotController>();

        lastPosZ += plotSize;
    }
}