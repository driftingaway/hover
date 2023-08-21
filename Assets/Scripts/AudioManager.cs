using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public ShipController playerController;
    public WormholeController worm;
    public ScoreManager score;
    public TMP_Text text;
    public Midi2Text midi;

    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInt, songPositionInBeats, prevSongPositionInBeats = 0;
    public float songPositionInBeatsPrecise;
    
    private float noteOffset = 8f;
    int spawnIndex, updateSpawnIndex, projectileSpawnIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = 0.25f;

    public List<Song> songs = new List<Song>();
    Song currentSong;
    float note;
    Updates updates;
    Projectiles projectiles;

    //How many seconds have passed since the song started
    public float dspSongTime;
    FMOD.Studio.EventInstance eventInstance;
    public FMODUnity.EventReference NoteEvent;

    void Start()
    {
        //UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo", false);

        currentSong = songs[GameValues.songIndex];
        currentSong.song = midi.readMidi(currentSong.midiPath, currentSong.timeSig);
        BPM = currentSong.BPM;

        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;

        //init wormhole color from song
        worm.InitColor(currentSong.color1, currentSong.color2);
        StartCoroutine(worm.SetSpeed(currentSong.speed1, currentSong.speed2, 2));
        StartCoroutine(worm.SetTiling(currentSong.tiling_x1, currentSong.tiling_y1, currentSong.tiling_x2, currentSong.tiling_y2, 2));

        //set song title
        text.SetText(currentSong.songTitle);
    
        // set up fmod instance
        eventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/" + currentSong.FMODSongName);
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
        if(projectileSpawnIndex != currentSong.projectiles.Count)
        {
            projectiles = currentSong.projectiles[projectileSpawnIndex];
        }

        // alternative case for timing camera switches and visual updates
        if (updateSpawnIndex < currentSong.updates.Count && updates.beat <= songPositionInBeatsPrecise)
        {
            playerController.Switch(updates.state);
            StartCoroutine(worm.SetSpeed(updates.speed1, updates.speed2, secPerBeat));
            worm.InitColor(updates.color1, updates.color2);
            StartCoroutine(worm.SetTiling(updates.tiling_x1, updates.tiling_y1, updates.tiling_x2, updates.tiling_y2, secPerBeat));

            updateSpawnIndex++;
        }

        // spawn note
        if (spawnIndex < currentSong.song.Count && note - noteOffset <= songPositionInBeatsPrecise)
        {
            if(spawnIndex % 2 == 0)
            {
                tileManager.SpawnTile(0, noteOffset, 0);
            }
            else
            {
                tileManager.SpawnTile(1, noteOffset, 0);
            }
            spawnIndex++;
        }

        // spawn projectile
        if (projectileSpawnIndex < currentSong.projectiles.Count && projectiles.beat <= songPositionInBeatsPrecise)
        {
            tileManager.SpawnTile(2, -1, -5f);
            tileManager.SpawnTile(2, -1, 5f);
            projectileSpawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - currentSong.song[noteIndex])) < timingThreshold)
        {
            if (validInput &&
            ((Input.GetKeyDown(KeyCode.A) && noteIndex % 2 == 0) || (Input.GetKeyDown(KeyCode.D) && noteIndex % 2 == 1)))
            {
                //FMODUnity.RuntimeManager.PlayOneShot(NoteEvent, transform.position);
                score.IncreaseScore();
                validInput = false;
                print("+1");
            } 
        } 

        // if timing is close enough to the next note, shift indicies and re-enable input
        if(noteIndex < currentSong.song.Count - 1 && currentSong.song[noteIndex + 1] - songPositionInBeatsPrecise < timingThreshold)
        {
            noteIndex++;
            validInput = true;
        }

        // color pulsing to the beat
        if(songPositionInBeats != prevSongPositionInBeats)
        {
            prevSongPositionInBeats = songPositionInBeats;
            StartCoroutine(worm.Pulse(1f, 0.4f, secPerBeat));
        }
    }
}
