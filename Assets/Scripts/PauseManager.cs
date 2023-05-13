using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gasButton;
    [SerializeField] private GameObject brakeButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject uiDistance;
    [SerializeField] private GameObject uiSpeed;
    [SerializeField] private GameObject coinsPanel;

    private bool isPaused = false;
    private int controlType;

    private void Start()
    {
        controlType = PlayerPrefs.GetInt("ControlType");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pausePanel.SetActive(isPaused);
        gasButton.SetActive(!isPaused);
        brakeButton.SetActive(!isPaused);
        uiDistance.SetActive(!isPaused);
        uiSpeed.SetActive(!isPaused);
        coinsPanel.SetActive(!isPaused);
        if (controlType == 0)
        {
            rightButton.SetActive(!isPaused);
            leftButton.SetActive(!isPaused);
        }    
    }
}