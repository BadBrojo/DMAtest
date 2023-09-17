using System;
using VmmFrost;

namespace DMATest
{
    public class MemoryWorker
    {
        private readonly MemDMA _mem;
        private readonly uint _pid;
        private readonly ulong _moduleBase;
        private const ulong UWorldOffset = 0x7B8F550;
        private const ulong UObjectOffset = 0x7A21410;
        private const ulong FNameOffset = 0x7981A80;

        private const ulong ULevelOffset = 0x30;

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

        public void ParseActorsFromUWorld()
        {
            try
            {
                ulong uWorldAddress = _moduleBase + UWorldOffset;
                ulong uWorldPtr = _mem.ReadValue<ulong>(_pid, uWorldAddress);
                ulong uLevelPtr = uWorldPtr + ULevelOffset;

                // Read ActorCount and ActorArray.
                int actorCount = _mem.ReadValue<int>(_pid, uLevelPtr);
                ulong actorArrayAddress = uLevelPtr + sizeof(int);
                ulong actorArrayPtr = _mem.ReadValue<ulong>(_pid, actorArrayAddress);


                Console.WriteLine($"Total actors: {actorCount}");

                for (int i = 0; i < actorCount; i++)
                {
                    ulong currentActorAddress = actorArrayPtr + (ulong)(i * sizeof(ulong));
                    ulong currentActorPtr = _mem.ReadValue<ulong>(_pid, currentActorAddress);

                    // Access and use properties of CurrentActor (if necessary).
                    // For instance, if you want to read the 'Owner' property:
                    // ulong ownerPtr = _mem.ReadValue<ulong>(_pid, currentActorPtr + 0x140);

                    // Print out each actor address for now.
                    Console.WriteLine($"Actor {i} address: {currentActorPtr:X}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing actors: {ex.Message}");
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
                    case '3':
                        Console.WriteLine("\nParsing Actors from UWorld");
                        ParseActorsFromUWorld();
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

}
