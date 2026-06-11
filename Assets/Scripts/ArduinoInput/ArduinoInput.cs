using UnityEngine;
using System.IO.Ports;
using System;
using System.Globalization;

public class ArduinoInput : MonoBehaviour
{
    public static ArduinoInput Instance;


    [Header("Serial")]

    public string portName = "COM3";

    public int baudRate = 9600;

    public bool autoReconnect = true;

    public float reconnectTime = 2f;


    private SerialPort serial;

    private float reconnectTimer;


    [Header("Calibration")]

    public float centerX = 512f;

    public float centerY = 512f;

    [Range(0f, 0.5f)]
    public float deadzone = 0.18f;


    [Header("Smoothing")]

    [Range(0f, 30f)]
    public float smoothing = 10f;


    [Header("Output")]

    [Range(-1f, 1f)]
    public float horizontal;

    [Range(-1f, 1f)]
    public float vertical;


    public bool IsConnected =>
        serial != null &&
        serial.IsOpen;



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }



    void Start()
    {
        Connect();
    }



    void Update()
    {
        if (!IsConnected)
        {
            HandleReconnect();
            return;
        }


        try
        {
            string data =
                serial.ReadLine().Trim();


            string[] values =
                data.Split(',');


            if (values.Length < 2)
                return;


            float x =
                float.Parse(
                    values[0],
                    CultureInfo.InvariantCulture
                );


            float y =
                float.Parse(
                    values[1],
                    CultureInfo.InvariantCulture
                );


            float targetHorizontal =
                (x - centerX) / 512f;


            float targetVertical =
                -(y - centerY) / 512f;


            ApplyDeadzone(
                ref targetHorizontal
            );

            ApplyDeadzone(
                ref targetVertical
            );


            targetHorizontal =
                Mathf.Clamp(
                    targetHorizontal,
                    -1f,
                    1f
                );


            targetVertical =
                Mathf.Clamp(
                    targetVertical,
                    -1f,
                    1f
                );


            /*
             Suavizado del joystick
            */

            horizontal =
                Mathf.Lerp(
                    horizontal,
                    targetHorizontal,
                    smoothing *
                    Time.deltaTime
                );


            vertical =
                Mathf.Lerp(
                    vertical,
                    targetVertical,
                    smoothing *
                    Time.deltaTime
                );
        }
        catch (TimeoutException)
        {
            // normal cuando no llegan datos
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "Arduino desconectado: "
                + e.Message
            );

            Disconnect();
        }
    }



    void Connect()
    {
        try
        {
            serial =
                new SerialPort(
                    portName,
                    baudRate
                );

            serial.ReadTimeout = 20;

            serial.Open();


            Debug.Log(
                "Arduino conectado en "
                + portName
            );
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "No se pudo abrir "
                + portName +
                ": " +
                e.Message
            );
        }
    }



    void Disconnect()
    {
        if (serial != null)
        {
            try
            {
                serial.Close();
            }
            catch
            {
            }
        }

        serial = null;
    }



    void HandleReconnect()
    {
        if (!autoReconnect)
            return;


        reconnectTimer += Time.deltaTime;


        if (reconnectTimer >= reconnectTime)
        {
            reconnectTimer = 0f;

            Connect();
        }
    }



    void ApplyDeadzone(ref float value)
    {
        if (Mathf.Abs(value) < deadzone)
        {
            value = 0f;
        }
    }



    void OnApplicationQuit()
    {
        Disconnect();
    }
}
