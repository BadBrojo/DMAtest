using System;
using VmmFrost;

public struct World
{
    public IntPtr PersistentLevel;
    public IntPtr[] StreamingLevels;  // This is an array of pointers since it uses TArray in UE.
    public IntPtr[] Levels;           // This is an array of pointers for the same reason.
}


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
        //process manager
        #region


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
        #endregion 
        //memory worker
        #region
        public class MemoryWorker
        {
            private readonly MemDMA _mem;
            private readonly uint _pid;
            private readonly ulong _moduleBase;
            private const ulong UWorldOffset = 0x7B8F550;
            private const ulong UObjectOffset = 0x7A21410;
            private const ulong FNameOffset = 0x7981A80;


            public MemoryWorker(MemDMA mem, uint pid, ulong moduleBase)
            {
                _mem = mem;
                _pid = pid;
                _moduleBase = moduleBase;
            }

            public void ParseUWorldAddress()
            {
                try
                {
                    ulong uWorldAddress = _moduleBase + UWorldOffset; // Offset for UWorld
                    ulong uWorldPtr = _mem.ReadValue<ulong>(_pid, uWorldAddress); // Read UWorld pointer
                    Console.WriteLine($"UWorld address: {uWorldPtr:X}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing UWorld address: {ex.Message}");
                }
            }
            public void ParseUObjectAddress()
            {
                try
                {
                    ulong uObjectAddress = _moduleBase + UObjectOffset; // Offset for UObject
                    ulong uObjectPtr = _mem.ReadValue<ulong>(_pid, uObjectAddress); // Read UObject pointer
                    Console.WriteLine($"UObject address: {uObjectPtr:X}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing UObject address: {ex.Message}");
                }
            }

            public void Run()
            {
                Console.WriteLine("Worker started. Press 'q' to quit.");
                while (true)
                {
                    // Listen for user input to break the loop.
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    switch (keyInfo.KeyChar)
                    {
                        case '1':
                            Console.WriteLine("\nParsing UWorld Address");
                            ParseUWorldAddress();
                            break;
                        case '2':
                            Console.WriteLine("\nParsing UObject Address");
                            ParseUObjectAddress();
                            break;
                        case 'q':
                            Console.WriteLine("\nExiting worker...");
                            return; // break the loop by returning from the method.
                        default:
                            break;
                    }
                }
            }
        }
        #endregion
    }
}

