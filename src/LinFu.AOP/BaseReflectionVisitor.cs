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

        public virtual void VisitCustomAttribute(CustomAttribute customAttr)
        {
        }

        public virtual void VisitEventDefinition(EventDefinition evt)
        {
        }

        public virtual void VisitExternType(TypeReference externType)
        {
        }

        public virtual void VisitFieldDefinition(FieldDefinition field)
        {
        }

        public virtual void VisitGenericParameter(GenericParameter genparam)
        {
        }

        public virtual void VisitInterface(TypeReference interf)
        {
        }

        public virtual void VisitMarshalSpec(MarshalSpec marshalSpec)
        {
        }

        public virtual void VisitMemberReference(MemberReference member)
        {
        }

        public virtual void VisitModuleDefinition(ModuleDefinition module)
        {
            VisitCustomAttributeCollection(module.CustomAttributes);
            VisitResourceCollection(module.Resources);
            VisitAssemblyNameReferenceCollection(module.AssemblyReferences);
            VisitModuleReferenceCollection(module.ModuleReferences);
            VisitMemberReferenceCollection(module.MemberReferences);
            VisitTypeReferenceCollection(module.TypeReferences);
            VisitTypeDefinitionCollection(module.Types);
            VisitExternTypeCollection(module.ExternTypes);
            TerminateModuleDefinition(module);
        }

        public virtual void VisitNestedTypeCollection(NestedTypeCollection nestedTypes)
        {
            foreach (TypeDefinition nestedType in nestedTypes)
            {
                VisitNestedType(nestedType);
            }
        }

        public virtual void VisitOverride(MethodReference ov)
        {
        }

        public virtual void VisitSecurityDeclaration(SecurityDeclaration secDecl)
        {
        }

        public virtual void VisitPInvokeInfo(PInvokeInfo pinvk)
        {
        }

        public virtual void VisitParameterDefinition(ParameterDefinition parameter)
        {
        }

        public virtual void VisitPropertyDefinition(PropertyDefinition property)
        {
        }

        public virtual void VisitNestedType(TypeDefinition nestedType)
        {
        }

        public virtual void VisitTypeReference(TypeReference type)
        {
        }

        public virtual void TerminateAssemblyDefinition(AssemblyDefinition asm)
        {
        }

        public virtual void VisitAssemblyDefinition(AssemblyDefinition asm)
        {
            VisitCustomAttributeCollection(asm.CustomAttributes);
            VisitModuleDefinitionCollection(asm.Modules);
            VisitSecurityDeclarationCollection(asm.SecurityDeclarations);

            TerminateAssemblyDefinition(asm);
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

        public virtual void VisitEmbeddedResource(EmbeddedResource res)
        {
        }

        public virtual void VisitLinkedResource(LinkedResource res)
        {
        }
        public virtual void VisitModuleReference(ModuleReference module)
        {
        }

        #region Collection Visitors
        public virtual void VisitTypeDefinition(TypeDefinition type)
        {
            // Visit the interfaces
            VisitInterfaceCollection(type.Interfaces);

            // Visit the constructors
            VisitConstructorCollection(type.Constructors);

            // Visit the fields
            VisitFieldDefinitionCollection(type.Fields);

            // Visit the methods
            VisitMethodDefinitionCollection(type.Methods);

            // Visit the properties
            VisitPropertyDefinitionCollection(type.Properties);

            // Visit the events
            VisitEventDefinitionCollection(type.Events);

            // Visit the nested types
            VisitNestedTypeCollection(type.NestedTypes);

            // Visit the security declaration
            VisitSecurityDeclarationCollection(type.SecurityDeclarations);
        }
        public virtual void VisitConstructorCollection(ConstructorCollection ctors)
        {
            foreach (MethodDefinition ctor in ctors)
            {
                VisitConstructor(ctor);
            }
        }        
        public virtual void VisitCustomAttributeCollection(CustomAttributeCollection customAttrs)
        {
            foreach (CustomAttribute ca in customAttrs)
            {
                VisitCustomAttribute(ca);
            }
        }
        public virtual void VisitEventDefinitionCollection(EventDefinitionCollection events)
        {
            foreach (EventDefinition eventDef in events)
            {
                VisitEventDefinition(eventDef);
            }
        }
        public virtual void VisitExternTypeCollection(ExternTypeCollection externs)
        {
            foreach (TypeReference type in externs)
            {
                VisitExternType(type);
            }
        }
        public virtual void VisitFieldDefinitionCollection(FieldDefinitionCollection fields)
        {
            foreach (FieldDefinition field in fields)
            {
                VisitFieldDefinition(field);
            }
        }
        public virtual void VisitGenericParameterCollection(GenericParameterCollection genparams)
        {
            foreach (GenericParameter param in genparams)
            {
                VisitGenericParameter(param);
            }
        }
        public virtual void VisitInterfaceCollection(InterfaceCollection interfaces)
        {
            foreach (TypeReference interfaceType in interfaces)
            {
                VisitInterface(interfaceType);
            }
        }        
        public virtual void VisitMemberReferenceCollection(MemberReferenceCollection members)
        {
            foreach (MemberReference memberRef in members)
            {
                VisitMemberReference(memberRef);
            }
        }
        public virtual void VisitMethodDefinition(MethodDefinition method)
        {
            // Visit the parameters
            VisitParameterDefinitionCollection(method.Parameters);
            
            // Visit the generic parameters
            VisitGenericParameterCollection(method.GenericParameters);

            // Visit the method overrides
            VisitOverrideCollection(method.Overrides);
            
            // Visit the security declarations
            VisitSecurityDeclarationCollection(method.SecurityDeclarations);
        }
        public virtual void VisitMethodDefinitionCollection(MethodDefinitionCollection methods)
        {
            foreach (MethodDefinition method in methods)
            {
                VisitMethodDefinition(method);
            }
        } 
        public virtual void VisitOverrideCollection(OverrideCollection overrides)
        {
            foreach (MethodReference method in overrides)
            {
                VisitOverride(method);
            }
        }
        public virtual void VisitParameterDefinitionCollection(ParameterDefinitionCollection parameters)
        {
            foreach (ParameterDefinition param in parameters)
            {
                VisitParameterDefinition(param);
            }
        }
        public virtual void VisitPropertyDefinitionCollection(PropertyDefinitionCollection properties)
        {
            foreach (PropertyDefinition property in properties)
            {
                VisitPropertyDefinition(property);
            }
        }
        public virtual void VisitSecurityDeclarationCollection(SecurityDeclarationCollection secDecls)
        {
            foreach (SecurityDeclaration securityDeclaration in secDecls)
            {
                VisitSecurityDeclaration(securityDeclaration);
            }
        }

        public virtual void VisitTypeDefinitionCollection(TypeDefinitionCollection types)
        {
            foreach (TypeDefinition typeDef in types)
            {
                VisitTypeDefinition(typeDef);
            }
        }
       
        public virtual void VisitTypeReferenceCollection(TypeReferenceCollection refs)
        {
            foreach (TypeReference type in refs)
            {
                VisitTypeReference(type);
            }
        }
       
        public virtual void VisitAssemblyNameReferenceCollection(AssemblyNameReferenceCollection names)
        {
            foreach (AssemblyNameReference nameRef in names)
            {
                VisitAssemblyNameReference(nameRef);
            }
        }        
        public virtual void VisitModuleDefinitionCollection(ModuleDefinitionCollection modules)
        {
            foreach (ModuleDefinition module in modules)
            {
                VisitModuleDefinition(module);
            }
        }

        public virtual void VisitModuleReferenceCollection(ModuleReferenceCollection modules)
        {
            foreach (ModuleReference module in modules)
            {
                VisitModuleReference(module);
            }
        }

        public virtual void VisitResourceCollection(ResourceCollection resources)
        {
        }
        #endregion
    }
}
#pragma warning restore 1591
