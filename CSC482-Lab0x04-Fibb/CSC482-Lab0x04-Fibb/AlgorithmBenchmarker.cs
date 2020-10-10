using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lab0x03
{
    class AlgStats
    {
        public double PrevTimeMicro;
        public double TimeMicro;
        public double ExpectedDoublingRatio;
        public double ActualDoublingRatio;

        public AlgStats()
        {
            PrevTimeMicro = 0;
            TimeMicro = 0;
            ExpectedDoublingRatio = 0;
            ActualDoublingRatio = 0;
        }
    }

    class AlgorithmBenchmarker
    {
        private const double MaxSecondsPerAlgorithm = 25;
        private const double MaxMicroSecondsPerAlg = MaxSecondsPerAlgorithm * 1000000;

        private const int NMin = 1;
        private const int NMax = int.MaxValue;

        private readonly Random _rand = new Random();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        // To use benchmarker, simply define the delegate with the signature of your algorithm to test and also the data
        // source it will use.
        internal delegate bool Algorithm(List<int> dataSource, int target);
        // The algorithms are responsible for defining their own doubling ratio calculator
        internal delegate void DoublingCalculator(long n, AlgStats stats);

        // This concrete AlgorithmBenchmark implementation will operate on a list of integers
        private List<Algorithm> _algorithms = new List<Algorithm>();
        private List<DoublingCalculator> _doublingCalculators = new List<DoublingCalculator>();

        // This is just the data generator used for this particular implementation. This should be abstracted
        // so it can be handled more generically, similarly to the doubling calculators.
        private List<int> GenerateUniqueSet(int setLength, int min = Int32.MinValue, int max = Int32.MaxValue)
        {
            // Use .Net HashSet, which does not store duplicates, to generate a unique set and return as list.
            var tempSet = new HashSet<int>();
            long range = (long)max - min;
            // Account for scenario where range from min to max doesn't have enough values to cover setLength
            while (tempSet.Count < (int)Math.Min(setLength, range))
            {
                tempSet.Add(_rand.Next(min, max));
            }

            return tempSet.ToList();
        }

        // Called from within the scope of your algorithms instantiation, simply pass the algorithm function name
        // and the doublingcalculator function name as parameters. Call RunTimeTests to run each algorithm added
        // and display statistics based on doubling calculator.
        public void AddAlgorithmToBenchmark(Algorithm algorithm, DoublingCalculator doublingCalc)
        {
            _algorithms.Add(algorithm);
            _doublingCalculators.Add(doublingCalc);
        }

        public void RunTimeTests()
        {
            Debug.Assert(_algorithms.Count == _doublingCalculators.Count);

            for (int i = 0; i < _algorithms.Count; i++)
            {
                AlgorithmTestRuntime(_algorithms[i], _doublingCalculators[i]);
            }
        }

        private void AlgorithmTestRuntime(Algorithm algorithm, DoublingCalculator doublingCalc)
        {
            PrintHeader(algorithm);

            var currentStats = new AlgStats();

            for (var n = NMin; n * 2 < NMax; n *= 2)
            {
                if (currentStats.TimeMicro > MaxMicroSecondsPerAlg || n*2 < 0) // handle overflow case.
                {
                    PrintAlgorithmTerminationMessage(algorithm);
                    break;
                }

                PrintIndexColumn(n);

                var testData = GenerateUniqueSet(n);
                _stopwatch.Restart();
                algorithm(testData, _rand.Next(NMin, NMax));
                _stopwatch.Stop();

                currentStats.PrevTimeMicro = currentStats.TimeMicro;
                currentStats.TimeMicro = TicksToMicroseconds(_stopwatch.ElapsedTicks);

                doublingCalc(n, currentStats);

                PrintData(n, currentStats);

                    // New Row
                Console.WriteLine();
            }
        }

        // Should be abstracted out so that column names etc, can be passed and this function doesn't have
        // to be modified in source code.
        private void PrintHeader(Algorithm algorithm)
        {
            Console.WriteLine($"Starting run-time tests for {algorithm.Method.Name}...\n");
            Console.WriteLine(
                " \t\t\t           |    |  Doubling Ratios   |");
            Console.WriteLine(
                "N\t\t\t       Time|    | Actual  | Expected |");
        }

        private void PrintAlgorithmTerminationMessage(Algorithm algorithm)
        {
            Console.WriteLine($"{algorithm.Method.Name} exceeded allotted time, terminating...\n");
        }

        private void PrintIndexColumn(int n)
        {
            Console.Write($"{n,-15}");
        }

        private void PrintData(int n, AlgStats stats)
        {
            var actualDoubleFormatted = stats.ActualDoublingRatio < 0
                ? "na".PadLeft(12)
                : stats.ActualDoublingRatio.ToString("F2").PadLeft(12);
            var expectDoubleFormatted = stats.ExpectedDoublingRatio < 0
                ? "na".PadLeft(7)
                : stats.ExpectedDoublingRatio.ToString("F2").PadLeft(7);

            Console.Write(
                $"{stats.TimeMicro,20:F2} {actualDoubleFormatted} {expectDoubleFormatted}");
        }

        private static double TicksToMicroseconds(long ticks)
        {
            return (double) ticks / Stopwatch.Frequency * 1000000;
        }
    }
}
