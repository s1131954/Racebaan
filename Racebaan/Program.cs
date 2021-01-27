using System;
using Model;
using Controller;
using System.Threading;

namespace Racebaan
{
    class Program
    {
        static void Main(string[] args)
        {
            Data.Initialize();
            Data.NextRace();
            Visualisation.Initialize(Data.CurrentRace.Track);

            for (; ;)
            {
                Thread.Sleep(100);
            }
        }
    }
}
