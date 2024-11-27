using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GridMapBuilder))]
public class GridMapInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Create a new VisualElement to be the root of our inspector UI
        GUILayout.BeginHorizontal();
        GUILayout.Label("Resolution");
        float resolution = float.Parse(GUILayout.TextField("0.5"));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Build"))
        {
            (target as GridMapBuilder).Build(out var map, resolution);

        }
        if (GUILayout.Button("Test"))
        {
            (target as GridMapBuilder).Build(out var map, resolution);

        }

        // Return the finished inspector UI
    }
}
