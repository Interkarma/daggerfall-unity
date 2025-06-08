namespace DaggerfallWorkshop.AudioSynthesis.Bank
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using DaggerfallWorkshop.AudioSynthesis.Util;
    using DaggerfallWorkshop.AudioSynthesis.Bank.Descriptors;
    using DaggerfallWorkshop.AudioSynthesis.Sfz;
    using DaggerfallWorkshop.AudioSynthesis.Synthesis;

    public class DescriptorList
    {
        //--Fields
        public EnvelopeDescriptor[] EnvelopeDescriptions;
        public FilterDescriptor[] FilterDescriptions;
        public LfoDescriptor[] LfoDescriptions;
        public GeneratorDescriptor[] GenDescriptions;
        public CustomDescriptor[] CustomDescriptions;

        //--Properties
        public int DescriptorCount
        {
            get { return EnvelopeDescriptions.Length + FilterDescriptions.Length + LfoDescriptions.Length + GenDescriptions.Length + CustomDescriptions.Length; }
        }

        //--Methods
        public DescriptorList()
        {
            EnvelopeDescriptions = new EnvelopeDescriptor[0];
            FilterDescriptions = new FilterDescriptor[0];
            LfoDescriptions = new LfoDescriptor[0];
            GenDescriptions = new GeneratorDescriptor[0];
            CustomDescriptions = new CustomDescriptor[0];
        }
        public DescriptorList(StreamReader reader)
        {
            List<EnvelopeDescriptor> envList = new List<EnvelopeDescriptor>();
            List<FilterDescriptor> fltrList = new List<FilterDescriptor>();
            List<LfoDescriptor> lfoList = new List<LfoDescriptor>();
            List<GeneratorDescriptor> genList = new List<GeneratorDescriptor>();
            List<CustomDescriptor> cList = new List<CustomDescriptor>();
            List<string> descList = new List<string>();
            while (!reader.EndOfStream)
            {
                string tag = ReadNextTag(reader, descList); 
                switch (tag)
                {
                    case "envelope":
                    {
                        EnvelopeDescriptor env = new EnvelopeDescriptor();
                        env.Read(descList.ToArray());
                        envList.Add(env);
                        break;
                    }
                    case "generator":
                    {
                        GeneratorDescriptor gen = new GeneratorDescriptor();
                        gen.Read(descList.ToArray());
                        genList.Add(gen);
                        break;
                    }
                    case "filter":
                    {
                        FilterDescriptor fltr = new FilterDescriptor();
                        fltr.Read(descList.ToArray());
                        fltrList.Add(fltr);
                        break;
                    }
                    case "lfo":
                    {
                        LfoDescriptor lfo = new LfoDescriptor();
                        lfo.Read(descList.ToArray());
                        lfoList.Add(lfo);
                        break;
                    }
                    default:
                        if (!tag.Equals(string.Empty))
                        {
                            CustomDescriptor cus = new CustomDescriptor(tag, 0);
                            cus.Read(descList.ToArray());
                            cList.Add(cus);
                        }
                        break;
                }
                descList.Clear();
            }
            EnvelopeDescriptions = envList.ToArray();
            FilterDescriptions = fltrList.ToArray();
            LfoDescriptions = lfoList.ToArray();
            GenDescriptions = genList.ToArray();
            CustomDescriptions = cList.ToArray();
        }
        public DescriptorList(BinaryReader reader)
        {
            List<EnvelopeDescriptor> envList = new List<EnvelopeDescriptor>();
            List<FilterDescriptor> fltrList = new List<FilterDescriptor>();
            List<LfoDescriptor> lfoList = new List<LfoDescriptor>();
            List<GeneratorDescriptor> genList = new List<GeneratorDescriptor>();
            List<CustomDescriptor> cList = new List<CustomDescriptor>();
            int count = reader.ReadInt16();
            for (int x = 0; x < count; x++)
            {
                string id = new string(IOHelper.Read8BitChars(reader, 4));
                int size = reader.ReadInt32();
                switch (id.ToLower())
                {
                    case EnvelopeDescriptor.ID:
                    {
                        EnvelopeDescriptor env = new EnvelopeDescriptor();
                        env.Read(reader);
                        envList.Add(env);
                        break;
                    }
                    case GeneratorDescriptor.ID:
                    {
                        GeneratorDescriptor gen = new GeneratorDescriptor();
                        gen.Read(reader);
                        genList.Add(gen);
                        break;
                    }
                    case FilterDescriptor.ID:
                    {
                        FilterDescriptor fltr = new FilterDescriptor();
                        fltr.Read(reader);
                        fltrList.Add(fltr);
                        break;
                    }
                    case LfoDescriptor.ID:
                    {
                        LfoDescriptor lfo = new LfoDescriptor();
                        lfo.Read(reader);
                        lfoList.Add(lfo);
                        break;
                    }
                    default:
                    {
                        CustomDescriptor cus = new CustomDescriptor(id, size);
                        cus.Read(reader);
                        cList.Add(cus);
                        break;
                    }
                }
            }
            EnvelopeDescriptions = envList.ToArray();
            FilterDescriptions = fltrList.ToArray();
            LfoDescriptions = lfoList.ToArray();
            GenDescriptions = genList.ToArray();
            CustomDescriptions = cList.ToArray();
        }
        public DescriptorList(SfzRegion region)
        {
            LoadSfzEnvelopes(region);
            LoadSfzFilters(region);
            LoadSfzLfos(region);
            LoadSfzGens(region);
            LoadSfzCustom(region);
        }

        public CustomDescriptor FindCustomDescriptor(string name)
        {
            for (int x = 0; x < CustomDescriptions.Length; x++)
            {
                if (CustomDescriptions[x].ID.Equals(name))
                    return CustomDescriptions[x];
            }
            return null;
        }
        public void Write(BinaryWriter writer)
        {
            for (int x = 0; x < EnvelopeDescriptions.Length; x++)
            {
                IOHelper.Write8BitString(writer, EnvelopeDescriptor.ID, 4);
                writer.Write((int)EnvelopeDescriptor.SIZE);
                EnvelopeDescriptions[x].Write(writer);
            }
            for (int x = 0; x < FilterDescriptions.Length; x++)
            {
                IOHelper.Write8BitString(writer, FilterDescriptor.ID, 4);
                writer.Write((int)FilterDescriptor.SIZE);
                FilterDescriptions[x].Write(writer);
            }
            for (int x = 0; x < LfoDescriptions.Length; x++)
            {
                IOHelper.Write8BitString(writer, LfoDescriptor.ID, 4);
                writer.Write((int)LfoDescriptor.SIZE);
                LfoDescriptions[x].Write(writer);
            }
            for (int x = 0; x < GenDescriptions.Length; x++)
            {
                IOHelper.Write8BitString(writer, GeneratorDescriptor.ID, 4);
                writer.Write((int)GeneratorDescriptor.SIZE);
                GenDescriptions[x].Write(writer);
            }
            for (int x = 0; x < CustomDescriptions.Length; x++)
            {
                IOHelper.Write8BitString(writer, CustomDescriptions[x].ID, 4);
                writer.Write((int)CustomDescriptions[x].Size);
                CustomDescriptions[x].Write(writer);
            }
        }

        private void LoadSfzEnvelopes(SfzRegion region)
        {
            EnvelopeDescriptions = new EnvelopeDescriptor[3];
            EnvelopeDescriptions[0] = new EnvelopeDescriptor();
            EnvelopeDescriptions[0].DelayTime = region.pitchEGDelay;
            EnvelopeDescriptions[0].AttackTime = region.pitchEGAttack;
            EnvelopeDescriptions[0].HoldTime = region.pitchEGHold;
            EnvelopeDescriptions[0].DecayTime = region.pitchEGDecay;
            EnvelopeDescriptions[0].SustainLevel = region.pitchEGSustain / 100f;
            EnvelopeDescriptions[0].ReleaseTime = region.pitchEGRelease;
            EnvelopeDescriptions[0].StartLevel = region.pitchEGStart / 100f;
            EnvelopeDescriptions[0].Depth = region.pitchEGDepth;
            EnvelopeDescriptions[0].Vel2Delay = region.pitchEGVel2Delay;
            EnvelopeDescriptions[0].Vel2Attack = region.pitchEGVel2Attack;
            EnvelopeDescriptions[0].Vel2Hold = region.pitchEGVel2Hold;
            EnvelopeDescriptions[0].Vel2Decay = region.pitchEGVel2Decay;
            EnvelopeDescriptions[0].Vel2Sustain = region.pitchEGVel2Sustain;
            EnvelopeDescriptions[0].Vel2Release = region.pitchEGVel2Release;
            EnvelopeDescriptions[0].Vel2Depth = region.pitchEGVel2Depth;
            EnvelopeDescriptions[1] = new EnvelopeDescriptor();
            EnvelopeDescriptions[1].DelayTime = region.filterEGDelay;
            EnvelopeDescriptions[1].AttackTime = region.filterEGAttack;
            EnvelopeDescriptions[1].HoldTime = region.filterEGHold;
            EnvelopeDescriptions[1].DecayTime = region.filterEGDecay;
            EnvelopeDescriptions[1].SustainLevel = region.filterEGSustain / 100f;
            EnvelopeDescriptions[1].ReleaseTime = region.filterEGRelease;
            EnvelopeDescriptions[1].StartLevel = region.filterEGStart / 100f;
            EnvelopeDescriptions[1].Depth = region.filterEGDepth;
            EnvelopeDescriptions[1].Vel2Delay = region.filterEGVel2Delay;
            EnvelopeDescriptions[1].Vel2Attack = region.filterEGVel2Attack;
            EnvelopeDescriptions[1].Vel2Hold = region.filterEGVel2Hold;
            EnvelopeDescriptions[1].Vel2Decay = region.filterEGVel2Decay;
            EnvelopeDescriptions[1].Vel2Sustain = region.filterEGVel2Sustain;
            EnvelopeDescriptions[1].Vel2Release = region.filterEGVel2Release;
            EnvelopeDescriptions[1].Vel2Depth = region.filterEGVel2Depth;
            EnvelopeDescriptions[2] = new EnvelopeDescriptor();
            EnvelopeDescriptions[2].DelayTime = region.ampEGDelay;
            EnvelopeDescriptions[2].AttackTime = region.ampEGAttack;
            EnvelopeDescriptions[2].HoldTime = region.ampEGHold;
            EnvelopeDescriptions[2].DecayTime = region.ampEGDecay;
            EnvelopeDescriptions[2].SustainLevel = region.ampEGSustain / 100f;
            EnvelopeDescriptions[2].ReleaseTime = region.ampEGRelease;
            EnvelopeDescriptions[2].StartLevel = region.ampEGStart / 100f;
            EnvelopeDescriptions[2].Depth = 1f;
            EnvelopeDescriptions[2].Vel2Delay = region.ampEGVel2Delay;
            EnvelopeDescriptions[2].Vel2Attack = region.ampEGVel2Attack;
            EnvelopeDescriptions[2].Vel2Hold = region.ampEGVel2Hold;
            EnvelopeDescriptions[2].Vel2Decay = region.ampEGVel2Decay;
            EnvelopeDescriptions[2].Vel2Sustain = region.ampEGVel2Sustain;
            EnvelopeDescriptions[2].Vel2Release = region.ampEGVel2Release;
            EnvelopeDescriptions[2].Vel2Depth = 0f;
        }
        private void LoadSfzFilters(SfzRegion region)
        {
            FilterDescriptions = new FilterDescriptor[1];
            FilterDescriptions[0] = new FilterDescriptor();
            FilterDescriptions[0].FilterMethod = region.filterType;
            FilterDescriptions[0].CutOff = region.cutOff;
            FilterDescriptions[0].KeyTrack = region.filterKeyTrack;
            FilterDescriptions[0].Resonance = (float)SynthHelper.DBtoLinear(region.resonance);
            FilterDescriptions[0].RootKey = region.filterKeyCenter;
            FilterDescriptions[0].VelTrack = region.filterVelTrack;
        }
        private void LoadSfzLfos(SfzRegion region)
        {
            LfoDescriptions = new LfoDescriptor[3];
            LfoDescriptions[0] = new LfoDescriptor();
            LfoDescriptions[0].DelayTime = region.pitchLfoDelay; //make sure pitch lfo is enabled for midi mod event
            LfoDescriptions[0].Frequency = region.pitchLfoFrequency > 0 ? region.pitchLfoFrequency : (float)Synthesizer.DefaultLfoFrequency;
            LfoDescriptions[0].Depth = region.pitchLfoDepth;
            LfoDescriptions[1] = new LfoDescriptor();
            LfoDescriptions[1].DelayTime = region.filterLfoDelay;
            LfoDescriptions[1].Frequency = region.filterLfoFrequency;
            LfoDescriptions[1].Depth = region.filterLfoDepth;
            LfoDescriptions[2] = new LfoDescriptor();
            LfoDescriptions[2].DelayTime = region.ampLfoDelay;
            LfoDescriptions[2].Frequency = region.ampLfoFrequency;
            LfoDescriptions[2].Depth = (float)SynthHelper.DBtoLinear(region.ampLfoDepth);
        }
        private void LoadSfzGens(SfzRegion region)
        {
            GenDescriptions = new GeneratorDescriptor[1];
            GenDescriptions[0] = new GeneratorDescriptor();
            GenDescriptions[0].SamplerType = Components.WaveformEnum.SampleData;
            GenDescriptions[0].AssetName = region.sample;
            //deal with end point
            if (region.end == -1) //-1 is silent region, so set end to 0 and let the generator figure it out later
                GenDescriptions[0].EndPhase = 0;
            else if (region.end == 0) //set end out of range and let the descriptor default it to the proper end value
                GenDescriptions[0].EndPhase = -1;
            else //add one to the value because its inclusive
                GenDescriptions[0].EndPhase = region.end + 1;
            GenDescriptions[0].KeyTrack = region.pitchKeyTrack;
            //deal with loop end
            if (region.loopEnd < 0)
                GenDescriptions[0].LoopEndPhase = -1;
            else
                GenDescriptions[0].LoopEndPhase = region.loopEnd + 1;
            GenDescriptions[0].LoopMethod = region.loopMode;
            if (region.loopStart < 0)
                GenDescriptions[0].LoopStartPhase = -1;
            else
                GenDescriptions[0].LoopStartPhase = region.loopStart;
            GenDescriptions[0].Offset = region.offset;
            GenDescriptions[0].Rootkey = region.pitchKeyCenter;
            GenDescriptions[0].Tune = (short)(region.tune + region.transpose * 100);
            GenDescriptions[0].VelTrack = region.pitchVelTrack;
        }
        private void LoadSfzCustom(SfzRegion region)
        {
            CustomDescriptions = new CustomDescriptor[1];
            CustomDescriptions[0] = new CustomDescriptor("sfzi", 32,
                new object[] { region.offBy, region.group, region.volume, region.pan / 100f, region.ampKeyTrack, region.ampKeyCenter, region.ampVelTrack / 100f });
        }

        private static string ReadNextTag(StreamReader reader, List<string> descList)
        {
            string tagName;
            string closeTag;
            string description;
            StringBuilder sbuild = new StringBuilder();
            int c = reader.Read();
            //skip anything outside of the tags
            while (c != -1 && c != '<')
                c = reader.Read();
            //read opening tag
            c = reader.Read();
            while (c != -1 && c != '>')
            {
                sbuild.Append((char)c);
                c = reader.Read();
            }
            tagName = sbuild.ToString().Trim().ToLower();
            //sbuild.Clear();
            sbuild.Remove(0, sbuild.Length);
            //read the description
            c = reader.Read();
            while (c != -1 && c != '<')
            {
                sbuild.Append((char)c);
                c = reader.Read();
            }
            description = sbuild.ToString();
            //sbuild.Clear();
            sbuild.Remove(0, sbuild.Length);
            //read closing tag
            c = reader.Read();
            while (c != -1 && c != '>')
            {
                sbuild.Append((char)c);
                c = reader.Read();
            }
            closeTag = sbuild.ToString().Trim().ToLower();
            if (closeTag.Length > 1 && closeTag.StartsWith("/") && closeTag.Substring(1).Equals(tagName))
            {
                descList.AddRange(description.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries));
                return tagName;
            }
            else
            {
                throw new Exception("Invalid tag! <" + tagName + ">...<" + closeTag + ">");
            }
        }
    }
}
