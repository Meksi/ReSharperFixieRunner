﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace ReSharperFixieRunner.UnitTestProvider.Elements
{
    public class FixieTestMethodElement : FixieBaseElement
    {
        private readonly DeclaredElementProvider declaredElementProvider;
        private readonly IClrTypeName typeName;
        private readonly string methodName;
        private readonly string assemblyLocation;
        private readonly string presentation;
        private readonly IUnitTestElement testClass;

        public FixieTestMethodElement(FixieTestProvider provider, IUnitTestElement parent, ProjectModelElementEnvoy projectModelElementEnvoy,
            DeclaredElementProvider declaredElementProvider, string id, IClrTypeName typeName, string methodName, string assemblyLocation)
            : base(provider, parent, id, projectModelElementEnvoy)
        {
            this.testClass = parent;
            this.declaredElementProvider = declaredElementProvider;
            this.typeName = typeName;
            this.methodName = methodName;
            this.assemblyLocation = assemblyLocation;

            ShortName = methodName;
            presentation = string.Format("{0}.{1}", typeName.ShortName, methodName);
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as FixieTestMethodElement);
        }

        private bool Equals(FixieTestMethodElement other)
        {
            if (other == null)
                return false;

            return Equals(Id, other.Id) &&
                   Equals(typeName, other.typeName) &&
                   Equals(methodName, other.methodName) &&
                   Equals(assemblyLocation, other.assemblyLocation);
        }

        public override string GetPresentation(IUnitTestElement parent)
        {
            return presentation;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return Parent != null ? Parent.GetNamespace() : new UnitTestNamespace(typeName.GetNamespaceName());
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();
            if (element == null || !element.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            var locations = from declaration in element.GetDeclarations()
                            let file = declaration.GetContainingFile()
                            where file != null
                            select new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                               declaration.GetNameDocumentRange().TextRange,
                                                               declaration.GetDocumentRange().TextRange);
            return new UnitTestElementDisposition(locations, this);
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            var declaredType = GetDeclaredType();
            if (declaredType == null)
                return null;

            // There is a small opportunity for this to choose the wrong method. If there is more than one
            // method with the same name (e.g. by error, or as an overload), this will arbitrarily choose the
            // first, whatever that means. Realistically, xunit throws an exception if there is more than
            // one method with the same name. We wouldn't know which one to go for anyway, unless we stored
            // the parameter types in this class. And that's overkill to fix such an edge case
            return (from member in declaredType.EnumerateMembers(methodName, declaredType.CaseSensistiveName)
                    where member is IMethod
                    select member).FirstOrDefault();
        }

        private ITypeElement GetDeclaredType()
        {
            return declaredElementProvider.GetDeclaredElement(GetProject(), typeName) as ITypeElement;
        }

        public override IEnumerable<IProjectFile> GetProjectFiles()
        {
            var declaredType = GetDeclaredType();
            if (declaredType != null)
            {
                var result = (from sourceFile in declaredType.GetSourceFiles()
                              select sourceFile.ToProjectFile()).ToList<IProjectFile>();
                if (result.Count == 1)
                    return result;
            }

            var declaredElement = GetDeclaredElement();
            if (declaredElement == null)
                return EmptyArray<IProjectFile>.Instance;

            return from sourceFile in declaredElement.GetSourceFiles()
                   select sourceFile.ToProjectFile();
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            var sequence = testClass.GetTaskSequence(explicitElements, launch);
            // TODO: Add a new task for this element
            return sequence;
        }

        public override string Kind
        {
            get { return "Fixie Test"; }
        }
    }
}
        