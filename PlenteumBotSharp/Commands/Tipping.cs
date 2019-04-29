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
        [Command("registerwallet")]
        public async Task RegisterWalletAsync(string Address, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check that user hasn't already registered an address
            if (PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You have already registered an address, use {0}updatewallet if you'd like to update it", PlenteumBot.botPrefix));

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
                Response.WithTitle("Successfully registered your wallet!");
                Response.Description = string.Format("Deposit {0} to start tipping!\n\n" +
                    "Address:\n**{1}**\n\nPayment ID:\n**{2}**", PlenteumBot.coinSymbol, PlenteumBot.tipDefaultAddress, PaymentId);

                // Send reply
                await Context.Message.Author.SendMessageAsync("", false, Response.Build());
            }
        }

        [Command("updatewallet")]
        public async Task UpdateWalletAsync(string Address, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check that user has registered an address, register it if not.
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
            {
                await RegisterWalletAsync(Address, Remainder);
                return;
            }

            // Check address validity
            else if (!PlenteumBot.VerifyAddress(Address))
                await Context.Message.Author.SendMessageAsync(string.Format("Address is not a valid {0} address!", PlenteumBot.coinName));

            // Check that address isn't in use by another user
            else if (PlenteumBot.CheckAddressExists(Address))
                await Context.Message.Author.SendMessageAsync("Address is in use by another user");

            // Passed checks
            else
            {
                // Update address in database
                PlenteumBot.UpdateWallet(Context.Message.Author.Id, Address);

                // Reply with success
                await Context.Message.Author.SendMessageAsync("Successfully updated your wallet");
            }
        }

        [Command("redirecttips")]
        public async Task RedirectTipsAsync([Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check that user has registered an address
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You must register a wallet before you can recieve tips! Use {0}help if you need any help.", PlenteumBot.botPrefix));

            // User is registered
            else
            {
                // Check if user is redirecting tips
                bool Redirect = PlenteumBot.GetRedirect(Context.Message.Author.Id);

                // Set new value
                if (Redirect) Redirect = false;
                else Redirect = true;

                // Set redirect preference
                PlenteumBot.SetRedirect(Context.Message.Author.Id, Redirect);

                // Send reply
                if (Redirect) await Context.Message.Author.SendMessageAsync("**Tip redirect preference changed**\nTips you receive will now go to your tip jar");
                else await Context.Message.Author.SendMessageAsync("**Tip redirect preference changed**\nTips you receive will now go to your registered wallet");
            }
        }
        [Command("redirecttips")]
        public async Task RedirectTipsAsync(bool Redirect, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check that user has registered an address
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You must register a wallet before you can recieve tips! Use {0}help if you need any help.", PlenteumBot.botPrefix));

            // User is registered
            else
            {
                // Set redirect preference
                PlenteumBot.SetRedirect(Context.Message.Author.Id, Redirect);

                // Send reply
                if (Redirect) await Context.Message.Author.SendMessageAsync("**Tip redirect preference changed**\nTips you receive will now go to your tip jar");
                else await Context.Message.Author.SendMessageAsync("**Tip redirect preference changed**\nTips you receive will now go to your registered wallet");
            }
        }

        [Command("wallet")] // Get own wallet
        public async Task WalletAsync([Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Try to grab address from the database
            string Address = "";
            if (PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                Address = PlenteumBot.GetAddress(Context.Message.Author.Id);

            // Check if result is empty
            if (string.IsNullOrEmpty(Address))
                await Context.Message.Author.SendMessageAsync(string.Format("You haven't registered a wallet! Use {0}help if you need any help.",
                    PlenteumBot.botPrefix));

            // Check if user is requesting their own wallet
            else await Context.Message.Author.SendMessageAsync(string.Format("**Your wallet:**```{0}```", Address));
        }
        [Command("wallet")] // Get by mention
        public async Task WalletAsync(SocketUser User, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Try to grab address from the database
            string Address = "";
            if (PlenteumBot.CheckUserExists(User.Id))
                Address = PlenteumBot.GetAddress(User.Id);

            // Check if result is empty
            if (string.IsNullOrEmpty(Address))
                await Context.Message.Author.SendMessageAsync(string.Format("{0} hasn't registered a wallet!", User.Username));

            // Check if user is requesting their own wallet
            else if (User == null || Context.Message.Author.Id == User.Id)
                await Context.Message.Author.SendMessageAsync(string.Format("**Your wallet:**```{0}```", Address));

            // User is requesting someone else's wallet
            else await Context.Message.Author.SendMessageAsync(string.Format("**{0}'s wallet:**```{1}```", User.Username, Address));
        }
        [Command("wallet")] // Get by uid
        public async Task WalletAsync(ulong UID, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Try to grab address from the database
            string Address = "";
            if (PlenteumBot.CheckUserExists(UID))
                Address = PlenteumBot.GetAddress(UID);

            // Get requested user
            string Username = "";
            SocketUser User = Context.Client.GetUser(UID);
            if (User != null) Username = User.Username;
            if (string.IsNullOrEmpty(Username)) Username = UID.ToString();

            // Check if result is empty
            if (string.IsNullOrEmpty(Address))
                await Context.Message.Author.SendMessageAsync(string.Format("{0} hasn't registered a wallet!", Username));

            // Check if user is requesting their own wallet
            else if (Context.Message.Author.Id == UID)
                await Context.Message.Author.SendMessageAsync(string.Format("**Your wallet:**```{0}```", Address));

            // User is requesting someone else's wallet
            else await Context.Message.Author.SendMessageAsync(string.Format("**{0}'s wallet:**```{1}```", Username, Address));
        }

        [Command("deposit")]
        public async Task DepositAsync([Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check if user exists in user table
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You must register a wallet before you can deposit! Use {0}help if you need any help.",
                    PlenteumBot.botPrefix));

            // Send reply
            else await Context.Message.Author.SendMessageAsync(string.Format("**Deposit {0} to start tipping!**```Address:\n{1}\n\nPayment ID:\n{2}```", 
                PlenteumBot.coinSymbol, PlenteumBot.tipDefaultAddress, PlenteumBot.GetPaymentId(Context.Message.Author.Id)));
        }

        [Command("withdraw")]
        public async Task WithdrawAsync(string Amount, [Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check if user exists in user table
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
            {
                await Context.Message.Author.SendMessageAsync(string.Format("You must register a wallet before you can withdraw! Use {0}help if you need any help.", PlenteumBot.botPrefix));
                return;
            }

            // Check that amount is over the minimum fee
            else if (Convert.ToDecimal(Amount) < PlenteumBot.Minimum)//PlenteumBot.Fee)
            {
                await ReplyAsync(string.Format("Amount must be at least {0:N} {1}", PlenteumBot.Minimum/*Fee*/, PlenteumBot.coinSymbol));
                return;
            }

            // Check if user has enough balance
            else if (PlenteumBot.GetBalance(Context.Message.Author.Id) < Convert.ToDecimal(Amount) + PlenteumBot.tipFee)
            {
                await Context.Message.Author.SendMessageAsync(string.Format("Your balance is too low! Amount + Fee = **{0:N}** {1}",
                    Convert.ToDecimal(Amount) + PlenteumBot.tipFee, PlenteumBot.coinSymbol));
                await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipLowBalanceReact));
            }

            // Send withdrawal
            else if (PlenteumBot.Tip(Context.Message.Author.Id, PlenteumBot.GetAddress(Context.Message.Author.Id), Convert.ToDecimal(Amount)))
            {
                // Send success react
                await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipSuccessReact));
            }
        }

        [Command("balance")]
        public async Task BalanceAsync([Remainder]string Remainder = "")
        {
            // Delete original message
            try { await Context.Message.DeleteAsync(); }
            catch { }

            // Check if user exists in user table
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
                await Context.Message.Author.SendMessageAsync(string.Format("You must register a wallet before you can check your tip jar balance! Use {0}help if you need any help.",
                    PlenteumBot.botPrefix));

            // Send reply with balance
            else
            {
                // Get balance
                decimal Balance = PlenteumBot.GetBalance(Context.Message.Author.Id);

                // Send reply
                await Context.Message.Author.SendMessageAsync(string.Format("You have **{0:N}** {1} in your tip jar", Balance, PlenteumBot.coinSymbol));
            }
        }

        [Command("tip")]
        public async Task TipAsync(string Amount, [Remainder]string Remainder = "")
        {
            // Check if user exists in user table
            if (!PlenteumBot.CheckUserExists(Context.Message.Author.Id))
            {
                await Context.Message.Author.SendMessageAsync(string.Format("You must register a wallet before you can tip! Use {0}help if you need any help.", PlenteumBot.botPrefix));
                return;
            }

            // Check that amount is over the minimum fee
            if (Convert.ToDecimal(Amount) < PlenteumBot.Minimum)
            {
                await ReplyAsync(string.Format("Amount must be at least {0:N} {1}", PlenteumBot.Minimum/*Fee*/, PlenteumBot.coinSymbol));
                return;
            }

            // Check if an address is specified instead of mentioned users
            string Address = "";
            if (Remainder.StartsWith(PlenteumBot.coinAddressPrefix) && Remainder.Length == PlenteumBot.coinAddressLength)
                Address = Remainder.Substring(0, 99);

            //
            List<ulong> Users = new List<ulong>();

            decimal tipAmount = 0;
            //check if "here" or "everyone" is mentioned
            bool multiUser = false;
            bool hasEveryone = false;
            bool hasHere = false;
            if (string.IsNullOrEmpty(Address) && (Context.Message.Tags.Count > 0 || Context.Message.MentionedRoles.Count > 0))
            {
                //first ensure we have all members downloaded
                if (!Context.Guild.HasAllMembers)
                    await Context.Guild.DownloadUsersAsync();

                foreach (var tag in Context.Message.Tags)
                {
                    if (tag.Type == TagType.EveryoneMention)
                    {
                        //add each member to the tip
                        foreach (var user in Context.Guild.Users)
                        {
                            if (!Users.Contains(user.Id) && !user.IsBot && !user.IsWebhook) //only add if they not already in the list of users
                                Users.Add(user.Id);
                        }
                        hasEveryone = true;

                    }
                    else if (tag.Type == TagType.HereMention)
                    {
                        foreach (var user in Context.Guild.Users)
                        {
                            //only add if they not already in the list of users & are Online or Idle or AFK
                            if (!Users.Contains(user.Id) && !user.IsBot && !user.IsWebhook && (user.Status == UserStatus.Online || user.Status == UserStatus.Idle || user.Status == UserStatus.AFK))
                                Users.Add(user.Id);
                        }
                        hasHere = true;
                    }
                    else if (tag.Type == TagType.RoleMention)
                    {
                        foreach (var role in Context.Message.MentionedRoles)
                        {
                            foreach (var user in Context.Guild.Users)
                            {
                                //only add if they not already in the list of users & are in one of the mentioned roles
                                if (!Users.Contains(user.Id) && user.Roles.Contains(role) && !user.IsBot && !user.IsWebhook)
                                    Users.Add(user.Id);
                            }
                        }
                    }
                }

                //get users in Mentioned Roles if there is no "RoleMention" tag and the Users List is empty
                if (Context.Message.MentionedRoles.Count > 0 && Users.Count == 0)
                {
                    foreach (var role in Context.Message.MentionedRoles)
                    {
                        foreach (var user in Context.Guild.Users)
                        {
                            //only add if they not already in the list of users & are in one of the mentioned roles
                            if (!Users.Contains(user.Id) && user.Roles.Contains(role))
                                Users.Add(user.Id);
                        }
                    }
                }

                //check if everyone or here is mentioned by a tipper who is not allowed to mention @everyone or @here
                //first ensure we have all members downloaded
                if (Users.Count <= 0)
                {
                    if (!Context.Guild.HasAllMembers)
                        await Context.Guild.DownloadUsersAsync();

                    if (Remainder.ToLower().IndexOf("@everyone") > -1 && !hasEveryone)
                    {
                        //everyone was mentioned by a tipper who does not have permission to mention everyone
                        //so, discord will prevent notifying everyone, but the tip should still be sent
                        foreach (var user in Context.Guild.Users)
                        {
                            if (!Users.Contains(user.Id) && !user.IsBot && !user.IsWebhook) //only add if they not already in the list of users
                                Users.Add(user.Id);
                        }
                    }

                    if (Remainder.ToLower().IndexOf("@here") > -1 && !hasHere)
                    {
                        //everyone was mentioned by a tipper who does not have permission to mention @here
                        //so, discord will prevent notifying @here, but the tip should still be sent
                        foreach (var user in Context.Guild.Users)
                        {
                            //only add if they not already in the list of users & are Online or Idle or AFK
                            if (!Users.Contains(user.Id) && !user.IsBot && !user.IsWebhook && (user.Status == UserStatus.Online || user.Status == UserStatus.Idle || user.Status == UserStatus.AFK))
                                Users.Add(user.Id);
                        }
                    }
                }

                //check there are now users to tip
                if (Users.Count <= 0)
                {
                    await Context.Message.Author.SendMessageAsync(string.Format("There were no users found with the mentioned role!", PlenteumBot.botPrefix));
                    await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipFailedReact));
                    return;
                }
                else
                {
                    //we're tipping users that do or don't have a wallet
                    multiUser = true;
                }
                //divide the tip Amount by the number of users
                tipAmount = Convert.ToDecimal(Amount) / Users.Count;
                
                //ensure round down and limit to two decimal places... 
                tipAmount = Math.Floor(tipAmount * 100) / 100; 
                //check that the splitAmount is LESS than or equal to the specified amount.. 
                if (tipAmount * Users.Count > Convert.ToDecimal(Amount))
                {
                    await Context.Message.Author.SendMessageAsync(string.Format("Something went wrong! The total tip amount is greater than the specified tip amount", PlenteumBot.botPrefix));
                    await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipFailedReact));
                    return;
                }

                if (tipAmount < PlenteumBot.Minimum)
                {
                    await Context.Message.Author.SendMessageAsync(string.Format("The amount you've specified would result in the individual tip amounts being less than the minimum tip amount!", PlenteumBot.botPrefix));
                    await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipFailedReact));
                    return;
                }

            }
            // Check that there is at least one mentioned user
            if (Address == "" && Context.Message.MentionedUsers.Count < 1 && !multiUser) return;

            // Remove duplicate mentions
            foreach (SocketUser MentionedUser in Context.Message.MentionedUsers)
                Users.Add(MentionedUser.Id);

            Users = Users.Distinct().ToList();

            // Create a list of users that have wallets - tippableUsers
            List<ulong> TippableUsers = new List<ulong>();
            foreach (ulong Id in Users)
            {
                if (PlenteumBot.CheckUserExists(Id) && Id != Context.Message.Author.Id)
                    TippableUsers.Add(Id);
            }



            // Check that user has enough balance for the tip
            if (Address == "" && PlenteumBot.GetBalance(Context.Message.Author.Id) < Convert.ToDecimal(Amount) * TippableUsers.Count + PlenteumBot.tipFee)
            {
                await Context.Message.Author.SendMessageAsync(string.Format("Your balance is too low! Amount + Fee = **{0:N}** {1}",
                    Convert.ToDecimal(Amount) * TippableUsers.Count + PlenteumBot.tipFee, PlenteumBot.coinSymbol));
                await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipLowBalanceReact));
            }
            else if (PlenteumBot.GetBalance(Context.Message.Author.Id) < Convert.ToDecimal(Amount) + PlenteumBot.tipFee)
            {
                await Context.Message.Author.SendMessageAsync(string.Format("Your balance is too low! Amount + Fee = **{0:N}** {1}",
                    Convert.ToDecimal(Amount) + PlenteumBot.tipFee, PlenteumBot.coinSymbol));
                await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipLowBalanceReact));
            }

            // Tip has required arguments
            else
            {
                if (Address == "")
                {

                    //if the splitAmount is > 0, tp that amount instead of the specified amount
                    if (tipAmount == 0)
                        tipAmount = Convert.ToDecimal(Amount);

                    // Send a failed react if a user isn't found
                    bool FailReactAdded = false;
                    foreach (ulong User in Users)
                        if (User != Context.Message.Author.Id && !TippableUsers.Contains(User))
                        {
                            if (!FailReactAdded)
                            {
                                await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipFailedReact));
                                FailReactAdded = true;
                            }

                            try {
                                // Begin building a response
                                var Response = new EmbedBuilder();
                                Response.WithTitle(string.Format("{0} wants to tip you!", Context.Message.Author.Username));
                                Response.Description = string.Format("Register your wallet with with `{0}registerwallet <your {1} address>` " +
                                    "to get started!\nTo create a wallet head to https://wallet.plenteum.com/",
                                    PlenteumBot.botPrefix, PlenteumBot.coinSymbol);

                                // Send reply
                                await Context.Client.GetUser(User).SendMessageAsync("", false, Response.Build());
                            }
                            catch { }
                        }

                    // Check that there is at least one user with a registered wallet
                    if (TippableUsers.Count > 0 && PlenteumBot.Tip(Context.Message.Author.Id, TippableUsers, tipAmount, Context.Message))
                    {
                        // Send success react
                        await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipSuccessReact));
                    }
                }
                else if (PlenteumBot.Tip(Context.Message.Author.Id, Address, Convert.ToDecimal(Amount)))
                {
                    // Send success react
                    await Context.Message.AddReactionAsync(new Emoji(PlenteumBot.tipSuccessReact));
                }
            }
        }
    }
}
