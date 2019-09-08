﻿#region License
//
// Copyright 2002-2019 Drew Noakes
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.Text;

namespace MetadataExtractor
{
    /// <summary>
    /// Wraps a byte array with an <see cref="Encoding"/>. Allows consumers to override the encoding if required.
    /// </summary>
    /// <remarks>
    /// String data is often in the incorrect format, and many issues have been raised in the past related to string
    /// encoding. Metadata Extractor used to decode string bytes at read-time, after which it was not possible to
    /// override the encoding at a later time by the user.
    /// <para />
    /// The introduction of this type allows full transparency and control over the use of string data extracted
    /// by the library during the read phase.
    /// </remarks>
    public readonly struct StringValue : IConvertible
    {
        /// <summary>
        /// The encoding used when decoding a <see cref="StringValue"/> that does not specify its encoding.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public StringValue(byte[] bytes, Encoding? encoding = null)
        {
            Bytes = bytes;
            Encoding = encoding;
        }

        public byte[] Bytes { get; }

        public Encoding? Encoding { get; }

        #region IConvertible

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

        string IConvertible.ToString(IFormatProvider provider) => ToString();

        double IConvertible.ToDouble(IFormatProvider provider) => double.Parse(ToString());

        decimal IConvertible.ToDecimal(IFormatProvider provider) => decimal.Parse(ToString());

        float IConvertible.ToSingle(IFormatProvider provider) => float.Parse(ToString());

        bool IConvertible.ToBoolean(IFormatProvider provider) => bool.Parse(ToString());

        byte IConvertible.ToByte(IFormatProvider provider) => byte.Parse(ToString());

        char IConvertible.ToChar(IFormatProvider provider)
        {
            var s = ToString();
            if (s.Length != 1)
                throw new FormatException();
            return s[0];
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => DateTime.Parse(ToString());

        short IConvertible.ToInt16(IFormatProvider provider) => short.Parse(ToString());

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            try
            {
                return int.Parse(ToString());
            }
            catch(Exception)
            {
                long val = 0;
                foreach (var b in Bytes)
                {
                    val <<= 8;
                    val += b & 0xff;
                }
                return (int)val;
            }
        }

        long IConvertible.ToInt64(IFormatProvider provider) => long.Parse(ToString());

        sbyte IConvertible.ToSByte(IFormatProvider provider) => sbyte.Parse(ToString());

        ushort IConvertible.ToUInt16(IFormatProvider provider) => ushort.Parse(ToString());

        uint IConvertible.ToUInt32(IFormatProvider provider) => uint.Parse(ToString());

        ulong IConvertible.ToUInt64(IFormatProvider provider) => ulong.Parse(ToString());

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(ToString(), conversionType, provider);

        #endregion

        #region Formatting

        public override string ToString() => ToString(Encoding ?? DefaultEncoding);

        public string ToString(Encoding encoder) => encoder.GetString(Bytes, 0, Bytes.Length);

        #endregion
    }
}
