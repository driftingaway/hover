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
    int spawnIndex, updateSpawnIndex, projectileSpawnIndex,bezierIndex = 0;
    int noteIndex = 0;
    bool validInput = true;
    float timingThreshold = .25f;

    public List<Song> songs = new List<Song>();
    public Song currentSong;
    float timeSig;
    Note note;
    Updates updates;
    Projectiles projectiles;

    //How many seconds have passed since the song started
    public float dspSongTime;
    FMOD.Studio.EventInstance eventInstance;
    public FMODUnity.EventReference NoteEvent;

    public List<Note> beatMap;
    public List<Updates> updateMap;
    public List<Projectiles> projectileMap;
    public int startBeat;
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
        worm.SetPattern(currentSong.pattern.speed1, currentSong.pattern.speed2, currentSong.pattern.tiling_x1, currentSong.pattern.tiling_y1, currentSong.pattern.tiling_x2, currentSong.pattern.tiling_y2);

        //set song title
        text.SetText(currentSong.songTitle);
    
        // set up fmod instance
        eventInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/" + currentSong.FMODSongName);
        //eventInstance.setTimelinePosition(startBeat);
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
            worm.SetPattern(updates.pattern.speed1, updates.pattern.speed2, updates.pattern.tiling_x1, updates.pattern.tiling_y1, updates.pattern.tiling_x2, updates.pattern.tiling_y2);
            worm.InitColor(updates.color1, updates.color2);
            if(updates.strobe.count != 0)
            {
                worm.Strobe(updates.strobe.count, updates.strobe.duration*secPerBeat, updates.strobe.strength);
            }
            updateSpawnIndex++;
        }

        // spawn note
        if (spawnIndex < beatMap.Count && note.beat - noteOffset <= songPositionInBeatsPrecise)
        {
            tileManager.SpawnNote(note);
            spawnIndex++;
        }

        // spawn projectile
        if (projectileSpawnIndex < projectileMap.Count && projectiles.beat <= songPositionInBeatsPrecise)
        {
            tileManager.SpawnTile(2, -1, -5f);
            tileManager.SpawnTile(2, -1, 5f);
            projectileSpawnIndex++;
        }

        // if timing is close enough to a note, check input for a potential hit 
        if((Mathf.Abs(songPositionInBeatsPrecise - beatMap[noteIndex].beat)) < timingThreshold)
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
        if(noteIndex < beatMap.Count - 1 && (songPositionInBeatsPrecise - beatMap[noteIndex].beat) > timingThreshold)
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

        // color pulsing to each beat
        /*
        if(songPositionInBeats == prevSongPositionInBeats + 1)
        {
            prevSongPositionInBeats = songPositionInBeats;
            worm.Flash(1f, 0f, secPerBeat);
        }*/

        // bezier curves
        /*
        if(bezierIndex < beatMap.Count - 1 && beatMap[bezierIndex].beat <= songPositionInBeatsPrecise) {
            float pos = -beatMap[bezierIndex + 1].beat;
            float duration = (beatMap[bezierIndex + 1].beat - beatMap[bezierIndex].beat) * secPerBeat;
            tileManager.Bezier(pos, duration);
            bezierIndex++;
        }*/
    }
}
