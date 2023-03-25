using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    AudioSource audioData;
    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    public float[] notes = new float[] {};
    private float circleIndex = 1f;
    private float barrierIndex = 0.5f;
    int nextIndex = 0;

    //How many seconds have passed since the song started
    public float dspSongTime;

    void Start()
    {
        //calculate how many seconds is one beat
        //we will see the declaration of bpm later
        secPerBeat = 60f / BPM;
    
        //record the time when the song starts
        dspSongTime = (float) AudioSettings.dspTime;

        //start the song
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        //calculate the position in seconds
        songPosition = (float) (AudioSettings.dspTime - dspSongTime);

        //calculate the position in beats
        songPositionInBeats = songPosition / secPerBeat;
        print(songPositionInBeats);
        if (nextIndex < notes.Length && notes[nextIndex] < songPositionInBeats + 2)
        {
            tileManager.SpawnTile(2);
            nextIndex++;
        }

        if (circleIndex < songPositionInBeats)
        {
            tileManager.SpawnTile(1);
            circleIndex++;
        }

        if (barrierIndex < songPositionInBeats)
        {
            tileManager.SpawnTile(2);
            barrierIndex += 0.5f;
        }
    }
}
