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
template <typename T1>
struct InterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};

// System.Action`1<System.Object>
struct Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0;
// System.Action`2<System.Object,System.Object>
struct Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C;
// System.Action`3<System.Object,System.Object,System.Object>
struct Action_3_tCC14115B7178951118504E7198B7C872630643C5;
// System.Action`4<System.Object,System.Object,System.Object,System.Object>
struct Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923;
// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Collections.Generic.List`1<System.String>
struct List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3;
// System.Collections.Generic.List`1<UnityEngine.UIElements.IEventHandler>
struct List_1_tC6DEF7083E8206203302B1B25E23947E49BBD537;
// System.Collections.Generic.List`1<UnityEngine.UIElements.VisualElement>
struct List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756;
// System.Collections.Generic.Stack`1<System.Object>
struct Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163;
// System.Collections.IDictionary
struct IDictionary_t1BD5C1546718A374EA8122FBD6C6EE45331E8CE7;
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
// System.NotSupportedException
struct NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010;
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770;
// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.Event
struct Event_t187FF6A6B357447B83EC2064823EE0AEC5263210;
// UnityEngine.UIElements.BaseVisualElementPanel
struct BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90;
// UnityEngine.UIElements.ElementUnderPointer
struct ElementUnderPointer_t95B57508EE9F6700FE873A796AAF66C0EE2A0035;
// UnityEngine.UIElements.EventBase
struct EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD;
// UnityEngine.UIElements.EventCallbackRegistry
struct EventCallbackRegistry_t0E675C9EC7F38446B7A42C43115CEDB7BE67776D;
// UnityEngine.UIElements.IEventHandler
struct IEventHandler_t404CEA309DA7B5B5EB5536186E3F34645F0372B2;
// UnityEngine.UIElements.IPanel
struct IPanel_t3AAE62317DEE1C12E547C78C27556B659DE4F20A;
// UnityEngine.UIElements.IPointerEvent
struct IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA;
// UnityEngine.UIElements.ObjectPool`1<System.Object>
struct ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E;
// UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>
struct PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE;
// UnityEngine.UIElements.PointerEventBase`1<System.Object>
struct PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634;
// UnityEngine.UIElements.PropagationPaths
struct PropagationPaths_t59C288865B38FA5B8E17CA28D79FC4E1C58CE10D;
// UnityEngine.UIElements.RepaintData
struct RepaintData_tE3656A64C3BE5ABCD95A72370251036B6E75F74F;
// UnityEngine.UIElements.StyleSheets.InheritedStylesData
struct InheritedStylesData_t03E6A5F7D9F2C86527C534C7A1A76EBAA039AE2E;
// UnityEngine.UIElements.StyleSheets.VisualElementStylesData
struct VisualElementStylesData_tA65DA46967965C0F2B590B64C424DBCE087CF867;
// UnityEngine.UIElements.VisualElement
struct VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57;
// UnityEngine.WaitForSeconds
struct WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8;
// UnityEngine.Yoga.YogaNode
struct YogaNode_tFCB801A447DAC7A335C686ABC5941A4357102A2C;
// Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>
struct U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1;
// Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>
struct U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB;
// Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>
struct U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07;
// Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>
struct U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A;

IL2CPP_EXTERN_C RuntimeClass* BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* EventType_t3D3937E705A4506226002DAB22071B7B181DA57B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PointerDeviceState_tBADA581B3E49FE39A2230120F84FCBD94BB8ADA4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StyleKeyword_t5C285A4249A1A7A807C1B4D2AAF5D1350B0A3560_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral04231B44477132B3DBEFE7768A921AE5A13A00FC;
IL2CPP_EXTERN_C String_t* _stringLiteralD21048C5C5B019FA0A181ABA5C8DC41BEE59E376;
IL2CPP_EXTERN_C String_t* _stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A;
IL2CPP_EXTERN_C String_t* _stringLiteralF950D9CCC7081471921E58DAA686F27E792540D3;
IL2CPP_EXTERN_C const RuntimeMethod* U3CDelayedInvokeU3Ed__6_1_System_Collections_IEnumerator_Reset_mB0F6B042AF13D81BE8AB6D1453B5ED67765AFA27_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CDelayedInvokeU3Ed__7_2_System_Collections_IEnumerator_Reset_mD17AB881B724B98138973411FEAF04404D1D1FAB_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CDelayedInvokeU3Ed__8_3_System_Collections_IEnumerator_Reset_m3DFBF2AC1E000261E6E165CF4A3E0838DCBA48C0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CDelayedInvokeU3Ed__9_4_System_Collections_IEnumerator_Reset_m17A21D466AB09EF4553CC3BB253C31498F0AD1B7_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t ObjectPool_1_Release_m2F54E298C569A530D9BE145E89A85461969B0E79_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ObjectPool_1_set_maxSize_m5732821144B90CB4DCAD3F663176B589E7AAE794_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerCaptureEventBase_1_LocalInit_m67C407968188A88A0860879AD1A1BE70884AA2F9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_GetPooled_m654B49807444AB3C852A3B8F68181B9203E352F7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_GetPooled_m86CCDFBD02BDF5AE0EB7F297281EF28A286C0162_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_GetPooled_mB4E401D8A4F768CEE0AC20377156F8AD7C9A32EE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_LocalInit_m6A72C0C79B21E843E37A6DDC0BE8A117365A28E6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_PostDispatch_m2DE466AABDA92057C28FD02AF8C130F459C06FBA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_PreDispatch_m359A5629E26AA079E72B1A00D41C93821C6368B0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PointerEventBase_1_set_currentTarget_m571C3135C0C55466E277FDA84EE500DCB69C1C51_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__6_1_MoveNext_m7A8BFC56E3FAC8345E61CD658C43201B8BA12F8A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__6_1_System_Collections_IEnumerator_Reset_mB0F6B042AF13D81BE8AB6D1453B5ED67765AFA27_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__7_2_MoveNext_mE184C4FC64AC3B2ED3940AC0441ECDA9B9C9DC0A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__7_2_System_Collections_IEnumerator_Reset_mD17AB881B724B98138973411FEAF04404D1D1FAB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__8_3_MoveNext_m5513FC810F1B6A121A15C664061E4816F393D724_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__8_3_System_Collections_IEnumerator_Reset_m3DFBF2AC1E000261E6E165CF4A3E0838DCBA48C0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__9_4_MoveNext_m6FF1B65B3B5A3519E409145A8FE083BFF9EB6FC8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CDelayedInvokeU3Ed__9_4_System_Collections_IEnumerator_Reset_m17A21D466AB09EF4553CC3BB253C31498F0AD1B7_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Collections.Generic.Stack`1<System.Object>
struct Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.Stack`1::_array
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ____array_0;
	// System.Int32 System.Collections.Generic.Stack`1::_size
	int32_t ____size_1;
	// System.Int32 System.Collections.Generic.Stack`1::_version
	int32_t ____version_2;
	// System.Object System.Collections.Generic.Stack`1::_syncRoot
	RuntimeObject * ____syncRoot_3;

public:
	inline static int32_t get_offset_of__array_0() { return static_cast<int32_t>(offsetof(Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163, ____array_0)); }
	inline ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* get__array_0() const { return ____array_0; }
	inline ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A** get_address_of__array_0() { return &____array_0; }
	inline void set__array_0(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* value)
	{
		____array_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____array_0), (void*)value);
	}

	inline static int32_t get_offset_of__size_1() { return static_cast<int32_t>(offsetof(Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163, ____size_1)); }
	inline int32_t get__size_1() const { return ____size_1; }
	inline int32_t* get_address_of__size_1() { return &____size_1; }
	inline void set__size_1(int32_t value)
	{
		____size_1 = value;
	}

	inline static int32_t get_offset_of__version_2() { return static_cast<int32_t>(offsetof(Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163, ____version_2)); }
	inline int32_t get__version_2() const { return ____version_2; }
	inline int32_t* get_address_of__version_2() { return &____version_2; }
	inline void set__version_2(int32_t value)
	{
		____version_2 = value;
	}

	inline static int32_t get_offset_of__syncRoot_3() { return static_cast<int32_t>(offsetof(Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163, ____syncRoot_3)); }
	inline RuntimeObject * get__syncRoot_3() const { return ____syncRoot_3; }
	inline RuntimeObject ** get_address_of__syncRoot_3() { return &____syncRoot_3; }
	inline void set__syncRoot_3(RuntimeObject * value)
	{
		____syncRoot_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_3), (void*)value);
	}
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

// UnityEngine.UIElements.BaseVisualElementPanel
struct BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90  : public RuntimeObject
{
public:
	// System.Single UnityEngine.UIElements.BaseVisualElementPanel::m_PixelsPerPoint
	float ___m_PixelsPerPoint_0;
	// System.Boolean UnityEngine.UIElements.BaseVisualElementPanel::<duringLayoutPhase>k__BackingField
	bool ___U3CduringLayoutPhaseU3Ek__BackingField_1;
	// UnityEngine.UIElements.RepaintData UnityEngine.UIElements.BaseVisualElementPanel::<repaintData>k__BackingField
	RepaintData_tE3656A64C3BE5ABCD95A72370251036B6E75F74F * ___U3CrepaintDataU3Ek__BackingField_2;
	// System.Boolean UnityEngine.UIElements.BaseVisualElementPanel::<disposed>k__BackingField
	bool ___U3CdisposedU3Ek__BackingField_3;
	// UnityEngine.UIElements.ElementUnderPointer UnityEngine.UIElements.BaseVisualElementPanel::m_TopElementUnderPointers
	ElementUnderPointer_t95B57508EE9F6700FE873A796AAF66C0EE2A0035 * ___m_TopElementUnderPointers_4;

public:
	inline static int32_t get_offset_of_m_PixelsPerPoint_0() { return static_cast<int32_t>(offsetof(BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90, ___m_PixelsPerPoint_0)); }
	inline float get_m_PixelsPerPoint_0() const { return ___m_PixelsPerPoint_0; }
	inline float* get_address_of_m_PixelsPerPoint_0() { return &___m_PixelsPerPoint_0; }
	inline void set_m_PixelsPerPoint_0(float value)
	{
		___m_PixelsPerPoint_0 = value;
	}

	inline static int32_t get_offset_of_U3CduringLayoutPhaseU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90, ___U3CduringLayoutPhaseU3Ek__BackingField_1)); }
	inline bool get_U3CduringLayoutPhaseU3Ek__BackingField_1() const { return ___U3CduringLayoutPhaseU3Ek__BackingField_1; }
	inline bool* get_address_of_U3CduringLayoutPhaseU3Ek__BackingField_1() { return &___U3CduringLayoutPhaseU3Ek__BackingField_1; }
	inline void set_U3CduringLayoutPhaseU3Ek__BackingField_1(bool value)
	{
		___U3CduringLayoutPhaseU3Ek__BackingField_1 = value;
	}

	inline static int32_t get_offset_of_U3CrepaintDataU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90, ___U3CrepaintDataU3Ek__BackingField_2)); }
	inline RepaintData_tE3656A64C3BE5ABCD95A72370251036B6E75F74F * get_U3CrepaintDataU3Ek__BackingField_2() const { return ___U3CrepaintDataU3Ek__BackingField_2; }
	inline RepaintData_tE3656A64C3BE5ABCD95A72370251036B6E75F74F ** get_address_of_U3CrepaintDataU3Ek__BackingField_2() { return &___U3CrepaintDataU3Ek__BackingField_2; }
	inline void set_U3CrepaintDataU3Ek__BackingField_2(RepaintData_tE3656A64C3BE5ABCD95A72370251036B6E75F74F * value)
	{
		___U3CrepaintDataU3Ek__BackingField_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CrepaintDataU3Ek__BackingField_2), (void*)value);
	}

	inline static int32_t get_offset_of_U3CdisposedU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90, ___U3CdisposedU3Ek__BackingField_3)); }
	inline bool get_U3CdisposedU3Ek__BackingField_3() const { return ___U3CdisposedU3Ek__BackingField_3; }
	inline bool* get_address_of_U3CdisposedU3Ek__BackingField_3() { return &___U3CdisposedU3Ek__BackingField_3; }
	inline void set_U3CdisposedU3Ek__BackingField_3(bool value)
	{
		___U3CdisposedU3Ek__BackingField_3 = value;
	}

	inline static int32_t get_offset_of_m_TopElementUnderPointers_4() { return static_cast<int32_t>(offsetof(BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90, ___m_TopElementUnderPointers_4)); }
	inline ElementUnderPointer_t95B57508EE9F6700FE873A796AAF66C0EE2A0035 * get_m_TopElementUnderPointers_4() const { return ___m_TopElementUnderPointers_4; }
	inline ElementUnderPointer_t95B57508EE9F6700FE873A796AAF66C0EE2A0035 ** get_address_of_m_TopElementUnderPointers_4() { return &___m_TopElementUnderPointers_4; }
	inline void set_m_TopElementUnderPointers_4(ElementUnderPointer_t95B57508EE9F6700FE873A796AAF66C0EE2A0035 * value)
	{
		___m_TopElementUnderPointers_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_TopElementUnderPointers_4), (void*)value);
	}
};


// UnityEngine.UIElements.CallbackEventHandler
struct CallbackEventHandler_t3B7336D446FAD9009A9C1DA1B793885F28D214EB  : public RuntimeObject
{
public:
	// UnityEngine.UIElements.EventCallbackRegistry UnityEngine.UIElements.CallbackEventHandler::m_CallbackRegistry
	EventCallbackRegistry_t0E675C9EC7F38446B7A42C43115CEDB7BE67776D * ___m_CallbackRegistry_0;

public:
	inline static int32_t get_offset_of_m_CallbackRegistry_0() { return static_cast<int32_t>(offsetof(CallbackEventHandler_t3B7336D446FAD9009A9C1DA1B793885F28D214EB, ___m_CallbackRegistry_0)); }
	inline EventCallbackRegistry_t0E675C9EC7F38446B7A42C43115CEDB7BE67776D * get_m_CallbackRegistry_0() const { return ___m_CallbackRegistry_0; }
	inline EventCallbackRegistry_t0E675C9EC7F38446B7A42C43115CEDB7BE67776D ** get_address_of_m_CallbackRegistry_0() { return &___m_CallbackRegistry_0; }
	inline void set_m_CallbackRegistry_0(EventCallbackRegistry_t0E675C9EC7F38446B7A42C43115CEDB7BE67776D * value)
	{
		___m_CallbackRegistry_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_CallbackRegistry_0), (void*)value);
	}
};


// UnityEngine.UIElements.ObjectPool`1<System.Object>
struct ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E  : public RuntimeObject
{
public:
	// System.Collections.Generic.Stack`1<T> UnityEngine.UIElements.ObjectPool`1::m_Stack
	Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * ___m_Stack_0;
	// System.Int32 UnityEngine.UIElements.ObjectPool`1::m_MaxSize
	int32_t ___m_MaxSize_1;

public:
	inline static int32_t get_offset_of_m_Stack_0() { return static_cast<int32_t>(offsetof(ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E, ___m_Stack_0)); }
	inline Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * get_m_Stack_0() const { return ___m_Stack_0; }
	inline Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 ** get_address_of_m_Stack_0() { return &___m_Stack_0; }
	inline void set_m_Stack_0(Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * value)
	{
		___m_Stack_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Stack_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_MaxSize_1() { return static_cast<int32_t>(offsetof(ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E, ___m_MaxSize_1)); }
	inline int32_t get_m_MaxSize_1() const { return ___m_MaxSize_1; }
	inline int32_t* get_address_of_m_MaxSize_1() { return &___m_MaxSize_1; }
	inline void set_m_MaxSize_1(int32_t value)
	{
		___m_MaxSize_1 = value;
	}
};


// UnityEngine.UIElements.PointerId
struct PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C  : public RuntimeObject
{
public:

public:
};

struct PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields
{
public:
	// System.Int32 UnityEngine.UIElements.PointerId::maxPointers
	int32_t ___maxPointers_0;
	// System.Int32 UnityEngine.UIElements.PointerId::invalidPointerId
	int32_t ___invalidPointerId_1;
	// System.Int32 UnityEngine.UIElements.PointerId::mousePointerId
	int32_t ___mousePointerId_2;
	// System.Int32 UnityEngine.UIElements.PointerId::touchPointerIdBase
	int32_t ___touchPointerIdBase_3;
	// System.Int32 UnityEngine.UIElements.PointerId::touchPointerCount
	int32_t ___touchPointerCount_4;
	// System.Int32 UnityEngine.UIElements.PointerId::penPointerIdBase
	int32_t ___penPointerIdBase_5;
	// System.Int32 UnityEngine.UIElements.PointerId::penPointerCount
	int32_t ___penPointerCount_6;

public:
	inline static int32_t get_offset_of_maxPointers_0() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___maxPointers_0)); }
	inline int32_t get_maxPointers_0() const { return ___maxPointers_0; }
	inline int32_t* get_address_of_maxPointers_0() { return &___maxPointers_0; }
	inline void set_maxPointers_0(int32_t value)
	{
		___maxPointers_0 = value;
	}

	inline static int32_t get_offset_of_invalidPointerId_1() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___invalidPointerId_1)); }
	inline int32_t get_invalidPointerId_1() const { return ___invalidPointerId_1; }
	inline int32_t* get_address_of_invalidPointerId_1() { return &___invalidPointerId_1; }
	inline void set_invalidPointerId_1(int32_t value)
	{
		___invalidPointerId_1 = value;
	}

	inline static int32_t get_offset_of_mousePointerId_2() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___mousePointerId_2)); }
	inline int32_t get_mousePointerId_2() const { return ___mousePointerId_2; }
	inline int32_t* get_address_of_mousePointerId_2() { return &___mousePointerId_2; }
	inline void set_mousePointerId_2(int32_t value)
	{
		___mousePointerId_2 = value;
	}

	inline static int32_t get_offset_of_touchPointerIdBase_3() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___touchPointerIdBase_3)); }
	inline int32_t get_touchPointerIdBase_3() const { return ___touchPointerIdBase_3; }
	inline int32_t* get_address_of_touchPointerIdBase_3() { return &___touchPointerIdBase_3; }
	inline void set_touchPointerIdBase_3(int32_t value)
	{
		___touchPointerIdBase_3 = value;
	}

	inline static int32_t get_offset_of_touchPointerCount_4() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___touchPointerCount_4)); }
	inline int32_t get_touchPointerCount_4() const { return ___touchPointerCount_4; }
	inline int32_t* get_address_of_touchPointerCount_4() { return &___touchPointerCount_4; }
	inline void set_touchPointerCount_4(int32_t value)
	{
		___touchPointerCount_4 = value;
	}

	inline static int32_t get_offset_of_penPointerIdBase_5() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___penPointerIdBase_5)); }
	inline int32_t get_penPointerIdBase_5() const { return ___penPointerIdBase_5; }
	inline int32_t* get_address_of_penPointerIdBase_5() { return &___penPointerIdBase_5; }
	inline void set_penPointerIdBase_5(int32_t value)
	{
		___penPointerIdBase_5 = value;
	}

	inline static int32_t get_offset_of_penPointerCount_6() { return static_cast<int32_t>(offsetof(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields, ___penPointerCount_6)); }
	inline int32_t get_penPointerCount_6() const { return ___penPointerCount_6; }
	inline int32_t* get_address_of_penPointerCount_6() { return &___penPointerCount_6; }
	inline void set_penPointerCount_6(int32_t value)
	{
		___penPointerCount_6 = value;
	}
};


// UnityEngine.UIElements.PointerType
struct PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB  : public RuntimeObject
{
public:

public:
};

struct PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields
{
public:
	// System.String UnityEngine.UIElements.PointerType::mouse
	String_t* ___mouse_0;
	// System.String UnityEngine.UIElements.PointerType::touch
	String_t* ___touch_1;
	// System.String UnityEngine.UIElements.PointerType::pen
	String_t* ___pen_2;
	// System.String UnityEngine.UIElements.PointerType::unknown
	String_t* ___unknown_3;

public:
	inline static int32_t get_offset_of_mouse_0() { return static_cast<int32_t>(offsetof(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields, ___mouse_0)); }
	inline String_t* get_mouse_0() const { return ___mouse_0; }
	inline String_t** get_address_of_mouse_0() { return &___mouse_0; }
	inline void set_mouse_0(String_t* value)
	{
		___mouse_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___mouse_0), (void*)value);
	}

	inline static int32_t get_offset_of_touch_1() { return static_cast<int32_t>(offsetof(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields, ___touch_1)); }
	inline String_t* get_touch_1() const { return ___touch_1; }
	inline String_t** get_address_of_touch_1() { return &___touch_1; }
	inline void set_touch_1(String_t* value)
	{
		___touch_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___touch_1), (void*)value);
	}

	inline static int32_t get_offset_of_pen_2() { return static_cast<int32_t>(offsetof(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields, ___pen_2)); }
	inline String_t* get_pen_2() const { return ___pen_2; }
	inline String_t** get_address_of_pen_2() { return &___pen_2; }
	inline void set_pen_2(String_t* value)
	{
		___pen_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___pen_2), (void*)value);
	}

	inline static int32_t get_offset_of_unknown_3() { return static_cast<int32_t>(offsetof(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields, ___unknown_3)); }
	inline String_t* get_unknown_3() const { return ___unknown_3; }
	inline String_t** get_address_of_unknown_3() { return &___unknown_3; }
	inline void set_unknown_3(String_t* value)
	{
		___unknown_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___unknown_3), (void*)value);
	}
};


// UnityEngine.YieldInstruction
struct YieldInstruction_t836035AC7BD07A3C7909F7AD2A5B42DE99D91C44  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of UnityEngine.YieldInstruction
struct YieldInstruction_t836035AC7BD07A3C7909F7AD2A5B42DE99D91C44_marshaled_pinvoke
{
};
// Native definition for COM marshalling of UnityEngine.YieldInstruction
struct YieldInstruction_t836035AC7BD07A3C7909F7AD2A5B42DE99D91C44_marshaled_com
{
};

// Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>
struct U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1  : public RuntimeObject
{
public:
	// System.Int32 Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1::<>2__current
	RuntimeObject * ___U3CU3E2__current_1;
	// System.Single Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1::delay
	float ___delay_2;
	// System.Action`1<T> Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1::action
	Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 * ___action_3;
	// T Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1::arg
	RuntimeObject * ___arg_4;

public:
	inline static int32_t get_offset_of_U3CU3E1__state_0() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1, ___U3CU3E1__state_0)); }
	inline int32_t get_U3CU3E1__state_0() const { return ___U3CU3E1__state_0; }
	inline int32_t* get_address_of_U3CU3E1__state_0() { return &___U3CU3E1__state_0; }
	inline void set_U3CU3E1__state_0(int32_t value)
	{
		___U3CU3E1__state_0 = value;
	}

	inline static int32_t get_offset_of_U3CU3E2__current_1() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1, ___U3CU3E2__current_1)); }
	inline RuntimeObject * get_U3CU3E2__current_1() const { return ___U3CU3E2__current_1; }
	inline RuntimeObject ** get_address_of_U3CU3E2__current_1() { return &___U3CU3E2__current_1; }
	inline void set_U3CU3E2__current_1(RuntimeObject * value)
	{
		___U3CU3E2__current_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E2__current_1), (void*)value);
	}

	inline static int32_t get_offset_of_delay_2() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1, ___delay_2)); }
	inline float get_delay_2() const { return ___delay_2; }
	inline float* get_address_of_delay_2() { return &___delay_2; }
	inline void set_delay_2(float value)
	{
		___delay_2 = value;
	}

	inline static int32_t get_offset_of_action_3() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1, ___action_3)); }
	inline Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 * get_action_3() const { return ___action_3; }
	inline Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 ** get_address_of_action_3() { return &___action_3; }
	inline void set_action_3(Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 * value)
	{
		___action_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___action_3), (void*)value);
	}

	inline static int32_t get_offset_of_arg_4() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1, ___arg_4)); }
	inline RuntimeObject * get_arg_4() const { return ___arg_4; }
	inline RuntimeObject ** get_address_of_arg_4() { return &___arg_4; }
	inline void set_arg_4(RuntimeObject * value)
	{
		___arg_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg_4), (void*)value);
	}
};


// Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>
struct U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB  : public RuntimeObject
{
public:
	// System.Int32 Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2::<>2__current
	RuntimeObject * ___U3CU3E2__current_1;
	// System.Single Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2::delay
	float ___delay_2;
	// System.Action`2<T1,T2> Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2::action
	Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C * ___action_3;
	// T1 Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2::arg1
	RuntimeObject * ___arg1_4;
	// T2 Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2::arg2
	RuntimeObject * ___arg2_5;

public:
	inline static int32_t get_offset_of_U3CU3E1__state_0() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB, ___U3CU3E1__state_0)); }
	inline int32_t get_U3CU3E1__state_0() const { return ___U3CU3E1__state_0; }
	inline int32_t* get_address_of_U3CU3E1__state_0() { return &___U3CU3E1__state_0; }
	inline void set_U3CU3E1__state_0(int32_t value)
	{
		___U3CU3E1__state_0 = value;
	}

	inline static int32_t get_offset_of_U3CU3E2__current_1() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB, ___U3CU3E2__current_1)); }
	inline RuntimeObject * get_U3CU3E2__current_1() const { return ___U3CU3E2__current_1; }
	inline RuntimeObject ** get_address_of_U3CU3E2__current_1() { return &___U3CU3E2__current_1; }
	inline void set_U3CU3E2__current_1(RuntimeObject * value)
	{
		___U3CU3E2__current_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E2__current_1), (void*)value);
	}

	inline static int32_t get_offset_of_delay_2() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB, ___delay_2)); }
	inline float get_delay_2() const { return ___delay_2; }
	inline float* get_address_of_delay_2() { return &___delay_2; }
	inline void set_delay_2(float value)
	{
		___delay_2 = value;
	}

	inline static int32_t get_offset_of_action_3() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB, ___action_3)); }
	inline Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C * get_action_3() const { return ___action_3; }
	inline Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C ** get_address_of_action_3() { return &___action_3; }
	inline void set_action_3(Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C * value)
	{
		___action_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___action_3), (void*)value);
	}

	inline static int32_t get_offset_of_arg1_4() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB, ___arg1_4)); }
	inline RuntimeObject * get_arg1_4() const { return ___arg1_4; }
	inline RuntimeObject ** get_address_of_arg1_4() { return &___arg1_4; }
	inline void set_arg1_4(RuntimeObject * value)
	{
		___arg1_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg1_4), (void*)value);
	}

	inline static int32_t get_offset_of_arg2_5() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB, ___arg2_5)); }
	inline RuntimeObject * get_arg2_5() const { return ___arg2_5; }
	inline RuntimeObject ** get_address_of_arg2_5() { return &___arg2_5; }
	inline void set_arg2_5(RuntimeObject * value)
	{
		___arg2_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg2_5), (void*)value);
	}
};


// Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>
struct U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07  : public RuntimeObject
{
public:
	// System.Int32 Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::<>2__current
	RuntimeObject * ___U3CU3E2__current_1;
	// System.Single Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::delay
	float ___delay_2;
	// System.Action`3<T1,T2,T3> Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::action
	Action_3_tCC14115B7178951118504E7198B7C872630643C5 * ___action_3;
	// T1 Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::arg1
	RuntimeObject * ___arg1_4;
	// T2 Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::arg2
	RuntimeObject * ___arg2_5;
	// T3 Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3::arg3
	RuntimeObject * ___arg3_6;

public:
	inline static int32_t get_offset_of_U3CU3E1__state_0() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___U3CU3E1__state_0)); }
	inline int32_t get_U3CU3E1__state_0() const { return ___U3CU3E1__state_0; }
	inline int32_t* get_address_of_U3CU3E1__state_0() { return &___U3CU3E1__state_0; }
	inline void set_U3CU3E1__state_0(int32_t value)
	{
		___U3CU3E1__state_0 = value;
	}

	inline static int32_t get_offset_of_U3CU3E2__current_1() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___U3CU3E2__current_1)); }
	inline RuntimeObject * get_U3CU3E2__current_1() const { return ___U3CU3E2__current_1; }
	inline RuntimeObject ** get_address_of_U3CU3E2__current_1() { return &___U3CU3E2__current_1; }
	inline void set_U3CU3E2__current_1(RuntimeObject * value)
	{
		___U3CU3E2__current_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E2__current_1), (void*)value);
	}

	inline static int32_t get_offset_of_delay_2() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___delay_2)); }
	inline float get_delay_2() const { return ___delay_2; }
	inline float* get_address_of_delay_2() { return &___delay_2; }
	inline void set_delay_2(float value)
	{
		___delay_2 = value;
	}

	inline static int32_t get_offset_of_action_3() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___action_3)); }
	inline Action_3_tCC14115B7178951118504E7198B7C872630643C5 * get_action_3() const { return ___action_3; }
	inline Action_3_tCC14115B7178951118504E7198B7C872630643C5 ** get_address_of_action_3() { return &___action_3; }
	inline void set_action_3(Action_3_tCC14115B7178951118504E7198B7C872630643C5 * value)
	{
		___action_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___action_3), (void*)value);
	}

	inline static int32_t get_offset_of_arg1_4() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___arg1_4)); }
	inline RuntimeObject * get_arg1_4() const { return ___arg1_4; }
	inline RuntimeObject ** get_address_of_arg1_4() { return &___arg1_4; }
	inline void set_arg1_4(RuntimeObject * value)
	{
		___arg1_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg1_4), (void*)value);
	}

	inline static int32_t get_offset_of_arg2_5() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___arg2_5)); }
	inline RuntimeObject * get_arg2_5() const { return ___arg2_5; }
	inline RuntimeObject ** get_address_of_arg2_5() { return &___arg2_5; }
	inline void set_arg2_5(RuntimeObject * value)
	{
		___arg2_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg2_5), (void*)value);
	}

	inline static int32_t get_offset_of_arg3_6() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07, ___arg3_6)); }
	inline RuntimeObject * get_arg3_6() const { return ___arg3_6; }
	inline RuntimeObject ** get_address_of_arg3_6() { return &___arg3_6; }
	inline void set_arg3_6(RuntimeObject * value)
	{
		___arg3_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg3_6), (void*)value);
	}
};


// Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>
struct U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A  : public RuntimeObject
{
public:
	// System.Int32 Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::<>1__state
	int32_t ___U3CU3E1__state_0;
	// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::<>2__current
	RuntimeObject * ___U3CU3E2__current_1;
	// System.Single Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::delay
	float ___delay_2;
	// System.Action`4<T1,T2,T3,T4> Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::action
	Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 * ___action_3;
	// T1 Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::arg1
	RuntimeObject * ___arg1_4;
	// T2 Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::arg2
	RuntimeObject * ___arg2_5;
	// T3 Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::arg3
	RuntimeObject * ___arg3_6;
	// T4 Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4::arg4
	RuntimeObject * ___arg4_7;

public:
	inline static int32_t get_offset_of_U3CU3E1__state_0() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___U3CU3E1__state_0)); }
	inline int32_t get_U3CU3E1__state_0() const { return ___U3CU3E1__state_0; }
	inline int32_t* get_address_of_U3CU3E1__state_0() { return &___U3CU3E1__state_0; }
	inline void set_U3CU3E1__state_0(int32_t value)
	{
		___U3CU3E1__state_0 = value;
	}

	inline static int32_t get_offset_of_U3CU3E2__current_1() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___U3CU3E2__current_1)); }
	inline RuntimeObject * get_U3CU3E2__current_1() const { return ___U3CU3E2__current_1; }
	inline RuntimeObject ** get_address_of_U3CU3E2__current_1() { return &___U3CU3E2__current_1; }
	inline void set_U3CU3E2__current_1(RuntimeObject * value)
	{
		___U3CU3E2__current_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CU3E2__current_1), (void*)value);
	}

	inline static int32_t get_offset_of_delay_2() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___delay_2)); }
	inline float get_delay_2() const { return ___delay_2; }
	inline float* get_address_of_delay_2() { return &___delay_2; }
	inline void set_delay_2(float value)
	{
		___delay_2 = value;
	}

	inline static int32_t get_offset_of_action_3() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___action_3)); }
	inline Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 * get_action_3() const { return ___action_3; }
	inline Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 ** get_address_of_action_3() { return &___action_3; }
	inline void set_action_3(Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 * value)
	{
		___action_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___action_3), (void*)value);
	}

	inline static int32_t get_offset_of_arg1_4() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___arg1_4)); }
	inline RuntimeObject * get_arg1_4() const { return ___arg1_4; }
	inline RuntimeObject ** get_address_of_arg1_4() { return &___arg1_4; }
	inline void set_arg1_4(RuntimeObject * value)
	{
		___arg1_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg1_4), (void*)value);
	}

	inline static int32_t get_offset_of_arg2_5() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___arg2_5)); }
	inline RuntimeObject * get_arg2_5() const { return ___arg2_5; }
	inline RuntimeObject ** get_address_of_arg2_5() { return &___arg2_5; }
	inline void set_arg2_5(RuntimeObject * value)
	{
		___arg2_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg2_5), (void*)value);
	}

	inline static int32_t get_offset_of_arg3_6() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___arg3_6)); }
	inline RuntimeObject * get_arg3_6() const { return ___arg3_6; }
	inline RuntimeObject ** get_address_of_arg3_6() { return &___arg3_6; }
	inline void set_arg3_6(RuntimeObject * value)
	{
		___arg3_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg3_6), (void*)value);
	}

	inline static int32_t get_offset_of_arg4_7() { return static_cast<int32_t>(offsetof(U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A, ___arg4_7)); }
	inline RuntimeObject * get_arg4_7() const { return ___arg4_7; }
	inline RuntimeObject ** get_address_of_arg4_7() { return &___arg4_7; }
	inline void set_arg4_7(RuntimeObject * value)
	{
		___arg4_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___arg4_7), (void*)value);
	}
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


// UnityEngine.Matrix4x4
struct Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA 
{
public:
	// System.Single UnityEngine.Matrix4x4::m00
	float ___m00_0;
	// System.Single UnityEngine.Matrix4x4::m10
	float ___m10_1;
	// System.Single UnityEngine.Matrix4x4::m20
	float ___m20_2;
	// System.Single UnityEngine.Matrix4x4::m30
	float ___m30_3;
	// System.Single UnityEngine.Matrix4x4::m01
	float ___m01_4;
	// System.Single UnityEngine.Matrix4x4::m11
	float ___m11_5;
	// System.Single UnityEngine.Matrix4x4::m21
	float ___m21_6;
	// System.Single UnityEngine.Matrix4x4::m31
	float ___m31_7;
	// System.Single UnityEngine.Matrix4x4::m02
	float ___m02_8;
	// System.Single UnityEngine.Matrix4x4::m12
	float ___m12_9;
	// System.Single UnityEngine.Matrix4x4::m22
	float ___m22_10;
	// System.Single UnityEngine.Matrix4x4::m32
	float ___m32_11;
	// System.Single UnityEngine.Matrix4x4::m03
	float ___m03_12;
	// System.Single UnityEngine.Matrix4x4::m13
	float ___m13_13;
	// System.Single UnityEngine.Matrix4x4::m23
	float ___m23_14;
	// System.Single UnityEngine.Matrix4x4::m33
	float ___m33_15;

public:
	inline static int32_t get_offset_of_m00_0() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m00_0)); }
	inline float get_m00_0() const { return ___m00_0; }
	inline float* get_address_of_m00_0() { return &___m00_0; }
	inline void set_m00_0(float value)
	{
		___m00_0 = value;
	}

	inline static int32_t get_offset_of_m10_1() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m10_1)); }
	inline float get_m10_1() const { return ___m10_1; }
	inline float* get_address_of_m10_1() { return &___m10_1; }
	inline void set_m10_1(float value)
	{
		___m10_1 = value;
	}

	inline static int32_t get_offset_of_m20_2() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m20_2)); }
	inline float get_m20_2() const { return ___m20_2; }
	inline float* get_address_of_m20_2() { return &___m20_2; }
	inline void set_m20_2(float value)
	{
		___m20_2 = value;
	}

	inline static int32_t get_offset_of_m30_3() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m30_3)); }
	inline float get_m30_3() const { return ___m30_3; }
	inline float* get_address_of_m30_3() { return &___m30_3; }
	inline void set_m30_3(float value)
	{
		___m30_3 = value;
	}

	inline static int32_t get_offset_of_m01_4() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m01_4)); }
	inline float get_m01_4() const { return ___m01_4; }
	inline float* get_address_of_m01_4() { return &___m01_4; }
	inline void set_m01_4(float value)
	{
		___m01_4 = value;
	}

	inline static int32_t get_offset_of_m11_5() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m11_5)); }
	inline float get_m11_5() const { return ___m11_5; }
	inline float* get_address_of_m11_5() { return &___m11_5; }
	inline void set_m11_5(float value)
	{
		___m11_5 = value;
	}

	inline static int32_t get_offset_of_m21_6() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m21_6)); }
	inline float get_m21_6() const { return ___m21_6; }
	inline float* get_address_of_m21_6() { return &___m21_6; }
	inline void set_m21_6(float value)
	{
		___m21_6 = value;
	}

	inline static int32_t get_offset_of_m31_7() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m31_7)); }
	inline float get_m31_7() const { return ___m31_7; }
	inline float* get_address_of_m31_7() { return &___m31_7; }
	inline void set_m31_7(float value)
	{
		___m31_7 = value;
	}

	inline static int32_t get_offset_of_m02_8() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m02_8)); }
	inline float get_m02_8() const { return ___m02_8; }
	inline float* get_address_of_m02_8() { return &___m02_8; }
	inline void set_m02_8(float value)
	{
		___m02_8 = value;
	}

	inline static int32_t get_offset_of_m12_9() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m12_9)); }
	inline float get_m12_9() const { return ___m12_9; }
	inline float* get_address_of_m12_9() { return &___m12_9; }
	inline void set_m12_9(float value)
	{
		___m12_9 = value;
	}

	inline static int32_t get_offset_of_m22_10() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m22_10)); }
	inline float get_m22_10() const { return ___m22_10; }
	inline float* get_address_of_m22_10() { return &___m22_10; }
	inline void set_m22_10(float value)
	{
		___m22_10 = value;
	}

	inline static int32_t get_offset_of_m32_11() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m32_11)); }
	inline float get_m32_11() const { return ___m32_11; }
	inline float* get_address_of_m32_11() { return &___m32_11; }
	inline void set_m32_11(float value)
	{
		___m32_11 = value;
	}

	inline static int32_t get_offset_of_m03_12() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m03_12)); }
	inline float get_m03_12() const { return ___m03_12; }
	inline float* get_address_of_m03_12() { return &___m03_12; }
	inline void set_m03_12(float value)
	{
		___m03_12 = value;
	}

	inline static int32_t get_offset_of_m13_13() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m13_13)); }
	inline float get_m13_13() const { return ___m13_13; }
	inline float* get_address_of_m13_13() { return &___m13_13; }
	inline void set_m13_13(float value)
	{
		___m13_13 = value;
	}

	inline static int32_t get_offset_of_m23_14() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m23_14)); }
	inline float get_m23_14() const { return ___m23_14; }
	inline float* get_address_of_m23_14() { return &___m23_14; }
	inline void set_m23_14(float value)
	{
		___m23_14 = value;
	}

	inline static int32_t get_offset_of_m33_15() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m33_15)); }
	inline float get_m33_15() const { return ___m33_15; }
	inline float* get_address_of_m33_15() { return &___m33_15; }
	inline void set_m33_15(float value)
	{
		___m33_15 = value;
	}
};

struct Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_StaticFields
{
public:
	// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::zeroMatrix
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___zeroMatrix_16;
	// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::identityMatrix
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___identityMatrix_17;

public:
	inline static int32_t get_offset_of_zeroMatrix_16() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_StaticFields, ___zeroMatrix_16)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_zeroMatrix_16() const { return ___zeroMatrix_16; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_zeroMatrix_16() { return &___zeroMatrix_16; }
	inline void set_zeroMatrix_16(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___zeroMatrix_16 = value;
	}

	inline static int32_t get_offset_of_identityMatrix_17() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_StaticFields, ___identityMatrix_17)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_identityMatrix_17() const { return ___identityMatrix_17; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_identityMatrix_17() { return &___identityMatrix_17; }
	inline void set_identityMatrix_17(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___identityMatrix_17 = value;
	}
};


// UnityEngine.PropertyName
struct PropertyName_t75EB843FEA2EC372093479A35C24364D2DF98529 
{
public:
	// System.Int32 UnityEngine.PropertyName::id
	int32_t ___id_0;

public:
	inline static int32_t get_offset_of_id_0() { return static_cast<int32_t>(offsetof(PropertyName_t75EB843FEA2EC372093479A35C24364D2DF98529, ___id_0)); }
	inline int32_t get_id_0() const { return ___id_0; }
	inline int32_t* get_address_of_id_0() { return &___id_0; }
	inline void set_id_0(int32_t value)
	{
		___id_0 = value;
	}
};


// UnityEngine.Quaternion
struct Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 
{
public:
	// System.Single UnityEngine.Quaternion::x
	float ___x_0;
	// System.Single UnityEngine.Quaternion::y
	float ___y_1;
	// System.Single UnityEngine.Quaternion::z
	float ___z_2;
	// System.Single UnityEngine.Quaternion::w
	float ___w_3;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}

	inline static int32_t get_offset_of_z_2() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___z_2)); }
	inline float get_z_2() const { return ___z_2; }
	inline float* get_address_of_z_2() { return &___z_2; }
	inline void set_z_2(float value)
	{
		___z_2 = value;
	}

	inline static int32_t get_offset_of_w_3() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___w_3)); }
	inline float get_w_3() const { return ___w_3; }
	inline float* get_address_of_w_3() { return &___w_3; }
	inline void set_w_3(float value)
	{
		___w_3 = value;
	}
};

struct Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_StaticFields
{
public:
	// UnityEngine.Quaternion UnityEngine.Quaternion::identityQuaternion
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___identityQuaternion_4;

public:
	inline static int32_t get_offset_of_identityQuaternion_4() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_StaticFields, ___identityQuaternion_4)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_identityQuaternion_4() const { return ___identityQuaternion_4; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_identityQuaternion_4() { return &___identityQuaternion_4; }
	inline void set_identityQuaternion_4(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___identityQuaternion_4 = value;
	}
};


// UnityEngine.Rect
struct Rect_t35B976DE901B5423C11705E156938EA27AB402CE 
{
public:
	// System.Single UnityEngine.Rect::m_XMin
	float ___m_XMin_0;
	// System.Single UnityEngine.Rect::m_YMin
	float ___m_YMin_1;
	// System.Single UnityEngine.Rect::m_Width
	float ___m_Width_2;
	// System.Single UnityEngine.Rect::m_Height
	float ___m_Height_3;

public:
	inline static int32_t get_offset_of_m_XMin_0() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_XMin_0)); }
	inline float get_m_XMin_0() const { return ___m_XMin_0; }
	inline float* get_address_of_m_XMin_0() { return &___m_XMin_0; }
	inline void set_m_XMin_0(float value)
	{
		___m_XMin_0 = value;
	}

	inline static int32_t get_offset_of_m_YMin_1() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_YMin_1)); }
	inline float get_m_YMin_1() const { return ___m_YMin_1; }
	inline float* get_address_of_m_YMin_1() { return &___m_YMin_1; }
	inline void set_m_YMin_1(float value)
	{
		___m_YMin_1 = value;
	}

	inline static int32_t get_offset_of_m_Width_2() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_Width_2)); }
	inline float get_m_Width_2() const { return ___m_Width_2; }
	inline float* get_address_of_m_Width_2() { return &___m_Width_2; }
	inline void set_m_Width_2(float value)
	{
		___m_Width_2 = value;
	}

	inline static int32_t get_offset_of_m_Height_3() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_Height_3)); }
	inline float get_m_Height_3() const { return ___m_Height_3; }
	inline float* get_address_of_m_Height_3() { return &___m_Height_3; }
	inline void set_m_Height_3(float value)
	{
		___m_Height_3 = value;
	}
};


// UnityEngine.UIElements.ComputedStyle
struct ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91 
{
public:
	// UnityEngine.UIElements.VisualElement UnityEngine.UIElements.ComputedStyle::m_Element
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_Element_0;

public:
	inline static int32_t get_offset_of_m_Element_0() { return static_cast<int32_t>(offsetof(ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91, ___m_Element_0)); }
	inline VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * get_m_Element_0() const { return ___m_Element_0; }
	inline VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 ** get_address_of_m_Element_0() { return &___m_Element_0; }
	inline void set_m_Element_0(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * value)
	{
		___m_Element_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Element_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.UIElements.ComputedStyle
struct ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91_marshaled_pinvoke
{
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_Element_0;
};
// Native definition for COM marshalling of UnityEngine.UIElements.ComputedStyle
struct ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91_marshaled_com
{
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_Element_0;
};

// UnityEngine.UIElements.Focusable
struct Focusable_tE75872B8E11B244036F83AB8FFBB20F782F19C6B  : public CallbackEventHandler_t3B7336D446FAD9009A9C1DA1B793885F28D214EB
{
public:
	// System.Boolean UnityEngine.UIElements.Focusable::<focusable>k__BackingField
	bool ___U3CfocusableU3Ek__BackingField_1;
	// System.Boolean UnityEngine.UIElements.Focusable::isIMGUIContainer
	bool ___isIMGUIContainer_2;

public:
	inline static int32_t get_offset_of_U3CfocusableU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(Focusable_tE75872B8E11B244036F83AB8FFBB20F782F19C6B, ___U3CfocusableU3Ek__BackingField_1)); }
	inline bool get_U3CfocusableU3Ek__BackingField_1() const { return ___U3CfocusableU3Ek__BackingField_1; }
	inline bool* get_address_of_U3CfocusableU3Ek__BackingField_1() { return &___U3CfocusableU3Ek__BackingField_1; }
	inline void set_U3CfocusableU3Ek__BackingField_1(bool value)
	{
		___U3CfocusableU3Ek__BackingField_1 = value;
	}

	inline static int32_t get_offset_of_isIMGUIContainer_2() { return static_cast<int32_t>(offsetof(Focusable_tE75872B8E11B244036F83AB8FFBB20F782F19C6B, ___isIMGUIContainer_2)); }
	inline bool get_isIMGUIContainer_2() const { return ___isIMGUIContainer_2; }
	inline bool* get_address_of_isIMGUIContainer_2() { return &___isIMGUIContainer_2; }
	inline void set_isIMGUIContainer_2(bool value)
	{
		___isIMGUIContainer_2 = value;
	}
};


// UnityEngine.UIElements.VisualElement/Hierarchy
struct Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F 
{
public:
	// UnityEngine.UIElements.VisualElement UnityEngine.UIElements.VisualElement/Hierarchy::m_Owner
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_Owner_0;

public:
	inline static int32_t get_offset_of_m_Owner_0() { return static_cast<int32_t>(offsetof(Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F, ___m_Owner_0)); }
	inline VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * get_m_Owner_0() const { return ___m_Owner_0; }
	inline VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 ** get_address_of_m_Owner_0() { return &___m_Owner_0; }
	inline void set_m_Owner_0(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * value)
	{
		___m_Owner_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Owner_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.UIElements.VisualElement/Hierarchy
struct Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F_marshaled_pinvoke
{
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_Owner_0;
};
// Native definition for COM marshalling of UnityEngine.UIElements.VisualElement/Hierarchy
struct Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F_marshaled_com
{
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_Owner_0;
};

// UnityEngine.Vector2
struct Vector2_tA85D2DD88578276CA8A8796756458277E72D073D 
{
public:
	// System.Single UnityEngine.Vector2::x
	float ___x_0;
	// System.Single UnityEngine.Vector2::y
	float ___y_1;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}
};

struct Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields
{
public:
	// UnityEngine.Vector2 UnityEngine.Vector2::zeroVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___zeroVector_2;
	// UnityEngine.Vector2 UnityEngine.Vector2::oneVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___oneVector_3;
	// UnityEngine.Vector2 UnityEngine.Vector2::upVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___upVector_4;
	// UnityEngine.Vector2 UnityEngine.Vector2::downVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___downVector_5;
	// UnityEngine.Vector2 UnityEngine.Vector2::leftVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___leftVector_6;
	// UnityEngine.Vector2 UnityEngine.Vector2::rightVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___rightVector_7;
	// UnityEngine.Vector2 UnityEngine.Vector2::positiveInfinityVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___positiveInfinityVector_8;
	// UnityEngine.Vector2 UnityEngine.Vector2::negativeInfinityVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___negativeInfinityVector_9;

public:
	inline static int32_t get_offset_of_zeroVector_2() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___zeroVector_2)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_zeroVector_2() const { return ___zeroVector_2; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_zeroVector_2() { return &___zeroVector_2; }
	inline void set_zeroVector_2(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___zeroVector_2 = value;
	}

	inline static int32_t get_offset_of_oneVector_3() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___oneVector_3)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_oneVector_3() const { return ___oneVector_3; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_oneVector_3() { return &___oneVector_3; }
	inline void set_oneVector_3(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___oneVector_3 = value;
	}

	inline static int32_t get_offset_of_upVector_4() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___upVector_4)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_upVector_4() const { return ___upVector_4; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_upVector_4() { return &___upVector_4; }
	inline void set_upVector_4(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___upVector_4 = value;
	}

	inline static int32_t get_offset_of_downVector_5() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___downVector_5)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_downVector_5() const { return ___downVector_5; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_downVector_5() { return &___downVector_5; }
	inline void set_downVector_5(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___downVector_5 = value;
	}

	inline static int32_t get_offset_of_leftVector_6() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___leftVector_6)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_leftVector_6() const { return ___leftVector_6; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_leftVector_6() { return &___leftVector_6; }
	inline void set_leftVector_6(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___leftVector_6 = value;
	}

	inline static int32_t get_offset_of_rightVector_7() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___rightVector_7)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_rightVector_7() const { return ___rightVector_7; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_rightVector_7() { return &___rightVector_7; }
	inline void set_rightVector_7(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___rightVector_7 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_8() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___positiveInfinityVector_8)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_positiveInfinityVector_8() const { return ___positiveInfinityVector_8; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_positiveInfinityVector_8() { return &___positiveInfinityVector_8; }
	inline void set_positiveInfinityVector_8(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___positiveInfinityVector_8 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_9() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___negativeInfinityVector_9)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_negativeInfinityVector_9() const { return ___negativeInfinityVector_9; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_negativeInfinityVector_9() { return &___negativeInfinityVector_9; }
	inline void set_negativeInfinityVector_9(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___negativeInfinityVector_9 = value;
	}
};


// UnityEngine.Vector3
struct Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 
{
public:
	// System.Single UnityEngine.Vector3::x
	float ___x_2;
	// System.Single UnityEngine.Vector3::y
	float ___y_3;
	// System.Single UnityEngine.Vector3::z
	float ___z_4;

public:
	inline static int32_t get_offset_of_x_2() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___x_2)); }
	inline float get_x_2() const { return ___x_2; }
	inline float* get_address_of_x_2() { return &___x_2; }
	inline void set_x_2(float value)
	{
		___x_2 = value;
	}

	inline static int32_t get_offset_of_y_3() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___y_3)); }
	inline float get_y_3() const { return ___y_3; }
	inline float* get_address_of_y_3() { return &___y_3; }
	inline void set_y_3(float value)
	{
		___y_3 = value;
	}

	inline static int32_t get_offset_of_z_4() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___z_4)); }
	inline float get_z_4() const { return ___z_4; }
	inline float* get_address_of_z_4() { return &___z_4; }
	inline void set_z_4(float value)
	{
		___z_4 = value;
	}
};

