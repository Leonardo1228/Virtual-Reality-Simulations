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

    private readonly List<Brick> bricks = new();

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

        float wallWidth = width * (brickSize.x + spacing);

        Vector3 start =
            transform.position
            - transform.right * wallWidth * 0.5f
            + transform.up * brickSize.y * 0.5f;

        for (int y = 0; y < height; y++)
        {
            float offset = (y % 2 == 0) ? 0f : brickSize.x * 0.5f;

            for (int x = 0; x < width; x++)
            {
                Vector3 pos =
                    start
                    + transform.right * (x * (brickSize.x + spacing) + offset)
                    + transform.up * (y * (brickSize.y + spacing));

                Brick brick = Instantiate(
                    brickPrefab,
                    pos,
                    transform.rotation,
                    transform
                );

                brick.transform.localScale = brickSize;

                // ✔ SOLO lógica (no física)
                brick.activated = false;

                // NO tocar isStatic / gravity / Stop()
                // el motor lo controla

                bricks.Add(brick);
            }
        }
    }

    public void BreakWall(Vector3 impactPoint, Vector3 impactForce, float radius)
    {
        foreach (Brick brick in bricks)
        {
            if (brick == null || brick.activated)
                continue;

            float dist = Vector3.Distance(brick.transform.position, impactPoint);

            if (dist > radius)
                continue;

            float strength = 1f - Mathf.Clamp01(dist / radius);

            // energía coherente
            Vector3 impulse = impactForce * strength;

            brick.Activate(impulse);
        }
    }

    public void ClearWall()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
#endif
                Destroy(child.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        float totalWidth = width * (brickSize.x + spacing);
        float totalHeight = height * (brickSize.y + spacing);

        Vector3 center =
            transform.position
            + transform.up * totalHeight * 0.5f;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            Vector3.up * totalHeight * 0.5f,
            new Vector3(totalWidth, totalHeight, brickSize.z)
        );

        Gizmos.matrix = Matrix4x4.identity;
    }
}