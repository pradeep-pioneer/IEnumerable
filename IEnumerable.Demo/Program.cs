using System;
using System.Diagnostics;
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

            //2. If the backing data store is a concrete collection the results to the call to any and all are consistent
            //AnyAndAllExampleConcrete();

            //3. If the backing data store is not a concrete collection the results to the call to any and all are not consistent
            //AnyAndAllExampleDynamic();

            Console.WriteLine("\nFinished.\nPress any key to exit...");
            Console.ReadLine();
        }

        static void AnyAndAllExampleConcrete()
        {
            /*
             * There are a few things to care about when using any and all on IEnumerable.
             *  1. On Concrete Implementations and Pure Functions, you get predictable and deterministic results
             *  2. On dynamic data sources returning IEnumerable because there is mutation involved results are not predictable and non deterministic.
             */
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var allAndAny = new AllAndAny();
            timer.Stop();
            var millisecondsForInit = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            if (allAndAny.ListBackedEnumerable.Any(x=>x>10000))
                EnumerateAndPrintValues(allAndAny.ListBackedEnumerable.Where(x => x > 10000));
            timer.Stop();
            var millisecondsForEnumeration = timer.ElapsedMilliseconds;
            Console.WriteLine($"\nConcrete Collection:\nTotal Time Taken: {millisecondsForInit+ millisecondsForEnumeration}\nInitialization Time: {millisecondsForInit}\nEnumeration Time: {millisecondsForEnumeration}");

            //Results are Idempotent (unless side effected)
            timer.Reset();
            timer.Start();
            var firstPass = allAndAny.ListBackedEnumerable.Where(x => x > 10000);
            var secondPass = allAndAny.ListBackedEnumerable.Where(x => x > 10000);
            //You can ignore the warning related to multiple enumerations below
            var equalNumberElements = firstPass.Count() == secondPass.Count();
            var eachElementExistsInOther = firstPass.All(thisItem => secondPass.Any(thatItem=>thisItem==thatItem));
            timer.Stop();
            Console.WriteLine($"\n\n**Concrete Collection**\nBoth Passes have Equal Items: {equalNumberElements}\nBoth passes have exact same elements: {eachElementExistsInOther}");
            Console.WriteLine($"Total Time Taken in All and Any Operations: {timer.ElapsedMilliseconds}");
        }

        static void AnyAndAllExampleDynamic()
        {
            var allAndAny = new AllAndAny(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            if (allAndAny.RealtimeDownloadedEnumerable.Any(x => x > 10000))
                EnumerateAndPrintValues(allAndAny.RealtimeDownloadedEnumerable.Where(x => x > 10000));
            timer.Stop();
            var millisecondsForEnumeration = timer.ElapsedMilliseconds;
            Console.WriteLine($"\nDynamic Collection:\nTotal Time Taken: {millisecondsForEnumeration}\nInitialization Time: {0}\nEnumeration Time: {millisecondsForEnumeration}");

            //Results are Not Idempotent
            timer.Reset();
            timer.Start();
            var firstPass = allAndAny.RealtimeDownloadedEnumerable.Where(x => x > 10000);
            var secondPass = allAndAny.RealtimeDownloadedEnumerable.Where(x => x > 10000);
            //You can not ignore the warning related to multiple enumerations below because multiple enumerations will yield different results and each enumeration would be costly
            var equalNumberElements = firstPass.Count() == secondPass.Count();
            var eachElementExistsInOther = firstPass.All(thisItem => secondPass.Any(thatItem => thisItem == thatItem));
            timer.Stop();
            Console.WriteLine($"\n\n**Dynamic Collection**\nBoth Passes have Equal Items: {equalNumberElements}\nBoth passes have exact same elements: {eachElementExistsInOther}");
            Console.WriteLine($"Total Time Taken in All and Any Operations: {timer.ElapsedMilliseconds}");
        }

        static void MutationExample()
        {
            
            var values = (new MutatingIEnumerable()).GetValues();
            var evenValues = values.Where(x => x % 2 == 0);
            var oddValues = values.Where(x => x % 2 != 0);

            Console.WriteLine("Trying to show Even values from the IEnumerable");
            EnumerateAndPrintValues(evenValues);

            Console.WriteLine("Trying to show Odd values from the IEnumerable");
            EnumerateAndPrintValues(oddValues);

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
            EnumerateAndPrintValues(concreteEvenValues);

            Console.WriteLine("Showing Odd values from the list");
            EnumerateAndPrintValues(concreteOddValues);
        }

        private static void EnumerateAndPrintValues(IEnumerable<int> enumerable)
        {
            foreach (var item in enumerable)
                Console.WriteLine($"{item}");
        }
    }
}