struct Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields
{
public:
	// UnityEngine.Vector3 UnityEngine.Vector3::zeroVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___zeroVector_5;
	// UnityEngine.Vector3 UnityEngine.Vector3::oneVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___oneVector_6;
	// UnityEngine.Vector3 UnityEngine.Vector3::upVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___upVector_7;
	// UnityEngine.Vector3 UnityEngine.Vector3::downVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___downVector_8;
	// UnityEngine.Vector3 UnityEngine.Vector3::leftVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___leftVector_9;
	// UnityEngine.Vector3 UnityEngine.Vector3::rightVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rightVector_10;
	// UnityEngine.Vector3 UnityEngine.Vector3::forwardVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___forwardVector_11;
	// UnityEngine.Vector3 UnityEngine.Vector3::backVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___backVector_12;
	// UnityEngine.Vector3 UnityEngine.Vector3::positiveInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___positiveInfinityVector_13;
	// UnityEngine.Vector3 UnityEngine.Vector3::negativeInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___negativeInfinityVector_14;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___zeroVector_5)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___oneVector_6)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_upVector_7() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___upVector_7)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_upVector_7() const { return ___upVector_7; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_upVector_7() { return &___upVector_7; }
	inline void set_upVector_7(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___upVector_7 = value;
	}

	inline static int32_t get_offset_of_downVector_8() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___downVector_8)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_downVector_8() const { return ___downVector_8; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_downVector_8() { return &___downVector_8; }
	inline void set_downVector_8(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___downVector_8 = value;
	}

	inline static int32_t get_offset_of_leftVector_9() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___leftVector_9)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_leftVector_9() const { return ___leftVector_9; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_leftVector_9() { return &___leftVector_9; }
	inline void set_leftVector_9(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___leftVector_9 = value;
	}

	inline static int32_t get_offset_of_rightVector_10() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___rightVector_10)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_rightVector_10() const { return ___rightVector_10; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_rightVector_10() { return &___rightVector_10; }
	inline void set_rightVector_10(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___rightVector_10 = value;
	}

	inline static int32_t get_offset_of_forwardVector_11() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___forwardVector_11)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_forwardVector_11() const { return ___forwardVector_11; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_forwardVector_11() { return &___forwardVector_11; }
	inline void set_forwardVector_11(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___forwardVector_11 = value;
	}

	inline static int32_t get_offset_of_backVector_12() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___backVector_12)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_backVector_12() const { return ___backVector_12; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_backVector_12() { return &___backVector_12; }
	inline void set_backVector_12(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___backVector_12 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_13() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___positiveInfinityVector_13)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_positiveInfinityVector_13() const { return ___positiveInfinityVector_13; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_positiveInfinityVector_13() { return &___positiveInfinityVector_13; }
	inline void set_positiveInfinityVector_13(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___positiveInfinityVector_13 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_14() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___negativeInfinityVector_14)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_negativeInfinityVector_14() const { return ___negativeInfinityVector_14; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_negativeInfinityVector_14() { return &___negativeInfinityVector_14; }
	inline void set_negativeInfinityVector_14(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___negativeInfinityVector_14 = value;
	}
};


// UnityEngine.WaitForSeconds
struct WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8  : public YieldInstruction_t836035AC7BD07A3C7909F7AD2A5B42DE99D91C44
{
public:
	// System.Single UnityEngine.WaitForSeconds::m_Seconds
	float ___m_Seconds_0;

public:
	inline static int32_t get_offset_of_m_Seconds_0() { return static_cast<int32_t>(offsetof(WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8, ___m_Seconds_0)); }
	inline float get_m_Seconds_0() const { return ___m_Seconds_0; }
	inline float* get_address_of_m_Seconds_0() { return &___m_Seconds_0; }
	inline void set_m_Seconds_0(float value)
	{
		___m_Seconds_0 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.WaitForSeconds
struct WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_marshaled_pinvoke : public YieldInstruction_t836035AC7BD07A3C7909F7AD2A5B42DE99D91C44_marshaled_pinvoke
{
	float ___m_Seconds_0;
};
// Native definition for COM marshalling of UnityEngine.WaitForSeconds
struct WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_marshaled_com : public YieldInstruction_t836035AC7BD07A3C7909F7AD2A5B42DE99D91C44_marshaled_com
{
	float ___m_Seconds_0;
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

// System.Int32Enum
struct Int32Enum_t6312CE4586C17FE2E2E513D2E7655B574F10FDCD 
{
public:
	// System.Int32 System.Int32Enum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Int32Enum_t6312CE4586C17FE2E2E513D2E7655B574F10FDCD, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Event
struct Event_t187FF6A6B357447B83EC2064823EE0AEC5263210  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.Event::m_Ptr
	intptr_t ___m_Ptr_0;

public:
	inline static int32_t get_offset_of_m_Ptr_0() { return static_cast<int32_t>(offsetof(Event_t187FF6A6B357447B83EC2064823EE0AEC5263210, ___m_Ptr_0)); }
	inline intptr_t get_m_Ptr_0() const { return ___m_Ptr_0; }
	inline intptr_t* get_address_of_m_Ptr_0() { return &___m_Ptr_0; }
	inline void set_m_Ptr_0(intptr_t value)
	{
		___m_Ptr_0 = value;
	}
};

struct Event_t187FF6A6B357447B83EC2064823EE0AEC5263210_StaticFields
{
public:
	// UnityEngine.Event UnityEngine.Event::s_Current
	Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * ___s_Current_1;
	// UnityEngine.Event UnityEngine.Event::s_MasterEvent
	Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * ___s_MasterEvent_2;

public:
	inline static int32_t get_offset_of_s_Current_1() { return static_cast<int32_t>(offsetof(Event_t187FF6A6B357447B83EC2064823EE0AEC5263210_StaticFields, ___s_Current_1)); }
	inline Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * get_s_Current_1() const { return ___s_Current_1; }
	inline Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 ** get_address_of_s_Current_1() { return &___s_Current_1; }
	inline void set_s_Current_1(Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * value)
	{
		___s_Current_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Current_1), (void*)value);
	}

	inline static int32_t get_offset_of_s_MasterEvent_2() { return static_cast<int32_t>(offsetof(Event_t187FF6A6B357447B83EC2064823EE0AEC5263210_StaticFields, ___s_MasterEvent_2)); }
	inline Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * get_s_MasterEvent_2() const { return ___s_MasterEvent_2; }
	inline Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 ** get_address_of_s_MasterEvent_2() { return &___s_MasterEvent_2; }
	inline void set_s_MasterEvent_2(Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * value)
	{
		___s_MasterEvent_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_MasterEvent_2), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Event
struct Event_t187FF6A6B357447B83EC2064823EE0AEC5263210_marshaled_pinvoke
{
	intptr_t ___m_Ptr_0;
};
// Native definition for COM marshalling of UnityEngine.Event
struct Event_t187FF6A6B357447B83EC2064823EE0AEC5263210_marshaled_com
{
	intptr_t ___m_Ptr_0;
};

// UnityEngine.EventModifiers
struct EventModifiers_tC34E3018F3697001F894187AF6E9E63D7E203061 
{
public:
	// System.Int32 UnityEngine.EventModifiers::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(EventModifiers_tC34E3018F3697001F894187AF6E9E63D7E203061, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.EventType
struct EventType_t3D3937E705A4506226002DAB22071B7B181DA57B 
{
public:
	// System.Int32 UnityEngine.EventType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(EventType_t3D3937E705A4506226002DAB22071B7B181DA57B, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.PointerType
struct PointerType_tFE78E7B29562C9813141CA1708800D39B0F3931D 
{
public:
	// System.Int32 UnityEngine.PointerType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(PointerType_tFE78E7B29562C9813141CA1708800D39B0F3931D, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.UIElements.EventBase/EventPropagation
struct EventPropagation_tA9A7EB847940ABD70842FA9EBA217C32F977DA20 
{
public:
	// System.Int32 UnityEngine.UIElements.EventBase/EventPropagation::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(EventPropagation_tA9A7EB847940ABD70842FA9EBA217C32F977DA20, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.UIElements.EventBase/LifeCycleStatus
struct LifeCycleStatus_t261998375F205A68CDAE646A82B1A16389926883 
{
public:
	// System.Int32 UnityEngine.UIElements.EventBase/LifeCycleStatus::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(LifeCycleStatus_t261998375F205A68CDAE646A82B1A16389926883, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.UIElements.PickingMode
struct PickingMode_t50B176123A8C1CFF46840654D7AFA06EC6EDD529 
{
public:
	// System.Int32 UnityEngine.UIElements.PickingMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(PickingMode_t50B176123A8C1CFF46840654D7AFA06EC6EDD529, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.UIElements.PropagationPhase
struct PropagationPhase_tF216B8A0984294D04F73CC36443D006EB659D9FC 
{
public:
	// System.Int32 UnityEngine.UIElements.PropagationPhase::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(PropagationPhase_tF216B8A0984294D04F73CC36443D006EB659D9FC, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.UIElements.PseudoStates
struct PseudoStates_t047BCA0D8B56DC2E0A08F001C9C96BB82BDD9545 
{
public:
	// System.Int32 UnityEngine.UIElements.PseudoStates::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(PseudoStates_t047BCA0D8B56DC2E0A08F001C9C96BB82BDD9545, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.UIElements.StyleKeyword
struct StyleKeyword_t5C285A4249A1A7A807C1B4D2AAF5D1350B0A3560 
{
public:
	// System.Int32 UnityEngine.UIElements.StyleKeyword::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(StyleKeyword_t5C285A4249A1A7A807C1B4D2AAF5D1350B0A3560, ___value___2)); }
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


// UnityEngine.UIElements.EventBase
struct EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD  : public RuntimeObject
{
public:
	// System.Int64 UnityEngine.UIElements.EventBase::<timestamp>k__BackingField
	int64_t ___U3CtimestampU3Ek__BackingField_2;
	// System.UInt64 UnityEngine.UIElements.EventBase::<eventId>k__BackingField
	uint64_t ___U3CeventIdU3Ek__BackingField_3;
	// System.UInt64 UnityEngine.UIElements.EventBase::<triggerEventId>k__BackingField
	uint64_t ___U3CtriggerEventIdU3Ek__BackingField_4;
	// UnityEngine.UIElements.EventBase/EventPropagation UnityEngine.UIElements.EventBase::<propagation>k__BackingField
	int32_t ___U3CpropagationU3Ek__BackingField_5;
	// UnityEngine.UIElements.PropagationPaths UnityEngine.UIElements.EventBase::m_Path
	PropagationPaths_t59C288865B38FA5B8E17CA28D79FC4E1C58CE10D * ___m_Path_6;
	// UnityEngine.UIElements.EventBase/LifeCycleStatus UnityEngine.UIElements.EventBase::<lifeCycleStatus>k__BackingField
	int32_t ___U3ClifeCycleStatusU3Ek__BackingField_7;
	// UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.EventBase::<leafTarget>k__BackingField
	RuntimeObject* ___U3CleafTargetU3Ek__BackingField_8;
	// UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.EventBase::m_Target
	RuntimeObject* ___m_Target_9;
	// System.Collections.Generic.List`1<UnityEngine.UIElements.IEventHandler> UnityEngine.UIElements.EventBase::<skipElements>k__BackingField
	List_1_tC6DEF7083E8206203302B1B25E23947E49BBD537 * ___U3CskipElementsU3Ek__BackingField_10;
	// UnityEngine.UIElements.PropagationPhase UnityEngine.UIElements.EventBase::<propagationPhase>k__BackingField
	int32_t ___U3CpropagationPhaseU3Ek__BackingField_11;
	// UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.EventBase::m_CurrentTarget
	RuntimeObject* ___m_CurrentTarget_12;
	// UnityEngine.Event UnityEngine.UIElements.EventBase::m_ImguiEvent
	Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * ___m_ImguiEvent_13;
	// UnityEngine.Vector2 UnityEngine.UIElements.EventBase::<originalMousePosition>k__BackingField
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___U3CoriginalMousePositionU3Ek__BackingField_14;

public:
	inline static int32_t get_offset_of_U3CtimestampU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CtimestampU3Ek__BackingField_2)); }
	inline int64_t get_U3CtimestampU3Ek__BackingField_2() const { return ___U3CtimestampU3Ek__BackingField_2; }
	inline int64_t* get_address_of_U3CtimestampU3Ek__BackingField_2() { return &___U3CtimestampU3Ek__BackingField_2; }
	inline void set_U3CtimestampU3Ek__BackingField_2(int64_t value)
	{
		___U3CtimestampU3Ek__BackingField_2 = value;
	}

	inline static int32_t get_offset_of_U3CeventIdU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CeventIdU3Ek__BackingField_3)); }
	inline uint64_t get_U3CeventIdU3Ek__BackingField_3() const { return ___U3CeventIdU3Ek__BackingField_3; }
	inline uint64_t* get_address_of_U3CeventIdU3Ek__BackingField_3() { return &___U3CeventIdU3Ek__BackingField_3; }
	inline void set_U3CeventIdU3Ek__BackingField_3(uint64_t value)
	{
		___U3CeventIdU3Ek__BackingField_3 = value;
	}

	inline static int32_t get_offset_of_U3CtriggerEventIdU3Ek__BackingField_4() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CtriggerEventIdU3Ek__BackingField_4)); }
	inline uint64_t get_U3CtriggerEventIdU3Ek__BackingField_4() const { return ___U3CtriggerEventIdU3Ek__BackingField_4; }
	inline uint64_t* get_address_of_U3CtriggerEventIdU3Ek__BackingField_4() { return &___U3CtriggerEventIdU3Ek__BackingField_4; }
	inline void set_U3CtriggerEventIdU3Ek__BackingField_4(uint64_t value)
	{
		___U3CtriggerEventIdU3Ek__BackingField_4 = value;
	}

	inline static int32_t get_offset_of_U3CpropagationU3Ek__BackingField_5() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CpropagationU3Ek__BackingField_5)); }
	inline int32_t get_U3CpropagationU3Ek__BackingField_5() const { return ___U3CpropagationU3Ek__BackingField_5; }
	inline int32_t* get_address_of_U3CpropagationU3Ek__BackingField_5() { return &___U3CpropagationU3Ek__BackingField_5; }
	inline void set_U3CpropagationU3Ek__BackingField_5(int32_t value)
	{
		___U3CpropagationU3Ek__BackingField_5 = value;
	}

	inline static int32_t get_offset_of_m_Path_6() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___m_Path_6)); }
	inline PropagationPaths_t59C288865B38FA5B8E17CA28D79FC4E1C58CE10D * get_m_Path_6() const { return ___m_Path_6; }
	inline PropagationPaths_t59C288865B38FA5B8E17CA28D79FC4E1C58CE10D ** get_address_of_m_Path_6() { return &___m_Path_6; }
	inline void set_m_Path_6(PropagationPaths_t59C288865B38FA5B8E17CA28D79FC4E1C58CE10D * value)
	{
		___m_Path_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Path_6), (void*)value);
	}

	inline static int32_t get_offset_of_U3ClifeCycleStatusU3Ek__BackingField_7() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3ClifeCycleStatusU3Ek__BackingField_7)); }
	inline int32_t get_U3ClifeCycleStatusU3Ek__BackingField_7() const { return ___U3ClifeCycleStatusU3Ek__BackingField_7; }
	inline int32_t* get_address_of_U3ClifeCycleStatusU3Ek__BackingField_7() { return &___U3ClifeCycleStatusU3Ek__BackingField_7; }
	inline void set_U3ClifeCycleStatusU3Ek__BackingField_7(int32_t value)
	{
		___U3ClifeCycleStatusU3Ek__BackingField_7 = value;
	}

	inline static int32_t get_offset_of_U3CleafTargetU3Ek__BackingField_8() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CleafTargetU3Ek__BackingField_8)); }
	inline RuntimeObject* get_U3CleafTargetU3Ek__BackingField_8() const { return ___U3CleafTargetU3Ek__BackingField_8; }
	inline RuntimeObject** get_address_of_U3CleafTargetU3Ek__BackingField_8() { return &___U3CleafTargetU3Ek__BackingField_8; }
	inline void set_U3CleafTargetU3Ek__BackingField_8(RuntimeObject* value)
	{
		___U3CleafTargetU3Ek__BackingField_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CleafTargetU3Ek__BackingField_8), (void*)value);
	}

	inline static int32_t get_offset_of_m_Target_9() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___m_Target_9)); }
	inline RuntimeObject* get_m_Target_9() const { return ___m_Target_9; }
	inline RuntimeObject** get_address_of_m_Target_9() { return &___m_Target_9; }
	inline void set_m_Target_9(RuntimeObject* value)
	{
		___m_Target_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Target_9), (void*)value);
	}

	inline static int32_t get_offset_of_U3CskipElementsU3Ek__BackingField_10() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CskipElementsU3Ek__BackingField_10)); }
	inline List_1_tC6DEF7083E8206203302B1B25E23947E49BBD537 * get_U3CskipElementsU3Ek__BackingField_10() const { return ___U3CskipElementsU3Ek__BackingField_10; }
	inline List_1_tC6DEF7083E8206203302B1B25E23947E49BBD537 ** get_address_of_U3CskipElementsU3Ek__BackingField_10() { return &___U3CskipElementsU3Ek__BackingField_10; }
	inline void set_U3CskipElementsU3Ek__BackingField_10(List_1_tC6DEF7083E8206203302B1B25E23947E49BBD537 * value)
	{
		___U3CskipElementsU3Ek__BackingField_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CskipElementsU3Ek__BackingField_10), (void*)value);
	}

	inline static int32_t get_offset_of_U3CpropagationPhaseU3Ek__BackingField_11() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CpropagationPhaseU3Ek__BackingField_11)); }
	inline int32_t get_U3CpropagationPhaseU3Ek__BackingField_11() const { return ___U3CpropagationPhaseU3Ek__BackingField_11; }
	inline int32_t* get_address_of_U3CpropagationPhaseU3Ek__BackingField_11() { return &___U3CpropagationPhaseU3Ek__BackingField_11; }
	inline void set_U3CpropagationPhaseU3Ek__BackingField_11(int32_t value)
	{
		___U3CpropagationPhaseU3Ek__BackingField_11 = value;
	}

	inline static int32_t get_offset_of_m_CurrentTarget_12() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___m_CurrentTarget_12)); }
	inline RuntimeObject* get_m_CurrentTarget_12() const { return ___m_CurrentTarget_12; }
	inline RuntimeObject** get_address_of_m_CurrentTarget_12() { return &___m_CurrentTarget_12; }
	inline void set_m_CurrentTarget_12(RuntimeObject* value)
	{
		___m_CurrentTarget_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_CurrentTarget_12), (void*)value);
	}

	inline static int32_t get_offset_of_m_ImguiEvent_13() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___m_ImguiEvent_13)); }
	inline Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * get_m_ImguiEvent_13() const { return ___m_ImguiEvent_13; }
	inline Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 ** get_address_of_m_ImguiEvent_13() { return &___m_ImguiEvent_13; }
	inline void set_m_ImguiEvent_13(Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * value)
	{
		___m_ImguiEvent_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ImguiEvent_13), (void*)value);
	}

	inline static int32_t get_offset_of_U3CoriginalMousePositionU3Ek__BackingField_14() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD, ___U3CoriginalMousePositionU3Ek__BackingField_14)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_U3CoriginalMousePositionU3Ek__BackingField_14() const { return ___U3CoriginalMousePositionU3Ek__BackingField_14; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_U3CoriginalMousePositionU3Ek__BackingField_14() { return &___U3CoriginalMousePositionU3Ek__BackingField_14; }
	inline void set_U3CoriginalMousePositionU3Ek__BackingField_14(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___U3CoriginalMousePositionU3Ek__BackingField_14 = value;
	}
};

struct EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD_StaticFields
{
public:
	// System.Int64 UnityEngine.UIElements.EventBase::s_LastTypeId
	int64_t ___s_LastTypeId_0;
	// System.UInt64 UnityEngine.UIElements.EventBase::s_NextEventId
	uint64_t ___s_NextEventId_1;

public:
	inline static int32_t get_offset_of_s_LastTypeId_0() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD_StaticFields, ___s_LastTypeId_0)); }
	inline int64_t get_s_LastTypeId_0() const { return ___s_LastTypeId_0; }
	inline int64_t* get_address_of_s_LastTypeId_0() { return &___s_LastTypeId_0; }
	inline void set_s_LastTypeId_0(int64_t value)
	{
		___s_LastTypeId_0 = value;
	}

	inline static int32_t get_offset_of_s_NextEventId_1() { return static_cast<int32_t>(offsetof(EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD_StaticFields, ___s_NextEventId_1)); }
	inline uint64_t get_s_NextEventId_1() const { return ___s_NextEventId_1; }
	inline uint64_t* get_address_of_s_NextEventId_1() { return &___s_NextEventId_1; }
	inline void set_s_NextEventId_1(uint64_t value)
	{
		___s_NextEventId_1 = value;
	}
};


// UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>
struct StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C 
{
public:
	// UnityEngine.UIElements.StyleKeyword UnityEngine.UIElements.StyleEnum`1::m_Keyword
	int32_t ___m_Keyword_0;
	// T UnityEngine.UIElements.StyleEnum`1::m_Value
	int32_t ___m_Value_1;
	// System.Int32 UnityEngine.UIElements.StyleEnum`1::m_Specificity
	int32_t ___m_Specificity_2;

public:
	inline static int32_t get_offset_of_m_Keyword_0() { return static_cast<int32_t>(offsetof(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C, ___m_Keyword_0)); }
	inline int32_t get_m_Keyword_0() const { return ___m_Keyword_0; }
	inline int32_t* get_address_of_m_Keyword_0() { return &___m_Keyword_0; }
	inline void set_m_Keyword_0(int32_t value)
	{
		___m_Keyword_0 = value;
	}

	inline static int32_t get_offset_of_m_Value_1() { return static_cast<int32_t>(offsetof(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C, ___m_Value_1)); }
	inline int32_t get_m_Value_1() const { return ___m_Value_1; }
	inline int32_t* get_address_of_m_Value_1() { return &___m_Value_1; }
	inline void set_m_Value_1(int32_t value)
	{
		___m_Value_1 = value;
	}

	inline static int32_t get_offset_of_m_Specificity_2() { return static_cast<int32_t>(offsetof(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C, ___m_Specificity_2)); }
	inline int32_t get_m_Specificity_2() const { return ___m_Specificity_2; }
	inline int32_t* get_address_of_m_Specificity_2() { return &___m_Specificity_2; }
	inline void set_m_Specificity_2(int32_t value)
	{
		___m_Specificity_2 = value;
	}
};


