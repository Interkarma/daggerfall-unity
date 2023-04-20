#pragma once

#include "il2cpp-config.h"

#include <cmath>

#include "il2cpp-object-internals.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-tabledefs.h"

#include "vm-utils/Debugger.h"
#include "utils/LeaveTargetStack.h"
#include "utils/Output.h"

REAL_NORETURN IL2CPP_NO_INLINE void il2cpp_codegen_no_return();

#if IL2CPP_COMPILER_MSVC
#define DEFAULT_CALL STDCALL
#else
#define DEFAULT_CALL
#endif

#if defined(__ARMCC_VERSION)
inline double bankers_round(double x)
{
    return __builtin_round(x);
}

inline float bankers_roundf(float x)
{
    return __builtin_roundf(x);
}

#else
inline double bankers_round(double x)
{
    double integerPart;
    if (x >= 0.0)
    {
        if (modf(x, &integerPart) == 0.5)
            return (int64_t)integerPart % 2 == 0 ? integerPart : integerPart + 1.0;
        return floor(x + 0.5);
    }
    else
    {
        if (modf(x, &integerPart) == -0.5)
            return (int64_t)integerPart % 2 == 0 ? integerPart : integerPart - 1.0;
        return ceil(x - 0.5);
    }
}

inline float bankers_roundf(float x)
{
    double integerPart;
    if (x >= 0.0f)
    {
        if (modf(x, &integerPart) == 0.5)
            return (int64_t)integerPart % 2 == 0 ? (float)integerPart : (float)integerPart + 1.0f;
        return floorf(x + 0.5f);
    }
    else
    {
        if (modf(x, &integerPart) == -0.5)
            return (int64_t)integerPart % 2 == 0 ? (float)integerPart : (float)integerPart - 1.0f;
        return ceilf(x - 0.5f);
    }
}

#endif

// returns true if overflow occurs
inline bool il2cpp_codegen_check_mul_overflow_i64(int64_t a, int64_t b, int64_t imin, int64_t imax)
{
    // TODO: use a better algorithm without division
    uint64_t ua = (uint64_t)llabs(a);
    uint64_t ub = (uint64_t)llabs(b);

    uint64_t c;
    if ((a > 0 && b > 0) || (a <= 0 && b <= 0))
        c = (uint64_t)llabs(imax);
    else
        c = (uint64_t)llabs(imin);

    return ua != 0 && ub > c / ua;
}

inline bool il2cpp_codegen_check_mul_oveflow_u64(uint64_t a, uint64_t b)
{
    return b != 0 && (a * b) / b != a;
}

inline int32_t il2cpp_codegen_abs(uint32_t value)
{
    return abs(static_cast<int32_t>(value));
}

inline int32_t il2cpp_codegen_abs(int32_t value)
{
    return abs(value);
}

inline int64_t il2cpp_codegen_abs(uint64_t value)
{
    return llabs(static_cast<int64_t>(value));
}

inline int64_t il2cpp_codegen_abs(int64_t value)
{
    return llabs(value);
}

template<typename TInput, typename TOutput, typename TFloat>
inline TOutput il2cpp_codegen_cast_floating_point(TFloat value)
{
#if IL2CPP_TARGET_ARM64 || IL2CPP_TARGET_ARMV7
    // On ARM, a cast from a floating point to integer value will use
    // the min or max value if the cast is out of range (instead of
    // overflowing like x86/x64). So first do a cast to the output
    // type (which is signed in .NET - the value stack does not have
    // unsigned types) to try to get the value into a range that will
    // actually be cast.
    if (value < 0)
        return (TOutput)((TInput)(TOutput)value);
#endif
    return (TOutput)((TInput)value);
}

// ARM targets handle a cast of floating point positive infinity (0x7F800000)
// differently from Intel targets. The expected behavior for .NET is from Intel,
// where the cast to a 32-bit int produces the value 0x80000000. On ARM, the sign
// is unchanged, producing 0x7FFFFFFF. To work around this change the positive
// infinity value to negative infinity.
template<typename T>
inline T il2cpp_codegen_cast_double_to_int(double value)
{
#if IL2CPP_TARGET_ARM64 || IL2CPP_TARGET_ARMV7
    if (value == INFINITY)
        return (T)-value;
#endif
    return (T)value;
}

// Exception support macros
#define IL2CPP_LEAVE(Offset, Target) \
    __leave_targets.push(Offset); \
    goto Target;

#define IL2CPP_END_FINALLY(Id) \
    goto __CLEANUP_ ## Id;

#define IL2CPP_CLEANUP(Id) \
    __CLEANUP_ ## Id:

#define IL2CPP_RETHROW_IF_UNHANDLED(ExcType) \
    if(__last_unhandled_exception) { \
        ExcType _tmp_exception_local = __last_unhandled_exception; \
        __last_unhandled_exception = 0; \
        il2cpp_codegen_raise_exception(_tmp_exception_local); \
        }

