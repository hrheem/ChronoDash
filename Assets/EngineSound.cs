using UnityEngine;
using System;

public class EngineSound : MonoBehaviour
{
    public UDPBaseDataListener udpSource;
    public float sensorVal;
    public bool isAbsoluteValue = true;
    public int dataPointIndex = 2; // Change this to 2 to get engine RPM data

    // Add AudioSource field for sound control
    public AudioSource engineSound;
    // Optional: Define a range for pitch adjustment based on RPM
    public float minRPM = 600; // Minimum RPM value for the sound
    public float maxRPM = 8000; // Maximum RPM value for the sound

    void Start()
    {
    }

    void Update()
    {
        // Get the engine RPM from udpSource
        sensorVal = udpSource.valArray[dataPointIndex];

        // Update sound based on RPM
        // Normalize RPM value between 0 and 1 for pitch adjustment
        float rpmNormalized = Mathf.InverseLerp(minRPM, maxRPM, sensorVal);

        // Adjust pitch based on normalized RPM
        engineSound.pitch = Mathf.Lerp(0.5f, 2.0f, rpmNormalized);
    }
}
