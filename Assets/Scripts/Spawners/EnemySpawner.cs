using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<float> spawnPositions = new List<float>() { -9f, -3f, 3f, 9f };
    private bool canSpawn;
    private int lvlType;
    private GameObject spawnedEnemy;
    private float spawnOffset = 100f;
    private float spawnHeigth = 0.2f;

    private void Start()
    {
        lvlType = PlayerPrefs.GetInt("LevelType");
    }

    public void SpawnEnemy(float zPosCar, List<GameObject> vehiclePrefabs)
    {
        canSpawn = true;
        int indexOfEnemy = Random.Range(0, vehiclePrefabs.Count);
        int indexOfPosition = Random.Range(0, spawnPositions.Count);
        GameObject enemyToSpawn = vehiclePrefabs[indexOfEnemy];
        Vector3 posToSpawn = new Vector3(spawnPositions[indexOfPosition], spawnHeigth, zPosCar + spawnOffset);
        Collider[] hitColliders = Physics.OverlapSphere(posToSpawn, 20f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                canSpawn = false;
                break;
            }
        }
        if (canSpawn)
        {
            if (lvlType == 1 && (indexOfPosition == 0 || indexOfPosition == 1))
            {
                spawnedEnemy = Instantiate(enemyToSpawn, posToSpawn, Quaternion.Euler(0, 180, 0));
            }
            else
            {
                spawnedEnemy = Instantiate(enemyToSpawn, posToSpawn, Quaternion.identity);
            }
            spawnedEnemy.tag = "Enemy";
            spawnedEnemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            spawnedEnemy.AddComponent<EnemyController>();
        }
    }
}