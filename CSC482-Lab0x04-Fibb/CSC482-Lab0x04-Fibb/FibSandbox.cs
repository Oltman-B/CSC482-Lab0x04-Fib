using System;
using System.Collections.Generic;

namespace CSC482_Lab0x04_Fibb
{
    class FibSandbox
    {
        public long FibRecur(int x)
        {
            if (x < 2) return x;

            return FibRecur(x-1) + FibRecur(x-2);
        }

        public void FibRecurDoublingCalc(AlgStats algStats)
        {

            // Only calculate doubling ratios for even values of n and if n/2 exists in prev times table.
            if (algStats.n % 2 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.n / 2, out double halfTime))
            {
                algStats.ActualDoublingRatio = (algStats.TimeMicro / halfTime);

                double goldenRatio = 1.61803398875; 
                algStats.ExpectedDoublingRatio = Math.Pow(goldenRatio, algStats.n) / Math.Pow(goldenRatio, ((double)algStats.n / 2));
            }
            else
            {
                algStats.ActualDoublingRatio = -1;
                algStats.ExpectedDoublingRatio = -1;
            }

        }

        public long FibCache(int x)
        {
            var cache = new Dictionary<int, long>();
            return FibCache(x, cache);
        }

        private long FibCache(int x, Dictionary<int, long> cache)
        {
            if (x < 2) return x;

            // Use cache to look-up x and get result if already calculated
            // If cache contains x, just return the stored fib result
            if (cache.ContainsKey(x)) return cache[x];

            // If cache does not contain x, store the fib result
            cache[x] = FibCache(x - 1, cache) + FibCache(x - 2, cache);
            // and return the result
            return cache[x];
        }

