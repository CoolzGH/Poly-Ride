using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour
{
    [SerializeField] private string carName;
    [SerializeField] private float carHorizontalSpeed;
    [SerializeField] private float carVerticalAcceleration;
    [SerializeField] private float maxCarVerticalSpeed;
    [SerializeField] private float minCarVerticalSpeed;
    [SerializeField] private float minCarBrakingSpeed;
    [SerializeField] private float brakeForce;
    [SerializeField] private float brakeFriction;
    [SerializeField] private int cost;
    [SerializeField] private bool isBought;

    public string CarName
    {
        get
        {
            return carName;
        }
    }
    public float CarHorizontalSpeed
    {
        get
        {
            return carHorizontalSpeed;
        } 
    }
    public float CarVerticalAcceleration
    {
        get
        {
            return carVerticalAcceleration;
        }
    }
    public float MaxCarVerticalSpeed
    {
        get
        {
            return maxCarVerticalSpeed;
        } 
    }
    public float MinCarVerticalSpeed
    {
        get
        {
            return minCarVerticalSpeed;
        }
    }
    public float MinCarBrakingSpeed
    {
        get
        {
            return minCarBrakingSpeed;
        }
    }
    public float BrakeForce
    {
        get
        {
            return brakeForce;
        } 
    }
    public float BrakeFriction
    {
        get
        {
            return brakeFriction;
        }
    }
    public int Cost
    {
        get
        {
            return cost;
        }
    }
    public bool IsBought
    {
        get
        {
            return isBought;
        }
        set
        {
            isBought = value;
        }
    }
}