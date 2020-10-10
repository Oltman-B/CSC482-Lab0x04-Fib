using System;

namespace CSC482_Lab0x04_Fibb
{
    class Program
    {
        static void Main(string[] args)
        {
            var sandbox = new FibSandbox();
            if (sandbox.RunVerificationTests())
            {
                Console.WriteLine("Verification Tests Passed!");
                sandbox.RunTimeTests();
            }
            else
            {
                Console.WriteLine("One or more verification tests failed.");
            }
        }
    }
}
