using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> vehicleMenuPrefabs;
    [Header("Buttons")]
    [SerializeField] private Button selectLvlBtn;
    [SerializeField] private Button selectCarMenuBtn;
    [SerializeField] private Button backToMenuBtn;
    [SerializeField] private Button nextCarBtn;
    [SerializeField] private Button prevCarBtn;
    [SerializeField] private Button selectCarBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button signInBtn;
    [SerializeField] private Button buttonsControlBtn;
    [SerializeField] private Button accelerometerControlBtn;
    [SerializeField] private Button leaderBoardBtn;
    [SerializeField] private Button shopBtn;
    [SerializeField] private Button buttonShowAd;
    [Header("Panels")]
    [SerializeField] private GameObject lvlsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject profileScreen;
    [SerializeField] private GameObject leaderBoardPanel;
    [SerializeField] private GameObject coinsPanel;
    [SerializeField] private GameObject shopPanel;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI buttonShowAdTxt;
    [SerializeField] private TextMeshProUGUI leaderBoardText;
    [SerializeField] private TextMeshProUGUI highscoreOneTxt;
    [SerializeField] private TextMeshProUGUI highscoreTwoTxt;
    [SerializeField] private TextMeshProUGUI maxSpeedInfo;
    [SerializeField] private TextMeshProUGUI coinsTxt;


    private GameObject playerMenu;
    private GameObject carToSelect;
    private CarData carToSelectData;
    private bool moveToSelectCar = false;
    private bool moveBackToMenu = false;
    private bool isBuyingCar = false;
    private int currentCarIndex;
    private static int selectedCarIndex;
    private int coinsCount;

    private Camera mainCamera;
    private Vector3 positionToMoveSelectCar = new Vector3(41, 7, 37);
    private Quaternion rotationToMoveSelectCar = Quaternion.Euler(5, 90, 0);
    private Vector3 positionToMoveMenu = new Vector3(45, 5, 13);
    private Quaternion rotationToMoveMenu = Quaternion.Euler(-2, -65, 0);
    private Vector3 positionToSpawnPlayerMenu = new Vector3(-11, 0.2f, 44);
    private Quaternion rotationToSpawnPlayerMenu = Quaternion.Euler(0, 105, 0);
    private Vector3 positionToSpawnCarSelect = new Vector3(65, 0, 38);
    private Quaternion rotationToSpawnCarSelect = Quaternion.Euler(0, -120, 0);
    private float tempTime = 0;
    private int duration = 3;

    private DatabaseReference databaseReference;
    private FirebaseAuth auth;

    private void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    private void Start()
    {
        selectedCarIndex = GetSelectedCarIndex();
        mainCamera = Camera.main;
        playerMenu = Instantiate(vehicleMenuPrefabs[selectedCarIndex], positionToSpawnPlayerMenu, rotationToSpawnPlayerMenu);
        playerMenu.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        playerMenu.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        backToMenuBtn.gameObject.SetActive(false);
        nextCarBtn.gameObject.SetActive(false);
        prevCarBtn.gameObject.SetActive(false);
        selectCarBtn.gameObject.SetActive(false);
        settingsPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        lvlsPanel.SetActive(false);
        maxSpeedInfo.gameObject.SetActive(false);
        shopPanel.SetActive(false);
        if (auth.CurrentUser == null)
        {
            coinsTxt.text = "Авторизуйтесь";
        }
    }

    private void Update()
    {
        if(moveToSelectCar)
        {
            GoToSelectCar();
        }
        if(moveBackToMenu)
        {
            GoBackToMenu();
        }
    }

    private void GoToSelectCar()
    {   
        tempTime += Time.deltaTime;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, positionToMoveSelectCar, Time.deltaTime * 2);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, rotationToMoveSelectCar, Time.deltaTime * 2);
        if (tempTime > duration)
        {
            tempTime = 0;
            moveToSelectCar = false;
            backToMenuBtn.interactable = true;
            prevCarBtn.interactable = currentCarIndex != 0;
            nextCarBtn.interactable = currentCarIndex != vehicleMenuPrefabs.Count - 1;
            selectCarBtn.interactable = currentCarIndex != selectedCarIndex;
            Destroy(playerMenu);
        }
    }

    private void GoBackToMenu()
    {
        tempTime += Time.deltaTime;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, positionToMoveMenu, Time.deltaTime * 2);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, rotationToMoveMenu, Time.deltaTime * 2);
        if (tempTime > duration)
        {
            tempTime = 0;
            moveBackToMenu = false;
            selectLvlBtn.interactable = true;
            selectCarMenuBtn.interactable = true;
            settingsBtn.interactable = true;
            leaderBoardBtn.interactable = true;
            shopBtn.interactable = true;
            if (auth.CurrentUser != null)
            {
                profileScreen.GetComponentInChildren<Button>().interactable = true;
            }
            else
            {
                signInBtn.interactable = true;
            }
            Destroy(carToSelect);
        }
    }

    public void ClickSelectLevel()
    {
        ChangeMainMenuObjects(false);
        coinsPanel.SetActive(false);
        lvlsPanel.SetActive(true);
        backToMenuBtn.gameObject.SetActive(true);
        if (auth.CurrentUser != null)
        {
            StartCoroutine(GetHighScores());
        }
        else
        {
            highscoreOneTxt.text = "Лучшая дистанция: \n Авторизуйтесь";
            highscoreTwoTxt.text = "Лучшая дистанция: \n Авторизуйтесь";
        }
    }

    public void ClickOneSideLevel()
    {
        PlayerPrefs.SetInt("LevelType", 0);
        ScenesManager.LoadMainLevelScene();
    }

    public void ClickTwoSideLevel()
    {
        PlayerPrefs.SetInt("LevelType", 1);
        ScenesManager.LoadMainLevelScene();
    }

    public void ClickSettings()
    {
        if (PlayerPrefs.GetInt("ControlType") == 0)
        {
            buttonsControlBtn.interactable = false;
        }
        else
        {
            accelerometerControlBtn.interactable = false;
        }
        ChangeMainMenuObjects(false);
        coinsPanel.SetActive(false);
        settingsPanel.SetActive(true);
        backToMenuBtn.gameObject.SetActive(true);
    }

    public void ClickLeaderBoard()
    {
        ChangeMainMenuObjects(false);
        coinsPanel.SetActive(false);
        leaderBoardPanel.SetActive(true);
        backToMenuBtn.gameObject.SetActive(true);
    }

    public void ClickShop()
    {
        ChangeMainMenuObjects(false);
        coinsPanel.SetActive(false);
        if (auth.CurrentUser == null)
        {
            buttonShowAd.interactable = false;
            buttonShowAdTxt.text = "Авторизуйтесь";
        }
        else if (!RewardedAds.isLoaded)
        {
            buttonShowAd.interactable = false;
            buttonShowAdTxt.text = "Пока\nнедоступна";
            
        }
        else
        {
            buttonShowAd.interactable = true;
            buttonShowAdTxt.text = "+1000 монет\nПосмотреть\nрекламу";
        }
        shopPanel.SetActive(true);
        backToMenuBtn.gameObject.SetActive(true);
    }

    public void ClickOneLeaders()
    {
        leaderBoardText.text = "";
        if (auth.CurrentUser != null)
        {
            StartCoroutine(GetLeaders("highscoreOne"));
        }
        else
        {
            leaderBoardText.text = "Авторизуйтесь";
        }
    }

    public void ClickTwoLeaders()
    {
        leaderBoardText.text = "";
        if (auth.CurrentUser != null)
        {
            StartCoroutine(GetLeaders("highscoreTwo"));
        }
        else
        {
            leaderBoardText.text = "Авторизуйтесь";
        }
    }

    public void ClickButtonsControl()
    {
        PlayerPrefs.SetInt("ControlType", 0);
        buttonsControlBtn.interactable = false;
        accelerometerControlBtn.interactable = true;
    }

    public void ClickAccelerometerControl()
    {
        PlayerPrefs.SetInt("ControlType", 1);
        accelerometerControlBtn.interactable = false;
        buttonsControlBtn.interactable = true;
    }

    public void ClickSelectMenuCar()
    {
        moveToSelectCar = true;
        if (auth.CurrentUser != null)
        {
            coinsCount = int.Parse(coinsTxt.text);
        }  
        ChangeMainMenuObjects(false);
        ChangeSelectCarMenuObjects(true);
        selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Выбрана";
        currentCarIndex = selectedCarIndex;
        carToSelect = Instantiate(vehicleMenuPrefabs[currentCarIndex], positionToSpawnCarSelect, rotationToSpawnCarSelect);
        carToSelect.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        carToSelectData = carToSelect.GetComponent<CarData>();
        maxSpeedInfo.text = $"МАКСИМАЛЬНАЯ\nСКОРОСТЬ\n{carToSelectData.MaxCarVerticalSpeed * 3}";
        carToSelect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void ClickBackToMenu()
    {
        if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            backToMenuBtn.gameObject.SetActive(false);
        }
        else if (leaderBoardPanel.activeSelf)
        {
            leaderBoardPanel.SetActive(false);
            leaderBoardText.text = " ";
            backToMenuBtn.gameObject.SetActive(false);
        }
        else if (lvlsPanel.activeSelf)
        {
            lvlsPanel.SetActive(false);
            backToMenuBtn.gameObject.SetActive(false);    
        }
        else if (shopPanel.activeSelf)
        {
            shopPanel.SetActive(false);
            backToMenuBtn.gameObject.SetActive(false);
        }
        else
        {
            moveBackToMenu = true;
            currentCarIndex = selectedCarIndex;
            ChangeSelectCarMenuObjects(false);    
            playerMenu = Instantiate(vehicleMenuPrefabs[currentCarIndex], positionToSpawnPlayerMenu, rotationToSpawnPlayerMenu);
            playerMenu.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            playerMenu.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
        ChangeMainMenuObjects(true);
        coinsPanel.SetActive(true);
    }

    public void ClickNextCar()
    {
        Destroy(carToSelect);
        currentCarIndex++;
        carToSelect = Instantiate(vehicleMenuPrefabs[currentCarIndex], positionToSpawnCarSelect, rotationToSpawnCarSelect);
        carToSelect.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        carToSelectData = carToSelect.GetComponent<CarData>();
        maxSpeedInfo.text = $"МАКСИМАЛЬНАЯ\nСКОРОСТЬ\n{carToSelectData.MaxCarVerticalSpeed * 3}";
        carToSelect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        nextCarBtn.interactable = currentCarIndex != vehicleMenuPrefabs.Count - 1;
        prevCarBtn.interactable = currentCarIndex != 0;
        if (auth.CurrentUser != null || currentCarIndex == 0)
        {
            if (carToSelectData.IsBought)
            {
                selectCarBtn.interactable = currentCarIndex != selectedCarIndex;
                selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = selectCarBtn.interactable ? "Выбрать" : "Выбрана";
            }
            else
            {
                selectCarBtn.interactable = coinsCount >= carToSelectData.Cost;
                selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"Цена {carToSelectData.Cost}";
            }
        }
        else
        {
            selectCarBtn.interactable = false;
            selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"Авторизуйтесь";
        }
    }

    public void ClickPrevCar()
    {
        Destroy(carToSelect);
        currentCarIndex--;
        carToSelect = Instantiate(vehicleMenuPrefabs[currentCarIndex], positionToSpawnCarSelect, rotationToSpawnCarSelect);
        carToSelect.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        carToSelectData = carToSelect.GetComponent<CarData>();
        maxSpeedInfo.text = $"МАКСИМАЛЬНАЯ\nСКОРОСТЬ\n{carToSelectData.MaxCarVerticalSpeed * 3}";
        carToSelect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        prevCarBtn.interactable = currentCarIndex != 0;
        nextCarBtn.interactable = currentCarIndex != vehicleMenuPrefabs.Count - 1;
        if (auth.CurrentUser != null || currentCarIndex == 0)
        {
            if (carToSelectData.IsBought)
            {
                selectCarBtn.interactable = currentCarIndex != selectedCarIndex;
                selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = selectCarBtn.interactable ? "Выбрать" : "Выбрана";
            }
            else
            {
                selectCarBtn.interactable = coinsCount >= carToSelectData.Cost;
                selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"Цена {carToSelectData.Cost}";
            }
        }
        else
        {
            selectCarBtn.interactable = false;
            selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"Авторизуйтесь";
        }
    }

    public void ClickSelectCar()
    {
        if (carToSelectData.IsBought)
        {
            selectedCarIndex = currentCarIndex;
            selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Выбрана";
            selectCarBtn.interactable = false;
        }
        else if (!isBuyingCar)
        {
            isBuyingCar = true;
            StartCoroutine(BuyCar(carToSelectData, currentCarIndex));
        }
    }

    private void ChangeMainMenuObjects(bool changeStatus)
    {
        if (moveBackToMenu)
        {
            selectLvlBtn.interactable = false;
            selectCarMenuBtn.interactable = false;
            settingsBtn.interactable = false;
            leaderBoardBtn.interactable = false;
            shopBtn.interactable = false;
        }
        if (auth.CurrentUser != null)
        {
            profileScreen.SetActive(changeStatus);
            if (moveBackToMenu)
            {
                profileScreen.GetComponentInChildren<Button>().interactable = false;
            }
        }
        else
        {
            signInBtn.gameObject.SetActive(changeStatus);
            if (moveBackToMenu)
            {
                signInBtn.interactable = false;
            }
        }
        selectLvlBtn.gameObject.SetActive(changeStatus);
        selectCarMenuBtn.gameObject.SetActive(changeStatus);
        settingsBtn.gameObject.SetActive(changeStatus);
        leaderBoardBtn.gameObject.SetActive(changeStatus);
        shopBtn.gameObject.SetActive(changeStatus);
    }

    private void ChangeSelectCarMenuObjects(bool changeStatus)
    {
        if (moveToSelectCar)
        {
            backToMenuBtn.interactable = false;
            nextCarBtn.interactable = false;
            prevCarBtn.interactable = false;
            selectCarBtn.interactable = false;
        }
        backToMenuBtn.gameObject.SetActive(changeStatus);
        nextCarBtn.gameObject.SetActive(changeStatus);
        prevCarBtn.gameObject.SetActive(changeStatus);
        selectCarBtn.gameObject.SetActive(changeStatus);
        maxSpeedInfo.gameObject.SetActive(changeStatus);
    }

    private IEnumerator GetLeaders(string highscoreType)
    {
        var leaders = databaseReference.Child("users").OrderByChild(highscoreType).GetValueAsync();
        yield return new WaitUntil(predicate: () => leaders.IsCompleted);
        if (leaders.Exception != null)
        {
            Debug.LogError("ERROR: " + leaders.Exception);
        }
        else if(leaders.Result.Value == null)
        {
            Debug.LogError("Result.Value == null");
        }
        else
        {
            DataSnapshot snapshot = leaders.Result;
            int num = 1;
            leaderBoardText.text = "";
            foreach (DataSnapshot dataChildSnapshot in snapshot.Children.Reverse())
            {
                if (dataChildSnapshot.Child(highscoreType).Value != null)
                {
                    leaderBoardText.text += "\n" + num + ") " + dataChildSnapshot.Child("username").Value.ToString() + " : " + dataChildSnapshot.Child(highscoreType).Value.ToString();
                    num++;
                }
            }
        }
    }

    private IEnumerator GetHighScores()
    {
        string userId = auth.CurrentUser.UserId;
        var highscore = databaseReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => highscore.IsCompleted);
        if (highscore.Exception != null)
        {
            Debug.LogError("ERROR: " + highscore.Exception);
        }
        else if (highscore.Result.Value == null)
        {
            Debug.LogError("Result.Value == null");
        }
        else
        {
            DataSnapshot snapshot = highscore.Result;
            if (snapshot.Child("highscoreOne").Value != null)
            {
                highscoreOneTxt.text = $"Лучшая дистанция:\n{snapshot.Child("highscoreOne").Value.ToString()}";
            }
            if (snapshot.Child("highscoreTwo").Value != null)
            {
                highscoreTwoTxt.text = $"Лучшая дистанция:\n{snapshot.Child("highscoreTwo").Value.ToString()}";
            }
        }
    }

    private IEnumerator BuyCar(CarData carData, int indexInPrefabs)
    {
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId;
            var task = databaseReference.Child("users").Child(userId).Child("coins").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.LogError("Failed to buy car: " + task.Exception);
                yield break;
            }
            long coins = (long)task.Result.Value;
            Dictionary<string, object> userValues = new Dictionary<string, object>();
            Debug.Log("User " + userId + " coins: " + coins);
            vehicleMenuPrefabs[indexInPrefabs].GetComponent<CarData>().IsBought = true;
            userValues["coins"] = coins - carData.Cost;
            coinsCount -= carData.Cost;
            databaseReference.Child("users").Child(userId).Child("cars").Child($"{carToSelectData.CarName}").Child("isBought").SetValueAsync(true);
            databaseReference.Child("users").Child(userId).UpdateChildrenAsync(userValues);
            coinsTxt.text = $"{coinsCount}";
            selectedCarIndex = currentCarIndex;
            selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Выбрана";
            selectCarBtn.interactable = false;
        }
        else
        {
            selectCarBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Авторизуйтесь";
        }
        isBuyingCar = false;
    }

    public List<GameObject> GetCarPrefabsList()
    {
        return vehicleMenuPrefabs;
    }

    public static int GetSelectedCarIndex()
    {
        return selectedCarIndex;
    }

    public void ClearIsBoughtInfo()
    {
        foreach (GameObject vehiclePrefab in vehicleMenuPrefabs)
        {
            CarData carData = vehiclePrefab.GetComponent<CarData>();
            if (carData.CarName != "Pickup")
            {
                carData.IsBought = false;
            }
        }
        Destroy(playerMenu);
        currentCarIndex = 0;
        selectedCarIndex = 0;
        playerMenu = Instantiate(vehicleMenuPrefabs[currentCarIndex], positionToSpawnPlayerMenu, rotationToSpawnPlayerMenu);
        playerMenu.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        playerMenu.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}