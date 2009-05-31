using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodImplAttributes = Mono.Cecil.MethodImplAttributes;
using PropertyAttributes = Mono.Cecil.PropertyAttributes;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that extends the <see cref="TypeDefinition"/>
    /// class with features similar to the features in the
    /// System.Reflection.Emit namespace.
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        /// <summary>
        /// Adds a new method to the <paramref name="typeDef">target type</paramref>.
        /// </summary>
        /// <param name="typeDef">The type that will hold the newly-created method.</param>
        /// <param name="attributes">The <see cref="Mono.Cecil.MethodAttributes"/> parameter that describes the characteristics of the method.</param>
        /// <param name="methodName">The name to be given to the new method.</param>
        /// <param name="returnType">The method return type.</param>
        /// <param name="callingConvention">The calling convention of the method being created.</param>
        /// <param name="parameterTypes">The list of argument types that will be used to define the method signature.</param>
        /// <returns>A <see cref="MethodDefinition"/> instance that represents the newly-created method.</returns>
        public static MethodDefinition DefineMethod(this TypeDefinition typeDef, string methodName,
            MethodAttributes attributes, MethodCallingConvention callingConvention, Type returnType, params Type[] parameterTypes)
        {
            var method = new MethodDefinition(methodName, attributes, null)
            {
                CallingConvention = callingConvention
            };
            
            typeDef.Methods.Add(method);

            // Match the parameter types
            method.AddParameters(parameterTypes);

            // Match the return type
            method.SetReturnType(returnType);

            return method;
        }

        /// <summary>
        /// Adds a new method to the <paramref name="typeDef">target type</paramref>.
        /// </summary>
        /// <param name="typeDef">The type that will hold the newly-created method.</param>
        /// <param name="attributes">The <see cref="MethodAttributes"/> parameter that describes the characteristics of the method.</param>
        /// <param name="methodName">The name to be given to the new method.</param>
        /// <param name="returnType">The method return type.</param>        
        /// <param name="parameterTypes">The list of argument types that will be used to define the method signature.</param>
        /// <returns>A <see cref="MethodDefinition"/> instance that represents the newly-created method.</returns>
        public static MethodDefinition DefineMethod(this TypeDefinition typeDef, string methodName,
            MethodAttributes attributes, Type returnType, params Type[] parameterTypes)
        {
            var method = new MethodDefinition(methodName, attributes, null);
            var module = typeDef.Module;


            typeDef.Methods.Add(method);

            // Match the parameter types
            method.AddParameters(parameterTypes);

            // Match the return type
            method.SetReturnType(returnType);

            return method;
        }        

        /// <summary>
        /// Adds a default constructor to the target type.
        /// </summary>
        /// <param name="targetType">The type that will contain the default constructor.</param>
        /// <returns>The default constructor.</returns>
        public static MethodDefinition AddDefaultConstructor(this TypeDefinition targetType)
        {
            var parentType = typeof(object);

            return AddDefaultConstructor(targetType, parentType);
        }

        /// <summary>
        /// Adds a default constructor to the target type.
        /// </summary>
        /// <param name="parentType">The base class that contains the default constructor that will be used for constructor chaining..</param>
        /// <param name="targetType">The type that will contain the default constructor.</param>
        /// <returns>The default constructor.</returns>
        public static MethodDefinition AddDefaultConstructor(this TypeDefinition targetType, Type parentType)
        {
            var module = targetType.Module;
            var voidType = module.Import(typeof(void));
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig
                                   | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;


            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var objectConstructor = parentType.GetConstructor(flags, null, new Type[0], null);

            // Revert to the System.Object constructor
            // if the parent type does not have a default constructor
            if (objectConstructor == null)
                objectConstructor = typeof(object).GetConstructor(new Type[0]);

            var baseConstructor = module.Import(objectConstructor);

            // Define the default constructor
            var ctor = new MethodDefinition(".ctor", methodAttributes, voidType)
                           {
                               CallingConvention = MethodCallingConvention.StdCall,
                               ImplAttributes = (MethodImplAttributes.IL | MethodImplAttributes.Managed)
                           };

            var IL = ctor.Body.CilWorker;

            // Call the constructor for System.Object, and exit
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Call, baseConstructor);
            IL.Emit(OpCodes.Ret);

            targetType.Constructors.Add(ctor);

            return ctor;
        }

        /// <summary>
        /// Adds a rewritable property to the <paramref name="typeDef">target type</paramref>.
        /// </summary>
        /// <param name="typeDef">The target type that will hold the newly-created property.</param>
        /// <param name="propertyName">The name of the property itself.</param>
        /// <param name="propertyType">The <see cref="System.Type"/> instance that describes the property type.</param>
        public static void AddProperty(this TypeDefinition typeDef, string propertyName, Type propertyType)
        {
            var module = typeDef.Module;
            var typeRef = module.Import(propertyType);
            typeDef.AddProperty(propertyName, typeRef);
        }

        /// <summary>
        /// Adds a rewritable property to the <paramref name="typeDef">target type</paramref>.
        /// </summary>
        /// <param name="typeDef">The target type that will hold the newly-created property.</param>
        /// <param name="propertyName">The name of the property itself.</param>
        /// <param name="propertyType">The <see cref="TypeReference"/> instance that describes the property type.</param>
        public static void AddProperty(this TypeDefinition typeDef, string propertyName,
            TypeReference propertyType)
        {
            #region Add the backing field
            string fieldName = string.Format("__{0}_backingField", propertyName);
            FieldDefinition actualField = new FieldDefinition(fieldName,
                propertyType, FieldAttributes.Private);


            typeDef.Fields.Add(actualField);
            #endregion


            FieldReference backingField = actualField;
            if (typeDef.GenericParameters.Count > 0)
                backingField = GetBackingField(fieldName, typeDef, propertyType);

            var getterName = string.Format("get_{0}", propertyName);
            var setterName = string.Format("set_{0}", propertyName);


            const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                MethodAttributes.SpecialName | MethodAttributes.NewSlot |
                                                MethodAttributes.Virtual;

            ModuleDefinition module = typeDef.Module;
            TypeReference voidType = module.Import(typeof(void));

            // Implement the getter and the setter
            MethodDefinition getter = AddPropertyGetter(propertyType, getterName, attributes, backingField);
            MethodDefinition setter = AddPropertySetter(propertyType, attributes, backingField, setterName, voidType);

            typeDef.AddProperty(propertyName, propertyType, getter, setter);
        }

        /// <summary>
        /// Adds a rewriteable property to the <paramref name="typeDef">target type</paramref>
        /// using an existing <paramref name="getter"/> and <paramref name="setter"/>.
        /// </summary>
        /// <param name="typeDef">The target type that will hold the newly-created property.</param>
        /// <param name="propertyName">The name of the property itself.</param>
        /// <param name="propertyType">The <see cref="TypeReference"/> instance that describes the property type.</param>
        /// <param name="getter">The property getter method.</param>
        /// <param name="setter">The property setter method.</param>
        public static void AddProperty(this TypeDefinition typeDef, string propertyName, TypeReference propertyType, MethodDefinition getter, MethodDefinition setter)
        {
            var newProperty = new PropertyDefinition(propertyName,
                propertyType, PropertyAttributes.Unused)
                                  {
                                      GetMethod = getter,
                                      SetMethod = setter
                                  };

            typeDef.Methods.Add(getter);
            typeDef.Methods.Add(setter);
            typeDef.Properties.Add(newProperty);
        }

        /// <summary>
        /// Retrieves the method that matches the given <paramref name="methodName"/>.
        /// </summary>
        /// <param name="typeDef">The target type to search.</param>
        /// <param name="methodName">The name of the target method.</param>
        /// <returns>A method that matches the given <paramref name="methodName"/>. If the method is not found, then it will return a <c>null</c> value. </returns>
        public static MethodDefinition GetMethod(this TypeDefinition typeDef, string methodName)
        {
            var result = from MethodDefinition m in typeDef.Methods
                         where m.Name == methodName
                         select m;

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Resolves the backing field for a generic type declaration.
        /// </summary>
        /// <param name="fieldName">The name of the field to reference.</param>
        /// <param name="typeDef">The type that holds the actual field.</param>
        /// <param name="propertyType">The <see cref="TypeReference"/> that describes the property type being referenced.</param>
        /// <returns>A <see cref="FieldReference"/> that points to the actual backing field.</returns>
        private static FieldReference GetBackingField(string fieldName, TypeDefinition typeDef, TypeReference propertyType)
        {
            // If the current type is a generic type, 
            // the current generic type must be resolved before
            // using the actual field
            var declaringType = new GenericInstanceType(typeDef);
            foreach (GenericParameter parameter in typeDef.GenericParameters)
            {
                declaringType.GenericArguments.Add(parameter);
            }

            return new FieldReference(fieldName, declaringType, propertyType); ;
        }

        /// <summary>
        /// Creates a property getter method implementation with the
        /// <paramref name="propertyType"/> as the return type.
        /// </summary>
        /// <param name="propertyType">Represents the <see cref="TypeReference">return type</see> for the getter method.</param>
        /// <param name="getterName">The getter method name.</param>
        /// <param name="attributes">The method attributes associated with the getter method.</param>
        /// <param name="backingField">The field that will store the instance that the getter method will retrieve.</param>
        /// <returns>A <see cref="MethodDefinition"/> representing the getter method itself.</returns>
        private static MethodDefinition AddPropertyGetter(TypeReference propertyType,
            string getterName, MethodAttributes attributes, FieldReference backingField)
        {
            var getter = new MethodDefinition(getterName, attributes, propertyType)
                             {
                                 IsPublic = true,
                                 ImplAttributes = (MethodImplAttributes.Managed | MethodImplAttributes.IL)
                             };

            var IL = getter.GetILGenerator();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldfld, backingField);
            IL.Emit(OpCodes.Ret);

            return getter;
        }

        /// <summary>
        /// Creates a property setter method implementation with the
        /// <paramref name="propertyType"/> as the setter parameter.
        /// </summary>
        /// <param name="propertyType">Represents the <see cref="TypeReference">parameter type</see> for the setter method.</param>
        /// <param name="attributes">The method attributes associated with the setter method.</param>
        /// <param name="backingField">The field that will store the instance for the setter method.</param>
        /// <param name="setterName">The method name of the setter method.</param>
        /// <param name="voidType">The <see cref="TypeReference"/> that represents <see cref="Void"/>.</param>
        /// <returns>A <see cref="MethodDefinition"/> that represents the setter method itself.</returns>
        private static MethodDefinition AddPropertySetter(TypeReference propertyType, MethodAttributes attributes, FieldReference backingField, string setterName, TypeReference voidType)
        {
            var setter = new MethodDefinition(setterName, attributes, voidType)
                             {
                                 IsPublic = true,
                                 ImplAttributes = (MethodImplAttributes.Managed | MethodImplAttributes.IL)
                             };

            setter.Parameters.Add(new ParameterDefinition(propertyType));

            var IL = setter.GetILGenerator();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Stfld, backingField);
            IL.Emit(OpCodes.Ret);

            return setter;
        }
    }
}
