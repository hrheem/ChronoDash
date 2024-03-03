using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
//this is the FoW script


public class UDPBaseDataListener : MonoBehaviour
{

    public int listenAtPort = 1209;//1209 csl, tony
    string logFilePath = "";
    //string logFilePath1 = "";
    //string logFilePath2 = "";
    //string logFilePathD = "";
    public bool listenForData = true;
    public bool listeningForChrono = false;

    Thread receiveThread;
    UdpClient udpReceiverClient;

    // Data from Remote Simulator
    private SimulatorMessage currMsgObj = null;
    private bool receivingData = false;

    // For Dashboard
    public float[] valArray;

    // Start is called before the first frame update
    void Start()
    {
        logFilePath = Application.persistentDataPath + "/" + "FoW_FinalData" + System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + ".txt";
        
        //string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
      
        string logEntry = "date" + " time" + " simTime" + " speed" + " rpm" + " egoX" + " egoY" + " throttle" + " brake" + " steer" + " lead1X" + " lead1Y" + " lead1Xvelo" + " lead1Yvelo"+ " lead1Throttle" + " lead1Brake" + " lead1Steer";//+ " lead1Yvelo" + " lead1throttle" + " lead1break" + " lead1steer" + " lead2X" + " lead2Y" + " lead2Xvelo" + " lead2Yvelo" + " lead2throttle" + " lead2break" + " lead2steer";
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        initializeSettings();
    }

    public void initializeSettings()
    {
        valArray = new float[15]; 
        startUdpThread();

    }

    public void startUdpThread()
    {

        receiveThread = new Thread(new ThreadStart(receiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    private void receiveData()
    {

        IPEndPoint controlPoint = new IPEndPoint(IPAddress.Any, listenAtPort);
        udpReceiverClient = new UdpClient(controlPoint);

        Debug.Log("UDP Receiver for Dashboard Initialized!");

        while (listenForData)
        {

            try
            {

                byte[] bData = udpReceiverClient.Receive(ref controlPoint);
                currMsgObj = processRawByteData(bData);
                receivingData = true;

            }
            catch (Exception err)
            {

                if (!listenForData)
                {

                    Debug.Log("Not listening for dashboard data anymore.");

                }
                else
                {

                    Debug.LogError(err.ToString());

                }

            }

        }

    }

    public SimulatorMessage processRawByteData(byte[] bData)
    {

        IPEndPoint controlPoint = new IPEndPoint(IPAddress.Any, listenAtPort);
        udpReceiverClient = new UdpClient(controlPoint);
        bData = udpReceiverClient.Receive(ref controlPoint);
        SimulatorMessage toReturn = new SimulatorMessage(bData, listenForData);
        //SimulatorMessage toReturn = new SimulatorMessage(bData, listeningForChrono);
	//https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter.todouble?view=net-8.0
        // Initialize an array to store the unpacked floats
        float[] floats = new float[bData.Length / sizeof(float)];
        // Iterate over the byte array and unpack floats
        for (int i = 0; i < floats.Length; i++)
        {
         // Extract 4 bytes representing a float from the byte array
            byte[] floatBytes = new byte[sizeof(float)];
            System.Array.Copy(bData, i * sizeof(float), floatBytes, 0, sizeof(float));

            // Convert the byte array to a float
            floats[i] = System.BitConverter.ToSingle(floatBytes, 0);
        }

	Debug.Log(floats.Length);
               
        valArray[0] = floats[0];   // sim time
        valArray[1] = floats[1];   // vehicle speed
        valArray[2] = floats[2];   // vehicle RPM
        valArray[3] = floats[3];   // vehicle xpos
        valArray[4] = floats[4];   // vehicle ypos
        valArray[5] = floats[5];   // throttle
        valArray[6] = floats[6];   // brake
        valArray[7] = floats[7];   // steering
        valArray[8] = floats[8];   // lead1 x
        valArray[9] = floats[9];   // lead1 y
        valArray[10] = floats[10];   // lead1 x velo
        valArray[11] = floats[11];   // lead1 y velo
        valArray[12] = floats[12];   // lead1 throttle
        valArray[13] = floats[13];   // lead1 brake
        valArray[14] = floats[14];   // lead1 steering
	
        string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logEntry = timestamp + " " + valArray[0] + " " + valArray[1] + " " + valArray[2] + " " + valArray[3] + " " + valArray[4] + " " + valArray[5] + " " + valArray[6] + " " + valArray[7] + " " + valArray[8] + " " + valArray[9] + " " + valArray[10]+ " " + valArray[11] + " " + valArray[12] + " " + valArray[13] + " " + valArray[14];//+ " " + valArray[11] + " " + valArray[12] + " " + valArray[13] + " " + valArray[14] + " " + valArray[15] + " " + valArray[16] + " " + valArray[17] + " " + valArray[18] + " " + valArray[19] + " " + valArray[20] + " " + valArray[21];
      
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        return toReturn;

    }

    void OnApplicationQuit()
    {

        try
        {
            listenForData = false;
            udpReceiverClient.Close();

        }
        catch (Exception err)
        {

            Debug.Log("Dashboard is closing.");
            Debug.LogError(err.ToString());

        }

    }

}

