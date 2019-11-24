using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace fNIRS.Hardware.ISS
{
    public class ISSConnection : IDisposable
    {
        private Process process;

        public ISSConnection()
        {
            process = new Process();
            process.StartInfo.FileName = @"C:\dev\fNIRS\boxy\DMC_ImagentDuae.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, data) => Console.WriteLine(data.Data);
            process.ErrorDataReceived += (sender, data) => Console.WriteLine(data.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void Dispose()
        {
            process.Kill();
        }
    }
}
