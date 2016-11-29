using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
    public static class PokerPlayer
    {
        public static readonly string VERSION = "Version 1.1 Bet on other team losing on pre-flop pair";

        public static Dictionary<string,int> Dictionary = new Dictionary<string, int>()
                {
                    {"1",1 },
                    {"2",2 },
                    {"3",3 },
                    {"4",4 },
                    {"5",5 },
                    {"6",6 },
                    {"7",7 },
                    {"8",8 },
                    {"9",9 },
                    {"10",10 },
                    {"J",11 },
                    {"Q",12 },
                    { "K",13 },
                    {"A",14 },
                };

        public static int BetRequest(JObject gameState)
        {
            Console.Error.WriteLine("Betting round started");
            int value = 0;
            try
            {
                //return 30;

                var current_buy_in = gameState["current_buy_in"].Value<int>();


                var in_action = gameState["in_action"].Value<int>();


                var ourPlayer = gameState["players"][in_action];


                var our_current_bet = ourPlayer["bet"].Value<int>();


                var hole_cards = ourPlayer["hole_cards"];

               



                var cards = new List<Tuple<string, string>>();
                foreach (var card in hole_cards)
                {
                    var rank = card["rank"].Value<string>();
                    var suit = card["suit"].Value<string>();

                    cards.Add(new Tuple<string, string>(rank, suit));

                }
                if (cards.Count == 2)
                {
                    Console.Error.WriteLine("We have " + cards[0].Item1 + " " + cards[1].Item1);
                }

                var community_cards = gameState["community_cards"];

                int betValue;
                bool hasOtherPlay = GetPlay(out betValue,cards, community_cards, ourPlayer);
                if (!hasOtherPlay)
                {
                    return current_buy_in - our_current_bet;
                }



            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
            }

            Console.Error.WriteLine("Playing: " + value);

            //TODO: Use this method to return the value You want to bet
            return value;
        }

        private static bool GetPlay(out int betValue, List<Tuple<string,string>> cards, JToken communityCards, JToken ourPlayer)
        {

            if (!communityCards.HasValues)
            {
                Console.Error.WriteLine("No community cards in place");
                if (cards.Count == 2)
                {
                    // Same rank
                    if (cards[0].Item1 == cards[1].Item1)
                    {
                        Console.Error.WriteLine("We have pairs, going all in");
                        betValue = ourPlayer["stack"].Value<int>();
                        return true;
                    }

                    // Same suit
                    if (cards[0].Item2 == cards[1].Item2)
                    {
                        Console.Error.WriteLine("We have Colors");
                        //value = ourPlayer["stack"].Value<int>();

                        var firstCardValue = Dictionary[cards[0].Item1];
                        var secondCardValue = Dictionary[cards[1].Item1];

                        if (Math.Abs(firstCardValue - secondCardValue) == 1)
                        {
                            // Possible straight flush
                        }
                    }

                }

            }
            betValue = 0;
            return false;
        }

        public static void ShowDown(JObject gameState)
        {
            //TODO: Use this method to showdown
        }
    }
}

