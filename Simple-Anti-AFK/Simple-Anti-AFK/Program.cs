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

            bool antiAfkBattleground = false;

            Console.Write("Do you wish to never go AFK (useful in battlegrounds) or never let your character go offline? Answer with 'A' or 'B': ");
            string answer = Console.ReadLine().ToUpper();

            while (answer != "A" && answer != "B")
            {
                Console.Write("This option does not exist. Try again: ");
                answer = Console.ReadLine().ToUpper();
            }

            antiAfkBattleground = answer == "A";
            Process process = processes[0];

            Console.WriteLine("Entering whisper loop now. Your character won't go AFK.");

            while (true)
            {
                try
                {
                    string antiAfkString = "/w Namethatdoesnotexistatall a";

                    //! Send the message
                    SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                    SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);

                    Thread.Sleep(500);

                    //! Write out the message above (slash opens chatbox automatically)
                    for (int i = 0; i < antiAfkString.Length; ++i)
                    {
                        SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(antiAfkString[i]), IntPtr.Zero);
                        Thread.Sleep(50);
                    }

                    //! Send the message
                    SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                    SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);

                    Thread.Sleep(1000);

                    if (!antiAfkBattleground)
                    {
                        string goAfkString = "/afk";

                        //! Let the player go AFK visually again so players won't bother them
                        for (int i = 0; i < goAfkString.Length; ++i)
                        {
                            SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(goAfkString[i]), IntPtr.Zero);
                            Thread.Sleep(50);
                        }

                        SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                        SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                    }
                }
                catch
                {

                }

                //! Sleep for ~28.3 minutes so we don't log to the char screen unless the user
                //! wants to never go AFK. In that case, we run this anti-AFK line every ~4 min.
                Thread.Sleep(antiAfkBattleground ? 290000 : 1700000);
            }
        }
    }
}
