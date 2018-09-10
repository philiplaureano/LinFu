using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Xunit;

namespace LinFu.UnitTests.Tools
{
    internal class PeVerifier : IVerifier
    {
        private readonly string _location = string.Empty;
        private bool _failed;
        private string _filename = string.Empty;

        internal PeVerifier(string filename)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _location = Path.Combine(baseDirectory, filename);

            _filename = filename;
        }


        public void Verify(AssemblyDefinition assembly)
        {
            // Save the assembly to the temporary file and verify it            
            assembly.Save(_location);
            PeVerify(_location);
        }


        ~PeVerifier()
        {
            try
            {
                if (File.Exists(_location) && !_failed)
                    File.Delete(_location);
            }
            catch
            {
                // Do nothing
            }
        }

        private void PeVerify(string assemblyLocation)
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
            Assert.True(false, $"PEVerify output: {Environment.NewLine}{processOutput}{result}");
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