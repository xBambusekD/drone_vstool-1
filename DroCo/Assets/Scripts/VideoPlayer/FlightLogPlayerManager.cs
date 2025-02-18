using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using Unity.Android.Gradle;
using UnityEngine;

public class FlightLogPlayerManager : Singleton<FlightLogPlayerManager> {

    public int LoadedLogLines = 0;
    //private string[] flightLog;


    private List<long> lineOffsets = new List<long>();
    private FileStream fileStream;
    private StreamReader reader;
    private string filePath;


    public void LoadDefaultFlightLog(string file = "flight_log_20241106_160218.txt") {
        filePath = Application.persistentDataPath + "/flightLogs/" + file;
        IndexFile(filePath);
        OpenFile();
        LoadedLogLines = lineOffsets.Count;

        DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(GetLine(0));

        ConnectDrone(flightData.client_id, "experiment_test_drone", "experiment_serial");
    }

    public void PlayLogMessage(int line) {
        string logLine = GetLine(line);
        //Debug.Log(logLine);

        try {
            DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(logLine);
            DroneManager.Instance.HandleReceivedDroneData(flightData);
        } catch(ArgumentException e) {
            Debug.Log(e.Message);
        }
    }

    public void PlayLogMessage(string message) {
        DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(message);
        DroneManager.Instance.HandleReceivedDroneData(flightData);
    }

    private void IndexFile(string path) {
        lineOffsets.Clear();
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            using (StreamReader reader = new StreamReader(fs)) {
                long position = 0;
                while (!reader.EndOfStream) {
                    lineOffsets.Add(position);
                    reader.ReadLine();  // Move to next line
                    position = fs.Position;  // Update position
                }
            }
        }
        Debug.Log($"Indexed {lineOffsets.Count} lines.");
    }

    private void OpenFile() {
        fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        reader = new StreamReader(fileStream);
    }

    private string GetLine(int lineNumber) {
        if (lineNumber < 0 || lineNumber >= lineOffsets.Count)
            return null;

        fileStream.Seek(lineOffsets[lineNumber], SeekOrigin.Begin);

        return reader.ReadLine();

        //using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //using (StreamReader reader = new StreamReader(fs)) {
        //    fs.Seek(lineOffsets[lineNumber], SeekOrigin.Begin);
        //    return reader.ReadLine();
        //}
    }

    private void OnApplicationQuit() {
        reader?.Close();
        fileStream?.Close();
    }







    //public void LoadDefaultFlightLog(string file = "flight_log_20241106_160218.txt") {
    //    flightLog = File.ReadAllLines(Application.persistentDataPath + "/flightLogs/" + file);
    //    LoadedLogLines = flightLog.Length;
    //    Debug.Log(flightLog.Length);

    //    DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(flightLog[0]);

    //    ConnectDrone(flightData.client_id, "experiment_test_drone", "experiment_serial");
    //}

    public void ConnectDrone(string client_id, string drone_name, string serial) {
        DroneStaticData newDrone = new DroneStaticData {
            client_id = client_id,
            drone_name = drone_name,
            serial = serial
        };

        DroneManager.Instance.AddDrone(newDrone);
    }

    //public string GetLogMessage(int line) {
    //    return flightLog[line];
    //}

    //public void PlayLogMessage(int line) {
    //    string logLine = flightLog[line];
    //    DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(logLine);
    //    DroneManager.Instance.HandleReceivedDroneData(flightData);
    //    //ExperimentManager.Instance.SendFlightLogMessage(logLine, line);
    //}

    //public void PlayLogMessage(string message) {
    //    DroneFlightData flightData = JsonUtility.FromJson<DroneFlightData>(message);
    //    DroneManager.Instance.HandleReceivedDroneData(flightData);
    //}
}
