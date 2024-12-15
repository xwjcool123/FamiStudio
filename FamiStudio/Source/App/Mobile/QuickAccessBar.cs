﻿using System;
using System.Diagnostics;
using System.Collections.Generic;

using Color     = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace FamiStudio
{
    public class QuickAccessBar : Container
    {
        // All of these were calibrated at 1080p and will scale up/down from there.
        const int DefaultNavButtonSize  = 120;
        const int DefaultButtonSize     = 144;
        const int DefaultIconSize       = 96;
        const int DefaultScrollBarSizeX = 16;

        const int DefaultListItemSize      = 120;
        const int DefaultListExtraIconSize = 120;

        private Button buttonSequencer;
        private Button buttonPianoRoll;
        private Button buttonProject;
        private Button buttonChannel;
        private Button buttonInstrument;
        private Button buttonEnvelope;
        private Button buttonArpeggio;
        private Button buttonSnap;
        private Button buttonEffect;
        private Button buttonDPCMEffect;
        private Button buttonDPCMPlay;
        private Button buttonWaveformEffect;
        private List<Button> allButtons = new List<Button>();

        private TouchScrollContainer listContainer;

        Font buttonFont;
        
        // These are only use for popup menu.
        private Button    popupButton;
        private Button    popupButtonNext;
        private Rectangle popupRect;
        private float     popupRatio = 0.0f;
        private bool      popupOpening;
        private bool      popupClosing;

        // Scaled layout variables.
        private int buttonSize;
        private int buttonSizeNav;
        private int listItemSize;
        private int listExtraIconSize;
        private int scrollBarSizeX;
        private float iconScaleFloat = 1.0f;

        public int   LayoutSize  => buttonSize;
        public float ExpandRatio => popupRatio;
        public bool  IsExpanded  => popupRatio > 0.001f;

        public override bool WantsFullScreenViewport => true;

        #region Localization

        // Labels
        LocalizedString SnapToBeatLabel;
        LocalizedString SnapToBeatsLabel;
        LocalizedString SnapEnabledLabel;
        LocalizedString SnapEffectValuesLabel;
        LocalizedString NoneLabel;
        LocalizedString RepeatLabel;
        LocalizedString VolumeLabel;
        LocalizedString RepeatEnvelopeLabel;
        LocalizedString VolumeEnvelopeLabel;
        LocalizedString SnapOffLabel;

        // Context menus
        LocalizedString ReplaceSelectionInstContext;
        LocalizedString ReplaceSelectionArpContext;
        LocalizedString ToggleMuteContext;
        LocalizedString ToggleSoloContext;
        LocalizedString ToggleForceDisplayContext;

        #endregion

        public QuickAccessBar()
        {
            Localization.Localize(this);
            SetTickEnabled(true);
        }

        private Button CreateBarButton(string image, string userData, string text = " ")
        {
            var button = new Button(image);
            button.UserData = userData;
            button.Visible = false;
            button.ImageScale = iconScaleFloat;
            button.Transparent = true;
            button.BottomText = true;
            button.Ellipsis = true;
            button.Margin = 1;
            button.Font = buttonFont;
            button.Text = text;
            button.VibrateOnClick = true;
            button.VibrateOnRightClick = true;
            button.Resize(buttonSize, buttonSize);
            allButtons.Add(button);
            AddControl(button);
            return button;
        }

        private Button CreateListButton(string image, object userData, string text = null)
        {
            var button = new Button(image);
            button.Margin = 2;
            button.UserData = userData;
            button.ImageScale = iconScaleFloat;
            button.Transparent = true;
            button.Text = text;
            return button;
        }

        protected override void OnAddedToContainer()
        {
            var g = ParentWindow.Graphics;
            var screenSize = Platform.GetScreenResolution();
            var scale = Math.Min(screenSize.Width, screenSize.Height) / 1080.0f;
            var refIcon = g.GetTextureAtlasRef("MobileSnapOn");

            buttonFont        = scale > 1.2f ? Fonts.FontSmall : Fonts.FontVerySmall;
            buttonSize        = DpiScaling.ScaleCustom(DefaultButtonSize, scale);
            buttonSizeNav     = DpiScaling.ScaleCustom(DefaultNavButtonSize, scale);
            listItemSize      = DpiScaling.ScaleCustom(DefaultListItemSize, scale);
            listExtraIconSize = DpiScaling.ScaleCustom(DefaultListExtraIconSize, scale);
            scrollBarSizeX    = DpiScaling.ScaleCustom(DefaultScrollBarSizeX, scale);
            iconScaleFloat    = DpiScaling.ScaleCustomFloat(DefaultIconSize / (float)refIcon.ElementSize.Width, scale);

            buttonSequencer = CreateBarButton("Sequencer", "Sequencer");
            buttonSequencer.Click += ButtonSequencer_Click;
            buttonSequencer.DimmedEvent += ButtonSequencer_DimmedEvent;

            buttonPianoRoll = CreateBarButton("PianoRoll", "PianoRoll");
            buttonPianoRoll.Click += ButtonPianoRoll_Click;
            buttonPianoRoll.DimmedEvent += ButtonPianoRoll_DimmedEvent;

            buttonProject = CreateBarButton("ProjectExplorer", "ProjectExplorer");
            buttonProject.Click += ButtonProject_Click;
            buttonProject.DimmedEvent += ButtonProject_DimmedEvent;

            buttonChannel = CreateBarButton(ChannelType.Icons[0], "Channel");
            buttonChannel.Click += ButtonChannel_Click;
            buttonChannel.TextEvent += ButtonChannel_TextEvent;
            buttonChannel.ImageEvent += ButtonChannel_ImageEvent;
            
            buttonInstrument = CreateBarButton(ExpansionType.Icons[0], "Instrument");
            buttonInstrument.Click += ButtonInstrument_Click;
            buttonInstrument.RightClick += ButtonInstrument_RightClick;
            buttonInstrument.TextEvent += ButtonInstrument_TextEvent;
            buttonInstrument.ImageEvent += ButtonInstrument_ImageEvent;
            
            buttonEnvelope = CreateBarButton(EnvelopeType.Icons[0], "Envelope");
            buttonEnvelope.Click += ButtonEnvelope_Click;
            buttonEnvelope.TextEvent += ButtonEnvelope_TextEvent;
            buttonEnvelope.ImageEvent += ButtonEnvelope_ImageEvent;
            
            buttonArpeggio = CreateBarButton("MobileArpeggio", "Arpeggio");
            buttonArpeggio.Click += ButtonArpeggio_Click;
            buttonArpeggio.RightClick += ButtonArpeggio_RightClick;
            buttonArpeggio.TextEvent += ButtonArpeggio_TextEvent;
            buttonArpeggio.ImageEvent += ButtonArpeggio_ImageEvent;

            buttonSnap = CreateBarButton("MobileSnapOn", "Snap");
            buttonSnap.Click += ButtonSnap_Click;
            buttonSnap.TextEvent += ButtonSnap_TextEvent;
            buttonSnap.ImageEvent += ButtonSnap_ImageEvent;
            
            buttonEffect = CreateBarButton("Mobile" + EffectType.Icons[0], "Effect");
            buttonEffect.Click += ButtonEffect_Click;
            buttonEffect.TextEvent += ButtonEffect_TextEvent;
            buttonEffect.ImageEvent += ButtonEffect_ImageEvent;
            
            buttonDPCMEffect = CreateBarButton("Mobile" + EffectType.Icons[Note.EffectVolume], "DPCMEffect");
            buttonDPCMEffect.Click += ButtonDPCMEffect_Click;
            buttonDPCMEffect.TextEvent += ButtonDPCMEffect_TextEvent;
            buttonDPCMEffect.ImageEvent += ButtonDPCMEffect_ImageEvent;
            
            buttonDPCMPlay = CreateBarButton("Play", "DPCMPlay", "Play");
            buttonDPCMPlay.Click += ButtonDPCMPlay_Click;
            buttonDPCMPlay.ImageEvent += ButtonDPCMPlay_ImageEvent;
            
            buttonWaveformEffect = CreateBarButton("MobileEffectRepeat", "WaveformEffect");
            buttonWaveformEffect.Click += ButtonWaveformEffect_Click;
            buttonWaveformEffect.TextEvent += ButtonWaveformEffect_TextEvent;
            buttonWaveformEffect.ImageEvent += ButtonWaveformEffect_ImageEvent;

            listContainer = new TouchScrollContainer();
            listContainer.Visible = false;
            listContainer.Border = true;
            listContainer.SetupClipRegion(true, false);
            AddControl(listContainer);
        }

        private bool ButtonSequencer_DimmedEvent(Control sender, ref int dimming)
        {
            return App.ActiveControl != App.Sequencer;
        }

        private bool ButtonPianoRoll_DimmedEvent(Control sender, ref int dimming)
        {
            return App.ActiveControl != App.PianoRoll;
        }

        private bool ButtonProject_DimmedEvent(Control sender, ref int dimming)
        {
            return App.ActiveControl != App.ProjectExplorer;
        }

        private void ButtonSequencer_Click(Control sender)
        {
            StartClosingList();
            App.SetActiveControl(App.Sequencer);
        }

        private void ButtonPianoRoll_Click(Control sender)
        {
            StartClosingList();
            if (!App.IsEditingChannel)
                App.StartEditChannel(App.SelectedChannelIndex);
            App.SetActiveControl(App.PianoRoll);
        }

        private void ButtonProject_Click(Control sender)
        {
            StartClosingList();
            App.SetActiveControl(App.ProjectExplorer);
        }

        private void ButtonChannel_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonChannel))
                return;

            var channelTypes   = App.Project.GetActiveChannelList();
            var channelButtons = new Button[channelTypes.Length];
            var ghostButtons   = new Button[channelTypes.Length];

            for (int i = 0; i < channelTypes.Length; i++)
            {
                var btn = CreateListButton(ChannelType.Icons[channelTypes[i]], i, ChannelType.GetLocalizedNameWithExpansion(channelTypes[i]));
                btn.TouchClick += Channel_TouchClick;
                btn.TouchDoubleClick += Channel_TouchDoubleClick;
                btn.RightClick += Channel_RightClick;
                btn.DimmedEvent += Channel_DimmedEvent;
                btn.Font = i == App.SelectedChannelIndex ? fonts.FontMediumBold : fonts.FontMedium;
                btn.SetSupportsDoubleClick(true);
                channelButtons[i] = btn;

                var ghost = CreateListButton("GhostSmall", i);
                ghost.DimmedEvent += ChannelGhost_DimmedEvent;
                ghost.Click += ChannelGhost_Click;
                ghost.TouchDoubleClick += Ghost_TouchDoubleClick;
                ghost.SetSupportsDoubleClick(true);
                ghostButtons[i] = ghost;
            }

            StartExpandingList(buttonChannel, channelButtons, ghostButtons, App.SelectedChannelIndex);
        }

        private void Ghost_TouchDoubleClick(Control sender, PointerEventArgs e)
        {
            App.ToggleChannelForceDisplayAll((int)sender.UserData, true);
        }

        private void Channel_TouchDoubleClick(Control sender, PointerEventArgs e)
        {
            if ((sender as Button).ImageRect.Contains(e.Position))
            {
                App.ToggleChannelSolo((int)sender.UserData, true);
            }
        }

        private void Channel_TouchClick(Control sender, PointerEventArgs e)
        {
            if ((sender as Button).ImageRect.Contains(e.Position))
            {
                App.ToggleChannelActive((int)sender.UserData);
            }
            else
            {
                App.SelectedChannelIndex = (int)sender.UserData;
                StartClosingList();
            }
        }

        private void Channel_RightClick(Control sender)
        {
            var idx = (int)sender.UserData;

            App.ShowContextMenuAsync(new[]
            {
                new ContextMenuOption("MenuMute", ToggleMuteContext, () => { App.ToggleChannelActive(idx); MarkDirty(); }),
                new ContextMenuOption("MenuSolo", ToggleSoloContext, () => { App.ToggleChannelSolo(idx); MarkDirty(); }),
                new ContextMenuOption("MenuForceDisplay", ToggleForceDisplayContext, () => { App.ToggleChannelForceDisplay(idx); MarkDirty(); })
            });
        }

        private bool Channel_DimmedEvent(Control sender, ref int dimming)
        {
            return !App.IsChannelActive((int)sender.UserData);
        }

        private void ChannelGhost_Click(Control sender)
        {
            var idx = (int)sender.UserData;
            App.ToggleChannelForceDisplay(idx); 
            MarkDirty();
        }

        private bool ChannelGhost_DimmedEvent(Control sender, ref int dimming)
        {
            return !App.IsChannelForceDisplay((int)sender.UserData);
        }

        private string ButtonChannel_TextEvent(Control sender)
        {
            return App.SelectedChannel.NameWithExpansion;
        }

        private string ButtonChannel_ImageEvent(Control sender, ref Color tint)
        {
            return ChannelType.Icons[App.SelectedChannel.Type];
        }

        private void ButtonInstrument_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonInstrument))
                return;

            var project = App.Project;
            var channel = App.SelectedChannel;
            var instButtons = new List<Button>();
            var selIdx = -1;

            for (int i = 0; i < project.Instruments.Count; i++)
            {
                var inst = project.Instruments[i];

                if (App.SelectedInstrument == inst)
                {
                    selIdx = instButtons.Count;
                }

                var btn = CreateListButton(ExpansionType.Icons[inst.Expansion], inst, inst.Name);
                btn.ForegroundColor = inst.Color;
                btn.Font = inst == App.SelectedInstrument ? fonts.FontMediumBold : fonts.FontMedium;
                btn.Dimmed = !channel.SupportsInstrument(inst);
                btn.Dimming = 92;
                btn.Click += Instrument_Click;
                btn.RightClick += Instrument_RightClick;
                instButtons.Add(btn);
            }

            if (instButtons.Count == 0)
              return;

            StartExpandingList(buttonInstrument, instButtons.ToArray(), null, selIdx, true);
        }

        private void ButtonInstrument_RightClick(Control sender)
        {
            if (App.IsEditingChannel && App.PianoRollHasSelection && App.SelectedChannel.SupportsInstrument(App.SelectedInstrument))
            {
                App.ShowContextMenuAsync(new[]
                {
                    new ContextMenuOption("MenuReplaceSelection", ReplaceSelectionInstContext, () => { App.ReplacePianoRollSelectionInstrument(App.SelectedInstrument); MarkDirty(); })
                });
            }
        }

        private void Instrument_Click(Control sender)
        {
            var inst = sender.UserData as Instrument;
            App.SelectedInstrument = inst;
            StartClosingList();
        }

        private void Instrument_RightClick(Control sender)
        {
            var inst = sender.UserData as Instrument;

            if (App.IsEditingChannel && App.PianoRollHasSelection && App.SelectedChannel.SupportsInstrument(inst))
            {
                App.ShowContextMenuAsync(new[]
                {
                    new ContextMenuOption("MenuReplaceSelection", ReplaceSelectionInstContext, () => { App.ReplacePianoRollSelectionInstrument(inst); MarkDirty(); })
                });
            }
        }

        private string ButtonInstrument_TextEvent(Control sender)
        {
            var inst = App.SelectedInstrument;
            return inst != null ? inst.Name : " ";
        }

        private string ButtonInstrument_ImageEvent(Control sender, ref Color tint)
        {
            var inst = App.SelectedInstrument;
            if (inst != null) tint = inst.Color;
            var exp = inst != null ? inst.Expansion : ExpansionType.None;
            return ExpansionType.Icons[exp];
        }

        private void ButtonEnvelope_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonEnvelope))
                return;

            var inst = App.SelectedInstrument;

            if (inst == null)
                return;

            var selIdx = -1;
            var buttons = new Button[inst.NumVisibleEnvelopes];

            for (int i = 0, j = 0; i < EnvelopeType.Count; i++)
            {
                var env = inst.Envelopes[i];

                if (env != null && inst.IsEnvelopeVisible(i))
                {
                    var btn = CreateListButton(EnvelopeType.Icons[i], i, EnvelopeType.LocalizedNames[i]);
                    btn.Dimmed = env.IsEmpty(i);
                    btn.Click += Envelope_Click;
                    buttons[j] = btn;

                    if (i == App.EditEnvelopeType)
                    {
                        selIdx = j;
                    }

                    j++;
                }
            }

            StartExpandingList(buttonEnvelope, buttons, null, selIdx);
        }

        private void Envelope_Click(Control sender)
        {
            App.StartEditInstrument(App.SelectedInstrument, (int)sender.UserData);
            StartClosingList();
        }

        private string ButtonEnvelope_TextEvent(Control sender)
        {
            var envType = App.EditEnvelopeType;
            return EnvelopeType.LocalizedNames[envType];
        }

        private string ButtonEnvelope_ImageEvent(Control sender, ref Color tint)
        {
            var inst = App.SelectedInstrument;
            var envType = App.EditEnvelopeType;
            if (inst != null) tint = inst.Color;
            return EnvelopeType.Icons[envType];
        }

        private void ButtonArpeggio_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonArpeggio))
                return;

            var project = App.Project;
            var buttons = new Button[project.Arpeggios.Count + (App.IsEditingArpeggio ? 0 : 1)];
            var arpeggios = project.Arpeggios;
            var icon = EnvelopeType.Icons[EnvelopeType.Arpeggio];
            var selIdx = -1;
            var idx = 0;

            if (!App.IsEditingArpeggio)
            {
                var btn = CreateListButton(icon, null, NoneLabel);
                btn.Click += Arpeggio_Click;
                btn.Font = App.SelectedArpeggio == null ? fonts.FontMediumBold : fonts.FontMedium;
                buttons[idx++] = btn;
            }

            for (int i = 0; i < arpeggios.Count; i++)
            {
                var arp = arpeggios[i];
                var selected = arp == App.SelectedArpeggio;
                var btn = CreateListButton(icon, arp, arp.Name);
                btn.Font = selected ? fonts.FontMediumBold : fonts.FontMedium;
                btn.Click += Arpeggio_Click;
                btn.RightClick += Arpeggio_RightClick;
                btn.ForegroundColor = arp.Color;
                buttons[idx++] = btn;
                
                if (selected)
                {
                    selIdx = idx;
                }
            }

            StartExpandingList(buttonArpeggio, buttons, null, selIdx, true);
        }

        private void ButtonArpeggio_RightClick(Control sender)
        {
            if (App.IsEditingChannel && App.PianoRollHasSelection && App.SelectedChannel.SupportsArpeggios)
            {
                App.ShowContextMenuAsync(new[]
                {
                    new ContextMenuOption("MenuReplaceSelection", ReplaceSelectionArpContext, () => { App.ReplacePianoRollSelectionArpeggio(App.SelectedArpeggio); MarkDirty(); })
                });
            }
        }

        private void Arpeggio_RightClick(Control sender)
        {
            var arp = sender.UserData as Arpeggio;

            if (App.IsEditingChannel && App.PianoRollHasSelection && App.SelectedChannel.SupportsArpeggios)
            {
                App.ShowContextMenuAsync(new[]
                {
                    new ContextMenuOption("MenuReplaceSelection", ReplaceSelectionArpContext, () => { App.ReplacePianoRollSelectionArpeggio(arp); MarkDirty(); })
                });
            }
        }

        private void Arpeggio_Click(Control sender)
        {
            App.SelectedArpeggio = sender.UserData as Arpeggio;
            StartClosingList();
        }

        private string ButtonArpeggio_TextEvent(Control sender)
        {
            var arp = App.SelectedArpeggio;
            return arp != null ? arp.Name : NoneLabel;
        }

        private string ButtonArpeggio_ImageEvent(Control sender, ref Color tint)
        {
            var arp = App.SelectedArpeggio;
            if (arp != null) tint = arp.Color;
            return "MobileArpeggio";
        }

        private void ButtonSnap_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonSnap))
                return;

            var buttons = new Button[SnapResolutionType.Max + 3];

            for (int i = 0; i < buttons.Length - 2; i++)
            {
                var img = App.SnapResolution == i ? "MobileRadioOn" : "MobileRadioOff";
                var text = (SnapResolutionType.Factors[i] > 1.0 ? SnapToBeatsLabel : SnapToBeatLabel).Format(SnapResolutionType.Names[i]); 
                var btn = CreateListButton(img, i, text);
                btn.Dimmed = !App.SnapEnabled;
                btn.Font = i == App.SnapResolution ? fonts.FontMediumBold : fonts.FontMedium;
                btn.Click += SnapResolution_Click;
                buttons[i] = btn;
            }

            var snapEffectsButton = CreateListButton(App.SnapEffectEnabled ? "MobileCheckOn" : "MobileCheckOff", null, SnapEffectValuesLabel);
            snapEffectsButton.Dimmed = !App.SnapEnabled;
            snapEffectsButton.Click += SnapEffects_Click;
            buttons[buttons.Length - 2] = snapEffectsButton;

            var snapEnableButton = CreateListButton(App.SnapEnabled ? "MobileCheckOn" : "MobileCheckOff", null, SnapEnabledLabel);
            snapEnableButton.Click += SnapEnable_Click;
            buttons[buttons.Length - 1] = snapEnableButton;

            StartExpandingList(buttonSnap, buttons, null, buttons.Length - 1);
        }

        private void SnapResolution_Click(Control sender)
        {
            App.SnapResolution = (int)sender.UserData;
            App.SnapEnabled = true;
            StartClosingList();
        }

        private void SnapEffects_Click(Control sender)
        {
            App.SnapEffectEnabled = !App.SnapEffectEnabled;
            StartClosingList();
        }

        private void SnapEnable_Click(Control sender)
        {
            App.SnapEnabled = !App.SnapEnabled;
            StartClosingList();
        }

        private string ButtonSnap_TextEvent(Control sender)
        {
            var snapEnabled = App.SnapEnabled;
            return snapEnabled ? SnapResolutionType.Names[App.SnapResolution] + (App.SnapEffectEnabled ? " (FX)" : "") : SnapOffLabel;
        }

        private string ButtonSnap_ImageEvent(Control sender, ref Color tint)
        {
            var snapEnabled = App.SnapEnabled;
            if (App.IsRecording) tint = Theme.DarkRedColor;
            return snapEnabled ? "MobileSnapOn" : "MobileSnapOff";
        }

        private void ButtonEffect_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonEffect))
                return;

            var channel = App.SelectedChannel;
            var selectedEffect = App.SelectedEffect;
            var effectPanelExpanded = App.EffectPanelExpanded;
            var count = 1;

            for (int i = 0; i < Note.EffectCount; i++)
            {
                if (channel.ShouldDisplayEffect(i))
                    count++;
            }

            var buttons = new Button[count];
            var none = CreateListButton("MobileEffectNone", -1, NoneLabel); ; ;
            none.Click += Effect_Click;
            buttons[0] = none;

            for (int i = 0, j = 1; i < Note.EffectCount; i++)
            {
                if (channel.ShouldDisplayEffect(i))
                {
                    var btn = CreateListButton("Mobile" + EffectType.Icons[i], i, EffectType.LocalizedNames[i]);
                    btn.Font = effectPanelExpanded && i == selectedEffect ? fonts.FontMediumBold : fonts.FontMedium;
                    btn.Click += Effect_Click;
                    buttons[j] = btn;
                    j++;
                }
            }

            StartExpandingList(buttonEffect, buttons);
        }

        private void Effect_Click(Control sender)
        {
            var effect = (int)sender.UserData;
            if (effect >= 0)
            {
                App.SelectedEffect = effect;
                App.EffectPanelExpanded = true;
            }
            else
            {
                App.EffectPanelExpanded = false;
            }
            StartClosingList();
        }

        private string ButtonEffect_TextEvent(Control sender)
        {
            var validEffect = App.SelectedEffect >= 0 && App.EffectPanelExpanded;
            return validEffect ? EffectType.LocalizedNames[App.SelectedEffect] : NoneLabel;
        }

        private string ButtonEffect_ImageEvent(Control sender, ref Color tint)
        {
            var validEffect = App.SelectedEffect >= 0 && App.EffectPanelExpanded;
            return validEffect ? "Mobile" + EffectType.Icons[App.SelectedEffect] : "MobileEffectNone";
        }

        private void ButtonDPCMEffect_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonDPCMEffect))
                return;

            var noneButton = CreateListButton("MobileEffectNone", -1, NoneLabel); ;
            noneButton.Click += DPCMEffect_Click;
            var volButton  = CreateListButton("Mobile" + EffectType.Icons[Note.EffectVolume], Note.EffectVolume, VolumeEnvelopeLabel);
            volButton.Click += DPCMEffect_Click;

            var buttons = new Button[2] { noneButton, volButton };
            buttons[App.EffectPanelExpanded ? 1 : 0].Font = fonts.FontMediumBold;

            StartExpandingList(buttonDPCMEffect, buttons);
        }

        private void DPCMEffect_Click(Control sender)
        {
            App.EffectPanelExpanded = (int)sender.UserData >= 0;
            StartClosingList();
        }

        private string ButtonDPCMEffect_TextEvent(Control sender)
        {
            return App.EffectPanelExpanded ? VolumeLabel : NoneLabel;
        }

        private string ButtonDPCMEffect_ImageEvent(Control sender, ref Color tint)
        {
            return App.EffectPanelExpanded ?  "Mobile" + EffectType.Icons[Note.EffectVolume] : "MobileEffectNone";
        }

        private void ButtonDPCMPlay_Click(Control sender)
        {
            App.PreviewDPCMSample(App.EditSample, false);
        }

        private string ButtonDPCMPlay_ImageEvent(Control sender, ref Color tint)
        {
            tint = App.EditSample.Color;
            return "Play";
        }

        private void ButtonWaveformEffect_Click(Control sender)
        {
            if (CheckNeedsClosing(buttonWaveformEffect))
                return;

            var noneButton = CreateListButton("MobileEffectNone", -1, NoneLabel);
            noneButton.Click += WaveformEffect_Click;
            var repeatButton = CreateListButton("MobileEffectRepeat", 0, RepeatEnvelopeLabel);
            repeatButton.Click += WaveformEffect_Click;

            var buttons = new Button[2] { noneButton, repeatButton };
            buttons[App.EffectPanelExpanded ? 1 : 0].Font = fonts.FontMediumBold;

            StartExpandingList(buttonWaveformEffect, buttons);
        }

        private void WaveformEffect_Click(Control sender)
        {
            App.EffectPanelExpanded = (int)sender.UserData >= 0;
            StartClosingList();
        }

        private string ButtonWaveformEffect_TextEvent(Control sender)
        {
            return App.EffectPanelExpanded ? RepeatLabel : NoneLabel;
        }

        private string ButtonWaveformEffect_ImageEvent(Control sender, ref Color tint)
        {
            return App.EffectPanelExpanded ? "MobileEffectRepeat" : "MobileEffectNone";
        }

        protected override void OnResize(EventArgs e)
        {
            listContainer.RemoveAllControls();
            listContainer.Visible = false;
            popupButton = null;
            popupOpening = false;
            popupClosing = false;
            popupRatio = 0.0f;

            UpdateButtonLayout();

            base.OnResize(e);
        }

        public override bool HitTest(int winX, int winY)
        {
            // Eat all the input when expanded.
            return IsExpanded || base.HitTest(winX, winY);
        }

        public override bool CanInteractWithContainer(Container c)
        {
            return c != listContainer || popupRatio > 0.75f;
        }

        protected override void OnTouchClick(PointerEventArgs e)
        {
            if (IsExpanded && !ClientRectangle.Contains(e.Position))
            {
                StartClosingList();
            }
            else
            {
                base.OnTouchClick(e);
            }
        }

        private bool SetButtonVisible(Button button,  bool vis)
        {
            if (button.Visible != vis)
            {
                button.Visible = vis;
                return true;
            }
            return false;
        }

        private void UpdateVisibleButtons()
        {
            if (ParentWindow == null)
                return;

            var needsLayout = false;

            needsLayout |= SetButtonVisible(buttonSequencer,      true);
            needsLayout |= SetButtonVisible(buttonPianoRoll,      true);
            needsLayout |= SetButtonVisible(buttonProject,        true);
            needsLayout |= SetButtonVisible(buttonChannel,        true);
            needsLayout |= SetButtonVisible(buttonInstrument,     true);
            needsLayout |= SetButtonVisible(buttonArpeggio,       true);
            needsLayout |= SetButtonVisible(buttonSnap,           App.IsPianoRollActive && App.IsEditingChannel);
            needsLayout |= SetButtonVisible(buttonEffect,         App.IsPianoRollActive && App.IsEditingChannel);
            needsLayout |= SetButtonVisible(buttonDPCMPlay,       App.IsPianoRollActive && App.IsEditingDPCMSample);
            needsLayout |= SetButtonVisible(buttonDPCMEffect,     App.IsPianoRollActive && App.IsEditingDPCMSample);
            needsLayout |= SetButtonVisible(buttonEnvelope,       App.IsPianoRollActive && App.IsEditingInstrument);
            needsLayout |= SetButtonVisible(buttonWaveformEffect, App.IsPianoRollActive && App.IsEditingInstrument && Instrument.EnvelopeHasRepeat(App.EditEnvelopeType));

            if (needsLayout)
                UpdateButtonLayout();
        }

        public override void Tick(float delta)
        {
            if (popupButton != null)
            {
                var needResize = false;

                if (popupOpening && popupRatio != 1.0f)
                {
                    delta *= 6.0f;
                    popupRatio = Math.Min(popupRatio + delta, 1.0f);
                    if (popupRatio == 1.0f)
                    {
                        popupOpening = false;
                    }
                    needResize = true;
                }
                else if (popupClosing && popupRatio != 0.0f)
                {
                    delta *= 10.0f;
                    popupRatio = Math.Max(popupRatio - delta, 0.0f);
                    if (popupRatio == 0.0f)
                    {
                        listContainer.RemoveAllControls();
                        listContainer.Visible = false;
                        popupButton = null;
                        popupClosing = false;

                        if (popupButtonNext != null)
                        {
                            var btn = popupButtonNext;
                            popupButtonNext = null;
                            btn.TriggerClick();
                        }
                    }
                    needResize = true;
                }

                if (needResize)
                {
                    if (IsLandscape)
                    {
                        var sx = (int)Math.Round(popupRect.Width * Utils.SmootherStep(popupRatio));
                        listContainer.Move(popupRect.Left - sx, popupRect.Top, sx, popupRect.Height);
                    }
                    else
                    {
                        var sy = (int)Math.Round(popupRect.Height * Utils.SmootherStep(popupRatio));
                        listContainer.Move(popupRect.Left, popupRect.Top - sy, popupRect.Width, sy);
                    }

                    MarkDirty();
                }
            }
            else
            {
                UpdateVisibleButtons();
            }
        }

        private void UpdateButtonLayout()
        {
            if (ParentWindow == null)
                return;

            var landscape = IsLandscape;
            var x = 0;

            foreach (var btn in allButtons)
            {
                if (!btn.Visible)
                    continue;

                var isNavButton =
                    btn == buttonSequencer ||
                    btn == buttonPianoRoll ||
                    btn == buttonProject;

                var size = isNavButton ? buttonSizeNav : buttonSize;

                if (landscape)
                    btn.Move(0, x, buttonSize, size);
                else
                    btn.Move(x, 0, size, buttonSize);

                x += size;
            }
        }

        private void StartExpandingList(Button button, Button[] buttons, Button[] iconButtons = null, int scrollItemIdx = -1, bool addPanel = false)
        {
            var landscape = IsLandscape;

            var maxWidth  = landscape ? Math.Min(ParentWindowSize.Width, ParentWindowSize.Height) - buttonSize : Width;
            var maxHeight = landscape ? Height : listItemSize * 8;

            var maxButtonSize = 0;
            var maxExtraSize  = 0;

            Debug.Assert(listContainer.Controls.Count == 0);

            // Add buttons, keep track of largest one.
            for (int i = 0; i < buttons.Length; i++)
            {
                var btn = buttons[i];

                if (addPanel)
                {
                    var panel = new PanelContainer(btn.ForegroundColor);
                    btn.ForegroundColor = Theme.BlackColor;
                    listContainer.AddControl(panel);
                    panel.AddControl(btn);
                    panel.SetupClipRegion(false);
                    btn.AutosizeWidth();
                    btn.Resize(btn.Width, listItemSize);
                    panel.Move(0, i * listItemSize);
                }
                else
                {
                    listContainer.AddControl(btn);
                    btn.AutosizeWidth();
                    btn.Move(0, i * listItemSize, btn.Width, listItemSize);
                }

                maxButtonSize = Math.Max(maxButtonSize, btn.Width);

                if (iconButtons != null)
                {
                    Debug.Assert(!addPanel);
                    var icon = iconButtons[i];
                    listContainer.AddControl(icon);
                    icon.Resize(listExtraIconSize, listItemSize);
                    maxExtraSize = Math.Max(maxExtraSize, icon.Width);
                }
            }

            // When there is no icon on right, add a bit of extra padding since images
            // tend of have padding in them. Looks weird otherwise.
            if (iconButtons == null)
                maxButtonSize += DpiScaling.ScaleForWindow(3);

            var popupWidth  = maxButtonSize + maxExtraSize;
            var popupHeight = buttons.Length * listItemSize + 1;

            popupWidth  = Math.Min(popupWidth,  maxWidth);
            popupHeight = Math.Min(popupHeight, maxHeight);

            // Final resize.
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Resize(maxButtonSize, buttons[i].Height);
                if (addPanel)
                {
                    buttons[i].ParentContainer.Resize(maxButtonSize, buttons[i].Height);
                }
                if (iconButtons != null)
                {
                    iconButtons[i].Move(maxButtonSize, buttons[i].Top, maxExtraSize, iconButtons[i].Height);
                }
            }

            popupRect = new Rectangle(0, 0, popupWidth, popupHeight);

            if (landscape)
            {
                if (popupRect.Height != Height)
                {
                    popupRect.Y = (button.Top + button.Bottom) / 2 - popupRect.Height / 2;

                    if (popupRect.Top < 0)
                        popupRect.Y -= popupRect.Top;
                    else if (popupRect.Bottom > Height)
                        popupRect.Y -= (popupRect.Bottom - Height);
                }
            }
            else
            {
                if (popupRect.Width != Width)
                {
                    popupRect.X = (button.Left + button.Right) / 2 - popupRect.Width / 2;

                    if (popupRect.Left < 0)
                        popupRect.X -= popupRect.Left;
                    else if (popupRect.Right > Width)
                        popupRect.X -= (popupRect.Right - Width);
                }
            }

            listContainer.Move(popupRect.Left, popupRect.Top, popupRect.Width, popupRect.Height);
            listContainer.ScollIndicatorColor = Color.FromArgb(128, addPanel ? Theme.BlackColor : Theme.LightGreyColor1);
            listContainer.Visible = true;
            listContainer.VirtualSizeY = buttons.Length * listItemSize;
            listContainer.ScrollY = (int)((scrollItemIdx + 0.5f) * listItemSize - popupRect.Height * 0.5f);
            listContainer.ClampScroll();
            listContainer.CancelFling();

            popupButton = button;
            popupRatio = 0.0f;
            popupOpening = true;
            popupClosing = false;
        }

        private void StartClosingList()
        {
            if (popupButton != null)
            {
                popupOpening = false;
                popupClosing = popupRatio > 0.0f ? true : false;
                if (!popupClosing)
                {
                    listContainer.RemoveAllControls();
                    listContainer.Visible = false;
                    popupButton = null;
                }
                listContainer.CancelFling();
            }
        }

        private void OnSequencer()
        {
            StartClosingList();
            App.SetActiveControl(App.Sequencer);
        }

        private void OnPianoRoll()
        {
            StartClosingList();
            if (!App.IsEditingChannel)
                App.StartEditChannel(App.SelectedChannelIndex);
            App.SetActiveControl(App.PianoRoll);
        }

        private void OnProjectExplorer()
        {
            StartClosingList();
            App.SetActiveControl(App.ProjectExplorer);
        }

        private bool CheckNeedsClosing(Button button)
        {
            StartClosingList();

            if (popupButton != null)
            {
                if (popupButton != button)
                {
                    Debug.Assert(popupRatio > 0.0f);
                    popupButtonNext = button;
                }
                return true;
            }

            return false;
        }

        private void OnDPCMPlayLongPress()
        {
            App.PreviewDPCMSample(App.EditSample, true);
        }

        protected override void OnRender(Graphics g)
        {
            var c = g.DefaultCommandList;
            var o = g.OverlayCommandList;

            c.Transform.GetOrigin(out var ox, out var oy);

            var listRect = listContainer.Rectangle;
            var screenRect = new Rectangle(Point.Empty, ParentWindow.Size);
            screenRect.Offset(-(int)ox, -(int)oy);

            // Background shadow.
            if (IsExpanded)
            {
                var shadowColor = Color.FromArgb((int)Utils.Clamp(popupRatio * 0.6f * 255.0f, 0, 255), Color.Black);

                if (IsLandscape)
                {
                    o.FillRectangle(screenRect.Left, screenRect.Top, listRect.Left, screenRect.Bottom, shadowColor);
                    o.FillRectangle(listRect.Left, screenRect.Top, 0, listRect.Top, shadowColor);
                    o.FillRectangle(listRect.Left, listRect.Bottom, 0, screenRect.Bottom, shadowColor);
                }
                else
                {
                    o.FillRectangle(screenRect.Left, screenRect.Top, screenRect.Right, listRect.Top, shadowColor);
                    o.FillRectangle(screenRect.Left, listRect.Top, listRect.Left, 0, shadowColor);
                    o.FillRectangle(listRect.Right, listRect.Top, screenRect.Right, 0, shadowColor);
                }
            }

            // Clear BG.
            var navBgRect = new Rectangle(
                buttonSequencer.Left,
                buttonSequencer.Top,
                buttonProject.Right  - buttonSequencer.Left,
                buttonProject.Bottom - buttonSequencer.Top);

            var bgRect = new Rectangle(0, 0, width, height);
            
            c.FillRectangleGradient(bgRect,    Theme.DarkGreyColor4, Theme.DarkGreyColor4.Scaled(0.8f), !IsLandscape, buttonSize);
            c.FillRectangleGradient(navBgRect, Theme.DarkGreyColor1, Theme.DarkGreyColor1.Scaled(0.8f), !IsLandscape, buttonSize);

            // Dividing line.
            if (IsLandscape)
                c.DrawLine(0, 0, 0, Height, Theme.BlackColor);
            else
                c.DrawLine(0, 0, Width, 0, Theme.BlackColor);

            base.OnRender(g);
        }
    }
}
