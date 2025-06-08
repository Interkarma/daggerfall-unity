namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System.IO;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class RiffTypeChunk : Chunk
    {
        //--Fields
        private string typeId;
        //--Properties
        public string TypeId
        {
            get { return typeId; }
        }
        //--Methods
        public RiffTypeChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            typeId = new string(IOHelper.Read8BitChars(reader, 4));
        }
    }
}
