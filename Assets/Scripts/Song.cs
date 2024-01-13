using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Note
{
    public float beat;
    public int type;

    public Note(float beat, int type)
    {
        this.beat = beat;
        this.type = type;
    }
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
public struct Projectiles
{
    public float beat;
}

[System.Serializable]
[CreateAssetMenu]
public class Song : ScriptableObject
{
    public string midiPath = "Assets/Audio/midi/really_you.mid";
    public float BPM = 100;
    public int timeSig = 4;
    public List<Note> song = new List<Note>();
    public List<Updates> updates = new List<Updates>();
    public List<Projectiles> projectiles = new List<Projectiles>();
    public string songTitle = "Song";
    public string FMODSongName = "Song";
    public Color color1, color2;
    public float speed1 = 1, speed2 = 1, tiling_x1 = 1, tiling_y1 = 1, tiling_x2 = 1, tiling_y2 = 1;
}
