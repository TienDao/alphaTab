﻿using AlphaTab.Collections;
using AlphaTab.Platform;
using SharpKit.JavaScript;

namespace AlphaTab.Model
{
    /// <summary>
    /// This class can convert a full <see cref="Score"/> instance to a simple JavaScript object and back for further
    /// JSON serialization. 
    /// </summary>
    public class JsonConverter
    {
        public Score ScoreToJsObject(Score score)
        {
            Score score2 = Std.NewObject();
            Score.CopyTo(score, score2);
            score2.MasterBars = new FastList<MasterBar>();
            score2.Tracks = new FastList<Track>();

            #region MasterBars

            for (var i = 0; i < score.MasterBars.Count; i++)
            {
                MasterBar masterBar = score.MasterBars[i];
                MasterBar masterBar2 = Std.NewObject();
                MasterBar.CopyTo(masterBar, masterBar2);
                if (masterBar.TempoAutomation != null)
                {
                    masterBar2.TempoAutomation = Std.NewObject();
                    Automation.CopyTo(masterBar.TempoAutomation, masterBar2.TempoAutomation);
                }
                if (masterBar.VolumeAutomation != null)
                {
                    masterBar2.VolumeAutomation = Std.NewObject();
                    Automation.CopyTo(masterBar.VolumeAutomation, masterBar2.VolumeAutomation);
                }
                if (masterBar.Section != null)
                {
                    masterBar2.Section = Std.NewObject();
                    Section.CopyTo(masterBar.Section, masterBar2.Section);
                }
                score2.MasterBars.Add(masterBar2);
            }

            #endregion

            #region Tracks

            for (int t = 0; t < score.Tracks.Count; t++)
            {
                var track = score.Tracks[t];
                Track track2 = Std.NewObject();
                track2.Color = Std.NewObject();
                Track.CopyTo(track, track2);

                track2.PlaybackInfo = Std.NewObject();
                PlaybackInformation.CopyTo(track.PlaybackInfo, track2.PlaybackInfo);

                track2.Chords = new FastDictionary<string, Chord>();
                foreach (var key in track.Chords.Keys)
                {
                    var chord = track.Chords[key];
                    Chord chord2 = Std.NewObject();
                    Chord.CopyTo(chord, chord2);
                    track2.Chords[key] = chord;
                }

                #region Staves
                track2.Staves = new FastList<Staff>();

                for (int s = 0; s < track.Staves.Count; s++)
                {
                    var staff = track.Staves[s];
                    Staff staff2 = Std.NewObject();

                    #region Bars

                    staff2.Bars = new FastList<Bar>();
                    for (int b = 0; b < staff.Bars.Count; b++)
                    {
                        var bar = staff.Bars[b];
                        Bar bar2 = Std.NewObject();
                        Bar.CopyTo(bar, bar2);

                        #region Voices

                        bar2.Voices = new FastList<Voice>();
                        for (int v = 0; v < bar.Voices.Count; v++)
                        {
                            var voice = bar.Voices[v];
                            Voice voice2 = Std.NewObject();
                            Voice.CopyTo(voice, voice2);

                            #region Beats

                            voice2.Beats = new FastList<Beat>();
                            for (int bb = 0; bb < voice.Beats.Count; bb++)
                            {
                                var beat = voice.Beats[bb];
                                Beat beat2 = Std.NewObject();
                                Beat.CopyTo(beat, beat2);

                                beat2.Automations = new FastList<Automation>();
                                for (int a = 0; a < beat.Automations.Count; a++)
                                {
                                    Automation automation = Std.NewObject();
                                    Automation.CopyTo(beat.Automations[a], automation);
                                    beat2.Automations.Add(automation);
                                }

                                beat2.WhammyBarPoints = new FastList<BendPoint>();
                                for (int i = 0; i < beat.WhammyBarPoints.Count; i++)
                                {
                                    BendPoint point = Std.NewObject();
                                    BendPoint.CopyTo(beat.WhammyBarPoints[i], point);
                                    beat2.WhammyBarPoints.Add(point);
                                }

                                #region Notes

                                beat2.Notes = new FastList<Note>();
                                for (int n = 0; n < beat.Notes.Count; n++)
                                {
                                    var note = beat.Notes[n];
                                    Note note2 = Std.NewObject();
                                    Note.CopyTo(note, note2);

                                    note2.BendPoints = new FastList<BendPoint>();
                                    for (int i = 0; i < note.BendPoints.Count; i++)
                                    {
                                        BendPoint point = Std.NewObject();
                                        BendPoint.CopyTo(note.BendPoints[i], point);
                                        note2.BendPoints.Add(point);
                                    }

                                    beat2.Notes.Add(note2);
                                }

                                #endregion

                                voice2.Beats.Add(beat2);
                            }

                            #endregion

                            bar2.Voices.Add(voice2);
                        }

                        #endregion

                        staff2.Bars.Add(bar2);
                    }

                    #endregion
                    track2.Staves.Add(staff);
                }

                #endregion

                score2.Tracks.Add(track2);
            }

            #endregion

            return score2;
        }

