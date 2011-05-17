﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace PostWeaver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ShowHelp();
                return;
            }

            string inputFile = args[0];
            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            string targetFile = inputFile;
        	AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(targetFile);
            string targetDirectory = Path.GetDirectoryName(targetFile);

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(targetFile);
            string pdbFileName = string.Format("{0}.pdb", filenameWithoutExtension);
            bool pdbExists = File.Exists(pdbFileName);

            ModuleDefinition module = assembly.MainModule;

			if (pdbExists)
				module.ReadSymbols();

            InterceptMethodCalls(assembly, targetDirectory);
            InterceptNewInstances(assembly, targetDirectory);
            InterceptMethodBodies(assembly, targetDirectory);
            InterceptFields(assembly, targetDirectory);
            InterceptExceptions(assembly, targetDirectory);

            // Update the PDB info if it exists
            if (pdbExists)
                module.Write(targetFile);

            assembly.Save(targetFile);

            Console.WriteLine("PostWeaving Assembly '{0}' -> '{0}'", targetFile);
        }

        private static void InterceptExceptions(AssemblyDefinition assembly, string targetDirectory)
        {
            var methodFilter = LoadFirstInstanceOf<IMethodFilter>(targetDirectory);

            if (methodFilter != null)
            {
                assembly.InterceptExceptions(methodFilter);
                return;
            }

            assembly.InterceptAllExceptions();
        }

        private static void InterceptFields(AssemblyDefinition assembly, string targetDirectory)
        {
            var fieldFilter = LoadFirstInstanceOf<IFieldFilter>(targetDirectory);
            var hostTypeFilter = LoadFirstInstanceOf<ITypeFilter>(targetDirectory);

            if (fieldFilter != null && hostTypeFilter != null)
            {
                assembly.InterceptFields(hostTypeFilter, fieldFilter);
                return;
            }

            assembly.InterceptAllFields();
        }

        private static void InterceptMethodBodies(AssemblyDefinition assembly, string targetDirectory)
        {
            var methodFilter = LoadFirstInstanceOf<IMethodFilter>(targetDirectory);
            if (methodFilter != null)
            {
                assembly.InterceptMethodBody(methodFilter);
                return;
            }

            assembly.InterceptAllMethodBodies();
        }

        private static void InterceptNewInstances(AssemblyDefinition assembly, string targetDirectory)
        {
            var newInstanceFilter = LoadFirstInstanceOf<INewInstanceFilter>(targetDirectory);
            var methodFilter = LoadFirstInstanceOf<IMethodFilter>(targetDirectory);

            if (newInstanceFilter != null && methodFilter != null)
            {
                assembly.InterceptNewInstances(newInstanceFilter, methodFilter);
                return;
            }

            assembly.InterceptAllNewInstances();
        }

        private static void InterceptMethodCalls(AssemblyDefinition assembly, string sourceDirectory)
        {
            var methodCallFilter = LoadFirstInstanceOf<IMethodCallFilter>(sourceDirectory);
            var hostMethodFilter = LoadFirstInstanceOf<IMethodFilter>(sourceDirectory);

            if (methodCallFilter != null && hostMethodFilter != null)
            {
                assembly.InterceptMethodCalls(methodCallFilter, hostMethodFilter);
                return;
            }

            assembly.InterceptAllMethodCalls();
        }

        private static T LoadFirstInstanceOf<T>(string sourceDirectory)
            where T : class
        {
            var items = new List<T>();
            items.LoadFrom(sourceDirectory, "*.filters.dll");

            return items.FirstOrDefault();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("PostWeaver syntax: PostWeaver [filename]");
        }
    }
}