// UnityEngine.UIElements.VisualElement
struct VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57  : public Focusable_tE75872B8E11B244036F83AB8FFBB20F782F19C6B
{
public:
	// System.Boolean UnityEngine.UIElements.VisualElement::<isCompositeRoot>k__BackingField
	bool ___U3CisCompositeRootU3Ek__BackingField_3;
	// UnityEngine.Vector3 UnityEngine.UIElements.VisualElement::m_Position
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_Position_7;
	// UnityEngine.Quaternion UnityEngine.UIElements.VisualElement::m_Rotation
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___m_Rotation_8;
	// UnityEngine.Vector3 UnityEngine.UIElements.VisualElement::m_Scale
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_Scale_9;
	// System.Boolean UnityEngine.UIElements.VisualElement::<isLayoutManual>k__BackingField
	bool ___U3CisLayoutManualU3Ek__BackingField_10;
	// UnityEngine.Rect UnityEngine.UIElements.VisualElement::m_Layout
	Rect_t35B976DE901B5423C11705E156938EA27AB402CE  ___m_Layout_11;
	// System.Boolean UnityEngine.UIElements.VisualElement::isBoundingBoxDirty
	bool ___isBoundingBoxDirty_12;
	// UnityEngine.Rect UnityEngine.UIElements.VisualElement::m_BoundingBox
	Rect_t35B976DE901B5423C11705E156938EA27AB402CE  ___m_BoundingBox_13;
	// System.Boolean UnityEngine.UIElements.VisualElement::isWorldBoundingBoxDirty
	bool ___isWorldBoundingBoxDirty_14;
	// UnityEngine.Rect UnityEngine.UIElements.VisualElement::m_WorldBoundingBox
	Rect_t35B976DE901B5423C11705E156938EA27AB402CE  ___m_WorldBoundingBox_15;
	// System.Boolean UnityEngine.UIElements.VisualElement::<isWorldTransformDirty>k__BackingField
	bool ___U3CisWorldTransformDirtyU3Ek__BackingField_16;
	// System.Boolean UnityEngine.UIElements.VisualElement::<isWorldTransformInverseDirty>k__BackingField
	bool ___U3CisWorldTransformInverseDirtyU3Ek__BackingField_17;
	// UnityEngine.Matrix4x4 UnityEngine.UIElements.VisualElement::m_WorldTransformCache
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___m_WorldTransformCache_18;
	// UnityEngine.Matrix4x4 UnityEngine.UIElements.VisualElement::m_WorldTransformInverseCache
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___m_WorldTransformInverseCache_19;
	// UnityEngine.UIElements.PseudoStates UnityEngine.UIElements.VisualElement::m_PseudoStates
	int32_t ___m_PseudoStates_21;
	// UnityEngine.UIElements.PickingMode UnityEngine.UIElements.VisualElement::<pickingMode>k__BackingField
	int32_t ___U3CpickingModeU3Ek__BackingField_22;
	// UnityEngine.Yoga.YogaNode UnityEngine.UIElements.VisualElement::<yogaNode>k__BackingField
	YogaNode_tFCB801A447DAC7A335C686ABC5941A4357102A2C * ___U3CyogaNodeU3Ek__BackingField_23;
	// UnityEngine.UIElements.StyleSheets.VisualElementStylesData UnityEngine.UIElements.VisualElement::m_Style
	VisualElementStylesData_tA65DA46967965C0F2B590B64C424DBCE087CF867 * ___m_Style_24;
	// UnityEngine.UIElements.StyleSheets.InheritedStylesData UnityEngine.UIElements.VisualElement::m_InheritedStylesData
	InheritedStylesData_t03E6A5F7D9F2C86527C534C7A1A76EBAA039AE2E * ___m_InheritedStylesData_25;
	// UnityEngine.UIElements.ComputedStyle UnityEngine.UIElements.VisualElement::<computedStyle>k__BackingField
	ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91  ___U3CcomputedStyleU3Ek__BackingField_26;
	// System.Int32 UnityEngine.UIElements.VisualElement::imguiContainerDescendantCount
	int32_t ___imguiContainerDescendantCount_27;
	// UnityEngine.UIElements.VisualElement/Hierarchy UnityEngine.UIElements.VisualElement::<hierarchy>k__BackingField
	Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F  ___U3ChierarchyU3Ek__BackingField_28;
	// UnityEngine.UIElements.VisualElement UnityEngine.UIElements.VisualElement::m_PhysicalParent
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___m_PhysicalParent_29;
	// System.Collections.Generic.List`1<UnityEngine.UIElements.VisualElement> UnityEngine.UIElements.VisualElement::m_Children
	List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 * ___m_Children_31;
	// UnityEngine.UIElements.BaseVisualElementPanel UnityEngine.UIElements.VisualElement::<elementPanel>k__BackingField
	BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * ___U3CelementPanelU3Ek__BackingField_32;

public:
	inline static int32_t get_offset_of_U3CisCompositeRootU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CisCompositeRootU3Ek__BackingField_3)); }
	inline bool get_U3CisCompositeRootU3Ek__BackingField_3() const { return ___U3CisCompositeRootU3Ek__BackingField_3; }
	inline bool* get_address_of_U3CisCompositeRootU3Ek__BackingField_3() { return &___U3CisCompositeRootU3Ek__BackingField_3; }
	inline void set_U3CisCompositeRootU3Ek__BackingField_3(bool value)
	{
		___U3CisCompositeRootU3Ek__BackingField_3 = value;
	}

	inline static int32_t get_offset_of_m_Position_7() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_Position_7)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_Position_7() const { return ___m_Position_7; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_Position_7() { return &___m_Position_7; }
	inline void set_m_Position_7(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_Position_7 = value;
	}

	inline static int32_t get_offset_of_m_Rotation_8() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_Rotation_8)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_m_Rotation_8() const { return ___m_Rotation_8; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_m_Rotation_8() { return &___m_Rotation_8; }
	inline void set_m_Rotation_8(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___m_Rotation_8 = value;
	}

	inline static int32_t get_offset_of_m_Scale_9() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_Scale_9)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_Scale_9() const { return ___m_Scale_9; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_Scale_9() { return &___m_Scale_9; }
	inline void set_m_Scale_9(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_Scale_9 = value;
	}

	inline static int32_t get_offset_of_U3CisLayoutManualU3Ek__BackingField_10() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CisLayoutManualU3Ek__BackingField_10)); }
	inline bool get_U3CisLayoutManualU3Ek__BackingField_10() const { return ___U3CisLayoutManualU3Ek__BackingField_10; }
	inline bool* get_address_of_U3CisLayoutManualU3Ek__BackingField_10() { return &___U3CisLayoutManualU3Ek__BackingField_10; }
	inline void set_U3CisLayoutManualU3Ek__BackingField_10(bool value)
	{
		___U3CisLayoutManualU3Ek__BackingField_10 = value;
	}

	inline static int32_t get_offset_of_m_Layout_11() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_Layout_11)); }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE  get_m_Layout_11() const { return ___m_Layout_11; }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE * get_address_of_m_Layout_11() { return &___m_Layout_11; }
	inline void set_m_Layout_11(Rect_t35B976DE901B5423C11705E156938EA27AB402CE  value)
	{
		___m_Layout_11 = value;
	}

	inline static int32_t get_offset_of_isBoundingBoxDirty_12() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___isBoundingBoxDirty_12)); }
	inline bool get_isBoundingBoxDirty_12() const { return ___isBoundingBoxDirty_12; }
	inline bool* get_address_of_isBoundingBoxDirty_12() { return &___isBoundingBoxDirty_12; }
	inline void set_isBoundingBoxDirty_12(bool value)
	{
		___isBoundingBoxDirty_12 = value;
	}

	inline static int32_t get_offset_of_m_BoundingBox_13() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_BoundingBox_13)); }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE  get_m_BoundingBox_13() const { return ___m_BoundingBox_13; }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE * get_address_of_m_BoundingBox_13() { return &___m_BoundingBox_13; }
	inline void set_m_BoundingBox_13(Rect_t35B976DE901B5423C11705E156938EA27AB402CE  value)
	{
		___m_BoundingBox_13 = value;
	}

	inline static int32_t get_offset_of_isWorldBoundingBoxDirty_14() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___isWorldBoundingBoxDirty_14)); }
	inline bool get_isWorldBoundingBoxDirty_14() const { return ___isWorldBoundingBoxDirty_14; }
	inline bool* get_address_of_isWorldBoundingBoxDirty_14() { return &___isWorldBoundingBoxDirty_14; }
	inline void set_isWorldBoundingBoxDirty_14(bool value)
	{
		___isWorldBoundingBoxDirty_14 = value;
	}

	inline static int32_t get_offset_of_m_WorldBoundingBox_15() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_WorldBoundingBox_15)); }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE  get_m_WorldBoundingBox_15() const { return ___m_WorldBoundingBox_15; }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE * get_address_of_m_WorldBoundingBox_15() { return &___m_WorldBoundingBox_15; }
	inline void set_m_WorldBoundingBox_15(Rect_t35B976DE901B5423C11705E156938EA27AB402CE  value)
	{
		___m_WorldBoundingBox_15 = value;
	}

	inline static int32_t get_offset_of_U3CisWorldTransformDirtyU3Ek__BackingField_16() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CisWorldTransformDirtyU3Ek__BackingField_16)); }
	inline bool get_U3CisWorldTransformDirtyU3Ek__BackingField_16() const { return ___U3CisWorldTransformDirtyU3Ek__BackingField_16; }
	inline bool* get_address_of_U3CisWorldTransformDirtyU3Ek__BackingField_16() { return &___U3CisWorldTransformDirtyU3Ek__BackingField_16; }
	inline void set_U3CisWorldTransformDirtyU3Ek__BackingField_16(bool value)
	{
		___U3CisWorldTransformDirtyU3Ek__BackingField_16 = value;
	}

	inline static int32_t get_offset_of_U3CisWorldTransformInverseDirtyU3Ek__BackingField_17() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CisWorldTransformInverseDirtyU3Ek__BackingField_17)); }
	inline bool get_U3CisWorldTransformInverseDirtyU3Ek__BackingField_17() const { return ___U3CisWorldTransformInverseDirtyU3Ek__BackingField_17; }
	inline bool* get_address_of_U3CisWorldTransformInverseDirtyU3Ek__BackingField_17() { return &___U3CisWorldTransformInverseDirtyU3Ek__BackingField_17; }
	inline void set_U3CisWorldTransformInverseDirtyU3Ek__BackingField_17(bool value)
	{
		___U3CisWorldTransformInverseDirtyU3Ek__BackingField_17 = value;
	}

	inline static int32_t get_offset_of_m_WorldTransformCache_18() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_WorldTransformCache_18)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_m_WorldTransformCache_18() const { return ___m_WorldTransformCache_18; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_m_WorldTransformCache_18() { return &___m_WorldTransformCache_18; }
	inline void set_m_WorldTransformCache_18(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___m_WorldTransformCache_18 = value;
	}

	inline static int32_t get_offset_of_m_WorldTransformInverseCache_19() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_WorldTransformInverseCache_19)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_m_WorldTransformInverseCache_19() const { return ___m_WorldTransformInverseCache_19; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_m_WorldTransformInverseCache_19() { return &___m_WorldTransformInverseCache_19; }
	inline void set_m_WorldTransformInverseCache_19(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___m_WorldTransformInverseCache_19 = value;
	}

	inline static int32_t get_offset_of_m_PseudoStates_21() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_PseudoStates_21)); }
	inline int32_t get_m_PseudoStates_21() const { return ___m_PseudoStates_21; }
	inline int32_t* get_address_of_m_PseudoStates_21() { return &___m_PseudoStates_21; }
	inline void set_m_PseudoStates_21(int32_t value)
	{
		___m_PseudoStates_21 = value;
	}

	inline static int32_t get_offset_of_U3CpickingModeU3Ek__BackingField_22() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CpickingModeU3Ek__BackingField_22)); }
	inline int32_t get_U3CpickingModeU3Ek__BackingField_22() const { return ___U3CpickingModeU3Ek__BackingField_22; }
	inline int32_t* get_address_of_U3CpickingModeU3Ek__BackingField_22() { return &___U3CpickingModeU3Ek__BackingField_22; }
	inline void set_U3CpickingModeU3Ek__BackingField_22(int32_t value)
	{
		___U3CpickingModeU3Ek__BackingField_22 = value;
	}

	inline static int32_t get_offset_of_U3CyogaNodeU3Ek__BackingField_23() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CyogaNodeU3Ek__BackingField_23)); }
	inline YogaNode_tFCB801A447DAC7A335C686ABC5941A4357102A2C * get_U3CyogaNodeU3Ek__BackingField_23() const { return ___U3CyogaNodeU3Ek__BackingField_23; }
	inline YogaNode_tFCB801A447DAC7A335C686ABC5941A4357102A2C ** get_address_of_U3CyogaNodeU3Ek__BackingField_23() { return &___U3CyogaNodeU3Ek__BackingField_23; }
	inline void set_U3CyogaNodeU3Ek__BackingField_23(YogaNode_tFCB801A447DAC7A335C686ABC5941A4357102A2C * value)
	{
		___U3CyogaNodeU3Ek__BackingField_23 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CyogaNodeU3Ek__BackingField_23), (void*)value);
	}

	inline static int32_t get_offset_of_m_Style_24() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_Style_24)); }
	inline VisualElementStylesData_tA65DA46967965C0F2B590B64C424DBCE087CF867 * get_m_Style_24() const { return ___m_Style_24; }
	inline VisualElementStylesData_tA65DA46967965C0F2B590B64C424DBCE087CF867 ** get_address_of_m_Style_24() { return &___m_Style_24; }
	inline void set_m_Style_24(VisualElementStylesData_tA65DA46967965C0F2B590B64C424DBCE087CF867 * value)
	{
		___m_Style_24 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Style_24), (void*)value);
	}

	inline static int32_t get_offset_of_m_InheritedStylesData_25() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_InheritedStylesData_25)); }
	inline InheritedStylesData_t03E6A5F7D9F2C86527C534C7A1A76EBAA039AE2E * get_m_InheritedStylesData_25() const { return ___m_InheritedStylesData_25; }
	inline InheritedStylesData_t03E6A5F7D9F2C86527C534C7A1A76EBAA039AE2E ** get_address_of_m_InheritedStylesData_25() { return &___m_InheritedStylesData_25; }
	inline void set_m_InheritedStylesData_25(InheritedStylesData_t03E6A5F7D9F2C86527C534C7A1A76EBAA039AE2E * value)
	{
		___m_InheritedStylesData_25 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_InheritedStylesData_25), (void*)value);
	}

	inline static int32_t get_offset_of_U3CcomputedStyleU3Ek__BackingField_26() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CcomputedStyleU3Ek__BackingField_26)); }
	inline ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91  get_U3CcomputedStyleU3Ek__BackingField_26() const { return ___U3CcomputedStyleU3Ek__BackingField_26; }
	inline ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91 * get_address_of_U3CcomputedStyleU3Ek__BackingField_26() { return &___U3CcomputedStyleU3Ek__BackingField_26; }
	inline void set_U3CcomputedStyleU3Ek__BackingField_26(ComputedStyle_tB5824B48158F8503B606231911B3BC842BB81A91  value)
	{
		___U3CcomputedStyleU3Ek__BackingField_26 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___U3CcomputedStyleU3Ek__BackingField_26))->___m_Element_0), (void*)NULL);
	}

	inline static int32_t get_offset_of_imguiContainerDescendantCount_27() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___imguiContainerDescendantCount_27)); }
	inline int32_t get_imguiContainerDescendantCount_27() const { return ___imguiContainerDescendantCount_27; }
	inline int32_t* get_address_of_imguiContainerDescendantCount_27() { return &___imguiContainerDescendantCount_27; }
	inline void set_imguiContainerDescendantCount_27(int32_t value)
	{
		___imguiContainerDescendantCount_27 = value;
	}

	inline static int32_t get_offset_of_U3ChierarchyU3Ek__BackingField_28() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3ChierarchyU3Ek__BackingField_28)); }
	inline Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F  get_U3ChierarchyU3Ek__BackingField_28() const { return ___U3ChierarchyU3Ek__BackingField_28; }
	inline Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F * get_address_of_U3ChierarchyU3Ek__BackingField_28() { return &___U3ChierarchyU3Ek__BackingField_28; }
	inline void set_U3ChierarchyU3Ek__BackingField_28(Hierarchy_tF2E587A276B2AC4CFA66BEAA0C74BD2F6711528F  value)
	{
		___U3ChierarchyU3Ek__BackingField_28 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___U3ChierarchyU3Ek__BackingField_28))->___m_Owner_0), (void*)NULL);
	}

	inline static int32_t get_offset_of_m_PhysicalParent_29() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_PhysicalParent_29)); }
	inline VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * get_m_PhysicalParent_29() const { return ___m_PhysicalParent_29; }
	inline VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 ** get_address_of_m_PhysicalParent_29() { return &___m_PhysicalParent_29; }
	inline void set_m_PhysicalParent_29(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * value)
	{
		___m_PhysicalParent_29 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_PhysicalParent_29), (void*)value);
	}

	inline static int32_t get_offset_of_m_Children_31() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___m_Children_31)); }
	inline List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 * get_m_Children_31() const { return ___m_Children_31; }
	inline List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 ** get_address_of_m_Children_31() { return &___m_Children_31; }
	inline void set_m_Children_31(List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 * value)
	{
		___m_Children_31 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Children_31), (void*)value);
	}

	inline static int32_t get_offset_of_U3CelementPanelU3Ek__BackingField_32() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57, ___U3CelementPanelU3Ek__BackingField_32)); }
	inline BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * get_U3CelementPanelU3Ek__BackingField_32() const { return ___U3CelementPanelU3Ek__BackingField_32; }
	inline BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 ** get_address_of_U3CelementPanelU3Ek__BackingField_32() { return &___U3CelementPanelU3Ek__BackingField_32; }
	inline void set_U3CelementPanelU3Ek__BackingField_32(BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * value)
	{
		___U3CelementPanelU3Ek__BackingField_32 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CelementPanelU3Ek__BackingField_32), (void*)value);
	}
};

struct VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_StaticFields
{
public:
	// System.Collections.Generic.List`1<System.String> UnityEngine.UIElements.VisualElement::s_EmptyClassList
	List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * ___s_EmptyClassList_4;
	// UnityEngine.PropertyName UnityEngine.UIElements.VisualElement::userDataPropertyKey
	PropertyName_t75EB843FEA2EC372093479A35C24364D2DF98529  ___userDataPropertyKey_5;
	// System.String UnityEngine.UIElements.VisualElement::disabledUssClassName
	String_t* ___disabledUssClassName_6;
	// UnityEngine.Rect UnityEngine.UIElements.VisualElement::s_InfiniteRect
	Rect_t35B976DE901B5423C11705E156938EA27AB402CE  ___s_InfiniteRect_20;
	// System.Collections.Generic.List`1<UnityEngine.UIElements.VisualElement> UnityEngine.UIElements.VisualElement::s_EmptyList
	List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 * ___s_EmptyList_30;

public:
	inline static int32_t get_offset_of_s_EmptyClassList_4() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_StaticFields, ___s_EmptyClassList_4)); }
	inline List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * get_s_EmptyClassList_4() const { return ___s_EmptyClassList_4; }
	inline List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 ** get_address_of_s_EmptyClassList_4() { return &___s_EmptyClassList_4; }
	inline void set_s_EmptyClassList_4(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * value)
	{
		___s_EmptyClassList_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_EmptyClassList_4), (void*)value);
	}

	inline static int32_t get_offset_of_userDataPropertyKey_5() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_StaticFields, ___userDataPropertyKey_5)); }
	inline PropertyName_t75EB843FEA2EC372093479A35C24364D2DF98529  get_userDataPropertyKey_5() const { return ___userDataPropertyKey_5; }
	inline PropertyName_t75EB843FEA2EC372093479A35C24364D2DF98529 * get_address_of_userDataPropertyKey_5() { return &___userDataPropertyKey_5; }
	inline void set_userDataPropertyKey_5(PropertyName_t75EB843FEA2EC372093479A35C24364D2DF98529  value)
	{
		___userDataPropertyKey_5 = value;
	}

	inline static int32_t get_offset_of_disabledUssClassName_6() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_StaticFields, ___disabledUssClassName_6)); }
	inline String_t* get_disabledUssClassName_6() const { return ___disabledUssClassName_6; }
	inline String_t** get_address_of_disabledUssClassName_6() { return &___disabledUssClassName_6; }
	inline void set_disabledUssClassName_6(String_t* value)
	{
		___disabledUssClassName_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___disabledUssClassName_6), (void*)value);
	}

	inline static int32_t get_offset_of_s_InfiniteRect_20() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_StaticFields, ___s_InfiniteRect_20)); }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE  get_s_InfiniteRect_20() const { return ___s_InfiniteRect_20; }
	inline Rect_t35B976DE901B5423C11705E156938EA27AB402CE * get_address_of_s_InfiniteRect_20() { return &___s_InfiniteRect_20; }
	inline void set_s_InfiniteRect_20(Rect_t35B976DE901B5423C11705E156938EA27AB402CE  value)
	{
		___s_InfiniteRect_20 = value;
	}

	inline static int32_t get_offset_of_s_EmptyList_30() { return static_cast<int32_t>(offsetof(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_StaticFields, ___s_EmptyList_30)); }
	inline List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 * get_s_EmptyList_30() const { return ___s_EmptyList_30; }
	inline List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 ** get_address_of_s_EmptyList_30() { return &___s_EmptyList_30; }
	inline void set_s_EmptyList_30(List_1_tDEB6BDC3A4648AD90BEEB387B685DE7E030D2756 * value)
	{
		___s_EmptyList_30 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_EmptyList_30), (void*)value);
	}
};


// System.Action`1<System.Object>
struct Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`2<System.Object,System.Object>
struct Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`3<System.Object,System.Object,System.Object>
struct Action_3_tCC14115B7178951118504E7198B7C872630643C5  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`4<System.Object,System.Object,System.Object,System.Object>
struct Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923  : public MulticastDelegate_t
{
public:

public:
};


// System.NotSupportedException
struct NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010  : public SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782
{
public:

public:
};


// UnityEngine.UIElements.EventBase`1<System.Object>
struct EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916  : public EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD
{
public:
	// System.Int32 UnityEngine.UIElements.EventBase`1::m_RefCount
	int32_t ___m_RefCount_17;

public:
	inline static int32_t get_offset_of_m_RefCount_17() { return static_cast<int32_t>(offsetof(EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916, ___m_RefCount_17)); }
	inline int32_t get_m_RefCount_17() const { return ___m_RefCount_17; }
	inline int32_t* get_address_of_m_RefCount_17() { return &___m_RefCount_17; }
	inline void set_m_RefCount_17(int32_t value)
	{
		___m_RefCount_17 = value;
	}
};

struct EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916_StaticFields
{
public:
	// System.Int64 UnityEngine.UIElements.EventBase`1::s_TypeId
	int64_t ___s_TypeId_15;
	// UnityEngine.UIElements.ObjectPool`1<T> UnityEngine.UIElements.EventBase`1::s_Pool
	ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * ___s_Pool_16;

public:
	inline static int32_t get_offset_of_s_TypeId_15() { return static_cast<int32_t>(offsetof(EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916_StaticFields, ___s_TypeId_15)); }
	inline int64_t get_s_TypeId_15() const { return ___s_TypeId_15; }
	inline int64_t* get_address_of_s_TypeId_15() { return &___s_TypeId_15; }
	inline void set_s_TypeId_15(int64_t value)
	{
		___s_TypeId_15 = value;
	}

	inline static int32_t get_offset_of_s_Pool_16() { return static_cast<int32_t>(offsetof(EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916_StaticFields, ___s_Pool_16)); }
	inline ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * get_s_Pool_16() const { return ___s_Pool_16; }
	inline ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E ** get_address_of_s_Pool_16() { return &___s_Pool_16; }
	inline void set_s_Pool_16(ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * value)
	{
		___s_Pool_16 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Pool_16), (void*)value);
	}
};


// UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>
struct PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE  : public EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916
{
public:
	// UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.PointerCaptureEventBase`1::<relatedTarget>k__BackingField
	RuntimeObject* ___U3CrelatedTargetU3Ek__BackingField_18;
	// System.Int32 UnityEngine.UIElements.PointerCaptureEventBase`1::<pointerId>k__BackingField
	int32_t ___U3CpointerIdU3Ek__BackingField_19;

public:
	inline static int32_t get_offset_of_U3CrelatedTargetU3Ek__BackingField_18() { return static_cast<int32_t>(offsetof(PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE, ___U3CrelatedTargetU3Ek__BackingField_18)); }
	inline RuntimeObject* get_U3CrelatedTargetU3Ek__BackingField_18() const { return ___U3CrelatedTargetU3Ek__BackingField_18; }
	inline RuntimeObject** get_address_of_U3CrelatedTargetU3Ek__BackingField_18() { return &___U3CrelatedTargetU3Ek__BackingField_18; }
	inline void set_U3CrelatedTargetU3Ek__BackingField_18(RuntimeObject* value)
	{
		___U3CrelatedTargetU3Ek__BackingField_18 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CrelatedTargetU3Ek__BackingField_18), (void*)value);
	}

	inline static int32_t get_offset_of_U3CpointerIdU3Ek__BackingField_19() { return static_cast<int32_t>(offsetof(PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE, ___U3CpointerIdU3Ek__BackingField_19)); }
	inline int32_t get_U3CpointerIdU3Ek__BackingField_19() const { return ___U3CpointerIdU3Ek__BackingField_19; }
	inline int32_t* get_address_of_U3CpointerIdU3Ek__BackingField_19() { return &___U3CpointerIdU3Ek__BackingField_19; }
	inline void set_U3CpointerIdU3Ek__BackingField_19(int32_t value)
	{
		___U3CpointerIdU3Ek__BackingField_19 = value;
	}
};


