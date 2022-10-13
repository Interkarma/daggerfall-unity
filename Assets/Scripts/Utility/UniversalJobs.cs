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

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

[Unity.Burst.BurstCompile]
public struct DeallocateArrayJob<T> : IJob where T : unmanaged
{
    [ReadOnly] [DeallocateOnJobCompletion] NativeArray<T> Array;
    public DeallocateArrayJob(NativeArray<T> array) => this.Array = array;
    void IJob.Execute() { }
}

[Unity.Burst.BurstCompile]
public struct FillJob<T> : IJob where T : unmanaged
{
    T Value;
    [WriteOnly] NativeArray<T> Array;
    public FillJob(NativeArray<T> array, T value)
    {
        this.Value = value;
        this.Array = array;
    }
    unsafe void IJob.Execute()
    {
        void* src = &Value;
        void* dst = NativeArrayUnsafeUtility.GetUnsafePtr<T>(Array);
        int size = UnsafeUtility.SizeOf<T>();
        int count = this.Array.Length;
        UnsafeUtility.MemCpyReplicate(destination: dst, source: src, size: size, count: count);
    }
}

[Unity.Burst.BurstCompile]
public struct CopyToJob<T> : IJob where T : unmanaged
{
    [ReadOnly] NativeArray<T> Src;
    [WriteOnly] NativeArray<T> Dst;
    int SrcIndex, DstIndex, Length;
    public CopyToJob(NativeArray<T> src, NativeArray<T> dst)
    {
        this.Src = src;
        this.SrcIndex = -1;
        this.Dst = dst;
        this.DstIndex = -1;
        this.Length = -1;
    }
    public CopyToJob(NativeArray<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
    {
        this.Src = src;
        this.SrcIndex = srcIndex;
        this.Dst = dst;
        this.DstIndex = dstIndex;
        this.Length = length;
    }
    void IJob.Execute()
    {
        if (Length == -1) NativeArray<T>.Copy(Src, Dst);
        else NativeArray<T>.Copy(Src, SrcIndex, Dst, DstIndex, Length);
    }
}

[Unity.Burst.BurstCompile]
public struct ReleaseGCObjectJob : IJob
{
    ulong Handle;
    public ReleaseGCObjectJob(ulong handle) => this.Handle = handle;
    void IJob.Execute() => UnsafeUtility.ReleaseGCObject(Handle);
}
