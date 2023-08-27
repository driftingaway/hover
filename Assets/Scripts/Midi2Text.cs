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

    public List<float> readMidi(string midiPath, int timeSig) {
        MidiFile file = MidiFile.Read(midiPath);
        IEnumerable<Melanchall.DryWetMidi.Interaction.Note> notes = file.GetNotes();

        List<float> retNotes = new List<float>();
        TempoMap tempoMap = file.GetTempoMap();
        foreach (Melanchall.DryWetMidi.Interaction.Note note in notes) {
            BarBeatFractionTimeSpan musicalTime = note.TimeAs<BarBeatFractionTimeSpan>(tempoMap);
            float fixedTime = timeSig * musicalTime.Bars + (float)musicalTime.Beats;
            retNotes.Add(fixedTime);
            //print(fixedTime);
        }
        return retNotes;
    }
}
