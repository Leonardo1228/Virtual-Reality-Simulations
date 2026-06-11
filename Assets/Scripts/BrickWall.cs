using UnityEngine;
using System.Collections.Generic;

public class BrickWall : MonoBehaviour
{
    [Header("Brick")]
    public Brick brickPrefab;

    [Header("Wall Size")]
    public int width = 8;
    public int height = 5;

    public Vector3 brickSize = new Vector3(1f, 0.5f, 0.5f);
    public float spacing = 0.02f;

    [Header("Generation")]
    public bool generateOnStart = true;
    public bool clearFirst = true;

    [Header("Optimization")]
    public bool usePhysicsSleep = true;

    List<Brick> bricks = new();

    void Start()
    {
        if (generateOnStart)
            GenerateWall();
    }

    public void GenerateWall()
    {
        if (brickPrefab == null)
            return;

        if (clearFirst)
            ClearWall();

        bricks.Clear();

        Vector3 origin = transform.position;

        for (int y = 0; y < height; y++)
        {
            float offset = (y % 2 == 0) ? 0f : (brickSize.x * 0.5f);

            for (int x = 0; x < width; x++)
            {
                Vector3 pos =
                    origin +
                    transform.right * (x * (brickSize.x + spacing) + offset) +
                    transform.up * (y * (brickSize.y + spacing));

                Brick brick = Instantiate(
                    brickPrefab,
                    pos,
                    transform.rotation,
                    transform
                );

                brick.transform.localScale = brickSize;
                brick.activated = false;

                SetupBrickRigidbody(brick);

                bricks.Add(brick);
            }
        }
    }

    void SetupBrickRigidbody(Brick brick)
    {
        Rigidbody rb = brick.GetComponent<Rigidbody>();

        rb.mass = 3f;
        rb.linearDamping = 0.2f;
        rb.angularDamping = 0.5f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (usePhysicsSleep)
            rb.sleepThreshold = 0.5f;
    }

    public void BreakWall(Vector3 impactPoint, float radius, Vector3 force)
    {
        foreach (var brick in bricks)
        {
            if (brick == null || brick.activated)
                continue;

            float dist = Vector3.Distance(brick.transform.position, impactPoint);

            if (dist > radius)
                continue;

            float strength = 1f - (dist / radius);

            Rigidbody rb = brick.GetComponent<Rigidbody>();

            if (rb == null) continue;

            rb.isKinematic = false;

            rb.AddForce(force * strength, ForceMode.Impulse);
        }
    }

    public void ClearWall()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
#endif
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}