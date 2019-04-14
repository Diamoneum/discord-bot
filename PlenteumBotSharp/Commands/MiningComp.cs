using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlenteumBot
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        /* TODO: Build Mining Promo Commands - keep things simple! */
        [Command("entercomp")]
        public async Task EnterCompAsync(string Address, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check that user has registered an address
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You have not registered an address, use {0}registerwallet <your PLE address> to register your wallet", PlenteumBot.botPrefix));

            // Check that user has not already registered for the competition
            if (PlenteumBot.CheckUserEntered(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You have already registered for the mining promotion! Good Luck!", PlenteumBot.botPrefix));

            // Check address validity
            else if (!PlenteumBot.VerifyAddress(Address))
                await Context.Message.Author.SendMessageAsync(string.Format("Address is not a valid {0} address!", PlenteumBot.coinName));

            // Check that address isn't in use by another user
            else if (PlenteumBot.CheckAddressExists(Address))
                await Context.Message.Author.SendMessageAsync("Address is in use by another user");

            // Passed checks
            else
            {
                // Register wallet into database
                string PaymentId = PlenteumBot.RegisterWallet(Context.Message.Author.Id, Address);

                // Begin building a response
                var Response = new EmbedBuilder();
                Response.WithTitle("Successfully registered you for the mining promotion!");
                Response.Description = string.Format("Deposit {0} to start tipping!\n\n" +
                    "Address:\n**{1}**\n\nPayment ID:\n**{2}**", PlenteumBot.coinSymbol, PlenteumBot.tipDefaultAddress, PaymentId);

                // Send reply
                await Context.Message.Author.SendMessageAsync("", false, Response);
            }
        }

        [Command("pools")]
        public async Task PoolsAsync([Remainder]string Remainder = "")
        {
            //return a list of pools participating in the promotion
            var Response = new EmbedBuilder();
            Response.WithTitle("Successfully registered you for the mining promotion!");
            Response.Description = string.Format("Deposit {0} to start tipping!\n\n");

            // Send reply
            await Context.Message.Author.SendMessageAsync("", false, Response);
        }
        
    }
}
