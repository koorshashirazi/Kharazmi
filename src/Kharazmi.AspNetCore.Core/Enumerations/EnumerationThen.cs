using System;

namespace Kharazmi.AspNetCore.Core.Enumerations
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public readonly struct EnumerationThen<TEnum, TValue>
        where TEnum : Enumeration<TEnum, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        private readonly bool _isMatch;
        private readonly Enumeration<TEnum, TValue> _enumeration;
        private readonly bool _stopEvaluating;

        internal EnumerationThen(bool isMatch, bool stopEvaluating, Enumeration<TEnum, TValue> enumeration)
        {
            _isMatch = isMatch;
            _enumeration = enumeration;
            _stopEvaluating = stopEvaluating;
        }

        /// <summary>
        /// Calls <paramref name="doThis"/> Action when the preceding When call matches.
        /// </summary>
        /// <param name="doThis">Action method to call.</param>
        /// <returns>A chainable instance of CaseWhen for more when calls.</returns>
        public EnumerationWhen<TEnum, TValue> Then(Action doThis)
        {
            if (!_stopEvaluating && _isMatch)
                doThis();

            return new EnumerationWhen<TEnum, TValue>(_stopEvaluating || _isMatch, _enumeration);
        }
    }
}