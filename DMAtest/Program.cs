using System;
using VmmFrost;

namespace DMATest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting DMATest...");

            try
            {
                string[] initArgs = new string[] { "-printf", "-v", "-device", "fpga" };
                using (var mem = new MemDMA(initArgs))
                {
                    Console.WriteLine("Initialization and memory mapping complete.");

                    var processManager = new ProcessManager(mem);
                    processManager.StartWorker("DungeonCrawler.exe", "DungeonCrawler.exe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during initialization: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
