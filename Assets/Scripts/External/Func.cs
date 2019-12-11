namespace System
{
    // Func delegates with more than four arguments are only available on .NET Framework 4.0 and above
    // Here's a substitute while we are on .NET Framework 3.5
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}