using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IEnumerable.Idempotency
{
    public class AllAndAny
    {

        public AllAndAny()
        {
            _listCollection = GetValuesInts().ToList();
        }

        public AllAndAny(bool initList)
        {
            _listCollection = initList ? GetValuesInts().ToList() : new List<int>();
        }

        private readonly List<int> _listCollection;


        public IEnumerable<int> ListBackedEnumerable => _listCollection;

        public IEnumerable<int> RealtimeDownloadedEnumerable => GetValuesInts();

        private static IEnumerable<int> GetValuesInts()
        {
            var randomGen = new Random(DateTime.Now.Second * DateTime.Now.Minute);
            var maxValue = randomGen.Next(1000, 2000);
            for (var i = 0; i < maxValue; i++)
            {
                //simulate some network activity
                Thread.Sleep(10);
                yield return Math.Abs(randomGen.Next(500065) - maxValue);
            }
        }
    }
}
