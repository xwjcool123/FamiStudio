﻿using System;
using System.Diagnostics;

namespace FamiStudio
{
    class MidiImportDialog
    {
        private PropertyDialog dialog;
        private string[] trackNames;
        private string filename;

        private MidiFileReader.MidiSource[] channelSources = new MidiFileReader.MidiSource[5]
        {
            new MidiFileReader.MidiSource() { index = 0 },
            new MidiFileReader.MidiSource() { index = 1 },
            new MidiFileReader.MidiSource() { index = 2 },
            new MidiFileReader.MidiSource() { index = 9 },
            new MidiFileReader.MidiSource() { type = MidiSourceType.None }
        };

        #region Localization

        LocalizedString MidiImportTitle;
        LocalizedString PolyphonyBehaviorLabel;
        LocalizedString MeasurePerPatternLabel;
        LocalizedString MeasurePerPatternTooltip;
        LocalizedString ImportVelocityAsVolume;
        LocalizedString CreatePALProject;
        LocalizedString ExpansionsLabel;
        LocalizedString ChannelMappingLabel;
        LocalizedString ChannelsLabel;
        LocalizedString NESChannelColumn;
        LocalizedString MIDISourceColumn;
        LocalizedString Channel10KeysColumn;
        LocalizedString MIDIDisclaimerLabel;
        LocalizedString SourceNoneOption;
        LocalizedString SourceChannelPrefix;
        LocalizedString SourceTrackPrefix;
        LocalizedString NotApplicableValue;
        LocalizedString AllKeysValue;
        LocalizedString FilteredKeysValue;

        // Channel 10 keys dialog
        LocalizedString MIDISourceTitle;
        LocalizedString Channel10KeysLabel;
        LocalizedString SelectAllLabel;
        LocalizedString SelectNoneLabel;

        #endregion

        public MidiImportDialog(FamiStudioWindow win, string file)
        {
            Localization.Localize(this);

            filename = file;
            trackNames = new MidiFileReader().GetTrackNames(file);

            if (trackNames != null)
            {
                var expNames = new string[ExpansionType.Count - 1];
                for (int i = ExpansionType.Start; i <= ExpansionType.End; i++)
                    expNames[i - ExpansionType.Start] = ExpansionType.LocalizedNames[i];

                dialog = new PropertyDialog(win, MidiImportTitle, 500);
                dialog.Properties.AddDropDownList(PolyphonyBehaviorLabel.Colon, Localization.ToStringArray(MidiPolyphonyBehavior.LocalizedNames), MidiPolyphonyBehavior.LocalizedNames[0]); // 0
                dialog.Properties.AddNumericUpDown(MeasurePerPatternLabel.Colon, 2, 1, 4, 1, MeasurePerPatternTooltip); // 1
                dialog.Properties.AddCheckBox(ImportVelocityAsVolume.Colon, true); // 2
                dialog.Properties.AddCheckBox(CreatePALProject.Colon, false); // 3
                dialog.Properties.AddCheckBoxList(ExpansionsLabel.Colon, expNames, new bool[expNames.Length], null); // 4
                dialog.Properties.AddLabel(null, ChannelMappingLabel.Colon); // 5
                dialog.Properties.AddGrid(ChannelsLabel, new[] { new ColumnDesc(NESChannelColumn, 0.25f), new ColumnDesc(MIDISourceColumn, 0.45f, GetSourceNames()), new ColumnDesc(Channel10KeysColumn, 0.3f, ColumnType.Button) }, null); // 6
                dialog.Properties.AddLabel(null, MIDIDisclaimerLabel, true);
                dialog.Properties.Build();
                dialog.Properties.PropertyChanged += Properties_PropertyChanged;
                dialog.Properties.PropertyClicked += Properties_PropertyClicked;

                UpdateListView();
            }
        }

