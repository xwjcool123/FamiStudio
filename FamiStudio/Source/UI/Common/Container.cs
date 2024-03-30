using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FamiStudio
{
    public class Container : Control
    {
        // These are only used by scroll container.
        protected int containerScrollX;
        protected int containerScrollY;
        protected List<Control> controls = new List<Control>();
        protected bool clipRegion = true;

        public int ScrollX { get => containerScrollX; set => SetAndMarkDirty(ref containerScrollX, value); }
        public int ScrollY { get => containerScrollY; set => SetAndMarkDirty(ref containerScrollY, value); }
        public IReadOnlyCollection<Control> Controls => controls.AsReadOnly();

        public Container()
        {
        }

        public virtual void ContainerMouseWheelNotify(Control control, MouseEventArgs e)
        {
        }

        public void AddControl(Control ctrl)
        {
            if (!controls.Contains(ctrl))
            {
                controls.Add(ctrl);
                ctrl.SetParentContainer(this);
                ctrl.AddedToContainer();
            }
        }

        public void RemoveControl(Control ctrl)
        {
            if (ctrl != null && controls.Contains(ctrl))
            {
                ctrl.SetParentContainer(null);
                controls.Remove(ctrl);
            }
        }

        public void RemoveAllControls()
        {
            for (int i = controls.Count - 1; i >= 0; i--) 
            {
                controls[i].SetParentContainer(null);
                controls.RemoveAt(i);
            }
        }

        public Control FindLastControlOfType(Type type)
        {
            for (int i = controls.Count - 1; i >= 0; i--)
            {
                if (controls[i].GetType() == type)
                    return controls[i];
            }
            return null;
        }

        public Rectangle GetControlsRect()
        {
            var rc = new Rectangle();
            foreach (var c in controls)
                rc = Rectangle.Union(rc, c.ClientRectangle.Offsetted(c.Left, c.Top));
            return rc;
        }

        public virtual bool CanInteractWithContainer(Container c)
        {
            return true;
        }

        public virtual Control GetControlAt(int winX, int winY, out int ctrlX, out int ctrlY)
        {
            // First look for containers. Last containers are considered to have higher Z-order.
            for (int i = controls.Count - 1; i >= 0; i--)
            {
                if (controls[i] is Container c && CanInteractWithContainer(c))
                {
                    if (c.Visible && c.HitTest(winX, winY))
                    {
                        var ctrl = c.GetControlAt(winX, winY, out ctrlX, out ctrlY);
                        
                        if (ctrl != null)
                        {
                            return ctrl;
                        }
                        else // We are in the container, but not in a specific child.
                        {
                            var winPos = c.WindowPosition;
                            ctrlX = winX - winPos.X;
                            ctrlY = winY - winPos.Y;
                            return c;
                        }
                    }
                }
            }

            // No better container found, look though our controls.
            for (int i = controls.Count - 1; i >= 0; i--)
            {
                if (!(controls[i] is Container))
                {
                    var c = controls[i];
                    if (c.Visible && c.HitTest(winX, winY))
                    {
                        var winPos = c.WindowPosition;
                        ctrlX = winX - winPos.X;
                        ctrlY = winY - winPos.Y;
                        return c;
                    }
                }
            }

            ctrlX = 0;
            ctrlY = 0;
            return null;
        }

        public override Point ControlToWindow(Point p)
        {
            if (container == null)
            {
                return p;
            }
            else
            {
                return base.ControlToWindow(p);
            }
        }

        public override Point WindowToControl(Point p)
        {
            p.X -= left;
            p.Y -= top;

            if (container == null)
            {
                return p;
            }
            else
            {
                return base.WindowToControl(p);
            }
        }

        public override void Tick(float delta)
        {
            foreach (var ctrl in controls)
            {
                if (ctrl.Visible)
                {
                    ctrl.Tick(delta);
                }
            }
        }

        protected void RenderChildControlsAndContainers(Graphics g)
        {
            // First render our controls, then other containers.
            RenderChildControls(g, false);
            RenderChildControls(g, true);
        }

        protected void RenderChildControls(Graphics g, bool container)
        {
            foreach (var c in controls)
            {
                if ((c is Container) == container && c.Visible)
                {
                    var tx = c.Left - containerScrollX;
                    var ty = c.Top  - containerScrollY;

                    // Simple culling for containers, mostly just for the project explorer.
                    if (container)
                    {
                        var containerRect = c.ClientRectangle.Offsetted(tx, ty);
                        if (!containerRect.Intersects(ClientRectangle))
                            continue;
                    }

                    g.Transform.PushTranslation(tx, ty);
                    c.Render(g);
                    g.Transform.PopTransform();
                }
            }
        }

        protected override void OnRender(Graphics g)
        {
            RenderChildControlsAndContainers(g);
        }

        public override void Render(Graphics g)
        {
            if (clipRegion) g.PushClipRegion(0, 0, width, height);
            OnRender(g);
            if (clipRegion) g.PopClipRegion();
        }
    }
}
