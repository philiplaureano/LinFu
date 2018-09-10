using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using LinFu.AOP.Interfaces;

#pragma warning disable 1591

namespace LinFu.AOP.Cecil
{
    public abstract class BaseReflectionVisitor 
    {
        public virtual void TerminateAssemblyDefinition(AssemblyDefinition asm)
        {
        }

        public virtual void VisitAssemblyDefinition(AssemblyDefinition asm)
        {
            VisitCustomAttributeCollection(asm.CustomAttributes);
            VisitModuleDefinitionCollection(asm.Modules);

            TerminateAssemblyDefinition(asm);
        }

        public virtual void VisitAssemblyNameReferenceCollection(IEnumerable<AssemblyNameReference> names)
        {
            foreach (AssemblyNameReference nameRef in names) VisitAssemblyNameReference(nameRef);
        }

        public virtual void VisitAssemblyNameReference(AssemblyNameReference nameRef)   
        {
        }

        public virtual void VisitModuleDefinitionCollection(IEnumerable<ModuleDefinition> modules)
        {
            foreach (ModuleDefinition module in modules) VisitModuleDefinition(module);
        }

        public virtual void VisitModuleReferenceCollection(IEnumerable<ModuleReference> modules)
        {
            foreach (ModuleReference module in modules) VisitModuleReference(module);
        }           

        public virtual void VisitTypeDefinition(TypeDefinition type)
        {
            // Visit the interfaces
            VisitInterfaceCollection(type.Interfaces);

            // Visit the constructors
            VisitConstructorCollection(type.GetConstructors());

            // Visit the fields
            VisitFieldDefinitionCollection(type.Fields);

            // Visit the methods
            VisitMethodDefinitionCollection(type.Methods);

            // Visit the properties
            VisitPropertyDefinitionCollection(type.Properties);

            // Visit the events
            VisitEventDefinitionCollection(type.Events);
        }

        public virtual void VisitConstructorCollection(IEnumerable<MethodDefinition> ctors)
        {
            foreach (MethodDefinition ctor in ctors) VisitConstructor(ctor);
        }

        public virtual void VisitCustomAttributeCollection(IEnumerable<CustomAttribute> customAttrs)
        {
            foreach (CustomAttribute ca in customAttrs) VisitCustomAttribute(ca);
        }

        private void VisitCustomAttribute(CustomAttribute ca)
        {
            throw new System.NotImplementedException();
        }
    
        public virtual void VisitEventDefinitionCollection(IEnumerable<EventDefinition> events)
        {
            foreach (EventDefinition eventDef in events) VisitEventDefinition(eventDef);
        }        

        public virtual void VisitFieldDefinitionCollection(IEnumerable<FieldDefinition> fields)
        {
            foreach (FieldDefinition field in fields) VisitFieldDefinition(field);
        }

        public virtual void VisitGenericParameterCollection(IEnumerable<GenericParameter> genparams)
        {
            foreach (GenericParameter param in genparams) VisitGenericParameter(param);
        }
    
        

        public virtual void VisitInterfaceCollection(IEnumerable<TypeReference> interfaces)
        {
            foreach (TypeReference interfaceType in interfaces) VisitInterface(interfaceType);
        }        

        public virtual void VisitMemberReferenceCollection(IEnumerable<MemberReference> members)
        {
            foreach (MemberReference memberRef in members) VisitMemberReference(memberRef);
        }        

        public virtual void VisitMethodDefinition(MethodDefinition method)
        {
            // Visit the parameters
            VisitParameterDefinitionCollection(method.Parameters);

            // Visit the generic parameters
            VisitGenericParameterCollection(method.GenericParameters);

            // Visit the method overrides
            VisitOverrideCollection(method.Overrides);
        }

        public virtual void VisitMethodDefinitionCollection(IEnumerable<MethodDefinition> methods)
        {
            foreach (MethodDefinition method in methods) 
                VisitMethodDefinition(method);
        }

        public virtual void VisitOverrideCollection(IEnumerable<MethodReference> overrides)
        {
            foreach (MethodReference method in overrides) 
                VisitOverride(method);
        }        

        public virtual void VisitParameterDefinitionCollection(IEnumerable<ParameterDefinition> parameters)
        {
            foreach (ParameterDefinition param in parameters) 
                VisitParameterDefinition(param);
        }

        public virtual void VisitPropertyDefinitionCollection(IEnumerable<PropertyDefinition> properties)
        {
            foreach (PropertyDefinition property in properties) 
                VisitPropertyDefinition(property);
        }

        public virtual void VisitTypeDefinitionCollection(IEnumerable<TypeDefinition> types)
        {
            foreach (TypeDefinition typeDef in types) 
                VisitTypeDefinition(typeDef);
        }

        public virtual void VisitTypeReferenceCollection(IEnumerable<TypeReference> refs)
        {
            foreach (TypeReference type in refs) 
                VisitTypeReference(type);
        }


        public virtual void TerminateModuleDefinition(ModuleDefinition module)
        {
            VisitTypeDefinitionCollection(module.Types);
        }

        public virtual void VisitConstructor(MethodDefinition ctor)
        {
        }

        public virtual void VisitModuleDefinition(ModuleDefinition module)
        {
            VisitCustomAttributeCollection(module.CustomAttributes);
            VisitAssemblyNameReferenceCollection(module.AssemblyReferences);
            VisitModuleReferenceCollection(module.ModuleReferences);
            VisitMemberReferenceCollection(module.GetMemberReferences());
            VisitTypeReferenceCollection(module.GetTypeReferences());
            TerminateModuleDefinition(module);
        }

        public virtual void VisitPropertyDefinition(PropertyDefinition property)
        {
        }

        public virtual void VisitModuleReference(ModuleReference module)
        {
        }
        
        public virtual void VisitEventDefinition(EventDefinition eventDef)
        {
        }
        
        public virtual void VisitFieldDefinition(FieldDefinition field)
        {
        }
        
        public virtual void VisitGenericParameter(GenericParameter genericParameter)
        {
        }

        public virtual void VisitInterface(TypeReference interfaceType)
        {
        }
        
        public virtual void VisitMemberReference(MemberReference memberRef)
        {
        }
        
        public virtual void VisitOverride(MethodReference method)
        {
        }
        
        public virtual void VisitParameterDefinition(ParameterDefinition parameterDefinition)
        {            
        }
        
        public virtual void VisitTypeReference(TypeReference type)
        {
        }
    }
}

#pragma warning restore 1591