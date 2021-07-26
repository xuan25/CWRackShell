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
using System.Windows.Shapes;

namespace CWRackContainer
{
    /// <summary>
    /// Interaction logic for ContainerWindow.xaml
    /// </summary>
    public partial class ContainerWindow : Window
    {
        public Thickness Crop = new Thickness(0, 0, 0, 0);
        private IntPtr AttachingHwnd;

        public ContainerWindow(IntPtr hwnd, Thickness crop)
        {
            InitializeComponent();

            AttachingHwnd = hwnd;
            Crop = crop;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            uint style = Native.GetWindowLong(hwnd, (int)Native.WindowLongFlags.GWL_STYLE);
            style &= ~(uint)Native.WindowStyles.WS_CAPTION;
            style &= ~(uint)Native.WindowStyles.WS_SIZEBOX;
            style |= (uint)Native.WindowStyles.WS_CHILD;
            style |= (uint)Native.WindowStyles.WS_CLIPCHILDREN;
            Native.SetWindowLong(hwnd, (int)Native.WindowLongFlags.GWL_STYLE, style);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Native.SetWindowPos(AttachingHwnd, IntPtr.Zero, (int)-Crop.Left, (int)-Crop.Top, (int)(this.ActualWidth + Crop.Left + Crop.Right), (int)(this.ActualHeight + Crop.Top + Crop.Bottom), Native.SetWindowPosFlags.IgnoreZOrder | Native.SetWindowPosFlags.AsynchronousWindowPosition);
        }
    }
}
