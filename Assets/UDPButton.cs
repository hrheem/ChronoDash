using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System;

public class UDPButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private bool automationEnabled = true;
    private int udpValue = 1; // Initial value to be sent via UDP

    void Start()
    {
        // Set initial button label
        UpdateButtonLabel();
    }

    public void OnButtonPressed()
    {
        Debug.Log("Button Clicked");
        // Toggle automation state
        automationEnabled = !automationEnabled;

        // Send UDP packet
        SendUDP(udpValue);

        // Update button label
        UpdateButtonLabel();

        // Debug which integer is being sent out
        Debug.Log("UDP value sent: " + udpValue);
    }

    private void UpdateButtonLabel()
    {
        // Update button label based on automation state
        if (automationEnabled)
        {
            buttonText.text = "Automation On";
            udpValue = 1; // Set UDP value to 0. When a button press leads to its label showing "Automation On", this means the driver pressed the button to turned it off so 0 should be sent  
        }
        else
        {
            buttonText.text = "Automation Off";
            udpValue = 0; // Set UDP value to 1. Same here. Seeing the "Off" message means participant pressed the button to turn it on. 
        }
    }

    
    private void SendUDP(int value)
    {
        string ipAddress = "128.104.190.248"; // Replace with your destination IP address
        int port = 1211; // Replace with your destination port number
        try
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(ipAddress, port);

            // Convert integer value to byte array in little-endian format
            byte[] data = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian) // Reverse the byte array if the system is big-endian
                Array.Reverse(data);

            udpClient.Send(data, data.Length);

            udpClient.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending UDP packet: " + e.Message);
        }
    }
}
