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
            Response.WithTitle("Current Price of PLE: " + PlenteumBot.marketSource);
            Response.WithUrl(PlenteumBot.marketEndpoint);
            Response.AddField("Low", string.Format("{0} PLE", (decimal)CoinPrice["low"]));
            Response.AddField("Ask", string.Format("{0} PLE", (decimal)CoinPrice["ask"]));
            Response.AddField("Bid", string.Format("{0} PLE", (decimal)CoinPrice["bid"]));
            Response.AddField("High", string.Format("{0} PLE", (decimal)CoinPrice["high"]));
            Response.AddField("Volume", string.Format("{0:N} BTC", (decimal)CoinPrice["volume"]));


            // Send reply
            if (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await ReplyAsync("", false, Response);
            }
            else await Context.Message.Author.SendMessageAsync("", false, Response);
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
                (decimal)CoinPrice["ask"] * (decimal)BTCPrice["last"] * PlenteumBot.GetSupply());

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
            Response.WithTitle("Current PLE Order Book ");
            Response.AddField("Ask", string.Format("Price: {0} PLE    Amount: {1} PLE   Total: {2} BTC", ((decimal)bid["price"]), ((decimal)bid["amount"]), ((decimal)bid["amount2"])));
            Response.AddField("Bid", string.Format("Price: {0} PLE    Amount: {1} PLE   Total: {2} BTC", ((decimal)ask["price"]), ((decimal)ask["amount"]), ((decimal)ask["amount2"])));
                        
            // Send reply
            if (Context.Channel != null && PlenteumBot.marketAllowedChannels.Contains(Context.Channel.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await ReplyAsync("", false, Response);
            }
            else await Context.Message.Author.SendMessageAsync("", false, Response);
        }
    }
}
