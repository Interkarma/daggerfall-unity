namespace DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators
{
    public class GeneratorParameters
    {
        public double phase;
        public double currentStart;
        public double currentEnd;
        public GeneratorStateEnum currentState;

        public void QuickSetup(Generator generator)
        {
            currentStart = generator.StartPhase;
            phase = currentStart + generator.Offset;
            switch (generator.LoopMode)
            {
                case LoopModeEnum.Continuous:
                case LoopModeEnum.LoopUntilNoteOff:
                    if (phase >= generator.EndPhase)
                    {//phase is greater than the end index so generator is finished
                        currentState = GeneratorStateEnum.Finished;
                    }
                    else if (phase >= generator.LoopEndPhase)
                    {//phase is greater than the loop end point so generator is in post loop
                        currentState = GeneratorStateEnum.PostLoop;
                        currentEnd = generator.EndPhase;
                    }
                    else if (phase >= generator.LoopStartPhase)
                    {//phase is greater than loop start so we are inside the loop
                        currentState = GeneratorStateEnum.Loop;
                        currentEnd = generator.LoopEndPhase;
                        currentStart = generator.LoopStartPhase;
                    }
                    else
                    {//phase is less than the loop so generator is in pre loop
                        currentState = GeneratorStateEnum.PreLoop;
                        currentEnd = generator.LoopStartPhase;
                    }
                    break;
                default:
                    currentEnd = generator.EndPhase;
                    if (phase >= currentEnd)
                        currentState = GeneratorStateEnum.Finished;
                    else
                        currentState = GeneratorStateEnum.PostLoop;
                    break;
            }
        }
        public override string ToString()
        {
            return string.Format("State: {0}, Bounds: {1} to {2}, CurrentIndex: {3:0.00}", currentState, currentStart, currentEnd, phase);
        }
    }
}
