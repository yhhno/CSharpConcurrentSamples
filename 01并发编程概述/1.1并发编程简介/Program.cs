﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1._1并发编程简介
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hello world");
        }
        //首先，我来解释几个贯穿本书始终的术语
        //1.并发   同时做多件事情
        // 这个解释直接表明了并发的作用，
        //终端用户程序利用并发功能，在输入数据库的同时响应用户输入，
        //服务器应用并发，在处理第一个请求的同时响应第二个请求，
        //只要你希望程序同时做多件事情，你就需要并发，几乎每个软件程序都会受益于并发。
        

        //2.多线程  并发的一种，它是采用多个线程来执行程序。
        //从字面上看，多线程就是使用多个线程。本书的后续章节将介绍，多线程是并发的一种形式，但不是唯一的形式，
        //实际上，直接使用底层线程类型在现代程序中基本不起作用，比起老式的多线程机制，采用高级的抽象机制会让程序功能更加强大、效率更高，
        //因此，本书将不涉及一些过时的技术，本书中的所有多线程的方法都是采用高级类型，而不是thread或BackgroudWorker
        // 一旦你输入new Thread（）， 那就糟糕了，说明项目中的代码太过时了
        //但是，不要认为多线程已经彻底淘汰了，因为线程池要求多线程继续存在。线程池存放任务的队列，这个队列能够根据需要自行调整，
        //相应地，线程池产生了另一个重要的并发形式：并行处理


        //3.并行处理  把正在执行的大量的任务分割成小块，分配给多个同时运行的线程。
        //为了让处理器的利用效率最大化，并行处理（或并行编程）采用多线程，当现代多核CPU执行大量任务时，若只用一个核执行所有的任务，而让其他核保持空闲，
        //这显然不合理，并行处理把任务分割成小块并分配给多个线程，让它们在不同的核上独立运行。
        //并行处理是多线程的一种，而多线程是并发的一种，
        //在现代程序中，还有一种非常重要的但很多人还不熟悉的并发类型：异步编程

        //4.异步编程  并发的一种形式，它采用future模式或回调（callback）模式，以避免产生不必要的线程。
        //一个future（或promise）类型代表一些即将完成的操作，在Net中新版的future类型有Task和Task<TResult>。 
        //在老式异步编程中API中，采用回调或事件（event），而不是future。
        //异步编程的核心理念是异步操作：启动了的操作将会在一段时间后完成。这个操作正在执行时，不会组合原来的线程，启动了这个操作的线程，可以继续执行其他的任务。当操作完成时，会通知它的future，或者调用回调函数，以便让程序知道操作已经结束。
        //异步编程是一种功能强大的并发形式，但直到不久前，实现异步编程仍需要特别复杂的代码。VS2012支持async和await，这让异步编程变得几乎和同步（非并发）一样简单
        //并发编程的另一种形式是响应式编程（reactive programming）。 异步编程意味着程序启动一个操作，而该操作将会在一段时间后完成。
        //响应式编程与异步编程非常相似，不过它是基于异步事件（asynchronous event）的，而不是异步操作（asynchronous operation）。
        //异步事件可以没有一个实际的“开始”，可以在任何事件发生，并且可以发生多次，例如用户输入。

        //5.响应式编程   一种声明式的编程模式，程序在该模式中对事件做出响应。
        //如果把一个程序看做一个大型的状态机，则该程序的行为便可视为它对一系列事件做出响应，即每换一个事件，它就更新一次自己的状态。这听起来很抽象和空洞，但实际上并非如此。利用现代的程序框架，响应式编程已经在实际开发中广泛使用。
        //响应式编程不一定是并发的，但它与并发编程联系紧密，因此本书介绍了响应式编程的基础知识。

        //通常情况下，一个并发程序要使用多种技术，大多数程序至少使用了多线程（通过线程池）和异步编程，
        //要大胆地把各种并发编程形式进行混合和匹配，在程序的各个部分使用合适的工具
    }
}
