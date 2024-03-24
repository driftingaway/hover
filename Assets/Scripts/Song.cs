using UnityEngine;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

[System.Serializable]
public struct Note
{
    public float beat;
    public int type;
    public float length;

    public Note(float beat, int type, float length)
    {
        this.beat = beat;
        this.type = type;
        this.length = length;
    }
}

[System.Serializable]
public struct Updates
{
    public string name;
    public float beat;
    public int state;
    public Color color1, color2;
    public Pattern pattern;
    public Strobe strobe;

    public Updates(float beat, int state, Color color1, Color color2, Pattern pattern, Strobe strobe, string name)
    {
        this.beat = beat;
        this.state = state;
        this.color1 = color1;
        this.color2 = color2;
        this.pattern = pattern;
        this.strobe = strobe;
        this.name = name;
    }
}

[System.Serializable]
public class Strobe
{
    public int count = 1;
    public float duration = 1;
    public float strength = 10;

    public Strobe(float strength) {
        this.strength = strength;
    }
    public Strobe(int count, float duration, float strength)
    {
        this.count = count;
        this.duration = duration;
        this.strength = strength;
    }
}

[System.Serializable]
public struct Projectiles
{
    public float beat;
}

[System.Serializable]
[CreateAssetMenu]
public class Song : ScriptableObject
{
    public string midiPath = "Assets/Audio/midi/really_you.mid";
    public float BPM = 100;
    public int timeSig = 4;
    public List<Note> song = new List<Note>();
    public List<Updates> updates = new List<Updates>();
    public List<Projectiles> projectiles = new List<Projectiles>();
    public string songTitle = "Song";
    public string FMODSongName = "Song";
    public Color color1, color2;
    public Pattern pattern;
    public float lightingIntensity = 10f;

    public void generateLighting(string midiPath, int timeSig) {
        updates.Clear();
        MidiFile midi = MidiFile.Read(midiPath);
        IEnumerable<Melanchall.DryWetMidi.Interaction.Note> notes = midi.GetNotes();
        TempoMap tempoMap = midi.GetTempoMap();

        foreach (Melanchall.DryWetMidi.Interaction.Note note in notes) {
            BarBeatFractionTimeSpan startTime = note.TimeAs<BarBeatFractionTimeSpan>(tempoMap);
            BarBeatFractionTimeSpan endTime = startTime + note.LengthAs<BarBeatFractionTimeSpan>(tempoMap);
            float fixedStartTime = timeSig * startTime.Bars + (float)startTime.Beats;
            float fixedEndTime = timeSig * endTime.Bars + (float)endTime.Beats;
            float noteLength = fixedEndTime - fixedStartTime;

            if (note.NoteName.ToString() == "F")
            {
                updates.Add(new Updates(fixedStartTime, 0, color1, color2, pattern, new Strobe(lightingIntensity), "FLASH"));
            } 
            if (note.NoteName.ToString() == "FSharp")
            {
                updates.Add(new Updates(fixedStartTime, 0, color1, color2, pattern, new Strobe(6,noteLength,lightingIntensity*2.5f), "STROBE"));
            } 
        }
    }
}
