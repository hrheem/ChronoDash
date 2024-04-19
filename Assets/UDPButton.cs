using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System;
using System.Collections;

 
public class UDPButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private bool automationEnabled = false;  // State to control automation
    private float[] udpValue = new float[1]{0.0f};  // Initial float value to be sent via UDP
    private UdpClient udpClient;  // UDP client for sending data
    private const int sendFrequency = 10000;  // Send data at 200 Hz
 
    void Start()
    {
        // Initialize UDP client
        udpClient = new UdpClient();
        udpClient.Connect("128.104.190.248", 1210);  // Replace with your destination IP and port
 
        // Set initial button label and start the coroutine
        UpdateButtonLabel();
        StartCoroutine(SendDataContinuously());
    }
 
    public void OnButtonPressed()
    {
        // Toggle automation state
        automationEnabled = !automationEnabled;
 
        // Update button label
        UpdateButtonLabel();
    }
 
    private void UpdateButtonLabel()
    {
        if (automationEnabled)
        {
            buttonText.text = "Deactivate";
            udpValue[0] = 1.0f;
        }
        else
        {
            buttonText.text = "Activate";
            udpValue[0] = 0.0f;
        }
    }
 
    private IEnumerator SendDataContinuously()
    {
        while (true)  // Always true to keep the coroutine running
        {
            SendUDP(udpValue);
            yield return new WaitForSeconds(1f / sendFrequency);
        }
    }
 
    private void SendUDP(float[] values)
    {
        try
        {
            foreach (float value in values)
            {
                byte[] data = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(data);
                }
                udpClient.Send(data, data.Length);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending UDP packet: " + e.Message);
        }
    }
 
    void OnDestroy()
    {
        // Clean up the UDP client when the script is destroyed
        udpClient?.Close();
    }
}
