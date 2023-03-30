using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public WormholeController worm;
    AudioSource audioData;
    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInBeats;
    public float[] notes = new float[] {};
    private float circleIndex = 24f;
    private float barrierIndex = 0.5f;
    private float noteOffset = 8f;
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
        songPositionInBeats = (int)(songPosition / secPerBeat);
        //print(songPositionInBeats);
        if (nextIndex < notes.Length && notes[nextIndex] - noteOffset <= songPositionInBeats)
        {
            print("spawning at: " + songPositionInBeats);
            tileManager.SpawnTile(1, noteOffset);
            nextIndex++;
        }

        if (circleIndex <= songPositionInBeats)
        {
            if (circleIndex % 4f == 0)
            {
                tileManager.SpawnTile(3, noteOffset);
            }
            else
            {
                tileManager.SpawnTile(1, noteOffset);
            }
            circleIndex++;
        }

        if (songPositionInBeats == 64)
        {
            worm.SetDetail1(1.2f);
            worm.SetDetail2(1.2f);
        }

        //if (barrierIndex <= songPositionInBeats)
        //{
        //    tileManager.SpawnTile(2);
        //    barrierIndex += 0.5f;
        //}
    }
}
