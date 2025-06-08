using System.IO;

namespace DaggerfallWorkshop.AudioSynthesis.Sf2
{
    public class Modulator
    {
        private ModulatorType sourceModulationData;
        private GeneratorEnum destinationGenerator;
        private short amount;
        private ModulatorType sourceModulationAmount;
        private TransformEnum sourceTransform;

        public Modulator(BinaryReader reader)
        {
            sourceModulationData = new ModulatorType(reader);
            destinationGenerator = (GeneratorEnum)reader.ReadUInt16();
            amount = reader.ReadInt16();
            sourceModulationAmount = new ModulatorType(reader);
            sourceTransform = (TransformEnum)reader.ReadUInt16();
        }
        public override string ToString()
        {
            return string.Format("Modulator {0} : Amount: {1}", sourceModulationData, amount);
        }
    }
}
