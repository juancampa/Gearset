//-----------------------------------------------------------------------------
// StringBuilderExtensions.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;

namespace Gearset.Extensions
{
    /// <summary>
    /// Options for StringBuilder extension methods.
    /// </summary>
    [Flags]
    public enum AppendNumberOptions
    {
        // Normal format.
        None = 0,

        // Added "+" sign for positive value.
        PositiveSign = 1,

        // Insert Number group separation characters.
        // In Use, added "," for every 3 digits.
        NumberGroup = 2,
    }

    /// <summary>
    /// Static class for string builder extension methods.
    /// </summary>
    /// <remarks>
    /// You can specified StringBuilder for SpriteFont.DrawString from XNA GS 3.0. And you can save unwanted memory allocations.
    /// 
    /// But there are still problems for adding numerical value to StringBuilder. One of them is boxing occurred when you use 
    /// StringBuilder.AppendFormat method. Another issue is memory allocation occurred when you specify int or float for
    /// StringBuild.Append method.
    /// 
    /// This class provides solution for those issue.
    /// 
    /// All methods are defined as extension methods as StringBuilder. So, you can use those method like below.
    /// 
    /// stringBuilder.AppendNumber(12345);
    /// 
    /// </remarks>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Cache for NumberGroupSizes of NumberFormat class.
        /// </summary>
        private static readonly int[] NumberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;

        /// <summary>
        /// string buffer.
        /// </summary>
        private static readonly char[] NumberString = new char[32];

        public static StringBuilder SetText(this StringBuilder builder, int number)
        {
            builder.Length = 0;
            return AppendNumberInternal(builder, number, 0, AppendNumberOptions.None);
        }

        public static StringBuilder SetText(this StringBuilder builder, float number)
        {
            builder.Length = 0;
            return AppendNumber(builder, number, 2, AppendNumberOptions.None);
        }

        public static StringBuilder SetText(this StringBuilder builder, string text)
        {
            builder.Length = 0;
            return builder.Append(text);
        }

        public static StringBuilder SetText(this StringBuilder builder, StringBuilder source)
        {
            builder.Length = 0;
            for (var i = 0; i < source.Length; i++)
                builder.Append(source[i]);

            return builder;
        }

        /// <summary>
        /// Convert integer to string and add to string builder.
        /// </summary>
        public static StringBuilder AppendNumber(this StringBuilder builder, int number)
        {
            return AppendNumberInternal(builder, number, 0, AppendNumberOptions.None);
        }

        /// <summary>
        /// Convert integer to string and add to string builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="number"></param>
        /// <param name="options">Format options</param>
        public static StringBuilder AppendNumber(this StringBuilder builder, int number, AppendNumberOptions options)
        {
            return AppendNumberInternal(builder, number, 0, options);
        }

        /// <summary>
        /// Convert float to string and add to string builder.
        /// </summary>
        /// <remarks>It shows 2 decimal digits.</remarks>
        public static StringBuilder AppendNumber(this StringBuilder builder, float number)
        {
            return AppendNumber(builder, number, 2, AppendNumberOptions.None);
        }

        /// <summary>
        /// Convert float to string and add to string builder.
        /// </summary>
        /// <remarks>It shows 2 decimal digits.</remarks>
        public static StringBuilder AppendNumber(this StringBuilder builder, float number, AppendNumberOptions options)
        {
            return AppendNumber(builder, number, 2, options);
        }

        /// <summary>
        /// Convert float to string and add to string builder.
        /// </summary>
        public static StringBuilder AppendNumber(this StringBuilder builder, float number, int decimalCount, AppendNumberOptions options)
        {
            // Handle NaN, Infinity cases.
            if (float.IsNaN(number))
            {
                builder.Append("NaN");
            }
            else if (float.IsNegativeInfinity(number))
            {
                builder.Append("-Infinity");
            }
            else if (float.IsPositiveInfinity(number))
            {
                builder.Append("+Infinity");
            }
            else
            {
                var intNumber = (int)(number * (float)Math.Pow(10, decimalCount) + 0.5f);

                AppendNumberInternal(builder, intNumber, decimalCount, options);
            }

            return builder;
        }

        static StringBuilder AppendNumberInternal(StringBuilder builder, int number, int decimalCount, AppendNumberOptions options)
        {
            var nfi = CultureInfo.CurrentCulture.NumberFormat;

            var idx = NumberString.Length;
            var decimalPos = idx - decimalCount;

            if (decimalPos == idx)
                decimalPos = idx + 1;

            var numberGroupIdx = 0;
            var numberGroupCount = NumberGroupSizes[numberGroupIdx] + decimalCount;

            var showNumberGroup = (options & AppendNumberOptions.NumberGroup) != 0;
            var showPositiveSign = (options & AppendNumberOptions.PositiveSign) != 0;

            var isNegative = number < 0;

            if (number > int.MinValue)
                number = Math.Abs(number);

            // Converting from smallest digit.
            do
            {
                // Add decimal separator ("." in US).
                if (idx == decimalPos)
                {
                    NumberString[--idx] = nfi.NumberDecimalSeparator[0];
                }

                // Added number group separator ("," in US).
                if (--numberGroupCount < 0 && showNumberGroup)
                {
                    NumberString[--idx] = nfi.NumberGroupSeparator[0];

                    if (numberGroupIdx < NumberGroupSizes.Length - 1)
                        numberGroupIdx++;

                    numberGroupCount = NumberGroupSizes[numberGroupIdx] - 1;
                }

                // Convert current digit to character and add to buffer.
                NumberString[--idx] = (char)('0' + (number % 10));
                number /= 10;

            } while (number > 0 || decimalPos <= idx);


            // Added sign character if needed.
            if (isNegative)
            {
                NumberString[--idx] = nfi.NegativeSign[0];
            }
            else if (showPositiveSign)
            {
                NumberString[--idx] = nfi.PositiveSign[0];
            }

            // Added converted string to StringBuilder.
            builder.Append(NumberString, idx, NumberString.Length - idx);

            return builder;
        }
    }
}