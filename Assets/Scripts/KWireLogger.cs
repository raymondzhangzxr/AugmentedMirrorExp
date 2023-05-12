using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class KWireLogger : MonoBehaviour
{
    // Using the logger
    private MyCSVUtils.CSVLogger logger;
    // the game object to log
    public GameObject objectToLog;
    public GameObject textOb;
    // Flush once per second
    private float nextFlushTime = 0.0f;
    public float flushPeriod = 5.0f;
    public bool isLogging = false;
    void Start()
    {
        logger = GetComponent<MyCSVUtils.CSVLogger>();
        if (logger == null)
        {
            logger = gameObject.AddComponent<MyCSVUtils.CSVLogger>();
            logger.DataSuffix = "KWire";
            logger.CSVHeader = "Timestamp,SessionID,RecordingID," +
                                        "ObjectName,TransX,TransY,TransZ,RotX,RotY,RotZ,RotW";
            // For Debugging
            // logger.textOb = textOb;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(objectToLog != null && isLogging)
        {
            Vector3 position = objectToLog.transform.position;
            Quaternion rotation = objectToLog.transform.rotation;

            // Create a new row of data to log
            List<string> rowData = logger.RowWithStartData();
            rowData.Add(objectToLog.name); // Add the name of the game object
            rowData.Add(position.x.ToString()); // Add the x-coordinate of the position
            rowData.Add(position.y.ToString()); // Add the y-coordinate of the position
            rowData.Add(position.z.ToString()); // Add the z-coordinate of the position
            rowData.Add(rotation.x.ToString()); // Add the x-component of the rotation quaternion
            rowData.Add(rotation.y.ToString()); // Add the y-component of the rotation quaternion
            rowData.Add(rotation.z.ToString()); // Add the z-component of the rotation quaternion
            rowData.Add(rotation.w.ToString()); // Add the w-component of the rotation quaternion

            logger.AddRow(rowData); // Add the row of data to the CSV file
        }

        if (isLogging && Time.time > nextFlushTime)
        {
            //TextMesh t = textOb.GetComponent<TextMesh>();
            //t.text = "Flushed" + " " + Time.time;
            nextFlushTime = Time.time + flushPeriod;
            logger.FlushData();
        }
        
        


    }

    public void StartLogging()
    {
        Debug.Log("Kwire logging started");
        TextMesh t = textOb.GetComponent<TextMesh>();
        t.text = "Logging started";
        
        isLogging = true;
        logger.StartNewCSV();
        logger.FlushData();
    }

    public void StopLogging()
    {
        Debug.Log("Kwire logging stopped");
        
        TextMesh t = textOb.GetComponent<TextMesh>();
        t.text = "Logging ended";
        isLogging = false;
        logger.EndCSV();
    }
}
