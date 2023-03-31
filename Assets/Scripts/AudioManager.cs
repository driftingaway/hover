using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public WormholeController worm;
    public ScoreManager score;

    AudioSource audioData;
    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInBeats;
    public float songPositionInBeatsPrecise;

    public float[] notes = new float[] {};
    public string[] noteTypes = new string[] {};
    
    private float circleIndex = 24f;
    private float barrierIndex = 0.5f;
    private float noteOffset = 8f;
    int spawnIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = 0.2f;

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
        songPositionInBeatsPrecise = songPosition / secPerBeat;
        songPositionInBeats = (int)songPositionInBeatsPrecise;
        //print(songPositionInBeatsPrecise);

        if (spawnIndex < notes.Length && notes[spawnIndex] - noteOffset <= songPositionInBeatsPrecise)
        {
            if (noteTypes[spawnIndex] == "L")
            {
                tileManager.SpawnTile(1, noteOffset);
            }
            else if (noteTypes[spawnIndex] == "R")
            {
                tileManager.SpawnTile(3, noteOffset);
            }
            //print("spawning at: " + songPositionInBeats);
            spawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - notes[noteIndex])) < timingThreshold)
        {
            if (noteTypes[noteIndex] == "L" && Input.GetKeyDown(KeyCode.Q) && validInput)
            {
                validInput = false;
                score.IncreaseScore();
            } 

            if (noteTypes[noteIndex] == "R" && Input.GetKeyDown(KeyCode.E) && validInput)
            {
                validInput = false;
                score.IncreaseScore();
            } 
        }

        // if timing is close enough to the next note, shift indicies and reenable input
        if(noteIndex < notes.Length - 1 && notes[noteIndex + 1] - songPositionInBeatsPrecise < timingThreshold)
        {
            noteIndex++;
            validInput = true;
        }

        print(noteIndex);

        //if (circleIndex <= songPositionInBeats)
        //{
        //    if (circleIndex % 4f == 0)
        //    {
        //        tileManager.SpawnTile(3, noteOffset);
        //        tileManager.SpawnTile(4, noteOffset);
        //    }
        //    else
        //    {
        //        tileManager.SpawnTile(1, noteOffset);
        //    }
        //    circleIndex++;
        //}

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
