using UnityEngine;

[System.Serializable]
public class CollisionSensor 
{
    public Transform anchor;

    public Vector3 offset;

    public float radius = 1f;

    public bool enabled = true;

    public Vector3 WorldPosition()
    {
        if (anchor == null)
        {
            return Vector3.zero;
        }

        return anchor.TransformPoint(
            offset
        );
    }
}
