using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoInput : MonoBehaviour
{
    public static ArduinoInput Instance;

    public float centerX = 512f;
    public float centerY = 512f;

    [Header("Serial")]

    public string portName = "COM3";

    public int baudRate = 9600;

    private SerialPort serial;

    [Header("Joystick")]

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

            serial.ReadTimeout = 50;

            serial.Open();

            Debug.Log(
                "Arduino conectado."
            );
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error Serial: "
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

            string[] values =
                data.Split(',');

            if (values.Length < 2)
                return;

            float x =
                float.Parse(values[0]);

            float y =
                float.Parse(values[1]);

            // Normalizacion del input de joystick
            horizontal =
                (x - centerX) / 512f;

            vertical =
                (y - centerY) / 512f;

            //Y invertido
            vertical *= -1f;

            // Filtración de ruido/Vibración del joystick
            if (Mathf.Abs(horizontal) < 0.15f)
            {
                horizontal = 0f;
            }

            if (Mathf.Abs(vertical) < 0.15f)
            {
                vertical = 0f;
            }
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
