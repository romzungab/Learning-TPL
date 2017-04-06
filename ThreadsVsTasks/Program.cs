using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsVsTasks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var watch = new Stopwatch();
            //64 is upper limit for WaitHandle.WaitAll() method
            var maxWaitHandleWaitAllAllowed = 64;
            var mres = new ManualResetEventSlim[maxWaitHandleWaitAllAllowed];

            for (var i = 0; i < mres.Length; i++)
                mres[i] = new ManualResetEventSlim(false);


            long threadTime = 0;
            long taskTime = 0;
            watch.Start();

            //start a new classic Thread and signal the ManualResetEvent when its done
            //so that we can snapshot time taken, and 

            for (var i = 0; i < mres.Length; i++)
            {
                var idx = i;
                var t = new Thread(state =>
                {
                    for (var j = 0; j < 10; j++)
                        Console.WriteLine("Thread : {0}, outputing {1}", state.ToString(), j.ToString());
                    mres[idx].Set();
                });
                t.Start(string.Format("Thread{0}", i));
            }

            WaitHandle.WaitAll((from x in mres select x.WaitHandle).ToArray());

            threadTime = watch.ElapsedMilliseconds;
            watch.Reset();

            for (var i = 0; i < mres.Length; i++)
                mres[i].Reset();

            watch.Start();

            for (var i = 0; i < mres.Length; i++)
            {
                var idx = i;
                var task = Task.Factory.StartNew(state =>
                {
                    for (var j = 0; j < 10; j++)
                        Console.WriteLine("Task : {0}, outputing {1}", state.ToString(), j.ToString());
                    mres[idx].Set();
                }, string.Format("Task{0}", i));
            }

            WaitHandle.WaitAll((from x in mres select x.WaitHandle).ToArray());
            taskTime = watch.ElapsedMilliseconds;
            Console.WriteLine("Thread Time waited : {0}ms", threadTime);
            Console.WriteLine("Task Time waited : {0}ms", taskTime);

            for (var i = 0; i < mres.Length; i++)
                mres[i].Reset();
            Console.WriteLine("All done, press Enter to Quit");

            Console.ReadLine();
        }
    }
}