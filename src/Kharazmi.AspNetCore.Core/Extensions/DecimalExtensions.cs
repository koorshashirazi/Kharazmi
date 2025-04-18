﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        /// <summary>
        ///     Rounds and formats a decimal culture invariant
        /// </summary>
        /// <param name="value">The decimal</param>
        /// <param name="decimals">Rounding decimal number</param>
        /// <returns>Formated value</returns>
        public static string FormatInvariant(this decimal value, int decimals = 2)
        {
            return Math.Round(value, decimals).ToString("0.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Calculates the tax (percental) from a gross and a net value.
        /// </summary>
        /// <param name="inclTax">Gross value</param>
        /// <param name="exclTax">Net value</param>
        /// <param name="decimals">Rounding decimal number</param>
        /// <returns>Tax percentage</returns>
        public static decimal ToTaxPercentage(this decimal inclTax, decimal exclTax, int? decimals = null)
        {
            if (exclTax == decimal.Zero)
                return decimal.Zero;

            var result = (inclTax / exclTax - 1.0M) * 100.0M;

            return decimals.HasValue ? Math.Round(result, decimals.Value) : result;
        }


        public static int ToSmallestCurrencyUnit(this decimal value,
            MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            if (!Enum.IsDefined(typeof(MidpointRounding), rounding))
                throw new InvalidEnumArgumentException(nameof(rounding), (int) rounding, typeof(MidpointRounding));
            var result = Math.Round(value * 100, 0, MidpointRounding.AwayFromZero);
            return Convert.ToInt32(result);
        }
    }
}