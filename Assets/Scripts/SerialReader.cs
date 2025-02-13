using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Linq;
using System;

public class SerialReader : MonoBehaviour
{

    public SerialPort port = new SerialPort("COM4", 115200);
    

    void Start()
    {
        port.Open();
        Debug.Log("Serial Port Opened.");
        port.DtrEnable = true;
        port.ReadTimeout = 5000;
    }

    private void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    private void OnDisable()
    {
        CloseSerialPort();
    }

    private void CloseSerialPort()
    {
        if (port.IsOpen)
        { 
            port.Close();
            port.Dispose();
            Debug.Log("Serial Port Closed.");
        }
    }

}
