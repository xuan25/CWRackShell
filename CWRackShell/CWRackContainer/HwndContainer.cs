using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CWRackContainer
{
    public class HwndContainer
    {
        public bool IsAttached { get; private set; } = false;

        public Thickness? Crop = null;

        private Window OwnerWindow;
        private IntPtr OwnerHwnd;

        private IntPtr AttachingHwnd;
        private FrameworkElement AttachingTarget;

        private uint OriginalStyle = 0;
        private Rect OriginalRect = new Rect(0, 0, 0, 0);


        public HwndContainer(Window window, FrameworkElement target, IntPtr hwnd)
        {
            OwnerWindow = window;
            OwnerHwnd = new WindowInteropHelper(window).Handle;

            AttachingHwnd = hwnd;
            AttachingTarget = target;

            AttachingTarget.LayoutUpdated += AttachingTarget_LayoutUpdated;
        }

        private void AttachingTarget_LayoutUpdated(object sender, EventArgs e)
        {
            if (IsAttached)
            {
                if (Container == null)
                {
                    Point pos = AttachingTarget.TranslatePoint(new Point(), OwnerWindow);
                    Native.SetWindowPos(AttachingHwnd, IntPtr.Zero, (int)pos.X, (int)pos.Y, (int)AttachingTarget.ActualWidth, (int)AttachingTarget.ActualHeight, Native.SetWindowPosFlags.IgnoreZOrder | Native.SetWindowPosFlags.AsynchronousWindowPosition);

                }
                else
                {
                    Point pos = AttachingTarget.TranslatePoint(new Point(), OwnerWindow);
                    Native.SetWindowPos(new WindowInteropHelper(Container).Handle, IntPtr.Zero, (int)pos.X, (int)pos.Y, (int)AttachingTarget.ActualWidth, (int)AttachingTarget.ActualHeight, Native.SetWindowPosFlags.IgnoreZOrder | Native.SetWindowPosFlags.AsynchronousWindowPosition);
                }
            }
        }

        private ContainerWindow Container;

        public void Attach()
        {
            Native.GetWindowRect(AttachingHwnd, out Native.RECT lpRect);
            OriginalRect = new Rect(lpRect.Left, lpRect.Top, lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);

            uint style = Native.GetWindowLong(AttachingHwnd, (int)Native.WindowLongFlags.GWL_STYLE);
            OriginalStyle = style;
            style &= ~(uint)Native.WindowStyles.WS_CAPTION;
            style &= ~(uint)Native.WindowStyles.WS_SIZEBOX;
            style |= (uint)Native.WindowStyles.WS_CHILD;
            Native.SetWindowLong(AttachingHwnd, (int)Native.WindowLongFlags.GWL_STYLE, style);

            if (Crop == null)
            {
                Point pos = AttachingTarget.TranslatePoint(new Point(), OwnerWindow);
                Native.SetWindowPos(AttachingHwnd, IntPtr.Zero, (int)pos.X, (int)pos.Y, (int)AttachingTarget.ActualWidth, (int)AttachingTarget.ActualHeight, Native.SetWindowPosFlags.IgnoreZOrder);

                Native.SetParent(AttachingHwnd, OwnerHwnd);
            }
            else
            {
                Point pos = AttachingTarget.TranslatePoint(new Point(), OwnerWindow);
                Container = new ContainerWindow(AttachingHwnd, Crop.Value);
                Container.Show();
                Container.Left = pos.X;
                Container.Top = pos.Y;
                Container.Width = AttachingTarget.ActualWidth;
                Container.Height = AttachingTarget.ActualHeight;

                IntPtr containerHwnd = new WindowInteropHelper(Container).Handle;
                Native.SetParent(containerHwnd, OwnerHwnd);
                Native.SetParent(AttachingHwnd, containerHwnd);
            }

            IsAttached = true;
        }

        public void Detach()
        {
            if(IsAttached)
            {
                Native.SetParent(AttachingHwnd, IntPtr.Zero);
                Native.SetWindowLong(AttachingHwnd, (int)Native.WindowLongFlags.GWL_STYLE, OriginalStyle);
                Native.SetWindowPos(AttachingHwnd, IntPtr.Zero, (int)OriginalRect.Left, (int)OriginalRect.Top, (int)OriginalRect.Width, (int)OriginalRect.Height, Native.SetWindowPosFlags.IgnoreZOrder);

                if(Container != null)
                {
                    Container.Close();
                    Container = null;
                }

                IsAttached = false;
            }
        }

        
        //public IntPtr GetCWHwnd(IntPtr parent)
        //{
        //    IntPtr cwHwnd = Native.FindWindowEx(parent, IntPtr.Zero, "Cakewalk Core", null);
        //    return cwHwnd;
        //}

        //public Rect GetCWWindowRect(IntPtr parent)
        //{
        //    IntPtr cwHwnd = GetCWHwnd(parent);
        //    Native.GetWindowRect(cwHwnd, out Native.RECT lpRect);

        //    Rect rect = new Rect(lpRect.Left, lpRect.Top, lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);
        //    return rect;
        //}

        //public IntPtr GetCWMixerHwnd(IntPtr cwHwnd)
        //{
        //    IntPtr midHwnd = Native.FindWindowEx(cwHwnd, IntPtr.Zero, "MDIClient", null);
        //    IntPtr tvHwnd = Native.FindWindowEx(midHwnd, IntPtr.Zero, "CTrackViewFrame", null);
        //    IntPtr tvftHwnd = Native.FindWindowEx(tvHwnd, IntPtr.Zero, "CTVFTabSite", null);
        //    IntPtr afxHwnd = Native.FindWindowEx(tvftHwnd, IntPtr.Zero, "AfxFrameOrView140u", null);
        //    IntPtr mixerHwnd = Native.FindWindowEx(afxHwnd, IntPtr.Zero, "CMixerViewFloatingFrame", null);
        //    return mixerHwnd;
        //}

        //public IntPtr GetCWConsoleHwnd(IntPtr mixerHwnd)
        //{
        //    IntPtr cvHwnd = Native.FindWindowEx(mixerHwnd, IntPtr.Zero, "AfxMDIFrame140u", "Console View");
        //    return cvHwnd;
        //}

        //public IntPtr GetCWTrackPanelHwnd(IntPtr consoleHwnd)
        //{
        //    IntPtr cvHwnd = Native.FindWindowEx(consoleHwnd, IntPtr.Zero, "CMixerBaseView", "Tracks Pane");
        //    return cvHwnd;
        //}

        //public IntPtr GetCWBusPanelHwnd(IntPtr consoleHwnd)
        //{
        //    IntPtr cvHwnd = Native.FindWindowEx(consoleHwnd, IntPtr.Zero, "CMixerBaseView", "Bus Pane");
        //    return cvHwnd;
        //}
    }
}
