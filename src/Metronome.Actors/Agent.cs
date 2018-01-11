using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Metronome.Core;

namespace Metronome.Actors
{
    public class Agent : ReceiveActor
    {
        private readonly Stopwatch watch = new Stopwatch();

        public IWorkProcessor WorkProcessor { get; }

        public class StartWorkMsg
        {
            public IActorRef Coordinator { get; set; }

            public WorkDefinition Work { get; set; }
        }

        public class WorkDoneMsg
        {
            public IActorRef Coordinator { get; set; }

            public WorkResult Result { get; set; }
        }


        public Agent(IWorkProcessor workProcessor)
        {
            WorkProcessor = workProcessor;

            Idle();
        }

        private void Working()
        {
            Receive<WorkDoneMsg>(m =>
            {
                var state = m.Result.Error == null ? "well" : "badly";
                Console.WriteLine($"Work is done ({state}) {m.Result.Work} for {m.Result.ExecutionTime.TotalMilliseconds} ms");

                m.Coordinator.Tell(new Coordinator.WorkDoneMsg { Result = m.Result });

                Become(Idle);
            });
        }

        private void Idle()
        {
            Receive<StartWorkMsg>(m =>
            {
                watch.Start();

                WorkProcessor.ProcessAsync(m.Work)
                    .ContinueWith(r =>
                    {
                        watch.Stop();
                        var executionTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                        watch.Reset();

                        return new WorkDoneMsg
                        {
                            Coordinator = m.Coordinator,
                            Result = new WorkResult
                            {
                                Work = m.Work,
                                ExecutionTime = executionTime,
                                Error = r.IsFaulted ? r.Exception : null
                            }
                        };
                    }).PipeTo(Self);

                Become(Working);
            });
        }
    }
}
