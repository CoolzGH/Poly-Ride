using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI currentDistanceText;
    [SerializeField] private TextMeshProUGUI highDistanceText;
    [SerializeField] private GameObject UI;
    [SerializeField] private InterstitialAds ad;
    [SerializeField] private DistanceController distanceController;
    [SerializeField] private CoinManager coinManager;
    public static bool isAdsOff = false;

    private string highscoreBase;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private Animator animator;
    private Dictionary<string, object> userValues = new Dictionary<string, object>();

    private void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        highscoreBase = PlayerPrefs.GetInt("LevelType") == 0 ? "highscoreOne" : "highscoreTwo";
        gameOverPanel.SetActive(false);
        highDistanceText.text = "Лучшая дистанция:\nЗагрузка...";
        if (user != null)
        {
            databaseReference.Child("users").Child(user.UserId).Child(highscoreBase).ValueChanged += HandleValueChanged;
        }
        else
        {
            highDistanceText.text = "Авторизуйтесь для получения лучшей дистанции";
        }
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Failed to get user high score: " + args.DatabaseError);
            return;
        }
        long highScore = (long)args.Snapshot.Value;
        Debug.Log("User " + user.UserId + " high score: " + highScore);
        highDistanceText.text = $"Лучшая дистанция:\n{highScore}";
    }

    public void DoGameOver(Rigidbody carRigidbody)
    {
        distanceController.SaveDistance();
        coinManager.StartCoroutine(coinManager.SaveCoins(distanceController.GetDistance()));
        gameOverPanel.SetActive(true);
        UI.SetActive(false);
        if (animator != null)
        {
            animator.Play("GameOver");
        }
        currentDistanceText.text = $"Текущая дистанция:\n{distanceController.GetDistance()}";
        if (user == null)
        {
            highDistanceText.text = "Авторизуйтесь для получения лучшей дистанции";
        }
        if (!isAdsOff)
        {
            PlayerPrefs.SetInt("tempAds", PlayerPrefs.GetInt("tempAds") + 1);
            if (PlayerPrefs.GetInt("tempAds") >= 3)
            {
                ad.ShowAd();
                PlayerPrefs.SetInt("tempAds", 0);
            }
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        carRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
