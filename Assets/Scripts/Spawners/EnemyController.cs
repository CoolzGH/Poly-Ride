using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const float speed = 10f;
    private const float despawnDistanceBehind = 15f;
    private const float despawnDistanceForward = 300f;

    private Camera mainCamera;
    private Rigidbody enemyRigidbody;
    private Transform enemyTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyTransform = transform;
        if (PlayerPrefs.GetInt("LevelType") == 1 && (enemyTransform.position.x == -9 || enemyTransform.position.x == -3))
        {
            enemyRigidbody.velocity = new Vector3(0, 0, -speed);
        }
        else
        {
            enemyRigidbody.velocity = new Vector3(0, 0, speed);
        }
    }

    private void Update()
    {
        if (mainCamera.transform.position.z - enemyTransform.position.z > despawnDistanceBehind || enemyTransform.position.z - mainCamera.transform.position.z > despawnDistanceForward)
        {
            Destroy(gameObject);
        }
    }
}