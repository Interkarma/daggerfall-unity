// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Andrzej Łukasik (andrew.r.lukasik)
// Contributors:    
// 
// Notes: Requires "Allow Unsafe Code" enabled ☑
//

using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;

public static class NativeArrayExtensionMethods
{

    static readonly ProfilerMarker
        ___CopyTo = new ProfilerMarker("CopyTo"),
        ___CopyFrom = new ProfilerMarker("CopyFrom");

    public static unsafe void CopyTo<T>(this NativeArray<T> src, T[,] dst) where T : unmanaged
    {
        ___CopyTo.Begin();
        if (src.Length == dst.Length)
        {
            int size = src.Length * UnsafeUtility.SizeOf<T>();
            void* srcPtr = NativeArrayUnsafeUtility.GetUnsafePtr(src);
            void* dstPtr = UnsafeUtility.PinGCArrayAndGetDataAddress(dst, out ulong dstHandle);
            UnsafeUtility.MemCpy(destination: dstPtr, source: srcPtr, size: size);
            UnsafeUtility.ReleaseGCObject(dstHandle);
        }
        else Debug.LogError($"<b>{nameof(src)}.Length</b> ({src}[b]) and <b>{nameof(dst)}.Length</b> ({dst.Length}[b]) must be equal. MemCpy aborted.");
        ___CopyTo.End();
    }
    public static unsafe void CopyTo<T>(this NativeArray<T> src, T[,,] dst) where T : unmanaged
    {
        ___CopyTo.Begin();
        if (src.Length == dst.Length)
        {
            int size = src.Length * UnsafeUtility.SizeOf<T>();
            void* srcPtr = NativeArrayUnsafeUtility.GetUnsafePtr(src);
            void* dstPtr = UnsafeUtility.PinGCArrayAndGetDataAddress(dst, out ulong dstHandle);
            UnsafeUtility.MemCpy(destination: dstPtr, source: srcPtr, size: size);
            UnsafeUtility.ReleaseGCObject(dstHandle);
        }
        else Debug.LogError($"<b>{nameof(src)}.Length</b> ({src}[b]) and <b>{nameof(dst)}.Length</b> ({dst.Length}[b]) must be equal. MemCpy aborted.");
        ___CopyTo.End();
    }

    public static unsafe void CopyFrom<T>(this NativeArray<T> dst, T[,] src) where T : unmanaged
    {
        ___CopyFrom.Begin();
        if (src.Length == dst.Length)
        {
            int size = src.Length * UnsafeUtility.SizeOf<T>();
            void* srcPtr = UnsafeUtility.PinGCArrayAndGetDataAddress(src, out ulong dstHandle);
            void* dstPtr = NativeArrayUnsafeUtility.GetUnsafePtr(dst);
            UnsafeUtility.MemCpy(destination: dstPtr, source: srcPtr, size: size);
            UnsafeUtility.ReleaseGCObject(dstHandle);
        }
        else Debug.LogError($"<b>{nameof(src)}.Length</b> ({src}[b]) and <b>{nameof(dst)}.Length</b> ({dst.Length}[b]) must be equal. MemCpy aborted.");
        ___CopyFrom.End();
    }
    public static unsafe void CopyFrom<T>(this NativeArray<T> dst, T[,,] src) where T : unmanaged
    {
        ___CopyFrom.Begin();
        if (src.Length == dst.Length)
        {
            int size = src.Length * UnsafeUtility.SizeOf<T>();
            void* srcPtr = UnsafeUtility.PinGCArrayAndGetDataAddress(src, out ulong dstHandle);
            void* dstPtr = NativeArrayUnsafeUtility.GetUnsafePtr(dst);
            UnsafeUtility.MemCpy(destination: dstPtr, source: srcPtr, size: size);
            UnsafeUtility.ReleaseGCObject(dstHandle);
        }
        else Debug.LogError($"<b>{nameof(src)}.Length</b> ({src}[b]) and <b>{nameof(dst)}.Length</b> ({dst.Length}[b]) must be equal. MemCpy aborted.");
        ___CopyFrom.End();
    }


