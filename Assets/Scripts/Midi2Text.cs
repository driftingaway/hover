using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;

public class Midi2Text : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Note> readMidi(string midiPath, int timeSig) {
        if(midiPath == "") { return new List<Note>{new Note(0, 0, 0)}; }

        MidiFile midi = MidiFile.Read(midiPath);
        IEnumerable<Melanchall.DryWetMidi.Interaction.Note> notes = midi.GetNotes();
        List<Note> retNotes = new List<Note>();
        TempoMap tempoMap = midi.GetTempoMap();

        foreach (Melanchall.DryWetMidi.Interaction.Note note in notes) {
            BarBeatFractionTimeSpan startTime = note.TimeAs<BarBeatFractionTimeSpan>(tempoMap);
            BarBeatFractionTimeSpan endTime = startTime + note.LengthAs<BarBeatFractionTimeSpan>(tempoMap);
            float fixedStartTime = timeSig * startTime.Bars + (float)startTime.Beats;
            float fixedEndTime = timeSig * endTime.Bars + (float)endTime.Beats;
            float noteLength = fixedEndTime - fixedStartTime;

            if (note.NoteName.ToString() == "C")
            {
                retNotes.Add(new Note(fixedStartTime, 0, 0));
            } 
            /*
            else if (note.NoteName.ToString() == "A")
            {
                retNotes.Add(new Note(fixedStartTime, 1, 0));
            } */
            else if (note.NoteName.ToString() == "CSharp")
            {
                retNotes.Add(new Note(fixedStartTime, 2, noteLength));
                retNotes.Add(new Note(fixedEndTime, 2, 0));
            } 
            /*
            else if (note.NoteName.ToString() == "ASharp")
            {
                retNotes.Add(new Note(fixedStartTime, 3, noteLength));
                retNotes.Add(new Note(fixedEndTime, 3, 0));
            } */
            //print(fixedTime);
        }

        retNotes.Sort((x, y) => x.beat.CompareTo(y.beat));

        return retNotes;
    }
}
