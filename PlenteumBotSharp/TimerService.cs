using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PlenteumBot
{
    public class TimerService
    {
        private readonly Timer _timer; // 2) Add a field like this
                                       // This example only concerns a single timer.
                                       // If you would like to have multiple independant timers,
                                       // you could use a collection such as List<Timer>,
                                       // or even a Dictionary<string, Timer> to quickly get
                                       // a specific Timer instance by name.

        public TimerService(DiscordSocketClient client)
        {
            _timer = new Timer(async _ =>
            {
                // 3) Any code you want to periodically run goes here, for example:
                var server = client.Guilds.Where(x => x.Name.StartsWith("Plenteum")).FirstOrDefault();
                if (server != null)
                {
                    if (!server.HasAllMembers)
                    {
                        PlenteumBot.Log(0, "PlenteumBot", "Downloading users");
                        await server.DownloadUsersAsync(); //ensure all users are downloaded
                        PlenteumBot.Log(0, "PlenteumBot", "Users Downloaded");
                    }
                    else {
                        PlenteumBot.Log(0, "PlenteumBot", "Users already Downloaded");
                    }
                }
            },
            null,
            TimeSpan.FromMinutes(1),  // 4) Time that download users should fire
            TimeSpan.FromMinutes(30)); // 5) Time after which download should repeat (use `Timeout.Infinite` for no repeat)
        }

        public void Stop() // 6) Example to make the timer stop running
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Start() // 7) Example to start the timer
        {
            _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(30));
        }
    }
}
