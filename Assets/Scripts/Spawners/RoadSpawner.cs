using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    private const float offset = 30f;

    [SerializeField] private List<GameObject> roads;
    

    private void Start()
    {
        if(roads != null && roads.Count > 0)
        {
            roads = roads.OrderBy(r => r.transform.position.z).ToList();
        }
    }

    public void MoveRoad()
    {
        GameObject moveRoad = roads.First();
        roads.Remove(moveRoad);
        float newZ = roads.Last().transform.position.z + offset;
        moveRoad.transform.position = new Vector3(0, 0, newZ);
        roads.Add(moveRoad);
    }
}