using System;
using System.Collections.Generic;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    /// A helper class that extends Cecil to support LinFu's weaver model.
    /// </summary>
    public static class CecilVisitorExtensions
    {
        /// <summary>
        /// Allows a <see cref="ITypeWeaver"/> instance to traverse any <see cref="object"/>
        /// instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="typeWeaver">The type weaver.</param>
        public static void Accept(this object visitable, ITypeWeaver typeWeaver)
        {
            var visitor = new TypeWeaverVisitor(typeWeaver);
            visitable.Accept(visitor);
        }

        /// <summary>
        /// Allows a <see cref="IMethodWeaver"/> instance to traverse any <see cref="object"/>
        /// instance.
        /// </summary>
        /// <param name="visitable">The visitable object.</param>
        /// <param name="methodWeaver">The method weaver.</param>
        public static void Accept(this object visitable, IMethodWeaver methodWeaver)
        {
            var visitor = new MethodWeaverVisitor(methodWeaver);
            visitable.Accept(visitor);
        }

        public static void Accept(this object visitable, BaseReflectionVisitor visitor)
        {
			if (visitable is AssemblyDefinition) {
				var assemblyDefinition = (AssemblyDefinition) visitable;
				assemblyDefinition.Accept(visitor);
			} else if (visitable is TypeDefinition) {
				var typeDefinition = (TypeDefinition) visitable;
				typeDefinition.Accept(visitor);
			} else {
				throw new NotImplementedException();
			}
        }


		public static void Accept(this AssemblyDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitAssemblyDefinition (visitable);

			visitable.Name.Accept (visitor);
			visitable.Modules.Accept (visitor);

			visitor.TerminateAssemblyDefinition (visitable);
		}

		public static void Accept(this AssemblyLinkedResource visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitAssemblyLinkedResource (visitable);
		}

		public static void Accept(this AssemblyNameDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitAssemblyNameDefinition (visitable);
		}

		public static void Accept(this AssemblyNameReference visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitAssemblyNameReference (visitable);
		}

		public static void Accept(this IEnumerable<AssemblyNameReference> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitAssemblyNameReferenceCollection (visitable);
		}

		public static void Accept(this EmbeddedResource visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitEmbeddedResource (visitable);
		}

		public static void Accept(this LinkedResource visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitLinkedResource (visitable);
		}

		public static void Accept(this ModuleDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitModuleDefinition (visitable);

			visitable.AssemblyReferences.Accept (visitor);
			visitable.ModuleReferences.Accept (visitor);
			visitable.Resources.Accept (visitor);

			///////

			visitable.Types.Accept (visitor);
			visitable.GetTypeReferences().Accept (visitor);
		}

		public static void Accept(this IEnumerable<ModuleDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitModuleDefinitionCollection (visitable);
		}

		public static void Accept(this ModuleReference visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitModuleReference (visitable);
		}

		public static void Accept(this IEnumerable<ModuleReference> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitModuleReferenceCollection (visitable);
		}

		public static void Accept(this IEnumerable<Resource> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitResourceCollection (visitable);
		}

		public static void Accept(this CustomAttribute visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitCustomAttribute (visitable);
		}

        public static void Accept(this IEnumerable<CustomAttribute> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitCustomAttributeCollection (visitable);
		}

        public static void Accept(this EventDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitEventDefinition (visitable);

			visitable.CustomAttributes.Accept (visitor);
		}

        public static void Accept(this IEnumerable<EventDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitEventDefinitionCollection (visitable);
		}

		public static void Accept(this FieldDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitFieldDefinition (visitable);

			//if (visitable.MarshalSpec != null)
			//	visitable.MarshalSpec.Accept (visitor);

			visitable.CustomAttributes.Accept (visitor);
		}

        public static void Accept(this IEnumerable<FieldDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitFieldDefinitionCollection (visitable);
		}

        public static void Accept(this IEnumerable<GenericParameter> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitGenericParameterCollection (visitable);
		}

		public static void Accept(this MemberReference visitable, BaseReflectionVisitor visitor)
		{
		}

        public static void Accept(this IEnumerable<MemberReference> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitMemberReferenceCollection (visitable);
		}

        public static void Accept(this MethodDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitMethodDefinition (visitable);

			visitable.GenericParameters.Accept (visitor);
			visitable.Parameters.Accept (visitor);

			if (visitable.PInvokeInfo != null)
				visitable.PInvokeInfo.Accept (visitor);

			visitable.SecurityDeclarations.Accept (visitor);
			//visitable.Overrides.Accept (visitor);
			visitable.CustomAttributes.Accept (visitor);
		}

        public static void Accept(this IEnumerable<MethodDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitMethodDefinitionCollection (visitable);
		}

        public static void Accept(this ParameterDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitParameterDefinition (visitable);

			//if (visitable.MarshalSpec != null)
			//	visitable.MarshalSpec.Accept (visitor);

			visitable.CustomAttributes.Accept (visitor);
		}

        public static void Accept(this IEnumerable<ParameterDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitParameterDefinitionCollection (visitable);
		}

        public static void Accept(this ParameterReference visitable, BaseReflectionVisitor visitor)
		{
		}

        public static void Accept(this PInvokeInfo visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitPInvokeInfo (visitable);
		}

        public static void Accept(this PropertyDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitPropertyDefinition (visitable);

			visitable.CustomAttributes.Accept (visitor);
		}

        public static void Accept(this IEnumerable<PropertyDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitPropertyDefinitionCollection (visitable);
		}

        public static void Accept(this SecurityDeclaration visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitSecurityDeclaration (visitable);
		}

        public static void Accept(this IEnumerable<SecurityDeclaration> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitSecurityDeclarationCollection (visitable);
		}

        public static void Accept(this TypeDefinition visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitTypeDefinition (visitable);

			visitable.GenericParameters.Accept (visitor);
			visitable.Interfaces.Accept (visitor);
			//visitable.Constructors.Accept (visitor);
			visitable.Methods.Accept (visitor);
			visitable.Fields.Accept (visitor);
			visitable.Properties.Accept (visitor);
			visitable.Events.Accept (visitor);
			visitable.NestedTypes.Accept (visitor);
			visitable.CustomAttributes.Accept (visitor);
			visitable.SecurityDeclarations.Accept (visitor);
		}
        public static void Accept(this IEnumerable<TypeDefinition> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitTypeDefinitionCollection (visitable);
		}

        public static void Accept(this TypeReference visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitTypeReference (visitable);
		}

        public static void Accept(this IEnumerable<TypeReference> visitable, BaseReflectionVisitor visitor)
		{
			visitor.VisitTypeReferenceCollection (visitable);
		}
    }
}