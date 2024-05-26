using UnityEngine;
using System;
using System.IO;

/// <summary>
/// This class serves as the single interface for all file writing.
/// The idea is to report all important events so that the game's
/// actions can be coded and compared to viewed reactions from video.
/// </summary>
public static class FileManagement
{
    private static bool wasInit = false;
    private static string FILENAME;
    private static string pid = PlayerPrefs.GetString("PID");
    private static string cond = PlayerPrefs.GetString("Condition");
    //private static string note = PlayerPrefs.GetString("Note");

    // Get the date to use as the file name.
    public static void init()
    {
        wasInit = true;
        // To ensure files are not overwritten and are easily identifiable, we will name them with the current date and time.
        int day = DateTime.Now.Day;
        int month = DateTime.Now.Month;
        int year = DateTime.Now.Year;
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;
        //string pid = PlayerPrefs.GetString("PID");
        //string cond = PlayerPrefs.GetString("Condition");
        FILENAME = ("Data_NDRT_" + pid + "-" + month + "-" + day + "-" + year + "-" + hour + "-" + minute + "-" + second + "-");
        FILENAME += cond;
        FILENAME = Path.Combine(Application.persistentDataPath, FILENAME);
        FILENAME += ".txt";

        //Debug.Log("Application.persistentDataPath === " + Application.persistentDataPath);
        // Test creating files. 
        print("Note: All times listed are in seconds since the start of the game!");
        print("Some times (such as this message) may be the same. This means both happened within the same frame in Unity.");
        //print("Note: -9999 means emotions could not be analyzed on that frame. (Likely because the face could not be tracked)");
        printRow("PID, Study condition, Trial order, Image set number, Time stamp(1st slide), Time stamp(2nd slide), " +
            "Time stamp(3rd slide), Time stamp (response), RT (s), Answer location, Selected location, Answer image, Selected image, Accuracy", false);
        //getAffectData();
    }

    // Helper to get timestamp string.
    private static string getTime()
    {
        return (Math.Round(Time.time, 2) + " ");
    }

    // Helper to open and write to the file. Keeping all the possible errors to one point.
    private static void print(string message)
    {
        if (!wasInit)
        {
            init();
        }

        using (StreamWriter file = new StreamWriter(FILENAME, true))
        {
            // The using command here automatically closes and flushes the file.
            file.WriteLine(getTime() + message);
        }

    }

    // Helper to open and write to the file. Keeping all the possible errors to one point.
    private static void printRow(string message, bool printTime = true)
    {
        if (!wasInit)
        {
            init();
        }

        using (StreamWriter file = new StreamWriter(FILENAME, true))
        {
            // The using command here automatically closes and flushes the file.
            if (printTime)
                file.WriteLine(getTime() + message);
            else
                file.WriteLine(message);
        }
    }

    public static void dumpRow(string RowVector)
    {
        if (!wasInit)
        {
            init();
        }

        using (StreamWriter file = new StreamWriter(FILENAME, true))
        {
            //file.WriteLine(getTime() + ": " + String.Join("|", arr));
            file.WriteLine(pid + "," + cond + "," + RowVector);
        }
    }
}
