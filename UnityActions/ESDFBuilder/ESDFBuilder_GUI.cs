using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ESDFBuilder)), CanEditMultipleObjects]
class ESDFBuilder_GUI : Editor
{

    private void OnEnable()
    {
    }
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("Bake"))
        {
            ((ESDFBuilder)target).Bake();
        }
        if (GUILayout.Button("Test"))
        {
            ((ESDFBuilder)target).Test();
        }
    }
}