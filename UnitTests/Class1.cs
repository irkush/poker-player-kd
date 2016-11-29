using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void CanParseJOBject()
        {



            var file = File.ReadAllText("data.json");


            var gameState = JObject.Parse(file);


            var current_buy_in = gameState["current_buy_in"].Value<int>();


            var in_action = gameState["in_action"].Value<int>();

            var our_current_bet = gameState["players"][in_action]["bet"].Value<int>();




            Assert.Fail();



        }
    }
}
