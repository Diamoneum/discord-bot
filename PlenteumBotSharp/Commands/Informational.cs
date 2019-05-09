using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace PlenteumBot
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync([Remainder]string Remainder = "")
        {
            // Begin building a response
            EmbedBuilder Response = new EmbedBuilder();
            Response.WithTitle("Help");
            string Output = "";

            if (Remainder.ToLower() == "hashrate")
            {
                Response.Title += string.Format(" - {0}hashrate", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}hashrate", PlenteumBot.botPrefix));
                Response.AddField("Description:", "Gives the current network hashrate");
            }
            else if (Remainder.ToLower() == "difficulty")
            {
                Response.Title += string.Format(" - {0}difficulty", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}difficulty", PlenteumBot.botPrefix));
                Response.AddField("Description:", "Gives the current network difficulty");
            }
            else if (Remainder.ToLower() == "height")
            {
                Response.Title += string.Format(" - {0}height", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}height", PlenteumBot.botPrefix));
                Response.AddField("Description:", "Gives the current network height");
            }
            else if (Remainder.ToLower() == "supply")
            {
                Response.Title += string.Format(" - {0}supply", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}supply", PlenteumBot.botPrefix));
                Response.AddField("Description:", string.Format("Gives the total circulating supply of {0}", PlenteumBot.coinSymbol));
            }
            else if (Remainder.ToLower() == "registerwallet")
            {
                Response.Title += string.Format(" - {0}registerwallet", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}registerwallet <{1} Address>", PlenteumBot.botPrefix, PlenteumBot.coinSymbol));
                Response.AddField("Description:", "Registers your address with the bot so you may send and recieve tips");
            }
            else if (Remainder.ToLower() == "updatewallet")
            {
                Response.Title += string.Format(" - {0}updatewallet", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}updatewallet <{1} Address>", PlenteumBot.botPrefix, PlenteumBot.coinSymbol));
                Response.AddField("Description:", "Updates your registered wallet to a new address");
            }
            else if (Remainder.ToLower() == "wallet")
            {
                Response.Title += string.Format(" - {0}uwallet", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}wallet\n{0}wallet <{1} Address>", PlenteumBot.botPrefix, PlenteumBot.coinSymbol));
                Response.AddField("Description:", "Gets a specified user's registered wallet address, or your own if no address is specified");
            }
            else if (Remainder.ToLower() == "deposit")
            {
                Response.Title += string.Format(" - {0}deposit", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}deposit", PlenteumBot.botPrefix));
                Response.AddField("Description:", string.Format("DMs you with your deposit information, including the address to send to, " +
                    "and the payment ID you **must** use when sending {0}", PlenteumBot.coinSymbol));
            }
            else if (Remainder.ToLower() == "withdraw")
            {
                Response.Title += string.Format(" - {0}withdraw", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}withdraw <Amount of {1}>", PlenteumBot.botPrefix, PlenteumBot.coinSymbol));
                Response.AddField("Description:", "Withdraws a specified amount from your tip jar into your registered wallet");
            }
            else if (Remainder.ToLower() == "balance")
            {
                Response.Title += string.Format(" - {0}balance", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}balance", PlenteumBot.botPrefix, PlenteumBot.coinSymbol));
                Response.AddField("Description:", "Gets your current tip jar balance");
            }
            else if (Remainder.ToLower() == "tip")
            {
                Response.Title += string.Format(" - {0}tip", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}tip <Amount of {1}> @Users1 @User2...\n{0}tip <Amount of {1}> <{1} Address>", //\n{0}tip <Amount of {1}> @role", 
                    PlenteumBot.botPrefix, PlenteumBot.coinSymbol));
                Response.AddField("Description:", string.Format("Sends a tip of a specified amount to one or more users\n*or* a specified {0} address", //\n*or* all users in the mentioned role\n**note**:tip amount sent to individuals will = specified amount / number of users", 
                    PlenteumBot.coinSymbol));
            }
            else if (Remainder.ToLower() == "redirecttips")
            {
                Response.Title += string.Format(" - {0}redirecttips", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}redirecttips\n{0}redirecttips <True or False>", PlenteumBot.botPrefix));
                Response.AddField("Description:", "Sets whether you'd like to have tips sent to you to go directly to your registered wallet " +
                    "(default) or redirected into your tip jar balance");
            }

            else if ((Remainder.ToLower() == "price" && (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))) || (Remainder.ToLower() == "price" && Context.IsPrivate))
            {
                Response.Title += string.Format(" - {0}price", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}price", PlenteumBot.botPrefix));
                Response.AddField("Description:", string.Format("Gives the current prices of {0} in BTC from STEX", PlenteumBot.coinSymbol));
            }
            else if ((Remainder.ToLower() == "mcap" && (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))) || (Remainder.ToLower() == "mcap" && Context.IsPrivate))
            {
                Response.Title += string.Format(" - {0}mcap", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}mcap", PlenteumBot.botPrefix));
                Response.AddField("Description:", string.Format("Gives {0}'s current market capitalization", PlenteumBot.coinSymbol));
            }
            else if ((Remainder.ToLower() == "book" && (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))) || (Remainder.ToLower() == "book" && Context.IsPrivate))
            {
                Response.Title += string.Format(" - {0}book", PlenteumBot.botPrefix);
                Response.AddField("Usage:", string.Format("{0}book", PlenteumBot.botPrefix));
                Response.AddField("Description:", string.Format("Gives {0}'s current order book on STEX", PlenteumBot.coinSymbol));
            }
            //competition related commands
            //else if (Remainder.ToLower() == "pools")
            //{
            //    Response.Title += string.Format(" - {0}pools", PlenteumBot.botPrefix);
            //    Response.AddField("Usage:", string.Format("{0}pools", PlenteumBot.botPrefix));
            //    Response.AddField("Description:", "Displays a list of pools participating in the current mining promotion");
            //}
            //else if (Remainder.ToLower() == "winners")
            //{
            //    Response.Title += string.Format(" - {0}winners", PlenteumBot.botPrefix);
            //    Response.AddField("Usage:", string.Format("{0}winners", PlenteumBot.botPrefix));
            //    Response.AddField("Description:", "Displays a list of recent mining promotion winners");
            //}


            // No requested command
            else
            {
                Output += "__**Informational**__:\n";
                Output += "  **{0}help**:  Lists all available commands\n";
                Output += "  **{0}help <command>**:  Lists information about a specific command\n";
                //Output += "  faucet\tGives faucet information\n";
                Output += "__**Network**__:\n";
                Output += "  **{0}hashrate**:  Gives current network hashrate\n";
                Output += "  **{0}difficulty**:  Gives current network difficulty\n";
                Output += "  **{0}height**:  Gives current network height\n";
                Output += "  **{0}supply**:  Gives current circulating supply\n";
                if (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id) || (Context.IsPrivate))
                {
                    Output += "__**Market**__:\n";
                    Output += "  **{0}price**:  Gives current price in BTC\n";
                    Output += "  **{0}book**:  Gives current order book\n";
                    Output += "  **{0}mcap**:  Gives current global marketcap\n";
                }
                Output += "__**Tipping**__:\n";
                Output += "  **{0}registerwallet**:  Registers your wallet with the tip bot\n";
                Output += "  **{0}updatewallet**:  Updates your registered wallet\n";
                Output += "  **{0}wallet**:  Gives the wallet address for a specified user or your own address if no user is specified\n";
                Output += "  **{0}deposit**:  Gives information on how to deposit into your tipping balance\n";
                Output += "  **{0}withdraw**:  Withdraws a specified amount from your tip jar into your registered wallet\n";
                Output += "  **{0}balance**:  Gives your current tip jar balance\n";
                Output += "  **{0}tip**:  Tips one or more users a specified amount\n";
                Output += "  **{0}redirecttips**:  Sets whether you'd like tips sent directly to your wallet or redirected back into your tip jar";
                Response.WithDescription(string.Format(Output, PlenteumBot.botPrefix));
                Response.WithTitle("Available Commands:");
            }

            // Send reply
            await ReplyAsync("", false, Response.Build());
        }

        //[Command("faucet")]
        //public async Task FaucetAsync([Remainder]string Remainder = "")
        //{
        //    // Get faucet balance
        //    JObject FaucetBalance = Request.GET(PlenteumBot.faucetEndpoint);
        //    if (FaucetBalance.Count < 1)
        //    {
        //        await ReplyAsync("Failed to connect to faucet");
        //        return;
        //    }

        //    // Begin building a response
        //    var Response = new EmbedBuilder();
        //    Response.WithTitle(string.Format("This faucet has {0:N} {1} left", (decimal)FaucetBalance["available"], PlenteumBot.coinSymbol));
        //    Response.WithUrl(PlenteumBot.faucetHost);
        //    Response.Description = "```Donations:\n" + PlenteumBot.faucetAddress + "```\n";

        //    // Send reply
        //    await ReplyAsync("", false, Response);
        //}
    }
}
