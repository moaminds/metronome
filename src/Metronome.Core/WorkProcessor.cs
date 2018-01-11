using System;
using System.Threading.Tasks;

namespace Metronome.Core
{
    public class WorkProcessor : 
        IWorkProcessor, 
        IWorkHandler<NumbersWorkDefinition>,
        IWorkHandler<DoubleWorkDefinition>
    {
        public Task ProcessAsync(WorkDefinition work)
        {
            return HandleAsync((dynamic)work);
        }

        public Task HandleAsync(NumbersWorkDefinition work)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"Staring work {work}");
                await Task.Delay(TimeSpan.FromMilliseconds(work.SleepingTimeInMs));
            });
        }

        public Task HandleAsync(DoubleWorkDefinition work)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"Staring work {work}");
                await Task.Delay(TimeSpan.FromMilliseconds(2 * work.SleepingTimeInMs));
            });
        }
    }
}
