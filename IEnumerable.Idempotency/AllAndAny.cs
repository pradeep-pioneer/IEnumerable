using System;
using System.Collections.Generic;
using System.Threading;

namespace IEnumerable.Idempotency
{
    public class AllAndAny
    {

        public AllAndAny()
        {
            for (int i = 0; i < max; i++)
            {

            }
        }

        private List<int> listCollection = new List<int>();


        public IEnumerable<int> ListBackedEnumerable { get { return listCollection; } }

        public IEnumerable<int> RealtimeDownloadedEnumerable()
        {
            var randomGen = new Random(DateTime.Now.Second * DateTime.Now.Minute);
            int maxValue = randomGen.Next(1000, 2000);
            for (int i = 0; i < maxValue; i++)
            {
                //simulate some network activity
                Thread.Sleep(200);
                yield return Math.Abs(randomGen.Next(500065) - maxValue);
            }
        }
    }
}
