using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Note
{
    public float beat;
    public string type;
}

[System.Serializable]
public struct Updates
{
    public float beat;
    public int state;
    public Color color1, color2;
    public float speed1, speed2, tiling_x1, tiling_y1, tiling_x2, tiling_y2;
}

[System.Serializable]
[CreateAssetMenu]
public class Song : ScriptableObject
{
    public List<Note> song = new List<Note>();
    public List<Updates> updates = new List<Updates>();
    public string songTitle;
    public AudioClip audioClip;
    public float BPM;
    public Color color1, color2;
    public float speed1, speed2, tiling_x1, tiling_y1, tiling_x2, tiling_y2;
}
