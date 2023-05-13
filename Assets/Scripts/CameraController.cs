using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private const float yOffset = 13f;
    private const float zOffset = -13f;
    private const float xRotation = 40;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        Quaternion.Euler(xRotation, 0, 0);
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y + yOffset, player.position.z + zOffset);
    }
}