using UnityEngine;

public class BrickWall : MonoBehaviour
{
    [Header("Destruction")]

    public float breakForce = 25000f;

    public GameObject brickPrefab;

    public int width = 8;

    public int height = 5;

    public Vector3 brickSize =
        new Vector3(1f, 0.5f, 0.5f);

    public float spacing = 0.05f;

    private bool destroyed;

    void OnEnable()
    {
        VehicleCollision.allBrickWalls.Add(this);
    }

    void OnDisable()
    {
        VehicleCollision.allBrickWalls.Remove(this);
    }

    public void ReceiveImpact(
        Vector3 impactForce)
    {
        if (destroyed)
            return;

        if (impactForce.magnitude < breakForce)
            return;

        DestroyWall(impactForce);
    }

    void DestroyWall(
        Vector3 impactForce)
    {
        destroyed = true;

        MeshRenderer renderer =
            GetComponent<MeshRenderer>();

        if (renderer != null)
            renderer.enabled = false;

        Collider collider =
            GetComponent<Collider>();

        if (collider != null)
            collider.enabled = false;

        GenerateBricks(impactForce);
    }

    void GenerateBricks(
        Vector3 impactForce)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float offset =
                    (y % 2 == 0)
                    ? 0f
                    : brickSize.x * 0.5f;

                Vector3 position =
                    transform.position +
                    new Vector3(
                        x * (brickSize.x + spacing)
                        + offset,

                        y * (brickSize.y + spacing),

                        0f
                    );

                GameObject brick =
                    Instantiate(
                        brickPrefab,
                        position,
                        Quaternion.identity
                    );

                brick.transform.localScale =
                    brickSize;

                Brick brickScript =
                    brick.GetComponent<Brick>();

                if (brickScript != null)
                {
                    brickScript.ApplyImpact(
                        impactForce * 0.05f
                    );
                }
            }
        }
    }
}
