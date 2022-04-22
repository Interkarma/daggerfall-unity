using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace DaggerfallWorkshop.Tests
{
    static class JobATests
    {
        [Test] public static void JobA_Col___0_7() => Assert.AreEqual(expected: 0, actual: JobA.Col(0, 7));
        [Test] public static void JobA_Col___1_7() => Assert.AreEqual(expected: 0, actual: JobA.Col(1, 7));
        [Test] public static void JobA_Col___7_1() => Assert.AreEqual(expected: 7, actual: JobA.Col(7, 1));
        [Test] public static void JobA_Col___7_7() => Assert.AreEqual(expected: 1, actual: JobA.Col(7, 7));

        // [Test] public static void JobA_Row___0_7() => Assert.AreEqual(expected: ?, actual: JobA.Row(0, 7));
        // [Test] public static void JobA_Row___1_7() => Assert.AreEqual(expected: ?, actual: JobA.Row(1, 7));
        // [Test] public static void JobA_Row___7_1() => Assert.AreEqual(expected: ?, actual: JobA.Row(7, 1));
        // [Test] public static void JobA_Row___7_7() => Assert.AreEqual(expected: ?, actual: JobA.Row(7, 7));
    }
}
