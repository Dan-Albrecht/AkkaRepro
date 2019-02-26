namespace SerializationError
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Akka.Configuration;
    using static Akka.Actor.CoordinatedShutdown;

    class Program
    {
        static async Task Main(string[] args)
        {
            string parsedConfig = File
                .ReadAllText("Config.ini")
                .Replace("__CURRENT_MACHINE_NAME__", Environment.MachineName);

            Config config = ConfigurationFactory.ParseString(parsedConfig);

            using (ActorSystem actorSystem = ActorSystem.Create("myActor", config))
            {
                // Need to give it a second or 2 to full startup and print everything to console
                await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
                Console.WriteLine("Press any key to shutdown...");
                Console.ReadKey(true);
                CoordinatedShutdown coordinatedShutdown = CoordinatedShutdown.Get(actorSystem);
                await coordinatedShutdown.Run(ClrExitReason.Instance).ConfigureAwait(false);
                await actorSystem.WhenTerminated.ConfigureAwait(false);
            }
        }
    }
}
