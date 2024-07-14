using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject cubePrefab;
    public int gridSize = 10;
    public float cubeSpacing = 1.1f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(x * cubeSpacing, 0, z * cubeSpacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.GetComponent<TileInfo>().SetPosition(x, z);
            }
        }
    }
}
