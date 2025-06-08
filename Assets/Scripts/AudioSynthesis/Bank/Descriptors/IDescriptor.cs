using System.IO;

namespace DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors
{
    public interface IDescriptor
    {
        void Read(string[] description);
        int Read(BinaryReader reader);
        int Write(BinaryWriter writer);
    }
}
