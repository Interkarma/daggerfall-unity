using System;
using DaggerfallWorkshop.AudioSynthesis.Synthesis;
using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators;

namespace DaggerfallWorkshop.AudioSynthesis.Util
{
    public static class Tables
    {
        internal static readonly float[][] EnvelopeTables;
        internal static readonly double[] SemitoneTable;
        internal static readonly double[] CentTable;
         
        /*Creates tables in static constructor*/
        static Tables()
        {
            const int EnvelopeSize = 128;
            EnvelopeTables = new float[4][];
            EnvelopeTables[0] = RemoveDenormals(CreateSustainTable(EnvelopeSize));
            EnvelopeTables[1] = RemoveDenormals(CreateLinearTable(EnvelopeSize));
            EnvelopeTables[2] = RemoveDenormals(CreateConcaveTable(EnvelopeSize));
            EnvelopeTables[3] = RemoveDenormals(CreateConvexTable(EnvelopeSize));
            CentTable = CreateCentTable();
            SemitoneTable = CreateSemitoneTable();
        }

        public static float[] CreateTable(int size, WaveformEnum type)
        {
            Generator generator;
            if (type == WaveformEnum.Sine)
                generator = new SineGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.Square)
                generator = new SquareGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.Triangle)
                generator = new TriangleGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.Saw)
                generator = new SawGenerator(new GeneratorDescriptor());
            else if (type == WaveformEnum.WhiteNoise)
                generator = new WhiteNoiseGenerator(new GeneratorDescriptor());
            else
                return null;
            float[] table = new float[size];
            double phase, increment;
            phase = generator.StartPhase;
            increment = generator.Period / size;
            for (int x = 0; x < table.Length; x++)
            {
                table[x] = generator.GetValue(phase);
                phase += increment;
            }
            return table;
        }

        /*Cent table contains 2^12 ratio for pitches in the range of (-1 to +1) semitone.
          Accuracy between semitones is 1/100th of a note or 1 cent. */
        public static double[] CreateCentTable()
        {//-100 to 100 cents
            double[] cents = new double[201];
            for(int x = 0; x < cents.Length; x++)
            {
                cents[x] = Math.Pow(2.0, (x - 100.0) / 1200.0);
            }
            return cents;
        }
             
        /*Semitone table contains pitches for notes in range of -127 to 127 semitones.
          Used to calculate base pitch when voice is started. ex. (basepitch = semiTable[midinote - rootkey]) */
        public static double[] CreateSemitoneTable()
        {//-127 to 127 semitones
            double[] table = new double[255];
            for (int x = 0; x < table.Length; x++)
            {
                table[x] = Math.Pow(2.0, (x - 127.0) / 12.0);
            }
            return table;
        }
        
        /*Envelope Equations*/
        public static float[] CreateSustainTable(int size)
        {
            float[] graph = new float[size];
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = 1;
            }
            return graph;
        }
        public static float[] CreateLinearTable(int size)
        {
            float[] graph = new float[size];
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = x / (float)(size - 1);
            }
            return graph;
        }
        public static float[] CreateConcaveTable(int size)
        {//follows sf2 spec
            float[] graph = new float[size];
            const double c = -(20.0 / 96.0);
            int max = (size - 1) * (size - 1);
            for (int x = 0; x < graph.Length; x++)
            {
                int i = (size - 1) - x;
                graph[x] = (float)(c * Math.Log10((i * i) / (double)max));
            }
            graph[size - 1] = 1f;
            return graph;
        }
        public static float[] CreateConvexTable(int size)
        {//follows sf2 spec
            float[] graph = new float[size];
            const double c = (20.0 / 96.0);
            int max = (size - 1) * (size - 1);
            for (int x = 0; x < graph.Length; x++)
            {
                graph[x] = (float)(1 + c * Math.Log10((x * x) / (double)max));
            }
            graph[0] = 0f;
            return graph;
        }
        private static float[] RemoveDenormals(float[] data)
        {
            for (int x = 0; x < data.Length; x++)
            {
                if (Math.Abs(data[x]) <= Synthesizer.DenormLimit)
                    data[x] = 0f;
            }
            return data;
        }

        /*Windowing methods*/
        public static double VonHannWindow(double i, int size)
        {
            return 0.5 - 0.5 * Math.Cos(Synthesizer.TwoPi * (0.5 + i / size));
        }
        public static double HammingWindow(double i, int size)
        {
            return 0.54 - 0.46 * Math.Cos(Synthesizer.TwoPi * i / size);  
        }
        public static double BlackmanWindow(double i, int size)
        {
            return 0.42659 - 0.49656 * Math.Cos(Synthesizer.TwoPi * i / size) + 0.076849 * Math.Cos(4.0 * Math.PI * i / size);
        }
    }
}
