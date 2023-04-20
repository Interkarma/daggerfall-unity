#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <cstring>
#include <string.h>
#include <stdio.h>
#include <cmath>
#include <limits>
#include <assert.h>
#include <stdint.h>

#include "codegen/il2cpp-codegen.h"
#include "il2cpp-object-internals.h"

template <typename R>
struct VirtFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct VirtFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1>
struct VirtActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct VirtActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};

// DaggerfallWorkshop.AudioSynthesis.Bank.AssetManager
struct AssetManager_tE8ACD4B229C3313F7B9B93EFBC968D7C93B3F12E;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope
struct Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope/EnvelopeStage
struct EnvelopeStage_t1CA1FE3D2A3E978010D5F2947D413259D3C537B0;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope/EnvelopeStage[]
struct EnvelopeStageU5BU5D_t1A244D19890F8C934ADAE6A638D8B912186A8525;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope[]
struct EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter
struct Filter_t8869C4D2146972E0AFC8080ADBB879E449534331;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter[]
struct FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.Generator
struct Generator_tBA37A5D5C61B631CE6CF68D692817DFEB41E2476;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters
struct GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters[]
struct GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo
struct Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B;
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo[]
struct LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78;
// DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank
struct PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225;
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch
struct MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C;
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch/PatchInterval[]
struct PatchIntervalU5BU5D_tF096E636EFC61D1CB4F2B68609DB139094220647;
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch
struct Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045;
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch[]
struct PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34;
// DaggerfallWorkshop.AudioSynthesis.IResource
struct IResource_t16A264501BB8C334BC7A082E85A75D8592F3F1B9;
// DaggerfallWorkshop.AudioSynthesis.Sf2.Generator
struct Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4;
// DaggerfallWorkshop.AudioSynthesis.Sf2.Generator[]
struct GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF;
// DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator
struct Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1;
// DaggerfallWorkshop.AudioSynthesis.Sf2.ModulatorType
struct ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424;
// DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator[]
struct ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D;
// DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData
struct SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9;
// DaggerfallWorkshop.AudioSynthesis.Sf2.Zone
struct Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage[]
struct MidiMessageU5BU5D_t7B151E712267B771E6E048527C739525FEE436C7;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters
struct SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters[]
struct SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer
struct Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData[]
struct UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice
struct Voice_t481B233F7BCA5C28D192670FC7590699211A984E;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager
struct VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode
struct VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode[0...,0...]
struct VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode[]
struct VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters
struct VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92;
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice[]
struct VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF;
// DaggerfallWorkshop.AudioSynthesis.Util.Riff.Chunk
struct Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15;
// DaggerfallWorkshop.AudioSynthesis.Util.Riff.RiffTypeChunk
struct RiffTypeChunk_t0EBCC7D1B96A4601BABB5AE304EAB15D844650FD;
// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData
struct PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC;
// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData16Bit
struct PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2;
// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData24Bit
struct PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA;
// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData32Bit
struct PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7;
// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData8Bit
struct PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965;
// System.ArgumentException
struct ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1;
// System.ArgumentNullException
struct ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD;
// System.Byte[]
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Collections.Generic.Dictionary`2<System.Int32,DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch[]>
struct Dictionary_2_tB90142A9681329953F8DF0D449F402CE9005D9B4;
// System.Collections.Generic.Dictionary`2<System.String,System.Type>
struct Dictionary_2_t2DB4209C32F7303EA559E23BF9E6AEAE8F21001A;
// System.Collections.Generic.IEnumerable`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>
struct IEnumerable_1_t8C9CB82E82B0171B684BD48D391E4C8C4651C573;
// System.Collections.Generic.IEnumerable`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>
struct IEnumerable_1_t4B60E65582B38A1CF9980DFDD5F51B9DFC646FE9;
// System.Collections.Generic.IEnumerable`1<System.Object>
struct IEnumerable_1_t2F75FCBEC68AFE08982DA43985F9D04056E2BE73;
// System.Collections.Generic.LinkedListNode`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>
struct LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21;
// System.Collections.Generic.LinkedListNode`1<System.Object>
struct LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683;
// System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>
struct LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261;
// System.Collections.Generic.LinkedList`1<System.Object>
struct LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384;
// System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>
struct Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C;
// System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>
struct Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3;
// System.Collections.Generic.Stack`1<System.Object>
struct Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163;
// System.Collections.IDictionary
struct IDictionary_t1BD5C1546718A374EA8122FBD6C6EE45331E8CE7;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196;
// System.Double[]
struct DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D;
// System.Exception
struct Exception_t;
// System.IO.BinaryReader
struct BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969;
// System.IO.Stream
struct Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7;
// System.IO.Stream/ReadWriteTask
struct ReadWriteTask_tFA17EEE8BC5C4C83EAEFCC3662A30DE351ABAA80;
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
// System.IntPtr[]
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD;
// System.NotSupportedException
struct NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010;
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770;
// System.Runtime.Serialization.SerializationInfo
struct SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26;
// System.Single[]
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5;
// System.Single[][]
struct SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E;
// System.String
struct String_t;
// System.Text.Decoder
struct Decoder_tEEF45EB6F965222036C49E8EC6BA8A0692AA1F26;
// System.Threading.SemaphoreSlim
struct SemaphoreSlim_t2E2888D1C0C8FAB80823C76F1602E4434B8FA048;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;

IL2CPP_EXTERN_C RuntimeClass* ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Exception_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Filter_t8869C4D2146972E0AFC8080ADBB879E449534331_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MidiEventTypeEnum_t99E303F3D268222216E7B24459B04ACF2C462C68_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* String_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceStateEnum_t1DC114A9113887829A4FAA949B8F4E77286A95FE_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Voice_t481B233F7BCA5C28D192670FC7590699211A984E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral10559120CF9232240447CDD267B70FDF56A163F2;
IL2CPP_EXTERN_C String_t* _stringLiteral1A843118298810BCF6B34D501F9D6F73F1AA6419;
IL2CPP_EXTERN_C String_t* _stringLiteral1CD78D8DF6D14DF92778EFEB1A8E78176DE95F65;
IL2CPP_EXTERN_C String_t* _stringLiteral2BE88CA4242C76E8253AC62474851065032D6833;
IL2CPP_EXTERN_C String_t* _stringLiteral38B62BE4BDDAA5661C7D6B8E36E28159314DF5C7;
IL2CPP_EXTERN_C String_t* _stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727;
IL2CPP_EXTERN_C String_t* _stringLiteral3A59C4EE70A9BD28671F1D01D0D6D049795CEF38;
IL2CPP_EXTERN_C String_t* _stringLiteral3D3689BED112D73D2D4D8B4DB76D757B55A74DA6;
IL2CPP_EXTERN_C String_t* _stringLiteral3E940EC9BDFD3BA55B88A6DAAD594599C853C4E5;
IL2CPP_EXTERN_C String_t* _stringLiteral49D5687F55D6FC626AC427EF56FF9B8825ABAFF5;
IL2CPP_EXTERN_C String_t* _stringLiteral5459DD4D5FDD4DFD4BDA0478319570A07D1252DA;
IL2CPP_EXTERN_C String_t* _stringLiteral597C00FD0A500F1B9807E0C4B6F599EC134A4A3D;
IL2CPP_EXTERN_C String_t* _stringLiteral5DED85134BC38E3A95EA0C581D2CBEE7434808CC;
IL2CPP_EXTERN_C String_t* _stringLiteral6FD7FCDC9FA3A9FC34F67E995715072478912067;
IL2CPP_EXTERN_C String_t* _stringLiteral703288E4EE95465B5CCF9AABCA87590C977C799A;
IL2CPP_EXTERN_C String_t* _stringLiteral8901BA5CF896C7D5ED70B8D0B338F5A7E36BF7AB;
IL2CPP_EXTERN_C String_t* _stringLiteral915AE5169E08423550843F4FEF5B491F8A5D3DF3;
IL2CPP_EXTERN_C String_t* _stringLiteral983B90B80B4D17D1EA0A7A83A5F60B2BB828E383;
IL2CPP_EXTERN_C String_t* _stringLiteral9C6F29F2FE7AEA0CF8F78533B8FADA0F3637CBEB;
IL2CPP_EXTERN_C String_t* _stringLiteral9C96333422FBF942C3C3295B5104572B59793D09;
IL2CPP_EXTERN_C String_t* _stringLiteralAF1F890FB609377FDA8E938B6F8DC24DE11C19B0;
IL2CPP_EXTERN_C String_t* _stringLiteralBC174A2CD22E610D267A506DA9B812AAAF499C15;
IL2CPP_EXTERN_C String_t* _stringLiteralCA9D82DEE7812B880BE910B9E5DD0A553F05CFA9;
IL2CPP_EXTERN_C String_t* _stringLiteralCD5F8179BE6552D86CB645C304C3FA048CE32DF9;
IL2CPP_EXTERN_C String_t* _stringLiteralD055BC390DB68441E98AE955CF3ECA6B94C8B738;
IL2CPP_EXTERN_C String_t* _stringLiteralE394A332E3C6FA7617E76D91BC28B84D6A24E4C6;
IL2CPP_EXTERN_C String_t* _stringLiteralF525B595E619268F68255D45F855A35B8E3ACAD0;
IL2CPP_EXTERN_C String_t* _stringLiteralFB2B350D8A7204988C23DC3DD8A2D8FECFAF11FB;
IL2CPP_EXTERN_C String_t* _stringLiteralFE15972F3A895A26455667F55C3DF6E7B75B1254;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_mA5A2805F07E244CB7DC592F13D3FCD04AC6BE726_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_mA3F181BF1A1468DD6B9F91E3627B9BE3811B8BAF_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m4592C40C9A75A2BBD09370ADA41D658FE1CD0032_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1_AddFirst_mF0C1BDD5A5B6AE94C12282626D65C8696B3D6CC7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1_AddLast_m726CDCE67E7FFAED24B9764BA1413870450FAA03_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1_RemoveFirst_m38227EF51CB2AC5FD93DD996AFAB4B2AA0A4775F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1__ctor_m351D42809C36084085287A7DC9266C18A38EBCBD_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1__ctor_m9B300260CEDD4F71EE5EB13A29F5A1DB7326EF83_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1_get_Count_mB6578B261B94BF2CA14E35C3F412FAC44697646E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* PcmData_Create_mEDE8A9F6F9942B882C6CF78E388D1E0720DBEE4C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Queue_1_Dequeue_mC42CAF829668D9D8CFB8B95046A7450284564FCC_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Queue_1__ctor_mB4BC9CA4838CC532EBAA314118FF6116CCF76208_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Stack_1_GetEnumerator_m75E27371A4143B1C392DFB79E958DA67D10DB975_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Stack_1_Pop_mC9241F45FA4B326F497400A6638358BB20C79648_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Stack_1__ctor_m6A17077686BD0E0D90E5FF918CEDB67A998021F6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Synthesizer__ctor_mD1E1F86B5C913D1042DB6C79568DC79675CDDBFE_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t CCValue_ToString_mBFE7FE09697E670A95307955D76D85473138EA91_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t IOHelper_GetExtension_m7EFAEAE127597F499B8F093F347F90AECBC3114E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t IOHelper_Read8BitString_mE8F430DAE0D8052644ED33F18F5AFCE11D0AE9D4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MidiMessage_ToString_mDBEC05B0D9113B1A597467CCE6B8D42B79CA179C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PanComponent_ToString_mCE8FA3534B904E393E7D21CCCCEF9B0286BFA123_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PcmData_Create_mEDE8A9F6F9942B882C6CF78E388D1E0720DBEE4C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SynthHelper_CentsToPitch_m97EADE1AEF113245C1DEC9FD83B50C69F7B29E16_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SynthHelper_DBtoLinear_mD8C65E96C0148506BA89CFF914203F329EEFB47C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SynthHelper_KeyToFrequency_mBB41A7F23826AE45F4397D85A2A8E36A097528A1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_FillWorkingBuffer_mA17FA0EBD99D7F5C5C236297A6DFC8460B437957_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_LoadBank_m806559636928C4F28448F3F8ADF0E12E6CEC7E4E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_NoteOn_mED47CC4A3C3105E86AF9A299B0824D5425499C3F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_ReleaseAllHoldPedals_m09DA747E7D9AD591C391C8D51D0FF01D04F2ECB7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer_ReleaseHoldPedal_mE43D3DCBF5F4728BEC6E8F778D333DFA080E105C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer__cctor_m8AD50A2E6E8C01C129497B28BF21A9B0410751AE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Synthesizer__ctor_mD1E1F86B5C913D1042DB6C79568DC79675CDDBFE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_CreateCentTable_m372317857C669E4D43EBBD028DB2E62BCC6CC50F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_CreateConcaveTable_m5544B299F299F355FB58B93619127874CCDB06CA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_CreateConvexTable_m8CEBC09758BE247C3EBE23F03D1C6A6D39AA520B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_CreateLinearTable_m62A5E3D5CE0F0D8F9F496A261468BE64E7B8FE40_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_CreateSemitoneTable_mA5A66F789BEC9D3168CC8AE2BA805F8309441AC8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_CreateSustainTable_mCC57225D8466B310B618EB3E4E3303F685BFC627_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tables__cctor_m0C56E0A94FA2CA0564F609F52960527ABB235CC0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_AddToRegistry_m7B92602F3DD706215F8B2549A2E206E46D9E213C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_ClearRegistry_m3E58BBD848C9B941AC9C3B038A5E3DBAAD552C34_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_GetFreeVoice_mFCEE90FF0C773A45F0DE6AF229D2F7AA1B88639E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_RemoveFromRegistry_mF4C348706F109014BC7594B960D24307B9539718_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_StealOldest_mA7E0A19BA1AA22AAA607A495B58E0BB26801F57D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_StealQuietestVoice_m7D807BDC795448730B4C6BF94C2D02A424559783_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager_UnloadPatches_m0B094424DF30EDE3C004B6EDFB7C97A60F9C32CB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceManager__ctor_mE544FA76F2141F9F6BE6FD198686AD516D74378D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceParameters_ToString_mE40D2988FEC43C9EC933FDF4E52594EE494E3246_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VoiceParameters__ctor_m0F6140BF4B6EAE4D1A03A011E048C662E8DBA418_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Voice_ToString_m251F2DC14D1438A36A586C71FA2DDD453CB2DD04_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Voice__ctor_m11F21BF3B861E923059674538CCAB852CE7396DF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WaveHelper_GetChannelPcmData_m399AD7F1A9592B079B810D4A57ACCCBE06657581_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t WaveHelper_SwapEndianess_mB4C9A2AC9A10F41C2F9BDC11EAFD474EF1386845_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Zone_ToString_m90F7349A5B098A6571FBFEC46F5B0491993CC464_MetadataUsageId;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E;
struct FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E;
struct GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8;
struct LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78;
struct PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34;
struct GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF;
struct ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D;
struct SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0;
struct UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD;
struct VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6;
struct VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC;
struct VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF;
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821;
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
struct DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D;
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5;
struct SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// System.Object


// DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank
struct PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225  : public RuntimeObject
{
public:
	// System.Collections.Generic.Dictionary`2<System.Int32,DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch[]> DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::bank
	Dictionary_2_tB90142A9681329953F8DF0D449F402CE9005D9B4 * ___bank_4;
	// DaggerfallWorkshop.AudioSynthesis.Bank.AssetManager DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::assets
	AssetManager_tE8ACD4B229C3313F7B9B93EFBC968D7C93B3F12E * ___assets_5;
	// System.String DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::bankName
	String_t* ___bankName_6;
	// System.String DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::comment
	String_t* ___comment_7;

public:
	inline static int32_t get_offset_of_bank_4() { return static_cast<int32_t>(offsetof(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225, ___bank_4)); }
	inline Dictionary_2_tB90142A9681329953F8DF0D449F402CE9005D9B4 * get_bank_4() const { return ___bank_4; }
	inline Dictionary_2_tB90142A9681329953F8DF0D449F402CE9005D9B4 ** get_address_of_bank_4() { return &___bank_4; }
	inline void set_bank_4(Dictionary_2_tB90142A9681329953F8DF0D449F402CE9005D9B4 * value)
	{
		___bank_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___bank_4), (void*)value);
	}

	inline static int32_t get_offset_of_assets_5() { return static_cast<int32_t>(offsetof(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225, ___assets_5)); }
	inline AssetManager_tE8ACD4B229C3313F7B9B93EFBC968D7C93B3F12E * get_assets_5() const { return ___assets_5; }
	inline AssetManager_tE8ACD4B229C3313F7B9B93EFBC968D7C93B3F12E ** get_address_of_assets_5() { return &___assets_5; }
	inline void set_assets_5(AssetManager_tE8ACD4B229C3313F7B9B93EFBC968D7C93B3F12E * value)
	{
		___assets_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___assets_5), (void*)value);
	}

	inline static int32_t get_offset_of_bankName_6() { return static_cast<int32_t>(offsetof(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225, ___bankName_6)); }
	inline String_t* get_bankName_6() const { return ___bankName_6; }
	inline String_t** get_address_of_bankName_6() { return &___bankName_6; }
	inline void set_bankName_6(String_t* value)
	{
		___bankName_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___bankName_6), (void*)value);
	}

	inline static int32_t get_offset_of_comment_7() { return static_cast<int32_t>(offsetof(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225, ___comment_7)); }
	inline String_t* get_comment_7() const { return ___comment_7; }
	inline String_t** get_address_of_comment_7() { return &___comment_7; }
	inline void set_comment_7(String_t* value)
	{
		___comment_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comment_7), (void*)value);
	}
};

struct PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225_StaticFields
{
public:
	// System.Collections.Generic.Dictionary`2<System.String,System.Type> DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::patchTypes
	Dictionary_2_t2DB4209C32F7303EA559E23BF9E6AEAE8F21001A * ___patchTypes_3;

public:
	inline static int32_t get_offset_of_patchTypes_3() { return static_cast<int32_t>(offsetof(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225_StaticFields, ___patchTypes_3)); }
	inline Dictionary_2_t2DB4209C32F7303EA559E23BF9E6AEAE8F21001A * get_patchTypes_3() const { return ___patchTypes_3; }
	inline Dictionary_2_t2DB4209C32F7303EA559E23BF9E6AEAE8F21001A ** get_address_of_patchTypes_3() { return &___patchTypes_3; }
	inline void set_patchTypes_3(Dictionary_2_t2DB4209C32F7303EA559E23BF9E6AEAE8F21001A * value)
	{
		___patchTypes_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___patchTypes_3), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch
struct Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045  : public RuntimeObject
{
public:
	// System.String DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::patchName
	String_t* ___patchName_0;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::exTarget
	int32_t ___exTarget_1;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::exGroup
	int32_t ___exGroup_2;

public:
	inline static int32_t get_offset_of_patchName_0() { return static_cast<int32_t>(offsetof(Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045, ___patchName_0)); }
	inline String_t* get_patchName_0() const { return ___patchName_0; }
	inline String_t** get_address_of_patchName_0() { return &___patchName_0; }
	inline void set_patchName_0(String_t* value)
	{
		___patchName_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___patchName_0), (void*)value);
	}

	inline static int32_t get_offset_of_exTarget_1() { return static_cast<int32_t>(offsetof(Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045, ___exTarget_1)); }
	inline int32_t get_exTarget_1() const { return ___exTarget_1; }
	inline int32_t* get_address_of_exTarget_1() { return &___exTarget_1; }
	inline void set_exTarget_1(int32_t value)
	{
		___exTarget_1 = value;
	}

	inline static int32_t get_offset_of_exGroup_2() { return static_cast<int32_t>(offsetof(Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045, ___exGroup_2)); }
	inline int32_t get_exGroup_2() const { return ___exGroup_2; }
	inline int32_t* get_address_of_exGroup_2() { return &___exGroup_2; }
	inline void set_exGroup_2(int32_t value)
	{
		___exGroup_2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData
struct SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9  : public RuntimeObject
{
public:
	// System.Byte[] DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData::samples
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___samples_0;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData::bitsPerSample
	int32_t ___bitsPerSample_1;

public:
	inline static int32_t get_offset_of_samples_0() { return static_cast<int32_t>(offsetof(SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9, ___samples_0)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_samples_0() const { return ___samples_0; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_samples_0() { return &___samples_0; }
	inline void set_samples_0(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___samples_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___samples_0), (void*)value);
	}

	inline static int32_t get_offset_of_bitsPerSample_1() { return static_cast<int32_t>(offsetof(SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9, ___bitsPerSample_1)); }
	inline int32_t get_bitsPerSample_1() const { return ___bitsPerSample_1; }
	inline int32_t* get_address_of_bitsPerSample_1() { return &___bitsPerSample_1; }
	inline void set_bitsPerSample_1(int32_t value)
	{
		___bitsPerSample_1 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.Zone
struct Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator[] DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::modulators
	ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* ___modulators_0;
	// DaggerfallWorkshop.AudioSynthesis.Sf2.Generator[] DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::generators
	GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* ___generators_1;

public:
	inline static int32_t get_offset_of_modulators_0() { return static_cast<int32_t>(offsetof(Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D, ___modulators_0)); }
	inline ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* get_modulators_0() const { return ___modulators_0; }
	inline ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D** get_address_of_modulators_0() { return &___modulators_0; }
	inline void set_modulators_0(ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* value)
	{
		___modulators_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___modulators_0), (void*)value);
	}

	inline static int32_t get_offset_of_generators_1() { return static_cast<int32_t>(offsetof(Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D, ___generators_1)); }
	inline GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* get_generators_1() const { return ___generators_1; }
	inline GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF** get_address_of_generators_1() { return &___generators_1; }
	inline void set_generators_1(GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* value)
	{
		___generators_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___generators_1), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper
struct SynthHelper_tB4AFC2B7EA622F8BC49463077252BE364E9BB415  : public RuntimeObject
{
public:

public:
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice
struct Voice_t481B233F7BCA5C28D192670FC7590699211A984E  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::patch
	Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * ___patch_0;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::voiceparams
	VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * ___voiceparams_1;

public:
	inline static int32_t get_offset_of_patch_0() { return static_cast<int32_t>(offsetof(Voice_t481B233F7BCA5C28D192670FC7590699211A984E, ___patch_0)); }
	inline Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * get_patch_0() const { return ___patch_0; }
	inline Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 ** get_address_of_patch_0() { return &___patch_0; }
	inline void set_patch_0(Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * value)
	{
		___patch_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___patch_0), (void*)value);
	}

	inline static int32_t get_offset_of_voiceparams_1() { return static_cast<int32_t>(offsetof(Voice_t481B233F7BCA5C28D192670FC7590699211A984E, ___voiceparams_1)); }
	inline VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * get_voiceparams_1() const { return ___voiceparams_1; }
	inline VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 ** get_address_of_voiceparams_1() { return &___voiceparams_1; }
	inline void set_voiceparams_1(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * value)
	{
		___voiceparams_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___voiceparams_1), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode
struct VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode::Value
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___Value_0;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode::Next
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * ___Next_1;

public:
	inline static int32_t get_offset_of_Value_0() { return static_cast<int32_t>(offsetof(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B, ___Value_0)); }
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E * get_Value_0() const { return ___Value_0; }
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E ** get_address_of_Value_0() { return &___Value_0; }
	inline void set_Value_0(Voice_t481B233F7BCA5C28D192670FC7590699211A984E * value)
	{
		___Value_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Value_0), (void*)value);
	}

	inline static int32_t get_offset_of_Next_1() { return static_cast<int32_t>(offsetof(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B, ___Next_1)); }
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * get_Next_1() const { return ___Next_1; }
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** get_address_of_Next_1() { return &___Next_1; }
	inline void set_Next_1(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		___Next_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Next_1), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Util.BigEndianHelper
struct BigEndianHelper_t9B936FEFECD96EDC136AFA0D3AB7B020D26FACA0  : public RuntimeObject
{
public:

public:
};


// DaggerfallWorkshop.AudioSynthesis.Util.IOHelper
struct IOHelper_t26802146634CE5A22B54B960C6ABEC1736D66F81  : public RuntimeObject
{
public:

public:
};


// DaggerfallWorkshop.AudioSynthesis.Util.Riff.Chunk
struct Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15  : public RuntimeObject
{
public:
	// System.String DaggerfallWorkshop.AudioSynthesis.Util.Riff.Chunk::id
	String_t* ___id_0;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Util.Riff.Chunk::size
	int32_t ___size_1;

public:
	inline static int32_t get_offset_of_id_0() { return static_cast<int32_t>(offsetof(Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15, ___id_0)); }
	inline String_t* get_id_0() const { return ___id_0; }
	inline String_t** get_address_of_id_0() { return &___id_0; }
	inline void set_id_0(String_t* value)
	{
		___id_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___id_0), (void*)value);
	}

	inline static int32_t get_offset_of_size_1() { return static_cast<int32_t>(offsetof(Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15, ___size_1)); }
	inline int32_t get_size_1() const { return ___size_1; }
	inline int32_t* get_address_of_size_1() { return &___size_1; }
	inline void set_size_1(int32_t value)
	{
		___size_1 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Util.Tables
struct Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7  : public RuntimeObject
{
public:

public:
};

struct Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields
{
public:
	// System.Single[][] DaggerfallWorkshop.AudioSynthesis.Util.Tables::EnvelopeTables
	SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* ___EnvelopeTables_0;
	// System.Double[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::SemitoneTable
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* ___SemitoneTable_1;
	// System.Double[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CentTable
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* ___CentTable_2;

public:
	inline static int32_t get_offset_of_EnvelopeTables_0() { return static_cast<int32_t>(offsetof(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields, ___EnvelopeTables_0)); }
	inline SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* get_EnvelopeTables_0() const { return ___EnvelopeTables_0; }
	inline SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E** get_address_of_EnvelopeTables_0() { return &___EnvelopeTables_0; }
	inline void set_EnvelopeTables_0(SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* value)
	{
		___EnvelopeTables_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___EnvelopeTables_0), (void*)value);
	}

	inline static int32_t get_offset_of_SemitoneTable_1() { return static_cast<int32_t>(offsetof(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields, ___SemitoneTable_1)); }
	inline DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* get_SemitoneTable_1() const { return ___SemitoneTable_1; }
	inline DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D** get_address_of_SemitoneTable_1() { return &___SemitoneTable_1; }
	inline void set_SemitoneTable_1(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* value)
	{
		___SemitoneTable_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___SemitoneTable_1), (void*)value);
	}

	inline static int32_t get_offset_of_CentTable_2() { return static_cast<int32_t>(offsetof(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields, ___CentTable_2)); }
	inline DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* get_CentTable_2() const { return ___CentTable_2; }
	inline DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D** get_address_of_CentTable_2() { return &___CentTable_2; }
	inline void set_CentTable_2(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* value)
	{
		___CentTable_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___CentTable_2), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData
struct PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC  : public RuntimeObject
{
public:
	// System.Byte[] DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::data
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___data_0;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::bytes
	uint8_t ___bytes_1;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::length
	int32_t ___length_2;

public:
	inline static int32_t get_offset_of_data_0() { return static_cast<int32_t>(offsetof(PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC, ___data_0)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_data_0() const { return ___data_0; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_data_0() { return &___data_0; }
	inline void set_data_0(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___data_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___data_0), (void*)value);
	}

	inline static int32_t get_offset_of_bytes_1() { return static_cast<int32_t>(offsetof(PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC, ___bytes_1)); }
	inline uint8_t get_bytes_1() const { return ___bytes_1; }
	inline uint8_t* get_address_of_bytes_1() { return &___bytes_1; }
	inline void set_bytes_1(uint8_t value)
	{
		___bytes_1 = value;
	}

	inline static int32_t get_offset_of_length_2() { return static_cast<int32_t>(offsetof(PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC, ___length_2)); }
	inline int32_t get_length_2() const { return ___length_2; }
	inline int32_t* get_address_of_length_2() { return &___length_2; }
	inline void set_length_2(int32_t value)
	{
		___length_2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Wave.WaveHelper
struct WaveHelper_tC0C41BF93E55E6A3C8BB13FBE5CD30582B94D52A  : public RuntimeObject
{
public:

public:
};

struct Il2CppArrayBounds;

// System.Array


// System.BitConverter
struct BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE  : public RuntimeObject
{
public:

public:
};

struct BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_StaticFields
{
public:
	// System.Boolean System.BitConverter::IsLittleEndian
	bool ___IsLittleEndian_0;

public:
	inline static int32_t get_offset_of_IsLittleEndian_0() { return static_cast<int32_t>(offsetof(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_StaticFields, ___IsLittleEndian_0)); }
	inline bool get_IsLittleEndian_0() const { return ___IsLittleEndian_0; }
	inline bool* get_address_of_IsLittleEndian_0() { return &___IsLittleEndian_0; }
	inline void set_IsLittleEndian_0(bool value)
	{
		___IsLittleEndian_0 = value;
	}
};


// System.Collections.Generic.LinkedListNode`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>
struct LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21  : public RuntimeObject
{
public:
	// System.Collections.Generic.LinkedList`1<T> System.Collections.Generic.LinkedListNode`1::list
	LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * ___list_0;
	// System.Collections.Generic.LinkedListNode`1<T> System.Collections.Generic.LinkedListNode`1::next
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * ___next_1;
	// System.Collections.Generic.LinkedListNode`1<T> System.Collections.Generic.LinkedListNode`1::prev
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * ___prev_2;
	// T System.Collections.Generic.LinkedListNode`1::item
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___item_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21, ___list_0)); }
	inline LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * get_list_0() const { return ___list_0; }
	inline LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_next_1() { return static_cast<int32_t>(offsetof(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21, ___next_1)); }
	inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * get_next_1() const { return ___next_1; }
	inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 ** get_address_of_next_1() { return &___next_1; }
	inline void set_next_1(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * value)
	{
		___next_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___next_1), (void*)value);
	}

	inline static int32_t get_offset_of_prev_2() { return static_cast<int32_t>(offsetof(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21, ___prev_2)); }
	inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * get_prev_2() const { return ___prev_2; }
	inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 ** get_address_of_prev_2() { return &___prev_2; }
	inline void set_prev_2(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * value)
	{
		___prev_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___prev_2), (void*)value);
	}

	inline static int32_t get_offset_of_item_3() { return static_cast<int32_t>(offsetof(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21, ___item_3)); }
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E * get_item_3() const { return ___item_3; }
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E ** get_address_of_item_3() { return &___item_3; }
	inline void set_item_3(Voice_t481B233F7BCA5C28D192670FC7590699211A984E * value)
	{
		___item_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___item_3), (void*)value);
	}
};


// System.Collections.Generic.LinkedListNode`1<System.Object>
struct LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683  : public RuntimeObject
{
public:
	// System.Collections.Generic.LinkedList`1<T> System.Collections.Generic.LinkedListNode`1::list
	LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * ___list_0;
	// System.Collections.Generic.LinkedListNode`1<T> System.Collections.Generic.LinkedListNode`1::next
	LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * ___next_1;
	// System.Collections.Generic.LinkedListNode`1<T> System.Collections.Generic.LinkedListNode`1::prev
	LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * ___prev_2;
	// T System.Collections.Generic.LinkedListNode`1::item
	RuntimeObject * ___item_3;

public:
	inline static int32_t get_offset_of_list_0() { return static_cast<int32_t>(offsetof(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683, ___list_0)); }
	inline LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * get_list_0() const { return ___list_0; }
	inline LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 ** get_address_of_list_0() { return &___list_0; }
	inline void set_list_0(LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * value)
	{
		___list_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___list_0), (void*)value);
	}

	inline static int32_t get_offset_of_next_1() { return static_cast<int32_t>(offsetof(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683, ___next_1)); }
	inline LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * get_next_1() const { return ___next_1; }
	inline LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 ** get_address_of_next_1() { return &___next_1; }
	inline void set_next_1(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * value)
	{
		___next_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___next_1), (void*)value);
	}

	inline static int32_t get_offset_of_prev_2() { return static_cast<int32_t>(offsetof(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683, ___prev_2)); }
	inline LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * get_prev_2() const { return ___prev_2; }
	inline LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 ** get_address_of_prev_2() { return &___prev_2; }
	inline void set_prev_2(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * value)
	{
		___prev_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___prev_2), (void*)value);
	}

	inline static int32_t get_offset_of_item_3() { return static_cast<int32_t>(offsetof(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683, ___item_3)); }
	inline RuntimeObject * get_item_3() const { return ___item_3; }
	inline RuntimeObject ** get_address_of_item_3() { return &___item_3; }
	inline void set_item_3(RuntimeObject * value)
	{
		___item_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___item_3), (void*)value);
	}
};


// System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>
struct LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261  : public RuntimeObject
{
public:
	// System.Collections.Generic.LinkedListNode`1<T> System.Collections.Generic.LinkedList`1::head
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * ___head_0;
	// System.Int32 System.Collections.Generic.LinkedList`1::count
	int32_t ___count_1;
	// System.Int32 System.Collections.Generic.LinkedList`1::version
	int32_t ___version_2;
	// System.Object System.Collections.Generic.LinkedList`1::_syncRoot
	RuntimeObject * ____syncRoot_3;
	// System.Runtime.Serialization.SerializationInfo System.Collections.Generic.LinkedList`1::_siInfo
	SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * ____siInfo_4;

public:
	inline static int32_t get_offset_of_head_0() { return static_cast<int32_t>(offsetof(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261, ___head_0)); }
	inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * get_head_0() const { return ___head_0; }
	inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 ** get_address_of_head_0() { return &___head_0; }
	inline void set_head_0(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * value)
	{
		___head_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___head_0), (void*)value);
	}

	inline static int32_t get_offset_of_count_1() { return static_cast<int32_t>(offsetof(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261, ___count_1)); }
	inline int32_t get_count_1() const { return ___count_1; }
	inline int32_t* get_address_of_count_1() { return &___count_1; }
	inline void set_count_1(int32_t value)
	{
		___count_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of__syncRoot_3() { return static_cast<int32_t>(offsetof(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261, ____syncRoot_3)); }
	inline RuntimeObject * get__syncRoot_3() const { return ____syncRoot_3; }
	inline RuntimeObject ** get_address_of__syncRoot_3() { return &____syncRoot_3; }
	inline void set__syncRoot_3(RuntimeObject * value)
	{
		____syncRoot_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_3), (void*)value);
	}

	inline static int32_t get_offset_of__siInfo_4() { return static_cast<int32_t>(offsetof(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261, ____siInfo_4)); }
	inline SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * get__siInfo_4() const { return ____siInfo_4; }
	inline SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 ** get_address_of__siInfo_4() { return &____siInfo_4; }
	inline void set__siInfo_4(SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * value)
	{
		____siInfo_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____siInfo_4), (void*)value);
	}
};


// System.Collections.Generic.LinkedList`1<System.Object>
struct LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384  : public RuntimeObject
{
public:
	// System.Collections.Generic.LinkedListNode`1<T> System.Collections.Generic.LinkedList`1::head
	LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * ___head_0;
	// System.Int32 System.Collections.Generic.LinkedList`1::count
	int32_t ___count_1;
	// System.Int32 System.Collections.Generic.LinkedList`1::version
	int32_t ___version_2;
	// System.Object System.Collections.Generic.LinkedList`1::_syncRoot
	RuntimeObject * ____syncRoot_3;
	// System.Runtime.Serialization.SerializationInfo System.Collections.Generic.LinkedList`1::_siInfo
	SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * ____siInfo_4;

public:
	inline static int32_t get_offset_of_head_0() { return static_cast<int32_t>(offsetof(LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384, ___head_0)); }
	inline LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * get_head_0() const { return ___head_0; }
	inline LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 ** get_address_of_head_0() { return &___head_0; }
	inline void set_head_0(LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * value)
	{
		___head_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___head_0), (void*)value);
	}

	inline static int32_t get_offset_of_count_1() { return static_cast<int32_t>(offsetof(LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384, ___count_1)); }
	inline int32_t get_count_1() const { return ___count_1; }
	inline int32_t* get_address_of_count_1() { return &___count_1; }
	inline void set_count_1(int32_t value)
	{
		___count_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of__syncRoot_3() { return static_cast<int32_t>(offsetof(LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384, ____syncRoot_3)); }
	inline RuntimeObject * get__syncRoot_3() const { return ____syncRoot_3; }
	inline RuntimeObject ** get_address_of__syncRoot_3() { return &____syncRoot_3; }
	inline void set__syncRoot_3(RuntimeObject * value)
	{
		____syncRoot_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_3), (void*)value);
	}

	inline static int32_t get_offset_of__siInfo_4() { return static_cast<int32_t>(offsetof(LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384, ____siInfo_4)); }
	inline SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * get__siInfo_4() const { return ____siInfo_4; }
	inline SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 ** get_address_of__siInfo_4() { return &____siInfo_4; }
	inline void set__siInfo_4(SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * value)
	{
		____siInfo_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____siInfo_4), (void*)value);
	}
};


// System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>
struct Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.Queue`1::_array
	MidiMessageU5BU5D_t7B151E712267B771E6E048527C739525FEE436C7* ____array_0;
	// System.Int32 System.Collections.Generic.Queue`1::_head
	int32_t ____head_1;
	// System.Int32 System.Collections.Generic.Queue`1::_tail
	int32_t ____tail_2;
	// System.Int32 System.Collections.Generic.Queue`1::_size
	int32_t ____size_3;
	// System.Int32 System.Collections.Generic.Queue`1::_version
	int32_t ____version_4;
	// System.Object System.Collections.Generic.Queue`1::_syncRoot
	RuntimeObject * ____syncRoot_5;

public:
	inline static int32_t get_offset_of__array_0() { return static_cast<int32_t>(offsetof(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C, ____array_0)); }
	inline MidiMessageU5BU5D_t7B151E712267B771E6E048527C739525FEE436C7* get__array_0() const { return ____array_0; }
	inline MidiMessageU5BU5D_t7B151E712267B771E6E048527C739525FEE436C7** get_address_of__array_0() { return &____array_0; }
	inline void set__array_0(MidiMessageU5BU5D_t7B151E712267B771E6E048527C739525FEE436C7* value)
	{
		____array_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____array_0), (void*)value);
	}

	inline static int32_t get_offset_of__head_1() { return static_cast<int32_t>(offsetof(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C, ____head_1)); }
	inline int32_t get__head_1() const { return ____head_1; }
	inline int32_t* get_address_of__head_1() { return &____head_1; }
	inline void set__head_1(int32_t value)
	{
		____head_1 = value;
	}

	inline static int32_t get_offset_of__tail_2() { return static_cast<int32_t>(offsetof(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C, ____tail_2)); }
	inline int32_t get__tail_2() const { return ____tail_2; }
	inline int32_t* get_address_of__tail_2() { return &____tail_2; }
	inline void set__tail_2(int32_t value)
	{
		____tail_2 = value;
	}

	inline static int32_t get_offset_of__size_3() { return static_cast<int32_t>(offsetof(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C, ____size_3)); }
	inline int32_t get__size_3() const { return ____size_3; }
	inline int32_t* get_address_of__size_3() { return &____size_3; }
	inline void set__size_3(int32_t value)
	{
		____size_3 = value;
	}

	inline static int32_t get_offset_of__version_4() { return static_cast<int32_t>(offsetof(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C, ____version_4)); }
	inline int32_t get__version_4() const { return ____version_4; }
	inline int32_t* get_address_of__version_4() { return &____version_4; }
	inline void set__version_4(int32_t value)
	{
		____version_4 = value;
	}

	inline static int32_t get_offset_of__syncRoot_5() { return static_cast<int32_t>(offsetof(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C, ____syncRoot_5)); }
	inline RuntimeObject * get__syncRoot_5() const { return ____syncRoot_5; }
	inline RuntimeObject ** get_address_of__syncRoot_5() { return &____syncRoot_5; }
	inline void set__syncRoot_5(RuntimeObject * value)
	{
		____syncRoot_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_5), (void*)value);
	}
};


// System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>
struct Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.Stack`1::_array
	VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* ____array_0;
	// System.Int32 System.Collections.Generic.Stack`1::_size
	int32_t ____size_1;
	// System.Int32 System.Collections.Generic.Stack`1::_version
	int32_t ____version_2;
	// System.Object System.Collections.Generic.Stack`1::_syncRoot
	RuntimeObject * ____syncRoot_3;

public:
	inline static int32_t get_offset_of__array_0() { return static_cast<int32_t>(offsetof(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3, ____array_0)); }
	inline VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* get__array_0() const { return ____array_0; }
	inline VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC** get_address_of__array_0() { return &____array_0; }
	inline void set__array_0(VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* value)
	{
		____array_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____array_0), (void*)value);
	}

	inline static int32_t get_offset_of__size_1() { return static_cast<int32_t>(offsetof(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3, ____size_1)); }
	inline int32_t get__size_1() const { return ____size_1; }
	inline int32_t* get_address_of__size_1() { return &____size_1; }
	inline void set__size_1(int32_t value)
	{
		____size_1 = value;
	}

	inline static int32_t get_offset_of__version_2() { return static_cast<int32_t>(offsetof(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3, ____version_2)); }
	inline int32_t get__version_2() const { return ____version_2; }
	inline int32_t* get_address_of__version_2() { return &____version_2; }
	inline void set__version_2(int32_t value)
	{
		____version_2 = value;
	}

	inline static int32_t get_offset_of__syncRoot_3() { return static_cast<int32_t>(offsetof(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3, ____syncRoot_3)); }
	inline RuntimeObject * get__syncRoot_3() const { return ____syncRoot_3; }
	inline RuntimeObject ** get_address_of__syncRoot_3() { return &____syncRoot_3; }
	inline void set__syncRoot_3(RuntimeObject * value)
	{
		____syncRoot_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_3), (void*)value);
	}
};


// System.IO.BinaryReader
struct BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969  : public RuntimeObject
{
public:
	// System.IO.Stream System.IO.BinaryReader::m_stream
	Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * ___m_stream_0;
	// System.Byte[] System.IO.BinaryReader::m_buffer
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___m_buffer_1;
	// System.Text.Decoder System.IO.BinaryReader::m_decoder
	Decoder_tEEF45EB6F965222036C49E8EC6BA8A0692AA1F26 * ___m_decoder_2;
	// System.Byte[] System.IO.BinaryReader::m_charBytes
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___m_charBytes_3;
	// System.Char[] System.IO.BinaryReader::m_singleChar
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___m_singleChar_4;
	// System.Char[] System.IO.BinaryReader::m_charBuffer
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___m_charBuffer_5;
	// System.Int32 System.IO.BinaryReader::m_maxCharsSize
	int32_t ___m_maxCharsSize_6;
	// System.Boolean System.IO.BinaryReader::m_2BytesPerChar
	bool ___m_2BytesPerChar_7;
	// System.Boolean System.IO.BinaryReader::m_isMemoryStream
	bool ___m_isMemoryStream_8;
	// System.Boolean System.IO.BinaryReader::m_leaveOpen
	bool ___m_leaveOpen_9;

public:
	inline static int32_t get_offset_of_m_stream_0() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_stream_0)); }
	inline Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * get_m_stream_0() const { return ___m_stream_0; }
	inline Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 ** get_address_of_m_stream_0() { return &___m_stream_0; }
	inline void set_m_stream_0(Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * value)
	{
		___m_stream_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_stream_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_buffer_1() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_buffer_1)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_m_buffer_1() const { return ___m_buffer_1; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_m_buffer_1() { return &___m_buffer_1; }
	inline void set_m_buffer_1(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___m_buffer_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_buffer_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_decoder_2() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_decoder_2)); }
	inline Decoder_tEEF45EB6F965222036C49E8EC6BA8A0692AA1F26 * get_m_decoder_2() const { return ___m_decoder_2; }
	inline Decoder_tEEF45EB6F965222036C49E8EC6BA8A0692AA1F26 ** get_address_of_m_decoder_2() { return &___m_decoder_2; }
	inline void set_m_decoder_2(Decoder_tEEF45EB6F965222036C49E8EC6BA8A0692AA1F26 * value)
	{
		___m_decoder_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_decoder_2), (void*)value);
	}

	inline static int32_t get_offset_of_m_charBytes_3() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_charBytes_3)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_m_charBytes_3() const { return ___m_charBytes_3; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_m_charBytes_3() { return &___m_charBytes_3; }
	inline void set_m_charBytes_3(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___m_charBytes_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_charBytes_3), (void*)value);
	}

	inline static int32_t get_offset_of_m_singleChar_4() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_singleChar_4)); }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* get_m_singleChar_4() const { return ___m_singleChar_4; }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2** get_address_of_m_singleChar_4() { return &___m_singleChar_4; }
	inline void set_m_singleChar_4(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* value)
	{
		___m_singleChar_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_singleChar_4), (void*)value);
	}

	inline static int32_t get_offset_of_m_charBuffer_5() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_charBuffer_5)); }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* get_m_charBuffer_5() const { return ___m_charBuffer_5; }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2** get_address_of_m_charBuffer_5() { return &___m_charBuffer_5; }
	inline void set_m_charBuffer_5(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* value)
	{
		___m_charBuffer_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_charBuffer_5), (void*)value);
	}

	inline static int32_t get_offset_of_m_maxCharsSize_6() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_maxCharsSize_6)); }
	inline int32_t get_m_maxCharsSize_6() const { return ___m_maxCharsSize_6; }
	inline int32_t* get_address_of_m_maxCharsSize_6() { return &___m_maxCharsSize_6; }
	inline void set_m_maxCharsSize_6(int32_t value)
	{
		___m_maxCharsSize_6 = value;
	}

	inline static int32_t get_offset_of_m_2BytesPerChar_7() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_2BytesPerChar_7)); }
	inline bool get_m_2BytesPerChar_7() const { return ___m_2BytesPerChar_7; }
	inline bool* get_address_of_m_2BytesPerChar_7() { return &___m_2BytesPerChar_7; }
	inline void set_m_2BytesPerChar_7(bool value)
	{
		___m_2BytesPerChar_7 = value;
	}

	inline static int32_t get_offset_of_m_isMemoryStream_8() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_isMemoryStream_8)); }
	inline bool get_m_isMemoryStream_8() const { return ___m_isMemoryStream_8; }
	inline bool* get_address_of_m_isMemoryStream_8() { return &___m_isMemoryStream_8; }
	inline void set_m_isMemoryStream_8(bool value)
	{
		___m_isMemoryStream_8 = value;
	}

	inline static int32_t get_offset_of_m_leaveOpen_9() { return static_cast<int32_t>(offsetof(BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969, ___m_leaveOpen_9)); }
	inline bool get_m_leaveOpen_9() const { return ___m_leaveOpen_9; }
	inline bool* get_address_of_m_leaveOpen_9() { return &___m_leaveOpen_9; }
	inline void set_m_leaveOpen_9(bool value)
	{
		___m_leaveOpen_9 = value;
	}
};


// System.MarshalByRefObject
struct MarshalByRefObject_tC4577953D0A44D0AB8597CFA868E01C858B1C9AF  : public RuntimeObject
{
public:
	// System.Object System.MarshalByRefObject::_identity
	RuntimeObject * ____identity_0;

public:
	inline static int32_t get_offset_of__identity_0() { return static_cast<int32_t>(offsetof(MarshalByRefObject_tC4577953D0A44D0AB8597CFA868E01C858B1C9AF, ____identity_0)); }
	inline RuntimeObject * get__identity_0() const { return ____identity_0; }
	inline RuntimeObject ** get_address_of__identity_0() { return &____identity_0; }
	inline void set__identity_0(RuntimeObject * value)
	{
		____identity_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____identity_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.MarshalByRefObject
struct MarshalByRefObject_tC4577953D0A44D0AB8597CFA868E01C858B1C9AF_marshaled_pinvoke
{
	Il2CppIUnknown* ____identity_0;
};
// Native definition for COM marshalling of System.MarshalByRefObject
struct MarshalByRefObject_tC4577953D0A44D0AB8597CFA868E01C858B1C9AF_marshaled_com
{
	Il2CppIUnknown* ____identity_0;
};

// System.String
struct String_t  : public RuntimeObject
{
public:
	// System.Int32 System.String::m_stringLength
	int32_t ___m_stringLength_0;
	// System.Char System.String::m_firstChar
	Il2CppChar ___m_firstChar_1;

public:
	inline static int32_t get_offset_of_m_stringLength_0() { return static_cast<int32_t>(offsetof(String_t, ___m_stringLength_0)); }
	inline int32_t get_m_stringLength_0() const { return ___m_stringLength_0; }
	inline int32_t* get_address_of_m_stringLength_0() { return &___m_stringLength_0; }
	inline void set_m_stringLength_0(int32_t value)
	{
		___m_stringLength_0 = value;
	}

	inline static int32_t get_offset_of_m_firstChar_1() { return static_cast<int32_t>(offsetof(String_t, ___m_firstChar_1)); }
	inline Il2CppChar get_m_firstChar_1() const { return ___m_firstChar_1; }
	inline Il2CppChar* get_address_of_m_firstChar_1() { return &___m_firstChar_1; }
	inline void set_m_firstChar_1(Il2CppChar value)
	{
		___m_firstChar_1 = value;
	}
};

struct String_t_StaticFields
{
public:
	// System.String System.String::Empty
	String_t* ___Empty_5;

public:
	inline static int32_t get_offset_of_Empty_5() { return static_cast<int32_t>(offsetof(String_t_StaticFields, ___Empty_5)); }
	inline String_t* get_Empty_5() const { return ___Empty_5; }
	inline String_t** get_address_of_Empty_5() { return &___Empty_5; }
	inline void set_Empty_5(String_t* value)
	{
		___Empty_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Empty_5), (void*)value);
	}
};


// System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_com
{
};

// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue
struct CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 
{
public:
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::coarseValue
	uint8_t ___coarseValue_0;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::fineValue
	uint8_t ___fineValue_1;
	// System.Int16 DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::combined
	int16_t ___combined_2;

public:
	inline static int32_t get_offset_of_coarseValue_0() { return static_cast<int32_t>(offsetof(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3, ___coarseValue_0)); }
	inline uint8_t get_coarseValue_0() const { return ___coarseValue_0; }
	inline uint8_t* get_address_of_coarseValue_0() { return &___coarseValue_0; }
	inline void set_coarseValue_0(uint8_t value)
	{
		___coarseValue_0 = value;
	}

	inline static int32_t get_offset_of_fineValue_1() { return static_cast<int32_t>(offsetof(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3, ___fineValue_1)); }
	inline uint8_t get_fineValue_1() const { return ___fineValue_1; }
	inline uint8_t* get_address_of_fineValue_1() { return &___fineValue_1; }
	inline void set_fineValue_1(uint8_t value)
	{
		___fineValue_1 = value;
	}

	inline static int32_t get_offset_of_combined_2() { return static_cast<int32_t>(offsetof(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3, ___combined_2)); }
	inline int16_t get_combined_2() const { return ___combined_2; }
	inline int16_t* get_address_of_combined_2() { return &___combined_2; }
	inline void set_combined_2(int16_t value)
	{
		___combined_2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage
struct MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::delta
	int32_t ___delta_0;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::channel
	uint8_t ___channel_1;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::command
	uint8_t ___command_2;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::data1
	uint8_t ___data1_3;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::data2
	uint8_t ___data2_4;

public:
	inline static int32_t get_offset_of_delta_0() { return static_cast<int32_t>(offsetof(MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484, ___delta_0)); }
	inline int32_t get_delta_0() const { return ___delta_0; }
	inline int32_t* get_address_of_delta_0() { return &___delta_0; }
	inline void set_delta_0(int32_t value)
	{
		___delta_0 = value;
	}

	inline static int32_t get_offset_of_channel_1() { return static_cast<int32_t>(offsetof(MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484, ___channel_1)); }
	inline uint8_t get_channel_1() const { return ___channel_1; }
	inline uint8_t* get_address_of_channel_1() { return &___channel_1; }
	inline void set_channel_1(uint8_t value)
	{
		___channel_1 = value;
	}

	inline static int32_t get_offset_of_command_2() { return static_cast<int32_t>(offsetof(MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484, ___command_2)); }
	inline uint8_t get_command_2() const { return ___command_2; }
	inline uint8_t* get_address_of_command_2() { return &___command_2; }
	inline void set_command_2(uint8_t value)
	{
		___command_2 = value;
	}

	inline static int32_t get_offset_of_data1_3() { return static_cast<int32_t>(offsetof(MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484, ___data1_3)); }
	inline uint8_t get_data1_3() const { return ___data1_3; }
	inline uint8_t* get_address_of_data1_3() { return &___data1_3; }
	inline void set_data1_3(uint8_t value)
	{
		___data1_3 = value;
	}

	inline static int32_t get_offset_of_data2_4() { return static_cast<int32_t>(offsetof(MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484, ___data2_4)); }
	inline uint8_t get_data2_4() const { return ___data2_4; }
	inline uint8_t* get_address_of_data2_4() { return &___data2_4; }
	inline void set_data2_4(uint8_t value)
	{
		___data2_4 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent
struct PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 
{
public:
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent::Left
	float ___Left_0;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent::Right
	float ___Right_1;

public:
	inline static int32_t get_offset_of_Left_0() { return static_cast<int32_t>(offsetof(PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90, ___Left_0)); }
	inline float get_Left_0() const { return ___Left_0; }
	inline float* get_address_of_Left_0() { return &___Left_0; }
	inline void set_Left_0(float value)
	{
		___Left_0 = value;
	}

	inline static int32_t get_offset_of_Right_1() { return static_cast<int32_t>(offsetof(PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90, ___Right_1)); }
	inline float get_Right_1() const { return ___Right_1; }
	inline float* get_address_of_Right_1() { return &___Right_1; }
	inline void set_Right_1(float value)
	{
		___Right_1 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData
struct UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079 
{
public:
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Double DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData::double1
			double ___double1_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			double ___double1_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData::float1
			float ___float1_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			float ___float1_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___float2_2_OffsetPadding[4];
			// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData::float2
			float ___float2_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___float2_2_OffsetPadding_forAlignmentOnly[4];
			float ___float2_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData::int1
			int32_t ___int1_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___int1_3_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___int2_4_OffsetPadding[4];
			// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData::int2
			int32_t ___int2_4;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___int2_4_OffsetPadding_forAlignmentOnly[4];
			int32_t ___int2_4_forAlignmentOnly;
		};
	};

public:
	inline static int32_t get_offset_of_double1_0() { return static_cast<int32_t>(offsetof(UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079, ___double1_0)); }
	inline double get_double1_0() const { return ___double1_0; }
	inline double* get_address_of_double1_0() { return &___double1_0; }
	inline void set_double1_0(double value)
	{
		___double1_0 = value;
	}

	inline static int32_t get_offset_of_float1_1() { return static_cast<int32_t>(offsetof(UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079, ___float1_1)); }
	inline float get_float1_1() const { return ___float1_1; }
	inline float* get_address_of_float1_1() { return &___float1_1; }
	inline void set_float1_1(float value)
	{
		___float1_1 = value;
	}

	inline static int32_t get_offset_of_float2_2() { return static_cast<int32_t>(offsetof(UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079, ___float2_2)); }
	inline float get_float2_2() const { return ___float2_2; }
	inline float* get_address_of_float2_2() { return &___float2_2; }
	inline void set_float2_2(float value)
	{
		___float2_2 = value;
	}

	inline static int32_t get_offset_of_int1_3() { return static_cast<int32_t>(offsetof(UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079, ___int1_3)); }
	inline int32_t get_int1_3() const { return ___int1_3; }
	inline int32_t* get_address_of_int1_3() { return &___int1_3; }
	inline void set_int1_3(int32_t value)
	{
		___int1_3 = value;
	}

	inline static int32_t get_offset_of_int2_4() { return static_cast<int32_t>(offsetof(UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079, ___int2_4)); }
	inline int32_t get_int2_4() const { return ___int2_4; }
	inline int32_t* get_address_of_int2_4() { return &___int2_4; }
	inline void set_int2_4(int32_t value)
	{
		___int2_4 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Util.Riff.RiffTypeChunk
struct RiffTypeChunk_t0EBCC7D1B96A4601BABB5AE304EAB15D844650FD  : public Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15
{
public:
	// System.String DaggerfallWorkshop.AudioSynthesis.Util.Riff.RiffTypeChunk::typeId
	String_t* ___typeId_2;

public:
	inline static int32_t get_offset_of_typeId_2() { return static_cast<int32_t>(offsetof(RiffTypeChunk_t0EBCC7D1B96A4601BABB5AE304EAB15D844650FD, ___typeId_2)); }
	inline String_t* get_typeId_2() const { return ___typeId_2; }
	inline String_t** get_address_of_typeId_2() { return &___typeId_2; }
	inline void set_typeId_2(String_t* value)
	{
		___typeId_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___typeId_2), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData16Bit
struct PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2  : public PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC
{
public:

public:
};


// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData24Bit
struct PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA  : public PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC
{
public:

public:
};


// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData32Bit
struct PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7  : public PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC
{
public:

public:
};


// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData8Bit
struct PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965  : public PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC
{
public:

public:
};


// System.Boolean
struct Boolean_tB53F6830F670160873277339AA58F15CAED4399C 
{
public:
	// System.Boolean System.Boolean::m_value
	bool ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C, ___m_value_0)); }
	inline bool get_m_value_0() const { return ___m_value_0; }
	inline bool* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(bool value)
	{
		___m_value_0 = value;
	}
};

struct Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields
{
public:
	// System.String System.Boolean::TrueString
	String_t* ___TrueString_5;
	// System.String System.Boolean::FalseString
	String_t* ___FalseString_6;

public:
	inline static int32_t get_offset_of_TrueString_5() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields, ___TrueString_5)); }
	inline String_t* get_TrueString_5() const { return ___TrueString_5; }
	inline String_t** get_address_of_TrueString_5() { return &___TrueString_5; }
	inline void set_TrueString_5(String_t* value)
	{
		___TrueString_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___TrueString_5), (void*)value);
	}

	inline static int32_t get_offset_of_FalseString_6() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields, ___FalseString_6)); }
	inline String_t* get_FalseString_6() const { return ___FalseString_6; }
	inline String_t** get_address_of_FalseString_6() { return &___FalseString_6; }
	inline void set_FalseString_6(String_t* value)
	{
		___FalseString_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FalseString_6), (void*)value);
	}
};


// System.Byte
struct Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07 
{
public:
	// System.Byte System.Byte::m_value
	uint8_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07, ___m_value_0)); }
	inline uint8_t get_m_value_0() const { return ___m_value_0; }
	inline uint8_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint8_t value)
	{
		___m_value_0 = value;
	}
};


// System.Char
struct Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9 
{
public:
	// System.Char System.Char::m_value
	Il2CppChar ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9, ___m_value_0)); }
	inline Il2CppChar get_m_value_0() const { return ___m_value_0; }
	inline Il2CppChar* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(Il2CppChar value)
	{
		___m_value_0 = value;
	}
};

struct Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_StaticFields
{
public:
	// System.Byte[] System.Char::categoryForLatin1
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___categoryForLatin1_3;

public:
	inline static int32_t get_offset_of_categoryForLatin1_3() { return static_cast<int32_t>(offsetof(Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_StaticFields, ___categoryForLatin1_3)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_categoryForLatin1_3() const { return ___categoryForLatin1_3; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_categoryForLatin1_3() { return &___categoryForLatin1_3; }
	inline void set_categoryForLatin1_3(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___categoryForLatin1_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___categoryForLatin1_3), (void*)value);
	}
};


// System.Collections.Generic.Stack`1/Enumerator<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>
struct Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 
{
public:
	// System.Collections.Generic.Stack`1<T> System.Collections.Generic.Stack`1/Enumerator::_stack
	Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * ____stack_0;
	// System.Int32 System.Collections.Generic.Stack`1/Enumerator::_version
	int32_t ____version_1;
	// System.Int32 System.Collections.Generic.Stack`1/Enumerator::_index
	int32_t ____index_2;
	// T System.Collections.Generic.Stack`1/Enumerator::_currentElement
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * ____currentElement_3;

public:
	inline static int32_t get_offset_of__stack_0() { return static_cast<int32_t>(offsetof(Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7, ____stack_0)); }
	inline Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * get__stack_0() const { return ____stack_0; }
	inline Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 ** get_address_of__stack_0() { return &____stack_0; }
	inline void set__stack_0(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * value)
	{
		____stack_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stack_0), (void*)value);
	}

	inline static int32_t get_offset_of__version_1() { return static_cast<int32_t>(offsetof(Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7, ____version_1)); }
	inline int32_t get__version_1() const { return ____version_1; }
	inline int32_t* get_address_of__version_1() { return &____version_1; }
	inline void set__version_1(int32_t value)
	{
		____version_1 = value;
	}

	inline static int32_t get_offset_of__index_2() { return static_cast<int32_t>(offsetof(Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7, ____index_2)); }
	inline int32_t get__index_2() const { return ____index_2; }
	inline int32_t* get_address_of__index_2() { return &____index_2; }
	inline void set__index_2(int32_t value)
	{
		____index_2 = value;
	}

	inline static int32_t get_offset_of__currentElement_3() { return static_cast<int32_t>(offsetof(Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7, ____currentElement_3)); }
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * get__currentElement_3() const { return ____currentElement_3; }
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** get_address_of__currentElement_3() { return &____currentElement_3; }
	inline void set__currentElement_3(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		____currentElement_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____currentElement_3), (void*)value);
	}
};


// System.Collections.Generic.Stack`1/Enumerator<System.Object>
struct Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A 
{
public:
	// System.Collections.Generic.Stack`1<T> System.Collections.Generic.Stack`1/Enumerator::_stack
	Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * ____stack_0;
	// System.Int32 System.Collections.Generic.Stack`1/Enumerator::_version
	int32_t ____version_1;
	// System.Int32 System.Collections.Generic.Stack`1/Enumerator::_index
	int32_t ____index_2;
	// T System.Collections.Generic.Stack`1/Enumerator::_currentElement
	RuntimeObject * ____currentElement_3;

public:
	inline static int32_t get_offset_of__stack_0() { return static_cast<int32_t>(offsetof(Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A, ____stack_0)); }
	inline Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * get__stack_0() const { return ____stack_0; }
	inline Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 ** get_address_of__stack_0() { return &____stack_0; }
	inline void set__stack_0(Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * value)
	{
		____stack_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stack_0), (void*)value);
	}

	inline static int32_t get_offset_of__version_1() { return static_cast<int32_t>(offsetof(Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A, ____version_1)); }
	inline int32_t get__version_1() const { return ____version_1; }
	inline int32_t* get_address_of__version_1() { return &____version_1; }
	inline void set__version_1(int32_t value)
	{
		____version_1 = value;
	}

	inline static int32_t get_offset_of__index_2() { return static_cast<int32_t>(offsetof(Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A, ____index_2)); }
	inline int32_t get__index_2() const { return ____index_2; }
	inline int32_t* get_address_of__index_2() { return &____index_2; }
	inline void set__index_2(int32_t value)
	{
		____index_2 = value;
	}

	inline static int32_t get_offset_of__currentElement_3() { return static_cast<int32_t>(offsetof(Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A, ____currentElement_3)); }
	inline RuntimeObject * get__currentElement_3() const { return ____currentElement_3; }
	inline RuntimeObject ** get_address_of__currentElement_3() { return &____currentElement_3; }
	inline void set__currentElement_3(RuntimeObject * value)
	{
		____currentElement_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____currentElement_3), (void*)value);
	}
};


// System.Double
struct Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409 
{
public:
	// System.Double System.Double::m_value
	double ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409, ___m_value_0)); }
	inline double get_m_value_0() const { return ___m_value_0; }
	inline double* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(double value)
	{
		___m_value_0 = value;
	}
};

struct Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_StaticFields
{
public:
	// System.Double System.Double::NegativeZero
	double ___NegativeZero_7;

public:
	inline static int32_t get_offset_of_NegativeZero_7() { return static_cast<int32_t>(offsetof(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_StaticFields, ___NegativeZero_7)); }
	inline double get_NegativeZero_7() const { return ___NegativeZero_7; }
	inline double* get_address_of_NegativeZero_7() { return &___NegativeZero_7; }
	inline void set_NegativeZero_7(double value)
	{
		___NegativeZero_7 = value;
	}
};


// System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521  : public ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF
{
public:

public:
};

struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_StaticFields
{
public:
	// System.Char[] System.Enum::enumSeperatorCharArray
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___enumSeperatorCharArray_0;

public:
	inline static int32_t get_offset_of_enumSeperatorCharArray_0() { return static_cast<int32_t>(offsetof(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_StaticFields, ___enumSeperatorCharArray_0)); }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* get_enumSeperatorCharArray_0() const { return ___enumSeperatorCharArray_0; }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2** get_address_of_enumSeperatorCharArray_0() { return &___enumSeperatorCharArray_0; }
	inline void set_enumSeperatorCharArray_0(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* value)
	{
		___enumSeperatorCharArray_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumSeperatorCharArray_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_marshaled_com
{
};

// System.IO.Stream
struct Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7  : public MarshalByRefObject_tC4577953D0A44D0AB8597CFA868E01C858B1C9AF
{
public:
	// System.IO.Stream/ReadWriteTask System.IO.Stream::_activeReadWriteTask
	ReadWriteTask_tFA17EEE8BC5C4C83EAEFCC3662A30DE351ABAA80 * ____activeReadWriteTask_3;
	// System.Threading.SemaphoreSlim System.IO.Stream::_asyncActiveSemaphore
	SemaphoreSlim_t2E2888D1C0C8FAB80823C76F1602E4434B8FA048 * ____asyncActiveSemaphore_4;

public:
	inline static int32_t get_offset_of__activeReadWriteTask_3() { return static_cast<int32_t>(offsetof(Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7, ____activeReadWriteTask_3)); }
	inline ReadWriteTask_tFA17EEE8BC5C4C83EAEFCC3662A30DE351ABAA80 * get__activeReadWriteTask_3() const { return ____activeReadWriteTask_3; }
	inline ReadWriteTask_tFA17EEE8BC5C4C83EAEFCC3662A30DE351ABAA80 ** get_address_of__activeReadWriteTask_3() { return &____activeReadWriteTask_3; }
	inline void set__activeReadWriteTask_3(ReadWriteTask_tFA17EEE8BC5C4C83EAEFCC3662A30DE351ABAA80 * value)
	{
		____activeReadWriteTask_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____activeReadWriteTask_3), (void*)value);
	}

	inline static int32_t get_offset_of__asyncActiveSemaphore_4() { return static_cast<int32_t>(offsetof(Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7, ____asyncActiveSemaphore_4)); }
	inline SemaphoreSlim_t2E2888D1C0C8FAB80823C76F1602E4434B8FA048 * get__asyncActiveSemaphore_4() const { return ____asyncActiveSemaphore_4; }
	inline SemaphoreSlim_t2E2888D1C0C8FAB80823C76F1602E4434B8FA048 ** get_address_of__asyncActiveSemaphore_4() { return &____asyncActiveSemaphore_4; }
	inline void set__asyncActiveSemaphore_4(SemaphoreSlim_t2E2888D1C0C8FAB80823C76F1602E4434B8FA048 * value)
	{
		____asyncActiveSemaphore_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____asyncActiveSemaphore_4), (void*)value);
	}
};

struct Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7_StaticFields
{
public:
	// System.IO.Stream System.IO.Stream::Null
	Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * ___Null_1;

public:
	inline static int32_t get_offset_of_Null_1() { return static_cast<int32_t>(offsetof(Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7_StaticFields, ___Null_1)); }
	inline Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * get_Null_1() const { return ___Null_1; }
	inline Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 ** get_address_of_Null_1() { return &___Null_1; }
	inline void set_Null_1(Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * value)
	{
		___Null_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Null_1), (void*)value);
	}
};


// System.Int16
struct Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D 
{
public:
	// System.Int16 System.Int16::m_value
	int16_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D, ___m_value_0)); }
	inline int16_t get_m_value_0() const { return ___m_value_0; }
	inline int16_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int16_t value)
	{
		___m_value_0 = value;
	}
};


// System.Int32
struct Int32_t585191389E07734F19F3156FF88FB3EF4800D102 
{
public:
	// System.Int32 System.Int32::m_value
	int32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int32_t585191389E07734F19F3156FF88FB3EF4800D102, ___m_value_0)); }
	inline int32_t get_m_value_0() const { return ___m_value_0; }
	inline int32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int32_t value)
	{
		___m_value_0 = value;
	}
};


// System.Int64
struct Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436 
{
public:
	// System.Int64 System.Int64::m_value
	int64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436, ___m_value_0)); }
	inline int64_t get_m_value_0() const { return ___m_value_0; }
	inline int64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int64_t value)
	{
		___m_value_0 = value;
	}
};


// System.IntPtr
struct IntPtr_t 
{
public:
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(IntPtr_t, ___m_value_0)); }
	inline void* get_m_value_0() const { return ___m_value_0; }
	inline void** get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(void* value)
	{
		___m_value_0 = value;
	}
};

struct IntPtr_t_StaticFields
{
public:
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;

public:
	inline static int32_t get_offset_of_Zero_1() { return static_cast<int32_t>(offsetof(IntPtr_t_StaticFields, ___Zero_1)); }
	inline intptr_t get_Zero_1() const { return ___Zero_1; }
	inline intptr_t* get_address_of_Zero_1() { return &___Zero_1; }
	inline void set_Zero_1(intptr_t value)
	{
		___Zero_1 = value;
	}
};


// System.Single
struct Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1 
{
public:
	// System.Single System.Single::m_value
	float ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1, ___m_value_0)); }
	inline float get_m_value_0() const { return ___m_value_0; }
	inline float* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(float value)
	{
		___m_value_0 = value;
	}
};


// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017__padding[1];
	};

public:
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.EnvelopeStateEnum
struct EnvelopeStateEnum_tAAD05052A2F7143E1B00B1504B92D05DC01D00F8 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.EnvelopeStateEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(EnvelopeStateEnum_tAAD05052A2F7143E1B00B1504B92D05DC01D00F8, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.FilterTypeEnum
struct FilterTypeEnum_t8DF3A2F852E55F112BF33473440DE41685C619A0 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.FilterTypeEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(FilterTypeEnum_t8DF3A2F852E55F112BF33473440DE41685C619A0, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.GeneratorStateEnum
struct GeneratorStateEnum_tC3A8B7DBD22E10EB247B5F619638339A56006D7D 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.GeneratorStateEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(GeneratorStateEnum_tC3A8B7DBD22E10EB247B5F619638339A56006D7D, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.InterpolationEnum
struct InterpolationEnum_t60F94282AED3ABD6120E3E58F2845A87A5CF322C 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.InterpolationEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(InterpolationEnum_t60F94282AED3ABD6120E3E58F2845A87A5CF322C, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.LfoStateEnum
struct LfoStateEnum_tD5611B955F282128D1AE6D9885904303A434EF56 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.LfoStateEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(LfoStateEnum_tD5611B955F282128D1AE6D9885904303A434EF56, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch/IntervalType
struct IntervalType_t4E9F44B56B8CCCF600C881BC5BF8F301B86E4097 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch/IntervalType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(IntervalType_t4E9F44B56B8CCCF600C881BC5BF8F301B86E4097, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Midi.MidiEventTypeEnum
struct MidiEventTypeEnum_t99E303F3D268222216E7B24459B04ACF2C462C68 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Midi.MidiEventTypeEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(MidiEventTypeEnum_t99E303F3D268222216E7B24459B04ACF2C462C68, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.GeneratorEnum
struct GeneratorEnum_t67B3C57C3455060F340C5F9A5CB7B73AEBCBAFFE 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Sf2.GeneratorEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(GeneratorEnum_t67B3C57C3455060F340C5F9A5CB7B73AEBCBAFFE, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.SourceTypeEnum
struct SourceTypeEnum_tACA8A85BF3FBC478703246F35F2180D42822B025 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Sf2.SourceTypeEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(SourceTypeEnum_tACA8A85BF3FBC478703246F35F2180D42822B025, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.TransformEnum
struct TransformEnum_t2D28A00DF766821179CC5359EB66E09135BD2A0C 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Sf2.TransformEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TransformEnum_t2D28A00DF766821179CC5359EB66E09135BD2A0C, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.PanFormulaEnum
struct PanFormulaEnum_t8FD1A5D74AF46AE20DCD44201585EC8F36F027C9 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.PanFormulaEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(PanFormulaEnum_t8FD1A5D74AF46AE20DCD44201585EC8F36F027C9, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters
struct SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33  : public RuntimeObject
{
public:
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::program
	uint8_t ___program_0;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::bankSelect
	uint8_t ___bankSelect_1;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::channelAfterTouch
	uint8_t ___channelAfterTouch_2;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::pan
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___pan_3;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::volume
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___volume_4;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::expression
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___expression_5;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::modRange
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___modRange_6;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::pitchBend
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___pitchBend_7;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::pitchBendRangeCoarse
	uint8_t ___pitchBendRangeCoarse_8;
	// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::pitchBendRangeFine
	uint8_t ___pitchBendRangeFine_9;
	// System.Int16 DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::masterCoarseTune
	int16_t ___masterCoarseTune_10;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::masterFineTune
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___masterFineTune_11;
	// System.Boolean DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::holdPedal
	bool ___holdPedal_12;
	// System.Boolean DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::legatoPedal
	bool ___legatoPedal_13;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::rpn
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  ___rpn_14;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::synth
	Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * ___synth_15;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::currentVolume
	float ___currentVolume_16;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::currentPitch
	int32_t ___currentPitch_17;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::currentMod
	int32_t ___currentMod_18;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::currentPan
	PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90  ___currentPan_19;

public:
	inline static int32_t get_offset_of_program_0() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___program_0)); }
	inline uint8_t get_program_0() const { return ___program_0; }
	inline uint8_t* get_address_of_program_0() { return &___program_0; }
	inline void set_program_0(uint8_t value)
	{
		___program_0 = value;
	}

	inline static int32_t get_offset_of_bankSelect_1() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___bankSelect_1)); }
	inline uint8_t get_bankSelect_1() const { return ___bankSelect_1; }
	inline uint8_t* get_address_of_bankSelect_1() { return &___bankSelect_1; }
	inline void set_bankSelect_1(uint8_t value)
	{
		___bankSelect_1 = value;
	}

	inline static int32_t get_offset_of_channelAfterTouch_2() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___channelAfterTouch_2)); }
	inline uint8_t get_channelAfterTouch_2() const { return ___channelAfterTouch_2; }
	inline uint8_t* get_address_of_channelAfterTouch_2() { return &___channelAfterTouch_2; }
	inline void set_channelAfterTouch_2(uint8_t value)
	{
		___channelAfterTouch_2 = value;
	}

	inline static int32_t get_offset_of_pan_3() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___pan_3)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_pan_3() const { return ___pan_3; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_pan_3() { return &___pan_3; }
	inline void set_pan_3(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___pan_3 = value;
	}

	inline static int32_t get_offset_of_volume_4() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___volume_4)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_volume_4() const { return ___volume_4; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_volume_4() { return &___volume_4; }
	inline void set_volume_4(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___volume_4 = value;
	}

	inline static int32_t get_offset_of_expression_5() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___expression_5)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_expression_5() const { return ___expression_5; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_expression_5() { return &___expression_5; }
	inline void set_expression_5(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___expression_5 = value;
	}

	inline static int32_t get_offset_of_modRange_6() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___modRange_6)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_modRange_6() const { return ___modRange_6; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_modRange_6() { return &___modRange_6; }
	inline void set_modRange_6(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___modRange_6 = value;
	}

	inline static int32_t get_offset_of_pitchBend_7() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___pitchBend_7)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_pitchBend_7() const { return ___pitchBend_7; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_pitchBend_7() { return &___pitchBend_7; }
	inline void set_pitchBend_7(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___pitchBend_7 = value;
	}

	inline static int32_t get_offset_of_pitchBendRangeCoarse_8() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___pitchBendRangeCoarse_8)); }
	inline uint8_t get_pitchBendRangeCoarse_8() const { return ___pitchBendRangeCoarse_8; }
	inline uint8_t* get_address_of_pitchBendRangeCoarse_8() { return &___pitchBendRangeCoarse_8; }
	inline void set_pitchBendRangeCoarse_8(uint8_t value)
	{
		___pitchBendRangeCoarse_8 = value;
	}

	inline static int32_t get_offset_of_pitchBendRangeFine_9() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___pitchBendRangeFine_9)); }
	inline uint8_t get_pitchBendRangeFine_9() const { return ___pitchBendRangeFine_9; }
	inline uint8_t* get_address_of_pitchBendRangeFine_9() { return &___pitchBendRangeFine_9; }
	inline void set_pitchBendRangeFine_9(uint8_t value)
	{
		___pitchBendRangeFine_9 = value;
	}

	inline static int32_t get_offset_of_masterCoarseTune_10() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___masterCoarseTune_10)); }
	inline int16_t get_masterCoarseTune_10() const { return ___masterCoarseTune_10; }
	inline int16_t* get_address_of_masterCoarseTune_10() { return &___masterCoarseTune_10; }
	inline void set_masterCoarseTune_10(int16_t value)
	{
		___masterCoarseTune_10 = value;
	}

	inline static int32_t get_offset_of_masterFineTune_11() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___masterFineTune_11)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_masterFineTune_11() const { return ___masterFineTune_11; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_masterFineTune_11() { return &___masterFineTune_11; }
	inline void set_masterFineTune_11(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___masterFineTune_11 = value;
	}

	inline static int32_t get_offset_of_holdPedal_12() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___holdPedal_12)); }
	inline bool get_holdPedal_12() const { return ___holdPedal_12; }
	inline bool* get_address_of_holdPedal_12() { return &___holdPedal_12; }
	inline void set_holdPedal_12(bool value)
	{
		___holdPedal_12 = value;
	}

	inline static int32_t get_offset_of_legatoPedal_13() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___legatoPedal_13)); }
	inline bool get_legatoPedal_13() const { return ___legatoPedal_13; }
	inline bool* get_address_of_legatoPedal_13() { return &___legatoPedal_13; }
	inline void set_legatoPedal_13(bool value)
	{
		___legatoPedal_13 = value;
	}

	inline static int32_t get_offset_of_rpn_14() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___rpn_14)); }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  get_rpn_14() const { return ___rpn_14; }
	inline CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * get_address_of_rpn_14() { return &___rpn_14; }
	inline void set_rpn_14(CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3  value)
	{
		___rpn_14 = value;
	}

	inline static int32_t get_offset_of_synth_15() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___synth_15)); }
	inline Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * get_synth_15() const { return ___synth_15; }
	inline Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 ** get_address_of_synth_15() { return &___synth_15; }
	inline void set_synth_15(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * value)
	{
		___synth_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___synth_15), (void*)value);
	}

	inline static int32_t get_offset_of_currentVolume_16() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___currentVolume_16)); }
	inline float get_currentVolume_16() const { return ___currentVolume_16; }
	inline float* get_address_of_currentVolume_16() { return &___currentVolume_16; }
	inline void set_currentVolume_16(float value)
	{
		___currentVolume_16 = value;
	}

	inline static int32_t get_offset_of_currentPitch_17() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___currentPitch_17)); }
	inline int32_t get_currentPitch_17() const { return ___currentPitch_17; }
	inline int32_t* get_address_of_currentPitch_17() { return &___currentPitch_17; }
	inline void set_currentPitch_17(int32_t value)
	{
		___currentPitch_17 = value;
	}

	inline static int32_t get_offset_of_currentMod_18() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___currentMod_18)); }
	inline int32_t get_currentMod_18() const { return ___currentMod_18; }
	inline int32_t* get_address_of_currentMod_18() { return &___currentMod_18; }
	inline void set_currentMod_18(int32_t value)
	{
		___currentMod_18 = value;
	}

	inline static int32_t get_offset_of_currentPan_19() { return static_cast<int32_t>(offsetof(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33, ___currentPan_19)); }
	inline PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90  get_currentPan_19() const { return ___currentPan_19; }
	inline PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * get_address_of_currentPan_19() { return &___currentPan_19; }
	inline void set_currentPan_19(PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90  value)
	{
		___currentPan_19 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceStateEnum
struct VoiceStateEnum_t1DC114A9113887829A4FAA949B8F4E77286A95FE 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceStateEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VoiceStateEnum_t1DC114A9113887829A4FAA949B8F4E77286A95FE, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceStealEnum
struct VoiceStealEnum_t7526B36B3E6408003D4909797C2DBA7643396ED3 
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceStealEnum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VoiceStealEnum_t7526B36B3E6408003D4909797C2DBA7643396ED3, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Exception
struct Exception_t  : public RuntimeObject
{
public:
	// System.String System.Exception::_className
	String_t* ____className_1;
	// System.String System.Exception::_message
	String_t* ____message_2;
	// System.Collections.IDictionary System.Exception::_data
	RuntimeObject* ____data_3;
	// System.Exception System.Exception::_innerException
	Exception_t * ____innerException_4;
	// System.String System.Exception::_helpURL
	String_t* ____helpURL_5;
	// System.Object System.Exception::_stackTrace
	RuntimeObject * ____stackTrace_6;
	// System.String System.Exception::_stackTraceString
	String_t* ____stackTraceString_7;
	// System.String System.Exception::_remoteStackTraceString
	String_t* ____remoteStackTraceString_8;
	// System.Int32 System.Exception::_remoteStackIndex
	int32_t ____remoteStackIndex_9;
	// System.Object System.Exception::_dynamicMethods
	RuntimeObject * ____dynamicMethods_10;
	// System.Int32 System.Exception::_HResult
	int32_t ____HResult_11;
	// System.String System.Exception::_source
	String_t* ____source_12;
	// System.Runtime.Serialization.SafeSerializationManager System.Exception::_safeSerializationManager
	SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * ____safeSerializationManager_13;
	// System.Diagnostics.StackTrace[] System.Exception::captured_traces
	StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* ___captured_traces_14;
	// System.IntPtr[] System.Exception::native_trace_ips
	IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* ___native_trace_ips_15;

public:
	inline static int32_t get_offset_of__className_1() { return static_cast<int32_t>(offsetof(Exception_t, ____className_1)); }
	inline String_t* get__className_1() const { return ____className_1; }
	inline String_t** get_address_of__className_1() { return &____className_1; }
	inline void set__className_1(String_t* value)
	{
		____className_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____className_1), (void*)value);
	}

	inline static int32_t get_offset_of__message_2() { return static_cast<int32_t>(offsetof(Exception_t, ____message_2)); }
	inline String_t* get__message_2() const { return ____message_2; }
	inline String_t** get_address_of__message_2() { return &____message_2; }
	inline void set__message_2(String_t* value)
	{
		____message_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____message_2), (void*)value);
	}

	inline static int32_t get_offset_of__data_3() { return static_cast<int32_t>(offsetof(Exception_t, ____data_3)); }
	inline RuntimeObject* get__data_3() const { return ____data_3; }
	inline RuntimeObject** get_address_of__data_3() { return &____data_3; }
	inline void set__data_3(RuntimeObject* value)
	{
		____data_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____data_3), (void*)value);
	}

	inline static int32_t get_offset_of__innerException_4() { return static_cast<int32_t>(offsetof(Exception_t, ____innerException_4)); }
	inline Exception_t * get__innerException_4() const { return ____innerException_4; }
	inline Exception_t ** get_address_of__innerException_4() { return &____innerException_4; }
	inline void set__innerException_4(Exception_t * value)
	{
		____innerException_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____innerException_4), (void*)value);
	}

	inline static int32_t get_offset_of__helpURL_5() { return static_cast<int32_t>(offsetof(Exception_t, ____helpURL_5)); }
	inline String_t* get__helpURL_5() const { return ____helpURL_5; }
	inline String_t** get_address_of__helpURL_5() { return &____helpURL_5; }
	inline void set__helpURL_5(String_t* value)
	{
		____helpURL_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____helpURL_5), (void*)value);
	}

	inline static int32_t get_offset_of__stackTrace_6() { return static_cast<int32_t>(offsetof(Exception_t, ____stackTrace_6)); }
	inline RuntimeObject * get__stackTrace_6() const { return ____stackTrace_6; }
	inline RuntimeObject ** get_address_of__stackTrace_6() { return &____stackTrace_6; }
	inline void set__stackTrace_6(RuntimeObject * value)
	{
		____stackTrace_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stackTrace_6), (void*)value);
	}

	inline static int32_t get_offset_of__stackTraceString_7() { return static_cast<int32_t>(offsetof(Exception_t, ____stackTraceString_7)); }
	inline String_t* get__stackTraceString_7() const { return ____stackTraceString_7; }
	inline String_t** get_address_of__stackTraceString_7() { return &____stackTraceString_7; }
	inline void set__stackTraceString_7(String_t* value)
	{
		____stackTraceString_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stackTraceString_7), (void*)value);
	}

	inline static int32_t get_offset_of__remoteStackTraceString_8() { return static_cast<int32_t>(offsetof(Exception_t, ____remoteStackTraceString_8)); }
	inline String_t* get__remoteStackTraceString_8() const { return ____remoteStackTraceString_8; }
	inline String_t** get_address_of__remoteStackTraceString_8() { return &____remoteStackTraceString_8; }
	inline void set__remoteStackTraceString_8(String_t* value)
	{
		____remoteStackTraceString_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____remoteStackTraceString_8), (void*)value);
	}

	inline static int32_t get_offset_of__remoteStackIndex_9() { return static_cast<int32_t>(offsetof(Exception_t, ____remoteStackIndex_9)); }
	inline int32_t get__remoteStackIndex_9() const { return ____remoteStackIndex_9; }
	inline int32_t* get_address_of__remoteStackIndex_9() { return &____remoteStackIndex_9; }
	inline void set__remoteStackIndex_9(int32_t value)
	{
		____remoteStackIndex_9 = value;
	}

	inline static int32_t get_offset_of__dynamicMethods_10() { return static_cast<int32_t>(offsetof(Exception_t, ____dynamicMethods_10)); }
	inline RuntimeObject * get__dynamicMethods_10() const { return ____dynamicMethods_10; }
	inline RuntimeObject ** get_address_of__dynamicMethods_10() { return &____dynamicMethods_10; }
	inline void set__dynamicMethods_10(RuntimeObject * value)
	{
		____dynamicMethods_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____dynamicMethods_10), (void*)value);
	}

	inline static int32_t get_offset_of__HResult_11() { return static_cast<int32_t>(offsetof(Exception_t, ____HResult_11)); }
	inline int32_t get__HResult_11() const { return ____HResult_11; }
	inline int32_t* get_address_of__HResult_11() { return &____HResult_11; }
	inline void set__HResult_11(int32_t value)
	{
		____HResult_11 = value;
	}

	inline static int32_t get_offset_of__source_12() { return static_cast<int32_t>(offsetof(Exception_t, ____source_12)); }
	inline String_t* get__source_12() const { return ____source_12; }
	inline String_t** get_address_of__source_12() { return &____source_12; }
	inline void set__source_12(String_t* value)
	{
		____source_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____source_12), (void*)value);
	}

	inline static int32_t get_offset_of__safeSerializationManager_13() { return static_cast<int32_t>(offsetof(Exception_t, ____safeSerializationManager_13)); }
	inline SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * get__safeSerializationManager_13() const { return ____safeSerializationManager_13; }
	inline SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 ** get_address_of__safeSerializationManager_13() { return &____safeSerializationManager_13; }
	inline void set__safeSerializationManager_13(SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * value)
	{
		____safeSerializationManager_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____safeSerializationManager_13), (void*)value);
	}

	inline static int32_t get_offset_of_captured_traces_14() { return static_cast<int32_t>(offsetof(Exception_t, ___captured_traces_14)); }
	inline StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* get_captured_traces_14() const { return ___captured_traces_14; }
	inline StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196** get_address_of_captured_traces_14() { return &___captured_traces_14; }
	inline void set_captured_traces_14(StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* value)
	{
		___captured_traces_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___captured_traces_14), (void*)value);
	}

	inline static int32_t get_offset_of_native_trace_ips_15() { return static_cast<int32_t>(offsetof(Exception_t, ___native_trace_ips_15)); }
	inline IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* get_native_trace_ips_15() const { return ___native_trace_ips_15; }
	inline IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD** get_address_of_native_trace_ips_15() { return &___native_trace_ips_15; }
	inline void set_native_trace_ips_15(IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* value)
	{
		___native_trace_ips_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___native_trace_ips_15), (void*)value);
	}
};

struct Exception_t_StaticFields
{
public:
	// System.Object System.Exception::s_EDILock
	RuntimeObject * ___s_EDILock_0;

public:
	inline static int32_t get_offset_of_s_EDILock_0() { return static_cast<int32_t>(offsetof(Exception_t_StaticFields, ___s_EDILock_0)); }
	inline RuntimeObject * get_s_EDILock_0() const { return ___s_EDILock_0; }
	inline RuntimeObject ** get_address_of_s_EDILock_0() { return &___s_EDILock_0; }
	inline void set_s_EDILock_0(RuntimeObject * value)
	{
		___s_EDILock_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_EDILock_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Exception
struct Exception_t_marshaled_pinvoke
{
	char* ____className_1;
	char* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_pinvoke* ____innerException_4;
	char* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	char* ____stackTraceString_7;
	char* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	char* ____source_12;
	SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * ____safeSerializationManager_13;
	StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
};
// Native definition for COM marshalling of System.Exception
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className_1;
	Il2CppChar* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_com* ____innerException_4;
	Il2CppChar* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	Il2CppChar* ____stackTraceString_7;
	Il2CppChar* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	Il2CppChar* ____source_12;
	SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * ____safeSerializationManager_13;
	StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
};

// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope
struct Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.EnvelopeStateEnum DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::envState
	int32_t ___envState_0;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope/EnvelopeStage[] DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::stages
	EnvelopeStageU5BU5D_t1A244D19890F8C934ADAE6A638D8B912186A8525* ___stages_1;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope/EnvelopeStage DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::stage
	EnvelopeStage_t1CA1FE3D2A3E978010D5F2947D413259D3C537B0 * ___stage_2;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::index
	int32_t ___index_3;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::value
	float ___value_4;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::depth
	float ___depth_5;

public:
	inline static int32_t get_offset_of_envState_0() { return static_cast<int32_t>(offsetof(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6, ___envState_0)); }
	inline int32_t get_envState_0() const { return ___envState_0; }
	inline int32_t* get_address_of_envState_0() { return &___envState_0; }
	inline void set_envState_0(int32_t value)
	{
		___envState_0 = value;
	}

	inline static int32_t get_offset_of_stages_1() { return static_cast<int32_t>(offsetof(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6, ___stages_1)); }
	inline EnvelopeStageU5BU5D_t1A244D19890F8C934ADAE6A638D8B912186A8525* get_stages_1() const { return ___stages_1; }
	inline EnvelopeStageU5BU5D_t1A244D19890F8C934ADAE6A638D8B912186A8525** get_address_of_stages_1() { return &___stages_1; }
	inline void set_stages_1(EnvelopeStageU5BU5D_t1A244D19890F8C934ADAE6A638D8B912186A8525* value)
	{
		___stages_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___stages_1), (void*)value);
	}

	inline static int32_t get_offset_of_stage_2() { return static_cast<int32_t>(offsetof(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6, ___stage_2)); }
	inline EnvelopeStage_t1CA1FE3D2A3E978010D5F2947D413259D3C537B0 * get_stage_2() const { return ___stage_2; }
	inline EnvelopeStage_t1CA1FE3D2A3E978010D5F2947D413259D3C537B0 ** get_address_of_stage_2() { return &___stage_2; }
	inline void set_stage_2(EnvelopeStage_t1CA1FE3D2A3E978010D5F2947D413259D3C537B0 * value)
	{
		___stage_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___stage_2), (void*)value);
	}

	inline static int32_t get_offset_of_index_3() { return static_cast<int32_t>(offsetof(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6, ___index_3)); }
	inline int32_t get_index_3() const { return ___index_3; }
	inline int32_t* get_address_of_index_3() { return &___index_3; }
	inline void set_index_3(int32_t value)
	{
		___index_3 = value;
	}

	inline static int32_t get_offset_of_value_4() { return static_cast<int32_t>(offsetof(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6, ___value_4)); }
	inline float get_value_4() const { return ___value_4; }
	inline float* get_address_of_value_4() { return &___value_4; }
	inline void set_value_4(float value)
	{
		___value_4 = value;
	}

	inline static int32_t get_offset_of_depth_5() { return static_cast<int32_t>(offsetof(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6, ___depth_5)); }
	inline float get_depth_5() const { return ___depth_5; }
	inline float* get_address_of_depth_5() { return &___depth_5; }
	inline void set_depth_5(float value)
	{
		___depth_5 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter
struct Filter_t8869C4D2146972E0AFC8080ADBB879E449534331  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.FilterTypeEnum DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::filterType
	int32_t ___filterType_0;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::a1
	float ___a1_1;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::a2
	float ___a2_2;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::b1
	float ___b1_3;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::b2
	float ___b2_4;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::m1
	float ___m1_5;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::m2
	float ___m2_6;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::m3
	float ___m3_7;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::cutOff
	double ___cutOff_8;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::resonance
	double ___resonance_9;
	// System.Boolean DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::coeffUpdateRequired
	bool ___coeffUpdateRequired_10;

public:
	inline static int32_t get_offset_of_filterType_0() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___filterType_0)); }
	inline int32_t get_filterType_0() const { return ___filterType_0; }
	inline int32_t* get_address_of_filterType_0() { return &___filterType_0; }
	inline void set_filterType_0(int32_t value)
	{
		___filterType_0 = value;
	}

	inline static int32_t get_offset_of_a1_1() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___a1_1)); }
	inline float get_a1_1() const { return ___a1_1; }
	inline float* get_address_of_a1_1() { return &___a1_1; }
	inline void set_a1_1(float value)
	{
		___a1_1 = value;
	}

	inline static int32_t get_offset_of_a2_2() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___a2_2)); }
	inline float get_a2_2() const { return ___a2_2; }
	inline float* get_address_of_a2_2() { return &___a2_2; }
	inline void set_a2_2(float value)
	{
		___a2_2 = value;
	}

	inline static int32_t get_offset_of_b1_3() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___b1_3)); }
	inline float get_b1_3() const { return ___b1_3; }
	inline float* get_address_of_b1_3() { return &___b1_3; }
	inline void set_b1_3(float value)
	{
		___b1_3 = value;
	}

	inline static int32_t get_offset_of_b2_4() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___b2_4)); }
	inline float get_b2_4() const { return ___b2_4; }
	inline float* get_address_of_b2_4() { return &___b2_4; }
	inline void set_b2_4(float value)
	{
		___b2_4 = value;
	}

	inline static int32_t get_offset_of_m1_5() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___m1_5)); }
	inline float get_m1_5() const { return ___m1_5; }
	inline float* get_address_of_m1_5() { return &___m1_5; }
	inline void set_m1_5(float value)
	{
		___m1_5 = value;
	}

	inline static int32_t get_offset_of_m2_6() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___m2_6)); }
	inline float get_m2_6() const { return ___m2_6; }
	inline float* get_address_of_m2_6() { return &___m2_6; }
	inline void set_m2_6(float value)
	{
		___m2_6 = value;
	}

	inline static int32_t get_offset_of_m3_7() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___m3_7)); }
	inline float get_m3_7() const { return ___m3_7; }
	inline float* get_address_of_m3_7() { return &___m3_7; }
	inline void set_m3_7(float value)
	{
		___m3_7 = value;
	}

	inline static int32_t get_offset_of_cutOff_8() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___cutOff_8)); }
	inline double get_cutOff_8() const { return ___cutOff_8; }
	inline double* get_address_of_cutOff_8() { return &___cutOff_8; }
	inline void set_cutOff_8(double value)
	{
		___cutOff_8 = value;
	}

	inline static int32_t get_offset_of_resonance_9() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___resonance_9)); }
	inline double get_resonance_9() const { return ___resonance_9; }
	inline double* get_address_of_resonance_9() { return &___resonance_9; }
	inline void set_resonance_9(double value)
	{
		___resonance_9 = value;
	}

	inline static int32_t get_offset_of_coeffUpdateRequired_10() { return static_cast<int32_t>(offsetof(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331, ___coeffUpdateRequired_10)); }
	inline bool get_coeffUpdateRequired_10() const { return ___coeffUpdateRequired_10; }
	inline bool* get_address_of_coeffUpdateRequired_10() { return &___coeffUpdateRequired_10; }
	inline void set_coeffUpdateRequired_10(bool value)
	{
		___coeffUpdateRequired_10 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters
struct GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB  : public RuntimeObject
{
public:
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters::phase
	double ___phase_0;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters::currentStart
	double ___currentStart_1;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters::currentEnd
	double ___currentEnd_2;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.GeneratorStateEnum DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters::currentState
	int32_t ___currentState_3;

public:
	inline static int32_t get_offset_of_phase_0() { return static_cast<int32_t>(offsetof(GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB, ___phase_0)); }
	inline double get_phase_0() const { return ___phase_0; }
	inline double* get_address_of_phase_0() { return &___phase_0; }
	inline void set_phase_0(double value)
	{
		___phase_0 = value;
	}

	inline static int32_t get_offset_of_currentStart_1() { return static_cast<int32_t>(offsetof(GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB, ___currentStart_1)); }
	inline double get_currentStart_1() const { return ___currentStart_1; }
	inline double* get_address_of_currentStart_1() { return &___currentStart_1; }
	inline void set_currentStart_1(double value)
	{
		___currentStart_1 = value;
	}

	inline static int32_t get_offset_of_currentEnd_2() { return static_cast<int32_t>(offsetof(GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB, ___currentEnd_2)); }
	inline double get_currentEnd_2() const { return ___currentEnd_2; }
	inline double* get_address_of_currentEnd_2() { return &___currentEnd_2; }
	inline void set_currentEnd_2(double value)
	{
		___currentEnd_2 = value;
	}

	inline static int32_t get_offset_of_currentState_3() { return static_cast<int32_t>(offsetof(GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB, ___currentState_3)); }
	inline int32_t get_currentState_3() const { return ___currentState_3; }
	inline int32_t* get_address_of_currentState_3() { return &___currentState_3; }
	inline void set_currentState_3(int32_t value)
	{
		___currentState_3 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo
struct Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.LfoStateEnum DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::lfoState
	int32_t ___lfoState_0;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::phase
	double ___phase_1;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::value
	double ___value_2;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::increment
	double ___increment_3;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::frequency
	double ___frequency_4;
	// System.Double DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::depth
	double ___depth_5;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::delayTime
	int32_t ___delayTime_6;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.Generator DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::generator
	Generator_tBA37A5D5C61B631CE6CF68D692817DFEB41E2476 * ___generator_7;

public:
	inline static int32_t get_offset_of_lfoState_0() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___lfoState_0)); }
	inline int32_t get_lfoState_0() const { return ___lfoState_0; }
	inline int32_t* get_address_of_lfoState_0() { return &___lfoState_0; }
	inline void set_lfoState_0(int32_t value)
	{
		___lfoState_0 = value;
	}

	inline static int32_t get_offset_of_phase_1() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___phase_1)); }
	inline double get_phase_1() const { return ___phase_1; }
	inline double* get_address_of_phase_1() { return &___phase_1; }
	inline void set_phase_1(double value)
	{
		___phase_1 = value;
	}

	inline static int32_t get_offset_of_value_2() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___value_2)); }
	inline double get_value_2() const { return ___value_2; }
	inline double* get_address_of_value_2() { return &___value_2; }
	inline void set_value_2(double value)
	{
		___value_2 = value;
	}

	inline static int32_t get_offset_of_increment_3() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___increment_3)); }
	inline double get_increment_3() const { return ___increment_3; }
	inline double* get_address_of_increment_3() { return &___increment_3; }
	inline void set_increment_3(double value)
	{
		___increment_3 = value;
	}

	inline static int32_t get_offset_of_frequency_4() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___frequency_4)); }
	inline double get_frequency_4() const { return ___frequency_4; }
	inline double* get_address_of_frequency_4() { return &___frequency_4; }
	inline void set_frequency_4(double value)
	{
		___frequency_4 = value;
	}

	inline static int32_t get_offset_of_depth_5() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___depth_5)); }
	inline double get_depth_5() const { return ___depth_5; }
	inline double* get_address_of_depth_5() { return &___depth_5; }
	inline void set_depth_5(double value)
	{
		___depth_5 = value;
	}

	inline static int32_t get_offset_of_delayTime_6() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___delayTime_6)); }
	inline int32_t get_delayTime_6() const { return ___delayTime_6; }
	inline int32_t* get_address_of_delayTime_6() { return &___delayTime_6; }
	inline void set_delayTime_6(int32_t value)
	{
		___delayTime_6 = value;
	}

	inline static int32_t get_offset_of_generator_7() { return static_cast<int32_t>(offsetof(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B, ___generator_7)); }
	inline Generator_tBA37A5D5C61B631CE6CF68D692817DFEB41E2476 * get_generator_7() const { return ___generator_7; }
	inline Generator_tBA37A5D5C61B631CE6CF68D692817DFEB41E2476 ** get_address_of_generator_7() { return &___generator_7; }
	inline void set_generator_7(Generator_tBA37A5D5C61B631CE6CF68D692817DFEB41E2476 * value)
	{
		___generator_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___generator_7), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch
struct MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C  : public Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch/IntervalType DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch::iType
	int32_t ___iType_3;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch/PatchInterval[] DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch::intervalList
	PatchIntervalU5BU5D_tF096E636EFC61D1CB4F2B68609DB139094220647* ___intervalList_4;

public:
	inline static int32_t get_offset_of_iType_3() { return static_cast<int32_t>(offsetof(MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C, ___iType_3)); }
	inline int32_t get_iType_3() const { return ___iType_3; }
	inline int32_t* get_address_of_iType_3() { return &___iType_3; }
	inline void set_iType_3(int32_t value)
	{
		___iType_3 = value;
	}

	inline static int32_t get_offset_of_intervalList_4() { return static_cast<int32_t>(offsetof(MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C, ___intervalList_4)); }
	inline PatchIntervalU5BU5D_tF096E636EFC61D1CB4F2B68609DB139094220647* get_intervalList_4() const { return ___intervalList_4; }
	inline PatchIntervalU5BU5D_tF096E636EFC61D1CB4F2B68609DB139094220647** get_address_of_intervalList_4() { return &___intervalList_4; }
	inline void set_intervalList_4(PatchIntervalU5BU5D_tF096E636EFC61D1CB4F2B68609DB139094220647* value)
	{
		___intervalList_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___intervalList_4), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.Generator
struct Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Sf2.GeneratorEnum DaggerfallWorkshop.AudioSynthesis.Sf2.Generator::gentype
	int32_t ___gentype_0;
	// System.UInt16 DaggerfallWorkshop.AudioSynthesis.Sf2.Generator::rawAmount
	uint16_t ___rawAmount_1;

public:
	inline static int32_t get_offset_of_gentype_0() { return static_cast<int32_t>(offsetof(Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4, ___gentype_0)); }
	inline int32_t get_gentype_0() const { return ___gentype_0; }
	inline int32_t* get_address_of_gentype_0() { return &___gentype_0; }
	inline void set_gentype_0(int32_t value)
	{
		___gentype_0 = value;
	}

	inline static int32_t get_offset_of_rawAmount_1() { return static_cast<int32_t>(offsetof(Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4, ___rawAmount_1)); }
	inline uint16_t get_rawAmount_1() const { return ___rawAmount_1; }
	inline uint16_t* get_address_of_rawAmount_1() { return &___rawAmount_1; }
	inline void set_rawAmount_1(uint16_t value)
	{
		___rawAmount_1 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator
struct Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Sf2.ModulatorType DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator::sourceModulationData
	ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 * ___sourceModulationData_0;
	// DaggerfallWorkshop.AudioSynthesis.Sf2.GeneratorEnum DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator::destinationGenerator
	int32_t ___destinationGenerator_1;
	// System.Int16 DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator::amount
	int16_t ___amount_2;
	// DaggerfallWorkshop.AudioSynthesis.Sf2.ModulatorType DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator::sourceModulationAmount
	ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 * ___sourceModulationAmount_3;
	// DaggerfallWorkshop.AudioSynthesis.Sf2.TransformEnum DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator::sourceTransform
	int32_t ___sourceTransform_4;

public:
	inline static int32_t get_offset_of_sourceModulationData_0() { return static_cast<int32_t>(offsetof(Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1, ___sourceModulationData_0)); }
	inline ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 * get_sourceModulationData_0() const { return ___sourceModulationData_0; }
	inline ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 ** get_address_of_sourceModulationData_0() { return &___sourceModulationData_0; }
	inline void set_sourceModulationData_0(ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 * value)
	{
		___sourceModulationData_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___sourceModulationData_0), (void*)value);
	}

	inline static int32_t get_offset_of_destinationGenerator_1() { return static_cast<int32_t>(offsetof(Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1, ___destinationGenerator_1)); }
	inline int32_t get_destinationGenerator_1() const { return ___destinationGenerator_1; }
	inline int32_t* get_address_of_destinationGenerator_1() { return &___destinationGenerator_1; }
	inline void set_destinationGenerator_1(int32_t value)
	{
		___destinationGenerator_1 = value;
	}

	inline static int32_t get_offset_of_amount_2() { return static_cast<int32_t>(offsetof(Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1, ___amount_2)); }
	inline int16_t get_amount_2() const { return ___amount_2; }
	inline int16_t* get_address_of_amount_2() { return &___amount_2; }
	inline void set_amount_2(int16_t value)
	{
		___amount_2 = value;
	}

	inline static int32_t get_offset_of_sourceModulationAmount_3() { return static_cast<int32_t>(offsetof(Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1, ___sourceModulationAmount_3)); }
	inline ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 * get_sourceModulationAmount_3() const { return ___sourceModulationAmount_3; }
	inline ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 ** get_address_of_sourceModulationAmount_3() { return &___sourceModulationAmount_3; }
	inline void set_sourceModulationAmount_3(ModulatorType_t424BF896B0F6F9AD9D4B181D1397482674136424 * value)
	{
		___sourceModulationAmount_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___sourceModulationAmount_3), (void*)value);
	}

	inline static int32_t get_offset_of_sourceTransform_4() { return static_cast<int32_t>(offsetof(Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1, ___sourceTransform_4)); }
	inline int32_t get_sourceTransform_4() const { return ___sourceTransform_4; }
	inline int32_t* get_address_of_sourceTransform_4() { return &___sourceTransform_4; }
	inline void set_sourceTransform_4(int32_t value)
	{
		___sourceTransform_4 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer
struct Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36  : public RuntimeObject
{
public:
	// System.Single[] DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::sampleBuffer
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___sampleBuffer_17;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::voiceManager
	VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * ___voiceManager_18;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::audioChannels
	int32_t ___audioChannels_19;
	// System.Boolean DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::littleEndian
	bool ___littleEndian_20;
	// DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::bank
	PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * ___bank_21;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::sampleRate
	int32_t ___sampleRate_22;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::mainVolume
	float ___mainVolume_23;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::synthGain
	float ___synthGain_24;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::microBufferSize
	int32_t ___microBufferSize_25;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::microBufferCount
	int32_t ___microBufferCount_26;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters[] DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::synthChannels
	SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* ___synthChannels_27;
	// System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage> DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::midiEventQueue
	Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * ___midiEventQueue_28;
	// System.Int32[] DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::midiEventCounts
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___midiEventCounts_29;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch[] DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::layerList
	PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* ___layerList_30;

public:
	inline static int32_t get_offset_of_sampleBuffer_17() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___sampleBuffer_17)); }
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* get_sampleBuffer_17() const { return ___sampleBuffer_17; }
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5** get_address_of_sampleBuffer_17() { return &___sampleBuffer_17; }
	inline void set_sampleBuffer_17(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* value)
	{
		___sampleBuffer_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___sampleBuffer_17), (void*)value);
	}

	inline static int32_t get_offset_of_voiceManager_18() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___voiceManager_18)); }
	inline VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * get_voiceManager_18() const { return ___voiceManager_18; }
	inline VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 ** get_address_of_voiceManager_18() { return &___voiceManager_18; }
	inline void set_voiceManager_18(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * value)
	{
		___voiceManager_18 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___voiceManager_18), (void*)value);
	}

	inline static int32_t get_offset_of_audioChannels_19() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___audioChannels_19)); }
	inline int32_t get_audioChannels_19() const { return ___audioChannels_19; }
	inline int32_t* get_address_of_audioChannels_19() { return &___audioChannels_19; }
	inline void set_audioChannels_19(int32_t value)
	{
		___audioChannels_19 = value;
	}

	inline static int32_t get_offset_of_littleEndian_20() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___littleEndian_20)); }
	inline bool get_littleEndian_20() const { return ___littleEndian_20; }
	inline bool* get_address_of_littleEndian_20() { return &___littleEndian_20; }
	inline void set_littleEndian_20(bool value)
	{
		___littleEndian_20 = value;
	}

	inline static int32_t get_offset_of_bank_21() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___bank_21)); }
	inline PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * get_bank_21() const { return ___bank_21; }
	inline PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 ** get_address_of_bank_21() { return &___bank_21; }
	inline void set_bank_21(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * value)
	{
		___bank_21 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___bank_21), (void*)value);
	}

	inline static int32_t get_offset_of_sampleRate_22() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___sampleRate_22)); }
	inline int32_t get_sampleRate_22() const { return ___sampleRate_22; }
	inline int32_t* get_address_of_sampleRate_22() { return &___sampleRate_22; }
	inline void set_sampleRate_22(int32_t value)
	{
		___sampleRate_22 = value;
	}

	inline static int32_t get_offset_of_mainVolume_23() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___mainVolume_23)); }
	inline float get_mainVolume_23() const { return ___mainVolume_23; }
	inline float* get_address_of_mainVolume_23() { return &___mainVolume_23; }
	inline void set_mainVolume_23(float value)
	{
		___mainVolume_23 = value;
	}

	inline static int32_t get_offset_of_synthGain_24() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___synthGain_24)); }
	inline float get_synthGain_24() const { return ___synthGain_24; }
	inline float* get_address_of_synthGain_24() { return &___synthGain_24; }
	inline void set_synthGain_24(float value)
	{
		___synthGain_24 = value;
	}

	inline static int32_t get_offset_of_microBufferSize_25() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___microBufferSize_25)); }
	inline int32_t get_microBufferSize_25() const { return ___microBufferSize_25; }
	inline int32_t* get_address_of_microBufferSize_25() { return &___microBufferSize_25; }
	inline void set_microBufferSize_25(int32_t value)
	{
		___microBufferSize_25 = value;
	}

	inline static int32_t get_offset_of_microBufferCount_26() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___microBufferCount_26)); }
	inline int32_t get_microBufferCount_26() const { return ___microBufferCount_26; }
	inline int32_t* get_address_of_microBufferCount_26() { return &___microBufferCount_26; }
	inline void set_microBufferCount_26(int32_t value)
	{
		___microBufferCount_26 = value;
	}

	inline static int32_t get_offset_of_synthChannels_27() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___synthChannels_27)); }
	inline SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* get_synthChannels_27() const { return ___synthChannels_27; }
	inline SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0** get_address_of_synthChannels_27() { return &___synthChannels_27; }
	inline void set_synthChannels_27(SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* value)
	{
		___synthChannels_27 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___synthChannels_27), (void*)value);
	}

	inline static int32_t get_offset_of_midiEventQueue_28() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___midiEventQueue_28)); }
	inline Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * get_midiEventQueue_28() const { return ___midiEventQueue_28; }
	inline Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C ** get_address_of_midiEventQueue_28() { return &___midiEventQueue_28; }
	inline void set_midiEventQueue_28(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * value)
	{
		___midiEventQueue_28 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___midiEventQueue_28), (void*)value);
	}

	inline static int32_t get_offset_of_midiEventCounts_29() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___midiEventCounts_29)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get_midiEventCounts_29() const { return ___midiEventCounts_29; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of_midiEventCounts_29() { return &___midiEventCounts_29; }
	inline void set_midiEventCounts_29(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		___midiEventCounts_29 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___midiEventCounts_29), (void*)value);
	}

	inline static int32_t get_offset_of_layerList_30() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36, ___layerList_30)); }
	inline PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* get_layerList_30() const { return ___layerList_30; }
	inline PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34** get_address_of_layerList_30() { return &___layerList_30; }
	inline void set_layerList_30(PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* value)
	{
		___layerList_30 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___layerList_30), (void*)value);
	}
};

struct Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36_StaticFields
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.InterpolationEnum DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::InterpolationMode
	int32_t ___InterpolationMode_16;

public:
	inline static int32_t get_offset_of_InterpolationMode_16() { return static_cast<int32_t>(offsetof(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36_StaticFields, ___InterpolationMode_16)); }
	inline int32_t get_InterpolationMode_16() const { return ___InterpolationMode_16; }
	inline int32_t* get_address_of_InterpolationMode_16() { return &___InterpolationMode_16; }
	inline void set_InterpolationMode_16(int32_t value)
	{
		___InterpolationMode_16 = value;
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager
struct VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8  : public RuntimeObject
{
public:
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceStealEnum DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::stealingMethod
	int32_t ___stealingMethod_0;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::polyphony
	int32_t ___polyphony_1;
	// System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice> DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::freeVoices
	LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * ___freeVoices_2;
	// System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice> DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::activeVoices
	LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * ___activeVoices_3;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode[0...,0...] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::registry
	VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* ___registry_4;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::voicePool
	VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* ___voicePool_5;
	// System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode> DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::vnodes
	Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * ___vnodes_6;

public:
	inline static int32_t get_offset_of_stealingMethod_0() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___stealingMethod_0)); }
	inline int32_t get_stealingMethod_0() const { return ___stealingMethod_0; }
	inline int32_t* get_address_of_stealingMethod_0() { return &___stealingMethod_0; }
	inline void set_stealingMethod_0(int32_t value)
	{
		___stealingMethod_0 = value;
	}

	inline static int32_t get_offset_of_polyphony_1() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___polyphony_1)); }
	inline int32_t get_polyphony_1() const { return ___polyphony_1; }
	inline int32_t* get_address_of_polyphony_1() { return &___polyphony_1; }
	inline void set_polyphony_1(int32_t value)
	{
		___polyphony_1 = value;
	}

	inline static int32_t get_offset_of_freeVoices_2() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___freeVoices_2)); }
	inline LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * get_freeVoices_2() const { return ___freeVoices_2; }
	inline LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 ** get_address_of_freeVoices_2() { return &___freeVoices_2; }
	inline void set_freeVoices_2(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * value)
	{
		___freeVoices_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___freeVoices_2), (void*)value);
	}

	inline static int32_t get_offset_of_activeVoices_3() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___activeVoices_3)); }
	inline LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * get_activeVoices_3() const { return ___activeVoices_3; }
	inline LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 ** get_address_of_activeVoices_3() { return &___activeVoices_3; }
	inline void set_activeVoices_3(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * value)
	{
		___activeVoices_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___activeVoices_3), (void*)value);
	}

	inline static int32_t get_offset_of_registry_4() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___registry_4)); }
	inline VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* get_registry_4() const { return ___registry_4; }
	inline VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6** get_address_of_registry_4() { return &___registry_4; }
	inline void set_registry_4(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* value)
	{
		___registry_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___registry_4), (void*)value);
	}

	inline static int32_t get_offset_of_voicePool_5() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___voicePool_5)); }
	inline VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* get_voicePool_5() const { return ___voicePool_5; }
	inline VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF** get_address_of_voicePool_5() { return &___voicePool_5; }
	inline void set_voicePool_5(VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* value)
	{
		___voicePool_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___voicePool_5), (void*)value);
	}

	inline static int32_t get_offset_of_vnodes_6() { return static_cast<int32_t>(offsetof(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8, ___vnodes_6)); }
	inline Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * get_vnodes_6() const { return ___vnodes_6; }
	inline Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 ** get_address_of_vnodes_6() { return &___vnodes_6; }
	inline void set_vnodes_6(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * value)
	{
		___vnodes_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___vnodes_6), (void*)value);
	}
};


// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters
struct VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92  : public RuntimeObject
{
public:
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::channel
	int32_t ___channel_0;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::note
	int32_t ___note_1;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::velocity
	int32_t ___velocity_2;
	// System.Boolean DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::noteOffPending
	bool ___noteOffPending_3;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceStateEnum DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::state
	int32_t ___state_4;
	// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::pitchOffset
	int32_t ___pitchOffset_5;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::volOffset
	float ___volOffset_6;
	// System.Single[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::blockBuffer
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___blockBuffer_7;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::pData
	UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD* ___pData_8;
	// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::synthParams
	SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * ___synthParams_9;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::generatorParams
	GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8* ___generatorParams_10;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::envelopes
	EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E* ___envelopes_11;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::filters
	FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E* ___filters_12;
	// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo[] DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::lfos
	LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78* ___lfos_13;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::mix1
	float ___mix1_14;
	// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::mix2
	float ___mix2_15;

public:
	inline static int32_t get_offset_of_channel_0() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___channel_0)); }
	inline int32_t get_channel_0() const { return ___channel_0; }
	inline int32_t* get_address_of_channel_0() { return &___channel_0; }
	inline void set_channel_0(int32_t value)
	{
		___channel_0 = value;
	}

	inline static int32_t get_offset_of_note_1() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___note_1)); }
	inline int32_t get_note_1() const { return ___note_1; }
	inline int32_t* get_address_of_note_1() { return &___note_1; }
	inline void set_note_1(int32_t value)
	{
		___note_1 = value;
	}

	inline static int32_t get_offset_of_velocity_2() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___velocity_2)); }
	inline int32_t get_velocity_2() const { return ___velocity_2; }
	inline int32_t* get_address_of_velocity_2() { return &___velocity_2; }
	inline void set_velocity_2(int32_t value)
	{
		___velocity_2 = value;
	}

	inline static int32_t get_offset_of_noteOffPending_3() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___noteOffPending_3)); }
	inline bool get_noteOffPending_3() const { return ___noteOffPending_3; }
	inline bool* get_address_of_noteOffPending_3() { return &___noteOffPending_3; }
	inline void set_noteOffPending_3(bool value)
	{
		___noteOffPending_3 = value;
	}

	inline static int32_t get_offset_of_state_4() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___state_4)); }
	inline int32_t get_state_4() const { return ___state_4; }
	inline int32_t* get_address_of_state_4() { return &___state_4; }
	inline void set_state_4(int32_t value)
	{
		___state_4 = value;
	}

	inline static int32_t get_offset_of_pitchOffset_5() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___pitchOffset_5)); }
	inline int32_t get_pitchOffset_5() const { return ___pitchOffset_5; }
	inline int32_t* get_address_of_pitchOffset_5() { return &___pitchOffset_5; }
	inline void set_pitchOffset_5(int32_t value)
	{
		___pitchOffset_5 = value;
	}

	inline static int32_t get_offset_of_volOffset_6() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___volOffset_6)); }
	inline float get_volOffset_6() const { return ___volOffset_6; }
	inline float* get_address_of_volOffset_6() { return &___volOffset_6; }
	inline void set_volOffset_6(float value)
	{
		___volOffset_6 = value;
	}

	inline static int32_t get_offset_of_blockBuffer_7() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___blockBuffer_7)); }
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* get_blockBuffer_7() const { return ___blockBuffer_7; }
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5** get_address_of_blockBuffer_7() { return &___blockBuffer_7; }
	inline void set_blockBuffer_7(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* value)
	{
		___blockBuffer_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___blockBuffer_7), (void*)value);
	}

	inline static int32_t get_offset_of_pData_8() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___pData_8)); }
	inline UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD* get_pData_8() const { return ___pData_8; }
	inline UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD** get_address_of_pData_8() { return &___pData_8; }
	inline void set_pData_8(UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD* value)
	{
		___pData_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___pData_8), (void*)value);
	}

	inline static int32_t get_offset_of_synthParams_9() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___synthParams_9)); }
	inline SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * get_synthParams_9() const { return ___synthParams_9; }
	inline SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 ** get_address_of_synthParams_9() { return &___synthParams_9; }
	inline void set_synthParams_9(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * value)
	{
		___synthParams_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___synthParams_9), (void*)value);
	}

	inline static int32_t get_offset_of_generatorParams_10() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___generatorParams_10)); }
	inline GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8* get_generatorParams_10() const { return ___generatorParams_10; }
	inline GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8** get_address_of_generatorParams_10() { return &___generatorParams_10; }
	inline void set_generatorParams_10(GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8* value)
	{
		___generatorParams_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___generatorParams_10), (void*)value);
	}

	inline static int32_t get_offset_of_envelopes_11() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___envelopes_11)); }
	inline EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E* get_envelopes_11() const { return ___envelopes_11; }
	inline EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E** get_address_of_envelopes_11() { return &___envelopes_11; }
	inline void set_envelopes_11(EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E* value)
	{
		___envelopes_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___envelopes_11), (void*)value);
	}

	inline static int32_t get_offset_of_filters_12() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___filters_12)); }
	inline FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E* get_filters_12() const { return ___filters_12; }
	inline FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E** get_address_of_filters_12() { return &___filters_12; }
	inline void set_filters_12(FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E* value)
	{
		___filters_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___filters_12), (void*)value);
	}

	inline static int32_t get_offset_of_lfos_13() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___lfos_13)); }
	inline LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78* get_lfos_13() const { return ___lfos_13; }
	inline LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78** get_address_of_lfos_13() { return &___lfos_13; }
	inline void set_lfos_13(LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78* value)
	{
		___lfos_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___lfos_13), (void*)value);
	}

	inline static int32_t get_offset_of_mix1_14() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___mix1_14)); }
	inline float get_mix1_14() const { return ___mix1_14; }
	inline float* get_address_of_mix1_14() { return &___mix1_14; }
	inline void set_mix1_14(float value)
	{
		___mix1_14 = value;
	}

	inline static int32_t get_offset_of_mix2_15() { return static_cast<int32_t>(offsetof(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92, ___mix2_15)); }
	inline float get_mix2_15() const { return ___mix2_15; }
	inline float* get_address_of_mix2_15() { return &___mix2_15; }
	inline void set_mix2_15(float value)
	{
		___mix2_15 = value;
	}
};


// System.SystemException
struct SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782  : public Exception_t
{
public:

public:
};


// System.ArgumentException
struct ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1  : public SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782
{
public:
	// System.String System.ArgumentException::m_paramName
	String_t* ___m_paramName_17;

public:
	inline static int32_t get_offset_of_m_paramName_17() { return static_cast<int32_t>(offsetof(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1, ___m_paramName_17)); }
	inline String_t* get_m_paramName_17() const { return ___m_paramName_17; }
	inline String_t** get_address_of_m_paramName_17() { return &___m_paramName_17; }
	inline void set_m_paramName_17(String_t* value)
	{
		___m_paramName_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_paramName_17), (void*)value);
	}
};


// System.NotSupportedException
struct NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010  : public SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782
{
public:

public:
};


// System.ArgumentNullException
struct ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD  : public ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// System.Byte[]
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) uint8_t m_Items[1];

public:
	inline uint8_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline uint8_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, uint8_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline uint8_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline uint8_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, uint8_t value)
	{
		m_Items[index] = value;
	}
};
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Il2CppChar m_Items[1];

public:
	inline Il2CppChar GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Il2CppChar* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Il2CppChar value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline Il2CppChar GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Il2CppChar* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Il2CppChar value)
	{
		m_Items[index] = value;
	}
};
// DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator[]
struct ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 * m_Items[1];

public:
	inline Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Modulator_tC69474D5FA4E280EBC7DAA4038F83D9CB026F3C1 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Sf2.Generator[]
struct GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 * m_Items[1];

public:
	inline Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Generator_t8EDE62561F27593F104F6EB50E6A17E176EE83D4 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) RuntimeObject * m_Items[1];

public:
	inline RuntimeObject * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline RuntimeObject ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, RuntimeObject * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline RuntimeObject * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline RuntimeObject ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, RuntimeObject * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Double[]
struct DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) double m_Items[1];

public:
	inline double GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline double* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, double value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline double GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline double* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, double value)
	{
		m_Items[index] = value;
	}
};
// System.Single[]
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) float m_Items[1];

public:
	inline float GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline float* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, float value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline float GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline float* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, float value)
	{
		m_Items[index] = value;
	}
};
// DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters[]
struct SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * m_Items[1];

public:
	inline SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int32_t m_Items[1];

public:
	inline int32_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int32_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int32_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int32_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int32_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int32_t value)
	{
		m_Items[index] = value;
	}
};
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch[]
struct PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * m_Items[1];

public:
	inline Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode[,]
struct VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * m_Items[1];

public:
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * GetAt(il2cpp_array_size_t i, il2cpp_array_size_t j) const
	{
		il2cpp_array_size_t iBound = bounds[0].length;
		IL2CPP_ARRAY_BOUNDS_CHECK(i, iBound);
		il2cpp_array_size_t jBound = bounds[1].length;
		IL2CPP_ARRAY_BOUNDS_CHECK(j, jBound);

		il2cpp_array_size_t index = i * jBound + j;
		return m_Items[index];
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** GetAddressAt(il2cpp_array_size_t i, il2cpp_array_size_t j)
	{
		il2cpp_array_size_t iBound = bounds[0].length;
		IL2CPP_ARRAY_BOUNDS_CHECK(i, iBound);
		il2cpp_array_size_t jBound = bounds[1].length;
		IL2CPP_ARRAY_BOUNDS_CHECK(j, jBound);

		il2cpp_array_size_t index = i * jBound + j;
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t i, il2cpp_array_size_t j, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		il2cpp_array_size_t iBound = bounds[0].length;
		IL2CPP_ARRAY_BOUNDS_CHECK(i, iBound);
		il2cpp_array_size_t jBound = bounds[1].length;
		IL2CPP_ARRAY_BOUNDS_CHECK(j, jBound);

		il2cpp_array_size_t index = i * jBound + j;
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * GetAtUnchecked(il2cpp_array_size_t i, il2cpp_array_size_t j) const
	{
		il2cpp_array_size_t jBound = bounds[1].length;

		il2cpp_array_size_t index = i * jBound + j;
		return m_Items[index];
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** GetAddressAtUnchecked(il2cpp_array_size_t i, il2cpp_array_size_t j)
	{
		il2cpp_array_size_t jBound = bounds[1].length;

		il2cpp_array_size_t index = i * jBound + j;
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t i, il2cpp_array_size_t j, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		il2cpp_array_size_t jBound = bounds[1].length;

		il2cpp_array_size_t index = i * jBound + j;
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode[]
struct VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * m_Items[1];

public:
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice[]
struct VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Voice_t481B233F7BCA5C28D192670FC7590699211A984E * m_Items[1];

public:
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Synthesis.UnionData[]
struct UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079  m_Items[1];

public:
	inline UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, UnionData_t2C2D32ABEBDA12259FB251FD481E237D675CA079  value)
	{
		m_Items[index] = value;
	}
};
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters[]
struct GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * m_Items[1];

public:
	inline GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope[]
struct EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * m_Items[1];

public:
	inline Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter[]
struct FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * m_Items[1];

public:
	inline Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo[]
struct LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * m_Items[1];

public:
	inline Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Single[][]
struct SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* m_Items[1];

public:
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


// System.Void System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Queue_1__ctor_mB4BC9CA4838CC532EBAA314118FF6116CCF76208_gshared (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>::get_Count()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_gshared_inline (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>::Dequeue()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484  Queue_1_Dequeue_mC42CAF829668D9D8CFB8B95046A7450284564FCC_gshared (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method);
// System.Collections.Generic.LinkedListNode`1<!0> System.Collections.Generic.LinkedList`1<System.Object>::get_First()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * LinkedList_1_get_First_m0C98E2DE4C013B92EDF858C9A5DEA9A30BB5523C_gshared_inline (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.LinkedListNode`1<System.Object>::get_Value()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR RuntimeObject * LinkedListNode_1_get_Value_m36A53343597D289FE50219266EDE929003F0EA89_gshared_inline (LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * __this, const RuntimeMethod* method);
// System.Collections.Generic.LinkedListNode`1<!0> System.Collections.Generic.LinkedListNode`1<System.Object>::get_Next()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * LinkedListNode_1_get_Next_mB33EA4DCE8E0BF4A4E30EDBB6941BF58970EF6A3_gshared (LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.LinkedList`1<System.Object>::Remove(System.Collections.Generic.LinkedListNode`1<!0>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LinkedList_1_Remove_mADE035B335CACEA3A43B1A81CC4A837E23372398_gshared (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * ___node0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.LinkedList`1<System.Object>::AddFirst(System.Collections.Generic.LinkedListNode`1<!0>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LinkedList_1_AddFirst_m2EFB30065D70A52B789D17A6D0177B9013D3BDFC_gshared (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * ___node0, const RuntimeMethod* method);
// System.Collections.Generic.LinkedListNode`1<!0> System.Collections.Generic.LinkedList`1<System.Object>::AddLast(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * LinkedList_1_AddLast_m968B782331A31FE20156A13687378A375B788568_gshared (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, RuntimeObject * ___value0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Stack`1<System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<!0>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Stack_1__ctor_m2999EF1E4A39F76F1350B1C76F39AFF975AB8E8F_gshared (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * __this, RuntimeObject* ___collection0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.LinkedList`1<System.Object>::.ctor(System.Collections.Generic.IEnumerable`1<!0>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LinkedList_1__ctor_m9973BCF825BA7151FC101022AAD2D0C13AE01C42_gshared (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, RuntimeObject* ___collection0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.LinkedList`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LinkedList_1__ctor_mAB175C80A916D8714D714BBC61066B970B47982E_gshared (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.LinkedList`1<System.Object>::get_Count()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t LinkedList_1_get_Count_m3FEDB19F06F4B650469DB1D5D2308832AC52B75D_gshared_inline (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.LinkedList`1<System.Object>::RemoveFirst()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LinkedList_1_RemoveFirst_m4E30C722E5D186139A990A279A2E7EC8AF8BBEFE_gshared (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.Stack`1<System.Object>::Pop()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * Stack_1_Pop_mD632EB4DA13E5CAEC62EECFAD1C88818F1223E20_gshared (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Stack`1<System.Object>::Push(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Stack_1_Push_mB892D933D8982A0702F4E09E2F0D7B0C33E2A4E1_gshared (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * __this, RuntimeObject * ___item0, const RuntimeMethod* method);
// System.Collections.Generic.Stack`1/Enumerator<!0> System.Collections.Generic.Stack`1<System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A  Stack_1_GetEnumerator_mA688333716057A61012D8BA8F4D8A24119D437EF_gshared (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.Stack`1/Enumerator<System.Object>::get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * Enumerator_get_Current_m33423473ED484E78201D81B5BD13722D3764B7B7_gshared (Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Stack`1/Enumerator<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_mA1442677307840443C164BF55B0FC8553A08BD67_gshared (Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Stack`1/Enumerator<System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_m7A23151CFECC40C6CF59700964B660A4D0402406_gshared (Enumerator_t85617C20911920C700367CCF021556DD3A84EA4A * __this, const RuntimeMethod* method);

// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Char[] DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::Read8BitChars(System.IO.BinaryReader,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031 (BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader0, int32_t ___length1, const RuntimeMethod* method);
// System.String System.String::CreateString(System.Char[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_CreateString_m394C06654854ADD4C51FF957BE0CC72EF52BAA96 (String_t* __this, CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___val0, const RuntimeMethod* method);
// System.String System.String::ToLower()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_ToLower_m5287204D93C9DDC4DF84581ADD756D0FDE2BA5A8 (String_t* __this, const RuntimeMethod* method);
// System.Boolean System.String::Equals(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_Equals_m9C4D78DFA0979504FE31429B64A4C26DF48020D1 (String_t* __this, String_t* ___value0, const RuntimeMethod* method);
// System.Void System.Exception::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0 (Exception_t * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Boolean System.String::op_Equality(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE (String_t* ___a0, String_t* ___b1, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mF4626905368D6558695A823466A1AF65EADB9923 (String_t* ___str00, String_t* ___str11, String_t* ___str22, const RuntimeMethod* method);
// System.Void System.NotSupportedException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NotSupportedException__ctor_mD023A89A5C1F740F43F0A9CD6C49DC21230B3CEE (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * __this, String_t* ___message0, const RuntimeMethod* method);
// System.String System.String::Format(System.String,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_m19325298DBC61AAC016C16F7B3CF97A8A3DEA34A (String_t* ___format0, RuntimeObject * ___arg01, RuntimeObject * ___arg12, const RuntimeMethod* method);
// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::get_Coarse()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t CCValue_get_Coarse_m058A8FAA33C4090561A2EF1A5F08E1587D5C8352 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::UpdateCombined()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::set_Coarse(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, uint8_t ___value0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::set_Fine(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, uint8_t ___value0, const RuntimeMethod* method);
// System.Int16 DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::get_Combined()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::UpdateCoarseFinePair()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::set_Combined(System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, int16_t ___value0, const RuntimeMethod* method);
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* CCValue_ToString_mBFE7FE09697E670A95307955D76D85473138EA91 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::.ctor(System.Int32,System.Byte,System.Byte,System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MidiMessage__ctor_m71B14656607C7B84FA2E3F3FB461F94A591E7765 (MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * __this, int32_t ___delta0, uint8_t ___channel1, uint8_t ___command2, uint8_t ___data13, uint8_t ___data24, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::.ctor(System.Byte,System.Byte,System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MidiMessage__ctor_m53974E30EEF89E43E22A772691EA3E63409E7FB3 (MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * __this, uint8_t ___channel0, uint8_t ___command1, uint8_t ___data12, uint8_t ___data23, const RuntimeMethod* method);
// System.String System.String::Format(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_mA3AC3FE7B23D97F3A5BAA082D25B0E01B341A865 (String_t* ___format0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MidiMessage_ToString_mDBEC05B0D9113B1A597467CCE6B8D42B79CA179C (MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * __this, const RuntimeMethod* method);
// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::Clamp(System.Single,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float SynthHelper_Clamp_m5647B8E90763F524A8AA958F061A8C96C2B9423B (float ___value0, float ___min1, float ___max2, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent::.ctor(System.Single,DaggerfallWorkshop.AudioSynthesis.Synthesis.PanFormulaEnum)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B (PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * __this, float ___value0, int32_t ___formula1, const RuntimeMethod* method);
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* PanComponent_ToString_mCE8FA3534B904E393E7D21CCCCEF9B0286BFA123 (PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * __this, const RuntimeMethod* method);
// System.Double System.Math::Pow(System.Double,System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double Math_Pow_m9CD842663B1A2FA4C66EEFFC6F0D705B40BE46F1 (double ___x0, double ___y1, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::ResetControllers()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_ResetControllers_mCE6BAEAB27EA90A2D744C8B30D15C5E0D32D2CAA (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentPan()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentPitch()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentVolume()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentVolume_m2BDC5BDA381F6ECD8778AD86BE1B8DEB712FA4C6 (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method);
// System.Void System.ArgumentException::.ctor(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m26DC3463C6F3C98BF33EA39598DD2B32F0249CA8 (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * __this, String_t* ___message0, String_t* ___paramName1, const RuntimeMethod* method);
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::Clamp(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SynthHelper_Clamp_m767D40B465457BDAFF1CB5EF2D2D0428E4B40A98 (int32_t ___value0, int32_t ___min1, int32_t ___max2, const RuntimeMethod* method);
// System.Int32 System.Math::Max(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Math_Max_mA99E48BB021F2E4B62D4EA9F52EA6928EED618A2 (int32_t ___val10, int32_t ___val21, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::.ctor(DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters__ctor_mF07CCFE15DD33513094F4EBC67A7D0F1E2870BE0 (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * ___synth0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager__ctor_mE544FA76F2141F9F6BE6FD198686AD516D74378D (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, int32_t ___voiceCount0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>::.ctor()
inline void Queue_1__ctor_mB4BC9CA4838CC532EBAA314118FF6116CCF76208 (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method)
{
	((  void (*) (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C *, const RuntimeMethod*))Queue_1__ctor_mB4BC9CA4838CC532EBAA314118FF6116CCF76208_gshared)(__this, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::.ctor(DaggerfallWorkshop.AudioSynthesis.IResource)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PatchBank__ctor_m2B2D79BB0EB7A7E80C4BC4A73E22A42996F3EE5D (PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * __this, RuntimeObject* ___bankFile0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::LoadBank(DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * ___bank0, const RuntimeMethod* method);
// System.Void System.ArgumentNullException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentNullException__ctor_mEE0C0D6FCB2D08CD7967DBB1329A0854BBED49ED (ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD * __this, String_t* ___paramName0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::UnloadBank()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_UnloadBank_mD52E476F76A6B446A5C9F01C4C633695FC7A4E10 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::NoteOffAll(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, bool ___immediate0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::UnloadPatches()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_UnloadPatches_m0B094424DF30EDE3C004B6EDFB7C97A60F9C32CB (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ReleaseAllHoldPedals()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ReleaseAllHoldPedals_m09DA747E7D9AD591C391C8D51D0FF01D04F2ECB7 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method);
// System.Void System.Array::Clear(System.Array,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_Clear_m174F4957D6DEDB6359835123005304B14E79132E (RuntimeArray * ___array0, int32_t ___index1, int32_t ___length2, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::FillWorkingBuffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_FillWorkingBuffer_mA17FA0EBD99D7F5C5C236297A6DFC8460B437957 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ConvertWorkingBuffer(System.Single[],System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ConvertWorkingBuffer_m906FF4E3B635EBCFF45FD5205902E442F17DB93A (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___to0, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___from1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>::get_Count()
inline int32_t Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_inline (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C *, const RuntimeMethod*))Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_gshared_inline)(__this, method);
}
// !0 System.Collections.Generic.Queue`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage>::Dequeue()
inline MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484  Queue_1_Dequeue_mC42CAF829668D9D8CFB8B95046A7450284564FCC (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method)
{
	return ((  MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484  (*) (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C *, const RuntimeMethod*))Queue_1_Dequeue_mC42CAF829668D9D8CFB8B95046A7450284564FCC_gshared)(__this, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ProcessMidiMessage(System.Int32,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ProcessMidiMessage_mFE72104144D15B8AD2A7354357B4EE8312CA0887 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, int32_t ___command1, int32_t ___data12, int32_t ___data23, const RuntimeMethod* method);
// System.Collections.Generic.LinkedListNode`1<!0> System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::get_First()
inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, const RuntimeMethod* method)
{
	return ((  LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, const RuntimeMethod*))LinkedList_1_get_First_m0C98E2DE4C013B92EDF858C9A5DEA9A30BB5523C_gshared_inline)(__this, method);
}
// !0 System.Collections.Generic.LinkedListNode`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::get_Value()
inline Voice_t481B233F7BCA5C28D192670FC7590699211A984E * LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline (LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * __this, const RuntimeMethod* method)
{
	return ((  Voice_t481B233F7BCA5C28D192670FC7590699211A984E * (*) (LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *, const RuntimeMethod*))LinkedListNode_1_get_Value_m36A53343597D289FE50219266EDE929003F0EA89_gshared_inline)(__this, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Process(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Process_m00ABF34E8418905D5F60FEB40A1B3184E69E3291 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, int32_t ___startIndex0, int32_t ___endIndex1, const RuntimeMethod* method);
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::get_VoiceParams()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method);
// System.Collections.Generic.LinkedListNode`1<!0> System.Collections.Generic.LinkedListNode`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::get_Next()
inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518 (LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * __this, const RuntimeMethod* method)
{
	return ((  LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * (*) (LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *, const RuntimeMethod*))LinkedListNode_1_get_Next_mB33EA4DCE8E0BF4A4E30EDBB6941BF58970EF6A3_gshared)(__this, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::RemoveFromRegistry(DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___voice0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::Remove(System.Collections.Generic.LinkedListNode`1<!0>)
inline void LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6 (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * ___node0, const RuntimeMethod* method)
{
	((  void (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *, const RuntimeMethod*))LinkedList_1_Remove_mADE035B335CACEA3A43B1A81CC4A837E23372398_gshared)(__this, ___node0, method);
}
// System.Void System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::AddFirst(System.Collections.Generic.LinkedListNode`1<!0>)
inline void LinkedList_1_AddFirst_mF0C1BDD5A5B6AE94C12282626D65C8696B3D6CC7 (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * ___node0, const RuntimeMethod* method)
{
	((  void (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *, const RuntimeMethod*))LinkedList_1_AddFirst_m2EFB30065D70A52B789D17A6D0177B9013D3BDFC_gshared)(__this, ___node0, method);
}
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::GetPatch(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * PatchBank_GetPatch_mF7D10E9652F19500B38F82A1BBA0799DF7C6318C (PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * __this, int32_t ___bankNumber0, int32_t ___patchNumber1, const RuntimeMethod* method);
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Patches.MultiPatch::FindPatches(System.Int32,System.Int32,System.Int32,DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MultiPatch_FindPatches_mFEACBF85B11F80A453E48DDFDB9E23C6459F673F (MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C * __this, int32_t ___channel0, int32_t ___key1, int32_t ___velocity2, PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* ___layers3, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Stop()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::RemoveFromRegistry(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_RemoveFromRegistry_mF4C348706F109014BC7594B960D24307B9539718 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, int32_t ___channel0, int32_t ___note1, const RuntimeMethod* method);
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::get_ExclusiveGroupTarget()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Patch_get_ExclusiveGroupTarget_m5369FE534BE7BDCB4B057B7B4CD570BE3FC47146 (Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * __this, const RuntimeMethod* method);
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::get_Patch()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * Voice_get_Patch_mD8E479EBB586F5AB6A190C02D1F990CE9083A5D1 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method);
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::get_ExclusiveGroup()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Patch_get_ExclusiveGroup_mA5E49C9E9B97AE3C523236194A17CD3C8937AAF8 (Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * __this, const RuntimeMethod* method);
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::GetFreeVoice()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Voice_t481B233F7BCA5C28D192670FC7590699211A984E * VoiceManager_GetFreeVoice_mFCEE90FF0C773A45F0DE6AF229D2F7AA1B88639E (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Configure(System.Int32,System.Int32,System.Int32,DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch,DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Configure_m1E88AADA9F95F0F6D3F9612100614B0CE5A91AD0 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, int32_t ___channel0, int32_t ___note1, int32_t ___velocity2, Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * ___patch3, SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * ___synthParams4, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::AddToRegistry(DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_AddToRegistry_m7B92602F3DD706215F8B2549A2E206E46D9E213C (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___voice0, const RuntimeMethod* method);
// System.Collections.Generic.LinkedListNode`1<!0> System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::AddLast(!0)
inline LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * LinkedList_1_AddLast_m726CDCE67E7FFAED24B9764BA1413870450FAA03 (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___value0, const RuntimeMethod* method)
{
	return ((  LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, Voice_t481B233F7BCA5C28D192670FC7590699211A984E *, const RuntimeMethod*))LinkedList_1_AddLast_m968B782331A31FE20156A13687378A375B788568_gshared)(__this, ___value0, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Start()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Start_mA06B8A91978ADEE54A6A942C1D400C628D2F1EED (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::ClearRegistry()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_ClearRegistry_m3E58BBD848C9B941AC9C3B038A5E3DBAAD552C34 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::StopImmediately()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_StopImmediately_m87339619572893D9AB49AB113BF63EC10C17DF17 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::NoteOff(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_NoteOff_mF877C7142704DC64276849E65E343B59172C2670 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, int32_t ___note1, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::NoteOn(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_NoteOn_mED47CC4A3C3105E86AF9A299B0824D5425499C3F (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, int32_t ___note1, int32_t ___velocity2, const RuntimeMethod* method);
// System.Boolean DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank::IsBankLoaded(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PatchBank_IsBankLoaded_m75149C89714DB65B99E44CCBE6C75EF07FAAE142 (PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * __this, int32_t ___bankNumber0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentMod()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentMod_mA575ADC4219635E77436872A0D375E5B93B9F8DE (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ReleaseHoldPedal(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ReleaseHoldPedal_mE43D3DCBF5F4728BEC6E8F778D333DFA080E105C (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceParameters__ctor_m0F6140BF4B6EAE4D1A03A011E048C662E8DBA418 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceParameters_Reset_mAA57CF4C4E6F6D8896958323E4E820258AC4B4B2 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method);
// System.String DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::get_Name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Patch_get_Name_mFD124ADFA88F426F53FF564A68159A8570284CE0 (Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice__ctor_m11F21BF3B861E923059674538CCAB852CE7396DF (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceNode__ctor_mF4DA85DA34F70AAEBFC3780DFF3B87A01D57BAE5 (VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::.ctor(System.Collections.Generic.IEnumerable`1<!0>)
inline void Stack_1__ctor_m6A17077686BD0E0D90E5FF918CEDB67A998021F6 (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * __this, RuntimeObject* ___collection0, const RuntimeMethod* method)
{
	((  void (*) (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 *, RuntimeObject*, const RuntimeMethod*))Stack_1__ctor_m2999EF1E4A39F76F1350B1C76F39AFF975AB8E8F_gshared)(__this, ___collection0, method);
}
// System.Void System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::.ctor(System.Collections.Generic.IEnumerable`1<!0>)
inline void LinkedList_1__ctor_m9B300260CEDD4F71EE5EB13A29F5A1DB7326EF83 (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, RuntimeObject* ___collection0, const RuntimeMethod* method)
{
	((  void (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, RuntimeObject*, const RuntimeMethod*))LinkedList_1__ctor_m9973BCF825BA7151FC101022AAD2D0C13AE01C42_gshared)(__this, ___collection0, method);
}
// System.Void System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::.ctor()
inline void LinkedList_1__ctor_m351D42809C36084085287A7DC9266C18A38EBCBD (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, const RuntimeMethod* method)
{
	((  void (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, const RuntimeMethod*))LinkedList_1__ctor_mAB175C80A916D8714D714BBC61066B970B47982E_gshared)(__this, method);
}
// System.Int32 System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::get_Count()
inline int32_t LinkedList_1_get_Count_mB6578B261B94BF2CA14E35C3F412FAC44697646E_inline (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, const RuntimeMethod*))LinkedList_1_get_Count_m3FEDB19F06F4B650469DB1D5D2308832AC52B75D_gshared_inline)(__this, method);
}
// System.Void System.Collections.Generic.LinkedList`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice>::RemoveFirst()
inline void LinkedList_1_RemoveFirst_m38227EF51CB2AC5FD93DD996AFAB4B2AA0A4775F (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * __this, const RuntimeMethod* method)
{
	((  void (*) (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *, const RuntimeMethod*))LinkedList_1_RemoveFirst_m4E30C722E5D186139A990A279A2E7EC8AF8BBEFE_gshared)(__this, method);
}
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::StealOldest()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Voice_t481B233F7BCA5C28D192670FC7590699211A984E * VoiceManager_StealOldest_mA7E0A19BA1AA22AAA607A495B58E0BB26801F57D (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method);
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::StealQuietestVoice()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Voice_t481B233F7BCA5C28D192670FC7590699211A984E * VoiceManager_StealQuietestVoice_m7D807BDC795448730B4C6BF94C2D02A424559783 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::Pop()
inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * Stack_1_Pop_mC9241F45FA4B326F497400A6638358BB20C79648 (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * __this, const RuntimeMethod* method)
{
	return ((  VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * (*) (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 *, const RuntimeMethod*))Stack_1_Pop_mD632EB4DA13E5CAEC62EECFAD1C88818F1223E20_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::Push(!0)
inline void Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * __this, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * ___item0, const RuntimeMethod* method)
{
	((  void (*) (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 *, VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *, const RuntimeMethod*))Stack_1_Push_mB892D933D8982A0702F4E09E2F0D7B0C33E2A4E1_gshared)(__this, ___item0, method);
}
// System.Collections.Generic.Stack`1/Enumerator<!0> System.Collections.Generic.Stack`1<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::GetEnumerator()
inline Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7  Stack_1_GetEnumerator_m75E27371A4143B1C392DFB79E958DA67D10DB975 (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7  (*) (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 *, const RuntimeMethod*))Stack_1_GetEnumerator_mA688333716057A61012D8BA8F4D8A24119D437EF_gshared)(__this, method);
}
// !0 System.Collections.Generic.Stack`1/Enumerator<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::get_Current()
inline VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * Enumerator_get_Current_m4592C40C9A75A2BBD09370ADA41D658FE1CD0032 (Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 * __this, const RuntimeMethod* method)
{
	return ((  VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * (*) (Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 *, const RuntimeMethod*))Enumerator_get_Current_m33423473ED484E78201D81B5BD13722D3764B7B7_gshared)(__this, method);
}
// System.Boolean System.Collections.Generic.Stack`1/Enumerator<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::MoveNext()
inline bool Enumerator_MoveNext_mA3F181BF1A1468DD6B9F91E3627B9BE3811B8BAF (Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 *, const RuntimeMethod*))Enumerator_MoveNext_mA1442677307840443C164BF55B0FC8553A08BD67_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Stack`1/Enumerator<DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode>::Dispose()
inline void Enumerator_Dispose_mA5A2805F07E244CB7DC592F13D3FCD04AC6BE726 (Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 *, const RuntimeMethod*))Enumerator_Dispose_m7A23151CFECC40C6CF59700964B660A4D0402406_gshared)(__this, method);
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::get_CombinedVolume()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float VoiceParameters_get_CombinedVolume_m3B5EA828CEAD322A9DD4BBF19FEEAD6F950ED6F0 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Bank.Components.Generators.GeneratorParameters::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GeneratorParameters__ctor_m6B001B6B0B1FA64E3B5B229C27E5F68EFC6AAAC7 (GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Bank.Components.Envelope::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Envelope__ctor_m737B844AF5B88FFA81328A5CA55A870BA888543F (Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Bank.Components.Filter::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Filter__ctor_m5B576C2DFA9D8809F2575B66F8765C4A3A5EE522 (Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * __this, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Bank.Components.Lfo::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Lfo__ctor_m4B9AEC7B2766FA0E40133C110B424B1CD623C188 (Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * __this, const RuntimeMethod* method);
// System.Int32 System.String::IndexOf(System.Char)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t String_IndexOf_m2909B8CF585E1BD0C81E11ACA2F48012156FD5BD (String_t* __this, Il2CppChar ___value0, const RuntimeMethod* method);
// System.String System.String::Remove(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Remove_mEB092613182657B160E4BC9587D71A9CF639AD8C (String_t* __this, int32_t ___startIndex0, const RuntimeMethod* method);
// System.Int32 System.String::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline (String_t* __this, const RuntimeMethod* method);
// System.Char System.String::get_Chars(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96 (String_t* __this, int32_t ___index0, const RuntimeMethod* method);
// System.String System.String::Substring(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Substring_m2C4AFF5E79DD8BADFD2DFBCF156BF728FBB8E1AE (String_t* __this, int32_t ___startIndex0, const RuntimeMethod* method);
// System.String DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::GetFileNameWithExtension(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* IOHelper_GetFileNameWithExtension_m2CD927C08AE14119DA36A77921B0BC48CA830A26 (String_t* ___fileName0, const RuntimeMethod* method);
// System.String System.String::Substring(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Substring_mB593C0A320C683E6E47EFFC0A12B7A465E5E43BB (String_t* __this, int32_t ___startIndex0, int32_t ___length1, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Util.Riff.Chunk::.ctor(System.String,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Chunk__ctor_m1EF9CA04B86EE77E27D24BD0164E6F521EA5F896 (Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15 * __this, String_t* ___id0, int32_t ___size1, const RuntimeMethod* method);
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateSustainTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateSustainTable_mCC57225D8466B310B618EB3E4E3303F685BFC627 (int32_t ___size0, const RuntimeMethod* method);
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::RemoveDenormals(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746 (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, const RuntimeMethod* method);
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateLinearTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateLinearTable_m62A5E3D5CE0F0D8F9F496A261468BE64E7B8FE40 (int32_t ___size0, const RuntimeMethod* method);
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateConcaveTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateConcaveTable_m5544B299F299F355FB58B93619127874CCDB06CA (int32_t ___size0, const RuntimeMethod* method);
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateConvexTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateConvexTable_m8CEBC09758BE247C3EBE23F03D1C6A6D39AA520B (int32_t ___size0, const RuntimeMethod* method);
// System.Double[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateCentTable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* Tables_CreateCentTable_m372317857C669E4D43EBBD028DB2E62BCC6CC50F (const RuntimeMethod* method);
// System.Double[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateSemitoneTable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* Tables_CreateSemitoneTable_mA5A66F789BEC9D3168CC8AE2BA805F8309441AC8 (const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.WaveHelper::SwapEndianess(System.Byte[],System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WaveHelper_SwapEndianess_mB4C9A2AC9A10F41C2F9BDC11EAFD474EF1386845 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___data0, int32_t ___bits1, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData8Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData8Bit__ctor_mD8EE69468C091A19FDC95B6456B20DB18DE0C53A (PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965 * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData16Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData16Bit__ctor_m8185692C2B4259CF6558004F94436054E8207DD1 (PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2 * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData24Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData24Bit__ctor_mB2218C1B89BBA0AFF0DD8284097D35FAAA62B916 (PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData32Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData32Bit__ctor_mA1F9B042EAE8C52FA58C4A1DEA4849A37791D3F3 (PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7 * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC (RuntimeObject * ___arg00, RuntimeObject * ___arg11, RuntimeObject * ___arg22, const RuntimeMethod* method);
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561 (PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method);
// System.Int32 System.Math::Min(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Math_Min_mC950438198519FB2B0260FCB91220847EE4BB525 (int32_t ___val10, int32_t ___val21, const RuntimeMethod* method);
// System.Void System.Array::Copy(System.Array,System.Int32,System.Array,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_Copy_mA10D079DD8D9700CA44721A219A934A2397653F6 (RuntimeArray * ___sourceArray0, int32_t ___sourceIndex1, RuntimeArray * ___destinationArray2, int32_t ___destinationIndex3, int32_t ___length4, const RuntimeMethod* method);
// System.Void System.Array::Reverse(System.Array)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_Reverse_mF6A81D8EC8E17D7B3BE5F9B4EE763E3D43E57440 (RuntimeArray * ___array0, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData::get_BitsPerSample()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SoundFontSampleData_get_BitsPerSample_m541E7481FAC2237F0775D1B6DF6C970D4E6CE92D (SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_bitsPerSample_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Byte[] DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData::get_SampleData()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* SoundFontSampleData_get_SampleData_mD31A21A5A7198139467B9568DCE5D316087B6628 (SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9 * __this, const RuntimeMethod* method)
{
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_0 = NULL;
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = __this->get_samples_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Sf2.SoundFontSampleData::.ctor(System.IO.BinaryReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED (SoundFontSampleData_t081444BB2BEE35F6AA012010F80AAE1FA712B1D9 * __this, BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int64_t V_0 = 0;
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_1 = NULL;
	String_t* V_2 = NULL;
	int32_t V_3 = 0;
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	bool V_6 = false;
	String_t* V_7 = NULL;
	int32_t G_B13_0 = 0;
	int32_t G_B22_0 = 0;
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_0 = ___reader0;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_1 = IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031(L_0, 4, /*hidden argument*/NULL);
		String_t* L_2 = String_CreateString_m394C06654854ADD4C51FF957BE0CC72EF52BAA96(NULL, L_1, /*hidden argument*/NULL);
		NullCheck(L_2);
		String_t* L_3 = String_ToLower_m5287204D93C9DDC4DF84581ADD756D0FDE2BA5A8(L_2, /*hidden argument*/NULL);
		NullCheck(L_3);
		bool L_4 = String_Equals_m9C4D78DFA0979504FE31429B64A4C26DF48020D1(L_3, _stringLiteral38B62BE4BDDAA5661C7D6B8E36E28159314DF5C7, /*hidden argument*/NULL);
		V_6 = L_4;
		bool L_5 = V_6;
		if (L_5)
		{
			goto IL_0034;
		}
	}
	{
		Exception_t * L_6 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_6, _stringLiteral703288E4EE95465B5CCF9AABCA87590C977C799A, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_6, SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_RuntimeMethod_var);
	}

IL_0034:
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_7 = ___reader0;
		NullCheck(L_7);
		int32_t L_8 = VirtFuncInvoker0< int32_t >::Invoke(16 /* System.Int32 System.IO.BinaryReader::ReadInt32() */, L_7);
		V_0 = (((int64_t)((int64_t)L_8)));
		int64_t L_9 = V_0;
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_10 = ___reader0;
		NullCheck(L_10);
		Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * L_11 = VirtFuncInvoker0< Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * >::Invoke(5 /* System.IO.Stream System.IO.BinaryReader::get_BaseStream() */, L_10);
		NullCheck(L_11);
		int64_t L_12 = VirtFuncInvoker0< int64_t >::Invoke(11 /* System.Int64 System.IO.Stream::get_Position() */, L_11);
		V_0 = ((int64_t)il2cpp_codegen_add((int64_t)L_9, (int64_t)L_12));
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_13 = ___reader0;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_14 = IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031(L_13, 4, /*hidden argument*/NULL);
		String_t* L_15 = String_CreateString_m394C06654854ADD4C51FF957BE0CC72EF52BAA96(NULL, L_14, /*hidden argument*/NULL);
		NullCheck(L_15);
		bool L_16 = String_Equals_m9C4D78DFA0979504FE31429B64A4C26DF48020D1(L_15, _stringLiteral3D3689BED112D73D2D4D8B4DB76D757B55A74DA6, /*hidden argument*/NULL);
		V_6 = L_16;
		bool L_17 = V_6;
		if (L_17)
		{
			goto IL_0071;
		}
	}
	{
		Exception_t * L_18 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_18, _stringLiteral49D5687F55D6FC626AC427EF56FF9B8825ABAFF5, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_18, SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_RuntimeMethod_var);
	}

IL_0071:
	{
		__this->set_bitsPerSample_1(0);
		V_1 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)NULL;
		goto IL_01bb;
	}

IL_007f:
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_19 = ___reader0;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_20 = IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031(L_19, 4, /*hidden argument*/NULL);
		String_t* L_21 = String_CreateString_m394C06654854ADD4C51FF957BE0CC72EF52BAA96(NULL, L_20, /*hidden argument*/NULL);
		V_2 = L_21;
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_22 = ___reader0;
		NullCheck(L_22);
		int32_t L_23 = VirtFuncInvoker0< int32_t >::Invoke(16 /* System.Int32 System.IO.BinaryReader::ReadInt32() */, L_22);
		V_3 = L_23;
		String_t* L_24 = V_2;
		NullCheck(L_24);
		String_t* L_25 = String_ToLower_m5287204D93C9DDC4DF84581ADD756D0FDE2BA5A8(L_24, /*hidden argument*/NULL);
		V_7 = L_25;
		String_t* L_26 = V_7;
		if (!L_26)
		{
			goto IL_01a4;
		}
	}
	{
		String_t* L_27 = V_7;
		bool L_28 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(L_27, _stringLiteralFE15972F3A895A26455667F55C3DF6E7B75B1254, /*hidden argument*/NULL);
		if (L_28)
		{
			goto IL_00c4;
		}
	}
	{
		String_t* L_29 = V_7;
		bool L_30 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(L_29, _stringLiteral9C6F29F2FE7AEA0CF8F78533B8FADA0F3637CBEB, /*hidden argument*/NULL);
		if (L_30)
		{
			goto IL_00d9;
		}
	}
	{
		goto IL_01a4;
	}

IL_00c4:
	{
		__this->set_bitsPerSample_1(((int32_t)16));
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_31 = ___reader0;
		int32_t L_32 = V_3;
		NullCheck(L_31);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_33 = VirtFuncInvoker1< ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*, int32_t >::Invoke(26 /* System.Byte[] System.IO.BinaryReader::ReadBytes(System.Int32) */, L_31, L_32);
		V_1 = L_33;
		goto IL_01ba;
	}

IL_00d9:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_34 = V_1;
		if (!L_34)
		{
			goto IL_00fa;
		}
	}
	{
		int32_t L_35 = V_3;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_36 = __this->get_samples_0();
		NullCheck(L_36);
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_37 = ceil(((double)((double)(((double)((double)(((int32_t)((int32_t)(((RuntimeArray*)L_36)->max_length)))))))/(double)(2.0))));
		G_B13_0 = ((((int32_t)L_35) == ((int32_t)(il2cpp_codegen_cast_double_to_int<int32_t>(L_37))))? 1 : 0);
		goto IL_00fb;
	}

IL_00fa:
	{
		G_B13_0 = 0;
	}

IL_00fb:
	{
		V_6 = (bool)G_B13_0;
		bool L_38 = V_6;
		if (L_38)
		{
			goto IL_010e;
		}
	}
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_39 = ___reader0;
		int32_t L_40 = V_3;
		NullCheck(L_39);
		VirtFuncInvoker1< ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*, int32_t >::Invoke(26 /* System.Byte[] System.IO.BinaryReader::ReadBytes(System.Int32) */, L_39, L_40);
		goto IL_017f;
	}

IL_010e:
	{
		__this->set_bitsPerSample_1(((int32_t)24));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_41 = V_1;
		NullCheck(L_41);
		int32_t L_42 = V_3;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_43 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)SZArrayNew(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var, (uint32_t)((int32_t)il2cpp_codegen_add((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_41)->max_length)))), (int32_t)L_42)));
		__this->set_samples_0(L_43);
		V_4 = 0;
		V_5 = 0;
		goto IL_016c;
	}

IL_012f:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_44 = __this->get_samples_0();
		int32_t L_45 = V_4;
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_46 = ___reader0;
		NullCheck(L_46);
		uint8_t L_47 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_46);
		NullCheck(L_44);
		(L_44)->SetAt(static_cast<il2cpp_array_size_t>(L_45), (uint8_t)L_47);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_48 = __this->get_samples_0();
		int32_t L_49 = V_4;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_50 = V_1;
		int32_t L_51 = V_5;
		NullCheck(L_50);
		int32_t L_52 = L_51;
		uint8_t L_53 = (L_50)->GetAt(static_cast<il2cpp_array_size_t>(L_52));
		NullCheck(L_48);
		(L_48)->SetAt(static_cast<il2cpp_array_size_t>(((int32_t)il2cpp_codegen_add((int32_t)L_49, (int32_t)1))), (uint8_t)L_53);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_54 = __this->get_samples_0();
		int32_t L_55 = V_4;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_56 = V_1;
		int32_t L_57 = V_5;
		NullCheck(L_56);
		int32_t L_58 = ((int32_t)il2cpp_codegen_add((int32_t)L_57, (int32_t)1));
		uint8_t L_59 = (L_56)->GetAt(static_cast<il2cpp_array_size_t>(L_58));
		NullCheck(L_54);
		(L_54)->SetAt(static_cast<il2cpp_array_size_t>(((int32_t)il2cpp_codegen_add((int32_t)L_55, (int32_t)2))), (uint8_t)L_59);
		int32_t L_60 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_60, (int32_t)3));
		int32_t L_61 = V_5;
		V_5 = ((int32_t)il2cpp_codegen_add((int32_t)L_61, (int32_t)2));
	}

IL_016c:
	{
		int32_t L_62 = V_4;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_63 = __this->get_samples_0();
		NullCheck(L_63);
		V_6 = (bool)((((int32_t)L_62) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_63)->max_length))))))? 1 : 0);
		bool L_64 = V_6;
		if (L_64)
		{
			goto IL_012f;
		}
	}
	{
	}

IL_017f:
	{
		int32_t L_65 = V_3;
		if ((!(((uint32_t)((int32_t)((int32_t)L_65%(int32_t)2))) == ((uint32_t)1))))
		{
			goto IL_0193;
		}
	}
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_66 = ___reader0;
		NullCheck(L_66);
		int32_t L_67 = VirtFuncInvoker0< int32_t >::Invoke(8 /* System.Int32 System.IO.BinaryReader::PeekChar() */, L_66);
		G_B22_0 = ((((int32_t)((((int32_t)L_67) == ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		goto IL_0194;
	}

IL_0193:
	{
		G_B22_0 = 1;
	}

IL_0194:
	{
		V_6 = (bool)G_B22_0;
		bool L_68 = V_6;
		if (L_68)
		{
			goto IL_01a2;
		}
	}
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_69 = ___reader0;
		NullCheck(L_69);
		VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_69);
	}

IL_01a2:
	{
		goto IL_01ba;
	}

IL_01a4:
	{
		String_t* L_70 = V_2;
		String_t* L_71 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(_stringLiteral6FD7FCDC9FA3A9FC34F67E995715072478912067, L_70, _stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727, /*hidden argument*/NULL);
		Exception_t * L_72 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_72, L_71, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_72, SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_RuntimeMethod_var);
	}

IL_01ba:
	{
	}

IL_01bb:
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_73 = ___reader0;
		NullCheck(L_73);
		Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * L_74 = VirtFuncInvoker0< Stream_tFC50657DD5AAB87770987F9179D934A51D99D5E7 * >::Invoke(5 /* System.IO.Stream System.IO.BinaryReader::get_BaseStream() */, L_73);
		NullCheck(L_74);
		int64_t L_75 = VirtFuncInvoker0< int64_t >::Invoke(11 /* System.Int64 System.IO.Stream::get_Position() */, L_74);
		int64_t L_76 = V_0;
		V_6 = (bool)((((int64_t)L_75) < ((int64_t)L_76))? 1 : 0);
		bool L_77 = V_6;
		if (L_77)
		{
			goto IL_007f;
		}
	}
	{
		int32_t L_78 = __this->get_bitsPerSample_1();
		V_6 = (bool)((((int32_t)((((int32_t)L_78) == ((int32_t)((int32_t)16)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_79 = V_6;
		if (L_79)
		{
			goto IL_01f0;
		}
	}
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_80 = V_1;
		__this->set_samples_0(L_80);
		goto IL_020b;
	}

IL_01f0:
	{
		int32_t L_81 = __this->get_bitsPerSample_1();
		V_6 = (bool)((((int32_t)L_81) == ((int32_t)((int32_t)24)))? 1 : 0);
		bool L_82 = V_6;
		if (L_82)
		{
			goto IL_020b;
		}
	}
	{
		NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * L_83 = (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 *)il2cpp_codegen_object_new(NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var);
		NotSupportedException__ctor_mD023A89A5C1F740F43F0A9CD6C49DC21230B3CEE(L_83, _stringLiteral5DED85134BC38E3A95EA0C581D2CBEE7434808CC, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_83, SoundFontSampleData__ctor_m55920F29CDA302D187DBC63651B512479DB0FAED_RuntimeMethod_var);
	}

IL_020b:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator[] DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::get_Modulators()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* Zone_get_Modulators_m3088A40D8BB00E45D49552F9E82041ADF74308CE (Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D * __this, const RuntimeMethod* method)
{
	ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* V_0 = NULL;
	{
		ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* L_0 = __this->get_modulators_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::set_Modulators(DaggerfallWorkshop.AudioSynthesis.Sf2.Modulator[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Zone_set_Modulators_mDFF15422AB93B5E52DFC0CE85FF805898B5D37C1 (Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D * __this, ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* ___value0, const RuntimeMethod* method)
{
	{
		ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* L_0 = ___value0;
		__this->set_modulators_0(L_0);
		return;
	}
}
// DaggerfallWorkshop.AudioSynthesis.Sf2.Generator[] DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::get_Generators()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* Zone_get_Generators_mC5B2CB70DEC7AC7AEA00D91E5F094B1315297B37 (Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D * __this, const RuntimeMethod* method)
{
	GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* V_0 = NULL;
	{
		GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* L_0 = __this->get_generators_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::set_Generators(DaggerfallWorkshop.AudioSynthesis.Sf2.Generator[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Zone_set_Generators_mC37E238FB416A3B1340B5D0ED6F5C81B53F1655B (Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D * __this, GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* ___value0, const RuntimeMethod* method)
{
	{
		GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* L_0 = ___value0;
		__this->set_generators_1(L_0);
		return;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Zone_ToString_m90F7349A5B098A6571FBFEC46F5B0491993CC464 (Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Zone_ToString_m90F7349A5B098A6571FBFEC46F5B0491993CC464_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	String_t* G_B2_0 = NULL;
	String_t* G_B1_0 = NULL;
	int32_t G_B3_0 = 0;
	String_t* G_B3_1 = NULL;
	RuntimeObject * G_B5_0 = NULL;
	String_t* G_B5_1 = NULL;
	RuntimeObject * G_B4_0 = NULL;
	String_t* G_B4_1 = NULL;
	int32_t G_B6_0 = 0;
	RuntimeObject * G_B6_1 = NULL;
	String_t* G_B6_2 = NULL;
	{
		GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* L_0 = __this->get_generators_1();
		G_B1_0 = _stringLiteralBC174A2CD22E610D267A506DA9B812AAAF499C15;
		if (!L_0)
		{
			G_B2_0 = _stringLiteralBC174A2CD22E610D267A506DA9B812AAAF499C15;
			goto IL_0018;
		}
	}
	{
		GeneratorU5BU5D_tE3C4F5E841FC32F9B1AABFEC5ED593F74D84A3BF* L_1 = __this->get_generators_1();
		NullCheck(L_1);
		G_B3_0 = (((int32_t)((int32_t)(((RuntimeArray*)L_1)->max_length))));
		G_B3_1 = G_B1_0;
		goto IL_0019;
	}

IL_0018:
	{
		G_B3_0 = 0;
		G_B3_1 = G_B2_0;
	}

IL_0019:
	{
		int32_t L_2 = G_B3_0;
		RuntimeObject * L_3 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_2);
		ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* L_4 = __this->get_modulators_0();
		G_B4_0 = L_3;
		G_B4_1 = G_B3_1;
		if (!L_4)
		{
			G_B5_0 = L_3;
			G_B5_1 = G_B3_1;
			goto IL_0031;
		}
	}
	{
		ModulatorU5BU5D_t6683D7C849D76028970527B7E84341953219E43D* L_5 = __this->get_modulators_0();
		NullCheck(L_5);
		G_B6_0 = (((int32_t)((int32_t)(((RuntimeArray*)L_5)->max_length))));
		G_B6_1 = G_B4_0;
		G_B6_2 = G_B4_1;
		goto IL_0032;
	}

IL_0031:
	{
		G_B6_0 = 0;
		G_B6_1 = G_B5_0;
		G_B6_2 = G_B5_1;
	}

IL_0032:
	{
		int32_t L_6 = G_B6_0;
		RuntimeObject * L_7 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_6);
		String_t* L_8 = String_Format_m19325298DBC61AAC016C16F7B3CF97A8A3DEA34A(G_B6_2, G_B6_1, L_7, /*hidden argument*/NULL);
		V_0 = L_8;
		goto IL_0040;
	}

IL_0040:
	{
		String_t* L_9 = V_0;
		return L_9;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Sf2.Zone::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Zone__ctor_m90236630CE64764F6B9367A02A494ADAA4F3D45C (Zone_t5FF67F23BDF14868EEEE36EA4BE9227D512A4D5D * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Byte DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::get_Coarse()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t CCValue_get_Coarse_m058A8FAA33C4090561A2EF1A5F08E1587D5C8352 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method)
{
	uint8_t V_0 = 0x0;
	{
		uint8_t L_0 = __this->get_coarseValue_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		uint8_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint8_t CCValue_get_Coarse_m058A8FAA33C4090561A2EF1A5F08E1587D5C8352_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	return CCValue_get_Coarse_m058A8FAA33C4090561A2EF1A5F08E1587D5C8352(_thisAdjusted, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::set_Coarse(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, uint8_t ___value0, const RuntimeMethod* method)
{
	{
		uint8_t L_0 = ___value0;
		__this->set_coarseValue_0(L_0);
		CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)__this, /*hidden argument*/NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150_AdjustorThunk (RuntimeObject * __this, uint8_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150(_thisAdjusted, ___value0, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::set_Fine(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, uint8_t ___value0, const RuntimeMethod* method)
{
	{
		uint8_t L_0 = ___value0;
		__this->set_fineValue_1(L_0);
		CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)__this, /*hidden argument*/NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C_AdjustorThunk (RuntimeObject * __this, uint8_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C(_thisAdjusted, ___value0, method);
}
// System.Int16 DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::get_Combined()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method)
{
	int16_t V_0 = 0;
	{
		int16_t L_0 = __this->get_combined_2();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int16_t L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  int16_t CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	return CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7(_thisAdjusted, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::set_Combined(System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, int16_t ___value0, const RuntimeMethod* method)
{
	{
		int16_t L_0 = ___value0;
		__this->set_combined_2(L_0);
		CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)__this, /*hidden argument*/NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D_AdjustorThunk (RuntimeObject * __this, int16_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D(_thisAdjusted, ___value0, method);
}
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* CCValue_ToString_mBFE7FE09697E670A95307955D76D85473138EA91 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (CCValue_ToString_mBFE7FE09697E670A95307955D76D85473138EA91_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	{
		uint8_t L_0 = __this->get_coarseValue_0();
		uint8_t L_1 = L_0;
		RuntimeObject * L_2 = Box(Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var, &L_1);
		int16_t L_3 = __this->get_combined_2();
		int16_t L_4 = L_3;
		RuntimeObject * L_5 = Box(Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m19325298DBC61AAC016C16F7B3CF97A8A3DEA34A(_stringLiteral9C96333422FBF942C3C3295B5104572B59793D09, L_2, L_5, /*hidden argument*/NULL);
		V_0 = L_6;
		goto IL_0024;
	}

IL_0024:
	{
		String_t* L_7 = V_0;
		return L_7;
	}
}
IL2CPP_EXTERN_C  String_t* CCValue_ToString_mBFE7FE09697E670A95307955D76D85473138EA91_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	return CCValue_ToString_mBFE7FE09697E670A95307955D76D85473138EA91(_thisAdjusted, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::UpdateCombined()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		IL2CPP_RUNTIME_CLASS_INIT(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var);
		bool L_0 = ((BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_StaticFields*)il2cpp_codegen_static_fields_for(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var))->get_IsLittleEndian_0();
		V_0 = (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (L_1)
		{
			goto IL_0025;
		}
	}
	{
		uint8_t L_2 = __this->get_coarseValue_0();
		uint8_t L_3 = __this->get_fineValue_1();
		__this->set_combined_2((((int16_t)((int16_t)((int32_t)((int32_t)((int32_t)((int32_t)L_2<<(int32_t)7))|(int32_t)L_3))))));
		goto IL_003b;
	}

IL_0025:
	{
		uint8_t L_4 = __this->get_fineValue_1();
		uint8_t L_5 = __this->get_coarseValue_0();
		__this->set_combined_2((((int16_t)((int16_t)((int32_t)((int32_t)((int32_t)((int32_t)L_4<<(int32_t)7))|(int32_t)L_5))))));
	}

IL_003b:
	{
		return;
	}
}
IL2CPP_EXTERN_C  void CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	CCValue_UpdateCombined_mFA195C1740CFF4AE611EED28A7F4536E77FF43BD(_thisAdjusted, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.CCValue::UpdateCoarseFinePair()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6 (CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		IL2CPP_RUNTIME_CLASS_INIT(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var);
		bool L_0 = ((BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_StaticFields*)il2cpp_codegen_static_fields_for(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var))->get_IsLittleEndian_0();
		V_0 = (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (L_1)
		{
			goto IL_0030;
		}
	}
	{
		int16_t L_2 = __this->get_combined_2();
		__this->set_coarseValue_0((uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_2>>(int32_t)7))))));
		int16_t L_3 = __this->get_combined_2();
		__this->set_fineValue_1((uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_3&(int32_t)((int32_t)127)))))));
		goto IL_0051;
	}

IL_0030:
	{
		int16_t L_4 = __this->get_combined_2();
		__this->set_fineValue_1((uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_4>>(int32_t)7))))));
		int16_t L_5 = __this->get_combined_2();
		__this->set_coarseValue_0((uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_5&(int32_t)((int32_t)127)))))));
	}

IL_0051:
	{
		return;
	}
}
IL2CPP_EXTERN_C  void CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * _thisAdjusted = reinterpret_cast<CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *>(__this + _offset);
	CCValue_UpdateCoarseFinePair_m1C264A23C2B583B6724A013D94DCE8BA9328CAA6(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::.ctor(System.Byte,System.Byte,System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MidiMessage__ctor_m53974E30EEF89E43E22A772691EA3E63409E7FB3 (MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * __this, uint8_t ___channel0, uint8_t ___command1, uint8_t ___data12, uint8_t ___data23, const RuntimeMethod* method)
{
	{
		uint8_t L_0 = ___channel0;
		uint8_t L_1 = ___command1;
		uint8_t L_2 = ___data12;
		uint8_t L_3 = ___data23;
		MidiMessage__ctor_m71B14656607C7B84FA2E3F3FB461F94A591E7765((MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 *)__this, 0, L_0, L_1, L_2, L_3, /*hidden argument*/NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void MidiMessage__ctor_m53974E30EEF89E43E22A772691EA3E63409E7FB3_AdjustorThunk (RuntimeObject * __this, uint8_t ___channel0, uint8_t ___command1, uint8_t ___data12, uint8_t ___data23, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * _thisAdjusted = reinterpret_cast<MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 *>(__this + _offset);
	MidiMessage__ctor_m53974E30EEF89E43E22A772691EA3E63409E7FB3(_thisAdjusted, ___channel0, ___command1, ___data12, ___data23, method);
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::.ctor(System.Int32,System.Byte,System.Byte,System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MidiMessage__ctor_m71B14656607C7B84FA2E3F3FB461F94A591E7765 (MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * __this, int32_t ___delta0, uint8_t ___channel1, uint8_t ___command2, uint8_t ___data13, uint8_t ___data24, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___delta0;
		__this->set_delta_0(L_0);
		uint8_t L_1 = ___channel1;
		__this->set_channel_1(L_1);
		uint8_t L_2 = ___command2;
		__this->set_command_2(L_2);
		uint8_t L_3 = ___data13;
		__this->set_data1_3(L_3);
		uint8_t L_4 = ___data24;
		__this->set_data2_4(L_4);
		return;
	}
}
IL2CPP_EXTERN_C  void MidiMessage__ctor_m71B14656607C7B84FA2E3F3FB461F94A591E7765_AdjustorThunk (RuntimeObject * __this, int32_t ___delta0, uint8_t ___channel1, uint8_t ___command2, uint8_t ___data13, uint8_t ___data24, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * _thisAdjusted = reinterpret_cast<MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 *>(__this + _offset);
	MidiMessage__ctor_m71B14656607C7B84FA2E3F3FB461F94A591E7765(_thisAdjusted, ___delta0, ___channel1, ___command2, ___data13, ___data24, method);
}
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.MidiMessage::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MidiMessage_ToString_mDBEC05B0D9113B1A597467CCE6B8D42B79CA179C (MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MidiMessage_ToString_mDBEC05B0D9113B1A597467CCE6B8D42B79CA179C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	bool V_1 = false;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_2 = NULL;
	int32_t G_B3_0 = 0;
	int32_t G_B8_0 = 0;
	int32_t G_B13_0 = 0;
	{
		uint8_t L_0 = __this->get_command_2();
		if ((((int32_t)L_0) < ((int32_t)((int32_t)128))))
		{
			goto IL_001d;
		}
	}
	{
		uint8_t L_1 = __this->get_command_2();
		G_B3_0 = ((((int32_t)L_1) > ((int32_t)((int32_t)239)))? 1 : 0);
		goto IL_001e;
	}

IL_001d:
	{
		G_B3_0 = 1;
	}

IL_001e:
	{
		V_1 = (bool)G_B3_0;
		bool L_2 = V_1;
		if (L_2)
		{
			goto IL_0076;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_3 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)4);
		V_2 = L_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_4 = V_2;
		uint8_t L_5 = __this->get_command_2();
		int32_t L_6 = ((int32_t)((int32_t)((int32_t)L_5&(int32_t)((int32_t)240))));
		RuntimeObject * L_7 = Box(MidiEventTypeEnum_t99E303F3D268222216E7B24459B04ACF2C462C68_il2cpp_TypeInfo_var, &L_6);
		NullCheck(L_4);
		ArrayElementTypeCheck (L_4, L_7);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_7);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_8 = V_2;
		uint8_t L_9 = __this->get_channel_1();
		uint8_t L_10 = L_9;
		RuntimeObject * L_11 = Box(Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var, &L_10);
		NullCheck(L_8);
		ArrayElementTypeCheck (L_8, L_11);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_11);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_12 = V_2;
		uint8_t L_13 = __this->get_data1_3();
		uint8_t L_14 = L_13;
		RuntimeObject * L_15 = Box(Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var, &L_14);
		NullCheck(L_12);
		ArrayElementTypeCheck (L_12, L_15);
		(L_12)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)L_15);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_16 = V_2;
		uint8_t L_17 = __this->get_data2_4();
		uint8_t L_18 = L_17;
		RuntimeObject * L_19 = Box(Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var, &L_18);
		NullCheck(L_16);
		ArrayElementTypeCheck (L_16, L_19);
		(L_16)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_19);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_20 = V_2;
		String_t* L_21 = String_Format_mA3AC3FE7B23D97F3A5BAA082D25B0E01B341A865(_stringLiteral1CD78D8DF6D14DF92778EFEB1A8E78176DE95F65, L_20, /*hidden argument*/NULL);
		V_0 = L_21;
		goto IL_00d2;
	}

IL_0076:
	{
		uint8_t L_22 = __this->get_command_2();
		if ((((int32_t)L_22) < ((int32_t)((int32_t)240))))
		{
			goto IL_0092;
		}
	}
	{
		uint8_t L_23 = __this->get_command_2();
		G_B8_0 = ((((int32_t)L_23) > ((int32_t)((int32_t)247)))? 1 : 0);
		goto IL_0093;
	}

IL_0092:
	{
		G_B8_0 = 1;
	}

IL_0093:
	{
		V_1 = (bool)G_B8_0;
		bool L_24 = V_1;
		if (L_24)
		{
			goto IL_00a0;
		}
	}
	{
		V_0 = _stringLiteralCD5F8179BE6552D86CB645C304C3FA048CE32DF9;
		goto IL_00d2;
	}

IL_00a0:
	{
		uint8_t L_25 = __this->get_command_2();
		if ((((int32_t)L_25) < ((int32_t)((int32_t)248))))
		{
			goto IL_00bc;
		}
	}
	{
		uint8_t L_26 = __this->get_command_2();
		G_B13_0 = ((((int32_t)L_26) > ((int32_t)((int32_t)255)))? 1 : 0);
		goto IL_00bd;
	}

IL_00bc:
	{
		G_B13_0 = 1;
	}

IL_00bd:
	{
		V_1 = (bool)G_B13_0;
		bool L_27 = V_1;
		if (L_27)
		{
			goto IL_00ca;
		}
	}
	{
		V_0 = _stringLiteral8901BA5CF896C7D5ED70B8D0B338F5A7E36BF7AB;
		goto IL_00d2;
	}

IL_00ca:
	{
		V_0 = _stringLiteralD055BC390DB68441E98AE955CF3ECA6B94C8B738;
		goto IL_00d2;
	}

IL_00d2:
	{
		String_t* L_28 = V_0;
		return L_28;
	}
}
IL2CPP_EXTERN_C  String_t* MidiMessage_ToString_mDBEC05B0D9113B1A597467CCE6B8D42B79CA179C_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 * _thisAdjusted = reinterpret_cast<MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484 *>(__this + _offset);
	return MidiMessage_ToString_mDBEC05B0D9113B1A597467CCE6B8D42B79CA179C(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent::.ctor(System.Single,DaggerfallWorkshop.AudioSynthesis.Synthesis.PanFormulaEnum)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B (PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * __this, float ___value0, int32_t ___formula1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	double V_0 = 0.0;
	int32_t V_1 = 0;
	{
		float L_0 = ___value0;
		float L_1 = SynthHelper_Clamp_m5647B8E90763F524A8AA958F061A8C96C2B9423B(L_0, (-1.0f), (1.0f), /*hidden argument*/NULL);
		___value0 = L_1;
		int32_t L_2 = ___formula1;
		V_1 = L_2;
		int32_t L_3 = V_1;
		switch (L_3)
		{
			case 0:
			{
				goto IL_002c;
			}
			case 1:
			{
				goto IL_006a;
			}
			case 2:
			{
				goto IL_0094;
			}
		}
	}
	{
		goto IL_00e7;
	}

IL_002c:
	{
		float L_4 = ___value0;
		V_0 = ((double)((double)((double)il2cpp_codegen_multiply((double)(1.5707963267948966), (double)(((double)((double)((float)il2cpp_codegen_add((float)L_4, (float)(1.0f))))))))/(double)(2.0)));
		double L_5 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_6 = cos(L_5);
		__this->set_Left_0((((float)((float)L_6))));
		double L_7 = V_0;
		double L_8 = sin(L_7);
		__this->set_Right_1((((float)((float)L_8))));
		goto IL_00f2;
	}

IL_006a:
	{
		float L_9 = ___value0;
		__this->set_Left_0(((float)il2cpp_codegen_add((float)(0.5f), (float)((float)il2cpp_codegen_multiply((float)L_9, (float)(-0.5f))))));
		float L_10 = ___value0;
		__this->set_Right_1(((float)il2cpp_codegen_add((float)(0.5f), (float)((float)il2cpp_codegen_multiply((float)L_10, (float)(0.5f))))));
		goto IL_00f2;
	}

IL_0094:
	{
		float L_11 = ___value0;
		V_0 = ((double)((double)((double)il2cpp_codegen_multiply((double)(1.5707963267948966), (double)((double)il2cpp_codegen_add((double)(((double)((double)L_11))), (double)(1.0)))))/(double)(2.0)));
		double L_12 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_13 = cos(L_12);
		__this->set_Left_0((((float)((float)((double)((double)L_13/(double)(0.707106781186)))))));
		double L_14 = V_0;
		double L_15 = sin(L_14);
		__this->set_Right_1((((float)((float)((double)((double)L_15/(double)(0.707106781186)))))));
		goto IL_00f2;
	}

IL_00e7:
	{
		Exception_t * L_16 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_16, _stringLiteral983B90B80B4D17D1EA0A7A83A5F60B2BB828E383, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_16, PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B_RuntimeMethod_var);
	}

IL_00f2:
	{
		return;
	}
}
IL2CPP_EXTERN_C  void PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B_AdjustorThunk (RuntimeObject * __this, float ___value0, int32_t ___formula1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * _thisAdjusted = reinterpret_cast<PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 *>(__this + _offset);
	PanComponent__ctor_m5F67E47A3874553AC59F72FAF5111F0C192D367B(_thisAdjusted, ___value0, ___formula1, method);
}
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.PanComponent::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* PanComponent_ToString_mCE8FA3534B904E393E7D21CCCCEF9B0286BFA123 (PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PanComponent_ToString_mCE8FA3534B904E393E7D21CCCCEF9B0286BFA123_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	{
		float L_0 = __this->get_Left_0();
		float L_1 = L_0;
		RuntimeObject * L_2 = Box(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var, &L_1);
		float L_3 = __this->get_Right_1();
		float L_4 = L_3;
		RuntimeObject * L_5 = Box(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Format_m19325298DBC61AAC016C16F7B3CF97A8A3DEA34A(_stringLiteral597C00FD0A500F1B9807E0C4B6F599EC134A4A3D, L_2, L_5, /*hidden argument*/NULL);
		V_0 = L_6;
		goto IL_0024;
	}

IL_0024:
	{
		String_t* L_7 = V_0;
		return L_7;
	}
}
IL2CPP_EXTERN_C  String_t* PanComponent_ToString_mCE8FA3534B904E393E7D21CCCCEF9B0286BFA123_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * _thisAdjusted = reinterpret_cast<PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 *>(__this + _offset);
	return PanComponent_ToString_mCE8FA3534B904E393E7D21CCCCEF9B0286BFA123(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Double DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::Clamp(System.Double,System.Double,System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double SynthHelper_Clamp_mF8F2ACFB7A6E2B5F3C1F2D19D6896CBA93B097A8 (double ___value0, double ___min1, double ___max2, const RuntimeMethod* method)
{
	double V_0 = 0.0;
	bool V_1 = false;
	{
		double L_0 = ___value0;
		double L_1 = ___min1;
		V_1 = (bool)((!(((double)L_0) <= ((double)L_1)))? 1 : 0);
		bool L_2 = V_1;
		if (L_2)
		{
			goto IL_000d;
		}
	}
	{
		double L_3 = ___min1;
		V_0 = L_3;
		goto IL_001d;
	}

IL_000d:
	{
		double L_4 = ___value0;
		double L_5 = ___max2;
		V_1 = (bool)((!(((double)L_4) >= ((double)L_5)))? 1 : 0);
		bool L_6 = V_1;
		if (L_6)
		{
			goto IL_0019;
		}
	}
	{
		double L_7 = ___max2;
		V_0 = L_7;
		goto IL_001d;
	}

IL_0019:
	{
		double L_8 = ___value0;
		V_0 = L_8;
		goto IL_001d;
	}

IL_001d:
	{
		double L_9 = V_0;
		return L_9;
	}
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::Clamp(System.Single,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float SynthHelper_Clamp_m5647B8E90763F524A8AA958F061A8C96C2B9423B (float ___value0, float ___min1, float ___max2, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	bool V_1 = false;
	{
		float L_0 = ___value0;
		float L_1 = ___min1;
		V_1 = (bool)((!(((float)L_0) <= ((float)L_1)))? 1 : 0);
		bool L_2 = V_1;
		if (L_2)
		{
			goto IL_000d;
		}
	}
	{
		float L_3 = ___min1;
		V_0 = L_3;
		goto IL_001d;
	}

IL_000d:
	{
		float L_4 = ___value0;
		float L_5 = ___max2;
		V_1 = (bool)((!(((float)L_4) >= ((float)L_5)))? 1 : 0);
		bool L_6 = V_1;
		if (L_6)
		{
			goto IL_0019;
		}
	}
	{
		float L_7 = ___max2;
		V_0 = L_7;
		goto IL_001d;
	}

IL_0019:
	{
		float L_8 = ___value0;
		V_0 = L_8;
		goto IL_001d;
	}

IL_001d:
	{
		float L_9 = V_0;
		return L_9;
	}
}
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::Clamp(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SynthHelper_Clamp_m767D40B465457BDAFF1CB5EF2D2D0428E4B40A98 (int32_t ___value0, int32_t ___min1, int32_t ___max2, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	{
		int32_t L_0 = ___value0;
		int32_t L_1 = ___min1;
		V_1 = (bool)((((int32_t)L_0) > ((int32_t)L_1))? 1 : 0);
		bool L_2 = V_1;
		if (L_2)
		{
			goto IL_000d;
		}
	}
	{
		int32_t L_3 = ___min1;
		V_0 = L_3;
		goto IL_001d;
	}

IL_000d:
	{
		int32_t L_4 = ___value0;
		int32_t L_5 = ___max2;
		V_1 = (bool)((((int32_t)L_4) < ((int32_t)L_5))? 1 : 0);
		bool L_6 = V_1;
		if (L_6)
		{
			goto IL_0019;
		}
	}
	{
		int32_t L_7 = ___max2;
		V_0 = L_7;
		goto IL_001d;
	}

IL_0019:
	{
		int32_t L_8 = ___value0;
		V_0 = L_8;
		goto IL_001d;
	}

IL_001d:
	{
		int32_t L_9 = V_0;
		return L_9;
	}
}
// System.Int16 DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::Clamp(System.Int16,System.Int16,System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t SynthHelper_Clamp_mB9BFD83FB114711204CB7E6F80D59D1E1724D689 (int16_t ___value0, int16_t ___min1, int16_t ___max2, const RuntimeMethod* method)
{
	int16_t V_0 = 0;
	bool V_1 = false;
	{
		int16_t L_0 = ___value0;
		int16_t L_1 = ___min1;
		V_1 = (bool)((((int32_t)L_0) > ((int32_t)L_1))? 1 : 0);
		bool L_2 = V_1;
		if (L_2)
		{
			goto IL_000d;
		}
	}
	{
		int16_t L_3 = ___min1;
		V_0 = L_3;
		goto IL_001d;
	}

IL_000d:
	{
		int16_t L_4 = ___value0;
		int16_t L_5 = ___max2;
		V_1 = (bool)((((int32_t)L_4) < ((int32_t)L_5))? 1 : 0);
		bool L_6 = V_1;
		if (L_6)
		{
			goto IL_0019;
		}
	}
	{
		int16_t L_7 = ___max2;
		V_0 = L_7;
		goto IL_001d;
	}

IL_0019:
	{
		int16_t L_8 = ___value0;
		V_0 = L_8;
		goto IL_001d;
	}

IL_001d:
	{
		int16_t L_9 = V_0;
		return L_9;
	}
}
// System.Double DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::DBtoLinear(System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double SynthHelper_DBtoLinear_mD8C65E96C0148506BA89CFF914203F329EEFB47C (double ___dBvalue0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SynthHelper_DBtoLinear_mD8C65E96C0148506BA89CFF914203F329EEFB47C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	double V_0 = 0.0;
	{
		double L_0 = ___dBvalue0;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_1 = Math_Pow_m9CD842663B1A2FA4C66EEFFC6F0D705B40BE46F1((10.0), ((double)((double)L_0/(double)(20.0))), /*hidden argument*/NULL);
		V_0 = L_1;
		goto IL_001d;
	}

IL_001d:
	{
		double L_2 = V_0;
		return L_2;
	}
}
// System.Double DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::KeyToFrequency(System.Double,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double SynthHelper_KeyToFrequency_mBB41A7F23826AE45F4397D85A2A8E36A097528A1 (double ___key0, int32_t ___rootkey1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SynthHelper_KeyToFrequency_mBB41A7F23826AE45F4397D85A2A8E36A097528A1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	double V_0 = 0.0;
	{
		double L_0 = ___key0;
		int32_t L_1 = ___rootkey1;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_2 = Math_Pow_m9CD842663B1A2FA4C66EEFFC6F0D705B40BE46F1((2.0), ((double)((double)((double)il2cpp_codegen_subtract((double)L_0, (double)(((double)((double)L_1)))))/(double)(12.0))), /*hidden argument*/NULL);
		V_0 = ((double)il2cpp_codegen_multiply((double)L_2, (double)(440.0)));
		goto IL_002a;
	}

IL_002a:
	{
		double L_3 = V_0;
		return L_3;
	}
}
// System.Double DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthHelper::CentsToPitch(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double SynthHelper_CentsToPitch_m97EADE1AEF113245C1DEC9FD83B50C69F7B29E16 (int32_t ___cents0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SynthHelper_CentsToPitch_m97EADE1AEF113245C1DEC9FD83B50C69F7B29E16_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	double V_1 = 0.0;
	bool V_2 = false;
	{
		int32_t L_0 = ___cents0;
		V_0 = ((int32_t)((int32_t)L_0/(int32_t)((int32_t)100)));
		int32_t L_1 = ___cents0;
		int32_t L_2 = V_0;
		___cents0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_1, (int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_2, (int32_t)((int32_t)100)))));
		int32_t L_3 = V_0;
		V_2 = (bool)((((int32_t)((((int32_t)L_3) < ((int32_t)((int32_t)-127)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_4 = V_2;
		if (L_4)
		{
			goto IL_001f;
		}
	}
	{
		V_0 = ((int32_t)-127);
		goto IL_002e;
	}

IL_001f:
	{
		int32_t L_5 = V_0;
		V_2 = (bool)((((int32_t)((((int32_t)L_5) > ((int32_t)((int32_t)127)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_6 = V_2;
		if (L_6)
		{
			goto IL_002e;
		}
	}
	{
		V_0 = ((int32_t)127);
	}

IL_002e:
	{
		IL2CPP_RUNTIME_CLASS_INIT(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var);
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_7 = ((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->get_SemitoneTable_1();
		int32_t L_8 = V_0;
		NullCheck(L_7);
		int32_t L_9 = ((int32_t)il2cpp_codegen_add((int32_t)((int32_t)127), (int32_t)L_8));
		double L_10 = (L_7)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_11 = ((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->get_CentTable_2();
		int32_t L_12 = ___cents0;
		NullCheck(L_11);
		int32_t L_13 = ((int32_t)il2cpp_codegen_add((int32_t)((int32_t)100), (int32_t)L_12));
		double L_14 = (L_11)->GetAt(static_cast<il2cpp_array_size_t>(L_13));
		V_1 = ((double)il2cpp_codegen_multiply((double)L_10, (double)L_14));
		goto IL_0046;
	}

IL_0046:
	{
		double L_15 = V_1;
		return L_15;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::.ctor(DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters__ctor_mF07CCFE15DD33513094F4EBC67A7D0F1E2870BE0 (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * ___synth0, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * L_0 = ___synth0;
		__this->set_synth_15(L_0);
		SynthParameters_ResetControllers_mCE6BAEAB27EA90A2D744C8B30D15C5E0D32D2CAA(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::ResetControllers()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_ResetControllers_mCE6BAEAB27EA90A2D744C8B30D15C5E0D32D2CAA (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method)
{
	{
		__this->set_program_0((uint8_t)0);
		__this->set_bankSelect_1((uint8_t)0);
		__this->set_channelAfterTouch_2((uint8_t)0);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_0 = __this->get_address_of_pan_3();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_0, (int16_t)((int32_t)8192), /*hidden argument*/NULL);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_1 = __this->get_address_of_volume_4();
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_1, (uint8_t)0, /*hidden argument*/NULL);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_2 = __this->get_address_of_volume_4();
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_2, (uint8_t)((int32_t)100), /*hidden argument*/NULL);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_3 = __this->get_address_of_expression_5();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_3, (int16_t)((int32_t)16383), /*hidden argument*/NULL);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_4 = __this->get_address_of_modRange_6();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_4, (int16_t)0, /*hidden argument*/NULL);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_5 = __this->get_address_of_pitchBend_7();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_5, (int16_t)((int32_t)8192), /*hidden argument*/NULL);
		__this->set_pitchBendRangeCoarse_8((uint8_t)2);
		__this->set_pitchBendRangeFine_9((uint8_t)0);
		__this->set_masterCoarseTune_10((int16_t)0);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_6 = __this->get_address_of_masterFineTune_11();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_6, (int16_t)((int32_t)8192), /*hidden argument*/NULL);
		__this->set_holdPedal_12((bool)0);
		__this->set_legatoPedal_13((bool)0);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_7 = __this->get_address_of_rpn_14();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_7, (int16_t)((int32_t)16383), /*hidden argument*/NULL);
		SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C(__this, /*hidden argument*/NULL);
		SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F(__this, /*hidden argument*/NULL);
		SynthParameters_UpdateCurrentVolume_m2BDC5BDA381F6ECD8778AD86BE1B8DEB712FA4C6(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentVolume()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentVolume_m2BDC5BDA381F6ECD8778AD86BE1B8DEB712FA4C6 (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method)
{
	{
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_0 = __this->get_address_of_expression_5();
		int16_t L_1 = CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_0, /*hidden argument*/NULL);
		__this->set_currentVolume_16(((float)((float)(((float)((float)L_1)))/(float)(16383.0f))));
		float L_2 = __this->get_currentVolume_16();
		float L_3 = __this->get_currentVolume_16();
		__this->set_currentVolume_16(((float)il2cpp_codegen_multiply((float)L_2, (float)L_3)));
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentPitch()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method)
{
	{
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_0 = __this->get_address_of_pitchBend_7();
		int16_t L_1 = CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_0, /*hidden argument*/NULL);
		uint8_t L_2 = __this->get_pitchBendRangeCoarse_8();
		uint8_t L_3 = __this->get_pitchBendRangeFine_9();
		__this->set_currentPitch_17((il2cpp_codegen_cast_double_to_int<int32_t>(((double)il2cpp_codegen_multiply((double)((double)((double)((double)il2cpp_codegen_subtract((double)(((double)((double)L_1))), (double)(8192.0)))/(double)(8192.0))), (double)(((double)((double)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)100), (int32_t)L_2)), (int32_t)L_3))))))))));
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentMod()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentMod_mA575ADC4219635E77436872A0D375E5B93B9F8DE (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method)
{
	{
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_0 = __this->get_address_of_modRange_6();
		int16_t L_1 = CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_0, /*hidden argument*/NULL);
		__this->set_currentMod_18((il2cpp_codegen_cast_double_to_int<int32_t>(((double)il2cpp_codegen_multiply((double)(100.0), (double)((double)((double)(((double)((double)L_1)))/(double)(16383.0))))))));
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters::UpdateCurrentPan()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	double V_0 = 0.0;
	{
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_0 = __this->get_address_of_pan_3();
		int16_t L_1 = CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_0, /*hidden argument*/NULL);
		V_0 = ((double)il2cpp_codegen_multiply((double)(1.5707963267948966), (double)((double)((double)(((double)((double)L_1)))/(double)(16383.0)))));
		PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * L_2 = __this->get_address_of_currentPan_19();
		double L_3 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_4 = cos(L_3);
		L_2->set_Left_0((((float)((float)L_4))));
		PanComponent_tCB23DF6B8F637B69F16D1FAD702B76A331117A90 * L_5 = __this->get_address_of_currentPan_19();
		double L_6 = V_0;
		double L_7 = sin(L_6);
		L_5->set_Right_1((((float)((float)L_7))));
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::get_MicroBufferSize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Synthesizer_get_MicroBufferSize_m49306D14B3E66FF4190EBD9B0FB5D138E7F3F386 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_microBufferSize_25();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::get_WorkingBufferSize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Synthesizer_get_WorkingBufferSize_m138D7FB64F23CC8382EF3DE3F5A1852DEEB7E15A (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_0 = __this->get_sampleBuffer_17();
		NullCheck(L_0);
		V_0 = (((int32_t)((int32_t)(((RuntimeArray*)L_0)->max_length))));
		goto IL_000c;
	}

IL_000c:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::get_MixGain()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Synthesizer_get_MixGain_m2BC60A97B706CD402EAEAACA4B5743594915BCE7 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		float L_0 = __this->get_synthGain_24();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		float L_1 = V_0;
		return L_1;
	}
}
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::get_SampleRate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Synthesizer_get_SampleRate_m103589FBDBC81DC993F2D44F0B9F99010395E94B (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_sampleRate_22();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::get_AudioChannels()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Synthesizer_get_AudioChannels_mCDA65B1F4E2122559421144BFFD8466E5C02C640 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_audioChannels_19();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::.ctor(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer__ctor_mD1E1F86B5C913D1042DB6C79568DC79675CDDBFE (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___sampleRate0, int32_t ___audioChannels1, int32_t ___bufferSize2, int32_t ___bufferCount3, int32_t ___polyphony4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer__ctor_mD1E1F86B5C913D1042DB6C79568DC79675CDDBFE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	bool V_1 = false;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_2 = NULL;
	int32_t G_B3_0 = 0;
	int32_t G_B8_0 = 0;
	{
		__this->set_mainVolume_23((1.0f));
		__this->set_synthGain_24((0.35f));
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		int32_t L_0 = ___sampleRate0;
		if ((((int32_t)L_0) < ((int32_t)((int32_t)8000))))
		{
			goto IL_0033;
		}
	}
	{
		int32_t L_1 = ___sampleRate0;
		G_B3_0 = ((((int32_t)((((int32_t)L_1) > ((int32_t)((int32_t)96000)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		goto IL_0034;
	}

IL_0033:
	{
		G_B3_0 = 0;
	}

IL_0034:
	{
		V_1 = (bool)G_B3_0;
		bool L_2 = V_1;
		if (L_2)
		{
			goto IL_007b;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_3 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)4);
		V_2 = L_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_4 = V_2;
		NullCheck(L_4);
		ArrayElementTypeCheck (L_4, _stringLiteral5459DD4D5FDD4DFD4BDA0478319570A07D1252DA);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)_stringLiteral5459DD4D5FDD4DFD4BDA0478319570A07D1252DA);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_5 = V_2;
		int32_t L_6 = ((int32_t)8000);
		RuntimeObject * L_7 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_6);
		NullCheck(L_5);
		ArrayElementTypeCheck (L_5, L_7);
		(L_5)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_7);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_8 = V_2;
		NullCheck(L_8);
		ArrayElementTypeCheck (L_8, _stringLiteral3A59C4EE70A9BD28671F1D01D0D6D049795CEF38);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)_stringLiteral3A59C4EE70A9BD28671F1D01D0D6D049795CEF38);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_9 = V_2;
		int32_t L_10 = ((int32_t)96000);
		RuntimeObject * L_11 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_10);
		NullCheck(L_9);
		ArrayElementTypeCheck (L_9, L_11);
		(L_9)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_11);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_12 = V_2;
		String_t* L_13 = String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07(L_12, /*hidden argument*/NULL);
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_14 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m26DC3463C6F3C98BF33EA39598DD2B32F0249CA8(L_14, L_13, _stringLiteralFB2B350D8A7204988C23DC3DD8A2D8FECFAF11FB, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_14, Synthesizer__ctor_mD1E1F86B5C913D1042DB6C79568DC79675CDDBFE_RuntimeMethod_var);
	}

IL_007b:
	{
		int32_t L_15 = ___sampleRate0;
		__this->set_sampleRate_22(L_15);
		int32_t L_16 = ___audioChannels1;
		if ((((int32_t)L_16) < ((int32_t)1)))
		{
			goto IL_008f;
		}
	}
	{
		int32_t L_17 = ___audioChannels1;
		G_B8_0 = ((((int32_t)((((int32_t)L_17) > ((int32_t)2))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		goto IL_0090;
	}

IL_008f:
	{
		G_B8_0 = 0;
	}

IL_0090:
	{
		V_1 = (bool)G_B8_0;
		bool L_18 = V_1;
		if (L_18)
		{
			goto IL_00cf;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_19 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)4);
		V_2 = L_19;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_20 = V_2;
		NullCheck(L_20);
		ArrayElementTypeCheck (L_20, _stringLiteral1A843118298810BCF6B34D501F9D6F73F1AA6419);
		(L_20)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)_stringLiteral1A843118298810BCF6B34D501F9D6F73F1AA6419);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_21 = V_2;
		int32_t L_22 = 1;
		RuntimeObject * L_23 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_22);
		NullCheck(L_21);
		ArrayElementTypeCheck (L_21, L_23);
		(L_21)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_23);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_24 = V_2;
		NullCheck(L_24);
		ArrayElementTypeCheck (L_24, _stringLiteral3A59C4EE70A9BD28671F1D01D0D6D049795CEF38);
		(L_24)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)_stringLiteral3A59C4EE70A9BD28671F1D01D0D6D049795CEF38);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_25 = V_2;
		int32_t L_26 = 2;
		RuntimeObject * L_27 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_26);
		NullCheck(L_25);
		ArrayElementTypeCheck (L_25, L_27);
		(L_25)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_27);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_28 = V_2;
		String_t* L_29 = String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07(L_28, /*hidden argument*/NULL);
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_30 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m26DC3463C6F3C98BF33EA39598DD2B32F0249CA8(L_30, L_29, _stringLiteral915AE5169E08423550843F4FEF5B491F8A5D3DF3, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_30, Synthesizer__ctor_mD1E1F86B5C913D1042DB6C79568DC79675CDDBFE_RuntimeMethod_var);
	}

IL_00cf:
	{
		int32_t L_31 = ___audioChannels1;
		__this->set_audioChannels_19(L_31);
		int32_t L_32 = ___bufferSize2;
		int32_t L_33 = ___sampleRate0;
		int32_t L_34 = ___sampleRate0;
		int32_t L_35 = SynthHelper_Clamp_m767D40B465457BDAFF1CB5EF2D2D0428E4B40A98(L_32, (il2cpp_codegen_cast_double_to_int<int32_t>(((double)il2cpp_codegen_multiply((double)(0.001), (double)(((double)((double)L_33))))))), (il2cpp_codegen_cast_double_to_int<int32_t>(((double)il2cpp_codegen_multiply((double)(0.05), (double)(((double)((double)L_34))))))), /*hidden argument*/NULL);
		__this->set_microBufferSize_25(L_35);
		int32_t L_36 = __this->get_microBufferSize_25();
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_37 = ceil(((double)((double)(((double)((double)L_36)))/(double)(64.0))));
		__this->set_microBufferSize_25(((int32_t)il2cpp_codegen_multiply((int32_t)(il2cpp_codegen_cast_double_to_int<int32_t>(L_37)), (int32_t)((int32_t)64))));
		int32_t L_38 = ___bufferCount3;
		int32_t L_39 = Math_Max_mA99E48BB021F2E4B62D4EA9F52EA6928EED618A2(1, L_38, /*hidden argument*/NULL);
		__this->set_microBufferCount_26(L_39);
		int32_t L_40 = __this->get_microBufferSize_25();
		int32_t L_41 = __this->get_microBufferCount_26();
		int32_t L_42 = ___audioChannels1;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_43 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_40, (int32_t)L_41)), (int32_t)L_42)));
		__this->set_sampleBuffer_17(L_43);
		__this->set_littleEndian_20((bool)1);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_44 = (SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0*)(SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0*)SZArrayNew(SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0_il2cpp_TypeInfo_var, (uint32_t)((int32_t)16));
		__this->set_synthChannels_27(L_44);
		V_0 = 0;
		goto IL_016e;
	}

IL_015c:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_45 = __this->get_synthChannels_27();
		int32_t L_46 = V_0;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_47 = (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 *)il2cpp_codegen_object_new(SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33_il2cpp_TypeInfo_var);
		SynthParameters__ctor_mF07CCFE15DD33513094F4EBC67A7D0F1E2870BE0(L_47, __this, /*hidden argument*/NULL);
		NullCheck(L_45);
		ArrayElementTypeCheck (L_45, L_47);
		(L_45)->SetAt(static_cast<il2cpp_array_size_t>(L_46), (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 *)L_47);
		int32_t L_48 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_48, (int32_t)1));
	}

IL_016e:
	{
		int32_t L_49 = V_0;
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_50 = __this->get_synthChannels_27();
		NullCheck(L_50);
		V_1 = (bool)((((int32_t)L_49) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_50)->max_length))))))? 1 : 0);
		bool L_51 = V_1;
		if (L_51)
		{
			goto IL_015c;
		}
	}
	{
		int32_t L_52 = ___polyphony4;
		int32_t L_53 = SynthHelper_Clamp_m767D40B465457BDAFF1CB5EF2D2D0428E4B40A98(L_52, 5, ((int32_t)250), /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_54 = (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 *)il2cpp_codegen_object_new(VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8_il2cpp_TypeInfo_var);
		VoiceManager__ctor_mE544FA76F2141F9F6BE6FD198686AD516D74378D(L_54, L_53, /*hidden argument*/NULL);
		__this->set_voiceManager_18(L_54);
		Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * L_55 = (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C *)il2cpp_codegen_object_new(Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C_il2cpp_TypeInfo_var);
		Queue_1__ctor_mB4BC9CA4838CC532EBAA314118FF6116CCF76208(L_55, /*hidden argument*/Queue_1__ctor_mB4BC9CA4838CC532EBAA314118FF6116CCF76208_RuntimeMethod_var);
		__this->set_midiEventQueue_28(L_55);
		int32_t L_56 = __this->get_microBufferCount_26();
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_57 = (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)SZArrayNew(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var, (uint32_t)L_56);
		__this->set_midiEventCounts_29(L_57);
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_58 = (PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34*)(PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34*)SZArrayNew(PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34_il2cpp_TypeInfo_var, (uint32_t)((int32_t)15));
		__this->set_layerList_30(L_58);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::LoadBank(DaggerfallWorkshop.AudioSynthesis.IResource)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_LoadBank_m806559636928C4F28448F3F8ADF0E12E6CEC7E4E (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, RuntimeObject* ___bankFile0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_LoadBank_m806559636928C4F28448F3F8ADF0E12E6CEC7E4E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeObject* L_0 = ___bankFile0;
		PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * L_1 = (PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 *)il2cpp_codegen_object_new(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225_il2cpp_TypeInfo_var);
		PatchBank__ctor_m2B2D79BB0EB7A7E80C4BC4A73E22A42996F3EE5D(L_1, L_0, /*hidden argument*/NULL);
		Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F(__this, L_1, /*hidden argument*/NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::LoadBank(DaggerfallWorkshop.AudioSynthesis.Bank.PatchBank)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * ___bank0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * L_0 = ___bank0;
		V_0 = (bool)((((int32_t)((((RuntimeObject*)(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 *)L_0) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_0;
		if (L_1)
		{
			goto IL_0017;
		}
	}
	{
		ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD * L_2 = (ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD *)il2cpp_codegen_object_new(ArgumentNullException_t581DF992B1F3E0EC6EFB30CC5DC43519A79B27AD_il2cpp_TypeInfo_var);
		ArgumentNullException__ctor_mEE0C0D6FCB2D08CD7967DBB1329A0854BBED49ED(L_2, _stringLiteralCA9D82DEE7812B880BE910B9E5DD0A553F05CFA9, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, Synthesizer_LoadBank_mD814F94719EA9369034C87E906165C7F4DF2771F_RuntimeMethod_var);
	}

IL_0017:
	{
		Synthesizer_UnloadBank_mD52E476F76A6B446A5C9F01C4C633695FC7A4E10(__this, /*hidden argument*/NULL);
		PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * L_3 = ___bank0;
		__this->set_bank_21(L_3);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::UnloadBank()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_UnloadBank_mD52E476F76A6B446A5C9F01C4C633695FC7A4E10 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * L_0 = __this->get_bank_21();
		V_0 = (bool)((((RuntimeObject*)(PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 *)L_0) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0);
		bool L_1 = V_0;
		if (L_1)
		{
			goto IL_002b;
		}
	}
	{
		Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D(__this, (bool)1, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_2 = __this->get_voiceManager_18();
		NullCheck(L_2);
		VoiceManager_UnloadPatches_m0B094424DF30EDE3C004B6EDFB7C97A60F9C32CB(L_2, /*hidden argument*/NULL);
		__this->set_bank_21((PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 *)NULL);
	}

IL_002b:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ResetSynthControls()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ResetSynthControls_mF275C87C8393B84B0128679AC3376FDE3DB930C5 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	{
		V_0 = 0;
		goto IL_0019;
	}

IL_0005:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_0 = __this->get_synthChannels_27();
		int32_t L_1 = V_0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		NullCheck(L_3);
		SynthParameters_ResetControllers_mCE6BAEAB27EA90A2D744C8B30D15C5E0D32D2CAA(L_3, /*hidden argument*/NULL);
		int32_t L_4 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_4, (int32_t)1));
	}

IL_0019:
	{
		int32_t L_5 = V_0;
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_6 = __this->get_synthChannels_27();
		NullCheck(L_6);
		V_1 = (bool)((((int32_t)L_5) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_6)->max_length))))))? 1 : 0);
		bool L_7 = V_1;
		if (L_7)
		{
			goto IL_0005;
		}
	}
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_8 = __this->get_synthChannels_27();
		NullCheck(L_8);
		int32_t L_9 = ((int32_t)9);
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_10 = (L_8)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		NullCheck(L_10);
		L_10->set_bankSelect_1((uint8_t)((int32_t)128));
		Synthesizer_ReleaseAllHoldPedals_m09DA747E7D9AD591C391C8D51D0FF01D04F2ECB7(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ResetPrograms()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ResetPrograms_m20668FD5DEB8709FCC313791D35981EFD4F91DEE (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	{
		V_0 = 0;
		goto IL_0019;
	}

IL_0005:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_0 = __this->get_synthChannels_27();
		int32_t L_1 = V_0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		NullCheck(L_3);
		L_3->set_program_0((uint8_t)0);
		int32_t L_4 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_4, (int32_t)1));
	}

IL_0019:
	{
		int32_t L_5 = V_0;
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_6 = __this->get_synthChannels_27();
		NullCheck(L_6);
		V_1 = (bool)((((int32_t)L_5) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_6)->max_length))))))? 1 : 0);
		bool L_7 = V_1;
		if (L_7)
		{
			goto IL_0005;
		}
	}
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::GetNext(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_GetNext_mDDA4297865ED9D00FBEFADFFC8535AD2D21F1F73 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___buffer0, const RuntimeMethod* method)
{
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_0 = __this->get_sampleBuffer_17();
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_1 = __this->get_sampleBuffer_17();
		NullCheck(L_1);
		Array_Clear_m174F4957D6DEDB6359835123005304B14E79132E((RuntimeArray *)(RuntimeArray *)L_0, 0, (((int32_t)((int32_t)(((RuntimeArray*)L_1)->max_length)))), /*hidden argument*/NULL);
		Synthesizer_FillWorkingBuffer_mA17FA0EBD99D7F5C5C236297A6DFC8460B437957(__this, /*hidden argument*/NULL);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_2 = ___buffer0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_3 = __this->get_sampleBuffer_17();
		Synthesizer_ConvertWorkingBuffer_m906FF4E3B635EBCFF45FD5205902E442F17DB93A(__this, L_2, L_3, /*hidden argument*/NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::FillWorkingBuffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_FillWorkingBuffer_mA17FA0EBD99D7F5C5C236297A6DFC8460B437957 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_FillWorkingBuffer_mA17FA0EBD99D7F5C5C236297A6DFC8460B437957_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484  V_3;
	memset((&V_3), 0, sizeof(V_3));
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_4 = NULL;
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_5 = NULL;
	bool V_6 = false;
	{
		V_0 = 0;
		V_1 = 0;
		goto IL_013c;
	}

IL_000a:
	{
		Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * L_0 = __this->get_midiEventQueue_28();
		NullCheck(L_0);
		int32_t L_1 = Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_inline(L_0, /*hidden argument*/Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_RuntimeMethod_var);
		V_6 = (bool)((((int32_t)((((int32_t)L_1) > ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_2 = V_6;
		if (L_2)
		{
			goto IL_006e;
		}
	}
	{
		V_2 = 0;
		goto IL_005c;
	}

IL_0027:
	{
		Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * L_3 = __this->get_midiEventQueue_28();
		NullCheck(L_3);
		MidiMessage_t79F6BF6576742ABB4C0169C2FF414894D03D5484  L_4 = Queue_1_Dequeue_mC42CAF829668D9D8CFB8B95046A7450284564FCC(L_3, /*hidden argument*/Queue_1_Dequeue_mC42CAF829668D9D8CFB8B95046A7450284564FCC_RuntimeMethod_var);
		V_3 = L_4;
		uint8_t L_5 = (&V_3)->get_channel_1();
		uint8_t L_6 = (&V_3)->get_command_2();
		uint8_t L_7 = (&V_3)->get_data1_3();
		uint8_t L_8 = (&V_3)->get_data2_4();
		Synthesizer_ProcessMidiMessage_mFE72104144D15B8AD2A7354357B4EE8312CA0887(__this, L_5, L_6, L_7, L_8, /*hidden argument*/NULL);
		int32_t L_9 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_9, (int32_t)1));
	}

IL_005c:
	{
		int32_t L_10 = V_2;
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_11 = __this->get_midiEventCounts_29();
		int32_t L_12 = V_1;
		NullCheck(L_11);
		int32_t L_13 = L_12;
		int32_t L_14 = (L_11)->GetAt(static_cast<il2cpp_array_size_t>(L_13));
		V_6 = (bool)((((int32_t)L_10) < ((int32_t)L_14))? 1 : 0);
		bool L_15 = V_6;
		if (L_15)
		{
			goto IL_0027;
		}
	}
	{
	}

IL_006e:
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_16 = __this->get_voiceManager_18();
		NullCheck(L_16);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_17 = L_16->get_activeVoices_3();
		NullCheck(L_17);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_18 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_17, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_4 = L_18;
		goto IL_0116;
	}

IL_0085:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_19 = V_4;
		NullCheck(L_19);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_20 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_19, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		int32_t L_21 = V_0;
		int32_t L_22 = V_0;
		int32_t L_23 = __this->get_microBufferSize_25();
		int32_t L_24 = __this->get_audioChannels_19();
		NullCheck(L_20);
		Voice_Process_m00ABF34E8418905D5F60FEB40A1B3184E69E3291(L_20, L_21, ((int32_t)il2cpp_codegen_add((int32_t)L_22, (int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_23, (int32_t)L_24)))), /*hidden argument*/NULL);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_25 = V_4;
		NullCheck(L_25);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_26 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_25, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_26);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_27 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_26, /*hidden argument*/NULL);
		NullCheck(L_27);
		int32_t L_28 = L_27->get_state_4();
		V_6 = (bool)((((int32_t)((((int32_t)L_28) == ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_29 = V_6;
		if (L_29)
		{
			goto IL_010a;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_30 = V_4;
		V_5 = L_30;
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_31 = V_4;
		NullCheck(L_31);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_32 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_31, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_4 = L_32;
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_33 = __this->get_voiceManager_18();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_34 = V_5;
		NullCheck(L_34);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_35 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_34, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_33);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(L_33, L_35, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_36 = __this->get_voiceManager_18();
		NullCheck(L_36);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_37 = L_36->get_activeVoices_3();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_38 = V_5;
		NullCheck(L_37);
		LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6(L_37, L_38, /*hidden argument*/LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6_RuntimeMethod_var);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_39 = __this->get_voiceManager_18();
		NullCheck(L_39);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_40 = L_39->get_freeVoices_2();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_41 = V_5;
		NullCheck(L_40);
		LinkedList_1_AddFirst_mF0C1BDD5A5B6AE94C12282626D65C8696B3D6CC7(L_40, L_41, /*hidden argument*/LinkedList_1_AddFirst_mF0C1BDD5A5B6AE94C12282626D65C8696B3D6CC7_RuntimeMethod_var);
		goto IL_0115;
	}

IL_010a:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_42 = V_4;
		NullCheck(L_42);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_43 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_42, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_4 = L_43;
	}

IL_0115:
	{
	}

IL_0116:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_44 = V_4;
		V_6 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_44) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_45 = V_6;
		if (L_45)
		{
			goto IL_0085;
		}
	}
	{
		int32_t L_46 = V_0;
		int32_t L_47 = __this->get_microBufferSize_25();
		int32_t L_48 = __this->get_audioChannels_19();
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_46, (int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_47, (int32_t)L_48))));
		int32_t L_49 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_49, (int32_t)1));
	}

IL_013c:
	{
		int32_t L_50 = V_1;
		int32_t L_51 = __this->get_microBufferCount_26();
		V_6 = (bool)((((int32_t)L_50) < ((int32_t)L_51))? 1 : 0);
		bool L_52 = V_6;
		if (L_52)
		{
			goto IL_000a;
		}
	}
	{
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_53 = __this->get_midiEventCounts_29();
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_54 = __this->get_midiEventCounts_29();
		NullCheck(L_54);
		Array_Clear_m174F4957D6DEDB6359835123005304B14E79132E((RuntimeArray *)(RuntimeArray *)L_53, 0, (((int32_t)((int32_t)(((RuntimeArray*)L_54)->max_length)))), /*hidden argument*/NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ConvertWorkingBuffer(System.Single[],System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ConvertWorkingBuffer_m906FF4E3B635EBCFF45FD5205902E442F17DB93A (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___to0, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___from1, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	float V_1 = 0.0f;
	bool V_2 = false;
	{
		V_0 = 0;
		goto IL_0029;
	}

IL_0005:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_0 = ___from1;
		int32_t L_1 = V_0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		float L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		float L_4 = __this->get_mainVolume_23();
		float L_5 = SynthHelper_Clamp_m5647B8E90763F524A8AA958F061A8C96C2B9423B(((float)il2cpp_codegen_multiply((float)L_3, (float)L_4)), (-1.0f), (1.0f), /*hidden argument*/NULL);
		V_1 = L_5;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_6 = ___to0;
		int32_t L_7 = V_0;
		float L_8 = V_1;
		NullCheck(L_6);
		(L_6)->SetAt(static_cast<il2cpp_array_size_t>(L_7), (float)L_8);
		int32_t L_9 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_9, (int32_t)1));
	}

IL_0029:
	{
		int32_t L_10 = V_0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_11 = ___from1;
		NullCheck(L_11);
		V_2 = (bool)((((int32_t)L_10) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_11)->max_length))))))? 1 : 0);
		bool L_12 = V_2;
		if (L_12)
		{
			goto IL_0005;
		}
	}
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::NoteOn(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_NoteOn_mED47CC4A3C3105E86AF9A299B0824D5425499C3F (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, int32_t ___note1, int32_t ___velocity2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_NoteOn_mED47CC4A3C3105E86AF9A299B0824D5425499C3F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * V_0 = NULL;
	Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * V_1 = NULL;
	int32_t V_2 = 0;
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_3 = NULL;
	int32_t V_4 = 0;
	bool V_5 = false;
	int32_t V_6 = 0;
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_7 = NULL;
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * V_8 = NULL;
	bool V_9 = false;
	int32_t G_B19_0 = 0;
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_0 = __this->get_synthChannels_27();
		int32_t L_1 = ___channel0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		V_0 = L_3;
		PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * L_4 = __this->get_bank_21();
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_5 = V_0;
		NullCheck(L_5);
		uint8_t L_6 = L_5->get_bankSelect_1();
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_7 = V_0;
		NullCheck(L_7);
		uint8_t L_8 = L_7->get_program_0();
		NullCheck(L_4);
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_9 = PatchBank_GetPatch_mF7D10E9652F19500B38F82A1BBA0799DF7C6318C(L_4, L_6, L_8, /*hidden argument*/NULL);
		V_1 = L_9;
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_10 = V_1;
		V_9 = (bool)((((int32_t)((((RuntimeObject*)(Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 *)L_10) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_11 = V_9;
		if (L_11)
		{
			goto IL_0034;
		}
	}
	{
		goto IL_026e;
	}

IL_0034:
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_12 = V_1;
		V_9 = (bool)((((int32_t)((!(((RuntimeObject*)(MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C *)((MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C *)IsInstClass((RuntimeObject*)L_12, MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C_il2cpp_TypeInfo_var))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_13 = V_9;
		if (L_13)
		{
			goto IL_005f;
		}
	}
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_14 = V_1;
		int32_t L_15 = ___channel0;
		int32_t L_16 = ___note1;
		int32_t L_17 = ___velocity2;
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_18 = __this->get_layerList_30();
		NullCheck(((MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C *)CastclassClass((RuntimeObject*)L_14, MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C_il2cpp_TypeInfo_var)));
		int32_t L_19 = MultiPatch_FindPatches_mFEACBF85B11F80A453E48DDFDB9E23C6459F673F(((MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C *)CastclassClass((RuntimeObject*)L_14, MultiPatch_t0C0028C70A559BFE6C25F47B36BD084CD4917C0C_il2cpp_TypeInfo_var)), L_15, L_16, L_17, L_18, /*hidden argument*/NULL);
		V_2 = L_19;
		goto IL_006c;
	}

IL_005f:
	{
		V_2 = 1;
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_20 = __this->get_layerList_30();
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_21 = V_1;
		NullCheck(L_20);
		ArrayElementTypeCheck (L_20, L_21);
		(L_20)->SetAt(static_cast<il2cpp_array_size_t>(0), (Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 *)L_21);
	}

IL_006c:
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_22 = __this->get_voiceManager_18();
		NullCheck(L_22);
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_23 = L_22->get_registry_4();
		int32_t L_24 = ___channel0;
		int32_t L_25 = ___note1;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_23);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_26 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_23)->GetAt(L_24, L_25);
		V_9 = (bool)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_26) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0);
		bool L_27 = V_9;
		if (L_27)
		{
			goto IL_00ce;
		}
	}
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_28 = __this->get_voiceManager_18();
		NullCheck(L_28);
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_29 = L_28->get_registry_4();
		int32_t L_30 = ___channel0;
		int32_t L_31 = ___note1;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_29);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_32 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_29)->GetAt(L_30, L_31);
		V_3 = L_32;
		goto IL_00b2;
	}

IL_009d:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_33 = V_3;
		NullCheck(L_33);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_34 = L_33->get_Value_0();
		NullCheck(L_34);
		Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC(L_34, /*hidden argument*/NULL);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_35 = V_3;
		NullCheck(L_35);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_36 = L_35->get_Next_1();
		V_3 = L_36;
	}

IL_00b2:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_37 = V_3;
		V_9 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_37) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_38 = V_9;
		if (L_38)
		{
			goto IL_009d;
		}
	}
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_39 = __this->get_voiceManager_18();
		int32_t L_40 = ___channel0;
		int32_t L_41 = ___note1;
		NullCheck(L_39);
		VoiceManager_RemoveFromRegistry_mF4C348706F109014BC7594B960D24307B9539718(L_39, L_40, L_41, /*hidden argument*/NULL);
	}

IL_00ce:
	{
		V_4 = 0;
		goto IL_01c6;
	}

IL_00d6:
	{
		V_5 = (bool)1;
		int32_t L_42 = V_4;
		V_6 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_42, (int32_t)1));
		goto IL_0117;
	}

IL_00e2:
	{
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_43 = __this->get_layerList_30();
		int32_t L_44 = V_4;
		NullCheck(L_43);
		int32_t L_45 = L_44;
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_46 = (L_43)->GetAt(static_cast<il2cpp_array_size_t>(L_45));
		NullCheck(L_46);
		int32_t L_47 = Patch_get_ExclusiveGroupTarget_m5369FE534BE7BDCB4B057B7B4CD570BE3FC47146(L_46, /*hidden argument*/NULL);
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_48 = __this->get_layerList_30();
		int32_t L_49 = V_6;
		NullCheck(L_48);
		int32_t L_50 = L_49;
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_51 = (L_48)->GetAt(static_cast<il2cpp_array_size_t>(L_50));
		NullCheck(L_51);
		int32_t L_52 = Patch_get_ExclusiveGroupTarget_m5369FE534BE7BDCB4B057B7B4CD570BE3FC47146(L_51, /*hidden argument*/NULL);
		V_9 = (bool)((((int32_t)((((int32_t)L_47) == ((int32_t)L_52))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_53 = V_9;
		if (L_53)
		{
			goto IL_0110;
		}
	}
	{
		V_5 = (bool)0;
		goto IL_0125;
	}

IL_0110:
	{
		int32_t L_54 = V_6;
		V_6 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_54, (int32_t)1));
	}

IL_0117:
	{
		int32_t L_55 = V_6;
		V_9 = (bool)((((int32_t)((((int32_t)L_55) < ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_56 = V_9;
		if (L_56)
		{
			goto IL_00e2;
		}
	}

IL_0125:
	{
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_57 = __this->get_layerList_30();
		int32_t L_58 = V_4;
		NullCheck(L_57);
		int32_t L_59 = L_58;
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_60 = (L_57)->GetAt(static_cast<il2cpp_array_size_t>(L_59));
		NullCheck(L_60);
		int32_t L_61 = Patch_get_ExclusiveGroupTarget_m5369FE534BE7BDCB4B057B7B4CD570BE3FC47146(L_60, /*hidden argument*/NULL);
		if (!L_61)
		{
			goto IL_013c;
		}
	}
	{
		bool L_62 = V_5;
		G_B19_0 = ((((int32_t)L_62) == ((int32_t)0))? 1 : 0);
		goto IL_013d;
	}

IL_013c:
	{
		G_B19_0 = 1;
	}

IL_013d:
	{
		V_9 = (bool)G_B19_0;
		bool L_63 = V_9;
		if (L_63)
		{
			goto IL_01bf;
		}
	}
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_64 = __this->get_voiceManager_18();
		NullCheck(L_64);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_65 = L_64->get_activeVoices_3();
		NullCheck(L_65);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_66 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_65, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_7 = L_66;
		goto IL_01b0;
	}

IL_0159:
	{
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_67 = __this->get_layerList_30();
		int32_t L_68 = V_4;
		NullCheck(L_67);
		int32_t L_69 = L_68;
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_70 = (L_67)->GetAt(static_cast<il2cpp_array_size_t>(L_69));
		NullCheck(L_70);
		int32_t L_71 = Patch_get_ExclusiveGroupTarget_m5369FE534BE7BDCB4B057B7B4CD570BE3FC47146(L_70, /*hidden argument*/NULL);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_72 = V_7;
		NullCheck(L_72);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_73 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_72, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_73);
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_74 = Voice_get_Patch_mD8E479EBB586F5AB6A190C02D1F990CE9083A5D1(L_73, /*hidden argument*/NULL);
		NullCheck(L_74);
		int32_t L_75 = Patch_get_ExclusiveGroup_mA5E49C9E9B97AE3C523236194A17CD3C8937AAF8(L_74, /*hidden argument*/NULL);
		V_9 = (bool)((((int32_t)((((int32_t)L_71) == ((int32_t)L_75))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_76 = V_9;
		if (L_76)
		{
			goto IL_01a6;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_77 = V_7;
		NullCheck(L_77);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_78 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_77, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_78);
		Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC(L_78, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_79 = __this->get_voiceManager_18();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_80 = V_7;
		NullCheck(L_80);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_81 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_80, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_79);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(L_79, L_81, /*hidden argument*/NULL);
	}

IL_01a6:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_82 = V_7;
		NullCheck(L_82);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_83 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_82, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_7 = L_83;
	}

IL_01b0:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_84 = V_7;
		V_9 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_84) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_85 = V_9;
		if (L_85)
		{
			goto IL_0159;
		}
	}
	{
	}

IL_01bf:
	{
		int32_t L_86 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_86, (int32_t)1));
	}

IL_01c6:
	{
		int32_t L_87 = V_4;
		int32_t L_88 = V_2;
		V_9 = (bool)((((int32_t)L_87) < ((int32_t)L_88))? 1 : 0);
		bool L_89 = V_9;
		if (L_89)
		{
			goto IL_00d6;
		}
	}
	{
		V_4 = 0;
		goto IL_0243;
	}

IL_01d9:
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_90 = __this->get_voiceManager_18();
		NullCheck(L_90);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_91 = VoiceManager_GetFreeVoice_mFCEE90FF0C773A45F0DE6AF229D2F7AA1B88639E(L_90, /*hidden argument*/NULL);
		V_8 = L_91;
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_92 = V_8;
		V_9 = (bool)((((int32_t)((((RuntimeObject*)(Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)L_92) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_93 = V_9;
		if (L_93)
		{
			goto IL_01f7;
		}
	}
	{
		goto IL_024e;
	}

IL_01f7:
	{
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_94 = V_8;
		int32_t L_95 = ___channel0;
		int32_t L_96 = ___note1;
		int32_t L_97 = ___velocity2;
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_98 = __this->get_layerList_30();
		int32_t L_99 = V_4;
		NullCheck(L_98);
		int32_t L_100 = L_99;
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_101 = (L_98)->GetAt(static_cast<il2cpp_array_size_t>(L_100));
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_102 = __this->get_synthChannels_27();
		int32_t L_103 = ___channel0;
		NullCheck(L_102);
		int32_t L_104 = L_103;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_105 = (L_102)->GetAt(static_cast<il2cpp_array_size_t>(L_104));
		NullCheck(L_94);
		Voice_Configure_m1E88AADA9F95F0F6D3F9612100614B0CE5A91AD0(L_94, L_95, L_96, L_97, L_101, L_105, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_106 = __this->get_voiceManager_18();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_107 = V_8;
		NullCheck(L_106);
		VoiceManager_AddToRegistry_m7B92602F3DD706215F8B2549A2E206E46D9E213C(L_106, L_107, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_108 = __this->get_voiceManager_18();
		NullCheck(L_108);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_109 = L_108->get_activeVoices_3();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_110 = V_8;
		NullCheck(L_109);
		LinkedList_1_AddLast_m726CDCE67E7FFAED24B9764BA1413870450FAA03(L_109, L_110, /*hidden argument*/LinkedList_1_AddLast_m726CDCE67E7FFAED24B9764BA1413870450FAA03_RuntimeMethod_var);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_111 = V_8;
		NullCheck(L_111);
		Voice_Start_mA06B8A91978ADEE54A6A942C1D400C628D2F1EED(L_111, /*hidden argument*/NULL);
		int32_t L_112 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_112, (int32_t)1));
	}

IL_0243:
	{
		int32_t L_113 = V_4;
		int32_t L_114 = V_2;
		V_9 = (bool)((((int32_t)L_113) < ((int32_t)L_114))? 1 : 0);
		bool L_115 = V_9;
		if (L_115)
		{
			goto IL_01d9;
		}
	}

IL_024e:
	{
		V_4 = 0;
		goto IL_0263;
	}

IL_0253:
	{
		PatchU5BU5D_t37BEB5B5F237DF6A69BB329CB7F73BB159859A34* L_116 = __this->get_layerList_30();
		int32_t L_117 = V_4;
		NullCheck(L_116);
		ArrayElementTypeCheck (L_116, NULL);
		(L_116)->SetAt(static_cast<il2cpp_array_size_t>(L_117), (Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 *)NULL);
		int32_t L_118 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_118, (int32_t)1));
	}

IL_0263:
	{
		int32_t L_119 = V_4;
		int32_t L_120 = V_2;
		V_9 = (bool)((((int32_t)L_119) < ((int32_t)L_120))? 1 : 0);
		bool L_121 = V_9;
		if (L_121)
		{
			goto IL_0253;
		}
	}

IL_026e:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::NoteOff(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_NoteOff_mF877C7142704DC64276849E65E343B59172C2670 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, int32_t ___note1, const RuntimeMethod* method)
{
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_0 = NULL;
	bool V_1 = false;
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_0 = __this->get_synthChannels_27();
		int32_t L_1 = ___channel0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		NullCheck(L_3);
		bool L_4 = L_3->get_holdPedal_12();
		V_1 = (bool)((((int32_t)L_4) == ((int32_t)0))? 1 : 0);
		bool L_5 = V_1;
		if (L_5)
		{
			goto IL_0053;
		}
	}
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_6 = __this->get_voiceManager_18();
		NullCheck(L_6);
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_7 = L_6->get_registry_4();
		int32_t L_8 = ___channel0;
		int32_t L_9 = ___note1;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_7);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_10 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_7)->GetAt(L_8, L_9);
		V_0 = L_10;
		goto IL_0045;
	}

IL_002b:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_11 = V_0;
		NullCheck(L_11);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_12 = L_11->get_Value_0();
		NullCheck(L_12);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_13 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_12, /*hidden argument*/NULL);
		NullCheck(L_13);
		L_13->set_noteOffPending_3((bool)1);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_14 = V_0;
		NullCheck(L_14);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_15 = L_14->get_Next_1();
		V_0 = L_15;
	}

IL_0045:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_16 = V_0;
		V_1 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_16) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_17 = V_1;
		if (L_17)
		{
			goto IL_002b;
		}
	}
	{
		goto IL_0098;
	}

IL_0053:
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_18 = __this->get_voiceManager_18();
		NullCheck(L_18);
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_19 = L_18->get_registry_4();
		int32_t L_20 = ___channel0;
		int32_t L_21 = ___note1;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_19);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_22 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_19)->GetAt(L_20, L_21);
		V_0 = L_22;
		goto IL_007e;
	}

IL_0069:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_23 = V_0;
		NullCheck(L_23);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_24 = L_23->get_Value_0();
		NullCheck(L_24);
		Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC(L_24, /*hidden argument*/NULL);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_25 = V_0;
		NullCheck(L_25);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_26 = L_25->get_Next_1();
		V_0 = L_26;
	}

IL_007e:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_27 = V_0;
		V_1 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_27) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_28 = V_1;
		if (L_28)
		{
			goto IL_0069;
		}
	}
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_29 = __this->get_voiceManager_18();
		int32_t L_30 = ___channel0;
		int32_t L_31 = ___note1;
		NullCheck(L_29);
		VoiceManager_RemoveFromRegistry_mF4C348706F109014BC7594B960D24307B9539718(L_29, L_30, L_31, /*hidden argument*/NULL);
	}

IL_0098:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::NoteOffAll(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, bool ___immediate0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_0 = NULL;
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_1 = NULL;
	VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * V_2 = NULL;
	bool V_3 = false;
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_0 = __this->get_voiceManager_18();
		NullCheck(L_0);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_1 = L_0->get_activeVoices_3();
		NullCheck(L_1);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_2 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_1, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_0 = L_2;
		bool L_3 = ___immediate0;
		V_3 = (bool)((((int32_t)L_3) == ((int32_t)0))? 1 : 0);
		bool L_4 = V_3;
		if (L_4)
		{
			goto IL_0072;
		}
	}
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_5 = __this->get_voiceManager_18();
		NullCheck(L_5);
		VoiceManager_ClearRegistry_m3E58BBD848C9B941AC9C3B038A5E3DBAAD552C34(L_5, /*hidden argument*/NULL);
		goto IL_0064;
	}

IL_0029:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_6 = V_0;
		NullCheck(L_6);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_7 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_6, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_7);
		Voice_StopImmediately_m87339619572893D9AB49AB113BF63EC10C17DF17(L_7, /*hidden argument*/NULL);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_8 = V_0;
		V_1 = L_8;
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_9 = V_0;
		NullCheck(L_9);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_10 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_9, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_0 = L_10;
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_11 = __this->get_voiceManager_18();
		NullCheck(L_11);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_12 = L_11->get_activeVoices_3();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_13 = V_1;
		NullCheck(L_12);
		LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6(L_12, L_13, /*hidden argument*/LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6_RuntimeMethod_var);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_14 = __this->get_voiceManager_18();
		NullCheck(L_14);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_15 = L_14->get_freeVoices_2();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_16 = V_1;
		NullCheck(L_15);
		LinkedList_1_AddFirst_mF0C1BDD5A5B6AE94C12282626D65C8696B3D6CC7(L_15, L_16, /*hidden argument*/LinkedList_1_AddFirst_mF0C1BDD5A5B6AE94C12282626D65C8696B3D6CC7_RuntimeMethod_var);
	}

IL_0064:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_17 = V_0;
		V_3 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_17) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_18 = V_3;
		if (L_18)
		{
			goto IL_0029;
		}
	}
	{
		goto IL_00ec;
	}

IL_0072:
	{
		goto IL_00e0;
	}

IL_0075:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_19 = V_0;
		NullCheck(L_19);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_20 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_19, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_20);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_21 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_20, /*hidden argument*/NULL);
		V_2 = L_21;
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_22 = V_2;
		NullCheck(L_22);
		int32_t L_23 = L_22->get_state_4();
		V_3 = (bool)((((int32_t)((((int32_t)L_23) == ((int32_t)2))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_24 = V_3;
		if (L_24)
		{
			goto IL_00d8;
		}
	}
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_25 = __this->get_synthChannels_27();
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_26 = V_2;
		NullCheck(L_26);
		int32_t L_27 = L_26->get_channel_0();
		NullCheck(L_25);
		int32_t L_28 = L_27;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_29 = (L_25)->GetAt(static_cast<il2cpp_array_size_t>(L_28));
		NullCheck(L_29);
		bool L_30 = L_29->get_holdPedal_12();
		V_3 = (bool)((((int32_t)L_30) == ((int32_t)0))? 1 : 0);
		bool L_31 = V_3;
		if (L_31)
		{
			goto IL_00b7;
		}
	}
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_32 = V_2;
		NullCheck(L_32);
		L_32->set_noteOffPending_3((bool)1);
		goto IL_00d7;
	}

IL_00b7:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_33 = V_0;
		NullCheck(L_33);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_34 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_33, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_34);
		Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC(L_34, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_35 = __this->get_voiceManager_18();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_36 = V_0;
		NullCheck(L_36);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_37 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_36, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_35);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(L_35, L_37, /*hidden argument*/NULL);
	}

IL_00d7:
	{
	}

IL_00d8:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_38 = V_0;
		NullCheck(L_38);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_39 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_38, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_0 = L_39;
	}

IL_00e0:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_40 = V_0;
		V_3 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_40) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_41 = V_3;
		if (L_41)
		{
			goto IL_0075;
		}
	}
	{
	}

IL_00ec:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ProcessMidiMessage(System.Int32,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ProcessMidiMessage_mFE72104144D15B8AD2A7354357B4EE8312CA0887 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, int32_t ___command1, int32_t ___data12, int32_t ___data23, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	int16_t V_2 = 0;
	SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * G_B35_0 = NULL;
	SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * G_B34_0 = NULL;
	int32_t G_B36_0 = 0;
	SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * G_B36_1 = NULL;
	int32_t G_B49_0 = 0;
	{
		int32_t L_0 = ___command1;
		V_0 = L_0;
		int32_t L_1 = V_0;
		if ((((int32_t)L_1) > ((int32_t)((int32_t)176))))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)((int32_t)128))))
		{
			goto IL_004e;
		}
	}
	{
		int32_t L_3 = V_0;
		if ((((int32_t)L_3) == ((int32_t)((int32_t)144))))
		{
			goto IL_005c;
		}
	}
	{
		int32_t L_4 = V_0;
		if ((((int32_t)L_4) == ((int32_t)((int32_t)176))))
		{
			goto IL_0083;
		}
	}
	{
		goto IL_05a7;
	}

IL_0028:
	{
		int32_t L_5 = V_0;
		if ((((int32_t)L_5) == ((int32_t)((int32_t)192))))
		{
			goto IL_0549;
		}
	}
	{
		int32_t L_6 = V_0;
		if ((((int32_t)L_6) == ((int32_t)((int32_t)208))))
		{
			goto IL_055a;
		}
	}
	{
		int32_t L_7 = V_0;
		if ((((int32_t)L_7) == ((int32_t)((int32_t)224))))
		{
			goto IL_056c;
		}
	}
	{
		goto IL_05a7;
	}

IL_004e:
	{
		int32_t L_8 = ___channel0;
		int32_t L_9 = ___data12;
		Synthesizer_NoteOff_mF877C7142704DC64276849E65E343B59172C2670(__this, L_8, L_9, /*hidden argument*/NULL);
		goto IL_05a9;
	}

IL_005c:
	{
		int32_t L_10 = ___data23;
		V_1 = (bool)((((int32_t)((((int32_t)L_10) == ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_11 = V_1;
		if (L_11)
		{
			goto IL_0073;
		}
	}
	{
		int32_t L_12 = ___channel0;
		int32_t L_13 = ___data12;
		Synthesizer_NoteOff_mF877C7142704DC64276849E65E343B59172C2670(__this, L_12, L_13, /*hidden argument*/NULL);
		goto IL_007e;
	}

IL_0073:
	{
		int32_t L_14 = ___channel0;
		int32_t L_15 = ___data12;
		int32_t L_16 = ___data23;
		Synthesizer_NoteOn_mED47CC4A3C3105E86AF9A299B0824D5425499C3F(__this, L_14, L_15, L_16, /*hidden argument*/NULL);
	}

IL_007e:
	{
		goto IL_05a9;
	}

IL_0083:
	{
		int32_t L_17 = ___data12;
		V_0 = L_17;
		int32_t L_18 = V_0;
		if ((((int32_t)L_18) > ((int32_t)((int32_t)43))))
		{
			goto IL_00f0;
		}
	}
	{
		int32_t L_19 = V_0;
		if ((((int32_t)L_19) > ((int32_t)((int32_t)11))))
		{
			goto IL_00c2;
		}
	}
	{
		int32_t L_20 = V_0;
		switch (L_20)
		{
			case 0:
			{
				goto IL_0141;
			}
			case 1:
			{
				goto IL_019d;
			}
		}
	}
	{
		int32_t L_21 = V_0;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_21, (int32_t)6)))
		{
			case 0:
			{
				goto IL_03a8;
			}
			case 1:
			{
				goto IL_01ef;
			}
			case 2:
			{
				goto IL_0545;
			}
			case 3:
			{
				goto IL_0545;
			}
			case 4:
			{
				goto IL_0225;
			}
			case 5:
			{
				goto IL_0277;
			}
		}
	}
	{
		goto IL_0545;
	}

IL_00c2:
	{
		int32_t L_22 = V_0;
		if ((((int32_t)L_22) == ((int32_t)((int32_t)33))))
		{
			goto IL_01c6;
		}
	}
	{
		int32_t L_23 = V_0;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_23, (int32_t)((int32_t)38))))
		{
			case 0:
			{
				goto IL_0423;
			}
			case 1:
			{
				goto IL_020a;
			}
			case 2:
			{
				goto IL_0545;
			}
			case 3:
			{
				goto IL_0545;
			}
			case 4:
			{
				goto IL_024e;
			}
			case 5:
			{
				goto IL_02a0;
			}
		}
	}
	{
		goto IL_0545;
	}

IL_00f0:
	{
		int32_t L_24 = V_0;
		if ((((int32_t)L_24) > ((int32_t)((int32_t)68))))
		{
			goto IL_010a;
		}
	}
	{
		int32_t L_25 = V_0;
		if ((((int32_t)L_25) == ((int32_t)((int32_t)64))))
		{
			goto IL_02c9;
		}
	}
	{
		int32_t L_26 = V_0;
		if ((((int32_t)L_26) == ((int32_t)((int32_t)68))))
		{
			goto IL_0306;
		}
	}
	{
		goto IL_0545;
	}

IL_010a:
	{
		int32_t L_27 = V_0;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_27, (int32_t)((int32_t)98))))
		{
			case 0:
			{
				goto IL_033b;
			}
			case 1:
			{
				goto IL_031e;
			}
			case 2:
			{
				goto IL_0373;
			}
			case 3:
			{
				goto IL_0358;
			}
		}
	}
	{
		int32_t L_28 = V_0;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_28, (int32_t)((int32_t)120))))
		{
			case 0:
			{
				goto IL_038e;
			}
			case 1:
			{
				goto IL_0485;
			}
			case 2:
			{
				goto IL_0545;
			}
			case 3:
			{
				goto IL_039b;
			}
		}
	}
	{
		goto IL_0545;
	}

IL_0141:
	{
		int32_t L_29 = ___channel0;
		V_1 = (bool)((((int32_t)((((int32_t)L_29) == ((int32_t)((int32_t)9)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_30 = V_1;
		if (L_30)
		{
			goto IL_0157;
		}
	}
	{
		int32_t L_31 = ___data23;
		___data23 = ((int32_t)il2cpp_codegen_add((int32_t)L_31, (int32_t)((int32_t)128)));
	}

IL_0157:
	{
		PatchBank_tFAAEAE539E98231539B419F451B7352EFE41C225 * L_32 = __this->get_bank_21();
		int32_t L_33 = ___data23;
		NullCheck(L_32);
		bool L_34 = PatchBank_IsBankLoaded_m75149C89714DB65B99E44CCBE6C75EF07FAAE142(L_32, L_33, /*hidden argument*/NULL);
		V_1 = (bool)((((int32_t)L_34) == ((int32_t)0))? 1 : 0);
		bool L_35 = V_1;
		if (L_35)
		{
			goto IL_017d;
		}
	}
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_36 = __this->get_synthChannels_27();
		int32_t L_37 = ___channel0;
		NullCheck(L_36);
		int32_t L_38 = L_37;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_39 = (L_36)->GetAt(static_cast<il2cpp_array_size_t>(L_38));
		int32_t L_40 = ___data23;
		NullCheck(L_39);
		L_39->set_bankSelect_1((uint8_t)(((int32_t)((uint8_t)L_40))));
		goto IL_0198;
	}

IL_017d:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_41 = __this->get_synthChannels_27();
		int32_t L_42 = ___channel0;
		NullCheck(L_41);
		int32_t L_43 = L_42;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_44 = (L_41)->GetAt(static_cast<il2cpp_array_size_t>(L_43));
		int32_t L_45 = ___channel0;
		G_B34_0 = L_44;
		if ((((int32_t)L_45) == ((int32_t)((int32_t)9))))
		{
			G_B35_0 = L_44;
			goto IL_018d;
		}
	}
	{
		G_B36_0 = 0;
		G_B36_1 = G_B34_0;
		goto IL_0192;
	}

IL_018d:
	{
		G_B36_0 = ((int32_t)128);
		G_B36_1 = G_B35_0;
	}

IL_0192:
	{
		NullCheck(G_B36_1);
		G_B36_1->set_bankSelect_1((uint8_t)G_B36_0);
	}

IL_0198:
	{
		goto IL_0547;
	}

IL_019d:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_46 = __this->get_synthChannels_27();
		int32_t L_47 = ___channel0;
		NullCheck(L_46);
		int32_t L_48 = L_47;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_49 = (L_46)->GetAt(static_cast<il2cpp_array_size_t>(L_48));
		NullCheck(L_49);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_50 = L_49->get_address_of_modRange_6();
		int32_t L_51 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_50, (uint8_t)(((int32_t)((uint8_t)L_51))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_52 = __this->get_synthChannels_27();
		int32_t L_53 = ___channel0;
		NullCheck(L_52);
		int32_t L_54 = L_53;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_55 = (L_52)->GetAt(static_cast<il2cpp_array_size_t>(L_54));
		NullCheck(L_55);
		SynthParameters_UpdateCurrentMod_mA575ADC4219635E77436872A0D375E5B93B9F8DE(L_55, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_01c6:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_56 = __this->get_synthChannels_27();
		int32_t L_57 = ___channel0;
		NullCheck(L_56);
		int32_t L_58 = L_57;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_59 = (L_56)->GetAt(static_cast<il2cpp_array_size_t>(L_58));
		NullCheck(L_59);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_60 = L_59->get_address_of_modRange_6();
		int32_t L_61 = ___data23;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_60, (uint8_t)(((int32_t)((uint8_t)L_61))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_62 = __this->get_synthChannels_27();
		int32_t L_63 = ___channel0;
		NullCheck(L_62);
		int32_t L_64 = L_63;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_65 = (L_62)->GetAt(static_cast<il2cpp_array_size_t>(L_64));
		NullCheck(L_65);
		SynthParameters_UpdateCurrentMod_mA575ADC4219635E77436872A0D375E5B93B9F8DE(L_65, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_01ef:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_66 = __this->get_synthChannels_27();
		int32_t L_67 = ___channel0;
		NullCheck(L_66);
		int32_t L_68 = L_67;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_69 = (L_66)->GetAt(static_cast<il2cpp_array_size_t>(L_68));
		NullCheck(L_69);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_70 = L_69->get_address_of_volume_4();
		int32_t L_71 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_70, (uint8_t)(((int32_t)((uint8_t)L_71))), /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_020a:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_72 = __this->get_synthChannels_27();
		int32_t L_73 = ___channel0;
		NullCheck(L_72);
		int32_t L_74 = L_73;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_75 = (L_72)->GetAt(static_cast<il2cpp_array_size_t>(L_74));
		NullCheck(L_75);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_76 = L_75->get_address_of_volume_4();
		int32_t L_77 = ___data23;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_76, (uint8_t)(((int32_t)((uint8_t)L_77))), /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_0225:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_78 = __this->get_synthChannels_27();
		int32_t L_79 = ___channel0;
		NullCheck(L_78);
		int32_t L_80 = L_79;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_81 = (L_78)->GetAt(static_cast<il2cpp_array_size_t>(L_80));
		NullCheck(L_81);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_82 = L_81->get_address_of_pan_3();
		int32_t L_83 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_82, (uint8_t)(((int32_t)((uint8_t)L_83))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_84 = __this->get_synthChannels_27();
		int32_t L_85 = ___channel0;
		NullCheck(L_84);
		int32_t L_86 = L_85;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_87 = (L_84)->GetAt(static_cast<il2cpp_array_size_t>(L_86));
		NullCheck(L_87);
		SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C(L_87, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_024e:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_88 = __this->get_synthChannels_27();
		int32_t L_89 = ___channel0;
		NullCheck(L_88);
		int32_t L_90 = L_89;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_91 = (L_88)->GetAt(static_cast<il2cpp_array_size_t>(L_90));
		NullCheck(L_91);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_92 = L_91->get_address_of_pan_3();
		int32_t L_93 = ___data23;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_92, (uint8_t)(((int32_t)((uint8_t)L_93))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_94 = __this->get_synthChannels_27();
		int32_t L_95 = ___channel0;
		NullCheck(L_94);
		int32_t L_96 = L_95;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_97 = (L_94)->GetAt(static_cast<il2cpp_array_size_t>(L_96));
		NullCheck(L_97);
		SynthParameters_UpdateCurrentPan_mD4DEFD5A02B006A47B1406415C0B761A5005A40C(L_97, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_0277:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_98 = __this->get_synthChannels_27();
		int32_t L_99 = ___channel0;
		NullCheck(L_98);
		int32_t L_100 = L_99;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_101 = (L_98)->GetAt(static_cast<il2cpp_array_size_t>(L_100));
		NullCheck(L_101);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_102 = L_101->get_address_of_expression_5();
		int32_t L_103 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_102, (uint8_t)(((int32_t)((uint8_t)L_103))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_104 = __this->get_synthChannels_27();
		int32_t L_105 = ___channel0;
		NullCheck(L_104);
		int32_t L_106 = L_105;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_107 = (L_104)->GetAt(static_cast<il2cpp_array_size_t>(L_106));
		NullCheck(L_107);
		SynthParameters_UpdateCurrentVolume_m2BDC5BDA381F6ECD8778AD86BE1B8DEB712FA4C6(L_107, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_02a0:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_108 = __this->get_synthChannels_27();
		int32_t L_109 = ___channel0;
		NullCheck(L_108);
		int32_t L_110 = L_109;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_111 = (L_108)->GetAt(static_cast<il2cpp_array_size_t>(L_110));
		NullCheck(L_111);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_112 = L_111->get_address_of_expression_5();
		int32_t L_113 = ___data23;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_112, (uint8_t)(((int32_t)((uint8_t)L_113))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_114 = __this->get_synthChannels_27();
		int32_t L_115 = ___channel0;
		NullCheck(L_114);
		int32_t L_116 = L_115;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_117 = (L_114)->GetAt(static_cast<il2cpp_array_size_t>(L_116));
		NullCheck(L_117);
		SynthParameters_UpdateCurrentVolume_m2BDC5BDA381F6ECD8778AD86BE1B8DEB712FA4C6(L_117, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_02c9:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_118 = __this->get_synthChannels_27();
		int32_t L_119 = ___channel0;
		NullCheck(L_118);
		int32_t L_120 = L_119;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_121 = (L_118)->GetAt(static_cast<il2cpp_array_size_t>(L_120));
		NullCheck(L_121);
		bool L_122 = L_121->get_holdPedal_12();
		if (!L_122)
		{
			goto IL_02e0;
		}
	}
	{
		int32_t L_123 = ___data23;
		G_B49_0 = ((((int32_t)L_123) > ((int32_t)((int32_t)63)))? 1 : 0);
		goto IL_02e1;
	}

IL_02e0:
	{
		G_B49_0 = 1;
	}

IL_02e1:
	{
		V_1 = (bool)G_B49_0;
		bool L_124 = V_1;
		if (L_124)
		{
			goto IL_02ee;
		}
	}
	{
		int32_t L_125 = ___channel0;
		Synthesizer_ReleaseHoldPedal_mE43D3DCBF5F4728BEC6E8F778D333DFA080E105C(__this, L_125, /*hidden argument*/NULL);
	}

IL_02ee:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_126 = __this->get_synthChannels_27();
		int32_t L_127 = ___channel0;
		NullCheck(L_126);
		int32_t L_128 = L_127;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_129 = (L_126)->GetAt(static_cast<il2cpp_array_size_t>(L_128));
		int32_t L_130 = ___data23;
		NullCheck(L_129);
		L_129->set_holdPedal_12((bool)((((int32_t)L_130) > ((int32_t)((int32_t)63)))? 1 : 0));
		goto IL_0547;
	}

IL_0306:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_131 = __this->get_synthChannels_27();
		int32_t L_132 = ___channel0;
		NullCheck(L_131);
		int32_t L_133 = L_132;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_134 = (L_131)->GetAt(static_cast<il2cpp_array_size_t>(L_133));
		int32_t L_135 = ___data23;
		NullCheck(L_134);
		L_134->set_legatoPedal_13((bool)((((int32_t)L_135) > ((int32_t)((int32_t)63)))? 1 : 0));
		goto IL_0547;
	}

IL_031e:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_136 = __this->get_synthChannels_27();
		int32_t L_137 = ___channel0;
		NullCheck(L_136);
		int32_t L_138 = L_137;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_139 = (L_136)->GetAt(static_cast<il2cpp_array_size_t>(L_138));
		NullCheck(L_139);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_140 = L_139->get_address_of_rpn_14();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_140, (int16_t)((int32_t)16383), /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_033b:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_141 = __this->get_synthChannels_27();
		int32_t L_142 = ___channel0;
		NullCheck(L_141);
		int32_t L_143 = L_142;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_144 = (L_141)->GetAt(static_cast<il2cpp_array_size_t>(L_143));
		NullCheck(L_144);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_145 = L_144->get_address_of_rpn_14();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_145, (int16_t)((int32_t)16383), /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_0358:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_146 = __this->get_synthChannels_27();
		int32_t L_147 = ___channel0;
		NullCheck(L_146);
		int32_t L_148 = L_147;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_149 = (L_146)->GetAt(static_cast<il2cpp_array_size_t>(L_148));
		NullCheck(L_149);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_150 = L_149->get_address_of_rpn_14();
		int32_t L_151 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_150, (uint8_t)(((int32_t)((uint8_t)L_151))), /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_0373:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_152 = __this->get_synthChannels_27();
		int32_t L_153 = ___channel0;
		NullCheck(L_152);
		int32_t L_154 = L_153;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_155 = (L_152)->GetAt(static_cast<il2cpp_array_size_t>(L_154));
		NullCheck(L_155);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_156 = L_155->get_address_of_rpn_14();
		int32_t L_157 = ___data23;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_156, (uint8_t)(((int32_t)((uint8_t)L_157))), /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_038e:
	{
		Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D(__this, (bool)1, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_039b:
	{
		Synthesizer_NoteOffAll_mA649ABF8FB5FD5E0D6152CF4E149FC6C2A77444D(__this, (bool)0, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_03a8:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_158 = __this->get_synthChannels_27();
		int32_t L_159 = ___channel0;
		NullCheck(L_158);
		int32_t L_160 = L_159;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_161 = (L_158)->GetAt(static_cast<il2cpp_array_size_t>(L_160));
		NullCheck(L_161);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_162 = L_161->get_address_of_rpn_14();
		int16_t L_163 = CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_162, /*hidden argument*/NULL);
		V_2 = L_163;
		int16_t L_164 = V_2;
		switch (L_164)
		{
			case 0:
			{
				goto IL_03cf;
			}
			case 1:
			{
				goto IL_03ef;
			}
			case 2:
			{
				goto IL_0407;
			}
		}
	}
	{
		goto IL_041c;
	}

IL_03cf:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_165 = __this->get_synthChannels_27();
		int32_t L_166 = ___channel0;
		NullCheck(L_165);
		int32_t L_167 = L_166;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_168 = (L_165)->GetAt(static_cast<il2cpp_array_size_t>(L_167));
		int32_t L_169 = ___data23;
		NullCheck(L_168);
		L_168->set_pitchBendRangeCoarse_8((uint8_t)(((int32_t)((uint8_t)L_169))));
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_170 = __this->get_synthChannels_27();
		int32_t L_171 = ___channel0;
		NullCheck(L_170);
		int32_t L_172 = L_171;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_173 = (L_170)->GetAt(static_cast<il2cpp_array_size_t>(L_172));
		NullCheck(L_173);
		SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F(L_173, /*hidden argument*/NULL);
		goto IL_041e;
	}

IL_03ef:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_174 = __this->get_synthChannels_27();
		int32_t L_175 = ___channel0;
		NullCheck(L_174);
		int32_t L_176 = L_175;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_177 = (L_174)->GetAt(static_cast<il2cpp_array_size_t>(L_176));
		NullCheck(L_177);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_178 = L_177->get_address_of_masterFineTune_11();
		int32_t L_179 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_178, (uint8_t)(((int32_t)((uint8_t)L_179))), /*hidden argument*/NULL);
		goto IL_041e;
	}

IL_0407:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_180 = __this->get_synthChannels_27();
		int32_t L_181 = ___channel0;
		NullCheck(L_180);
		int32_t L_182 = L_181;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_183 = (L_180)->GetAt(static_cast<il2cpp_array_size_t>(L_182));
		int32_t L_184 = ___data23;
		NullCheck(L_183);
		L_183->set_masterCoarseTune_10((((int16_t)((int16_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_184, (int32_t)((int32_t)64)))))));
		goto IL_041e;
	}

IL_041c:
	{
		goto IL_041e;
	}

IL_041e:
	{
		goto IL_0547;
	}

IL_0423:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_185 = __this->get_synthChannels_27();
		int32_t L_186 = ___channel0;
		NullCheck(L_185);
		int32_t L_187 = L_186;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_188 = (L_185)->GetAt(static_cast<il2cpp_array_size_t>(L_187));
		NullCheck(L_188);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_189 = L_188->get_address_of_rpn_14();
		int16_t L_190 = CCValue_get_Combined_m2ABC9A182E08718D7D2610244A7DAC3369C672D7((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_189, /*hidden argument*/NULL);
		V_2 = L_190;
		int16_t L_191 = V_2;
		switch (L_191)
		{
			case 0:
			{
				goto IL_0446;
			}
			case 1:
			{
				goto IL_0466;
			}
		}
	}
	{
		goto IL_047e;
	}

IL_0446:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_192 = __this->get_synthChannels_27();
		int32_t L_193 = ___channel0;
		NullCheck(L_192);
		int32_t L_194 = L_193;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_195 = (L_192)->GetAt(static_cast<il2cpp_array_size_t>(L_194));
		int32_t L_196 = ___data23;
		NullCheck(L_195);
		L_195->set_pitchBendRangeFine_9((uint8_t)(((int32_t)((uint8_t)L_196))));
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_197 = __this->get_synthChannels_27();
		int32_t L_198 = ___channel0;
		NullCheck(L_197);
		int32_t L_199 = L_198;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_200 = (L_197)->GetAt(static_cast<il2cpp_array_size_t>(L_199));
		NullCheck(L_200);
		SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F(L_200, /*hidden argument*/NULL);
		goto IL_0480;
	}

IL_0466:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_201 = __this->get_synthChannels_27();
		int32_t L_202 = ___channel0;
		NullCheck(L_201);
		int32_t L_203 = L_202;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_204 = (L_201)->GetAt(static_cast<il2cpp_array_size_t>(L_203));
		NullCheck(L_204);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_205 = L_204->get_address_of_masterFineTune_11();
		int32_t L_206 = ___data23;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_205, (uint8_t)(((int32_t)((uint8_t)L_206))), /*hidden argument*/NULL);
		goto IL_0480;
	}

IL_047e:
	{
		goto IL_0480;
	}

IL_0480:
	{
		goto IL_0547;
	}

IL_0485:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_207 = __this->get_synthChannels_27();
		int32_t L_208 = ___channel0;
		NullCheck(L_207);
		int32_t L_209 = L_208;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_210 = (L_207)->GetAt(static_cast<il2cpp_array_size_t>(L_209));
		NullCheck(L_210);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_211 = L_210->get_address_of_expression_5();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_211, (int16_t)((int32_t)16383), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_212 = __this->get_synthChannels_27();
		int32_t L_213 = ___channel0;
		NullCheck(L_212);
		int32_t L_214 = L_213;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_215 = (L_212)->GetAt(static_cast<il2cpp_array_size_t>(L_214));
		NullCheck(L_215);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_216 = L_215->get_address_of_modRange_6();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_216, (int16_t)0, /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_217 = __this->get_synthChannels_27();
		int32_t L_218 = ___channel0;
		NullCheck(L_217);
		int32_t L_219 = L_218;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_220 = (L_217)->GetAt(static_cast<il2cpp_array_size_t>(L_219));
		NullCheck(L_220);
		bool L_221 = L_220->get_holdPedal_12();
		V_1 = (bool)((((int32_t)L_221) == ((int32_t)0))? 1 : 0);
		bool L_222 = V_1;
		if (L_222)
		{
			goto IL_04cd;
		}
	}
	{
		int32_t L_223 = ___channel0;
		Synthesizer_ReleaseHoldPedal_mE43D3DCBF5F4728BEC6E8F778D333DFA080E105C(__this, L_223, /*hidden argument*/NULL);
	}

IL_04cd:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_224 = __this->get_synthChannels_27();
		int32_t L_225 = ___channel0;
		NullCheck(L_224);
		int32_t L_226 = L_225;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_227 = (L_224)->GetAt(static_cast<il2cpp_array_size_t>(L_226));
		NullCheck(L_227);
		L_227->set_holdPedal_12((bool)0);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_228 = __this->get_synthChannels_27();
		int32_t L_229 = ___channel0;
		NullCheck(L_228);
		int32_t L_230 = L_229;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_231 = (L_228)->GetAt(static_cast<il2cpp_array_size_t>(L_230));
		NullCheck(L_231);
		L_231->set_legatoPedal_13((bool)0);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_232 = __this->get_synthChannels_27();
		int32_t L_233 = ___channel0;
		NullCheck(L_232);
		int32_t L_234 = L_233;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_235 = (L_232)->GetAt(static_cast<il2cpp_array_size_t>(L_234));
		NullCheck(L_235);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_236 = L_235->get_address_of_rpn_14();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_236, (int16_t)((int32_t)16383), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_237 = __this->get_synthChannels_27();
		int32_t L_238 = ___channel0;
		NullCheck(L_237);
		int32_t L_239 = L_238;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_240 = (L_237)->GetAt(static_cast<il2cpp_array_size_t>(L_239));
		NullCheck(L_240);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_241 = L_240->get_address_of_pitchBend_7();
		CCValue_set_Combined_mD30F1ADB18B44AEA973850928AF49797E56B403D((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_241, (int16_t)((int32_t)8192), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_242 = __this->get_synthChannels_27();
		int32_t L_243 = ___channel0;
		NullCheck(L_242);
		int32_t L_244 = L_243;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_245 = (L_242)->GetAt(static_cast<il2cpp_array_size_t>(L_244));
		NullCheck(L_245);
		L_245->set_channelAfterTouch_2((uint8_t)0);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_246 = __this->get_synthChannels_27();
		int32_t L_247 = ___channel0;
		NullCheck(L_246);
		int32_t L_248 = L_247;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_249 = (L_246)->GetAt(static_cast<il2cpp_array_size_t>(L_248));
		NullCheck(L_249);
		SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F(L_249, /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_250 = __this->get_synthChannels_27();
		int32_t L_251 = ___channel0;
		NullCheck(L_250);
		int32_t L_252 = L_251;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_253 = (L_250)->GetAt(static_cast<il2cpp_array_size_t>(L_252));
		NullCheck(L_253);
		SynthParameters_UpdateCurrentVolume_m2BDC5BDA381F6ECD8778AD86BE1B8DEB712FA4C6(L_253, /*hidden argument*/NULL);
		goto IL_0547;
	}

IL_0545:
	{
		goto IL_05a9;
	}

IL_0547:
	{
		goto IL_05a9;
	}

IL_0549:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_254 = __this->get_synthChannels_27();
		int32_t L_255 = ___channel0;
		NullCheck(L_254);
		int32_t L_256 = L_255;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_257 = (L_254)->GetAt(static_cast<il2cpp_array_size_t>(L_256));
		int32_t L_258 = ___data12;
		NullCheck(L_257);
		L_257->set_program_0((uint8_t)(((int32_t)((uint8_t)L_258))));
		goto IL_05a9;
	}

IL_055a:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_259 = __this->get_synthChannels_27();
		int32_t L_260 = ___channel0;
		NullCheck(L_259);
		int32_t L_261 = L_260;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_262 = (L_259)->GetAt(static_cast<il2cpp_array_size_t>(L_261));
		int32_t L_263 = ___data23;
		NullCheck(L_262);
		L_262->set_channelAfterTouch_2((uint8_t)(((int32_t)((uint8_t)L_263))));
		goto IL_05a9;
	}

IL_056c:
	{
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_264 = __this->get_synthChannels_27();
		int32_t L_265 = ___channel0;
		NullCheck(L_264);
		int32_t L_266 = L_265;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_267 = (L_264)->GetAt(static_cast<il2cpp_array_size_t>(L_266));
		NullCheck(L_267);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_268 = L_267->get_address_of_pitchBend_7();
		int32_t L_269 = ___data23;
		CCValue_set_Coarse_m2EB9CE298DC9D2DA306D3F8FA2C984D63827A150((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_268, (uint8_t)(((int32_t)((uint8_t)L_269))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_270 = __this->get_synthChannels_27();
		int32_t L_271 = ___channel0;
		NullCheck(L_270);
		int32_t L_272 = L_271;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_273 = (L_270)->GetAt(static_cast<il2cpp_array_size_t>(L_272));
		NullCheck(L_273);
		CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 * L_274 = L_273->get_address_of_pitchBend_7();
		int32_t L_275 = ___data12;
		CCValue_set_Fine_m829A23C3C34C780D5FC9368E2F25607A8A70EE9C((CCValue_t052AE70FE7C81E147CF5E9506A89E022905BDFA3 *)L_274, (uint8_t)(((int32_t)((uint8_t)L_275))), /*hidden argument*/NULL);
		SynthParametersU5BU5D_t83E46115FF9957035028881FBA90C0B895B567C0* L_276 = __this->get_synthChannels_27();
		int32_t L_277 = ___channel0;
		NullCheck(L_276);
		int32_t L_278 = L_277;
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_279 = (L_276)->GetAt(static_cast<il2cpp_array_size_t>(L_278));
		NullCheck(L_279);
		SynthParameters_UpdateCurrentPitch_m9BAE15037199DC7C1FAD85BE81FC884E9255180F(L_279, /*hidden argument*/NULL);
		goto IL_05a9;
	}

IL_05a7:
	{
		goto IL_05a9;
	}

IL_05a9:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ReleaseAllHoldPedals()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ReleaseAllHoldPedals_m09DA747E7D9AD591C391C8D51D0FF01D04F2ECB7 (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_ReleaseAllHoldPedals_m09DA747E7D9AD591C391C8D51D0FF01D04F2ECB7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_0 = NULL;
	bool V_1 = false;
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_0 = __this->get_voiceManager_18();
		NullCheck(L_0);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_1 = L_0->get_activeVoices_3();
		NullCheck(L_1);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_2 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_1, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_0 = L_2;
		goto IL_0054;
	}

IL_0014:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_3 = V_0;
		NullCheck(L_3);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_4 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_3, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_4);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_5 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_4, /*hidden argument*/NULL);
		NullCheck(L_5);
		bool L_6 = L_5->get_noteOffPending_3();
		V_1 = (bool)((((int32_t)L_6) == ((int32_t)0))? 1 : 0);
		bool L_7 = V_1;
		if (L_7)
		{
			goto IL_004c;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_8 = V_0;
		NullCheck(L_8);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_9 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_8, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_9);
		Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC(L_9, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_10 = __this->get_voiceManager_18();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_11 = V_0;
		NullCheck(L_11);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_12 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_11, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_10);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(L_10, L_12, /*hidden argument*/NULL);
	}

IL_004c:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_13 = V_0;
		NullCheck(L_13);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_14 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_13, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_0 = L_14;
	}

IL_0054:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_15 = V_0;
		V_1 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_15) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_16 = V_1;
		if (L_16)
		{
			goto IL_0014;
		}
	}
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::ReleaseHoldPedal(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer_ReleaseHoldPedal_mE43D3DCBF5F4728BEC6E8F778D333DFA080E105C (Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * __this, int32_t ___channel0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer_ReleaseHoldPedal_mE43D3DCBF5F4728BEC6E8F778D333DFA080E105C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_0 = NULL;
	bool V_1 = false;
	int32_t G_B4_0 = 0;
	{
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_0 = __this->get_voiceManager_18();
		NullCheck(L_0);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_1 = L_0->get_activeVoices_3();
		NullCheck(L_1);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_2 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_1, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_0 = L_2;
		goto IL_006b;
	}

IL_0014:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_3 = V_0;
		NullCheck(L_3);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_4 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_3, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_4);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_5 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_4, /*hidden argument*/NULL);
		NullCheck(L_5);
		int32_t L_6 = L_5->get_channel_0();
		int32_t L_7 = ___channel0;
		if ((!(((uint32_t)L_6) == ((uint32_t)L_7))))
		{
			goto IL_003d;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_8 = V_0;
		NullCheck(L_8);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_9 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_8, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_9);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_10 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_9, /*hidden argument*/NULL);
		NullCheck(L_10);
		bool L_11 = L_10->get_noteOffPending_3();
		G_B4_0 = ((((int32_t)L_11) == ((int32_t)0))? 1 : 0);
		goto IL_003e;
	}

IL_003d:
	{
		G_B4_0 = 1;
	}

IL_003e:
	{
		V_1 = (bool)G_B4_0;
		bool L_12 = V_1;
		if (L_12)
		{
			goto IL_0063;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_13 = V_0;
		NullCheck(L_13);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_14 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_13, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_14);
		Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC(L_14, /*hidden argument*/NULL);
		VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * L_15 = __this->get_voiceManager_18();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_16 = V_0;
		NullCheck(L_16);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_17 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_16, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_15);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(L_15, L_17, /*hidden argument*/NULL);
	}

IL_0063:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_18 = V_0;
		NullCheck(L_18);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_19 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_18, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_0 = L_19;
	}

IL_006b:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_20 = V_0;
		V_1 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_20) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_21 = V_1;
		if (L_21)
		{
			goto IL_0014;
		}
	}
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Synthesizer::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Synthesizer__cctor_m8AD50A2E6E8C01C129497B28BF21A9B0410751AE (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Synthesizer__cctor_m8AD50A2E6E8C01C129497B28BF21A9B0410751AE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		((Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36_StaticFields*)il2cpp_codegen_static_fields_for(Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36_il2cpp_TypeInfo_var))->set_InterpolationMode_16(1);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::get_Patch()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * Voice_get_Patch_mD8E479EBB586F5AB6A190C02D1F990CE9083A5D1 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * V_0 = NULL;
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_0 = __this->get_patch_0();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_1 = V_0;
		return L_1;
	}
}
// DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::get_VoiceParams()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * V_0 = NULL;
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice__ctor_m11F21BF3B861E923059674538CCAB852CE7396DF (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Voice__ctor_m11F21BF3B861E923059674538CCAB852CE7396DF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 *)il2cpp_codegen_object_new(VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92_il2cpp_TypeInfo_var);
		VoiceParameters__ctor_m0F6140BF4B6EAE4D1A03A011E048C662E8DBA418(L_0, /*hidden argument*/NULL);
		__this->set_voiceparams_1(L_0);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Start()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Start_mA06B8A91978ADEE54A6A942C1D400C628D2F1EED (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		NullCheck(L_0);
		int32_t L_1 = L_0->get_state_4();
		V_0 = (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
		bool L_2 = V_0;
		if (L_2)
		{
			goto IL_0015;
		}
	}
	{
		goto IL_0039;
	}

IL_0015:
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_3 = __this->get_patch_0();
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_4 = __this->get_voiceparams_1();
		NullCheck(L_3);
		bool L_5 = VirtFuncInvoker1< bool, VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * >::Invoke(5 /* System.Boolean DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::Start(DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters) */, L_3, L_4);
		V_0 = (bool)((((int32_t)L_5) == ((int32_t)0))? 1 : 0);
		bool L_6 = V_0;
		if (L_6)
		{
			goto IL_0039;
		}
	}
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_7 = __this->get_voiceparams_1();
		NullCheck(L_7);
		L_7->set_state_4(2);
	}

IL_0039:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Stop()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Stop_m63C7034330568236D960C6B2BE297A102539F9FC (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		NullCheck(L_0);
		int32_t L_1 = L_0->get_state_4();
		V_0 = (bool)((((int32_t)L_1) == ((int32_t)2))? 1 : 0);
		bool L_2 = V_0;
		if (L_2)
		{
			goto IL_0015;
		}
	}
	{
		goto IL_0033;
	}

IL_0015:
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_3 = __this->get_voiceparams_1();
		NullCheck(L_3);
		L_3->set_state_4(1);
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_4 = __this->get_patch_0();
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_5 = __this->get_voiceparams_1();
		NullCheck(L_4);
		VirtActionInvoker1< VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * >::Invoke(6 /* System.Void DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::Stop(DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters) */, L_4, L_5);
	}

IL_0033:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::StopImmediately()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_StopImmediately_m87339619572893D9AB49AB113BF63EC10C17DF17 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		NullCheck(L_0);
		L_0->set_state_4(0);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Process(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Process_m00ABF34E8418905D5F60FEB40A1B3184E69E3291 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, int32_t ___startIndex0, int32_t ___endIndex1, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		NullCheck(L_0);
		int32_t L_1 = L_0->get_state_4();
		V_0 = (bool)((((int32_t)((((int32_t)L_1) == ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_2 = V_0;
		if (L_2)
		{
			goto IL_0018;
		}
	}
	{
		goto IL_002c;
	}

IL_0018:
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_3 = __this->get_patch_0();
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_4 = __this->get_voiceparams_1();
		int32_t L_5 = ___startIndex0;
		int32_t L_6 = ___endIndex1;
		NullCheck(L_3);
		VirtActionInvoker3< VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 *, int32_t, int32_t >::Invoke(4 /* System.Void DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch::Process(DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters,System.Int32,System.Int32) */, L_3, L_4, L_5, L_6);
	}

IL_002c:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::Configure(System.Int32,System.Int32,System.Int32,DaggerfallWorkshop.AudioSynthesis.Bank.Patches.Patch,DaggerfallWorkshop.AudioSynthesis.Synthesis.SynthParameters)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Voice_Configure_m1E88AADA9F95F0F6D3F9612100614B0CE5A91AD0 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, int32_t ___channel0, int32_t ___note1, int32_t ___velocity2, Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * ___patch3, SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * ___synthParams4, const RuntimeMethod* method)
{
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		NullCheck(L_0);
		VoiceParameters_Reset_mAA57CF4C4E6F6D8896958323E4E820258AC4B4B2(L_0, /*hidden argument*/NULL);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_1 = __this->get_voiceparams_1();
		int32_t L_2 = ___channel0;
		NullCheck(L_1);
		L_1->set_channel_0(L_2);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_3 = __this->get_voiceparams_1();
		int32_t L_4 = ___note1;
		NullCheck(L_3);
		L_3->set_note_1(L_4);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_5 = __this->get_voiceparams_1();
		int32_t L_6 = ___velocity2;
		NullCheck(L_5);
		L_5->set_velocity_2(L_6);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_7 = __this->get_voiceparams_1();
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_8 = ___synthParams4;
		NullCheck(L_7);
		L_7->set_synthParams_9(L_8);
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_9 = ___patch3;
		__this->set_patch_0(L_9);
		return;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Voice_ToString_m251F2DC14D1438A36A586C71FA2DDD453CB2DD04 (Voice_t481B233F7BCA5C28D192670FC7590699211A984E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Voice_ToString_m251F2DC14D1438A36A586C71FA2DDD453CB2DD04_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	String_t* G_B2_0 = NULL;
	String_t* G_B2_1 = NULL;
	String_t* G_B1_0 = NULL;
	String_t* G_B1_1 = NULL;
	String_t* G_B3_0 = NULL;
	String_t* G_B3_1 = NULL;
	String_t* G_B3_2 = NULL;
	{
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_0 = __this->get_voiceparams_1();
		NullCheck(L_0);
		String_t* L_1 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_0);
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_2 = __this->get_patch_0();
		G_B1_0 = _stringLiteralF525B595E619268F68255D45F855A35B8E3ACAD0;
		G_B1_1 = L_1;
		if (!L_2)
		{
			G_B2_0 = _stringLiteralF525B595E619268F68255D45F855A35B8E3ACAD0;
			G_B2_1 = L_1;
			goto IL_0026;
		}
	}
	{
		Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 * L_3 = __this->get_patch_0();
		NullCheck(L_3);
		String_t* L_4 = Patch_get_Name_mFD124ADFA88F426F53FF564A68159A8570284CE0(L_3, /*hidden argument*/NULL);
		G_B3_0 = L_4;
		G_B3_1 = G_B1_0;
		G_B3_2 = G_B1_1;
		goto IL_002b;
	}

IL_0026:
	{
		G_B3_0 = _stringLiteral2BE88CA4242C76E8253AC62474851065032D6833;
		G_B3_1 = G_B2_0;
		G_B3_2 = G_B2_1;
	}

IL_002b:
	{
		String_t* L_5 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(G_B3_2, G_B3_1, G_B3_0, /*hidden argument*/NULL);
		V_0 = L_5;
		goto IL_0034;
	}

IL_0034:
	{
		String_t* L_6 = V_0;
		return L_6;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager__ctor_mE544FA76F2141F9F6BE6FD198686AD516D74378D (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, int32_t ___voiceCount0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager__ctor_mE544FA76F2141F9F6BE6FD198686AD516D74378D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* V_0 = NULL;
	int32_t V_1 = 0;
	bool V_2 = false;
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		__this->set_stealingMethod_0(1);
		int32_t L_0 = ___voiceCount0;
		__this->set_polyphony_1(L_0);
		int32_t L_1 = ___voiceCount0;
		VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* L_2 = (VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF*)(VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF*)SZArrayNew(VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF_il2cpp_TypeInfo_var, (uint32_t)L_1);
		__this->set_voicePool_5(L_2);
		int32_t L_3 = ___voiceCount0;
		VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* L_4 = (VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC*)(VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC*)SZArrayNew(VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC_il2cpp_TypeInfo_var, (uint32_t)L_3);
		V_0 = L_4;
		V_1 = 0;
		goto IL_0048;
	}

IL_002d:
	{
		VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* L_5 = __this->get_voicePool_5();
		int32_t L_6 = V_1;
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_7 = (Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)il2cpp_codegen_object_new(Voice_t481B233F7BCA5C28D192670FC7590699211A984E_il2cpp_TypeInfo_var);
		Voice__ctor_m11F21BF3B861E923059674538CCAB852CE7396DF(L_7, /*hidden argument*/NULL);
		NullCheck(L_5);
		ArrayElementTypeCheck (L_5, L_7);
		(L_5)->SetAt(static_cast<il2cpp_array_size_t>(L_6), (Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)L_7);
		VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* L_8 = V_0;
		int32_t L_9 = V_1;
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_10 = (VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)il2cpp_codegen_object_new(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B_il2cpp_TypeInfo_var);
		VoiceNode__ctor_mF4DA85DA34F70AAEBFC3780DFF3B87A01D57BAE5(L_10, /*hidden argument*/NULL);
		NullCheck(L_8);
		ArrayElementTypeCheck (L_8, L_10);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(L_9), (VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_10);
		int32_t L_11 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_11, (int32_t)1));
	}

IL_0048:
	{
		int32_t L_12 = V_1;
		VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* L_13 = __this->get_voicePool_5();
		NullCheck(L_13);
		V_2 = (bool)((((int32_t)L_12) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_13)->max_length))))))? 1 : 0);
		bool L_14 = V_2;
		if (L_14)
		{
			goto IL_002d;
		}
	}
	{
		VoiceNodeU5BU5D_tC9DDD127401F6F175A7712A82F36B6AD0DE41AEC* L_15 = V_0;
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_16 = (Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 *)il2cpp_codegen_object_new(Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3_il2cpp_TypeInfo_var);
		Stack_1__ctor_m6A17077686BD0E0D90E5FF918CEDB67A998021F6(L_16, (RuntimeObject*)(RuntimeObject*)L_15, /*hidden argument*/Stack_1__ctor_m6A17077686BD0E0D90E5FF918CEDB67A998021F6_RuntimeMethod_var);
		__this->set_vnodes_6(L_16);
		VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* L_17 = __this->get_voicePool_5();
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_18 = (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *)il2cpp_codegen_object_new(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261_il2cpp_TypeInfo_var);
		LinkedList_1__ctor_m9B300260CEDD4F71EE5EB13A29F5A1DB7326EF83(L_18, (RuntimeObject*)(RuntimeObject*)L_17, /*hidden argument*/LinkedList_1__ctor_m9B300260CEDD4F71EE5EB13A29F5A1DB7326EF83_RuntimeMethod_var);
		__this->set_freeVoices_2(L_18);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_19 = (LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 *)il2cpp_codegen_object_new(LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261_il2cpp_TypeInfo_var);
		LinkedList_1__ctor_m351D42809C36084085287A7DC9266C18A38EBCBD(L_19, /*hidden argument*/LinkedList_1__ctor_m351D42809C36084085287A7DC9266C18A38EBCBD_RuntimeMethod_var);
		__this->set_activeVoices_3(L_19);
		il2cpp_array_size_t L_21[] = { (il2cpp_array_size_t)((int32_t)16), (il2cpp_array_size_t)((int32_t)128) };
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_20 = (VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)GenArrayNew(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6_il2cpp_TypeInfo_var, L_21);
		__this->set_registry_4((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_20);
		return;
	}
}
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::GetFreeVoice()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Voice_t481B233F7BCA5C28D192670FC7590699211A984E * VoiceManager_GetFreeVoice_mFCEE90FF0C773A45F0DE6AF229D2F7AA1B88639E (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_GetFreeVoice_mFCEE90FF0C773A45F0DE6AF229D2F7AA1B88639E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * V_0 = NULL;
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * V_1 = NULL;
	bool V_2 = false;
	int32_t V_3 = 0;
	{
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_0 = __this->get_freeVoices_2();
		NullCheck(L_0);
		int32_t L_1 = LinkedList_1_get_Count_mB6578B261B94BF2CA14E35C3F412FAC44697646E_inline(L_0, /*hidden argument*/LinkedList_1_get_Count_mB6578B261B94BF2CA14E35C3F412FAC44697646E_RuntimeMethod_var);
		V_2 = (bool)((((int32_t)((((int32_t)L_1) > ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_2 = V_2;
		if (L_2)
		{
			goto IL_0038;
		}
	}
	{
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_3 = __this->get_freeVoices_2();
		NullCheck(L_3);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_4 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_3, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		NullCheck(L_4);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_5 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_4, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		V_0 = L_5;
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_6 = __this->get_freeVoices_2();
		NullCheck(L_6);
		LinkedList_1_RemoveFirst_m38227EF51CB2AC5FD93DD996AFAB4B2AA0A4775F(L_6, /*hidden argument*/LinkedList_1_RemoveFirst_m38227EF51CB2AC5FD93DD996AFAB4B2AA0A4775F_RuntimeMethod_var);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_7 = V_0;
		V_1 = L_7;
		goto IL_0065;
	}

IL_0038:
	{
		int32_t L_8 = __this->get_stealingMethod_0();
		V_3 = L_8;
		int32_t L_9 = V_3;
		switch (L_9)
		{
			case 0:
			{
				goto IL_004f;
			}
			case 1:
			{
				goto IL_0058;
			}
		}
	}
	{
		goto IL_0061;
	}

IL_004f:
	{
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_10 = VoiceManager_StealOldest_mA7E0A19BA1AA22AAA607A495B58E0BB26801F57D(__this, /*hidden argument*/NULL);
		V_1 = L_10;
		goto IL_0065;
	}

IL_0058:
	{
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_11 = VoiceManager_StealQuietestVoice_m7D807BDC795448730B4C6BF94C2D02A424559783(__this, /*hidden argument*/NULL);
		V_1 = L_11;
		goto IL_0065;
	}

IL_0061:
	{
		V_1 = (Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)NULL;
		goto IL_0065;
	}

IL_0065:
	{
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_12 = V_1;
		return L_12;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::AddToRegistry(DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_AddToRegistry_m7B92602F3DD706215F8B2549A2E206E46D9E213C (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___voice0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_AddToRegistry_m7B92602F3DD706215F8B2549A2E206E46D9E213C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_0 = NULL;
	{
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_0 = __this->get_vnodes_6();
		NullCheck(L_0);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_1 = Stack_1_Pop_mC9241F45FA4B326F497400A6638358BB20C79648(L_0, /*hidden argument*/Stack_1_Pop_mC9241F45FA4B326F497400A6638358BB20C79648_RuntimeMethod_var);
		V_0 = L_1;
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_2 = V_0;
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_3 = ___voice0;
		NullCheck(L_2);
		L_2->set_Value_0(L_3);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_4 = V_0;
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_5 = __this->get_registry_4();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_6 = ___voice0;
		NullCheck(L_6);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_7 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_6, /*hidden argument*/NULL);
		NullCheck(L_7);
		int32_t L_8 = L_7->get_channel_0();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_9 = ___voice0;
		NullCheck(L_9);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_10 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_9, /*hidden argument*/NULL);
		NullCheck(L_10);
		int32_t L_11 = L_10->get_note_1();
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_5);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_12 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_5)->GetAt(L_8, L_11);
		NullCheck(L_4);
		L_4->set_Next_1(L_12);
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_13 = __this->get_registry_4();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_14 = ___voice0;
		NullCheck(L_14);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_15 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_14, /*hidden argument*/NULL);
		NullCheck(L_15);
		int32_t L_16 = L_15->get_channel_0();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_17 = ___voice0;
		NullCheck(L_17);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_18 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_17, /*hidden argument*/NULL);
		NullCheck(L_18);
		int32_t L_19 = L_18->get_note_1();
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_20 = V_0;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_13);
		((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_13)->SetAt(L_16, L_19, L_20);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::RemoveFromRegistry(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_RemoveFromRegistry_mF4C348706F109014BC7594B960D24307B9539718 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, int32_t ___channel0, int32_t ___note1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_RemoveFromRegistry_mF4C348706F109014BC7594B960D24307B9539718_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_0 = NULL;
	bool V_1 = false;
	{
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_0 = __this->get_registry_4();
		int32_t L_1 = ___channel0;
		int32_t L_2 = ___note1;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_0);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_3 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_0)->GetAt(L_1, L_2);
		V_0 = L_3;
		goto IL_0027;
	}

IL_0011:
	{
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_4 = __this->get_vnodes_6();
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_5 = V_0;
		NullCheck(L_4);
		Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B(L_4, L_5, /*hidden argument*/Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B_RuntimeMethod_var);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_6 = V_0;
		NullCheck(L_6);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_7 = L_6->get_Next_1();
		V_0 = L_7;
	}

IL_0027:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_8 = V_0;
		V_1 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_8) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_9 = V_1;
		if (L_9)
		{
			goto IL_0011;
		}
	}
	{
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_10 = __this->get_registry_4();
		int32_t L_11 = ___channel0;
		int32_t L_12 = ___note1;
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_10);
		((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_10)->SetAt(L_11, L_12, (VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)NULL);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::RemoveFromRegistry(DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, Voice_t481B233F7BCA5C28D192670FC7590699211A984E * ___voice0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_0 = NULL;
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_1 = NULL;
	bool V_2 = false;
	{
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_0 = __this->get_registry_4();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_1 = ___voice0;
		NullCheck(L_1);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_2 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_1, /*hidden argument*/NULL);
		NullCheck(L_2);
		int32_t L_3 = L_2->get_channel_0();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_4 = ___voice0;
		NullCheck(L_4);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_5 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_4, /*hidden argument*/NULL);
		NullCheck(L_5);
		int32_t L_6 = L_5->get_note_1();
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_0);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_7 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_0)->GetAt(L_3, L_6);
		V_0 = L_7;
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_8 = V_0;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_8) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_9 = V_2;
		if (L_9)
		{
			goto IL_0033;
		}
	}
	{
		goto IL_00c9;
	}

IL_0033:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_10 = V_0;
		NullCheck(L_10);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_11 = L_10->get_Value_0();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_12 = ___voice0;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)L_11) == ((RuntimeObject*)(Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)L_12))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_13 = V_2;
		if (L_13)
		{
			goto IL_007a;
		}
	}
	{
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_14 = __this->get_registry_4();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_15 = ___voice0;
		NullCheck(L_15);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_16 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_15, /*hidden argument*/NULL);
		NullCheck(L_16);
		int32_t L_17 = L_16->get_channel_0();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_18 = ___voice0;
		NullCheck(L_18);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_19 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_18, /*hidden argument*/NULL);
		NullCheck(L_19);
		int32_t L_20 = L_19->get_note_1();
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_21 = V_0;
		NullCheck(L_21);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_22 = L_21->get_Next_1();
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_14);
		((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_14)->SetAt(L_17, L_20, L_22);
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_23 = __this->get_vnodes_6();
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_24 = V_0;
		NullCheck(L_23);
		Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B(L_23, L_24, /*hidden argument*/Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B_RuntimeMethod_var);
		goto IL_00c9;
	}

IL_007a:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_25 = V_0;
		V_1 = L_25;
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_26 = V_0;
		NullCheck(L_26);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_27 = L_26->get_Next_1();
		V_0 = L_27;
		goto IL_00bd;
	}

IL_0086:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_28 = V_0;
		NullCheck(L_28);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_29 = L_28->get_Value_0();
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_30 = ___voice0;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)L_29) == ((RuntimeObject*)(Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)L_30))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_31 = V_2;
		if (L_31)
		{
			goto IL_00b3;
		}
	}
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_32 = V_1;
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_33 = V_0;
		NullCheck(L_33);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_34 = L_33->get_Next_1();
		NullCheck(L_32);
		L_32->set_Next_1(L_34);
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_35 = __this->get_vnodes_6();
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_36 = V_0;
		NullCheck(L_35);
		Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B(L_35, L_36, /*hidden argument*/Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B_RuntimeMethod_var);
		goto IL_00c9;
	}

IL_00b3:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_37 = V_0;
		V_1 = L_37;
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_38 = V_0;
		NullCheck(L_38);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_39 = L_38->get_Next_1();
		V_0 = L_39;
	}

IL_00bd:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_40 = V_0;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_40) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_41 = V_2;
		if (L_41)
		{
			goto IL_0086;
		}
	}
	{
	}

IL_00c9:
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::ClearRegistry()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_ClearRegistry_m3E58BBD848C9B941AC9C3B038A5E3DBAAD552C34 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_ClearRegistry_m3E58BBD848C9B941AC9C3B038A5E3DBAAD552C34_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_0 = NULL;
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_1 = NULL;
	bool V_2 = false;
	{
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_0 = __this->get_activeVoices_3();
		NullCheck(L_0);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_1 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_0, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_0 = L_1;
		goto IL_0096;
	}

IL_0012:
	{
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_2 = __this->get_registry_4();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_3 = V_0;
		NullCheck(L_3);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_4 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_3, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_4);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_5 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_4, /*hidden argument*/NULL);
		NullCheck(L_5);
		int32_t L_6 = L_5->get_channel_0();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_7 = V_0;
		NullCheck(L_7);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_8 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_7, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_8);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_9 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_8, /*hidden argument*/NULL);
		NullCheck(L_9);
		int32_t L_10 = L_9->get_note_1();
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_2);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_11 = ((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_2)->GetAt(L_6, L_10);
		V_1 = L_11;
		goto IL_0057;
	}

IL_0041:
	{
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_12 = __this->get_vnodes_6();
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_13 = V_1;
		NullCheck(L_12);
		Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B(L_12, L_13, /*hidden argument*/Stack_1_Push_m37F1649B27DD42657170C6D892EF01DBFAF68B8B_RuntimeMethod_var);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_14 = V_1;
		NullCheck(L_14);
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_15 = L_14->get_Next_1();
		V_1 = L_15;
	}

IL_0057:
	{
		VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_16 = V_1;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)L_16) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_17 = V_2;
		if (L_17)
		{
			goto IL_0041;
		}
	}
	{
		VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6* L_18 = __this->get_registry_4();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_19 = V_0;
		NullCheck(L_19);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_20 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_19, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_20);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_21 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_20, /*hidden argument*/NULL);
		NullCheck(L_21);
		int32_t L_22 = L_21->get_channel_0();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_23 = V_0;
		NullCheck(L_23);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_24 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_23, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_24);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_25 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_24, /*hidden argument*/NULL);
		NullCheck(L_25);
		int32_t L_26 = L_25->get_note_1();
		NullCheck((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_18);
		((VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)(VoiceNodeU5BU2CU5D_tDB8CADF0C35F3600FB5B8784CCC6C5F3B259FCE6*)L_18)->SetAt(L_22, L_26, (VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B *)NULL);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_27 = V_0;
		NullCheck(L_27);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_28 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_27, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_0 = L_28;
	}

IL_0096:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_29 = V_0;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_29) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_30 = V_2;
		if (L_30)
		{
			goto IL_0012;
		}
	}
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::UnloadPatches()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceManager_UnloadPatches_m0B094424DF30EDE3C004B6EDFB7C97A60F9C32CB (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_UnloadPatches_m0B094424DF30EDE3C004B6EDFB7C97A60F9C32CB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * V_1 = NULL;
	Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	bool V_3 = false;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		V_0 = 0;
		goto IL_0059;
	}

IL_0005:
	{
		VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* L_0 = __this->get_voicePool_5();
		int32_t L_1 = V_0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		NullCheck(L_3);
		Voice_Configure_m1E88AADA9F95F0F6D3F9612100614B0CE5A91AD0(L_3, 0, 0, 0, (Patch_t70233CC16730E7429D44009DA9AF4E4B3E344045 *)NULL, (SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 *)NULL, /*hidden argument*/NULL);
		Stack_1_t3DCD0A7FD34216D961EDC17F0FB5CB950748F1E3 * L_4 = __this->get_vnodes_6();
		NullCheck(L_4);
		Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7  L_5 = Stack_1_GetEnumerator_m75E27371A4143B1C392DFB79E958DA67D10DB975(L_4, /*hidden argument*/Stack_1_GetEnumerator_m75E27371A4143B1C392DFB79E958DA67D10DB975_RuntimeMethod_var);
		V_2 = L_5;
	}

IL_0026:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0037;
		}

IL_0028:
		{
			VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_6 = Enumerator_get_Current_m4592C40C9A75A2BBD09370ADA41D658FE1CD0032((Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 *)(&V_2), /*hidden argument*/Enumerator_get_Current_m4592C40C9A75A2BBD09370ADA41D658FE1CD0032_RuntimeMethod_var);
			V_1 = L_6;
			VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * L_7 = V_1;
			NullCheck(L_7);
			L_7->set_Value_0((Voice_t481B233F7BCA5C28D192670FC7590699211A984E *)NULL);
		}

IL_0037:
		{
			bool L_8 = Enumerator_MoveNext_mA3F181BF1A1468DD6B9F91E3627B9BE3811B8BAF((Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 *)(&V_2), /*hidden argument*/Enumerator_MoveNext_mA3F181BF1A1468DD6B9F91E3627B9BE3811B8BAF_RuntimeMethod_var);
			V_3 = L_8;
			bool L_9 = V_3;
			if (L_9)
			{
				goto IL_0028;
			}
		}

IL_0042:
		{
			IL2CPP_LEAVE(0x53, FINALLY_0044);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0044;
	}

FINALLY_0044:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_mA5A2805F07E244CB7DC592F13D3FCD04AC6BE726((Enumerator_t6F77319C64AA22D5AA117245BF449F30E580E4D7 *)(&V_2), /*hidden argument*/Enumerator_Dispose_mA5A2805F07E244CB7DC592F13D3FCD04AC6BE726_RuntimeMethod_var);
		IL2CPP_END_FINALLY(68)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(68)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
		IL2CPP_JUMP_TBL(0x53, IL_0053)
	}

IL_0053:
	{
		int32_t L_10 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0059:
	{
		int32_t L_11 = V_0;
		VoiceU5BU5D_t689BF4AF858706A88C1F2535F42CB2CFBEB34EAF* L_12 = __this->get_voicePool_5();
		NullCheck(L_12);
		V_3 = (bool)((((int32_t)L_11) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_12)->max_length))))))? 1 : 0);
		bool L_13 = V_3;
		if (L_13)
		{
			goto IL_0005;
		}
	}
	{
		return;
	}
}
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::StealOldest()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Voice_t481B233F7BCA5C28D192670FC7590699211A984E * VoiceManager_StealOldest_mA7E0A19BA1AA22AAA607A495B58E0BB26801F57D (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_StealOldest_mA7E0A19BA1AA22AAA607A495B58E0BB26801F57D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_0 = NULL;
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * V_1 = NULL;
	bool V_2 = false;
	int32_t G_B5_0 = 0;
	{
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_0 = __this->get_activeVoices_3();
		NullCheck(L_0);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_1 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_0, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_0 = L_1;
		goto IL_0016;
	}

IL_000f:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_2 = V_0;
		NullCheck(L_2);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_3 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_2, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_0 = L_3;
	}

IL_0016:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_4 = V_0;
		if (!L_4)
		{
			goto IL_002e;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_5 = V_0;
		NullCheck(L_5);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_6 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_5, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_6);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_7 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_6, /*hidden argument*/NULL);
		NullCheck(L_7);
		int32_t L_8 = L_7->get_state_4();
		G_B5_0 = ((((int32_t)L_8) == ((int32_t)2))? 1 : 0);
		goto IL_002f;
	}

IL_002e:
	{
		G_B5_0 = 0;
	}

IL_002f:
	{
		V_2 = (bool)G_B5_0;
		bool L_9 = V_2;
		if (L_9)
		{
			goto IL_000f;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_10 = V_0;
		V_2 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_10) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_11 = V_2;
		if (L_11)
		{
			goto IL_004b;
		}
	}
	{
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_12 = __this->get_activeVoices_3();
		NullCheck(L_12);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_13 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_12, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_0 = L_13;
	}

IL_004b:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_14 = V_0;
		NullCheck(L_14);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_15 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_14, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(__this, L_15, /*hidden argument*/NULL);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_16 = __this->get_activeVoices_3();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_17 = V_0;
		NullCheck(L_16);
		LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6(L_16, L_17, /*hidden argument*/LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6_RuntimeMethod_var);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_18 = V_0;
		NullCheck(L_18);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_19 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_18, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_19);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_20 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_19, /*hidden argument*/NULL);
		NullCheck(L_20);
		L_20->set_state_4(0);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_21 = V_0;
		NullCheck(L_21);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_22 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_21, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		V_1 = L_22;
		goto IL_007f;
	}

IL_007f:
	{
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_23 = V_1;
		return L_23;
	}
}
// DaggerfallWorkshop.AudioSynthesis.Synthesis.Voice DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager::StealQuietestVoice()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Voice_t481B233F7BCA5C28D192670FC7590699211A984E * VoiceManager_StealQuietestVoice_m7D807BDC795448730B4C6BF94C2D02A424559783 (VoiceManager_t4549D18C4B2368F75D89A683277F917AD0067DD8 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceManager_StealQuietestVoice_m7D807BDC795448730B4C6BF94C2D02A424559783_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	float V_0 = 0.0f;
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_1 = NULL;
	LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * V_2 = NULL;
	float V_3 = 0.0f;
	Voice_t481B233F7BCA5C28D192670FC7590699211A984E * V_4 = NULL;
	bool V_5 = false;
	{
		V_0 = (1000.0f);
		V_1 = (LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)NULL;
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_0 = __this->get_activeVoices_3();
		NullCheck(L_0);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_1 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_0, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_2 = L_1;
		goto IL_005f;
	}

IL_0017:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_2 = V_2;
		NullCheck(L_2);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_3 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_2, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_3);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_4 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_3, /*hidden argument*/NULL);
		NullCheck(L_4);
		int32_t L_5 = L_4->get_state_4();
		V_5 = (bool)((((int32_t)L_5) == ((int32_t)2))? 1 : 0);
		bool L_6 = V_5;
		if (L_6)
		{
			goto IL_0057;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_7 = V_2;
		NullCheck(L_7);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_8 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_7, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_8);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_9 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_8, /*hidden argument*/NULL);
		NullCheck(L_9);
		float L_10 = VoiceParameters_get_CombinedVolume_m3B5EA828CEAD322A9DD4BBF19FEEAD6F950ED6F0(L_9, /*hidden argument*/NULL);
		V_3 = L_10;
		float L_11 = V_3;
		float L_12 = V_0;
		V_5 = (bool)((((int32_t)((((float)L_11) < ((float)L_12))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_13 = V_5;
		if (L_13)
		{
			goto IL_0056;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_14 = V_2;
		V_1 = L_14;
		float L_15 = V_3;
		V_0 = L_15;
	}

IL_0056:
	{
	}

IL_0057:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_16 = V_2;
		NullCheck(L_16);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_17 = LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518(L_16, /*hidden argument*/LinkedListNode_1_get_Next_mA29A33D7A9FB0DE57D999A1AAB47714B8C2B2518_RuntimeMethod_var);
		V_2 = L_17;
	}

IL_005f:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_18 = V_2;
		V_5 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_18) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_19 = V_5;
		if (L_19)
		{
			goto IL_0017;
		}
	}
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_20 = V_1;
		V_5 = (bool)((((int32_t)((((RuntimeObject*)(LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 *)L_20) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_21 = V_5;
		if (L_21)
		{
			goto IL_0085;
		}
	}
	{
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_22 = __this->get_activeVoices_3();
		NullCheck(L_22);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_23 = LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_inline(L_22, /*hidden argument*/LinkedList_1_get_First_mE24ECE56D2737A6AD0C4E49660BE4622F1496653_RuntimeMethod_var);
		V_1 = L_23;
	}

IL_0085:
	{
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_24 = V_1;
		NullCheck(L_24);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_25 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_24, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		VoiceManager_RemoveFromRegistry_mE27762C96B6FAF1BF9F5AF4F32109E4895CEBCE5(__this, L_25, /*hidden argument*/NULL);
		LinkedList_1_tD3E933DE534F42CFC8782A5BC03C743E2A173261 * L_26 = __this->get_activeVoices_3();
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_27 = V_1;
		NullCheck(L_26);
		LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6(L_26, L_27, /*hidden argument*/LinkedList_1_Remove_m18A23D7CEA66F67B260DDC3B1E09F10346D8AEE6_RuntimeMethod_var);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_28 = V_1;
		NullCheck(L_28);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_29 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_28, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		NullCheck(L_29);
		VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * L_30 = Voice_get_VoiceParams_mB0BD132A47F484F29AE2C9318CBF10FD05EF5391(L_29, /*hidden argument*/NULL);
		NullCheck(L_30);
		L_30->set_state_4(0);
		LinkedListNode_1_tF436360E76928A5B2D397FB7E021AD05A4818F21 * L_31 = V_1;
		NullCheck(L_31);
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_32 = LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_inline(L_31, /*hidden argument*/LinkedListNode_1_get_Value_mC794AFA3529F42C2FD6E7851C24FB77F288FDC53_RuntimeMethod_var);
		V_4 = L_32;
		goto IL_00ba;
	}

IL_00ba:
	{
		Voice_t481B233F7BCA5C28D192670FC7590699211A984E * L_33 = V_4;
		return L_33;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceManager/VoiceNode::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceNode__ctor_mF4DA85DA34F70AAEBFC3780DFF3B87A01D57BAE5 (VoiceNode_t3D8E79BDA37BE869ECA964A9AF40ED49F264205B * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Single DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::get_CombinedVolume()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float VoiceParameters_get_CombinedVolume_m3B5EA828CEAD322A9DD4BBF19FEEAD6F950ED6F0 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		float L_0 = __this->get_mix1_14();
		float L_1 = __this->get_mix2_15();
		V_0 = ((float)il2cpp_codegen_add((float)L_0, (float)L_1));
		goto IL_0011;
	}

IL_0011:
	{
		float L_2 = V_0;
		return L_2;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceParameters__ctor_m0F6140BF4B6EAE4D1A03A011E048C662E8DBA418 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceParameters__ctor_m0F6140BF4B6EAE4D1A03A011E048C662E8DBA418_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	bool V_1 = false;
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_0 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)((int32_t)64));
		__this->set_blockBuffer_7(L_0);
		UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD* L_1 = (UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD*)(UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD*)SZArrayNew(UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD_il2cpp_TypeInfo_var, (uint32_t)4);
		__this->set_pData_8(L_1);
		GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8* L_2 = (GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8*)(GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8*)SZArrayNew(GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8_il2cpp_TypeInfo_var, (uint32_t)4);
		__this->set_generatorParams_10(L_2);
		EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E* L_3 = (EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E*)(EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E*)SZArrayNew(EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E_il2cpp_TypeInfo_var, (uint32_t)4);
		__this->set_envelopes_11(L_3);
		FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E* L_4 = (FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E*)(FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E*)SZArrayNew(FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E_il2cpp_TypeInfo_var, (uint32_t)4);
		__this->set_filters_12(L_4);
		LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78* L_5 = (LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78*)(LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78*)SZArrayNew(LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78_il2cpp_TypeInfo_var, (uint32_t)4);
		__this->set_lfos_13(L_5);
		V_0 = 0;
		goto IL_008f;
	}

IL_0055:
	{
		GeneratorParametersU5BU5D_tC3F9F7E0A13DF9F659173C7545CF14AFDD76DEC8* L_6 = __this->get_generatorParams_10();
		int32_t L_7 = V_0;
		GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB * L_8 = (GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB *)il2cpp_codegen_object_new(GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB_il2cpp_TypeInfo_var);
		GeneratorParameters__ctor_m6B001B6B0B1FA64E3B5B229C27E5F68EFC6AAAC7(L_8, /*hidden argument*/NULL);
		NullCheck(L_6);
		ArrayElementTypeCheck (L_6, L_8);
		(L_6)->SetAt(static_cast<il2cpp_array_size_t>(L_7), (GeneratorParameters_t4DF2E89645D189712DC9A6E6FA040084D249DAAB *)L_8);
		EnvelopeU5BU5D_t27286D6069545792C4F23ADB6D1CDC399FEEC16E* L_9 = __this->get_envelopes_11();
		int32_t L_10 = V_0;
		Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 * L_11 = (Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 *)il2cpp_codegen_object_new(Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6_il2cpp_TypeInfo_var);
		Envelope__ctor_m737B844AF5B88FFA81328A5CA55A870BA888543F(L_11, /*hidden argument*/NULL);
		NullCheck(L_9);
		ArrayElementTypeCheck (L_9, L_11);
		(L_9)->SetAt(static_cast<il2cpp_array_size_t>(L_10), (Envelope_tC51E5439CA36BCA463C9B9E4BEAEA7FC304020B6 *)L_11);
		FilterU5BU5D_t4D6A85F0A2C91A66EDEDD3721CC09486B1E08A1E* L_12 = __this->get_filters_12();
		int32_t L_13 = V_0;
		Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 * L_14 = (Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 *)il2cpp_codegen_object_new(Filter_t8869C4D2146972E0AFC8080ADBB879E449534331_il2cpp_TypeInfo_var);
		Filter__ctor_m5B576C2DFA9D8809F2575B66F8765C4A3A5EE522(L_14, /*hidden argument*/NULL);
		NullCheck(L_12);
		ArrayElementTypeCheck (L_12, L_14);
		(L_12)->SetAt(static_cast<il2cpp_array_size_t>(L_13), (Filter_t8869C4D2146972E0AFC8080ADBB879E449534331 *)L_14);
		LfoU5BU5D_t5A6EED26B654760E055110CCCB4859FB2389BE78* L_15 = __this->get_lfos_13();
		int32_t L_16 = V_0;
		Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B * L_17 = (Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B *)il2cpp_codegen_object_new(Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B_il2cpp_TypeInfo_var);
		Lfo__ctor_m4B9AEC7B2766FA0E40133C110B424B1CD623C188(L_17, /*hidden argument*/NULL);
		NullCheck(L_15);
		ArrayElementTypeCheck (L_15, L_17);
		(L_15)->SetAt(static_cast<il2cpp_array_size_t>(L_16), (Lfo_tC25CD78E1A5EA8FBDC5761E90F533A4D5DDF593B *)L_17);
		int32_t L_18 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_18, (int32_t)1));
	}

IL_008f:
	{
		int32_t L_19 = V_0;
		V_1 = (bool)((((int32_t)L_19) < ((int32_t)4))? 1 : 0);
		bool L_20 = V_1;
		if (L_20)
		{
			goto IL_0055;
		}
	}
	{
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceParameters_Reset_mAA57CF4C4E6F6D8896958323E4E820258AC4B4B2 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method)
{
	{
		__this->set_noteOffPending_3((bool)0);
		__this->set_pitchOffset_5(0);
		__this->set_volOffset_6((0.0f));
		UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD* L_0 = __this->get_pData_8();
		UnionDataU5BU5D_tCC02DEEFA8E46182F0AC48FAF4E1AC84BF30BEDD* L_1 = __this->get_pData_8();
		NullCheck(L_1);
		Array_Clear_m174F4957D6DEDB6359835123005304B14E79132E((RuntimeArray *)(RuntimeArray *)L_0, 0, (((int32_t)((int32_t)(((RuntimeArray*)L_1)->max_length)))), /*hidden argument*/NULL);
		__this->set_mix1_14((0.0f));
		__this->set_mix2_15((0.0f));
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::MixMonoToMonoInterp(System.Int32,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceParameters_MixMonoToMonoInterp_m1506E4CFD76E52FFDE680D4218B69C3573954B81 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, int32_t ___startIndex0, float ___volume1, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	int32_t V_1 = 0;
	bool V_2 = false;
	{
		float L_0 = ___volume1;
		float L_1 = __this->get_mix1_14();
		V_0 = ((float)((float)((float)il2cpp_codegen_subtract((float)L_0, (float)L_1))/(float)(64.0f)));
		V_1 = 0;
		goto IL_005b;
	}

IL_0014:
	{
		float L_2 = __this->get_mix1_14();
		float L_3 = V_0;
		__this->set_mix1_14(((float)il2cpp_codegen_add((float)L_2, (float)L_3)));
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_4 = __this->get_synthParams_9();
		NullCheck(L_4);
		Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * L_5 = L_4->get_synth_15();
		NullCheck(L_5);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_6 = L_5->get_sampleBuffer_17();
		int32_t L_7 = ___startIndex0;
		int32_t L_8 = V_1;
		NullCheck(L_6);
		float* L_9 = ((L_6)->GetAddressAt(static_cast<il2cpp_array_size_t>(((int32_t)il2cpp_codegen_add((int32_t)L_7, (int32_t)L_8)))));
		float L_10 = (*(float*)L_9);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_11 = __this->get_blockBuffer_7();
		int32_t L_12 = V_1;
		NullCheck(L_11);
		int32_t L_13 = L_12;
		float L_14 = (L_11)->GetAt(static_cast<il2cpp_array_size_t>(L_13));
		float L_15 = __this->get_mix1_14();
		*(float*)L_9 = ((float)il2cpp_codegen_add((float)L_10, (float)((float)il2cpp_codegen_multiply((float)L_14, (float)L_15))));
		int32_t L_16 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_16, (int32_t)1));
	}

IL_005b:
	{
		int32_t L_17 = V_1;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_18 = __this->get_blockBuffer_7();
		NullCheck(L_18);
		V_2 = (bool)((((int32_t)L_17) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_18)->max_length))))))? 1 : 0);
		bool L_19 = V_2;
		if (L_19)
		{
			goto IL_0014;
		}
	}
	{
		float L_20 = ___volume1;
		__this->set_mix1_14(L_20);
		return;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::MixMonoToStereoInterp(System.Int32,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VoiceParameters_MixMonoToStereoInterp_m9FD34A36E5479263497995EEDAF1E05277A06543 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, int32_t ___startIndex0, float ___leftVol1, float ___rightVol2, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	float V_1 = 0.0f;
	int32_t V_2 = 0;
	bool V_3 = false;
	{
		float L_0 = ___leftVol1;
		float L_1 = __this->get_mix1_14();
		V_0 = ((float)((float)((float)il2cpp_codegen_subtract((float)L_0, (float)L_1))/(float)(64.0f)));
		float L_2 = ___rightVol2;
		float L_3 = __this->get_mix2_15();
		V_1 = ((float)((float)((float)il2cpp_codegen_subtract((float)L_2, (float)L_3))/(float)(64.0f)));
		V_2 = 0;
		goto IL_00b1;
	}

IL_0026:
	{
		float L_4 = __this->get_mix1_14();
		float L_5 = V_0;
		__this->set_mix1_14(((float)il2cpp_codegen_add((float)L_4, (float)L_5)));
		float L_6 = __this->get_mix2_15();
		float L_7 = V_1;
		__this->set_mix2_15(((float)il2cpp_codegen_add((float)L_6, (float)L_7)));
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_8 = __this->get_synthParams_9();
		NullCheck(L_8);
		Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * L_9 = L_8->get_synth_15();
		NullCheck(L_9);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_10 = L_9->get_sampleBuffer_17();
		int32_t L_11 = ___startIndex0;
		NullCheck(L_10);
		float* L_12 = ((L_10)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_11)));
		float L_13 = (*(float*)L_12);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_14 = __this->get_blockBuffer_7();
		int32_t L_15 = V_2;
		NullCheck(L_14);
		int32_t L_16 = L_15;
		float L_17 = (L_14)->GetAt(static_cast<il2cpp_array_size_t>(L_16));
		float L_18 = __this->get_mix1_14();
		*(float*)L_12 = ((float)il2cpp_codegen_add((float)L_13, (float)((float)il2cpp_codegen_multiply((float)L_17, (float)L_18))));
		SynthParameters_t28F332ECC5709E0FF13634E25619A532DACFAD33 * L_19 = __this->get_synthParams_9();
		NullCheck(L_19);
		Synthesizer_t2EC8B46135E21CA2F091971911EAD946590D3D36 * L_20 = L_19->get_synth_15();
		NullCheck(L_20);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_21 = L_20->get_sampleBuffer_17();
		int32_t L_22 = ___startIndex0;
		NullCheck(L_21);
		float* L_23 = ((L_21)->GetAddressAt(static_cast<il2cpp_array_size_t>(((int32_t)il2cpp_codegen_add((int32_t)L_22, (int32_t)1)))));
		float L_24 = (*(float*)L_23);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_25 = __this->get_blockBuffer_7();
		int32_t L_26 = V_2;
		NullCheck(L_25);
		int32_t L_27 = L_26;
		float L_28 = (L_25)->GetAt(static_cast<il2cpp_array_size_t>(L_27));
		float L_29 = __this->get_mix2_15();
		*(float*)L_23 = ((float)il2cpp_codegen_add((float)L_24, (float)((float)il2cpp_codegen_multiply((float)L_28, (float)L_29))));
		int32_t L_30 = ___startIndex0;
		___startIndex0 = ((int32_t)il2cpp_codegen_add((int32_t)L_30, (int32_t)2));
		int32_t L_31 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_31, (int32_t)1));
	}

IL_00b1:
	{
		int32_t L_32 = V_2;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_33 = __this->get_blockBuffer_7();
		NullCheck(L_33);
		V_3 = (bool)((((int32_t)L_32) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_33)->max_length))))))? 1 : 0);
		bool L_34 = V_3;
		if (L_34)
		{
			goto IL_0026;
		}
	}
	{
		float L_35 = ___leftVol1;
		__this->set_mix1_14(L_35);
		float L_36 = ___rightVol2;
		__this->set_mix2_15(L_36);
		return;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Synthesis.VoiceParameters::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* VoiceParameters_ToString_mE40D2988FEC43C9EC933FDF4E52594EE494E3246 (VoiceParameters_t86565997CB6855C5571EE22223CB6EAB2DD1CB92 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VoiceParameters_ToString_mE40D2988FEC43C9EC933FDF4E52594EE494E3246_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_1 = NULL;
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)4);
		V_1 = L_0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = V_1;
		int32_t L_2 = __this->get_channel_0();
		int32_t L_3 = L_2;
		RuntimeObject * L_4 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_3);
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, L_4);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_4);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_5 = V_1;
		int32_t L_6 = __this->get_note_1();
		int32_t L_7 = L_6;
		RuntimeObject * L_8 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_7);
		NullCheck(L_5);
		ArrayElementTypeCheck (L_5, L_8);
		(L_5)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_8);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_9 = V_1;
		int32_t L_10 = __this->get_velocity_2();
		int32_t L_11 = L_10;
		RuntimeObject * L_12 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_11);
		NullCheck(L_9);
		ArrayElementTypeCheck (L_9, L_12);
		(L_9)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)L_12);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_13 = V_1;
		int32_t L_14 = __this->get_state_4();
		int32_t L_15 = L_14;
		RuntimeObject * L_16 = Box(VoiceStateEnum_t1DC114A9113887829A4FAA949B8F4E77286A95FE_il2cpp_TypeInfo_var, &L_15);
		NullCheck(L_13);
		ArrayElementTypeCheck (L_13, L_16);
		(L_13)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_16);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_17 = V_1;
		String_t* L_18 = String_Format_mA3AC3FE7B23D97F3A5BAA082D25B0E01B341A865(_stringLiteral10559120CF9232240447CDD267B70FDF56A163F2, L_17, /*hidden argument*/NULL);
		V_0 = L_18;
		goto IL_004e;
	}

IL_004e:
	{
		String_t* L_19 = V_0;
		return L_19;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int16 DaggerfallWorkshop.AudioSynthesis.Util.BigEndianHelper::ReadInt16(System.IO.BinaryReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t BigEndianHelper_ReadInt16_m8E353309428D5A0F66BB8001C4056E5966AFE001 (BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader0, const RuntimeMethod* method)
{
	int16_t V_0 = 0;
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_0 = ___reader0;
		NullCheck(L_0);
		uint8_t L_1 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_0);
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_2 = ___reader0;
		NullCheck(L_2);
		uint8_t L_3 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_2);
		V_0 = (((int16_t)((int16_t)((int32_t)((int32_t)((int32_t)((int32_t)L_1<<(int32_t)8))|(int32_t)L_3)))));
		goto IL_0014;
	}

IL_0014:
	{
		int16_t L_4 = V_0;
		return L_4;
	}
}
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Util.BigEndianHelper::ReadInt32(System.IO.BinaryReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t BigEndianHelper_ReadInt32_mFEE6D4F41D51225D19DEDB580D85A3755FEB97EF (BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_0 = ___reader0;
		NullCheck(L_0);
		uint8_t L_1 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_0);
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_2 = ___reader0;
		NullCheck(L_2);
		uint8_t L_3 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_2);
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_4 = ___reader0;
		NullCheck(L_4);
		uint8_t L_5 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_4);
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_6 = ___reader0;
		NullCheck(L_6);
		uint8_t L_7 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_6);
		V_0 = ((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)L_1<<(int32_t)((int32_t)24)))|(int32_t)((int32_t)((int32_t)L_3<<(int32_t)((int32_t)16)))))|(int32_t)((int32_t)((int32_t)L_5<<(int32_t)8))))|(int32_t)L_7));
		goto IL_0027;
	}

IL_0027:
	{
		int32_t L_8 = V_0;
		return L_8;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Char[] DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::Read8BitChars(System.IO.BinaryReader,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031 (BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader0, int32_t ___length1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* V_0 = NULL;
	int32_t V_1 = 0;
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* V_2 = NULL;
	bool V_3 = false;
	{
		int32_t L_0 = ___length1;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_1 = (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)SZArrayNew(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		V_1 = 0;
		goto IL_0019;
	}

IL_000c:
	{
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_2 = V_0;
		int32_t L_3 = V_1;
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_4 = ___reader0;
		NullCheck(L_4);
		uint8_t L_5 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_4);
		NullCheck(L_2);
		(L_2)->SetAt(static_cast<il2cpp_array_size_t>(L_3), (Il2CppChar)L_5);
		int32_t L_6 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
	}

IL_0019:
	{
		int32_t L_7 = V_1;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_8 = V_0;
		NullCheck(L_8);
		V_3 = (bool)((((int32_t)L_7) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_8)->max_length))))))? 1 : 0);
		bool L_9 = V_3;
		if (L_9)
		{
			goto IL_000c;
		}
	}
	{
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_10 = V_0;
		V_2 = L_10;
		goto IL_0027;
	}

IL_0027:
	{
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_11 = V_2;
		return L_11;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::Read8BitString(System.IO.BinaryReader,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* IOHelper_Read8BitString_mE8F430DAE0D8052644ED33F18F5AFCE11D0AE9D4 (BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader0, int32_t ___length1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (IOHelper_Read8BitString_mE8F430DAE0D8052644ED33F18F5AFCE11D0AE9D4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* V_0 = NULL;
	int32_t V_1 = 0;
	String_t* V_2 = NULL;
	int32_t V_3 = 0;
	String_t* V_4 = NULL;
	bool V_5 = false;
	{
		int32_t L_0 = ___length1;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_1 = (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)SZArrayNew(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		V_1 = 0;
		goto IL_0019;
	}

IL_000c:
	{
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_2 = V_0;
		int32_t L_3 = V_1;
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_4 = ___reader0;
		NullCheck(L_4);
		uint8_t L_5 = VirtFuncInvoker0< uint8_t >::Invoke(11 /* System.Byte System.IO.BinaryReader::ReadByte() */, L_4);
		NullCheck(L_2);
		(L_2)->SetAt(static_cast<il2cpp_array_size_t>(L_3), (Il2CppChar)L_5);
		int32_t L_6 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
	}

IL_0019:
	{
		int32_t L_7 = V_1;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_8 = V_0;
		NullCheck(L_8);
		V_5 = (bool)((((int32_t)L_7) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_8)->max_length))))))? 1 : 0);
		bool L_9 = V_5;
		if (L_9)
		{
			goto IL_000c;
		}
	}
	{
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_10 = V_0;
		String_t* L_11 = String_CreateString_m394C06654854ADD4C51FF957BE0CC72EF52BAA96(NULL, L_10, /*hidden argument*/NULL);
		V_2 = L_11;
		String_t* L_12 = V_2;
		NullCheck(L_12);
		int32_t L_13 = String_IndexOf_m2909B8CF585E1BD0C81E11ACA2F48012156FD5BD(L_12, 0, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = V_3;
		V_5 = (bool)((((int32_t)L_14) < ((int32_t)0))? 1 : 0);
		bool L_15 = V_5;
		if (L_15)
		{
			goto IL_0049;
		}
	}
	{
		String_t* L_16 = V_2;
		int32_t L_17 = V_3;
		NullCheck(L_16);
		String_t* L_18 = String_Remove_mEB092613182657B160E4BC9587D71A9CF639AD8C(L_16, L_17, /*hidden argument*/NULL);
		V_4 = L_18;
		goto IL_004e;
	}

IL_0049:
	{
		String_t* L_19 = V_2;
		V_4 = L_19;
		goto IL_004e;
	}

IL_004e:
	{
		String_t* L_20 = V_4;
		return L_20;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::GetExtension(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* IOHelper_GetExtension_m7EFAEAE127597F499B8F093F347F90AECBC3114E (String_t* ___fileName0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (IOHelper_GetExtension_m7EFAEAE127597F499B8F093F347F90AECBC3114E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	String_t* V_1 = NULL;
	bool V_2 = false;
	int32_t G_B6_0 = 0;
	{
		String_t* L_0 = ___fileName0;
		NullCheck(L_0);
		int32_t L_1 = String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline(L_0, /*hidden argument*/NULL);
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_1, (int32_t)1));
		goto IL_0051;
	}

IL_000c:
	{
		String_t* L_2 = ___fileName0;
		int32_t L_3 = V_0;
		NullCheck(L_2);
		Il2CppChar L_4 = String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96(L_2, L_3, /*hidden argument*/NULL);
		V_2 = (bool)((((int32_t)((((int32_t)L_4) == ((int32_t)((int32_t)46)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_5 = V_2;
		if (L_5)
		{
			goto IL_0029;
		}
	}
	{
		String_t* L_6 = ___fileName0;
		int32_t L_7 = V_0;
		NullCheck(L_6);
		String_t* L_8 = String_Substring_m2C4AFF5E79DD8BADFD2DFBCF156BF728FBB8E1AE(L_6, L_7, /*hidden argument*/NULL);
		V_1 = L_8;
		goto IL_0064;
	}

IL_0029:
	{
		String_t* L_9 = ___fileName0;
		int32_t L_10 = V_0;
		NullCheck(L_9);
		Il2CppChar L_11 = String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96(L_9, L_10, /*hidden argument*/NULL);
		if ((((int32_t)L_11) == ((int32_t)((int32_t)47))))
		{
			goto IL_0044;
		}
	}
	{
		String_t* L_12 = ___fileName0;
		int32_t L_13 = V_0;
		NullCheck(L_12);
		Il2CppChar L_14 = String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96(L_12, L_13, /*hidden argument*/NULL);
		G_B6_0 = ((((int32_t)((((int32_t)L_14) == ((int32_t)((int32_t)92)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		goto IL_0045;
	}

IL_0044:
	{
		G_B6_0 = 0;
	}

IL_0045:
	{
		V_2 = (bool)G_B6_0;
		bool L_15 = V_2;
		if (L_15)
		{
			goto IL_004c;
		}
	}
	{
		goto IL_005c;
	}

IL_004c:
	{
		int32_t L_16 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_16, (int32_t)1));
	}

IL_0051:
	{
		int32_t L_17 = V_0;
		V_2 = (bool)((((int32_t)((((int32_t)L_17) < ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_18 = V_2;
		if (L_18)
		{
			goto IL_000c;
		}
	}

IL_005c:
	{
		String_t* L_19 = ((String_t_StaticFields*)il2cpp_codegen_static_fields_for(String_t_il2cpp_TypeInfo_var))->get_Empty_5();
		V_1 = L_19;
		goto IL_0064;
	}

IL_0064:
	{
		String_t* L_20 = V_1;
		return L_20;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::GetFileNameWithExtension(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* IOHelper_GetFileNameWithExtension_m2CD927C08AE14119DA36A77921B0BC48CA830A26 (String_t* ___fileName0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	String_t* V_1 = NULL;
	bool V_2 = false;
	int32_t G_B4_0 = 0;
	{
		String_t* L_0 = ___fileName0;
		NullCheck(L_0);
		int32_t L_1 = String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline(L_0, /*hidden argument*/NULL);
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_1, (int32_t)1));
		goto IL_003f;
	}

IL_000c:
	{
		String_t* L_2 = ___fileName0;
		int32_t L_3 = V_0;
		NullCheck(L_2);
		Il2CppChar L_4 = String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96(L_2, L_3, /*hidden argument*/NULL);
		if ((((int32_t)L_4) == ((int32_t)((int32_t)47))))
		{
			goto IL_0028;
		}
	}
	{
		String_t* L_5 = ___fileName0;
		int32_t L_6 = V_0;
		NullCheck(L_5);
		Il2CppChar L_7 = String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96(L_5, L_6, /*hidden argument*/NULL);
		G_B4_0 = ((((int32_t)((((int32_t)L_7) == ((int32_t)((int32_t)92)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		goto IL_0029;
	}

IL_0028:
	{
		G_B4_0 = 0;
	}

IL_0029:
	{
		V_2 = (bool)G_B4_0;
		bool L_8 = V_2;
		if (L_8)
		{
			goto IL_003a;
		}
	}
	{
		String_t* L_9 = ___fileName0;
		int32_t L_10 = V_0;
		NullCheck(L_9);
		String_t* L_11 = String_Substring_m2C4AFF5E79DD8BADFD2DFBCF156BF728FBB8E1AE(L_9, ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1)), /*hidden argument*/NULL);
		V_1 = L_11;
		goto IL_004e;
	}

IL_003a:
	{
		int32_t L_12 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_12, (int32_t)1));
	}

IL_003f:
	{
		int32_t L_13 = V_0;
		V_2 = (bool)((((int32_t)((((int32_t)L_13) < ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_14 = V_2;
		if (L_14)
		{
			goto IL_000c;
		}
	}
	{
		String_t* L_15 = ___fileName0;
		V_1 = L_15;
		goto IL_004e;
	}

IL_004e:
	{
		String_t* L_16 = V_1;
		return L_16;
	}
}
// System.String DaggerfallWorkshop.AudioSynthesis.Util.IOHelper::GetFileNameWithoutExtension(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* IOHelper_GetFileNameWithoutExtension_m6E9622C0F5D33C67D78BA597988C979E386C6016 (String_t* ___fileName0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	String_t* V_1 = NULL;
	bool V_2 = false;
	{
		String_t* L_0 = ___fileName0;
		String_t* L_1 = IOHelper_GetFileNameWithExtension_m2CD927C08AE14119DA36A77921B0BC48CA830A26(L_0, /*hidden argument*/NULL);
		___fileName0 = L_1;
		String_t* L_2 = ___fileName0;
		NullCheck(L_2);
		int32_t L_3 = String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline(L_2, /*hidden argument*/NULL);
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_3, (int32_t)1));
		goto IL_0037;
	}

IL_0014:
	{
		String_t* L_4 = ___fileName0;
		int32_t L_5 = V_0;
		NullCheck(L_4);
		Il2CppChar L_6 = String_get_Chars_m14308AC3B95F8C1D9F1D1055B116B37D595F1D96(L_4, L_5, /*hidden argument*/NULL);
		V_2 = (bool)((((int32_t)((((int32_t)L_6) == ((int32_t)((int32_t)46)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_7 = V_2;
		if (L_7)
		{
			goto IL_0032;
		}
	}
	{
		String_t* L_8 = ___fileName0;
		int32_t L_9 = V_0;
		NullCheck(L_8);
		String_t* L_10 = String_Substring_mB593C0A320C683E6E47EFFC0A12B7A465E5E43BB(L_8, 0, L_9, /*hidden argument*/NULL);
		V_1 = L_10;
		goto IL_0046;
	}

IL_0032:
	{
		int32_t L_11 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_11, (int32_t)1));
	}

IL_0037:
	{
		int32_t L_12 = V_0;
		V_2 = (bool)((((int32_t)((((int32_t)L_12) < ((int32_t)0))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_13 = V_2;
		if (L_13)
		{
			goto IL_0014;
		}
	}
	{
		String_t* L_14 = ___fileName0;
		V_1 = L_14;
		goto IL_0046;
	}

IL_0046:
	{
		String_t* L_15 = V_1;
		return L_15;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Util.Riff.Chunk::.ctor(System.String,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Chunk__ctor_m1EF9CA04B86EE77E27D24BD0164E6F521EA5F896 (Chunk_t9D4FD5DACA483337BDBED87ED983109BAF733C15 * __this, String_t* ___id0, int32_t ___size1, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		String_t* L_0 = ___id0;
		__this->set_id_0(L_0);
		int32_t L_1 = ___size1;
		__this->set_size_1(L_1);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.String DaggerfallWorkshop.AudioSynthesis.Util.Riff.RiffTypeChunk::get_TypeId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* RiffTypeChunk_get_TypeId_mA74611721D9944B5960746AC550CF9A8AC43DEDF (RiffTypeChunk_t0EBCC7D1B96A4601BABB5AE304EAB15D844650FD * __this, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
	{
		String_t* L_0 = __this->get_typeId_2();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		String_t* L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Util.Riff.RiffTypeChunk::.ctor(System.String,System.Int32,System.IO.BinaryReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RiffTypeChunk__ctor_mCFC2627384BC7928A8165D9B455C333030FD74B6 (RiffTypeChunk_t0EBCC7D1B96A4601BABB5AE304EAB15D844650FD * __this, String_t* ___id0, int32_t ___size1, BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * ___reader2, const RuntimeMethod* method)
{
	{
		String_t* L_0 = ___id0;
		int32_t L_1 = ___size1;
		Chunk__ctor_m1EF9CA04B86EE77E27D24BD0164E6F521EA5F896(__this, L_0, L_1, /*hidden argument*/NULL);
		BinaryReader_t7467E057B24C42E81B1C3E5C60288BB4B1718969 * L_2 = ___reader2;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_3 = IOHelper_Read8BitChars_m58099613BE7E266F40FEEECF072D362788716031(L_2, 4, /*hidden argument*/NULL);
		String_t* L_4 = String_CreateString_m394C06654854ADD4C51FF957BE0CC72EF52BAA96(NULL, L_3, /*hidden argument*/NULL);
		__this->set_typeId_2(L_4);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Util.Tables::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tables__cctor_m0C56E0A94FA2CA0564F609F52960527ABB235CC0 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables__cctor_m0C56E0A94FA2CA0564F609F52960527ABB235CC0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* L_0 = (SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E*)(SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E*)SZArrayNew(SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E_il2cpp_TypeInfo_var, (uint32_t)4);
		((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->set_EnvelopeTables_0(L_0);
		SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* L_1 = ((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->get_EnvelopeTables_0();
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_2 = Tables_CreateSustainTable_mCC57225D8466B310B618EB3E4E3303F685BFC627(((int32_t)128), /*hidden argument*/NULL);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_3 = Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746(L_2, /*hidden argument*/NULL);
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, L_3);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)L_3);
		SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* L_4 = ((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->get_EnvelopeTables_0();
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_5 = Tables_CreateLinearTable_m62A5E3D5CE0F0D8F9F496A261468BE64E7B8FE40(((int32_t)128), /*hidden argument*/NULL);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_6 = Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746(L_5, /*hidden argument*/NULL);
		NullCheck(L_4);
		ArrayElementTypeCheck (L_4, L_6);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(1), (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)L_6);
		SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* L_7 = ((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->get_EnvelopeTables_0();
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_8 = Tables_CreateConcaveTable_m5544B299F299F355FB58B93619127874CCDB06CA(((int32_t)128), /*hidden argument*/NULL);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_9 = Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746(L_8, /*hidden argument*/NULL);
		NullCheck(L_7);
		ArrayElementTypeCheck (L_7, L_9);
		(L_7)->SetAt(static_cast<il2cpp_array_size_t>(2), (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)L_9);
		SingleU5BU5DU5BU5D_tC2E25498616DDBEA3B03D43855DEBC928046392E* L_10 = ((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->get_EnvelopeTables_0();
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_11 = Tables_CreateConvexTable_m8CEBC09758BE247C3EBE23F03D1C6A6D39AA520B(((int32_t)128), /*hidden argument*/NULL);
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_12 = Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746(L_11, /*hidden argument*/NULL);
		NullCheck(L_10);
		ArrayElementTypeCheck (L_10, L_12);
		(L_10)->SetAt(static_cast<il2cpp_array_size_t>(3), (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)L_12);
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_13 = Tables_CreateCentTable_m372317857C669E4D43EBBD028DB2E62BCC6CC50F(/*hidden argument*/NULL);
		((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->set_CentTable_2(L_13);
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_14 = Tables_CreateSemitoneTable_mA5A66F789BEC9D3168CC8AE2BA805F8309441AC8(/*hidden argument*/NULL);
		((Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_StaticFields*)il2cpp_codegen_static_fields_for(Tables_t1652068E49F8FDCA9BC4400E6A6B1FA5F68CFDE7_il2cpp_TypeInfo_var))->set_SemitoneTable_1(L_14);
		return;
	}
}
// System.Double[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateCentTable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* Tables_CreateCentTable_m372317857C669E4D43EBBD028DB2E62BCC6CC50F (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_CreateCentTable_m372317857C669E4D43EBBD028DB2E62BCC6CC50F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* V_0 = NULL;
	int32_t V_1 = 0;
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* V_2 = NULL;
	bool V_3 = false;
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_0 = (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)SZArrayNew(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D_il2cpp_TypeInfo_var, (uint32_t)((int32_t)201));
		V_0 = L_0;
		V_1 = 0;
		goto IL_003d;
	}

IL_0010:
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_1 = V_0;
		int32_t L_2 = V_1;
		int32_t L_3 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_4 = Math_Pow_m9CD842663B1A2FA4C66EEFFC6F0D705B40BE46F1((2.0), ((double)((double)((double)il2cpp_codegen_subtract((double)(((double)((double)L_3))), (double)(100.0)))/(double)(1200.0))), /*hidden argument*/NULL);
		NullCheck(L_1);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(L_2), (double)L_4);
		int32_t L_5 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1));
	}

IL_003d:
	{
		int32_t L_6 = V_1;
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_7 = V_0;
		NullCheck(L_7);
		V_3 = (bool)((((int32_t)L_6) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_7)->max_length))))))? 1 : 0);
		bool L_8 = V_3;
		if (L_8)
		{
			goto IL_0010;
		}
	}
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_9 = V_0;
		V_2 = L_9;
		goto IL_004b;
	}

IL_004b:
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_10 = V_2;
		return L_10;
	}
}
// System.Double[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateSemitoneTable()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* Tables_CreateSemitoneTable_mA5A66F789BEC9D3168CC8AE2BA805F8309441AC8 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_CreateSemitoneTable_mA5A66F789BEC9D3168CC8AE2BA805F8309441AC8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* V_0 = NULL;
	int32_t V_1 = 0;
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* V_2 = NULL;
	bool V_3 = false;
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_0 = (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)SZArrayNew(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D_il2cpp_TypeInfo_var, (uint32_t)((int32_t)255));
		V_0 = L_0;
		V_1 = 0;
		goto IL_003d;
	}

IL_0010:
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_1 = V_0;
		int32_t L_2 = V_1;
		int32_t L_3 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_4 = Math_Pow_m9CD842663B1A2FA4C66EEFFC6F0D705B40BE46F1((2.0), ((double)((double)((double)il2cpp_codegen_subtract((double)(((double)((double)L_3))), (double)(127.0)))/(double)(12.0))), /*hidden argument*/NULL);
		NullCheck(L_1);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(L_2), (double)L_4);
		int32_t L_5 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_5, (int32_t)1));
	}

IL_003d:
	{
		int32_t L_6 = V_1;
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_7 = V_0;
		NullCheck(L_7);
		V_3 = (bool)((((int32_t)L_6) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_7)->max_length))))))? 1 : 0);
		bool L_8 = V_3;
		if (L_8)
		{
			goto IL_0010;
		}
	}
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_9 = V_0;
		V_2 = L_9;
		goto IL_004b;
	}

IL_004b:
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_10 = V_2;
		return L_10;
	}
}
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateSustainTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateSustainTable_mCC57225D8466B310B618EB3E4E3303F685BFC627 (int32_t ___size0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_CreateSustainTable_mCC57225D8466B310B618EB3E4E3303F685BFC627_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_0 = NULL;
	int32_t V_1 = 0;
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_2 = NULL;
	bool V_3 = false;
	{
		int32_t L_0 = ___size0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_1 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		V_1 = 0;
		goto IL_001a;
	}

IL_000c:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_2 = V_0;
		int32_t L_3 = V_1;
		NullCheck(L_2);
		(L_2)->SetAt(static_cast<il2cpp_array_size_t>(L_3), (float)(1.0f));
		int32_t L_4 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_4, (int32_t)1));
	}

IL_001a:
	{
		int32_t L_5 = V_1;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_6 = V_0;
		NullCheck(L_6);
		V_3 = (bool)((((int32_t)L_5) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_6)->max_length))))))? 1 : 0);
		bool L_7 = V_3;
		if (L_7)
		{
			goto IL_000c;
		}
	}
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_8 = V_0;
		V_2 = L_8;
		goto IL_0028;
	}

IL_0028:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_9 = V_2;
		return L_9;
	}
}
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateLinearTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateLinearTable_m62A5E3D5CE0F0D8F9F496A261468BE64E7B8FE40 (int32_t ___size0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_CreateLinearTable_m62A5E3D5CE0F0D8F9F496A261468BE64E7B8FE40_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_0 = NULL;
	int32_t V_1 = 0;
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_2 = NULL;
	bool V_3 = false;
	{
		int32_t L_0 = ___size0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_1 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		V_1 = 0;
		goto IL_001c;
	}

IL_000c:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_2 = V_0;
		int32_t L_3 = V_1;
		int32_t L_4 = V_1;
		int32_t L_5 = ___size0;
		NullCheck(L_2);
		(L_2)->SetAt(static_cast<il2cpp_array_size_t>(L_3), (float)((float)((float)(((float)((float)L_4)))/(float)(((float)((float)((int32_t)il2cpp_codegen_subtract((int32_t)L_5, (int32_t)1))))))));
		int32_t L_6 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
	}

IL_001c:
	{
		int32_t L_7 = V_1;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_8 = V_0;
		NullCheck(L_8);
		V_3 = (bool)((((int32_t)L_7) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_8)->max_length))))))? 1 : 0);
		bool L_9 = V_3;
		if (L_9)
		{
			goto IL_000c;
		}
	}
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_10 = V_0;
		V_2 = L_10;
		goto IL_002a;
	}

IL_002a:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_11 = V_2;
		return L_11;
	}
}
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateConcaveTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateConcaveTable_m5544B299F299F355FB58B93619127874CCDB06CA (int32_t ___size0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_CreateConcaveTable_m5544B299F299F355FB58B93619127874CCDB06CA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_0 = NULL;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_4 = NULL;
	bool V_5 = false;
	{
		int32_t L_0 = ___size0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_1 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		int32_t L_2 = ___size0;
		int32_t L_3 = ___size0;
		V_1 = ((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_2, (int32_t)1)), (int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_3, (int32_t)1))));
		V_2 = 0;
		goto IL_003a;
	}

IL_0014:
	{
		int32_t L_4 = ___size0;
		int32_t L_5 = V_2;
		V_3 = ((int32_t)il2cpp_codegen_subtract((int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_4, (int32_t)1)), (int32_t)L_5));
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_6 = V_0;
		int32_t L_7 = V_2;
		int32_t L_8 = V_3;
		int32_t L_9 = V_3;
		int32_t L_10 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_11 = log10(((double)((double)(((double)((double)((int32_t)il2cpp_codegen_multiply((int32_t)L_8, (int32_t)L_9)))))/(double)(((double)((double)L_10))))));
		NullCheck(L_6);
		(L_6)->SetAt(static_cast<il2cpp_array_size_t>(L_7), (float)(((float)((float)((double)il2cpp_codegen_multiply((double)(-0.20833333333333334), (double)L_11))))));
		int32_t L_12 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_12, (int32_t)1));
	}

IL_003a:
	{
		int32_t L_13 = V_2;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_14 = V_0;
		NullCheck(L_14);
		V_5 = (bool)((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length))))))? 1 : 0);
		bool L_15 = V_5;
		if (L_15)
		{
			goto IL_0014;
		}
	}
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_16 = V_0;
		int32_t L_17 = ___size0;
		NullCheck(L_16);
		(L_16)->SetAt(static_cast<il2cpp_array_size_t>(((int32_t)il2cpp_codegen_subtract((int32_t)L_17, (int32_t)1))), (float)(1.0f));
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_18 = V_0;
		V_4 = L_18;
		goto IL_0055;
	}

IL_0055:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_19 = V_4;
		return L_19;
	}
}
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::CreateConvexTable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_CreateConvexTable_m8CEBC09758BE247C3EBE23F03D1C6A6D39AA520B (int32_t ___size0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_CreateConvexTable_m8CEBC09758BE247C3EBE23F03D1C6A6D39AA520B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_0 = NULL;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_3 = NULL;
	bool V_4 = false;
	{
		int32_t L_0 = ___size0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_1 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		int32_t L_2 = ___size0;
		int32_t L_3 = ___size0;
		V_1 = ((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_2, (int32_t)1)), (int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_3, (int32_t)1))));
		V_2 = 0;
		goto IL_003e;
	}

IL_0014:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_4 = V_0;
		int32_t L_5 = V_2;
		int32_t L_6 = V_2;
		int32_t L_7 = V_2;
		int32_t L_8 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		double L_9 = log10(((double)((double)(((double)((double)((int32_t)il2cpp_codegen_multiply((int32_t)L_6, (int32_t)L_7)))))/(double)(((double)((double)L_8))))));
		NullCheck(L_4);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(L_5), (float)(((float)((float)((double)il2cpp_codegen_add((double)(1.0), (double)((double)il2cpp_codegen_multiply((double)(0.20833333333333334), (double)L_9))))))));
		int32_t L_10 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_003e:
	{
		int32_t L_11 = V_2;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_12 = V_0;
		NullCheck(L_12);
		V_4 = (bool)((((int32_t)L_11) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_12)->max_length))))))? 1 : 0);
		bool L_13 = V_4;
		if (L_13)
		{
			goto IL_0014;
		}
	}
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_14 = V_0;
		NullCheck(L_14);
		(L_14)->SetAt(static_cast<il2cpp_array_size_t>(0), (float)(0.0f));
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_15 = V_0;
		V_3 = L_15;
		goto IL_0056;
	}

IL_0056:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_16 = V_3;
		return L_16;
	}
}
// System.Single[] DaggerfallWorkshop.AudioSynthesis.Util.Tables::RemoveDenormals(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746 (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tables_RemoveDenormals_m9B58BD5B41136465AAB71DD8545B7201AAB26746_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_1 = NULL;
	bool V_2 = false;
	{
		V_0 = 0;
		goto IL_0026;
	}

IL_0005:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_0 = ___data0;
		int32_t L_1 = V_0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		float L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		float L_4 = fabsf(L_3);
		V_2 = (bool)((!(((float)L_4) <= ((float)(9.999999E-39f))))? 1 : 0);
		bool L_5 = V_2;
		if (L_5)
		{
			goto IL_0021;
		}
	}
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_6 = ___data0;
		int32_t L_7 = V_0;
		NullCheck(L_6);
		(L_6)->SetAt(static_cast<il2cpp_array_size_t>(L_7), (float)(0.0f));
	}

IL_0021:
	{
		int32_t L_8 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_8, (int32_t)1));
	}

IL_0026:
	{
		int32_t L_9 = V_0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_10 = ___data0;
		NullCheck(L_10);
		V_2 = (bool)((((int32_t)L_9) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_10)->max_length))))))? 1 : 0);
		bool L_11 = V_2;
		if (L_11)
		{
			goto IL_0005;
		}
	}
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_12 = ___data0;
		V_1 = L_12;
		goto IL_0034;
	}

IL_0034:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_13 = V_1;
		return L_13;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::get_Length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PcmData_get_Length_mE0EE48C397F388B35BB5A8B9199AE76132AEE2FC (PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_length_2();
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561 (PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		int32_t L_0 = ___bits0;
		__this->set_bytes_1((uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_0/(int32_t)8))))));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___pcmData1;
		NullCheck(L_1);
		uint8_t L_2 = __this->get_bytes_1();
		V_0 = (bool)((((int32_t)((int32_t)((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_1)->max_length))))%(int32_t)L_2))) == ((int32_t)0))? 1 : 0);
		bool L_3 = V_0;
		if (L_3)
		{
			goto IL_002e;
		}
	}
	{
		Exception_t * L_4 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_4, _stringLiteral3E940EC9BDFD3BA55B88A6DAAD594599C853C4E5, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_4, PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561_RuntimeMethod_var);
	}

IL_002e:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_5 = ___pcmData1;
		__this->set_data_0(L_5);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_6 = __this->get_data_0();
		NullCheck(L_6);
		uint8_t L_7 = __this->get_bytes_1();
		__this->set_length_2(((int32_t)((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_6)->max_length))))/(int32_t)L_7)));
		IL2CPP_RUNTIME_CLASS_INIT(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var);
		bool L_8 = ((BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_StaticFields*)il2cpp_codegen_static_fields_for(BitConverter_tD5DF1CB5C5A5CB087D90BD881C8E75A332E546EE_il2cpp_TypeInfo_var))->get_IsLittleEndian_0();
		bool L_9 = ___isDataInLittleEndianFormat2;
		V_0 = (bool)((((int32_t)L_8) == ((int32_t)L_9))? 1 : 0);
		bool L_10 = V_0;
		if (L_10)
		{
			goto IL_0063;
		}
	}
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_11 = __this->get_data_0();
		int32_t L_12 = ___bits0;
		WaveHelper_SwapEndianess_mB4C9A2AC9A10F41C2F9BDC11EAFD474EF1386845(L_11, L_12, /*hidden argument*/NULL);
	}

IL_0063:
	{
		return;
	}
}
// DaggerfallWorkshop.AudioSynthesis.Wave.PcmData DaggerfallWorkshop.AudioSynthesis.Wave.PcmData::Create(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC * PcmData_Create_mEDE8A9F6F9942B882C6CF78E388D1E0720DBEE4C (int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PcmData_Create_mEDE8A9F6F9942B882C6CF78E388D1E0720DBEE4C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC * V_0 = NULL;
	int32_t V_1 = 0;
	{
		int32_t L_0 = ___bits0;
		V_1 = L_0;
		int32_t L_1 = V_1;
		if ((((int32_t)L_1) > ((int32_t)((int32_t)16))))
		{
			goto IL_0013;
		}
	}
	{
		int32_t L_2 = V_1;
		if ((((int32_t)L_2) == ((int32_t)8)))
		{
			goto IL_001f;
		}
	}
	{
		int32_t L_3 = V_1;
		if ((((int32_t)L_3) == ((int32_t)((int32_t)16))))
		{
			goto IL_002a;
		}
	}
	{
		goto IL_004b;
	}

IL_0013:
	{
		int32_t L_4 = V_1;
		if ((((int32_t)L_4) == ((int32_t)((int32_t)24))))
		{
			goto IL_0035;
		}
	}
	{
		int32_t L_5 = V_1;
		if ((((int32_t)L_5) == ((int32_t)((int32_t)32))))
		{
			goto IL_0040;
		}
	}
	{
		goto IL_004b;
	}

IL_001f:
	{
		int32_t L_6 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = ___pcmData1;
		bool L_8 = ___isDataInLittleEndianFormat2;
		PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965 * L_9 = (PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965 *)il2cpp_codegen_object_new(PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965_il2cpp_TypeInfo_var);
		PcmData8Bit__ctor_mD8EE69468C091A19FDC95B6456B20DB18DE0C53A(L_9, L_6, L_7, L_8, /*hidden argument*/NULL);
		V_0 = L_9;
		goto IL_0066;
	}

IL_002a:
	{
		int32_t L_10 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_11 = ___pcmData1;
		bool L_12 = ___isDataInLittleEndianFormat2;
		PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2 * L_13 = (PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2 *)il2cpp_codegen_object_new(PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2_il2cpp_TypeInfo_var);
		PcmData16Bit__ctor_m8185692C2B4259CF6558004F94436054E8207DD1(L_13, L_10, L_11, L_12, /*hidden argument*/NULL);
		V_0 = L_13;
		goto IL_0066;
	}

IL_0035:
	{
		int32_t L_14 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_15 = ___pcmData1;
		bool L_16 = ___isDataInLittleEndianFormat2;
		PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA * L_17 = (PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA *)il2cpp_codegen_object_new(PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA_il2cpp_TypeInfo_var);
		PcmData24Bit__ctor_mB2218C1B89BBA0AFF0DD8284097D35FAAA62B916(L_17, L_14, L_15, L_16, /*hidden argument*/NULL);
		V_0 = L_17;
		goto IL_0066;
	}

IL_0040:
	{
		int32_t L_18 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_19 = ___pcmData1;
		bool L_20 = ___isDataInLittleEndianFormat2;
		PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7 * L_21 = (PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7 *)il2cpp_codegen_object_new(PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7_il2cpp_TypeInfo_var);
		PcmData32Bit__ctor_mA1F9B042EAE8C52FA58C4A1DEA4849A37791D3F3(L_21, L_18, L_19, L_20, /*hidden argument*/NULL);
		V_0 = L_21;
		goto IL_0066;
	}

IL_004b:
	{
		int32_t L_22 = ___bits0;
		int32_t L_23 = L_22;
		RuntimeObject * L_24 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_23);
		String_t* L_25 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralAF1F890FB609377FDA8E938B6F8DC24DE11C19B0, L_24, _stringLiteralE394A332E3C6FA7617E76D91BC28B84D6A24E4C6, /*hidden argument*/NULL);
		Exception_t * L_26 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_26, L_25, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_26, PcmData_Create_mEDE8A9F6F9942B882C6CF78E388D1E0720DBEE4C_RuntimeMethod_var);
	}

IL_0066:
	{
		PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC * L_27 = V_0;
		return L_27;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData16Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData16Bit__ctor_m8185692C2B4259CF6558004F94436054E8207DD1 (PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2 * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___pcmData1;
		bool L_2 = ___isDataInLittleEndianFormat2;
		PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561(__this, L_0, L_1, L_2, /*hidden argument*/NULL);
		return;
	}
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Wave.PcmData16Bit::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PcmData16Bit_get_Item_m73328CF2CD6948F5C849A86BB81219724A887370 (PcmData16Bit_t6BDB2A11307C3461F3673DAD67C25416E1C4D8D2 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		int32_t L_0 = ___index0;
		___index0 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)2));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_2 = ___index0;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		uint8_t L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_5 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_6 = ___index0;
		NullCheck(L_5);
		int32_t L_7 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
		uint8_t L_8 = (L_5)->GetAt(static_cast<il2cpp_array_size_t>(L_7));
		V_0 = ((float)((float)(((float)((float)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)L_4|(int32_t)((int32_t)((int32_t)L_8<<(int32_t)8))))<<(int32_t)((int32_t)16)))>>(int32_t)((int32_t)16))))))/(float)(32768.0f)));
		goto IL_002b;
	}

IL_002b:
	{
		float L_9 = V_0;
		return L_9;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData24Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData24Bit__ctor_mB2218C1B89BBA0AFF0DD8284097D35FAAA62B916 (PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___pcmData1;
		bool L_2 = ___isDataInLittleEndianFormat2;
		PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561(__this, L_0, L_1, L_2, /*hidden argument*/NULL);
		return;
	}
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Wave.PcmData24Bit::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PcmData24Bit_get_Item_mCAD2281C1DC4B7A7CD2EB4CDF0CAD36A8DE1639B (PcmData24Bit_tE922125E1CD0CA654C64885DC7D9D1A7FB0E00BA * __this, int32_t ___index0, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		int32_t L_0 = ___index0;
		___index0 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)3));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_2 = ___index0;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		uint8_t L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_5 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_6 = ___index0;
		NullCheck(L_5);
		int32_t L_7 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
		uint8_t L_8 = (L_5)->GetAt(static_cast<il2cpp_array_size_t>(L_7));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_9 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_10 = ___index0;
		NullCheck(L_9);
		int32_t L_11 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)2));
		uint8_t L_12 = (L_9)->GetAt(static_cast<il2cpp_array_size_t>(L_11));
		V_0 = ((float)((float)(((float)((float)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)L_4|(int32_t)((int32_t)((int32_t)L_8<<(int32_t)8))))|(int32_t)((int32_t)((int32_t)L_12<<(int32_t)((int32_t)16)))))<<(int32_t)((int32_t)12)))>>(int32_t)((int32_t)12))))))/(float)(8388608.0f)));
		goto IL_0039;
	}

IL_0039:
	{
		float L_13 = V_0;
		return L_13;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData32Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData32Bit__ctor_mA1F9B042EAE8C52FA58C4A1DEA4849A37791D3F3 (PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7 * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___pcmData1;
		bool L_2 = ___isDataInLittleEndianFormat2;
		PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561(__this, L_0, L_1, L_2, /*hidden argument*/NULL);
		return;
	}
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Wave.PcmData32Bit::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PcmData32Bit_get_Item_m3F8D4FB0C837B98613E74BA47CAD42A1F6CC18C0 (PcmData32Bit_t9AE5966783DA2AB37289B1A49FAFACFF0987E2A7 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		int32_t L_0 = ___index0;
		___index0 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)4));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_2 = ___index0;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		uint8_t L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_5 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_6 = ___index0;
		NullCheck(L_5);
		int32_t L_7 = ((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1));
		uint8_t L_8 = (L_5)->GetAt(static_cast<il2cpp_array_size_t>(L_7));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_9 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_10 = ___index0;
		NullCheck(L_9);
		int32_t L_11 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)2));
		uint8_t L_12 = (L_9)->GetAt(static_cast<il2cpp_array_size_t>(L_11));
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_13 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_14 = ___index0;
		NullCheck(L_13);
		int32_t L_15 = ((int32_t)il2cpp_codegen_add((int32_t)L_14, (int32_t)3));
		uint8_t L_16 = (L_13)->GetAt(static_cast<il2cpp_array_size_t>(L_15));
		V_0 = ((float)((float)(((float)((float)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)((int32_t)L_4|(int32_t)((int32_t)((int32_t)L_8<<(int32_t)8))))|(int32_t)((int32_t)((int32_t)L_12<<(int32_t)((int32_t)16)))))|(int32_t)((int32_t)((int32_t)L_16<<(int32_t)((int32_t)24))))))))/(float)(2.14748365E+09f)));
		goto IL_0041;
	}

IL_0041:
	{
		float L_17 = V_0;
		return L_17;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.PcmData8Bit::.ctor(System.Int32,System.Byte[],System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PcmData8Bit__ctor_mD8EE69468C091A19FDC95B6456B20DB18DE0C53A (PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965 * __this, int32_t ___bits0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData1, bool ___isDataInLittleEndianFormat2, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___bits0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___pcmData1;
		bool L_2 = ___isDataInLittleEndianFormat2;
		PcmData__ctor_m9CD7638DC2C32E94B8F4E2D3BF2DC309D3324561(__this, L_0, L_1, L_2, /*hidden argument*/NULL);
		return;
	}
}
// System.Single DaggerfallWorkshop.AudioSynthesis.Wave.PcmData8Bit::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PcmData8Bit_get_Item_m6FB25CCCAD6AB9D3D365475587B15E9DD6E60AA4 (PcmData8Bit_t8FC3A1EF9C501B38320AEA0F88BECB3F3F234965 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = ((PcmData_t8CB8F99CA9B6BD69A1ADC0950689DBB6B8CD52FC *)__this)->get_data_0();
		int32_t L_1 = ___index0;
		NullCheck(L_0);
		int32_t L_2 = L_1;
		uint8_t L_3 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_2));
		V_0 = ((float)il2cpp_codegen_subtract((float)((float)il2cpp_codegen_multiply((float)((float)((float)(((float)((float)L_3)))/(float)(255.0f))), (float)(2.0f))), (float)(1.0f)));
		goto IL_001f;
	}

IL_001f:
	{
		float L_4 = V_0;
		return L_4;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Byte[] DaggerfallWorkshop.AudioSynthesis.Wave.WaveHelper::GetChannelPcmData(System.Byte[],System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* WaveHelper_GetChannelPcmData_m399AD7F1A9592B079B810D4A57ACCCBE06657581 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___pcmData0, int32_t ___bits1, int32_t ___channelCount2, int32_t ___expectedChannels3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WaveHelper_GetChannelPcmData_m399AD7F1A9592B079B810D4A57ACCCBE06657581_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_2 = NULL;
	int32_t V_3 = 0;
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_6 = NULL;
	bool V_7 = false;
	{
		int32_t L_0 = ___bits1;
		V_0 = ((int32_t)((int32_t)L_0/(int32_t)8));
		int32_t L_1 = ___expectedChannels3;
		int32_t L_2 = ___channelCount2;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		int32_t L_3 = Math_Min_mC950438198519FB2B0260FCB91220847EE4BB525(L_1, L_2, /*hidden argument*/NULL);
		V_1 = L_3;
		int32_t L_4 = ___expectedChannels3;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_5 = ___pcmData0;
		NullCheck(L_5);
		int32_t L_6 = ___channelCount2;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)SZArrayNew(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var, (uint32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_4, (int32_t)((int32_t)((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_5)->max_length))))/(int32_t)L_6)))));
		V_2 = L_7;
		int32_t L_8 = V_0;
		int32_t L_9 = ___channelCount2;
		V_3 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_8, (int32_t)L_9));
		int32_t L_10 = V_0;
		int32_t L_11 = V_1;
		V_4 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_10, (int32_t)L_11));
		V_5 = 0;
		goto IL_0041;
	}

IL_0028:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_12 = ___pcmData0;
		int32_t L_13 = V_5;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_14 = V_2;
		int32_t L_15 = V_5;
		int32_t L_16 = V_3;
		int32_t L_17 = V_4;
		int32_t L_18 = V_4;
		Array_Copy_mA10D079DD8D9700CA44721A219A934A2397653F6((RuntimeArray *)(RuntimeArray *)L_12, L_13, (RuntimeArray *)(RuntimeArray *)L_14, ((int32_t)il2cpp_codegen_multiply((int32_t)((int32_t)((int32_t)L_15/(int32_t)L_16)), (int32_t)L_17)), L_18, /*hidden argument*/NULL);
		int32_t L_19 = V_5;
		int32_t L_20 = V_3;
		V_5 = ((int32_t)il2cpp_codegen_add((int32_t)L_19, (int32_t)L_20));
	}

IL_0041:
	{
		int32_t L_21 = V_5;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_22 = ___pcmData0;
		NullCheck(L_22);
		V_7 = (bool)((((int32_t)L_21) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_22)->max_length))))))? 1 : 0);
		bool L_23 = V_7;
		if (L_23)
		{
			goto IL_0028;
		}
	}
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_24 = V_2;
		V_6 = L_24;
		goto IL_0053;
	}

IL_0053:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_25 = V_6;
		return L_25;
	}
}
// System.Void DaggerfallWorkshop.AudioSynthesis.Wave.WaveHelper::SwapEndianess(System.Byte[],System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WaveHelper_SwapEndianess_mB4C9A2AC9A10F41C2F9BDC11EAFD474EF1386845 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___data0, int32_t ___bits1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (WaveHelper_SwapEndianess_mB4C9A2AC9A10F41C2F9BDC11EAFD474EF1386845_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_0 = NULL;
	int32_t V_1 = 0;
	bool V_2 = false;
	{
		int32_t L_0 = ___bits1;
		___bits1 = ((int32_t)((int32_t)L_0/(int32_t)8));
		int32_t L_1 = ___bits1;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_2 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)SZArrayNew(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var, (uint32_t)L_1);
		V_0 = L_2;
		V_1 = 0;
		goto IL_0034;
	}

IL_0011:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_3 = ___data0;
		int32_t L_4 = V_1;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_5 = V_0;
		int32_t L_6 = ___bits1;
		Array_Copy_mA10D079DD8D9700CA44721A219A934A2397653F6((RuntimeArray *)(RuntimeArray *)L_3, L_4, (RuntimeArray *)(RuntimeArray *)L_5, 0, L_6, /*hidden argument*/NULL);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = V_0;
		Array_Reverse_mF6A81D8EC8E17D7B3BE5F9B4EE763E3D43E57440((RuntimeArray *)(RuntimeArray *)L_7, /*hidden argument*/NULL);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_8 = V_0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_9 = ___data0;
		int32_t L_10 = V_1;
		int32_t L_11 = ___bits1;
		Array_Copy_mA10D079DD8D9700CA44721A219A934A2397653F6((RuntimeArray *)(RuntimeArray *)L_8, 0, (RuntimeArray *)(RuntimeArray *)L_9, L_10, L_11, /*hidden argument*/NULL);
		int32_t L_12 = V_1;
		int32_t L_13 = ___bits1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_12, (int32_t)L_13));
	}

IL_0034:
	{
		int32_t L_14 = V_1;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_15 = ___data0;
		NullCheck(L_15);
		V_2 = (bool)((((int32_t)L_14) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_15)->max_length))))))? 1 : 0);
		bool L_16 = V_2;
		if (L_16)
		{
			goto IL_0011;
		}
	}
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline (String_t* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_m_stringLength_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t Queue_1_get_Count_mFEE12FE1484863514D6F54B92A471361F6AB2C9D_gshared_inline (Queue_1_t82EC7A92F1285D89D455EFE26CF00DDF4EB26E8C * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get__size_3();
		return (int32_t)L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * LinkedList_1_get_First_m0C98E2DE4C013B92EDF858C9A5DEA9A30BB5523C_gshared_inline (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, const RuntimeMethod* method)
{
	{
		LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * L_0 = (LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 *)__this->get_head_0();
		return (LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 *)L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR RuntimeObject * LinkedListNode_1_get_Value_m36A53343597D289FE50219266EDE929003F0EA89_gshared_inline (LinkedListNode_1_t29FE2977C490DD49F9F19A1FCBD4B2510F580683 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_item_3();
		return (RuntimeObject *)L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t LinkedList_1_get_Count_m3FEDB19F06F4B650469DB1D5D2308832AC52B75D_gshared_inline (LinkedList_1_t53CE3B6C8AC75667A89B320FD72FAF18BAB09384 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_count_1();
		return (int32_t)L_0;
	}
}