        public Score JsObjectToScore(Score score)
        {
            var score2 = new Score();
            Score.CopyTo(score, score2);

            #region MasterBars

            for (var i = 0; i < score.MasterBars.Count; i++)
            {
                var masterBar = score.MasterBars[i];
                var masterBar2 = new MasterBar();
                MasterBar.CopyTo(masterBar, masterBar2);
                if (masterBar.TempoAutomation != null)
                {
                    masterBar2.TempoAutomation = new Automation();
                    Automation.CopyTo(masterBar.TempoAutomation, masterBar2.TempoAutomation);
                }
                if (masterBar.VolumeAutomation != null)
                {
                    masterBar2.VolumeAutomation = new Automation();
                    Automation.CopyTo(masterBar.VolumeAutomation, masterBar2.VolumeAutomation);
                }
                if (masterBar.Section != null)
                {
                    masterBar2.Section = new Section();
                    Section.CopyTo(masterBar.Section, masterBar2.Section);
                }
                score2.AddMasterBar(masterBar2);
            }

            #endregion

            #region Tracks

            for (int t = 0; t < score.Tracks.Count; t++)
            {
                var track = score.Tracks[t];
                var track2 = new Track(track.Staves.Count);
                Track.CopyTo(track, track2);
                score2.AddTrack(track2);

                PlaybackInformation.CopyTo(track.PlaybackInfo, track2.PlaybackInfo);

                foreach (var key in track.Chords.Keys)
                {
                    var chord = track.Chords[key];
                    var chord2 = new Chord();
                    Chord.CopyTo(chord, chord2);
                    track2.Chords[key] = chord2;
                }

                #region Staves

                for (var s = 0; s < track.Staves.Count; s++)
                {
                    var staff = track.Staves[s];
                    #region Bars

                    for (int b = 0; b < staff.Bars.Count; b++)
                    {
                        var bar = staff.Bars[b];
                        var bar2 = new Bar();
                        Bar.CopyTo(bar, bar2);
                        track2.AddBarToStaff(s, bar2);

                        #region Voices

                        for (int v = 0; v < bar.Voices.Count; v++)
                        {
                            var voice = bar.Voices[v];
                            var voice2 = new Voice();
                            Voice.CopyTo(voice, voice2);
                            bar2.AddVoice(voice2);

                            #region Beats

                            for (int bb = 0; bb < voice.Beats.Count; bb++)
                            {
                                var beat = voice.Beats[bb];
                                var beat2 = new Beat();
                                Beat.CopyTo(beat, beat2);
                                voice2.AddBeat(beat2);

                                for (int a = 0; a < beat.Automations.Count; a++)
                                {
                                    var automation = new Automation();
                                    Automation.CopyTo(beat.Automations[a], automation);
                                    beat2.Automations.Add(automation);
                                }

                                for (int i = 0; i < beat.WhammyBarPoints.Count; i++)
                                {
                                    var point = new BendPoint();
                                    BendPoint.CopyTo(beat.WhammyBarPoints[i], point);
                                    beat2.WhammyBarPoints.Add(point);
                                }

                                #region Notes

                                for (int n = 0; n < beat.Notes.Count; n++)
                                {
                                    var note = beat.Notes[n];
                                    var note2 = new Note();
                                    Note.CopyTo(note, note2);
                                    beat2.AddNote(note2);

                                    for (int i = 0; i < note.BendPoints.Count; i++)
                                    {
                                        var point = new BendPoint();
                                        BendPoint.CopyTo(note.BendPoints[i], point);
                                        note2.AddBendPoint(point);
                                    }
                                }

                                #endregion
                            }

                            #endregion
                        }

                        #endregion
                    }

                    #endregion

                }
                #endregion
            }

            #endregion

            score2.Finish();
            return score2;
        }
    }
}
