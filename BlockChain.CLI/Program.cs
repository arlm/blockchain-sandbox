using System;
using System.Threading;
using System.Threading.Tasks;
using BlockChain.Core;
using CommandLine;

namespace BlockChain.CLI
{
    class Program
    {
        private static readonly Chain blockchain = new Chain();
        private static readonly char[] spinner = { '-', '\\', '|', '/' };

        private static volatile bool marqueeRunning;
        private static Task marqueeThread;
        private static CancellationTokenSource cts;
        private static volatile bool marqueeWithProgress;
        private static volatile float complete;
        private static int spinnerPos;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.CancelKeyPress += Console_CancelKeyPress;

            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.Verbose && !string.IsNullOrWhiteSpace(options.Command))
                {
                    Console.Out.WriteLine($"Running {options.Command} verb [{options.Data}]\n");

                }

                if ((options.Command == "mine" || options.Command == "connect" || options.Command == "open")
                    ^ !string.IsNullOrWhiteSpace(options.Data))
                {
                    Console.Out.WriteLine(options.GetUsage());
                    return;
                }

                switch (options.Command)
                {
                    case "blockchain":
                        Console.Out.WriteLine(blockchain.Dump());
                        break;
                    case "mine":
                        StartMarquee();
                        var newBlock = blockchain.Mine(options.Data);
                        StopMarquee();
                        Console.Out.WriteLine($"🎉  Congratulations!A new block was mined. 💎\n\n{newBlock.Dump()}\n");
                        break;
                    case "peers":
                    case "connect":
                    case "discover":
                    case "open":
                        Console.Out.WriteLine("Not implemented yet!");
                        break;
                    default:
                        Console.Out.WriteLine(options.GetUsage());
                        break;
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StopMarquee();
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            StopMarquee();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            StopMarquee();
        }

        private static void UpdateProgress(float completion)
        {
            if (completion >= 100f)
            {
                Console.WriteLine("\rProcess complete!".PadRight(Console.BufferWidth));
            }
            else
            {
                string marquee;

                if (marqueeWithProgress)
                {
                    marquee = $"\rWorking... {spinner[spinnerPos]} - {completion:0.0}%";
                }
                else
                {
                    marquee = $"\rWorking... {spinner[spinnerPos]}";
                }

                Console.Write(marquee.PadRight(Console.BufferWidth));
                spinnerPos = (spinnerPos >= 3) ? 0 : spinnerPos + 1;
            }
        }

        public static float Progress
        {
            get => complete;

            set
            {
                if (value >= 100f)
                {
                    complete = 100;

                    if (marqueeRunning)
                    {
                        StopMarquee();
                    }
                }
                else if (value <= 0f)
                {
                    complete = 0;
                }

                UpdateProgress(complete);
            }
        }

        public static bool ShowProgress => marqueeWithProgress;

        public static void StartMarquee(bool showProgress = false)
        {
            StopMarquee();

            marqueeWithProgress = showProgress;
            marqueeRunning = true;
            cts = new CancellationTokenSource();
            marqueeThread = new Task(MarqueThread, cts.Token);
            marqueeThread.Start();
        }

        public static void StopMarquee()
        {
            marqueeRunning = false;

            if (marqueeThread?.IsCompleted ?? false)
            {
                cts?.Cancel();
            }

            marqueeThread = null;
            cts?.Dispose();
            cts = null;

            Console.WriteLine("\r".PadRight(Console.BufferWidth));
        }

        private static void MarqueThread()
        {
            while (marqueeRunning && complete <= 100)
            {
                UpdateProgress(complete);

                new AutoResetEvent(false).WaitOne(250);
            }
        }
    }
}
