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
    public bool listenForData = true;
    private List<Vector2[]> trackDataList = new List<Vector2[]>();     // Array to store loaded track data

    Thread receiveThread;
    UdpClient udpReceiverClient;

    // Data from Remote Simulator
    private SimulatorMessage currMsgObj = null;
    //private bool receivingData = false;

    // For Dashboard
    public float[] valArray;

    // Start is called before the first frame update
    void Start()
    {
        logFilePath = Application.persistentDataPath + "/" + "FoW_FinalData" + System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + ".txt";
        //string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        //!!CHANGE WITH THE NUMBER OF LEAD VEHICLES
        string logEntry = "date" + " time" + " simTime" + " speed" + " rpm" + " egoX" + " egoY" + " throttle" + " brake" + " steer" + " laneDev" +" lead1X" + " lead1Y" + " lead1Xvelo" + " lead1Yvelo"+ " lead1Throttle" + " lead1Brake" + " lead1Steer" + " lead2X" + " lead2Y" + " lead2Xvelo" + " lead2Yvelo"+ " lead2Throttle" + " lead2Brake" + " lead2Steer"  + " LaneCenterX"  + " LaneCenterY";
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        LoadTrackData();
        initializeSettings();
    }

    public void initializeSettings()
    {
        valArray = new float[25]; 
        startUdpThread();

    }

    public void LoadTrackData()
    {
        string directoryPath = Application.dataPath + "/Tracks/";
        int trackNumber = 1;

        // Debug log to indicate the start of track data loading
        Debug.Log("Loading track data...");

        while (true)
        {
            string filePath = directoryPath + "track" + trackNumber + ".txt";

            if (File.Exists(filePath))
            {
                // Read the text file
                string[] lines = File.ReadAllLines(filePath);

                // Check if the file contains any lines
                if (lines.Length == 0)
                {
                    Debug.LogError("Track file " + filePath + " is empty.");
                    trackNumber++;
                    continue;
                }

                // Convert lines to Vector2 array
                List<Vector2> trackData = new List<Vector2>();
                foreach (string line in lines)
                {
                    string[] coordinates = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                    // Check if the line contains at least two coordinates
                    if (coordinates.Length < 2)
                    {
                        Debug.LogError("Insufficient number of coordinates in line of track " + trackNumber + ": " + line);
                        continue; // Skip this line
                    }

                    // Attempt to parse coordinates
                    if (float.TryParse(coordinates[0], out float x) && float.TryParse(coordinates[1], out float y))
                    {
                        trackData.Add(new Vector2(x, y));
                    }
                    else
                    {
                        Debug.LogError("Error parsing coordinates in line of track " + trackNumber + ": " + line);
                    }
                }

                // Add track data to the list
                trackDataList.Add(trackData.ToArray());

                // Debug log to indicate successful loading of track data
                Debug.Log("Track " + trackNumber + " loaded. Number of points: " + trackData.Count);

                trackNumber++;
            }
            else
            {
                // No more track files found
                break;
            }
        }

        // Debug log to indicate the end of track data loading
        Debug.Log("Track data loading complete. Total tracks loaded: " + trackDataList.Count);
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
                //receivingData = true;

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
        //!!CHANGE WITH THE NUMBER OF LEAD VEHICLES
        valArray[0] = floats[0];   // sim time
        valArray[1] = floats[1];   // vehicle speed
        valArray[2] = floats[2];   // vehicle RPM
        valArray[3] = floats[3];   // vehicle xpos
        valArray[4] = floats[4];   // vehicle ypos
        valArray[5] = floats[5];   // throttle
        valArray[6] = floats[6];   // brake
        valArray[7] = floats[7];   // steering
              
        valArray[9] = floats[8];   // lead1 x
        valArray[10] = floats[9];   // lead1 y
        valArray[11] = floats[10];   // lead1 x velo
        valArray[12] = floats[11];   // lead1 y velo
        valArray[13] = floats[12];   // lead1 throttle
        valArray[14] = floats[13];   // lead1 brake
        valArray[15] = floats[14];   // lead1 steering        
        valArray[16] = floats[15];   // lead2 x
        valArray[17] = floats[16];   // lead2 y
        valArray[18] = floats[17];   // lead2 x velo
        valArray[19] = floats[18];   // lead2 y velo
        valArray[20] = floats[19];   // lead2 throttle
        valArray[21] = floats[20];   // lead2 brake
        valArray[22] = floats[21];   // lead2 steering

        // Initialize variables to store the closest point
        Vector2 closestTrackPoint = Vector2.zero;
        float minDistanceSquared = float.MaxValue; // Use squared distance for efficiency

        // Iterate through each track in trackDataList
        foreach (Vector2[] trackData in trackDataList)
        {
            // Iterate through each point in the track
            foreach (Vector2 trackPoint in trackData)
            {
                // Calculate the squared distance to the ego vehicle's location
                float dx = trackPoint.x - valArray[3]; // Difference in x coordinates
                float dy = trackPoint.y - valArray[4]; // Difference in y coordinates
                float distanceSquared = dx * dx + dy * dy; // Squared distance

                // Check if this point is closer than the current closest point
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closestTrackPoint = trackPoint;
                }
            }
        }

        // Now, closestTrackPoint contains the closest point on the track to the ego vehicle's location

        // Calculate the deviation from the lane center
        float deviationFromCenter = closestTrackPoint.x - valArray[3]; // Subtract x-coordinate of closest point from ego vehicle's x-coordinate

        // Log the deviation from the lane center
        valArray[8] = deviationFromCenter;   // LaneDeviation
        valArray[23] = closestTrackPoint.x;   // LaneDeviation
        valArray[24] = closestTrackPoint.y;   // LaneDeviation
          
        //!!CHANGE WITH THE NUMBER OF LEAD VEHICLES
        string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logEntry = timestamp + " " + valArray[0] + " " + valArray[1] + " " + valArray[2] + " " + valArray[3] + " " + valArray[4] + " " + valArray[5] + " " + valArray[6] + " " + valArray[7] + " " + valArray[8] + " " + valArray[9] + " " + valArray[10]+ " " + valArray[11] + " " + valArray[12] + " " + valArray[13] + " " + valArray[14] + " " + valArray[15] + " " + valArray[16] + " " + valArray[17] + " " + valArray[18] + " " + valArray[19] + " " + valArray[20] + " " + valArray[21] + " " + valArray[22]+ " " + valArray[23]+ " " + valArray[24];
      
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
