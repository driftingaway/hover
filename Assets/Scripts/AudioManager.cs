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
    
    //private float noteOffset = 80000f;
    int spawnIndex, updateSpawnIndex, projectileSpawnIndex,bezierIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = .25f;

    public List<Song> songs = new List<Song>();
    public Song currentSong;
    float note, timeSig;
    Updates updates;
    Projectiles projectiles;

    //How many seconds have passed since the song started
    public float dspSongTime;
    FMOD.Studio.EventInstance eventInstance;
    public FMODUnity.EventReference NoteEvent;

    public List<float> beatMap;
    public List<Updates> updateMap;
    public List<Projectiles> projectileMap;
    public List<float> notePos;
    private bool hit = false;

    private float health = 1;

    void Start()
    {
        //UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo", false);

        currentSong = songs[GameValues.songIndex];
        currentSong.song = midi.readMidi(currentSong.midiPath, currentSong.timeSig);
        beatMap = currentSong.song;
        updateMap = currentSong.updates;
        projectileMap = currentSong.projectiles;
        timeSig = currentSong.timeSig;
        BPM = currentSong.BPM;

        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;

        //init wormhole color from song
        worm.InitColor(currentSong.color1, currentSong.color2);
        StartCoroutine(worm.SetSpeed(currentSong.speed1, currentSong.speed2, 0));
        StartCoroutine(worm.SetTiling(currentSong.tiling_x1, currentSong.tiling_y1, currentSong.tiling_x2, currentSong.tiling_y2, 0));

        //set song title
        text.SetText(currentSong.songTitle);
    
        // set up fmod instance
        eventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/" + currentSong.FMODSongName);
        eventInstance.start();

        notePos = tileManager.SpawnNotes(beatMap);
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
        if(spawnIndex != beatMap.Count)
        {
            note = beatMap[spawnIndex];
        }
        if(updateSpawnIndex != updateMap.Count)
        {
            updates = updateMap[updateSpawnIndex];
        }
        if(projectileSpawnIndex != projectileMap.Count)
        {
            projectiles = projectileMap[projectileSpawnIndex];
        }

        // alternative case for timing camera switches and visual updates
        if (updateSpawnIndex < updateMap.Count && updates.beat <= songPositionInBeatsPrecise)
        {
            playerController.Switch(updates.state);
            StartCoroutine(worm.SetSpeed(updates.speed1, updates.speed2, secPerBeat));
            worm.InitColor(updates.color1, updates.color2);
            StartCoroutine(worm.SetTiling(updates.tiling_x1, updates.tiling_y1, updates.tiling_x2, updates.tiling_y2, secPerBeat));

            updateSpawnIndex++;
        }

        // spawn note
        /*
        if (spawnIndex < beatMap.Count && note - noteOffset <= songPositionInBeatsPrecise)
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
        }*/

        // spawn projectile
        if (projectileSpawnIndex < projectileMap.Count && projectiles.beat <= songPositionInBeatsPrecise)
        {
            tileManager.SpawnTile(2, -1, -5f);
            tileManager.SpawnTile(2, -1, 5f);
            projectileSpawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - beatMap[noteIndex])) < timingThreshold)
        {
            if (validInput && Input.GetButtonDown("Left")) //&& noteIndex % 2 == 0) || (Input.GetButtonDown("Right") && noteIndex % 2 == 1)))
            {
                FMODUnity.RuntimeManager.PlayOneShot(NoteEvent, transform.position);
                hit = true;
                if(health < 1f) {
                    health += .25f;
                    //eventInstance.setParameterByName("Health", health);
                }
                validInput = false;
            } 
        } 

        // if timing for hitting a note has elapsed, check for miss, move to next note and re-enable input
        if(noteIndex < beatMap.Count - 1 && (songPositionInBeatsPrecise - beatMap[noteIndex]) > timingThreshold)
        {
            // check for missed note
            if (hit == false && health > 0) {
                health -= .25f;
                //eventInstance.setParameterByName("Health", health);
            }
            hit = false;
            noteIndex++;
            validInput = true;
        }

        // color pulsing to each measure
        if(songPositionInBeats == prevSongPositionInBeats + timeSig)
        {
            prevSongPositionInBeats = songPositionInBeats;
            StartCoroutine(worm.Pulse(1f, 0.15f, secPerBeat * timeSig));
        }

        // bezier curves
        if(bezierIndex < beatMap.Count - 1 && beatMap[bezierIndex] <= songPositionInBeatsPrecise) {
            //print("NOW");
            //print(notePos[10]);
            float pos = -notePos[bezierIndex + 1];
            float duration = (beatMap[bezierIndex + 1] - beatMap[bezierIndex]) * secPerBeat;
            tileManager.Bezier(pos, duration);
            bezierIndex++;
        }
    }
}
