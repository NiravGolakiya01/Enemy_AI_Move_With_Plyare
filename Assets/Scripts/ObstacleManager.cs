using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public ObstacleData obstacleData;
    public GameObject obstaclePrefab;
    public float cubeSpacing = 1.1f;

    void Start()
    {
        GenerateObstacles();
    }

    void GenerateObstacles()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                int index = z * 10 + x;
                if (obstacleData.obstacles[index])
                {
                    Vector3 position = new Vector3(x * cubeSpacing, 0.5f, z * cubeSpacing); // 0.5 to place sphere above the grid
                    Instantiate(obstaclePrefab, position, Quaternion.identity);
                }
            }
        }
    }
}
