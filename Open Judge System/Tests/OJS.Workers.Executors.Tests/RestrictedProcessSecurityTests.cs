﻿namespace OJS.Workers.Executors.Tests
{
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OJS.Workers.Common;

    [TestClass]
    public class RestrictedProcessSecurityTests : BaseExecutorsTestClass
    {
        [TestMethod]
        public void RestrictedProcessShouldNotBeAbleToReadClipboard()
        {
            const string ReadClipboardSourceCode = @"using System;
using System.Windows.Forms;
class Program
{
    public static void Main()
    {
        if (string.IsNullOrEmpty(Clipboard.GetText()))
        {
            throw new Exception(""Clipboard empty!"");
        }
    }
}";
            Clipboard.SetText("clipboard test");
            var exePath = this.CreateExe("RestrictedProcessShouldNotBeAbleToReadClipboard.exe", ReadClipboardSourceCode);

            var process = new RestrictedProcessExecutor();
            var result = process.Execute(exePath, string.Empty, 1500, 32 * 1024 * 1024);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Type == ProcessExecutionResultType.RunTimeError, "No exception is thrown!");
        }

        [TestMethod]
        public void RestrictedProcessShouldNotBeAbleToWriteToClipboard()
        {
            const string WriteToClipboardSourceCode = @"using System;
using System.Windows.Forms;
class Program
{
    public static void Main()
    {
        Clipboard.SetText(""i did it"");
    }
}";
            var exePath = this.CreateExe("RestrictedProcessShouldNotBeAbleToWriteToClipboard.exe", WriteToClipboardSourceCode);

            var process = new RestrictedProcessExecutor();
            var result = process.Execute(exePath, string.Empty, 1500, 32 * 1024 * 1024);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Type == ProcessExecutionResultType.RunTimeError, "No exception is thrown!");
            Assert.AreNotEqual("i did it", Clipboard.GetText());
        }

        [TestMethod]
        public void RestrictedProcessShouldNotBeAbleToStartProcess()
        {
            const string StartNotepadProcessSourceCode = @"using System;
using System.Diagnostics;
class Program
{
    public static void Main()
    {
        Process.Start(string.Format(""{0}\\notepad.exe"", Environment.SystemDirectory));
    }
}";
            var notepadsBefore = Process.GetProcessesByName("notepad.exe").Count();
            var exePath = this.CreateExe("RestrictedProcessShouldNotBeAbleToStartProcess.exe", StartNotepadProcessSourceCode);

            var process = new RestrictedProcessExecutor();
            var result = process.Execute(exePath, string.Empty, 1500, 32 * 1024 * 1024);

            var notepadsAfter = Process.GetProcessesByName("notepad.exe").Count();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Type == ProcessExecutionResultType.RunTimeError, "No exception is thrown!");
            Assert.AreEqual(notepadsBefore, notepadsAfter);
        }
    }
}