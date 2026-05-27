using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoInput : MonoBehaviour
{
    public static ArduinoInput Instance;

    [Header("Serial")]

    public string portName = "COM3";

    public int baudRate = 9600;

    private SerialPort serial;

    [Header("Calibration")]

    public float centerX = 512f;

    public float centerY = 512f;

    public float deadzone = 0.18f;

    [Header("Output")]

    [Range(-1f, 1f)]
    public float horizontal;

    [Range(-1f, 1f)]
    public float vertical;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        try
        {
            serial = new SerialPort(
                portName,
                baudRate
            );

            serial.ReadTimeout = 20;

            serial.Open();

            Debug.Log(
                "Arduino conectado."
            );
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Serial Error: "
                + e.Message
            );
        }
    }

    void Update()
    {
        if (serial == null)
            return;

        if (!serial.IsOpen)
            return;

        try
        {
            string data =
                serial.ReadLine();

            data = data.Trim();

            string[] values =
                data.Split(',');

            if (values.Length < 2)
                return;

            float x =
                float.Parse(
                    values[0].Trim()
                );

            float y =
                float.Parse(
                    values[1].Trim()
                );

            horizontal =
                (x - centerX)
                / 512f;

            vertical =
                (y - centerY)
                / 512f;

            // Invertir vertical
            vertical *= -1f;

            // Deadzone
            if (Mathf.Abs(horizontal)
                < deadzone)
            {
                horizontal = 0f;
            }

            if (Mathf.Abs(vertical)
                < deadzone)
            {
                vertical = 0f;
            }

            // Clamp
            horizontal =
                Mathf.Clamp(
                    horizontal,
                    -1f,
                    1f
                );

            vertical =
                Mathf.Clamp(
                    vertical,
                    -1f,
                    1f
                );
        }
        catch
        {

        }
    }

    void OnApplicationQuit()
    {
        if (serial != null
            && serial.IsOpen)
        {
            serial.Close();
        }
    }
}
