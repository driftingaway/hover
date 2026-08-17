using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using FMODUnity;
using FMOD.Studio;
using static ShipController;

/*
This is the core of my rhythm game: it mainly handles FMOD integration, tracks note positions and timings for visual updates, and calls into a lot of other scripts. Hopefully a lot of this is self explanatory but I added some more comments to clarify
If I had more time to fix this further, I would probably clean up the way I'm tracking notes (I'm not sure having all of this in Update is the best way to do it) and I would also probably break the scoring functions off into their own script
*/

public class AudioManager : MonoBehaviour
{
    public TileManager tileManager;
    public ShipController shipController;
    public WormholeController worm;
    public HUDController HUD;
    public TMP_Text text;
    public Midi2Text midi;
    public Camera cam;

    public float BPM;
    public float secPerBeat;
    public float songPosition;
    public int songPositionInt, songPositionInBeats = 0;
    public float songPositionInBeatsPrecise, currentNoteBeat;
    public float prevSongPositionInBeatsPrecise = 0;
    private bool isHolding;
    private bool hitLastNote = false;
    
    private float noteOffset = 8f;
    int noteIndex, spawnIndex, updateIndex = 0;
    float timingThreshold; 

    public List<Song> songs = new List<Song>();
    public Song currentSong;
    Note note;
    Updates updates;

    //How many seconds have passed since the song started
    public float dspSongTime;
    EventInstance eventInstance;
    public EventReference NoteEvent;
    public EventReference TickEvent;

    public List<Note> beatMap;
    public List<Updates> updateMap;
    private int streak = 0;
    public int startBeat;
    public float speedMult = 1;

    private float health = 1;

    public ScoreManager scoreRef;
    public Material noteMaterial;
    public Material playerMaterial;
    public ParticleSystem shield;
    private bool allowNextSongPlayback = true;
    private PLAYBACK_STATE state;

    void Start()
    {
        PlaySong();
    }

    void PlaySong() {
        currentSong = songs[GameValues.songIndex];

        // set up fmod instance
        eventInstance = RuntimeManager.CreateInstance("event:/Music/" + currentSong.FMODSongName);
        eventInstance.start();

        // i'm using midi to handle all of the information needed for each song
        currentSong.song = midi.readMidi(currentSong.midiPath, currentSong.timeSig);
        beatMap = currentSong.song;
        updateMap = currentSong.updates;
        BPM = currentSong.BPM;
        speedMult = currentSong.noteSpeed;

        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;
        timingThreshold = 0.1f * (1/secPerBeat);

        //begin spawning notes and environment updates
        tileManager.InitChart();
        worm.InitWormhole();

        noteIndex = spawnIndex = updateIndex = 0;
        prevSongPositionInBeatsPrecise = 0f;
        isHolding = false;

        //set song title
        text.SetText(currentSong.songTitle);

        allowNextSongPlayback = true;
    }

