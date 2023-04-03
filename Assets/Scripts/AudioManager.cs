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
    public int songPositionInBeats, prevSongPositionInBeats = 0;
    public float songPositionInBeatsPrecise;
    
    private float circleIndex = 24f;
    private float barrierIndex = 0.5f;
    private float noteOffset = 8f;
    int spawnIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = 0.3f;

    private List<Note> notes;

    //How many seconds have passed since the song started
    public float dspSongTime;

    void Start()
    {
        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;
        print(secPerBeat);
    
        //record the time when the song starts
        dspSongTime = (float) AudioSettings.dspTime; 
        notes = tileManager.notes;

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

        if (spawnIndex < notes.Count && notes[spawnIndex].beat - noteOffset <= songPositionInBeatsPrecise)
        {
            if (notes[spawnIndex].type == "L")
            {
                tileManager.SpawnTile(1, noteOffset, notes[spawnIndex].rotation);
            }
            else if (notes[spawnIndex].type == "R")
            {
                tileManager.SpawnTile(3, noteOffset, notes[spawnIndex].rotation);
            }
            //print("spawning at: " + songPositionInBeats);
            spawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - notes[noteIndex].beat)) < timingThreshold)
        {
            if (notes[noteIndex].type == "L" && Input.GetKeyDown(KeyCode.Mouse0) && validInput)
            {
                validInput = false;
                score.IncreaseScore();
            } 

            if (notes[noteIndex].type == "R" && Input.GetKeyDown(KeyCode.Mouse1) && validInput)
            {
                validInput = false;
                score.IncreaseScore();
            } 
        }

        // if timing is close enough to the next note, shift indicies and re-enable input
        if(noteIndex < notes.Count - 1 && notes[noteIndex + 1].beat - songPositionInBeatsPrecise < timingThreshold)
        {
            noteIndex++;
            validInput = true;
        }

        // color pulsing to the beat
        if(songPositionInBeats != prevSongPositionInBeats)
        {
            prevSongPositionInBeats = songPositionInBeats;
            StartCoroutine(worm.Pulse(1f, 0f, secPerBeat / 2));
        }

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

        //if (songPositionInBeats == 64)
        //{
        //    worm.SetDetail1(1.2f);
        //    worm.SetDetail2(1.2f);
        //}

        //if (barrierIndex <= songPositionInBeats)
        //{
        //    tileManager.SpawnTile(2);
        //    barrierIndex += 0.5f;
        //}
    }
}
