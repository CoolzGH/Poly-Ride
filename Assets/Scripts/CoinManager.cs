using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsInGameTxt;
    [SerializeField] private TextMeshProUGUI coinsGameOverTxt;
    [SerializeField] private GameObject coinsPrefab;
    private List<float> spawnPositions = new List<float>() { -9f, -3f, 3f, 9f };
    private int coinsCount;
    private int spawnTry;
    private float spawnOffset = 100f;
    private float spawnHeigth = 0.2f; 
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference databaseReference;
    private Dictionary<string, object> userValues = new Dictionary<string, object>();

    private void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }

    private void Start()
    {
        coinsCount = 0;
        spawnTry = 0;
        if (user != null)
        {
            coinsInGameTxt.text = "0";
        }
    }

    public void SpawnCoins(float zPosCar)
    {
        if (spawnTry == 0)
        {
            int indexOfPosition = Random.Range(0, spawnPositions.Count);
            Vector3 posToSpawn = new Vector3(spawnPositions[indexOfPosition], spawnHeigth, zPosCar + spawnOffset);
            Instantiate(coinsPrefab, posToSpawn, Quaternion.identity);
            spawnTry = 4;
        }
        else
        {
            spawnTry--;
        }
    }

    public void AddCoinToCount(GameObject coin)
    {
        if (user != null)
        {
            coinsCount += 50;
            coinsInGameTxt.text = $"{coinsCount}";
        }
        Destroy(coin);
    }

    public IEnumerator SaveCoins(int distance)
    {
        if (user != null)
        {
            string userId = user.UserId;
            var task = databaseReference.Child("users").Child(userId).Child("coins").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.LogError("Failed to get user coins: " + task.Exception);
                yield break;
            }
            if (task.Result.Value == null)
            {
                userValues["coins"] = (distance / 2) + coinsCount;
                databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
                coinsGameOverTxt.text = $"+{(distance / 2) + coinsCount}";
            }
            else
            {
                long coins = (long)task.Result.Value;
                Debug.Log("User " + userId + " coins: " + coins);
                userValues["coins"] = coins + ((distance / 2) + coinsCount);
                databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
                coinsGameOverTxt.text = $"+{(distance / 2) + coinsCount}";
            }
        }
        else
        {
            coinsGameOverTxt.text = "Авторизуйтесь";
        }
    }
}