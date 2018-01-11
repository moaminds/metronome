namespace Metronome.Core
{
    public abstract class WorkDefinition
    {
        public override string ToString()
        {
            return "I am a base work definition";
        }
    }

    public class NumbersWorkDefinition : WorkDefinition
    {
        public int Num { get; }
        public int SleepingTimeInMs { get; }

        public NumbersWorkDefinition(int num, int sleepingTimeInMs)
        {
            Num = num;
            SleepingTimeInMs = sleepingTimeInMs;
        }

        public override string ToString()
        {
            return $"I am a Numbers work definition {Num}";
        }
    }

    public class DoubleWorkDefinition : NumbersWorkDefinition
    {
        public DoubleWorkDefinition(int num, int sleepingTimeInMs)
            : base(num, sleepingTimeInMs)
        {
        }

        public override string ToString()
        {
            return $"I am a Double work definition {Num}";
        }
    }
}
