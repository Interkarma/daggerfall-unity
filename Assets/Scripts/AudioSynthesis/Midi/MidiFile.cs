namespace DaggerfallWorkshop.AudioSynthesis.Midi
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using DaggerfallWorkshop.AudioSynthesis.Midi.Event;
    using DaggerfallWorkshop.AudioSynthesis.Util;

    public class MidiFile
    {
        public enum TrackFormat { SingleTrack, MultiTrack, MultiSong }
        public enum TimeFormat { TicksPerBeat, FamesPerSecond }
        private int mDivision; //either in ticks per beat or frames per second
        private TrackFormat mTrackFormat;
        private TimeFormat mTimeFormat;
        private MidiTrack[] mTracks;

        public int Division
        {
            get { return mDivision; }
        }
        public TrackFormat MidiFormat
        {
            get { return mTrackFormat; }
        }
        public TimeFormat TimingStandard
        {
            get { return mTimeFormat; }
        }
        public MidiTrack[] Tracks
        {
            get { return mTracks; }
        }

        public MidiFile(IResource midiFile)
        {
            if (!midiFile.ReadAllowed())
                throw new Exception("The midi file must have read access.");
            using (BinaryReader reader = new BinaryReader(midiFile.OpenResourceForRead()))
            {
                LoadStream(reader);
            }
        }
        public void CombineTracks()
        {
            //create a new track of the appropriate size
            MidiTrack finalTrack = MergeTracks();
            MidiEvent[][] absevents = new MidiEvent[mTracks.Length][];
            //we have to convert delta times to absolute delta times
            for (int x = 0; x < absevents.Length; x++)
            {
                absevents[x] = new MidiEvent[mTracks[x].MidiEvents.Length];
                for (int x2 = 0, totalDeltaTime = 0; x2 < absevents[x].Length; x2++)
                {//create copies
                    absevents[x][x2] = mTracks[x].MidiEvents[x2];
                    totalDeltaTime += absevents[x][x2].DeltaTime;
                    absevents[x][x2].DeltaTime = totalDeltaTime;
                }
            }
            //sort by absolute delta time also makes sure events occur in order of track and when they are recieved. 
            int eventcount = 0;
            int delta = 0;
            int nextdelta = int.MaxValue;
            int[] counters = new int[absevents.Length];
            while (eventcount < finalTrack.MidiEvents.Length)
            {
                for (int x = 0; x < absevents.Length; x++)
                {
                    while (counters[x] < absevents[x].Length && absevents[x][counters[x]].DeltaTime == delta)
                    {
                        finalTrack.MidiEvents[eventcount] = absevents[x][counters[x]];
                        eventcount++;
                        counters[x]++;
                    }
                    if (counters[x] < absevents[x].Length && absevents[x][counters[x]].DeltaTime < nextdelta)
                        nextdelta = absevents[x][counters[x]].DeltaTime;
                }
                delta = nextdelta;
                nextdelta = int.MaxValue;
            }
            //set total time
            finalTrack.EndTime = finalTrack.MidiEvents[finalTrack.MidiEvents.Length - 1].DeltaTime;
            //put back into regular delta time
            for (int x = 0, deltadiff = 0; x < finalTrack.MidiEvents.Length; x++)
            {
                int oldtime = finalTrack.MidiEvents[x].DeltaTime;
                finalTrack.MidiEvents[x].DeltaTime -= deltadiff;
                deltadiff = oldtime;
            }
            this.mTracks = new MidiTrack[] { finalTrack };
            this.mTrackFormat = TrackFormat.SingleTrack;
        }

        private MidiTrack MergeTracks()
        {
            int eventCount = 0;
            int notesPlayed = 0;
            int activeChannels = 0;
            List<byte> programsUsed = new List<byte>();
            List<byte> drumprogramsUsed = new List<byte>();
            //Loop to get track info
            for (int x = 0; x < mTracks.Length; x++)
            {
                eventCount += mTracks[x].MidiEvents.Length;
                notesPlayed += mTracks[x].NoteOnCount;

                foreach (byte p in mTracks[x].Instruments)
                {
                    if (!programsUsed.Contains(p))
                        programsUsed.Add(p);
                }
                foreach (byte p in mTracks[x].DrumInstruments)
                {
                    if (!drumprogramsUsed.Contains(p))
                        drumprogramsUsed.Add(p);
                }
                activeChannels |= mTracks[x].ActiveChannels;
            }
            MidiTrack track = new MidiTrack(programsUsed.ToArray(), drumprogramsUsed.ToArray(), new MidiEvent[eventCount]);
            track.NoteOnCount = notesPlayed;
            track.ActiveChannels = activeChannels;
            return track;
        }
        private void LoadStream(BinaryReader reader)
        {
            if (!FindHead(reader, 500))
                throw new Exception("Invalid midi file : MThd chunk could not be found.");
            ReadHeader(reader);
            try
            {
                for (int x = 0; x < mTracks.Length; x++)
                {
                    mTracks[x] = ReadTrack(reader);
                }
            }
            catch(EndOfStreamException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + "\nWarning: the midi file may not have one or more invalid tracks.");
                byte[] emptyByte = new byte[0];
                MidiEvent[] emptyEvents = new MidiEvent[0];
                for (int x = 0; x < mTracks.Length; x++)
                {
                    if (mTracks[x] == null)
                        mTracks[x] = new MidiTrack(emptyByte, emptyByte, emptyEvents);
                }
            }
        }
        private void ReadHeader(BinaryReader reader)
        {
            if (BigEndianHelper.ReadInt32(reader) != 6) //midi header should be 6 bytes long
                throw new Exception("Midi header is invalid.");
            mTrackFormat = (TrackFormat)BigEndianHelper.ReadInt16(reader);
            mTracks = new MidiTrack[BigEndianHelper.ReadInt16(reader)];
            int div = BigEndianHelper.ReadInt16(reader);
            mDivision = div & 0x7FFF;
            mTimeFormat = ((div & 0x8000) > 0) ? TimeFormat.FamesPerSecond : TimeFormat.TicksPerBeat;
        }
        private static MidiTrack ReadTrack(BinaryReader reader)
        {
            List<byte> instList = new List<byte>();
            List<byte> drumList = new List<byte>();
            List<MidiEvent> eventList = new List<MidiEvent>();
            int channelList = 0;
            int noteOnCount = 0;
            int totalTime = 0;
            while (!new string(IOHelper.Read8BitChars(reader, 4)).Equals("MTrk", StringComparison.InvariantCultureIgnoreCase))
            {
                int length = BigEndianHelper.ReadInt32(reader);
                while (length > 0)
                {
                    length--;
                    reader.ReadByte();
                }
            }
            long endPosition = BigEndianHelper.ReadInt32(reader) + reader.BaseStream.Position;
            byte prevStatus = 0;
            while (reader.BaseStream.Position < endPosition)
            {
                int delta = ReadVariableLength(reader);
                totalTime += delta;
                byte status = reader.ReadByte();
                if (status >= 0x80 && status <= 0xEF)
                {//voice message
                    prevStatus = status;
                    eventList.Add(ReadVoiceMessage(reader, delta, status, reader.ReadByte()));
                    TrackVoiceStats(eventList[eventList.Count - 1], instList, drumList, ref channelList, ref noteOnCount);
                }
                else if (status >= 0xF0 && status <= 0xF7)
                {//system common message
                    prevStatus = 0;
                    eventList.Add(ReadSystemCommonMessage(reader, delta, status));
                }
                else if (status >= 0xF8 && status <= 0xFF)
                {//realtime message
                    eventList.Add(ReadRealTimeMessage(reader, delta, status));
                }
                else
                {//data bytes
                    if (prevStatus == 0)
                    {//if no running status continue to next status byte
                        while ((status & 0x80) != 0x80)
                            status = reader.ReadByte();
                        if (status >= 0x80 && status <= 0xEF)
                        {//voice message
                            prevStatus = status;
                            eventList.Add(ReadVoiceMessage(reader, delta, status, reader.ReadByte()));
                            TrackVoiceStats(eventList[eventList.Count - 1], instList, drumList, ref channelList, ref noteOnCount);
                        }
                        else if (status >= 0xF0 && status <= 0xF7)
                        {//system common message
                            eventList.Add(ReadSystemCommonMessage(reader, delta, status));
                        }
                        else if (status >= 0xF8 && status <= 0xFF)
                        {//realtime message
                            eventList.Add(ReadRealTimeMessage(reader, delta, status));
                        }
                    }
                    else
                    {//otherwise apply running status
                        eventList.Add(ReadVoiceMessage(reader, delta, prevStatus, status));
                        TrackVoiceStats(eventList[eventList.Count - 1], instList, drumList, ref channelList, ref noteOnCount);
                    }
                }
            }
            if (reader.BaseStream.Position != endPosition)
                throw new Exception("The track length was invalid for the current MTrk chunk.");
            if (((channelList >> MidiHelper.DrumChannel) & 1) == 1)
            {
                if(!drumList.Contains(0))
                    drumList.Add(0);
            }
            else
            {
                if(!instList.Contains(0))
                    instList.Add(0);
            }
            MidiTrack track = new MidiTrack(instList.ToArray(), drumList.ToArray(), eventList.ToArray());
            track.NoteOnCount = noteOnCount;
            track.EndTime = totalTime;
            track.ActiveChannels = channelList;
            return track;
        }
        private static MidiEvent ReadMetaMessage(BinaryReader reader, int delta, byte status)
        {
            byte metaStatus = reader.ReadByte();
            switch (metaStatus)
            {
                case 0x0://sequence number
                    {
                        int count = reader.ReadByte();
                        if (count == 0)
                            return new MetaNumberEvent(delta, status, metaStatus, -1); //current track
                        else if (count == 2)
                            return new MetaNumberEvent(delta, status, metaStatus, reader.ReadInt16());
                        else
                            throw new Exception("Invalid sequence number event.");
                    }
                case 0x1://text
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x2://copyright
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x3://trackname
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x4://inst name
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x5://lyric
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x6://marker
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x7://cuepoint
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x8://patchname
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x9://portname
                    return new MetaTextEvent(delta, status, metaStatus, ReadString(reader));
                case 0x20://midichannel
                    if (reader.ReadByte() != 1)
                        throw new Exception("Invalid midi channel event. Expected size of 1.");
                    return new MetaEvent(delta, status, metaStatus, reader.ReadByte());
                case 0x21://midiport
                    if (reader.ReadByte() != 1)
                        throw new Exception("Invalid midi port event. Expected size of 1.");
                    return new MetaEvent(delta, status, metaStatus, reader.ReadByte());
                case 0x2F://endoftrack
                    return new MetaEvent(delta, status, metaStatus, reader.ReadByte());
                case 0x51://tempo
                    if (reader.ReadByte() != 3)
                        throw new Exception("Invalid tempo event. Expected size of 3.");
                    return new MetaNumberEvent(delta, status, metaStatus, (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte());
                case 0x54://smpte
                    if (reader.ReadByte() != 5)
                        throw new Exception("Invalid smpte event. Expected size of 5.");
                    return new MetaTextEvent(delta, status, metaStatus, reader.ReadByte() + ":" + reader.ReadByte() + ":" + reader.ReadByte() + ":" + reader.ReadByte() + ":" + reader.ReadByte());
                case 0x58://time sig
                    if (reader.ReadByte() != 4)
                        throw new Exception("Invalid time signature event. Expected size of 4.");
                    return new MetaTextEvent(delta, status, metaStatus, reader.ReadByte() + ":" + reader.ReadByte() + ":" + reader.ReadByte() + ":" + reader.ReadByte());
                case 0x59://key sig
                    if (reader.ReadByte() != 2)
                        throw new Exception("Invalid key signature event. Expected size of 2.");
                    return new MetaTextEvent(delta, status, metaStatus, reader.ReadByte() + ":" + reader.ReadByte());
                case 0x7F://seq specific
                    return new MetaDataEvent(delta, status, metaStatus, reader.ReadBytes(ReadVariableLength(reader)));
            }
            throw new Exception("Not a valid meta message Status: " + status + " Meta: " + metaStatus);
        }
        private static MidiEvent ReadRealTimeMessage(BinaryReader reader, int delta, byte status)
        {
            switch (status)
            {
                case 0xF8://midi clock
                    return new RealTimeEvent(delta, status, 0, 0);
                case 0xF9://midi tick
                    return new RealTimeEvent(delta, status, 0, 0);
                case 0xFA://midi start
                    return new RealTimeEvent(delta, status, 0, 0);
                case 0xFB://midi continue
                    return new RealTimeEvent(delta, status, 0, 0);
                case 0xFC://midi stop
                    return new RealTimeEvent(delta, status, 0, 0);
                case 0xFE://active sense
                    return new RealTimeEvent(delta, status, 0, 0);
                case 0xFF://meta message
                    return ReadMetaMessage(reader, delta, status);
                default:
                    throw new Exception("The real time message was invalid or unsupported : " + status);
            }
        }
        private static MidiEvent ReadSystemCommonMessage(BinaryReader reader, int delta, byte status)
        {
            switch (status)
            {
                case 0xF7://sysEx (either or)
                case 0xF0://sysEx
                    {
                        short maker = reader.ReadByte();
                        if (maker == 0x0)
                            maker = reader.ReadInt16();
                        else if (maker == 0xF7)
                            return null;
                        List<byte> data = new List<byte>();
                        byte b = reader.ReadByte();
                        while (b != 0xF7)
                        {
                            data.Add(b);
                            b = reader.ReadByte();
                        }
                        return new SystemExclusiveEvent(delta, status, maker, data.ToArray());
                    }
                case 0xF1://mtc quarter frame
                    return new SystemCommonEvent(delta, status, reader.ReadByte(), 0);
                case 0xF2://song position
                    return new SystemCommonEvent(delta, status, reader.ReadByte(), reader.ReadByte());
                case 0xF3://song select
                    return new SystemCommonEvent(delta, status, reader.ReadByte(), 0);
                case 0xF6://tune request
                    return new SystemCommonEvent(delta, status, 0, 0);
                default:
                    throw new Exception("The system common message was invalid or unsupported : " + status);
            }
        }
        private static MidiEvent ReadVoiceMessage(BinaryReader reader, int delta, byte status, byte data1)
        {
            switch (status & 0xF0)
            {
                case 0x80: //NoteOff
                    return new MidiEvent(delta, status, data1, reader.ReadByte());
                case 0x90: //NoteOn
                    byte velocity = reader.ReadByte();
                    if (velocity == 0) //actually a note off event
                        status = (byte)((status & 0x0F) | 0x80);
                    return new MidiEvent(delta, status, data1, velocity);
                case 0xA0: //AfterTouch
                    return new MidiEvent(delta, status, data1, reader.ReadByte());
                case 0xB0: //ControlChange
                    return new MidiEvent(delta, status, data1, reader.ReadByte());
                case 0xC0: //ProgramChange
                    return new MidiEvent(delta, status, data1, 0);
                case 0xD0: //ChannelPressure
                    return new MidiEvent(delta, status, data1, 0);
                case 0xE0: //PitchWheel
                    return new MidiEvent(delta, status, data1, reader.ReadByte());
                default:
                    throw new NotSupportedException("The status provided was not that of a voice message.");
            }
        }
        private static void TrackVoiceStats(MidiEvent midiEvent, List<byte> instList, List<byte> drumList, ref int channelList, ref int noteOnCount)
        {
            if (midiEvent.Command == 0x90) //note on
            {
                channelList |= 1 << midiEvent.Channel;
                noteOnCount++;
            }
            else if (midiEvent.Command == 0xC0) //prog change
            {
                byte prog = (byte)midiEvent.Data1;
                if (midiEvent.Channel == MidiHelper.DrumChannel && !drumList.Contains(prog))
                {
                    drumList.Add(prog);
                }
                else if (!instList.Contains(prog))
                {
                    instList.Add(prog);
                }
            }
        }
        private static int ReadVariableLength(BinaryReader reader)
        {
            int value = 0;
            int next;
            do
            {
                next = reader.ReadByte();
                value = value << 7;
                value = value | (next & 0x7F);
            } while ((next & 0x80) == 0x80);
            return value;
        }
        private static string ReadString(BinaryReader reader)
        {
            int length = ReadVariableLength(reader);
            return Encoding.UTF8.GetString(reader.ReadBytes(length), 0, length);
        }     
        private static bool FindHead(BinaryReader reader, int attempts)
        {//Attempts to find the "MThd" midi header in a stream
            int match = 0;
            while (attempts > 0)
            {
                switch ((char)reader.ReadByte())
                {
                    case 'M':
                        match = 1;
                        break;
                    case 'T':
                        if (match == 1)
                            match = 2;
                        else
                            match = 0;
                        break;
                    case 'h':
                        if (match == 2)
                            match = 3;
                        else
                            match = 0;
                        break;
                    case 'd':
                        if (match == 3)
                            return true;
                        else
                            match = 0;
                        break;
                    default:
                        break;
                }
                attempts--;
            }
            return false;
        }
    }
}
