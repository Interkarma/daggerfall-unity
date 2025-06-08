using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using DaggerfallWorkshop.AudioSynthesis.Bank.Components;
using DaggerfallWorkshop.AudioSynthesis.Util;

namespace DaggerfallWorkshop.AudioSynthesis.Sfz
{
    public class SfzReader
    {
        private string name;
        private SfzRegion[] regionList;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public SfzRegion[] Regions
        {
            get { return regionList; }
        }

        public SfzReader(Stream stream, string name)
        {
            this.name = IOHelper.GetFileNameWithoutExtension(name);
            Load(stream);
        }

        private void Load(Stream reader)
        {
            List<SfzRegion> regions = new List<SfzRegion>();
            using (reader)
            {
                SfzRegion group = new SfzRegion(true);
                SfzRegion global = new SfzRegion(true);
                SfzRegion master = new SfzRegion(true);
                string[] regionText = new string[2];
                ReadNextString(reader, '<'); //skip everything before the first region
                while (ReadNextRegion(reader, regionText)) 
                {
                    switch (regionText[0].ToLower())
                    {
                        case "global":
                            ToRegion(regionText[1], global);
                            break;
                        case "master":
                            ToRegion(regionText[1], master);
                            break;
                        case "group":
                            ToRegion(regionText[1], group);
                            break;
                        case "region":
                            SfzRegion r = new SfzRegion(false);
                            r.ApplyGlobal(global);
                            r.ApplyGlobal(master);
                            r.ApplyGlobal(group);
                            ToRegion(regionText[1], r);
                            if (!r.sample.Equals(string.Empty))
                                regions.Add(r);
                            break;
                        default:
                            break;
                    }
                }
            }
            regionList = regions.ToArray();
        }
        private string ReadNextString(Stream reader, char marker)
        {
            StringBuilder sbuild = new StringBuilder();
            int i = reader.ReadByte();
            while (true)
            {
                if (i == -1 || i == marker)
                    break;
                else if (i == '/')
                {
                    i = reader.ReadByte();
                    if (i == '/')
                    {
                        do { i = reader.ReadByte(); } while (i != '\n' && i != -1);
                        i = reader.ReadByte();
                    }
                    else
                        sbuild.Append('/');
                }
                else
                {
                    sbuild.Append((char)i);
                    i = reader.ReadByte();
                }       
            }
            return sbuild.ToString();
        }
        private bool ReadNextRegion(Stream reader, string[] regionData)
        {
            regionData[0] = ReadNextString(reader, '>');
            regionData[1] = ReadNextString(reader, '<');
            return !regionData[0].Equals(string.Empty) || !regionData[1].Equals(string.Empty);
        }
        private void ToRegion(string regionText, SfzRegion region)
        {
            string[] param = new string[2];
            int index = ReadNextParam(0, regionText, param);
            while (index != -1)
            {
                string command = param[0].ToLower();
                string parameter = param[1].ToLower();
                switch (command)
                {
                    case "sample":
                        if (IOHelper.GetExtension(parameter).Equals(string.Empty))
                            parameter += ".wav";
                        region.sample = parameter;
                        break;
                    case "lochan":
                        region.loChan = (byte)(int.Parse(parameter) - 1);
                        break;
                    case "hichan":
                        region.hiChan = (byte)(int.Parse(parameter) - 1);
                        break;
                    case "lokey":
                        region.loKey = NoteNameToValue(parameter);
                        break;
                    case "hikey":
                        region.hiKey = NoteNameToValue(parameter);
                        break;
                    case "key":
                        region.loKey = NoteNameToValue(parameter);
                        region.hiKey = region.loKey;
                        region.pitchKeyCenter = region.loKey;
                        break;
                    case "lovel":
                        region.loVel = byte.Parse(parameter);
                        break;
                    case "hivel":
                        region.hiVel = byte.Parse(parameter);
                        break;
                    case "lobend":
                        region.loBend = short.Parse(parameter);
                        break;
                    case "hibend":
                        region.hiBend = short.Parse(parameter);
                        break;
                    case "lochanaft":
                        region.loChanAft = byte.Parse(parameter);
                        break;
                    case "hichanaft":
                        region.hiChanAft = byte.Parse(parameter);
                        break;
                    case "lopolyaft":
                        region.loPolyAft = byte.Parse(parameter);
                        break;
                    case "hipolyaft":
                        region.hiPolyAft = byte.Parse(parameter);
                        break;
                    case "group":
                        region.group = int.Parse(parameter);
                        break;
                    case "off_by":
                        region.offBy = int.Parse(parameter);
                        break;
                    case "off_mode":
                        region.offMode = parameter.Equals("fast", StringComparison.InvariantCultureIgnoreCase) ? SfzRegion.OffModeEnum.Fast : SfzRegion.OffModeEnum.Normal;
                        break;
                    case "delay":
                        region.delay = float.Parse(parameter);
                        break;
                    case "offset":
                        region.offset = int.Parse(parameter);
                        break;
                    case "end":
                        region.end = int.Parse(parameter);
                        break;
                    case "count":
                        region.count = int.Parse(parameter);
                        region.loopMode = LoopModeEnum.OneShot;
                        break;
                    case "loop_mode":
                        switch (parameter)
                        {
                            case "no_loop":
                                region.loopMode = LoopModeEnum.NoLoop;
                                break;
                            case "one_shot":
                                region.loopMode = LoopModeEnum.OneShot;
                                break;
                            case "loop_continuous":
                                region.loopMode = LoopModeEnum.Continuous;
                                break;
                            case "loop_sustain":
                                region.loopMode = LoopModeEnum.LoopUntilNoteOff;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "loop_start":
                        region.loopStart = int.Parse(parameter);
                        break;
                    case "loop_end":
                        region.loopEnd = int.Parse(parameter);
                        break;
                    case "transpose":
                        region.transpose = short.Parse(parameter);
                        break;
                    case "tune":
                        region.tune = short.Parse(parameter);
                        break;
                    case "pitch_keycenter":
                        region.pitchKeyCenter = NoteNameToValue(parameter);
                        break;
                    case "pitch_keytrack":
                        region.pitchKeyTrack = short.Parse(parameter);
                        break;
                    case "pitch_veltrack":
                        region.pitchVelTrack = short.Parse(parameter);
                        break;
                    case "pitcheg_delay":
                        region.pitchEGDelay = float.Parse(parameter);
                        break;
                    case "pitcheg_start":
                        region.pitchEGStart = float.Parse(parameter);
                        break;
                    case "pitcheg_attack":
                        region.pitchEGAttack = float.Parse(parameter);
                        break;
                    case "pitcheg_hold":
                        region.pitchEGHold = float.Parse(parameter);
                        break;
                    case "pitcheg_decay":
                        region.pitchEGDecay = float.Parse(parameter);
                        break;
                    case "pitcheg_sustain":
                        region.pitchEGSustain = float.Parse(parameter);
                        break;
                    case "pitcheg_release":
                        region.pitchEGRelease = float.Parse(parameter);
                        break;
                    case "pitcheg_depth":
                        region.pitchEGDepth = short.Parse(parameter);
                        break;
                    case "pitcheg_vel2delay":
                        region.pitchEGVel2Delay = float.Parse(parameter);
                        break;
                    case "pitcheg_vel2attack":
                        region.pitchEGVel2Attack = float.Parse(parameter);
                        break;
                    case "pitcheg_vel2hold":
                        region.pitchEGVel2Hold = float.Parse(parameter);
                        break;
                    case "pitcheg_vel2decay":
                        region.pitchEGVel2Decay = float.Parse(parameter);
                        break;
                    case "pitcheg_vel2sustain":
                        region.pitchEGVel2Sustain = float.Parse(parameter);
                        break;
                    case "pitcheg_vel2release":
                        region.pitchEGVel2Release = float.Parse(parameter);
                        break;
                    case "pitcheg_vel2depth":
                        region.pitchEGVel2Depth = short.Parse(parameter);
                        break;
                    case "pitchlfo_delay":
                        region.pitchLfoDelay = float.Parse(parameter);
                        break;
                    case "pitchlfo_freq":
                        region.pitchLfoFrequency = float.Parse(parameter);
                        break;
                    case "pitchlfo_depth":
                        region.pitchLfoDepth = short.Parse(parameter);
                        break;
                    case "fil_type":
                        switch (parameter)
                        {
                            case "lpf_1p":
                                region.filterType = FilterTypeEnum.OnePoleLowpass;
                                break;
                            case "hpf_1p":
                                region.filterType = FilterTypeEnum.None;//unsupported
                                break;
                            case "lpf_2p":
                                region.filterType = FilterTypeEnum.BiquadLowpass;
                                break;
                            case "hpf_2p":
                                region.filterType = FilterTypeEnum.BiquadHighpass;
                                break;
                            case "bpf_2p":
                                region.filterType = FilterTypeEnum.None;//unsupported
                                break;
                            case "brf_2p":
                                region.filterType = FilterTypeEnum.None;//unsupported
                                break;
                            default:
                                break;
                        }
                        break;
                    case "cutoff":
                        region.cutOff = float.Parse(parameter);
                        break;
                    case "resonance":
                        region.resonance = float.Parse(parameter);
                        break;
                    case "fil_keytrack":
                        region.filterKeyTrack = short.Parse(parameter);
                        break;
                    case "fil_keycenter":
                        region.filterKeyCenter = byte.Parse(parameter);
                        break;
                    case "fil_veltrack":
                        region.filterVelTrack = short.Parse(parameter);
                        break;
                    case "fileg_delay":
                        region.filterEGDelay = float.Parse(parameter);
                        break;
                    case "fileg_start":
                        region.filterEGStart = float.Parse(parameter);
                        break;
                    case "fileg_attack":
                        region.filterEGAttack = float.Parse(parameter);
                        break;
                    case "fileg_hold":
                        region.filterEGHold = float.Parse(parameter);
                        break;
                    case "fileg_decay":
                        region.filterEGDecay = float.Parse(parameter);
                        break;
                    case "fileg_sustain":
                        region.filterEGSustain = float.Parse(parameter);
                        break;
                    case "fileg_release":
                        region.filterEGRelease = float.Parse(parameter);
                        break;
                    case "fileg_depth":
                        region.filterEGDepth = short.Parse(parameter);
                        break;
                    case "fileg_vel2delay":
                        region.filterEGVel2Delay = float.Parse(parameter);
                        break;
                    case "fileg_vel2attack":
                        region.filterEGVel2Attack = float.Parse(parameter);
                        break;
                    case "fileg_vel2hold":
                        region.filterEGVel2Hold = float.Parse(parameter);
                        break;
                    case "fileg_vel2decay":
                        region.filterEGVel2Decay = float.Parse(parameter);
                        break;
                    case "fileg_vel2sustain":
                        region.filterEGVel2Sustain = float.Parse(parameter);
                        break;
                    case "fileg_vel2release":
                        region.filterEGVel2Release = float.Parse(parameter);
                        break;
                    case "fileg_vel2depth":
                        region.filterEGVel2Depth = short.Parse(parameter);
                        break;
                    case "fillfo_delay":
                        region.filterLfoDelay = float.Parse(parameter);
                        break;
                    case "fillfo_freq":
                        region.filterLfoFrequency = float.Parse(parameter);
                        break;
                    case "fillfo_depth":
                        region.filterLfoDepth = float.Parse(parameter);
                        break;
                    case "volume":
                        region.volume = float.Parse(parameter);
                        break;
                    case "pan":
                        region.pan = float.Parse(parameter);
                        break;
                    case "amp_keytrack":
                        region.ampKeyTrack = float.Parse(parameter);
                        break;
                    case "amp_keycenter":
                        region.ampKeyCenter = byte.Parse(parameter);
                        break;
                    case "amp_veltrack":
                        region.ampVelTrack = float.Parse(parameter);
                        break;
                    case "ampeg_delay":
                        region.ampEGDelay = float.Parse(parameter);
                        break;
                    case "ampeg_start":
                        region.ampEGStart = float.Parse(parameter);
                        break;
                    case "ampeg_attack":
                        region.ampEGAttack = float.Parse(parameter);
                        break;
                    case "ampeg_hold":
                        region.ampEGHold = float.Parse(parameter);
                        break;
                    case "ampeg_decay":
                        region.ampEGDecay = float.Parse(parameter);
                        break;
                    case "ampeg_sustain":
                        region.ampEGSustain = float.Parse(parameter);
                        break;
                    case "ampeg_release":
                        region.ampEGRelease = float.Parse(parameter);
                        break;
                    case "ampeg_vel2delay":
                        region.ampEGVel2Delay = float.Parse(parameter);
                        break;
                    case "ampeg_vel2attack":
                        region.ampEGVel2Attack = float.Parse(parameter);
                        break;
                    case "ampeg_vel2hold":
                        region.ampEGVel2Hold = float.Parse(parameter);
                        break;
                    case "ampeg_vel2decay":
                        region.ampEGVel2Decay = float.Parse(parameter);
                        break;
                    case "ampeg_vel2sustain":
                        region.ampEGVel2Sustain = float.Parse(parameter);
                        break;
                    case "ampeg_vel2release":
                        region.ampEGVel2Release = float.Parse(parameter);
                        break;
                    case "amplfo_delay":
                        region.ampLfoDelay = float.Parse(parameter);
                        break;
                    case "amplfo_freq":
                        region.ampLfoFrequency = float.Parse(parameter);
                        break;
                    case "amplfo_depth":
                        region.ampLfoDepth = float.Parse(parameter);
                        break;
                    default:
                        break;
                }
                index = ReadNextParam(index, regionText, param);
            }
        }
        private int ReadNextParam(int index, string regionText, string[] result)
        {
            int i1, i2;
            i1 = regionText.IndexOf('=', index);
            if (i1 < 0 || i1 == regionText.Length - 1) 
                return -1;
            result[0] = regionText.Substring(index, i1 - index).Trim();
            i2 = regionText.IndexOf('=', i1 + 1);
            if (i2 < 0)
                i2 = regionText.Length - 1;
            else
            {
                while(char.IsWhiteSpace(regionText[i2])) i2--;
                while(!char.IsWhiteSpace(regionText[i2])) i2--;
            }
            i1++;
            result[1] = regionText.Substring(i1,i2 - i1).Trim();
            return i2;
        }
        private byte NoteNameToValue(string name)
        {
            int value = 0, i;
            if (int.TryParse(name, out value))
                return (byte)value;
            const string notes = "cdefgab";
            int[] noteValues = { 0, 2, 4, 5, 7, 9, 11 };
            name = name.Trim().ToLower();
            
            for (i = 0; i < name.Length; i++)
            {
                int index = notes.IndexOf(name[i]);
                if (index >= 0)
                {
                    value = noteValues[index];
                    i++;
                    break;
                }
            }
            while (i < name.Length)
            {
                if (name[i] == '#')
                {
                    value--;
                    i++;
                    break;
                }
                else if (name[i] == 'b')
                {
                    value--;
                    i++;
                    break;
                }
                i++;
            }
            string digit = string.Empty;
            while (i < name.Length)
            {
                if (char.IsDigit(name[i]))
                {
                    digit += name[i];
                    i++;
                }
                else
                    break;
            }
            if (digit.Equals(string.Empty))
                digit = "0";
            return (byte)((int.Parse(digit) + 1) * 12 + value);
        }
    }
}
