using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using NUnit.Framework;

namespace LinFu.UnitTests.Tools
{
    internal class PEVerifier : IVerifier
    {
        private string _filename = string.Empty;
        private string location = string.Empty;
        private bool _failed;
        internal PEVerifier(string filename)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            location = Path.Combine(baseDirectory, filename);

            _filename = filename;
        }

        ~PEVerifier()
        {
            try
            {
                if (File.Exists(location) && !_failed)
                    File.Delete(location);
            }
            catch
            {
                // Do nothing
            }
        }

        #region IVerifier Members

        public void Verify(Mono.Cecil.AssemblyDefinition assembly)
        {
            // Save the assembly to the temporary file and verify it
            AssemblyFactory.SaveAssembly(assembly, location);
            PEVerify(location);
        }

        #endregion

        private void PEVerify(string assemblyLocation)
        {
            var pathKeys = new[]
                           {
                               "sdkDir",
                               "x86SdkDir",
                               "sdkDirUnderVista"
                           };

            var process = new Process();
            var peVerifyLocation = string.Empty;


            peVerifyLocation = GetPEVerifyLocation(pathKeys, peVerifyLocation);

            // Use PEVerify.exe if it exists
            if (!File.Exists(peVerifyLocation))
            {
                Console.WriteLine("Warning: PEVerify.exe could not be found. Skipping test.");
                return;
            }

            process.StartInfo.FileName = peVerifyLocation;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            process.StartInfo.Arguments = "\"" + assemblyLocation + "\" /VERBOSE";
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            var processOutput = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var result = string.Format("PEVerify Exit Code: {0}", process.ExitCode);

            Console.WriteLine(GetType().FullName + ": " + result);

            if (process.ExitCode == 0)
                return;

            Console.WriteLine(processOutput);
            _failed = true;
            Assert.Fail("PEVerify output: " + Environment.NewLine + processOutput, result);
        }

        private static string GetPEVerifyLocation(IEnumerable<string> pathKeys, string peVerifyLocation)
        {
            foreach (var key in pathKeys)
            {
                var directory = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(directory))
                    continue;

                peVerifyLocation = Path.Combine(directory, "peverify.exe");

                if (File.Exists(peVerifyLocation))
                    break;
            }
            return peVerifyLocation;
        }
    }
}
