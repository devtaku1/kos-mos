using Discord;
using Discord.WebSocket;
using KOS_MOS.Service;

namespace KOS_MOS
{
    public class Program
    {
        private static readonly string? DiscordToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main program.
        /// </summary>
        /// <returns></returns>
        public static async Task MainAsync()
        {
            // Load up Environment Variables
            DotNetEnv.Env.Load();
            DotNetEnv.Env.TraversePath().Load();

            // Create a config with specified gateway intents
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            // Create a new Discord client
            var client = new DiscordSocketClient(config);

            // Log messages to the console
            client.Log += Log;

            // Handle messages received
            client.MessageReceived += HandleCommand;

            // Login to Discord
            await client.LoginAsync(TokenType.Bot, DiscordToken);

            // Start the client
            await client.StartAsync();

            // Wait for the client to stop
            await Task.Delay(-1);
        }

        /// <summary>
        /// This is the method called whenever a message is received.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task HandleCommand(SocketMessage message)
        {
            if (message.Content.StartsWith("!chat")) await OpenAiService.ChatGpt(message);
        }

        /// <summary>
        /// This is the method called whenever a log message is received.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}