    /// <summary> Schedules a <see cref="CopyToJob{T}"/>. </summary>
    [Unity.Burst.BurstDiscard]// Burst warning without it, not sure why
    public static JobHandle CopyTo<T>(this NativeArray<T> src, NativeArray<T> dst, JobHandle dependency) where T : unmanaged
        => new CopyToJob<T>(src,dst).Schedule(dependency);
    
    /// <summary> Schedules a <see cref="CopyToJob{T}"/>. </summary>
    public static JobHandle CopyFrom<T>(this NativeArray<T> dst, NativeArray<T> src, JobHandle dependency) where T : unmanaged
        => new CopyToJob<T>(src,dst).Schedule(dependency);

    /// <summary> Fills entire array using given value. </summary>
    public static unsafe void Fill<T>(this NativeArray<T> Array, T value) where T : unmanaged
    {
        void* src = UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), Allocator.Temp);
        void* dst = NativeArrayUnsafeUtility.GetUnsafePtr<T>(Array);
        int size = UnsafeUtility.SizeOf<T>();
        int count = Array.Length;
        UnsafeUtility.MemCpyReplicate(destination: dst, source: src, size: size, count: count);
    }


    /// <summary>
    /// Raises an exception when given pointer refers to address outside this array.
    /// This makes sure this pointer won't crash the game.
    /// <b>NOTE</b>: Editor and DEBUG builds only.
    /// </summary>
    [System.Diagnostics.Conditional("DEBUG")]
    public static unsafe void AssertPtrScope<ARRAY_ITEM, POINTER>(this NativeSlice<ARRAY_ITEM> array, POINTER* ptr)
        where ARRAY_ITEM : unmanaged
        where POINTER : unmanaged
    {
        long addr = (long)ptr;
        long firstItemAddr = (long)NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr(array);
        long lastItemAddr = firstItemAddr + array.Length * (long)UnsafeUtility.SizeOf<ARRAY_ITEM>() - UnsafeUtility.SizeOf<POINTER>();
        if (!(addr >= firstItemAddr && addr < lastItemAddr))
        {
            string message = $"Pointer is out of scope, so considered unsafe (possible crash prevented). Ptr:{addr}, array first item addr:{firstItemAddr}, array last item addr:{lastItemAddr}";
            Debug.LogWarning(message);
            throw new System.ArgumentOutOfRangeException(message);
        }
    }

    /// <inheritdoc />
    [System.Diagnostics.Conditional("DEBUG")]
    public static unsafe void AssertPtrScope<ARRAY_ITEM, POINTER>(this NativeArray<ARRAY_ITEM> array, POINTER* ptr)
        where ARRAY_ITEM : unmanaged
        where POINTER : unmanaged
    {
        long addr = (long)ptr;
        long firstItemAddr = (long)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array);
        long lastItemAddr = firstItemAddr + array.Length * (long)UnsafeUtility.SizeOf<ARRAY_ITEM>() - UnsafeUtility.SizeOf<POINTER>();
        if (!(addr >= firstItemAddr && addr <= lastItemAddr))
        {
            string message = $"Pointer is out of scope, so considered unsafe (possible crash prevented). Ptr:{addr}, array first item addr:{firstItemAddr}, array last item addr:{lastItemAddr}";
            Debug.LogWarning(message);
            throw new System.ArgumentOutOfRangeException(message);
        }
    }

    public static string ToReadableString<T>(this NativeArray<T> arr) where T : unmanaged
    {
        if (arr.Length == 0) return "()";
        var sb = new System.Text.StringBuilder();
        sb.Append($"({arr[0]}");
        for (int i = 1; i < arr.Length; i++)
            sb.Append($",{arr[i]}");
        sb.Append(')');
        return sb.ToString();
    }


}
