using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Note
{
    public float beat;
    public string type;
    public float rotation;
    public string turn;
}

[System.Serializable]
[CreateAssetMenu]
public class Song : ScriptableObject
{
    public List<Note> song = new List<Note>();
    public AudioClip audioClip;
    public float BPM;
    public Color color;
}
