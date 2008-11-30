using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A method builder that generates dynamic methods using existing constructors.
    /// </summary>
    public class ConstructorMethodBuilder : BaseMethodBuilder<ConstructorInfo>
    {
        /// <summary>
        /// Returns the declaring type of the target constructor.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns>The declaring type of the target constructor.</returns>
        protected override Type GetReturnType(ConstructorInfo constructor)
        {
            return constructor.DeclaringType;
        }

        /// <summary>
        /// Emits an instruction that instantiates the type associated with the
        /// <paramref name="constructor"/>.
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the target method body.</param>
        /// <param name="constructor">The target constructor.</param>
        protected override void EmitCall(ILGenerator IL, ConstructorInfo constructor)
        {
            IL.Emit(OpCodes.Newobj, constructor);
        }
    }
}
