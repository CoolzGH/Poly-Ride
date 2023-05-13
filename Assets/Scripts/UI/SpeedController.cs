using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI uiSpeed;

    private GameObject player;
    private Rigidbody playerRigidbody;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        int speed = Mathf.RoundToInt(playerRigidbody.velocity.z) * 3;
        uiSpeed.text = speed.ToString() + " km/h";
    }
}