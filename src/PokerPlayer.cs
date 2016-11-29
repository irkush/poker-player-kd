using System;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "Version 1.1 CheckAll With Logging";

		public static int BetRequest(JObject gameState)
		{
            Console.Error.WriteLine("Betting round started");
		    int value = 0;
		    try
		    {
                //return 30;

                var current_buy_in = gameState["current_buy_in"].Value<int>();


                var in_action = gameState["in_action"].Value<int>();

                var our_current_bet = gameState["players"][in_action]["bet"].Value<int>();


		        var hole_cards = gameState["players"][in_action]["hole_cards"];

                // foreach()


                // Om vi har bra par gå all in. ( KnKn, DD, KK, EE )


                // Allways check

                value = current_buy_in - our_current_bet;


            }
            catch (Exception ex)
		    {
		        Console.Error.Write(ex);
		    }

            Console.Error.WriteLine("Playing: " + value);

			//TODO: Use this method to return the value You want to bet
			return value;
		}

		public static void ShowDown(JObject gameState)
		{
			//TODO: Use this method to showdown
		}
	}
}