// UnityEngine.UIElements.PointerEventBase`1<System.Object>
struct PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634  : public EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916
{
public:
	// System.Int32 UnityEngine.UIElements.PointerEventBase`1::<pointerId>k__BackingField
	int32_t ___U3CpointerIdU3Ek__BackingField_18;
	// System.String UnityEngine.UIElements.PointerEventBase`1::<pointerType>k__BackingField
	String_t* ___U3CpointerTypeU3Ek__BackingField_19;
	// System.Boolean UnityEngine.UIElements.PointerEventBase`1::<isPrimary>k__BackingField
	bool ___U3CisPrimaryU3Ek__BackingField_20;
	// System.Int32 UnityEngine.UIElements.PointerEventBase`1::<button>k__BackingField
	int32_t ___U3CbuttonU3Ek__BackingField_21;
	// System.Int32 UnityEngine.UIElements.PointerEventBase`1::<pressedButtons>k__BackingField
	int32_t ___U3CpressedButtonsU3Ek__BackingField_22;
	// UnityEngine.Vector3 UnityEngine.UIElements.PointerEventBase`1::<position>k__BackingField
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___U3CpositionU3Ek__BackingField_23;
	// UnityEngine.Vector3 UnityEngine.UIElements.PointerEventBase`1::<localPosition>k__BackingField
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___U3ClocalPositionU3Ek__BackingField_24;
	// UnityEngine.Vector3 UnityEngine.UIElements.PointerEventBase`1::<deltaPosition>k__BackingField
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___U3CdeltaPositionU3Ek__BackingField_25;
	// System.Single UnityEngine.UIElements.PointerEventBase`1::<deltaTime>k__BackingField
	float ___U3CdeltaTimeU3Ek__BackingField_26;
	// System.Int32 UnityEngine.UIElements.PointerEventBase`1::<clickCount>k__BackingField
	int32_t ___U3CclickCountU3Ek__BackingField_27;
	// System.Single UnityEngine.UIElements.PointerEventBase`1::<pressure>k__BackingField
	float ___U3CpressureU3Ek__BackingField_28;
	// System.Single UnityEngine.UIElements.PointerEventBase`1::<tangentialPressure>k__BackingField
	float ___U3CtangentialPressureU3Ek__BackingField_29;
	// System.Single UnityEngine.UIElements.PointerEventBase`1::<altitudeAngle>k__BackingField
	float ___U3CaltitudeAngleU3Ek__BackingField_30;
	// System.Single UnityEngine.UIElements.PointerEventBase`1::<azimuthAngle>k__BackingField
	float ___U3CazimuthAngleU3Ek__BackingField_31;
	// System.Single UnityEngine.UIElements.PointerEventBase`1::<twist>k__BackingField
	float ___U3CtwistU3Ek__BackingField_32;
	// UnityEngine.Vector2 UnityEngine.UIElements.PointerEventBase`1::<radius>k__BackingField
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___U3CradiusU3Ek__BackingField_33;
	// UnityEngine.Vector2 UnityEngine.UIElements.PointerEventBase`1::<radiusVariance>k__BackingField
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___U3CradiusVarianceU3Ek__BackingField_34;
	// UnityEngine.EventModifiers UnityEngine.UIElements.PointerEventBase`1::<modifiers>k__BackingField
	int32_t ___U3CmodifiersU3Ek__BackingField_35;
	// System.Boolean UnityEngine.UIElements.PointerEventBase`1::<UnityEngine.UIElements.IPointerEventInternal.triggeredByOS>k__BackingField
	bool ___U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36;
	// System.Boolean UnityEngine.UIElements.PointerEventBase`1::<UnityEngine.UIElements.IPointerEventInternal.recomputeTopElementUnderPointer>k__BackingField
	bool ___U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37;

public:
	inline static int32_t get_offset_of_U3CpointerIdU3Ek__BackingField_18() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CpointerIdU3Ek__BackingField_18)); }
	inline int32_t get_U3CpointerIdU3Ek__BackingField_18() const { return ___U3CpointerIdU3Ek__BackingField_18; }
	inline int32_t* get_address_of_U3CpointerIdU3Ek__BackingField_18() { return &___U3CpointerIdU3Ek__BackingField_18; }
	inline void set_U3CpointerIdU3Ek__BackingField_18(int32_t value)
	{
		___U3CpointerIdU3Ek__BackingField_18 = value;
	}

	inline static int32_t get_offset_of_U3CpointerTypeU3Ek__BackingField_19() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CpointerTypeU3Ek__BackingField_19)); }
	inline String_t* get_U3CpointerTypeU3Ek__BackingField_19() const { return ___U3CpointerTypeU3Ek__BackingField_19; }
	inline String_t** get_address_of_U3CpointerTypeU3Ek__BackingField_19() { return &___U3CpointerTypeU3Ek__BackingField_19; }
	inline void set_U3CpointerTypeU3Ek__BackingField_19(String_t* value)
	{
		___U3CpointerTypeU3Ek__BackingField_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CpointerTypeU3Ek__BackingField_19), (void*)value);
	}

	inline static int32_t get_offset_of_U3CisPrimaryU3Ek__BackingField_20() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CisPrimaryU3Ek__BackingField_20)); }
	inline bool get_U3CisPrimaryU3Ek__BackingField_20() const { return ___U3CisPrimaryU3Ek__BackingField_20; }
	inline bool* get_address_of_U3CisPrimaryU3Ek__BackingField_20() { return &___U3CisPrimaryU3Ek__BackingField_20; }
	inline void set_U3CisPrimaryU3Ek__BackingField_20(bool value)
	{
		___U3CisPrimaryU3Ek__BackingField_20 = value;
	}

	inline static int32_t get_offset_of_U3CbuttonU3Ek__BackingField_21() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CbuttonU3Ek__BackingField_21)); }
	inline int32_t get_U3CbuttonU3Ek__BackingField_21() const { return ___U3CbuttonU3Ek__BackingField_21; }
	inline int32_t* get_address_of_U3CbuttonU3Ek__BackingField_21() { return &___U3CbuttonU3Ek__BackingField_21; }
	inline void set_U3CbuttonU3Ek__BackingField_21(int32_t value)
	{
		___U3CbuttonU3Ek__BackingField_21 = value;
	}

	inline static int32_t get_offset_of_U3CpressedButtonsU3Ek__BackingField_22() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CpressedButtonsU3Ek__BackingField_22)); }
	inline int32_t get_U3CpressedButtonsU3Ek__BackingField_22() const { return ___U3CpressedButtonsU3Ek__BackingField_22; }
	inline int32_t* get_address_of_U3CpressedButtonsU3Ek__BackingField_22() { return &___U3CpressedButtonsU3Ek__BackingField_22; }
	inline void set_U3CpressedButtonsU3Ek__BackingField_22(int32_t value)
	{
		___U3CpressedButtonsU3Ek__BackingField_22 = value;
	}

	inline static int32_t get_offset_of_U3CpositionU3Ek__BackingField_23() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CpositionU3Ek__BackingField_23)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_U3CpositionU3Ek__BackingField_23() const { return ___U3CpositionU3Ek__BackingField_23; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_U3CpositionU3Ek__BackingField_23() { return &___U3CpositionU3Ek__BackingField_23; }
	inline void set_U3CpositionU3Ek__BackingField_23(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___U3CpositionU3Ek__BackingField_23 = value;
	}

	inline static int32_t get_offset_of_U3ClocalPositionU3Ek__BackingField_24() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3ClocalPositionU3Ek__BackingField_24)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_U3ClocalPositionU3Ek__BackingField_24() const { return ___U3ClocalPositionU3Ek__BackingField_24; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_U3ClocalPositionU3Ek__BackingField_24() { return &___U3ClocalPositionU3Ek__BackingField_24; }
	inline void set_U3ClocalPositionU3Ek__BackingField_24(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___U3ClocalPositionU3Ek__BackingField_24 = value;
	}

	inline static int32_t get_offset_of_U3CdeltaPositionU3Ek__BackingField_25() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CdeltaPositionU3Ek__BackingField_25)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_U3CdeltaPositionU3Ek__BackingField_25() const { return ___U3CdeltaPositionU3Ek__BackingField_25; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_U3CdeltaPositionU3Ek__BackingField_25() { return &___U3CdeltaPositionU3Ek__BackingField_25; }
	inline void set_U3CdeltaPositionU3Ek__BackingField_25(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___U3CdeltaPositionU3Ek__BackingField_25 = value;
	}

	inline static int32_t get_offset_of_U3CdeltaTimeU3Ek__BackingField_26() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CdeltaTimeU3Ek__BackingField_26)); }
	inline float get_U3CdeltaTimeU3Ek__BackingField_26() const { return ___U3CdeltaTimeU3Ek__BackingField_26; }
	inline float* get_address_of_U3CdeltaTimeU3Ek__BackingField_26() { return &___U3CdeltaTimeU3Ek__BackingField_26; }
	inline void set_U3CdeltaTimeU3Ek__BackingField_26(float value)
	{
		___U3CdeltaTimeU3Ek__BackingField_26 = value;
	}

	inline static int32_t get_offset_of_U3CclickCountU3Ek__BackingField_27() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CclickCountU3Ek__BackingField_27)); }
	inline int32_t get_U3CclickCountU3Ek__BackingField_27() const { return ___U3CclickCountU3Ek__BackingField_27; }
	inline int32_t* get_address_of_U3CclickCountU3Ek__BackingField_27() { return &___U3CclickCountU3Ek__BackingField_27; }
	inline void set_U3CclickCountU3Ek__BackingField_27(int32_t value)
	{
		___U3CclickCountU3Ek__BackingField_27 = value;
	}

	inline static int32_t get_offset_of_U3CpressureU3Ek__BackingField_28() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CpressureU3Ek__BackingField_28)); }
	inline float get_U3CpressureU3Ek__BackingField_28() const { return ___U3CpressureU3Ek__BackingField_28; }
	inline float* get_address_of_U3CpressureU3Ek__BackingField_28() { return &___U3CpressureU3Ek__BackingField_28; }
	inline void set_U3CpressureU3Ek__BackingField_28(float value)
	{
		___U3CpressureU3Ek__BackingField_28 = value;
	}

	inline static int32_t get_offset_of_U3CtangentialPressureU3Ek__BackingField_29() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CtangentialPressureU3Ek__BackingField_29)); }
	inline float get_U3CtangentialPressureU3Ek__BackingField_29() const { return ___U3CtangentialPressureU3Ek__BackingField_29; }
	inline float* get_address_of_U3CtangentialPressureU3Ek__BackingField_29() { return &___U3CtangentialPressureU3Ek__BackingField_29; }
	inline void set_U3CtangentialPressureU3Ek__BackingField_29(float value)
	{
		___U3CtangentialPressureU3Ek__BackingField_29 = value;
	}

	inline static int32_t get_offset_of_U3CaltitudeAngleU3Ek__BackingField_30() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CaltitudeAngleU3Ek__BackingField_30)); }
	inline float get_U3CaltitudeAngleU3Ek__BackingField_30() const { return ___U3CaltitudeAngleU3Ek__BackingField_30; }
	inline float* get_address_of_U3CaltitudeAngleU3Ek__BackingField_30() { return &___U3CaltitudeAngleU3Ek__BackingField_30; }
	inline void set_U3CaltitudeAngleU3Ek__BackingField_30(float value)
	{
		___U3CaltitudeAngleU3Ek__BackingField_30 = value;
	}

	inline static int32_t get_offset_of_U3CazimuthAngleU3Ek__BackingField_31() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CazimuthAngleU3Ek__BackingField_31)); }
	inline float get_U3CazimuthAngleU3Ek__BackingField_31() const { return ___U3CazimuthAngleU3Ek__BackingField_31; }
	inline float* get_address_of_U3CazimuthAngleU3Ek__BackingField_31() { return &___U3CazimuthAngleU3Ek__BackingField_31; }
	inline void set_U3CazimuthAngleU3Ek__BackingField_31(float value)
	{
		___U3CazimuthAngleU3Ek__BackingField_31 = value;
	}

	inline static int32_t get_offset_of_U3CtwistU3Ek__BackingField_32() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CtwistU3Ek__BackingField_32)); }
	inline float get_U3CtwistU3Ek__BackingField_32() const { return ___U3CtwistU3Ek__BackingField_32; }
	inline float* get_address_of_U3CtwistU3Ek__BackingField_32() { return &___U3CtwistU3Ek__BackingField_32; }
	inline void set_U3CtwistU3Ek__BackingField_32(float value)
	{
		___U3CtwistU3Ek__BackingField_32 = value;
	}

	inline static int32_t get_offset_of_U3CradiusU3Ek__BackingField_33() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CradiusU3Ek__BackingField_33)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_U3CradiusU3Ek__BackingField_33() const { return ___U3CradiusU3Ek__BackingField_33; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_U3CradiusU3Ek__BackingField_33() { return &___U3CradiusU3Ek__BackingField_33; }
	inline void set_U3CradiusU3Ek__BackingField_33(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___U3CradiusU3Ek__BackingField_33 = value;
	}

	inline static int32_t get_offset_of_U3CradiusVarianceU3Ek__BackingField_34() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CradiusVarianceU3Ek__BackingField_34)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_U3CradiusVarianceU3Ek__BackingField_34() const { return ___U3CradiusVarianceU3Ek__BackingField_34; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_U3CradiusVarianceU3Ek__BackingField_34() { return &___U3CradiusVarianceU3Ek__BackingField_34; }
	inline void set_U3CradiusVarianceU3Ek__BackingField_34(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___U3CradiusVarianceU3Ek__BackingField_34 = value;
	}

	inline static int32_t get_offset_of_U3CmodifiersU3Ek__BackingField_35() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CmodifiersU3Ek__BackingField_35)); }
	inline int32_t get_U3CmodifiersU3Ek__BackingField_35() const { return ___U3CmodifiersU3Ek__BackingField_35; }
	inline int32_t* get_address_of_U3CmodifiersU3Ek__BackingField_35() { return &___U3CmodifiersU3Ek__BackingField_35; }
	inline void set_U3CmodifiersU3Ek__BackingField_35(int32_t value)
	{
		___U3CmodifiersU3Ek__BackingField_35 = value;
	}

	inline static int32_t get_offset_of_U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36)); }
	inline bool get_U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36() const { return ___U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36; }
	inline bool* get_address_of_U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36() { return &___U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36; }
	inline void set_U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36(bool value)
	{
		___U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36 = value;
	}

	inline static int32_t get_offset_of_U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37() { return static_cast<int32_t>(offsetof(PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634, ___U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37)); }
	inline bool get_U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37() const { return ___U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37; }
	inline bool* get_address_of_U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37() { return &___U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37; }
	inline void set_U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37(bool value)
	{
		___U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
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


// T UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StyleEnum_1_get_value_m863E731D80D0A943F5A206AE32787D5C073A3109_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::set_specificity(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StyleEnum_1_set_specificity_mF1C94EF10622C93C04EA507D94C7294E5AFAC06B_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, int32_t ___value0, const RuntimeMethod* method);
// UnityEngine.UIElements.StyleKeyword UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::get_keyword()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StyleEnum_1_get_keyword_m73D1031A571AC4D33C8FAE3C20FAC0E5C2A456A2_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::.ctor(T,UnityEngine.UIElements.StyleKeyword)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StyleEnum_1__ctor_m9808A49AC4955C97AB41242A2EB776741D566E82_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, int32_t ___v0, int32_t ___keyword1, const RuntimeMethod* method);
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::Equals(UnityEngine.UIElements.StyleEnum`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool StyleEnum_1_Equals_mFA300F4368EBB8B5AED6497B6462C7BC7A851FC2_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  ___other0, const RuntimeMethod* method);
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool StyleEnum_1_Equals_m24B121AC6754A2F66CF25C7F26F5D73B79AD7FD6_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Int32 UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method);
// System.String UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* StyleEnum_1_ToString_m8F8781B4E85F3CF71A0EB27291996FC8607775BF_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method);

// System.Int32 System.Math::Max(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Math_Max_mA99E48BB021F2E4B62D4EA9F52EA6928EED618A2 (int32_t ___val10, int32_t ___val21, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogError(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::set_propagation(UnityEngine.UIElements.EventBase/EventPropagation)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void EventBase_set_propagation_mF770530A5B592FAB33C549ED74453ABC7ED0E7A6_inline (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::set_target(UnityEngine.UIElements.IEventHandler)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventBase_set_target_mC6CE8B7FED46EC08C07F2279750CBF2780E90C6C (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, RuntimeObject* ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::set_propagateToIMGUI(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventBase_set_propagateToIMGUI_m4B5E6109B619AF7761D310984BEB6C2210E05DD2 (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, bool ___value0, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::get_zero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2 (const RuntimeMethod* method);
// UnityEngine.Vector2 UnityEngine.Vector2::get_zero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  Vector2_get_zero_mFE0C3213BB698130D6C5247AB4B887A59074D0A8 (const RuntimeMethod* method);
// UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.EventBase::get_currentTarget()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* EventBase_get_currentTarget_mB3B3ED90CE63BFFE6E789E0BC5FA5B904BDEF581 (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::set_currentTarget(UnityEngine.UIElements.IEventHandler)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventBase_set_currentTarget_mBA8C39A26A9E8B67E1003496F77C00BA48FA1B90 (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, RuntimeObject* ___value0, const RuntimeMethod* method);
// UnityEngine.Vector2 UnityEngine.Vector2::op_Implicit(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  Vector2_op_Implicit_mEA1F75961E3D368418BA8CEB9C40E55C25BA3C28 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___v0, const RuntimeMethod* method);
// UnityEngine.Vector2 UnityEngine.UIElements.VisualElementExtensions::WorldToLocal(UnityEngine.UIElements.VisualElement,UnityEngine.Vector2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  VisualElementExtensions_WorldToLocal_mFDEA66A0B4B27B27D235DE5E11E68E3145CA18C8 (VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * ___ele0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___p1, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector2::op_Implicit(UnityEngine.Vector2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___v0, const RuntimeMethod* method);
// UnityEngine.EventType UnityEngine.Event::get_rawType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9 (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// UnityEngine.EventType UnityEngine.Event::get_type()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Event_get_type_mAABE4A35E5658E0079A1518D318AF2592F51D6FA (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::Assert(System.Boolean,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_Assert_m84EE43ACFD01E8C0CEC0160C494B2CE77338F7BC (bool ___condition0, String_t* ___message1, const RuntimeMethod* method);
// UnityEngine.PointerType UnityEngine.Event::get_pointerType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Event_get_pointerType_m1D8031C5A8BFBAC21BAAB396199CDFCD3AAC42CA (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::set_imguiEvent(UnityEngine.Event)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventBase_set_imguiEvent_mD693DAF0735050C06640E8C9DD5683E210D18E15 (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * ___value0, const RuntimeMethod* method);
// System.Int32 UnityEngine.Event::get_button()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Event_get_button_mA6E2D86802D75E24DB3C52EB52142EA98A3B93CE (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.PointerDeviceState::PressButton(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerDeviceState_PressButton_mD0A791144315B3CFEB74A02FECBEDFFD24A4A9CE (int32_t ___pointerId0, int32_t ___buttonId1, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.PointerDeviceState::ReleaseButton(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerDeviceState_ReleaseButton_m841973E840794947778B7057D4DD315556F7A6A5 (int32_t ___pointerId0, int32_t ___buttonId1, const RuntimeMethod* method);
// System.Int32 UnityEngine.UIElements.PointerDeviceState::GetPressedButtons(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PointerDeviceState_GetPressedButtons_m92D8D7B9F9B7A2251807764D45D133CE96E7E8C2 (int32_t ___pointerId0, const RuntimeMethod* method);
// UnityEngine.Vector2 UnityEngine.Event::get_mousePosition()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  Event_get_mousePosition_m92AD98489A626864E9B179273270802EB1D2C085 (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// UnityEngine.Vector2 UnityEngine.Event::get_delta()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  Event_get_delta_m552632C8BD6AFB1FF814636177843C6E28E87B85 (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Event::get_clickCount()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Event_get_clickCount_m8C296AD53AA9F14D43DB3682227DD343B9499F21 (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// UnityEngine.EventModifiers UnityEngine.Event::get_modifiers()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Event_get_modifiers_m4D1BDE843A9379F50C3F32CB78CCDAD84B779108 (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Event::get_pressure()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Event_get_pressure_m2F2732871C11B870A4DE6CF3056C495252B07024 (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * __this, const RuntimeMethod* method);
// System.String UnityEngine.UIElements.PointerType::GetPointerType(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* PointerType_GetPointerType_mFC253D2B16A977A73555E236FBD45A36F271D9A1 (int32_t ___pointerId0, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::PreDispatch(UnityEngine.UIElements.IPanel)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventBase_PreDispatch_m242A7911A4139A5BF3BDAF7952AE67DF0DBC779A (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, RuntimeObject* ___panel0, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.PointerDeviceState::SavePointerPosition(System.Int32,UnityEngine.Vector2,UnityEngine.UIElements.IPanel)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerDeviceState_SavePointerPosition_mFFAA7F8C884E231C7D950676565CDE8A9BAF2390 (int32_t ___pointerId0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___position1, RuntimeObject* ___panel2, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.PointerCaptureHelper::ProcessPointerCapture(UnityEngine.UIElements.IPanel,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerCaptureHelper_ProcessPointerCapture_m09CC6875E7044A577341CFA82E5E665DA7434EC6 (RuntimeObject* ___panel0, int32_t ___pointerId1, const RuntimeMethod* method);
// System.Boolean UnityEngine.UIElements.PointerCaptureHelper::ShouldSendCompatibilityMouseEvents(UnityEngine.UIElements.IPanel,UnityEngine.UIElements.IPointerEvent)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PointerCaptureHelper_ShouldSendCompatibilityMouseEvents_m7719A65D3C28756BA1F509CAD2362E2B7A05DD83 (RuntimeObject* ___panel0, RuntimeObject* ___evt1, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.BaseVisualElementPanel::CommitElementUnderPointers()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BaseVisualElementPanel_CommitElementUnderPointers_mFBDB91138145CF30ADB72FE74511C7E8BD3374F3 (BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.UIElements.EventBase::PostDispatch(UnityEngine.UIElements.IPanel)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventBase_PostDispatch_m4182DABA2D5674FC82F67DC57219840840065644 (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, RuntimeObject* ___panel0, const RuntimeMethod* method);
// T UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::get_value()
inline int32_t StyleEnum_1_get_value_m863E731D80D0A943F5A206AE32787D5C073A3109 (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, const RuntimeMethod*))StyleEnum_1_get_value_m863E731D80D0A943F5A206AE32787D5C073A3109_gshared)(__this, method);
}
// System.Void UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::set_specificity(System.Int32)
inline void StyleEnum_1_set_specificity_mF1C94EF10622C93C04EA507D94C7294E5AFAC06B (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, int32_t ___value0, const RuntimeMethod* method)
{
	((  void (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, int32_t, const RuntimeMethod*))StyleEnum_1_set_specificity_mF1C94EF10622C93C04EA507D94C7294E5AFAC06B_gshared)(__this, ___value0, method);
}
// UnityEngine.UIElements.StyleKeyword UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::get_keyword()
inline int32_t StyleEnum_1_get_keyword_m73D1031A571AC4D33C8FAE3C20FAC0E5C2A456A2 (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, const RuntimeMethod*))StyleEnum_1_get_keyword_m73D1031A571AC4D33C8FAE3C20FAC0E5C2A456A2_gshared)(__this, method);
}
// System.Void UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::.ctor(T,UnityEngine.UIElements.StyleKeyword)
inline void StyleEnum_1__ctor_m9808A49AC4955C97AB41242A2EB776741D566E82 (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, int32_t ___v0, int32_t ___keyword1, const RuntimeMethod* method)
{
	((  void (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, int32_t, int32_t, const RuntimeMethod*))StyleEnum_1__ctor_m9808A49AC4955C97AB41242A2EB776741D566E82_gshared)(__this, ___v0, ___keyword1, method);
}
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::Equals(UnityEngine.UIElements.StyleEnum`1<T>)
inline bool StyleEnum_1_Equals_mFA300F4368EBB8B5AED6497B6462C7BC7A851FC2 (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  ___other0, const RuntimeMethod* method)
{
	return ((  bool (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C , const RuntimeMethod*))StyleEnum_1_Equals_mFA300F4368EBB8B5AED6497B6462C7BC7A851FC2_gshared)(__this, ___other0, method);
}
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::Equals(System.Object)
inline bool StyleEnum_1_Equals_m24B121AC6754A2F66CF25C7F26F5D73B79AD7FD6 (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	return ((  bool (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, RuntimeObject *, const RuntimeMethod*))StyleEnum_1_Equals_m24B121AC6754A2F66CF25C7F26F5D73B79AD7FD6_gshared)(__this, ___obj0, method);
}
// System.Int32 System.Int32::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Int32_GetHashCode_m245C424ECE351E5FE3277A88EEB02132DAB8C25A (int32_t* __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::GetHashCode()
inline int32_t StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94 (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, const RuntimeMethod*))StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94_gshared)(__this, method);
}
// System.String UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::ToString()
inline String_t* StyleEnum_1_ToString_m8F8781B4E85F3CF71A0EB27291996FC8607775BF (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	return ((  String_t* (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *, const RuntimeMethod*))StyleEnum_1_ToString_m8F8781B4E85F3CF71A0EB27291996FC8607775BF_gshared)(__this, method);
}
// System.Void UnityEngine.WaitForSeconds::.ctor(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void WaitForSeconds__ctor_m8E4BA3E27AEFFE5B74A815F26FF8AAB99743F559 (WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 * __this, float ___seconds0, const RuntimeMethod* method);
// System.Void System.NotSupportedException::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NotSupportedException__ctor_mA121DE1CAC8F25277DEB489DC7771209D91CAE33 (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * __this, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 UnityEngine.UIElements.ObjectPool`1<System.Object>::get_maxSize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ObjectPool_1_get_maxSize_m7B65C7F2E8778BB8C93F97B437BB0028A8E9D080_gshared (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_m_MaxSize_1();
		V_0 = (int32_t)L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return (int32_t)L_1;
	}
}
// System.Void UnityEngine.UIElements.ObjectPool`1<System.Object>::set_maxSize(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ObjectPool_1_set_maxSize_m5732821144B90CB4DCAD3F663176B589E7AAE794_gshared (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * __this, int32_t ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ObjectPool_1_set_maxSize_m5732821144B90CB4DCAD3F663176B589E7AAE794_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		int32_t L_0 = ___value0;
		IL2CPP_RUNTIME_CLASS_INIT(Math_tFB388E53C7FDC6FCCF9A19ABF5A4E521FBD52E19_il2cpp_TypeInfo_var);
		int32_t L_1 = Math_Max_mA99E48BB021F2E4B62D4EA9F52EA6928EED618A2((int32_t)0, (int32_t)L_0, /*hidden argument*/NULL);
		__this->set_m_MaxSize_1(L_1);
		goto IL_0019;
	}

