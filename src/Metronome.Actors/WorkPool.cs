using System;
using System.Collections.Generic;
using Akka.Actor;
using Metronome.Core;

namespace Metronome.Actors
{
    public class WorkPool : ReceiveActor
    {
        private readonly IEnumerable<WorkDefinition> testCases;
        private IEnumerator<WorkDefinition> testCasesEnum;

        public class RequestWorkMsg
        {
        }

        public WorkPool(IEnumerable<WorkDefinition> testCases)
        {
            this.testCases = testCases;
            testCasesEnum = testCases.GetEnumerator();

            Receive<RequestWorkMsg>(m =>
            {
                if (!testCasesEnum.MoveNext())
                {
                    Console.WriteLine("Restarting the counter");
                    testCasesEnum.Reset();

                    testCasesEnum.MoveNext();
                }

                Context.Sender.Tell(
                    new Coordinator.NewWorkMsg
                    {
                        Work = testCasesEnum.Current
                    });
            });
        }
    }
}
