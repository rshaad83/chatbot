using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace FDCBot
{
    public class EchoBot : IBot
    {
        static string lastFourDigitsOfCard = "";
        static string cvv = "";
        static string lastFourDigitsOfSSN = "";

        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            string operation = "activate";
            /*
            string lastFourDigitsOfCard = "";
            string cvv = "";
            string lastFourDigitsOfSSN = "";
            */
            if (context.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                await context.SendActivity("Hello and welcome to my bot.");
            }

            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state from the turn context
                var state = context.GetConversationState<EchoState>();

                // Bump the turn count. 
                state.TurnCount++;

                DialogSet dialogs = new DialogSet();
                dialogs.Add("firstRun", new WaterfallStep[]
                {
                    async (dc, args, next) =>
                    {
                        await dc.Context.SendActivity("Hi...We need to ask a few questions to get started.");
                        await dc.Begin("options");
                    },
                    async (dc, args, next) =>
                    {
                        await dc.Context.SendActivity("Thanks for using our bot service !");
                        await dc.End();
                    }
                });
                dialogs.Add("options", new WaterfallStep[]
                {
                    // Each step takes in a dialog context, arguments, and the next delegate
                    async (dc, args, next) =>
                    {
                        // Prompt for the card activity.
                        await dc.Prompt("operationPrompt","Do you want to activate or deactivate your card?");
                    },
                   
                   
                    async(dc, args, next) =>
                    {
                        operation = (string) args["Text"];
                        await dc.Prompt("cardNumberPrompt", $"I understand that you want to '{operation}' your card! Please enter last 4 digits of your card");
                        
                    }, 
                    async (dc, args, next) =>
                    {
                        lastFourDigitsOfCard = (string) args["Text"];

                        Boolean cardExists = verifyCardNumber(lastFourDigitsOfCard);

                        if(!cardExists)
                        {
                            // show message
                            await dc.Prompt("verificationPrompt", $"Sorry, card with last 4 digits '{lastFourDigitsOfCard}' not found. Please verify and try again");
                        }
                        else {
                            // Prompt for the card verification code
                            await dc.Prompt("verificationPrompt", "We found Your card ending with " + lastFourDigitsOfCard + 
                                ". Please enter the card verification/cvv number");
                        }

                    },

                    async (dc, args, next) =>
                    {
                        cvv = (string) args["Text"];

                        Boolean validCvv = verifyCvvForCard(lastFourDigitsOfCard, cvv);

                        if(!validCvv)
                        {
                            // show message
                            await dc.Prompt("ssnPrompt", $"Sorry, the verificationNumber '{cvv}' not found for the card. Please verify and try again");
                        }
                        else {
                            // Prompt for the card verification code
                            await dc.Prompt("ssnPrompt", $"Found verification number '{cvv}' for card ending with '{lastFourDigitsOfCard}'. " +
                                $"Please enter last four digits of your social security number");
                        }

                    },

                    async (dc, args, next) =>
                    {
                        lastFourDigitsOfSSN = (string) args["Text"];

                        Boolean validSsn = verifySsnForCard(lastFourDigitsOfCard, cvv, lastFourDigitsOfSSN);

                        if(!validSsn)
                        {
                            // show message
                            await dc.Prompt("activationPrompt", $"Sorry, the SSN '{lastFourDigitsOfSSN}' not found for the card. Please verify and try again");
                        }
                        else {

                            Boolean activated = activateCard(lastFourDigitsOfCard);

                            if(activated)
                            {
                                 await dc.Prompt("activationPrompt", $"Card ending with '{lastFourDigitsOfCard}' successfully activated. Thank You for using our bot service !!!");
                            }
                            else
                            {
                            // Prompt for the card verification code
                            await dc.Prompt("activationPrompt", $"Sorry, there was an error activating card ending with '{lastFourDigitsOfCard}'. " +
                                $"Please call us");
                            }
                        }

                    }




                });

                dialogs.Add("operationPrompt", new TextPrompt());
                dialogs.Add("cardNumberPrompt", new TextPrompt());
                dialogs.Add("verificationPrompt", new TextPrompt());
                dialogs.Add("ssnPrompt", new TextPrompt());
                dialogs.Add("activationPrompt", new TextPrompt());



                await createAndBeginDialogs(context, dialogs);

                // Echo back to the user whatever they typed.
                // await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");
            }
        }

        private static async Task createAndBeginDialogs(ITurnContext context, DialogSet dialogs)
        {
            var dialogContext = dialogs.CreateContext(context, context.GetConversationState<EchoState>());
            // await dialogContext.Begin("firstRun");
             
            await dialogContext.Continue();

            if (!context.Responded)
            {
                await dialogContext.Begin("firstRun");
            }
            
        }

        private Boolean verifyCardNumber(string lastFourDigitsOfCard)
        {
            Boolean doesCardExist = CardDAO.isValidCard(lastFourDigitsOfCard);

            return doesCardExist;

        }

        private Boolean verifyCvvForCard(string lastFourDigitsOfCard, string cvv)
        {
            Boolean validCvvForCard = CardDAO.isValidCvvForCard(lastFourDigitsOfCard, cvv);

            return validCvvForCard;

        }

        private Boolean verifySsnForCard(string lastFourDigitsOfCard, string cvv, string ssn)
        {
            Boolean validCvvForCard = CardDAO.isValidSsnForCard(lastFourDigitsOfCard, cvv, ssn);

            return validCvvForCard;

        }

        private bool activateCard(string lastFourDigitsOfCard)
        {
            Boolean validCvvForCard = CardDAO.activateCard(lastFourDigitsOfCard);

            return validCvvForCard;
        }

    }    
}
