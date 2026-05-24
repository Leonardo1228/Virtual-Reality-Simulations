using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VehicleController))]
public class VehicleControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VehicleController vehicle =
            (VehicleController)target;

        EditorGUILayout.LabelField(
            "Vehicle Settings",
            EditorStyles.boldLabel
        );

        vehicle.engineForce =
            EditorGUILayout.FloatField(
                "Engine Force",
                vehicle.engineForce
            );

        vehicle.steeringSpeed =
            EditorGUILayout.FloatField(
                "Steering Speed",
                vehicle.steeringSpeed
            );

        EditorUtility.SetDirty(vehicle);
    }
}
