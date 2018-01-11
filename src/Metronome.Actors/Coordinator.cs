using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Akka.Actor;
using Metronome.Core;

namespace Metronome.Actors
{
    public class Coordinator : ReceiveActor
    {
        private readonly Stopwatch watch = new Stopwatch();

        private readonly List<WorkResult> results = new List<WorkResult>();

        private readonly int workers;
        private readonly IActorRef router;
        private readonly IActorRef workPool;
        private int pendingTasks;

        public class GoMsg
        {
        }

        public class WorkDoneMsg
        {
            public WorkResult Result { get; set; }
        }

        public class NewWorkMsg
        {
            public WorkDefinition Work { get; set; }
        }

        public Coordinator(int workers, IActorRef router, IActorRef workPool)
        {
            this.workers = workers;
            this.router = router;
            this.workPool = workPool;

            Receive<GoMsg>(m =>
            {
                workPool.Tell(new WorkPool.RequestWorkMsg());
            });

            Receive<NewWorkMsg>(w =>
            {
                Console.WriteLine("Starting new batch");

                results.Clear();
                watch.Start();

                pendingTasks = workers;

                router.Tell(new Agent.StartWorkMsg { Work = w.Work, Coordinator = Self });
            });

            Receive<WorkDoneMsg>(w =>
            {
                results.Add(w.Result);

                pendingTasks--;

                if (pendingTasks == 0)
                {
                    watch.Stop();

                    var ms = watch.ElapsedMilliseconds;
                    var avgTime = results.Average(x => x.ExecutionTime.TotalMilliseconds);

                    Console.WriteLine($"batch is done for {ms} ms. The avg exec time {avgTime} ms");

                    watch.Reset();

                    results.Clear();

                    workPool.Tell(new WorkPool.RequestWorkMsg());
                }
            });
        }
    }
}