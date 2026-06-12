using UnityEngine;

public class DebugControls : MonoBehaviour
{
    public bool show = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            show = !show;
        }
    }


    void OnGUI()
    {
        if (!show)
            return;

        var sim = SimulationManager.Instance;

        if (sim == null)
            return;


        GUILayout.BeginArea(
            new Rect(20, 20, 350, 500),
            GUI.skin.window
        );


        GUILayout.Label("SIMULATION CONTROLS");


        // =================
        // VEHICLE
        // =================

        if (sim.vehicle != null)
        {
            GUILayout.Space(10);

            GUILayout.Label("VEHICLE");


            GUILayout.Label(
                "Max Speed: " +
                sim.vehicle.maxSpeed.ToString("F1")
            );

            sim.vehicle.maxSpeed =
                GUILayout.HorizontalSlider(
                    sim.vehicle.maxSpeed,
                    0,
                    100
                );


            GUILayout.Label(
                "Acceleration: " +
                sim.vehicle.acceleration.ToString("F1")
            );

            sim.vehicle.acceleration =
                GUILayout.HorizontalSlider(
                    sim.vehicle.acceleration,
                    0,
                    200
                );


            GUILayout.Label(
                "Turn Speed: " +
                sim.vehicle.turnSpeed.ToString("F1")
            );

            sim.vehicle.turnSpeed =
                GUILayout.HorizontalSlider(
                    sim.vehicle.turnSpeed,
                    0,
                    360
                );
        }


        // =================
        // WIND
        // =================

        if (sim.wind != null)
        {
            GUILayout.Space(10);

            GUILayout.Label("WIND");


            GUILayout.Label(
                "Strength: " +
                sim.wind.strength.ToString("F1")
            );

            sim.wind.strength =
                GUILayout.HorizontalSlider(
                    sim.wind.strength,
                    0,
                    100
                );


            GUILayout.Label(
                "Turbulence: " +
                sim.wind.turbulenceStrength.ToString("F1")
            );

            sim.wind.turbulenceStrength =
                GUILayout.HorizontalSlider(
                    sim.wind.turbulenceStrength,
                    0,
                    50
                );
        }


        // =================
        // TIME
        // =================

        GUILayout.Space(10);

        GUILayout.Label(
            "Time Scale: " +
            Time.timeScale.ToString("F2")
        );

        Time.timeScale =
            GUILayout.HorizontalSlider(
                Time.timeScale,
                0.1f,
                3f
            );


        // =================
        // BUTTONS
        // =================

        GUILayout.Space(15);


        if (GUILayout.Button("Pause Physics"))
        {
            Time.timeScale = 0;
        }


        if (GUILayout.Button("Normal Speed"))
        {
            Time.timeScale = 1;
        }


        GUILayout.EndArea();
    }
}
