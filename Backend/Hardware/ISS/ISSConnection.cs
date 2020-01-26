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

        public ISSConnection(string exeFile)
        {
            process = new Process();
            if (string.IsNullOrEmpty(exeFile)) return;
            process.StartInfo.FileName = exeFile;
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
            try
            {
                process.Kill();
            }
            catch (Exception) {}
        }
    }
}
