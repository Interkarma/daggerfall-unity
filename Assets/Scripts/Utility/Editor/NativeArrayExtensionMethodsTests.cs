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

using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

static class NativeArrayExtensionMethodsTests
{


    [Test] public static void CopyTo__NativeArray__managed_array_2d()
    {
        // create both arrays
        var dst = new int[2, 4];
        int
            lengthY = dst.GetLength(0),
            lengthX = dst.GetLength(1),
            length = dst.Length;
        var src = new NativeArray<int>(length, Allocator.Temp);
        for (int i = 0; i < length; i++)
            src[i] = i;

        Debug.Log($"src before: {src.ToReadableString()}");
        Debug.Log($"dst before: {dst.ToReadableString()}");

        // copy
        src.CopyTo(dst);

        Debug.Log($"src after: {src.ToReadableString()}");
        Debug.Log($"dst after: {dst.ToReadableString()}");

        // test
        {
            int i = 0;
            for (int y = 0; y < lengthY; y++)
            for (int x = 0; x < lengthX; x++)
            {
                int
                    expected = src[i++],
                    actual = dst[y, x];
                Assert.AreEqual(expected: expected, actual: actual, delta: 0);
            }
        }
    }

    [Test] public static void CopyTo__NativeArray__managed_array_3d()
    {
        // create both arrays
        var dst = new int[2, 4, 3];
        int
            lengthZ = dst.GetLength(0),
            lengthY = dst.GetLength(1),
            lengthX = dst.GetLength(2),
            length = dst.Length;
        var src = new NativeArray<int>(length, Allocator.Temp);
        for (int i = 0; i < length; i++)
            src[i] = i;

        Debug.Log($"src before: {src.ToReadableString()}");
        Debug.Log($"dst before: {dst.ToReadableString()}");

        // copy
        src.CopyTo(dst);

        Debug.Log($"src after: {src.ToReadableString()}");
        Debug.Log($"dst after: {dst.ToReadableString()}");

        // test
        {
            int i = 0;
            for (int z = 0; z < lengthZ; z++)
            for (int y = 0; y < lengthY; y++)
            for (int x = 0; x < lengthX; x++)
            {
                int
                    expected = src[i++],
                    actual = dst[z, y, x];
                Assert.AreEqual(expected: expected, actual: actual, delta: 0);
            }
        }
    }

    [Test] public static void CopyFrom__NativeArray__managed_array_2d()
    {
        // create both arrays
        var src = new int[2, 4];
        int
            lengthY = src.GetLength(0),
            lengthX = src.GetLength(1),
            length = src.Length;
        for (int y = 0; y < lengthY; y++)
        for (int x = 0; x < lengthX; x++)
            src[y, x] = y * 100 + x;
        var dst = new NativeArray<int>(length, Allocator.Temp);        

        Debug.Log($"src before: {src.ToReadableString()}");
        Debug.Log($"dst before: {dst.ToReadableString()}");

        // copy
        dst.CopyFrom(src);

        Debug.Log($"src after: {src.ToReadableString()}");
        Debug.Log($"dst after: {dst.ToReadableString()}");

        // test
        {
            int i = 0;
            for (int y = 0; y < lengthY; y++)
            for (int x = 0; x < lengthX; x++)
            {
                int
                    expected = src[y, x],
                    actual = dst[i++];
                Assert.AreEqual(expected: expected, actual: actual, delta: 0);
            }
        }
    }

    [Test] public static void CopyFrom__NativeArray__managed_array_3d()
    {
        // create both arrays
        var src = new int[2, 4, 3];
        int
            lengthZ = src.GetLength(0),
            lengthY = src.GetLength(1),
            lengthX = src.GetLength(2),
            length = src.Length;
        for (int z = 0; z < lengthZ; z++)
        for (int y = 0; y < lengthY; y++)
        for (int x = 0; x < lengthX; x++)
            src[z, y, x] = z * 10000 + y * 100 + x;
        var dst = new NativeArray<int>(length, Allocator.Temp);

        Debug.Log($"src before: {src.ToReadableString()}");
        Debug.Log($"dst before: {dst.ToReadableString()}");

        // copy
        dst.CopyFrom(src);

        Debug.Log($"src after: {src.ToReadableString()}");
        Debug.Log($"dst after: {dst.ToReadableString()}");

        // test
        {
            int i = 0;
            for (int z = 0; z < lengthZ; z++)
            for (int y = 0; y < lengthY; y++)
            for (int x = 0; x < lengthX; x++)
            {
                int
                    expected = src[z, y, x],
                    actual = dst[i++];
                Assert.AreEqual(expected: expected, actual: actual, delta: 0);
            }
        }
    }


