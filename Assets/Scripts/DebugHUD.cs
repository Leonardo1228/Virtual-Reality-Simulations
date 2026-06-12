using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    void OnGUI()
    {
        var sim = SimulationManager.Instance;

        if (sim == null)
            return;

        GUILayout.BeginArea(
            new Rect(10, 10, 300, 300),
            GUI.skin.box
        );

        GUILayout.Label("SIMULATION DEBUG");

        if (sim.vehicle != null)
        {
            GUILayout.Label(
                "Vehicle speed: " +
                sim.vehicle.CurrentSpeed.ToString("F2")
            );
        }

        if (sim.wind != null)
        {
            GUILayout.Label(
                "Wind: " +
                sim.wind.strength.ToString("F2")
            );
        }

        if (sim.brickWall != null)
        {
            GUILayout.Label(
                "Bricks: " +
                sim.brickWall.ActiveBricks +
                " / " +
                sim.brickWall.BrickCount
            );
        }

        GUILayout.Label(
            "Time Scale: " +
            Time.timeScale
        );

        GUILayout.EndArea();
    }
}