IL_0010:
	{
		NullCheck((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this);
		((  RuntimeObject * (*) (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
	}

IL_0019:
	{
		NullCheck((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this);
		int32_t L_2 = ((  int32_t (*) (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		int32_t L_3 = (int32_t)__this->get_m_MaxSize_1();
		V_0 = (bool)((((int32_t)L_2) > ((int32_t)L_3))? 1 : 0);
		bool L_4 = V_0;
		if (L_4)
		{
			goto IL_0010;
		}
	}
	{
		return;
	}
}
// System.Void UnityEngine.UIElements.ObjectPool`1<System.Object>::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ObjectPool_1__ctor_m4440121448D0B3D2EBC5FE2B0B69167D83A7C74D_gshared (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * __this, int32_t ___maxSize0, const RuntimeMethod* method)
{
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_0 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)il2cpp_codegen_object_new(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 2));
		((  void (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)(L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		__this->set_m_Stack_0(L_0);
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0((RuntimeObject *)__this, /*hidden argument*/NULL);
		int32_t L_1 = ___maxSize0;
		NullCheck((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this);
		((  void (*) (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this, (int32_t)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		return;
	}
}
// System.Int32 UnityEngine.UIElements.ObjectPool`1<System.Object>::Size()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t ObjectPool_1_Size_m3074E09D3EB79A9C1315D23B78F7DC9B1D7ACC78_gshared (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_0 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_0);
		int32_t L_1 = ((  int32_t (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		V_0 = (int32_t)L_1;
		goto IL_000f;
	}

IL_000f:
	{
		int32_t L_2 = V_0;
		return (int32_t)L_2;
	}
}
// T UnityEngine.UIElements.ObjectPool`1<System.Object>::Get()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * ObjectPool_1_Get_m3FDC1AFD2FA6F9F4C0D541DB29C3CE84B3C244D2_gshared (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * __this, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	RuntimeObject * V_1 = NULL;
	RuntimeObject * G_B3_0 = NULL;
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_0 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_0);
		int32_t L_1 = ((  int32_t (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		if (!L_1)
		{
			goto IL_001b;
		}
	}
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_2 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_2);
		RuntimeObject * L_3 = ((  RuntimeObject * (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		G_B3_0 = L_3;
		goto IL_0020;
	}

IL_001b:
	{
		RuntimeObject * L_4 = ((  RuntimeObject * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		G_B3_0 = L_4;
	}

IL_0020:
	{
		V_0 = (RuntimeObject *)G_B3_0;
		RuntimeObject * L_5 = V_0;
		V_1 = (RuntimeObject *)L_5;
		goto IL_0025;
	}

IL_0025:
	{
		RuntimeObject * L_6 = V_1;
		return (RuntimeObject *)L_6;
	}
}
// System.Void UnityEngine.UIElements.ObjectPool`1<System.Object>::Release(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ObjectPool_1_Release_m2F54E298C569A530D9BE145E89A85461969B0E79_gshared (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E * __this, RuntimeObject * ___element0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ObjectPool_1_Release_m2F54E298C569A530D9BE145E89A85461969B0E79_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	int32_t G_B3_0 = 0;
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_0 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_0);
		int32_t L_1 = ((  int32_t (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		if ((((int32_t)L_1) <= ((int32_t)0)))
		{
			goto IL_0029;
		}
	}
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_2 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_2);
		RuntimeObject * L_3 = ((  RuntimeObject * (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		RuntimeObject * L_4 = ___element0;
		G_B3_0 = ((((RuntimeObject*)(RuntimeObject *)L_3) == ((RuntimeObject*)(RuntimeObject *)L_4))? 1 : 0);
		goto IL_002a;
	}

IL_0029:
	{
		G_B3_0 = 0;
	}

IL_002a:
	{
		V_0 = (bool)G_B3_0;
		bool L_5 = V_0;
		if (!L_5)
		{
			goto IL_0039;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29((RuntimeObject *)_stringLiteral04231B44477132B3DBEFE7768A921AE5A13A00FC, /*hidden argument*/NULL);
	}

IL_0039:
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_6 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_6);
		int32_t L_7 = ((  int32_t (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_6, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		NullCheck((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this);
		int32_t L_8 = ((  int32_t (*) (ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)((ObjectPool_1_tF86F778576B5A5C04A8D2A318DC0AF803837125E *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		V_1 = (bool)((((int32_t)L_7) < ((int32_t)L_8))? 1 : 0);
		bool L_9 = V_1;
		if (!L_9)
		{
			goto IL_005f;
		}
	}
	{
		Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 * L_10 = (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)__this->get_m_Stack_0();
		RuntimeObject * L_11 = ___element0;
		NullCheck((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_10);
		((  void (*) (Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((Stack_1_t5697A763CE21E705BB0297FFBE9AFCB5F95C9163 *)L_10, (RuntimeObject *)L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
	}

IL_005f:
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
// System.Void UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>::set_relatedTarget(UnityEngine.UIElements.IEventHandler)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerCaptureEventBase_1_set_relatedTarget_m3EA1321494E9B18A8D0A6D66F3524C5AFC4C35CC_gshared (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE * __this, RuntimeObject* ___value0, const RuntimeMethod* method)
{
	{
		RuntimeObject* L_0 = ___value0;
		__this->set_U3CrelatedTargetU3Ek__BackingField_18(L_0);
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>::set_pointerId(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerCaptureEventBase_1_set_pointerId_m131956A66460FB60C9C7FE46BF8EC24B43001024_gshared (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CpointerIdU3Ek__BackingField_19(L_0);
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>::Init()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerCaptureEventBase_1_Init_mADB7C17F819950EDB793FDDC8D710F0E987A6ABC_gshared (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE * __this, const RuntimeMethod* method)
{
	{
		NullCheck((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this);
		((  void (*) (EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		NullCheck((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this);
		((  void (*) (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>::LocalInit()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerCaptureEventBase_1_LocalInit_m67C407968188A88A0860879AD1A1BE70884AA2F9_gshared (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerCaptureEventBase_1_LocalInit_m67C407968188A88A0860879AD1A1BE70884AA2F9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		EventBase_set_propagation_mF770530A5B592FAB33C549ED74453ABC7ED0E7A6_inline((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, (int32_t)3, /*hidden argument*/NULL);
		NullCheck((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this);
		((  void (*) (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *, RuntimeObject*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 2)->methodPointer)((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this, (RuntimeObject*)NULL, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 2));
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_0 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_invalidPointerId_1();
		NullCheck((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this);
		((  void (*) (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this, (int32_t)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		return;
	}
}
// T UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>::GetPooled(UnityEngine.UIElements.IEventHandler,UnityEngine.UIElements.IEventHandler,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * PointerCaptureEventBase_1_GetPooled_m37A56A955B99D923D58810CC73366C7F9E081D1E_gshared (RuntimeObject* ___target0, RuntimeObject* ___relatedTarget1, int32_t ___pointerId2, const RuntimeMethod* method)
{
	RuntimeObject * V_0 = NULL;
	RuntimeObject * V_1 = NULL;
	{
		IL2CPP_RUNTIME_CLASS_INIT(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		RuntimeObject * L_0 = ((  RuntimeObject * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 4)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 4));
		V_0 = (RuntimeObject *)L_0;
		RuntimeObject * L_1 = V_0;
		RuntimeObject* L_2 = ___target0;
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)L_1);
		EventBase_set_target_mC6CE8B7FED46EC08C07F2279750CBF2780E90C6C((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)L_1, (RuntimeObject*)L_2, /*hidden argument*/NULL);
		RuntimeObject * L_3 = V_0;
		RuntimeObject* L_4 = ___relatedTarget1;
		NullCheck((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)L_3);
		((  void (*) (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *, RuntimeObject*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)L_3, (RuntimeObject*)L_4, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		RuntimeObject * L_5 = V_0;
		int32_t L_6 = ___pointerId2;
		NullCheck((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)L_5);
		((  void (*) (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3)->methodPointer)((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)L_5, (int32_t)L_6, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3));
		RuntimeObject * L_7 = V_0;
		V_1 = (RuntimeObject *)L_7;
		goto IL_0032;
	}

IL_0032:
	{
		RuntimeObject * L_8 = V_1;
		return (RuntimeObject *)L_8;
	}
}
// System.Void UnityEngine.UIElements.PointerCaptureEventBase`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerCaptureEventBase_1__ctor_m1E21BD75F6DA45A5F5645B7DB72A39A03E25A23F_gshared (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE * __this, const RuntimeMethod* method)
{
	{
		NullCheck((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this);
		IL2CPP_RUNTIME_CLASS_INIT(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 5));
		((  void (*) (EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		NullCheck((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this);
		((  void (*) (PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((PointerCaptureEventBase_1_t05AEB8D5A945DF031507835AEFB34F782724C7BE *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
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
// System.Int32 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_pointerId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PointerEventBase_1_get_pointerId_m7CA84EB539A37964E5D19464ED332F1F107D4367_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_U3CpointerIdU3Ek__BackingField_18();
		return (int32_t)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_pointerId(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_pointerId_m73E0E3E080812EEADF344A559C81F1685AF67272_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CpointerIdU3Ek__BackingField_18(L_0);
		return;
	}
}
// System.String UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_pointerType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* PointerEventBase_1_get_pointerType_m5D203DC46E47C6C2E99359651C632C994840CEBA_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		String_t* L_0 = (String_t*)__this->get_U3CpointerTypeU3Ek__BackingField_19();
		return (String_t*)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_pointerType(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_pointerType_m9EFA612B59E695BBB3FDE50D5E8A8F7A1C7BD86B_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, String_t* ___value0, const RuntimeMethod* method)
{
	{
		String_t* L_0 = ___value0;
		__this->set_U3CpointerTypeU3Ek__BackingField_19(L_0);
		return;
	}
}
// System.Boolean UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_isPrimary()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PointerEventBase_1_get_isPrimary_mE3193189BF33881D8EFF9EAF7E94480962B59F79_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		bool L_0 = (bool)__this->get_U3CisPrimaryU3Ek__BackingField_20();
		return (bool)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_isPrimary(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_isPrimary_mD607B525FF277744F4034911634F9252D7542CEA_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		bool L_0 = ___value0;
		__this->set_U3CisPrimaryU3Ek__BackingField_20(L_0);
		return;
	}
}
// System.Int32 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_button()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PointerEventBase_1_get_button_m0D0E8B13FC7560B3358674DAE780B19B2BAAB48D_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_U3CbuttonU3Ek__BackingField_21();
		return (int32_t)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_button(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_button_mC84D7125A74D3F1F78D16570EFA7F8B7A9278AC3_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CbuttonU3Ek__BackingField_21(L_0);
		return;
	}
}
// System.Int32 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_pressedButtons()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PointerEventBase_1_get_pressedButtons_m2C19F09018D09DD294634CFC5363625564AA9024_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_U3CpressedButtonsU3Ek__BackingField_22();
		return (int32_t)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_pressedButtons(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_pressedButtons_m0C99E74A43A66239412365312DBFAF0423F6070F_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CpressedButtonsU3Ek__BackingField_22(L_0);
		return;
	}
}
// UnityEngine.Vector3 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_position()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  PointerEventBase_1_get_position_m992F34BD47136A2DD09331FED6D77F5F78A435AF_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )__this->get_U3CpositionU3Ek__BackingField_23();
		return (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_position(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_position_m5827E83B94FF2072E328504EFDA3A3EEFCDEF833_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method)
{
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = ___value0;
		__this->set_U3CpositionU3Ek__BackingField_23(L_0);
		return;
	}
}
// UnityEngine.Vector3 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_localPosition()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  PointerEventBase_1_get_localPosition_m485F241C84CFEB51D3BBAACFCFD642E2BDFB0FA2_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )__this->get_U3ClocalPositionU3Ek__BackingField_24();
		return (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_localPosition(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_localPosition_m0971802414579099BF332652111EAFA8E1F4398A_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method)
{
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = ___value0;
		__this->set_U3ClocalPositionU3Ek__BackingField_24(L_0);
		return;
	}
}
// UnityEngine.Vector3 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_deltaPosition()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  PointerEventBase_1_get_deltaPosition_mBC9EF90DB115E91F144385EA5C3586FFC10267E1_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )__this->get_U3CdeltaPositionU3Ek__BackingField_25();
		return (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_deltaPosition(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_deltaPosition_mF2974C44764B51BC1C3846459C12CC0A80F5AFC5_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method)
{
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = ___value0;
		__this->set_U3CdeltaPositionU3Ek__BackingField_25(L_0);
		return;
	}
}
// System.Single UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_deltaTime()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PointerEventBase_1_get_deltaTime_mD41C50F43E3620C934A1D2A79F6C5F25290C4ED8_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		float L_0 = (float)__this->get_U3CdeltaTimeU3Ek__BackingField_26();
		return (float)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_deltaTime(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_deltaTime_m8486E3AD17311F65DF38A1265BC4350CFFD04484_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		__this->set_U3CdeltaTimeU3Ek__BackingField_26(L_0);
		return;
	}
}
// System.Int32 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_clickCount()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PointerEventBase_1_get_clickCount_m9E159BB7F901B4A48338529DE77E53C9AC5C5272_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_U3CclickCountU3Ek__BackingField_27();
		return (int32_t)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_clickCount(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_clickCount_mEC196417BBA71F4FD69728348A3F87B19B2A57E0_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CclickCountU3Ek__BackingField_27(L_0);
		return;
	}
}
// System.Single UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_pressure()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PointerEventBase_1_get_pressure_mC519753D46A4BDCD2692C2F58ED2CEADC0ADB937_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		float L_0 = (float)__this->get_U3CpressureU3Ek__BackingField_28();
		return (float)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_pressure(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_pressure_m9C86A7A5093E11764B287E353F2BCC187527DCCC_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		__this->set_U3CpressureU3Ek__BackingField_28(L_0);
		return;
	}
}
// System.Single UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_tangentialPressure()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PointerEventBase_1_get_tangentialPressure_mC0CFC086A4DFB3E17BCD21E4160ACD603665B14F_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		float L_0 = (float)__this->get_U3CtangentialPressureU3Ek__BackingField_29();
		return (float)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_tangentialPressure(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_tangentialPressure_mF4F73800C1371922C764A68571EE5FEBAE6483B3_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		__this->set_U3CtangentialPressureU3Ek__BackingField_29(L_0);
		return;
	}
}
// System.Single UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_altitudeAngle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PointerEventBase_1_get_altitudeAngle_m2435AB1E4C0356CD8B7480CCC9DA0A29C9E20552_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		float L_0 = (float)__this->get_U3CaltitudeAngleU3Ek__BackingField_30();
		return (float)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_altitudeAngle(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_altitudeAngle_mD17E4BCCD7FC8DA192CE62B3698F679D1B02C90F_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		__this->set_U3CaltitudeAngleU3Ek__BackingField_30(L_0);
		return;
	}
}
// System.Single UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_azimuthAngle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PointerEventBase_1_get_azimuthAngle_m422781577714067F6E02EE35455068A32DC5BBE6_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		float L_0 = (float)__this->get_U3CazimuthAngleU3Ek__BackingField_31();
		return (float)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_azimuthAngle(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_azimuthAngle_m87F09C90AECC255663699363D24566411029ADED_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		__this->set_U3CazimuthAngleU3Ek__BackingField_31(L_0);
		return;
	}
}
// System.Single UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_twist()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float PointerEventBase_1_get_twist_m17C6FF7DDF22A7304109ECCA102DDA11261BB565_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		float L_0 = (float)__this->get_U3CtwistU3Ek__BackingField_32();
		return (float)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_twist(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_twist_m24FC39D16A9DF59BC6FC90D0BB5FAE89C323419F_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		__this->set_U3CtwistU3Ek__BackingField_32(L_0);
		return;
	}
}
// UnityEngine.Vector2 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_radius()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  PointerEventBase_1_get_radius_m1151001376A0E1E4820A047BBA764E06200CA020_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_0 = (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )__this->get_U3CradiusU3Ek__BackingField_33();
		return (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_radius(UnityEngine.Vector2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_radius_m9D032DF72E8A5A33D1730B14740E27A3B2E129D4_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___value0, const RuntimeMethod* method)
{
	{
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_0 = ___value0;
		__this->set_U3CradiusU3Ek__BackingField_33(L_0);
		return;
	}
}
// UnityEngine.Vector2 UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_radiusVariance()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  PointerEventBase_1_get_radiusVariance_m75EF5AA70912A20B5A88B7384C9BE39348C2CDA2_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_0 = (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )__this->get_U3CradiusVarianceU3Ek__BackingField_34();
		return (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_radiusVariance(UnityEngine.Vector2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_radiusVariance_m168F0A2BE17E4F2AFA153081D70CC1958AD0D019_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___value0, const RuntimeMethod* method)
{
	{
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_0 = ___value0;
		__this->set_U3CradiusVarianceU3Ek__BackingField_34(L_0);
		return;
	}
}
// UnityEngine.EventModifiers UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_modifiers()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t PointerEventBase_1_get_modifiers_m67237FCFA88DDCD62B80091F6315A74BAE0A0077_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_U3CmodifiersU3Ek__BackingField_35();
		return (int32_t)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_modifiers(UnityEngine.EventModifiers)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_modifiers_m3CC28C71D396F95B636A41012A984545B8E413B7_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CmodifiersU3Ek__BackingField_35(L_0);
		return;
	}
}
// System.Boolean UnityEngine.UIElements.PointerEventBase`1<System.Object>::UnityEngine.UIElements.IPointerEventInternal.get_triggeredByOS()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PointerEventBase_1_UnityEngine_UIElements_IPointerEventInternal_get_triggeredByOS_m28CFFE1E3C06B3D805F23F9CFE8E33EEDBB35C34_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		bool L_0 = (bool)__this->get_U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36();
		return (bool)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::UnityEngine.UIElements.IPointerEventInternal.set_triggeredByOS(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_UnityEngine_UIElements_IPointerEventInternal_set_triggeredByOS_m1D6BB02F12528516E81F64FAA600D55A9E532BAD_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		bool L_0 = ___value0;
		__this->set_U3CUnityEngine_UIElements_IPointerEventInternal_triggeredByOSU3Ek__BackingField_36(L_0);
		return;
	}
}
// System.Boolean UnityEngine.UIElements.PointerEventBase`1<System.Object>::UnityEngine.UIElements.IPointerEventInternal.get_recomputeTopElementUnderPointer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PointerEventBase_1_UnityEngine_UIElements_IPointerEventInternal_get_recomputeTopElementUnderPointer_mA9F483E5E02D290ACDFE1B9271B5C45EC6D81F66_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		bool L_0 = (bool)__this->get_U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37();
		return (bool)L_0;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::UnityEngine.UIElements.IPointerEventInternal.set_recomputeTopElementUnderPointer(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_UnityEngine_UIElements_IPointerEventInternal_set_recomputeTopElementUnderPointer_mCA845A070CB9408D18F78C866D26507BC98A91CF_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		bool L_0 = ___value0;
		__this->set_U3CUnityEngine_UIElements_IPointerEventInternal_recomputeTopElementUnderPointerU3Ek__BackingField_37(L_0);
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::Init()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_Init_m3D1D7E029EF2063D3F1D7B293BC638403992F95D_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		NullCheck((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this);
		((  void (*) (EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::LocalInit()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_LocalInit_m6A72C0C79B21E843E37A6DDC0BE8A117365A28E6_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_LocalInit_m6A72C0C79B21E843E37A6DDC0BE8A117365A28E6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		EventBase_set_propagation_mF770530A5B592FAB33C549ED74453ABC7ED0E7A6_inline((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, (int32_t)7, /*hidden argument*/NULL);
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		EventBase_set_propagateToIMGUI_m4B5E6109B619AF7761D310984BEB6C2210E05DD2((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, (bool)0, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 2)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (int32_t)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 2));
		IL2CPP_RUNTIME_CLASS_INIT(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var);
		String_t* L_0 = ((PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields*)il2cpp_codegen_static_fields_for(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var))->get_unknown_3();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, String_t*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (String_t*)L_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 3));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (bool)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 4));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (int32_t)(-1), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 5));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (int32_t)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 6));
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_1 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 7));
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_3 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 9)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_3, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 9));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 10));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (int32_t)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 11));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 12)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 12));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 13)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 13));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 14)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 14));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 15)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 15));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 16)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 16));
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_4 = Vector2_get_zero_mFE0C3213BB698130D6C5247AB4B887A59074D0A8(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 17)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_4, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 17));
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_5 = Vector2_get_zero_mFE0C3213BB698130D6C5247AB4B887A59074D0A8(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 18)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 18));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 19)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (int32_t)0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 19));
		NullCheck((RuntimeObject*)__this);
		InterfaceActionInvoker1< bool >::Invoke(1 /* System.Void UnityEngine.UIElements.IPointerEventInternal::set_triggeredByOS(System.Boolean) */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)__this, (bool)0);
		NullCheck((RuntimeObject*)__this);
		InterfaceActionInvoker1< bool >::Invoke(3 /* System.Void UnityEngine.UIElements.IPointerEventInternal::set_recomputeTopElementUnderPointer(System.Boolean) */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)__this, (bool)0);
		return;
	}
}
// UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.PointerEventBase`1<System.Object>::get_currentTarget()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* PointerEventBase_1_get_currentTarget_mB32ECD29E26275732B4E311717D53E8B921014DC_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	RuntimeObject* V_0 = NULL;
	{
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		RuntimeObject* L_0 = EventBase_get_currentTarget_mB3B3ED90CE63BFFE6E789E0BC5FA5B904BDEF581((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, /*hidden argument*/NULL);
		V_0 = (RuntimeObject*)L_0;
		goto IL_000a;
	}

IL_000a:
	{
		RuntimeObject* L_1 = V_0;
		return (RuntimeObject*)L_1;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::set_currentTarget(UnityEngine.UIElements.IEventHandler)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_set_currentTarget_m571C3135C0C55466E277FDA84EE500DCB69C1C51_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, RuntimeObject* ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_set_currentTarget_m571C3135C0C55466E277FDA84EE500DCB69C1C51_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * V_0 = NULL;
	bool V_1 = false;
	{
		RuntimeObject* L_0 = ___value0;
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		EventBase_set_currentTarget_mBA8C39A26A9E8B67E1003496F77C00BA48FA1B90((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, (RuntimeObject*)L_0, /*hidden argument*/NULL);
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		RuntimeObject* L_1 = VirtFuncInvoker0< RuntimeObject* >::Invoke(10 /* UnityEngine.UIElements.IEventHandler UnityEngine.UIElements.EventBase::get_currentTarget() */, (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		V_0 = (VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 *)((VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 *)IsInst((RuntimeObject*)L_1, VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57_il2cpp_TypeInfo_var));
		VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * L_2 = V_0;
		V_1 = (bool)((!(((RuntimeObject*)(VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 *)L_2) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_3 = V_1;
		if (!L_3)
		{
			goto IL_003e;
		}
	}
	{
		VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 * L_4 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = ((  Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 20)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 20));
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_6 = Vector2_op_Implicit_mEA1F75961E3D368418BA8CEB9C40E55C25BA3C28((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_5, /*hidden argument*/NULL);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_7 = VisualElementExtensions_WorldToLocal_mFDEA66A0B4B27B27D235DE5E11E68E3145CA18C8((VisualElement_t0EB50F3AD9103B6EEB58682651950BE7C7A4AD57 *)L_4, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_6, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8 = Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F((Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_7, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
		goto IL_004d;
	}

IL_003e:
	{
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_9 = ((  Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 20)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 20));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 8));
	}

IL_004d:
	{
		return;
	}
}
// System.Boolean UnityEngine.UIElements.PointerEventBase`1<System.Object>::IsMouse(UnityEngine.Event)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PointerEventBase_1_IsMouse_m3CAAD93D850AB416844EC5AE97338036DEE7F21B_gshared (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * ___systemEvent0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	int32_t G_B8_0 = 0;
	{
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_0 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_0);
		int32_t L_1 = Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_0, /*hidden argument*/NULL);
		V_0 = (int32_t)L_1;
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)2)))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_3 = V_0;
		if (!L_3)
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_4 = V_0;
		if ((((int32_t)L_4) == ((int32_t)1)))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_5 = V_0;
		if ((((int32_t)L_5) == ((int32_t)3)))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_6 = V_0;
		if ((((int32_t)L_6) == ((int32_t)((int32_t)16))))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_7 = V_0;
		if ((((int32_t)L_7) == ((int32_t)((int32_t)20))))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_8 = V_0;
		G_B8_0 = ((((int32_t)L_8) == ((int32_t)((int32_t)21)))? 1 : 0);
		goto IL_0029;
	}

IL_0028:
	{
		G_B8_0 = 1;
	}

IL_0029:
	{
		V_1 = (bool)G_B8_0;
		goto IL_002c;
	}

IL_002c:
	{
		bool L_9 = V_1;
		return (bool)L_9;
	}
}
// T UnityEngine.UIElements.PointerEventBase`1<System.Object>::GetPooled(UnityEngine.Event)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * PointerEventBase_1_GetPooled_m654B49807444AB3C852A3B8F68181B9203E352F7_gshared (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * ___systemEvent0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_GetPooled_m654B49807444AB3C852A3B8F68181B9203E352F7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	bool V_1 = false;
	int32_t V_2 = 0;
	bool V_3 = false;
	bool V_4 = false;
	bool V_5 = false;
	int32_t V_6 = 0;
	RuntimeObject * V_7 = NULL;
	int32_t G_B3_0 = 0;
	RuntimeObject * G_B22_0 = NULL;
	RuntimeObject * G_B21_0 = NULL;
	float G_B23_0 = 0.0f;
	RuntimeObject * G_B23_1 = NULL;
	{
		IL2CPP_RUNTIME_CLASS_INIT(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 22));
		RuntimeObject * L_0 = ((  RuntimeObject * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21));
		V_0 = (RuntimeObject *)L_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_1 = ___systemEvent0;
		bool L_2 = ((  bool (*) (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 23)->methodPointer)((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 23));
		if (L_2)
		{
			goto IL_001e;
		}
	}
	{
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_3 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_3);
		int32_t L_4 = Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_3, /*hidden argument*/NULL);
		G_B3_0 = ((((int32_t)((((int32_t)L_4) == ((int32_t)((int32_t)9)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		goto IL_001f;
	}

IL_001e:
	{
		G_B3_0 = 0;
	}

IL_001f:
	{
		V_1 = (bool)G_B3_0;
		bool L_5 = V_1;
		if (!L_5)
		{
			goto IL_006b;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_6 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)5);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_7 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)L_6;
		NullCheck(L_7);
		ArrayElementTypeCheck (L_7, _stringLiteralF950D9CCC7081471921E58DAA686F27E792540D3);
		(L_7)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)_stringLiteralF950D9CCC7081471921E58DAA686F27E792540D3);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_8 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)L_7;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_9 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_9);
		int32_t L_10 = Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_9, /*hidden argument*/NULL);
		int32_t L_11 = L_10;
		RuntimeObject * L_12 = Box(EventType_t3D3937E705A4506226002DAB22071B7B181DA57B_il2cpp_TypeInfo_var, &L_11);
		NullCheck(L_8);
		ArrayElementTypeCheck (L_8, L_12);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_12);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_13 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)L_8;
		NullCheck(L_13);
		ArrayElementTypeCheck (L_13, _stringLiteralD21048C5C5B019FA0A181ABA5C8DC41BEE59E376);
		(L_13)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)_stringLiteralD21048C5C5B019FA0A181ABA5C8DC41BEE59E376);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_14 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)L_13;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_15 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_15);
		int32_t L_16 = Event_get_type_mAABE4A35E5658E0079A1518D318AF2592F51D6FA((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_15, /*hidden argument*/NULL);
		int32_t L_17 = L_16;
		RuntimeObject * L_18 = Box(EventType_t3D3937E705A4506226002DAB22071B7B181DA57B_il2cpp_TypeInfo_var, &L_17);
		NullCheck(L_14);
		ArrayElementTypeCheck (L_14, L_18);
		(L_14)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_18);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_19 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)L_14;
		NullCheck(L_19);
		ArrayElementTypeCheck (L_19, _stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A);
		(L_19)->SetAt(static_cast<il2cpp_array_size_t>(4), (RuntimeObject *)_stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A);
		String_t* L_20 = String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07((ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)L_19, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_Assert_m84EE43ACFD01E8C0CEC0160C494B2CE77338F7BC((bool)0, (String_t*)L_20, /*hidden argument*/NULL);
	}

IL_006b:
	{
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_21 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_21);
		int32_t L_22 = Event_get_pointerType_m1D8031C5A8BFBAC21BAAB396199CDFCD3AAC42CA((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_21, /*hidden argument*/NULL);
		V_2 = (int32_t)L_22;
		int32_t L_23 = V_2;
		if ((((int32_t)L_23) == ((int32_t)1)))
		{
			goto IL_00a2;
		}
	}
	{
		goto IL_0078;
	}

IL_0078:
	{
		int32_t L_24 = V_2;
		if ((((int32_t)L_24) == ((int32_t)2)))
		{
			goto IL_00c6;
		}
	}
	{
		goto IL_007e;
	}

IL_007e:
	{
		RuntimeObject * L_25 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var);
		String_t* L_26 = ((PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields*)il2cpp_codegen_static_fields_for(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var))->get_mouse_0();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_25);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, String_t*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_25, (String_t*)L_26, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3));
		RuntimeObject * L_27 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_28 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_mousePointerId_2();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_27);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_27, (int32_t)L_28, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		goto IL_00ea;
	}

