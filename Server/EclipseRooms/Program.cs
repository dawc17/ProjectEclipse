using System;
using System.Diagnostics;
using System.Threading;

namespace Eclipse.RoomServer
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            int port = Eclipse.Multiplayer.Online.Rooms.RoomProtocol.DefaultPort;
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "--port" || args[i] == "-p") && i + 1 < args.Length && int.TryParse(args[i + 1], out int value) && value > 0 && value < 65536)
                    port = value;
                else if (args[i] == "--help" || args[i] == "-h")
                {
                    Console.WriteLine("Eclipse room server. Usage: EclipseRooms [--port " + port + "]");
                    Console.WriteLine("Open UDP " + port + " to the internet (the only port it uses).");
                    return 0;
                }
            }
            string env = Environment.GetEnvironmentVariable("ECLIPSE_ROOMS_PORT");
            if (!string.IsNullOrEmpty(env) && int.TryParse(env, out int envPort) && envPort > 0 && envPort < 65536) port = envPort;

            Eclipse.Multiplayer.Online.Rooms.PlaytestWindow window;
            try { window = Eclipse.Multiplayer.Online.Rooms.PlaytestWindow.FromEnvironment(Environment.GetEnvironmentVariable); }
            catch (ArgumentException exception) { Console.Error.WriteLine(exception.Message); return 1; }
            Console.WriteLine(window.Message(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
            using (var server = new RoomServer(port, window))
            {
                server.Log = message => Console.WriteLine(DateTime.UtcNow.ToString("u") + " " + message);
                bool stop = false;
                Console.CancelKeyPress += (_, e) => { e.Cancel = true; stop = true; };
                AppDomain.CurrentDomain.ProcessExit += (_, _) => stop = true;
                Console.WriteLine("Eclipse room server listening on UDP " + server.Port);
                var clock = Stopwatch.StartNew();
                long lastStatus = 0;
                while (!stop)
                {
                    server.Update(clock.ElapsedMilliseconds);
                    if (clock.ElapsedMilliseconds - lastStatus > 60000)
                    {
                        lastStatus = clock.ElapsedMilliseconds;
                        Console.WriteLine(DateTime.UtcNow.ToString("u") + " status: " + server.ClientCount + " clients, " + server.RoomCount + " rooms");
                    }
                    Thread.Sleep(1);
                }
            }
            Console.WriteLine("Stopped.");
            return 0;
        }
    }
}
