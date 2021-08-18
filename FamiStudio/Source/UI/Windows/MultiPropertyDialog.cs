﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FamiStudio
{
    public partial class MultiPropertyDialog : Form
    {
        class PropertyPageTab
        {
            public Button button;
            public PropertyPage properties;
        }

        int selectedIndex = 0;
        List<PropertyPageTab> tabs = new List<PropertyPageTab>();

        Font font;
        Font fontBold;

        public MultiPropertyDialog(int width, int height, int tabsWidth = 150)
        {
            InitializeComponent();

            string suffix = DpiScaling.Dialog >= 2.0f ? "@2x" : "";
            this.buttonYes.Image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"FamiStudio.Resources.Yes{suffix}.png"));
            this.buttonNo.Image  = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"FamiStudio.Resources.No{suffix}.png"));
            this.Width    = DpiScaling.ScaleForDialog(width);
            this.font     = new Font(PlatformUtils.PrivateFontCollection.Families[0], 10.0f, FontStyle.Regular);
            this.fontBold = new Font(PlatformUtils.PrivateFontCollection.Families[0], 10.0f, FontStyle.Bold);

            toolTip.SetToolTip(buttonYes, "Accept");
            toolTip.SetToolTip(buttonNo, "Cancel");

            tableLayout.ColumnStyles[0].Width = tabsWidth * DpiScaling.Dialog;
        }

        public PropertyPage AddPropertyPage(string text, string image)
        {
            var suffix = DpiScaling.Dialog > 1.0f ? "@2x" : "";
            var bmp = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"FamiStudio.Resources.{image}{suffix}.png")) as Bitmap;

            if ((DpiScaling.Dialog % 1.0f) != 0.0f)
            {
                var newWidth  = (int)(bmp.Width  * (DpiScaling.Dialog / 2.0f));
                var newHeight = (int)(bmp.Height * (DpiScaling.Dialog / 2.0f));

                bmp = new System.Drawing.Bitmap(bmp, newWidth, newHeight);
            }

            var page = new PropertyPage();
            page.Dock = DockStyle.Fill;
            panelProps.Controls.Add(page);

            var tab = new PropertyPageTab();
            tab.button = AddButton(text, bmp);
            tab.properties = page;

            tabs.Add(tab);

            return page;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            // Property pages need to be visible when doing the layout otherwise
            // they have the wrong size. 
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].properties.Visible = i == selectedIndex;
            }

            base.OnHandleCreated(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            SuspendLayout();

            int maxHeight = Math.Max((int)(32 * DpiScaling.Dialog * tabs.Count), Width / 2);
            for (int i = 0; i < tabs.Count; i++)
            {
                maxHeight = Math.Max(maxHeight, tabs[i].properties.LayoutHeight);
            }

            tableLayout.Height = maxHeight;

            buttonYes.Width  = DpiScaling.ScaleForDialog(buttonYes.Width);
            buttonYes.Height = DpiScaling.ScaleForDialog(buttonYes.Height);
            buttonNo.Width   = DpiScaling.ScaleForDialog(buttonNo.Width);
            buttonNo.Height  = DpiScaling.ScaleForDialog(buttonNo.Height);

            Height = maxHeight + buttonNo.Height + 20;

            buttonYes.Left = Width  - buttonYes.Width * 2 - 17;
            buttonYes.Top  = Height - buttonNo.Height - 12;
            buttonNo.Left  = Width  - buttonNo.Width  - 12;
            buttonNo.Top   = Height - buttonNo.Height - 12;

            ResumeLayout();
            CenterToParent();

            base.OnLoad(e);
        }
        
        public PropertyPage GetPropertyPage(int idx)
        {
            return tabs[idx].properties;
        }

        public int SelectedIndex => selectedIndex;

        private void Btn_Click(object sender, EventArgs e)
        {
            SuspendLayout();

            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].button == sender)
                {
                    selectedIndex = i;
                    tabs[i].button.Font = fontBold;
                    tabs[i].properties.Visible = true;
                }
                else
                {
                    tabs[i].button.Font = font;
                    tabs[i].properties.Visible = false;
                }
            }

            ResumeLayout();
        }

        private Button AddButton(string text, Bitmap image)
        {
            var btn = new NoFocusButton();
            var sizeY = DpiScaling.ScaleForDialog(32);

            btn.BackColor = BackColor;
            btn.ForeColor = Theme.LightGreyFillColor2;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Theme.DarkGreyFillColor2;
            btn.FlatAppearance.MouseDownBackColor = Theme.DarkGreyFillColor2;
            btn.Image = image;
            btn.Top = tabs.Count * sizeY;
            btn.Left = 0;
            btn.Width = panelTabs.Width;
            btn.Height = sizeY;
            btn.Font = tabs.Count == 0 ? fontBold : font;
            btn.Text = text;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.Click += Btn_Click;

            panelTabs.Controls.Add(btn);

            return btn;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var p = base.CreateParams;
                p.Style &= (~0x02000000); // WS_CLIPCHILDREN
                return p;
            }
        }

        private void MultiPropertyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
