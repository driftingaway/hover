using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public PlayerController playerController;
    public WormholeController worm;
    public ScoreManager score;
    public TMP_Text text;

    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInt, songPositionInBeats, prevSongPositionInBeats = 0;
    public float songPositionInBeatsPrecise;
    
    private float noteOffset = 8f;
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
    FMOD.Studio.EventInstance eventInstance;

    void Start()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo", false);

        currentSong = songs[GameValues.songIndex];
        BPM = currentSong.BPM;

        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;

        //init wormhole color from song
        worm.InitColor(currentSong.color1, currentSong.color2);
        worm.SetSpeed(currentSong.speed1, currentSong.speed2);
        worm.SetTiling(currentSong.tiling_x1, currentSong.tiling_y1, currentSong.tiling_x2, currentSong.tiling_y2);

        //set song title
        text.SetText(currentSong.songTitle);
    
        // set up fmod instance
        eventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/" + GameValues.songName);
        eventInstance.start();
    }

    void Update()
    {
        //calculate the position in seconds
        eventInstance.getTimelinePosition(out songPositionInt); 
        songPosition = (float) songPositionInt / 1000f;

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
                tileManager.SpawnTile(2, noteOffset);
            }
            if (note.type == "Score")
            {
                tileManager.SpawnTile(3, noteOffset);
            }
            if (note.type == "Death")
            {
                tileManager.SpawnTile(7, noteOffset);
            }
            
            spawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - currentSong.song[noteIndex].beat)) < timingThreshold)
        {
            if (currentSong.song[noteIndex].type == "Score" && Input.GetKeyDown(KeyCode.Space) && validInput)
            {
                score.IncreaseScore();
                validInput = false;
                print("+1");
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
