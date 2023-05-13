using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float carHorizontalSpeed = 10f;
    [SerializeField] private float carVerticalAcceleration = 0.1f;
    [SerializeField] private float maxCarVerticalSpeed = 40f;
    [SerializeField] private float minCarVerticalSpeed = 7f;
    [SerializeField] private float minCarBrakingSpeed = 5f;
    [SerializeField] private float brakeForce = 0.995f;
    [SerializeField] private float brakeFriction = 0.999f;

    [Header("Links")]
    public SpawnManager spawnManager;
    public GameOver gameOver;
    public CoinManager coinManager;

    [Header("UI Buttons")]
    public GameObject gasButton;
    public GameObject brakeButton;
    public GameObject rightButton;
    public GameObject leftButton;


    private Rigidbody carRigidbody;
    private TouchInput gasTI;
    private TouchInput brakeTI;
    private TouchInput rightTI;
    private TouchInput leftTI;
    private bool isBraking;
    private bool gameOverBool;
    private int horizontalAxis = 0;
    private int controlType;




    private void Start()
    {
        isBraking = false;
        gameOverBool = false;
        controlType = PlayerPrefs.GetInt("ControlType");
        if (controlType == 1)
        {
            rightButton.SetActive(false);
            leftButton.SetActive(false);
        }

        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.velocity = new Vector3(0, 0, minCarVerticalSpeed);

        gasTI = gasButton.GetComponent<TouchInput>();
        brakeTI = brakeButton.GetComponent<TouchInput>();
        rightTI = rightButton.GetComponent<TouchInput>();
        leftTI = leftButton.GetComponent<TouchInput>();
    }

    private void FixedUpdate()
    {
        if (!gameOverBool)
        {
            Brake();
            if (!isBraking)
            {
                MoveForward();
            }
            if (controlType == 0)
            {
                MoveHorizontalWithButtons();
            }
            else
            {
                MoveHorizontalWithAccelerometer();
            }
        }   
    }

    private void MoveForward()
    {
        if (gasTI.buttonPressed)
        {
            if (carRigidbody.velocity.z < maxCarVerticalSpeed)
            {
                carRigidbody.velocity += new Vector3(0, 0, carVerticalAcceleration);
            }
            else
            {
                carRigidbody.velocity = new Vector3(0, 0, maxCarVerticalSpeed);
            }
        }
        else
        {
            if (carRigidbody.velocity.z > minCarVerticalSpeed)
            {
                carRigidbody.velocity = carRigidbody.velocity * brakeFriction;
            }
            else
            {
                carRigidbody.velocity = new Vector3(0, 0, minCarVerticalSpeed);
            }
        }
    }

    private void MoveHorizontalWithButtons()
    {
        horizontalAxis = (rightTI.buttonPressed ? 1 : 0) + (leftTI.buttonPressed ? -1 : 0);
        float horizontalToMove = horizontalAxis * carHorizontalSpeed * Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + horizontalToMove, -9f, 9f), transform.position.y, transform.position.z);
    }

    private void MoveHorizontalWithAccelerometer()
    {
        Vector3 movementDirection = new Vector3(Input.acceleration.x, 0f, 0f);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + movementDirection.x * carHorizontalSpeed * Time.deltaTime * 3, -9f, 9f), transform.position.y, transform.position.z);
    }

    private void Brake()
    {
        if (brakeTI.buttonPressed)
        {
            isBraking = true;
            if (carRigidbody.velocity.z > minCarBrakingSpeed)
            {
                carRigidbody.velocity = carRigidbody.velocity * brakeForce;
            }
            else
            {
                carRigidbody.velocity = new Vector3(0, 0, minCarBrakingSpeed);
            }
        }
        else
        {
            isBraking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpawnTrigger"))
        {
            spawnManager.SpawnTriggerEnterred(transform.position.z);
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            coinManager.AddCoinToCount(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameOverBool = true;
            gameOver.DoGameOver(carRigidbody);
        }
    }

    public void InitCarData(CarData carData)
    {
        carHorizontalSpeed = carData.CarHorizontalSpeed;
        carVerticalAcceleration = carData.CarVerticalAcceleration;
        maxCarVerticalSpeed = carData.MaxCarVerticalSpeed;
        minCarVerticalSpeed = carData.MinCarVerticalSpeed;
        minCarBrakingSpeed = carData.MinCarBrakingSpeed;
        brakeForce = carData.BrakeForce;
        brakeFriction = carData.BrakeFriction;
    }
}