using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.UI;
using Google;
using TMPro;
using UnityEngine.Networking;

public class FireBaseGoogleLogin : MonoBehaviour
{
    public string GoogleWebAPI = "921366014530-q4m5ojr80gv45l3hmeki7m2qvuhspal5.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    private Dictionary<string, bool> carsInfo = new Dictionary<string, bool>();
    private List<GameObject> carPrefabs;

    [SerializeField] private TextMeshProUGUI usernameTxt;
    [SerializeField] private Image userProfilePic;
    [SerializeField] private string imageUrl;
    [SerializeField] private GameObject signInBtn;
    [SerializeField] private GameObject profileScreen;
    [SerializeField] private TextMeshProUGUI coinsTxt;
    [SerializeField] private MainMenu mainMenuScript;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        InitFirebase();
        carPrefabs = mainMenuScript.GetCarPrefabsList();
        if (auth.CurrentUser != null)
        {
            OnSignInSilently();
        }
    }

    private void InitFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void GoogleSignInClick()
    {
        if (GoogleSignIn.Configuration == null)
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
        }
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    public void GoogleSignOutClick()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
        signInBtn.SetActive(true);
        profileScreen.SetActive(false);
        coinsTxt.text = "Авторизуйтесь";
        carsInfo.Clear();
        mainMenuScript.ClearIsBoughtInfo();
        GameOver.isAdsOff = false;
    }

    private void OnSignInSilently()
    {
        try
        {
            if (GoogleSignIn.Configuration == null)
            {
                GoogleSignIn.Configuration = configuration;
                GoogleSignIn.Configuration.UseGameSignIn = false;
                GoogleSignIn.Configuration.RequestIdToken = true;
                GoogleSignIn.Configuration.RequestEmail = true;
            }
            
            GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnGoogleAuthenticatedFinished);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
            GoogleSignOutClick();
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    GoogleSignOutClick();
                    return;
                }
                user = auth.CurrentUser;

                usernameTxt.text = user.DisplayName;
                signInBtn.SetActive(false);
                profileScreen.SetActive(true);

                StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));

                SaveUserDataToDatabase();

                StartCoroutine(GetCoins());

                StartCoroutine(GetUserCarsInfo());

                StartCoroutine(GetUserAdsInfo());
            });
        }
    }

    private string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }
        return imageUrl;
    }

    private IEnumerator LoadImage(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();
        Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
        userProfilePic.sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
    }

    private void SaveUserDataToDatabase()
    {
        string userId = user.UserId;
        string email = user.Email;
        string username = user.DisplayName;
        string imageUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : "";

        Dictionary<string, object> userValues = new Dictionary<string, object>();
        userValues["email"] = email;
        userValues["username"] = username;
        userValues["image_url"] = imageUrl;

        databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
    }

    private IEnumerator GetCoins()
    {
        string userId = auth.CurrentUser.UserId;
        var task = databaseReference.Child("users").Child(userId).Child("coins").GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsFaulted)
        {
            Debug.LogError("Failed to get user coins: " + task.Exception);
        }
        if (task.Result.Value == null)
        {
            coinsTxt.text = "0";
        }
        else
        {
            long coins = (long)task.Result.Value;
            Debug.Log("User " + userId + " coins: " + coins);
            coinsTxt.text = $"{coins}";
        }
    }

    private IEnumerator GetUserAdsInfo()
    {
        string userId = auth.CurrentUser.UserId;
        var task = databaseReference.Child("users").Child(userId).Child("isAdsOff").GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.IsFaulted)
        {
            Debug.LogError("Failed to get user coins: " + task.Exception);
        }
        if (task.Result.Value == null)
        {
            databaseReference.Child("users").Child(userId).Child("isAdsOff").SetValueAsync(false);
        }
        else
        {
            GameOver.isAdsOff = (bool)task.Result.Value;
        }
    }

    private IEnumerator GetUserCarsInfo()
    {
        string userId = auth.CurrentUser.UserId;
        var task = databaseReference.Child("users").Child(userId).Child("cars").GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogError("ERROR: " + task.Exception);
        }
        else if (task.Result.Value == null)
        {
            Debug.LogError("task.Result.Value == null");
            foreach (GameObject carPrefab in carPrefabs)
            {
                CarData carPrefabData = carPrefab.GetComponent<CarData>();
                databaseReference.Child("users").Child(userId).Child("cars").Child($"{carPrefabData.CarName}").Child("isBought").SetValueAsync(carPrefabData.IsBought);
            }
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            var userCarsInfo = snapshot.Children;
            Debug.Log(userCarsInfo);
            foreach (DataSnapshot dataChildSnapshot in userCarsInfo)
            {
                carsInfo[$"{dataChildSnapshot.Key}"] = (bool)dataChildSnapshot.Child("isBought").Value;
            }
            foreach (GameObject carPrefab in carPrefabs)
            {
                CarData carPrefabData = carPrefab.GetComponent<CarData>();
                carPrefabData.IsBought = carsInfo[$"{carPrefabData.CarName}"];
            }
        }
    }
}