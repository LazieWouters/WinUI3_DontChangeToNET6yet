using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Windows.UI.Core;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_DontChangeToNET6yet
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            //Activated += Current_Activated;

            //Update HD size
            ShowHDDSize();

        }


        //private async void PageOnLoaded(object sender, RoutedEventArgs e)
        //{
        //    //ApplyTransparencyToTitlebar();
        //    //await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("DriversUsedSpace");

        //    //HDUsedSpace();

        //}


        //private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        //{
        //    if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
        //    {
        //        Exit_or_Minimize();
        //    }            
        //}


        #region MinimizeWindow with user32.dll

        //The following code minimizes or maximizes all windows for a process.
        //It takes a process name, iterates over all windows using EnumWindows,
        //determines the current window placement using GetWindowPlacement, and
        //maximizes or minimizes the window using ShowWindowAsync.
        public static void MinimizeWindow()
        {
            string processName = "WinUI3_DontChangeToNET6yet";
            CloseWindow(processName);
        }
        
        public static void CloseWindow(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    IDictionary<IntPtr, string> windows = List_Windows_By_PID(process.Id);
                    foreach (KeyValuePair<IntPtr, string> pair in windows)
                    {
                        var placement = new WINDOWPLACEMENT();
                        GetWindowPlacement(pair.Key, ref placement);

                        if (placement.showCmd == SW_SHOWMINIMIZED)
                        {
                            //if minimized, show maximized
                            ShowWindowAsync(pair.Key, SW_SHOWMAXIMIZED);
                        }
                        else
                        {
                            //default to minimize
                            ShowWindowAsync(pair.Key, SW_SHOWMINIMIZED);
                        }
                    }
                }
            }
        }

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("USER32.DLL")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        public static IDictionary<IntPtr, string> List_Windows_By_PID(int processID)
        {
            IntPtr hShellWindow = GetShellWindow();
            Dictionary<IntPtr, string> dictWindows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                //ignore the shell window
                if (hWnd == hShellWindow)
                {
                    return true;
                }

                //ignore non-visible windows
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                //ignore windows with no text
                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                {
                    return true;
                }

                uint windowPid;
                GetWindowThreadProcessId(hWnd, out windowPid);

                //ignore windows from a different process
                if (windowPid != processID)
                {
                    return true;
                }

                StringBuilder stringBuilder = new StringBuilder(length);
                GetWindowText(hWnd, stringBuilder, length + 1);
                dictWindows.Add(hWnd, stringBuilder.ToString());

                return true;

            }, 0);

            return dictWindows;
        }

        #endregion

        private void Exit_or_Minimize()
        {
            //close the Launcher
            //System.Environment.Exit(0);

            //Minimize the Launcher
            MinimizeWindow();

            //Update HD size
            ShowHDDSize();
        }

        private void runAsAdmin(string path)
        {
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = path;
                process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.Arguments = $"-s {ServiceName} -start";
                process.StartInfo.Verb = "runas";
                process.Start();
            }
        }

        private void ShowHDDSize()
        {
            DriveC.Text = GetHDDSize(@"C:\");
            DriveD.Text = GetHDDSize(@"D:\");
        }

        private string GetHDDSize(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.Name.Substring(0, 2) + " " +
                        (Convert.ToDouble((drive.TotalSize - drive.TotalFreeSpace)) / (1024 * 1024 * 1024)).ToString("N1") + " / " +
                        (drive.TotalSize) / (1024 * 1024 * 1024) + " GB";

                }
            }
            return "null";
        }



        //private void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var process = Process.Start(@"C:\Windows\notepad.exe");
        //}

        private void Button700_Click(object sender, RoutedEventArgs e)
        {
            //Update HD size
            ShowHDDSize();
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"C:\Users\lazie\AppData\Local\Microsoft\Edge Dev\Application\msedge.exe");
            Exit_or_Minimize();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //Chrome
            Exit_or_Minimize();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //Opera
            Exit_or_Minimize();
        }



        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            runAsAdmin(@"D:\VS 2022\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\devenv.exe");
            Exit_or_Minimize();
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //VSCodePreview
            Exit_or_Minimize();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //VSCodeStable
            Exit_or_Minimize();
        }




        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"C:\Users\lazie\AppData\Roaming\uTorrent\App\uTorrent\uTorrent.exe");
            Exit_or_Minimize();
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            runAsAdmin(@""); //Beyond Compare (Adm)
            Exit_or_Minimize();
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //CCleaner
            Exit_or_Minimize();
        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //Core Temp
            Exit_or_Minimize();
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"xxx"); //CPU-Z
            Exit_or_Minimize();
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"C:\Program Files (x86)\MusicBee\MusicBee.exe");
            Exit_or_Minimize();
        }

        private void Button13_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"C:\Program Files\Tracker Software\PDF Editor\PDFXEdit.exe");
            Exit_or_Minimize();
        }

        private void Button14_Click(object sender, RoutedEventArgs e)
        {
            runAsAdmin(@"C:\Program Files\VS Revo Group\Revo Uninstaller Pro\RevoUninPro.exe");
            Exit_or_Minimize();
        }

        private void Button15_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"C:\Program Files\VideoLAN\VLC\vlc.exe");
            Exit_or_Minimize();
        }

        private void Button16_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"C:\Program Files\WinRAR\WinRAR.exe");
            Exit_or_Minimize();
        }







        private void Button17_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button18_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button19_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button20_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button21_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button22_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button23_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button24_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button25_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button26_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button27_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button28_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button29_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button30_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button31_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button32_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button33_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button34_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button35_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button36_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }













        private void Button37_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button38_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button39_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button40_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

















        private void Button47_Click(object sender, RoutedEventArgs e)
        {
            runAsAdmin(@"C:\Meus Programas\Captura_e_Esboço\Captura_e_Esboço.lnk");
            Exit_or_Minimize();
        }

        private void Button48_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button49_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button50_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button51_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button52_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button53_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button54_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }
















        private void Button100_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }









        private void Button200_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button201_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }















        private void Button300_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }











        private void Button400_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button401_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button402_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button403_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

















        private void Button500_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button501_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }










        private void Button600_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button601_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button602_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button603_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button604_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button605_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button606_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button607_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button608_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button609_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button610_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button611_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button612_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

        private void Button614_Click(object sender, RoutedEventArgs e)
        {
            var process = Process.Start(@"notepad");
            Exit_or_Minimize();
        }

    }
}
