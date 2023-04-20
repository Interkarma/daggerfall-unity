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

template <typename R, typename T1, typename T2>
struct VirtFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct GenericVirtFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct InterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct GenericInterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};

// Microsoft.CodeAnalysis.EmbeddedAttribute
struct EmbeddedAttribute_t1911FF370C2DCB631528386EA2A75E72C8B94CCA;
// System.ArgumentException
struct ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1;
// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Attribute
struct Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Collections.Generic.IEnumerator`1<System.Int32>
struct IEnumerator_1_t7348E69CA57FC75395C9BBB4A9FBB33953F29F27;
// System.Collections.IDictionary
struct IDictionary_t1BD5C1546718A374EA8122FBD6C6EE45331E8CE7;
// System.Collections.IEnumerator
struct IEnumerator_t8789118187258CC88B77AFAC6315B5AF87D3E18A;
// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE;
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.IntPtr[]
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD;
// System.NotImplementedException
struct NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Runtime.CompilerServices.IsUnmanagedAttribute
struct IsUnmanagedAttribute_t861EFFE3B040EF1C98B66A8008E18FD7FE360621;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770;
// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$PostfixBurstDelegate
struct Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46;
// Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$PostfixBurstDelegate
struct Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197;
// Unity.Collections.AllocatorManager/TryFunction
struct TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268;

IL2CPP_EXTERN_C RuntimeClass* AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtr_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteralE8AFCAC606B8AF997C2C44DB820ED0DA9DEBBAA4;
IL2CPP_EXTERN_C String_t* _stringLiteralEE37BE011FCAC99E8AC621D6F90BC4B75E848823;
IL2CPP_EXTERN_C const RuntimeMethod* AllocatorManager_Allocate_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mB55752C04C2E0BF01026A46F7628A6A6C83A1B96_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AllocatorManager_Free_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mC9DEFCD77EAF09FCF9CF7C11E3C9D8233DDF9950_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FunctionPointer_1__ctor_m88698023CA5671A2E19B7EF5076962FBD71EEC45_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* FunctionPointer_1_get_Invoke_m031425B62699967966A4CAF4D062D1197B730E5E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_GetOrCreateUnsafe_m175F8547F0CF4DBA988A8BC79AF6F443590B595A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SharedStatic_1_get_Data_m2585600653432550FCB33327F0EC2989C76F5DC6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SlabAllocator_TryU24BurstManaged_m0E85623C6B44B7C3B6A5829BF73EFF1BD5A1E8B1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SlabAllocator_Try_mC1CF8ACD30D0745D62FB9E07BF900AD0743FA85F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* StackAllocator_TryU24BurstManaged_m46B078D3E5C2608D24398E4A9B1AA71F352F3FBE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* StackAllocator_Try_mB50327267831F16523E96A7E7FF4829A2EE4C60E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnsafeUtility_SizeOf_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mF5121761EBFDF720A7C139604AFCBAA28008C170_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* UnsafeUtility_WriteArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m21A03DD8050619772A9117BE97EDD6CF543115EA_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeType* Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46_0_0_0_var;
IL2CPP_EXTERN_C const uint32_t AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AllocatorManager_Free_mDE1594E464749B50FF597BC1080C547AE5DD7634_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AllocatorManager_TryLegacy_m8FA25D28AD957F9E1695FB2FFC9BF65338E48BCF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AllocatorManager__cctor_mCD35A58B61B05634E2E7F220CD5A1B5350A567B8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt128_Equals_mB686B28CC8F05B63BB4246D93B00ECB2AC62D31C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt32_Equals_m57F5A272F557C2BC73A44A7FAF2C80AFC572AECE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt4096_Equals_m6425DE1D251DE6D5988FFE46AEBEA141B33F08A7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt512_Equals_m2273812A8319ADB7E01CBF949ACA4B090982A134_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt64_Equals_mA735702468591FA565B01C107F10E86E5D2C817A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SlabAllocator_Try_mC1CF8ACD30D0745D62FB9E07BF900AD0743FA85F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t StackAllocator_Try_mB50327267831F16523E96A7E7FF4829A2EE4C60E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t StaticFunctionTable__cctor_m3D2D05C6947655D0683241ACD39F9017756DE0B1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TryFunction_BeginInvoke_m154FD78AF32570B8B1EA1B807864BA46C5B7C873_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000AC0U24BurstDirectCall_Constructor_m0D678E2482010B83B9C308B93E82FF5251FAC73A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000AC0U24BurstDirectCall_GetFunctionPointerDiscard_m8840DE8BB24CFF03BAE3663B68B0BBB0B7A9950A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000AC0U24BurstDirectCall_GetFunctionPointer_mF5ACDA3A6A948DD3EDBDD62EBA0807FAA1B70CC6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000AC0U24BurstDirectCall_Invoke_m06D304524A473E4768EC248F5F9853E51FF43DD5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000AC0U24PostfixBurstDelegate_BeginInvoke_m6A5938036CCF6C398B498030D083B9F8E2ED9A0F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000ACAU24BurstDirectCall_Constructor_m505A404645A774A1EECA51E6288C6426BD5FAE00_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000ACAU24BurstDirectCall_GetFunctionPointerDiscard_m6D57CFDB0E90300242AD61433E9B2E9E161AC1A9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000ACAU24BurstDirectCall_GetFunctionPointer_mE960ACE388298074ADD1CEF79DD565A04158BFAD_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000ACAU24BurstDirectCall_Invoke_mEBE8EE6CA0E3E1822B42856658237A0ABF3DD68B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Try_00000ACAU24PostfixBurstDelegate_BeginInvoke_mB854D30E6267FE5EB11D82B1DEDB46CBB8005ADF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U24BurstDirectCallInitializer_Initialize_mDCD2B8EE19E4E17A8E64F1F0C8075E50F422C913_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t UnsafeList_Create_mC233310AC75D512F5929FE2BF9EBD8CDE3E944E6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t UnsafeList_Destroy_mCB345E35771EEF73569EF65D07DC63A09803F7C2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct U3CModuleU3E_t6D5723898D6A268FC74BBAE1CD150C05603EBA2E 
{
public:

public:
};


// System.Object


// $BurstDirectCallInitializer
struct U24BurstDirectCallInitializer_tE194BE57E81948AA9302E0B5DC29BCBFBC5F246A  : public RuntimeObject
{
public:

public:
};

struct Il2CppArrayBounds;

// System.Array


// System.Attribute
struct Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74  : public RuntimeObject
{
public:

public:
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

// Unity.Collections.CollectionHelper
struct CollectionHelper_t828C4B208A5D1DB4546AFA458529E673EC9F5DC9  : public RuntimeObject
{
public:

public:
};


// Unity.Collections.FixedListInt128DebugView
struct FixedListInt128DebugView_t121FE2A45BDA8A449CC77B4CCDAADCCD46C3FC3D  : public RuntimeObject
{
public:

public:
};


// Unity.Collections.FixedListInt32DebugView
struct FixedListInt32DebugView_t1C2D5D2F8563A13BBAA0DE2FD95A5522D4294856  : public RuntimeObject
{
public:

public:
};


// Unity.Collections.FixedListInt4096DebugView
struct FixedListInt4096DebugView_t468D142D3BE37EACBCE2AAC403AFCDBA93747F12  : public RuntimeObject
{
public:

public:
};


// Unity.Collections.FixedListInt512DebugView
struct FixedListInt512DebugView_tC0DB276B9ADCE77C89F886E9F6FB11F8832A5898  : public RuntimeObject
{
public:

public:
};


// Unity.Collections.FixedListInt64DebugView
struct FixedListInt64DebugView_t768F52C766CE18CD1D5CDCB7F699CE0833B5653A  : public RuntimeObject
{
public:

public:
};


// Microsoft.CodeAnalysis.EmbeddedAttribute
struct EmbeddedAttribute_t1911FF370C2DCB631528386EA2A75E72C8B94CCA  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
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


// System.Runtime.CompilerServices.IsUnmanagedAttribute
struct IsUnmanagedAttribute_t861EFFE3B040EF1C98B66A8008E18FD7FE360621  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
{
public:

public:
};


// System.UInt16
struct UInt16_tAE45CEF73BF720100519F6867F32145D075F928E 
{
public:
	// System.UInt16 System.UInt16::m_value
	uint16_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt16_tAE45CEF73BF720100519F6867F32145D075F928E, ___m_value_0)); }
	inline uint16_t get_m_value_0() const { return ___m_value_0; }
	inline uint16_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint16_t value)
	{
		___m_value_0 = value;
	}
};


// System.UInt32
struct UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B 
{
public:
	// System.UInt32 System.UInt32::m_value
	uint32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B, ___m_value_0)); }
	inline uint32_t get_m_value_0() const { return ___m_value_0; }
	inline uint32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint32_t value)
	{
		___m_value_0 = value;
	}
};


// System.UInt64
struct UInt64_tA02DF3B59C8FC4A849BD207DA11038CC64E4CB4E 
{
public:
	// System.UInt64 System.UInt64::m_value
	uint64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt64_tA02DF3B59C8FC4A849BD207DA11038CC64E4CB4E, ___m_value_0)); }
	inline uint64_t get_m_value_0() const { return ___m_value_0; }
	inline uint64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint64_t value)
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


// Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/TableEntry65536>
struct SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F 
{
public:
	// System.Void* Unity.Burst.SharedStatic`1::_buffer
	void* ____buffer_0;

public:
	inline static int32_t get_offset_of__buffer_0() { return static_cast<int32_t>(offsetof(SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F, ____buffer_0)); }
	inline void* get__buffer_0() const { return ____buffer_0; }
	inline void** get_address_of__buffer_0() { return &____buffer_0; }
	inline void set__buffer_0(void* value)
	{
		____buffer_0 = value;
	}
};


// Unity.Collections.AllocatorManager/AllocatorHandle
struct AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 
{
public:
	// System.Int32 Unity.Collections.AllocatorManager/AllocatorHandle::Value
	int32_t ___Value_0;

public:
	inline static int32_t get_offset_of_Value_0() { return static_cast<int32_t>(offsetof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07, ___Value_0)); }
	inline int32_t get_Value_0() const { return ___Value_0; }
	inline int32_t* get_address_of_Value_0() { return &___Value_0; }
	inline void set_Value_0(int32_t value)
	{
		___Value_0 = value;
	}
};


// Unity.Collections.AllocatorManager/BlockHandle
struct BlockHandle_t06584EAE324E1124DEE24393CE3DDA434C9F1717 
{
public:
	// System.UInt16 Unity.Collections.AllocatorManager/BlockHandle::Value
	uint16_t ___Value_0;

public:
	inline static int32_t get_offset_of_Value_0() { return static_cast<int32_t>(offsetof(BlockHandle_t06584EAE324E1124DEE24393CE3DDA434C9F1717, ___Value_0)); }
	inline uint16_t get_Value_0() const { return ___Value_0; }
	inline uint16_t* get_address_of_Value_0() { return &___Value_0; }
	inline void set_Value_0(uint16_t value)
	{
		___Value_0 = value;
	}
};


// Unity.Collections.AllocatorManager/SmallAllocatorHandle
struct SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 
{
public:
	// System.UInt16 Unity.Collections.AllocatorManager/SmallAllocatorHandle::Value
	uint16_t ___Value_0;

public:
	inline static int32_t get_offset_of_Value_0() { return static_cast<int32_t>(offsetof(SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9, ___Value_0)); }
	inline uint16_t get_Value_0() const { return ___Value_0; }
	inline uint16_t* get_address_of_Value_0() { return &___Value_0; }
	inline void set_Value_0(uint16_t value)
	{
		___Value_0 = value;
	}
};


// Unity.Collections.FixedBytes16
struct FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.Byte Unity.Collections.FixedBytes16::byte0000
					uint8_t ___byte0000_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint8_t ___byte0000_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0001_1_OffsetPadding[1];
					// System.Byte Unity.Collections.FixedBytes16::byte0001
					uint8_t ___byte0001_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0001_1_OffsetPadding_forAlignmentOnly[1];
					uint8_t ___byte0001_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0002_2_OffsetPadding[2];
					// System.Byte Unity.Collections.FixedBytes16::byte0002
					uint8_t ___byte0002_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0002_2_OffsetPadding_forAlignmentOnly[2];
					uint8_t ___byte0002_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0003_3_OffsetPadding[3];
					// System.Byte Unity.Collections.FixedBytes16::byte0003
					uint8_t ___byte0003_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0003_3_OffsetPadding_forAlignmentOnly[3];
					uint8_t ___byte0003_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0004_4_OffsetPadding[4];
					// System.Byte Unity.Collections.FixedBytes16::byte0004
					uint8_t ___byte0004_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0004_4_OffsetPadding_forAlignmentOnly[4];
					uint8_t ___byte0004_4_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0005_5_OffsetPadding[5];
					// System.Byte Unity.Collections.FixedBytes16::byte0005
					uint8_t ___byte0005_5;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0005_5_OffsetPadding_forAlignmentOnly[5];
					uint8_t ___byte0005_5_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0006_6_OffsetPadding[6];
					// System.Byte Unity.Collections.FixedBytes16::byte0006
					uint8_t ___byte0006_6;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0006_6_OffsetPadding_forAlignmentOnly[6];
					uint8_t ___byte0006_6_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0007_7_OffsetPadding[7];
					// System.Byte Unity.Collections.FixedBytes16::byte0007
					uint8_t ___byte0007_7;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0007_7_OffsetPadding_forAlignmentOnly[7];
					uint8_t ___byte0007_7_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0008_8_OffsetPadding[8];
					// System.Byte Unity.Collections.FixedBytes16::byte0008
					uint8_t ___byte0008_8;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0008_8_OffsetPadding_forAlignmentOnly[8];
					uint8_t ___byte0008_8_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0009_9_OffsetPadding[9];
					// System.Byte Unity.Collections.FixedBytes16::byte0009
					uint8_t ___byte0009_9;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0009_9_OffsetPadding_forAlignmentOnly[9];
					uint8_t ___byte0009_9_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0010_10_OffsetPadding[10];
					// System.Byte Unity.Collections.FixedBytes16::byte0010
					uint8_t ___byte0010_10;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0010_10_OffsetPadding_forAlignmentOnly[10];
					uint8_t ___byte0010_10_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0011_11_OffsetPadding[11];
					// System.Byte Unity.Collections.FixedBytes16::byte0011
					uint8_t ___byte0011_11;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0011_11_OffsetPadding_forAlignmentOnly[11];
					uint8_t ___byte0011_11_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0012_12_OffsetPadding[12];
					// System.Byte Unity.Collections.FixedBytes16::byte0012
					uint8_t ___byte0012_12;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0012_12_OffsetPadding_forAlignmentOnly[12];
					uint8_t ___byte0012_12_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0013_13_OffsetPadding[13];
					// System.Byte Unity.Collections.FixedBytes16::byte0013
					uint8_t ___byte0013_13;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0013_13_OffsetPadding_forAlignmentOnly[13];
					uint8_t ___byte0013_13_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0014_14_OffsetPadding[14];
					// System.Byte Unity.Collections.FixedBytes16::byte0014
					uint8_t ___byte0014_14;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0014_14_OffsetPadding_forAlignmentOnly[14];
					uint8_t ___byte0014_14_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0015_15_OffsetPadding[15];
					// System.Byte Unity.Collections.FixedBytes16::byte0015
					uint8_t ___byte0015_15;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0015_15_OffsetPadding_forAlignmentOnly[15];
					uint8_t ___byte0015_15_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45__padding[16];
	};

public:
	inline static int32_t get_offset_of_byte0000_0() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0000_0)); }
	inline uint8_t get_byte0000_0() const { return ___byte0000_0; }
	inline uint8_t* get_address_of_byte0000_0() { return &___byte0000_0; }
	inline void set_byte0000_0(uint8_t value)
	{
		___byte0000_0 = value;
	}

	inline static int32_t get_offset_of_byte0001_1() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0001_1)); }
	inline uint8_t get_byte0001_1() const { return ___byte0001_1; }
	inline uint8_t* get_address_of_byte0001_1() { return &___byte0001_1; }
	inline void set_byte0001_1(uint8_t value)
	{
		___byte0001_1 = value;
	}

	inline static int32_t get_offset_of_byte0002_2() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0002_2)); }
	inline uint8_t get_byte0002_2() const { return ___byte0002_2; }
	inline uint8_t* get_address_of_byte0002_2() { return &___byte0002_2; }
	inline void set_byte0002_2(uint8_t value)
	{
		___byte0002_2 = value;
	}

	inline static int32_t get_offset_of_byte0003_3() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0003_3)); }
	inline uint8_t get_byte0003_3() const { return ___byte0003_3; }
	inline uint8_t* get_address_of_byte0003_3() { return &___byte0003_3; }
	inline void set_byte0003_3(uint8_t value)
	{
		___byte0003_3 = value;
	}

	inline static int32_t get_offset_of_byte0004_4() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0004_4)); }
	inline uint8_t get_byte0004_4() const { return ___byte0004_4; }
	inline uint8_t* get_address_of_byte0004_4() { return &___byte0004_4; }
	inline void set_byte0004_4(uint8_t value)
	{
		___byte0004_4 = value;
	}

	inline static int32_t get_offset_of_byte0005_5() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0005_5)); }
	inline uint8_t get_byte0005_5() const { return ___byte0005_5; }
	inline uint8_t* get_address_of_byte0005_5() { return &___byte0005_5; }
	inline void set_byte0005_5(uint8_t value)
	{
		___byte0005_5 = value;
	}

	inline static int32_t get_offset_of_byte0006_6() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0006_6)); }
	inline uint8_t get_byte0006_6() const { return ___byte0006_6; }
	inline uint8_t* get_address_of_byte0006_6() { return &___byte0006_6; }
	inline void set_byte0006_6(uint8_t value)
	{
		___byte0006_6 = value;
	}

	inline static int32_t get_offset_of_byte0007_7() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0007_7)); }
	inline uint8_t get_byte0007_7() const { return ___byte0007_7; }
	inline uint8_t* get_address_of_byte0007_7() { return &___byte0007_7; }
	inline void set_byte0007_7(uint8_t value)
	{
		___byte0007_7 = value;
	}

	inline static int32_t get_offset_of_byte0008_8() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0008_8)); }
	inline uint8_t get_byte0008_8() const { return ___byte0008_8; }
	inline uint8_t* get_address_of_byte0008_8() { return &___byte0008_8; }
	inline void set_byte0008_8(uint8_t value)
	{
		___byte0008_8 = value;
	}

	inline static int32_t get_offset_of_byte0009_9() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0009_9)); }
	inline uint8_t get_byte0009_9() const { return ___byte0009_9; }
	inline uint8_t* get_address_of_byte0009_9() { return &___byte0009_9; }
	inline void set_byte0009_9(uint8_t value)
	{
		___byte0009_9 = value;
	}

	inline static int32_t get_offset_of_byte0010_10() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0010_10)); }
	inline uint8_t get_byte0010_10() const { return ___byte0010_10; }
	inline uint8_t* get_address_of_byte0010_10() { return &___byte0010_10; }
	inline void set_byte0010_10(uint8_t value)
	{
		___byte0010_10 = value;
	}

	inline static int32_t get_offset_of_byte0011_11() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0011_11)); }
	inline uint8_t get_byte0011_11() const { return ___byte0011_11; }
	inline uint8_t* get_address_of_byte0011_11() { return &___byte0011_11; }
	inline void set_byte0011_11(uint8_t value)
	{
		___byte0011_11 = value;
	}

	inline static int32_t get_offset_of_byte0012_12() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0012_12)); }
	inline uint8_t get_byte0012_12() const { return ___byte0012_12; }
	inline uint8_t* get_address_of_byte0012_12() { return &___byte0012_12; }
	inline void set_byte0012_12(uint8_t value)
	{
		___byte0012_12 = value;
	}

	inline static int32_t get_offset_of_byte0013_13() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0013_13)); }
	inline uint8_t get_byte0013_13() const { return ___byte0013_13; }
	inline uint8_t* get_address_of_byte0013_13() { return &___byte0013_13; }
	inline void set_byte0013_13(uint8_t value)
	{
		___byte0013_13 = value;
	}

	inline static int32_t get_offset_of_byte0014_14() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0014_14)); }
	inline uint8_t get_byte0014_14() const { return ___byte0014_14; }
	inline uint8_t* get_address_of_byte0014_14() { return &___byte0014_14; }
	inline void set_byte0014_14(uint8_t value)
	{
		___byte0014_14 = value;
	}

	inline static int32_t get_offset_of_byte0015_15() { return static_cast<int32_t>(offsetof(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45, ___byte0015_15)); }
	inline uint8_t get_byte0015_15() const { return ___byte0015_15; }
	inline uint8_t* get_address_of_byte0015_15() { return &___byte0015_15; }
	inline void set_byte0015_15(uint8_t value)
	{
		___byte0015_15 = value;
	}
};


// Unity.Collections.FixedList
struct FixedList_tE8D42E3CEF3AD688B0F1BBC593BAD69AB2B5E6F0 
{
public:
	union
	{
		struct
		{
		};
		uint8_t FixedList_tE8D42E3CEF3AD688B0F1BBC593BAD69AB2B5E6F0__padding[1];
	};

public:
};


// Unity.Mathematics.math/LongDoubleUnion
struct LongDoubleUnion_tEA9A08E85EB44174AE90CB7541C8306BDDF61ECD 
{
public:
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int64 Unity.Mathematics.math/LongDoubleUnion::longValue
			int64_t ___longValue_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			int64_t ___longValue_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Double Unity.Mathematics.math/LongDoubleUnion::doubleValue
			double ___doubleValue_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			double ___doubleValue_1_forAlignmentOnly;
		};
	};

public:
	inline static int32_t get_offset_of_longValue_0() { return static_cast<int32_t>(offsetof(LongDoubleUnion_tEA9A08E85EB44174AE90CB7541C8306BDDF61ECD, ___longValue_0)); }
	inline int64_t get_longValue_0() const { return ___longValue_0; }
	inline int64_t* get_address_of_longValue_0() { return &___longValue_0; }
	inline void set_longValue_0(int64_t value)
	{
		___longValue_0 = value;
	}

	inline static int32_t get_offset_of_doubleValue_1() { return static_cast<int32_t>(offsetof(LongDoubleUnion_tEA9A08E85EB44174AE90CB7541C8306BDDF61ECD, ___doubleValue_1)); }
	inline double get_doubleValue_1() const { return ___doubleValue_1; }
	inline double* get_address_of_doubleValue_1() { return &___doubleValue_1; }
	inline void set_doubleValue_1(double value)
	{
		___doubleValue_1 = value;
	}
};


// System.Delegate
struct Delegate_t  : public RuntimeObject
{
public:
	// System.IntPtr System.Delegate::method_ptr
	Il2CppMethodPointer ___method_ptr_0;
	// System.IntPtr System.Delegate::invoke_impl
	intptr_t ___invoke_impl_1;
	// System.Object System.Delegate::m_target
	RuntimeObject * ___m_target_2;
	// System.IntPtr System.Delegate::method
	intptr_t ___method_3;
	// System.IntPtr System.Delegate::delegate_trampoline
	intptr_t ___delegate_trampoline_4;
	// System.IntPtr System.Delegate::extra_arg
	intptr_t ___extra_arg_5;
	// System.IntPtr System.Delegate::method_code
	intptr_t ___method_code_6;
	// System.Reflection.MethodInfo System.Delegate::method_info
	MethodInfo_t * ___method_info_7;
	// System.Reflection.MethodInfo System.Delegate::original_method_info
	MethodInfo_t * ___original_method_info_8;
	// System.DelegateData System.Delegate::data
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	// System.Boolean System.Delegate::method_is_virtual
	bool ___method_is_virtual_10;

public:
	inline static int32_t get_offset_of_method_ptr_0() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_ptr_0)); }
	inline Il2CppMethodPointer get_method_ptr_0() const { return ___method_ptr_0; }
	inline Il2CppMethodPointer* get_address_of_method_ptr_0() { return &___method_ptr_0; }
	inline void set_method_ptr_0(Il2CppMethodPointer value)
	{
		___method_ptr_0 = value;
	}

	inline static int32_t get_offset_of_invoke_impl_1() { return static_cast<int32_t>(offsetof(Delegate_t, ___invoke_impl_1)); }
	inline intptr_t get_invoke_impl_1() const { return ___invoke_impl_1; }
	inline intptr_t* get_address_of_invoke_impl_1() { return &___invoke_impl_1; }
	inline void set_invoke_impl_1(intptr_t value)
	{
		___invoke_impl_1 = value;
	}

	inline static int32_t get_offset_of_m_target_2() { return static_cast<int32_t>(offsetof(Delegate_t, ___m_target_2)); }
	inline RuntimeObject * get_m_target_2() const { return ___m_target_2; }
	inline RuntimeObject ** get_address_of_m_target_2() { return &___m_target_2; }
	inline void set_m_target_2(RuntimeObject * value)
	{
		___m_target_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_target_2), (void*)value);
	}

	inline static int32_t get_offset_of_method_3() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_3)); }
	inline intptr_t get_method_3() const { return ___method_3; }
	inline intptr_t* get_address_of_method_3() { return &___method_3; }
	inline void set_method_3(intptr_t value)
	{
		___method_3 = value;
	}

	inline static int32_t get_offset_of_delegate_trampoline_4() { return static_cast<int32_t>(offsetof(Delegate_t, ___delegate_trampoline_4)); }
	inline intptr_t get_delegate_trampoline_4() const { return ___delegate_trampoline_4; }
	inline intptr_t* get_address_of_delegate_trampoline_4() { return &___delegate_trampoline_4; }
	inline void set_delegate_trampoline_4(intptr_t value)
	{
		___delegate_trampoline_4 = value;
	}

	inline static int32_t get_offset_of_extra_arg_5() { return static_cast<int32_t>(offsetof(Delegate_t, ___extra_arg_5)); }
	inline intptr_t get_extra_arg_5() const { return ___extra_arg_5; }
	inline intptr_t* get_address_of_extra_arg_5() { return &___extra_arg_5; }
	inline void set_extra_arg_5(intptr_t value)
	{
		___extra_arg_5 = value;
	}

	inline static int32_t get_offset_of_method_code_6() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_code_6)); }
	inline intptr_t get_method_code_6() const { return ___method_code_6; }
	inline intptr_t* get_address_of_method_code_6() { return &___method_code_6; }
	inline void set_method_code_6(intptr_t value)
	{
		___method_code_6 = value;
	}

	inline static int32_t get_offset_of_method_info_7() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_info_7)); }
	inline MethodInfo_t * get_method_info_7() const { return ___method_info_7; }
	inline MethodInfo_t ** get_address_of_method_info_7() { return &___method_info_7; }
	inline void set_method_info_7(MethodInfo_t * value)
	{
		___method_info_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___method_info_7), (void*)value);
	}

	inline static int32_t get_offset_of_original_method_info_8() { return static_cast<int32_t>(offsetof(Delegate_t, ___original_method_info_8)); }
	inline MethodInfo_t * get_original_method_info_8() const { return ___original_method_info_8; }
	inline MethodInfo_t ** get_address_of_original_method_info_8() { return &___original_method_info_8; }
	inline void set_original_method_info_8(MethodInfo_t * value)
	{
		___original_method_info_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___original_method_info_8), (void*)value);
	}

	inline static int32_t get_offset_of_data_9() { return static_cast<int32_t>(offsetof(Delegate_t, ___data_9)); }
	inline DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * get_data_9() const { return ___data_9; }
	inline DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE ** get_address_of_data_9() { return &___data_9; }
	inline void set_data_9(DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * value)
	{
		___data_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___data_9), (void*)value);
	}

	inline static int32_t get_offset_of_method_is_virtual_10() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_is_virtual_10)); }
	inline bool get_method_is_virtual_10() const { return ___method_is_virtual_10; }
	inline bool* get_address_of_method_is_virtual_10() { return &___method_is_virtual_10; }
	inline void set_method_is_virtual_10(bool value)
	{
		___method_is_virtual_10 = value;
	}
};

// Native definition for P/Invoke marshalling of System.Delegate
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	int32_t ___method_is_virtual_10;
};
// Native definition for COM marshalling of System.Delegate
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	int32_t ___method_is_virtual_10;
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

// System.RuntimeMethodHandle
struct RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F 
{
public:
	// System.IntPtr System.RuntimeMethodHandle::value
	intptr_t ___value_0;

public:
	inline static int32_t get_offset_of_value_0() { return static_cast<int32_t>(offsetof(RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F, ___value_0)); }
	inline intptr_t get_value_0() const { return ___value_0; }
	inline intptr_t* get_address_of_value_0() { return &___value_0; }
	inline void set_value_0(intptr_t value)
	{
		___value_0 = value;
	}
};


// System.RuntimeTypeHandle
struct RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D 
{
public:
	// System.IntPtr System.RuntimeTypeHandle::value
	intptr_t ___value_0;

public:
	inline static int32_t get_offset_of_value_0() { return static_cast<int32_t>(offsetof(RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D, ___value_0)); }
	inline intptr_t get_value_0() const { return ___value_0; }
	inline intptr_t* get_address_of_value_0() { return &___value_0; }
	inline void set_value_0(intptr_t value)
	{
		___value_0 = value;
	}
};


// Unity.Burst.FunctionPointer`1<System.Object>
struct FunctionPointer_1_t5AF97C37E92E5F70B805E2C94E6BB3582D040303 
{
public:
	// System.IntPtr Unity.Burst.FunctionPointer`1::_ptr
	intptr_t ____ptr_0;

public:
	inline static int32_t get_offset_of__ptr_0() { return static_cast<int32_t>(offsetof(FunctionPointer_1_t5AF97C37E92E5F70B805E2C94E6BB3582D040303, ____ptr_0)); }
	inline intptr_t get__ptr_0() const { return ____ptr_0; }
	inline intptr_t* get_address_of__ptr_0() { return &____ptr_0; }
	inline void set__ptr_0(intptr_t value)
	{
		____ptr_0 = value;
	}
};


// Unity.Burst.FunctionPointer`1<Unity.Collections.AllocatorManager/TryFunction>
struct FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 
{
public:
	// System.IntPtr Unity.Burst.FunctionPointer`1::_ptr
	intptr_t ____ptr_0;

public:
	inline static int32_t get_offset_of__ptr_0() { return static_cast<int32_t>(offsetof(FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89, ____ptr_0)); }
	inline intptr_t get__ptr_0() const { return ____ptr_0; }
	inline intptr_t* get_address_of__ptr_0() { return &____ptr_0; }
	inline void set__ptr_0(intptr_t value)
	{
		____ptr_0 = value;
	}
};


// Unity.Collections.Allocator
struct Allocator_t62A091275262E7067EAAD565B67764FA877D58D6 
{
public:
	// System.Int32 Unity.Collections.Allocator::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Allocator_t62A091275262E7067EAAD565B67764FA877D58D6, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Unity.Collections.AllocatorManager
struct AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5  : public RuntimeObject
{
public:

public:
};

struct AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields
{
public:
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager::Invalid
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___Invalid_0;
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager::None
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___None_1;
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager::Temp
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___Temp_2;
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager::TempJob
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___TempJob_3;
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager::Persistent
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___Persistent_4;
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager::AudioKernel
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___AudioKernel_5;

public:
	inline static int32_t get_offset_of_Invalid_0() { return static_cast<int32_t>(offsetof(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields, ___Invalid_0)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_Invalid_0() const { return ___Invalid_0; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_Invalid_0() { return &___Invalid_0; }
	inline void set_Invalid_0(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___Invalid_0 = value;
	}

	inline static int32_t get_offset_of_None_1() { return static_cast<int32_t>(offsetof(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields, ___None_1)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_None_1() const { return ___None_1; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_None_1() { return &___None_1; }
	inline void set_None_1(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___None_1 = value;
	}

	inline static int32_t get_offset_of_Temp_2() { return static_cast<int32_t>(offsetof(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields, ___Temp_2)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_Temp_2() const { return ___Temp_2; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_Temp_2() { return &___Temp_2; }
	inline void set_Temp_2(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___Temp_2 = value;
	}

	inline static int32_t get_offset_of_TempJob_3() { return static_cast<int32_t>(offsetof(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields, ___TempJob_3)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_TempJob_3() const { return ___TempJob_3; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_TempJob_3() { return &___TempJob_3; }
	inline void set_TempJob_3(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___TempJob_3 = value;
	}

	inline static int32_t get_offset_of_Persistent_4() { return static_cast<int32_t>(offsetof(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields, ___Persistent_4)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_Persistent_4() const { return ___Persistent_4; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_Persistent_4() { return &___Persistent_4; }
	inline void set_Persistent_4(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___Persistent_4 = value;
	}

	inline static int32_t get_offset_of_AudioKernel_5() { return static_cast<int32_t>(offsetof(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields, ___AudioKernel_5)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_AudioKernel_5() const { return ___AudioKernel_5; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_AudioKernel_5() { return &___AudioKernel_5; }
	inline void set_AudioKernel_5(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___AudioKernel_5 = value;
	}
};


// Unity.Collections.AllocatorManager/Range
struct Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E 
{
public:
	// System.IntPtr Unity.Collections.AllocatorManager/Range::Pointer
	intptr_t ___Pointer_0;
	// System.Int32 Unity.Collections.AllocatorManager/Range::Items
	int32_t ___Items_1;
	// Unity.Collections.AllocatorManager/SmallAllocatorHandle Unity.Collections.AllocatorManager/Range::Allocator
	SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  ___Allocator_2;
	// Unity.Collections.AllocatorManager/BlockHandle Unity.Collections.AllocatorManager/Range::Block
	BlockHandle_t06584EAE324E1124DEE24393CE3DDA434C9F1717  ___Block_3;

public:
	inline static int32_t get_offset_of_Pointer_0() { return static_cast<int32_t>(offsetof(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E, ___Pointer_0)); }
	inline intptr_t get_Pointer_0() const { return ___Pointer_0; }
	inline intptr_t* get_address_of_Pointer_0() { return &___Pointer_0; }
	inline void set_Pointer_0(intptr_t value)
	{
		___Pointer_0 = value;
	}

	inline static int32_t get_offset_of_Items_1() { return static_cast<int32_t>(offsetof(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E, ___Items_1)); }
	inline int32_t get_Items_1() const { return ___Items_1; }
	inline int32_t* get_address_of_Items_1() { return &___Items_1; }
	inline void set_Items_1(int32_t value)
	{
		___Items_1 = value;
	}

	inline static int32_t get_offset_of_Allocator_2() { return static_cast<int32_t>(offsetof(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E, ___Allocator_2)); }
	inline SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  get_Allocator_2() const { return ___Allocator_2; }
	inline SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 * get_address_of_Allocator_2() { return &___Allocator_2; }
	inline void set_Allocator_2(SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  value)
	{
		___Allocator_2 = value;
	}

	inline static int32_t get_offset_of_Block_3() { return static_cast<int32_t>(offsetof(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E, ___Block_3)); }
	inline BlockHandle_t06584EAE324E1124DEE24393CE3DDA434C9F1717  get_Block_3() const { return ___Block_3; }
	inline BlockHandle_t06584EAE324E1124DEE24393CE3DDA434C9F1717 * get_address_of_Block_3() { return &___Block_3; }
	inline void set_Block_3(BlockHandle_t06584EAE324E1124DEE24393CE3DDA434C9F1717  value)
	{
		___Block_3 = value;
	}
};


// Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall
struct Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB  : public RuntimeObject
{
public:

public:
};

struct Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields
{
public:
	// System.IntPtr Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Pointer
	intptr_t ___Pointer_0;
	// System.IntPtr Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::DeferredCompilation
	intptr_t ___DeferredCompilation_1;

public:
	inline static int32_t get_offset_of_Pointer_0() { return static_cast<int32_t>(offsetof(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields, ___Pointer_0)); }
	inline intptr_t get_Pointer_0() const { return ___Pointer_0; }
	inline intptr_t* get_address_of_Pointer_0() { return &___Pointer_0; }
	inline void set_Pointer_0(intptr_t value)
	{
		___Pointer_0 = value;
	}

	inline static int32_t get_offset_of_DeferredCompilation_1() { return static_cast<int32_t>(offsetof(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields, ___DeferredCompilation_1)); }
	inline intptr_t get_DeferredCompilation_1() const { return ___DeferredCompilation_1; }
	inline intptr_t* get_address_of_DeferredCompilation_1() { return &___DeferredCompilation_1; }
	inline void set_DeferredCompilation_1(intptr_t value)
	{
		___DeferredCompilation_1 = value;
	}
};


// Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall
struct Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0  : public RuntimeObject
{
public:

public:
};

struct Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields
{
public:
	// System.IntPtr Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Pointer
	intptr_t ___Pointer_0;
	// System.IntPtr Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::DeferredCompilation
	intptr_t ___DeferredCompilation_1;

public:
	inline static int32_t get_offset_of_Pointer_0() { return static_cast<int32_t>(offsetof(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields, ___Pointer_0)); }
	inline intptr_t get_Pointer_0() const { return ___Pointer_0; }
	inline intptr_t* get_address_of_Pointer_0() { return &___Pointer_0; }
	inline void set_Pointer_0(intptr_t value)
	{
		___Pointer_0 = value;
	}

	inline static int32_t get_offset_of_DeferredCompilation_1() { return static_cast<int32_t>(offsetof(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields, ___DeferredCompilation_1)); }
	inline intptr_t get_DeferredCompilation_1() const { return ___DeferredCompilation_1; }
	inline intptr_t* get_address_of_DeferredCompilation_1() { return &___DeferredCompilation_1; }
	inline void set_DeferredCompilation_1(intptr_t value)
	{
		___DeferredCompilation_1 = value;
	}
};


// Unity.Collections.AllocatorManager/StaticFunctionTable
struct StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5  : public RuntimeObject
{
public:

public:
};

struct StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_StaticFields
{
public:
	// Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/TableEntry65536> Unity.Collections.AllocatorManager/StaticFunctionTable::Ref
	SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  ___Ref_0;

public:
	inline static int32_t get_offset_of_Ref_0() { return static_cast<int32_t>(offsetof(StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_StaticFields, ___Ref_0)); }
	inline SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  get_Ref_0() const { return ___Ref_0; }
	inline SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F * get_address_of_Ref_0() { return &___Ref_0; }
	inline void set_Ref_0(SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  value)
	{
		___Ref_0 = value;
	}
};


// Unity.Collections.AllocatorManager/TableEntry
struct TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC 
{
public:
	// System.IntPtr Unity.Collections.AllocatorManager/TableEntry::function
	intptr_t ___function_0;
	// System.IntPtr Unity.Collections.AllocatorManager/TableEntry::state
	intptr_t ___state_1;

public:
	inline static int32_t get_offset_of_function_0() { return static_cast<int32_t>(offsetof(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC, ___function_0)); }
	inline intptr_t get_function_0() const { return ___function_0; }
	inline intptr_t* get_address_of_function_0() { return &___function_0; }
	inline void set_function_0(intptr_t value)
	{
		___function_0 = value;
	}

	inline static int32_t get_offset_of_state_1() { return static_cast<int32_t>(offsetof(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC, ___state_1)); }
	inline intptr_t get_state_1() const { return ___state_1; }
	inline intptr_t* get_address_of_state_1() { return &___state_1; }
	inline void set_state_1(intptr_t value)
	{
		___state_1 = value;
	}
};


// Unity.Collections.FixedBytes126
struct FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0016_1_OffsetPadding[16];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0016
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0016_1_OffsetPadding_forAlignmentOnly[16];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0032_2_OffsetPadding[32];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0032
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0032_2_OffsetPadding_forAlignmentOnly[32];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0048_3_OffsetPadding[48];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0048
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0048_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0048_3_OffsetPadding_forAlignmentOnly[48];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0048_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0064_4_OffsetPadding[64];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0064
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0064_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0064_4_OffsetPadding_forAlignmentOnly[64];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0064_4_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0080_5_OffsetPadding[80];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0080
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0080_5;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0080_5_OffsetPadding_forAlignmentOnly[80];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0080_5_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0096_6_OffsetPadding[96];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes126::offset0096
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0096_6;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0096_6_OffsetPadding_forAlignmentOnly[96];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0096_6_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0112_7_OffsetPadding[112];
					// System.Byte Unity.Collections.FixedBytes126::byte0112
					uint8_t ___byte0112_7;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0112_7_OffsetPadding_forAlignmentOnly[112];
					uint8_t ___byte0112_7_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0113_8_OffsetPadding[113];
					// System.Byte Unity.Collections.FixedBytes126::byte0113
					uint8_t ___byte0113_8;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0113_8_OffsetPadding_forAlignmentOnly[113];
					uint8_t ___byte0113_8_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0114_9_OffsetPadding[114];
					// System.Byte Unity.Collections.FixedBytes126::byte0114
					uint8_t ___byte0114_9;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0114_9_OffsetPadding_forAlignmentOnly[114];
					uint8_t ___byte0114_9_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0115_10_OffsetPadding[115];
					// System.Byte Unity.Collections.FixedBytes126::byte0115
					uint8_t ___byte0115_10;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0115_10_OffsetPadding_forAlignmentOnly[115];
					uint8_t ___byte0115_10_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0116_11_OffsetPadding[116];
					// System.Byte Unity.Collections.FixedBytes126::byte0116
					uint8_t ___byte0116_11;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0116_11_OffsetPadding_forAlignmentOnly[116];
					uint8_t ___byte0116_11_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0117_12_OffsetPadding[117];
					// System.Byte Unity.Collections.FixedBytes126::byte0117
					uint8_t ___byte0117_12;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0117_12_OffsetPadding_forAlignmentOnly[117];
					uint8_t ___byte0117_12_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0118_13_OffsetPadding[118];
					// System.Byte Unity.Collections.FixedBytes126::byte0118
					uint8_t ___byte0118_13;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0118_13_OffsetPadding_forAlignmentOnly[118];
					uint8_t ___byte0118_13_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0119_14_OffsetPadding[119];
					// System.Byte Unity.Collections.FixedBytes126::byte0119
					uint8_t ___byte0119_14;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0119_14_OffsetPadding_forAlignmentOnly[119];
					uint8_t ___byte0119_14_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0120_15_OffsetPadding[120];
					// System.Byte Unity.Collections.FixedBytes126::byte0120
					uint8_t ___byte0120_15;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0120_15_OffsetPadding_forAlignmentOnly[120];
					uint8_t ___byte0120_15_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0121_16_OffsetPadding[121];
					// System.Byte Unity.Collections.FixedBytes126::byte0121
					uint8_t ___byte0121_16;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0121_16_OffsetPadding_forAlignmentOnly[121];
					uint8_t ___byte0121_16_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0122_17_OffsetPadding[122];
					// System.Byte Unity.Collections.FixedBytes126::byte0122
					uint8_t ___byte0122_17;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0122_17_OffsetPadding_forAlignmentOnly[122];
					uint8_t ___byte0122_17_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0123_18_OffsetPadding[123];
					// System.Byte Unity.Collections.FixedBytes126::byte0123
					uint8_t ___byte0123_18;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0123_18_OffsetPadding_forAlignmentOnly[123];
					uint8_t ___byte0123_18_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0124_19_OffsetPadding[124];
					// System.Byte Unity.Collections.FixedBytes126::byte0124
					uint8_t ___byte0124_19;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0124_19_OffsetPadding_forAlignmentOnly[124];
					uint8_t ___byte0124_19_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0125_20_OffsetPadding[125];
					// System.Byte Unity.Collections.FixedBytes126::byte0125
					uint8_t ___byte0125_20;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0125_20_OffsetPadding_forAlignmentOnly[125];
					uint8_t ___byte0125_20_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1__padding[126];
	};

public:
	inline static int32_t get_offset_of_offset0000_0() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0000_0)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0000_0() const { return ___offset0000_0; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0000_0() { return &___offset0000_0; }
	inline void set_offset0000_0(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0000_0 = value;
	}

	inline static int32_t get_offset_of_offset0016_1() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0016_1)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0016_1() const { return ___offset0016_1; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0016_1() { return &___offset0016_1; }
	inline void set_offset0016_1(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0016_1 = value;
	}

	inline static int32_t get_offset_of_offset0032_2() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0032_2)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0032_2() const { return ___offset0032_2; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0032_2() { return &___offset0032_2; }
	inline void set_offset0032_2(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0032_2 = value;
	}

	inline static int32_t get_offset_of_offset0048_3() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0048_3)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0048_3() const { return ___offset0048_3; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0048_3() { return &___offset0048_3; }
	inline void set_offset0048_3(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0048_3 = value;
	}

	inline static int32_t get_offset_of_offset0064_4() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0064_4)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0064_4() const { return ___offset0064_4; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0064_4() { return &___offset0064_4; }
	inline void set_offset0064_4(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0064_4 = value;
	}

	inline static int32_t get_offset_of_offset0080_5() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0080_5)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0080_5() const { return ___offset0080_5; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0080_5() { return &___offset0080_5; }
	inline void set_offset0080_5(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0080_5 = value;
	}

	inline static int32_t get_offset_of_offset0096_6() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___offset0096_6)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0096_6() const { return ___offset0096_6; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0096_6() { return &___offset0096_6; }
	inline void set_offset0096_6(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0096_6 = value;
	}

	inline static int32_t get_offset_of_byte0112_7() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0112_7)); }
	inline uint8_t get_byte0112_7() const { return ___byte0112_7; }
	inline uint8_t* get_address_of_byte0112_7() { return &___byte0112_7; }
	inline void set_byte0112_7(uint8_t value)
	{
		___byte0112_7 = value;
	}

	inline static int32_t get_offset_of_byte0113_8() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0113_8)); }
	inline uint8_t get_byte0113_8() const { return ___byte0113_8; }
	inline uint8_t* get_address_of_byte0113_8() { return &___byte0113_8; }
	inline void set_byte0113_8(uint8_t value)
	{
		___byte0113_8 = value;
	}

	inline static int32_t get_offset_of_byte0114_9() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0114_9)); }
	inline uint8_t get_byte0114_9() const { return ___byte0114_9; }
	inline uint8_t* get_address_of_byte0114_9() { return &___byte0114_9; }
	inline void set_byte0114_9(uint8_t value)
	{
		___byte0114_9 = value;
	}

	inline static int32_t get_offset_of_byte0115_10() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0115_10)); }
	inline uint8_t get_byte0115_10() const { return ___byte0115_10; }
	inline uint8_t* get_address_of_byte0115_10() { return &___byte0115_10; }
	inline void set_byte0115_10(uint8_t value)
	{
		___byte0115_10 = value;
	}

	inline static int32_t get_offset_of_byte0116_11() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0116_11)); }
	inline uint8_t get_byte0116_11() const { return ___byte0116_11; }
	inline uint8_t* get_address_of_byte0116_11() { return &___byte0116_11; }
	inline void set_byte0116_11(uint8_t value)
	{
		___byte0116_11 = value;
	}

	inline static int32_t get_offset_of_byte0117_12() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0117_12)); }
	inline uint8_t get_byte0117_12() const { return ___byte0117_12; }
	inline uint8_t* get_address_of_byte0117_12() { return &___byte0117_12; }
	inline void set_byte0117_12(uint8_t value)
	{
		___byte0117_12 = value;
	}

	inline static int32_t get_offset_of_byte0118_13() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0118_13)); }
	inline uint8_t get_byte0118_13() const { return ___byte0118_13; }
	inline uint8_t* get_address_of_byte0118_13() { return &___byte0118_13; }
	inline void set_byte0118_13(uint8_t value)
	{
		___byte0118_13 = value;
	}

	inline static int32_t get_offset_of_byte0119_14() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0119_14)); }
	inline uint8_t get_byte0119_14() const { return ___byte0119_14; }
	inline uint8_t* get_address_of_byte0119_14() { return &___byte0119_14; }
	inline void set_byte0119_14(uint8_t value)
	{
		___byte0119_14 = value;
	}

	inline static int32_t get_offset_of_byte0120_15() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0120_15)); }
	inline uint8_t get_byte0120_15() const { return ___byte0120_15; }
	inline uint8_t* get_address_of_byte0120_15() { return &___byte0120_15; }
	inline void set_byte0120_15(uint8_t value)
	{
		___byte0120_15 = value;
	}

	inline static int32_t get_offset_of_byte0121_16() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0121_16)); }
	inline uint8_t get_byte0121_16() const { return ___byte0121_16; }
	inline uint8_t* get_address_of_byte0121_16() { return &___byte0121_16; }
	inline void set_byte0121_16(uint8_t value)
	{
		___byte0121_16 = value;
	}

	inline static int32_t get_offset_of_byte0122_17() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0122_17)); }
	inline uint8_t get_byte0122_17() const { return ___byte0122_17; }
	inline uint8_t* get_address_of_byte0122_17() { return &___byte0122_17; }
	inline void set_byte0122_17(uint8_t value)
	{
		___byte0122_17 = value;
	}

	inline static int32_t get_offset_of_byte0123_18() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0123_18)); }
	inline uint8_t get_byte0123_18() const { return ___byte0123_18; }
	inline uint8_t* get_address_of_byte0123_18() { return &___byte0123_18; }
	inline void set_byte0123_18(uint8_t value)
	{
		___byte0123_18 = value;
	}

	inline static int32_t get_offset_of_byte0124_19() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0124_19)); }
	inline uint8_t get_byte0124_19() const { return ___byte0124_19; }
	inline uint8_t* get_address_of_byte0124_19() { return &___byte0124_19; }
	inline void set_byte0124_19(uint8_t value)
	{
		___byte0124_19 = value;
	}

	inline static int32_t get_offset_of_byte0125_20() { return static_cast<int32_t>(offsetof(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1, ___byte0125_20)); }
	inline uint8_t get_byte0125_20() const { return ___byte0125_20; }
	inline uint8_t* get_address_of_byte0125_20() { return &___byte0125_20; }
	inline void set_byte0125_20(uint8_t value)
	{
		___byte0125_20 = value;
	}
};


// Unity.Collections.FixedBytes30
struct FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes30::offset0000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0016_1_OffsetPadding[16];
					// System.Byte Unity.Collections.FixedBytes30::byte0016
					uint8_t ___byte0016_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0016_1_OffsetPadding_forAlignmentOnly[16];
					uint8_t ___byte0016_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0017_2_OffsetPadding[17];
					// System.Byte Unity.Collections.FixedBytes30::byte0017
					uint8_t ___byte0017_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0017_2_OffsetPadding_forAlignmentOnly[17];
					uint8_t ___byte0017_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0018_3_OffsetPadding[18];
					// System.Byte Unity.Collections.FixedBytes30::byte0018
					uint8_t ___byte0018_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0018_3_OffsetPadding_forAlignmentOnly[18];
					uint8_t ___byte0018_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0019_4_OffsetPadding[19];
					// System.Byte Unity.Collections.FixedBytes30::byte0019
					uint8_t ___byte0019_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0019_4_OffsetPadding_forAlignmentOnly[19];
					uint8_t ___byte0019_4_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0020_5_OffsetPadding[20];
					// System.Byte Unity.Collections.FixedBytes30::byte0020
					uint8_t ___byte0020_5;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0020_5_OffsetPadding_forAlignmentOnly[20];
					uint8_t ___byte0020_5_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0021_6_OffsetPadding[21];
					// System.Byte Unity.Collections.FixedBytes30::byte0021
					uint8_t ___byte0021_6;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0021_6_OffsetPadding_forAlignmentOnly[21];
					uint8_t ___byte0021_6_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0022_7_OffsetPadding[22];
					// System.Byte Unity.Collections.FixedBytes30::byte0022
					uint8_t ___byte0022_7;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0022_7_OffsetPadding_forAlignmentOnly[22];
					uint8_t ___byte0022_7_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0023_8_OffsetPadding[23];
					// System.Byte Unity.Collections.FixedBytes30::byte0023
					uint8_t ___byte0023_8;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0023_8_OffsetPadding_forAlignmentOnly[23];
					uint8_t ___byte0023_8_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0024_9_OffsetPadding[24];
					// System.Byte Unity.Collections.FixedBytes30::byte0024
					uint8_t ___byte0024_9;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0024_9_OffsetPadding_forAlignmentOnly[24];
					uint8_t ___byte0024_9_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0025_10_OffsetPadding[25];
					// System.Byte Unity.Collections.FixedBytes30::byte0025
					uint8_t ___byte0025_10;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0025_10_OffsetPadding_forAlignmentOnly[25];
					uint8_t ___byte0025_10_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0026_11_OffsetPadding[26];
					// System.Byte Unity.Collections.FixedBytes30::byte0026
					uint8_t ___byte0026_11;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0026_11_OffsetPadding_forAlignmentOnly[26];
					uint8_t ___byte0026_11_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0027_12_OffsetPadding[27];
					// System.Byte Unity.Collections.FixedBytes30::byte0027
					uint8_t ___byte0027_12;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0027_12_OffsetPadding_forAlignmentOnly[27];
					uint8_t ___byte0027_12_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0028_13_OffsetPadding[28];
					// System.Byte Unity.Collections.FixedBytes30::byte0028
					uint8_t ___byte0028_13;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0028_13_OffsetPadding_forAlignmentOnly[28];
					uint8_t ___byte0028_13_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0029_14_OffsetPadding[29];
					// System.Byte Unity.Collections.FixedBytes30::byte0029
					uint8_t ___byte0029_14;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0029_14_OffsetPadding_forAlignmentOnly[29];
					uint8_t ___byte0029_14_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28__padding[30];
	};

public:
	inline static int32_t get_offset_of_offset0000_0() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___offset0000_0)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0000_0() const { return ___offset0000_0; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0000_0() { return &___offset0000_0; }
	inline void set_offset0000_0(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0000_0 = value;
	}

	inline static int32_t get_offset_of_byte0016_1() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0016_1)); }
	inline uint8_t get_byte0016_1() const { return ___byte0016_1; }
	inline uint8_t* get_address_of_byte0016_1() { return &___byte0016_1; }
	inline void set_byte0016_1(uint8_t value)
	{
		___byte0016_1 = value;
	}

	inline static int32_t get_offset_of_byte0017_2() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0017_2)); }
	inline uint8_t get_byte0017_2() const { return ___byte0017_2; }
	inline uint8_t* get_address_of_byte0017_2() { return &___byte0017_2; }
	inline void set_byte0017_2(uint8_t value)
	{
		___byte0017_2 = value;
	}

	inline static int32_t get_offset_of_byte0018_3() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0018_3)); }
	inline uint8_t get_byte0018_3() const { return ___byte0018_3; }
	inline uint8_t* get_address_of_byte0018_3() { return &___byte0018_3; }
	inline void set_byte0018_3(uint8_t value)
	{
		___byte0018_3 = value;
	}

	inline static int32_t get_offset_of_byte0019_4() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0019_4)); }
	inline uint8_t get_byte0019_4() const { return ___byte0019_4; }
	inline uint8_t* get_address_of_byte0019_4() { return &___byte0019_4; }
	inline void set_byte0019_4(uint8_t value)
	{
		___byte0019_4 = value;
	}

	inline static int32_t get_offset_of_byte0020_5() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0020_5)); }
	inline uint8_t get_byte0020_5() const { return ___byte0020_5; }
	inline uint8_t* get_address_of_byte0020_5() { return &___byte0020_5; }
	inline void set_byte0020_5(uint8_t value)
	{
		___byte0020_5 = value;
	}

	inline static int32_t get_offset_of_byte0021_6() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0021_6)); }
	inline uint8_t get_byte0021_6() const { return ___byte0021_6; }
	inline uint8_t* get_address_of_byte0021_6() { return &___byte0021_6; }
	inline void set_byte0021_6(uint8_t value)
	{
		___byte0021_6 = value;
	}

	inline static int32_t get_offset_of_byte0022_7() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0022_7)); }
	inline uint8_t get_byte0022_7() const { return ___byte0022_7; }
	inline uint8_t* get_address_of_byte0022_7() { return &___byte0022_7; }
	inline void set_byte0022_7(uint8_t value)
	{
		___byte0022_7 = value;
	}

	inline static int32_t get_offset_of_byte0023_8() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0023_8)); }
	inline uint8_t get_byte0023_8() const { return ___byte0023_8; }
	inline uint8_t* get_address_of_byte0023_8() { return &___byte0023_8; }
	inline void set_byte0023_8(uint8_t value)
	{
		___byte0023_8 = value;
	}

	inline static int32_t get_offset_of_byte0024_9() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0024_9)); }
	inline uint8_t get_byte0024_9() const { return ___byte0024_9; }
	inline uint8_t* get_address_of_byte0024_9() { return &___byte0024_9; }
	inline void set_byte0024_9(uint8_t value)
	{
		___byte0024_9 = value;
	}

	inline static int32_t get_offset_of_byte0025_10() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0025_10)); }
	inline uint8_t get_byte0025_10() const { return ___byte0025_10; }
	inline uint8_t* get_address_of_byte0025_10() { return &___byte0025_10; }
	inline void set_byte0025_10(uint8_t value)
	{
		___byte0025_10 = value;
	}

	inline static int32_t get_offset_of_byte0026_11() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0026_11)); }
	inline uint8_t get_byte0026_11() const { return ___byte0026_11; }
	inline uint8_t* get_address_of_byte0026_11() { return &___byte0026_11; }
	inline void set_byte0026_11(uint8_t value)
	{
		___byte0026_11 = value;
	}

	inline static int32_t get_offset_of_byte0027_12() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0027_12)); }
	inline uint8_t get_byte0027_12() const { return ___byte0027_12; }
	inline uint8_t* get_address_of_byte0027_12() { return &___byte0027_12; }
	inline void set_byte0027_12(uint8_t value)
	{
		___byte0027_12 = value;
	}

	inline static int32_t get_offset_of_byte0028_13() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0028_13)); }
	inline uint8_t get_byte0028_13() const { return ___byte0028_13; }
	inline uint8_t* get_address_of_byte0028_13() { return &___byte0028_13; }
	inline void set_byte0028_13(uint8_t value)
	{
		___byte0028_13 = value;
	}

	inline static int32_t get_offset_of_byte0029_14() { return static_cast<int32_t>(offsetof(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28, ___byte0029_14)); }
	inline uint8_t get_byte0029_14() const { return ___byte0029_14; }
	inline uint8_t* get_address_of_byte0029_14() { return &___byte0029_14; }
	inline void set_byte0029_14(uint8_t value)
	{
		___byte0029_14 = value;
	}
};


// Unity.Collections.FixedBytes4094
struct FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0016_1_OffsetPadding[16];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0016
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0016_1_OffsetPadding_forAlignmentOnly[16];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0032_2_OffsetPadding[32];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0032
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0032_2_OffsetPadding_forAlignmentOnly[32];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0048_3_OffsetPadding[48];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0048
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0048_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0048_3_OffsetPadding_forAlignmentOnly[48];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0048_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0064_4_OffsetPadding[64];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0064
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0064_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0064_4_OffsetPadding_forAlignmentOnly[64];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0064_4_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0080_5_OffsetPadding[80];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0080
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0080_5;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0080_5_OffsetPadding_forAlignmentOnly[80];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0080_5_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0096_6_OffsetPadding[96];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0096
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0096_6;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0096_6_OffsetPadding_forAlignmentOnly[96];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0096_6_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0112_7_OffsetPadding[112];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0112
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0112_7;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0112_7_OffsetPadding_forAlignmentOnly[112];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0112_7_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0128_8_OffsetPadding[128];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0128
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0128_8;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0128_8_OffsetPadding_forAlignmentOnly[128];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0128_8_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0144_9_OffsetPadding[144];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0144
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0144_9;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0144_9_OffsetPadding_forAlignmentOnly[144];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0144_9_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0160_10_OffsetPadding[160];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0160
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0160_10;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0160_10_OffsetPadding_forAlignmentOnly[160];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0160_10_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0176_11_OffsetPadding[176];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0176
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0176_11;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0176_11_OffsetPadding_forAlignmentOnly[176];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0176_11_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0192_12_OffsetPadding[192];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0192
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0192_12;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0192_12_OffsetPadding_forAlignmentOnly[192];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0192_12_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0208_13_OffsetPadding[208];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0208
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0208_13;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0208_13_OffsetPadding_forAlignmentOnly[208];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0208_13_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0224_14_OffsetPadding[224];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0224
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0224_14;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0224_14_OffsetPadding_forAlignmentOnly[224];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0224_14_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0240_15_OffsetPadding[240];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0240
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0240_15;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0240_15_OffsetPadding_forAlignmentOnly[240];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0240_15_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0256_16_OffsetPadding[256];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0256
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0256_16;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0256_16_OffsetPadding_forAlignmentOnly[256];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0256_16_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0272_17_OffsetPadding[272];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0272
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0272_17;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0272_17_OffsetPadding_forAlignmentOnly[272];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0272_17_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0288_18_OffsetPadding[288];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0288
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0288_18;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0288_18_OffsetPadding_forAlignmentOnly[288];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0288_18_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0304_19_OffsetPadding[304];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0304
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0304_19;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0304_19_OffsetPadding_forAlignmentOnly[304];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0304_19_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0320_20_OffsetPadding[320];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0320
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0320_20;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0320_20_OffsetPadding_forAlignmentOnly[320];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0320_20_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0336_21_OffsetPadding[336];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0336
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0336_21;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0336_21_OffsetPadding_forAlignmentOnly[336];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0336_21_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0352_22_OffsetPadding[352];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0352
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0352_22;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0352_22_OffsetPadding_forAlignmentOnly[352];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0352_22_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0368_23_OffsetPadding[368];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0368
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0368_23;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0368_23_OffsetPadding_forAlignmentOnly[368];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0368_23_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0384_24_OffsetPadding[384];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0384
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0384_24;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0384_24_OffsetPadding_forAlignmentOnly[384];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0384_24_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0400_25_OffsetPadding[400];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0400
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0400_25;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0400_25_OffsetPadding_forAlignmentOnly[400];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0400_25_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0416_26_OffsetPadding[416];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0416
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0416_26;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0416_26_OffsetPadding_forAlignmentOnly[416];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0416_26_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0432_27_OffsetPadding[432];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0432
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0432_27;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0432_27_OffsetPadding_forAlignmentOnly[432];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0432_27_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0448_28_OffsetPadding[448];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0448
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0448_28;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0448_28_OffsetPadding_forAlignmentOnly[448];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0448_28_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0464_29_OffsetPadding[464];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0464
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0464_29;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0464_29_OffsetPadding_forAlignmentOnly[464];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0464_29_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0480_30_OffsetPadding[480];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0480
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0480_30;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0480_30_OffsetPadding_forAlignmentOnly[480];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0480_30_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0496_31_OffsetPadding[496];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0496
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0496_31;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0496_31_OffsetPadding_forAlignmentOnly[496];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0496_31_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0512_32_OffsetPadding[512];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0512
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0512_32;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0512_32_OffsetPadding_forAlignmentOnly[512];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0512_32_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0528_33_OffsetPadding[528];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0528
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0528_33;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0528_33_OffsetPadding_forAlignmentOnly[528];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0528_33_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0544_34_OffsetPadding[544];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0544
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0544_34;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0544_34_OffsetPadding_forAlignmentOnly[544];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0544_34_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0560_35_OffsetPadding[560];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0560
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0560_35;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0560_35_OffsetPadding_forAlignmentOnly[560];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0560_35_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0576_36_OffsetPadding[576];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0576
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0576_36;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0576_36_OffsetPadding_forAlignmentOnly[576];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0576_36_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0592_37_OffsetPadding[592];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0592
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0592_37;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0592_37_OffsetPadding_forAlignmentOnly[592];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0592_37_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0608_38_OffsetPadding[608];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0608
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0608_38;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0608_38_OffsetPadding_forAlignmentOnly[608];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0608_38_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0624_39_OffsetPadding[624];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0624
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0624_39;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0624_39_OffsetPadding_forAlignmentOnly[624];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0624_39_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0640_40_OffsetPadding[640];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0640
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0640_40;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0640_40_OffsetPadding_forAlignmentOnly[640];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0640_40_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0656_41_OffsetPadding[656];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0656
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0656_41;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0656_41_OffsetPadding_forAlignmentOnly[656];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0656_41_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0672_42_OffsetPadding[672];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0672
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0672_42;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0672_42_OffsetPadding_forAlignmentOnly[672];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0672_42_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0688_43_OffsetPadding[688];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0688
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0688_43;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0688_43_OffsetPadding_forAlignmentOnly[688];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0688_43_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0704_44_OffsetPadding[704];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0704
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0704_44;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0704_44_OffsetPadding_forAlignmentOnly[704];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0704_44_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0720_45_OffsetPadding[720];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0720
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0720_45;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0720_45_OffsetPadding_forAlignmentOnly[720];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0720_45_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0736_46_OffsetPadding[736];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0736
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0736_46;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0736_46_OffsetPadding_forAlignmentOnly[736];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0736_46_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0752_47_OffsetPadding[752];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0752
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0752_47;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0752_47_OffsetPadding_forAlignmentOnly[752];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0752_47_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0768_48_OffsetPadding[768];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0768
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0768_48;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0768_48_OffsetPadding_forAlignmentOnly[768];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0768_48_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0784_49_OffsetPadding[784];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0784
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0784_49;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0784_49_OffsetPadding_forAlignmentOnly[784];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0784_49_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0800_50_OffsetPadding[800];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0800
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0800_50;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0800_50_OffsetPadding_forAlignmentOnly[800];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0800_50_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0816_51_OffsetPadding[816];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0816
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0816_51;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0816_51_OffsetPadding_forAlignmentOnly[816];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0816_51_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0832_52_OffsetPadding[832];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0832
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0832_52;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0832_52_OffsetPadding_forAlignmentOnly[832];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0832_52_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0848_53_OffsetPadding[848];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0848
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0848_53;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0848_53_OffsetPadding_forAlignmentOnly[848];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0848_53_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0864_54_OffsetPadding[864];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0864
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0864_54;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0864_54_OffsetPadding_forAlignmentOnly[864];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0864_54_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0880_55_OffsetPadding[880];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0880
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0880_55;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0880_55_OffsetPadding_forAlignmentOnly[880];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0880_55_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0896_56_OffsetPadding[896];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0896
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0896_56;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0896_56_OffsetPadding_forAlignmentOnly[896];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0896_56_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0912_57_OffsetPadding[912];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0912
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0912_57;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0912_57_OffsetPadding_forAlignmentOnly[912];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0912_57_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0928_58_OffsetPadding[928];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0928
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0928_58;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0928_58_OffsetPadding_forAlignmentOnly[928];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0928_58_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0944_59_OffsetPadding[944];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0944
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0944_59;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0944_59_OffsetPadding_forAlignmentOnly[944];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0944_59_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0960_60_OffsetPadding[960];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0960
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0960_60;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0960_60_OffsetPadding_forAlignmentOnly[960];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0960_60_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0976_61_OffsetPadding[976];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0976
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0976_61;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0976_61_OffsetPadding_forAlignmentOnly[976];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0976_61_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0992_62_OffsetPadding[992];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset0992
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0992_62;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0992_62_OffsetPadding_forAlignmentOnly[992];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0992_62_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1008_63_OffsetPadding[1008];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1008
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1008_63;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1008_63_OffsetPadding_forAlignmentOnly[1008];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1008_63_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1024_64_OffsetPadding[1024];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1024
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1024_64;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1024_64_OffsetPadding_forAlignmentOnly[1024];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1024_64_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1040_65_OffsetPadding[1040];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1040
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1040_65;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1040_65_OffsetPadding_forAlignmentOnly[1040];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1040_65_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1056_66_OffsetPadding[1056];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1056
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1056_66;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1056_66_OffsetPadding_forAlignmentOnly[1056];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1056_66_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1072_67_OffsetPadding[1072];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1072
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1072_67;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1072_67_OffsetPadding_forAlignmentOnly[1072];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1072_67_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1088_68_OffsetPadding[1088];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1088
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1088_68;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1088_68_OffsetPadding_forAlignmentOnly[1088];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1088_68_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1104_69_OffsetPadding[1104];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1104
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1104_69;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1104_69_OffsetPadding_forAlignmentOnly[1104];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1104_69_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1120_70_OffsetPadding[1120];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1120
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1120_70;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1120_70_OffsetPadding_forAlignmentOnly[1120];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1120_70_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1136_71_OffsetPadding[1136];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1136
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1136_71;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1136_71_OffsetPadding_forAlignmentOnly[1136];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1136_71_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1152_72_OffsetPadding[1152];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1152
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1152_72;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1152_72_OffsetPadding_forAlignmentOnly[1152];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1152_72_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1168_73_OffsetPadding[1168];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1168
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1168_73;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1168_73_OffsetPadding_forAlignmentOnly[1168];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1168_73_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1184_74_OffsetPadding[1184];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1184
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1184_74;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1184_74_OffsetPadding_forAlignmentOnly[1184];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1184_74_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1200_75_OffsetPadding[1200];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1200
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1200_75;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1200_75_OffsetPadding_forAlignmentOnly[1200];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1200_75_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1216_76_OffsetPadding[1216];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1216
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1216_76;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1216_76_OffsetPadding_forAlignmentOnly[1216];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1216_76_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1232_77_OffsetPadding[1232];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1232
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1232_77;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1232_77_OffsetPadding_forAlignmentOnly[1232];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1232_77_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1248_78_OffsetPadding[1248];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1248
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1248_78;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1248_78_OffsetPadding_forAlignmentOnly[1248];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1248_78_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1264_79_OffsetPadding[1264];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1264
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1264_79;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1264_79_OffsetPadding_forAlignmentOnly[1264];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1264_79_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1280_80_OffsetPadding[1280];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1280
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1280_80;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1280_80_OffsetPadding_forAlignmentOnly[1280];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1280_80_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1296_81_OffsetPadding[1296];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1296
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1296_81;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1296_81_OffsetPadding_forAlignmentOnly[1296];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1296_81_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1312_82_OffsetPadding[1312];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1312
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1312_82;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1312_82_OffsetPadding_forAlignmentOnly[1312];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1312_82_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1328_83_OffsetPadding[1328];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1328
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1328_83;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1328_83_OffsetPadding_forAlignmentOnly[1328];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1328_83_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1344_84_OffsetPadding[1344];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1344
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1344_84;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1344_84_OffsetPadding_forAlignmentOnly[1344];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1344_84_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1360_85_OffsetPadding[1360];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1360
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1360_85;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1360_85_OffsetPadding_forAlignmentOnly[1360];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1360_85_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1376_86_OffsetPadding[1376];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1376
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1376_86;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1376_86_OffsetPadding_forAlignmentOnly[1376];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1376_86_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1392_87_OffsetPadding[1392];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1392
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1392_87;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1392_87_OffsetPadding_forAlignmentOnly[1392];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1392_87_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1408_88_OffsetPadding[1408];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1408
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1408_88;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1408_88_OffsetPadding_forAlignmentOnly[1408];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1408_88_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1424_89_OffsetPadding[1424];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1424
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1424_89;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1424_89_OffsetPadding_forAlignmentOnly[1424];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1424_89_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1440_90_OffsetPadding[1440];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1440
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1440_90;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1440_90_OffsetPadding_forAlignmentOnly[1440];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1440_90_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1456_91_OffsetPadding[1456];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1456
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1456_91;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1456_91_OffsetPadding_forAlignmentOnly[1456];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1456_91_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1472_92_OffsetPadding[1472];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1472
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1472_92;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1472_92_OffsetPadding_forAlignmentOnly[1472];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1472_92_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1488_93_OffsetPadding[1488];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1488
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1488_93;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1488_93_OffsetPadding_forAlignmentOnly[1488];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1488_93_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1504_94_OffsetPadding[1504];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1504
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1504_94;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1504_94_OffsetPadding_forAlignmentOnly[1504];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1504_94_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1520_95_OffsetPadding[1520];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1520
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1520_95;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1520_95_OffsetPadding_forAlignmentOnly[1520];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1520_95_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1536_96_OffsetPadding[1536];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1536
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1536_96;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1536_96_OffsetPadding_forAlignmentOnly[1536];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1536_96_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1552_97_OffsetPadding[1552];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1552
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1552_97;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1552_97_OffsetPadding_forAlignmentOnly[1552];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1552_97_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1568_98_OffsetPadding[1568];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1568
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1568_98;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1568_98_OffsetPadding_forAlignmentOnly[1568];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1568_98_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1584_99_OffsetPadding[1584];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1584
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1584_99;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1584_99_OffsetPadding_forAlignmentOnly[1584];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1584_99_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1600_100_OffsetPadding[1600];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1600
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1600_100;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1600_100_OffsetPadding_forAlignmentOnly[1600];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1600_100_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1616_101_OffsetPadding[1616];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1616
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1616_101;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1616_101_OffsetPadding_forAlignmentOnly[1616];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1616_101_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1632_102_OffsetPadding[1632];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1632
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1632_102;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1632_102_OffsetPadding_forAlignmentOnly[1632];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1632_102_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1648_103_OffsetPadding[1648];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1648
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1648_103;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1648_103_OffsetPadding_forAlignmentOnly[1648];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1648_103_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1664_104_OffsetPadding[1664];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1664
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1664_104;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1664_104_OffsetPadding_forAlignmentOnly[1664];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1664_104_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1680_105_OffsetPadding[1680];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1680
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1680_105;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1680_105_OffsetPadding_forAlignmentOnly[1680];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1680_105_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1696_106_OffsetPadding[1696];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1696
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1696_106;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1696_106_OffsetPadding_forAlignmentOnly[1696];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1696_106_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1712_107_OffsetPadding[1712];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1712
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1712_107;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1712_107_OffsetPadding_forAlignmentOnly[1712];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1712_107_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1728_108_OffsetPadding[1728];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1728
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1728_108;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1728_108_OffsetPadding_forAlignmentOnly[1728];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1728_108_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1744_109_OffsetPadding[1744];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1744
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1744_109;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1744_109_OffsetPadding_forAlignmentOnly[1744];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1744_109_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1760_110_OffsetPadding[1760];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1760
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1760_110;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1760_110_OffsetPadding_forAlignmentOnly[1760];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1760_110_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1776_111_OffsetPadding[1776];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1776
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1776_111;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1776_111_OffsetPadding_forAlignmentOnly[1776];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1776_111_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1792_112_OffsetPadding[1792];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1792
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1792_112;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1792_112_OffsetPadding_forAlignmentOnly[1792];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1792_112_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1808_113_OffsetPadding[1808];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1808
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1808_113;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1808_113_OffsetPadding_forAlignmentOnly[1808];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1808_113_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1824_114_OffsetPadding[1824];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1824
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1824_114;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1824_114_OffsetPadding_forAlignmentOnly[1824];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1824_114_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1840_115_OffsetPadding[1840];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1840
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1840_115;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1840_115_OffsetPadding_forAlignmentOnly[1840];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1840_115_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1856_116_OffsetPadding[1856];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1856
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1856_116;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1856_116_OffsetPadding_forAlignmentOnly[1856];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1856_116_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1872_117_OffsetPadding[1872];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1872
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1872_117;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1872_117_OffsetPadding_forAlignmentOnly[1872];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1872_117_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1888_118_OffsetPadding[1888];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1888
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1888_118;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1888_118_OffsetPadding_forAlignmentOnly[1888];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1888_118_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1904_119_OffsetPadding[1904];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1904
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1904_119;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1904_119_OffsetPadding_forAlignmentOnly[1904];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1904_119_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1920_120_OffsetPadding[1920];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1920
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1920_120;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1920_120_OffsetPadding_forAlignmentOnly[1920];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1920_120_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1936_121_OffsetPadding[1936];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1936
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1936_121;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1936_121_OffsetPadding_forAlignmentOnly[1936];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1936_121_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1952_122_OffsetPadding[1952];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1952
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1952_122;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1952_122_OffsetPadding_forAlignmentOnly[1952];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1952_122_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1968_123_OffsetPadding[1968];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1968
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1968_123;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1968_123_OffsetPadding_forAlignmentOnly[1968];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1968_123_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset1984_124_OffsetPadding[1984];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset1984
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1984_124;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset1984_124_OffsetPadding_forAlignmentOnly[1984];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset1984_124_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2000_125_OffsetPadding[2000];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2000_125;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2000_125_OffsetPadding_forAlignmentOnly[2000];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2000_125_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2016_126_OffsetPadding[2016];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2016
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2016_126;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2016_126_OffsetPadding_forAlignmentOnly[2016];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2016_126_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2032_127_OffsetPadding[2032];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2032
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2032_127;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2032_127_OffsetPadding_forAlignmentOnly[2032];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2032_127_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2048_128_OffsetPadding[2048];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2048
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2048_128;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2048_128_OffsetPadding_forAlignmentOnly[2048];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2048_128_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2064_129_OffsetPadding[2064];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2064
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2064_129;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2064_129_OffsetPadding_forAlignmentOnly[2064];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2064_129_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2080_130_OffsetPadding[2080];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2080
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2080_130;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2080_130_OffsetPadding_forAlignmentOnly[2080];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2080_130_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2096_131_OffsetPadding[2096];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2096
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2096_131;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2096_131_OffsetPadding_forAlignmentOnly[2096];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2096_131_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2112_132_OffsetPadding[2112];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2112
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2112_132;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2112_132_OffsetPadding_forAlignmentOnly[2112];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2112_132_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2128_133_OffsetPadding[2128];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2128
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2128_133;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2128_133_OffsetPadding_forAlignmentOnly[2128];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2128_133_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2144_134_OffsetPadding[2144];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2144
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2144_134;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2144_134_OffsetPadding_forAlignmentOnly[2144];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2144_134_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2160_135_OffsetPadding[2160];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2160
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2160_135;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2160_135_OffsetPadding_forAlignmentOnly[2160];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2160_135_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2176_136_OffsetPadding[2176];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2176
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2176_136;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2176_136_OffsetPadding_forAlignmentOnly[2176];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2176_136_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2192_137_OffsetPadding[2192];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2192
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2192_137;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2192_137_OffsetPadding_forAlignmentOnly[2192];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2192_137_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2208_138_OffsetPadding[2208];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2208
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2208_138;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2208_138_OffsetPadding_forAlignmentOnly[2208];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2208_138_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2224_139_OffsetPadding[2224];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2224
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2224_139;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2224_139_OffsetPadding_forAlignmentOnly[2224];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2224_139_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2240_140_OffsetPadding[2240];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2240
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2240_140;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2240_140_OffsetPadding_forAlignmentOnly[2240];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2240_140_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2256_141_OffsetPadding[2256];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2256
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2256_141;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2256_141_OffsetPadding_forAlignmentOnly[2256];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2256_141_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2272_142_OffsetPadding[2272];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2272
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2272_142;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2272_142_OffsetPadding_forAlignmentOnly[2272];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2272_142_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2288_143_OffsetPadding[2288];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2288
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2288_143;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2288_143_OffsetPadding_forAlignmentOnly[2288];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2288_143_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2304_144_OffsetPadding[2304];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2304
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2304_144;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2304_144_OffsetPadding_forAlignmentOnly[2304];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2304_144_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2320_145_OffsetPadding[2320];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2320
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2320_145;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2320_145_OffsetPadding_forAlignmentOnly[2320];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2320_145_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2336_146_OffsetPadding[2336];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2336
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2336_146;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2336_146_OffsetPadding_forAlignmentOnly[2336];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2336_146_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2352_147_OffsetPadding[2352];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2352
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2352_147;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2352_147_OffsetPadding_forAlignmentOnly[2352];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2352_147_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2368_148_OffsetPadding[2368];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2368
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2368_148;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2368_148_OffsetPadding_forAlignmentOnly[2368];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2368_148_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2384_149_OffsetPadding[2384];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2384
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2384_149;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2384_149_OffsetPadding_forAlignmentOnly[2384];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2384_149_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2400_150_OffsetPadding[2400];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2400
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2400_150;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2400_150_OffsetPadding_forAlignmentOnly[2400];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2400_150_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2416_151_OffsetPadding[2416];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2416
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2416_151;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2416_151_OffsetPadding_forAlignmentOnly[2416];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2416_151_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2432_152_OffsetPadding[2432];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2432
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2432_152;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2432_152_OffsetPadding_forAlignmentOnly[2432];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2432_152_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2448_153_OffsetPadding[2448];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2448
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2448_153;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2448_153_OffsetPadding_forAlignmentOnly[2448];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2448_153_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2464_154_OffsetPadding[2464];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2464
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2464_154;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2464_154_OffsetPadding_forAlignmentOnly[2464];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2464_154_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2480_155_OffsetPadding[2480];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2480
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2480_155;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2480_155_OffsetPadding_forAlignmentOnly[2480];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2480_155_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2496_156_OffsetPadding[2496];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2496
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2496_156;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2496_156_OffsetPadding_forAlignmentOnly[2496];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2496_156_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2512_157_OffsetPadding[2512];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2512
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2512_157;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2512_157_OffsetPadding_forAlignmentOnly[2512];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2512_157_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2528_158_OffsetPadding[2528];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2528
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2528_158;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2528_158_OffsetPadding_forAlignmentOnly[2528];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2528_158_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2544_159_OffsetPadding[2544];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2544
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2544_159;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2544_159_OffsetPadding_forAlignmentOnly[2544];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2544_159_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2560_160_OffsetPadding[2560];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2560
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2560_160;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2560_160_OffsetPadding_forAlignmentOnly[2560];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2560_160_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2576_161_OffsetPadding[2576];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2576
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2576_161;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2576_161_OffsetPadding_forAlignmentOnly[2576];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2576_161_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2592_162_OffsetPadding[2592];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2592
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2592_162;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2592_162_OffsetPadding_forAlignmentOnly[2592];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2592_162_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2608_163_OffsetPadding[2608];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2608
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2608_163;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2608_163_OffsetPadding_forAlignmentOnly[2608];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2608_163_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2624_164_OffsetPadding[2624];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2624
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2624_164;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2624_164_OffsetPadding_forAlignmentOnly[2624];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2624_164_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2640_165_OffsetPadding[2640];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2640
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2640_165;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2640_165_OffsetPadding_forAlignmentOnly[2640];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2640_165_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2656_166_OffsetPadding[2656];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2656
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2656_166;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2656_166_OffsetPadding_forAlignmentOnly[2656];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2656_166_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2672_167_OffsetPadding[2672];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2672
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2672_167;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2672_167_OffsetPadding_forAlignmentOnly[2672];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2672_167_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2688_168_OffsetPadding[2688];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2688
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2688_168;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2688_168_OffsetPadding_forAlignmentOnly[2688];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2688_168_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2704_169_OffsetPadding[2704];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2704
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2704_169;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2704_169_OffsetPadding_forAlignmentOnly[2704];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2704_169_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2720_170_OffsetPadding[2720];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2720
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2720_170;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2720_170_OffsetPadding_forAlignmentOnly[2720];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2720_170_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2736_171_OffsetPadding[2736];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2736
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2736_171;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2736_171_OffsetPadding_forAlignmentOnly[2736];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2736_171_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2752_172_OffsetPadding[2752];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2752
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2752_172;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2752_172_OffsetPadding_forAlignmentOnly[2752];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2752_172_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2768_173_OffsetPadding[2768];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2768
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2768_173;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2768_173_OffsetPadding_forAlignmentOnly[2768];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2768_173_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2784_174_OffsetPadding[2784];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2784
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2784_174;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2784_174_OffsetPadding_forAlignmentOnly[2784];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2784_174_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2800_175_OffsetPadding[2800];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2800
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2800_175;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2800_175_OffsetPadding_forAlignmentOnly[2800];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2800_175_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2816_176_OffsetPadding[2816];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2816
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2816_176;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2816_176_OffsetPadding_forAlignmentOnly[2816];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2816_176_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2832_177_OffsetPadding[2832];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2832
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2832_177;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2832_177_OffsetPadding_forAlignmentOnly[2832];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2832_177_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2848_178_OffsetPadding[2848];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2848
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2848_178;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2848_178_OffsetPadding_forAlignmentOnly[2848];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2848_178_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2864_179_OffsetPadding[2864];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2864
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2864_179;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2864_179_OffsetPadding_forAlignmentOnly[2864];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2864_179_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2880_180_OffsetPadding[2880];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2880
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2880_180;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2880_180_OffsetPadding_forAlignmentOnly[2880];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2880_180_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2896_181_OffsetPadding[2896];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2896
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2896_181;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2896_181_OffsetPadding_forAlignmentOnly[2896];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2896_181_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2912_182_OffsetPadding[2912];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2912
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2912_182;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2912_182_OffsetPadding_forAlignmentOnly[2912];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2912_182_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2928_183_OffsetPadding[2928];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2928
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2928_183;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2928_183_OffsetPadding_forAlignmentOnly[2928];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2928_183_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2944_184_OffsetPadding[2944];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2944
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2944_184;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2944_184_OffsetPadding_forAlignmentOnly[2944];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2944_184_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2960_185_OffsetPadding[2960];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2960
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2960_185;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2960_185_OffsetPadding_forAlignmentOnly[2960];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2960_185_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2976_186_OffsetPadding[2976];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2976
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2976_186;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2976_186_OffsetPadding_forAlignmentOnly[2976];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2976_186_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset2992_187_OffsetPadding[2992];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset2992
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2992_187;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset2992_187_OffsetPadding_forAlignmentOnly[2992];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset2992_187_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3008_188_OffsetPadding[3008];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3008
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3008_188;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3008_188_OffsetPadding_forAlignmentOnly[3008];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3008_188_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3024_189_OffsetPadding[3024];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3024
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3024_189;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3024_189_OffsetPadding_forAlignmentOnly[3024];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3024_189_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3040_190_OffsetPadding[3040];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3040
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3040_190;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3040_190_OffsetPadding_forAlignmentOnly[3040];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3040_190_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3056_191_OffsetPadding[3056];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3056
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3056_191;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3056_191_OffsetPadding_forAlignmentOnly[3056];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3056_191_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3072_192_OffsetPadding[3072];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3072
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3072_192;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3072_192_OffsetPadding_forAlignmentOnly[3072];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3072_192_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3088_193_OffsetPadding[3088];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3088
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3088_193;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3088_193_OffsetPadding_forAlignmentOnly[3088];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3088_193_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3104_194_OffsetPadding[3104];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3104
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3104_194;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3104_194_OffsetPadding_forAlignmentOnly[3104];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3104_194_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3120_195_OffsetPadding[3120];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3120
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3120_195;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3120_195_OffsetPadding_forAlignmentOnly[3120];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3120_195_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3136_196_OffsetPadding[3136];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3136
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3136_196;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3136_196_OffsetPadding_forAlignmentOnly[3136];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3136_196_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3152_197_OffsetPadding[3152];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3152
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3152_197;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3152_197_OffsetPadding_forAlignmentOnly[3152];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3152_197_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3168_198_OffsetPadding[3168];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3168
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3168_198;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3168_198_OffsetPadding_forAlignmentOnly[3168];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3168_198_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3184_199_OffsetPadding[3184];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3184
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3184_199;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3184_199_OffsetPadding_forAlignmentOnly[3184];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3184_199_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3200_200_OffsetPadding[3200];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3200
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3200_200;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3200_200_OffsetPadding_forAlignmentOnly[3200];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3200_200_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3216_201_OffsetPadding[3216];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3216
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3216_201;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3216_201_OffsetPadding_forAlignmentOnly[3216];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3216_201_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3232_202_OffsetPadding[3232];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3232
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3232_202;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3232_202_OffsetPadding_forAlignmentOnly[3232];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3232_202_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3248_203_OffsetPadding[3248];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3248
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3248_203;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3248_203_OffsetPadding_forAlignmentOnly[3248];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3248_203_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3264_204_OffsetPadding[3264];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3264
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3264_204;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3264_204_OffsetPadding_forAlignmentOnly[3264];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3264_204_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3280_205_OffsetPadding[3280];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3280
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3280_205;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3280_205_OffsetPadding_forAlignmentOnly[3280];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3280_205_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3296_206_OffsetPadding[3296];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3296
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3296_206;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3296_206_OffsetPadding_forAlignmentOnly[3296];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3296_206_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3312_207_OffsetPadding[3312];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3312
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3312_207;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3312_207_OffsetPadding_forAlignmentOnly[3312];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3312_207_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3328_208_OffsetPadding[3328];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3328
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3328_208;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3328_208_OffsetPadding_forAlignmentOnly[3328];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3328_208_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3344_209_OffsetPadding[3344];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3344
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3344_209;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3344_209_OffsetPadding_forAlignmentOnly[3344];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3344_209_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3360_210_OffsetPadding[3360];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3360
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3360_210;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3360_210_OffsetPadding_forAlignmentOnly[3360];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3360_210_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3376_211_OffsetPadding[3376];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3376
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3376_211;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3376_211_OffsetPadding_forAlignmentOnly[3376];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3376_211_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3392_212_OffsetPadding[3392];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3392
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3392_212;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3392_212_OffsetPadding_forAlignmentOnly[3392];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3392_212_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3408_213_OffsetPadding[3408];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3408
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3408_213;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3408_213_OffsetPadding_forAlignmentOnly[3408];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3408_213_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3424_214_OffsetPadding[3424];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3424
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3424_214;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3424_214_OffsetPadding_forAlignmentOnly[3424];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3424_214_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3440_215_OffsetPadding[3440];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3440
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3440_215;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3440_215_OffsetPadding_forAlignmentOnly[3440];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3440_215_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3456_216_OffsetPadding[3456];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3456
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3456_216;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3456_216_OffsetPadding_forAlignmentOnly[3456];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3456_216_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3472_217_OffsetPadding[3472];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3472
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3472_217;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3472_217_OffsetPadding_forAlignmentOnly[3472];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3472_217_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3488_218_OffsetPadding[3488];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3488
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3488_218;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3488_218_OffsetPadding_forAlignmentOnly[3488];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3488_218_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3504_219_OffsetPadding[3504];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3504
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3504_219;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3504_219_OffsetPadding_forAlignmentOnly[3504];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3504_219_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3520_220_OffsetPadding[3520];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3520
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3520_220;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3520_220_OffsetPadding_forAlignmentOnly[3520];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3520_220_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3536_221_OffsetPadding[3536];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3536
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3536_221;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3536_221_OffsetPadding_forAlignmentOnly[3536];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3536_221_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3552_222_OffsetPadding[3552];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3552
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3552_222;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3552_222_OffsetPadding_forAlignmentOnly[3552];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3552_222_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3568_223_OffsetPadding[3568];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3568
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3568_223;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3568_223_OffsetPadding_forAlignmentOnly[3568];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3568_223_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3584_224_OffsetPadding[3584];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3584
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3584_224;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3584_224_OffsetPadding_forAlignmentOnly[3584];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3584_224_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3600_225_OffsetPadding[3600];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3600
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3600_225;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3600_225_OffsetPadding_forAlignmentOnly[3600];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3600_225_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3616_226_OffsetPadding[3616];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3616
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3616_226;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3616_226_OffsetPadding_forAlignmentOnly[3616];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3616_226_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3632_227_OffsetPadding[3632];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3632
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3632_227;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3632_227_OffsetPadding_forAlignmentOnly[3632];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3632_227_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3648_228_OffsetPadding[3648];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3648
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3648_228;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3648_228_OffsetPadding_forAlignmentOnly[3648];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3648_228_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3664_229_OffsetPadding[3664];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3664
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3664_229;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3664_229_OffsetPadding_forAlignmentOnly[3664];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3664_229_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3680_230_OffsetPadding[3680];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3680
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3680_230;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3680_230_OffsetPadding_forAlignmentOnly[3680];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3680_230_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3696_231_OffsetPadding[3696];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3696
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3696_231;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3696_231_OffsetPadding_forAlignmentOnly[3696];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3696_231_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3712_232_OffsetPadding[3712];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3712
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3712_232;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3712_232_OffsetPadding_forAlignmentOnly[3712];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3712_232_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3728_233_OffsetPadding[3728];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3728
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3728_233;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3728_233_OffsetPadding_forAlignmentOnly[3728];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3728_233_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3744_234_OffsetPadding[3744];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3744
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3744_234;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3744_234_OffsetPadding_forAlignmentOnly[3744];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3744_234_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3760_235_OffsetPadding[3760];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3760
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3760_235;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3760_235_OffsetPadding_forAlignmentOnly[3760];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3760_235_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3776_236_OffsetPadding[3776];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3776
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3776_236;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3776_236_OffsetPadding_forAlignmentOnly[3776];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3776_236_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3792_237_OffsetPadding[3792];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3792
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3792_237;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3792_237_OffsetPadding_forAlignmentOnly[3792];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3792_237_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3808_238_OffsetPadding[3808];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3808
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3808_238;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3808_238_OffsetPadding_forAlignmentOnly[3808];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3808_238_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3824_239_OffsetPadding[3824];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3824
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3824_239;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3824_239_OffsetPadding_forAlignmentOnly[3824];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3824_239_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3840_240_OffsetPadding[3840];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3840
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3840_240;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3840_240_OffsetPadding_forAlignmentOnly[3840];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3840_240_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3856_241_OffsetPadding[3856];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3856
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3856_241;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3856_241_OffsetPadding_forAlignmentOnly[3856];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3856_241_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3872_242_OffsetPadding[3872];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3872
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3872_242;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3872_242_OffsetPadding_forAlignmentOnly[3872];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3872_242_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3888_243_OffsetPadding[3888];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3888
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3888_243;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3888_243_OffsetPadding_forAlignmentOnly[3888];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3888_243_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3904_244_OffsetPadding[3904];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3904
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3904_244;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3904_244_OffsetPadding_forAlignmentOnly[3904];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3904_244_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3920_245_OffsetPadding[3920];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3920
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3920_245;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3920_245_OffsetPadding_forAlignmentOnly[3920];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3920_245_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3936_246_OffsetPadding[3936];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3936
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3936_246;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3936_246_OffsetPadding_forAlignmentOnly[3936];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3936_246_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3952_247_OffsetPadding[3952];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3952
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3952_247;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3952_247_OffsetPadding_forAlignmentOnly[3952];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3952_247_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3968_248_OffsetPadding[3968];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3968
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3968_248;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3968_248_OffsetPadding_forAlignmentOnly[3968];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3968_248_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset3984_249_OffsetPadding[3984];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset3984
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3984_249;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset3984_249_OffsetPadding_forAlignmentOnly[3984];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset3984_249_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset4000_250_OffsetPadding[4000];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset4000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4000_250;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset4000_250_OffsetPadding_forAlignmentOnly[4000];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4000_250_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset4016_251_OffsetPadding[4016];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset4016
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4016_251;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset4016_251_OffsetPadding_forAlignmentOnly[4016];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4016_251_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset4032_252_OffsetPadding[4032];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset4032
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4032_252;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset4032_252_OffsetPadding_forAlignmentOnly[4032];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4032_252_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset4048_253_OffsetPadding[4048];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset4048
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4048_253;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset4048_253_OffsetPadding_forAlignmentOnly[4048];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4048_253_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset4064_254_OffsetPadding[4064];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes4094::offset4064
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4064_254;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset4064_254_OffsetPadding_forAlignmentOnly[4064];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset4064_254_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4080_255_OffsetPadding[4080];
					// System.Byte Unity.Collections.FixedBytes4094::byte4080
					uint8_t ___byte4080_255;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4080_255_OffsetPadding_forAlignmentOnly[4080];
					uint8_t ___byte4080_255_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4081_256_OffsetPadding[4081];
					// System.Byte Unity.Collections.FixedBytes4094::byte4081
					uint8_t ___byte4081_256;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4081_256_OffsetPadding_forAlignmentOnly[4081];
					uint8_t ___byte4081_256_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4082_257_OffsetPadding[4082];
					// System.Byte Unity.Collections.FixedBytes4094::byte4082
					uint8_t ___byte4082_257;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4082_257_OffsetPadding_forAlignmentOnly[4082];
					uint8_t ___byte4082_257_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4083_258_OffsetPadding[4083];
					// System.Byte Unity.Collections.FixedBytes4094::byte4083
					uint8_t ___byte4083_258;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4083_258_OffsetPadding_forAlignmentOnly[4083];
					uint8_t ___byte4083_258_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4084_259_OffsetPadding[4084];
					// System.Byte Unity.Collections.FixedBytes4094::byte4084
					uint8_t ___byte4084_259;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4084_259_OffsetPadding_forAlignmentOnly[4084];
					uint8_t ___byte4084_259_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4085_260_OffsetPadding[4085];
					// System.Byte Unity.Collections.FixedBytes4094::byte4085
					uint8_t ___byte4085_260;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4085_260_OffsetPadding_forAlignmentOnly[4085];
					uint8_t ___byte4085_260_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4086_261_OffsetPadding[4086];
					// System.Byte Unity.Collections.FixedBytes4094::byte4086
					uint8_t ___byte4086_261;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4086_261_OffsetPadding_forAlignmentOnly[4086];
					uint8_t ___byte4086_261_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4087_262_OffsetPadding[4087];
					// System.Byte Unity.Collections.FixedBytes4094::byte4087
					uint8_t ___byte4087_262;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4087_262_OffsetPadding_forAlignmentOnly[4087];
					uint8_t ___byte4087_262_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4088_263_OffsetPadding[4088];
					// System.Byte Unity.Collections.FixedBytes4094::byte4088
					uint8_t ___byte4088_263;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4088_263_OffsetPadding_forAlignmentOnly[4088];
					uint8_t ___byte4088_263_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4089_264_OffsetPadding[4089];
					// System.Byte Unity.Collections.FixedBytes4094::byte4089
					uint8_t ___byte4089_264;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4089_264_OffsetPadding_forAlignmentOnly[4089];
					uint8_t ___byte4089_264_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4090_265_OffsetPadding[4090];
					// System.Byte Unity.Collections.FixedBytes4094::byte4090
					uint8_t ___byte4090_265;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4090_265_OffsetPadding_forAlignmentOnly[4090];
					uint8_t ___byte4090_265_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4091_266_OffsetPadding[4091];
					// System.Byte Unity.Collections.FixedBytes4094::byte4091
					uint8_t ___byte4091_266;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4091_266_OffsetPadding_forAlignmentOnly[4091];
					uint8_t ___byte4091_266_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4092_267_OffsetPadding[4092];
					// System.Byte Unity.Collections.FixedBytes4094::byte4092
					uint8_t ___byte4092_267;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4092_267_OffsetPadding_forAlignmentOnly[4092];
					uint8_t ___byte4092_267_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte4093_268_OffsetPadding[4093];
					// System.Byte Unity.Collections.FixedBytes4094::byte4093
					uint8_t ___byte4093_268;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte4093_268_OffsetPadding_forAlignmentOnly[4093];
					uint8_t ___byte4093_268_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70__padding[4094];
	};

public:
	inline static int32_t get_offset_of_offset0000_0() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0000_0)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0000_0() const { return ___offset0000_0; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0000_0() { return &___offset0000_0; }
	inline void set_offset0000_0(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0000_0 = value;
	}

	inline static int32_t get_offset_of_offset0016_1() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0016_1)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0016_1() const { return ___offset0016_1; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0016_1() { return &___offset0016_1; }
	inline void set_offset0016_1(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0016_1 = value;
	}

	inline static int32_t get_offset_of_offset0032_2() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0032_2)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0032_2() const { return ___offset0032_2; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0032_2() { return &___offset0032_2; }
	inline void set_offset0032_2(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0032_2 = value;
	}

	inline static int32_t get_offset_of_offset0048_3() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0048_3)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0048_3() const { return ___offset0048_3; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0048_3() { return &___offset0048_3; }
	inline void set_offset0048_3(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0048_3 = value;
	}

	inline static int32_t get_offset_of_offset0064_4() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0064_4)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0064_4() const { return ___offset0064_4; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0064_4() { return &___offset0064_4; }
	inline void set_offset0064_4(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0064_4 = value;
	}

	inline static int32_t get_offset_of_offset0080_5() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0080_5)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0080_5() const { return ___offset0080_5; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0080_5() { return &___offset0080_5; }
	inline void set_offset0080_5(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0080_5 = value;
	}

	inline static int32_t get_offset_of_offset0096_6() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0096_6)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0096_6() const { return ___offset0096_6; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0096_6() { return &___offset0096_6; }
	inline void set_offset0096_6(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0096_6 = value;
	}

	inline static int32_t get_offset_of_offset0112_7() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0112_7)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0112_7() const { return ___offset0112_7; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0112_7() { return &___offset0112_7; }
	inline void set_offset0112_7(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0112_7 = value;
	}

	inline static int32_t get_offset_of_offset0128_8() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0128_8)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0128_8() const { return ___offset0128_8; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0128_8() { return &___offset0128_8; }
	inline void set_offset0128_8(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0128_8 = value;
	}

	inline static int32_t get_offset_of_offset0144_9() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0144_9)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0144_9() const { return ___offset0144_9; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0144_9() { return &___offset0144_9; }
	inline void set_offset0144_9(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0144_9 = value;
	}

	inline static int32_t get_offset_of_offset0160_10() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0160_10)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0160_10() const { return ___offset0160_10; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0160_10() { return &___offset0160_10; }
	inline void set_offset0160_10(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0160_10 = value;
	}

	inline static int32_t get_offset_of_offset0176_11() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0176_11)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0176_11() const { return ___offset0176_11; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0176_11() { return &___offset0176_11; }
	inline void set_offset0176_11(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0176_11 = value;
	}

	inline static int32_t get_offset_of_offset0192_12() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0192_12)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0192_12() const { return ___offset0192_12; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0192_12() { return &___offset0192_12; }
	inline void set_offset0192_12(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0192_12 = value;
	}

	inline static int32_t get_offset_of_offset0208_13() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0208_13)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0208_13() const { return ___offset0208_13; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0208_13() { return &___offset0208_13; }
	inline void set_offset0208_13(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0208_13 = value;
	}

	inline static int32_t get_offset_of_offset0224_14() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0224_14)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0224_14() const { return ___offset0224_14; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0224_14() { return &___offset0224_14; }
	inline void set_offset0224_14(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0224_14 = value;
	}

	inline static int32_t get_offset_of_offset0240_15() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0240_15)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0240_15() const { return ___offset0240_15; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0240_15() { return &___offset0240_15; }
	inline void set_offset0240_15(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0240_15 = value;
	}

	inline static int32_t get_offset_of_offset0256_16() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0256_16)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0256_16() const { return ___offset0256_16; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0256_16() { return &___offset0256_16; }
	inline void set_offset0256_16(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0256_16 = value;
	}

	inline static int32_t get_offset_of_offset0272_17() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0272_17)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0272_17() const { return ___offset0272_17; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0272_17() { return &___offset0272_17; }
	inline void set_offset0272_17(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0272_17 = value;
	}

	inline static int32_t get_offset_of_offset0288_18() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0288_18)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0288_18() const { return ___offset0288_18; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0288_18() { return &___offset0288_18; }
	inline void set_offset0288_18(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0288_18 = value;
	}

	inline static int32_t get_offset_of_offset0304_19() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0304_19)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0304_19() const { return ___offset0304_19; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0304_19() { return &___offset0304_19; }
	inline void set_offset0304_19(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0304_19 = value;
	}

	inline static int32_t get_offset_of_offset0320_20() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0320_20)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0320_20() const { return ___offset0320_20; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0320_20() { return &___offset0320_20; }
	inline void set_offset0320_20(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0320_20 = value;
	}

	inline static int32_t get_offset_of_offset0336_21() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0336_21)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0336_21() const { return ___offset0336_21; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0336_21() { return &___offset0336_21; }
	inline void set_offset0336_21(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0336_21 = value;
	}

	inline static int32_t get_offset_of_offset0352_22() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0352_22)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0352_22() const { return ___offset0352_22; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0352_22() { return &___offset0352_22; }
	inline void set_offset0352_22(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0352_22 = value;
	}

	inline static int32_t get_offset_of_offset0368_23() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0368_23)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0368_23() const { return ___offset0368_23; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0368_23() { return &___offset0368_23; }
	inline void set_offset0368_23(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0368_23 = value;
	}

	inline static int32_t get_offset_of_offset0384_24() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0384_24)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0384_24() const { return ___offset0384_24; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0384_24() { return &___offset0384_24; }
	inline void set_offset0384_24(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0384_24 = value;
	}

	inline static int32_t get_offset_of_offset0400_25() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0400_25)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0400_25() const { return ___offset0400_25; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0400_25() { return &___offset0400_25; }
	inline void set_offset0400_25(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0400_25 = value;
	}

	inline static int32_t get_offset_of_offset0416_26() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0416_26)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0416_26() const { return ___offset0416_26; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0416_26() { return &___offset0416_26; }
	inline void set_offset0416_26(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0416_26 = value;
	}

	inline static int32_t get_offset_of_offset0432_27() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0432_27)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0432_27() const { return ___offset0432_27; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0432_27() { return &___offset0432_27; }
	inline void set_offset0432_27(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0432_27 = value;
	}

	inline static int32_t get_offset_of_offset0448_28() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0448_28)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0448_28() const { return ___offset0448_28; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0448_28() { return &___offset0448_28; }
	inline void set_offset0448_28(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0448_28 = value;
	}

	inline static int32_t get_offset_of_offset0464_29() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0464_29)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0464_29() const { return ___offset0464_29; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0464_29() { return &___offset0464_29; }
	inline void set_offset0464_29(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0464_29 = value;
	}

	inline static int32_t get_offset_of_offset0480_30() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0480_30)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0480_30() const { return ___offset0480_30; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0480_30() { return &___offset0480_30; }
	inline void set_offset0480_30(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0480_30 = value;
	}

	inline static int32_t get_offset_of_offset0496_31() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0496_31)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0496_31() const { return ___offset0496_31; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0496_31() { return &___offset0496_31; }
	inline void set_offset0496_31(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0496_31 = value;
	}

	inline static int32_t get_offset_of_offset0512_32() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0512_32)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0512_32() const { return ___offset0512_32; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0512_32() { return &___offset0512_32; }
	inline void set_offset0512_32(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0512_32 = value;
	}

	inline static int32_t get_offset_of_offset0528_33() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0528_33)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0528_33() const { return ___offset0528_33; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0528_33() { return &___offset0528_33; }
	inline void set_offset0528_33(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0528_33 = value;
	}

	inline static int32_t get_offset_of_offset0544_34() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0544_34)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0544_34() const { return ___offset0544_34; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0544_34() { return &___offset0544_34; }
	inline void set_offset0544_34(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0544_34 = value;
	}

	inline static int32_t get_offset_of_offset0560_35() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0560_35)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0560_35() const { return ___offset0560_35; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0560_35() { return &___offset0560_35; }
	inline void set_offset0560_35(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0560_35 = value;
	}

	inline static int32_t get_offset_of_offset0576_36() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0576_36)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0576_36() const { return ___offset0576_36; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0576_36() { return &___offset0576_36; }
	inline void set_offset0576_36(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0576_36 = value;
	}

	inline static int32_t get_offset_of_offset0592_37() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0592_37)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0592_37() const { return ___offset0592_37; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0592_37() { return &___offset0592_37; }
	inline void set_offset0592_37(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0592_37 = value;
	}

	inline static int32_t get_offset_of_offset0608_38() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0608_38)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0608_38() const { return ___offset0608_38; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0608_38() { return &___offset0608_38; }
	inline void set_offset0608_38(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0608_38 = value;
	}

	inline static int32_t get_offset_of_offset0624_39() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0624_39)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0624_39() const { return ___offset0624_39; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0624_39() { return &___offset0624_39; }
	inline void set_offset0624_39(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0624_39 = value;
	}

	inline static int32_t get_offset_of_offset0640_40() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0640_40)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0640_40() const { return ___offset0640_40; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0640_40() { return &___offset0640_40; }
	inline void set_offset0640_40(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0640_40 = value;
	}

	inline static int32_t get_offset_of_offset0656_41() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0656_41)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0656_41() const { return ___offset0656_41; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0656_41() { return &___offset0656_41; }
	inline void set_offset0656_41(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0656_41 = value;
	}

	inline static int32_t get_offset_of_offset0672_42() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0672_42)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0672_42() const { return ___offset0672_42; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0672_42() { return &___offset0672_42; }
	inline void set_offset0672_42(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0672_42 = value;
	}

	inline static int32_t get_offset_of_offset0688_43() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0688_43)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0688_43() const { return ___offset0688_43; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0688_43() { return &___offset0688_43; }
	inline void set_offset0688_43(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0688_43 = value;
	}

	inline static int32_t get_offset_of_offset0704_44() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0704_44)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0704_44() const { return ___offset0704_44; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0704_44() { return &___offset0704_44; }
	inline void set_offset0704_44(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0704_44 = value;
	}

	inline static int32_t get_offset_of_offset0720_45() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0720_45)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0720_45() const { return ___offset0720_45; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0720_45() { return &___offset0720_45; }
	inline void set_offset0720_45(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0720_45 = value;
	}

	inline static int32_t get_offset_of_offset0736_46() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0736_46)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0736_46() const { return ___offset0736_46; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0736_46() { return &___offset0736_46; }
	inline void set_offset0736_46(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0736_46 = value;
	}

	inline static int32_t get_offset_of_offset0752_47() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0752_47)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0752_47() const { return ___offset0752_47; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0752_47() { return &___offset0752_47; }
	inline void set_offset0752_47(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0752_47 = value;
	}

	inline static int32_t get_offset_of_offset0768_48() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0768_48)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0768_48() const { return ___offset0768_48; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0768_48() { return &___offset0768_48; }
	inline void set_offset0768_48(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0768_48 = value;
	}

	inline static int32_t get_offset_of_offset0784_49() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0784_49)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0784_49() const { return ___offset0784_49; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0784_49() { return &___offset0784_49; }
	inline void set_offset0784_49(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0784_49 = value;
	}

	inline static int32_t get_offset_of_offset0800_50() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0800_50)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0800_50() const { return ___offset0800_50; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0800_50() { return &___offset0800_50; }
	inline void set_offset0800_50(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0800_50 = value;
	}

	inline static int32_t get_offset_of_offset0816_51() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0816_51)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0816_51() const { return ___offset0816_51; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0816_51() { return &___offset0816_51; }
	inline void set_offset0816_51(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0816_51 = value;
	}

	inline static int32_t get_offset_of_offset0832_52() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0832_52)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0832_52() const { return ___offset0832_52; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0832_52() { return &___offset0832_52; }
	inline void set_offset0832_52(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0832_52 = value;
	}

	inline static int32_t get_offset_of_offset0848_53() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0848_53)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0848_53() const { return ___offset0848_53; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0848_53() { return &___offset0848_53; }
	inline void set_offset0848_53(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0848_53 = value;
	}

	inline static int32_t get_offset_of_offset0864_54() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0864_54)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0864_54() const { return ___offset0864_54; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0864_54() { return &___offset0864_54; }
	inline void set_offset0864_54(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0864_54 = value;
	}

	inline static int32_t get_offset_of_offset0880_55() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0880_55)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0880_55() const { return ___offset0880_55; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0880_55() { return &___offset0880_55; }
	inline void set_offset0880_55(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0880_55 = value;
	}

	inline static int32_t get_offset_of_offset0896_56() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0896_56)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0896_56() const { return ___offset0896_56; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0896_56() { return &___offset0896_56; }
	inline void set_offset0896_56(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0896_56 = value;
	}

	inline static int32_t get_offset_of_offset0912_57() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0912_57)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0912_57() const { return ___offset0912_57; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0912_57() { return &___offset0912_57; }
	inline void set_offset0912_57(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0912_57 = value;
	}

	inline static int32_t get_offset_of_offset0928_58() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0928_58)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0928_58() const { return ___offset0928_58; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0928_58() { return &___offset0928_58; }
	inline void set_offset0928_58(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0928_58 = value;
	}

	inline static int32_t get_offset_of_offset0944_59() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0944_59)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0944_59() const { return ___offset0944_59; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0944_59() { return &___offset0944_59; }
	inline void set_offset0944_59(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0944_59 = value;
	}

	inline static int32_t get_offset_of_offset0960_60() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0960_60)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0960_60() const { return ___offset0960_60; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0960_60() { return &___offset0960_60; }
	inline void set_offset0960_60(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0960_60 = value;
	}

	inline static int32_t get_offset_of_offset0976_61() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0976_61)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0976_61() const { return ___offset0976_61; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0976_61() { return &___offset0976_61; }
	inline void set_offset0976_61(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0976_61 = value;
	}

	inline static int32_t get_offset_of_offset0992_62() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset0992_62)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0992_62() const { return ___offset0992_62; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0992_62() { return &___offset0992_62; }
	inline void set_offset0992_62(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0992_62 = value;
	}

	inline static int32_t get_offset_of_offset1008_63() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1008_63)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1008_63() const { return ___offset1008_63; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1008_63() { return &___offset1008_63; }
	inline void set_offset1008_63(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1008_63 = value;
	}

	inline static int32_t get_offset_of_offset1024_64() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1024_64)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1024_64() const { return ___offset1024_64; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1024_64() { return &___offset1024_64; }
	inline void set_offset1024_64(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1024_64 = value;
	}

	inline static int32_t get_offset_of_offset1040_65() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1040_65)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1040_65() const { return ___offset1040_65; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1040_65() { return &___offset1040_65; }
	inline void set_offset1040_65(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1040_65 = value;
	}

	inline static int32_t get_offset_of_offset1056_66() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1056_66)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1056_66() const { return ___offset1056_66; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1056_66() { return &___offset1056_66; }
	inline void set_offset1056_66(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1056_66 = value;
	}

	inline static int32_t get_offset_of_offset1072_67() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1072_67)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1072_67() const { return ___offset1072_67; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1072_67() { return &___offset1072_67; }
	inline void set_offset1072_67(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1072_67 = value;
	}

	inline static int32_t get_offset_of_offset1088_68() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1088_68)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1088_68() const { return ___offset1088_68; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1088_68() { return &___offset1088_68; }
	inline void set_offset1088_68(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1088_68 = value;
	}

	inline static int32_t get_offset_of_offset1104_69() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1104_69)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1104_69() const { return ___offset1104_69; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1104_69() { return &___offset1104_69; }
	inline void set_offset1104_69(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1104_69 = value;
	}

	inline static int32_t get_offset_of_offset1120_70() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1120_70)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1120_70() const { return ___offset1120_70; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1120_70() { return &___offset1120_70; }
	inline void set_offset1120_70(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1120_70 = value;
	}

	inline static int32_t get_offset_of_offset1136_71() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1136_71)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1136_71() const { return ___offset1136_71; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1136_71() { return &___offset1136_71; }
	inline void set_offset1136_71(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1136_71 = value;
	}

	inline static int32_t get_offset_of_offset1152_72() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1152_72)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1152_72() const { return ___offset1152_72; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1152_72() { return &___offset1152_72; }
	inline void set_offset1152_72(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1152_72 = value;
	}

	inline static int32_t get_offset_of_offset1168_73() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1168_73)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1168_73() const { return ___offset1168_73; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1168_73() { return &___offset1168_73; }
	inline void set_offset1168_73(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1168_73 = value;
	}

	inline static int32_t get_offset_of_offset1184_74() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1184_74)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1184_74() const { return ___offset1184_74; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1184_74() { return &___offset1184_74; }
	inline void set_offset1184_74(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1184_74 = value;
	}

	inline static int32_t get_offset_of_offset1200_75() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1200_75)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1200_75() const { return ___offset1200_75; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1200_75() { return &___offset1200_75; }
	inline void set_offset1200_75(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1200_75 = value;
	}

	inline static int32_t get_offset_of_offset1216_76() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1216_76)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1216_76() const { return ___offset1216_76; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1216_76() { return &___offset1216_76; }
	inline void set_offset1216_76(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1216_76 = value;
	}

	inline static int32_t get_offset_of_offset1232_77() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1232_77)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1232_77() const { return ___offset1232_77; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1232_77() { return &___offset1232_77; }
	inline void set_offset1232_77(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1232_77 = value;
	}

	inline static int32_t get_offset_of_offset1248_78() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1248_78)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1248_78() const { return ___offset1248_78; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1248_78() { return &___offset1248_78; }
	inline void set_offset1248_78(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1248_78 = value;
	}

	inline static int32_t get_offset_of_offset1264_79() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1264_79)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1264_79() const { return ___offset1264_79; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1264_79() { return &___offset1264_79; }
	inline void set_offset1264_79(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1264_79 = value;
	}

	inline static int32_t get_offset_of_offset1280_80() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1280_80)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1280_80() const { return ___offset1280_80; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1280_80() { return &___offset1280_80; }
	inline void set_offset1280_80(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1280_80 = value;
	}

	inline static int32_t get_offset_of_offset1296_81() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1296_81)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1296_81() const { return ___offset1296_81; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1296_81() { return &___offset1296_81; }
	inline void set_offset1296_81(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1296_81 = value;
	}

	inline static int32_t get_offset_of_offset1312_82() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1312_82)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1312_82() const { return ___offset1312_82; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1312_82() { return &___offset1312_82; }
	inline void set_offset1312_82(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1312_82 = value;
	}

	inline static int32_t get_offset_of_offset1328_83() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1328_83)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1328_83() const { return ___offset1328_83; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1328_83() { return &___offset1328_83; }
	inline void set_offset1328_83(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1328_83 = value;
	}

	inline static int32_t get_offset_of_offset1344_84() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1344_84)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1344_84() const { return ___offset1344_84; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1344_84() { return &___offset1344_84; }
	inline void set_offset1344_84(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1344_84 = value;
	}

	inline static int32_t get_offset_of_offset1360_85() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1360_85)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1360_85() const { return ___offset1360_85; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1360_85() { return &___offset1360_85; }
	inline void set_offset1360_85(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1360_85 = value;
	}

	inline static int32_t get_offset_of_offset1376_86() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1376_86)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1376_86() const { return ___offset1376_86; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1376_86() { return &___offset1376_86; }
	inline void set_offset1376_86(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1376_86 = value;
	}

	inline static int32_t get_offset_of_offset1392_87() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1392_87)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1392_87() const { return ___offset1392_87; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1392_87() { return &___offset1392_87; }
	inline void set_offset1392_87(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1392_87 = value;
	}

	inline static int32_t get_offset_of_offset1408_88() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1408_88)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1408_88() const { return ___offset1408_88; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1408_88() { return &___offset1408_88; }
	inline void set_offset1408_88(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1408_88 = value;
	}

	inline static int32_t get_offset_of_offset1424_89() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1424_89)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1424_89() const { return ___offset1424_89; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1424_89() { return &___offset1424_89; }
	inline void set_offset1424_89(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1424_89 = value;
	}

	inline static int32_t get_offset_of_offset1440_90() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1440_90)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1440_90() const { return ___offset1440_90; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1440_90() { return &___offset1440_90; }
	inline void set_offset1440_90(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1440_90 = value;
	}

	inline static int32_t get_offset_of_offset1456_91() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1456_91)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1456_91() const { return ___offset1456_91; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1456_91() { return &___offset1456_91; }
	inline void set_offset1456_91(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1456_91 = value;
	}

	inline static int32_t get_offset_of_offset1472_92() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1472_92)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1472_92() const { return ___offset1472_92; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1472_92() { return &___offset1472_92; }
	inline void set_offset1472_92(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1472_92 = value;
	}

	inline static int32_t get_offset_of_offset1488_93() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1488_93)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1488_93() const { return ___offset1488_93; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1488_93() { return &___offset1488_93; }
	inline void set_offset1488_93(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1488_93 = value;
	}

	inline static int32_t get_offset_of_offset1504_94() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1504_94)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1504_94() const { return ___offset1504_94; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1504_94() { return &___offset1504_94; }
	inline void set_offset1504_94(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1504_94 = value;
	}

	inline static int32_t get_offset_of_offset1520_95() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1520_95)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1520_95() const { return ___offset1520_95; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1520_95() { return &___offset1520_95; }
	inline void set_offset1520_95(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1520_95 = value;
	}

	inline static int32_t get_offset_of_offset1536_96() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1536_96)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1536_96() const { return ___offset1536_96; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1536_96() { return &___offset1536_96; }
	inline void set_offset1536_96(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1536_96 = value;
	}

	inline static int32_t get_offset_of_offset1552_97() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1552_97)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1552_97() const { return ___offset1552_97; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1552_97() { return &___offset1552_97; }
	inline void set_offset1552_97(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1552_97 = value;
	}

	inline static int32_t get_offset_of_offset1568_98() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1568_98)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1568_98() const { return ___offset1568_98; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1568_98() { return &___offset1568_98; }
	inline void set_offset1568_98(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1568_98 = value;
	}

	inline static int32_t get_offset_of_offset1584_99() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1584_99)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1584_99() const { return ___offset1584_99; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1584_99() { return &___offset1584_99; }
	inline void set_offset1584_99(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1584_99 = value;
	}

	inline static int32_t get_offset_of_offset1600_100() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1600_100)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1600_100() const { return ___offset1600_100; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1600_100() { return &___offset1600_100; }
	inline void set_offset1600_100(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1600_100 = value;
	}

	inline static int32_t get_offset_of_offset1616_101() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1616_101)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1616_101() const { return ___offset1616_101; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1616_101() { return &___offset1616_101; }
	inline void set_offset1616_101(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1616_101 = value;
	}

	inline static int32_t get_offset_of_offset1632_102() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1632_102)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1632_102() const { return ___offset1632_102; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1632_102() { return &___offset1632_102; }
	inline void set_offset1632_102(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1632_102 = value;
	}

	inline static int32_t get_offset_of_offset1648_103() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1648_103)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1648_103() const { return ___offset1648_103; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1648_103() { return &___offset1648_103; }
	inline void set_offset1648_103(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1648_103 = value;
	}

	inline static int32_t get_offset_of_offset1664_104() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1664_104)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1664_104() const { return ___offset1664_104; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1664_104() { return &___offset1664_104; }
	inline void set_offset1664_104(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1664_104 = value;
	}

	inline static int32_t get_offset_of_offset1680_105() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1680_105)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1680_105() const { return ___offset1680_105; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1680_105() { return &___offset1680_105; }
	inline void set_offset1680_105(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1680_105 = value;
	}

	inline static int32_t get_offset_of_offset1696_106() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1696_106)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1696_106() const { return ___offset1696_106; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1696_106() { return &___offset1696_106; }
	inline void set_offset1696_106(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1696_106 = value;
	}

	inline static int32_t get_offset_of_offset1712_107() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1712_107)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1712_107() const { return ___offset1712_107; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1712_107() { return &___offset1712_107; }
	inline void set_offset1712_107(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1712_107 = value;
	}

	inline static int32_t get_offset_of_offset1728_108() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1728_108)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1728_108() const { return ___offset1728_108; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1728_108() { return &___offset1728_108; }
	inline void set_offset1728_108(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1728_108 = value;
	}

	inline static int32_t get_offset_of_offset1744_109() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1744_109)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1744_109() const { return ___offset1744_109; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1744_109() { return &___offset1744_109; }
	inline void set_offset1744_109(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1744_109 = value;
	}

	inline static int32_t get_offset_of_offset1760_110() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1760_110)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1760_110() const { return ___offset1760_110; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1760_110() { return &___offset1760_110; }
	inline void set_offset1760_110(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1760_110 = value;
	}

	inline static int32_t get_offset_of_offset1776_111() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1776_111)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1776_111() const { return ___offset1776_111; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1776_111() { return &___offset1776_111; }
	inline void set_offset1776_111(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1776_111 = value;
	}

	inline static int32_t get_offset_of_offset1792_112() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1792_112)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1792_112() const { return ___offset1792_112; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1792_112() { return &___offset1792_112; }
	inline void set_offset1792_112(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1792_112 = value;
	}

	inline static int32_t get_offset_of_offset1808_113() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1808_113)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1808_113() const { return ___offset1808_113; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1808_113() { return &___offset1808_113; }
	inline void set_offset1808_113(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1808_113 = value;
	}

	inline static int32_t get_offset_of_offset1824_114() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1824_114)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1824_114() const { return ___offset1824_114; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1824_114() { return &___offset1824_114; }
	inline void set_offset1824_114(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1824_114 = value;
	}

	inline static int32_t get_offset_of_offset1840_115() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1840_115)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1840_115() const { return ___offset1840_115; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1840_115() { return &___offset1840_115; }
	inline void set_offset1840_115(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1840_115 = value;
	}

	inline static int32_t get_offset_of_offset1856_116() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1856_116)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1856_116() const { return ___offset1856_116; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1856_116() { return &___offset1856_116; }
	inline void set_offset1856_116(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1856_116 = value;
	}

	inline static int32_t get_offset_of_offset1872_117() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1872_117)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1872_117() const { return ___offset1872_117; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1872_117() { return &___offset1872_117; }
	inline void set_offset1872_117(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1872_117 = value;
	}

	inline static int32_t get_offset_of_offset1888_118() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1888_118)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1888_118() const { return ___offset1888_118; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1888_118() { return &___offset1888_118; }
	inline void set_offset1888_118(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1888_118 = value;
	}

	inline static int32_t get_offset_of_offset1904_119() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1904_119)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1904_119() const { return ___offset1904_119; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1904_119() { return &___offset1904_119; }
	inline void set_offset1904_119(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1904_119 = value;
	}

	inline static int32_t get_offset_of_offset1920_120() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1920_120)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1920_120() const { return ___offset1920_120; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1920_120() { return &___offset1920_120; }
	inline void set_offset1920_120(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1920_120 = value;
	}

	inline static int32_t get_offset_of_offset1936_121() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1936_121)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1936_121() const { return ___offset1936_121; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1936_121() { return &___offset1936_121; }
	inline void set_offset1936_121(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1936_121 = value;
	}

	inline static int32_t get_offset_of_offset1952_122() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1952_122)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1952_122() const { return ___offset1952_122; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1952_122() { return &___offset1952_122; }
	inline void set_offset1952_122(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1952_122 = value;
	}

	inline static int32_t get_offset_of_offset1968_123() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1968_123)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1968_123() const { return ___offset1968_123; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1968_123() { return &___offset1968_123; }
	inline void set_offset1968_123(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1968_123 = value;
	}

	inline static int32_t get_offset_of_offset1984_124() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset1984_124)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset1984_124() const { return ___offset1984_124; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset1984_124() { return &___offset1984_124; }
	inline void set_offset1984_124(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset1984_124 = value;
	}

	inline static int32_t get_offset_of_offset2000_125() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2000_125)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2000_125() const { return ___offset2000_125; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2000_125() { return &___offset2000_125; }
	inline void set_offset2000_125(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2000_125 = value;
	}

	inline static int32_t get_offset_of_offset2016_126() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2016_126)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2016_126() const { return ___offset2016_126; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2016_126() { return &___offset2016_126; }
	inline void set_offset2016_126(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2016_126 = value;
	}

	inline static int32_t get_offset_of_offset2032_127() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2032_127)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2032_127() const { return ___offset2032_127; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2032_127() { return &___offset2032_127; }
	inline void set_offset2032_127(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2032_127 = value;
	}

	inline static int32_t get_offset_of_offset2048_128() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2048_128)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2048_128() const { return ___offset2048_128; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2048_128() { return &___offset2048_128; }
	inline void set_offset2048_128(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2048_128 = value;
	}

	inline static int32_t get_offset_of_offset2064_129() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2064_129)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2064_129() const { return ___offset2064_129; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2064_129() { return &___offset2064_129; }
	inline void set_offset2064_129(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2064_129 = value;
	}

	inline static int32_t get_offset_of_offset2080_130() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2080_130)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2080_130() const { return ___offset2080_130; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2080_130() { return &___offset2080_130; }
	inline void set_offset2080_130(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2080_130 = value;
	}

	inline static int32_t get_offset_of_offset2096_131() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2096_131)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2096_131() const { return ___offset2096_131; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2096_131() { return &___offset2096_131; }
	inline void set_offset2096_131(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2096_131 = value;
	}

	inline static int32_t get_offset_of_offset2112_132() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2112_132)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2112_132() const { return ___offset2112_132; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2112_132() { return &___offset2112_132; }
	inline void set_offset2112_132(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2112_132 = value;
	}

	inline static int32_t get_offset_of_offset2128_133() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2128_133)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2128_133() const { return ___offset2128_133; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2128_133() { return &___offset2128_133; }
	inline void set_offset2128_133(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2128_133 = value;
	}

	inline static int32_t get_offset_of_offset2144_134() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2144_134)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2144_134() const { return ___offset2144_134; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2144_134() { return &___offset2144_134; }
	inline void set_offset2144_134(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2144_134 = value;
	}

	inline static int32_t get_offset_of_offset2160_135() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2160_135)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2160_135() const { return ___offset2160_135; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2160_135() { return &___offset2160_135; }
	inline void set_offset2160_135(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2160_135 = value;
	}

	inline static int32_t get_offset_of_offset2176_136() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2176_136)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2176_136() const { return ___offset2176_136; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2176_136() { return &___offset2176_136; }
	inline void set_offset2176_136(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2176_136 = value;
	}

	inline static int32_t get_offset_of_offset2192_137() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2192_137)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2192_137() const { return ___offset2192_137; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2192_137() { return &___offset2192_137; }
	inline void set_offset2192_137(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2192_137 = value;
	}

	inline static int32_t get_offset_of_offset2208_138() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2208_138)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2208_138() const { return ___offset2208_138; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2208_138() { return &___offset2208_138; }
	inline void set_offset2208_138(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2208_138 = value;
	}

	inline static int32_t get_offset_of_offset2224_139() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2224_139)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2224_139() const { return ___offset2224_139; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2224_139() { return &___offset2224_139; }
	inline void set_offset2224_139(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2224_139 = value;
	}

	inline static int32_t get_offset_of_offset2240_140() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2240_140)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2240_140() const { return ___offset2240_140; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2240_140() { return &___offset2240_140; }
	inline void set_offset2240_140(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2240_140 = value;
	}

	inline static int32_t get_offset_of_offset2256_141() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2256_141)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2256_141() const { return ___offset2256_141; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2256_141() { return &___offset2256_141; }
	inline void set_offset2256_141(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2256_141 = value;
	}

	inline static int32_t get_offset_of_offset2272_142() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2272_142)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2272_142() const { return ___offset2272_142; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2272_142() { return &___offset2272_142; }
	inline void set_offset2272_142(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2272_142 = value;
	}

	inline static int32_t get_offset_of_offset2288_143() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2288_143)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2288_143() const { return ___offset2288_143; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2288_143() { return &___offset2288_143; }
	inline void set_offset2288_143(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2288_143 = value;
	}

	inline static int32_t get_offset_of_offset2304_144() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2304_144)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2304_144() const { return ___offset2304_144; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2304_144() { return &___offset2304_144; }
	inline void set_offset2304_144(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2304_144 = value;
	}

	inline static int32_t get_offset_of_offset2320_145() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2320_145)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2320_145() const { return ___offset2320_145; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2320_145() { return &___offset2320_145; }
	inline void set_offset2320_145(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2320_145 = value;
	}

	inline static int32_t get_offset_of_offset2336_146() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2336_146)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2336_146() const { return ___offset2336_146; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2336_146() { return &___offset2336_146; }
	inline void set_offset2336_146(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2336_146 = value;
	}

	inline static int32_t get_offset_of_offset2352_147() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2352_147)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2352_147() const { return ___offset2352_147; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2352_147() { return &___offset2352_147; }
	inline void set_offset2352_147(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2352_147 = value;
	}

	inline static int32_t get_offset_of_offset2368_148() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2368_148)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2368_148() const { return ___offset2368_148; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2368_148() { return &___offset2368_148; }
	inline void set_offset2368_148(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2368_148 = value;
	}

	inline static int32_t get_offset_of_offset2384_149() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2384_149)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2384_149() const { return ___offset2384_149; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2384_149() { return &___offset2384_149; }
	inline void set_offset2384_149(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2384_149 = value;
	}

	inline static int32_t get_offset_of_offset2400_150() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2400_150)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2400_150() const { return ___offset2400_150; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2400_150() { return &___offset2400_150; }
	inline void set_offset2400_150(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2400_150 = value;
	}

	inline static int32_t get_offset_of_offset2416_151() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2416_151)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2416_151() const { return ___offset2416_151; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2416_151() { return &___offset2416_151; }
	inline void set_offset2416_151(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2416_151 = value;
	}

	inline static int32_t get_offset_of_offset2432_152() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2432_152)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2432_152() const { return ___offset2432_152; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2432_152() { return &___offset2432_152; }
	inline void set_offset2432_152(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2432_152 = value;
	}

	inline static int32_t get_offset_of_offset2448_153() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2448_153)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2448_153() const { return ___offset2448_153; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2448_153() { return &___offset2448_153; }
	inline void set_offset2448_153(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2448_153 = value;
	}

	inline static int32_t get_offset_of_offset2464_154() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2464_154)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2464_154() const { return ___offset2464_154; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2464_154() { return &___offset2464_154; }
	inline void set_offset2464_154(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2464_154 = value;
	}

	inline static int32_t get_offset_of_offset2480_155() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2480_155)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2480_155() const { return ___offset2480_155; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2480_155() { return &___offset2480_155; }
	inline void set_offset2480_155(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2480_155 = value;
	}

	inline static int32_t get_offset_of_offset2496_156() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2496_156)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2496_156() const { return ___offset2496_156; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2496_156() { return &___offset2496_156; }
	inline void set_offset2496_156(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2496_156 = value;
	}

	inline static int32_t get_offset_of_offset2512_157() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2512_157)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2512_157() const { return ___offset2512_157; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2512_157() { return &___offset2512_157; }
	inline void set_offset2512_157(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2512_157 = value;
	}

	inline static int32_t get_offset_of_offset2528_158() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2528_158)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2528_158() const { return ___offset2528_158; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2528_158() { return &___offset2528_158; }
	inline void set_offset2528_158(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2528_158 = value;
	}

	inline static int32_t get_offset_of_offset2544_159() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2544_159)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2544_159() const { return ___offset2544_159; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2544_159() { return &___offset2544_159; }
	inline void set_offset2544_159(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2544_159 = value;
	}

	inline static int32_t get_offset_of_offset2560_160() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2560_160)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2560_160() const { return ___offset2560_160; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2560_160() { return &___offset2560_160; }
	inline void set_offset2560_160(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2560_160 = value;
	}

	inline static int32_t get_offset_of_offset2576_161() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2576_161)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2576_161() const { return ___offset2576_161; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2576_161() { return &___offset2576_161; }
	inline void set_offset2576_161(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2576_161 = value;
	}

	inline static int32_t get_offset_of_offset2592_162() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2592_162)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2592_162() const { return ___offset2592_162; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2592_162() { return &___offset2592_162; }
	inline void set_offset2592_162(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2592_162 = value;
	}

	inline static int32_t get_offset_of_offset2608_163() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2608_163)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2608_163() const { return ___offset2608_163; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2608_163() { return &___offset2608_163; }
	inline void set_offset2608_163(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2608_163 = value;
	}

	inline static int32_t get_offset_of_offset2624_164() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2624_164)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2624_164() const { return ___offset2624_164; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2624_164() { return &___offset2624_164; }
	inline void set_offset2624_164(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2624_164 = value;
	}

	inline static int32_t get_offset_of_offset2640_165() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2640_165)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2640_165() const { return ___offset2640_165; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2640_165() { return &___offset2640_165; }
	inline void set_offset2640_165(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2640_165 = value;
	}

	inline static int32_t get_offset_of_offset2656_166() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2656_166)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2656_166() const { return ___offset2656_166; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2656_166() { return &___offset2656_166; }
	inline void set_offset2656_166(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2656_166 = value;
	}

	inline static int32_t get_offset_of_offset2672_167() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2672_167)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2672_167() const { return ___offset2672_167; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2672_167() { return &___offset2672_167; }
	inline void set_offset2672_167(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2672_167 = value;
	}

	inline static int32_t get_offset_of_offset2688_168() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2688_168)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2688_168() const { return ___offset2688_168; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2688_168() { return &___offset2688_168; }
	inline void set_offset2688_168(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2688_168 = value;
	}

	inline static int32_t get_offset_of_offset2704_169() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2704_169)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2704_169() const { return ___offset2704_169; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2704_169() { return &___offset2704_169; }
	inline void set_offset2704_169(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2704_169 = value;
	}

	inline static int32_t get_offset_of_offset2720_170() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2720_170)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2720_170() const { return ___offset2720_170; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2720_170() { return &___offset2720_170; }
	inline void set_offset2720_170(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2720_170 = value;
	}

	inline static int32_t get_offset_of_offset2736_171() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2736_171)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2736_171() const { return ___offset2736_171; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2736_171() { return &___offset2736_171; }
	inline void set_offset2736_171(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2736_171 = value;
	}

	inline static int32_t get_offset_of_offset2752_172() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2752_172)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2752_172() const { return ___offset2752_172; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2752_172() { return &___offset2752_172; }
	inline void set_offset2752_172(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2752_172 = value;
	}

	inline static int32_t get_offset_of_offset2768_173() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2768_173)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2768_173() const { return ___offset2768_173; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2768_173() { return &___offset2768_173; }
	inline void set_offset2768_173(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2768_173 = value;
	}

	inline static int32_t get_offset_of_offset2784_174() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2784_174)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2784_174() const { return ___offset2784_174; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2784_174() { return &___offset2784_174; }
	inline void set_offset2784_174(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2784_174 = value;
	}

	inline static int32_t get_offset_of_offset2800_175() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2800_175)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2800_175() const { return ___offset2800_175; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2800_175() { return &___offset2800_175; }
	inline void set_offset2800_175(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2800_175 = value;
	}

	inline static int32_t get_offset_of_offset2816_176() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2816_176)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2816_176() const { return ___offset2816_176; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2816_176() { return &___offset2816_176; }
	inline void set_offset2816_176(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2816_176 = value;
	}

	inline static int32_t get_offset_of_offset2832_177() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2832_177)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2832_177() const { return ___offset2832_177; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2832_177() { return &___offset2832_177; }
	inline void set_offset2832_177(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2832_177 = value;
	}

	inline static int32_t get_offset_of_offset2848_178() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2848_178)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2848_178() const { return ___offset2848_178; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2848_178() { return &___offset2848_178; }
	inline void set_offset2848_178(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2848_178 = value;
	}

	inline static int32_t get_offset_of_offset2864_179() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2864_179)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2864_179() const { return ___offset2864_179; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2864_179() { return &___offset2864_179; }
	inline void set_offset2864_179(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2864_179 = value;
	}

	inline static int32_t get_offset_of_offset2880_180() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2880_180)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2880_180() const { return ___offset2880_180; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2880_180() { return &___offset2880_180; }
	inline void set_offset2880_180(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2880_180 = value;
	}

	inline static int32_t get_offset_of_offset2896_181() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2896_181)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2896_181() const { return ___offset2896_181; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2896_181() { return &___offset2896_181; }
	inline void set_offset2896_181(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2896_181 = value;
	}

	inline static int32_t get_offset_of_offset2912_182() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2912_182)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2912_182() const { return ___offset2912_182; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2912_182() { return &___offset2912_182; }
	inline void set_offset2912_182(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2912_182 = value;
	}

	inline static int32_t get_offset_of_offset2928_183() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2928_183)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2928_183() const { return ___offset2928_183; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2928_183() { return &___offset2928_183; }
	inline void set_offset2928_183(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2928_183 = value;
	}

	inline static int32_t get_offset_of_offset2944_184() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2944_184)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2944_184() const { return ___offset2944_184; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2944_184() { return &___offset2944_184; }
	inline void set_offset2944_184(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2944_184 = value;
	}

	inline static int32_t get_offset_of_offset2960_185() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2960_185)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2960_185() const { return ___offset2960_185; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2960_185() { return &___offset2960_185; }
	inline void set_offset2960_185(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2960_185 = value;
	}

	inline static int32_t get_offset_of_offset2976_186() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2976_186)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2976_186() const { return ___offset2976_186; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2976_186() { return &___offset2976_186; }
	inline void set_offset2976_186(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2976_186 = value;
	}

	inline static int32_t get_offset_of_offset2992_187() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset2992_187)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset2992_187() const { return ___offset2992_187; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset2992_187() { return &___offset2992_187; }
	inline void set_offset2992_187(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset2992_187 = value;
	}

	inline static int32_t get_offset_of_offset3008_188() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3008_188)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3008_188() const { return ___offset3008_188; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3008_188() { return &___offset3008_188; }
	inline void set_offset3008_188(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3008_188 = value;
	}

	inline static int32_t get_offset_of_offset3024_189() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3024_189)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3024_189() const { return ___offset3024_189; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3024_189() { return &___offset3024_189; }
	inline void set_offset3024_189(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3024_189 = value;
	}

	inline static int32_t get_offset_of_offset3040_190() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3040_190)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3040_190() const { return ___offset3040_190; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3040_190() { return &___offset3040_190; }
	inline void set_offset3040_190(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3040_190 = value;
	}

	inline static int32_t get_offset_of_offset3056_191() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3056_191)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3056_191() const { return ___offset3056_191; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3056_191() { return &___offset3056_191; }
	inline void set_offset3056_191(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3056_191 = value;
	}

	inline static int32_t get_offset_of_offset3072_192() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3072_192)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3072_192() const { return ___offset3072_192; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3072_192() { return &___offset3072_192; }
	inline void set_offset3072_192(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3072_192 = value;
	}

	inline static int32_t get_offset_of_offset3088_193() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3088_193)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3088_193() const { return ___offset3088_193; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3088_193() { return &___offset3088_193; }
	inline void set_offset3088_193(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3088_193 = value;
	}

	inline static int32_t get_offset_of_offset3104_194() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3104_194)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3104_194() const { return ___offset3104_194; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3104_194() { return &___offset3104_194; }
	inline void set_offset3104_194(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3104_194 = value;
	}

	inline static int32_t get_offset_of_offset3120_195() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3120_195)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3120_195() const { return ___offset3120_195; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3120_195() { return &___offset3120_195; }
	inline void set_offset3120_195(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3120_195 = value;
	}

	inline static int32_t get_offset_of_offset3136_196() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3136_196)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3136_196() const { return ___offset3136_196; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3136_196() { return &___offset3136_196; }
	inline void set_offset3136_196(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3136_196 = value;
	}

	inline static int32_t get_offset_of_offset3152_197() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3152_197)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3152_197() const { return ___offset3152_197; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3152_197() { return &___offset3152_197; }
	inline void set_offset3152_197(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3152_197 = value;
	}

	inline static int32_t get_offset_of_offset3168_198() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3168_198)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3168_198() const { return ___offset3168_198; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3168_198() { return &___offset3168_198; }
	inline void set_offset3168_198(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3168_198 = value;
	}

	inline static int32_t get_offset_of_offset3184_199() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3184_199)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3184_199() const { return ___offset3184_199; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3184_199() { return &___offset3184_199; }
	inline void set_offset3184_199(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3184_199 = value;
	}

	inline static int32_t get_offset_of_offset3200_200() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3200_200)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3200_200() const { return ___offset3200_200; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3200_200() { return &___offset3200_200; }
	inline void set_offset3200_200(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3200_200 = value;
	}

	inline static int32_t get_offset_of_offset3216_201() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3216_201)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3216_201() const { return ___offset3216_201; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3216_201() { return &___offset3216_201; }
	inline void set_offset3216_201(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3216_201 = value;
	}

	inline static int32_t get_offset_of_offset3232_202() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3232_202)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3232_202() const { return ___offset3232_202; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3232_202() { return &___offset3232_202; }
	inline void set_offset3232_202(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3232_202 = value;
	}

	inline static int32_t get_offset_of_offset3248_203() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3248_203)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3248_203() const { return ___offset3248_203; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3248_203() { return &___offset3248_203; }
	inline void set_offset3248_203(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3248_203 = value;
	}

	inline static int32_t get_offset_of_offset3264_204() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3264_204)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3264_204() const { return ___offset3264_204; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3264_204() { return &___offset3264_204; }
	inline void set_offset3264_204(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3264_204 = value;
	}

	inline static int32_t get_offset_of_offset3280_205() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3280_205)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3280_205() const { return ___offset3280_205; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3280_205() { return &___offset3280_205; }
	inline void set_offset3280_205(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3280_205 = value;
	}

	inline static int32_t get_offset_of_offset3296_206() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3296_206)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3296_206() const { return ___offset3296_206; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3296_206() { return &___offset3296_206; }
	inline void set_offset3296_206(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3296_206 = value;
	}

	inline static int32_t get_offset_of_offset3312_207() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3312_207)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3312_207() const { return ___offset3312_207; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3312_207() { return &___offset3312_207; }
	inline void set_offset3312_207(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3312_207 = value;
	}

	inline static int32_t get_offset_of_offset3328_208() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3328_208)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3328_208() const { return ___offset3328_208; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3328_208() { return &___offset3328_208; }
	inline void set_offset3328_208(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3328_208 = value;
	}

	inline static int32_t get_offset_of_offset3344_209() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3344_209)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3344_209() const { return ___offset3344_209; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3344_209() { return &___offset3344_209; }
	inline void set_offset3344_209(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3344_209 = value;
	}

	inline static int32_t get_offset_of_offset3360_210() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3360_210)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3360_210() const { return ___offset3360_210; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3360_210() { return &___offset3360_210; }
	inline void set_offset3360_210(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3360_210 = value;
	}

	inline static int32_t get_offset_of_offset3376_211() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3376_211)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3376_211() const { return ___offset3376_211; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3376_211() { return &___offset3376_211; }
	inline void set_offset3376_211(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3376_211 = value;
	}

	inline static int32_t get_offset_of_offset3392_212() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3392_212)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3392_212() const { return ___offset3392_212; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3392_212() { return &___offset3392_212; }
	inline void set_offset3392_212(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3392_212 = value;
	}

	inline static int32_t get_offset_of_offset3408_213() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3408_213)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3408_213() const { return ___offset3408_213; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3408_213() { return &___offset3408_213; }
	inline void set_offset3408_213(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3408_213 = value;
	}

	inline static int32_t get_offset_of_offset3424_214() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3424_214)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3424_214() const { return ___offset3424_214; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3424_214() { return &___offset3424_214; }
	inline void set_offset3424_214(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3424_214 = value;
	}

	inline static int32_t get_offset_of_offset3440_215() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3440_215)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3440_215() const { return ___offset3440_215; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3440_215() { return &___offset3440_215; }
	inline void set_offset3440_215(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3440_215 = value;
	}

	inline static int32_t get_offset_of_offset3456_216() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3456_216)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3456_216() const { return ___offset3456_216; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3456_216() { return &___offset3456_216; }
	inline void set_offset3456_216(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3456_216 = value;
	}

	inline static int32_t get_offset_of_offset3472_217() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3472_217)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3472_217() const { return ___offset3472_217; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3472_217() { return &___offset3472_217; }
	inline void set_offset3472_217(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3472_217 = value;
	}

	inline static int32_t get_offset_of_offset3488_218() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3488_218)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3488_218() const { return ___offset3488_218; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3488_218() { return &___offset3488_218; }
	inline void set_offset3488_218(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3488_218 = value;
	}

	inline static int32_t get_offset_of_offset3504_219() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3504_219)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3504_219() const { return ___offset3504_219; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3504_219() { return &___offset3504_219; }
	inline void set_offset3504_219(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3504_219 = value;
	}

	inline static int32_t get_offset_of_offset3520_220() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3520_220)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3520_220() const { return ___offset3520_220; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3520_220() { return &___offset3520_220; }
	inline void set_offset3520_220(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3520_220 = value;
	}

	inline static int32_t get_offset_of_offset3536_221() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3536_221)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3536_221() const { return ___offset3536_221; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3536_221() { return &___offset3536_221; }
	inline void set_offset3536_221(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3536_221 = value;
	}

	inline static int32_t get_offset_of_offset3552_222() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3552_222)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3552_222() const { return ___offset3552_222; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3552_222() { return &___offset3552_222; }
	inline void set_offset3552_222(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3552_222 = value;
	}

	inline static int32_t get_offset_of_offset3568_223() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3568_223)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3568_223() const { return ___offset3568_223; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3568_223() { return &___offset3568_223; }
	inline void set_offset3568_223(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3568_223 = value;
	}

	inline static int32_t get_offset_of_offset3584_224() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3584_224)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3584_224() const { return ___offset3584_224; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3584_224() { return &___offset3584_224; }
	inline void set_offset3584_224(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3584_224 = value;
	}

	inline static int32_t get_offset_of_offset3600_225() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3600_225)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3600_225() const { return ___offset3600_225; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3600_225() { return &___offset3600_225; }
	inline void set_offset3600_225(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3600_225 = value;
	}

	inline static int32_t get_offset_of_offset3616_226() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3616_226)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3616_226() const { return ___offset3616_226; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3616_226() { return &___offset3616_226; }
	inline void set_offset3616_226(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3616_226 = value;
	}

	inline static int32_t get_offset_of_offset3632_227() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3632_227)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3632_227() const { return ___offset3632_227; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3632_227() { return &___offset3632_227; }
	inline void set_offset3632_227(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3632_227 = value;
	}

	inline static int32_t get_offset_of_offset3648_228() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3648_228)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3648_228() const { return ___offset3648_228; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3648_228() { return &___offset3648_228; }
	inline void set_offset3648_228(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3648_228 = value;
	}

	inline static int32_t get_offset_of_offset3664_229() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3664_229)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3664_229() const { return ___offset3664_229; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3664_229() { return &___offset3664_229; }
	inline void set_offset3664_229(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3664_229 = value;
	}

	inline static int32_t get_offset_of_offset3680_230() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3680_230)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3680_230() const { return ___offset3680_230; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3680_230() { return &___offset3680_230; }
	inline void set_offset3680_230(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3680_230 = value;
	}

	inline static int32_t get_offset_of_offset3696_231() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3696_231)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3696_231() const { return ___offset3696_231; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3696_231() { return &___offset3696_231; }
	inline void set_offset3696_231(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3696_231 = value;
	}

	inline static int32_t get_offset_of_offset3712_232() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3712_232)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3712_232() const { return ___offset3712_232; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3712_232() { return &___offset3712_232; }
	inline void set_offset3712_232(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3712_232 = value;
	}

	inline static int32_t get_offset_of_offset3728_233() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3728_233)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3728_233() const { return ___offset3728_233; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3728_233() { return &___offset3728_233; }
	inline void set_offset3728_233(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3728_233 = value;
	}

	inline static int32_t get_offset_of_offset3744_234() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3744_234)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3744_234() const { return ___offset3744_234; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3744_234() { return &___offset3744_234; }
	inline void set_offset3744_234(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3744_234 = value;
	}

	inline static int32_t get_offset_of_offset3760_235() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3760_235)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3760_235() const { return ___offset3760_235; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3760_235() { return &___offset3760_235; }
	inline void set_offset3760_235(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3760_235 = value;
	}

	inline static int32_t get_offset_of_offset3776_236() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3776_236)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3776_236() const { return ___offset3776_236; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3776_236() { return &___offset3776_236; }
	inline void set_offset3776_236(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3776_236 = value;
	}

	inline static int32_t get_offset_of_offset3792_237() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3792_237)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3792_237() const { return ___offset3792_237; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3792_237() { return &___offset3792_237; }
	inline void set_offset3792_237(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3792_237 = value;
	}

	inline static int32_t get_offset_of_offset3808_238() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3808_238)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3808_238() const { return ___offset3808_238; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3808_238() { return &___offset3808_238; }
	inline void set_offset3808_238(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3808_238 = value;
	}

	inline static int32_t get_offset_of_offset3824_239() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3824_239)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3824_239() const { return ___offset3824_239; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3824_239() { return &___offset3824_239; }
	inline void set_offset3824_239(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3824_239 = value;
	}

	inline static int32_t get_offset_of_offset3840_240() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3840_240)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3840_240() const { return ___offset3840_240; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3840_240() { return &___offset3840_240; }
	inline void set_offset3840_240(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3840_240 = value;
	}

	inline static int32_t get_offset_of_offset3856_241() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3856_241)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3856_241() const { return ___offset3856_241; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3856_241() { return &___offset3856_241; }
	inline void set_offset3856_241(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3856_241 = value;
	}

	inline static int32_t get_offset_of_offset3872_242() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3872_242)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3872_242() const { return ___offset3872_242; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3872_242() { return &___offset3872_242; }
	inline void set_offset3872_242(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3872_242 = value;
	}

	inline static int32_t get_offset_of_offset3888_243() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3888_243)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3888_243() const { return ___offset3888_243; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3888_243() { return &___offset3888_243; }
	inline void set_offset3888_243(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3888_243 = value;
	}

	inline static int32_t get_offset_of_offset3904_244() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3904_244)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3904_244() const { return ___offset3904_244; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3904_244() { return &___offset3904_244; }
	inline void set_offset3904_244(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3904_244 = value;
	}

	inline static int32_t get_offset_of_offset3920_245() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3920_245)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3920_245() const { return ___offset3920_245; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3920_245() { return &___offset3920_245; }
	inline void set_offset3920_245(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3920_245 = value;
	}

	inline static int32_t get_offset_of_offset3936_246() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3936_246)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3936_246() const { return ___offset3936_246; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3936_246() { return &___offset3936_246; }
	inline void set_offset3936_246(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3936_246 = value;
	}

	inline static int32_t get_offset_of_offset3952_247() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3952_247)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3952_247() const { return ___offset3952_247; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3952_247() { return &___offset3952_247; }
	inline void set_offset3952_247(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3952_247 = value;
	}

	inline static int32_t get_offset_of_offset3968_248() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3968_248)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3968_248() const { return ___offset3968_248; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3968_248() { return &___offset3968_248; }
	inline void set_offset3968_248(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3968_248 = value;
	}

	inline static int32_t get_offset_of_offset3984_249() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset3984_249)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset3984_249() const { return ___offset3984_249; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset3984_249() { return &___offset3984_249; }
	inline void set_offset3984_249(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset3984_249 = value;
	}

	inline static int32_t get_offset_of_offset4000_250() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset4000_250)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset4000_250() const { return ___offset4000_250; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset4000_250() { return &___offset4000_250; }
	inline void set_offset4000_250(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset4000_250 = value;
	}

	inline static int32_t get_offset_of_offset4016_251() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset4016_251)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset4016_251() const { return ___offset4016_251; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset4016_251() { return &___offset4016_251; }
	inline void set_offset4016_251(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset4016_251 = value;
	}

	inline static int32_t get_offset_of_offset4032_252() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset4032_252)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset4032_252() const { return ___offset4032_252; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset4032_252() { return &___offset4032_252; }
	inline void set_offset4032_252(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset4032_252 = value;
	}

	inline static int32_t get_offset_of_offset4048_253() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset4048_253)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset4048_253() const { return ___offset4048_253; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset4048_253() { return &___offset4048_253; }
	inline void set_offset4048_253(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset4048_253 = value;
	}

	inline static int32_t get_offset_of_offset4064_254() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___offset4064_254)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset4064_254() const { return ___offset4064_254; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset4064_254() { return &___offset4064_254; }
	inline void set_offset4064_254(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset4064_254 = value;
	}

	inline static int32_t get_offset_of_byte4080_255() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4080_255)); }
	inline uint8_t get_byte4080_255() const { return ___byte4080_255; }
	inline uint8_t* get_address_of_byte4080_255() { return &___byte4080_255; }
	inline void set_byte4080_255(uint8_t value)
	{
		___byte4080_255 = value;
	}

	inline static int32_t get_offset_of_byte4081_256() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4081_256)); }
	inline uint8_t get_byte4081_256() const { return ___byte4081_256; }
	inline uint8_t* get_address_of_byte4081_256() { return &___byte4081_256; }
	inline void set_byte4081_256(uint8_t value)
	{
		___byte4081_256 = value;
	}

	inline static int32_t get_offset_of_byte4082_257() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4082_257)); }
	inline uint8_t get_byte4082_257() const { return ___byte4082_257; }
	inline uint8_t* get_address_of_byte4082_257() { return &___byte4082_257; }
	inline void set_byte4082_257(uint8_t value)
	{
		___byte4082_257 = value;
	}

	inline static int32_t get_offset_of_byte4083_258() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4083_258)); }
	inline uint8_t get_byte4083_258() const { return ___byte4083_258; }
	inline uint8_t* get_address_of_byte4083_258() { return &___byte4083_258; }
	inline void set_byte4083_258(uint8_t value)
	{
		___byte4083_258 = value;
	}

	inline static int32_t get_offset_of_byte4084_259() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4084_259)); }
	inline uint8_t get_byte4084_259() const { return ___byte4084_259; }
	inline uint8_t* get_address_of_byte4084_259() { return &___byte4084_259; }
	inline void set_byte4084_259(uint8_t value)
	{
		___byte4084_259 = value;
	}

	inline static int32_t get_offset_of_byte4085_260() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4085_260)); }
	inline uint8_t get_byte4085_260() const { return ___byte4085_260; }
	inline uint8_t* get_address_of_byte4085_260() { return &___byte4085_260; }
	inline void set_byte4085_260(uint8_t value)
	{
		___byte4085_260 = value;
	}

	inline static int32_t get_offset_of_byte4086_261() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4086_261)); }
	inline uint8_t get_byte4086_261() const { return ___byte4086_261; }
	inline uint8_t* get_address_of_byte4086_261() { return &___byte4086_261; }
	inline void set_byte4086_261(uint8_t value)
	{
		___byte4086_261 = value;
	}

	inline static int32_t get_offset_of_byte4087_262() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4087_262)); }
	inline uint8_t get_byte4087_262() const { return ___byte4087_262; }
	inline uint8_t* get_address_of_byte4087_262() { return &___byte4087_262; }
	inline void set_byte4087_262(uint8_t value)
	{
		___byte4087_262 = value;
	}

	inline static int32_t get_offset_of_byte4088_263() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4088_263)); }
	inline uint8_t get_byte4088_263() const { return ___byte4088_263; }
	inline uint8_t* get_address_of_byte4088_263() { return &___byte4088_263; }
	inline void set_byte4088_263(uint8_t value)
	{
		___byte4088_263 = value;
	}

	inline static int32_t get_offset_of_byte4089_264() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4089_264)); }
	inline uint8_t get_byte4089_264() const { return ___byte4089_264; }
	inline uint8_t* get_address_of_byte4089_264() { return &___byte4089_264; }
	inline void set_byte4089_264(uint8_t value)
	{
		___byte4089_264 = value;
	}

	inline static int32_t get_offset_of_byte4090_265() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4090_265)); }
	inline uint8_t get_byte4090_265() const { return ___byte4090_265; }
	inline uint8_t* get_address_of_byte4090_265() { return &___byte4090_265; }
	inline void set_byte4090_265(uint8_t value)
	{
		___byte4090_265 = value;
	}

	inline static int32_t get_offset_of_byte4091_266() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4091_266)); }
	inline uint8_t get_byte4091_266() const { return ___byte4091_266; }
	inline uint8_t* get_address_of_byte4091_266() { return &___byte4091_266; }
	inline void set_byte4091_266(uint8_t value)
	{
		___byte4091_266 = value;
	}

	inline static int32_t get_offset_of_byte4092_267() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4092_267)); }
	inline uint8_t get_byte4092_267() const { return ___byte4092_267; }
	inline uint8_t* get_address_of_byte4092_267() { return &___byte4092_267; }
	inline void set_byte4092_267(uint8_t value)
	{
		___byte4092_267 = value;
	}

	inline static int32_t get_offset_of_byte4093_268() { return static_cast<int32_t>(offsetof(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70, ___byte4093_268)); }
	inline uint8_t get_byte4093_268() const { return ___byte4093_268; }
	inline uint8_t* get_address_of_byte4093_268() { return &___byte4093_268; }
	inline void set_byte4093_268(uint8_t value)
	{
		___byte4093_268 = value;
	}
};


// Unity.Collections.FixedBytes510
struct FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0016_1_OffsetPadding[16];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0016
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0016_1_OffsetPadding_forAlignmentOnly[16];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0032_2_OffsetPadding[32];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0032
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0032_2_OffsetPadding_forAlignmentOnly[32];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0048_3_OffsetPadding[48];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0048
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0048_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0048_3_OffsetPadding_forAlignmentOnly[48];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0048_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0064_4_OffsetPadding[64];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0064
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0064_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0064_4_OffsetPadding_forAlignmentOnly[64];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0064_4_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0080_5_OffsetPadding[80];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0080
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0080_5;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0080_5_OffsetPadding_forAlignmentOnly[80];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0080_5_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0096_6_OffsetPadding[96];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0096
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0096_6;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0096_6_OffsetPadding_forAlignmentOnly[96];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0096_6_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0112_7_OffsetPadding[112];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0112
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0112_7;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0112_7_OffsetPadding_forAlignmentOnly[112];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0112_7_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0128_8_OffsetPadding[128];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0128
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0128_8;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0128_8_OffsetPadding_forAlignmentOnly[128];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0128_8_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0144_9_OffsetPadding[144];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0144
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0144_9;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0144_9_OffsetPadding_forAlignmentOnly[144];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0144_9_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0160_10_OffsetPadding[160];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0160
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0160_10;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0160_10_OffsetPadding_forAlignmentOnly[160];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0160_10_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0176_11_OffsetPadding[176];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0176
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0176_11;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0176_11_OffsetPadding_forAlignmentOnly[176];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0176_11_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0192_12_OffsetPadding[192];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0192
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0192_12;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0192_12_OffsetPadding_forAlignmentOnly[192];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0192_12_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0208_13_OffsetPadding[208];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0208
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0208_13;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0208_13_OffsetPadding_forAlignmentOnly[208];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0208_13_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0224_14_OffsetPadding[224];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0224
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0224_14;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0224_14_OffsetPadding_forAlignmentOnly[224];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0224_14_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0240_15_OffsetPadding[240];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0240
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0240_15;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0240_15_OffsetPadding_forAlignmentOnly[240];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0240_15_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0256_16_OffsetPadding[256];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0256
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0256_16;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0256_16_OffsetPadding_forAlignmentOnly[256];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0256_16_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0272_17_OffsetPadding[272];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0272
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0272_17;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0272_17_OffsetPadding_forAlignmentOnly[272];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0272_17_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0288_18_OffsetPadding[288];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0288
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0288_18;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0288_18_OffsetPadding_forAlignmentOnly[288];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0288_18_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0304_19_OffsetPadding[304];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0304
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0304_19;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0304_19_OffsetPadding_forAlignmentOnly[304];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0304_19_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0320_20_OffsetPadding[320];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0320
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0320_20;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0320_20_OffsetPadding_forAlignmentOnly[320];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0320_20_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0336_21_OffsetPadding[336];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0336
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0336_21;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0336_21_OffsetPadding_forAlignmentOnly[336];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0336_21_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0352_22_OffsetPadding[352];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0352
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0352_22;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0352_22_OffsetPadding_forAlignmentOnly[352];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0352_22_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0368_23_OffsetPadding[368];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0368
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0368_23;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0368_23_OffsetPadding_forAlignmentOnly[368];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0368_23_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0384_24_OffsetPadding[384];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0384
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0384_24;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0384_24_OffsetPadding_forAlignmentOnly[384];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0384_24_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0400_25_OffsetPadding[400];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0400
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0400_25;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0400_25_OffsetPadding_forAlignmentOnly[400];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0400_25_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0416_26_OffsetPadding[416];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0416
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0416_26;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0416_26_OffsetPadding_forAlignmentOnly[416];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0416_26_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0432_27_OffsetPadding[432];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0432
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0432_27;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0432_27_OffsetPadding_forAlignmentOnly[432];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0432_27_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0448_28_OffsetPadding[448];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0448
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0448_28;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0448_28_OffsetPadding_forAlignmentOnly[448];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0448_28_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0464_29_OffsetPadding[464];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0464
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0464_29;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0464_29_OffsetPadding_forAlignmentOnly[464];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0464_29_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0480_30_OffsetPadding[480];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes510::offset0480
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0480_30;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0480_30_OffsetPadding_forAlignmentOnly[480];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0480_30_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0496_31_OffsetPadding[496];
					// System.Byte Unity.Collections.FixedBytes510::byte0496
					uint8_t ___byte0496_31;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0496_31_OffsetPadding_forAlignmentOnly[496];
					uint8_t ___byte0496_31_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0497_32_OffsetPadding[497];
					// System.Byte Unity.Collections.FixedBytes510::byte0497
					uint8_t ___byte0497_32;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0497_32_OffsetPadding_forAlignmentOnly[497];
					uint8_t ___byte0497_32_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0498_33_OffsetPadding[498];
					// System.Byte Unity.Collections.FixedBytes510::byte0498
					uint8_t ___byte0498_33;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0498_33_OffsetPadding_forAlignmentOnly[498];
					uint8_t ___byte0498_33_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0499_34_OffsetPadding[499];
					// System.Byte Unity.Collections.FixedBytes510::byte0499
					uint8_t ___byte0499_34;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0499_34_OffsetPadding_forAlignmentOnly[499];
					uint8_t ___byte0499_34_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0500_35_OffsetPadding[500];
					// System.Byte Unity.Collections.FixedBytes510::byte0500
					uint8_t ___byte0500_35;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0500_35_OffsetPadding_forAlignmentOnly[500];
					uint8_t ___byte0500_35_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0501_36_OffsetPadding[501];
					// System.Byte Unity.Collections.FixedBytes510::byte0501
					uint8_t ___byte0501_36;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0501_36_OffsetPadding_forAlignmentOnly[501];
					uint8_t ___byte0501_36_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0502_37_OffsetPadding[502];
					// System.Byte Unity.Collections.FixedBytes510::byte0502
					uint8_t ___byte0502_37;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0502_37_OffsetPadding_forAlignmentOnly[502];
					uint8_t ___byte0502_37_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0503_38_OffsetPadding[503];
					// System.Byte Unity.Collections.FixedBytes510::byte0503
					uint8_t ___byte0503_38;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0503_38_OffsetPadding_forAlignmentOnly[503];
					uint8_t ___byte0503_38_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0504_39_OffsetPadding[504];
					// System.Byte Unity.Collections.FixedBytes510::byte0504
					uint8_t ___byte0504_39;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0504_39_OffsetPadding_forAlignmentOnly[504];
					uint8_t ___byte0504_39_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0505_40_OffsetPadding[505];
					// System.Byte Unity.Collections.FixedBytes510::byte0505
					uint8_t ___byte0505_40;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0505_40_OffsetPadding_forAlignmentOnly[505];
					uint8_t ___byte0505_40_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0506_41_OffsetPadding[506];
					// System.Byte Unity.Collections.FixedBytes510::byte0506
					uint8_t ___byte0506_41;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0506_41_OffsetPadding_forAlignmentOnly[506];
					uint8_t ___byte0506_41_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0507_42_OffsetPadding[507];
					// System.Byte Unity.Collections.FixedBytes510::byte0507
					uint8_t ___byte0507_42;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0507_42_OffsetPadding_forAlignmentOnly[507];
					uint8_t ___byte0507_42_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0508_43_OffsetPadding[508];
					// System.Byte Unity.Collections.FixedBytes510::byte0508
					uint8_t ___byte0508_43;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0508_43_OffsetPadding_forAlignmentOnly[508];
					uint8_t ___byte0508_43_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0509_44_OffsetPadding[509];
					// System.Byte Unity.Collections.FixedBytes510::byte0509
					uint8_t ___byte0509_44;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0509_44_OffsetPadding_forAlignmentOnly[509];
					uint8_t ___byte0509_44_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C__padding[510];
	};

public:
	inline static int32_t get_offset_of_offset0000_0() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0000_0)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0000_0() const { return ___offset0000_0; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0000_0() { return &___offset0000_0; }
	inline void set_offset0000_0(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0000_0 = value;
	}

	inline static int32_t get_offset_of_offset0016_1() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0016_1)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0016_1() const { return ___offset0016_1; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0016_1() { return &___offset0016_1; }
	inline void set_offset0016_1(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0016_1 = value;
	}

	inline static int32_t get_offset_of_offset0032_2() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0032_2)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0032_2() const { return ___offset0032_2; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0032_2() { return &___offset0032_2; }
	inline void set_offset0032_2(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0032_2 = value;
	}

	inline static int32_t get_offset_of_offset0048_3() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0048_3)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0048_3() const { return ___offset0048_3; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0048_3() { return &___offset0048_3; }
	inline void set_offset0048_3(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0048_3 = value;
	}

	inline static int32_t get_offset_of_offset0064_4() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0064_4)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0064_4() const { return ___offset0064_4; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0064_4() { return &___offset0064_4; }
	inline void set_offset0064_4(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0064_4 = value;
	}

	inline static int32_t get_offset_of_offset0080_5() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0080_5)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0080_5() const { return ___offset0080_5; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0080_5() { return &___offset0080_5; }
	inline void set_offset0080_5(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0080_5 = value;
	}

	inline static int32_t get_offset_of_offset0096_6() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0096_6)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0096_6() const { return ___offset0096_6; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0096_6() { return &___offset0096_6; }
	inline void set_offset0096_6(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0096_6 = value;
	}

	inline static int32_t get_offset_of_offset0112_7() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0112_7)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0112_7() const { return ___offset0112_7; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0112_7() { return &___offset0112_7; }
	inline void set_offset0112_7(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0112_7 = value;
	}

	inline static int32_t get_offset_of_offset0128_8() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0128_8)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0128_8() const { return ___offset0128_8; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0128_8() { return &___offset0128_8; }
	inline void set_offset0128_8(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0128_8 = value;
	}

	inline static int32_t get_offset_of_offset0144_9() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0144_9)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0144_9() const { return ___offset0144_9; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0144_9() { return &___offset0144_9; }
	inline void set_offset0144_9(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0144_9 = value;
	}

	inline static int32_t get_offset_of_offset0160_10() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0160_10)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0160_10() const { return ___offset0160_10; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0160_10() { return &___offset0160_10; }
	inline void set_offset0160_10(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0160_10 = value;
	}

	inline static int32_t get_offset_of_offset0176_11() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0176_11)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0176_11() const { return ___offset0176_11; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0176_11() { return &___offset0176_11; }
	inline void set_offset0176_11(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0176_11 = value;
	}

	inline static int32_t get_offset_of_offset0192_12() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0192_12)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0192_12() const { return ___offset0192_12; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0192_12() { return &___offset0192_12; }
	inline void set_offset0192_12(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0192_12 = value;
	}

	inline static int32_t get_offset_of_offset0208_13() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0208_13)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0208_13() const { return ___offset0208_13; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0208_13() { return &___offset0208_13; }
	inline void set_offset0208_13(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0208_13 = value;
	}

	inline static int32_t get_offset_of_offset0224_14() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0224_14)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0224_14() const { return ___offset0224_14; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0224_14() { return &___offset0224_14; }
	inline void set_offset0224_14(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0224_14 = value;
	}

	inline static int32_t get_offset_of_offset0240_15() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0240_15)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0240_15() const { return ___offset0240_15; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0240_15() { return &___offset0240_15; }
	inline void set_offset0240_15(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0240_15 = value;
	}

	inline static int32_t get_offset_of_offset0256_16() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0256_16)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0256_16() const { return ___offset0256_16; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0256_16() { return &___offset0256_16; }
	inline void set_offset0256_16(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0256_16 = value;
	}

	inline static int32_t get_offset_of_offset0272_17() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0272_17)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0272_17() const { return ___offset0272_17; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0272_17() { return &___offset0272_17; }
	inline void set_offset0272_17(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0272_17 = value;
	}

	inline static int32_t get_offset_of_offset0288_18() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0288_18)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0288_18() const { return ___offset0288_18; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0288_18() { return &___offset0288_18; }
	inline void set_offset0288_18(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0288_18 = value;
	}

	inline static int32_t get_offset_of_offset0304_19() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0304_19)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0304_19() const { return ___offset0304_19; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0304_19() { return &___offset0304_19; }
	inline void set_offset0304_19(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0304_19 = value;
	}

	inline static int32_t get_offset_of_offset0320_20() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0320_20)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0320_20() const { return ___offset0320_20; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0320_20() { return &___offset0320_20; }
	inline void set_offset0320_20(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0320_20 = value;
	}

	inline static int32_t get_offset_of_offset0336_21() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0336_21)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0336_21() const { return ___offset0336_21; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0336_21() { return &___offset0336_21; }
	inline void set_offset0336_21(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0336_21 = value;
	}

	inline static int32_t get_offset_of_offset0352_22() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0352_22)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0352_22() const { return ___offset0352_22; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0352_22() { return &___offset0352_22; }
	inline void set_offset0352_22(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0352_22 = value;
	}

	inline static int32_t get_offset_of_offset0368_23() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0368_23)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0368_23() const { return ___offset0368_23; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0368_23() { return &___offset0368_23; }
	inline void set_offset0368_23(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0368_23 = value;
	}

	inline static int32_t get_offset_of_offset0384_24() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0384_24)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0384_24() const { return ___offset0384_24; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0384_24() { return &___offset0384_24; }
	inline void set_offset0384_24(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0384_24 = value;
	}

	inline static int32_t get_offset_of_offset0400_25() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0400_25)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0400_25() const { return ___offset0400_25; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0400_25() { return &___offset0400_25; }
	inline void set_offset0400_25(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0400_25 = value;
	}

	inline static int32_t get_offset_of_offset0416_26() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0416_26)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0416_26() const { return ___offset0416_26; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0416_26() { return &___offset0416_26; }
	inline void set_offset0416_26(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0416_26 = value;
	}

	inline static int32_t get_offset_of_offset0432_27() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0432_27)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0432_27() const { return ___offset0432_27; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0432_27() { return &___offset0432_27; }
	inline void set_offset0432_27(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0432_27 = value;
	}

	inline static int32_t get_offset_of_offset0448_28() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0448_28)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0448_28() const { return ___offset0448_28; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0448_28() { return &___offset0448_28; }
	inline void set_offset0448_28(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0448_28 = value;
	}

	inline static int32_t get_offset_of_offset0464_29() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0464_29)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0464_29() const { return ___offset0464_29; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0464_29() { return &___offset0464_29; }
	inline void set_offset0464_29(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0464_29 = value;
	}

	inline static int32_t get_offset_of_offset0480_30() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___offset0480_30)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0480_30() const { return ___offset0480_30; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0480_30() { return &___offset0480_30; }
	inline void set_offset0480_30(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0480_30 = value;
	}

	inline static int32_t get_offset_of_byte0496_31() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0496_31)); }
	inline uint8_t get_byte0496_31() const { return ___byte0496_31; }
	inline uint8_t* get_address_of_byte0496_31() { return &___byte0496_31; }
	inline void set_byte0496_31(uint8_t value)
	{
		___byte0496_31 = value;
	}

	inline static int32_t get_offset_of_byte0497_32() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0497_32)); }
	inline uint8_t get_byte0497_32() const { return ___byte0497_32; }
	inline uint8_t* get_address_of_byte0497_32() { return &___byte0497_32; }
	inline void set_byte0497_32(uint8_t value)
	{
		___byte0497_32 = value;
	}

	inline static int32_t get_offset_of_byte0498_33() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0498_33)); }
	inline uint8_t get_byte0498_33() const { return ___byte0498_33; }
	inline uint8_t* get_address_of_byte0498_33() { return &___byte0498_33; }
	inline void set_byte0498_33(uint8_t value)
	{
		___byte0498_33 = value;
	}

	inline static int32_t get_offset_of_byte0499_34() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0499_34)); }
	inline uint8_t get_byte0499_34() const { return ___byte0499_34; }
	inline uint8_t* get_address_of_byte0499_34() { return &___byte0499_34; }
	inline void set_byte0499_34(uint8_t value)
	{
		___byte0499_34 = value;
	}

	inline static int32_t get_offset_of_byte0500_35() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0500_35)); }
	inline uint8_t get_byte0500_35() const { return ___byte0500_35; }
	inline uint8_t* get_address_of_byte0500_35() { return &___byte0500_35; }
	inline void set_byte0500_35(uint8_t value)
	{
		___byte0500_35 = value;
	}

	inline static int32_t get_offset_of_byte0501_36() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0501_36)); }
	inline uint8_t get_byte0501_36() const { return ___byte0501_36; }
	inline uint8_t* get_address_of_byte0501_36() { return &___byte0501_36; }
	inline void set_byte0501_36(uint8_t value)
	{
		___byte0501_36 = value;
	}

	inline static int32_t get_offset_of_byte0502_37() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0502_37)); }
	inline uint8_t get_byte0502_37() const { return ___byte0502_37; }
	inline uint8_t* get_address_of_byte0502_37() { return &___byte0502_37; }
	inline void set_byte0502_37(uint8_t value)
	{
		___byte0502_37 = value;
	}

	inline static int32_t get_offset_of_byte0503_38() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0503_38)); }
	inline uint8_t get_byte0503_38() const { return ___byte0503_38; }
	inline uint8_t* get_address_of_byte0503_38() { return &___byte0503_38; }
	inline void set_byte0503_38(uint8_t value)
	{
		___byte0503_38 = value;
	}

	inline static int32_t get_offset_of_byte0504_39() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0504_39)); }
	inline uint8_t get_byte0504_39() const { return ___byte0504_39; }
	inline uint8_t* get_address_of_byte0504_39() { return &___byte0504_39; }
	inline void set_byte0504_39(uint8_t value)
	{
		___byte0504_39 = value;
	}

	inline static int32_t get_offset_of_byte0505_40() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0505_40)); }
	inline uint8_t get_byte0505_40() const { return ___byte0505_40; }
	inline uint8_t* get_address_of_byte0505_40() { return &___byte0505_40; }
	inline void set_byte0505_40(uint8_t value)
	{
		___byte0505_40 = value;
	}

	inline static int32_t get_offset_of_byte0506_41() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0506_41)); }
	inline uint8_t get_byte0506_41() const { return ___byte0506_41; }
	inline uint8_t* get_address_of_byte0506_41() { return &___byte0506_41; }
	inline void set_byte0506_41(uint8_t value)
	{
		___byte0506_41 = value;
	}

	inline static int32_t get_offset_of_byte0507_42() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0507_42)); }
	inline uint8_t get_byte0507_42() const { return ___byte0507_42; }
	inline uint8_t* get_address_of_byte0507_42() { return &___byte0507_42; }
	inline void set_byte0507_42(uint8_t value)
	{
		___byte0507_42 = value;
	}

	inline static int32_t get_offset_of_byte0508_43() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0508_43)); }
	inline uint8_t get_byte0508_43() const { return ___byte0508_43; }
	inline uint8_t* get_address_of_byte0508_43() { return &___byte0508_43; }
	inline void set_byte0508_43(uint8_t value)
	{
		___byte0508_43 = value;
	}

	inline static int32_t get_offset_of_byte0509_44() { return static_cast<int32_t>(offsetof(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C, ___byte0509_44)); }
	inline uint8_t get_byte0509_44() const { return ___byte0509_44; }
	inline uint8_t* get_address_of_byte0509_44() { return &___byte0509_44; }
	inline void set_byte0509_44(uint8_t value)
	{
		___byte0509_44 = value;
	}
};


// Unity.Collections.FixedBytes62
struct FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes62::offset0000
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0000_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0016_1_OffsetPadding[16];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes62::offset0016
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0016_1_OffsetPadding_forAlignmentOnly[16];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0016_1_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___offset0032_2_OffsetPadding[32];
					// Unity.Collections.FixedBytes16 Unity.Collections.FixedBytes62::offset0032
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___offset0032_2_OffsetPadding_forAlignmentOnly[32];
					FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  ___offset0032_2_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0048_3_OffsetPadding[48];
					// System.Byte Unity.Collections.FixedBytes62::byte0048
					uint8_t ___byte0048_3;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0048_3_OffsetPadding_forAlignmentOnly[48];
					uint8_t ___byte0048_3_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0049_4_OffsetPadding[49];
					// System.Byte Unity.Collections.FixedBytes62::byte0049
					uint8_t ___byte0049_4;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0049_4_OffsetPadding_forAlignmentOnly[49];
					uint8_t ___byte0049_4_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0050_5_OffsetPadding[50];
					// System.Byte Unity.Collections.FixedBytes62::byte0050
					uint8_t ___byte0050_5;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0050_5_OffsetPadding_forAlignmentOnly[50];
					uint8_t ___byte0050_5_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0051_6_OffsetPadding[51];
					// System.Byte Unity.Collections.FixedBytes62::byte0051
					uint8_t ___byte0051_6;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0051_6_OffsetPadding_forAlignmentOnly[51];
					uint8_t ___byte0051_6_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0052_7_OffsetPadding[52];
					// System.Byte Unity.Collections.FixedBytes62::byte0052
					uint8_t ___byte0052_7;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0052_7_OffsetPadding_forAlignmentOnly[52];
					uint8_t ___byte0052_7_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0053_8_OffsetPadding[53];
					// System.Byte Unity.Collections.FixedBytes62::byte0053
					uint8_t ___byte0053_8;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0053_8_OffsetPadding_forAlignmentOnly[53];
					uint8_t ___byte0053_8_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0054_9_OffsetPadding[54];
					// System.Byte Unity.Collections.FixedBytes62::byte0054
					uint8_t ___byte0054_9;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0054_9_OffsetPadding_forAlignmentOnly[54];
					uint8_t ___byte0054_9_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0055_10_OffsetPadding[55];
					// System.Byte Unity.Collections.FixedBytes62::byte0055
					uint8_t ___byte0055_10;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0055_10_OffsetPadding_forAlignmentOnly[55];
					uint8_t ___byte0055_10_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0056_11_OffsetPadding[56];
					// System.Byte Unity.Collections.FixedBytes62::byte0056
					uint8_t ___byte0056_11;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0056_11_OffsetPadding_forAlignmentOnly[56];
					uint8_t ___byte0056_11_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0057_12_OffsetPadding[57];
					// System.Byte Unity.Collections.FixedBytes62::byte0057
					uint8_t ___byte0057_12;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0057_12_OffsetPadding_forAlignmentOnly[57];
					uint8_t ___byte0057_12_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0058_13_OffsetPadding[58];
					// System.Byte Unity.Collections.FixedBytes62::byte0058
					uint8_t ___byte0058_13;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0058_13_OffsetPadding_forAlignmentOnly[58];
					uint8_t ___byte0058_13_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0059_14_OffsetPadding[59];
					// System.Byte Unity.Collections.FixedBytes62::byte0059
					uint8_t ___byte0059_14;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0059_14_OffsetPadding_forAlignmentOnly[59];
					uint8_t ___byte0059_14_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0060_15_OffsetPadding[60];
					// System.Byte Unity.Collections.FixedBytes62::byte0060
					uint8_t ___byte0060_15;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0060_15_OffsetPadding_forAlignmentOnly[60];
					uint8_t ___byte0060_15_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___byte0061_16_OffsetPadding[61];
					// System.Byte Unity.Collections.FixedBytes62::byte0061
					uint8_t ___byte0061_16;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___byte0061_16_OffsetPadding_forAlignmentOnly[61];
					uint8_t ___byte0061_16_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4__padding[62];
	};

public:
	inline static int32_t get_offset_of_offset0000_0() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___offset0000_0)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0000_0() const { return ___offset0000_0; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0000_0() { return &___offset0000_0; }
	inline void set_offset0000_0(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0000_0 = value;
	}

	inline static int32_t get_offset_of_offset0016_1() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___offset0016_1)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0016_1() const { return ___offset0016_1; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0016_1() { return &___offset0016_1; }
	inline void set_offset0016_1(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0016_1 = value;
	}

	inline static int32_t get_offset_of_offset0032_2() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___offset0032_2)); }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  get_offset0032_2() const { return ___offset0032_2; }
	inline FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * get_address_of_offset0032_2() { return &___offset0032_2; }
	inline void set_offset0032_2(FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45  value)
	{
		___offset0032_2 = value;
	}

	inline static int32_t get_offset_of_byte0048_3() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0048_3)); }
	inline uint8_t get_byte0048_3() const { return ___byte0048_3; }
	inline uint8_t* get_address_of_byte0048_3() { return &___byte0048_3; }
	inline void set_byte0048_3(uint8_t value)
	{
		___byte0048_3 = value;
	}

	inline static int32_t get_offset_of_byte0049_4() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0049_4)); }
	inline uint8_t get_byte0049_4() const { return ___byte0049_4; }
	inline uint8_t* get_address_of_byte0049_4() { return &___byte0049_4; }
	inline void set_byte0049_4(uint8_t value)
	{
		___byte0049_4 = value;
	}

	inline static int32_t get_offset_of_byte0050_5() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0050_5)); }
	inline uint8_t get_byte0050_5() const { return ___byte0050_5; }
	inline uint8_t* get_address_of_byte0050_5() { return &___byte0050_5; }
	inline void set_byte0050_5(uint8_t value)
	{
		___byte0050_5 = value;
	}

	inline static int32_t get_offset_of_byte0051_6() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0051_6)); }
	inline uint8_t get_byte0051_6() const { return ___byte0051_6; }
	inline uint8_t* get_address_of_byte0051_6() { return &___byte0051_6; }
	inline void set_byte0051_6(uint8_t value)
	{
		___byte0051_6 = value;
	}

	inline static int32_t get_offset_of_byte0052_7() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0052_7)); }
	inline uint8_t get_byte0052_7() const { return ___byte0052_7; }
	inline uint8_t* get_address_of_byte0052_7() { return &___byte0052_7; }
	inline void set_byte0052_7(uint8_t value)
	{
		___byte0052_7 = value;
	}

	inline static int32_t get_offset_of_byte0053_8() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0053_8)); }
	inline uint8_t get_byte0053_8() const { return ___byte0053_8; }
	inline uint8_t* get_address_of_byte0053_8() { return &___byte0053_8; }
	inline void set_byte0053_8(uint8_t value)
	{
		___byte0053_8 = value;
	}

	inline static int32_t get_offset_of_byte0054_9() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0054_9)); }
	inline uint8_t get_byte0054_9() const { return ___byte0054_9; }
	inline uint8_t* get_address_of_byte0054_9() { return &___byte0054_9; }
	inline void set_byte0054_9(uint8_t value)
	{
		___byte0054_9 = value;
	}

	inline static int32_t get_offset_of_byte0055_10() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0055_10)); }
	inline uint8_t get_byte0055_10() const { return ___byte0055_10; }
	inline uint8_t* get_address_of_byte0055_10() { return &___byte0055_10; }
	inline void set_byte0055_10(uint8_t value)
	{
		___byte0055_10 = value;
	}

	inline static int32_t get_offset_of_byte0056_11() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0056_11)); }
	inline uint8_t get_byte0056_11() const { return ___byte0056_11; }
	inline uint8_t* get_address_of_byte0056_11() { return &___byte0056_11; }
	inline void set_byte0056_11(uint8_t value)
	{
		___byte0056_11 = value;
	}

	inline static int32_t get_offset_of_byte0057_12() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0057_12)); }
	inline uint8_t get_byte0057_12() const { return ___byte0057_12; }
	inline uint8_t* get_address_of_byte0057_12() { return &___byte0057_12; }
	inline void set_byte0057_12(uint8_t value)
	{
		___byte0057_12 = value;
	}

	inline static int32_t get_offset_of_byte0058_13() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0058_13)); }
	inline uint8_t get_byte0058_13() const { return ___byte0058_13; }
	inline uint8_t* get_address_of_byte0058_13() { return &___byte0058_13; }
	inline void set_byte0058_13(uint8_t value)
	{
		___byte0058_13 = value;
	}

	inline static int32_t get_offset_of_byte0059_14() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0059_14)); }
	inline uint8_t get_byte0059_14() const { return ___byte0059_14; }
	inline uint8_t* get_address_of_byte0059_14() { return &___byte0059_14; }
	inline void set_byte0059_14(uint8_t value)
	{
		___byte0059_14 = value;
	}

	inline static int32_t get_offset_of_byte0060_15() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0060_15)); }
	inline uint8_t get_byte0060_15() const { return ___byte0060_15; }
	inline uint8_t* get_address_of_byte0060_15() { return &___byte0060_15; }
	inline void set_byte0060_15(uint8_t value)
	{
		___byte0060_15 = value;
	}

	inline static int32_t get_offset_of_byte0061_16() { return static_cast<int32_t>(offsetof(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4, ___byte0061_16)); }
	inline uint8_t get_byte0061_16() const { return ___byte0061_16; }
	inline uint8_t* get_address_of_byte0061_16() { return &___byte0061_16; }
	inline void set_byte0061_16(uint8_t value)
	{
		___byte0061_16 = value;
	}
};


// Unity.Collections.LowLevel.Unsafe.UnsafeList
struct UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A 
{
public:
	// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeList::Ptr
	void* ___Ptr_0;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeList::Length
	int32_t ___Length_1;
	// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeList::Capacity
	int32_t ___Capacity_2;
	// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.LowLevel.Unsafe.UnsafeList::Allocator
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___Allocator_3;

public:
	inline static int32_t get_offset_of_Ptr_0() { return static_cast<int32_t>(offsetof(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A, ___Ptr_0)); }
	inline void* get_Ptr_0() const { return ___Ptr_0; }
	inline void** get_address_of_Ptr_0() { return &___Ptr_0; }
	inline void set_Ptr_0(void* value)
	{
		___Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Length_1() { return static_cast<int32_t>(offsetof(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A, ___Length_1)); }
	inline int32_t get_Length_1() const { return ___Length_1; }
	inline int32_t* get_address_of_Length_1() { return &___Length_1; }
	inline void set_Length_1(int32_t value)
	{
		___Length_1 = value;
	}

	inline static int32_t get_offset_of_Capacity_2() { return static_cast<int32_t>(offsetof(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A, ___Capacity_2)); }
	inline int32_t get_Capacity_2() const { return ___Capacity_2; }
	inline int32_t* get_address_of_Capacity_2() { return &___Capacity_2; }
	inline void set_Capacity_2(int32_t value)
	{
		___Capacity_2 = value;
	}

	inline static int32_t get_offset_of_Allocator_3() { return static_cast<int32_t>(offsetof(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A, ___Allocator_3)); }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  get_Allocator_3() const { return ___Allocator_3; }
	inline AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 * get_address_of_Allocator_3() { return &___Allocator_3; }
	inline void set_Allocator_3(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  value)
	{
		___Allocator_3 = value;
	}
};


// Unity.Collections.NativeArrayOptions
struct NativeArrayOptions_t23897F2D7CA2F1B58D2539C64062DD7C77615B6A 
{
public:
	// System.Int32 Unity.Collections.NativeArrayOptions::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(NativeArrayOptions_t23897F2D7CA2F1B58D2539C64062DD7C77615B6A, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.MulticastDelegate
struct MulticastDelegate_t  : public Delegate_t
{
public:
	// System.Delegate[] System.MulticastDelegate::delegates
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* ___delegates_11;

public:
	inline static int32_t get_offset_of_delegates_11() { return static_cast<int32_t>(offsetof(MulticastDelegate_t, ___delegates_11)); }
	inline DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* get_delegates_11() const { return ___delegates_11; }
	inline DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86** get_address_of_delegates_11() { return &___delegates_11; }
	inline void set_delegates_11(DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* value)
	{
		___delegates_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___delegates_11), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates_11;
};
// Native definition for COM marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates_11;
};

// System.SystemException
struct SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782  : public Exception_t
{
public:

public:
};


// Unity.Collections.AllocatorManager/Block
struct Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 
{
public:
	// Unity.Collections.AllocatorManager/Range Unity.Collections.AllocatorManager/Block::Range
	Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E  ___Range_0;
	// System.Int32 Unity.Collections.AllocatorManager/Block::BytesPerItem
	int32_t ___BytesPerItem_1;
	// System.Int32 Unity.Collections.AllocatorManager/Block::AllocatedItems
	int32_t ___AllocatedItems_2;
	// System.Byte Unity.Collections.AllocatorManager/Block::Log2Alignment
	uint8_t ___Log2Alignment_3;
	// System.Byte Unity.Collections.AllocatorManager/Block::Padding0
	uint8_t ___Padding0_4;
	// System.UInt16 Unity.Collections.AllocatorManager/Block::Padding1
	uint16_t ___Padding1_5;
	// System.UInt32 Unity.Collections.AllocatorManager/Block::Padding2
	uint32_t ___Padding2_6;

public:
	inline static int32_t get_offset_of_Range_0() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___Range_0)); }
	inline Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E  get_Range_0() const { return ___Range_0; }
	inline Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * get_address_of_Range_0() { return &___Range_0; }
	inline void set_Range_0(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E  value)
	{
		___Range_0 = value;
	}

	inline static int32_t get_offset_of_BytesPerItem_1() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___BytesPerItem_1)); }
	inline int32_t get_BytesPerItem_1() const { return ___BytesPerItem_1; }
	inline int32_t* get_address_of_BytesPerItem_1() { return &___BytesPerItem_1; }
	inline void set_BytesPerItem_1(int32_t value)
	{
		___BytesPerItem_1 = value;
	}

	inline static int32_t get_offset_of_AllocatedItems_2() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___AllocatedItems_2)); }
	inline int32_t get_AllocatedItems_2() const { return ___AllocatedItems_2; }
	inline int32_t* get_address_of_AllocatedItems_2() { return &___AllocatedItems_2; }
	inline void set_AllocatedItems_2(int32_t value)
	{
		___AllocatedItems_2 = value;
	}

	inline static int32_t get_offset_of_Log2Alignment_3() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___Log2Alignment_3)); }
	inline uint8_t get_Log2Alignment_3() const { return ___Log2Alignment_3; }
	inline uint8_t* get_address_of_Log2Alignment_3() { return &___Log2Alignment_3; }
	inline void set_Log2Alignment_3(uint8_t value)
	{
		___Log2Alignment_3 = value;
	}

	inline static int32_t get_offset_of_Padding0_4() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___Padding0_4)); }
	inline uint8_t get_Padding0_4() const { return ___Padding0_4; }
	inline uint8_t* get_address_of_Padding0_4() { return &___Padding0_4; }
	inline void set_Padding0_4(uint8_t value)
	{
		___Padding0_4 = value;
	}

	inline static int32_t get_offset_of_Padding1_5() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___Padding1_5)); }
	inline uint16_t get_Padding1_5() const { return ___Padding1_5; }
	inline uint16_t* get_address_of_Padding1_5() { return &___Padding1_5; }
	inline void set_Padding1_5(uint16_t value)
	{
		___Padding1_5 = value;
	}

	inline static int32_t get_offset_of_Padding2_6() { return static_cast<int32_t>(offsetof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5, ___Padding2_6)); }
	inline uint32_t get_Padding2_6() const { return ___Padding2_6; }
	inline uint32_t* get_address_of_Padding2_6() { return &___Padding2_6; }
	inline void set_Padding2_6(uint32_t value)
	{
		___Padding2_6 = value;
	}
};


// Unity.Collections.AllocatorManager/TableEntry16
struct TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 
{
public:
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f0
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f0_0;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f1
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f1_1;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f2
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f2_2;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f3
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f3_3;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f4
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f4_4;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f5
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f5_5;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f6
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f6_6;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f7
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f7_7;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f8
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f8_8;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f9
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f9_9;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f10
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f10_10;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f11
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f11_11;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f12
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f12_12;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f13
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f13_13;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f14
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f14_14;
	// Unity.Collections.AllocatorManager/TableEntry Unity.Collections.AllocatorManager/TableEntry16::f15
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  ___f15_15;

public:
	inline static int32_t get_offset_of_f0_0() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f0_0)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f0_0() const { return ___f0_0; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f0_0() { return &___f0_0; }
	inline void set_f0_0(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f0_0 = value;
	}

	inline static int32_t get_offset_of_f1_1() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f1_1)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f1_1() const { return ___f1_1; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f1_1() { return &___f1_1; }
	inline void set_f1_1(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f1_1 = value;
	}

	inline static int32_t get_offset_of_f2_2() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f2_2)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f2_2() const { return ___f2_2; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f2_2() { return &___f2_2; }
	inline void set_f2_2(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f2_2 = value;
	}

	inline static int32_t get_offset_of_f3_3() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f3_3)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f3_3() const { return ___f3_3; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f3_3() { return &___f3_3; }
	inline void set_f3_3(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f3_3 = value;
	}

	inline static int32_t get_offset_of_f4_4() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f4_4)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f4_4() const { return ___f4_4; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f4_4() { return &___f4_4; }
	inline void set_f4_4(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f4_4 = value;
	}

	inline static int32_t get_offset_of_f5_5() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f5_5)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f5_5() const { return ___f5_5; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f5_5() { return &___f5_5; }
	inline void set_f5_5(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f5_5 = value;
	}

	inline static int32_t get_offset_of_f6_6() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f6_6)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f6_6() const { return ___f6_6; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f6_6() { return &___f6_6; }
	inline void set_f6_6(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f6_6 = value;
	}

	inline static int32_t get_offset_of_f7_7() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f7_7)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f7_7() const { return ___f7_7; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f7_7() { return &___f7_7; }
	inline void set_f7_7(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f7_7 = value;
	}

	inline static int32_t get_offset_of_f8_8() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f8_8)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f8_8() const { return ___f8_8; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f8_8() { return &___f8_8; }
	inline void set_f8_8(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f8_8 = value;
	}

	inline static int32_t get_offset_of_f9_9() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f9_9)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f9_9() const { return ___f9_9; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f9_9() { return &___f9_9; }
	inline void set_f9_9(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f9_9 = value;
	}

	inline static int32_t get_offset_of_f10_10() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f10_10)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f10_10() const { return ___f10_10; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f10_10() { return &___f10_10; }
	inline void set_f10_10(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f10_10 = value;
	}

	inline static int32_t get_offset_of_f11_11() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f11_11)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f11_11() const { return ___f11_11; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f11_11() { return &___f11_11; }
	inline void set_f11_11(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f11_11 = value;
	}

	inline static int32_t get_offset_of_f12_12() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f12_12)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f12_12() const { return ___f12_12; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f12_12() { return &___f12_12; }
	inline void set_f12_12(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f12_12 = value;
	}

	inline static int32_t get_offset_of_f13_13() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f13_13)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f13_13() const { return ___f13_13; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f13_13() { return &___f13_13; }
	inline void set_f13_13(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f13_13 = value;
	}

	inline static int32_t get_offset_of_f14_14() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f14_14)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f14_14() const { return ___f14_14; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f14_14() { return &___f14_14; }
	inline void set_f14_14(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f14_14 = value;
	}

	inline static int32_t get_offset_of_f15_15() { return static_cast<int32_t>(offsetof(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9, ___f15_15)); }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  get_f15_15() const { return ___f15_15; }
	inline TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC * get_address_of_f15_15() { return &___f15_15; }
	inline void set_f15_15(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  value)
	{
		___f15_15 = value;
	}
};


// Unity.Collections.FixedListInt128
struct FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.UInt16 Unity.Collections.FixedListInt128::length
					uint16_t ___length_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint16_t ___length_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___buffer_1_OffsetPadding[2];
					// Unity.Collections.FixedBytes126 Unity.Collections.FixedListInt128::buffer
					FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1  ___buffer_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___buffer_1_OffsetPadding_forAlignmentOnly[2];
					FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1  ___buffer_1_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7__padding[128];
	};

public:
	inline static int32_t get_offset_of_length_0() { return static_cast<int32_t>(offsetof(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7, ___length_0)); }
	inline uint16_t get_length_0() const { return ___length_0; }
	inline uint16_t* get_address_of_length_0() { return &___length_0; }
	inline void set_length_0(uint16_t value)
	{
		___length_0 = value;
	}

	inline static int32_t get_offset_of_buffer_1() { return static_cast<int32_t>(offsetof(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7, ___buffer_1)); }
	inline FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1  get_buffer_1() const { return ___buffer_1; }
	inline FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1 * get_address_of_buffer_1() { return &___buffer_1; }
	inline void set_buffer_1(FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1  value)
	{
		___buffer_1 = value;
	}
};


// Unity.Collections.FixedListInt32
struct FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.UInt16 Unity.Collections.FixedListInt32::length
					uint16_t ___length_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint16_t ___length_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___buffer_1_OffsetPadding[2];
					// Unity.Collections.FixedBytes30 Unity.Collections.FixedListInt32::buffer
					FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28  ___buffer_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___buffer_1_OffsetPadding_forAlignmentOnly[2];
					FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28  ___buffer_1_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF__padding[32];
	};

public:
	inline static int32_t get_offset_of_length_0() { return static_cast<int32_t>(offsetof(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF, ___length_0)); }
	inline uint16_t get_length_0() const { return ___length_0; }
	inline uint16_t* get_address_of_length_0() { return &___length_0; }
	inline void set_length_0(uint16_t value)
	{
		___length_0 = value;
	}

	inline static int32_t get_offset_of_buffer_1() { return static_cast<int32_t>(offsetof(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF, ___buffer_1)); }
	inline FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28  get_buffer_1() const { return ___buffer_1; }
	inline FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28 * get_address_of_buffer_1() { return &___buffer_1; }
	inline void set_buffer_1(FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28  value)
	{
		___buffer_1 = value;
	}
};


// Unity.Collections.FixedListInt4096
struct FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.UInt16 Unity.Collections.FixedListInt4096::length
					uint16_t ___length_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint16_t ___length_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___buffer_1_OffsetPadding[2];
					// Unity.Collections.FixedBytes4094 Unity.Collections.FixedListInt4096::buffer
					FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70  ___buffer_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___buffer_1_OffsetPadding_forAlignmentOnly[2];
					FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70  ___buffer_1_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983__padding[4096];
	};

public:
	inline static int32_t get_offset_of_length_0() { return static_cast<int32_t>(offsetof(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983, ___length_0)); }
	inline uint16_t get_length_0() const { return ___length_0; }
	inline uint16_t* get_address_of_length_0() { return &___length_0; }
	inline void set_length_0(uint16_t value)
	{
		___length_0 = value;
	}

	inline static int32_t get_offset_of_buffer_1() { return static_cast<int32_t>(offsetof(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983, ___buffer_1)); }
	inline FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70  get_buffer_1() const { return ___buffer_1; }
	inline FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70 * get_address_of_buffer_1() { return &___buffer_1; }
	inline void set_buffer_1(FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70  value)
	{
		___buffer_1 = value;
	}
};


// Unity.Collections.FixedListInt512
struct FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.UInt16 Unity.Collections.FixedListInt512::length
					uint16_t ___length_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint16_t ___length_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___buffer_1_OffsetPadding[2];
					// Unity.Collections.FixedBytes510 Unity.Collections.FixedListInt512::buffer
					FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C  ___buffer_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___buffer_1_OffsetPadding_forAlignmentOnly[2];
					FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C  ___buffer_1_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794__padding[512];
	};

public:
	inline static int32_t get_offset_of_length_0() { return static_cast<int32_t>(offsetof(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794, ___length_0)); }
	inline uint16_t get_length_0() const { return ___length_0; }
	inline uint16_t* get_address_of_length_0() { return &___length_0; }
	inline void set_length_0(uint16_t value)
	{
		___length_0 = value;
	}

	inline static int32_t get_offset_of_buffer_1() { return static_cast<int32_t>(offsetof(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794, ___buffer_1)); }
	inline FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C  get_buffer_1() const { return ___buffer_1; }
	inline FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C * get_address_of_buffer_1() { return &___buffer_1; }
	inline void set_buffer_1(FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C  value)
	{
		___buffer_1 = value;
	}
};


// Unity.Collections.FixedListInt64
struct FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 
{
public:
	union
	{
		struct
		{
			union
			{
				#pragma pack(push, tp, 1)
				struct
				{
					// System.UInt16 Unity.Collections.FixedListInt64::length
					uint16_t ___length_0;
				};
				#pragma pack(pop, tp)
				struct
				{
					uint16_t ___length_0_forAlignmentOnly;
				};
				#pragma pack(push, tp, 1)
				struct
				{
					char ___buffer_1_OffsetPadding[2];
					// Unity.Collections.FixedBytes62 Unity.Collections.FixedListInt64::buffer
					FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4  ___buffer_1;
				};
				#pragma pack(pop, tp)
				struct
				{
					char ___buffer_1_OffsetPadding_forAlignmentOnly[2];
					FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4  ___buffer_1_forAlignmentOnly;
				};
			};
		};
		uint8_t FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0__padding[64];
	};

public:
	inline static int32_t get_offset_of_length_0() { return static_cast<int32_t>(offsetof(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0, ___length_0)); }
	inline uint16_t get_length_0() const { return ___length_0; }
	inline uint16_t* get_address_of_length_0() { return &___length_0; }
	inline void set_length_0(uint16_t value)
	{
		___length_0 = value;
	}

	inline static int32_t get_offset_of_buffer_1() { return static_cast<int32_t>(offsetof(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0, ___buffer_1)); }
	inline FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4  get_buffer_1() const { return ___buffer_1; }
	inline FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4 * get_address_of_buffer_1() { return &___buffer_1; }
	inline void set_buffer_1(FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4  value)
	{
		___buffer_1 = value;
	}
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


// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4  : public MulticastDelegate_t
{
public:

public:
};


// System.NotImplementedException
struct NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4  : public SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782
{
public:

public:
};


// Unity.Collections.AllocatorManager/SlabAllocator
struct SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C 
{
public:
	// Unity.Collections.AllocatorManager/Block Unity.Collections.AllocatorManager/SlabAllocator::Storage
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  ___Storage_0;
	// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::Log2SlabSizeInBytes
	int32_t ___Log2SlabSizeInBytes_1;
	// Unity.Collections.FixedListInt4096 Unity.Collections.AllocatorManager/SlabAllocator::Occupied
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___Occupied_2;
	// System.Int64 Unity.Collections.AllocatorManager/SlabAllocator::budgetInBytes
	int64_t ___budgetInBytes_3;
	// System.Int64 Unity.Collections.AllocatorManager/SlabAllocator::allocatedBytes
	int64_t ___allocatedBytes_4;

public:
	inline static int32_t get_offset_of_Storage_0() { return static_cast<int32_t>(offsetof(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C, ___Storage_0)); }
	inline Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  get_Storage_0() const { return ___Storage_0; }
	inline Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * get_address_of_Storage_0() { return &___Storage_0; }
	inline void set_Storage_0(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  value)
	{
		___Storage_0 = value;
	}

	inline static int32_t get_offset_of_Log2SlabSizeInBytes_1() { return static_cast<int32_t>(offsetof(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C, ___Log2SlabSizeInBytes_1)); }
	inline int32_t get_Log2SlabSizeInBytes_1() const { return ___Log2SlabSizeInBytes_1; }
	inline int32_t* get_address_of_Log2SlabSizeInBytes_1() { return &___Log2SlabSizeInBytes_1; }
	inline void set_Log2SlabSizeInBytes_1(int32_t value)
	{
		___Log2SlabSizeInBytes_1 = value;
	}

	inline static int32_t get_offset_of_Occupied_2() { return static_cast<int32_t>(offsetof(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C, ___Occupied_2)); }
	inline FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  get_Occupied_2() const { return ___Occupied_2; }
	inline FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * get_address_of_Occupied_2() { return &___Occupied_2; }
	inline void set_Occupied_2(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  value)
	{
		___Occupied_2 = value;
	}

	inline static int32_t get_offset_of_budgetInBytes_3() { return static_cast<int32_t>(offsetof(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C, ___budgetInBytes_3)); }
	inline int64_t get_budgetInBytes_3() const { return ___budgetInBytes_3; }
	inline int64_t* get_address_of_budgetInBytes_3() { return &___budgetInBytes_3; }
	inline void set_budgetInBytes_3(int64_t value)
	{
		___budgetInBytes_3 = value;
	}

	inline static int32_t get_offset_of_allocatedBytes_4() { return static_cast<int32_t>(offsetof(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C, ___allocatedBytes_4)); }
	inline int64_t get_allocatedBytes_4() const { return ___allocatedBytes_4; }
	inline int64_t* get_address_of_allocatedBytes_4() { return &___allocatedBytes_4; }
	inline void set_allocatedBytes_4(int64_t value)
	{
		___allocatedBytes_4 = value;
	}
};


// Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$PostfixBurstDelegate
struct Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Collections.AllocatorManager/StackAllocator
struct StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 
{
public:
	// Unity.Collections.AllocatorManager/Block Unity.Collections.AllocatorManager/StackAllocator::m_storage
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  ___m_storage_0;
	// System.Int64 Unity.Collections.AllocatorManager/StackAllocator::m_top
	int64_t ___m_top_1;
	// System.Int64 Unity.Collections.AllocatorManager/StackAllocator::budgetInBytes
	int64_t ___budgetInBytes_2;
	// System.Int64 Unity.Collections.AllocatorManager/StackAllocator::allocatedBytes
	int64_t ___allocatedBytes_3;

public:
	inline static int32_t get_offset_of_m_storage_0() { return static_cast<int32_t>(offsetof(StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330, ___m_storage_0)); }
	inline Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  get_m_storage_0() const { return ___m_storage_0; }
	inline Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * get_address_of_m_storage_0() { return &___m_storage_0; }
	inline void set_m_storage_0(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  value)
	{
		___m_storage_0 = value;
	}

	inline static int32_t get_offset_of_m_top_1() { return static_cast<int32_t>(offsetof(StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330, ___m_top_1)); }
	inline int64_t get_m_top_1() const { return ___m_top_1; }
	inline int64_t* get_address_of_m_top_1() { return &___m_top_1; }
	inline void set_m_top_1(int64_t value)
	{
		___m_top_1 = value;
	}

	inline static int32_t get_offset_of_budgetInBytes_2() { return static_cast<int32_t>(offsetof(StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330, ___budgetInBytes_2)); }
	inline int64_t get_budgetInBytes_2() const { return ___budgetInBytes_2; }
	inline int64_t* get_address_of_budgetInBytes_2() { return &___budgetInBytes_2; }
	inline void set_budgetInBytes_2(int64_t value)
	{
		___budgetInBytes_2 = value;
	}

	inline static int32_t get_offset_of_allocatedBytes_3() { return static_cast<int32_t>(offsetof(StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330, ___allocatedBytes_3)); }
	inline int64_t get_allocatedBytes_3() const { return ___allocatedBytes_3; }
	inline int64_t* get_address_of_allocatedBytes_3() { return &___allocatedBytes_3; }
	inline void set_allocatedBytes_3(int64_t value)
	{
		___allocatedBytes_3 = value;
	}
};


// Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$PostfixBurstDelegate
struct Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Collections.AllocatorManager/TableEntry256
struct TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 
{
public:
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f0
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f0_0;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f1
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f1_1;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f2
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f2_2;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f3
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f3_3;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f4
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f4_4;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f5
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f5_5;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f6
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f6_6;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f7
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f7_7;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f8
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f8_8;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f9
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f9_9;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f10
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f10_10;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f11
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f11_11;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f12
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f12_12;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f13
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f13_13;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f14
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f14_14;
	// Unity.Collections.AllocatorManager/TableEntry16 Unity.Collections.AllocatorManager/TableEntry256::f15
	TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  ___f15_15;

public:
	inline static int32_t get_offset_of_f0_0() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f0_0)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f0_0() const { return ___f0_0; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f0_0() { return &___f0_0; }
	inline void set_f0_0(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f0_0 = value;
	}

	inline static int32_t get_offset_of_f1_1() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f1_1)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f1_1() const { return ___f1_1; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f1_1() { return &___f1_1; }
	inline void set_f1_1(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f1_1 = value;
	}

	inline static int32_t get_offset_of_f2_2() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f2_2)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f2_2() const { return ___f2_2; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f2_2() { return &___f2_2; }
	inline void set_f2_2(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f2_2 = value;
	}

	inline static int32_t get_offset_of_f3_3() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f3_3)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f3_3() const { return ___f3_3; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f3_3() { return &___f3_3; }
	inline void set_f3_3(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f3_3 = value;
	}

	inline static int32_t get_offset_of_f4_4() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f4_4)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f4_4() const { return ___f4_4; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f4_4() { return &___f4_4; }
	inline void set_f4_4(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f4_4 = value;
	}

	inline static int32_t get_offset_of_f5_5() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f5_5)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f5_5() const { return ___f5_5; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f5_5() { return &___f5_5; }
	inline void set_f5_5(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f5_5 = value;
	}

	inline static int32_t get_offset_of_f6_6() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f6_6)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f6_6() const { return ___f6_6; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f6_6() { return &___f6_6; }
	inline void set_f6_6(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f6_6 = value;
	}

	inline static int32_t get_offset_of_f7_7() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f7_7)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f7_7() const { return ___f7_7; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f7_7() { return &___f7_7; }
	inline void set_f7_7(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f7_7 = value;
	}

	inline static int32_t get_offset_of_f8_8() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f8_8)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f8_8() const { return ___f8_8; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f8_8() { return &___f8_8; }
	inline void set_f8_8(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f8_8 = value;
	}

	inline static int32_t get_offset_of_f9_9() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f9_9)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f9_9() const { return ___f9_9; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f9_9() { return &___f9_9; }
	inline void set_f9_9(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f9_9 = value;
	}

	inline static int32_t get_offset_of_f10_10() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f10_10)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f10_10() const { return ___f10_10; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f10_10() { return &___f10_10; }
	inline void set_f10_10(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f10_10 = value;
	}

	inline static int32_t get_offset_of_f11_11() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f11_11)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f11_11() const { return ___f11_11; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f11_11() { return &___f11_11; }
	inline void set_f11_11(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f11_11 = value;
	}

	inline static int32_t get_offset_of_f12_12() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f12_12)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f12_12() const { return ___f12_12; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f12_12() { return &___f12_12; }
	inline void set_f12_12(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f12_12 = value;
	}

	inline static int32_t get_offset_of_f13_13() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f13_13)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f13_13() const { return ___f13_13; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f13_13() { return &___f13_13; }
	inline void set_f13_13(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f13_13 = value;
	}

	inline static int32_t get_offset_of_f14_14() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f14_14)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f14_14() const { return ___f14_14; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f14_14() { return &___f14_14; }
	inline void set_f14_14(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f14_14 = value;
	}

	inline static int32_t get_offset_of_f15_15() { return static_cast<int32_t>(offsetof(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358, ___f15_15)); }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  get_f15_15() const { return ___f15_15; }
	inline TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9 * get_address_of_f15_15() { return &___f15_15; }
	inline void set_f15_15(TableEntry16_tFBC63829CD97305D40061E0CEFD454CB148FA5E9  value)
	{
		___f15_15 = value;
	}
};


// Unity.Collections.AllocatorManager/TryFunction
struct TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268  : public MulticastDelegate_t
{
public:

public:
};


// Unity.Collections.AllocatorManager/TableEntry4096
struct TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 
{
public:
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f0
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f0_0;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f1
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f1_1;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f2
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f2_2;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f3
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f3_3;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f4
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f4_4;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f5
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f5_5;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f6
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f6_6;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f7
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f7_7;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f8
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f8_8;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f9
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f9_9;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f10
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f10_10;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f11
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f11_11;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f12
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f12_12;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f13
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f13_13;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f14
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f14_14;
	// Unity.Collections.AllocatorManager/TableEntry256 Unity.Collections.AllocatorManager/TableEntry4096::f15
	TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  ___f15_15;

public:
	inline static int32_t get_offset_of_f0_0() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f0_0)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f0_0() const { return ___f0_0; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f0_0() { return &___f0_0; }
	inline void set_f0_0(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f0_0 = value;
	}

	inline static int32_t get_offset_of_f1_1() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f1_1)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f1_1() const { return ___f1_1; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f1_1() { return &___f1_1; }
	inline void set_f1_1(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f1_1 = value;
	}

	inline static int32_t get_offset_of_f2_2() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f2_2)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f2_2() const { return ___f2_2; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f2_2() { return &___f2_2; }
	inline void set_f2_2(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f2_2 = value;
	}

	inline static int32_t get_offset_of_f3_3() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f3_3)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f3_3() const { return ___f3_3; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f3_3() { return &___f3_3; }
	inline void set_f3_3(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f3_3 = value;
	}

	inline static int32_t get_offset_of_f4_4() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f4_4)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f4_4() const { return ___f4_4; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f4_4() { return &___f4_4; }
	inline void set_f4_4(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f4_4 = value;
	}

	inline static int32_t get_offset_of_f5_5() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f5_5)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f5_5() const { return ___f5_5; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f5_5() { return &___f5_5; }
	inline void set_f5_5(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f5_5 = value;
	}

	inline static int32_t get_offset_of_f6_6() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f6_6)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f6_6() const { return ___f6_6; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f6_6() { return &___f6_6; }
	inline void set_f6_6(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f6_6 = value;
	}

	inline static int32_t get_offset_of_f7_7() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f7_7)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f7_7() const { return ___f7_7; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f7_7() { return &___f7_7; }
	inline void set_f7_7(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f7_7 = value;
	}

	inline static int32_t get_offset_of_f8_8() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f8_8)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f8_8() const { return ___f8_8; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f8_8() { return &___f8_8; }
	inline void set_f8_8(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f8_8 = value;
	}

	inline static int32_t get_offset_of_f9_9() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f9_9)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f9_9() const { return ___f9_9; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f9_9() { return &___f9_9; }
	inline void set_f9_9(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f9_9 = value;
	}

	inline static int32_t get_offset_of_f10_10() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f10_10)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f10_10() const { return ___f10_10; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f10_10() { return &___f10_10; }
	inline void set_f10_10(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f10_10 = value;
	}

	inline static int32_t get_offset_of_f11_11() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f11_11)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f11_11() const { return ___f11_11; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f11_11() { return &___f11_11; }
	inline void set_f11_11(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f11_11 = value;
	}

	inline static int32_t get_offset_of_f12_12() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f12_12)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f12_12() const { return ___f12_12; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f12_12() { return &___f12_12; }
	inline void set_f12_12(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f12_12 = value;
	}

	inline static int32_t get_offset_of_f13_13() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f13_13)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f13_13() const { return ___f13_13; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f13_13() { return &___f13_13; }
	inline void set_f13_13(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f13_13 = value;
	}

	inline static int32_t get_offset_of_f14_14() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f14_14)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f14_14() const { return ___f14_14; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f14_14() { return &___f14_14; }
	inline void set_f14_14(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f14_14 = value;
	}

	inline static int32_t get_offset_of_f15_15() { return static_cast<int32_t>(offsetof(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7, ___f15_15)); }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  get_f15_15() const { return ___f15_15; }
	inline TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358 * get_address_of_f15_15() { return &___f15_15; }
	inline void set_f15_15(TableEntry256_t984E49F00C9CE946F011B47755494989BA9D4358  value)
	{
		___f15_15 = value;
	}
};


// Unity.Collections.AllocatorManager/TableEntry65536
struct TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 
{
public:
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f0
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f0_0;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f1
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f1_1;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f2
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f2_2;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f3
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f3_3;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f4
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f4_4;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f5
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f5_5;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f6
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f6_6;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f7
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f7_7;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f8
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f8_8;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f9
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f9_9;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f10
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f10_10;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f11
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f11_11;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f12
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f12_12;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f13
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f13_13;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f14
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f14_14;
	// Unity.Collections.AllocatorManager/TableEntry4096 Unity.Collections.AllocatorManager/TableEntry65536::f15
	TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  ___f15_15;

public:
	inline static int32_t get_offset_of_f0_0() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f0_0)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f0_0() const { return ___f0_0; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f0_0() { return &___f0_0; }
	inline void set_f0_0(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f0_0 = value;
	}

	inline static int32_t get_offset_of_f1_1() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f1_1)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f1_1() const { return ___f1_1; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f1_1() { return &___f1_1; }
	inline void set_f1_1(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f1_1 = value;
	}

	inline static int32_t get_offset_of_f2_2() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f2_2)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f2_2() const { return ___f2_2; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f2_2() { return &___f2_2; }
	inline void set_f2_2(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f2_2 = value;
	}

	inline static int32_t get_offset_of_f3_3() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f3_3)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f3_3() const { return ___f3_3; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f3_3() { return &___f3_3; }
	inline void set_f3_3(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f3_3 = value;
	}

	inline static int32_t get_offset_of_f4_4() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f4_4)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f4_4() const { return ___f4_4; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f4_4() { return &___f4_4; }
	inline void set_f4_4(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f4_4 = value;
	}

	inline static int32_t get_offset_of_f5_5() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f5_5)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f5_5() const { return ___f5_5; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f5_5() { return &___f5_5; }
	inline void set_f5_5(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f5_5 = value;
	}

	inline static int32_t get_offset_of_f6_6() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f6_6)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f6_6() const { return ___f6_6; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f6_6() { return &___f6_6; }
	inline void set_f6_6(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f6_6 = value;
	}

	inline static int32_t get_offset_of_f7_7() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f7_7)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f7_7() const { return ___f7_7; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f7_7() { return &___f7_7; }
	inline void set_f7_7(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f7_7 = value;
	}

	inline static int32_t get_offset_of_f8_8() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f8_8)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f8_8() const { return ___f8_8; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f8_8() { return &___f8_8; }
	inline void set_f8_8(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f8_8 = value;
	}

	inline static int32_t get_offset_of_f9_9() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f9_9)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f9_9() const { return ___f9_9; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f9_9() { return &___f9_9; }
	inline void set_f9_9(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f9_9 = value;
	}

	inline static int32_t get_offset_of_f10_10() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f10_10)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f10_10() const { return ___f10_10; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f10_10() { return &___f10_10; }
	inline void set_f10_10(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f10_10 = value;
	}

	inline static int32_t get_offset_of_f11_11() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f11_11)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f11_11() const { return ___f11_11; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f11_11() { return &___f11_11; }
	inline void set_f11_11(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f11_11 = value;
	}

	inline static int32_t get_offset_of_f12_12() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f12_12)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f12_12() const { return ___f12_12; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f12_12() { return &___f12_12; }
	inline void set_f12_12(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f12_12 = value;
	}

	inline static int32_t get_offset_of_f13_13() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f13_13)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f13_13() const { return ___f13_13; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f13_13() { return &___f13_13; }
	inline void set_f13_13(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f13_13 = value;
	}

	inline static int32_t get_offset_of_f14_14() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f14_14)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f14_14() const { return ___f14_14; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f14_14() { return &___f14_14; }
	inline void set_f14_14(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f14_14 = value;
	}

	inline static int32_t get_offset_of_f15_15() { return static_cast<int32_t>(offsetof(TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7, ___f15_15)); }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  get_f15_15() const { return ___f15_15; }
	inline TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7 * get_address_of_f15_15() { return &___f15_15; }
	inline void set_f15_15(TableEntry4096_t89478ECD9CA334EADAB71E97C351EDBE2350ECF7  value)
	{
		___f15_15 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Delegate_t * m_Items[1];

public:
	inline Delegate_t * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Delegate_t * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Delegate_t * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Delegate_t * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


// !0& Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/TableEntry65536>::get_Data()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 * SharedStatic_1_get_Data_m2585600653432550FCB33327F0EC2989C76F5DC6_gshared (SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F * __this, const RuntimeMethod* method);
// System.Void Unity.Burst.FunctionPointer`1<System.Object>::.ctor(System.IntPtr)
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR void FunctionPointer_1__ctor_m05E91DF0F7C983F68774073D8477FAF5943068CD_gshared_inline (FunctionPointer_1_t5AF97C37E92E5F70B805E2C94E6BB3582D040303 * __this, intptr_t ___ptr0, const RuntimeMethod* method);
// !0 Unity.Burst.FunctionPointer`1<System.Object>::get_Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * FunctionPointer_1_get_Invoke_mE4FCA6BFE9733EBA205506DE74C088EA90FFC891_gshared (FunctionPointer_1_t5AF97C37E92E5F70B805E2C94E6BB3582D040303 * __this, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/TableEntry65536>::GetOrCreateUnsafe(System.UInt32,System.Int64,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  SharedStatic_1_GetOrCreateUnsafe_m175F8547F0CF4DBA988A8BC79AF6F443590B595A_gshared (uint32_t ___alignment0, int64_t ___hashCode1, int64_t ___subHashCode2, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedList::PaddingBytes<System.Int32>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_gshared (const RuntimeMethod* method);
// !!0 Unity.Collections.LowLevel.Unsafe.UnsafeUtility::ReadArrayElement<System.Int32>(System.Void*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_gshared (void* ___source0, int32_t ___index1, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::WriteArrayElement<System.Int32>(System.Void*,System.Int32,!!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_WriteArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m21A03DD8050619772A9117BE97EDD6CF543115EA_gshared (void* ___destination0, int32_t ___index1, int32_t ___value2, const RuntimeMethod* method);
// T* Unity.Collections.AllocatorManager::Allocate<Unity.Collections.LowLevel.Unsafe.UnsafeList>(Unity.Collections.AllocatorManager/AllocatorHandle,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * AllocatorManager_Allocate_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mB55752C04C2E0BF01026A46F7628A6A6C83A1B96_gshared (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, int32_t ___items1, const RuntimeMethod* method);
// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeUtility::SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeList>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t UnsafeUtility_SizeOf_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mF5121761EBFDF720A7C139604AFCBAA28008C170_gshared (const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager::Free<Unity.Collections.LowLevel.Unsafe.UnsafeList>(Unity.Collections.AllocatorManager/AllocatorHandle,T*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AllocatorManager_Free_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mC9DEFCD77EAF09FCF9CF7C11E3C9D8233DDF9950_gshared (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * ___pointer1, int32_t ___items2, const RuntimeMethod* method);

// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Initialize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall_Initialize_m9B6FD30747453E4F5D0B56D91EA3E08D9A968DD0 (const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Initialize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall_Initialize_mE2FFC9DD8B84C7107F795C02DA5D8D08A107B1F1 (const RuntimeMethod* method);
// System.Void System.Attribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0 (Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74 * __this, const RuntimeMethod* method);
// Unity.Collections.AllocatorManager/SmallAllocatorHandle Unity.Collections.AllocatorManager/SmallAllocatorHandle::op_Implicit(Unity.Collections.AllocatorManager/AllocatorHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  SmallAllocatorHandle_op_Implicit_mBD832FEF0A1B4FC3BCC74B3EEA3FC295C015BDA2 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___a0, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/Block::set_Alignment(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Block_set_Alignment_mE1B6FDB79DD245BDAD3344390AB275D5ADC0BF9F (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager::Try(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32 (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method);
// System.Void System.ArgumentException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7 (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void* System.IntPtr::op_Explicit(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027 (intptr_t ___value0, const RuntimeMethod* method);
// System.IntPtr System.IntPtr::op_Explicit(System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t IntPtr_op_Explicit_m7F0C4B884FFB05BD231154CBDAEBCF1917019C21 (void* ___value0, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager::Free(Unity.Collections.AllocatorManager/AllocatorHandle,System.Void*,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, void* ___pointer1, int32_t ___itemSizeInBytes2, int32_t ___alignmentInBytes3, int32_t ___items4, const RuntimeMethod* method);
// System.Boolean System.IntPtr::op_Equality(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934 (intptr_t ___value10, intptr_t ___value21, const RuntimeMethod* method);
// System.Int64 Unity.Collections.AllocatorManager/Block::get_Bytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614 (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/Block::get_Alignment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Block_get_Alignment_mF11F2FB35FBF18414BD78A301DD0E19373F2BCCC (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method);
// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility::Malloc(System.Int64,System.Int32,Unity.Collections.Allocator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* UnsafeUtility_Malloc_m43BC7C9BE1437A70DD9A236418B0906CD3617331 (int64_t ___size0, int32_t ___alignment1, int32_t ___allocator2, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::Free(System.Void*,Unity.Collections.Allocator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_Free_mAC082BB03B10D20CA9E5AD7FBA33164DF2B52E89 (void* ___memory0, int32_t ___allocator1, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager::TryLegacy(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AllocatorManager_TryLegacy_m8FA25D28AD957F9E1695FB2FFC9BF65338E48BCF (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method);
// !0& Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/TableEntry65536>::get_Data()
inline TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 * SharedStatic_1_get_Data_m2585600653432550FCB33327F0EC2989C76F5DC6 (SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F * __this, const RuntimeMethod* method)
{
	return ((  TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 * (*) (SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F *, const RuntimeMethod*))SharedStatic_1_get_Data_m2585600653432550FCB33327F0EC2989C76F5DC6_gshared)(__this, method);
}
// System.Void Unity.Burst.FunctionPointer`1<Unity.Collections.AllocatorManager/TryFunction>::.ctor(System.IntPtr)
inline void FunctionPointer_1__ctor_m88698023CA5671A2E19B7EF5076962FBD71EEC45_inline (FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 * __this, intptr_t ___ptr0, const RuntimeMethod* method)
{
	((  void (*) (FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 *, intptr_t, const RuntimeMethod*))FunctionPointer_1__ctor_m05E91DF0F7C983F68774073D8477FAF5943068CD_gshared_inline)(__this, ___ptr0, method);
}
// !0 Unity.Burst.FunctionPointer`1<Unity.Collections.AllocatorManager/TryFunction>::get_Invoke()
inline TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * FunctionPointer_1_get_Invoke_m031425B62699967966A4CAF4D062D1197B730E5E (FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 * __this, const RuntimeMethod* method)
{
	return ((  TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * (*) (FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 *, const RuntimeMethod*))FunctionPointer_1_get_Invoke_mE4FCA6BFE9733EBA205506DE74C088EA90FFC891_gshared)(__this, method);
}
// System.Int32 Unity.Collections.AllocatorManager/TryFunction::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TryFunction_Invoke_m0C76EC4668A2F6116EC0FF3AA01B12ECEA8D4C85 (TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method);
// System.Int32 Unity.Mathematics.math::max(System.Int32,System.Int32)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_max_mE358BDDC8FCC6DCACBC5DAACE15C1B74CAA41CF7_inline (int32_t ___x0, int32_t ___y1, const RuntimeMethod* method);
// System.Int32 Unity.Mathematics.math::lzcnt(System.Int32)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_lzcnt_m960E448337A464EAFF1261B9F67725F97207300C_inline (int32_t ___x0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/Block::TryFree()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/Block::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Block_Dispose_m394AE073CC03A5812CBC2696FD28D350FE710CDF (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/Range::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Range_Dispose_mB5304E6725120C599A3A4DDDC6339E6CA474CB7A (Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::get_SlabSizeInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05 (SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, int32_t ___index0, const RuntimeMethod* method);
// System.Void Unity.Collections.FixedListInt4096::set_Item(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, int32_t ___index0, int32_t ___value1, const RuntimeMethod* method);
// System.IntPtr System.IntPtr::op_Addition(System.IntPtr,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t IntPtr_op_Addition_mD815D6B36C7DFA1F89481720D3D46A6484BB9644 (intptr_t ___pointer0, int32_t ___offset1, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method);
// System.Int64 System.IntPtr::op_Explicit(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t IntPtr_op_Explicit_m254924E8680FCCF870F18064DC0B114445B09172 (intptr_t ___value0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::Try(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972 (SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000ACAU24BurstDirectCall_Invoke_mEBE8EE6CA0E3E1822B42856658237A0ABF3DD68B (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/SlabAllocator::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SlabAllocator_Dispose_m0B657EF865140D5BB55C843B61D8E8F3A2CBB005 (SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * __this, const RuntimeMethod* method);
// System.Void* Unity.Burst.BurstCompiler::GetILPPMethodFunctionPointer2(System.IntPtr,System.RuntimeMethodHandle,System.RuntimeTypeHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* BurstCompiler_GetILPPMethodFunctionPointer2_mC5481172A163C21818ED26A7263F024D6A7752BE (intptr_t ___ilppMethod0, RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F  ___managedMethodHandle1, RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  ___delegateTypeHandle2, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::GetFunctionPointerDiscard(System.IntPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall_GetFunctionPointerDiscard_m6D57CFDB0E90300242AD61433E9B2E9E161AC1A9 (intptr_t* p0, const RuntimeMethod* method);
// System.IntPtr Unity.Burst.BurstCompiler::CompileILPPMethod2(System.RuntimeMethodHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t BurstCompiler_CompileILPPMethod2_mCE18C77E36D7BB2CF708E02DAB88BAECE602E29A (RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F  ___burstMethodHandle0, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Constructor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall_Constructor_m505A404645A774A1EECA51E6288C6426BD5FAE00 (const RuntimeMethod* method);
// System.Boolean Unity.Burst.BurstCompiler::get_IsEnabled()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool BurstCompiler_get_IsEnabled_m05F933051E525210A3A999C9EA671AF9C51312F0 (const RuntimeMethod* method);
// System.IntPtr Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::GetFunctionPointer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Try_00000ACAU24BurstDirectCall_GetFunctionPointer_mE960ACE388298074ADD1CEF79DD565A04158BFAD (const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::Try$BurstManaged(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t SlabAllocator_TryU24BurstManaged_m0E85623C6B44B7C3B6A5829BF73EFF1BD5A1E8B1_inline (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator::Try(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9 (StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000AC0U24BurstDirectCall_Invoke_m06D304524A473E4768EC248F5F9853E51FF43DD5 (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/StackAllocator::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StackAllocator_Dispose_m17928A2A471E508BD80B87B6A0D3BEAD07E66200 (StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::GetFunctionPointerDiscard(System.IntPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall_GetFunctionPointerDiscard_m8840DE8BB24CFF03BAE3663B68B0BBB0B7A9950A (intptr_t* p0, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Constructor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall_Constructor_m0D678E2482010B83B9C308B93E82FF5251FAC73A (const RuntimeMethod* method);
// System.IntPtr Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::GetFunctionPointer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Try_00000AC0U24BurstDirectCall_GetFunctionPointer_mF5ACDA3A6A948DD3EDBDD62EBA0807FAA1B70CC6 (const RuntimeMethod* method);
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator::Try$BurstManaged(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t StackAllocator_TryU24BurstManaged_m46B078D3E5C2608D24398E4A9B1AA71F352F3FBE_inline (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method);
// Unity.Burst.SharedStatic`1<!0> Unity.Burst.SharedStatic`1<Unity.Collections.AllocatorManager/TableEntry65536>::GetOrCreateUnsafe(System.UInt32,System.Int64,System.Int64)
inline SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  SharedStatic_1_GetOrCreateUnsafe_m175F8547F0CF4DBA988A8BC79AF6F443590B595A (uint32_t ___alignment0, int64_t ___hashCode1, int64_t ___subHashCode2, const RuntimeMethod* method)
{
	return ((  SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  (*) (uint32_t, int64_t, int64_t, const RuntimeMethod*))SharedStatic_1_GetOrCreateUnsafe_m175F8547F0CF4DBA988A8BC79AF6F443590B595A_gshared)(___alignment0, ___hashCode1, ___subHashCode2, method);
}
// System.Int32 Unity.Collections.FixedListInt128::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_LengthInBytes_m0325EE4F0B2509330EE1C98963454B480BE5A2D4 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedList::PaddingBytes<System.Int32>()
inline int32_t FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37 (const RuntimeMethod* method)
{
	return ((  int32_t (*) (const RuntimeMethod*))FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_gshared)(method);
}
// System.Byte* Unity.Collections.FixedListInt128::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method);
// !!0 Unity.Collections.LowLevel.Unsafe.UnsafeUtility::ReadArrayElement<System.Int32>(System.Void*,System.Int32)
inline int32_t UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D (void* ___source0, int32_t ___index1, const RuntimeMethod* method)
{
	return ((  int32_t (*) (void*, int32_t, const RuntimeMethod*))UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_gshared)(___source0, ___index1, method);
}
// System.Int32 Unity.Collections.FixedListInt128::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, int32_t ___index0, const RuntimeMethod* method);
// System.UInt32 Unity.Collections.CollectionHelper::Hash(System.Void*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517 (void* ___pointer0, int32_t ___bytes1, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_GetHashCode_m36E604637F0B9E956D38D4F8DBADFF2414C0BABA (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method);
// System.Int32 Unity.Mathematics.math::min(System.Int32,System.Int32)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline (int32_t ___x0, int32_t ___y1, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, int32_t ___index0, const RuntimeMethod* method);
// System.Int32 System.Int32::CompareTo(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195 (int32_t* __this, int32_t ___value0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_mC94C48315E2CC254EDB192D63E8A3417E152185F (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_m1F712C0AB7D83AAB0856EFF896116E8EFE32E791 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, int32_t ___index0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_mB5EC715871DA565C70D863DF706822F7E067FC18 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_m1F0902DC7ADCED7E17DB901B2A116E6E2A977224 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_m8319D1AA0407ED664000E714FBAFF32F94A9613A (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_m0BDB5A185692A45140F6289D6D8D651F2E561082 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, int32_t ___index0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_m8D2466754C727E9F22BB83BA76E780B1CF462FE2 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_mE569B14EEF1C0E45EDDADB2793CBD843679BB1B7 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_mEA7A950BEC9588B430C806BD527E05433EBA89AE (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_mBD792DFEAE2F89DA165E52BECAA2B91C7205A91D (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt128::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_mB686B28CC8F05B63BB4246D93B00ECB2AC62D31C (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Void System.NotImplementedException::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4 (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * __this, const RuntimeMethod* method);
// System.Collections.IEnumerator Unity.Collections.FixedListInt128::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt128::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_LengthInBytes_m350F216ED1E5D3ADE5F8CB01E4C09481E79F5FCE (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method);
// System.Byte* Unity.Collections.FixedListInt32::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_GetHashCode_mED71044DA76AF56781CCF3AED232CE2040B8B2BE (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m051AB0C39B761522764D3B79CAFD01925FE2273A (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m051CADA0101FDD7BAB849FEE9062FE3BCBDFDDCB (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m5B87A003649B50172375B6CCC01DA9E6307D7C79 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m46F5DCFB90A27FBBE79C7A17921FAF983294C7F4 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_mE56D3F480B6D084B3649359CF4682322BF1E3288 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m9CBEBCFB378CDAF7902EB87F8367D026B7EFE301 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m7F67FA3C982D2822AF2C2B80DEAF6ED125318873 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_mC3EFEC6CAB5E1EEC0C7644152B345639A2F7BBA8 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m68AC22237D7F90E44D3FD6F2FC4CD352C2EB8F2B (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m6C846A2E3D739C136BFFBD7CE1ED24370620C506 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt32::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m57F5A272F557C2BC73A44A7FAF2C80AFC572AECE (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Collections.IEnumerator Unity.Collections.FixedListInt32::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt32::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_LengthInBytes_m48A47979BDC97B2126C21C219739D2DF50B5FFC7 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method);
// System.Byte* Unity.Collections.FixedListInt4096::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::WriteArrayElement<System.Int32>(System.Void*,System.Int32,!!0)
inline void UnsafeUtility_WriteArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m21A03DD8050619772A9117BE97EDD6CF543115EA (void* ___destination0, int32_t ___index1, int32_t ___value2, const RuntimeMethod* method)
{
	((  void (*) (void*, int32_t, int32_t, const RuntimeMethod*))UnsafeUtility_WriteArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m21A03DD8050619772A9117BE97EDD6CF543115EA_gshared)(___destination0, ___index1, ___value2, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_GetHashCode_m9920DEC070046A6D1F0B5CCF079BD51FD632307B (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m5701F729EB47E4EDE6F47177E7978EE26681DBD0 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m83774C41A568471A05ED2B4F594C4289C8755274 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m46F1FEACC4C6AA7AA943EB9488E903F6A774A50C (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m0782219FD99F97CC3DB3A4FA4A1F56372D48251F (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m06DE10845994FDF3AA3D122CBB4E8D4363ABC9F3 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m6C6F5FC73BC597D40DC88B8915AD8AB14470586D (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m7936D17714F132E656F1CFFB2008959537CD16FC (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m526D0CC2C90CC4573747FDD42884EE803275ECF8 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m92DC90E0318186C8EE0110CF63F850FEF138C513 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m1792C55B4ACB6D4C8C2B68E820845AB0B9B1E705 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt4096::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m6425DE1D251DE6D5988FFE46AEBEA141B33F08A7 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Collections.IEnumerator Unity.Collections.FixedListInt4096::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt4096::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_LengthInBytes_mD882B0A7E9F1A9F5008DB6491D0771ED70660FC9 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method);
// System.Byte* Unity.Collections.FixedListInt512::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_GetHashCode_m69740F82E67908615101CF5DB5F466CF6BEBCB49 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_mCB73D45EDB88DC195F38E3003F7A6F73AA0688C9 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_mB47BCAF211FA20675F1E6D0C790FCEF9C60B1C7B (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_mE9705C42F0427EEEF2F7CF85CD14C84C8AD4434C (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_mDD82622AC6DA037C25CDCB2C046D023920A87C32 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_m203238E70DDAB70424C2B63EB29926D5CD2698A8 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_mC35294CA8330B87B3614BED2612F56830F4CF638 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_m37BC3A1BCC73C9AF64CD9A31E3D3D924E925F5C8 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_m8AB74F458DC82CE0026D7B902A31456914222D5A (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_m02BD5572370DCC4F0D9110E3E20827C1595CF48A (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_m390DA9D2C356C7DF2390402B98094A6533EBDA12 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt512::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_m2273812A8319ADB7E01CBF949ACA4B090982A134 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Collections.IEnumerator Unity.Collections.FixedListInt512::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt512::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_LengthInBytes_m759AAC132FCE21FB9431D3651096ED2DA7C6FC1B (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method);
// System.Byte* Unity.Collections.FixedListInt64::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_GetHashCode_m6233D06769FE02170A4C25BF9ADB7DAA1F4D0DDF (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_m0F39A6CA2CFC1E89DE789CE800882CAE817C5D8A (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m7E17FE5FC3FB44FDE44BCD4A72B6C7BACB015A25 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_m4B4FAD68DACA0C57789369B6325283B9FEB39FF6 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m76E2A940A74D24747C4DCC9B5C215C9EFA7DF648 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_mBDF6F0304435B144667FF11279F8A46D34592C14 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m12A1A77A26EBF0A0CCFA1EB0791C78C70DA08068 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_mFBD299F0A35A970B9507DC6F93020B3DD7D03EB8 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_mB5306CCBAAABD58C0C781B93A066025BD4DED9BB (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method);
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_mF6FFE011D12EFDF7D8565A75F5291BE4DEF7326B (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m6010E0A174946BBA0E3EE5DD77058282ACA09100 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method);
// System.Boolean Unity.Collections.FixedListInt64::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_mA735702468591FA565B01C107F10E86E5D2C817A (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Collections.IEnumerator Unity.Collections.FixedListInt64::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt64::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method);
// T* Unity.Collections.AllocatorManager::Allocate<Unity.Collections.LowLevel.Unsafe.UnsafeList>(Unity.Collections.AllocatorManager/AllocatorHandle,System.Int32)
inline UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * AllocatorManager_Allocate_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mB55752C04C2E0BF01026A46F7628A6A6C83A1B96 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, int32_t ___items1, const RuntimeMethod* method)
{
	return ((  UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * (*) (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 , int32_t, const RuntimeMethod*))AllocatorManager_Allocate_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mB55752C04C2E0BF01026A46F7628A6A6C83A1B96_gshared)(___handle0, ___items1, method);
}
// System.Int32 Unity.Collections.LowLevel.Unsafe.UnsafeUtility::SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeList>()
inline int32_t UnsafeUtility_SizeOf_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mF5121761EBFDF720A7C139604AFCBAA28008C170 (const RuntimeMethod* method)
{
	return ((  int32_t (*) (const RuntimeMethod*))UnsafeUtility_SizeOf_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mF5121761EBFDF720A7C139604AFCBAA28008C170_gshared)(method);
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MemClear(System.Void*,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_MemClear_m288BC0ABEB3E1A7B941FB28033D391E661887545 (void* ___destination0, int64_t ___size1, const RuntimeMethod* method);
// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager/AllocatorHandle::op_Implicit(Unity.Collections.Allocator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  AllocatorHandle_op_Implicit_m47019517CEBC57A221413D37A6950816A1BA17F4 (int32_t ___a0, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::SetCapacity(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_SetCapacity_mF763B8AEDC2E1E65FFA068955326306EDB3B22C7 (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___capacity2, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager::Free<Unity.Collections.LowLevel.Unsafe.UnsafeList>(Unity.Collections.AllocatorManager/AllocatorHandle,T*,System.Int32)
inline void AllocatorManager_Free_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mC9DEFCD77EAF09FCF9CF7C11E3C9D8233DDF9950 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * ___pointer1, int32_t ___items2, const RuntimeMethod* method)
{
	((  void (*) (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 , UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *, int32_t, const RuntimeMethod*))AllocatorManager_Free_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mC9DEFCD77EAF09FCF9CF7C11E3C9D8233DDF9950_gshared)(___handle0, ___pointer1, ___items2, method);
}
// System.Boolean Unity.Collections.CollectionHelper::ShouldDeallocate(Unity.Collections.AllocatorManager/AllocatorHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CollectionHelper_ShouldDeallocate_m05F1EA772FCA1D6A975343CEB7853C3A4F3008F8 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___allocator0, const RuntimeMethod* method);
// System.Void Unity.Collections.AllocatorManager::Free(Unity.Collections.AllocatorManager/AllocatorHandle,System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AllocatorManager_Free_mDE1594E464749B50FF597BC1080C547AE5DD7634 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, void* ___pointer1, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Clear_mE7B1A02E25F65569A525FA0D7CF2BBCB0A0F53AE (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Resize(System.Int32,System.Int32,System.Int32,Unity.Collections.NativeArrayOptions)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Resize_m07DF56383367580C079E9DE31C33866F80054CBF (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___length2, int32_t ___options3, const RuntimeMethod* method);
// System.Void* Unity.Collections.AllocatorManager::Allocate(Unity.Collections.AllocatorManager/AllocatorHandle,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, int32_t ___itemSizeInBytes1, int32_t ___alignmentInBytes2, int32_t ___items3, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility::MemCpy(System.Void*,System.Void*,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_MemCpy_mA675903DD7350CC5EC22947C0899B18944E3578C (void* ___destination0, void* ___source1, int64_t ___size2, const RuntimeMethod* method);
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Realloc(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023 (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___capacity2, const RuntimeMethod* method);
// System.Int32 Unity.Mathematics.math::ceilpow2(System.Int32)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_ceilpow2_mF2EC71F87ADC86C9B3F00E03B9B042B3DCEE89A9_inline (int32_t ___x0, const RuntimeMethod* method);
// System.Int32 Unity.Mathematics.math::lzcnt(System.UInt32)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_lzcnt_m7CAAF4F0B52359FD893C21E195915EAD7B16E373_inline (uint32_t ___x0, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void $BurstDirectCallInitializer::Initialize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U24BurstDirectCallInitializer_Initialize_mDCD2B8EE19E4E17A8E64F1F0C8075E50F422C913 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U24BurstDirectCallInitializer_Initialize_mDCD2B8EE19E4E17A8E64F1F0C8075E50F422C913_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		Try_00000AC0U24BurstDirectCall_Initialize_m9B6FD30747453E4F5D0B56D91EA3E08D9A968DD0(/*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		Try_00000ACAU24BurstDirectCall_Initialize_mE2FFC9DD8B84C7107F795C02DA5D8D08A107B1F1(/*hidden argument*/NULL);
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
// System.Void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EmbeddedAttribute__ctor_mF289C9E78E72B83E01FC51E4596FE1B5BFE6F047 (EmbeddedAttribute_t1911FF370C2DCB631528386EA2A75E72C8B94CCA * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// System.Void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IsUnmanagedAttribute__ctor_mB548EDA7DB3621DEEF9002B94179A7B582601753 (IsUnmanagedAttribute_t861EFFE3B040EF1C98B66A8008E18FD7FE360621 * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// System.Void* Unity.Collections.AllocatorManager::Allocate(Unity.Collections.AllocatorManager/AllocatorHandle,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, int32_t ___itemSizeInBytes1, int32_t ___alignmentInBytes2, int32_t ___items3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// Block block = default;
		il2cpp_codegen_initobj((&V_0), sizeof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 ));
		// block.Range.Allocator = handle;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_0 = (&V_0)->get_address_of_Range_0();
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_1 = ___handle0;
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  L_2 = SmallAllocatorHandle_op_Implicit_mBD832FEF0A1B4FC3BCC74B3EEA3FC295C015BDA2(L_1, /*hidden argument*/NULL);
		L_0->set_Allocator_2(L_2);
		// block.Range.Items = items;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_3 = (&V_0)->get_address_of_Range_0();
		int32_t L_4 = ___items3;
		L_3->set_Items_1(L_4);
		// block.Range.Pointer = IntPtr.Zero;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_5 = (&V_0)->get_address_of_Range_0();
		L_5->set_Pointer_0((intptr_t)(0));
		// block.BytesPerItem = itemSizeInBytes;
		int32_t L_6 = ___itemSizeInBytes1;
		(&V_0)->set_BytesPerItem_1(L_6);
		// block.Alignment = alignmentInBytes;
		int32_t L_7 = ___alignmentInBytes2;
		Block_set_Alignment_mE1B6FDB79DD245BDAD3344390AB275D5ADC0BF9F((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)(&V_0), L_7, /*hidden argument*/NULL);
		// var error = Try(ref block);
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		int32_t L_8 = AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)(&V_0), /*hidden argument*/NULL);
		// if (error != 0)
		if (!L_8)
		{
			goto IL_005c;
		}
	}
	{
		// throw new ArgumentException("failed to allocate");
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_9 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7(L_9, _stringLiteralEE37BE011FCAC99E8AC621D6F90BC4B75E848823, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_9, AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D_RuntimeMethod_var);
	}

IL_005c:
	{
		// return (void*)block.Range.Pointer;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  L_10 = V_0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E  L_11 = L_10.get_Range_0();
		intptr_t L_12 = L_11.get_Pointer_0();
		void* L_13 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_12, /*hidden argument*/NULL);
		return (void*)(L_13);
	}
}
// System.Void Unity.Collections.AllocatorManager::Free(Unity.Collections.AllocatorManager/AllocatorHandle,System.Void*,System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, void* ___pointer1, int32_t ___itemSizeInBytes2, int32_t ___alignmentInBytes3, int32_t ___items4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// if (pointer == null)
		void* L_0 = ___pointer1;
		if ((!(((uintptr_t)L_0) == ((uintptr_t)(((uintptr_t)0))))))
		{
			goto IL_0006;
		}
	}
	{
		// return;
		return;
	}

IL_0006:
	{
		// Block block = default;
		il2cpp_codegen_initobj((&V_0), sizeof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 ));
		// block.Range.Allocator = handle;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_1 = (&V_0)->get_address_of_Range_0();
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_2 = ___handle0;
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  L_3 = SmallAllocatorHandle_op_Implicit_mBD832FEF0A1B4FC3BCC74B3EEA3FC295C015BDA2(L_2, /*hidden argument*/NULL);
		L_1->set_Allocator_2(L_3);
		// block.Range.Items = 0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_4 = (&V_0)->get_address_of_Range_0();
		L_4->set_Items_1(0);
		// block.Range.Pointer = (IntPtr)pointer;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_5 = (&V_0)->get_address_of_Range_0();
		void* L_6 = ___pointer1;
		intptr_t L_7 = IntPtr_op_Explicit_m7F0C4B884FFB05BD231154CBDAEBCF1917019C21((void*)(void*)L_6, /*hidden argument*/NULL);
		L_5->set_Pointer_0((intptr_t)L_7);
		// block.BytesPerItem = itemSizeInBytes;
		int32_t L_8 = ___itemSizeInBytes2;
		(&V_0)->set_BytesPerItem_1(L_8);
		// block.Alignment = alignmentInBytes;
		int32_t L_9 = ___alignmentInBytes3;
		Block_set_Alignment_mE1B6FDB79DD245BDAD3344390AB275D5ADC0BF9F((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)(&V_0), L_9, /*hidden argument*/NULL);
		// var error = Try(ref block);
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		int32_t L_10 = AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)(&V_0), /*hidden argument*/NULL);
		// if (error != 0)
		if (!L_10)
		{
			goto IL_0063;
		}
	}
	{
		// throw new ArgumentException("failed to free");
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_11 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7(L_11, _stringLiteralE8AFCAC606B8AF997C2C44DB820ED0DA9DEBBAA4, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_11, AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E_RuntimeMethod_var);
	}

IL_0063:
	{
		// }
		return;
	}
}
// System.Void Unity.Collections.AllocatorManager::Free(Unity.Collections.AllocatorManager/AllocatorHandle,System.Void*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AllocatorManager_Free_mDE1594E464749B50FF597BC1080C547AE5DD7634 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___handle0, void* ___pointer1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AllocatorManager_Free_mDE1594E464749B50FF597BC1080C547AE5DD7634_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// Free(handle, pointer, 1, 1, 1);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_0 = ___handle0;
		void* L_1 = ___pointer1;
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		AllocatorManager_Free_m684E7DF11045672658EE9EADB754A41C2390697E(L_0, (void*)(void*)L_1, 1, 1, 1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Int32 Unity.Collections.AllocatorManager::TryLegacy(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AllocatorManager_TryLegacy_m8FA25D28AD957F9E1695FB2FFC9BF65338E48BCF (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AllocatorManager_TryLegacy_m8FA25D28AD957F9E1695FB2FFC9BF65338E48BCF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (block.Range.Pointer == IntPtr.Zero) // Allocate
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_0 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_1 = L_0->get_address_of_Range_0();
		intptr_t L_2 = L_1->get_Pointer_0();
		bool L_3 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_2, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0074;
		}
	}
	{
		// block.Range.Pointer =
		//     (IntPtr)UnsafeUtility.Malloc(block.Bytes, block.Alignment, (Allocator)block.Range.Allocator.Value);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_4 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_5 = L_4->get_address_of_Range_0();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_6 = ___block0;
		int64_t L_7 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_6, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_8 = ___block0;
		int32_t L_9 = Block_get_Alignment_mF11F2FB35FBF18414BD78A301DD0E19373F2BCCC((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_8, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_10 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_11 = L_10->get_address_of_Range_0();
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 * L_12 = L_11->get_address_of_Allocator_2();
		uint16_t L_13 = L_12->get_Value_0();
		void* L_14 = UnsafeUtility_Malloc_m43BC7C9BE1437A70DD9A236418B0906CD3617331(L_7, L_9, L_13, /*hidden argument*/NULL);
		intptr_t L_15 = IntPtr_op_Explicit_m7F0C4B884FFB05BD231154CBDAEBCF1917019C21((void*)(void*)L_14, /*hidden argument*/NULL);
		L_5->set_Pointer_0((intptr_t)L_15);
		// block.AllocatedItems = block.Range.Items;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_16 = ___block0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_17 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_18 = L_17->get_address_of_Range_0();
		int32_t L_19 = L_18->get_Items_1();
		L_16->set_AllocatedItems_2(L_19);
		// return (block.Range.Pointer == IntPtr.Zero) ? -1 : 0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_20 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_21 = L_20->get_address_of_Range_0();
		intptr_t L_22 = L_21->get_Pointer_0();
		bool L_23 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_22, (intptr_t)(0), /*hidden argument*/NULL);
		if (L_23)
		{
			goto IL_0072;
		}
	}
	{
		return 0;
	}

IL_0072:
	{
		return (-1);
	}

IL_0074:
	{
		// if (block.Bytes == 0) // Free
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_24 = ___block0;
		int64_t L_25 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_24, /*hidden argument*/NULL);
		if (L_25)
		{
			goto IL_00ba;
		}
	}
	{
		// UnsafeUtility.Free((void*)block.Range.Pointer, (Allocator)block.Range.Allocator.Value);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_26 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_27 = L_26->get_address_of_Range_0();
		intptr_t L_28 = L_27->get_Pointer_0();
		void* L_29 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_28, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_30 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_31 = L_30->get_address_of_Range_0();
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 * L_32 = L_31->get_address_of_Allocator_2();
		uint16_t L_33 = L_32->get_Value_0();
		UnsafeUtility_Free_mAC082BB03B10D20CA9E5AD7FBA33164DF2B52E89((void*)(void*)L_29, L_33, /*hidden argument*/NULL);
		// block.Range.Pointer = IntPtr.Zero;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_34 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_35 = L_34->get_address_of_Range_0();
		L_35->set_Pointer_0((intptr_t)(0));
		// block.AllocatedItems = 0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_36 = ___block0;
		L_36->set_AllocatedItems_2(0);
		// return 0;
		return 0;
	}

IL_00ba:
	{
		// return -1;
		return (-1);
	}
}
// System.Int32 Unity.Collections.AllocatorManager::Try(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32 (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  V_0;
	memset((&V_0), 0, sizeof(V_0));
	FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89  V_1;
	memset((&V_1), 0, sizeof(V_1));
	TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 * V_2 = NULL;
	{
		// if (block.Range.Allocator.Value < FirstUserIndex)
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_0 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_1 = L_0->get_address_of_Range_0();
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 * L_2 = L_1->get_address_of_Allocator_2();
		uint16_t L_3 = L_2->get_Value_0();
		if ((((int32_t)L_3) >= ((int32_t)((int32_t)32))))
		{
			goto IL_001b;
		}
	}
	{
		// return TryLegacy(ref block);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_4 = ___block0;
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		int32_t L_5 = AllocatorManager_TryLegacy_m8FA25D28AD957F9E1695FB2FFC9BF65338E48BCF((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_4, /*hidden argument*/NULL);
		return L_5;
	}

IL_001b:
	{
		// TableEntry tableEntry = default;
		il2cpp_codegen_initobj((&V_0), sizeof(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC ));
		IL2CPP_RUNTIME_CLASS_INIT(StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_il2cpp_TypeInfo_var);
		TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 * L_6 = SharedStatic_1_get_Data_m2585600653432550FCB33327F0EC2989C76F5DC6((SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F *)(((StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_StaticFields*)il2cpp_codegen_static_fields_for(StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_il2cpp_TypeInfo_var))->get_address_of_Ref_0()), /*hidden argument*/SharedStatic_1_get_Data_m2585600653432550FCB33327F0EC2989C76F5DC6_RuntimeMethod_var);
		V_2 = (TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 *)L_6;
		// fixed(TableEntry65536* tableEntry65536 = &StaticFunctionTable.Ref.Data)
		TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 * L_7 = V_2;
		// tableEntry = ((TableEntry*)tableEntry65536)[block.Range.Allocator.Value];
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_8 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_9 = L_8->get_address_of_Range_0();
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 * L_10 = L_9->get_address_of_Allocator_2();
		uint16_t L_11 = L_10->get_Value_0();
		uint32_t L_12 = sizeof(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC );
		TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  L_13 = (*(TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC *)((intptr_t)il2cpp_codegen_add((intptr_t)(((uintptr_t)L_7)), (intptr_t)((intptr_t)il2cpp_codegen_multiply((intptr_t)(((intptr_t)L_11)), (int32_t)L_12)))));
		V_0 = L_13;
		V_2 = (TableEntry65536_t643A5A3A00B362A1B57D3F861368D0C5EF309AB7 *)(((uintptr_t)0));
		// var function = new FunctionPointer<TryFunction>(tableEntry.function);
		TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  L_14 = V_0;
		intptr_t L_15 = L_14.get_function_0();
		FunctionPointer_1__ctor_m88698023CA5671A2E19B7EF5076962FBD71EEC45_inline((FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 *)(&V_1), (intptr_t)L_15, /*hidden argument*/FunctionPointer_1__ctor_m88698023CA5671A2E19B7EF5076962FBD71EEC45_RuntimeMethod_var);
		// return function.Invoke(tableEntry.state, ref block);
		TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * L_16 = FunctionPointer_1_get_Invoke_m031425B62699967966A4CAF4D062D1197B730E5E((FunctionPointer_1_tD38713833B5177BAC93F8BBD0915A2789DFA8B89 *)(&V_1), /*hidden argument*/FunctionPointer_1_get_Invoke_m031425B62699967966A4CAF4D062D1197B730E5E_RuntimeMethod_var);
		TableEntry_t92240D5DB96C572B8F8D1725DEE9A4666D0625AC  L_17 = V_0;
		intptr_t L_18 = L_17.get_state_1();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_19 = ___block0;
		NullCheck(L_16);
		int32_t L_20 = TryFunction_Invoke_m0C76EC4668A2F6116EC0FF3AA01B12ECEA8D4C85(L_16, (intptr_t)L_18, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_19, /*hidden argument*/NULL);
		return L_20;
	}
}
// System.Void Unity.Collections.AllocatorManager::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AllocatorManager__cctor_mCD35A58B61B05634E2E7F220CD5A1B5350A567B8 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AllocatorManager__cctor_mCD35A58B61B05634E2E7F220CD5A1B5350A567B8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static readonly AllocatorHandle Invalid = new AllocatorHandle {Value = 0};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		(&V_0)->set_Value_0(0);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_0 = V_0;
		((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->set_Invalid_0(L_0);
		// public static readonly AllocatorHandle None = new AllocatorHandle {Value = 1};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		(&V_0)->set_Value_0(1);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_1 = V_0;
		((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->set_None_1(L_1);
		// public static readonly AllocatorHandle Temp = new AllocatorHandle {Value = 2};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		(&V_0)->set_Value_0(2);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_2 = V_0;
		((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->set_Temp_2(L_2);
		// public static readonly AllocatorHandle TempJob = new AllocatorHandle {Value = 3};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		(&V_0)->set_Value_0(3);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_3 = V_0;
		((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->set_TempJob_3(L_3);
		// public static readonly AllocatorHandle Persistent = new AllocatorHandle {Value = 4};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		(&V_0)->set_Value_0(4);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_4 = V_0;
		((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->set_Persistent_4(L_4);
		// public static readonly AllocatorHandle AudioKernel = new AllocatorHandle {Value = 5};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		(&V_0)->set_Value_0(5);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_5 = V_0;
		((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->set_AudioKernel_5(L_5);
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
// Unity.Collections.AllocatorManager/AllocatorHandle Unity.Collections.AllocatorManager/AllocatorHandle::op_Implicit(Unity.Collections.Allocator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  AllocatorHandle_op_Implicit_m47019517CEBC57A221413D37A6950816A1BA17F4 (int32_t ___a0, const RuntimeMethod* method)
{
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static implicit operator AllocatorHandle(Allocator a) => new AllocatorHandle {Value = (int)a};
		il2cpp_codegen_initobj((&V_0), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		int32_t L_0 = ___a0;
		(&V_0)->set_Value_0(L_0);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_1 = V_0;
		return L_1;
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
// System.Int64 Unity.Collections.AllocatorManager/Block::get_Bytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614 (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method)
{
	{
		// public long Bytes => BytesPerItem * Range.Items;
		int32_t L_0 = __this->get_BytesPerItem_1();
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_1 = __this->get_address_of_Range_0();
		int32_t L_2 = L_1->get_Items_1();
		return (((int64_t)((int64_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)L_2)))));
	}
}
IL2CPP_EXTERN_C  int64_t Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * _thisAdjusted = reinterpret_cast<Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *>(__this + _offset);
	return Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.AllocatorManager/Block::get_Alignment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Block_get_Alignment_mF11F2FB35FBF18414BD78A301DD0E19373F2BCCC (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method)
{
	{
		// get => 1 << Log2Alignment;
		uint8_t L_0 = __this->get_Log2Alignment_3();
		return ((int32_t)((int32_t)1<<(int32_t)((int32_t)((int32_t)L_0&(int32_t)((int32_t)31)))));
	}
}
IL2CPP_EXTERN_C  int32_t Block_get_Alignment_mF11F2FB35FBF18414BD78A301DD0E19373F2BCCC_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * _thisAdjusted = reinterpret_cast<Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *>(__this + _offset);
	return Block_get_Alignment_mF11F2FB35FBF18414BD78A301DD0E19373F2BCCC(_thisAdjusted, method);
}
// System.Void Unity.Collections.AllocatorManager/Block::set_Alignment(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Block_set_Alignment_mE1B6FDB79DD245BDAD3344390AB275D5ADC0BF9F (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// set => Log2Alignment = (byte)(32 - math.lzcnt(math.max(1, value) - 1));
		int32_t L_0 = ___value0;
		int32_t L_1 = math_max_mE358BDDC8FCC6DCACBC5DAACE15C1B74CAA41CF7_inline(1, L_0, /*hidden argument*/NULL);
		int32_t L_2 = math_lzcnt_m960E448337A464EAFF1261B9F67725F97207300C_inline(((int32_t)il2cpp_codegen_subtract((int32_t)L_1, (int32_t)1)), /*hidden argument*/NULL);
		__this->set_Log2Alignment_3((uint8_t)(((int32_t)((uint8_t)((int32_t)il2cpp_codegen_subtract((int32_t)((int32_t)32), (int32_t)L_2))))));
		return;
	}
}
IL2CPP_EXTERN_C  void Block_set_Alignment_mE1B6FDB79DD245BDAD3344390AB275D5ADC0BF9F_AdjustorThunk (RuntimeObject * __this, int32_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * _thisAdjusted = reinterpret_cast<Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *>(__this + _offset);
	Block_set_Alignment_mE1B6FDB79DD245BDAD3344390AB275D5ADC0BF9F(_thisAdjusted, ___value0, method);
}
// System.Void Unity.Collections.AllocatorManager/Block::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Block_Dispose_m394AE073CC03A5812CBC2696FD28D350FE710CDF (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method)
{
	{
		// TryFree();
		Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)__this, /*hidden argument*/NULL);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void Block_Dispose_m394AE073CC03A5812CBC2696FD28D350FE710CDF_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * _thisAdjusted = reinterpret_cast<Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *>(__this + _offset);
	Block_Dispose_m394AE073CC03A5812CBC2696FD28D350FE710CDF(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.AllocatorManager/Block::TryFree()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// Range.Items = 0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_0 = __this->get_address_of_Range_0();
		L_0->set_Items_1(0);
		// return Try(ref this);
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		int32_t L_1 = AllocatorManager_Try_m0723794762505E4393FFF274B01EFFE20F643D32((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)__this, /*hidden argument*/NULL);
		return L_1;
	}
}
IL2CPP_EXTERN_C  int32_t Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * _thisAdjusted = reinterpret_cast<Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *>(__this + _offset);
	return Block_TryFree_m9A08B451A5F8215B600B9823F14EDE59CFA02F7E(_thisAdjusted, method);
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
// System.Void Unity.Collections.AllocatorManager/Range::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Range_Dispose_mB5304E6725120C599A3A4DDDC6339E6CA474CB7A (Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * __this, const RuntimeMethod* method)
{
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		// Block block = new Block { Range = this };
		il2cpp_codegen_initobj((&V_1), sizeof(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 ));
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E  L_0 = (*(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E *)__this);
		(&V_1)->set_Range_0(L_0);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  L_1 = V_1;
		V_0 = L_1;
		// block.Dispose();
		Block_Dispose_m394AE073CC03A5812CBC2696FD28D350FE710CDF((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)(&V_0), /*hidden argument*/NULL);
		// this = block.Range;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5  L_2 = V_0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E  L_3 = L_2.get_Range_0();
		*(Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E *)__this = L_3;
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void Range_Dispose_mB5304E6725120C599A3A4DDDC6339E6CA474CB7A_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * _thisAdjusted = reinterpret_cast<Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E *>(__this + _offset);
	Range_Dispose_mB5304E6725120C599A3A4DDDC6339E6CA474CB7A(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::get_SlabSizeInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05 (SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * __this, const RuntimeMethod* method)
{
	{
		// get => 1 << Log2SlabSizeInBytes;
		int32_t L_0 = __this->get_Log2SlabSizeInBytes_1();
		return ((int32_t)((int32_t)1<<(int32_t)((int32_t)((int32_t)L_0&(int32_t)((int32_t)31)))));
	}
}
IL2CPP_EXTERN_C  int32_t SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * _thisAdjusted = reinterpret_cast<SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *>(__this + _offset);
	return SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::Try(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972 (SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * V_3 = NULL;
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	int32_t V_6 = 0;
	int32_t V_7 = 0;
	{
		// if (block.Range.Pointer == IntPtr.Zero) // Allocate
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_0 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_1 = L_0->get_address_of_Range_0();
		intptr_t L_2 = L_1->get_Pointer_0();
		bool L_3 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_2, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_010d;
		}
	}
	{
		// if (block.Bytes + allocatedBytes > budgetInBytes)
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_4 = ___block0;
		int64_t L_5 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_4, /*hidden argument*/NULL);
		int64_t L_6 = __this->get_allocatedBytes_4();
		int64_t L_7 = __this->get_budgetInBytes_3();
		if ((((int64_t)((int64_t)il2cpp_codegen_add((int64_t)L_5, (int64_t)L_6))) <= ((int64_t)L_7)))
		{
			goto IL_0032;
		}
	}
	{
		// return -2; //over allocator budget
		return ((int32_t)-2);
	}

IL_0032:
	{
		// if (block.Bytes > SlabSizeInBytes)
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_8 = ___block0;
		int64_t L_9 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_8, /*hidden argument*/NULL);
		int32_t L_10 = SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05((SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)__this, /*hidden argument*/NULL);
		if ((((int64_t)L_9) <= ((int64_t)(((int64_t)((int64_t)L_10))))))
		{
			goto IL_0043;
		}
	}
	{
		// return -1;
		return (-1);
	}

IL_0043:
	{
		// for (var wordIndex = 0; wordIndex < Occupied.Length; ++wordIndex)
		V_0 = 0;
		goto IL_00fa;
	}

IL_004a:
	{
		// var word = Occupied[wordIndex];
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_11 = __this->get_address_of_Occupied_2();
		int32_t L_12 = V_0;
		int32_t L_13 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_11, L_12, /*hidden argument*/NULL);
		V_1 = L_13;
		// if (word == -1)
		int32_t L_14 = V_1;
		if ((((int32_t)L_14) == ((int32_t)(-1))))
		{
			goto IL_00f6;
		}
	}
	{
		// for (var bitIndex = 0; bitIndex < 32; ++bitIndex)
		V_2 = 0;
		goto IL_00ee;
	}

IL_0065:
	{
		// if ((word & (1 << bitIndex)) == 0)
		int32_t L_15 = V_1;
		int32_t L_16 = V_2;
		if (((int32_t)((int32_t)L_15&(int32_t)((int32_t)((int32_t)1<<(int32_t)((int32_t)((int32_t)L_16&(int32_t)((int32_t)31))))))))
		{
			goto IL_00ea;
		}
	}
	{
		// Occupied[wordIndex] |= 1 << bitIndex;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_17 = __this->get_address_of_Occupied_2();
		V_3 = (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_17;
		int32_t L_18 = V_0;
		V_4 = L_18;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_19 = V_3;
		int32_t L_20 = V_4;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_21 = V_3;
		int32_t L_22 = V_4;
		int32_t L_23 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_21, L_22, /*hidden argument*/NULL);
		int32_t L_24 = V_2;
		FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_19, L_20, ((int32_t)((int32_t)L_23|(int32_t)((int32_t)((int32_t)1<<(int32_t)((int32_t)((int32_t)L_24&(int32_t)((int32_t)31))))))), /*hidden argument*/NULL);
		// block.Range.Pointer = Storage.Range.Pointer +
		//     (int)(SlabSizeInBytes * (wordIndex * 32U + bitIndex));
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_25 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_26 = L_25->get_address_of_Range_0();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_27 = __this->get_address_of_Storage_0();
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_28 = L_27->get_address_of_Range_0();
		intptr_t L_29 = L_28->get_Pointer_0();
		int32_t L_30 = SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05((SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)__this, /*hidden argument*/NULL);
		int32_t L_31 = V_0;
		int32_t L_32 = V_2;
		intptr_t L_33 = IntPtr_op_Addition_mD815D6B36C7DFA1F89481720D3D46A6484BB9644((intptr_t)L_29, (((int32_t)((int32_t)((int64_t)il2cpp_codegen_multiply((int64_t)(((int64_t)((int64_t)L_30))), (int64_t)((int64_t)il2cpp_codegen_add((int64_t)((int64_t)il2cpp_codegen_multiply((int64_t)(((int64_t)((int64_t)L_31))), (int64_t)(((int64_t)((int64_t)((int32_t)32)))))), (int64_t)(((int64_t)((int64_t)L_32)))))))))), /*hidden argument*/NULL);
		L_26->set_Pointer_0((intptr_t)L_33);
		// block.AllocatedItems = SlabSizeInBytes / block.BytesPerItem;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_34 = ___block0;
		int32_t L_35 = SlabAllocator_get_SlabSizeInBytes_m777E42BE61C96614206E6AE0368BAE548D338E05((SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)__this, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_36 = ___block0;
		int32_t L_37 = L_36->get_BytesPerItem_1();
		L_34->set_AllocatedItems_2(((int32_t)((int32_t)L_35/(int32_t)L_37)));
		// allocatedBytes += block.Bytes;
		int64_t L_38 = __this->get_allocatedBytes_4();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_39 = ___block0;
		int64_t L_40 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_39, /*hidden argument*/NULL);
		__this->set_allocatedBytes_4(((int64_t)il2cpp_codegen_add((int64_t)L_38, (int64_t)L_40)));
		// return 0;
		return 0;
	}

IL_00ea:
	{
		// for (var bitIndex = 0; bitIndex < 32; ++bitIndex)
		int32_t L_41 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_41, (int32_t)1));
	}

IL_00ee:
	{
		// for (var bitIndex = 0; bitIndex < 32; ++bitIndex)
		int32_t L_42 = V_2;
		if ((((int32_t)L_42) < ((int32_t)((int32_t)32))))
		{
			goto IL_0065;
		}
	}

IL_00f6:
	{
		// for (var wordIndex = 0; wordIndex < Occupied.Length; ++wordIndex)
		int32_t L_43 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_43, (int32_t)1));
	}

IL_00fa:
	{
		// for (var wordIndex = 0; wordIndex < Occupied.Length; ++wordIndex)
		int32_t L_44 = V_0;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_45 = __this->get_address_of_Occupied_2();
		int32_t L_46 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_45, /*hidden argument*/NULL);
		if ((((int32_t)L_44) < ((int32_t)L_46)))
		{
			goto IL_004a;
		}
	}
	{
		// return -1;
		return (-1);
	}

IL_010d:
	{
		// if (block.Bytes == 0) // Free
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_47 = ___block0;
		int64_t L_48 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_47, /*hidden argument*/NULL);
		if (L_48)
		{
			goto IL_01b1;
		}
	}
	{
		// var slabIndex = ((ulong)block.Range.Pointer - (ulong)Storage.Range.Pointer) >>
		//     Log2SlabSizeInBytes;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_49 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_50 = L_49->get_address_of_Range_0();
		intptr_t L_51 = L_50->get_Pointer_0();
		int64_t L_52 = IntPtr_op_Explicit_m254924E8680FCCF870F18064DC0B114445B09172((intptr_t)L_51, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_53 = __this->get_address_of_Storage_0();
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_54 = L_53->get_address_of_Range_0();
		intptr_t L_55 = L_54->get_Pointer_0();
		int64_t L_56 = IntPtr_op_Explicit_m254924E8680FCCF870F18064DC0B114445B09172((intptr_t)L_55, /*hidden argument*/NULL);
		int32_t L_57 = __this->get_Log2SlabSizeInBytes_1();
		// int wordIndex = (int)(slabIndex >> 5);
		int64_t L_58 = ((int64_t)((uint64_t)((int64_t)il2cpp_codegen_subtract((int64_t)L_52, (int64_t)L_56))>>((int32_t)((int32_t)L_57&(int32_t)((int32_t)63)))));
		V_5 = (((int32_t)((int32_t)((int64_t)((uint64_t)L_58>>5)))));
		// int bitIndex = (int)(slabIndex & 31);
		V_6 = (((int32_t)((int32_t)((int64_t)((int64_t)L_58&(int64_t)(((int64_t)((int64_t)((int32_t)31)))))))));
		// Occupied[wordIndex] &= ~(1 << bitIndex);
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_59 = __this->get_address_of_Occupied_2();
		V_3 = (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_59;
		int32_t L_60 = V_5;
		V_4 = L_60;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_61 = V_3;
		int32_t L_62 = V_4;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * L_63 = V_3;
		int32_t L_64 = V_4;
		int32_t L_65 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_63, L_64, /*hidden argument*/NULL);
		int32_t L_66 = V_6;
		FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)L_61, L_62, ((int32_t)((int32_t)L_65&(int32_t)((~((int32_t)((int32_t)1<<(int32_t)((int32_t)((int32_t)L_66&(int32_t)((int32_t)31))))))))), /*hidden argument*/NULL);
		// block.Range.Pointer = IntPtr.Zero;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_67 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_68 = L_67->get_address_of_Range_0();
		L_68->set_Pointer_0((intptr_t)(0));
		// var blockSizeInBytes = block.AllocatedItems * block.BytesPerItem;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_69 = ___block0;
		int32_t L_70 = L_69->get_AllocatedItems_2();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_71 = ___block0;
		int32_t L_72 = L_71->get_BytesPerItem_1();
		V_7 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_70, (int32_t)L_72));
		// allocatedBytes -= blockSizeInBytes;
		int64_t L_73 = __this->get_allocatedBytes_4();
		int32_t L_74 = V_7;
		__this->set_allocatedBytes_4(((int64_t)il2cpp_codegen_subtract((int64_t)L_73, (int64_t)(((int64_t)((int64_t)L_74))))));
		// block.AllocatedItems = 0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_75 = ___block0;
		L_75->set_AllocatedItems_2(0);
		// return 0;
		return 0;
	}

IL_01b1:
	{
		// return -1;
		return (-1);
	}
}
IL2CPP_EXTERN_C  int32_t SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972_AdjustorThunk (RuntimeObject * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * _thisAdjusted = reinterpret_cast<SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *>(__this + _offset);
	return SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972(_thisAdjusted, ___block0, method);
}
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::Try(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SlabAllocator_Try_mC1CF8ACD30D0745D62FB9E07BF900AD0743FA85F (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SlabAllocator_Try_mC1CF8ACD30D0745D62FB9E07BF900AD0743FA85F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return ((SlabAllocator*)allocatorState)->Try(ref block);
		intptr_t L_0 = ___allocatorState0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_1 = ___block1;
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		int32_t L_2 = Try_00000ACAU24BurstDirectCall_Invoke_mEBE8EE6CA0E3E1822B42856658237A0ABF3DD68B((intptr_t)L_0, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
// System.Void Unity.Collections.AllocatorManager/SlabAllocator::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SlabAllocator_Dispose_m0B657EF865140D5BB55C843B61D8E8F3A2CBB005 (SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * __this, const RuntimeMethod* method)
{
	{
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void SlabAllocator_Dispose_m0B657EF865140D5BB55C843B61D8E8F3A2CBB005_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C * _thisAdjusted = reinterpret_cast<SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *>(__this + _offset);
	SlabAllocator_Dispose_m0B657EF865140D5BB55C843B61D8E8F3A2CBB005(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator::Try$BurstManaged(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SlabAllocator_TryU24BurstManaged_m0E85623C6B44B7C3B6A5829BF73EFF1BD5A1E8B1 (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	{
		// return ((SlabAllocator*)allocatorState)->Try(ref block);
		intptr_t L_0 = ___allocatorState0;
		void* L_1 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_0, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_2 = ___block1;
		int32_t L_3 = SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972((SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)L_1, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_2, /*hidden argument*/NULL);
		return L_3;
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
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::GetFunctionPointerDiscard(System.IntPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall_GetFunctionPointerDiscard_m6D57CFDB0E90300242AD61433E9B2E9E161AC1A9 (intptr_t* p0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000ACAU24BurstDirectCall_GetFunctionPointerDiscard_m6D57CFDB0E90300242AD61433E9B2E9E161AC1A9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		intptr_t L_0 = ((Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var))->get_Pointer_0();
		if (L_0)
		{
			goto IL_0023;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		intptr_t L_1 = ((Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var))->get_DeferredCompilation_1();
		RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F  L_2 = { reinterpret_cast<intptr_t> (SlabAllocator_TryU24BurstManaged_m0E85623C6B44B7C3B6A5829BF73EFF1BD5A1E8B1_RuntimeMethod_var) };
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_3 = { reinterpret_cast<intptr_t> (Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var);
		void* L_4 = BurstCompiler_GetILPPMethodFunctionPointer2_mC5481172A163C21818ED26A7263F024D6A7752BE((intptr_t)L_1, L_2, L_3, /*hidden argument*/NULL);
		((Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var))->set_Pointer_0((intptr_t)L_4);
	}

IL_0023:
	{
		intptr_t* L_5 = p0;
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		intptr_t L_6 = ((Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var))->get_Pointer_0();
		*((intptr_t*)L_5) = (intptr_t)L_6;
		return;
	}
}
// System.IntPtr Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::GetFunctionPointer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Try_00000ACAU24BurstDirectCall_GetFunctionPointer_mE960ACE388298074ADD1CEF79DD565A04158BFAD (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000ACAU24BurstDirectCall_GetFunctionPointer_mE960ACE388298074ADD1CEF79DD565A04158BFAD_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		V_0 = (intptr_t)(((intptr_t)0));
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		Try_00000ACAU24BurstDirectCall_GetFunctionPointerDiscard_m6D57CFDB0E90300242AD61433E9B2E9E161AC1A9((intptr_t*)(&V_0), /*hidden argument*/NULL);
		intptr_t L_0 = V_0;
		return (intptr_t)L_0;
	}
}
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Constructor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall_Constructor_m505A404645A774A1EECA51E6288C6426BD5FAE00 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000ACAU24BurstDirectCall_Constructor_m505A404645A774A1EECA51E6288C6426BD5FAE00_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F  L_0 = { reinterpret_cast<intptr_t> (SlabAllocator_Try_mC1CF8ACD30D0745D62FB9E07BF900AD0743FA85F_RuntimeMethod_var) };
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var);
		intptr_t L_1 = BurstCompiler_CompileILPPMethod2_mCE18C77E36D7BB2CF708E02DAB88BAECE602E29A(L_0, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		((Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var))->set_DeferredCompilation_1((intptr_t)L_1);
		return;
	}
}
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Initialize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall_Initialize_mE2FFC9DD8B84C7107F795C02DA5D8D08A107B1F1 (const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24BurstDirectCall__cctor_m0098E5A97F708EB39C7C696E4ECE9DD177B51F88 (const RuntimeMethod* method)
{
	{
		Try_00000ACAU24BurstDirectCall_Constructor_m505A404645A774A1EECA51E6288C6426BD5FAE00(/*hidden argument*/NULL);
		return;
	}
}
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$BurstDirectCall::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000ACAU24BurstDirectCall_Invoke_mEBE8EE6CA0E3E1822B42856658237A0ABF3DD68B (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000ACAU24BurstDirectCall_Invoke_mEBE8EE6CA0E3E1822B42856658237A0ABF3DD68B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var);
		bool L_0 = BurstCompiler_get_IsEnabled_m05F933051E525210A3A999C9EA671AF9C51312F0(/*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_001f;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000ACAU24BurstDirectCall_t2B00F8093521F038BE4E9A9469DAE307EE5134BB_il2cpp_TypeInfo_var);
		intptr_t L_1 = Try_00000ACAU24BurstDirectCall_GetFunctionPointer_mE960ACE388298074ADD1CEF79DD565A04158BFAD(/*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		intptr_t L_2 = V_0;
		if (!L_2)
		{
			goto IL_001f;
		}
	}
	{
		intptr_t L_3 = ___allocatorState0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_4 = ___block1;
		intptr_t L_5 = V_0;
		typedef int32_t (CDECL *func_6F8319169202531ECA6DD54202F19EF6BF0B6A1F)(intptr_t,Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *);
		int32_t L_6 = ((func_6F8319169202531ECA6DD54202F19EF6BF0B6A1F)L_5)((intptr_t)L_3,(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_4);
		return L_6;
	}

IL_001f:
	{
		intptr_t L_7 = ___allocatorState0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_8 = ___block1;
		int32_t L_9 = SlabAllocator_TryU24BurstManaged_m0E85623C6B44B7C3B6A5829BF73EFF1BD5A1E8B1_inline((intptr_t)L_7, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_8, /*hidden argument*/NULL);
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
IL2CPP_EXTERN_C  int32_t DelegatePInvokeWrapper_Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46 (Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(((RuntimeDelegate*)__this)->method->nativeFunction);

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___allocatorState0, ___block1);

	return returnValue;
}
// System.Void Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$PostfixBurstDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000ACAU24PostfixBurstDelegate__ctor_m39EA43B564C1C3DACF59316BBC17FBD7AC3D92EC (Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46 * __this, RuntimeObject * p0, intptr_t p1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)p1));
	__this->set_method_3(p1);
	__this->set_m_target_2(p0);
}
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$PostfixBurstDelegate::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000ACAU24PostfixBurstDelegate_Invoke_m60EF19DA453893B62B0E1A86571E82B7DC94899E (Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	int32_t result = 0;
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 2)
			{
				// open
				typedef int32_t (*FunctionPointerType) (intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___allocatorState0, ___block1, targetMethod);
			}
			else
			{
				// closed
				typedef int32_t (*FunctionPointerType) (void*, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___allocatorState0, ___block1, targetMethod);
			}
		}
		else
		{
			// closed
			if (targetThis != NULL && il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(targetMethod, targetThis, ___allocatorState0, ___block1);
					else
						result = GenericVirtFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(targetMethod, targetThis, ___allocatorState0, ___block1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___allocatorState0, ___block1);
					else
						result = VirtFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___allocatorState0, ___block1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef int32_t (*FunctionPointerType) (RuntimeObject*, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(&___allocatorState0) - 1), ___block1, targetMethod);
				}
				typedef int32_t (*FunctionPointerType) (void*, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___allocatorState0, ___block1, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$PostfixBurstDelegate::BeginInvoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Try_00000ACAU24PostfixBurstDelegate_BeginInvoke_mB854D30E6267FE5EB11D82B1DEDB46CBB8005ADF (Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * p2, RuntimeObject * p3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000ACAU24PostfixBurstDelegate_BeginInvoke_mB854D30E6267FE5EB11D82B1DEDB46CBB8005ADF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = Box(IntPtr_t_il2cpp_TypeInfo_var, &___allocatorState0);
	__d_args[1] = Box(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5_il2cpp_TypeInfo_var, &*___block1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)p2, (RuntimeObject*)p3);
}
// System.Int32 Unity.Collections.AllocatorManager/SlabAllocator/Try_00000ACA$PostfixBurstDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000ACAU24PostfixBurstDelegate_EndInvoke_mE09116946048ADEC00425E8E9AC39DBEAA80956C (Try_00000ACAU24PostfixBurstDelegate_t06EC73D8F8942489407964B1C4A4513DD0BECA46 * __this, RuntimeObject* p0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) p0, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Unity.Collections.AllocatorManager/SmallAllocatorHandle Unity.Collections.AllocatorManager/SmallAllocatorHandle::op_Implicit(Unity.Collections.AllocatorManager/AllocatorHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  SmallAllocatorHandle_op_Implicit_mBD832FEF0A1B4FC3BCC74B3EEA3FC295C015BDA2 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___a0, const RuntimeMethod* method)
{
	SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// public static implicit operator SmallAllocatorHandle(AllocatorHandle a) => new SmallAllocatorHandle {Value = (ushort)a.Value};
		il2cpp_codegen_initobj((&V_0), sizeof(SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9 ));
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_0 = ___a0;
		int32_t L_1 = L_0.get_Value_0();
		(&V_0)->set_Value_0((uint16_t)(((int32_t)((uint16_t)L_1))));
		SmallAllocatorHandle_tDA2EDAD8CBA8DD06DA48589D68FEA7ABA36D12B9  L_2 = V_0;
		return L_2;
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
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator::Try(Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9 (StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// if (block.Range.Pointer == IntPtr.Zero) // Allocate
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_0 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_1 = L_0->get_address_of_Range_0();
		intptr_t L_2 = L_1->get_Pointer_0();
		bool L_3 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_2, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_009c;
		}
	}
	{
		// if (m_top + block.Bytes > m_storage.Bytes)
		int64_t L_4 = __this->get_m_top_1();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_5 = ___block0;
		int64_t L_6 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_5, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_7 = __this->get_address_of_m_storage_0();
		int64_t L_8 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_7, /*hidden argument*/NULL);
		if ((((int64_t)((int64_t)il2cpp_codegen_add((int64_t)L_4, (int64_t)L_6))) <= ((int64_t)L_8)))
		{
			goto IL_0036;
		}
	}
	{
		// return -1;
		return (-1);
	}

IL_0036:
	{
		// block.Range.Pointer = (IntPtr)((byte*)m_storage.Range.Pointer + m_top);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_9 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_10 = L_9->get_address_of_Range_0();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_11 = __this->get_address_of_m_storage_0();
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_12 = L_11->get_address_of_Range_0();
		intptr_t L_13 = L_12->get_Pointer_0();
		void* L_14 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_13, /*hidden argument*/NULL);
		int64_t L_15 = __this->get_m_top_1();
		intptr_t L_16 = IntPtr_op_Explicit_m7F0C4B884FFB05BD231154CBDAEBCF1917019C21((void*)(void*)((void*)il2cpp_codegen_add((intptr_t)L_14, (intptr_t)(((intptr_t)L_15)))), /*hidden argument*/NULL);
		L_10->set_Pointer_0((intptr_t)L_16);
		// block.AllocatedItems = block.Range.Items;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_17 = ___block0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_18 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_19 = L_18->get_address_of_Range_0();
		int32_t L_20 = L_19->get_Items_1();
		L_17->set_AllocatedItems_2(L_20);
		// allocatedBytes += block.Bytes;
		int64_t L_21 = __this->get_allocatedBytes_3();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_22 = ___block0;
		int64_t L_23 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_22, /*hidden argument*/NULL);
		__this->set_allocatedBytes_3(((int64_t)il2cpp_codegen_add((int64_t)L_21, (int64_t)L_23)));
		// m_top += block.Bytes;
		int64_t L_24 = __this->get_m_top_1();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_25 = ___block0;
		int64_t L_26 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_25, /*hidden argument*/NULL);
		__this->set_m_top_1(((int64_t)il2cpp_codegen_add((int64_t)L_24, (int64_t)L_26)));
		// return 0;
		return 0;
	}

IL_009c:
	{
		// if (block.Bytes == 0) // Free
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_27 = ___block0;
		int64_t L_28 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_27, /*hidden argument*/NULL);
		if (L_28)
		{
			goto IL_012a;
		}
	}
	{
		// if ((byte*)block.Range.Pointer - (byte*)m_storage.Range.Pointer == (long)(m_top - block.Bytes))
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_29 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_30 = L_29->get_address_of_Range_0();
		intptr_t L_31 = L_30->get_Pointer_0();
		void* L_32 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_31, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_33 = __this->get_address_of_m_storage_0();
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_34 = L_33->get_address_of_Range_0();
		intptr_t L_35 = L_34->get_Pointer_0();
		void* L_36 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_35, /*hidden argument*/NULL);
		int64_t L_37 = __this->get_m_top_1();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_38 = ___block0;
		int64_t L_39 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_38, /*hidden argument*/NULL);
		if ((!(((uint64_t)(((int64_t)((int64_t)(intptr_t)((void*)((intptr_t)((void*)il2cpp_codegen_subtract((intptr_t)L_32, (intptr_t)L_36))/(int32_t)1)))))) == ((uint64_t)((int64_t)il2cpp_codegen_subtract((int64_t)L_37, (int64_t)L_39))))))
		{
			goto IL_0128;
		}
	}
	{
		// m_top -= block.Bytes;
		int64_t L_40 = __this->get_m_top_1();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_41 = ___block0;
		int64_t L_42 = Block_get_Bytes_m4692690E8A4756DD9694FDE08D66C93196EE4614((Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_41, /*hidden argument*/NULL);
		__this->set_m_top_1(((int64_t)il2cpp_codegen_subtract((int64_t)L_40, (int64_t)L_42)));
		// var blockSizeInBytes = block.AllocatedItems * block.BytesPerItem;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_43 = ___block0;
		int32_t L_44 = L_43->get_AllocatedItems_2();
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_45 = ___block0;
		int32_t L_46 = L_45->get_BytesPerItem_1();
		V_0 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_44, (int32_t)L_46));
		// allocatedBytes -= blockSizeInBytes;
		int64_t L_47 = __this->get_allocatedBytes_3();
		int32_t L_48 = V_0;
		__this->set_allocatedBytes_3(((int64_t)il2cpp_codegen_subtract((int64_t)L_47, (int64_t)(((int64_t)((int64_t)L_48))))));
		// block.Range.Pointer = IntPtr.Zero;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_49 = ___block0;
		Range_t69B30CB796A0947F2B7ED79A569A0FF535B2912E * L_50 = L_49->get_address_of_Range_0();
		L_50->set_Pointer_0((intptr_t)(0));
		// block.AllocatedItems = 0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_51 = ___block0;
		L_51->set_AllocatedItems_2(0);
		// return 0;
		return 0;
	}

IL_0128:
	{
		// return -1;
		return (-1);
	}

IL_012a:
	{
		// return -1;
		return (-1);
	}
}
IL2CPP_EXTERN_C  int32_t StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9_AdjustorThunk (RuntimeObject * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 * _thisAdjusted = reinterpret_cast<StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 *>(__this + _offset);
	return StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9(_thisAdjusted, ___block0, method);
}
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator::Try(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StackAllocator_Try_mB50327267831F16523E96A7E7FF4829A2EE4C60E (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (StackAllocator_Try_mB50327267831F16523E96A7E7FF4829A2EE4C60E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return ((StackAllocator*)allocatorState)->Try(ref block);
		intptr_t L_0 = ___allocatorState0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_1 = ___block1;
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		int32_t L_2 = Try_00000AC0U24BurstDirectCall_Invoke_m06D304524A473E4768EC248F5F9853E51FF43DD5((intptr_t)L_0, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
// System.Void Unity.Collections.AllocatorManager/StackAllocator::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StackAllocator_Dispose_m17928A2A471E508BD80B87B6A0D3BEAD07E66200 (StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 * __this, const RuntimeMethod* method)
{
	{
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void StackAllocator_Dispose_m17928A2A471E508BD80B87B6A0D3BEAD07E66200_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 * _thisAdjusted = reinterpret_cast<StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 *>(__this + _offset);
	StackAllocator_Dispose_m17928A2A471E508BD80B87B6A0D3BEAD07E66200(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator::Try$BurstManaged(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StackAllocator_TryU24BurstManaged_m46B078D3E5C2608D24398E4A9B1AA71F352F3FBE (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	{
		// return ((StackAllocator*)allocatorState)->Try(ref block);
		intptr_t L_0 = ___allocatorState0;
		void* L_1 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_0, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_2 = ___block1;
		int32_t L_3 = StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9((StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 *)(StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 *)L_1, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_2, /*hidden argument*/NULL);
		return L_3;
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
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::GetFunctionPointerDiscard(System.IntPtr&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall_GetFunctionPointerDiscard_m8840DE8BB24CFF03BAE3663B68B0BBB0B7A9950A (intptr_t* p0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000AC0U24BurstDirectCall_GetFunctionPointerDiscard_m8840DE8BB24CFF03BAE3663B68B0BBB0B7A9950A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		intptr_t L_0 = ((Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var))->get_Pointer_0();
		if (L_0)
		{
			goto IL_0023;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		intptr_t L_1 = ((Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var))->get_DeferredCompilation_1();
		RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F  L_2 = { reinterpret_cast<intptr_t> (StackAllocator_TryU24BurstManaged_m46B078D3E5C2608D24398E4A9B1AA71F352F3FBE_RuntimeMethod_var) };
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_3 = { reinterpret_cast<intptr_t> (Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var);
		void* L_4 = BurstCompiler_GetILPPMethodFunctionPointer2_mC5481172A163C21818ED26A7263F024D6A7752BE((intptr_t)L_1, L_2, L_3, /*hidden argument*/NULL);
		((Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var))->set_Pointer_0((intptr_t)L_4);
	}

IL_0023:
	{
		intptr_t* L_5 = p0;
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		intptr_t L_6 = ((Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var))->get_Pointer_0();
		*((intptr_t*)L_5) = (intptr_t)L_6;
		return;
	}
}
// System.IntPtr Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::GetFunctionPointer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Try_00000AC0U24BurstDirectCall_GetFunctionPointer_mF5ACDA3A6A948DD3EDBDD62EBA0807FAA1B70CC6 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000AC0U24BurstDirectCall_GetFunctionPointer_mF5ACDA3A6A948DD3EDBDD62EBA0807FAA1B70CC6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		V_0 = (intptr_t)(((intptr_t)0));
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		Try_00000AC0U24BurstDirectCall_GetFunctionPointerDiscard_m8840DE8BB24CFF03BAE3663B68B0BBB0B7A9950A((intptr_t*)(&V_0), /*hidden argument*/NULL);
		intptr_t L_0 = V_0;
		return (intptr_t)L_0;
	}
}
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Constructor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall_Constructor_m0D678E2482010B83B9C308B93E82FF5251FAC73A (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000AC0U24BurstDirectCall_Constructor_m0D678E2482010B83B9C308B93E82FF5251FAC73A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		RuntimeMethodHandle_t85058E06EFF8AE085FAB91CE2B9E28E7F6FAE33F  L_0 = { reinterpret_cast<intptr_t> (StackAllocator_Try_mB50327267831F16523E96A7E7FF4829A2EE4C60E_RuntimeMethod_var) };
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var);
		intptr_t L_1 = BurstCompiler_CompileILPPMethod2_mCE18C77E36D7BB2CF708E02DAB88BAECE602E29A(L_0, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		((Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_StaticFields*)il2cpp_codegen_static_fields_for(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var))->set_DeferredCompilation_1((intptr_t)L_1);
		return;
	}
}
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Initialize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall_Initialize_m9B6FD30747453E4F5D0B56D91EA3E08D9A968DD0 (const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24BurstDirectCall__cctor_mAB34687A0750293F3E5599DDCD653A8E84D1D5F5 (const RuntimeMethod* method)
{
	{
		Try_00000AC0U24BurstDirectCall_Constructor_m0D678E2482010B83B9C308B93E82FF5251FAC73A(/*hidden argument*/NULL);
		return;
	}
}
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$BurstDirectCall::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000AC0U24BurstDirectCall_Invoke_m06D304524A473E4768EC248F5F9853E51FF43DD5 (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000AC0U24BurstDirectCall_Invoke_m06D304524A473E4768EC248F5F9853E51FF43DD5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		IL2CPP_RUNTIME_CLASS_INIT(BurstCompiler_t0062A3F5AF87415C5FB2913A5DEC058CE790CD56_il2cpp_TypeInfo_var);
		bool L_0 = BurstCompiler_get_IsEnabled_m05F933051E525210A3A999C9EA671AF9C51312F0(/*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_001f;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Try_00000AC0U24BurstDirectCall_t7C5F0AA92B361835F3A6579E597884ADD7153BD0_il2cpp_TypeInfo_var);
		intptr_t L_1 = Try_00000AC0U24BurstDirectCall_GetFunctionPointer_mF5ACDA3A6A948DD3EDBDD62EBA0807FAA1B70CC6(/*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		intptr_t L_2 = V_0;
		if (!L_2)
		{
			goto IL_001f;
		}
	}
	{
		intptr_t L_3 = ___allocatorState0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_4 = ___block1;
		intptr_t L_5 = V_0;
		typedef int32_t (CDECL *func_6F8319169202531ECA6DD54202F19EF6BF0B6A1F)(intptr_t,Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *);
		int32_t L_6 = ((func_6F8319169202531ECA6DD54202F19EF6BF0B6A1F)L_5)((intptr_t)L_3,(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_4);
		return L_6;
	}

IL_001f:
	{
		intptr_t L_7 = ___allocatorState0;
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_8 = ___block1;
		int32_t L_9 = StackAllocator_TryU24BurstManaged_m46B078D3E5C2608D24398E4A9B1AA71F352F3FBE_inline((intptr_t)L_7, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_8, /*hidden argument*/NULL);
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
IL2CPP_EXTERN_C  int32_t DelegatePInvokeWrapper_Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197 (Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(((RuntimeDelegate*)__this)->method->nativeFunction);

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___allocatorState0, ___block1);

	return returnValue;
}
// System.Void Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$PostfixBurstDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Try_00000AC0U24PostfixBurstDelegate__ctor_mD898C917837B3799DFAE1BCA1953E7710C37B440 (Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197 * __this, RuntimeObject * p0, intptr_t p1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)p1));
	__this->set_method_3(p1);
	__this->set_m_target_2(p0);
}
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$PostfixBurstDelegate::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000AC0U24PostfixBurstDelegate_Invoke_m78D9A999CD01C531D649B5A22856DC4664156DE3 (Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	int32_t result = 0;
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 2)
			{
				// open
				typedef int32_t (*FunctionPointerType) (intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___allocatorState0, ___block1, targetMethod);
			}
			else
			{
				// closed
				typedef int32_t (*FunctionPointerType) (void*, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___allocatorState0, ___block1, targetMethod);
			}
		}
		else
		{
			// closed
			if (targetThis != NULL && il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(targetMethod, targetThis, ___allocatorState0, ___block1);
					else
						result = GenericVirtFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(targetMethod, targetThis, ___allocatorState0, ___block1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___allocatorState0, ___block1);
					else
						result = VirtFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___allocatorState0, ___block1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef int32_t (*FunctionPointerType) (RuntimeObject*, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(&___allocatorState0) - 1), ___block1, targetMethod);
				}
				typedef int32_t (*FunctionPointerType) (void*, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___allocatorState0, ___block1, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$PostfixBurstDelegate::BeginInvoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Try_00000AC0U24PostfixBurstDelegate_BeginInvoke_m6A5938036CCF6C398B498030D083B9F8E2ED9A0F (Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * p2, RuntimeObject * p3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Try_00000AC0U24PostfixBurstDelegate_BeginInvoke_m6A5938036CCF6C398B498030D083B9F8E2ED9A0F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = Box(IntPtr_t_il2cpp_TypeInfo_var, &___allocatorState0);
	__d_args[1] = Box(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5_il2cpp_TypeInfo_var, &*___block1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)p2, (RuntimeObject*)p3);
}
// System.Int32 Unity.Collections.AllocatorManager/StackAllocator/Try_00000AC0$PostfixBurstDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Try_00000AC0U24PostfixBurstDelegate_EndInvoke_mCCF220F5767586993081B024E129F3C67F5AB0D3 (Try_00000AC0U24PostfixBurstDelegate_t31EADC098744DAB41B775D78B50E4590305B4197 * __this, RuntimeObject* p0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) p0, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Collections.AllocatorManager/StaticFunctionTable::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StaticFunctionTable__cctor_m3D2D05C6947655D0683241ACD39F9017756DE0B1 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (StaticFunctionTable__cctor_m3D2D05C6947655D0683241ACD39F9017756DE0B1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static readonly SharedStatic<TableEntry65536> Ref =
		//     SharedStatic<TableEntry65536>.GetOrCreate<StaticFunctionTable>();
		SharedStatic_1_tC72860C7BC0D0B90EA965B5B769434038F41FA9F  L_0 = SharedStatic_1_GetOrCreateUnsafe_m175F8547F0CF4DBA988A8BC79AF6F443590B595A(0, ((int64_t)8299449114801376948LL), ((int64_t)0LL), /*hidden argument*/SharedStatic_1_GetOrCreateUnsafe_m175F8547F0CF4DBA988A8BC79AF6F443590B595A_RuntimeMethod_var);
		((StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_StaticFields*)il2cpp_codegen_static_fields_for(StaticFunctionTable_t2287D3309E31A2A73AA19528C043C38EAB4175D5_il2cpp_TypeInfo_var))->set_Ref_0(L_0);
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
IL2CPP_EXTERN_C  int32_t DelegatePInvokeWrapper_TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 (TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	typedef int32_t (CDECL *PInvokeFunc)(intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(((RuntimeDelegate*)__this)->method->nativeFunction);

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___allocatorState0, ___block1);

	return returnValue;
}
// System.Void Unity.Collections.AllocatorManager/TryFunction::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TryFunction__ctor_mA9FDC769454CB60A90C296E8A068CA6682F8AF29 (TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Int32 Unity.Collections.AllocatorManager/TryFunction::Invoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TryFunction_Invoke_m0C76EC4668A2F6116EC0FF3AA01B12ECEA8D4C85 (TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	int32_t result = 0;
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 2)
			{
				// open
				typedef int32_t (*FunctionPointerType) (intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___allocatorState0, ___block1, targetMethod);
			}
			else
			{
				// closed
				typedef int32_t (*FunctionPointerType) (void*, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___allocatorState0, ___block1, targetMethod);
			}
		}
		else
		{
			// closed
			if (targetThis != NULL && il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(targetMethod, targetThis, ___allocatorState0, ___block1);
					else
						result = GenericVirtFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(targetMethod, targetThis, ___allocatorState0, ___block1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___allocatorState0, ___block1);
					else
						result = VirtFuncInvoker2< int32_t, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___allocatorState0, ___block1);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef int32_t (*FunctionPointerType) (RuntimeObject*, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(&___allocatorState0) - 1), ___block1, targetMethod);
				}
				typedef int32_t (*FunctionPointerType) (void*, intptr_t, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___allocatorState0, ___block1, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult Unity.Collections.AllocatorManager/TryFunction::BeginInvoke(System.IntPtr,Unity.Collections.AllocatorManager/Block&,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TryFunction_BeginInvoke_m154FD78AF32570B8B1EA1B807864BA46C5B7C873 (TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * __this, intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TryFunction_BeginInvoke_m154FD78AF32570B8B1EA1B807864BA46C5B7C873_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = Box(IntPtr_t_il2cpp_TypeInfo_var, &___allocatorState0);
	__d_args[1] = Box(Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5_il2cpp_TypeInfo_var, &*___block1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Int32 Unity.Collections.AllocatorManager/TryFunction::EndInvoke(Unity.Collections.AllocatorManager/Block&,System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TryFunction_EndInvoke_mCE190E8FD89A25A42202A9D21502F2EC29B6DD81 (TryFunction_t3B5A0EC09C638B4FFA6FCE03FB9439722950C268 * __this, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block0, RuntimeObject* ___result1, const RuntimeMethod* method)
{
	void* ___out_args[] = {
	___block0,
	};
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result1, ___out_args);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.UInt32 Unity.Collections.CollectionHelper::Hash(System.Void*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517 (void* ___pointer0, int32_t ___bytes1, const RuntimeMethod* method)
{
	uint8_t* V_0 = NULL;
	uint64_t V_1 = 0;
	uint64_t V_2 = 0;
	{
		// byte* str = (byte*)pointer;
		void* L_0 = ___pointer0;
		V_0 = (uint8_t*)L_0;
		// ulong hash = 5381;
		V_1 = (((int64_t)((int64_t)((int32_t)5381))));
		goto IL_001e;
	}

IL_000b:
	{
		// ulong c = str[--bytes];
		uint8_t* L_1 = V_0;
		int32_t L_2 = ___bytes1;
		int32_t L_3 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_2, (int32_t)1));
		___bytes1 = L_3;
		int32_t L_4 = *((uint8_t*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_1, (int32_t)L_3)));
		V_2 = (((int64_t)((uint64_t)(((uint32_t)((uint32_t)L_4))))));
		// hash = ((hash << 5) + hash) + c;
		uint64_t L_5 = V_1;
		uint64_t L_6 = V_1;
		uint64_t L_7 = V_2;
		V_1 = ((int64_t)il2cpp_codegen_add((int64_t)((int64_t)il2cpp_codegen_add((int64_t)((int64_t)((int64_t)L_5<<(int32_t)5)), (int64_t)L_6)), (int64_t)L_7));
	}

IL_001e:
	{
		// while (bytes > 0)
		int32_t L_8 = ___bytes1;
		if ((((int32_t)L_8) > ((int32_t)0)))
		{
			goto IL_000b;
		}
	}
	{
		// return (uint)hash;
		uint64_t L_9 = V_1;
		return (((int32_t)((uint32_t)L_9)));
	}
}
// System.Boolean Unity.Collections.CollectionHelper::ShouldDeallocate(Unity.Collections.AllocatorManager/AllocatorHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CollectionHelper_ShouldDeallocate_m05F1EA772FCA1D6A975343CEB7853C3A4F3008F8 (AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  ___allocator0, const RuntimeMethod* method)
{
	{
		// return allocator.Value > (int)Allocator.None;
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_0 = ___allocator0;
		int32_t L_1 = L_0.get_Value_0();
		return (bool)((((int32_t)L_1) > ((int32_t)1))? 1 : 0);
	}
}
// System.Int32 Unity.Collections.CollectionHelper::AssumePositive(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t CollectionHelper_AssumePositive_mFEF267CFFB1A952D25D241C237EFC5D1F3053CEE (int32_t ___x0, const RuntimeMethod* method)
{
	{
		// return x;
		int32_t L_0 = ___x0;
		return L_0;
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
// System.Int32 Unity.Collections.FixedListInt128::get_Length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt128::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_LengthInBytes_m0325EE4F0B2509330EE1C98963454B480BE5A2D4 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	{
		// internal int LengthInBytes => Length * sizeof(int);
		int32_t L_0 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		return ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)4));
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_get_LengthInBytes_m0325EE4F0B2509330EE1C98963454B480BE5A2D4_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_get_LengthInBytes_m0325EE4F0B2509330EE1C98963454B480BE5A2D4(_thisAdjusted, method);
}
// System.Byte* Unity.Collections.FixedListInt128::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	{
		// {
		FixedBytes126_tEA89B8D667C198C10D2170A6891770484E86C6C1 * L_0 = __this->get_address_of_buffer_1();
		FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * L_1 = L_0->get_address_of_offset0000_0();
		uint8_t* L_2 = L_1->get_address_of_byte0000_0();
		V_0 = (uint8_t*)L_2;
		// fixed(byte* b = &buffer.offset0000.byte0000)
		uint8_t* L_3 = V_0;
		// return b + FixedList.PaddingBytes<int>();
		int32_t L_4 = FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37(/*hidden argument*/FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_RuntimeMethod_var);
		return (uint8_t*)(((intptr_t)il2cpp_codegen_add((intptr_t)(((uintptr_t)L_3)), (int32_t)L_4)));
	}
}
IL2CPP_EXTERN_C  uint8_t* FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt128::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return UnsafeUtility.ReadArrayElement<int>(Buffer, index);
		uint8_t* L_0 = FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___index0;
		int32_t L_2 = UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D((void*)(void*)L_0, L_1, /*hidden argument*/UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_RuntimeMethod_var);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D_AdjustorThunk (RuntimeObject * __this, int32_t ___index0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D(_thisAdjusted, ___index0, method);
}
// System.Int32 Unity.Collections.FixedListInt128::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_GetHashCode_m36E604637F0B9E956D38D4F8DBADFF2414C0BABA (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	{
		// return (int)CollectionHelper.Hash(Buffer, LengthInBytes);
		uint8_t* L_0 = FixedListInt128_get_Buffer_m6EBC6E0FBFFA84AD197A2AEF3A01EF9EF38BEED0((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt128_get_LengthInBytes_m0325EE4F0B2509330EE1C98963454B480BE5A2D4((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		uint32_t L_2 = CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517((void*)(void*)L_0, L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_GetHashCode_m36E604637F0B9E956D38D4F8DBADFF2414C0BABA_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_GetHashCode_m36E604637F0B9E956D38D4F8DBADFF2414C0BABA(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_mC94C48315E2CC254EDB192D63E8A3417E152185F (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_CompareTo_mC94C48315E2CC254EDB192D63E8A3417E152185F_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_CompareTo_mC94C48315E2CC254EDB192D63E8A3417E152185F(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_m1F712C0AB7D83AAB0856EFF896116E8EFE32E791 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_0 = ___other0;
		int32_t L_1 = FixedListInt128_CompareTo_mC94C48315E2CC254EDB192D63E8A3417E152185F((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt128_Equals_m1F712C0AB7D83AAB0856EFF896116E8EFE32E791_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_Equals_m1F712C0AB7D83AAB0856EFF896116E8EFE32E791(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_mB5EC715871DA565C70D863DF706822F7E067FC18 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_CompareTo_mB5EC715871DA565C70D863DF706822F7E067FC18_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_CompareTo_mB5EC715871DA565C70D863DF706822F7E067FC18(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_m1F0902DC7ADCED7E17DB901B2A116E6E2A977224 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_0 = ___other0;
		int32_t L_1 = FixedListInt128_CompareTo_mB5EC715871DA565C70D863DF706822F7E067FC18((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt128_Equals_m1F0902DC7ADCED7E17DB901B2A116E6E2A977224_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_Equals_m1F0902DC7ADCED7E17DB901B2A116E6E2A977224(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_m8319D1AA0407ED664000E714FBAFF32F94A9613A (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_CompareTo_m8319D1AA0407ED664000E714FBAFF32F94A9613A_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_CompareTo_m8319D1AA0407ED664000E714FBAFF32F94A9613A(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_m0BDB5A185692A45140F6289D6D8D651F2E561082 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_0 = ___other0;
		int32_t L_1 = FixedListInt128_CompareTo_m8319D1AA0407ED664000E714FBAFF32F94A9613A((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt128_Equals_m0BDB5A185692A45140F6289D6D8D651F2E561082_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_Equals_m0BDB5A185692A45140F6289D6D8D651F2E561082(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_m8D2466754C727E9F22BB83BA76E780B1CF462FE2 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_CompareTo_m8D2466754C727E9F22BB83BA76E780B1CF462FE2_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_CompareTo_m8D2466754C727E9F22BB83BA76E780B1CF462FE2(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_mE569B14EEF1C0E45EDDADB2793CBD843679BB1B7 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_0 = ___other0;
		int32_t L_1 = FixedListInt128_CompareTo_m8D2466754C727E9F22BB83BA76E780B1CF462FE2((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt128_Equals_mE569B14EEF1C0E45EDDADB2793CBD843679BB1B7_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_Equals_mE569B14EEF1C0E45EDDADB2793CBD843679BB1B7(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt128::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt128_CompareTo_mEA7A950BEC9588B430C806BD527E05433EBA89AE (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt128_CompareTo_mEA7A950BEC9588B430C806BD527E05433EBA89AE_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_CompareTo_mEA7A950BEC9588B430C806BD527E05433EBA89AE(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt128::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_mBD792DFEAE2F89DA165E52BECAA2B91C7205A91D (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_0 = ___other0;
		int32_t L_1 = FixedListInt128_CompareTo_mEA7A950BEC9588B430C806BD527E05433EBA89AE((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt128_Equals_mBD792DFEAE2F89DA165E52BECAA2B91C7205A91D_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_Equals_mBD792DFEAE2F89DA165E52BECAA2B91C7205A91D(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt128::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt128_Equals_mB686B28CC8F05B63BB4246D93B00ECB2AC62D31C (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt128_Equals_mB686B28CC8F05B63BB4246D93B00ECB2AC62D31C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  V_0;
	memset((&V_0), 0, sizeof(V_0));
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  V_1;
	memset((&V_1), 0, sizeof(V_1));
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  V_3;
	memset((&V_3), 0, sizeof(V_3));
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  V_4;
	memset((&V_4), 0, sizeof(V_4));
	RuntimeObject * V_5 = NULL;
	{
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		RuntimeObject * L_0 = ___obj0;
		RuntimeObject * L_1 = L_0;
		V_5 = L_1;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_1, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var)))
		{
			goto IL_001b;
		}
	}
	{
		RuntimeObject * L_2 = V_5;
		V_0 = ((*(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)UnBox(L_2, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_3 = V_0;
		bool L_4 = FixedListInt128_Equals_m1F712C0AB7D83AAB0856EFF896116E8EFE32E791((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_3, /*hidden argument*/NULL);
		return L_4;
	}

IL_001b:
	{
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		RuntimeObject * L_5 = ___obj0;
		RuntimeObject * L_6 = L_5;
		V_5 = L_6;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_6, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var)))
		{
			goto IL_0036;
		}
	}
	{
		RuntimeObject * L_7 = V_5;
		V_1 = ((*(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)UnBox(L_7, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_8 = V_1;
		bool L_9 = FixedListInt128_Equals_m1F0902DC7ADCED7E17DB901B2A116E6E2A977224((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_8, /*hidden argument*/NULL);
		return L_9;
	}

IL_0036:
	{
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		RuntimeObject * L_10 = ___obj0;
		RuntimeObject * L_11 = L_10;
		V_5 = L_11;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_11, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var)))
		{
			goto IL_0051;
		}
	}
	{
		RuntimeObject * L_12 = V_5;
		V_2 = ((*(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)UnBox(L_12, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_13 = V_2;
		bool L_14 = FixedListInt128_Equals_m0BDB5A185692A45140F6289D6D8D651F2E561082((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_13, /*hidden argument*/NULL);
		return L_14;
	}

IL_0051:
	{
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		RuntimeObject * L_15 = ___obj0;
		RuntimeObject * L_16 = L_15;
		V_5 = L_16;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_16, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var)))
		{
			goto IL_006c;
		}
	}
	{
		RuntimeObject * L_17 = V_5;
		V_3 = ((*(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)UnBox(L_17, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_18 = V_3;
		bool L_19 = FixedListInt128_Equals_mE569B14EEF1C0E45EDDADB2793CBD843679BB1B7((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_18, /*hidden argument*/NULL);
		return L_19;
	}

IL_006c:
	{
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		RuntimeObject * L_20 = ___obj0;
		RuntimeObject * L_21 = L_20;
		V_5 = L_21;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_21, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var)))
		{
			goto IL_0089;
		}
	}
	{
		RuntimeObject * L_22 = V_5;
		V_4 = ((*(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)UnBox(L_22, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_23 = V_4;
		bool L_24 = FixedListInt128_Equals_mBD792DFEAE2F89DA165E52BECAA2B91C7205A91D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)__this, L_23, /*hidden argument*/NULL);
		return L_24;
	}

IL_0089:
	{
		// return false;
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool FixedListInt128_Equals_mB686B28CC8F05B63BB4246D93B00ECB2AC62D31C_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_Equals_mB686B28CC8F05B63BB4246D93B00ECB2AC62D31C(_thisAdjusted, ___obj0, method);
}
// System.Collections.IEnumerator Unity.Collections.FixedListInt128::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_System_Collections_IEnumerable_GetEnumerator_mFCEE1D4F4EE76938E1F0770B6EFE56887DA85902(_thisAdjusted, method);
}
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt128::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287 (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator<int> IEnumerable<int>.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * _thisAdjusted = reinterpret_cast<FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *>(__this + _offset);
	return FixedListInt128_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m6FBEC486984866BC4F2BCBF3E9D6444445543287(_thisAdjusted, method);
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
// System.Int32 Unity.Collections.FixedListInt32::get_Length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt32::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_LengthInBytes_m350F216ED1E5D3ADE5F8CB01E4C09481E79F5FCE (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	{
		// internal int LengthInBytes => Length * sizeof(int);
		int32_t L_0 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		return ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)4));
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_get_LengthInBytes_m350F216ED1E5D3ADE5F8CB01E4C09481E79F5FCE_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_get_LengthInBytes_m350F216ED1E5D3ADE5F8CB01E4C09481E79F5FCE(_thisAdjusted, method);
}
// System.Byte* Unity.Collections.FixedListInt32::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	{
		// {
		FixedBytes30_t2A39D899268C0892EDD7CA47CF09B06A3C56AD28 * L_0 = __this->get_address_of_buffer_1();
		FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * L_1 = L_0->get_address_of_offset0000_0();
		uint8_t* L_2 = L_1->get_address_of_byte0000_0();
		V_0 = (uint8_t*)L_2;
		// fixed(byte* b = &buffer.offset0000.byte0000)
		uint8_t* L_3 = V_0;
		// return b + FixedList.PaddingBytes<int>();
		int32_t L_4 = FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37(/*hidden argument*/FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_RuntimeMethod_var);
		return (uint8_t*)(((intptr_t)il2cpp_codegen_add((intptr_t)(((uintptr_t)L_3)), (int32_t)L_4)));
	}
}
IL2CPP_EXTERN_C  uint8_t* FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt32::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, int32_t ___index0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return UnsafeUtility.ReadArrayElement<int>(Buffer, index);
		uint8_t* L_0 = FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___index0;
		int32_t L_2 = UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D((void*)(void*)L_0, L_1, /*hidden argument*/UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_RuntimeMethod_var);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC_AdjustorThunk (RuntimeObject * __this, int32_t ___index0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC(_thisAdjusted, ___index0, method);
}
// System.Int32 Unity.Collections.FixedListInt32::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_GetHashCode_mED71044DA76AF56781CCF3AED232CE2040B8B2BE (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	{
		// return (int)CollectionHelper.Hash(Buffer, LengthInBytes);
		uint8_t* L_0 = FixedListInt32_get_Buffer_m21BA5DAEE5BE63CCA46037F8C499291A8D4D72AF((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt32_get_LengthInBytes_m350F216ED1E5D3ADE5F8CB01E4C09481E79F5FCE((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		uint32_t L_2 = CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517((void*)(void*)L_0, L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_GetHashCode_mED71044DA76AF56781CCF3AED232CE2040B8B2BE_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_GetHashCode_mED71044DA76AF56781CCF3AED232CE2040B8B2BE(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m051AB0C39B761522764D3B79CAFD01925FE2273A (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_CompareTo_m051AB0C39B761522764D3B79CAFD01925FE2273A_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_CompareTo_m051AB0C39B761522764D3B79CAFD01925FE2273A(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m051CADA0101FDD7BAB849FEE9062FE3BCBDFDDCB (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_0 = ___other0;
		int32_t L_1 = FixedListInt32_CompareTo_m051AB0C39B761522764D3B79CAFD01925FE2273A((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt32_Equals_m051CADA0101FDD7BAB849FEE9062FE3BCBDFDDCB_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_Equals_m051CADA0101FDD7BAB849FEE9062FE3BCBDFDDCB(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m5B87A003649B50172375B6CCC01DA9E6307D7C79 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_CompareTo_m5B87A003649B50172375B6CCC01DA9E6307D7C79_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_CompareTo_m5B87A003649B50172375B6CCC01DA9E6307D7C79(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m46F5DCFB90A27FBBE79C7A17921FAF983294C7F4 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_0 = ___other0;
		int32_t L_1 = FixedListInt32_CompareTo_m5B87A003649B50172375B6CCC01DA9E6307D7C79((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt32_Equals_m46F5DCFB90A27FBBE79C7A17921FAF983294C7F4_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_Equals_m46F5DCFB90A27FBBE79C7A17921FAF983294C7F4(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_mE56D3F480B6D084B3649359CF4682322BF1E3288 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_CompareTo_mE56D3F480B6D084B3649359CF4682322BF1E3288_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_CompareTo_mE56D3F480B6D084B3649359CF4682322BF1E3288(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m9CBEBCFB378CDAF7902EB87F8367D026B7EFE301 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_0 = ___other0;
		int32_t L_1 = FixedListInt32_CompareTo_mE56D3F480B6D084B3649359CF4682322BF1E3288((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt32_Equals_m9CBEBCFB378CDAF7902EB87F8367D026B7EFE301_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_Equals_m9CBEBCFB378CDAF7902EB87F8367D026B7EFE301(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m7F67FA3C982D2822AF2C2B80DEAF6ED125318873 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_CompareTo_m7F67FA3C982D2822AF2C2B80DEAF6ED125318873_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_CompareTo_m7F67FA3C982D2822AF2C2B80DEAF6ED125318873(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_mC3EFEC6CAB5E1EEC0C7644152B345639A2F7BBA8 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_0 = ___other0;
		int32_t L_1 = FixedListInt32_CompareTo_m7F67FA3C982D2822AF2C2B80DEAF6ED125318873((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt32_Equals_mC3EFEC6CAB5E1EEC0C7644152B345639A2F7BBA8_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_Equals_mC3EFEC6CAB5E1EEC0C7644152B345639A2F7BBA8(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt32::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt32_CompareTo_m68AC22237D7F90E44D3FD6F2FC4CD352C2EB8F2B (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt32_CompareTo_m68AC22237D7F90E44D3FD6F2FC4CD352C2EB8F2B_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_CompareTo_m68AC22237D7F90E44D3FD6F2FC4CD352C2EB8F2B(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt32::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m6C846A2E3D739C136BFFBD7CE1ED24370620C506 (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_0 = ___other0;
		int32_t L_1 = FixedListInt32_CompareTo_m68AC22237D7F90E44D3FD6F2FC4CD352C2EB8F2B((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt32_Equals_m6C846A2E3D739C136BFFBD7CE1ED24370620C506_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_Equals_m6C846A2E3D739C136BFFBD7CE1ED24370620C506(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt32::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt32_Equals_m57F5A272F557C2BC73A44A7FAF2C80AFC572AECE (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt32_Equals_m57F5A272F557C2BC73A44A7FAF2C80AFC572AECE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  V_0;
	memset((&V_0), 0, sizeof(V_0));
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  V_1;
	memset((&V_1), 0, sizeof(V_1));
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  V_3;
	memset((&V_3), 0, sizeof(V_3));
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  V_4;
	memset((&V_4), 0, sizeof(V_4));
	RuntimeObject * V_5 = NULL;
	{
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		RuntimeObject * L_0 = ___obj0;
		RuntimeObject * L_1 = L_0;
		V_5 = L_1;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_1, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var)))
		{
			goto IL_001b;
		}
	}
	{
		RuntimeObject * L_2 = V_5;
		V_0 = ((*(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)UnBox(L_2, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_3 = V_0;
		bool L_4 = FixedListInt32_Equals_m051CADA0101FDD7BAB849FEE9062FE3BCBDFDDCB((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_3, /*hidden argument*/NULL);
		return L_4;
	}

IL_001b:
	{
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		RuntimeObject * L_5 = ___obj0;
		RuntimeObject * L_6 = L_5;
		V_5 = L_6;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_6, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var)))
		{
			goto IL_0036;
		}
	}
	{
		RuntimeObject * L_7 = V_5;
		V_1 = ((*(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)UnBox(L_7, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_8 = V_1;
		bool L_9 = FixedListInt32_Equals_m46F5DCFB90A27FBBE79C7A17921FAF983294C7F4((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_8, /*hidden argument*/NULL);
		return L_9;
	}

IL_0036:
	{
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		RuntimeObject * L_10 = ___obj0;
		RuntimeObject * L_11 = L_10;
		V_5 = L_11;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_11, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var)))
		{
			goto IL_0051;
		}
	}
	{
		RuntimeObject * L_12 = V_5;
		V_2 = ((*(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)UnBox(L_12, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_13 = V_2;
		bool L_14 = FixedListInt32_Equals_m9CBEBCFB378CDAF7902EB87F8367D026B7EFE301((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_13, /*hidden argument*/NULL);
		return L_14;
	}

IL_0051:
	{
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		RuntimeObject * L_15 = ___obj0;
		RuntimeObject * L_16 = L_15;
		V_5 = L_16;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_16, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var)))
		{
			goto IL_006c;
		}
	}
	{
		RuntimeObject * L_17 = V_5;
		V_3 = ((*(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)UnBox(L_17, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_18 = V_3;
		bool L_19 = FixedListInt32_Equals_mC3EFEC6CAB5E1EEC0C7644152B345639A2F7BBA8((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_18, /*hidden argument*/NULL);
		return L_19;
	}

IL_006c:
	{
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		RuntimeObject * L_20 = ___obj0;
		RuntimeObject * L_21 = L_20;
		V_5 = L_21;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_21, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var)))
		{
			goto IL_0089;
		}
	}
	{
		RuntimeObject * L_22 = V_5;
		V_4 = ((*(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)UnBox(L_22, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_23 = V_4;
		bool L_24 = FixedListInt32_Equals_m6C846A2E3D739C136BFFBD7CE1ED24370620C506((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)__this, L_23, /*hidden argument*/NULL);
		return L_24;
	}

IL_0089:
	{
		// return false;
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool FixedListInt32_Equals_m57F5A272F557C2BC73A44A7FAF2C80AFC572AECE_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_Equals_m57F5A272F557C2BC73A44A7FAF2C80AFC572AECE(_thisAdjusted, ___obj0, method);
}
// System.Collections.IEnumerator Unity.Collections.FixedListInt32::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_System_Collections_IEnumerable_GetEnumerator_mF0A938D811BB7A259E9AD69F4BDD4B03916D97FA(_thisAdjusted, method);
}
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt32::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator<int> IEnumerable<int>.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * _thisAdjusted = reinterpret_cast<FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *>(__this + _offset);
	return FixedListInt32_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m1FB748B570FA33406750428DB52A4D94D501EF3E(_thisAdjusted, method);
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
// System.Int32 Unity.Collections.FixedListInt4096::get_Length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_LengthInBytes_m48A47979BDC97B2126C21C219739D2DF50B5FFC7 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	{
		// internal int LengthInBytes => Length * sizeof(int);
		int32_t L_0 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		return ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)4));
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_get_LengthInBytes_m48A47979BDC97B2126C21C219739D2DF50B5FFC7_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_get_LengthInBytes_m48A47979BDC97B2126C21C219739D2DF50B5FFC7(_thisAdjusted, method);
}
// System.Byte* Unity.Collections.FixedListInt4096::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	{
		// {
		FixedBytes4094_t39445938F4E21DF89ADB1D2B30A161FBB061AF70 * L_0 = __this->get_address_of_buffer_1();
		FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * L_1 = L_0->get_address_of_offset0000_0();
		uint8_t* L_2 = L_1->get_address_of_byte0000_0();
		V_0 = (uint8_t*)L_2;
		// fixed(byte* b = &buffer.offset0000.byte0000)
		uint8_t* L_3 = V_0;
		// return b + FixedList.PaddingBytes<int>();
		int32_t L_4 = FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37(/*hidden argument*/FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_RuntimeMethod_var);
		return (uint8_t*)(((intptr_t)il2cpp_codegen_add((intptr_t)(((uintptr_t)L_3)), (int32_t)L_4)));
	}
}
IL2CPP_EXTERN_C  uint8_t* FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return UnsafeUtility.ReadArrayElement<int>(Buffer, index);
		uint8_t* L_0 = FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___index0;
		int32_t L_2 = UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D((void*)(void*)L_0, L_1, /*hidden argument*/UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_RuntimeMethod_var);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F_AdjustorThunk (RuntimeObject * __this, int32_t ___index0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F(_thisAdjusted, ___index0, method);
}
// System.Void Unity.Collections.FixedListInt4096::set_Item(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, int32_t ___index0, int32_t ___value1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// UnsafeUtility.WriteArrayElement<int>(Buffer, index, value);
		uint8_t* L_0 = FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___index0;
		int32_t L_2 = ___value1;
		UnsafeUtility_WriteArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m21A03DD8050619772A9117BE97EDD6CF543115EA((void*)(void*)L_0, L_1, L_2, /*hidden argument*/UnsafeUtility_WriteArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m21A03DD8050619772A9117BE97EDD6CF543115EA_RuntimeMethod_var);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9_AdjustorThunk (RuntimeObject * __this, int32_t ___index0, int32_t ___value1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	FixedListInt4096_set_Item_m0389D5972A14B4F13233C0212D892880685610A9(_thisAdjusted, ___index0, ___value1, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_GetHashCode_m9920DEC070046A6D1F0B5CCF079BD51FD632307B (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	{
		// return (int)CollectionHelper.Hash(Buffer, LengthInBytes);
		uint8_t* L_0 = FixedListInt4096_get_Buffer_m682DC7AB0F8CD85400116F540DF1229EDCA4BB75((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt4096_get_LengthInBytes_m48A47979BDC97B2126C21C219739D2DF50B5FFC7((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		uint32_t L_2 = CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517((void*)(void*)L_0, L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_GetHashCode_m9920DEC070046A6D1F0B5CCF079BD51FD632307B_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_GetHashCode_m9920DEC070046A6D1F0B5CCF079BD51FD632307B(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m5701F729EB47E4EDE6F47177E7978EE26681DBD0 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_CompareTo_m5701F729EB47E4EDE6F47177E7978EE26681DBD0_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_CompareTo_m5701F729EB47E4EDE6F47177E7978EE26681DBD0(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m83774C41A568471A05ED2B4F594C4289C8755274 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_0 = ___other0;
		int32_t L_1 = FixedListInt4096_CompareTo_m5701F729EB47E4EDE6F47177E7978EE26681DBD0((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt4096_Equals_m83774C41A568471A05ED2B4F594C4289C8755274_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_Equals_m83774C41A568471A05ED2B4F594C4289C8755274(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m46F1FEACC4C6AA7AA943EB9488E903F6A774A50C (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_CompareTo_m46F1FEACC4C6AA7AA943EB9488E903F6A774A50C_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_CompareTo_m46F1FEACC4C6AA7AA943EB9488E903F6A774A50C(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m0782219FD99F97CC3DB3A4FA4A1F56372D48251F (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_0 = ___other0;
		int32_t L_1 = FixedListInt4096_CompareTo_m46F1FEACC4C6AA7AA943EB9488E903F6A774A50C((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt4096_Equals_m0782219FD99F97CC3DB3A4FA4A1F56372D48251F_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_Equals_m0782219FD99F97CC3DB3A4FA4A1F56372D48251F(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m06DE10845994FDF3AA3D122CBB4E8D4363ABC9F3 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_CompareTo_m06DE10845994FDF3AA3D122CBB4E8D4363ABC9F3_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_CompareTo_m06DE10845994FDF3AA3D122CBB4E8D4363ABC9F3(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m6C6F5FC73BC597D40DC88B8915AD8AB14470586D (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_0 = ___other0;
		int32_t L_1 = FixedListInt4096_CompareTo_m06DE10845994FDF3AA3D122CBB4E8D4363ABC9F3((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt4096_Equals_m6C6F5FC73BC597D40DC88B8915AD8AB14470586D_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_Equals_m6C6F5FC73BC597D40DC88B8915AD8AB14470586D(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m7936D17714F132E656F1CFFB2008959537CD16FC (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_CompareTo_m7936D17714F132E656F1CFFB2008959537CD16FC_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_CompareTo_m7936D17714F132E656F1CFFB2008959537CD16FC(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m526D0CC2C90CC4573747FDD42884EE803275ECF8 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_0 = ___other0;
		int32_t L_1 = FixedListInt4096_CompareTo_m7936D17714F132E656F1CFFB2008959537CD16FC((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt4096_Equals_m526D0CC2C90CC4573747FDD42884EE803275ECF8_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_Equals_m526D0CC2C90CC4573747FDD42884EE803275ECF8(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt4096::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt4096_CompareTo_m92DC90E0318186C8EE0110CF63F850FEF138C513 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt4096_CompareTo_m92DC90E0318186C8EE0110CF63F850FEF138C513_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_CompareTo_m92DC90E0318186C8EE0110CF63F850FEF138C513(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt4096::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m1792C55B4ACB6D4C8C2B68E820845AB0B9B1E705 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_0 = ___other0;
		int32_t L_1 = FixedListInt4096_CompareTo_m92DC90E0318186C8EE0110CF63F850FEF138C513((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt4096_Equals_m1792C55B4ACB6D4C8C2B68E820845AB0B9B1E705_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_Equals_m1792C55B4ACB6D4C8C2B68E820845AB0B9B1E705(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt4096::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt4096_Equals_m6425DE1D251DE6D5988FFE46AEBEA141B33F08A7 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt4096_Equals_m6425DE1D251DE6D5988FFE46AEBEA141B33F08A7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  V_0;
	memset((&V_0), 0, sizeof(V_0));
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  V_1;
	memset((&V_1), 0, sizeof(V_1));
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  V_3;
	memset((&V_3), 0, sizeof(V_3));
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  V_4;
	memset((&V_4), 0, sizeof(V_4));
	RuntimeObject * V_5 = NULL;
	{
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		RuntimeObject * L_0 = ___obj0;
		RuntimeObject * L_1 = L_0;
		V_5 = L_1;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_1, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var)))
		{
			goto IL_001b;
		}
	}
	{
		RuntimeObject * L_2 = V_5;
		V_0 = ((*(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)UnBox(L_2, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_3 = V_0;
		bool L_4 = FixedListInt4096_Equals_m83774C41A568471A05ED2B4F594C4289C8755274((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_3, /*hidden argument*/NULL);
		return L_4;
	}

IL_001b:
	{
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		RuntimeObject * L_5 = ___obj0;
		RuntimeObject * L_6 = L_5;
		V_5 = L_6;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_6, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var)))
		{
			goto IL_0036;
		}
	}
	{
		RuntimeObject * L_7 = V_5;
		V_1 = ((*(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)UnBox(L_7, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_8 = V_1;
		bool L_9 = FixedListInt4096_Equals_m0782219FD99F97CC3DB3A4FA4A1F56372D48251F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_8, /*hidden argument*/NULL);
		return L_9;
	}

IL_0036:
	{
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		RuntimeObject * L_10 = ___obj0;
		RuntimeObject * L_11 = L_10;
		V_5 = L_11;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_11, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var)))
		{
			goto IL_0051;
		}
	}
	{
		RuntimeObject * L_12 = V_5;
		V_2 = ((*(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)UnBox(L_12, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_13 = V_2;
		bool L_14 = FixedListInt4096_Equals_m6C6F5FC73BC597D40DC88B8915AD8AB14470586D((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_13, /*hidden argument*/NULL);
		return L_14;
	}

IL_0051:
	{
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		RuntimeObject * L_15 = ___obj0;
		RuntimeObject * L_16 = L_15;
		V_5 = L_16;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_16, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var)))
		{
			goto IL_006c;
		}
	}
	{
		RuntimeObject * L_17 = V_5;
		V_3 = ((*(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)UnBox(L_17, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_18 = V_3;
		bool L_19 = FixedListInt4096_Equals_m526D0CC2C90CC4573747FDD42884EE803275ECF8((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_18, /*hidden argument*/NULL);
		return L_19;
	}

IL_006c:
	{
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		RuntimeObject * L_20 = ___obj0;
		RuntimeObject * L_21 = L_20;
		V_5 = L_21;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_21, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var)))
		{
			goto IL_0089;
		}
	}
	{
		RuntimeObject * L_22 = V_5;
		V_4 = ((*(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)UnBox(L_22, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_23 = V_4;
		bool L_24 = FixedListInt4096_Equals_m1792C55B4ACB6D4C8C2B68E820845AB0B9B1E705((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)__this, L_23, /*hidden argument*/NULL);
		return L_24;
	}

IL_0089:
	{
		// return false;
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool FixedListInt4096_Equals_m6425DE1D251DE6D5988FFE46AEBEA141B33F08A7_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_Equals_m6425DE1D251DE6D5988FFE46AEBEA141B33F08A7(_thisAdjusted, ___obj0, method);
}
// System.Collections.IEnumerator Unity.Collections.FixedListInt4096::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64 (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_System_Collections_IEnumerable_GetEnumerator_mF0B5802A4EC07253680A9EC5EFA62F0FDBB52C64(_thisAdjusted, method);
}
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt4096::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator<int> IEnumerable<int>.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * _thisAdjusted = reinterpret_cast<FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *>(__this + _offset);
	return FixedListInt4096_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mA338299D668DE7A8F57F77256A699AD89EAE1C6C(_thisAdjusted, method);
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
// System.Int32 Unity.Collections.FixedListInt512::get_Length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt512::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_LengthInBytes_mD882B0A7E9F1A9F5008DB6491D0771ED70660FC9 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	{
		// internal int LengthInBytes => Length * sizeof(int);
		int32_t L_0 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		return ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)4));
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_get_LengthInBytes_mD882B0A7E9F1A9F5008DB6491D0771ED70660FC9_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_get_LengthInBytes_mD882B0A7E9F1A9F5008DB6491D0771ED70660FC9(_thisAdjusted, method);
}
// System.Byte* Unity.Collections.FixedListInt512::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	{
		// {
		FixedBytes510_t716ECE82DC31933DA23274CBDA9CDCC483C58A0C * L_0 = __this->get_address_of_buffer_1();
		FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * L_1 = L_0->get_address_of_offset0000_0();
		uint8_t* L_2 = L_1->get_address_of_byte0000_0();
		V_0 = (uint8_t*)L_2;
		// fixed(byte* b = &buffer.offset0000.byte0000)
		uint8_t* L_3 = V_0;
		// return b + FixedList.PaddingBytes<int>();
		int32_t L_4 = FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37(/*hidden argument*/FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_RuntimeMethod_var);
		return (uint8_t*)(((intptr_t)il2cpp_codegen_add((intptr_t)(((uintptr_t)L_3)), (int32_t)L_4)));
	}
}
IL2CPP_EXTERN_C  uint8_t* FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt512::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return UnsafeUtility.ReadArrayElement<int>(Buffer, index);
		uint8_t* L_0 = FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___index0;
		int32_t L_2 = UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D((void*)(void*)L_0, L_1, /*hidden argument*/UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_RuntimeMethod_var);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448_AdjustorThunk (RuntimeObject * __this, int32_t ___index0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448(_thisAdjusted, ___index0, method);
}
// System.Int32 Unity.Collections.FixedListInt512::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_GetHashCode_m69740F82E67908615101CF5DB5F466CF6BEBCB49 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	{
		// return (int)CollectionHelper.Hash(Buffer, LengthInBytes);
		uint8_t* L_0 = FixedListInt512_get_Buffer_mA1BA057C7CF014EF28E537C6EF2166AF1076A0F6((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt512_get_LengthInBytes_mD882B0A7E9F1A9F5008DB6491D0771ED70660FC9((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		uint32_t L_2 = CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517((void*)(void*)L_0, L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_GetHashCode_m69740F82E67908615101CF5DB5F466CF6BEBCB49_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_GetHashCode_m69740F82E67908615101CF5DB5F466CF6BEBCB49(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_mCB73D45EDB88DC195F38E3003F7A6F73AA0688C9 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_CompareTo_mCB73D45EDB88DC195F38E3003F7A6F73AA0688C9_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_CompareTo_mCB73D45EDB88DC195F38E3003F7A6F73AA0688C9(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_mB47BCAF211FA20675F1E6D0C790FCEF9C60B1C7B (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_0 = ___other0;
		int32_t L_1 = FixedListInt512_CompareTo_mCB73D45EDB88DC195F38E3003F7A6F73AA0688C9((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt512_Equals_mB47BCAF211FA20675F1E6D0C790FCEF9C60B1C7B_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_Equals_mB47BCAF211FA20675F1E6D0C790FCEF9C60B1C7B(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_mE9705C42F0427EEEF2F7CF85CD14C84C8AD4434C (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_CompareTo_mE9705C42F0427EEEF2F7CF85CD14C84C8AD4434C_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_CompareTo_mE9705C42F0427EEEF2F7CF85CD14C84C8AD4434C(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_mDD82622AC6DA037C25CDCB2C046D023920A87C32 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_0 = ___other0;
		int32_t L_1 = FixedListInt512_CompareTo_mE9705C42F0427EEEF2F7CF85CD14C84C8AD4434C((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt512_Equals_mDD82622AC6DA037C25CDCB2C046D023920A87C32_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_Equals_mDD82622AC6DA037C25CDCB2C046D023920A87C32(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_m203238E70DDAB70424C2B63EB29926D5CD2698A8 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_CompareTo_m203238E70DDAB70424C2B63EB29926D5CD2698A8_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_CompareTo_m203238E70DDAB70424C2B63EB29926D5CD2698A8(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_mC35294CA8330B87B3614BED2612F56830F4CF638 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_0 = ___other0;
		int32_t L_1 = FixedListInt512_CompareTo_m203238E70DDAB70424C2B63EB29926D5CD2698A8((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt512_Equals_mC35294CA8330B87B3614BED2612F56830F4CF638_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_Equals_mC35294CA8330B87B3614BED2612F56830F4CF638(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_m37BC3A1BCC73C9AF64CD9A31E3D3D924E925F5C8 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_CompareTo_m37BC3A1BCC73C9AF64CD9A31E3D3D924E925F5C8_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_CompareTo_m37BC3A1BCC73C9AF64CD9A31E3D3D924E925F5C8(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_m8AB74F458DC82CE0026D7B902A31456914222D5A (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_0 = ___other0;
		int32_t L_1 = FixedListInt512_CompareTo_m37BC3A1BCC73C9AF64CD9A31E3D3D924E925F5C8((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt512_Equals_m8AB74F458DC82CE0026D7B902A31456914222D5A_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_Equals_m8AB74F458DC82CE0026D7B902A31456914222D5A(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt512::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt512_CompareTo_m02BD5572370DCC4F0D9110E3E20827C1595CF48A (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt512_CompareTo_m02BD5572370DCC4F0D9110E3E20827C1595CF48A_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_CompareTo_m02BD5572370DCC4F0D9110E3E20827C1595CF48A(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt512::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_m390DA9D2C356C7DF2390402B98094A6533EBDA12 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_0 = ___other0;
		int32_t L_1 = FixedListInt512_CompareTo_m02BD5572370DCC4F0D9110E3E20827C1595CF48A((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt512_Equals_m390DA9D2C356C7DF2390402B98094A6533EBDA12_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_Equals_m390DA9D2C356C7DF2390402B98094A6533EBDA12(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt512::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt512_Equals_m2273812A8319ADB7E01CBF949ACA4B090982A134 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt512_Equals_m2273812A8319ADB7E01CBF949ACA4B090982A134_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  V_0;
	memset((&V_0), 0, sizeof(V_0));
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  V_1;
	memset((&V_1), 0, sizeof(V_1));
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  V_3;
	memset((&V_3), 0, sizeof(V_3));
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  V_4;
	memset((&V_4), 0, sizeof(V_4));
	RuntimeObject * V_5 = NULL;
	{
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		RuntimeObject * L_0 = ___obj0;
		RuntimeObject * L_1 = L_0;
		V_5 = L_1;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_1, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var)))
		{
			goto IL_001b;
		}
	}
	{
		RuntimeObject * L_2 = V_5;
		V_0 = ((*(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)UnBox(L_2, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_3 = V_0;
		bool L_4 = FixedListInt512_Equals_mB47BCAF211FA20675F1E6D0C790FCEF9C60B1C7B((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_3, /*hidden argument*/NULL);
		return L_4;
	}

IL_001b:
	{
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		RuntimeObject * L_5 = ___obj0;
		RuntimeObject * L_6 = L_5;
		V_5 = L_6;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_6, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var)))
		{
			goto IL_0036;
		}
	}
	{
		RuntimeObject * L_7 = V_5;
		V_1 = ((*(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)UnBox(L_7, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_8 = V_1;
		bool L_9 = FixedListInt512_Equals_mDD82622AC6DA037C25CDCB2C046D023920A87C32((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_8, /*hidden argument*/NULL);
		return L_9;
	}

IL_0036:
	{
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		RuntimeObject * L_10 = ___obj0;
		RuntimeObject * L_11 = L_10;
		V_5 = L_11;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_11, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var)))
		{
			goto IL_0051;
		}
	}
	{
		RuntimeObject * L_12 = V_5;
		V_2 = ((*(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)UnBox(L_12, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_13 = V_2;
		bool L_14 = FixedListInt512_Equals_mC35294CA8330B87B3614BED2612F56830F4CF638((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_13, /*hidden argument*/NULL);
		return L_14;
	}

IL_0051:
	{
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		RuntimeObject * L_15 = ___obj0;
		RuntimeObject * L_16 = L_15;
		V_5 = L_16;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_16, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var)))
		{
			goto IL_006c;
		}
	}
	{
		RuntimeObject * L_17 = V_5;
		V_3 = ((*(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)UnBox(L_17, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_18 = V_3;
		bool L_19 = FixedListInt512_Equals_m8AB74F458DC82CE0026D7B902A31456914222D5A((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_18, /*hidden argument*/NULL);
		return L_19;
	}

IL_006c:
	{
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		RuntimeObject * L_20 = ___obj0;
		RuntimeObject * L_21 = L_20;
		V_5 = L_21;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_21, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var)))
		{
			goto IL_0089;
		}
	}
	{
		RuntimeObject * L_22 = V_5;
		V_4 = ((*(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)UnBox(L_22, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_23 = V_4;
		bool L_24 = FixedListInt512_Equals_m390DA9D2C356C7DF2390402B98094A6533EBDA12((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)__this, L_23, /*hidden argument*/NULL);
		return L_24;
	}

IL_0089:
	{
		// return false;
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool FixedListInt512_Equals_m2273812A8319ADB7E01CBF949ACA4B090982A134_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_Equals_m2273812A8319ADB7E01CBF949ACA4B090982A134(_thisAdjusted, ___obj0, method);
}
// System.Collections.IEnumerator Unity.Collections.FixedListInt512::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_System_Collections_IEnumerable_GetEnumerator_mA50E47B5BCB2ED89994F3574E161B48D8D6272E3(_thisAdjusted, method);
}
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt512::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961 (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator<int> IEnumerable<int>.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * _thisAdjusted = reinterpret_cast<FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *>(__this + _offset);
	return FixedListInt512_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_m751221E9ABD4C162512100B709BEC1EFD1D86961(_thisAdjusted, method);
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
// System.Int32 Unity.Collections.FixedListInt64::get_Length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt64::get_LengthInBytes()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_LengthInBytes_m759AAC132FCE21FB9431D3651096ED2DA7C6FC1B (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	{
		// internal int LengthInBytes => Length * sizeof(int);
		int32_t L_0 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		return ((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)4));
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_get_LengthInBytes_m759AAC132FCE21FB9431D3651096ED2DA7C6FC1B_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_get_LengthInBytes_m759AAC132FCE21FB9431D3651096ED2DA7C6FC1B(_thisAdjusted, method);
}
// System.Byte* Unity.Collections.FixedListInt64::get_Buffer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	{
		// {
		FixedBytes62_t1F884E50C61A91B8AA636AC573F9B1F94C7408A4 * L_0 = __this->get_address_of_buffer_1();
		FixedBytes16_t82A7539DACA6BC09BF01ED7A5BF5156D907D9F45 * L_1 = L_0->get_address_of_offset0000_0();
		uint8_t* L_2 = L_1->get_address_of_byte0000_0();
		V_0 = (uint8_t*)L_2;
		// fixed(byte* b = &buffer.offset0000.byte0000)
		uint8_t* L_3 = V_0;
		// return b + FixedList.PaddingBytes<int>();
		int32_t L_4 = FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37(/*hidden argument*/FixedList_PaddingBytes_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m1CD053AB2E167358630221BCF35FE40A78628A37_RuntimeMethod_var);
		return (uint8_t*)(((intptr_t)il2cpp_codegen_add((intptr_t)(((uintptr_t)L_3)), (int32_t)L_4)));
	}
}
IL2CPP_EXTERN_C  uint8_t* FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt64::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, int32_t ___index0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return UnsafeUtility.ReadArrayElement<int>(Buffer, index);
		uint8_t* L_0 = FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___index0;
		int32_t L_2 = UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D((void*)(void*)L_0, L_1, /*hidden argument*/UnsafeUtility_ReadArrayElement_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m5F3076B6E76939AA7DA6DE21EFBD97D7F6B3C86D_RuntimeMethod_var);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176_AdjustorThunk (RuntimeObject * __this, int32_t ___index0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176(_thisAdjusted, ___index0, method);
}
// System.Int32 Unity.Collections.FixedListInt64::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_GetHashCode_m6233D06769FE02170A4C25BF9ADB7DAA1F4D0DDF (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	{
		// return (int)CollectionHelper.Hash(Buffer, LengthInBytes);
		uint8_t* L_0 = FixedListInt64_get_Buffer_m76A62C88FEB17939508A31AA61D3DF0583BD46B5((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt64_get_LengthInBytes_m759AAC132FCE21FB9431D3651096ED2DA7C6FC1B((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		uint32_t L_2 = CollectionHelper_Hash_mED96D80B94A160AA21CFD7A8C9C105DF201A5517((void*)(void*)L_0, L_1, /*hidden argument*/NULL);
		return L_2;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_GetHashCode_m6233D06769FE02170A4C25BF9ADB7DAA1F4D0DDF_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_GetHashCode_m6233D06769FE02170A4C25BF9ADB7DAA1F4D0DDF(_thisAdjusted, method);
}
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_m0F39A6CA2CFC1E89DE789CE800882CAE817C5D8A (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt32_get_Item_mF4B39506459B3AC8E8737C423DA71CECB0079BFC((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_CompareTo_m0F39A6CA2CFC1E89DE789CE800882CAE817C5D8A_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_CompareTo_m0F39A6CA2CFC1E89DE789CE800882CAE817C5D8A(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m7E17FE5FC3FB44FDE44BCD4A72B6C7BACB015A25 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_0 = ___other0;
		int32_t L_1 = FixedListInt64_CompareTo_m0F39A6CA2CFC1E89DE789CE800882CAE817C5D8A((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt64_Equals_m7E17FE5FC3FB44FDE44BCD4A72B6C7BACB015A25_AdjustorThunk (RuntimeObject * __this, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_Equals_m7E17FE5FC3FB44FDE44BCD4A72B6C7BACB015A25(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_m4B4FAD68DACA0C57789369B6325283B9FEB39FF6 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_CompareTo_m4B4FAD68DACA0C57789369B6325283B9FEB39FF6_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_CompareTo_m4B4FAD68DACA0C57789369B6325283B9FEB39FF6(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m76E2A940A74D24747C4DCC9B5C215C9EFA7DF648 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_0 = ___other0;
		int32_t L_1 = FixedListInt64_CompareTo_m4B4FAD68DACA0C57789369B6325283B9FEB39FF6((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt64_Equals_m76E2A940A74D24747C4DCC9B5C215C9EFA7DF648_AdjustorThunk (RuntimeObject * __this, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_Equals_m76E2A940A74D24747C4DCC9B5C215C9EFA7DF648(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_mBDF6F0304435B144667FF11279F8A46D34592C14 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt128_get_Item_mE0561DA471AFDFAD7B4164A8B34C021719CF465D((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_CompareTo_mBDF6F0304435B144667FF11279F8A46D34592C14_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_CompareTo_mBDF6F0304435B144667FF11279F8A46D34592C14(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m12A1A77A26EBF0A0CCFA1EB0791C78C70DA08068 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_0 = ___other0;
		int32_t L_1 = FixedListInt64_CompareTo_mBDF6F0304435B144667FF11279F8A46D34592C14((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt64_Equals_m12A1A77A26EBF0A0CCFA1EB0791C78C70DA08068_AdjustorThunk (RuntimeObject * __this, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_Equals_m12A1A77A26EBF0A0CCFA1EB0791C78C70DA08068(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_mFBD299F0A35A970B9507DC6F93020B3DD7D03EB8 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt512_get_Item_mC96D95753887236E53B55D978A83CCAF39BC9448((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_CompareTo_mFBD299F0A35A970B9507DC6F93020B3DD7D03EB8_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_CompareTo_mFBD299F0A35A970B9507DC6F93020B3DD7D03EB8(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt512)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_mB5306CCBAAABD58C0C781B93A066025BD4DED9BB (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_0 = ___other0;
		int32_t L_1 = FixedListInt64_CompareTo_mFBD299F0A35A970B9507DC6F93020B3DD7D03EB8((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt64_Equals_mB5306CCBAAABD58C0C781B93A066025BD4DED9BB_AdjustorThunk (RuntimeObject * __this, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_Equals_mB5306CCBAAABD58C0C781B93A066025BD4DED9BB(_thisAdjusted, ___other0, method);
}
// System.Int32 Unity.Collections.FixedListInt64::CompareTo(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FixedListInt64_CompareTo_mF6FFE011D12EFDF7D8565A75F5291BE4DEF7326B (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// var mini = math.min(Length, other.Length);
		int32_t L_0 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		int32_t L_1 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_2 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// for(var i = 0; i < mini; ++i)
		V_1 = 0;
		goto IL_0038;
	}

IL_0017:
	{
		// var j = this[i].CompareTo(other[i]);
		int32_t L_3 = V_1;
		int32_t L_4 = FixedListInt64_get_Item_mB61D501448A4322ECF92543E78AB64F517D17176((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_3, /*hidden argument*/NULL);
		V_3 = L_4;
		int32_t L_5 = V_1;
		int32_t L_6 = FixedListInt4096_get_Item_mA5296F4BCB09A89D819692BF575B3328B48FF66F((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), L_5, /*hidden argument*/NULL);
		int32_t L_7 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if(j != 0)
		int32_t L_8 = V_2;
		if (!L_8)
		{
			goto IL_0034;
		}
	}
	{
		// return j;
		int32_t L_9 = V_2;
		return L_9;
	}

IL_0034:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_10 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0038:
	{
		// for(var i = 0; i < mini; ++i)
		int32_t L_11 = V_1;
		int32_t L_12 = V_0;
		if ((((int32_t)L_11) < ((int32_t)L_12)))
		{
			goto IL_0017;
		}
	}
	{
		// return Length.CompareTo(other.Length);
		int32_t L_13 = FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, /*hidden argument*/NULL);
		V_3 = L_13;
		int32_t L_14 = FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)(&___other0), /*hidden argument*/NULL);
		int32_t L_15 = Int32_CompareTo_m2EB2B72F9095FF3438D830118D57E32E1CC67195((int32_t*)(&V_3), L_14, /*hidden argument*/NULL);
		return L_15;
	}
}
IL2CPP_EXTERN_C  int32_t FixedListInt64_CompareTo_mF6FFE011D12EFDF7D8565A75F5291BE4DEF7326B_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_CompareTo_mF6FFE011D12EFDF7D8565A75F5291BE4DEF7326B(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt64::Equals(Unity.Collections.FixedListInt4096)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_m6010E0A174946BBA0E3EE5DD77058282ACA09100 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	{
		// return CompareTo(other) == 0;
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_0 = ___other0;
		int32_t L_1 = FixedListInt64_CompareTo_mF6FFE011D12EFDF7D8565A75F5291BE4DEF7326B((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_0, /*hidden argument*/NULL);
		return (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool FixedListInt64_Equals_m6010E0A174946BBA0E3EE5DD77058282ACA09100_AdjustorThunk (RuntimeObject * __this, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_Equals_m6010E0A174946BBA0E3EE5DD77058282ACA09100(_thisAdjusted, ___other0, method);
}
// System.Boolean Unity.Collections.FixedListInt64::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool FixedListInt64_Equals_mA735702468591FA565B01C107F10E86E5D2C817A (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt64_Equals_mA735702468591FA565B01C107F10E86E5D2C817A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  V_0;
	memset((&V_0), 0, sizeof(V_0));
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  V_1;
	memset((&V_1), 0, sizeof(V_1));
	FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  V_2;
	memset((&V_2), 0, sizeof(V_2));
	FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  V_3;
	memset((&V_3), 0, sizeof(V_3));
	FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  V_4;
	memset((&V_4), 0, sizeof(V_4));
	RuntimeObject * V_5 = NULL;
	{
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		RuntimeObject * L_0 = ___obj0;
		RuntimeObject * L_1 = L_0;
		V_5 = L_1;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_1, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var)))
		{
			goto IL_001b;
		}
	}
	{
		RuntimeObject * L_2 = V_5;
		V_0 = ((*(FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)((FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF *)UnBox(L_2, FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt32 aFixedListInt32) return Equals(aFixedListInt32);
		FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF  L_3 = V_0;
		bool L_4 = FixedListInt64_Equals_m7E17FE5FC3FB44FDE44BCD4A72B6C7BACB015A25((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_3, /*hidden argument*/NULL);
		return L_4;
	}

IL_001b:
	{
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		RuntimeObject * L_5 = ___obj0;
		RuntimeObject * L_6 = L_5;
		V_5 = L_6;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_6, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var)))
		{
			goto IL_0036;
		}
	}
	{
		RuntimeObject * L_7 = V_5;
		V_1 = ((*(FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)UnBox(L_7, FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt64 aFixedListInt64) return Equals(aFixedListInt64);
		FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0  L_8 = V_1;
		bool L_9 = FixedListInt64_Equals_m76E2A940A74D24747C4DCC9B5C215C9EFA7DF648((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_8, /*hidden argument*/NULL);
		return L_9;
	}

IL_0036:
	{
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		RuntimeObject * L_10 = ___obj0;
		RuntimeObject * L_11 = L_10;
		V_5 = L_11;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_11, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var)))
		{
			goto IL_0051;
		}
	}
	{
		RuntimeObject * L_12 = V_5;
		V_2 = ((*(FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)((FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 *)UnBox(L_12, FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt128 aFixedListInt128) return Equals(aFixedListInt128);
		FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7  L_13 = V_2;
		bool L_14 = FixedListInt64_Equals_m12A1A77A26EBF0A0CCFA1EB0791C78C70DA08068((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_13, /*hidden argument*/NULL);
		return L_14;
	}

IL_0051:
	{
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		RuntimeObject * L_15 = ___obj0;
		RuntimeObject * L_16 = L_15;
		V_5 = L_16;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_16, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var)))
		{
			goto IL_006c;
		}
	}
	{
		RuntimeObject * L_17 = V_5;
		V_3 = ((*(FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)((FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 *)UnBox(L_17, FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt512 aFixedListInt512) return Equals(aFixedListInt512);
		FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794  L_18 = V_3;
		bool L_19 = FixedListInt64_Equals_mB5306CCBAAABD58C0C781B93A066025BD4DED9BB((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_18, /*hidden argument*/NULL);
		return L_19;
	}

IL_006c:
	{
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		RuntimeObject * L_20 = ___obj0;
		RuntimeObject * L_21 = L_20;
		V_5 = L_21;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_21, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var)))
		{
			goto IL_0089;
		}
	}
	{
		RuntimeObject * L_22 = V_5;
		V_4 = ((*(FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)((FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 *)UnBox(L_22, FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983_il2cpp_TypeInfo_var))));
		// if(obj is FixedListInt4096 aFixedListInt4096) return Equals(aFixedListInt4096);
		FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983  L_23 = V_4;
		bool L_24 = FixedListInt64_Equals_m6010E0A174946BBA0E3EE5DD77058282ACA09100((FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *)__this, L_23, /*hidden argument*/NULL);
		return L_24;
	}

IL_0089:
	{
		// return false;
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool FixedListInt64_Equals_mA735702468591FA565B01C107F10E86E5D2C817A_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_Equals_mA735702468591FA565B01C107F10E86E5D2C817A(_thisAdjusted, ___obj0, method);
}
// System.Collections.IEnumerator Unity.Collections.FixedListInt64::System.Collections.IEnumerable.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_System_Collections_IEnumerable_GetEnumerator_m73026B50BD5A4B5804DF01F3AD054C4C8825E905(_thisAdjusted, method);
}
// System.Collections.Generic.IEnumerator`1<System.Int32> Unity.Collections.FixedListInt64::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121 (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// IEnumerator<int> IEnumerable<int>.GetEnumerator() { throw new NotImplementedException(); }
		NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 * L_0 = (NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4 *)il2cpp_codegen_object_new(NotImplementedException_t8AD6EBE5FEDB0AEBECEE0961CF73C35B372EFFA4_il2cpp_TypeInfo_var);
		NotImplementedException__ctor_m8BEA657E260FC05F0C6D2C43A6E9BC08040F59C4(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121_RuntimeMethod_var);
	}
}
IL2CPP_EXTERN_C  RuntimeObject* FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * _thisAdjusted = reinterpret_cast<FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 *>(__this + _offset);
	return FixedListInt64_System_Collections_Generic_IEnumerableU3CSystem_Int32U3E_GetEnumerator_mC3B6FE1E532F21C7E81D47A62FC260593E956121(_thisAdjusted, method);
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
// Unity.Collections.LowLevel.Unsafe.UnsafeList* Unity.Collections.LowLevel.Unsafe.UnsafeList::Create(System.Int32,System.Int32,System.Int32,Unity.Collections.Allocator,Unity.Collections.NativeArrayOptions)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * UnsafeList_Create_mC233310AC75D512F5929FE2BF9EBD8CDE3E944E6 (int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___initialCapacity2, int32_t ___allocator3, int32_t ___options4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (UnsafeList_Create_mC233310AC75D512F5929FE2BF9EBD8CDE3E944E6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * V_0 = NULL;
	AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		// var handle = new AllocatorManager.AllocatorHandle {Value = (int)allocator};
		il2cpp_codegen_initobj((&V_1), sizeof(AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07 ));
		int32_t L_0 = ___allocator3;
		(&V_1)->set_Value_0(L_0);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_1 = V_1;
		// UnsafeList* listData = AllocatorManager.Allocate<UnsafeList>(handle);
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_2 = AllocatorManager_Allocate_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mB55752C04C2E0BF01026A46F7628A6A6C83A1B96(L_1, 1, /*hidden argument*/AllocatorManager_Allocate_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mB55752C04C2E0BF01026A46F7628A6A6C83A1B96_RuntimeMethod_var);
		V_0 = (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)L_2;
		// UnsafeUtility.MemClear(listData, UnsafeUtility.SizeOf<UnsafeList>());
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_3 = V_0;
		int32_t L_4 = UnsafeUtility_SizeOf_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mF5121761EBFDF720A7C139604AFCBAA28008C170(/*hidden argument*/UnsafeUtility_SizeOf_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mF5121761EBFDF720A7C139604AFCBAA28008C170_RuntimeMethod_var);
		UnsafeUtility_MemClear_m288BC0ABEB3E1A7B941FB28033D391E661887545((void*)(void*)L_3, (((int64_t)((int64_t)L_4))), /*hidden argument*/NULL);
		// listData->Allocator = allocator;
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_5 = V_0;
		int32_t L_6 = ___allocator3;
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_7 = AllocatorHandle_op_Implicit_m47019517CEBC57A221413D37A6950816A1BA17F4(L_6, /*hidden argument*/NULL);
		NullCheck(L_5);
		L_5->set_Allocator_3(L_7);
		// if (initialCapacity != 0)
		int32_t L_8 = ___initialCapacity2;
		if (!L_8)
		{
			goto IL_003c;
		}
	}
	{
		// listData->SetCapacity(sizeOf, alignOf, initialCapacity);
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_9 = V_0;
		int32_t L_10 = ___sizeOf0;
		int32_t L_11 = ___alignOf1;
		int32_t L_12 = ___initialCapacity2;
		UnsafeList_SetCapacity_mF763B8AEDC2E1E65FFA068955326306EDB3B22C7((UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)L_9, L_10, L_11, L_12, /*hidden argument*/NULL);
	}

IL_003c:
	{
		// if (options == NativeArrayOptions.ClearMemory
		//     && listData->Ptr != null)
		int32_t L_13 = ___options4;
		if ((!(((uint32_t)L_13) == ((uint32_t)1))))
		{
			goto IL_005f;
		}
	}
	{
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_14 = V_0;
		NullCheck(L_14);
		void* L_15 = L_14->get_Ptr_0();
		if ((((intptr_t)L_15) == ((intptr_t)(((uintptr_t)0)))))
		{
			goto IL_005f;
		}
	}
	{
		// UnsafeUtility.MemClear(listData->Ptr, listData->Capacity * sizeOf);
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_16 = V_0;
		NullCheck(L_16);
		void* L_17 = L_16->get_Ptr_0();
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_18 = V_0;
		NullCheck(L_18);
		int32_t L_19 = L_18->get_Capacity_2();
		int32_t L_20 = ___sizeOf0;
		UnsafeUtility_MemClear_m288BC0ABEB3E1A7B941FB28033D391E661887545((void*)(void*)L_17, (((int64_t)((int64_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_19, (int32_t)L_20))))), /*hidden argument*/NULL);
	}

IL_005f:
	{
		// return listData;
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_21 = V_0;
		return (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)(L_21);
	}
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Destroy(Unity.Collections.LowLevel.Unsafe.UnsafeList*)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Destroy_mCB345E35771EEF73569EF65D07DC63A09803F7C2 (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * ___listData0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (UnsafeList_Destroy_mCB345E35771EEF73569EF65D07DC63A09803F7C2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// var allocator = listData->Allocator;
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_0 = ___listData0;
		NullCheck(L_0);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_1 = L_0->get_Allocator_3();
		// listData->Dispose();
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_2 = ___listData0;
		UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D((UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)L_2, /*hidden argument*/NULL);
		// AllocatorManager.Free(allocator, listData);
		UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * L_3 = ___listData0;
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		AllocatorManager_Free_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mC9DEFCD77EAF09FCF9CF7C11E3C9D8233DDF9950(L_1, (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)(UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)L_3, 1, /*hidden argument*/AllocatorManager_Free_TisUnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A_mC9DEFCD77EAF09FCF9CF7C11E3C9D8233DDF9950_RuntimeMethod_var);
		// }
		return;
	}
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (CollectionHelper.ShouldDeallocate(Allocator))
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_0 = __this->get_Allocator_3();
		bool L_1 = CollectionHelper_ShouldDeallocate_m05F1EA772FCA1D6A975343CEB7853C3A4F3008F8(L_0, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0029;
		}
	}
	{
		// AllocatorManager.Free(Allocator, Ptr);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_2 = __this->get_Allocator_3();
		void* L_3 = __this->get_Ptr_0();
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		AllocatorManager_Free_mDE1594E464749B50FF597BC1080C547AE5DD7634(L_2, (void*)(void*)L_3, /*hidden argument*/NULL);
		// Allocator = AllocatorManager.Invalid;
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_4 = ((AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_StaticFields*)il2cpp_codegen_static_fields_for(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var))->get_Invalid_0();
		__this->set_Allocator_3(L_4);
	}

IL_0029:
	{
		// Ptr = null;
		__this->set_Ptr_0((void*)(((uintptr_t)0)));
		// Length = 0;
		__this->set_Length_1(0);
		// Capacity = 0;
		__this->set_Capacity_2(0);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * _thisAdjusted = reinterpret_cast<UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *>(__this + _offset);
	UnsafeList_Dispose_m7095D947A629CEFDBE98667D4A832AEBCE34D73D(_thisAdjusted, method);
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Clear_mE7B1A02E25F65569A525FA0D7CF2BBCB0A0F53AE (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, const RuntimeMethod* method)
{
	{
		// Length = 0;
		__this->set_Length_1(0);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void UnsafeList_Clear_mE7B1A02E25F65569A525FA0D7CF2BBCB0A0F53AE_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * _thisAdjusted = reinterpret_cast<UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *>(__this + _offset);
	UnsafeList_Clear_mE7B1A02E25F65569A525FA0D7CF2BBCB0A0F53AE(_thisAdjusted, method);
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Resize(System.Int32,System.Int32,System.Int32,Unity.Collections.NativeArrayOptions)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Resize_m07DF56383367580C079E9DE31C33866F80054CBF (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___length2, int32_t ___options3, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	uint8_t* V_2 = NULL;
	{
		// var oldLength = Length;
		int32_t L_0 = __this->get_Length_1();
		V_0 = L_0;
		// if (length > Capacity)
		int32_t L_1 = ___length2;
		int32_t L_2 = __this->get_Capacity_2();
		if ((((int32_t)L_1) <= ((int32_t)L_2)))
		{
			goto IL_0019;
		}
	}
	{
		// SetCapacity(sizeOf, alignOf, length);
		int32_t L_3 = ___sizeOf0;
		int32_t L_4 = ___alignOf1;
		int32_t L_5 = ___length2;
		UnsafeList_SetCapacity_mF763B8AEDC2E1E65FFA068955326306EDB3B22C7((UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)__this, L_3, L_4, L_5, /*hidden argument*/NULL);
	}

IL_0019:
	{
		// Length = length;
		int32_t L_6 = ___length2;
		__this->set_Length_1(L_6);
		// if (options == NativeArrayOptions.ClearMemory
		//     && oldLength < length)
		int32_t L_7 = ___options3;
		if ((!(((uint32_t)L_7) == ((uint32_t)1))))
		{
			goto IL_0042;
		}
	}
	{
		int32_t L_8 = V_0;
		int32_t L_9 = ___length2;
		if ((((int32_t)L_8) >= ((int32_t)L_9)))
		{
			goto IL_0042;
		}
	}
	{
		// var num = length - oldLength;
		int32_t L_10 = ___length2;
		int32_t L_11 = V_0;
		V_1 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_10, (int32_t)L_11));
		// byte* ptr = (byte*)Ptr;
		void* L_12 = __this->get_Ptr_0();
		V_2 = (uint8_t*)L_12;
		// UnsafeUtility.MemClear(ptr + oldLength * sizeOf, num * sizeOf);
		uint8_t* L_13 = V_2;
		int32_t L_14 = V_0;
		int32_t L_15 = ___sizeOf0;
		int32_t L_16 = V_1;
		int32_t L_17 = ___sizeOf0;
		UnsafeUtility_MemClear_m288BC0ABEB3E1A7B941FB28033D391E661887545((void*)(void*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_13, (int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_14, (int32_t)L_15)))), (((int64_t)((int64_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_16, (int32_t)L_17))))), /*hidden argument*/NULL);
	}

IL_0042:
	{
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void UnsafeList_Resize_m07DF56383367580C079E9DE31C33866F80054CBF_AdjustorThunk (RuntimeObject * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___length2, int32_t ___options3, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * _thisAdjusted = reinterpret_cast<UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *>(__this + _offset);
	UnsafeList_Resize_m07DF56383367580C079E9DE31C33866F80054CBF(_thisAdjusted, ___sizeOf0, ___alignOf1, ___length2, ___options3, method);
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::Realloc(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023 (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___capacity2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void* V_0 = NULL;
	int32_t V_1 = 0;
	{
		// void* newPointer = null;
		V_0 = (void*)(((uintptr_t)0));
		// if (capacity > 0)
		int32_t L_0 = ___capacity2;
		if ((((int32_t)L_0) <= ((int32_t)0)))
		{
			goto IL_003c;
		}
	}
	{
		// newPointer = AllocatorManager.Allocate(Allocator, sizeOf, alignOf, capacity);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_1 = __this->get_Allocator_3();
		int32_t L_2 = ___sizeOf0;
		int32_t L_3 = ___alignOf1;
		int32_t L_4 = ___capacity2;
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		void* L_5 = AllocatorManager_Allocate_mCF16ACBCDD6B16DC91335AFD498497C374E62A9D(L_1, L_2, L_3, L_4, /*hidden argument*/NULL);
		V_0 = (void*)L_5;
		// if (Capacity > 0)
		int32_t L_6 = __this->get_Capacity_2();
		if ((((int32_t)L_6) <= ((int32_t)0)))
		{
			goto IL_003c;
		}
	}
	{
		// var itemsToCopy = math.min(capacity, Capacity);
		int32_t L_7 = ___capacity2;
		int32_t L_8 = __this->get_Capacity_2();
		int32_t L_9 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_7, L_8, /*hidden argument*/NULL);
		// var bytesToCopy = itemsToCopy * sizeOf;
		int32_t L_10 = ___sizeOf0;
		V_1 = ((int32_t)il2cpp_codegen_multiply((int32_t)L_9, (int32_t)L_10));
		// UnsafeUtility.MemCpy(newPointer, Ptr, bytesToCopy);
		void* L_11 = V_0;
		void* L_12 = __this->get_Ptr_0();
		int32_t L_13 = V_1;
		UnsafeUtility_MemCpy_mA675903DD7350CC5EC22947C0899B18944E3578C((void*)(void*)L_11, (void*)(void*)L_12, (((int64_t)((int64_t)L_13))), /*hidden argument*/NULL);
	}

IL_003c:
	{
		// AllocatorManager.Free(Allocator, Ptr);
		AllocatorHandle_tBE43AA4BBDEA80E5482C6F24F36A70925A7B5F07  L_14 = __this->get_Allocator_3();
		void* L_15 = __this->get_Ptr_0();
		IL2CPP_RUNTIME_CLASS_INIT(AllocatorManager_tB141F64B5CB15FAE1D9E5682359B53A20FBC3EA5_il2cpp_TypeInfo_var);
		AllocatorManager_Free_mDE1594E464749B50FF597BC1080C547AE5DD7634(L_14, (void*)(void*)L_15, /*hidden argument*/NULL);
		// Ptr = newPointer;
		void* L_16 = V_0;
		__this->set_Ptr_0((void*)L_16);
		// Capacity = capacity;
		int32_t L_17 = ___capacity2;
		__this->set_Capacity_2(L_17);
		// Length = math.min(Length, capacity);
		int32_t L_18 = __this->get_Length_1();
		int32_t L_19 = ___capacity2;
		int32_t L_20 = math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline(L_18, L_19, /*hidden argument*/NULL);
		__this->set_Length_1(L_20);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023_AdjustorThunk (RuntimeObject * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___capacity2, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * _thisAdjusted = reinterpret_cast<UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *>(__this + _offset);
	UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023(_thisAdjusted, ___sizeOf0, ___alignOf1, ___capacity2, method);
}
// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeList::SetCapacity(System.Int32,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeList_SetCapacity_mF763B8AEDC2E1E65FFA068955326306EDB3B22C7 (UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___capacity2, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		// var newCapacity = math.max(capacity, 64 / sizeOf);
		int32_t L_0 = ___capacity2;
		int32_t L_1 = ___sizeOf0;
		int32_t L_2 = math_max_mE358BDDC8FCC6DCACBC5DAACE15C1B74CAA41CF7_inline(L_0, ((int32_t)((int32_t)((int32_t)64)/(int32_t)L_1)), /*hidden argument*/NULL);
		V_0 = L_2;
		// newCapacity = math.ceilpow2(newCapacity);
		int32_t L_3 = V_0;
		int32_t L_4 = math_ceilpow2_mF2EC71F87ADC86C9B3F00E03B9B042B3DCEE89A9_inline(L_3, /*hidden argument*/NULL);
		V_0 = L_4;
		// if (newCapacity == Capacity)
		int32_t L_5 = V_0;
		int32_t L_6 = __this->get_Capacity_2();
		if ((!(((uint32_t)L_5) == ((uint32_t)L_6))))
		{
			goto IL_001c;
		}
	}
	{
		// return;
		return;
	}

IL_001c:
	{
		// Realloc(sizeOf, alignOf, newCapacity);
		int32_t L_7 = ___sizeOf0;
		int32_t L_8 = ___alignOf1;
		int32_t L_9 = V_0;
		UnsafeList_Realloc_m7708AC993EFD44BC6DEA7192961F58B82C464023((UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *)__this, L_7, L_8, L_9, /*hidden argument*/NULL);
		// }
		return;
	}
}
IL2CPP_EXTERN_C  void UnsafeList_SetCapacity_mF763B8AEDC2E1E65FFA068955326306EDB3B22C7_AdjustorThunk (RuntimeObject * __this, int32_t ___sizeOf0, int32_t ___alignOf1, int32_t ___capacity2, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A * _thisAdjusted = reinterpret_cast<UnsafeList_t63311B831D16D018CEB25902C3C9B9BFF02CD34A *>(__this + _offset);
	UnsafeList_SetCapacity_mF763B8AEDC2E1E65FFA068955326306EDB3B22C7(_thisAdjusted, ___sizeOf0, ___alignOf1, ___capacity2, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_max_mE358BDDC8FCC6DCACBC5DAACE15C1B74CAA41CF7_inline (int32_t ___x0, int32_t ___y1, const RuntimeMethod* method)
{
	{
		// public static int max(int x, int y) { return x > y ? x : y; }
		int32_t L_0 = ___x0;
		int32_t L_1 = ___y1;
		if ((((int32_t)L_0) > ((int32_t)L_1)))
		{
			goto IL_0006;
		}
	}
	{
		int32_t L_2 = ___y1;
		return L_2;
	}

IL_0006:
	{
		int32_t L_3 = ___x0;
		return L_3;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_lzcnt_m960E448337A464EAFF1261B9F67725F97207300C_inline (int32_t ___x0, const RuntimeMethod* method)
{
	{
		// public static int lzcnt(int x) { return lzcnt((uint)x); }
		int32_t L_0 = ___x0;
		int32_t L_1 = math_lzcnt_m7CAAF4F0B52359FD893C21E195915EAD7B16E373_inline(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt4096_get_Length_mE6E9E0A99EE958BCABB693ADA3C31BD7B3B79BE9_inline (FixedListInt4096_tF276BFFEB0FCAD76212608E9BD2EF3AEE0939983 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t SlabAllocator_TryU24BurstManaged_m0E85623C6B44B7C3B6A5829BF73EFF1BD5A1E8B1_inline (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	{
		// return ((SlabAllocator*)allocatorState)->Try(ref block);
		intptr_t L_0 = ___allocatorState0;
		void* L_1 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_0, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_2 = ___block1;
		int32_t L_3 = SlabAllocator_Try_m817D1D627E27AD82F85EF5E20BC46270D3CCC972((SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)(SlabAllocator_t2CC4D631F1C798C6B87D1E3A82519569AA3C4E1C *)L_1, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_2, /*hidden argument*/NULL);
		return L_3;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t StackAllocator_TryU24BurstManaged_m46B078D3E5C2608D24398E4A9B1AA71F352F3FBE_inline (intptr_t ___allocatorState0, Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * ___block1, const RuntimeMethod* method)
{
	{
		// return ((StackAllocator*)allocatorState)->Try(ref block);
		intptr_t L_0 = ___allocatorState0;
		void* L_1 = IntPtr_op_Explicit_mB8A512095BCE1A23B2840310C8A27C928ADAD027((intptr_t)L_0, /*hidden argument*/NULL);
		Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 * L_2 = ___block1;
		int32_t L_3 = StackAllocator_Try_m59D465C25E5A380FA6F69D0F7CF68C99E0A5C5A9((StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 *)(StackAllocator_t4E46683D1602FF3B73D39F541F8365AECA88A330 *)L_1, (Block_t8FA82F74EF4F452746424BF951AB03A757DFDAF5 *)L_2, /*hidden argument*/NULL);
		return L_3;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt128_get_Length_m532F8BD816C39C0BBCBE5EBD6DC0C6ADB976AD57_inline (FixedListInt128_t3184BEFC6B96A1DF45785F1A429FDFA1246170D7 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt32_get_Length_m0E814D3EAFE49EAB66B2B9CAEE0FFC6D1C8082C1_inline (FixedListInt32_t748E34851B69379EFA416B02BACD397B26F353CF * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_min_m97B3633177A38A438B439C64D4F5516DF888D3DB_inline (int32_t ___x0, int32_t ___y1, const RuntimeMethod* method)
{
	{
		// public static int min(int x, int y) { return x < y ? x : y; }
		int32_t L_0 = ___x0;
		int32_t L_1 = ___y1;
		if ((((int32_t)L_0) < ((int32_t)L_1)))
		{
			goto IL_0006;
		}
	}
	{
		int32_t L_2 = ___y1;
		return L_2;
	}

IL_0006:
	{
		int32_t L_3 = ___x0;
		return L_3;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt64_get_Length_m9CF5B5E74BA6AE8ED0D7AA3437B7A5D3A91D0FA0_inline (FixedListInt64_tBE3DC5CAA5D9DD73C578A6B0F6E798A71DDB08C0 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t FixedListInt512_get_Length_mDBD2EB5F166EB949C4DDB86811937FFD410296F7_inline (FixedListInt512_t28A2A9E477211DFAD1E383673CD02A0728381794 * __this, const RuntimeMethod* method)
{
	{
		// get => length;
		uint16_t L_0 = __this->get_length_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_ceilpow2_mF2EC71F87ADC86C9B3F00E03B9B042B3DCEE89A9_inline (int32_t ___x0, const RuntimeMethod* method)
{
	{
		// x -= 1;
		int32_t L_0 = ___x0;
		___x0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_0, (int32_t)1));
		// x |= x >> 1;
		int32_t L_1 = ___x0;
		int32_t L_2 = ___x0;
		___x0 = ((int32_t)((int32_t)L_1|(int32_t)((int32_t)((int32_t)L_2>>(int32_t)1))));
		// x |= x >> 2;
		int32_t L_3 = ___x0;
		int32_t L_4 = ___x0;
		___x0 = ((int32_t)((int32_t)L_3|(int32_t)((int32_t)((int32_t)L_4>>(int32_t)2))));
		// x |= x >> 4;
		int32_t L_5 = ___x0;
		int32_t L_6 = ___x0;
		___x0 = ((int32_t)((int32_t)L_5|(int32_t)((int32_t)((int32_t)L_6>>(int32_t)4))));
		// x |= x >> 8;
		int32_t L_7 = ___x0;
		int32_t L_8 = ___x0;
		___x0 = ((int32_t)((int32_t)L_7|(int32_t)((int32_t)((int32_t)L_8>>(int32_t)8))));
		// x |= x >> 16;
		int32_t L_9 = ___x0;
		int32_t L_10 = ___x0;
		___x0 = ((int32_t)((int32_t)L_9|(int32_t)((int32_t)((int32_t)L_10>>(int32_t)((int32_t)16)))));
		// return x + 1;
		int32_t L_11 = ___x0;
		return ((int32_t)il2cpp_codegen_add((int32_t)L_11, (int32_t)1));
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR void FunctionPointer_1__ctor_m05E91DF0F7C983F68774073D8477FAF5943068CD_gshared_inline (FunctionPointer_1_t5AF97C37E92E5F70B805E2C94E6BB3582D040303 * __this, intptr_t ___ptr0, const RuntimeMethod* method)
{
	{
		// _ptr = ptr;
		intptr_t L_0 = ___ptr0;
		__this->set__ptr_0((intptr_t)L_0);
		// }
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t math_lzcnt_m7CAAF4F0B52359FD893C21E195915EAD7B16E373_inline (uint32_t ___x0, const RuntimeMethod* method)
{
	LongDoubleUnion_tEA9A08E85EB44174AE90CB7541C8306BDDF61ECD  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// if (x == 0)
		uint32_t L_0 = ___x0;
		if (L_0)
		{
			goto IL_0006;
		}
	}
	{
		// return 32;
		return ((int32_t)32);
	}

IL_0006:
	{
		// u.doubleValue = 0.0;
		(&V_0)->set_doubleValue_1((0.0));
		// u.longValue = 0x4330000000000000L + x;
		uint32_t L_1 = ___x0;
		(&V_0)->set_longValue_0(((int64_t)il2cpp_codegen_add((int64_t)((int64_t)4841369599423283200LL), (int64_t)(((int64_t)((uint64_t)L_1))))));
		// u.doubleValue -= 4503599627370496.0;
		double* L_2 = (&V_0)->get_address_of_doubleValue_1();
		double* L_3 = L_2;
		double L_4 = *((double*)L_3);
		*((double*)L_3) = (double)((double)il2cpp_codegen_subtract((double)L_4, (double)(4503599627370496.0)));
		// return 0x41E - (int)(u.longValue >> 52);
		LongDoubleUnion_tEA9A08E85EB44174AE90CB7541C8306BDDF61ECD  L_5 = V_0;
		int64_t L_6 = L_5.get_longValue_0();
		return ((int32_t)il2cpp_codegen_subtract((int32_t)((int32_t)1054), (int32_t)(((int32_t)((int32_t)((int64_t)((int64_t)L_6>>(int32_t)((int32_t)52))))))));
	}
}
