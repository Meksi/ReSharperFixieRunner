﻿using System;
using System.IO;
using System.Reflection;
using Fixie;

using ReSharperFixieTestRunner;

namespace FixieRemoteRunner
{
    public class RemoteRunner : MarshalByRefObject, IRemoteRunner
    {
        public ITestResult RunTest(TestSetup setup)
        {
            var listener = new ReSharperListener();
            var runner = new Runner(listener);

            Directory.SetCurrentDirectory(Path.GetDirectoryName(setup.AssemblyLocation));
            var assembly = Assembly.LoadFile(setup.AssemblyLocation);
            var type = assembly.GetType(setup.TypeName);
            var method = type.GetMethod(setup.MethodName);
            runner.RunMethod(assembly, method);
            return listener.TestResult;
        }
    }
}