#define IL2CPP_JUMP_TBL(Offset, Target) \
    if(!__leave_targets.empty() && __leave_targets.top() == Offset) { \
        __leave_targets.pop(); \
        goto Target; \
        }

#define IL2CPP_END_CLEANUP(Offset, Target) \
    if(!__leave_targets.empty() && __leave_targets.top() == Offset) \
        goto Target;


#define IL2CPP_RAISE_MANAGED_EXCEPTION(message, lastManagedFrame) \
    do {\
        il2cpp_codegen_raise_exception((Exception_t*)message, (MethodInfo*)lastManagedFrame);\
        il2cpp_codegen_no_return();\
    } while (0)

#if IL2CPP_ENABLE_WRITE_BARRIERS
void Il2CppCodeGenWriteBarrier(void** targetAddress, void* object);
#else
inline void Il2CppCodeGenWriteBarrier(void** targetAddress, void* object) {}
#endif

void il2cpp_codegen_memory_barrier();

template<typename T>
inline T VolatileRead(T* location)
{
    T result = *location;
    il2cpp_codegen_memory_barrier();
    return result;
}

template<typename T, typename U>
inline void VolatileWrite(T** location, U* value)
{
    il2cpp_codegen_memory_barrier();
    *location = value;
    Il2CppCodeGenWriteBarrier((void**)location, value);
}

template<typename T, typename U>
inline void VolatileWrite(T* location, U value)
{
    il2cpp_codegen_memory_barrier();
    *location = value;
}

inline void il2cpp_codegen_write_to_stdout(const char* str)
{
    il2cpp::utils::Output::WriteToStdout(str);
}

#if IL2CPP_TARGET_LUMIN
inline void il2cpp_codegen_write_to_stdout_args(const char* str, ...)
{
    va_list args, local;
    char* buffer = nullptr;
    va_start(args, str);
    va_copy(local, args);
    int size = vsnprintf(nullptr, 0, str, local);
    if (size < 0)
    {
        va_end(local);
        va_end(args);
        return;
    }
    va_end(local);
    va_copy(local, args);
    buffer = new char[size + 1];
    vsnprintf(buffer, size + 1, str, local);
    il2cpp::utils::Output::WriteToStdout(buffer);
    if (buffer != nullptr)
        delete[] buffer;
    va_end(local);
    va_end(args);
}

#endif

inline void il2cpp_codegen_write_to_stderr(const char* str)
{
    il2cpp::utils::Output::WriteToStderr(str);
}

#if IL2CPP_TARGET_LUMIN
inline void il2cpp_codegen_write_to_stderr_args(const char* str, ...)
{
    va_list args, local;
    char* buffer = nullptr;
    va_start(args, str);
    va_copy(local, args);
    int size = vsnprintf(nullptr, 0, str, local);
    if (size < 0)
    {
        va_end(local);
        va_end(args);
        return;
    }
    va_end(local);
    va_copy(local, args);
    buffer = new char[size + 1];
    vsnprintf(buffer, size + 1, str, local);
    il2cpp::utils::Output::WriteToStderr(buffer);
    if (buffer != nullptr)
        delete[] buffer;
    va_end(local);
    va_end(args);
}

#endif

REAL_NORETURN void il2cpp_codegen_abort();

inline bool il2cpp_codegen_check_add_overflow(int64_t left, int64_t right)
{
    return (right >= 0 && left > kIl2CppInt64Max - right) ||
        (left < 0 && right < kIl2CppInt64Min - left);
}

inline bool il2cpp_codegen_check_sub_overflow(int64_t left, int64_t right)
{
    return (right >= 0 && left < kIl2CppInt64Min + right) ||
        (right < 0 && left > kIl2CppInt64Max + right);
}

template<bool, class T, class U>
struct pick_first;

template<class T, class U>
struct pick_first<true, T, U>
{
    typedef T type;
};

template<class T, class U>
struct pick_first<false, T, U>
{
    typedef U type;
};

template<class T, class U>
struct pick_bigger
{
    typedef typename pick_first<(sizeof(T) >= sizeof(U)), T, U>::type type;
};

template<typename T, typename U>
inline typename pick_bigger<T, U>::type il2cpp_codegen_multiply(T left, U right)
{
    return left * right;
}

template<typename T, typename U>
inline typename pick_bigger<T, U>::type il2cpp_codegen_add(T left, U right)
{
    return left + right;
}

template<typename T, typename U>
inline typename pick_bigger<T, U>::type il2cpp_codegen_subtract(T left, U right)
{
    return left - right;
}

inline void il2cpp_codegen_memcpy(void* dest, const void* src, size_t count)
{
    memcpy(dest, src, count);
}

inline void il2cpp_codegen_memset(void* ptr, int value, size_t num)
{
    memset(ptr, value, num);
}

