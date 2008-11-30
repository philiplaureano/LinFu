// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// LEGAL NOTICE: The original BasePEVerifyTestCase code was taken from Castle
// and modified to suit LinFu's purposes. The following code
// is still subject to the terms of the Apache License and retains
// the rights of its original owners.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using NUnit.Framework;


namespace LinFu.UnitTests.Reflection
{
    public abstract class BasePEVerifyTestCase
    {
        private static readonly List<string> _disposalList = new List<string>();

        [SetUp]
        public void Init()
        {
            OnInit();
        }

        [TearDown]     
        public void Term()
        {
            OnTerm();

            lock (_disposalList)
            {
                // Delete the files tagged for removal
                foreach(var file in _disposalList)
                {
                    if (!File.Exists(file))
                        continue;

                    File.Delete(file);
                }
                _disposalList.Clear();
            }            
        }

        protected virtual void OnInit()
        {
        }
        protected virtual void OnTerm()
        {
            
        }

        protected static void AutoDelete(string filename)
        {
            if (_disposalList.Contains(filename) || !File.Exists(filename))
                return;

            _disposalList.Add(filename);
        }
        protected void PEVerify(string assemblyLocation)
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

            if (!File.Exists(peVerifyLocation))
                throw new FileNotFoundException(
                    "Please check the sdkDir configuration setting and set it to the location of peverify.exe");

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
            Assert.Fail("PEVerify output: " + Environment.NewLine + processOutput, result);
        }

        private static string GetPEVerifyLocation(IEnumerable<string> pathKeys, string peVerifyLocation)
        {
            foreach(var key in pathKeys)
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