IL_00a2:
	{
		RuntimeObject * L_29 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var);
		String_t* L_30 = ((PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields*)il2cpp_codegen_static_fields_for(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var))->get_touch_1();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_29);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, String_t*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_29, (String_t*)L_30, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3));
		RuntimeObject * L_31 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_32 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_touchPointerIdBase_3();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_31);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_31, (int32_t)L_32, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		goto IL_00ea;
	}

IL_00c6:
	{
		RuntimeObject * L_33 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var);
		String_t* L_34 = ((PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_StaticFields*)il2cpp_codegen_static_fields_for(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var))->get_pen_2();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_33);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, String_t*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_33, (String_t*)L_34, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3));
		RuntimeObject * L_35 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_36 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_penPointerIdBase_5();
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_35);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_35, (int32_t)L_36, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		goto IL_00ea;
	}

IL_00ea:
	{
		RuntimeObject * L_37 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_37);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 4)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_37, (bool)1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 4));
		RuntimeObject * L_38 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_38);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 14)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_38, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 14));
		RuntimeObject * L_39 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_39);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_39, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15));
		RuntimeObject * L_40 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_40);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 16)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_40, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 16));
		RuntimeObject * L_41 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_42 = Vector2_get_zero_mFE0C3213BB698130D6C5247AB4B887A59074D0A8(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_41);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 17)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_41, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_42, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 17));
		RuntimeObject * L_43 = V_0;
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_44 = Vector2_get_zero_mFE0C3213BB698130D6C5247AB4B887A59074D0A8(/*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_43);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_43, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_44, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18));
		RuntimeObject * L_45 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_46 = ___systemEvent0;
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)L_45);
		EventBase_set_imguiEvent_mD693DAF0735050C06640E8C9DD5683E210D18E15((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)L_45, (Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_46, /*hidden argument*/NULL);
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_47 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_47);
		int32_t L_48 = Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_47, /*hidden argument*/NULL);
		V_3 = (bool)((((int32_t)L_48) == ((int32_t)0))? 1 : 0);
		bool L_49 = V_3;
		if (!L_49)
		{
			goto IL_018d;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_50 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_mousePointerId_2();
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_51 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_51);
		int32_t L_52 = Event_get_button_mA6E2D86802D75E24DB3C52EB52142EA98A3B93CE((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_51, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(PointerDeviceState_tBADA581B3E49FE39A2230120F84FCBD94BB8ADA4_il2cpp_TypeInfo_var);
		PointerDeviceState_PressButton_mD0A791144315B3CFEB74A02FECBEDFFD24A4A9CE((int32_t)L_50, (int32_t)L_52, /*hidden argument*/NULL);
		RuntimeObject * L_53 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_54 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_54);
		int32_t L_55 = Event_get_button_mA6E2D86802D75E24DB3C52EB52142EA98A3B93CE((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_54, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_53);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_53, (int32_t)L_55, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		goto IL_01e1;
	}

IL_018d:
	{
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_56 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_56);
		int32_t L_57 = Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_56, /*hidden argument*/NULL);
		V_4 = (bool)((((int32_t)L_57) == ((int32_t)1))? 1 : 0);
		bool L_58 = V_4;
		if (!L_58)
		{
			goto IL_01c3;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_59 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_mousePointerId_2();
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_60 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_60);
		int32_t L_61 = Event_get_button_mA6E2D86802D75E24DB3C52EB52142EA98A3B93CE((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_60, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(PointerDeviceState_tBADA581B3E49FE39A2230120F84FCBD94BB8ADA4_il2cpp_TypeInfo_var);
		PointerDeviceState_ReleaseButton_m841973E840794947778B7057D4DD315556F7A6A5((int32_t)L_59, (int32_t)L_61, /*hidden argument*/NULL);
		RuntimeObject * L_62 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_63 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_63);
		int32_t L_64 = Event_get_button_mA6E2D86802D75E24DB3C52EB52142EA98A3B93CE((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_63, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_62);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_62, (int32_t)L_64, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		goto IL_01e1;
	}

IL_01c3:
	{
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_65 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_65);
		int32_t L_66 = Event_get_rawType_m4139BB74440F552F5FD31F56215938E227EF0CD9((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_65, /*hidden argument*/NULL);
		V_5 = (bool)((((int32_t)L_66) == ((int32_t)2))? 1 : 0);
		bool L_67 = V_5;
		if (!L_67)
		{
			goto IL_01e1;
		}
	}
	{
		RuntimeObject * L_68 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_68);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_68, (int32_t)(-1), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
	}

IL_01e1:
	{
		RuntimeObject * L_69 = V_0;
		RuntimeObject * L_70 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_70);
		int32_t L_71 = ((  int32_t (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 26)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_70, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 26));
		IL2CPP_RUNTIME_CLASS_INIT(PointerDeviceState_tBADA581B3E49FE39A2230120F84FCBD94BB8ADA4_il2cpp_TypeInfo_var);
		int32_t L_72 = PointerDeviceState_GetPressedButtons_m92D8D7B9F9B7A2251807764D45D133CE96E7E8C2((int32_t)L_71, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_69);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 6)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_69, (int32_t)L_72, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 6));
		RuntimeObject * L_73 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_74 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_74);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_75 = Event_get_mousePosition_m92AD98489A626864E9B179273270802EB1D2C085((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_74, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_76 = Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F((Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_75, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_73);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 7)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_73, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_76, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 7));
		RuntimeObject * L_77 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_78 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_78);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_79 = Event_get_mousePosition_m92AD98489A626864E9B179273270802EB1D2C085((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_78, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_80 = Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F((Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_79, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_77);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_77, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_80, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		RuntimeObject * L_81 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_82 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_82);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_83 = Event_get_delta_m552632C8BD6AFB1FF814636177843C6E28E87B85((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_82, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_84 = Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F((Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_83, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_81);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 9)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_81, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_84, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 9));
		RuntimeObject * L_85 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_86 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_86);
		int32_t L_87 = Event_get_clickCount_m8C296AD53AA9F14D43DB3682227DD343B9499F21((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_86, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_85);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 11)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_85, (int32_t)L_87, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 11));
		RuntimeObject * L_88 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_89 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_89);
		int32_t L_90 = Event_get_modifiers_m4D1BDE843A9379F50C3F32CB78CCDAD84B779108((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_89, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_88);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 19)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_88, (int32_t)L_90, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 19));
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_91 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_91);
		int32_t L_92 = Event_get_pointerType_m1D8031C5A8BFBAC21BAAB396199CDFCD3AAC42CA((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_91, /*hidden argument*/NULL);
		V_6 = (int32_t)L_92;
		int32_t L_93 = V_6;
		if ((!(((uint32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_93, (int32_t)1))) > ((uint32_t)1))))
		{
			goto IL_029e;
		}
	}
	{
		goto IL_0277;
	}

IL_0277:
	{
		RuntimeObject * L_94 = V_0;
		RuntimeObject * L_95 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_95);
		int32_t L_96 = ((  int32_t (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 27)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_95, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 27));
		G_B21_0 = L_94;
		if (!L_96)
		{
			G_B22_0 = L_94;
			goto IL_0291;
		}
	}
	{
		G_B23_0 = (0.5f);
		G_B23_1 = G_B21_0;
		goto IL_0296;
	}

IL_0291:
	{
		G_B23_0 = (0.0f);
		G_B23_1 = G_B22_0;
	}

IL_0296:
	{
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)G_B23_1);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 12)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)G_B23_1, (float)G_B23_0, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 12));
		goto IL_02b2;
	}

IL_029e:
	{
		RuntimeObject * L_97 = V_0;
		Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 * L_98 = ___systemEvent0;
		NullCheck((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_98);
		float L_99 = Event_get_pressure_m2F2732871C11B870A4DE6CF3056C495252B07024((Event_t187FF6A6B357447B83EC2064823EE0AEC5263210 *)L_98, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_97);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 12)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_97, (float)L_99, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 12));
		goto IL_02b2;
	}

IL_02b2:
	{
		RuntimeObject * L_100 = V_0;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_100);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 13)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_100, (float)(0.0f), /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 13));
		RuntimeObject * L_101 = V_0;
		NullCheck((RuntimeObject*)L_101);
		InterfaceActionInvoker1< bool >::Invoke(1 /* System.Void UnityEngine.UIElements.IPointerEventInternal::set_triggeredByOS(System.Boolean) */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)L_101, (bool)1);
		RuntimeObject * L_102 = V_0;
		V_7 = (RuntimeObject *)L_102;
		goto IL_02d5;
	}

IL_02d5:
	{
		RuntimeObject * L_103 = V_7;
		return (RuntimeObject *)L_103;
	}
}
// T UnityEngine.UIElements.PointerEventBase`1<System.Object>::GetPooled(UnityEngine.UIElements.IPointerEvent,UnityEngine.Vector2,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * PointerEventBase_1_GetPooled_mB4E401D8A4F768CEE0AC20377156F8AD7C9A32EE_gshared (RuntimeObject* ___triggerEvent0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___position1, int32_t ___pointerId2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_GetPooled_mB4E401D8A4F768CEE0AC20377156F8AD7C9A32EE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	bool V_1 = false;
	RuntimeObject * V_2 = NULL;
	{
		RuntimeObject* L_0 = ___triggerEvent0;
		V_1 = (bool)((!(((RuntimeObject*)(RuntimeObject*)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_1;
		if (!L_1)
		{
			goto IL_0013;
		}
	}
	{
		RuntimeObject* L_2 = ___triggerEvent0;
		RuntimeObject * L_3 = ((  RuntimeObject * (*) (RuntimeObject*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 28)->methodPointer)((RuntimeObject*)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 28));
		V_2 = (RuntimeObject *)L_3;
		goto IL_0060;
	}

IL_0013:
	{
		IL2CPP_RUNTIME_CLASS_INIT(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 22));
		RuntimeObject * L_4 = ((  RuntimeObject * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21));
		V_0 = (RuntimeObject *)L_4;
		RuntimeObject * L_5 = V_0;
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_6 = ___position1;
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_7 = Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F((Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_6, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_5);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 7)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_5, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_7, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 7));
		RuntimeObject * L_8 = V_0;
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_9 = ___position1;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = Vector2_op_Implicit_mD152B6A34B4DB7FFECC2844D74718568FE867D6F((Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_9, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_8);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_8, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_10, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		RuntimeObject * L_11 = V_0;
		int32_t L_12 = ___pointerId2;
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_11);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_11, (int32_t)L_12, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		RuntimeObject * L_13 = V_0;
		int32_t L_14 = ___pointerId2;
		IL2CPP_RUNTIME_CLASS_INIT(PointerType_t145158020193F50A503F65BE41CBAC1CE9E3C6CB_il2cpp_TypeInfo_var);
		String_t* L_15 = PointerType_GetPointerType_mFC253D2B16A977A73555E236FBD45A36F271D9A1((int32_t)L_14, /*hidden argument*/NULL);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_13);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, String_t*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_13, (String_t*)L_15, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3));
		RuntimeObject * L_16 = V_0;
		V_2 = (RuntimeObject *)L_16;
		goto IL_0060;
	}

IL_0060:
	{
		RuntimeObject * L_17 = V_2;
		return (RuntimeObject *)L_17;
	}
}
// T UnityEngine.UIElements.PointerEventBase`1<System.Object>::GetPooled(UnityEngine.UIElements.IPointerEvent)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * PointerEventBase_1_GetPooled_m86CCDFBD02BDF5AE0EB7F297281EF28A286C0162_gshared (RuntimeObject* ___triggerEvent0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_GetPooled_m86CCDFBD02BDF5AE0EB7F297281EF28A286C0162_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	bool V_1 = false;
	RuntimeObject* V_2 = NULL;
	bool V_3 = false;
	RuntimeObject * V_4 = NULL;
	{
		IL2CPP_RUNTIME_CLASS_INIT(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 22));
		RuntimeObject * L_0 = ((  RuntimeObject * (*) (const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21)->methodPointer)(/*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 21));
		V_0 = (RuntimeObject *)L_0;
		RuntimeObject* L_1 = ___triggerEvent0;
		V_1 = (bool)((!(((RuntimeObject*)(RuntimeObject*)L_1) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_2 = V_1;
		if (!L_2)
		{
			goto IL_017b;
		}
	}
	{
		RuntimeObject * L_3 = V_0;
		RuntimeObject* L_4 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_4);
		int32_t L_5 = InterfaceFuncInvoker0< int32_t >::Invoke(0 /* System.Int32 UnityEngine.UIElements.IPointerEvent::get_pointerId() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_4);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_3);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_3, (int32_t)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 2));
		RuntimeObject * L_6 = V_0;
		RuntimeObject* L_7 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_7);
		String_t* L_8 = InterfaceFuncInvoker0< String_t* >::Invoke(1 /* System.String UnityEngine.UIElements.IPointerEvent::get_pointerType() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_7);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_6);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, String_t*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_6, (String_t*)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 3));
		RuntimeObject * L_9 = V_0;
		RuntimeObject* L_10 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_10);
		bool L_11 = InterfaceFuncInvoker0< bool >::Invoke(2 /* System.Boolean UnityEngine.UIElements.IPointerEvent::get_isPrimary() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_10);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_9);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, bool, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 4)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_9, (bool)L_11, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 4));
		RuntimeObject * L_12 = V_0;
		RuntimeObject* L_13 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_13);
		int32_t L_14 = InterfaceFuncInvoker0< int32_t >::Invoke(3 /* System.Int32 UnityEngine.UIElements.IPointerEvent::get_button() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_13);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_12);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_12, (int32_t)L_14, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		RuntimeObject * L_15 = V_0;
		RuntimeObject* L_16 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_16);
		int32_t L_17 = InterfaceFuncInvoker0< int32_t >::Invoke(4 /* System.Int32 UnityEngine.UIElements.IPointerEvent::get_pressedButtons() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_16);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_15);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 6)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_15, (int32_t)L_17, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 6));
		RuntimeObject * L_18 = V_0;
		RuntimeObject* L_19 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_19);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_20 = InterfaceFuncInvoker0< Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(5 /* UnityEngine.Vector3 UnityEngine.UIElements.IPointerEvent::get_position() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_19);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_18);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 7)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_18, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_20, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 7));
		RuntimeObject * L_21 = V_0;
		RuntimeObject* L_22 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_22);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_23 = InterfaceFuncInvoker0< Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(6 /* UnityEngine.Vector3 UnityEngine.UIElements.IPointerEvent::get_localPosition() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_22);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_21);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_21, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_23, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 8));
		RuntimeObject * L_24 = V_0;
		RuntimeObject* L_25 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_25);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_26 = InterfaceFuncInvoker0< Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(7 /* UnityEngine.Vector3 UnityEngine.UIElements.IPointerEvent::get_deltaPosition() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_25);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_24);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 9)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_24, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_26, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 9));
		RuntimeObject * L_27 = V_0;
		RuntimeObject* L_28 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_28);
		float L_29 = InterfaceFuncInvoker0< float >::Invoke(8 /* System.Single UnityEngine.UIElements.IPointerEvent::get_deltaTime() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_28);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_27);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 10)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_27, (float)L_29, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 10));
		RuntimeObject * L_30 = V_0;
		RuntimeObject* L_31 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_31);
		int32_t L_32 = InterfaceFuncInvoker0< int32_t >::Invoke(9 /* System.Int32 UnityEngine.UIElements.IPointerEvent::get_clickCount() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_31);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_30);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 11)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_30, (int32_t)L_32, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 11));
		RuntimeObject * L_33 = V_0;
		RuntimeObject* L_34 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_34);
		float L_35 = InterfaceFuncInvoker0< float >::Invoke(10 /* System.Single UnityEngine.UIElements.IPointerEvent::get_pressure() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_34);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_33);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 12)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_33, (float)L_35, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 12));
		RuntimeObject * L_36 = V_0;
		RuntimeObject* L_37 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_37);
		float L_38 = InterfaceFuncInvoker0< float >::Invoke(11 /* System.Single UnityEngine.UIElements.IPointerEvent::get_tangentialPressure() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_37);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_36);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 13)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_36, (float)L_38, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 13));
		RuntimeObject * L_39 = V_0;
		RuntimeObject* L_40 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_40);
		float L_41 = InterfaceFuncInvoker0< float >::Invoke(12 /* System.Single UnityEngine.UIElements.IPointerEvent::get_altitudeAngle() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_40);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_39);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 14)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_39, (float)L_41, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 14));
		RuntimeObject * L_42 = V_0;
		RuntimeObject* L_43 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_43);
		float L_44 = InterfaceFuncInvoker0< float >::Invoke(13 /* System.Single UnityEngine.UIElements.IPointerEvent::get_azimuthAngle() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_43);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_42);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_42, (float)L_44, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 15));
		RuntimeObject * L_45 = V_0;
		RuntimeObject* L_46 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_46);
		float L_47 = InterfaceFuncInvoker0< float >::Invoke(14 /* System.Single UnityEngine.UIElements.IPointerEvent::get_twist() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_46);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_45);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, float, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 16)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_45, (float)L_47, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 16));
		RuntimeObject * L_48 = V_0;
		RuntimeObject* L_49 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_49);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_50 = InterfaceFuncInvoker0< Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  >::Invoke(15 /* UnityEngine.Vector2 UnityEngine.UIElements.IPointerEvent::get_radius() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_49);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_48);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 17)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_48, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_50, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 17));
		RuntimeObject * L_51 = V_0;
		RuntimeObject* L_52 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_52);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_53 = InterfaceFuncInvoker0< Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  >::Invoke(16 /* UnityEngine.Vector2 UnityEngine.UIElements.IPointerEvent::get_radiusVariance() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_52);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_51);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_51, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_53, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 18));
		RuntimeObject * L_54 = V_0;
		RuntimeObject* L_55 = ___triggerEvent0;
		NullCheck((RuntimeObject*)L_55);
		int32_t L_56 = InterfaceFuncInvoker0< int32_t >::Invoke(17 /* UnityEngine.EventModifiers UnityEngine.UIElements.IPointerEvent::get_modifiers() */, IPointerEvent_t9961D65B2C344F89413595E31CBCD1DCE0F47BEA_il2cpp_TypeInfo_var, (RuntimeObject*)L_55);
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_54);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 19)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)L_54, (int32_t)L_56, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 19));
		RuntimeObject* L_57 = ___triggerEvent0;
		V_2 = (RuntimeObject*)((RuntimeObject*)IsInst((RuntimeObject*)L_57, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var));
		RuntimeObject* L_58 = V_2;
		V_3 = (bool)((!(((RuntimeObject*)(RuntimeObject*)L_58) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_59 = V_3;
		if (!L_59)
		{
			goto IL_017a;
		}
	}
	{
		RuntimeObject * L_60 = V_0;
		RuntimeObject* L_61 = V_2;
		NullCheck((RuntimeObject*)L_61);
		bool L_62 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean UnityEngine.UIElements.IPointerEventInternal::get_triggeredByOS() */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)L_61);
		NullCheck((RuntimeObject*)L_60);
		InterfaceActionInvoker1< bool >::Invoke(1 /* System.Void UnityEngine.UIElements.IPointerEventInternal::set_triggeredByOS(System.Boolean) */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)L_60, (bool)L_62);
	}

IL_017a:
	{
	}

IL_017b:
	{
		RuntimeObject * L_63 = V_0;
		V_4 = (RuntimeObject *)L_63;
		goto IL_0180;
	}

IL_0180:
	{
		RuntimeObject * L_64 = V_4;
		return (RuntimeObject *)L_64;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::PreDispatch(UnityEngine.UIElements.IPanel)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_PreDispatch_m359A5629E26AA079E72B1A00D41C93821C6368B0_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, RuntimeObject* ___panel0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_PreDispatch_m359A5629E26AA079E72B1A00D41C93821C6368B0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		RuntimeObject* L_0 = ___panel0;
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		EventBase_PreDispatch_m242A7911A4139A5BF3BDAF7952AE67DF0DBC779A((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, (RuntimeObject*)L_0, /*hidden argument*/NULL);
		NullCheck((RuntimeObject*)__this);
		bool L_1 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean UnityEngine.UIElements.IPointerEventInternal::get_triggeredByOS() */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)__this);
		V_0 = (bool)L_1;
		bool L_2 = V_0;
		if (!L_2)
		{
			goto IL_002d;
		}
	}
	{
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		int32_t L_3 = ((  int32_t (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 26)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 26));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = ((  Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 20)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 20));
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_5 = Vector2_op_Implicit_mEA1F75961E3D368418BA8CEB9C40E55C25BA3C28((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 )L_4, /*hidden argument*/NULL);
		RuntimeObject* L_6 = ___panel0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerDeviceState_tBADA581B3E49FE39A2230120F84FCBD94BB8ADA4_il2cpp_TypeInfo_var);
		PointerDeviceState_SavePointerPosition_mFFAA7F8C884E231C7D950676565CDE8A9BAF2390((int32_t)L_3, (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D )L_5, (RuntimeObject*)L_6, /*hidden argument*/NULL);
	}

IL_002d:
	{
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::PostDispatch(UnityEngine.UIElements.IPanel)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1_PostDispatch_m2DE466AABDA92057C28FD02AF8C130F459C06FBA_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, RuntimeObject* ___panel0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PointerEventBase_1_PostDispatch_m2DE466AABDA92057C28FD02AF8C130F459C06FBA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	bool V_1 = false;
	bool V_2 = false;
	int32_t G_B6_0 = 0;
	BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * G_B9_0 = NULL;
	BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * G_B8_0 = NULL;
	{
		V_0 = (int32_t)0;
		goto IL_0013;
	}

IL_0005:
	{
		RuntimeObject* L_0 = ___panel0;
		int32_t L_1 = V_0;
		PointerCaptureHelper_ProcessPointerCapture_m09CC6875E7044A577341CFA82E5E665DA7434EC6((RuntimeObject*)L_0, (int32_t)L_1, /*hidden argument*/NULL);
		int32_t L_2 = V_0;
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)L_2, (int32_t)1));
	}

IL_0013:
	{
		int32_t L_3 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var);
		int32_t L_4 = ((PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_StaticFields*)il2cpp_codegen_static_fields_for(PointerId_t79333F6490B7D6CEBCC740AC1A58DE52B63B511C_il2cpp_TypeInfo_var))->get_maxPointers_0();
		V_1 = (bool)((((int32_t)L_3) < ((int32_t)L_4))? 1 : 0);
		bool L_5 = V_1;
		if (L_5)
		{
			goto IL_0005;
		}
	}
	{
		RuntimeObject* L_6 = ___panel0;
		bool L_7 = PointerCaptureHelper_ShouldSendCompatibilityMouseEvents_m7719A65D3C28756BA1F509CAD2362E2B7A05DD83((RuntimeObject*)L_6, (RuntimeObject*)__this, /*hidden argument*/NULL);
		if (L_7)
		{
			goto IL_0030;
		}
	}
	{
		NullCheck((RuntimeObject*)__this);
		bool L_8 = InterfaceFuncInvoker0< bool >::Invoke(0 /* System.Boolean UnityEngine.UIElements.IPointerEventInternal::get_triggeredByOS() */, IPointerEventInternal_tA7CFA0A98B5E87A72EC8D777BB1ACEC1F98F8405_il2cpp_TypeInfo_var, (RuntimeObject*)__this);
		G_B6_0 = ((int32_t)(L_8));
		goto IL_0031;
	}

IL_0030:
	{
		G_B6_0 = 0;
	}

IL_0031:
	{
		V_2 = (bool)G_B6_0;
		bool L_9 = V_2;
		if (!L_9)
		{
			goto IL_0049;
		}
	}
	{
		RuntimeObject* L_10 = ___panel0;
		BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 * L_11 = (BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 *)((BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 *)IsInst((RuntimeObject*)L_10, BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90_il2cpp_TypeInfo_var));
		G_B8_0 = L_11;
		if (L_11)
		{
			G_B9_0 = L_11;
			goto IL_0042;
		}
	}
	{
		goto IL_0048;
	}

IL_0042:
	{
		NullCheck((BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 *)G_B9_0);
		BaseVisualElementPanel_CommitElementUnderPointers_mFBDB91138145CF30ADB72FE74511C7E8BD3374F3((BaseVisualElementPanel_t508E21628181848188EE9CDA4C4AF692FB574C90 *)G_B9_0, /*hidden argument*/NULL);
	}

IL_0048:
	{
	}

