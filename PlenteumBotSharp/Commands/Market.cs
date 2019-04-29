using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlenteumBot
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("price")]
        public async Task PriceAsync([Remainder]string Remainder = "")
        {
            // Get current coin price
            JObject CoinPrice = ((JObject)(Request.GET(PlenteumBot.marketEndpoint)["data"]));
            if (CoinPrice.Count < 1)
            {
                await ReplyAsync("Failed to connect to STEX API (" + PlenteumBot.marketSource + ")");
                return;
            }

            // Begin building a response
            var Response = new EmbedBuilder();
            Response.WithTitle("Current Price of PLE on " + PlenteumBot.marketSource);
            Response.WithUrl(PlenteumBot.marketEndpoint);
            Response.Description += string.Format("```Low:     {0} BTC\n", (decimal)CoinPrice["low"]);
            Response.Description += string.Format("Ask:     {0} BTC\n", (decimal)CoinPrice["ask"]);
            Response.Description += string.Format("Bid:     {0} BTC\n", (decimal)CoinPrice["bid"]);
            Response.Description += string.Format("High:    {0} BTC\n", (decimal)CoinPrice["high"]);
            Response.Description += string.Format("Volume:  {0:N} BTC\n```", (decimal)CoinPrice["volume"]);


            // Send reply
            if (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await ReplyAsync("", false, Response.Build());
            }
            else await Context.Message.Author.SendMessageAsync("", false, Response.Build());
        }

        [Command("mcap")]
        public async Task MarketCapAsync([Remainder]string Remainder = "")
        {
            // Get current coin price
            JObject CoinPrice = ((JObject)(Request.GET(PlenteumBot.marketEndpoint)["data"]));
            if (CoinPrice.Count < 1)
            {
                await ReplyAsync("Failed to connect to STEX API (" + PlenteumBot.marketSource + ")");
                return;
            }

            // Get current BTC price
            JObject BTCPrice = Request.GET(PlenteumBot.marketBTCEndpoint);
            if (BTCPrice.Count < 1)
            {
                await ReplyAsync("Failed to connect to BTC API (" + PlenteumBot.marketBTCEndpoint + ")");
                return;
            }

            // Begin building a response
            string Response = string.Format("{0}'s market cap is **{1:c}** USD", PlenteumBot.coinName,
                (decimal)CoinPrice["bid"] * (decimal)BTCPrice["last"] * PlenteumBot.GetSupply());

            // Send reply
            if (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await ReplyAsync(Response);
            }
            else await Context.Message.Author.SendMessageAsync(Response);
        }

        [Command("book")]
        public async Task BookAsync([Remainder]string Remainder = "")
        {
            // Get current order Book
            JObject OrderBook = ((JObject)(Request.GET(PlenteumBot.bookEndpoint)["data"]));
            if (OrderBook.Count < 1)
            {
                await ReplyAsync("Failed to connect to STEX API (" + PlenteumBot.marketSource +")");
                return;
            }

            JObject ask = ((JArray)OrderBook["ask"]).ToObject<List<JObject>>().FirstOrDefault();
            JObject bid = ((JArray)OrderBook["bid"]).ToObject<List<JObject>>().FirstOrDefault();

            // Begin building a response
            var Response = new EmbedBuilder();
            Response.WithTitle("Current Order Book of PLE on " + PlenteumBot.marketSource);
            Response.WithUrl(PlenteumBot.bookEndpoint);
            Response.AddField("Ask", string.Format("```Price:    {0} BTC\nAmount:   {1} PLE\nTotal:    {2} BTC```", ((decimal)bid["price"]), Math.Round((decimal)bid["amount"], 2), ((decimal)bid["amount2"])));
            Response.AddField("Bid", string.Format("```Price:    {0} BTC\nAmount:   {1} PLE\nTotal:    {2} BTC```", ((decimal)ask["price"]), Math.Round((decimal)ask["amount"], 2), ((decimal)ask["amount2"])));
                        
            // Send reply
            if (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await ReplyAsync("", false, Response.Build());
            }
            else await Context.Message.Author.SendMessageAsync("", false, Response.Build());
        }
    }
}
