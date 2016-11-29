using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "Version 1.1 Bet 1000 on pre-flop pair";

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


		        var cards = new List<Tuple<string, string>>();
		        foreach (var card in hole_cards)
		        {
		            var rank = card["rank"].Value<string>();
		            var suit = card["suit"].Value<string>();

                    cards.Add(new Tuple<string, string>(rank,suit));

		        }
		        if (cards.Count == 2)
		        {
		            Console.Error.WriteLine("We have " + cards[0].Item1 + " " + cards[1].Item1);
		        }


		        var community_cards = gameState["community_cards"];
		        if (!community_cards.HasValues)
		        {
		            Console.Error.WriteLine("No community cards in place");
		            if (cards.Count == 2 && cards[0].Item1 == cards[1].Item1)
		            {
                        Console.Error.WriteLine("We have pairs, going all in");
		                value = 1000;
		            }


		        }
		        else
		        {
                    value = current_buy_in - our_current_bet;
                }


                // foreach()


                // Om vi har bra par gå all in. ( KnKn, DD, KK, EE )


                // Allways check

               


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

