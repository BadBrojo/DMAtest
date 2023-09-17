using System;
using System.Diagnostics;
using VmmFrost;
using static DMATest.Program;

namespace DMATest
{
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
}
