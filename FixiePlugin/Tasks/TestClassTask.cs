﻿using System;
using System.Xml;
using FixiePlugin.TestRun;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public class TestClassTask : FixieRemoteTask, IEquatable<TestClassTask>
    {
        private readonly string typeName;

        public TestClassTask(XmlElement element) : base(element)
        {
            typeName = GetXmlAttribute(element, AttributeNames.TypeName);
        }

        public TestClassTask(string assemblyLocation, string typeName)
            : base(TaskRunner.RunnerId, assemblyLocation)
        {
            this.typeName = typeName;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, AttributeNames.TypeName, typeName);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as TestClassTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestClassTask);
        }

        public bool Equals(TestClassTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(AssemblyLocation, other.AssemblyLocation) &&
                   Equals(typeName, other.typeName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Don't include base.GetHashCode, as RemoteTask.GetHashCode includes RemoteTask.Id
                // in the calculation, and this is a new guid generated for each new instance.
                // This would mean two instances that return true from Equals (i.e. value objects)
                // would have different hash codes
                int result = (AssemblyLocation != null ? AssemblyLocation.GetHashCode() : 0);
                result = (result * 397) ^ (typeName != null ? typeName.GetHashCode() : 0);
                return result;
            }
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }


        public override string ToString()
        {
            return string.Format("TestClassTask<{0}>({1})", Id, typeName);
        }
    }
}