using System;

namespace CSC482_Lab0x04_Fibb
{
    class Program
    {
        static void Main(string[] args)
        {
            var sandbox = new FibSandbox();

            Console.WriteLine($"Verification tests passed? {sandbox.RunVerificationTests()}");
        }
    }
}
