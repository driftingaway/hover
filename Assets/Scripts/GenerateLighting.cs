using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Song))]
public class GenerateLighting : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Song song = (Song)target;
        if(GUILayout.Button("Generate Lighting"))
        {
            song.generateLighting(song.midiPath, song.timeSig);
        }
    }
}
