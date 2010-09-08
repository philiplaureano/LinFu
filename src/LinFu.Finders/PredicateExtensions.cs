using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Finders
{
    /// <summary>
    /// A class that adds logical extensions to the <see cref="Func{T,TResult}"/> predicate
    /// class.
    /// </summary>
    public static class PredicateExtensions
    {
        /// <summary>
        /// Logically ORs two <see cref="Func{T,TResult}"/> predicates together.
        /// </summary>
        /// <typeparam name="TItem">The type of item being compared.</typeparam>
        /// <param name="left">The left hand predicate.</param>
        /// <param name="right">The right hand predicate.</param>
        /// <returns>A predicate that will return <c>true</c> if and only if one of the given predicates is <c>true</c>; otherwise, it will return <c>false</c>.</returns>
        public static Func<TItem, bool> Or<TItem>(this Func<TItem, bool> left, Func<TItem, bool> right)
        {
            return item => left(item) || right(item);
        }

        /// <summary>
        /// Logically ANDs two <see cref="Func{T,TResult}"/> predicates together.
        /// </summary>
        /// <typeparam name="TItem">The type of item being compared.</typeparam>
        /// <param name="left">The left hand predicate.</param>
        /// <param name="right">The right hand predicate.</param>
        /// <returns>A predicate that will return <c>true</c> if and only if both of the given predicates are <c>true</c>; otherwise, it will return <c>false</c>.</returns>
        public static Func<TItem, bool> And<TItem>(this Func<TItem, bool> left, Func<TItem, bool> right)
        {
            return item => left(item) && right(item);
        }

        /// <summary>
        /// Logically negates a single predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of item being compared.</typeparam>
        /// <param name="predicate">The predicate to negate.</param>
        /// <returns>Returns <c>true</c> if the given predicate is <c>false</c>.</returns>
        public static Func<TItem, bool> Inverse<TItem>(this Func<TItem, bool> predicate)
        {
            return item => !predicate(item);
        }
    }
}
