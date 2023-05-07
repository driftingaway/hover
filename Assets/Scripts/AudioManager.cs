using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public LanePlayerController playerController;
    public WormholeController worm;
    public ScoreManager score;

    public int songIndex = 0;
    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInBeats, prevSongPositionInBeats = 0;
    public float songPositionInBeatsPrecise;
    
    //private float circleIndex = 24f;
    //private float barrierIndex = 0.5f;
    private float noteOffset = 8f;
    private float noteGap = 0.25f;
    int spawnIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = 0.3f;

    public List<Song> songs = new List<Song>();
    Song currentSong;

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
    
        //record the time when the song starts
        dspSongTime = (float) AudioSettings.dspTime; 

        GetComponent<AudioSource>().clip = currentSong.audioClip;
        //start the song
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        print(validInput);
        //calculate the position in seconds
        songPosition = (float) (AudioSettings.dspTime - dspSongTime);

        //calculate the position in beats
        songPositionInBeatsPrecise = songPosition / secPerBeat;
        songPositionInBeats = (int)songPositionInBeatsPrecise;
        //print(songPositionInBeatsPrecise);

        // alternative case for timing camera switch
        if (spawnIndex < currentSong.song.Count && currentSong.song[spawnIndex].beat <= songPositionInBeatsPrecise && currentSong.song[spawnIndex].type == "Switch")
        {
            playerController.Switch();
            spawnIndex++;
        }

        if (spawnIndex < currentSong.song.Count && currentSong.song[spawnIndex].beat - noteOffset <= songPositionInBeatsPrecise && currentSong.song[spawnIndex].type != "Switch")
        {
            if (currentSong.song[spawnIndex].type == "Wall")
            {
                string lane = currentSong.song[spawnIndex].lane;
                if(lane == "L")
                {
                    tileManager.SpawnTile(2, noteOffset - noteGap, currentSong.song[spawnIndex].rotation, "L");
                    tileManager.SpawnTile(4, noteOffset + noteGap, currentSong.song[spawnIndex].rotation, "M");
                    tileManager.SpawnTile(4, noteOffset + noteGap, currentSong.song[spawnIndex].rotation, "R");
                }
                else if(lane == "M")
                {
                    tileManager.SpawnTile(4, noteOffset + noteGap, currentSong.song[spawnIndex].rotation, "L");
                    tileManager.SpawnTile(2, noteOffset - noteGap, currentSong.song[spawnIndex].rotation, "M");
                    tileManager.SpawnTile(4, noteOffset + noteGap, currentSong.song[spawnIndex].rotation, "R");
                }
                else if(lane == "R")
                {
                    tileManager.SpawnTile(4, noteOffset + noteGap, currentSong.song[spawnIndex].rotation, "L");
                    tileManager.SpawnTile(4, noteOffset + noteGap, currentSong.song[spawnIndex].rotation, "M");
                    tileManager.SpawnTile(2, noteOffset - noteGap, currentSong.song[spawnIndex].rotation, "R");
                }
            }
            if (currentSong.song[spawnIndex].type == "Score")
            {
                tileManager.SpawnTile(3, noteOffset, currentSong.song[spawnIndex].rotation, currentSong.song[spawnIndex].lane);
            }
            if (currentSong.song[spawnIndex].type == "Death")
            {
                tileManager.SpawnTile(7, noteOffset, currentSong.song[spawnIndex].rotation, currentSong.song[spawnIndex].lane);
            }

            if (currentSong.song[spawnIndex].turn == "L")
            {
                tileManager.SpawnTile(6, noteOffset, 0, currentSong.song[spawnIndex].lane);
            }
            if (currentSong.song[spawnIndex].turn == "R")
            {
                tileManager.SpawnTile(5, noteOffset, 0, currentSong.song[spawnIndex].lane);
            }
            
            //print("spawning at: " + songPositionInBeats);
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
                if(currentSong.song[noteIndex + 1].type == "Death")
                {
                    tileManager.DestroyWall();
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
            StartCoroutine(worm.Pulse(1f, 0f, secPerBeat));
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
