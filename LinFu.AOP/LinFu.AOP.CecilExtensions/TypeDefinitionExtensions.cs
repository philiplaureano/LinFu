using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.CecilExtensions
{
    public static class TypeDefinitionExtensions
    {
        public static void WeaveWith(this TypeDefinition typeDefinition, IMethodWeaver methodWeaver)
        {
            if (typeDefinition == null)
                throw new ArgumentNullException("typeDefinition");

            if (methodWeaver == null)
                throw new ArgumentNullException("methodWeaver");

            ModuleDefinition module = typeDefinition.Module;
            methodWeaver.ImportReferences(module);
            methodWeaver.AddAdditionalMembers(typeDefinition);

            var matches = (from MethodDefinition m in typeDefinition.Methods
                           where m != null && methodWeaver.ShouldWeave(m)
                           select m).ToList();

            matches.ForEach(match => methodWeaver.Weave(match));
        }
        public static void AddProperty(this TypeDefinition typeDef, string propertyName, Type propertyType)
        {
            ModuleDefinition module = typeDef.Module;
            TypeReference typeRef = module.Import(propertyType);
            typeDef.AddProperty(propertyName, typeRef);
        }
        public static void AddProperty(this TypeDefinition typeDef, string propertyName, TypeReference propertyType)
        {
            string fieldName = string.Format("__{0}_backingField", propertyName);
            FieldDefinition actualField = new FieldDefinition(fieldName,
                propertyType, Mono.Cecil.FieldAttributes.Private);


            typeDef.Fields.Add(actualField);

            FieldReference backingField = actualField;
            if (typeDef.GenericParameters.Count > 0)
            {
                GenericInstanceType declaringType = new GenericInstanceType(typeDef);
                foreach (GenericParameter parameter in typeDef.GenericParameters)
                {
                    declaringType.GenericArguments.Add(parameter);
                }

                backingField = new FieldReference(fieldName, declaringType, propertyType);
            }

            string getterName = string.Format("get_{0}", propertyName);
            string setterName = string.Format("set_{0}", propertyName);


            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                          MethodAttributes.SpecialName | MethodAttributes.NewSlot |
                                          MethodAttributes.Virtual;

            ModuleDefinition module = typeDef.Module;
            TypeReference voidType = module.Import(typeof(void));
            MethodDefinition getter = new MethodDefinition(getterName, attributes, propertyType);

            getter.ImplAttributes = MethodImplAttributes.Managed | MethodImplAttributes.IL;
            CilWorker IL = getter.Body.CilWorker;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldfld, backingField);
            IL.Emit(OpCodes.Ret);

            // Implement the setter
            MethodDefinition setter = new MethodDefinition(setterName, attributes, voidType);
            setter.ImplAttributes = MethodImplAttributes.Managed | MethodImplAttributes.IL;
            setter.Parameters.Add(new ParameterDefinition(propertyType));

            IL = setter.Body.CilWorker;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Stfld, backingField);
            IL.Emit(OpCodes.Ret);

            PropertyDefinition newProperty =
                new PropertyDefinition(propertyName, propertyType, PropertyAttributes.Unused);

            getter.IsPublic = true;
            setter.IsPublic = true;

            newProperty.GetMethod = getter;
            newProperty.SetMethod = setter;

            typeDef.Methods.Add(getter);
            typeDef.Methods.Add(setter);
            typeDef.Properties.Add(newProperty);
        }
    }
}
