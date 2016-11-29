using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
    public static class PokerPlayer
    {
        public static readonly string VERSION = "Version 1.337 Other teams are going?" +Guid.NewGuid().ToString() ;

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

            Console.Error.WriteLine(gameState["game_id"].Value<string>());
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


                Console.Error.WriteLine("Our current bet: " + ourPlayer["bet"].Value<int>());


                var cards = new List<Card>();
                foreach (var card in hole_cards)
                {
                    var rank = card["rank"].Value<string>();
                    var suit = card["suit"].Value<string>();

                    cards.Add(new Card(rank, suit));

                }
                if (cards.Count == 2)
                {
                    Console.Error.WriteLine("We have " + cards[0].Rank + " " + cards[0].Suit);
                    Console.Error.WriteLine("We have " + cards[1].Rank + " " + cards[1].Suit);
                }

                var community_cards = gameState["community_cards"];
                return gameState["stack"].Value<int>();



                // If we are only 2 players remaining with big blind. Go all in.
                if (gameState["players"].Count() == 2)
                {
                    if (small_blind > 500)
                    {
                        return 99999;
                    }
                }

                bool hasOtherPlay = GetPlay(out value,cards, community_cards, ourPlayer, current_buy_in, small_blind);
                if (!hasOtherPlay)
                {
                    var amount = current_buy_in - our_current_bet;
                    Console.Error.WriteLine("Got no better to do, defaulting " + amount);
                   
                    return amount;
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

        private static bool GetPlay(out int betValue, List<Card> cards, JToken communityCards, JToken ourPlayer, int currentBuyIn, int smallBlind)
        {
            var currentBet = ourPlayer["bet"].Value<int>();

            // If only 2 players and big blind > 250. 
            // Go all in.


            // Do stuff during the pre-flop
            var actedOnPreFlop = ActOnPreFlop(out betValue,cards,communityCards,ourPlayer,currentBuyIn,smallBlind);
            if (actedOnPreFlop)
            {
                return true;
            }

            var commCount = communityCards.Count();
            var allCards = new List<Card>(cards);
            foreach (var card in communityCards)
            {
                var rank = card["rank"].Value<string>();
                var suit = card["suit"].Value<string>();
                allCards.Add(new Card(rank,suit));
            }

            try
            {
                //ContactRainman(allCards);
            }
            catch (Exception ex)
            {
                
                Console.Error.WriteLine(ex);
            }

            if (communityCards.Any())
            {
                var sortedCards = allCards.OrderBy(x => x.Rank)
               .ThenBy(x => x.Suit);

                var groupResults = from sort in sortedCards
                                   group sort by sort.Rank
                    into gro
                                   select new { Rank = gro.Key, Count = gro.Count() };

                var resultsGroup = groupResults.OrderByDescending(x => x.Count);

                int firstCount = resultsGroup.First().Count;
                // triss eller fyrtal
                if (firstCount >= 3)
                {
                    betValue = 9999;
                    return true;
                }
                else if(firstCount <=2)
                {
                    betValue = 0;
                    return false;
                }
            }


                // Can we do something with the cards?   


            betValue = 0;
            return false;
        }

        public static bool ActOnPreFlop(out int betValue, List<Card> cards, JToken communityCards, JToken ourPlayer, int currentBuyIn, int smallBlind)
        {
            var currentBet = ourPlayer["bet"].Value<int>();
            // Pre-flop
            if (!communityCards.HasValues)
            {
                Console.Error.WriteLine("No community cards in place");
                // We have both cards on hand.
                if (cards.Count == 2)
                {
                    // Om vi har par
                    if (cards[0].Rank == cards[1].Rank)
                    {
                        // Om vi har ett bra par. (Knekt eller bättre)
                        if (Dictionary[cards[0].Rank] > 10)
                        {
                            Console.Error.WriteLine("We have pairs, going all in");
                            betValue = ourPlayer["stack"].Value<int>();
                            return true;
                        }
                        // Resten
                        else
                        {
                            if (currentBuyIn > currentBet + smallBlind*2)
                            {
                                betValue = 0;
                                Console.Error.WriteLine("We have no good cards, folding");
                                return true;
                            }


                            // Om vi har ett "sämre" par så lägger höjer vi med 2 * smallBlind.
                            betValue = currentBuyIn - currentBet + smallBlind * 2;
                            Console.Error.WriteLine("We have bad pairs, betting: " + betValue);
                            return true;
                        }
                    }

                    // Samma färg
                    if (cards[0].Suit == cards[1].Suit)
                    {
                        Console.Error.WriteLine("We have Colors");

                        var firstCardValue = Dictionary[cards[0].Rank];
                        var secondCardValue = Dictionary[cards[1].Rank];

                        // Om vi har steg-chans.
                        if (Math.Abs(firstCardValue - secondCardValue) < 5)
                        {
                            betValue = currentBuyIn - currentBet + smallBlind * 2;
                            Console.Error.WriteLine("We have chance for straight, betting: " + betValue);
                            return true;
                        }
                    }


                    //Someone has raised/all in
                    //if (ourPlayer["bet"].Value<int>() < currentBuyIn)
                    {

                        var firstCardValue = Dictionary[cards[0].Rank];
                        var secondCardValue = Dictionary[cards[1].Rank];
                        if (firstCardValue > 10 && secondCardValue > 10)
                        {
                            betValue = SmallRaise(currentBuyIn, currentBet, currentBet);
                            Console.Error.WriteLine("We have two high cards: " + betValue);
                            return true;
                        }

                        // Om vi inte har par, fold
                        if (cards[0].Rank != cards[1].Rank)
                        {
                            betValue = 0;
                            Console.Error.WriteLine("We have no good cards, folding");
                            return true;
                        }
                    }
                }
                betValue = 0;
                return true;
            }
            betValue = 0;
            return false;
        }

        public static int SmallRaise(int currentBuyIn, int currentBet, int smallBlind)
        {
            return currentBuyIn + currentBet + smallBlind*2;
        }

        public static void ContactRainman(List<Card>  allCards)
        {
            string address = "http://rainman.leanpoker.org/rank";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "text/plain";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()
                ))
            {
                var baseStr = "cards=";

                var result = JsonConvert.SerializeObject(allCards);

                string json = baseStr + result;
                json = json.ToLower();
                Console.Error.WriteLine("Json sent: " + json);




               

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.Error.WriteLine(result);
            }
        }

        public static void ShowDown(JObject gameState)
        {
            //TODO: Use this method to showdown
        }

        public static void Junk()
        {
            //try
            //{
            //    string address = "http://rainman.leanpoker.org/rank";
            //    var http = (HttpWebRequest)WebRequest.Create(new Uri(address));
            //    http.Accept = "application/json";
            //    http.ContentType = "application/json";
            //    http.Method = "POST";

            //    string parsedContent = @"cards=[{""rank"":""5"",""suit"":""diamonds""}]";
            //    ASCIIEncoding encoding = new ASCIIEncoding();
            //    Byte[] bytes = encoding.GetBytes(parsedContent);

            //    Stream newStream = http.GetRequestStream();
            //    newStream.Write(bytes, 0, bytes.Length);
            //    newStream.Close();

            //    var response = http.GetResponse();

            //    var stream = response.GetResponseStream();
            //    var sr = new StreamReader(stream);
            //    var content = sr.ReadToEnd();
            //    Console.Error.WriteLine(content);
            //}
            //catch (Exception ex)
            //{
            //    Console.Error.WriteLine(ex);
            //}


            //var commCount = communityCards.Count();

            //// We have the flop on the table.
            //if (commCount == 3)
            //{
            //    // Can we do something with the cards?   
            //}

            //// We have the turn on the table.
            //if (commCount == 4)
            //{
            //    // Can we do something with the cards?   
            //}

            //// We have the river on the table.
            //if (commCount == 5)
            //{
            //    // Can we do something with the cards?   
            //}

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

