using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CWRackContainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HwndContainer CWContainer;
        HwndContainer Maya22Container;
        HwndContainer DWContainer;

        public MainWindow()
        {
            InitializeComponent();

            UpdateRootBorder();
        }

        private void UpdateRootBorder()
        {
            double baseDpi = 96;

            // Get Dpi scale
            double dpiX = baseDpi;
            double dpiY = baseDpi;
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }
            double dpiScaleFactorX = dpiX / baseDpi;
            double dpiScaleFactorY = dpiY / baseDpi;

            // Update root border
            double hb = Math.Ceiling(4 / dpiScaleFactorX);
            double vb = Math.Ceiling(4 / dpiScaleFactorY);
            RootBorder.Margin = new Thickness(hb, 0, hb, vb);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DetachAll();
        }

        public void AttachAll()
        {
            if(CWContainer == null)
            {
                IntPtr cwHwnd = Native.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Cakewalk Core", null);
                if (cwHwnd != IntPtr.Zero)
                {
                    CWContainer = new HwndContainer(this, CWPanel, cwHwnd);
                    CWContainer.Crop = new Thickness(6, 26, 35, 32);

                    CWContainer.Attach();
                }
            }

            if(Maya22Container == null)
            {
                IntPtr maya22Hwnd = Native.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "MAYA22USB Class", null);
                if (maya22Hwnd != IntPtr.Zero)
                {
                    Maya22Container = new HwndContainer(this, Maya22Panel, maya22Hwnd);
                    Maya22Container.Attach();
                }
            }

            if (DWContainer == null)
            {
                IntPtr dwHwnd = Native.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Ego_DwireAdvanceClass", null);
                if (dwHwnd != IntPtr.Zero)
                {
                    DWContainer = new HwndContainer(this, DWPanel, dwHwnd);
                    DWContainer.Attach();
                }
            }
        }

        public void DetachAll()
        {
            if (CWContainer != null)
            {
                CWContainer.Detach();
                CWContainer = null;
            }

            if (Maya22Container != null)
            {
                Maya22Container.Detach();
                Maya22Container = null;
            }

            if (DWContainer != null)
            {
                DWContainer.Detach();
                DWContainer = null;
            }
        }

        private void AttachBtn_Click(object sender, RoutedEventArgs e)
        {
            AttachAll();
        }

        private void DetachBtn_Click(object sender, RoutedEventArgs e)
        {
            DetachAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    AttachAll();
                }, System.Windows.Threading.DispatcherPriority.Background);
            });
           
        }
    }
}
