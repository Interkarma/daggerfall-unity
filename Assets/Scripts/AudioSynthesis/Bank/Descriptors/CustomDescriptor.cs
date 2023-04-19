namespace DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class CustomDescriptor : IDescriptor
    {
        //--Fields
        private string id;
        private int size;
        private object[] objs;
        //--Properties
        public string ID { get { return id; } }
        public int Size { get { return size; } }
        public object[] Objects { get { return objs; } }
        //--Methods
        public CustomDescriptor(string id, int size, object[] objs)
        {
            this.id = id;
            this.size = size;
            this.objs = objs;
        }
        public CustomDescriptor(string id, int size)
        {
            this.id = id;
            this.size = size;
        }
        public void Read(string[] description)
        {
            Dictionary<string, object> desc = new Dictionary<string, object>();
            size = 0;
            for (int x = 0; x < description.Length; x++)
            {
                int index = description[x].IndexOf('=');
                if (index >= 0 && index < description[x].Length)
                {
                    int sizeInc;
                    string paramName = description[x].Substring(0, index).Trim().ToLower();
                    string paramValue = description[x].Substring(index + 1).Trim();
                    char type = paramValue[paramValue.Length - 1];
                    paramValue = paramValue.Substring(0, paramValue.Length - 1);
                    object obj = null;
                    switch(type)
                    {
                        case 'i':
                            obj = int.Parse(paramValue);
                            sizeInc = 5;
                            break;
                        case 's':
                            obj = short.Parse(paramValue);
                            sizeInc = 3;
                            break;
                        case 'b':
                            obj = byte.Parse(paramValue);
                            sizeInc = 2;
                            break;
                        case 'd':
                            obj = double.Parse(paramValue);
                            sizeInc = 9;
                            break;
                        case 'f':
                            obj = float.Parse(paramValue);
                            sizeInc = 5;
                            break;
                        case '&':
                            obj = paramValue;
                            if (paramValue.Length > 255)
                                sizeInc = 2 + 255;
                            else
                                sizeInc = 2 + paramValue.Length;
                            break;
                        default:
                            sizeInc = 0;
                            break;
                    }
                    if (obj != null)
                    {
                        if (desc.ContainsKey(paramName))
                            desc[paramName] = obj;
                        else
                        {
                            size += sizeInc;
                            desc.Add(paramName, obj);
                        }
                    }
                }
            }
            if (size % 2 == 1)
                size++;
            objs = new object[desc.Values.Count];
            desc.Values.CopyTo(objs, 0);
        }
        public int Read(BinaryReader reader)
        {
            List<object> objList = new List<object>();
            int read = 0;
            while (read < size)
            {
                switch ((char)reader.ReadByte())
                {
                    case '\0':
                        read++;
                        break;
                    case 'i':
                        objList.Add(reader.ReadInt32());
                        read += 5;
                        break;
                    case 's':
                        objList.Add(reader.ReadInt16());
                        read += 3;
                        break;
                    case 'b':
                        objList.Add(reader.ReadByte());
                        read += 2;
                        break;
                    case 'd':
                        objList.Add(reader.ReadDouble());
                        read += 9;
                        break;
                    case 'f':
                        objList.Add(reader.ReadSingle());
                        read += 5;
                        break;
                    case '&':
                        int strLen = reader.ReadByte();
                        objList.Add(IOHelper.Read8BitString(reader, strLen));
                        read += strLen + 2;
                        break;
                    default:
                        throw new Exception("Invalid custom descriptor: " + id);
                }
            }
            if (read > size)
                throw new Exception("Invalid custom descriptor: " + id);
            objs = objList.ToArray();
            return read;
        }
        public int Write(BinaryWriter writer)
        {
            int written = 0;
            for (int x = 0; x < objs.Length; x++)
            {
                if (objs[x] is int)
                {
                    writer.Write((byte)'i');
                    writer.Write((int)objs[x]);
                    written += 5;
                }
                else if (objs[x] is short)
                {
                    writer.Write((byte)'s');
                    writer.Write((short)objs[x]);
                    written += 3;
                }
                else if (objs[x] is byte)
                {
                    writer.Write((byte)'b');
                    writer.Write((byte)objs[x]);
                    written += 2;
                }
                else if (objs[x] is double)
                {
                    writer.Write((byte)'d');
                    writer.Write((double)objs[x]);
                    written += 9;
                }
                else if (objs[x] is float)
                {
                    writer.Write((byte)'f');
                    writer.Write((float)objs[x]);
                    written += 5;
                }
                else if (objs[x] is string)
                {
                    writer.Write((byte)'&');
                    string s = (string)objs[x];
                    writer.Write((byte)s.Length);
                    IOHelper.Write8BitString(writer, s, s.Length);
                    written += s.Length + 2;
                }
            }
            if (written < size)
            {
                do
                {
                    writer.Write((byte)0);
                    written++;
                } while (written < size);
            }
            else if (written > size)
            {
                throw new Exception("More bytes were written than expected.");
            }
            return written;
        }
    }
}
