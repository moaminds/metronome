using System;
using Metronome.Core;

namespace Metronome.Actors
{
    public class WorkResult
    {
        public TimeSpan ExecutionTime { get; set; }

        public Exception Error { get; set; }

        public WorkDefinition Work { get; set; }
    }
}
