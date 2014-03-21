using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Anti_AFK
{
    class Program
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;
        private const int VK_RETURN = 0x0D;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        static void Main(string[] args)
        {
            //! Find the running WoW process. Only one may be running at the time this application is started.
            Process[] processes = Process.GetProcessesByName("Wow");

            if (processes.Length == 0)
            {
                Console.WriteLine("There's no WoW process running.");
                Console.ReadKey();
                return;
            }

            if (processes.Length > 1)
            {
                Console.WriteLine("There's more than one WoW process running. Please only turn on the one you wish to not go AFK with. Afterwards, you can start the others again.");
                Console.ReadKey();
                return;
            }

            Process process = processes[0];

            Console.WriteLine("Entering whisper loop now. Your character won't go AFK.");

            while (true)
            {
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;

                        string antiAfkString = "/w Namethatdoesnotexistatall a";

                        //! Write out the message above (slash opens chatbox automatically)
                        for (int i = 0; i < antiAfkString.Length; ++i)
                        {
                            SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(antiAfkString[i]), IntPtr.Zero);
                            Thread.Sleep(50);
                        }

                        //! Send the message
                        SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                        SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);

                        Thread.CurrentThread.Abort();
                    }
                    catch
                    {
                        Thread.CurrentThread.Abort();
                    }
                }).Start();

                Thread.Sleep(1700000); //! Sleep for ~28.3 minutes so we don't log to the char screen
            }
        }
    }
}
