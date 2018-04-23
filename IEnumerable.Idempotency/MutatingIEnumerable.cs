using System;
using System.Collections.Generic;
using System.Linq;

namespace IEnumerable.Idempotency
{
    public class MutatingIEnumerable
    {
        private int pass = 0;
        public IEnumerable<int> GetValues()
        {
            pass++;
            foreach (var num in Enumerable.Range(1, 10))
                yield return num * pass;
        }
    }
}
