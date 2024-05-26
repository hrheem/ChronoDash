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

    // For Dashboard
    public float[] valArray;

    // Start is called before the first frame update
    void Start()
    {
    	IPEndPoint controlPoint = new IPEndPoint(IPAddress.Any, listenAtPort);
        udpReceiverClient = new UdpClient(controlPoint);
        byte[] cData = udpReceiverClient.Receive(ref controlPoint);
        float[] floats = new float[cData.Length / sizeof(float)];              
        int numLead = (floats.Length-9)/7;
    
        logFilePath = Application.persistentDataPath + "/" + "Data_Simulator_" + System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + ".txt";
        string logEntry = "date" + " time" + " simTime" + " speed" + " rpm" + " egoX" + " egoY" + " throttle" + " brake" + " steer" + " laneDev" + " LaneCenterX" + " LaneCenterY" + " isAuto";

        // Dynamically add columns for each lead vehicle
        for (int i = 1; i <= numLead; i++)
        {
            logEntry += " lead" + i + "X" +
                        " lead" + i + "Y" +
                        " lead" + i + "Xvelo" +
                        " lead" + i + "Yvelo" +
                        " lead" + i + "Throttle" +
                        " lead" + i + "Brake" +
                        " lead" + i + "Steer";
        }

        // Write the log entry header to the file
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        
        //File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        LoadTrackData();
        initializeSettings();

    }
    
    public void initializeSettings()
    {
        IPEndPoint controlPoint = new IPEndPoint(IPAddress.Any, listenAtPort);
        udpReceiverClient = new UdpClient(controlPoint);
        byte[] cData = udpReceiverClient.Receive(ref controlPoint);
        float[] floats = new float[cData.Length / sizeof(float)];              
        valArray = new float[floats.Length+3]; 
        startUdpThread();

    }

    public void LoadTrackData()
    {
        string directoryPath = Application.streamingAssetsPath + "/Tracks/";
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
        int numLead = (floats.Length-9)/7;
        
        // Iterate over the byte array and unpack floats
        for (int i = 0; i < floats.Length; i++)
        {
         // Extract 4 bytes representing a float from the byte array
            byte[] floatBytes = new byte[sizeof(float)];
            System.Array.Copy(bData, i * sizeof(float), floatBytes, 0, sizeof(float));

            // Convert the byte array to a float
            floats[i] = System.BitConverter.ToSingle(floatBytes, 0);
        }
        
        // Assign the received float values to valArray
        int valIndex = 0;
        valArray[valIndex++] = floats[0];   // simTime
        valArray[valIndex++] = floats[1];   // speed
        valArray[valIndex++] = floats[2];   // rpm
        valArray[valIndex++] = floats[3];   // egoX
        valArray[valIndex++] = floats[4];   // egoY
        valArray[valIndex++] = floats[5];   // throttle
        valArray[valIndex++] = floats[6];   // brake
        valArray[valIndex++] = floats[7];   // steer
        valArray[valIndex++] = 0;// valArray[8] is laneDev to be assigned later
        valArray[valIndex++] = 0;// valArray[9] is LaneCenterX to be assigned later
        valArray[valIndex++] = 0;// valArray[10] is LaneCenterY to be assigned later
        valArray[valIndex++] = floats[8];   // isAuto

        int floatIndex = 9;  // Starting index for lead vehicle data in floats

        for (int i = 0; i < numLead; i++)
        {
            valArray[valIndex++] = floats[floatIndex++];  // leadX
            valArray[valIndex++] = floats[floatIndex++];  // leadY
            valArray[valIndex++] = floats[floatIndex++];  // leadXvelo
            valArray[valIndex++] = floats[floatIndex++];  // leadYvelo
            valArray[valIndex++] = floats[floatIndex++];  // leadThrottle
            valArray[valIndex++] = floats[floatIndex++];  // leadBrake
            valArray[valIndex++] = floats[floatIndex++];  // leadSteer
        }       

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
        valArray[9] = closestTrackPoint.x;   // LaneDeviation
        valArray[10] = closestTrackPoint.y;   // LaneDeviation
        
        // Dynamically create the logEntry string based on the contents of valArray
        string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        // Use StringBuilder for efficient string concatenation
        StringBuilder logEntryBuilder = new StringBuilder();
	logEntryBuilder.Append(timestamp);

	for (int i = 0; i < valArray.Length; i++)
	{
	    logEntryBuilder.Append(" ").Append(valArray[i]);
	}

	// Convert StringBuilder to string
	string logEntry = logEntryBuilder.ToString();

	// Append the log entry to the file
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