        public void FibCacheDoublingCalc(AlgStats algStats)
        {
            // Only calculate doubling ratios for even values of n and if n/2 exists in prev times table.
            if (algStats.n % 2 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.n / 2, out double halfTime))
            {
                algStats.ActualDoublingRatio = (algStats.TimeMicro / halfTime);
                algStats.ExpectedDoublingRatio = algStats.n / ((double)algStats.n / 2);
            }
            else
            {
                algStats.ActualDoublingRatio = -1;
                algStats.ExpectedDoublingRatio = -1;
            }
        }

        public long FibLoop(int x)
        {
            if (x < 2) return x;

            long fib1 = 0;
            long fib2 = 1;

            // Iterate from 2->x and calculate each proceeding term of the fib
            // sequence until we have calculated the final xth term
            for (int i = 2; i <= x; i++)
            {
                long nextFib = fib1 + fib2;
                fib1 = fib2;
                fib2 = nextFib;
            }

            // Return xth fibonacci.
            return fib2;
        }

        public void FibLoopDoublingCalc(AlgStats algStats)
        {
            // Only calculate doubling ratios for even values of n and if n/2 exists in prev times table.
            if (algStats.n % 2 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.n / 2, out double halfTime))
            {
                algStats.ActualDoublingRatio = (algStats.TimeMicro / halfTime);
                algStats.ExpectedDoublingRatio = algStats.n / ((double)algStats.n / 2);
            }
            else
            {
                algStats.ActualDoublingRatio = -1;
                algStats.ExpectedDoublingRatio = -1;
            }
        }

        public long FibMatrix(int x)
        {
            var multiplier = new long[2, 2] {{1, 1}, {1, 0}};
            var result = CalculatePowerMatrix(multiplier, x);

            // The way I implemented calculate power matrix results in the fibonacci value
            // stored in the bottom right corner of matrix rather than top left.
            return result[1, 1];
        }

        public void FibMatrixDoublingCalc(AlgStats algStats)
        {
            // Only calculate doubling ratios for even values of n and if n/2 exists in prev times table. n must be > 2 for log, otherwise div by 0.
            if (algStats.n > 2 && algStats.n % 2 == 0 && algStats.PrevTimesTable.TryGetValue(algStats.n / 2, out double halfTime))
            {
                algStats.ActualDoublingRatio = (algStats.TimeMicro / halfTime);
                algStats.ExpectedDoublingRatio = Math.Log2(algStats.n) / Math.Log2(((double)algStats.n / 2));
            }
            else
            {
                algStats.ActualDoublingRatio = -1;
                algStats.ExpectedDoublingRatio = -1;
            }
        }

        // Similar to the fast calculate power int
        private long[,] CalculatePowerMatrix(long[,] m, long y)
        {
            //Calculates m^y
            if (y == 0) return new long[2, 2] {{1, 1}, {1, 0}};

            var doublePower = m;
            var result = new long[2, 2] {{1, 1}, {1, 0}};

            // while there are digits left to process
            while (y > 0)
            {
                // if current low-order digit is 1
                if ((y & 1) > 0)
                {
                    // multiply current matrix by the value of the doublePower matrix
                    result = MultiplyMatrices(result, doublePower);
                }

                // move next digit to low order position
                y >>= 1;

                // just like with ints, the power can be doubled for each 'bit position' in the 
                // y value. this will cut down on the number of matrix multiplications required.
                //13 ->  b:     1            1           0 <- skip 1
                //x^13 = M^8 * (1)  * M^4 * (1) * M^2 * (0) * M * (1)
                doublePower = MultiplyMatrices(doublePower, doublePower);
            }

            return result;
        }

        private long CalculatePowerLong(long x, long y)
        {
            if (y == 0) return 1;

            long doublePower = x;
            long result = 1;
            while (y > 0)
            {
                // if low-order bit is 1, do calculation
                if ((y & 1) > 0)
                {
                    result *= (doublePower);
                }

                // shift to next highest bit to low order position
                y >>= 1;

                //13 ->  b:     1            1           0         1
                //x^13 = x^8 * (1)  * x^4 * (1) * x^2 * (0) * x * (1)
                doublePower *= doublePower;
            }

            return result;
        }

        private long[,] MultiplyMatrices(long[,] A, long[,] B)
        {
            var result = new long[A.GetLength(0), B.GetLength(1)];
            // iterate through A's rows
            for (int i = 0; i < A.GetLength(0); i++)
            {
                // iterate B's columns
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    // iterate B's rows
                    for (int k = 0; k < B.GetLength(0); k++)
                    {
                        // sum product of each element in A's row and B's column
                        result[i,j] += A[i,k] * B[k,j];
                    }
                }
            }

            return result;
        }
        private bool Verify2X2MatrixMultiplication()
        {
            var matrixA = new long[2, 2] {{1, 1}, {1, 0}};
            var matrixB = new long[2, 2] {{1, 1}, {1, 0}};
            var matrixExpected = new long[2, 2] {{2, 1}, {1, 1}};
            var matrixActual = MultiplyMatrices(matrixA, matrixB);
            for (int i = 0; i < matrixExpected.GetLength(0); i++)
            {
                for (int j = 0; j < matrixExpected.GetLength(1); j++)
                {
                    if (matrixExpected[i, j] != matrixActual[i, j]) return false;
                }
            }

            return true;
        }

        public void RunTimeTests()
        {
            var benchmarker = new AlgorithmBenchmarker();
            benchmarker.AddAlgorithmToBenchmark(FibRecur, FibRecurDoublingCalc);
            benchmarker.AddAlgorithmToBenchmark(FibCache, FibCacheDoublingCalc);
            benchmarker.AddAlgorithmToBenchmark(FibLoop, FibLoopDoublingCalc);
            benchmarker.AddAlgorithmToBenchmark(FibMatrix, FibMatrixDoublingCalc);

            benchmarker.RunTimeTests();
        }

        public bool RunVerificationTests()
        {
            const int tenthFibonacci = 55;

            if (FibRecur(10) != tenthFibonacci) return false;
            if (FibCache(10) != tenthFibonacci) return false;
            if (FibLoop(10) != tenthFibonacci) return false;

            if (!Verify2X2MatrixMultiplication()) return false;

            if (FibMatrix(10) != tenthFibonacci) return false;
            if (FibMatrix(11) != 89) return false;
            if (FibMatrix(28) != 317811) return false;

            if (CalculatePowerLong(10, 5) != 100000) return false;
            if (CalculatePowerLong(3, 22) != 31381059609) return false;

            return true;
        }
    }
}