        private int GetExpansionMask(bool[] values)
        {
            var mask = 0;
            for (int i = ExpansionType.Start; i <= ExpansionType.End; i++)
            {
                if (values[i - ExpansionType.Start])
                    mask |= ExpansionType.GetMaskFromValue(i);
            }
            return mask;
        }

        private void Properties_PropertyChanged(PropertyPage props, int propIdx, int rowIdx, int colIdx, object value)
        {
            if (propIdx == 4)
            {
                var expansionMask = GetExpansionMask(props.GetPropertyValue<bool[]>(4));
                var newChannelCount = Channel.GetChannelCountForExpansionMask(expansionMask, 8);
                var oldChannelCount = channelSources.Length;

                var maxChannelIndex = 2;
                for (int i = 0; i < oldChannelCount; i++)
                {
                    if (channelSources[i].type == MidiSourceType.Channel && channelSources[i].index != 9)
                        maxChannelIndex = Math.Max(maxChannelIndex, channelSources[i].index);
                }

                Array.Resize(ref channelSources, newChannelCount);

                for (int i = oldChannelCount; i < newChannelCount; i++)
                {
                    maxChannelIndex = Math.Min(maxChannelIndex + 1, 15);
                    if (maxChannelIndex == 9) maxChannelIndex++;
                    channelSources[i] = new MidiFileReader.MidiSource() { index = maxChannelIndex };
                }

                UpdateListView();

                bool allowPal = expansionMask == ExpansionType.NoneMask;
                dialog.Properties.SetPropertyEnabled(3, allowPal);
                if (!allowPal)
                    dialog.Properties.SetPropertyValue(3, false);
            }
            else if (propIdx == 6)
            {
                Debug.Assert(colIdx == 1);

                var src = channelSources[rowIdx];
                var str = (string)value;

                if (str.StartsWith(SourceTrackPrefix))
                {
                    src.type  = MidiSourceType.Track;
                    src.index = Utils.ParseIntAmongAllSortsOfGarbage(str) - 1;
                }
                else if (str.StartsWith(SourceChannelPrefix))
                {
                    src.type  = MidiSourceType.Channel;
                    src.index = Utils.ParseIntAmongAllSortsOfGarbage(str) - 1;
                }
                else
                {
                    src.type  = MidiSourceType.None;
                    src.index = 0;
                }

                UpdateListView();
            }
        }

        private void Properties_PropertyClicked(PropertyPage props, ClickType click, int propIdx, int rowIdx, int colIdx)
        {
            if (click == ClickType.Button && colIdx == 2)
            {
                var src = channelSources[rowIdx];

                if (src.type == MidiSourceType.Channel && src.index == 9)
                {
                    var dlg = new PropertyDialog(dialog.ParentWindow, MIDISourceTitle, 300, true, true);
                    dlg.Properties.AddLabel(null, Channel10KeysLabel.Colon); // 0
                    dlg.Properties.AddCheckBoxList(null, MidiFileReader.MidiDrumKeyNames, GetSelectedChannel10Keys(src)); // 1
                    dlg.Properties.AddButton(null, SelectAllLabel); // 2
                    dlg.Properties.AddButton(null, SelectNoneLabel); // 3
                    dlg.Properties.Build();
                    dlg.Properties.PropertyClicked += MappingProperties_PropertyClicked;

                    dlg.ShowDialogAsync((r) =>
                    {
                        if (r == DialogResult.OK)
                        {
                            var keysBool = dlg.Properties.GetPropertyValue<bool[]>(1);

                            src.keys = 0ul;
                            for (int i = 0; i < keysBool.Length; i++)
                            {
                                if (keysBool[i])
                                    src.keys |= (1ul << i);
                            }

                            UpdateListView();
                        }
                    });
                }
                else
                {
                    Platform.Beep();
                }
            }
        }

