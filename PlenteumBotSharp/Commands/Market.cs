using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
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
            Response.AddInlineField("Low", string.Format("{0} BTC", Math.Round((decimal)CoinPrice["low"])));
            Response.AddInlineField("Ask", string.Format("{0} BTC", Math.Round((decimal)CoinPrice["ask"])));
            Response.AddInlineField("Bid", string.Format("{0} BTC", Math.Round((decimal)CoinPrice["bid"])));
            Response.AddInlineField("High", string.Format("{0} BTC", Math.Round((decimal)CoinPrice["high"])));
            Response.AddInlineField("Volume", string.Format("{0:N} BTC", (decimal)CoinPrice["volume"]));


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

            JObject asks = ((JObject)OrderBook["ask"]);
            JObject bids = ((JObject)OrderBook["bid"]);

            // Begin building a response
            var Response = new EmbedBuilder();
            Response.WithTitle("Current PLE Order Book ");
            Response.AddInlineField("Ask", string.Format("Price: {0} PLE    Amount: {1} PLE   Total: {2} BTC", Math.Round((decimal)bids[0]["price"]), Math.Round((decimal)bids[0]["amount"]), Math.Round((decimal)bids[0]["amount2"])));
            Response.AddInlineField("Bid", string.Format("Price: {0} PLE    Amount: {1} PLE   Total: {2} BTC", Math.Round((decimal)asks[0]["price"]), Math.Round((decimal)asks[0]["amount"]), Math.Round((decimal)asks[0]["amount2"])));
                        
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
