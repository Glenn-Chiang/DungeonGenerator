using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator), true)]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DungeonGenerator generator = (DungeonGenerator) target;

        DrawDefaultInspector();

        // Button to generate map
        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
    }
}