    void Update()
    {
        eventInstance.getPlaybackState(out state);
        // transition into next song in queue when first song finishes
        if(state == PLAYBACK_STATE.STOPPED && allowNextSongPlayback) {
            allowNextSongPlayback = false;
            GameValues.songIndex += 1;
            PlaySong();
        }

        //calculate the position in seconds
        eventInstance.getTimelinePosition(out songPositionInt); 
        songPosition = (float) songPositionInt / 1000f;

        //calculate the position in beats
        songPositionInBeatsPrecise = songPosition / secPerBeat;
        songPositionInBeats = (int)songPositionInBeatsPrecise;

        Debug.Log(noteIndex);

        // initialize indices: noteIndex tracks which note the player is next to hit, spawnIndex tracks which note will be spawned next, updateSpawnIndex tracks which environment update should happen next
        if(noteIndex != beatMap.Count)
        {
            currentNoteBeat = beatMap[noteIndex].beat;
        }

        if(spawnIndex != beatMap.Count)
        {
            note = beatMap[spawnIndex];
        }

        if(updateIndex != updateMap.Count)
        {
            updates = updateMap[updateIndex];
        }

        // timing camera switches and visual updates
        if (updateIndex < updateMap.Count && updates.beat <= songPositionInBeatsPrecise)
        {
            // these are various camera and movement states handled by the ship controller
            if(updates.state == State.Trailing)
            {
                shipController.Trailing();
            }
            else if(updates.state == State.TopDown)
            {
                shipController.Boss();
            }
            else if(updates.state == State.Rail)
            {
                shipController.Shooter();
            }

            // update color and pattern of background visuals
            worm.SetColor(updates.color1, updates.color2);
            worm.SetPattern(updates.pattern.speed1, updates.pattern.speed2, updates.pattern.tiling_x1, updates.pattern.tiling_y1, updates.pattern.tiling_x2, updates.pattern.tiling_y2);

            // strobe light effects
            if(updates.strobe.count != 0)
            {
                worm.Strobe(updates.strobe.count, updates.strobe.duration*secPerBeat, updates.strobe.startIntensity, updates.strobe.endIntensity);
            }

            updateIndex++;
        }

        // spawn note
        if (spawnIndex < beatMap.Count && note.beat - noteOffset <= songPositionInBeatsPrecise)
        {
            tileManager.SpawnNote(note);
            spawnIndex++;
        }

        // handle hitting notes   
        if(Input.GetButtonDown("Hit") && !isHolding)
        {
            //Debug.Log("Hit");
            float acc = Mathf.Abs(songPositionInBeatsPrecise - currentNoteBeat);
            Debug.Log(currentNoteBeat);
            if (acc < timingThreshold)
            {
                //Debug.Log(beatMap[noteIndex].type);
                float length = 0.15f;
                // check held notes too
                if(beatMap[noteIndex].type == 1)
                {
                    isHolding = true;
                    length = beatMap[noteIndex].length * secPerBeat;
                    // extra dramatic camera effects for holding a held note
                    HUD.ChangeFOV(155, length);
                    HUD.BlackBars(60, length);
                }
                HitNote(length);
            }
            else
            {
                MissNote();
            }
        }

        // handle scoring letting go of a held note at the end of the note
        if(Input.GetButtonUp("Hit") && isHolding)
        {
            float hitAcc = Mathf.Abs(songPositionInBeatsPrecise - currentNoteBeat);

            if (hitAcc < timingThreshold)
            {
                HitNote(0.15f);
            }   
            else
            {
                MissNote();
            } 
            // reset FOV and black bars after letting go of held note
            HUD.ChangeFOV(150, .1f);
            HUD.BlackBars(100, .1f);
            isHolding = false;
        }

        if(songPositionInBeatsPrecise > (timingThreshold + currentNoteBeat) && noteIndex != beatMap.Count - 1)
        {
            if(!hitLastNote && beatMap[noteIndex].type != 3)
            {
                MissNote();
            }
            hitLastNote = false;
            noteIndex++;
        }

        // handle firing projectiles (wip), can only fire on downbeats
        if(songPositionInBeatsPrecise >= prevSongPositionInBeatsPrecise + .25f)
        {
            prevSongPositionInBeatsPrecise = (float)(Math.Round (songPositionInBeatsPrecise * 4f, MidpointRounding.ToEven) / 4);
            shipController.canFire = true;
        }
    }

    private void HitNote(float length)
    {
        //Debug.Log("HIT!");
        hitLastNote = true;
        FMODUnity.RuntimeManager.PlayOneShot(NoteEvent, transform.position);
        shield.Play();
        if(health < 1f) {
            health += .25f;
        }
        streak += 1;
        scoreRef.IncreaseScore();
        if(streak % 5 == 0) {
            scoreRef.IncreaseCombo();
        }
        // flash to signify hit
        HitFlash(Color.white, length);
    }

    private void MissNote()
    {
        //Debug.Log("MISS!");
        if (health > 0) {
            hitLastNote = false;
            isHolding = false;
            health -= .25f;
            streak = 0;
            scoreRef.ResetCombo();
            HUD.ChangeFOV(150, .1f);
            HUD.BlackBars(100, .1f);
        }
    }

    public void HitFlash(Color color, float length)
    {
        StartCoroutine(HUD.ImpactFrame());
        noteMaterial.DOColor(color * currentSong.startIntensity, "_Wormhole_colour", 0f);
        playerMaterial.DOColor(color * currentSong.startIntensity, "_Details_1_colour", 0f);
        noteMaterial.DOColor(Color.white * 5, "_Wormhole_colour", length);
        playerMaterial.DOColor(Color.white * 10, "_Details_1_colour", length);
    }
}
