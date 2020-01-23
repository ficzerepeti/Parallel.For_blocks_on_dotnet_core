using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static int Main()
    {
#if NETCOREAPP
        Console.WriteLine("Running dotnet core");
#else
        Console.WriteLine("Running framework");
#endif

        var maxThreadCount = Environment.ProcessorCount;
        Console.WriteLine($"Limiting ThreadPool max thread count to {maxThreadCount}");
        ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
        
        var tasks = new List<Task>();
        var runningTaskCount = 0;
        var obj = new object();
        
        Console.WriteLine($"[main thread,{Thread.CurrentThread.ManagedThreadId}] Entering lock");
        lock (obj)
        {
            // Creating maxThreadCount number of tasks that block thus using and blocking all ThreadPool threads
            for (var i = 0; i < maxThreadCount; ++i)
            {
                var num = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Interlocked.Increment(ref runningTaskCount);
                    Console.WriteLine($"{num},{Thread.CurrentThread.ManagedThreadId}: About to enter lock");
                    lock (obj)
                    {
                        Console.WriteLine($"{num},{Thread.CurrentThread.ManagedThreadId}: Entered lock and now exiting it");
                    }
                    Console.WriteLine($"{num},{Thread.CurrentThread.ManagedThreadId}: Released lock");
                }));
                Thread.Sleep(5);
            }

            Console.WriteLine($"[main thread,{Thread.CurrentThread.ManagedThreadId}] Waiting for {maxThreadCount} blocking tasks to start");
            while (runningTaskCount < maxThreadCount)
            {
                Thread.Sleep(1);
            }
		
            Console.WriteLine($"[main thread,{Thread.CurrentThread.ManagedThreadId}] Running Parallel.For that runs fine on framework but blocks on dotnet core");
            Parallel.For(0, 20, num => Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: Parallel for {num}"));
        
            Console.WriteLine($"[main thread,{Thread.CurrentThread.ManagedThreadId}] Exiting lock");
        }
        
        Console.WriteLine($"[main thread,{Thread.CurrentThread.ManagedThreadId}] Waiting for {tasks.Count} tasks to finish");
        Task.WaitAll(tasks.ToArray());
        
        Console.WriteLine($"[main thread,{Thread.CurrentThread.ManagedThreadId}] Returning from Main");
        return 0;
    }
}