    [Test] public static unsafe void AssertPtrScope__NativeArray__ptr___byte_pointer_address_0()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        byte* ptr = (byte*)0;
        Assert.Throws<System.ArgumentOutOfRangeException>(() => array.AssertPtrScope(ptr));
    }
    [Test] public static unsafe void AssertPtrScope__NativeArray__ptr___long_pointer_address_12345566890()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        long* ptr = (long*)12345566890;
        Assert.Throws<System.ArgumentOutOfRangeException>(() => array.AssertPtrScope(ptr));
    }
    [Test] public static unsafe void AssertPtrScope__NativeArray__ptr___long_pointer_address_12345566890_negative()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        long* ptr = (long*)-12345566890;
        Assert.Throws<System.ArgumentOutOfRangeException>(() => array.AssertPtrScope(ptr));
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__start_of_the_array()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array);
        array.AssertPtrScope(ptr);
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__start_of_the_array_plus_1_byte()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)((byte*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array)+1);
        array.AssertPtrScope(ptr);
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__end_of_the_array_minus_1_byte()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)((byte*)((float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array)+3)-1);
        Assert.Throws<System.ArgumentOutOfRangeException>(() => array.AssertPtrScope(ptr));
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__end_of_the_array_plus_1_byte()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)((byte*)((float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array)+3)+1);
        Assert.Throws<System.ArgumentOutOfRangeException>(() => array.AssertPtrScope(ptr));
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__last_item_of_the_array()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array)+2;
        array.AssertPtrScope(ptr);
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__last_item_of_the_array_minus_1_byte()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)((byte*)((float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array)+2)-1);
        array.AssertPtrScope(ptr);
    }
    [Test] public static unsafe void AssertPtrScope_float__NativeArray__last_item_of_the_array_plus_1_byte()
    {
        var array = new NativeArray<float>(3, Allocator.Temp);
        float* ptr = (float*)((byte*)((float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(array)+2)+1);
        Assert.Throws<System.ArgumentOutOfRangeException>(() => array.AssertPtrScope(ptr));
    }


    [Test] public static unsafe void does_AsNativeArray_work()
    {
        var array = new int[]{ 1 , 2 , 3 , 4 };
        var nativeArray = array.AsNativeArray(out ulong gcHandle);

        Assert.AreEqual(expected:array.Length, actual:nativeArray.Length);
        for (int i = 0; i < array.Length; i++)
            Assert.AreEqual(expected:array[i], actual:nativeArray[i]);
        
        JobUtility.ReleaseGCObject(gcHandle);
    }

    [Test] public static unsafe void does_ConvertExistingDataToNativeArray_work()
    {
        var array = new int[]{ 1 , 2 , 3 , 4 };
        
        void* ptr = UnsafeUtility.PinGCArrayAndGetDataAddress(array, out ulong gcHandle);
        var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(ptr, array.Length, Allocator.None);
        
        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.GetTempMemoryHandle());
        #endif

        Assert.AreEqual(expected:array.Length, actual:nativeArray.Length);
        for (int i = 0; i < array.Length; i++)
            Assert.AreEqual(expected:array[i], actual:nativeArray[i]);
        
        UnsafeUtility.ReleaseGCObject(gcHandle);
    }


    [Test] public static unsafe void does_AsNativeArray_2d_work()
    {
        var array = new int[,] { { 1, 2, 3, 4 }, { 11, 22, 33, 44}, { 111, 222, 333, 444} };
        var nativeArray = array.AsNativeArray(out ulong gcHandle);
        
        int dim0 = array.GetLength(0);
        int dim1 = array.GetLength(1);

        Assert.AreEqual(expected:array.Length, actual:nativeArray.Length);
        int i = 0;
        for (int y = 0; y < dim0; y++)
        for (int x = 0; x < dim1; x++)
            Assert.AreEqual(expected:array[y,x], actual:nativeArray[i++]);
        
        JobUtility.ReleaseGCObject(gcHandle);
    }


}