        private void MappingProperties_PropertyClicked(PropertyPage props, ClickType click, int propIdx, int rowIdx, int colIdx)
        {
            if (click == ClickType.Button && (propIdx == 2 || propIdx == 3))
            {
                var keys = new bool[MidiFileReader.MidiDrumKeyNames.Length];

                if (propIdx == 2)
                {
                    for (int i = 0; i < keys.Length; i++)
                        keys[i] = true;
                }

                props.UpdateCheckBoxList(1, keys);
            }
        }

        private string GetChannelName(int idx)
        {
            return $"{SourceChannelPrefix} {idx + 1}";
        }

        private string GetTrackName(int idx)
        {
            var str = $"{SourceTrackPrefix} {idx + 1}";

            if (!string.IsNullOrEmpty(trackNames[idx]))
                str += $" ({trackNames[idx]})";

            return str;
        }

        private string[] GetSourceNames()
        {
            var sourceNames = new string[16 + trackNames.Length + 1];

            sourceNames[0] = SourceNoneOption;
            for (int i = 0; i < 16; i++)
                sourceNames[i + 1] = GetChannelName(i);
            for (int i = 0; i < trackNames.Length; i++)
                sourceNames[i + 17] = GetTrackName(i);

            return sourceNames;
        }

        private bool[] GetSelectedChannel10Keys(MidiFileReader.MidiSource source)
        {
            var keys = new bool[MidiFileReader.MidiDrumKeyNames.Length];

            for (int i = 0; i < MidiFileReader.MidiDrumKeyNames.Length; i++)
                keys[i] = (((1ul << i) & source.keys) != 0);

            return keys;
        }

        public void UpdateListView()
        {
            var expansionMask = GetExpansionMask(dialog.Properties.GetPropertyValue<bool[]>(4));
            var channels = Channel.GetChannelsForExpansionMask(expansionMask, 8);

            Debug.Assert(channelSources.Length == channels.Length);

            var gridData = new object[channels.Length, 3];

            for (int i = 0; i < channels.Length; i++)
            {
                var src = channelSources[i];

                gridData[i, 0] = ChannelType.LocalizedNames[channels[i]].Value;
                gridData[i, 2] = NotApplicableValue.Value;

                if (i >= ChannelType.ExpansionAudioStart)
                    gridData[i, 0] += $" ({ExpansionType.InternalNames[ChannelType.GetExpansionTypeForChannelType(channels[i])]})";

                if (src.type == MidiSourceType.Track)
                {
                    gridData[i, 1] = GetTrackName(src.index);
                }
                else if (src.type == MidiSourceType.Channel)
                {
                    gridData[i, 1] = GetChannelName(src.index);

                    if (src.index == 9)
                        gridData[i, 2] = src.keys == MidiFileReader.AllDrumKeysMask ? AllKeysValue.Value : FilteredKeysValue.Value;
                }
                else
                {
                    gridData[i, 1] = SourceNoneOption.Value;
                }
            }

            dialog.Properties.UpdateGrid(6, gridData);
        }

        public void ShowDialogAsync(FamiStudioWindow parent, Action<Project> action)
        {
            if (dialog != null)
            {
                // This is only ran in desktop and this isnt really async, so its ok.
                dialog.ShowDialogAsync((r) =>
                {
                    if (r == DialogResult.OK)
                    {
                        var expansionMask = GetExpansionMask(dialog.Properties.GetPropertyValue<bool[]>(4));
                        var polyphony = dialog.Properties.GetSelectedIndex(0);
                        var measuresPerPattern = dialog.Properties.GetPropertyValue<int>(1);
                        var velocityAsVolume = dialog.Properties.GetPropertyValue<bool>(2);
                        var pal = expansionMask != ExpansionType.NoneMask ? false : dialog.Properties.GetPropertyValue<bool>(3);

                        var project = new MidiFileReader().Load(filename, expansionMask, pal, channelSources, velocityAsVolume, polyphony, measuresPerPattern);
                        action(project);
                    }
                    else
                    {
                        action(null);
                    }
                });
            }
            else
            {
                action(null);
            }
        }
    }
}
