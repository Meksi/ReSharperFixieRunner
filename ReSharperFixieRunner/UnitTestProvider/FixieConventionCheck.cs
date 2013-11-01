﻿using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [SolutionComponent]
    public class FixieConventionCheck
    {
        public bool IsValidTestClass(IProject project, IClass testClass)
        {
            if (project == null || testClass == null)
                return false;

            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsTestClass(testClass.GetClrName().FullName);
        }

        public bool IsValidTestMethod(IProject project, IClass testClass, IMethod testMethod)
        {
            if (project == null || testClass == null)
                return false;

            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsTestMethod(testClass.GetClrName().FullName, testMethod.ShortName);
        }

        private FixieConventionInfo GetConventionInfo(string assemblyPath)
        {
            return FixieConventionInfo.Create(assemblyPath);
        }

    }
}
