using System;
using System.Globalization;

namespace SchoolERPWebService.Utilities
{
    public static class CurrencyFormatter
    {
        /// <summary>
        /// Formats a decimal amount based on the provided format mask from the Company Master.
        /// </summary>
        /// <param name="amount">The numeric value to format.</param>
        /// <param name="formatMask">The mask string (e.g., "#,###.##").</param>
        /// <returns>A formatted string representing the amount.</returns>
        public static string Format(decimal amount, string? formatMask)
        {
            if (string.IsNullOrEmpty(formatMask)) 
                return amount.ToString("0.00", CultureInfo.InvariantCulture);

            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberDecimalDigits = 2;

            switch (formatMask)
            {
                case "####.##":
                    nfi.NumberGroupSeparator = "";
                    nfi.NumberDecimalSeparator = ".";
                    return amount.ToString("N", nfi);

                case "#,###.##":
                    nfi.NumberGroupSeparator = ",";
                    nfi.NumberDecimalSeparator = ".";
                    return amount.ToString("N", nfi);

                case "#,##,###.##":
                    // Indian standard: 1,23,45,678.00
                    return amount.ToString("N", new CultureInfo("en-IN").NumberFormat);

                case "#.###.##":
                    // Custom case: 12.345.678.00 (Dots for both)
                    // Since C# doesn't allow identical separators, we format with commas first then swap
                    nfi.NumberGroupSeparator = ",";
                    nfi.NumberDecimalSeparator = ".";
                    string formatted = amount.ToString("N", nfi);
                    return formatted.Replace(",", ".");

                case "#.###,##":
                    // European standard: 12.345.678,00
                    nfi.NumberGroupSeparator = ".";
                    nfi.NumberDecimalSeparator = ",";
                    return amount.ToString("N", nfi);

                case "# ###.##":
                    // Space separator: 12 345 678.00
                    nfi.NumberGroupSeparator = " ";
                    nfi.NumberDecimalSeparator = ".";
                    return amount.ToString("N", nfi);

                default:
                    return amount.ToString("N2", CultureInfo.InvariantCulture);
            }
        }
    }
}
