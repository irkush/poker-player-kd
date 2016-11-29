using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
    public static class PokerPlayer
    {
        public static readonly string VERSION = "Version 1.337 Other teams are going?";

        public static Dictionary<string,int> Dictionary = new Dictionary<string, int>()
                {
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
            Console.Error.WriteLine("Betting round started " + VERSION);
            int value = 0;
            try
            {
                //return 30;

                var current_buy_in = gameState["current_buy_in"].Value<int>();


                var in_action = gameState["in_action"].Value<int>();


                var ourPlayer = gameState["players"][in_action];


                var our_current_bet = ourPlayer["bet"].Value<int>();


                var hole_cards = ourPlayer["hole_cards"];


                var small_blind = gameState["small_blind"].Value<int>();


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
                bool hasOtherPlay = GetPlay(out betValue,cards, community_cards, ourPlayer, current_buy_in, small_blind);
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

        private static bool GetPlay(out int betValue, List<Tuple<string,string>> cards, JToken communityCards, JToken ourPlayer, int currentBuyIn, int smallBlind)
        {
            // Pre-flop
            if (!communityCards.HasValues)
            {
                Console.Error.WriteLine("No community cards in place");
                // We have both cards on hand.
                if (cards.Count == 2)
                {
                    // Our hand

                    // Same rank and suit, go all in.
                    if (cards[0].Item1 == cards[1].Item1)
                        
                    {
                        // ALl in på bra kort.
                        if (Dictionary[cards[0].Item1] > 10)
                        {
                            Console.Error.WriteLine("We have pairs, going all in");
                            betValue = ourPlayer["stack"].Value<int>();
                            return true;
                        }
                        else
                        {
                            // Om vi har ett "sämre" par så lägger höjer vi med 2 * smallBlind.
                            betValue = currentBuyIn - ourPlayer["bet"].Value<int>() + smallBlind*2;
                            return true;
                        }
                    }

                    // Same suit and close difference, bet small blind.
                    if (cards[0].Item2 == cards[1].Item2)
                    {
                        Console.Error.WriteLine("We have Colors");

                        var firstCardValue = Dictionary[cards[0].Item1];
                        var secondCardValue = Dictionary[cards[1].Item1];

                        // Need to account for Ace being 1 or 14.
                        if (firstCardValue == 2 || secondCardValue == 2)
                        {
                            if (secondCardValue == 14 || firstCardValue == 14)
                            {
                                betValue = currentBuyIn - ourPlayer["bet"].Value<int>() + smallBlind * 2;
                                return true;
                            }
                        }

                        if (Math.Abs(firstCardValue - secondCardValue) < 3)
                        {
                            betValue = currentBuyIn - ourPlayer["bet"].Value<int>() + smallBlind * 2;
                            return true;
                        }
                    }


                    //Someone has raised/all in
                    if (ourPlayer["bet"].Value<int>() < currentBuyIn)
                    {
                        // Om vi inte har par, fold
                        if (cards[0].Item1 != cards[1].Item1)
                        {
                            betValue = 0;
                            return true;
                        }
                    }
                }
            }

            try
            {
                string address = "http://rainman.leanpoker.org/rank";
                var http = (HttpWebRequest)WebRequest.Create(new Uri(address));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                string parsedContent = @"cards=[{""rank"":""5"",""suit"":""diamonds""}]";
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                Console.Error.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }


            var commCount = communityCards.Count();

            // We have the flop on the table.
            if (commCount == 3)
            {
                // Can we do something with the cards?   
            }

            // We have the turn on the table.
            if (commCount == 4)
            {
                // Can we do something with the cards?   
            }

            // We have the river on the table.
            if (commCount == 5)
            {
                // Can we do something with the cards?   
            }


            betValue = 0;
            return false;
        }

        public static void ShowDown(JObject gameState)
        {
            //TODO: Use this method to showdown
        }

        public static void Junk()
        {

            //// Same suit
            //if (cards[0].Item2 == cards[1].Item2)
            //{
            //    Console.Error.WriteLine("We have Colors");
            //    //value = ourPlayer["stack"].Value<int>();

            //    var firstCardValue = Dictionary[cards[0].Item1];
            //    var secondCardValue = Dictionary[cards[1].Item1];

            //    if (Math.Abs(firstCardValue - secondCardValue) == 1)
            //    {
            //        // Possible straight flush
            //    }
            //}
        }
    }
}

