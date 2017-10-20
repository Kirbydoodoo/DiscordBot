using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Giveaway : ModuleBase<SocketCommandContext>
    {

        private EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
        private EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();
        private Emoji dice = new Emoji("🎲");
        private Emoji trophy = new Emoji("🏆");
        private Random rand = new Random();


        //Initates command, summary, and assigns permissions 
        [Command("gstart")]
        [Summary("Starts text giveaway on server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.AddReactions)]

        //Type is what you are giving away (Rank, kit, key, etc.)
        public async Task gAway(string time, string prize, string type)
        {
            Console.WriteLine("Starting giveaway");
            int t = Int32.Parse(time.Remove(time.Length - 1));


            //Starts the Embeded Message
            MyEmbedBuilder.WithColor(new Color(255, 255, 0));
            var Name = MyEmbedField.WithName(":game_die: **GIVEAWAY**  :game_die:");
            MyEmbedField.WithIsInline(true);


            //Changes message if time is seconds or minutes
            if (time.Contains('h'))
            {

                var msg = MyEmbedField.WithValue("Free " + "***" + prize + "***" + " " + type + "\nReact with :cdicon2: to win!\n" + "Time remaining: " + t.ToString() + " hours");
                t = t * 3600;
                
            }
            if (time.Contains('m'))
            {
                var msg = MyEmbedField.WithValue("Free " + "***" + prize + "***" + " " + type + "\nReact with :cdicon2: to win!\n" + "Time remaining: " + t.ToString() + " minutes");

                t = t * 60;
                
            }
            else
            {
                var msg = MyEmbedField.WithValue("Free " + "***" + prize + "***" + " " + type + "\nReact with :cdicon2: to win!\n" + "Time remaining: " + t.ToString() + " seconds");
                
            }



            //Sends message
            MyEmbedBuilder.AddField(MyEmbedField);
            var message = await Context.Channel.SendMessageAsync("", false, MyEmbedBuilder.Build());

            //Reacts to message
            await message.AddReactionAsync(dice);

            //Begins countdown and edits embeded field every hour, minute, or second
            while (t > 0)
            {
                await Task.Delay(1000);
                Console.WriteLine(t);
                t--;
                var newMessage = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;
                var embed2 = new EmbedBuilder();
                embed2.AddField(Name);
                embed2.WithColor(new Color(255, 255, 0));

                if (t >= 3600)
                {
                    int t3 = t;
                    t3 = t3 / 3600;
                    int time_minutes = t;
                    time_minutes = (t / 60) % 60;

                    MyEmbedField.WithValue("Free " + "***" + prize + "***" + " " + type + "!" + "\nReact with :game_die: to win!\n" + "Time remaining: " + t3.ToString() + " hours " + time_minutes + " minutes");
                }
                if (t >= 60 && t < 3600)
                {
                    int t2 = t;
                    t2 = t2 / 60;
                    int time_seconds = t % 60;
                    MyEmbedField.WithValue("Free " + "***" + prize + "***" + " " + type + "!" + "\nReact with :game_die: to win!\n" + "Time remaining: " + t2.ToString() + " minutes " + time_seconds + " seconds");
                }

                if (t < 60)
                {
                    MyEmbedField.WithValue("Free " + "***" + prize + "***" + " " + type + "!" + "\nReact with :game_die: to win!\n" + "Time remaining: " + t.ToString() + " seconds");
                }
                await newMessage.ModifyAsync(m => m.Embed = embed2.Build());

            }


            //Adds users to list and randomly selects winner
            await message.RemoveReactionAsync(dice, message.Author);
            IReadOnlyCollection<IUser> temp = await message.GetReactionUsersAsync("🎲");

            if (temp.Count() > 0)
            {
                IUser winner = temp.ElementAt(rand.Next(temp.Count));
                var message3 = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;
                var embed3 = new EmbedBuilder();
                embed3.AddField(Name);
                embed3.WithColor(new Color(255, 255, 0));

                MyEmbedField.WithValue("***Congratulations*** " + "! " + winner.Mention + " You won the free " + prize + " " + type + "!");
                await message3.ModifyAsync(m => m.Embed = embed3.Build());
                await message3.AddReactionAsync(trophy);
            }
            else {
                var message4 = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;
                var embed4 = new EmbedBuilder();
                embed4.AddField(Name);
                embed4.WithColor(new Color(255, 255, 0));

                MyEmbedField.WithValue("There are no winners today :)");
                await message4.ModifyAsync(m => m.Embed = embed4.Build());
                await message4.AddReactionAsync(trophy);

            }
        }

        


        [Command("help")]
        [Summary("Describes syntax for issuing command of bot")]
        [RequireUserPermission(GuildPermission.Administrator)]

        public async Task Info()
        {
            await Context.Channel.SendMessageAsync("```To activate giveaway type the following: " + "\n!gstart (time, prize, type)" + "\nTime in seconds or minutes represented with a s,m, or h" + " \nPrize must be 1 word" + "\nType must be one word (Rank, Kit, etc.)```");
           
        }

    }
}



