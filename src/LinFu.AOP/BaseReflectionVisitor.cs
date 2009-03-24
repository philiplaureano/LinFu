using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
#pragma warning disable 1591

namespace LinFu.AOP.Cecil
{
    public abstract class BaseReflectionVisitor : IReflectionVisitor, IReflectionStructureVisitor
    {
        public virtual void TerminateModuleDefinition(ModuleDefinition module)
        {
        }

        public virtual void VisitConstructor(MethodDefinition ctor)
        {
        }

        public virtual void VisitConstructorCollection(ConstructorCollection ctors)
        {
        }

        public virtual void VisitCustomAttribute(CustomAttribute customAttr)
        {
        }

        public virtual void VisitCustomAttributeCollection(CustomAttributeCollection customAttrs)
        {
        }

        public virtual void VisitEventDefinition(EventDefinition evt)
        {
        }

        public virtual void VisitEventDefinitionCollection(EventDefinitionCollection events)
        {
        }

        public virtual void VisitExternType(TypeReference externType)
        {
        }

        public virtual void VisitExternTypeCollection(ExternTypeCollection externs)
        {
        }

        public virtual void VisitFieldDefinition(FieldDefinition field)
        {
        }

        public virtual void VisitFieldDefinitionCollection(FieldDefinitionCollection fields)
        {
        }

        public virtual void VisitGenericParameter(GenericParameter genparam)
        {
        }

        public virtual void VisitGenericParameterCollection(GenericParameterCollection genparams)
        {
        }

        public virtual void VisitInterface(TypeReference interf)
        {
        }

        public virtual void VisitInterfaceCollection(InterfaceCollection interfaces)
        {
        }

        public virtual void VisitMarshalSpec(MarshalSpec marshalSpec)
        {
        }

        public virtual void VisitMemberReference(MemberReference member)
        {
        }

        public virtual void VisitMemberReferenceCollection(MemberReferenceCollection members)
        {
        }

        public virtual void VisitMethodDefinition(MethodDefinition method)
        {
        }

        public virtual void VisitMethodDefinitionCollection(MethodDefinitionCollection methods)
        {
        }

        public virtual void VisitModuleDefinition(ModuleDefinition module)
        {
        }

        public virtual void VisitNestedType(TypeDefinition nestedType)
        {
        }

        public virtual void VisitNestedTypeCollection(NestedTypeCollection nestedTypes)
        {
        }

        public virtual void VisitOverride(MethodReference ov)
        {
        }

        public virtual void VisitOverrideCollection(OverrideCollection meth)
        {
        }

        public virtual void VisitPInvokeInfo(PInvokeInfo pinvk)
        {
        }

        public virtual void VisitParameterDefinition(ParameterDefinition parameter)
        {
        }

        public virtual void VisitParameterDefinitionCollection(ParameterDefinitionCollection parameters)
        {
        }

        public virtual void VisitPropertyDefinition(PropertyDefinition property)
        {
        }

        public virtual void VisitPropertyDefinitionCollection(PropertyDefinitionCollection properties)
        {
        }

        public virtual void VisitSecurityDeclaration(SecurityDeclaration secDecl)
        {
        }

        public virtual void VisitSecurityDeclarationCollection(SecurityDeclarationCollection secDecls)
        {
        }

        public virtual void VisitTypeDefinition(TypeDefinition type)
        {
        }

        public virtual void VisitTypeDefinitionCollection(TypeDefinitionCollection types)
        {
        }

        public virtual void VisitTypeReference(TypeReference type)
        {
        }

        public virtual void VisitTypeReferenceCollection(TypeReferenceCollection refs)
        {
        }

        public virtual void TerminateAssemblyDefinition(AssemblyDefinition asm)
        {
        }

        public virtual void VisitAssemblyDefinition(AssemblyDefinition asm)
        {
        }

        public virtual void VisitAssemblyLinkedResource(AssemblyLinkedResource res)
        {
        }

        public virtual void VisitAssemblyNameDefinition(AssemblyNameDefinition name)
        {
        }

        public virtual void VisitAssemblyNameReference(AssemblyNameReference name)
        {
        }

        public virtual void VisitAssemblyNameReferenceCollection(AssemblyNameReferenceCollection names)
        {
        }

        public virtual void VisitEmbeddedResource(EmbeddedResource res)
        {
        }

        public virtual void VisitLinkedResource(LinkedResource res)
        {
        }

        public virtual void VisitModuleDefinitionCollection(ModuleDefinitionCollection modules)
        {
        }

        public virtual void VisitModuleReference(ModuleReference module)
        {
        }

        public virtual void VisitModuleReferenceCollection(ModuleReferenceCollection modules)
        {
        }

        public virtual void VisitResourceCollection(ResourceCollection resources)
        {
        }
    }
}
#pragma warning restore 1591
