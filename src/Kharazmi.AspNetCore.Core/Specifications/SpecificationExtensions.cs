﻿using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.Specifications
{
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that both the current and
        /// the given specification must be satisfied by the given object.
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="other">The specification instance with which the current specification is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public static ISpecification<T> And<T>(this ISpecification<T> specification, ISpecification<T> other)
        {
            
            Ensure.ArgumentIsNotNull(specification, nameof(specification));
            Ensure.ArgumentIsNotNull(other, nameof(other));

            return new AndSpecification<T>(specification, other);
        }

        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that either the current or
        /// the given specification should be satisfied by the given object.
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="other">The specification instance with which the current specification
        /// is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public static ISpecification<T> Or<T>(this ISpecification<T> specification, ISpecification<T> other)
        {
            Ensure.ArgumentIsNotNull(specification, nameof(specification));
            Ensure.ArgumentIsNotNull(other, nameof(other));

            return new OrSpecification<T>(specification, other);
        }

        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that the current specification
        /// should be satisfied by the given object but the specified specification should not.
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="other">The specification instance with which the current specification
        /// is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public static ISpecification<T> AndNot<T>(this ISpecification<T> specification, ISpecification<T> other)
        {
            Ensure.ArgumentIsNotNull(specification, nameof(specification));
            Ensure.ArgumentIsNotNull(other, nameof(other));

            return new AndNotSpecification<T>(specification, other);
        }

        /// <summary>
        /// Reverses the current specification instance and returns a specification which represents
        /// the semantics opposite to the current specification.
        /// </summary>
        /// <returns>The reversed specification instance.</returns>
        public static ISpecification<T> Not<T>(this ISpecification<T> specification)
        {
            Ensure.ArgumentIsNotNull(specification, nameof(specification));

            return new NotSpecification<T>(specification);
        }
    }
}