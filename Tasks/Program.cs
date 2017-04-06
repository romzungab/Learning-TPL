using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var task1 = new Task(new Action(HelloConsole));
            var task11 = new Task(new Action<object>(HelloConsole), "Task 11");
            var task2 = new Task(delegate
            {
                HelloConsole();
            });

            var task22 = new Task(delegate (object obj)
            {
                HelloConsole(obj);
            }, "Task 22");

            Task<int> task5 = new Task<int>(() =>
            {
                int result = 1;
                for (int i = 1; i < 10; i++)
                    result *= i;
                return result;
            });
            
            var task3 = new Task(() => HelloConsole());
            var task33 = new Task((obj) => HelloConsole(obj), "Task 33");
           
           Task.Factory.StartNew(() => { HelloConsole(); });
           Task.Factory.StartNew((obj) => { HelloConsole(obj); }, "Task 44");
            var task7 = new Task(new Action(WorkLoad));
            var task8 = new Task(new Action(WorkLoad));
            task33.Start();
            task5.Start();
            task8.Start();
            task1.Start();
            task11.Start();
            task2.Start();
            task22.Start();
            task3.Start();
           
            Console.WriteLine("Waiting 2 secs for task8 to complete");
            task8.Wait(2000);
            Console.WriteLine("Wait ended - task8 completed");
            task7.Start();
            Console.WriteLine("Waiting for task7 to complete");
            task7.Wait();
            Console.WriteLine("Task 7 completed");
            
         
            Console.WriteLine("Waiting for either task7 or task3 to complete");
            int taskIndex = Task.WaitAny(task7, task33);
            Console.WriteLine("Task Completed is at index{0}", taskIndex);

            

            Console.WriteLine("TaskResult for task5:{0}", task5.Result);
            
            //Console.WriteLine("Waiting for all tasks to complete");
            //Task.WaitAll(task1, task2, task3, task5, task7, task8, task11, task22, task33);
            //Console.WriteLine("Tasks 1,2,3,5,7,8,11,22,33 have completed");
            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            var task6 = new Task(() =>
            {
                for (int i = 0; i < 100000; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Cancel() called.");
                        return;
                    }
                    Console.WriteLine("Loop value {0}", i);
                }
            }, token);
            Console.WriteLine("Press any key to start task");
            Console.WriteLine("Press any key again to cancel the running task");
            Console.ReadKey();
            task6.Start();
            Console.ReadKey();
            Console.WriteLine("Cancelling Task");
            cancellationTokenSource.Cancel();
            
           
            Console.WriteLine("Main Method complete. Press any key to finish");
            Console.ReadKey();

        }

        public static void HelloConsole()
        {
            Console.WriteLine("Hello Task");
        }
        public static void HelloConsole(object msg)
        {
            Console.WriteLine("Hello {0}", msg);
        }

        static void WorkLoad()
        {

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Task - iteration{0}",i);
                Thread.Sleep(1000);
            }
        }
    }
}
