using Newtonsoft.Json.Linq;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "Version 1.1 CheckAll";

		public static int BetRequest(JObject gameState)
		{


		    var current_buy_in = gameState["current_buy_in"].Value<int>();


		    var in_action = gameState["in_action"].Value<int>();

            var our_current_bet = gameState["players"][in_action]["bet"].Value<int>();


            // Allways check

            return current_buy_in - our_current_bet;




			//TODO: Use this method to return the value You want to bet
			//return 0;
		}

		public static void ShowDown(JObject gameState)
		{
			//TODO: Use this method to showdown
		}
	}
}

