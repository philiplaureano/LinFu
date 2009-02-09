using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a class that can weave (or modify) 
    /// a member embedded in an assembly.
    /// </summary>
    /// <typeparam name="T">The type of object to modify.</typeparam>
    /// <typeparam name="THost">The host that holds the item to be modified.</typeparam>
    public interface IWeaver<T, THost> : IHostWeaver<THost>
    {        
        /// <summary>
        /// Determines whether or not the current item should be modified.
        /// </summary>
        /// <param name="item">The target item.</param>
        /// <returns>Returns <c>true</c> if the current item can be modified; otherwise, it should return <c>false.</c></returns>
        bool ShouldWeave(T item);

        /// <summary>
        /// Modifies the target <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to be modified.</param>
        void Weave(T item);
    }
}
