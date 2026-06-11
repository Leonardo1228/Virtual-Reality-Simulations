using UnityEngine;
using System.Collections.Generic;

public class BrickWall : MonoBehaviour
{
    [Header("Brick")]

    public Brick brickPrefab;


    [Header("Wall Size")]

    public int width = 8;

    public int height = 5;


    public Vector3 brickSize =
        new Vector3(
            1f,
            0.5f,
            0.5f
        );


    public float spacing = 0.02f;


    [Header("Generation")]

    public bool generateOnStart = true;

    public bool clearFirst = true;


    private readonly List<Brick> bricks =
        new List<Brick>();



    void Start()
    {
        if (generateOnStart)
        {
            GenerateWall();
        }
    }


    public void GenerateWall()
    {
        if (brickPrefab == null)
            return;


        if (clearFirst)
        {
            ClearWall();
        }


        bricks.Clear();


        float wallWidth =
            width *
            (brickSize.x + spacing);


        Vector3 start =
            transform.position
            -
            Vector3.right *
            wallWidth * 0.5f;


        start +=
            Vector3.up *
            brickSize.y * 0.5f;



        for (int y = 0; y < height; y++)
        {
            float offset =
                (y % 2 == 0)
                ? 0f
                : brickSize.x * 0.5f;


            for (int x = 0; x < width; x++)
            {
                Vector3 position =
                    start
                    +
                    transform.right *
                    (
                        x *
                        (brickSize.x + spacing)
                        +
                        offset
                    )
                    +
                    transform.up *
                    (
                        y *
                        (brickSize.y + spacing)
                    );


                Brick brick =
                    Instantiate(
                        brickPrefab,
                        position,
                        transform.rotation,
                        transform
                    );


                brick.transform.localScale =
                    brickSize;


                /*
                 Estado inicial del ladrillo
                */

                brick.activated = false;

                brick.isStatic = true;

                brick.useGravity = false;

                brick.Stop();


                bricks.Add(brick);
            }
        }
    }



    public void BreakWall(
        Vector3 impactPoint,
        Vector3 impactForce,
        float radius)
    {
        foreach (Brick brick in bricks)
        {
            if (brick == null)
                continue;


            if (brick.activated)
                continue;


            float distance =
                Vector3.Distance(
                    brick.transform.position,
                    impactPoint
                );


            if (distance > radius)
                continue;


            float strength =
                1f -
                Mathf.Clamp01(
                    distance / radius
                );


            brick.Activate(
                impactForce * strength
            );
        }
    }



    public void ClearWall()
    {
        while (transform.childCount > 0)
        {
            Transform child =
                transform.GetChild(0);


#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(
                    child.gameObject
                );
            }
            else
#endif
            {
                Destroy(
                    child.gameObject
                );
            }
        }
    }



    void OnDrawGizmos()
    {
        Gizmos.color =
            Color.yellow;


        float totalWidth =
            width *
            (brickSize.x + spacing);


        float totalHeight =
            height *
            (brickSize.y + spacing);


        Vector3 center =
            transform.position
            +
            transform.up *
            totalHeight * 0.5f;


        Vector3 size =
            new Vector3(
                totalWidth,
                totalHeight,
                brickSize.z
            );


        Gizmos.matrix =
            transform.localToWorldMatrix;


        Gizmos.DrawWireCube(
            Vector3.up * totalHeight * 0.5f,
            size
        );


        Gizmos.matrix =
            Matrix4x4.identity;
    }
}