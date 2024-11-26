﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FamiStudio
{
    public class ExportDialog
    {
        enum ExportFormat
        {
            WavMp3,
            Video,
            Nsf,
            Rom,
            Midi,
            VGM,
            Text,
            FamiTracker,
            FamiStudioMusic,
            FamiStudioSfx,
            FamiTone2Music,
            FamiTone2Sfx,
            Share,
            MobileVideoPreview,
            Max
        };

        LocalizedString[] ExportFormatNames = new LocalizedString[(int)ExportFormat.Max];

        string[] ExportIcons =
        {
            "ExportWav",
            "ExportVideo",
            "ExportNsf",
            "ExportRom",
            "ExportMIDI",
            "ExportVGM",
            "ExportText",
            "ExportFamiTracker",
            "ExportFamiStudioEngine",
            "ExportFamiStudioEngine",
            "ExportFamiTone2",
            "ExportFamiTone2",
            "ExportShare",
            "ExportVideo",
        };

        private Project project;
        private MultiPropertyDialog dialog;
        private uint lastProjectCrc;
        private string lastExportFilename;
        private FamiStudio app;
#if FAMISTUDIO_ANDROID
        private AndroidPreviewEncoder mobileVideoPreviewEncoder;
        private Task mobileVideoPreviewTask;
#endif

        private bool canExportToRom         = true;
        private bool canExportToFamiTracker = true;
        private bool canExportToFamiTone2   = true;
        private bool canExportToSoundEngine = true;
        private bool canExportToVideo       = true;

        public delegate void EmptyDelegate();
        public event EmptyDelegate Exporting;

        #region Localization

        // Title
        LocalizedString Title;
        LocalizedString Verb;

        // Export formats (for result message)
        LocalizedString FormatAudioMessage;
        LocalizedString FormatVideoMessage;
        LocalizedString FormatNsfMessage;
        LocalizedString FormatRomMessage;
        LocalizedString FormatFdsMessage;
        LocalizedString FormatMidiMessage;
        LocalizedString FormatFamiStudioTextMessage;
        LocalizedString FormatFamiTrackerMessage;
        LocalizedString FormatVgmMessage;
        LocalizedString FormatAssemblyMessage;

        // Export results
        LocalizedString SucessMessage;
        LocalizedString FailedMessage;

        // General tooltips
        LocalizedString SingleSongTooltip;
        LocalizedString SongListTooltip;
        LocalizedString MachineTooltip;

        // General labels
        LocalizedString SongLabel;
        LocalizedString SongsLabel;

        // WAV/MP3 tooltips           
        LocalizedString WavFormatTooltip;
        LocalizedString SampleRateTooltip;
        LocalizedString AudioBitRateTooltip;
        LocalizedString LoopModeTooltip;
        LocalizedString LoopCountTooltip;
        LocalizedString DurationTooltip;
        LocalizedString DelayTooltip;
        LocalizedString SeperateFilesTooltip;
        LocalizedString SeperateIntroTooltip;
        LocalizedString StereoTooltip;
        LocalizedString ChannelGridTooltip;
        LocalizedString ChannelGridTooltipVid;

        // WAV/MP3 labels
        LocalizedString FormatLabel;
        LocalizedString SampleRateLabel;
        LocalizedString BitRateLabel;
        LocalizedString ModeLabel;
        LocalizedString DurationSecLabel;
        LocalizedString SeparateChannelFilesLabel;
        LocalizedString SeparateIntroFileLabel;
        LocalizedString StereoLabel;
        LocalizedString ChannelsLabel;
        LocalizedString LoopNTimesOption;
        LocalizedString DurationOption;

        // Video tooltips
        LocalizedString VideoModeTooltip;
        LocalizedString VideoResTooltip;
        LocalizedString FpsTooltip;
        LocalizedString VideoBitRateTooltip;
        LocalizedString OscWindowTooltip;
        LocalizedString OscColumnsTooltip;
        LocalizedString OscThicknessTooltip;
        LocalizedString PianoRollNoteWidthTooltip;
        LocalizedString PianoRollZoomTooltip;
        LocalizedString PianoRollNumRowsTooltip;
        LocalizedString PianoRollPerspectiveTooltip;
        LocalizedString VideoOverlayRegistersTooltip;
        LocalizedString MobileExportVideoMessage;
        LocalizedString MobileVideoPreviewTooltip;

        // Video labels
        LocalizedString VideoModeLabel;
        LocalizedString ResolutionLabel;
        LocalizedString FrameRateLabel;
        LocalizedString AudioBitRateLabel;
        LocalizedString VideoBitRateLabel;
        LocalizedString LoopCountLabel;
        LocalizedString AudioDelayMsLabel;
        LocalizedString OscilloscopeWindowLabel;
        LocalizedString RequireFFMpegLabel;
        LocalizedString PianoRollNoteWidthLabel;
        LocalizedString PianoRollZoomLabel;
        LocalizedString PianoRollNumRowsLabel;
        LocalizedString PianoRollPerspectiveLabel;
        LocalizedString OscColumnsLabel;
        LocalizedString OscThicknessLabel;
        LocalizedString OscColorLabel;
        LocalizedString ExportingVideoLabel;
        LocalizedString VideoOverlayRegistersLabel;
        LocalizedString MobileVideoPreviewLabel;
        LocalizedString PreviewLabel;

        // Video grid
        LocalizedString ChannelColumn;
        LocalizedString PanColumn;
        LocalizedString TransposeColumn;
        LocalizedString TriggerColumn;
        LocalizedString EmulationOption;
        LocalizedString PeakSpeedOption;

        // NSF tooltips                    
        LocalizedString NsfFormatTooltip;

        // NSF labels
        LocalizedString NameLabel;
        LocalizedString ArtistLabel;
        LocalizedString CopyrightLabel;

        // ROM/FDS tooltips           
        LocalizedString RomFdsFormatTooltip;

        // ROM/FDS labels
        LocalizedString TypeLabel;
        LocalizedString RomMultipleExpansionsLabel;

        // MIDI tooltips
        LocalizedString MidiVelocityTooltip;
        LocalizedString MidiPitchTooltip;
        LocalizedString MidiPitchRangeTooltip;
        LocalizedString MidiInstrumentTooltip;
        LocalizedString MidiInstGridTooltip;

        // MIDI labels
        LocalizedString ExportVolumeAsVelocityLabel;
        LocalizedString ExportSlideAsPitchWheelLabel;
        LocalizedString PitchWheelRangeLabel;
        LocalizedString InstrumentModeLabel;
        LocalizedString InstrumentsLabels;

        // FamiStudio Text tooltips
        LocalizedString DeleteUnusedTooltip;

        // FamiStudio Text labels
        LocalizedString DeleteUnusedDataLabel;

        // Music ASM tooltips
        LocalizedString FT2AssemblyTooltip;
        LocalizedString FT2SepFilesTooltip;
        LocalizedString FT2SepFilesFmtTooltip;
        LocalizedString FT2DmcFmtTooltip;
        LocalizedString FT2SongListTooltip;
        LocalizedString FT2SfxSongListTooltip;

        // Music ASM labels
        LocalizedString FamiTone2ExpLabel;
        LocalizedString SoundEngineMultExpLabel;
        LocalizedString SeparateFilesLabel;
        LocalizedString SongNamePatternLabel;
        LocalizedString DmcNamePatternLabel;
        LocalizedString GenerateSongListIncludeLabel;

        // Share tooltips
        LocalizedString ShareTooltip;

        // Share labels
        LocalizedString SharingModeLabel;
        LocalizedString CopyToStorageOption;
        LocalizedString ShareOption;

        // FamiTracker Text labels
        LocalizedString FamiTrackerMultipleExpLabel;

        // SFX ASM labels
        LocalizedString GenerateSfxInclude;

        // VGM tooltips
        LocalizedString TrackTitleEnglishTooltip;
        LocalizedString GameNameEnglishTooltip;
        LocalizedString SystemNameEnglishTooltip;
        LocalizedString ComposerEnglishTooltip;
        LocalizedString ReleaseDateTooltip;
        LocalizedString VGMByTooltip;
        LocalizedString SmoothLoopingTooltip;

        // VGM labels
        LocalizedString TrackTitleEnglishLabel;
        // LocalizedString TrackNameOriginalLabel;
        LocalizedString GameNameEnglishLabel;
        // LocalizedString GameNameOriginalLabel;
        LocalizedString SystemEnglishLabel;
        // LocalizedString SystemOriginalLabel;
        LocalizedString ComposerEnglishLabel;
        // LocalizedString ComposerOriginalLabel;
        LocalizedString ReleaseDateLabel;
        LocalizedString VGMByLabel;
        LocalizedString NotesLabel;
        LocalizedString SmoothLoopingLabel;
        LocalizedString VGMUnsupportedExpLabel;
        #endregion

        public unsafe ExportDialog(FamiStudioWindow win)
        {
            Localization.Localize(this);

            dialog = new MultiPropertyDialog(win, Title, 640, 200);
            dialog.SetVerb(Verb);
            app = win.FamiStudio;
            project = app.Project;

            for (int i = 0; i < (int)ExportFormat.Max; i++)
            {
                var format = (ExportFormat)i;
                var page = dialog.AddPropertyPage(ExportFormatNames[i], ExportIcons[i]);
                CreatePropertyPage(page, format);
            }

            // Hide a few formats we don't care about on mobile.
            dialog.SetPageVisible((int)ExportFormat.Midi, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.Text, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.FamiTracker, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.FamiStudioMusic, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.FamiStudioSfx, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.FamiTone2Music, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.FamiTone2Sfx, Platform.IsDesktop);
            dialog.SetPageVisible((int)ExportFormat.Share, Platform.IsMobile);
            dialog.SetPageVisible((int)ExportFormat.MobileVideoPreview, false);
            dialog.SetPageBackPage((int)ExportFormat.MobileVideoPreview, (int)ExportFormat.Video);
            dialog.AddPageCustomVerb((int)ExportFormat.Video, PreviewLabel);
            dialog.PageChanging += Dialog_PageChanging;
            dialog.CustomVerbActivated += Dialog_CustomVerbActivated;
            dialog.AppSuspended += Dialog_AppSuspended;

            if (Platform.IsDesktop)
                UpdateMidiInstrumentMapping();
        }

        private void Dialog_AppSuspended()
        {
            // Abort video preview of app is suspended.
            if (dialog.SelectedIndex == (int)ExportFormat.MobileVideoPreview)
            {
                dialog.SwitchToPage((int)ExportFormat.Video);
            }
        }

        private void Dialog_PageChanging(int oldPage, int newPage)
        {
        #if FAMISTUDIO_ANDROID
            if (oldPage == (int)ExportFormat.MobileVideoPreview && mobileVideoPreviewTask != null)
            {
                mobileVideoPreviewEncoder.Abort();
                mobileVideoPreviewEncoder = null;
                mobileVideoPreviewTask.Wait();
                mobileVideoPreviewTask.Dispose();
                mobileVideoPreviewTask = null;
                Log.ClearLogOutput();
            }
        #endif
        }

        private void Dialog_CustomVerbActivated()
        {
        #if FAMISTUDIO_ANDROID
            Debug.Assert(mobileVideoPreviewEncoder == null && mobileVideoPreviewTask == null);
            dialog.SwitchToPage((int)ExportFormat.MobileVideoPreview);
            mobileVideoPreviewEncoder = new AndroidPreviewEncoder(dialog.GetPropertyPage((int)ExportFormat.MobileVideoPreview), 0);
            Exporting?.Invoke(); // Needed to stop the song.
            Log.SetLogOutput(mobileVideoPreviewEncoder);
            mobileVideoPreviewTask = Task.Factory.StartNew(() =>
            {
                LaunchVideoEncoding(null, true, mobileVideoPreviewEncoder);
            }, TaskCreationOptions.LongRunning);
        #endif
        }

        private string[] GetSongNames()
        {
            var names = new string[project.Songs.Count];
            for (var i = 0; i < project.Songs.Count; i++)
                names[i] = project.Songs[i].Name;
            return names;
        }

        private string[] GetChannelNames()
        {
            var channelTypes = project.GetActiveChannelList();
            var channelNames = new string[channelTypes.Length];
            for (int i = 0; i < channelTypes.Length; i++)
            {
                channelNames[i] = ChannelType.GetLocalizedNameWithExpansion(channelTypes[i]);
            }

            return channelNames;
        }

        private bool[] GetDefaultActiveChannels()
        {
            // Find all channels used by the project.
            var anyChannelActive = false;
            var channelActives = new bool[project.GetActiveChannelCount()];

            foreach (var song in project.Songs)
            {
                for (int i = 0; i < song.Channels.Length; i++)
                {
                    var channel = song.Channels[i];
                    if (channel.Patterns.Count > 0)
                    {
                        anyChannelActive = true;
                        channelActives[i] = true;
                    }
                }
            }

            if (!anyChannelActive)
                return null;

            return channelActives;
        }

        private object[,] GetDefaultChannelsGridData(bool tranpose, bool trigger, Song song)
        {
            // Find all channels used by the project.
            var anyChannelActive = false;
            var channelActives = new bool[project.GetActiveChannelCount()];
            var songs = song != null ? new [] { song } : project.Songs.ToArray();

            foreach (var s in songs)
            {
                for (int i = 0; i < s.Channels.Length; i++)
                {
                    var channel = s.Channels[i];
                    if (channel.Patterns.Count > 0)
                    {
                        anyChannelActive = true;
                        channelActives[i] = true;
                    }
                }
            }

            var channelTypes = project.GetActiveChannelList();
            var data = new object[channelTypes.Length, trigger ? 5 : 3];

            for (var i = 0; i < channelTypes.Length; i++)
            {
                var j = 0;
                data[i, j++] = !anyChannelActive || channelActives[i];
                data[i, j++] = ChannelType.GetLocalizedNameWithExpansion(channelTypes[i]);
                data[i, j++] = 50;
                if (tranpose) data[i, j++] = 0;
                if (trigger)  data[i, j++] = channelTypes[i] != ChannelType.Dpcm && channelTypes[i] != ChannelType.Noise && !ChannelType.IsEPSMRythmChannel(channelTypes[i]) ? EmulationOption.Value : PeakSpeedOption.Value;
            }

            return data;
        }

        private PropertyPage CreatePropertyPage(PropertyPage page, ExportFormat format)
        {
            var songNames = GetSongNames();

            switch (format)
            {
                case ExportFormat.WavMp3:
                    page.AddDropDownList(SongLabel.Colon, songNames, app.SelectedSong.Name, SingleSongTooltip); // 0
                    page.AddDropDownList(FormatLabel.Colon, AudioFormatType.Names, AudioFormatType.Names[0], WavFormatTooltip); // 1
                    page.AddDropDownList(SampleRateLabel.Colon, new[] { "11025", "22050", "44100", "48000" }, "44100", SampleRateTooltip); // 2
                    page.AddDropDownList(BitRateLabel.Colon, new[] { "96", "112", "128", "160", "192", "224", "256" }, "192", AudioBitRateTooltip); // 3
                    page.AddDropDownList(ModeLabel.Colon, new string[] { LoopNTimesOption, DurationOption }, LoopNTimesOption, LoopModeTooltip); // 4
                    page.AddNumericUpDown(LoopCountLabel.Colon, 1, 1, 10, 1, LoopCountTooltip); // 5
                    page.AddNumericUpDown(DurationSecLabel.Colon, 120, 1, 1000, 1, DurationTooltip); // 6
                    page.AddNumericUpDown(AudioDelayMsLabel.Colon, 0, 0, 100, 1, DelayTooltip); // 7
                    page.AddCheckBox(SeparateChannelFilesLabel.Colon, false, SeperateFilesTooltip); // 8
                    page.AddCheckBox(SeparateIntroFileLabel.Colon, false, SeperateIntroTooltip); // 9
                    page.AddCheckBox(StereoLabel.Colon, project.OutputsStereoAudio, StereoTooltip); // 10
                    page.AddGrid(ChannelsLabel, new[] { 
                        new ColumnDesc("", 0.0f, ColumnType.CheckBox), 
                        new ColumnDesc(ChannelColumn, 0.4f), 
                        new ColumnDesc(PanColumn, 0.6f, 0, 100, (o) => FormattableString.Invariant($"{o} %")) 
                    }, GetDefaultChannelsGridData(false, false, app.SelectedSong), 7, ChannelGridTooltip); // 11
                    page.SetPropertyEnabled( 3, false);
                    page.SetPropertyEnabled( 6, false);
                    page.SetPropertyVisible( 8, Platform.IsDesktop); // No separate files on mobile.
                    page.SetPropertyVisible( 9, Platform.IsDesktop); // No separate intro on mobile.
                    page.SetPropertyEnabled(10, !project.OutputsStereoAudio); // Force stereo for EPSM.
                    page.SetColumnEnabled(11, 2, project.OutputsStereoAudio);
                    page.PropertyChanged += WavMp3_PropertyChanged;
                    page.PropertyClicked += WavMp3_PropertyClicked;
                    break;
                case ExportFormat.Video:
                    if (Platform.CanExportToVideo)
                    {
                        var gridColumns = new List<ColumnDesc>();

                        page.AddDropDownList(VideoModeLabel.Colon, Localization.ToStringArray(VideoMode.LocalizedNames), VideoMode.LocalizedNames[0], VideoModeTooltip); // 0
                        page.AddDropDownList(SongLabel.Colon, songNames, app.SelectedSong.Name, SingleSongTooltip); // 1
                        page.AddDropDownList(ResolutionLabel.Colon, Localization.ToStringArray(VideoResolution.LocalizedNames), VideoResolution.LocalizedNames[0], VideoResTooltip); // 2
                        page.AddDropDownList(FrameRateLabel.Colon, new[] { "50/60 FPS", "25/30 FPS" }, "50/60 FPS", FpsTooltip); // 3
                        page.AddDropDownList(AudioBitRateLabel.Colon, new[] { "64", "96", "112", "128", "160", "192", "224", "256", "320" }, "192", AudioBitRateTooltip); // 4
                        page.AddDropDownList(VideoBitRateLabel.Colon, new[] { "250", "500", "750", "1000", "1500", "2000", "3000", "4000", "5000", "8000", "10000", "20000", "30000" }, "8000", VideoBitRateTooltip); // 5
                        page.AddNumericUpDown(LoopCountLabel.Colon, 1, 1, 8, 1, LoopCountTooltip); // 6
                        page.AddNumericUpDown(AudioDelayMsLabel.Colon, 0, 0, 100, 1, DelayTooltip); // 7
                        page.AddNumericUpDown(OscColumnsLabel.Colon, 1, 1, 5, 1, OscColumnsTooltip); // 8
                        page.AddNumericUpDown(OscilloscopeWindowLabel.Colon, 2, 1, 4, 1, OscWindowTooltip); // 9
                        page.AddNumericUpDown(OscThicknessLabel.Colon, 2, 2, 10, 2, OscThicknessTooltip); // 10
                        page.AddDropDownList(OscColorLabel.Colon, Localization.ToStringArray(OscilloscopeColorType.LocalizedNames), OscilloscopeColorType.LocalizedNames[OscilloscopeColorType.Instruments]); // 11
                        page.AddDropDownList(PianoRollNoteWidthLabel.Colon, new[] { "Auto", "50%", "75%", "100%", "125%", "150%", "175%", "200%" }, "Auto", PianoRollNoteWidthTooltip); // 12
                        page.AddDropDownList(PianoRollZoomLabel.Colon, new[] { "6.25%", "12.5%", "25%", "50%", "100%", "200%", "400%", "800%" }, project.UsesFamiTrackerTempo ? "100%" : "25%", PianoRollZoomTooltip); // 13
                        page.AddNumericUpDown(PianoRollNumRowsLabel.Colon, 1, 1, 16, 1, PianoRollNumRowsTooltip); // 14
                        page.AddDropDownList(PianoRollPerspectiveLabel.Colon, new[] { "0°", "30°", "45°", "60°", "75°" }, "60°", PianoRollPerspectiveTooltip); // 15
                        page.AddCheckBox(VideoOverlayRegistersLabel.Colon, false, VideoOverlayRegistersTooltip); // 16
                        page.AddCheckBox(StereoLabel.Colon, project.OutputsStereoAudio, StereoTooltip); // 17
                        page.AddGrid(ChannelsLabel, new[] {
                            new ColumnDesc("", 0.0f, ColumnType.CheckBox),
                            new ColumnDesc(ChannelColumn, 0.3f),
                            new ColumnDesc(PanColumn, 0.2f, 0, 100, (o) => FormattableString.Invariant($"{o} %")),
                            new ColumnDesc(TransposeColumn, 0.2f, -8, 8),
                            new ColumnDesc(TriggerColumn, 0.2f, new string[] { EmulationOption, PeakSpeedOption })
                        }, GetDefaultChannelsGridData(true, true, app.SelectedSong), project.GetActiveChannelCount() + 1, ChannelGridTooltipVid); // 18
                        page.AddButton(null, PreviewLabel); // 19
                        page.SetPropertyEnabled(12, false);
                        page.SetPropertyEnabled(13, false);
                        page.SetPropertyEnabled(14, false);
                        page.SetPropertyEnabled(15, false);
                        page.SetPropertyEnabled(17, !project.OutputsStereoAudio); // Force stereo for EPSM.
                        page.SetPropertyVisible(19, Platform.IsDesktop);
                        page.SetColumnEnabled(18, 2, project.OutputsStereoAudio);
                        page.SetColumnEnabled(18, 3, false);
                        page.SetScrolling(500);
                        page.PropertyChanged += VideoPage_PropertyChanged;
                        page.PropertyClicked += VideoPage_PropertyClicked;
                    }
                    else
                    {
                        page.AddLabel(null, RequireFFMpegLabel, true);
                        canExportToVideo = false;
                    }
                    break;
                case ExportFormat.Nsf:
                    page.AddTextBox(NameLabel.Colon, project.Name, 31); // 0
                    page.AddTextBox(ArtistLabel.Colon, project.Author, 31); // 1
                    page.AddTextBox(CopyrightLabel.Colon, project.Copyright, 31); // 2
                    page.AddDropDownList(FormatLabel.Colon, new[] { "NSF", "NSFe" }, "NSF", NsfFormatTooltip); // 3
                    page.AddDropDownList(ModeLabel.Colon, Localization.ToStringArray(MachineType.LocalizedNames), MachineType.LocalizedNames[project.PalMode ? MachineType.PAL : MachineType.NTSC], MachineTooltip); // 4
                    page.AddCheckBoxList(Platform.IsDesktop ? null : SongsLabel, songNames, null, SongListTooltip, 12); // 5
#if DEBUG
                    page.AddDropDownList("Engine :", FamiToneKernel.Names, FamiToneKernel.Names[FamiToneKernel.FamiStudio]); // 6
#endif
                    page.SetPropertyEnabled(4, !project.UsesAnyExpansionAudio);
                    break;
                case ExportFormat.Rom:
                    if (!project.UsesMultipleExpansionAudios)
                    {
                        page.AddDropDownList(TypeLabel.Colon, new[] { "NES ROM", "FDS Disk" }, project.UsesFdsExpansion ? "FDS Disk" : "NES ROM", RomFdsFormatTooltip); // 0
                        page.AddTextBox(NameLabel.Colon, project.Name.Substring(0, Math.Min(28, project.Name.Length)), 28); // 1
                        page.AddTextBox(ArtistLabel.Colon, project.Author.Substring(0, Math.Min(28, project.Author.Length)), 28); // 2
                        page.AddDropDownList(ModeLabel.Colon, new[] { "NTSC", "PAL" }, project.PalMode ? "PAL" : "NTSC", MachineTooltip); // 3
                        page.AddCheckBoxList(Platform.IsDesktop ? null : SongsLabel, songNames, null, SongListTooltip, 12); // 4
                        page.SetPropertyEnabled(0, project.UsesFdsExpansion);
                        page.SetPropertyEnabled(3, !project.UsesAnyExpansionAudio);
                    }
                    else
                    {
                        page.AddLabel(null, RomMultipleExpansionsLabel, true);
                        canExportToRom = false;
                    }
                    break;
                case ExportFormat.Midi:
                    page.AddDropDownList(SongLabel.Colon, songNames, app.SelectedSong.Name, SingleSongTooltip); // 0
                    page.AddCheckBox(ExportVolumeAsVelocityLabel.Colon, true, MidiVelocityTooltip); // 1
                    page.AddCheckBox(ExportSlideAsPitchWheelLabel.Colon, true, MidiPitchTooltip); // 2
                    page.AddNumericUpDown(PitchWheelRangeLabel.Colon, 24, 1, 24, 1, MidiPitchRangeTooltip); // 3
                    page.AddDropDownList(InstrumentModeLabel.Colon, Localization.ToStringArray(MidiExportInstrumentMode.LocalizedNames), MidiExportInstrumentMode.LocalizedNames[0], MidiInstrumentTooltip); // 4
                    page.AddGrid(InstrumentsLabels, new[] { new ColumnDesc("", 0.4f), new ColumnDesc("", 0.6f, MidiFileReader.MidiInstrumentNames) }, null, 14, MidiInstGridTooltip); // 5
                    page.PropertyChanged += Midi_PropertyChanged;
                    break;
                case ExportFormat.Text:
                    page.AddCheckBoxList(null, songNames, null, SongListTooltip, 12); // 0
                    page.AddCheckBox(DeleteUnusedDataLabel.Colon, false, DeleteUnusedTooltip); // 1
                    break;
                case ExportFormat.FamiTracker:
                    if (!project.UsesMultipleExpansionAudios)
                    {
                        page.AddCheckBoxList(null, songNames, null, SongListTooltip, 12); // 0
                        canExportToFamiTracker = true;
                    }
                    else
                    {
                        page.AddLabel(null, FamiTrackerMultipleExpLabel, true);
                        canExportToFamiTracker = false;
                    }
                    break;
                case ExportFormat.FamiTone2Music:
                case ExportFormat.FamiStudioMusic:
                    if (format == ExportFormat.FamiTone2Music && project.UsesAnyExpansionAudio)
                    {
                        page.AddLabel(null, FamiTone2ExpLabel, true);
                        canExportToFamiTone2 = false;
                    }
                    else if (format == ExportFormat.FamiStudioMusic && project.UsesMultipleExpansionAudios)
                    {
                        page.AddLabel(null, SoundEngineMultExpLabel, true);
                        canExportToSoundEngine = false;
                    }
                    else
                    {
                        page.AddDropDownList(FormatLabel.Colon, AssemblyFormat.Names, AssemblyFormat.Names[AssemblyFormat.CA65], FT2AssemblyTooltip); // 0
                        page.AddCheckBox(SeparateFilesLabel.Colon, false, FT2SepFilesTooltip); // 1
                        page.AddTextBox(SongNamePatternLabel.Colon, "{project}_{song}", 0, false, FT2SepFilesFmtTooltip); // 2
                        page.AddTextBox(DmcNamePatternLabel.Colon, "{project}", 0, false, FT2DmcFmtTooltip); // 3
                        page.AddCheckBox(GenerateSongListIncludeLabel.Colon, false, FT2SongListTooltip); // 4
                        page.AddCheckBoxList(null, songNames, null, SongListTooltip, 12); // 5
                        page.SetPropertyEnabled(2, false);
                        page.SetPropertyEnabled(3, false);
                        page.PropertyChanged += SoundEngine_PropertyChanged;
                    }
                    break;
                case ExportFormat.FamiTone2Sfx:
                case ExportFormat.FamiStudioSfx:
                    page.AddDropDownList(FormatLabel.Colon, AssemblyFormat.Names, AssemblyFormat.Names[AssemblyFormat.CA65], FT2AssemblyTooltip); // 0
                    page.AddDropDownList(ModeLabel.Colon, Localization.ToStringArray(MachineType.LocalizedNames), MachineType.LocalizedNames[project.PalMode ? MachineType.PAL : MachineType.NTSC], MachineTooltip); // 1
                    page.AddCheckBox(GenerateSfxInclude.Colon, false, FT2SfxSongListTooltip); // 2
                    page.AddCheckBoxList(null, songNames, null, SongListTooltip, 12); // 3
                    break;
                case ExportFormat.VGM:
                    int VGMWarnID;
                    const int unsupportedExpansionMask = ExpansionType.Vrc6Mask | ExpansionType.Mmc5Mask | ExpansionType.N163Mask;
                    if (Platform.IsMobile)
                        VGMWarnID = page.AddLabel(VGMUnsupportedExpLabel.Format(ExpansionType.GetStringForMask(project.ExpansionAudioMask & unsupportedExpansionMask)), null, true); // 0
                    int VGMSongSelect = page.AddDropDownList(SongLabel.Colon, songNames, app.SelectedSong.Name, SongListTooltip); // 0/1
                    page.AddTextBox(TrackTitleEnglishLabel.Colon, page.GetPropertyValue<string>(0), 0, false, TrackTitleEnglishTooltip); // 1/2
                    page.AddTextBox(GameNameEnglishLabel.Colon, project.Name, 0, false, GameNameEnglishTooltip); // 2/3
                    page.AddTextBox(SystemEnglishLabel.Colon,
                    (project.PalMode ? "PAL NES" : "NTSC NES/Famicom") +
                    (project.UsesVrc7Expansion ? $" + {ExpansionType.GetLocalizedName(ExpansionType.Vrc7)}" : "") +
                    (project.UsesFdsExpansion ? $" + {ExpansionType.GetLocalizedName(ExpansionType.Fds)}" : "") +
                    (project.UsesS5BExpansion ? $" + {ExpansionType.GetLocalizedName(ExpansionType.S5B)}" : "") +
                    (project.UsesEPSMExpansion ? $" + {ExpansionType.GetLocalizedName(ExpansionType.EPSM)}" : ""),
                    0, false, SystemNameEnglishTooltip); // 3/4
                    page.AddTextBox(ComposerEnglishLabel.Colon, project.Author, 0, false, ComposerEnglishTooltip); // 4/5
                    page.AddTextBox(ReleaseDateLabel.Colon, DateTime.Now.ToString("yyyy\\/MM\\/dd"), 0, false, ReleaseDateTooltip); // 5/6
                    page.AddTextBox(VGMByLabel.Colon, "FamiStudio Export", 0, false, VGMByTooltip); // 6/7
                    page.AddTextBox(NotesLabel.Colon, project.Copyright, 0); // 7/8
                    page.AddCheckBox(SmoothLoopingLabel.Colon, true, SmoothLoopingTooltip); // 8/9
                    if (Platform.IsDesktop)
                        VGMWarnID = page.AddLabel(null, VGMUnsupportedExpLabel.Format(ExpansionType.GetStringForMask(project.ExpansionAudioMask & unsupportedExpansionMask)), true); // 9
                    page.SetPropertyVisible(VGMWarnID, (project.ExpansionAudioMask & unsupportedExpansionMask) != 0);  // Unsupported expansions
                    page.SetPropertyEnabled(VGMSongSelect+8, project.GetSong(page.GetPropertyValue<string>(VGMSongSelect)).LoopPoint >= 0);
                    page.PropertyChanged += VGM_PropertyChanged;
                    break;
                case ExportFormat.Share:
                    page.AddRadioButtonList(SharingModeLabel.Colon, new string[] { CopyToStorageOption, ShareOption }, 0, ShareTooltip);
                    break;
                case ExportFormat.MobileVideoPreview:
                    if (Platform.IsMobile)
                        page.AddImageBox(MobileVideoPreviewLabel, MobileVideoPreviewTooltip); // 0
                    break;
            }

            page.Build();

            return page;
        }

        private void VideoPage_PropertyClicked(PropertyPage props, ClickType click, int propIdx, int rowIdx, int colIdx)
        {
        #if !FAMISTUDIO_ANDROID
            if (propIdx == 19)
            {
                var resolutionIdx = props.GetSelectedIndex(2);
                var halfFrameRate = props.GetSelectedIndex(3) == 1;
                var previewResX = VideoResolution.ResolutionX[resolutionIdx];
                var previewResY = VideoResolution.ResolutionY[resolutionIdx];
                var previewDialog = new VideoPreviewDialog(dialog.ParentWindow, previewResX, previewResY, (project.PalMode ? NesApu.FpsPAL : NesApu.FpsNTSC) * (halfFrameRate ? 0.5f : 1.0f));

                Exporting?.Invoke(); // Needed to stop the song.

                previewDialog.ShowDialogNonModal();
                Log.SetLogOutput(previewDialog);
                LaunchVideoEncoding(null, true, previewDialog);
                Log.ClearLogOutput();
            }
        #endif
        }

        private void VideoPage_PropertyChanged(PropertyPage props, int propIdx, int rowIdx, int colIdx, object value)
        {
            if (propIdx == 0) // Video mode
            {
                var newMode = props.GetSelectedIndex(propIdx);
                props.SetPropertyEnabled(8,  newMode == VideoMode.Oscilloscope);
                props.SetPropertyEnabled(12, newMode != VideoMode.Oscilloscope);
                props.SetPropertyEnabled(13, newMode != VideoMode.Oscilloscope);
                props.SetPropertyEnabled(14, newMode == VideoMode.PianoRollSeparateChannels);
                props.SetPropertyEnabled(15, newMode != VideoMode.Oscilloscope);
                props.SetColumnEnabled(18, 3, newMode == VideoMode.PianoRollUnified);
            }
            else if (propIdx == 1) // Song
            {
                props.UpdateGrid(18, GetDefaultChannelsGridData(true, true, project.Songs[props.GetSelectedIndex(1)]));
            }
            else if (propIdx == 17) // Stereo
            {
                props.SetColumnEnabled(18, 2, (bool)value);
            }
        }

        private void WavMp3_PropertyChanged(PropertyPage props, int propIdx, int rowIdx, int colIdx, object value)
        {
            if (propIdx == 0)
            {
                props.UpdateGrid(11, GetDefaultChannelsGridData(false, false, project.Songs[props.GetSelectedIndex(0)]));
            }
            else if (propIdx == 1)
            {
                props.SetPropertyEnabled(3, (string)value != "WAV");
            }
            else if (propIdx == 4)
            {
                props.SetPropertyEnabled(5, (string)value != DurationOption);
                props.SetPropertyEnabled(6, (string)value == DurationOption);
            }
            else if (propIdx == 8)
            {
                var separateChannels = (bool)value;

                props.SetPropertyEnabled(7, !separateChannels);
                props.SetPropertyEnabled(10, !separateChannels && !project.OutputsStereoAudio);

                if (separateChannels)
                {
                    props.SetPropertyValue(7, 0);
                    props.SetPropertyValue(10, project.OutputsStereoAudio);
                }

                props.SetColumnEnabled(11, 2, props.GetPropertyValue<bool>(10) && !separateChannels);
            }
            else if (propIdx == 10)
            {
                props.SetColumnEnabled(11, 2, (bool)value);
            }
        }

        private void WavMp3_PropertyClicked(PropertyPage props, ClickType click, int propIdx, int rowIdx, int colIdx)
        {
            if (propIdx == 10 && click == ClickType.Right && colIdx == 2)
            {
                props.UpdateGrid(propIdx, rowIdx, colIdx, 50);
            }
        }

        private int[] GetSongIds(bool[] selectedSongs)
        {
            var songIds = new List<int>();

            for (int i = 0; i < selectedSongs.Length; i++)
            {
                if (selectedSongs[i])
                    songIds.Add(project.Songs[i].Id);
            }

            return songIds.ToArray();
        }

        private void ShowExportResultToast(string format, bool success = true)
        {
            var msg = success ? SucessMessage : FailedMessage;
            Platform.ShowToast(app.Window, msg.Format(format));
        }

        private void ExportWavMp3()
        {
            var props = dialog.GetPropertyPage((int)ExportFormat.WavMp3);
            var format = props.GetSelectedIndex(1);
            
            Action<string> ExportWavMp3Action = (filename) =>
            {
                if (filename != null)
                {
                    var songName = props.GetPropertyValue<string>(0);
                    var sampleRate = Convert.ToInt32(props.GetPropertyValue<string>(2), CultureInfo.InvariantCulture);
                    var bitrate = Convert.ToInt32(props.GetPropertyValue<string>(3), CultureInfo.InvariantCulture);
                    var loopCount = props.GetPropertyValue<string>(4) != DurationOption ? props.GetPropertyValue<int>(5) : -1;
                    var duration = props.GetPropertyValue<string>(4) == DurationOption ? props.GetPropertyValue<int>(6) : -1;
                    var delay = props.GetPropertyValue<int>(7);
                    var separateFiles = props.GetPropertyValue<bool>(8);
                    var separateIntro = props.GetPropertyValue<bool>(9);
                    var stereo = props.GetPropertyValue<bool>(10) && (!separateFiles || project.OutputsStereoAudio);
                    var song = project.GetSong(songName);

                    var channelCount = project.GetActiveChannelCount();
                    var channelMask = 0L;
                    var pan = new float[channelCount]; 

                    for (int i = 0; i < channelCount; i++)
                    {
                        if (props.GetPropertyValue<bool>(11, i, 0))
                            channelMask |= (1L << i);

                        pan[i] = props.GetPropertyValue<int>(11, i, 2) / 100.0f;
                    }

                    AudioExportUtils.Save(song, filename, sampleRate, loopCount, duration, channelMask, separateFiles, separateIntro, stereo, pan, delay, Platform.IsMobile || project.UsesEPSMExpansion,
                         (samples, samplesChannels, fn) =>
                         {
                             switch (format)
                             {
                                 case AudioFormatType.Mp3:
                                     Mp3File.Save(samples, fn, sampleRate, bitrate, samplesChannels);
                                     break;
                                 case AudioFormatType.Wav:
                                     WaveFile.Save(samples, fn, sampleRate, samplesChannels);
                                     break;
                                 case AudioFormatType.Vorbis:
                                     VorbisFile.Save(samples, fn, sampleRate, bitrate, samplesChannels);
                                     break;
                             }
                         });

                    lastExportFilename = filename;
                }
            };


            if (Platform.IsMobile)
            {
                var songName = props.GetPropertyValue<string>(0);
                Platform.StartMobileSaveFileOperationAsync(AudioFormatType.MimeTypes[format], $"{songName}", (f) =>
                {
                    new Thread(() =>
                    {
                        app.BeginLogTask(true, "Exporting Audio", "Exporting long songs with lots of channels may take a while.");

                        ExportWavMp3Action(f);
                        
                        Platform.FinishMobileSaveFileOperationAsync(true, () =>
                        {
                            var aborted = Log.ShouldAbortOperation;
                            app.EndLogTask();
                            ShowExportResultToast(FormatAudioMessage, !aborted);
                        });
                    }).Start();
                });
            }
            else
            {
                var filename = (string)null;

                if (lastExportFilename != null)
                {
                    filename = lastExportFilename;
                }
                else
                {
                    filename = Platform.ShowSaveFileDialog(
                        $"Export {AudioFormatType.Names[format]} File",
                        $"{AudioFormatType.Names[format]} Audio File (*.{AudioFormatType.Extensions[format]})|*.{AudioFormatType.Extensions[format]}",
                        ref Settings.LastExportFolder);
                }

                if (filename != null)
                {
                    ExportWavMp3Action(filename);
                    ShowExportResultToast(FormatAudioMessage);
                }
            }
        }

        private bool LaunchVideoEncoding(string filename, bool preview, IVideoEncoder forcedEncoder = null)
        {
            var props = dialog.GetPropertyPage((int)ExportFormat.Video);

            var videoMode = props.GetSelectedIndex(0);
            var resolutionIdx = props.GetSelectedIndex(2);
            var channelCount = project.GetActiveChannelCount();

            var settings = new VideoExportSettings();
            settings.Filename = filename;
            settings.Project = project;
            settings.VideoMode = videoMode;
            settings.SongId = project.GetSong(props.GetPropertyValue<string>(1)).Id;
            settings.ResX = VideoResolution.ResolutionX[resolutionIdx];
            settings.ResY = VideoResolution.ResolutionY[resolutionIdx];
            settings.Downsample = Platform.IsMobile && preview ? 2 : 1;
            settings.HalfFrameRate = props.GetSelectedIndex(3) == 1;
            settings.AudioBitRate = Convert.ToInt32(props.GetPropertyValue<string>(4), CultureInfo.InvariantCulture);
            settings.VideoBitRate = Convert.ToInt32(props.GetPropertyValue<string>(5), CultureInfo.InvariantCulture);
            settings.LoopCount = props.GetPropertyValue<int>(6);
            settings.AudioDelay = props.GetPropertyValue<int>(7);
            settings.OscNumColumns = props.GetPropertyValue<int>(8);
            settings.OscWindow = props.GetPropertyValue<int>(9);
            settings.OscLineThickness = props.GetPropertyValue<int>(10);
            settings.OscColorMode = props.GetSelectedIndex(11);
            settings.PianoRollNoteWidth = Utils.ParseIntWithTrailingGarbage(props.GetPropertyValue<string>(12)) / 100.0f;
            settings.PianoRollZoom = (float)Math.Pow(2.0, props.GetSelectedIndex(13) - 3);
            settings.PianoRollNumRows = props.GetPropertyValue<int>(14);
            settings.PianoRollPerspective = Utils.ParseIntWithTrailingGarbage(props.GetPropertyValue<string>(15));
            settings.ShowRegisters = props.GetPropertyValue<bool>(16);
            settings.Stereo = preview ? project.OutputsStereoAudio : props.GetPropertyValue<bool>(17);
            settings.ChannelPan = new float[channelCount];
            settings.ChannelTranspose = new int[channelCount];
            settings.EmuTriggers = new bool[channelCount];
            settings.ChannelMask = 0;
            settings.PreviewMode = preview;
            settings.Encoder = forcedEncoder != null ? forcedEncoder : Platform.CreateVideoEncoder();

            for (int i = 0; i < channelCount; i++)
            {
                if (props.GetPropertyValue<bool>(18, i, 0))
                    settings.ChannelMask |= (1L << i);

                settings.ChannelPan[i] = preview ? 0.5f : props.GetPropertyValue<int>(18, i, 2) / 100.0f;
                settings.ChannelTranspose[i] = props.GetPropertyValue<int>(18, i, 3) * 12;
                settings.EmuTriggers[i] = props.GetPropertyValue<string>(18, i, 4) == EmulationOption;
            }

            if (videoMode == VideoMode.Oscilloscope)
            {
                return new VideoFileOscilloscope().Save(settings);
            }
            else
            {
                return new VideoFilePianoRoll().Save(settings);
            }
        }

        private void ExportVideo()
        {
            if (!canExportToVideo)
                return;

            var props = dialog.GetPropertyPage((int)ExportFormat.Video);

            Func<string, bool> ExportVideoAction = (filename) =>
            {
                if (filename != null)
                {
                    lastExportFilename = filename;
                    return LaunchVideoEncoding(filename, false);

                }
                else
                {
                    return false;
                }
            };

            if (Platform.IsMobile)
            {
                var songName = props.GetPropertyValue<string>(0);
                Platform.StartMobileSaveFileOperationAsync("video/mp4", $"{songName}", (f) =>
                {
                    new Thread(() =>
                    {
                        app.BeginLogTask(true, ExportingVideoLabel, MobileExportVideoMessage);
                        
                        var success = ExportVideoAction(f);

                        Platform.FinishMobileSaveFileOperationAsync(success, () =>
                        {
                            app.EndLogTask();
                            ShowExportResultToast(FormatVideoMessage, success);
                        });

                    }).Start();
                });
            }
            else
            {
                var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export Video File", "MP4 Video File (*.mp4)|*.mp4", ref Settings.LastExportFolder);
                ExportVideoAction(filename);
                ShowExportResultToast(FormatVideoMessage);
            }
        }

        private void ExportNsf()
        {
            var props = dialog.GetPropertyPage((int)ExportFormat.Nsf);
            var nsfe = props.GetSelectedIndex(3) > 0;
            var extension = nsfe ? "nsfe" : "nsf";

            Action<string> ExportNsfAction = (filename) =>
            {
                if (filename != null)
                {
                    var mode = MachineType.GetValueForName(props.GetPropertyValue<string>(4));
#if DEBUG
                    var kernel = FamiToneKernel.GetValueForName(props.GetPropertyValue<string>(6));
#else
                    var kernel = FamiToneKernel.FamiStudio;
#endif

                    new NsfFile().Save(project, kernel, filename,
                        GetSongIds(props.GetPropertyValue<bool[]>(5)),
                        props.GetPropertyValue<string>(0),
                        props.GetPropertyValue<string>(1),
                        props.GetPropertyValue<string>(2),
                        mode,
                        nsfe);

                    lastExportFilename = filename;
                }
            };

            if (Platform.IsMobile)
            {
                Platform.StartMobileSaveFileOperationAsync("*/*", $"{project.Name}.{extension}", (f) =>
                {
                    ExportNsfAction(f);
                    Platform.FinishMobileSaveFileOperationAsync(true, () => { ShowExportResultToast(FormatNsfMessage); });
                });
            }
            else
            {
                var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export NSF File", $"Nintendo Sound Files (*.{extension})|*.{extension}", ref Settings.LastExportFolder);
                if (filename != null)
                {
                    ExportNsfAction(filename);
                    ShowExportResultToast(FormatNsfMessage);
                }
            }
        }

        private void ExportRom()
        {
            if (!canExportToRom)
                return;

            var props = dialog.GetPropertyPage((int)ExportFormat.Rom);
            var songIds = GetSongIds(props.GetPropertyValue<bool[]>(4));

            if (songIds.Length > RomFile.RomMaxSongs)
            {
                Platform.MessageBoxAsync(dialog.ParentWindow, $"Please select {RomFile.RomMaxSongs} songs or less.", "ROM Export", MessageBoxButtons.OK);
                return;
            }

            if (props.GetPropertyValue<string>(0) == "NES ROM")
            {
                Action<string> ExportRomAction = (filename) =>
                {
                    if (filename != null)
                    {
                        var rom = new RomFile();
                        rom.Save(
                            project, filename, songIds,
                            props.GetPropertyValue<string>(1),
                            props.GetPropertyValue<string>(2),
                            props.GetPropertyValue<string>(3) == "PAL");

                        lastExportFilename = filename;
                    }
                };

                if (Platform.IsMobile)
                {
                    Platform.StartMobileSaveFileOperationAsync("*/*", $"{project.Name}.nes", (f) =>
                    {
                        ExportRomAction(f);
                        Platform.FinishMobileSaveFileOperationAsync(true, () => { ShowExportResultToast(FormatRomMessage); });
                    });
                }
                else
                {
                    var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export ROM File", "NES ROM (*.nes)|*.nes", ref Settings.LastExportFolder);
                    if (filename != null)
                    {
                        ExportRomAction(filename);
                        ShowExportResultToast(FormatRomMessage);
                    }
                }
            }
            else
            {
                Action<string> ExportFdsAction = (filename) =>
                {
                    if (filename != null)
                    {
                        var fds = new FdsFile();
                        fds.Save(
                            project, filename, songIds,
                            props.GetPropertyValue<string>(1),
                            props.GetPropertyValue<string>(2));

                        lastExportFilename = filename;
                    }
                };

                if (Platform.IsMobile)
                {
                    Platform.StartMobileSaveFileOperationAsync("*/*", $"{project.Name}.fds", (f) =>
                    {
                        ExportFdsAction(f);
                        Platform.FinishMobileSaveFileOperationAsync(true, () => { ShowExportResultToast(FormatFdsMessage); });
                    });
                }
                else
                {
                    var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export Famicom Disk", "FDS Disk (*.fds)|*.fds", ref Settings.LastExportFolder);
                    if (filename != null)
                    {
                        ExportFdsAction(filename);
                        ShowExportResultToast(FormatFdsMessage);
                    }
                }
            }
        }

        private void ExportShare()
        {
            var props = dialog.GetPropertyPage((int)ExportFormat.Share);
            var share = props.GetSelectedIndex(0) == 1;

            var filename = !string.IsNullOrEmpty(app.Project.Filename) ? Path.GetFileName(app.Project.Filename) : $"{project.Name}.fms";

            if (share)
            {
                filename = Platform.GetShareFilename(filename);
                app.SaveProjectCopy(filename);
                Platform.StartShareFileAsync(filename, () => 
                {
                    Platform.ShowToast(dialog.ParentWindow, "Sharing Successful!");
                });
            }
            else
            {
                Platform.StartMobileSaveFileOperationAsync("*/*", filename, (f) =>
                {
                    app.SaveProjectCopy(f);
                    Platform.FinishMobileSaveFileOperationAsync(true, () => { Platform.ShowToast(dialog.ParentWindow, "Sharing Successful!"); });
                });
            }
        }

        private void SoundEngine_PropertyChanged(PropertyPage props, int propIdx, int rowIdx, int colIdx, object value)
        {
            if (propIdx == 1)
            {
                props.SetPropertyEnabled(2, (bool)value);
                props.SetPropertyEnabled(3, (bool)value);
            }
        }

        private void Midi_PropertyChanged(PropertyPage props, int propIdx, int rowIdx, int colIdx, object value)
        {
            if (propIdx == 4)
                UpdateMidiInstrumentMapping();
        }

        private void UpdateMidiInstrumentMapping()
        {
            var props = dialog.GetPropertyPage((int)ExportFormat.Midi);
            var mode  = props.GetSelectedIndex(4);
            var data  = (object[,])null;
            var cols = new[] { "", "MIDI Instrument" };

            if (mode == MidiExportInstrumentMode.Instrument)
            {
                data = new object[project.Instruments.Count, 2];
                for (int i = 0; i < project.Instruments.Count; i++)
                {
                    var inst = project.Instruments[i];
                    data[i, 0] = inst.NameWithExpansion;
                    data[i, 1] = MidiFileReader.MidiInstrumentNames[0];
                }

                cols[0] = "FamiStudio Instrument";
            }
            else
            {
                var firstSong = project.Songs[0];

                data = new object[firstSong.Channels.Length, 2];
                for (int i = 0; i < firstSong.Channels.Length; i++)
                {
                    data[i, 0] = firstSong.Channels[i].Name;
                    data[i, 1] = MidiFileReader.MidiInstrumentNames[0];
                }

                cols[0] = "NES Channel";
            }

            props.UpdateGrid(5, data, cols);
        }

        private void ExportMidi()
        {
            var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export MIDI File", "MIDI Files (*.mid)|*.mid", ref Settings.LastExportFolder);
            if (filename != null)
            {
                var props = dialog.GetPropertyPage((int)ExportFormat.Midi);
                var songName = props.GetPropertyValue<string>(0);
                var velocity = props.GetPropertyValue<bool>(1);
                var slideNotes = props.GetPropertyValue<bool>(2);
                var pitchRange = props.GetPropertyValue<int>(3);
                var instrumentMode = props.GetSelectedIndex(4);
                var song = project.GetSong(songName);
                var instrumentMapping = new int[instrumentMode == MidiExportInstrumentMode.Channel ? song.Channels.Length : song.Project.Instruments.Count];

                for (int i = 0; i < instrumentMapping.Length; i++)
                    instrumentMapping[i] = Array.IndexOf(MidiFileReader.MidiInstrumentNames, props.GetPropertyValue<string>(5, i, 1));

                new MidiFileWriter().Save(project, filename, song.Id, instrumentMode, instrumentMapping, velocity, slideNotes, pitchRange);

                ShowExportResultToast(FormatMidiMessage);

                lastExportFilename = filename;
            }
        }

        private void ExportText()
        {
            var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export FamiStudio Text File", "FamiStudio Text Export (*.txt)|*.txt", ref Settings.LastExportFolder);
            if (filename != null)
            {
                var props = dialog.GetPropertyPage((int)ExportFormat.Text);
                var deleteUnusedData = props.GetPropertyValue<bool>(1);
                new FamistudioTextFile().Save(project, filename, GetSongIds(props.GetPropertyValue<bool[]>(0)), deleteUnusedData);
                ShowExportResultToast(FormatFamiStudioTextMessage);
                lastExportFilename = filename;
            }
        }

        private void VGM_PropertyChanged(PropertyPage props, int propIdx, int rowIdx, int colIdx, object value)
        {
            int songSelect = Platform.IsDesktop ? 0 : 1;
            if (propIdx == songSelect)
            {
                props.SetPropertyValue(songSelect+1, (string)value);
                props.SetPropertyEnabled(songSelect+8, project.GetSong((string)value).LoopPoint >= 0);
            }
        }

        private void ExportVGM()
        {
            var props = dialog.GetPropertyPage((int)ExportFormat.VGM);
            string trackTitle;
            if (Platform.IsMobile)
            {
                trackTitle = props.GetPropertyValue<string>(2);
                Platform.StartMobileSaveFileOperationAsync("*/*", $"{trackTitle}.vgm", (f) =>
                {
                    VgmFile.Save(project.GetSong(props.GetPropertyValue<string>(1)), (f), trackTitle,
                        props.GetPropertyValue<string>(3),
                        props.GetPropertyValue<string>(4),
                        props.GetPropertyValue<string>(5),
                        props.GetPropertyValue<string>(6),
                        props.GetPropertyValue<string>(7),
                        props.GetPropertyValue<string>(8),
                        props.GetPropertyValue<bool>(9));
                    Platform.FinishMobileSaveFileOperationAsync(true, () => { ShowExportResultToast(FormatVgmMessage); });
                });
            }
            else
            {
                trackTitle = props.GetPropertyValue<string>(1);
                var filename = lastExportFilename ?? Platform.ShowSaveFileDialog($"Export {FormatVgmMessage}", $"{FormatVgmMessage} File (*.vgm)|*.vgm", ref Settings.LastExportFolder);
                if (filename != null)
                {
                    VgmFile.Save(project.GetSong(props.GetPropertyValue<string>(0)), filename, trackTitle,
                        props.GetPropertyValue<string>(2),
                        props.GetPropertyValue<string>(3),
                        props.GetPropertyValue<string>(4),
                        props.GetPropertyValue<string>(5),
                        props.GetPropertyValue<string>(6),
                        props.GetPropertyValue<string>(7),
                        props.GetPropertyValue<bool>(8));
                    lastExportFilename = filename;
                    ShowExportResultToast(FormatVgmMessage);
                }
            }
            
        }
		
        private void ExportFamiTracker()
        {
            if (!canExportToFamiTracker)
                return;

            var props = dialog.GetPropertyPage((int)ExportFormat.FamiTracker);

            var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog("Export FamiTracker Text File", "FamiTracker Text Format (*.txt)|*.txt", ref Settings.LastExportFolder);
            if (filename != null)
            {
                new FamitrackerTextFile().Save(project, filename, GetSongIds(props.GetPropertyValue<bool[]>(0)));
                ShowExportResultToast(FormatFamiTrackerMessage);
                lastExportFilename = filename;
            }
        }

        private void ExportFamiTone2Music(bool famiStudio)
        {
            if ((famiStudio && !canExportToSoundEngine) || (!famiStudio && !canExportToFamiTone2))
                return;

            var props = dialog.GetPropertyPage(famiStudio ? (int)ExportFormat.FamiStudioMusic : (int)ExportFormat.FamiTone2Music);

            var separate = props.GetPropertyValue<bool>(1);
            var songIds = GetSongIds(props.GetPropertyValue<bool[]>(5));
            var kernel = famiStudio ? FamiToneKernel.FamiStudio : FamiToneKernel.FamiTone2;
            var exportFormat = AssemblyFormat.GetValueForName(props.GetPropertyValue<string>(0));
            var ext = exportFormat == AssemblyFormat.CA65 ? "s" : "asm";
            var songNamePattern = props.GetPropertyValue<string>(2);
            var dpcmNamePattern = props.GetPropertyValue<string>(3);
            var generateInclude = props.GetPropertyValue<bool>(4);

            if (separate)
            {
                var folder = lastExportFilename != null ? lastExportFilename : Platform.ShowBrowseFolderDialog("Select the export folder", ref Settings.LastExportFolder);

                if (folder != null)
                {
                    if (!project.EnsureSongAssemblyNamesAreUnique())
                    {
                        ShowExportResultToast(FormatAssemblyMessage, false);
                        return;
                    }

                    var success = true;

                    foreach (var songId in songIds)
                    {
                        var song = project.GetSong(songId);
                        var formattedSongName = songNamePattern.Replace("{project}", project.Name).Replace("{song}", song.Name);
                        var formattedDpcmName = dpcmNamePattern.Replace("{project}", project.Name).Replace("{song}", song.Name);
                        var songFilename = Path.Combine(folder, Utils.MakeNiceAsmName(formattedSongName) + "." + ext);
                        var dpcmFilename = Path.Combine(folder, Utils.MakeNiceAsmName(formattedDpcmName) + ".dmc");
                        var includeFilename = generateInclude ? Path.ChangeExtension(songFilename, null) + "_songlist.inc" : null;

                        Log.LogMessage(LogSeverity.Info, $"Exporting song '{song.Name}' as separate assembly files.");

                        FamitoneMusicFile f = new FamitoneMusicFile(kernel, true);
                        success = success && f.Save(project, new int[] { songId }, exportFormat, -1, true, songFilename, dpcmFilename, includeFilename, MachineType.Dual);
                    }

                    lastExportFilename = folder;
                    ShowExportResultToast(FormatAssemblyMessage, success);
                }
            }
            else
            {
                var engineName = famiStudio ? "FamiStudio" : "FamiTone2";
                var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog($"Export {engineName} Assembly Code", $"{engineName} Assembly File (*.{ext})|*.{ext}", ref Settings.LastExportFolder);
                if (filename != null)
                {
                    var includeFilename = generateInclude ? Path.ChangeExtension(filename, null) + "_songlist.inc" : null;

                    // LOCTODO : Still some strings to localize 
                    Log.LogMessage(LogSeverity.Info, $"Exporting all songs to a single assembly file.");

                    FamitoneMusicFile f = new FamitoneMusicFile(kernel, true);
                    var success = f.Save(project, songIds, exportFormat, -1, false, filename, Path.ChangeExtension(filename, ".dmc"), includeFilename, MachineType.Dual);

                    lastExportFilename = filename;
                    ShowExportResultToast(FormatAssemblyMessage, success);
                }
            }
        }

        private void ExportFamiTone2Sfx(bool famiStudio)
        {
            var props = dialog.GetPropertyPage(famiStudio ? (int)ExportFormat.FamiStudioSfx : (int)ExportFormat.FamiTone2Sfx);
            var exportFormat = AssemblyFormat.GetValueForName(props.GetPropertyValue<string>(0));
            var ext = exportFormat == AssemblyFormat.CA65 ? "s" : "asm";
            var mode = MachineType.GetValueForName(props.GetPropertyValue<string>(1));
            var engineName = famiStudio ? "FamiStudio" : "FamiTone2";
            var generateInclude = props.GetPropertyValue<bool>(2);
            var songIds = GetSongIds(props.GetPropertyValue<bool[]>(3));

            var filename = lastExportFilename != null ? lastExportFilename : Platform.ShowSaveFileDialog($"Export {engineName} Code", $"{engineName} Assembly File (*.{ext})|*.{ext}", ref Settings.LastExportFolder);
            if (filename != null)
            {
                var includeFilename = generateInclude ? Path.ChangeExtension(filename, null) + "_sfxlist.inc" : null;

                FamitoneSoundEffectFile f = new FamitoneSoundEffectFile();
                var result = f.Save(project, songIds, exportFormat, mode, famiStudio ? FamiToneKernel.FamiStudio : FamiToneKernel.FamiTone2, filename, includeFilename);
                ShowExportResultToast(FormatAssemblyMessage, result);
                lastExportFilename = filename;
            }
        }

        private uint ComputeProjectCrc(Project project)
        {
            // Only hashing fields that would have an impact on the generated UI.
            uint crc = CRC32.Compute(project.ExpansionAudioMask);
            crc = CRC32.Compute(project.ExpansionNumN163Channels, crc);

            foreach (var song in project.Songs)
            {
                crc = CRC32.Compute(song.Id,   crc);
                crc = CRC32.Compute(song.Name, crc);
            }

            foreach (var inst in project.Instruments)
            {
                crc = CRC32.Compute(inst.Id,   crc);
                crc = CRC32.Compute(inst.Name, crc);
            }

            return crc;
        }

        public bool HasAnyPreviousExport => lastProjectCrc != 0 && !string.IsNullOrEmpty(lastExportFilename);

        public bool IsProjectStillCompatible(Project project)
        {
            if (project != this.project)
                return false;

            return lastProjectCrc == ComputeProjectCrc(project);
        }
        
        public void Export(bool repeatLast)
        {
            if (Platform.IsDesktop)
                app.BeginLogTask(true);

            var selectedFormat = (ExportFormat)dialog.SelectedIndex;

            Exporting?.Invoke();

            if (!repeatLast)
                lastExportFilename = null;

            switch (selectedFormat)
            {
                case ExportFormat.WavMp3: ExportWavMp3(); break;
                case ExportFormat.Video: ExportVideo(); break;
                case ExportFormat.Nsf: ExportNsf(); break;
                case ExportFormat.Rom: ExportRom(); break;
                case ExportFormat.Midi: ExportMidi(); break;
                case ExportFormat.Text: ExportText(); break;
                case ExportFormat.FamiTracker: ExportFamiTracker(); break;
                case ExportFormat.FamiTone2Music: ExportFamiTone2Music(false); break;
                case ExportFormat.FamiStudioMusic: ExportFamiTone2Music(true); break;
                case ExportFormat.FamiTone2Sfx: ExportFamiTone2Sfx(false); break;
                case ExportFormat.FamiStudioSfx: ExportFamiTone2Sfx(true); break;
                case ExportFormat.VGM: ExportVGM(); break;
                case ExportFormat.Share: ExportShare(); break;
            }

            if (Platform.IsDesktop)
                app.EndLogTask();
        }

        public void ShowDialogAsync()
        {
            dialog.ShowDialogAsync((r) =>
            {
                if (r == DialogResult.OK)
                {
                    Export(false);
                }

                lastProjectCrc = ComputeProjectCrc(project);
            });
        }
    }
}
