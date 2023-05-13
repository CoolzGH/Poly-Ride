using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class DistanceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI uiDistance;

    private GameObject player;
    private int distance;
    private string highscoreBase;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private Dictionary<string, object> userValues = new Dictionary<string, object>();

    private void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }

    private void Start()
    {
        highscoreBase = PlayerPrefs.GetInt("LevelType") == 0 ? "highscoreOne" : "highscoreTwo";
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        distance = Mathf.RoundToInt(player.transform.position.z);
        uiDistance.text = distance.ToString() + " m";
    }

    public void SaveDistance()
    {
        if (user != null)
        {
            string userId = user.UserId;
            databaseReference.Child("users").Child(userId).Child(highscoreBase).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to get user high score: " + task.Exception);
                    return;
                }
                if (task.Result.Value == null)
                {
                    userValues[highscoreBase] = distance;
                    databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
                }
                else
                {
                    long highScore = (long)task.Result.Value;
                    Debug.Log("User " + userId + " high score: " + highScore + " In DistanceController");
                    if (highScore < distance)
                    {
                        userValues[highscoreBase] = distance;
                        databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
                    }
                }
            });
        }
    }

    public int GetDistance()
    {
        return distance;
    }
}