using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static PInvoke.User32;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_DontChangeToNET6yet
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);         

            SetWindowDetails(hwnd, 420, 418, 1050, 612);

            m_window.Activate();

        }

        private Window m_window;

        private static void SetWindowDetails(IntPtr hwnd, int left, int top, int width, int height)
        {
            var dpi = GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            left = (int)(left * scalingFactor);
            top = (int)(top * scalingFactor);
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            _ = SetWindowPos(hwnd, SpecialWindowHandles.HWND_TOP,
                                         left, top, width, height,
                                        SetWindowPosFlags.SWP_NOREPOSITION);            
           

        }
    }
}
