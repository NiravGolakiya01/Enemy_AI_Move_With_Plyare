using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleData))]
public class GridEditor : Editor
{
    private ObstacleData obstacleData;
    private bool[] obstacleToggles = new bool[100]; // 10x10 grid

    private void OnEnable()
    {
        obstacleData = (ObstacleData)target;
        if (obstacleData.obstacles.Length != 100)
        {
            obstacleData.obstacles = new bool[100];
        }
        obstacleToggles = obstacleData.obstacles;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Obstacle Grid", EditorStyles.boldLabel);

        for (int y = 0; y < 10; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < 10; x++)
            {
                int index = y * 10 + x;
                obstacleToggles[index] = GUILayout.Toggle(obstacleToggles[index], GUIContent.none, GUILayout.Width(20), GUILayout.Height(20));
            }
            GUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(obstacleData);
            obstacleData.obstacles = obstacleToggles;
        }
    }
}
