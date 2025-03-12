using System;
using System.Collections.Generic;
using System.Linq;

namespace Kharazmi.AspNetCore.Core.Enumerations
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public readonly struct EnumerationWhen<TEnum, TValue>
        where TEnum : Enumeration<TEnum, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        private readonly Enumeration<TEnum, TValue> _enumeration;
        private readonly bool _stopEvaluating;

        internal EnumerationWhen(bool stopEvaluating, Enumeration<TEnum, TValue> enumeration)
        {
            _stopEvaluating = stopEvaluating;
            _enumeration = enumeration;
        }

        /// <summary>
        /// Execute this action if no other calls to When have matched.
        /// </summary>
        /// <param name="action">The Action to call.</param>
        public void Default(Action action)
        {
            if (!_stopEvaluating)
            {
                action();
            }
        }

        /// <summary>
        /// When this instance is one of the specified <see cref="Enumeration{TEnum,TValue}"/> parameters.
        /// Execute the action in the subsequent call to Then().
        /// </summary>
        /// <param name="enumerationWhen">A collection of <see cref="Enumeration{TEnum,TValue}"/> values to compare to this instance.</param>
        /// <returns>A executor object to execute a supplied action.</returns>
        public EnumerationThen<TEnum, TValue> When(Enumeration<TEnum, TValue> enumerationWhen) =>
            new EnumerationThen<TEnum, TValue>(_enumeration.Equals(enumerationWhen), _stopEvaluating,  _enumeration);

        /// <summary>
        /// When this instance is one of the specified <see cref="Enumeration{TEnum,TValue}"/> parameters.
        /// Execute the action in the subsequent call to Then().
        /// </summary>
        /// <param name="enumConverters">A collection of <see cref="Enumeration{TEnum,TValue}"/> values to compare to this instance.</param>
        /// <returns>A executor object to execute a supplied action.</returns>
        public EnumerationThen<TEnum, TValue> When(params Enumeration<TEnum, TValue>[] enumConverters) =>
            new EnumerationThen<TEnum, TValue>(isMatch: enumConverters.Contains(_enumeration), _stopEvaluating,  _enumeration);

        /// <summary>
        /// When this instance is one of the specified <see cref="Enumeration{TEnum,TValue}"/> parameters.
        /// Execute the action in the subsequent call to Then().
        /// </summary>
        /// <param name="enumConverters">A collection of <see cref="Enumeration{TEnum,TValue}"/> values to compare to this instance.</param>
        /// <returns>A executor object to execute a supplied action.</returns>
        public EnumerationThen<TEnum, TValue> When(IEnumerable<Enumeration<TEnum, TValue>> enumConverters) =>
            new EnumerationThen<TEnum, TValue>(enumConverters.Contains(_enumeration), _stopEvaluating, _enumeration);
    }
}