IL_0049:
	{
		RuntimeObject* L_12 = ___panel0;
		NullCheck((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this);
		EventBase_PostDispatch_m4182DABA2D5674FC82F67DC57219840840065644((EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD *)__this, (RuntimeObject*)L_12, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.UIElements.PointerEventBase`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PointerEventBase_1__ctor_m6EC743C864C40687A3E8E3C3EE82BCF6B76B2595_gshared (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 * __this, const RuntimeMethod* method)
{
	{
		NullCheck((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this);
		IL2CPP_RUNTIME_CLASS_INIT(IL2CPP_RGCTX_DATA(method->klass->rgctx_data, 22));
		((  void (*) (EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 29)->methodPointer)((EventBase_1_t94F9D620FD1F389AC742837202B18355992FA916 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 29));
		NullCheck((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this);
		((  void (*) (PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1)->methodPointer)((PointerEventBase_1_tF77F595F9F1B19B34E1897A9470F0DCD26B95634 *)__this, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 1));
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
// T UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::get_value()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StyleEnum_1_get_value_m863E731D80D0A943F5A206AE32787D5C073A3109_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	int32_t G_B3_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_m_Keyword_0();
		if (!L_0)
		{
			goto IL_0014;
		}
	}
	{
		il2cpp_codegen_initobj((&V_0), sizeof(int32_t));
		int32_t L_1 = V_0;
		G_B3_0 = L_1;
		goto IL_001a;
	}

IL_0014:
	{
		int32_t L_2 = (int32_t)__this->get_m_Value_1();
		G_B3_0 = L_2;
	}

IL_001a:
	{
		V_1 = (int32_t)G_B3_0;
		goto IL_001d;
	}

IL_001d:
	{
		int32_t L_3 = V_1;
		return (int32_t)L_3;
	}
}
IL2CPP_EXTERN_C  int32_t StyleEnum_1_get_value_m863E731D80D0A943F5A206AE32787D5C073A3109_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	return StyleEnum_1_get_value_m863E731D80D0A943F5A206AE32787D5C073A3109(_thisAdjusted, method);
}
// System.Void UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::set_specificity(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StyleEnum_1_set_specificity_mF1C94EF10622C93C04EA507D94C7294E5AFAC06B_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_m_Specificity_2(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void StyleEnum_1_set_specificity_mF1C94EF10622C93C04EA507D94C7294E5AFAC06B_AdjustorThunk (RuntimeObject * __this, int32_t ___value0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	StyleEnum_1_set_specificity_mF1C94EF10622C93C04EA507D94C7294E5AFAC06B(_thisAdjusted, ___value0, method);
}
// UnityEngine.UIElements.StyleKeyword UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::get_keyword()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StyleEnum_1_get_keyword_m73D1031A571AC4D33C8FAE3C20FAC0E5C2A456A2_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_m_Keyword_0();
		V_0 = (int32_t)L_0;
		goto IL_000a;
	}

IL_000a:
	{
		int32_t L_1 = V_0;
		return (int32_t)L_1;
	}
}
IL2CPP_EXTERN_C  int32_t StyleEnum_1_get_keyword_m73D1031A571AC4D33C8FAE3C20FAC0E5C2A456A2_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	return StyleEnum_1_get_keyword_m73D1031A571AC4D33C8FAE3C20FAC0E5C2A456A2(_thisAdjusted, method);
}
// System.Void UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::.ctor(T,UnityEngine.UIElements.StyleKeyword)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StyleEnum_1__ctor_m9808A49AC4955C97AB41242A2EB776741D566E82_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, int32_t ___v0, int32_t ___keyword1, const RuntimeMethod* method)
{
	{
		__this->set_m_Specificity_2(0);
		int32_t L_0 = ___keyword1;
		__this->set_m_Keyword_0(L_0);
		int32_t L_1 = ___v0;
		__this->set_m_Value_1(L_1);
		return;
	}
}
IL2CPP_EXTERN_C  void StyleEnum_1__ctor_m9808A49AC4955C97AB41242A2EB776741D566E82_AdjustorThunk (RuntimeObject * __this, int32_t ___v0, int32_t ___keyword1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	StyleEnum_1__ctor_m9808A49AC4955C97AB41242A2EB776741D566E82(_thisAdjusted, ___v0, ___keyword1, method);
}
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::op_Equality(UnityEngine.UIElements.StyleEnum`1<T>,UnityEngine.UIElements.StyleEnum`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool StyleEnum_1_op_Equality_mCBBFEC68B4112344CE885DAA95737BD217263E3E_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  ___lhs0, StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  ___rhs1, const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_0 = ___lhs0;
		int32_t L_1 = (int32_t)L_0.get_m_Keyword_0();
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_2 = ___rhs1;
		int32_t L_3 = (int32_t)L_2.get_m_Keyword_0();
		if ((!(((uint32_t)L_1) == ((uint32_t)L_3))))
		{
			goto IL_0029;
		}
	}
	{
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_4 = ___lhs0;
		int32_t L_5 = (int32_t)L_4.get_m_Value_1();
		int32_t L_6 = ((  int32_t (*) (int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 0)->methodPointer)((int32_t)L_5, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 0));
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_7 = ___rhs1;
		int32_t L_8 = (int32_t)L_7.get_m_Value_1();
		int32_t L_9 = ((  int32_t (*) (int32_t, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 0)->methodPointer)((int32_t)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 0));
		G_B3_0 = ((((int32_t)L_6) == ((int32_t)L_9))? 1 : 0);
		goto IL_002a;
	}

IL_0029:
	{
		G_B3_0 = 0;
	}

IL_002a:
	{
		V_0 = (bool)G_B3_0;
		goto IL_002d;
	}

IL_002d:
	{
		bool L_10 = V_0;
		return (bool)L_10;
	}
}
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::Equals(UnityEngine.UIElements.StyleEnum`1<T>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool StyleEnum_1_Equals_mFA300F4368EBB8B5AED6497B6462C7BC7A851FC2_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  ___other0, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_0 = ___other0;
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_1 = (*(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *)__this);
		bool L_2 = ((  bool (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C , StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 1)->methodPointer)((StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C )L_0, (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C )L_1, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 1));
		V_0 = (bool)L_2;
		goto IL_0010;
	}

IL_0010:
	{
		bool L_3 = V_0;
		return (bool)L_3;
	}
}
IL2CPP_EXTERN_C  bool StyleEnum_1_Equals_mFA300F4368EBB8B5AED6497B6462C7BC7A851FC2_AdjustorThunk (RuntimeObject * __this, StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  ___other0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	return StyleEnum_1_Equals_mFA300F4368EBB8B5AED6497B6462C7BC7A851FC2(_thisAdjusted, ___other0, method);
}
// System.Boolean UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::Equals(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool StyleEnum_1_Equals_m24B121AC6754A2F66CF25C7F26F5D73B79AD7FD6_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  V_0;
	memset((&V_0), 0, sizeof(V_0));
	bool V_1 = false;
	bool V_2 = false;
	{
		RuntimeObject * L_0 = ___obj0;
		V_1 = (bool)((((int32_t)((!(((RuntimeObject*)(RuntimeObject *)((RuntimeObject *)IsInst((RuntimeObject*)L_0, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 3)))) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_1;
		if (!L_1)
		{
			goto IL_0016;
		}
	}
	{
		V_2 = (bool)0;
		goto IL_002c;
	}

IL_0016:
	{
		RuntimeObject * L_2 = ___obj0;
		V_0 = (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C )((*(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *)((StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *)UnBox(L_2, IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 3)))));
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_3 = V_0;
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_4 = (*(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *)__this);
		bool L_5 = ((  bool (*) (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C , StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C , const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 1)->methodPointer)((StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C )L_3, (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C )L_4, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 1));
		V_2 = (bool)L_5;
		goto IL_002c;
	}

IL_002c:
	{
		bool L_6 = V_2;
		return (bool)L_6;
	}
}
IL2CPP_EXTERN_C  bool StyleEnum_1_Equals_m24B121AC6754A2F66CF25C7F26F5D73B79AD7FD6_AdjustorThunk (RuntimeObject * __this, RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	return StyleEnum_1_Equals_m24B121AC6754A2F66CF25C7F26F5D73B79AD7FD6(_thisAdjusted, ___obj0, method);
}
// System.Int32 UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::GetHashCode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		V_0 = (int32_t)((int32_t)917506989);
		int32_t L_0 = V_0;
		int32_t* L_1 = (int32_t*)__this->get_address_of_m_Keyword_0();
		Il2CppFakeBox<int32_t> L_2(StyleKeyword_t5C285A4249A1A7A807C1B4D2AAF5D1350B0A3560_il2cpp_TypeInfo_var, L_1);
		int32_t L_3 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(&L_2));
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_0, (int32_t)((int32_t)-1521134295))), (int32_t)L_3));
		int32_t L_4 = V_0;
		int32_t* L_5 = (int32_t*)__this->get_address_of_m_Value_1();
		Il2CppFakeBox<int32_t> L_6(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 4), L_5);
		int32_t L_7 = VirtFuncInvoker0< int32_t >::Invoke(2 /* System.Int32 System.Object::GetHashCode() */, (RuntimeObject *)(&L_6));
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_4, (int32_t)((int32_t)-1521134295))), (int32_t)L_7));
		int32_t L_8 = V_0;
		int32_t* L_9 = (int32_t*)__this->get_address_of_m_Specificity_2();
		int32_t L_10 = Int32_GetHashCode_m245C424ECE351E5FE3277A88EEB02132DAB8C25A((int32_t*)(int32_t*)L_9, /*hidden argument*/NULL);
		V_0 = (int32_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)il2cpp_codegen_multiply((int32_t)L_8, (int32_t)((int32_t)-1521134295))), (int32_t)L_10));
		int32_t L_11 = V_0;
		V_1 = (int32_t)L_11;
		goto IL_0053;
	}

IL_0053:
	{
		int32_t L_12 = V_1;
		return (int32_t)L_12;
	}
}
IL2CPP_EXTERN_C  int32_t StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	return StyleEnum_1_GetHashCode_mA3BDF8E038179D79DFE761A60E55AB94F2F73A94(_thisAdjusted, method);
}
// System.String UnityEngine.UIElements.StyleEnum`1<System.Int32Enum>::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* StyleEnum_1_ToString_m8F8781B4E85F3CF71A0EB27291996FC8607775BF_gshared (StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * __this, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
	{
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_0 = (*(StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *)__this);
		StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C  L_1 = L_0;
		RuntimeObject * L_2 = Box(IL2CPP_RGCTX_DATA(InitializedTypeInfo(method->klass)->rgctx_data, 3), &L_1);
		String_t* L_3 = ((  String_t* (*) (RuntimeObject*, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5)->methodPointer)((RuntimeObject*)L_2, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(InitializedTypeInfo(method->klass)->rgctx_data, 5));
		V_0 = (String_t*)L_3;
		goto IL_0014;
	}

IL_0014:
	{
		String_t* L_4 = V_0;
		return (String_t*)L_4;
	}
}
IL2CPP_EXTERN_C  String_t* StyleEnum_1_ToString_m8F8781B4E85F3CF71A0EB27291996FC8607775BF_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C * _thisAdjusted = reinterpret_cast<StyleEnum_1_tA4AF2FC0364C1E39AD284C192C959587F92E411C *>(__this + _offset);
	return StyleEnum_1_ToString_m8F8781B4E85F3CF71A0EB27291996FC8607775BF(_thisAdjusted, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__6_1__ctor_m7957FDE9B0D4D26BF0A44447ED9CB4BFC2B99F2B_gshared (U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1 * __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0((RuntimeObject *)__this, /*hidden argument*/NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->set_U3CU3E1__state_0(L_0);
		return;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__6_1_System_IDisposable_Dispose_m179FAC356FC0A2E121BF2C56AD23D52A1B3B5087_gshared (U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1 * __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CDelayedInvokeU3Ed__6_1_MoveNext_m7A8BFC56E3FAC8345E61CD658C43201B8BA12F8A_gshared (U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__6_1_MoveNext_m7A8BFC56E3FAC8345E61CD658C43201B8BA12F8A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_U3CU3E1__state_0();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if (!L_1)
		{
			goto IL_0010;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)1)))
		{
			goto IL_0031;
		}
	}
	{
		return (bool)0;
	}

IL_0010:
	{
		__this->set_U3CU3E1__state_0((-1));
		// yield return new WaitForSeconds(delay);
		float L_3 = (float)__this->get_delay_2();
		WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 * L_4 = (WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 *)il2cpp_codegen_object_new(WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_il2cpp_TypeInfo_var);
		WaitForSeconds__ctor_m8E4BA3E27AEFFE5B74A815F26FF8AAB99743F559(L_4, (float)L_3, /*hidden argument*/NULL);
		__this->set_U3CU3E2__current_1(L_4);
		__this->set_U3CU3E1__state_0(1);
		return (bool)1;
	}

IL_0031:
	{
		__this->set_U3CU3E1__state_0((-1));
		// action(arg);
		Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 * L_5 = (Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 *)__this->get_action_3();
		RuntimeObject * L_6 = (RuntimeObject *)__this->get_arg_4();
		NullCheck((Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 *)L_5);
		((  void (*) (Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Action_1_t551A279CEADCF6EEAE8FA2B1E1E757D0D15290D0 *)L_5, (RuntimeObject *)L_6, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return (bool)0;
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__6_1_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_mB51D248F44B9859A5C1CFA638F97900F5D1FD98B_gshared (U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__6_1_System_Collections_IEnumerator_Reset_mB0F6B042AF13D81BE8AB6D1453B5ED67765AFA27_gshared (U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__6_1_System_Collections_IEnumerator_Reset_mB0F6B042AF13D81BE8AB6D1453B5ED67765AFA27_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * L_0 = (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 *)il2cpp_codegen_object_new(NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var);
		NotSupportedException__ctor_mA121DE1CAC8F25277DEB489DC7771209D91CAE33(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, U3CDelayedInvokeU3Ed__6_1_System_Collections_IEnumerator_Reset_mB0F6B042AF13D81BE8AB6D1453B5ED67765AFA27_RuntimeMethod_var);
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__6`1<System.Object>::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__6_1_System_Collections_IEnumerator_get_Current_m262B7223E5F42995F54353516706D4605A298F85_gshared (U3CDelayedInvokeU3Ed__6_1_t6301132D2FFE9FE0A0E9957AD11B7D998777B9E1 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
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
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__7_2__ctor_mAD1E48C6E4A2D475B0FAEDD54D77580E16E7DC5E_gshared (U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB * __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0((RuntimeObject *)__this, /*hidden argument*/NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->set_U3CU3E1__state_0(L_0);
		return;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__7_2_System_IDisposable_Dispose_m549FFA8A72E075502A370C30FAF1400DA5539FBB_gshared (U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB * __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CDelayedInvokeU3Ed__7_2_MoveNext_mE184C4FC64AC3B2ED3940AC0441ECDA9B9C9DC0A_gshared (U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__7_2_MoveNext_mE184C4FC64AC3B2ED3940AC0441ECDA9B9C9DC0A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_U3CU3E1__state_0();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if (!L_1)
		{
			goto IL_0010;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)1)))
		{
			goto IL_0031;
		}
	}
	{
		return (bool)0;
	}

IL_0010:
	{
		__this->set_U3CU3E1__state_0((-1));
		// yield return new WaitForSeconds(delay);
		float L_3 = (float)__this->get_delay_2();
		WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 * L_4 = (WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 *)il2cpp_codegen_object_new(WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_il2cpp_TypeInfo_var);
		WaitForSeconds__ctor_m8E4BA3E27AEFFE5B74A815F26FF8AAB99743F559(L_4, (float)L_3, /*hidden argument*/NULL);
		__this->set_U3CU3E2__current_1(L_4);
		__this->set_U3CU3E1__state_0(1);
		return (bool)1;
	}

IL_0031:
	{
		__this->set_U3CU3E1__state_0((-1));
		// action(arg1, arg2);
		Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C * L_5 = (Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C *)__this->get_action_3();
		RuntimeObject * L_6 = (RuntimeObject *)__this->get_arg1_4();
		RuntimeObject * L_7 = (RuntimeObject *)__this->get_arg2_5();
		NullCheck((Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C *)L_5);
		((  void (*) (Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C *, RuntimeObject *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Action_2_t0DB6FD6F515527EAB552B690A291778C6F00D48C *)L_5, (RuntimeObject *)L_6, (RuntimeObject *)L_7, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return (bool)0;
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__7_2_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_m0CB32B84C1C9F4C7917D577EAD5E9C515405DD51_gshared (U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__7_2_System_Collections_IEnumerator_Reset_mD17AB881B724B98138973411FEAF04404D1D1FAB_gshared (U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__7_2_System_Collections_IEnumerator_Reset_mD17AB881B724B98138973411FEAF04404D1D1FAB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * L_0 = (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 *)il2cpp_codegen_object_new(NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var);
		NotSupportedException__ctor_mA121DE1CAC8F25277DEB489DC7771209D91CAE33(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, U3CDelayedInvokeU3Ed__7_2_System_Collections_IEnumerator_Reset_mD17AB881B724B98138973411FEAF04404D1D1FAB_RuntimeMethod_var);
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__7`2<System.Object,System.Object>::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__7_2_System_Collections_IEnumerator_get_Current_m51A61CDFF454710D5F7ADD1456D7AE38635CA31E_gshared (U3CDelayedInvokeU3Ed__7_2_t700C52144FC66BBCA8037BE7CD03396DB91DCBFB * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
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
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__8_3__ctor_m3C52ED2623C3EC79F612E2B7B64D535B655B528F_gshared (U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07 * __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0((RuntimeObject *)__this, /*hidden argument*/NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->set_U3CU3E1__state_0(L_0);
		return;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__8_3_System_IDisposable_Dispose_m42E8F6DB507305DA2B09B4C27BCA73A9B4F73D63_gshared (U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07 * __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CDelayedInvokeU3Ed__8_3_MoveNext_m5513FC810F1B6A121A15C664061E4816F393D724_gshared (U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__8_3_MoveNext_m5513FC810F1B6A121A15C664061E4816F393D724_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_U3CU3E1__state_0();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if (!L_1)
		{
			goto IL_0010;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)1)))
		{
			goto IL_0031;
		}
	}
	{
		return (bool)0;
	}

IL_0010:
	{
		__this->set_U3CU3E1__state_0((-1));
		// yield return new WaitForSeconds(delay);
		float L_3 = (float)__this->get_delay_2();
		WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 * L_4 = (WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 *)il2cpp_codegen_object_new(WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_il2cpp_TypeInfo_var);
		WaitForSeconds__ctor_m8E4BA3E27AEFFE5B74A815F26FF8AAB99743F559(L_4, (float)L_3, /*hidden argument*/NULL);
		__this->set_U3CU3E2__current_1(L_4);
		__this->set_U3CU3E1__state_0(1);
		return (bool)1;
	}

IL_0031:
	{
		__this->set_U3CU3E1__state_0((-1));
		// action(arg1, arg2, arg3);
		Action_3_tCC14115B7178951118504E7198B7C872630643C5 * L_5 = (Action_3_tCC14115B7178951118504E7198B7C872630643C5 *)__this->get_action_3();
		RuntimeObject * L_6 = (RuntimeObject *)__this->get_arg1_4();
		RuntimeObject * L_7 = (RuntimeObject *)__this->get_arg2_5();
		RuntimeObject * L_8 = (RuntimeObject *)__this->get_arg3_6();
		NullCheck((Action_3_tCC14115B7178951118504E7198B7C872630643C5 *)L_5);
		((  void (*) (Action_3_tCC14115B7178951118504E7198B7C872630643C5 *, RuntimeObject *, RuntimeObject *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Action_3_tCC14115B7178951118504E7198B7C872630643C5 *)L_5, (RuntimeObject *)L_6, (RuntimeObject *)L_7, (RuntimeObject *)L_8, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return (bool)0;
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__8_3_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_mF57CA6483A851106D56CB4FB5D7913D1E1E62C22_gshared (U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__8_3_System_Collections_IEnumerator_Reset_m3DFBF2AC1E000261E6E165CF4A3E0838DCBA48C0_gshared (U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__8_3_System_Collections_IEnumerator_Reset_m3DFBF2AC1E000261E6E165CF4A3E0838DCBA48C0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * L_0 = (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 *)il2cpp_codegen_object_new(NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var);
		NotSupportedException__ctor_mA121DE1CAC8F25277DEB489DC7771209D91CAE33(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, U3CDelayedInvokeU3Ed__8_3_System_Collections_IEnumerator_Reset_m3DFBF2AC1E000261E6E165CF4A3E0838DCBA48C0_RuntimeMethod_var);
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__8`3<System.Object,System.Object,System.Object>::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__8_3_System_Collections_IEnumerator_get_Current_mF7BD53BAE223CF524CECDE72F476C1FD2AF0249B_gshared (U3CDelayedInvokeU3Ed__8_3_tB56F3C5B18192E1C380FD261412BE96EC9767F07 * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
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
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>::.ctor(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__9_4__ctor_m795FB01233C3A25F4DAC3106679F990CCFED9CFB_gshared (U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A * __this, int32_t ___U3CU3E1__state0, const RuntimeMethod* method)
{
	{
		NullCheck((RuntimeObject *)__this);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0((RuntimeObject *)__this, /*hidden argument*/NULL);
		int32_t L_0 = ___U3CU3E1__state0;
		__this->set_U3CU3E1__state_0(L_0);
		return;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>::System.IDisposable.Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__9_4_System_IDisposable_Dispose_m02F41DE85F3CEB9B3DA39F35BD16C4A8BA7BC568_gshared (U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A * __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Boolean Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CDelayedInvokeU3Ed__9_4_MoveNext_m6FF1B65B3B5A3519E409145A8FE083BFF9EB6FC8_gshared (U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__9_4_MoveNext_m6FF1B65B3B5A3519E409145A8FE083BFF9EB6FC8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		int32_t L_0 = (int32_t)__this->get_U3CU3E1__state_0();
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		if (!L_1)
		{
			goto IL_0010;
		}
	}
	{
		int32_t L_2 = V_0;
		if ((((int32_t)L_2) == ((int32_t)1)))
		{
			goto IL_0031;
		}
	}
	{
		return (bool)0;
	}

IL_0010:
	{
		__this->set_U3CU3E1__state_0((-1));
		// yield return new WaitForSeconds(delay);
		float L_3 = (float)__this->get_delay_2();
		WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 * L_4 = (WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8 *)il2cpp_codegen_object_new(WaitForSeconds_t3E9E78D3BB53F03F96C7F28BA9B9086CD1A5F4E8_il2cpp_TypeInfo_var);
		WaitForSeconds__ctor_m8E4BA3E27AEFFE5B74A815F26FF8AAB99743F559(L_4, (float)L_3, /*hidden argument*/NULL);
		__this->set_U3CU3E2__current_1(L_4);
		__this->set_U3CU3E1__state_0(1);
		return (bool)1;
	}

IL_0031:
	{
		__this->set_U3CU3E1__state_0((-1));
		// action(arg1, arg2, arg3, arg4);
		Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 * L_5 = (Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 *)__this->get_action_3();
		RuntimeObject * L_6 = (RuntimeObject *)__this->get_arg1_4();
		RuntimeObject * L_7 = (RuntimeObject *)__this->get_arg2_5();
		RuntimeObject * L_8 = (RuntimeObject *)__this->get_arg3_6();
		RuntimeObject * L_9 = (RuntimeObject *)__this->get_arg4_7();
		NullCheck((Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 *)L_5);
		((  void (*) (Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 *, RuntimeObject *, RuntimeObject *, RuntimeObject *, RuntimeObject *, const RuntimeMethod*))IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0)->methodPointer)((Action_4_t46ACC504A4217F2E92EF2A827836993B819AF923 *)L_5, (RuntimeObject *)L_6, (RuntimeObject *)L_7, (RuntimeObject *)L_8, (RuntimeObject *)L_9, /*hidden argument*/IL2CPP_RGCTX_METHOD_INFO(method->klass->rgctx_data, 0));
		// }
		return (bool)0;
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>::System.Collections.Generic.IEnumerator<System.Object>.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__9_4_System_Collections_Generic_IEnumeratorU3CSystem_ObjectU3E_get_Current_mFF41982CF18AED6617E08F9B8D3B4D4E025C2B98_gshared (U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
	}
}
// System.Void Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>::System.Collections.IEnumerator.Reset()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CDelayedInvokeU3Ed__9_4_System_Collections_IEnumerator_Reset_m17A21D466AB09EF4553CC3BB253C31498F0AD1B7_gshared (U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CDelayedInvokeU3Ed__9_4_System_Collections_IEnumerator_Reset_m17A21D466AB09EF4553CC3BB253C31498F0AD1B7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 * L_0 = (NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010 *)il2cpp_codegen_object_new(NotSupportedException_tE75B318D6590A02A5D9B29FD97409B1750FA0010_il2cpp_TypeInfo_var);
		NotSupportedException__ctor_mA121DE1CAC8F25277DEB489DC7771209D91CAE33(L_0, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_0, U3CDelayedInvokeU3Ed__9_4_System_Collections_IEnumerator_Reset_m17A21D466AB09EF4553CC3BB253C31498F0AD1B7_RuntimeMethod_var);
	}
}
// System.Object Wenzil.Console.MBExtensions/<DelayedInvoke>d__9`4<System.Object,System.Object,System.Object,System.Object>::System.Collections.IEnumerator.get_Current()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * U3CDelayedInvokeU3Ed__9_4_System_Collections_IEnumerator_get_Current_m6B2BD51D78A3557C73CCC868CB6FDE8FBDB24CC0_gshared (U3CDelayedInvokeU3Ed__9_4_t985D836B93123BE398A2AB4479E80520ED79565A * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_U3CU3E2__current_1();
		return (RuntimeObject *)L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void EventBase_set_propagation_mF770530A5B592FAB33C549ED74453ABC7ED0E7A6_inline (EventBase_t0292727F923C187143EFD8B7E78B2A3FB5EFF0CD * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_U3CpropagationU3Ek__BackingField_5(L_0);
		return;
	}
}
