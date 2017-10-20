using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;

namespace DiscordBot
{
    public class Program
    {
        private DiscordSocketClient client;
        private IServiceProvider services;
        private CommandService commands;
        static void Main(string[] args)
        
        => new Program().StartAsync().GetAwaiter().GetResult();
        

        public async Task StartAsync()
        {

            client = new DiscordSocketClient();

            commands = new CommandService();

            services = new ServiceCollection().BuildServiceProvider();
            try
            {
                StreamReader sr = new StreamReader("C:/Users/Jackson/Desktop/Discord_Bot.txt");
                String token = sr.ReadLine();
                sr.Close();
                await InstallCommands();

                await client.LoginAsync(TokenType.Bot, token);

                await client.StartAsync();


                await Task.Delay(-1);
            }
            catch(Exception e)
            {
                Console.WriteLine("The File could not be read " + e.Message);
            }

        }

        public async Task InstallCommands()
        {

            //Hook the MessagedReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;

            //Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
                //Don't process the commands if it was a System Message
                var message = messageParam as SocketUserMessage;


                if (message == null) return;

                int argPos = 0;

                //Determine if the message is a command, based on if it starts with "!" or a mention prefix
                if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.Author.IsBot)) return;

                //Create a Command Context

                var context = new SocketCommandContext(client, message);

                //Execute the command. (result does not indicate a return value,
                //rather an object stating if the command executed successfully

                var result = await commands.ExecuteAsync(context, argPos, services);

            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                await context.User.SendMessageAsync(result.ErrorReason);
        }
        }
    }
