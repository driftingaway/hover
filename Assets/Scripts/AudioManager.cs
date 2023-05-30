using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public LanePlayerController playerController;
    public WormholeController worm;
    public ScoreManager score;
    public TMP_Text text;

    public int songIndex = 0;
    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInBeats, prevSongPositionInBeats = 0;
    public float songPositionInBeatsPrecise;
    
    private float noteOffset = 8f;
    private float noteGap = 0.3f;
    int spawnIndex, updateSpawnIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = 0.3f;

    public List<Song> songs = new List<Song>();
    Song currentSong;
    Note note;
    Updates updates;

    //How many seconds have passed since the song started
    public float dspSongTime;

    void Start()
    {
        currentSong = songs[songIndex];
        BPM = currentSong.BPM;

        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;

        //init wormhole color from song
        worm.InitColor(currentSong.color1, currentSong.color2);
        worm.SetSpeed(currentSong.speed1, currentSong.speed2);
        worm.SetTiling(currentSong.tiling_x1, currentSong.tiling_y1, currentSong.tiling_x2, currentSong.tiling_y2);

        //set song title
        text.SetText(currentSong.songTitle);
    
        //record the time when the song starts
        dspSongTime = (float) AudioSettings.dspTime; 

        GetComponent<AudioSource>().clip = currentSong.audioClip;
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
        if(spawnIndex != currentSong.song.Count)
        {
            note = currentSong.song[spawnIndex];
        }
        if(updateSpawnIndex != currentSong.updates.Count)
        {
            updates = currentSong.updates[updateSpawnIndex];
        }

        // alternative case for timing camera switches and visual updates
        if (updateSpawnIndex < currentSong.updates.Count && updates.beat <= songPositionInBeatsPrecise)
        {
            print("yea");
            playerController.Switch(updates.state);
            worm.SetSpeed(updates.speed1, updates.speed2);
            worm.InitColor(updates.color1, updates.color2);
            worm.SetTiling(updates.tiling_x1, updates.tiling_y1, updates.tiling_x2, updates.tiling_y2);

            updateSpawnIndex++;
        }

        if (spawnIndex < currentSong.song.Count && note.beat - noteOffset <= songPositionInBeatsPrecise && note.type != "Update")
        {
            if (note.type == "Wall")
            {
                string lane = note.lane;
                if(lane == "L")
                {
                    tileManager.SpawnTile(2, noteOffset - noteGap, "L");
                    tileManager.SpawnTile(4, noteOffset + noteGap, "M");
                    tileManager.SpawnTile(4, noteOffset + noteGap, "R");
                }
                else if(lane == "M")
                {
                    tileManager.SpawnTile(4, noteOffset + noteGap, "L");
                    tileManager.SpawnTile(2, noteOffset - noteGap, "M");
                    tileManager.SpawnTile(4, noteOffset + noteGap, "R");
                }
                else if(lane == "R")
                {
                    tileManager.SpawnTile(4, noteOffset + noteGap, "L");
                    tileManager.SpawnTile(4, noteOffset + noteGap, "M");
                    tileManager.SpawnTile(2, noteOffset - noteGap, "R");
                }
            }
            if (note.type == "Score")
            {
                tileManager.SpawnTile(3, noteOffset, note.lane);
            }
            if (note.type == "Death")
            {
                tileManager.SpawnTile(7, noteOffset, note.lane);
            }
            
            spawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - currentSong.song[noteIndex].beat)) < timingThreshold)
        {
            if (currentSong.song[noteIndex].type == "Score" && Input.GetKeyDown(KeyCode.Space) && validInput && !playerController.charging)
            {
                validInput = false;
                playerController.charging = true;
            } 

            if (currentSong.song[noteIndex].type == "Score" && Input.GetKeyUp(KeyCode.Space) && validInput && playerController.charging)
            {
                validInput = false;
                playerController.charging = false;
            } 
            else
            {
                if(playerController.charging && Input.GetKeyUp(KeyCode.Space))
                {
                    print("dumbass u suck");
                    playerController.charging = false;
                }
            }
        } 
        else
        {
            if(playerController.charging && Input.GetKeyUp(KeyCode.Space))
            {
                print("dumbass u suck");
                playerController.charging = false;
            }
        }

        // if timing is close enough to the next note, shift indicies and re-enable input
        if(noteIndex < currentSong.song.Count - 1 && currentSong.song[noteIndex + 1].beat - songPositionInBeatsPrecise < timingThreshold)
        {
            noteIndex++;
            validInput = true;
        }

        // color pulsing to the beat
        if(songPositionInBeats != prevSongPositionInBeats)
        {
            prevSongPositionInBeats = songPositionInBeats;
            StartCoroutine(worm.Pulse(1f, 0.5f, secPerBeat));
        }
    }
}
