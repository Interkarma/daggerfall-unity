// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Andrzej Łukasik (andrew.r.lukasik)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;

public static class ArrayExtensionMethods
{


    /// <summary>
    /// Pins this GC array and turns it's pointer into a NativeArray.
    /// Do not Dispose but call UnsafeUtility.ReleaseGCObject(gcHandle) when done with it.
    /// </summary>
    public static unsafe NativeArray<T> AsNativeArray<T>(this T[] array, out ulong gcHandle) where T : unmanaged
    {
        void* ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle);
        var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, array.Length, Allocator.None);
        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
        #endif
        return nativeArray;
    }
    /// <inheritdoc />
    public static unsafe NativeArray<T> AsNativeArray<T>(this T[,] array, out ulong gcHandle) where T : unmanaged
    {
        void* ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle);
        var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, array.Length, Allocator.None);
        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
        #endif
        return nativeArray;
    }
    /// <inheritdoc />
    public static unsafe NativeArray<T> AsNativeArray<T>(this T[,,] array, out ulong gcHandle) where T : unmanaged
    {
        void* ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle);
        var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, array.Length, Allocator.None);
        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
        #endif
        return nativeArray;
    }
    /// <inheritdoc />
    public static unsafe NativeArray<(T1,T2)> AsNativeArray<T1,T2>(this (T1,T2)[] array, out ulong gcHandle)
        where T1 : unmanaged
        where T2 : unmanaged
    {
        void* ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle);
        var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<(T1,T2)>(ptr, array.Length, Allocator.None);
        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
        #endif
        return nativeArray;
    }
    /// <inheritdoc />
    public static unsafe NativeArray<(T1,T2,T3)> AsNativeArray<T1,T2,T3>(this (T1,T2,T3)[] array, out ulong gcHandle)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        void* ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle);
        var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<(T1,T2,T3)>(ptr, array.Length, Allocator.None);
        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
        #endif
        return nativeArray;
    }


    public static string ToReadableString<T>(this T[] arr)
    {
        if (arr.Length == 0) return "()";
        var sb = new System.Text.StringBuilder();
        sb.Append($"({arr[0]}");
        for (int i = 1; i < arr.Length; i++)
            sb.Append($",{arr[i]}");
        sb.Append(')');
        return sb.ToString();
    }
    public static string ToReadableString<T>(this T[,] arr)
    {
        int
            lengthY = arr.GetLength(0),
            lengthX = arr.GetLength(1),
            length = arr.Length;
        var sb = new System.Text.StringBuilder($"[{lengthY},{lengthX}]( ");
        for (int y = 0; y < lengthY; y++)
        {
            if (y != 0)
                sb.Append(" , ");
            if (lengthX != 0)
                sb.Append($"({arr[y, 0]}");
            for (int x = 1; x < lengthX; x++)
                sb.Append($",{arr[y, x]}");
            sb.Append(')');
        }
        sb.Append($" )");
        return sb.ToString();
    }
    public static string ToReadableString<T>(this T[,,] arr)
    {
        int
            lengthZ = arr.GetLength(0),
            lengthY = arr.GetLength(1),
            lengthX = arr.GetLength(2),
            length = arr.Length;
        var sb = new System.Text.StringBuilder($"[{lengthZ},{lengthY},{lengthX}]( ");
        for (int z = 0; z < lengthZ; z++)
        {
            if (z != 0) sb.Append(" , ");
            sb.Append("( ");
            for (int y = 0; y < lengthY; y++)
            {
                if (y != 0)
                    sb.Append(" , ");
                if (lengthX != 0)
                sb.Append($"({arr[z, y, 0]}");
                for (int x = 1; x < lengthX; x++)
                    sb.Append($",{arr[z, y, x]}");
                sb.Append(')');
            }
            sb.Append(" )");
        }
        sb.Append($" )");
        return sb.ToString();
    }


}
