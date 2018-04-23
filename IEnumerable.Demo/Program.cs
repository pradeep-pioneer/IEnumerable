using System;
using System.Linq;
namespace IEnumerable.Demo
{
    using System.Collections.Generic;
    using IEnumerable.Idempotency;
    class Program
    {
        static void Main(string[] args)
        {
            //1. How mutation during enumeration can have undesired effects;
            MutationExample();
        }

        static void AnyExample()
        {
            
        }

        static void MutationExample()
        {
            Action<IEnumerable<int>> enumerateAndPrintValues = (IEnumerable<int> enumerable) =>
            {
                foreach (var item in enumerable)
                    Console.WriteLine($"{item}");
            };

            var values = (new MutatingIEnumerable()).GetValues();
            var evenValues = values.Where(x => x % 2 == 0);
            var oddValues = values.Where(x => x % 2 != 0);

            Console.WriteLine("Trying to show Even values from the IEnumerable");
            enumerateAndPrintValues(evenValues);

            Console.WriteLine("Trying to show Odd values from the IEnumerable");
            enumerateAndPrintValues(oddValues);

            /*
             * Why don't we get the values for the odd numbers?
             * But my code reviews guidelines say that I shouldn't call ToList() on an IEnumerable, what should I do?
             * Let's do it again
            */

            // 1. Let's first get a concrete colletion - yes you heard it right, IEnumerable is not a collection - it's just a query.
            var concreteValues = (new MutatingIEnumerable()).GetValues().ToList();
            var concreteEvenValues = concreteValues.Where(x => x % 2 == 0);
            var concreteOddValues = concreteValues.Where(x => x % 2 != 0);
            Console.WriteLine("\n\n****************\nRevised Version\n****************");
            Console.WriteLine("Showing Even values from the list");
            enumerateAndPrintValues(concreteEvenValues);

            Console.WriteLine("Showing Odd values from the list");
            enumerateAndPrintValues(concreteOddValues);
        }
    }
}
