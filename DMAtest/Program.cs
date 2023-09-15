using System;
using VmmFrost;

namespace DMATest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting DMATest...");
            //
            try
            {
                string[] initArgs = new string[] { "-printf", "-v", "-device", "fpga" };
                using (var mem = new MemDMA(initArgs))
                {
                    Console.WriteLine("Initialization and memory mapping complete.");

                    var processManager = new ProcessManager(mem);
                    processManager.StartWorker("dungeoncrawler.exe", "dungeoncrawler.exe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during initialization: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public class ProcessManager
        {
            private readonly MemDMA _mem;

            public ProcessManager(MemDMA mem)
            {
                _mem = mem;
            }

            public uint GetPid(string process)
            {
                return _mem.GetPid(process);
            }

            public ulong GetModuleBase(uint pid, string module)
            {
                return _mem.GetModuleBase(pid, module);
            }

            public void StartWorker(string process, string module)
            {
                try
                {
                    uint pid = GetPid(process);
                    Console.WriteLine($"PID for {process}: {pid}");

                    ulong moduleBase = GetModuleBase(pid, module);
                    Console.WriteLine($"Base Address for {module}: {moduleBase:X}");

                    var worker = new MemoryWorker(_mem, pid, moduleBase);
                    worker.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public class MemoryWorker
        {
            private readonly MemDMA _mem;
            private readonly uint _pid;
            private readonly ulong _moduleBase;

            public MemoryWorker(MemDMA mem, uint pid, ulong moduleBase)
            {
                _mem = mem;
                _pid = pid;
                _moduleBase = moduleBase;
            }

            public void Run()
            {
                Console.WriteLine("Worker started. Press 'q' to quit.");
                while (true)
                {
                    // Implement your logic here.

                    // Listen for user input to break the loop.
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'q')
                    {
                        Console.WriteLine("\nExiting worker...");
                        break;
                    }
                }
            }
        }
    }
}
