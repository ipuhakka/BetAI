﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BetAI
{
    class Program
    {
        private static string savefile;
        private static string[] simArgs;
        private static CancellationToken token;

        /// <summary>
        /// Starting point for the program. First parameter in args is the savefile name.
        /// It is created if it does not yet exist. Rest are optional parameters for simulation.
        /// They are alpha (double), minimumStake (double), numberOfNodes(int), sampleSize(int),
        /// database(string), mutationProbability(double) and mutationMethod(string). Optional paremeters are given in format
        /// argumentName=argumentValue.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No savefile name given as argument, exiting");
                return;
            }

            savefile = args[0];
            simArgs = args.Skip(0).ToArray();

            var cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            var thread = new Thread(StartSimulation);
            thread.Start();

            Console.WriteLine("Thread started");
            Console.WriteLine("Press esc to stop simulation");

            do
            {
                
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Console.WriteLine("Finishing thread..");
            cancelTokenSource.Cancel();
        }

        static void StartSimulation()
        {
            new Master(savefile, token, simArgs).Run();
        }
    }
}
