using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NUnit.Framework;
using Portkit.Core.Cryptography;

namespace Portkit.UnitTests.Core
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class Sha1Tests
    {
        [Test]
        public void PerformanceTest()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 1000000; i++)
            {
                sb.Append(i);
            }
            var shaGenerator = new Sha1();
            string msg = sb.ToString();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            stopwatch.Stop();
            Assert.IsTrue(actual.Length == 40, "Hash length is wrong.");
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 600, "Hashing is too slow.");
        }

        [Test]
        public void InvalidMessageHashingTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "abc";
            const string expected = "81fe8bfe87576c3ecb22426f8e57847382917acf"; //abcd
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual != expected);
        }

        [Test]
        public void SingleGeneratorDoubleHashingTest()
        {
            var shaGenerator = new Sha1();

            const string msg = "abc";
            const string expected = "a9993e364706816aba3e25717850c26c9cd0d89d"; //abc
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual == expected);

            actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual != expected);
        }

        [Test]
        public void EmptyMessageHashingTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "";
            const string expected = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual == expected);
        }

        [Test]
        public void ShortMessageHashingTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "abc";
            const string expected = "a9993e364706816aba3e25717850c26c9cd0d89d";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual == expected);
        }

        [Test]
        public void LongMessageHashingTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc";
            const string expected = "c19e35da566dd5503cd6ced86b538f5289b982f2";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual == expected);
        }

        [Test]
        public void VeryLongMessageHashingTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc";

            const string expected = "5af05fa45275660d32755100db217c8097a93c90";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual == expected);
        }

        [Test]
        public void OneCharDifferenceAtRandomPositionHashTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabZ" + // Z at the end of this line
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc";

            const string expected = "5af05fa45275660d32755100db217c8097a93c90";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual != expected);
        }

        [Test]
        public void OneCharDifferenceAtEndHashTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabcA";

            const string expected = "5af05fa45275660d32755100db217c8097a93c90";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual != expected);
        }

        [Test]
        public void OneCharDifferenceAtStartHashTest()
        {
            var shaGenerator = new Sha1();
            const string msg = "Abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc" +
                               "abcabcabcabcabcabcabcabcabcabcabcabcabadfghdhdgccabcabcabcabcabcabc";

            const string expected = "5af05fa45275660d32755100db217c8097a93c90";
            var actual = shaGenerator.ComputeHashString(Encoding.UTF8.GetBytes(msg));
            Assert.IsTrue(actual != expected);
        }
    }
}
