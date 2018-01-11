using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using Akka.Routing;
using CommandLine;
using Metronome.Actors;
using Metronome.Core;
using SimpleInjector;

namespace Metronome.Main
{
    internal class Options
    {
        public Options()
        {
            Agents = 3;
            SleepingTimeIsMs = 3000;
            Tasks = 3;
        }
        [Value(1, MetaName = "stress", HelpText = "How many parallel agents")]
        public int Agents { get; set; }

        [Value(2, MetaName = "sleeping", HelpText = "How long to sleep simulating real work")]
        public int SleepingTimeIsMs { get; set; }

        [Value(3, MetaName = "tasks", HelpText = "How many tasks are there in the work pool")]
        public int Tasks { get; set; }
    }


    class Program
    {
        public static ActorSystem ActorSystemInstance;

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var container = new Container();
            container.RegisterSingleton<IWorkProcessor, WorkProcessor>();

            var argsParsed = CommandLine.Parser.Default.ParseArguments<Options>(args);
            var ops = argsParsed.MapResult(a => a, _ => throw new InvalidOperationException());

            Console.WriteLine($"Agents set to {ops.Agents}");

            ActorSystemInstance = ActorSystem.Create("TheActorSystem");
            var system = ActorSystemInstance;

            // Create the dependency resolver
            IDependencyResolver resolver = new SimpleInjectorDependencyResolver(container, system);

            var workItems = Enumerable.Range(0, ops.Tasks)
                .Select(i => i % 2 == 0 ?
                new NumbersWorkDefinition(i + 1, ops.SleepingTimeIsMs) :
                new DoubleWorkDefinition(i + 1, ops.SleepingTimeIsMs))
                .ToList();

            var workPool = system.ActorOf(Props.Create<WorkPool>(workItems));

            var router = system.ActorOf(resolver.Create<Agent>().WithRouter(new BroadcastPool(ops.Agents)), "some-pool");
            var coordinator = system.ActorOf(Props.Create<Coordinator>(ops.Agents, router, workPool));
            coordinator.Tell(new Coordinator.GoMsg());

            // This blocks the current thread from exiting until MyActorSystem is shut down
            // The ConsoleReaderActor will shut down the ActorSystem once it receives an 
            // "exit" command from the user
            await ActorSystemInstance.WhenTerminated;
        }
    }
}
