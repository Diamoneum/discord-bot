using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PlenteumBot
{
    partial class PlenteumBot
    {
        // Discord.Net Variables
        public static DiscordSocketClient _client;
        public static CommandService _commands;
        public static IServiceProvider _services;

        // File Variables
        public static string
            configFile = "config.json",
            databaseFile = "users.db";

        // Operation Variables
        public static int
            logLevel = 1;

        // Permission Variables
        [JsonExtensionData]
        public static List<ulong>
            marketAllowedChannels = new List<ulong>
            {
                515407608589320192, //PLE Market Talk Channel
                562944440952684546 //PLE Test
            };

        // Bot Variables
        public static string
            botToken = "0",
            botPrefix = "!";
        public static int
            botMessageCache = 0;

        // Currency Variables
        public static string
            coinName = "Plenteum",
            coinSymbol = "PLE",
            coinAddressPrefix = "PLe";
        public static decimal
            coinUnits = 100;
        public static int
            coinAddressLength = 98;

        // Tipping Variables
        public static decimal
            tipFee = 10;
        public static int
            tipMixin = 0;
        public static string
            tipDefaultAddress = "",
            tipSuccessReact = "👍",
            tipFailedReact = "👎",
            tipLowBalanceReact = "❗",
            tipJoinReact = "👌";
        public static List<string>
            tipAddresses = new List<string>();
        public static Dictionary<string, decimal>
            tipCustomReacts = new Dictionary<string, decimal>();

        // Market Variables
        public static string
            marketSource = "STEX",
            marketEndpoint = "https://api3.stex.com/public/ticker/864",
            bookEndpoint = "https://api3.stex.com/public/orderbook/864",
            marketBTCEndpoint = "https://www.bitstamp.net/api/ticker/";

        // Daemon Variables
        public static string
            daemonHost = "127.0.0.1";
        public static int
            daemonPort = 44016;

        // Wallet Variables
        public static string
            walletHost = "127.0.0.1",
            walletRpcPassword = "password";
        public static int
            walletPort = 8070,
            walletUpdateDelay = 5000;
        
        //mining incentive competition
        public static string
            pools = "https://ple.optimusblue.com:8119/stats_address?address={0}&longpoll=false,"; //comma seperated list of participating pools in the promo

        public static int
            shareDelaySeconds = 60000 * 15; //15 minutes 

    }
}
