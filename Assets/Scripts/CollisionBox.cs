using UnityEngine;

[System.Serializable]
public class CollisionBox
{
    public Transform anchor;

    public Vector3 offset;

    public Vector3 size =
        Vector3.one;

    public bool enabled = true;

    public Vector3 WorldCenter()
    {
        if (anchor == null)
        {
            return Vector3.zero;
        }

        return anchor.TransformPoint(
            offset
        );
    }

    public Quaternion WorldRotation()
    {
        if (anchor == null)
        {
            return Quaternion.identity;
        }

        return anchor.rotation;
    }
}
