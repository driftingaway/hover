using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using FMODUnity;
using FMOD.Studio;

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
    int noteIndex, spawnIndex, updateSpawnIndex = 0;
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

        currentSong.song = midi.readMidi(currentSong.midiPath, currentSong.timeSig);
        beatMap = currentSong.song;
        updateMap = currentSong.updates;
        BPM = currentSong.BPM;
        speedMult = currentSong.noteSpeed;

        //calculate how many seconds is one beat
        secPerBeat = 60f / BPM;
        timingThreshold = 0.1f * (1/secPerBeat);

        tileManager.InitChart();
        worm.InitWormhole();

        noteIndex = spawnIndex = updateSpawnIndex = 0;
        prevSongPositionInBeatsPrecise = 0f;
        isHolding = false;

        //set song title
        text.SetText(currentSong.songTitle);

        allowNextSongPlayback = true;
    }

    void Update()
    {
        eventInstance.getPlaybackState(out state);
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
        //print(songPositionInBeatsPrecise);

        if(noteIndex != beatMap.Count)
        {
            currentNoteBeat = beatMap[noteIndex].beat;
        }
        if(spawnIndex != beatMap.Count)
        {
            note = beatMap[spawnIndex];
        }
        if(updateSpawnIndex != updateMap.Count)
        {
            updates = updateMap[updateSpawnIndex];
        }

        // alternative case for timing camera switches and visual updates
        if (updateSpawnIndex < updateMap.Count && updates.beat <= songPositionInBeatsPrecise)
        {
            if(updates.state == 1)
            {
                shipController.Boss();
                worm.SceneTransition(true);
            }
            if(updates.state == 2)
            {
                worm.SceneTransition(false);
            }
            if(updates.state == 3)
            {
                StartCoroutine(HUD.TitleDrop(updates.strobe.duration*secPerBeat));
            }
            worm.SetColor(updates.color1, updates.color2);
            worm.SetPattern(updates.pattern.speed1, updates.pattern.speed2, updates.pattern.tiling_x1, updates.pattern.tiling_y1, updates.pattern.tiling_x2, updates.pattern.tiling_y2);
            if(updates.strobe.count != 0)
            {
                worm.Strobe(updates.strobe.count, updates.strobe.duration*secPerBeat, updates.strobe.startIntensity, updates.strobe.endIntensity);
            }
            updateSpawnIndex++;
        }

        // spawn note
        if (spawnIndex < beatMap.Count && note.beat - noteOffset <= songPositionInBeatsPrecise)
        {
            tileManager.SpawnNote(note);
            spawnIndex++;
        }

        if(Input.GetButtonDown("Hit") && !isHolding && beatMap[noteIndex].type != 2)
        {
            float acc = Mathf.Abs(songPositionInBeatsPrecise - currentNoteBeat);
            if (acc < timingThreshold)
            {
                float length = 0.15f;
                if(beatMap[noteIndex].type == 1)
                {
                    isHolding = true;
                    length = beatMap[noteIndex].length * secPerBeat;
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

        if(Input.GetButtonUp("Hit") && isHolding)
        {
            float acc = Mathf.Abs(songPositionInBeatsPrecise - currentNoteBeat);
            //Debug.Log(acc);
            //Debug.Log(timingThreshold);
            if (acc < timingThreshold)
            {
                HitNote(0.15f);
            }   
            else
            {
                MissNote();
            } 
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

        // shoot
        if(songPositionInBeatsPrecise >= prevSongPositionInBeatsPrecise + .25f)
        {
            prevSongPositionInBeatsPrecise = (float)(Math.Round (songPositionInBeatsPrecise * 4f, MidpointRounding.ToEven) / 4);
            shipController.canFire = true;

            //metronome
            //FMODUnity.RuntimeManager.PlayOneShot(TickEvent, transform.position);
        }
    }

    private void HitNote(float length)
    {
        hitLastNote = true;
        FMODUnity.RuntimeManager.PlayOneShot(NoteEvent, transform.position);
        shield.Play();
        //Debug.Log("HIT!");
        if(health < 1f) {
            health += .25f;
            //eventInstance.setParameterByName("Health", health);
        }
        scoreRef.IncreaseScore();
        streak += 1;
        if(streak % 5 == 0) {
            scoreRef.IncreaseCombo();
        }
        //cam.DOShakeRotation(.15f, 1, 1, 25, true);
        HitFlash(Color.white, length);
    }

    private void MissNote()
    {
        //Debug.Log("MISS!");
        if (health > 0) {
            isHolding = false;
            hitLastNote = false;
            health -= .25f;
            //eventInstance.setParameterByName("Health", health);
            streak = 0;
            scoreRef.ResetCombo();
            if(shipController.state != ShipController.State.Overdrive)
            {
                HitFlash(Color.red, 0.5f);
            }
            HUD.ChangeFOV(150, .1f);
            HUD.BlackBars(100, .1f);
        }
    }

    public void HitFlash(Color color, float length)
    {
        //StartCoroutine(HUD.ImpactFrame());
        noteMaterial.DOColor(color * currentSong.startIntensity, "_Wormhole_colour", 0f);
        playerMaterial.DOColor(color * currentSong.startIntensity, "_Details_1_colour", 0f);
        noteMaterial.DOColor(Color.white * 5, "_Wormhole_colour", length);
        playerMaterial.DOColor(Color.white * 10, "_Details_1_colour", length);
    }
}
