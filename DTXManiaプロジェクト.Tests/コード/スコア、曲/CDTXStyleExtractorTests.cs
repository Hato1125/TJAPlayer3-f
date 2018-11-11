﻿using System;
using System.IO;
using NUnit.Framework;

namespace DTXMania.Tests
{
    [TestFixture]
    public class CDTXStyleExtractorTests
    {
        [Test, Combinatorial]
        public void Test_tセッション譜面がある(
            [Values(
                "205 example",
                "expected case couple only",
                "expected case double only",
                "expected case single and couple",
                "expected case single and double",
                "expected case single only",
                "kitchen sink couple only",
                "kitchen sink double only",
                "kitchen sink single and couple",
                "kitchen sink single and double",
                "kitchen sink single only",
                "mixed case double only",
                "mixed case single and double",
                "mixed case single only",
                "no style",
                "trailing characters double only",
                "trailing characters single and double",
                "trailing characters single only")]
            string scenarioName,
            [Values(0, 1, 2)] int seqNo)
        {
            var assemblyPath = new Uri(GetType().Assembly.EscapedCodeBase).LocalPath;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var testDataDirectory = Path.Combine(Path.Combine(assemblyDirectory, "コード"), "スコア、曲");

            var scenarioFileNamePart = scenarioName.Replace(' ', '_');

            var inputDirectory = Path.Combine(testDataDirectory, "input");
            var inputFileName = $"{scenarioFileNamePart}.tja";
            var inputPath = Path.Combine(inputDirectory, inputFileName);
            var input = File.ReadAllText(inputPath)
                .Replace("\r\n", "\n")
                .Replace('\t', ' ');

            var result = CDTXStyleExtractor.tセッション譜面がある(input, seqNo, inputPath);

            // I would use ApprovalTests.Net for this,
            // but cannot until we upgrade past .net 3.5.
            // Until then, this test will approximate it.

            // JDG inputReference is temporary
            var inputReferenceDirectory = Path.Combine(testDataDirectory, "inputReference");
            Directory.CreateDirectory(inputReferenceDirectory);
            var inputReferenceFileName = $"{scenarioFileNamePart}.{seqNo}.tja";
            var inputReferencePath = Path.Combine(inputReferenceDirectory, inputReferenceFileName);
            File.Delete(inputReferencePath);
            File.Copy(inputPath, inputReferencePath);
            
            var receivedDirectory = Path.Combine(testDataDirectory, "received");
            Directory.CreateDirectory(receivedDirectory);
            var receivedFileName = $"{scenarioFileNamePart}.{seqNo}.tja";
            var receivedPath = Path.Combine(receivedDirectory, receivedFileName);
            File.Delete(receivedPath);
            File.WriteAllText(receivedPath, result);

            var approvedDirectory = Path.Combine(testDataDirectory, "approved");
            Directory.CreateDirectory(approvedDirectory);
            var approvedFileName = $"{scenarioFileNamePart}.{seqNo}.tja";
            var approvedPath = Path.Combine(approvedDirectory, approvedFileName);

            var approved = File.ReadAllText(approvedPath).Replace("\r\n", "\n");
            var received = File.ReadAllText(receivedPath);

            Assert.AreEqual(approved, received);
        }
    }
}