inline void il2cpp_codegen_register_debugger_data(const Il2CppDebuggerMetadataRegistration *data)
{
#if IL2CPP_MONO_DEBUGGER
    il2cpp::utils::Debugger::RegisterMetadata(data);
#endif
}

inline void il2cpp_codegen_check_sequence_point(Il2CppSequencePointExecutionContext* executionContext, Il2CppSequencePoint* seqPoint)
{
#if IL2CPP_MONO_DEBUGGER
    il2cpp::utils::Debugger::CheckSequencePoint(executionContext, seqPoint);
#endif
}

inline void il2cpp_codegen_check_sequence_point_entry(Il2CppSequencePointExecutionContext* executionContext, Il2CppSequencePoint* seqPoint)
{
#if IL2CPP_MONO_DEBUGGER
    il2cpp::utils::Debugger::CheckSequencePointEntry(executionContext, seqPoint);
#endif
}

inline void il2cpp_codegen_check_sequence_point_exit(Il2CppSequencePointExecutionContext* executionContext, Il2CppSequencePoint* seqPoint)
{
#if IL2CPP_MONO_DEBUGGER
    il2cpp::utils::Debugger::CheckSequencePointExit(executionContext, seqPoint);
#endif
}

inline void il2cpp_codegen_check_pause_point()
{
#if IL2CPP_MONO_DEBUGGER
    il2cpp::utils::Debugger::CheckPausePoint();
#endif
}

class MethodExitSequencePointChecker
{
private:
    Il2CppSequencePoint* m_seqPoint;
    Il2CppSequencePointExecutionContext* m_seqPointStorage;

public:
    MethodExitSequencePointChecker(Il2CppSequencePointExecutionContext* seqPointStorage, Il2CppSequencePoint* seqPoint) :
        m_seqPointStorage(seqPointStorage), m_seqPoint(seqPoint)
    {
    }

    ~MethodExitSequencePointChecker()
    {
#if IL2CPP_MONO_DEBUGGER
        il2cpp_codegen_check_sequence_point_exit(m_seqPointStorage, m_seqPoint);
#endif
    }
};

#ifdef _MSC_VER
#define IL2CPP_DISABLE_OPTIMIZATIONS __pragma(optimize("", off))
#define IL2CPP_ENABLE_OPTIMIZATIONS __pragma(optimize("", on))
#elif IL2CPP_TARGET_LINUX
#define IL2CPP_DISABLE_OPTIMIZATIONS
#define IL2CPP_ENABLE_OPTIMIZATIONS
#else
#define IL2CPP_DISABLE_OPTIMIZATIONS __attribute__ ((optnone))
#define IL2CPP_ENABLE_OPTIMIZATIONS
#endif

// NativeArray macros
#define IL2CPP_NATIVEARRAY_GET_ITEM(TElementType, TTField, TIndex) \
    *(reinterpret_cast<TElementType*>(TTField) + TIndex)

#define IL2CPP_NATIVEARRAY_SET_ITEM(TElementType, TTField, TIndex, TValue) \
   *(reinterpret_cast<TElementType*>(TTField) + TIndex) = TValue;

#define IL2CPP_NATIVEARRAY_GET_LENGTH(TLengthField) \
   (TLengthField)

// Array Unsafe
#define IL2CPP_ARRAY_UNSAFE_LOAD(TArray, TIndex) \
    (TArray)->GetAtUnchecked(static_cast<il2cpp_array_size_t>(TIndex))

inline bool il2cpp_codegen_object_reference_equals(const RuntimeObject *obj1, const RuntimeObject *obj2)
{
    return obj1 == obj2;
}

inline bool il2cpp_codegen_platform_is_osx_or_ios()
{
    return IL2CPP_TARGET_OSX != 0 || IL2CPP_TARGET_IOS != 0;
}

inline bool il2cpp_codegen_platform_is_freebsd()
{
    // we don't currently support FreeBSD
    return false;
}

inline bool il2cpp_codegen_platform_disable_libc_pinvoke()
{
    return IL2CPP_PLATFORM_DISABLE_LIBC_PINVOKE;
}

template<typename T>
inline T il2cpp_unsafe_read_unaligned(void* location)
{
    T result;
#if IL2CPP_TARGET_ARMV7 || IL2CPP_TARGET_JAVASCRIPT
    memcpy(&result, location, sizeof(T));
#else
    result = *((T*)location);
#endif
    return result;
}

#define IL2CPP_UNSAFE_READ_UNALIGNED(TReturnType, location) il2cpp_unsafe_read_unaligned<TReturnType>(location)

template<typename T>
inline void il2cpp_unsafe_write_unaligned(void* location, T value)
{
#if IL2CPP_TARGET_ARMV7 || IL2CPP_TARGET_JAVASCRIPT
    memcpy(location, &value, sizeof(T));
#else
    *((T*)location) = value;
#endif
}
