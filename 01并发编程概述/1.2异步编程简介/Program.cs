using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1._2异步编程简介
{
    class Program
    {
        static void Main(string[] args)
        {
            DoSomethingAsync();
            DoSomethingAsyncV2();
        }

        //异步编程有两大好处：
        //1.第一个好处是对于面向终端用户的GUI程序：异步编程提高响应能力。 我们都遇到过在运行时会临时锁定界面的程序，异步编程可以使程序在执行任务时仍能响应用户输入，
        //2.第二个好处是对于服务器端应用：异步编程实现了扩展性。服务器应用可以利用线程池满足其可扩展性，使用异步编程后，可扩展性通常可以提高一个数量级。

        //现代的异步Net 程序使用两个关键字：async和await。async关键字加在方法声明上，它的主要目的使方法内的await关键字生效（为了保持向后兼容，同时引入了这两个关键字）。如果async方法有返回值，应返回Task<T>;如果没有返回值，应返回Task。这些Task类型相当于future，用来在异步方法结束时通知主程序。
        //不用void作为async方法的返回类型!,async方法可以返回void，，但是这仅限于编写事件处理程序，一个普通的async方法如果没有返回值，要返回Task，而不是void

        //有个上述背景知识，我们来快速看一个例子：
       static  async Task DoSomethingAsync()
        {
           Console.WriteLine("1");
            int val = 13;
            //异步方式等待1秒
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("2");
            val *= 2;
            //异步方式等待1秒
            await Task.Delay(TimeSpan.FromSeconds(1));
            Console.WriteLine("3");
            Trace.WriteLine(val.ToString());
            Console.WriteLine("4");
        }

        //和其他方法一样，async方法在开始时以同步方式执行，
        //在async方法内部，await关键字对它的参数执行一个异步等待。
        //它首先检查异步操作是否已经完成，如果完成了，就继续运行（同步方式）,
        //否则，它会暂停async方法，并返回，留下一个未完成的task。
        //一段时间后，异步操作完成，async方法就恢复运行。

        //一个async方法是由多个同步执行的程序块组成的，每个同步程序块之间是由await语句分隔。
        //第一个同步程序块在调用这个方法的线程中运行，但其他同步程序块在哪里运行呢？情况比较复杂？

        //最常见的情况是，用await语句等待一个任务完成，当该方法在await处暂停时，就可以捕捉上下文（context）
        //如果当前SynchronizationContext不为空，这个上下文就是当前SynchronizationContext。
        //如果当前SynchronizationContext为空，则这个上下文为当前TaskScheduler。该方法会在这个上下文中继续运行。
        //一般来说，运行UI线程时采用UI上下文，处理asp.net请求时采用asp.net上下文，其他很多情况下则采用线程池上下文。

        //因此，在上面的代码中，每个同步程序块会试图在原始的上下文中恢复运行。
        //如果在UI线程中调用DoSomethingAsync，这个方法的每个同步程序块都将在此UI线程上运行，但是，如果在线程池线程中调用，每个同步程序块将在线程池线程上运行。

        //要避免这种错误行为，可以在await中使用ConfigureAwait方法，将参数continueOnCapturedContext设为false，
        //接下来的代码刚开始会在调用的线程里运行，在被await暂停后，则会在线程池线程里运行：

        static async Task DoSomethingAsyncV2()
        {
            int val = 13;
            //异步方式等待1秒
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            val *= 2;
            //异步方式等待1秒
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            Trace.WriteLine(val.ToString());
     
        }


        //最好的做法是： 在核心库代码中一直使用ConfigureAwait，在外围的用户界面代码中，只在需要时才恢复上下文。





        //关键字await不仅能用于任务，还能用于所有遵循特定模式的awaitable类型。
        //例如，Windows Runtime API定义了自己专用的异步操作接口。
        //这些接口不能转化为Task类型，但遵循了可等待的（awaitable）模式，因此可以直接使用await。
        //这种awaitable类型在Windows应用商店程序中更加常见，
        //但是在大多数情况下，await使用Task或Task<T>.


        //有两种基本的方法可以创建Task实例， 
        //有些任务表示CPU需要实际执行的指令，创建这种计算类的任务时，使用Task.Run（如需要按照特定的计划运行，则用TaskFactory.StartNew).
        //其他的任务表示一个通知（Notification),创建这种基于事件的任务时，使用TaskCompletionSource<T>,
        //大部分I/O型任务采用TaskCompletionSource<T>.


        //使用async和await时，自然要处理错误。在下面的代码中，PossibelExceptionAsync会抛出以个NotSupportedException异常，而TrySomethingAsync方法可以很顺利地捕捉到这个异常。这个捕捉到的异常完整地保留了栈轨迹，没有人为地将它封装进TargetInvocationException或AggregateException类：

        static async Task TrySomethingAsyncV3()
        {
            try
            {
                //await PossibleExceptionAsync();//PossibleExceptionAsync()为用户代码
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch (NotSupportedException ex)
            {
                //LogException(ex);
                throw;
            }
        }


        //一旦异步方法抛出（或传递出）异常，该异常会放在返回的Task对象中，并且这个Task对象的状态变为“已完成”。
        //当await调用该Task对象时，await会获得并（重新）抛出该异常，并且保留着原始的栈轨迹，
        //因此如果PossibleExceptionAsync是异步方法，以下的代码就能正常运行：

        static async Task TrySomethingAsyncV4()
        {
            //发生异常时，任务结束，不会直接抛出异常。
            //Task task = PossibleExceptionAsync(); //PossibleExceptionAsync()为用户代码
            Task task=Task.Delay(TimeSpan.FromSeconds(1));
            try
            {
                //Task对象中的异常，会在这条await语句中引发
                await task;
            }
            catch (NotSupportedException ex)
            {
                //LogException(ex);
                throw;
            }
        }

        //关于异步方法，还有一条重要的准则：你一旦在代码中使用了异步，最好一直使用。
        //调用异步方法时，应该（在调用结束时）用await等待它返回的task对象，一定要避免使用Task.Wait或Task<T>.Result方法，因为它们会导致死锁，
        //参考下面这个方法。

        async Task WaitAsync()
        {
            //这里await会捕获当前上下文
            await Task.Delay(TimeSpan.FromSeconds(1));
            //...这里会试图用上面捕获的上下文继续执行
        }

        void Deadlock()
        {
            //开始延迟
            Task task = WaitAsync();

            //同步程序块，正在等待异步方法完成
            task.Wait();
        }


        //如果从UI或Asp.Net的上下文调用这段代码，就会发生死锁，这是因为，这两种上下文每次只能运行一个线程。
        //Deadlock方法调用WaitAsync方法，WaitAsync方法开始调用delay语句。
        //然后，Deadlock方法（同步）等待WaitAsync方法完成，同时阻塞了上下文线程。
        //当delay语句结束时，await试图在已捕获的上下文中继续运行WaitAsync方法，
        //但这个步骤无法成功，因为上下文中已经有了一个阻塞的线程，并且这种上下文只允许同时运行一个线程，
        //这里有两个方法可以避免死锁：
        //在WaitAsync中使用ConfigureAwait（false）（导致await忽略该方法的上下文）,
        //或者用await语句调用WaitAsync方法（让Deadlock变成一个异步方法）；



        //如果使用Async，最好一直使用它。

        //如果想要更全面地了解关于异步编程的知识。
        //可参阅 Alex Davies编写的Async in C#5.0
        //或者 微软文档  async overview 或者  Task-based asynchronous Pattern（TAP） overview

    }
}
