using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;


public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static bool isLoaded = false;

    [SerializeField] private Button buttonShowAd;
    [SerializeField] private TextMeshProUGUI buttonShowAdTxt;
    [SerializeField] private TextMeshProUGUI coinsTxt;

    [SerializeField] private string androidAdID = "Rewarded_Android";
    [SerializeField] private string iOSAdID = "Rewarded_iOS";

    private string adID;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private Dictionary<string, object> userValues = new Dictionary<string, object>();

    private void Awake()
    {
        adID = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? iOSAdID
            : androidAdID;

        buttonShowAd.interactable = false;
        buttonShowAdTxt.text = "Авторизуйтесь";
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }

    private void Start()
    {
        if (Advertisement.isInitialized)
        {
            LoadAd();
        }
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + adID);
        Advertisement.Load(adID, this);
    }

    public void ShowAd()
    {
        buttonShowAd.interactable = false;

        Advertisement.Show(adID, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Ad Loaded: " + placementId);

        if (placementId.Equals(adID))
        {
            isLoaded = true;
            buttonShowAd.onClick.AddListener(ShowAd);  
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Ads Show Start");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(adID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            buttonShowAd.interactable = false;
            buttonShowAdTxt.text = "Пока\nнедоступна";
            isLoaded = false;
            StartCoroutine(AddAdsCoins());
        }
    }

    private void OnDestroy()
    {
        buttonShowAd.onClick.RemoveAllListeners();
    }

    private IEnumerator AddAdsCoins()
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
                userValues["coins"] = 1000;
                databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
                coinsTxt.text = "1000";
            }
            else
            {
                long coins = (long)task.Result.Value;
                Debug.Log("User " + userId + " coins: " + coins);
                userValues["coins"] = coins + 1000;
                databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
                coinsTxt.text = $"{coins + 1000}";
            }
        }
        else
        {
            coinsTxt.text = "Авторизуйтесь";
        }
    }
}