using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// This class is the same as the BooleanToVisibilityConverter
    /// but negated.
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class TypeToSpinnerModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            TypeCode code = Type.GetTypeCode((Type)value);
            switch (code)
            {
                case TypeCode.Byte: return NumericSpinnerMode.Byte;
                case TypeCode.Char: return NumericSpinnerMode.Char;
                case TypeCode.Decimal: return NumericSpinnerMode.Decimal;
                case TypeCode.Int16: return NumericSpinnerMode.Short;
                case TypeCode.Int32: return NumericSpinnerMode.Int;
                case TypeCode.Int64: return NumericSpinnerMode.Long;
                case TypeCode.SByte: return NumericSpinnerMode.SByte;
                case TypeCode.UInt16: return NumericSpinnerMode.UShort;
                case TypeCode.UInt32: return NumericSpinnerMode.UInt;
                case TypeCode.UInt64: return NumericSpinnerMode.ULong;
                case TypeCode.Single: return NumericSpinnerMode.Float;
                case TypeCode.Double: return NumericSpinnerMode.Double;
            }
            throw new InvalidCastException("Error while trying to convert a Type Object to a SpinnerMode");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NumericSpinnerMode mode = (NumericSpinnerMode)value;
            switch (mode)
            {
                case NumericSpinnerMode.Byte: return TypeCode.Byte;
                case NumericSpinnerMode.Char: return TypeCode.Char;
                case NumericSpinnerMode.Decimal: return TypeCode.Decimal;
                case NumericSpinnerMode.Int: return TypeCode.Int32;
                case NumericSpinnerMode.Long: return TypeCode.Int64;
                case NumericSpinnerMode.SByte: return TypeCode.SByte;
                case NumericSpinnerMode.Short: return TypeCode.Int16;
                case NumericSpinnerMode.UInt: return TypeCode.UInt32;
                case NumericSpinnerMode.ULong: return TypeCode.UInt64;
                case NumericSpinnerMode.UShort: return TypeCode.UInt16;
                case NumericSpinnerMode.Float: return TypeCode.Single;
                case NumericSpinnerMode.Double: return TypeCode.Double;
            }
            throw new InvalidCastException("Error while trying to convert a SpinnerMode to a Type Object");
        }
    }
}
