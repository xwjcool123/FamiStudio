using System;
using System.Runtime.InteropServices;

namespace FamiStudio
{
    public class Dialog : Container
    {
        public delegate void KeyDownDelegate(Dialog dlg, KeyEventArgs e);
        public delegate void DialogClosingDelegate(Dialog dlg, DialogResult result, ref int numDialogsToPop);

        public event KeyDownDelegate DialogKeyDown;
        public event DialogClosingDelegate DialogClosing;

        protected Action<DialogResult> callback;
        protected DialogResult result = DialogResult.None;

        private bool fullscreen = true;

        public DialogResult Result => result;
        public bool Fullscreen => fullscreen;

        public Control FocusedControl
        {
            get => null;
            set { }
        }

        public Dialog(FamiStudioWindow win, string t = "", bool fs = true)
        {
            visible = false;
            fullscreen = fs;
            win.InitDialog(this);
        }

        public void Close(DialogResult res)
        {
            result = res;
            OnCloseDialog(result);

            // Pop dialog after the callback, otherwise sometimes the callback takes some time
            // and this counts in the transition time, making the fade look abrupt.
            var numDialogsToPop = 1;
            if (fullscreen)
            {
                DialogClosing?.Invoke(this, result, ref numDialogsToPop);
            }
            window.PopDialog(this, numDialogsToPop);
            callback?.Invoke(result);
        }

        protected virtual void OnShowDialog()
        {
        }

        protected virtual void OnCloseDialog(DialogResult res)
        {
        }

        public void ShowDialogAsync(Action<DialogResult> cb = null)
        {
            callback = cb;
            result = DialogResult.None;
            OnShowDialog();
            window.PushDialog(this);
        }

        protected override void OnTouchClick(PointerEventArgs e)
        {
            if (!fullscreen && !ClientRectangle.Contains(e.Position))
            {
                Close(DialogResult.Cancel);
            }
            else
            {
                base.OnTouchClick(e);
            }
        }

        public override bool HitTest(int winX, int winY)
        {
            // We eat all the inputs so we can close when clicking outside.
            return true;
        }

        public virtual void OnWindowResize(EventArgs e)
        {
        }

        protected override void OnRender(Graphics g)
        {
            if (!fullscreen)
            {
                g.OverlayCommandList.DrawRectangle(ClientRectangle, Theme.BlackColor, 1);
            }

            base.OnRender(g);
        }
    }
}
