
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;



public class EyeHMDTrackingLogger : MonoBehaviour
{
    // Using the logger
    private MyCSVUtils.CSVLogger logger;
    
    
    // Flush once per second
    private float nextFlushTime = 0.0f;
    public float flushPeriod = 1.0f;
    public bool isLogging = false;
    private GameObject object_looking = null;
    public GameObject objectToLog;
    
    void Start()
    {
        logger = GetComponent<MyCSVUtils.CSVLogger>();
        if (logger == null)
        {
            logger = gameObject.AddComponent<MyCSVUtils.CSVLogger>();
            logger.DataSuffix = "HMDAndEyeTarget";
            logger.CSVHeader = "Timestamp,SessionID,RecordingID," +
                                        "EyeTarget,TransX,TransY,TransZ,RotX,RotY,RotZ,RotW";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLogging && Time.time > nextFlushTime)
        {
            nextFlushTime += flushPeriod;
            logger.FlushData();
        }

        if (objectToLog != null && isLogging)
        {
            Vector3 position = objectToLog.transform.position;
            Quaternion rotation = objectToLog.transform.rotation;
            object_looking = CoreServices.InputSystem.EyeGazeProvider.GazeTarget;

            // Create a new row of data to log
            List<string> rowData = logger.RowWithStartData();
            
            if (object_looking != null)
            {
                rowData.Add(object_looking.name); // Add the name of the game object
            }
            else
            {
                rowData.Add("NULL");
            }
            
            rowData.Add(position.x.ToString()); // Add the x-coordinate of the position
            rowData.Add(position.y.ToString()); // Add the y-coordinate of the position
            rowData.Add(position.z.ToString()); // Add the z-coordinate of the position
            rowData.Add(rotation.x.ToString()); // Add the x-component of the rotation quaternion
            rowData.Add(rotation.y.ToString()); // Add the y-component of the rotation quaternion
            rowData.Add(rotation.z.ToString()); // Add the z-component of the rotation quaternion
            rowData.Add(rotation.w.ToString()); // Add the w-component of the rotation quaternion

            logger.AddRow(rowData); // Add the row of data to the CSV file
        }


    }


    public void StartLogging()
    {
        Debug.Log("Eye Tracking and HMD logging started");
        isLogging = true;
        logger.StartNewCSV();
        logger.FlushData();
    }

    public void StopLogging()
    {
        Debug.Log("Eye Tracking and HMD logging stopped");
        isLogging = false;
        logger.EndCSV();
    }
}
