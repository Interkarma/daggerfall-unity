namespace DaggerfallWorkshop.AudioSynthesis.Util.Riff
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using DaggerfallWorkshop.AudioSynthesis.Util;
    
    public class ListChunk : Chunk
    {
        //--Fields
        private string listTypeId;
        private Chunk[] listSubChunks;
        //--Properties
        public string TypeId
        {
            get { return listTypeId; }
        }
        public Chunk[] SubChunks
        {
            get { return listSubChunks; }
        }
        //--Methods
        public ListChunk(string id, int size, BinaryReader reader, Func<BinaryReader, Chunk> listCallback)
            : base(id, size)
        {
            long readTo = reader.BaseStream.Position + size;
            listTypeId = new string(IOHelper.Read8BitChars(reader, 4));
            List<Chunk> chunkList = new List<Chunk>();
            while (reader.BaseStream.Position < readTo)
            {
                Chunk chk = listCallback.Invoke(reader);
                chunkList.Add(chk);
            }
            listSubChunks = chunkList.ToArray();
        }
    }
}
