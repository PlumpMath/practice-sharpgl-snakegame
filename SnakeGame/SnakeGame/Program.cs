using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {

            var hashSet = new HashSet<int>();

            hashSet.Add(1);
            hashSet.Add(2);
            hashSet.Add(3);
            hashSet.Add(4);
            hashSet.Add(5);

            hashSet.Clear();
            hashSet.Add(1);
            hashSet.Add(1);
            hashSet.Add(1);
            hashSet.Add(1);
            hashSet.Add(1);

            Console.WriteLine("hashSet = {0}", String.Join(", ", (from x in hashSet select x).ToArray()));

            App.sharedInst.run();
          

           
        }
    }
}
