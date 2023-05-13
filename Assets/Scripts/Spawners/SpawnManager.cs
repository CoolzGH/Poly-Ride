using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> vehiclePrefabs;
    [SerializeField] private GameObject gasButton;
    [SerializeField] private GameObject brakeButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject gameManager;
    private RoadSpawner roadSpawner;
    private PlotSpawner plotSpawner;
    private EnemySpawner enemySpawner;
    private PlayerCar playerScript;
    private CarData carData;

    private void Awake()
    {
        GameObject player = Instantiate(vehiclePrefabs[MainMenu.GetSelectedCarIndex()], new Vector3(0, 0.2f, 0), Quaternion.identity);
        player.name = "Player";
        player.tag = "Player";
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        playerScript = player.AddComponent<PlayerCar>();
        carData = player.GetComponent<CarData>();
        playerScript.InitCarData(carData);
        playerScript.gasButton = gasButton;
        playerScript.brakeButton = brakeButton;
        playerScript.leftButton = leftButton;
        playerScript.rightButton = rightButton;
        playerScript.spawnManager = gameObject.GetComponent<SpawnManager>();
        playerScript.gameOver = gameManager.GetComponent<GameOver>();
        playerScript.coinManager = gameManager.GetComponent<CoinManager>();
    }

    private void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
        plotSpawner = GetComponent<PlotSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
    }

    public void SpawnTriggerEnterred(float zCarPos)
    {
        roadSpawner.MoveRoad();
        plotSpawner.SpawnPlot();
        enemySpawner.SpawnEnemy(zCarPos, vehiclePrefabs);
        playerScript.coinManager.SpawnCoins(zCarPos);
    }
}