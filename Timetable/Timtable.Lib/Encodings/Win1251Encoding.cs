﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Timetable.Encodings
{
    /// <summary>
    /// A custom encoding class that provides encoding capabilities for the
    /// 'Cyrillic (Windows)' encoding under Silverlight.<br/>
    /// This class was generated by a tool. For more information, visit
    /// http://www.hardcodet.net/2010/03/silverlight-text-encoding-class-generator
    /// </summary>
    public class Win1251Encoding : Encoding
    {
        /// <summary>
        /// Gets the name registered with the
        /// Internet Assigned Numbers Authority (IANA) for the current encoding.
        /// </summary>
        /// <returns>
        /// The IANA name for the current <see cref="System.Text.Encoding"/>.
        /// </returns>
        public override string WebName
        {
            get
            {
                return "windows-1251";
            }
        }


        private char? fallbackCharacter;

        /// <summary>
        /// A character that can be set in order to make the encoding class
        /// more fault tolerant. If this property is set, the encoding class will
        /// use this property instead of throwing an exception if an unsupported
        /// byte value is being passed for decoding.
        /// </summary>
        public char? FallbackCharacter
        {
            get { return fallbackCharacter; }
            set
            {
                fallbackCharacter = value;

                if (value.HasValue && !charToByte.ContainsKey(value.Value))
                {
                    string msg = "Cannot use the character [{0}] (int value {1}) as fallback value "
                    + "- the fallback character itself is not supported by the encoding.";
                    msg = String.Format(msg, value.Value, (int)value.Value);
                    throw new EncoderFallbackException(msg);
                }

                FallbackByte = value.HasValue ? charToByte[value.Value] : (byte?)null;
            }
        }

        /// <summary>
        /// A byte value that corresponds to the <see cref="FallbackCharacter"/>.
        /// It is used in encoding scenarios in case an unsupported character is
        /// being passed for encoding.
        /// </summary>
        public byte? FallbackByte { get; private set; }


        public Win1251Encoding()
        {
            FallbackCharacter = '?';
        }

        /// <summary>
        /// Encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <returns>
        /// The actual number of bytes written into <paramref name="bytes"/>.
        /// </returns>
        /// <param name="chars">The character array containing the set of characters to encode. 
        /// </param><param name="charIndex">The index of the first character to encode. 
        /// </param><param name="charCount">The number of characters to encode. 
        /// </param><param name="bytes">The byte array to contain the resulting sequence of bytes.
        /// </param><param name="byteIndex">The index at which to start writing the resulting sequence of bytes. 
        /// </param>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return FallbackByte.HasValue
                     ? GetBytesWithFallBack(chars, charIndex, charCount, bytes, byteIndex)
                     : GetBytesWithoutFallback(chars, charIndex, charCount, bytes, byteIndex);
        }


        private int GetBytesWithFallBack(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                var character = chars[i + charIndex];
                byte byteValue;
                bool status = charToByte.TryGetValue(character, out byteValue);

                bytes[byteIndex + i] = status ? byteValue : FallbackByte.Value;
            }

            return charCount;
        }

        private int GetBytesWithoutFallback(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                var character = chars[i + charIndex];
                byte byteValue;
                bool status = charToByte.TryGetValue(character, out byteValue);

                if (!status)
                {
                    //throw exception
                    string msg =
                      "The encoding [{0}] cannot encode the character [{1}] (int value {2}). Set the FallbackCharacter property in order to suppress this exception and encode a default character instead.";
                    msg = String.Format(msg, WebName, character, (int)character);
                    throw new EncoderFallbackException(msg);
                }

                bytes[byteIndex + i] = byteValue;
            }

            return charCount;
        }



        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <returns>
        /// The actual number of characters written into <paramref name="chars"/>.
        /// </returns>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode. 
        /// </param><param name="byteIndex">The index of the first byte to decode. 
        /// </param><param name="byteCount">The number of bytes to decode. 
        /// </param><param name="chars">The character array to contain the resulting set of characters. 
        /// </param><param name="charIndex">The index at which to start writing the resulting set of characters. 
        /// </param>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return FallbackCharacter.HasValue
                     ? GetCharsWithFallback(bytes, byteIndex, byteCount, chars, charIndex)
                     : GetCharsWithoutFallback(bytes, byteIndex, byteCount, chars, charIndex);
        }


        private int GetCharsWithFallback(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                byte lookupIndex = bytes[i + byteIndex];

                //if the byte value is not in our lookup array, fall back to default character
                char result = lookupIndex >= byteToChar.Length
                                ? FallbackCharacter.Value
                                : byteToChar[lookupIndex];

                chars[charIndex + i] = result;
            }

            return byteCount;
        }



        private int GetCharsWithoutFallback(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                byte lookupIndex = bytes[i + byteIndex];
                if (lookupIndex >= byteToChar.Length)
                {
                    //throw exception
                    string msg = "The encoding [{0}] cannot decode byte value [{1}]. Set the FallbackCharacter property in order to suppress this exception and decode the value as a default character instead.";
                    msg = String.Format(msg, WebName, lookupIndex);
                    throw new EncoderFallbackException(msg);
                }


                chars[charIndex + i] = byteToChar[lookupIndex];
            }

            return byteCount;
        }



        /// <summary>
        /// Calculates the number of bytes produced by encoding a set of characters
        /// from the specified character array.
        /// </summary>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters. This class
        /// alwas returns the value of <paramref name="count"/>.
        /// </returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }


        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence
        /// of bytes from the specified byte array.
        /// </summary>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes. This class
        /// alwas returns the value of <paramref name="count"/>. 
        /// </returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }


        /// <summary>
        /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <returns>
        /// The maximum number of bytes produced by encoding the specified number of characters. This
        /// class alwas returns the value of <paramref name="charCount"/>.
        /// </returns>
        /// <param name="charCount">The number of characters to encode. 
        /// </param>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <returns>
        /// The maximum number of characters produced by decoding the specified number of bytes. This class
        /// alwas returns the value of <paramref name="byteCount"/>.
        /// </returns>
        /// <param name="byteCount">The number of bytes to decode.</param> 
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }


        /// <summary>
        /// Gets the number of characters that are supported by this encoding.
        /// This property returns a maximum value of 256, as the encoding class
        /// only supports single byte encodings (1 byte == 256 possible values).
        /// </summary>
        public static int CharacterCount
        {
            get { return byteToChar.Length; }
        }


        #region Character Table

        /// <summary>
        /// This table contains characters in an array. The index within the
        /// array corresponds to the encoding's mapping of bytes to characters
        /// (e.g. if a byte value of 5 is used to encode the character 'x', this
        /// character will be stored at the array index 5.
        /// </summary>
        private static char[] byteToChar = new char[]
        {
      (char)0 /* byte 0 */  ,
      (char)1 /* byte 1 */  ,
      (char)2 /* byte 2 */  ,
      (char)3 /* byte 3 */  ,
      (char)4 /* byte 4 */  ,
      (char)5 /* byte 5 */  ,
      (char)6 /* byte 6 */  ,
      (char)7 /* byte 7 */  ,
      (char)8 /* byte 8 */  ,
      (char)9 /* byte 9 */  ,
      (char)10 /* byte 10 */  ,
      (char)11 /* byte 11 */  ,
      (char)12 /* byte 12 */  ,
      (char)13 /* byte 13 */  ,
      (char)14 /* byte 14 */  ,
      (char)15 /* byte 15 */  ,
      (char)16 /* byte 16 */  ,
      (char)17 /* byte 17 */  ,
      (char)18 /* byte 18 */  ,
      (char)19 /* byte 19 */  ,
      (char)20 /* byte 20 */  ,
      (char)21 /* byte 21 */  ,
      (char)22 /* byte 22 */  ,
      (char)23 /* byte 23 */  ,
      (char)24 /* byte 24 */  ,
      (char)25 /* byte 25 */  ,
      (char)26 /* byte 26 */  ,
      (char)27 /* byte 27 */  ,
      (char)28 /* byte 28 */  ,
      (char)29 /* byte 29 */  ,
      (char)30 /* byte 30 */  ,
      (char)31 /* byte 31 */  ,
      (char)32 /* byte 32 */  ,
      (char)33 /* byte 33 */  ,
      (char)34 /* byte 34 */  ,
      (char)35 /* byte 35 */  ,
      (char)36 /* byte 36 */  ,
      (char)37 /* byte 37 */  ,
      (char)38 /* byte 38 */  ,
      (char)39 /* byte 39 */  ,
      (char)40 /* byte 40 */  ,
      (char)41 /* byte 41 */  ,
      (char)42 /* byte 42 */  ,
      (char)43 /* byte 43 */  ,
      (char)44 /* byte 44 */  ,
      (char)45 /* byte 45 */  ,
      (char)46 /* byte 46 */  ,
      (char)47 /* byte 47 */  ,
      (char)48 /* byte 48 */  ,
      (char)49 /* byte 49 */  ,
      (char)50 /* byte 50 */  ,
      (char)51 /* byte 51 */  ,
      (char)52 /* byte 52 */  ,
      (char)53 /* byte 53 */  ,
      (char)54 /* byte 54 */  ,
      (char)55 /* byte 55 */  ,
      (char)56 /* byte 56 */  ,
      (char)57 /* byte 57 */  ,
      (char)58 /* byte 58 */  ,
      (char)59 /* byte 59 */  ,
      (char)60 /* byte 60 */  ,
      (char)61 /* byte 61 */  ,
      (char)62 /* byte 62 */  ,
      (char)63 /* byte 63 */  ,
      (char)64 /* byte 64 */  ,
      (char)65 /* byte 65 */  ,
      (char)66 /* byte 66 */  ,
      (char)67 /* byte 67 */  ,
      (char)68 /* byte 68 */  ,
      (char)69 /* byte 69 */  ,
      (char)70 /* byte 70 */  ,
      (char)71 /* byte 71 */  ,
      (char)72 /* byte 72 */  ,
      (char)73 /* byte 73 */  ,
      (char)74 /* byte 74 */  ,
      (char)75 /* byte 75 */  ,
      (char)76 /* byte 76 */  ,
      (char)77 /* byte 77 */  ,
      (char)78 /* byte 78 */  ,
      (char)79 /* byte 79 */  ,
      (char)80 /* byte 80 */  ,
      (char)81 /* byte 81 */  ,
      (char)82 /* byte 82 */  ,
      (char)83 /* byte 83 */  ,
      (char)84 /* byte 84 */  ,
      (char)85 /* byte 85 */  ,
      (char)86 /* byte 86 */  ,
      (char)87 /* byte 87 */  ,
      (char)88 /* byte 88 */  ,
      (char)89 /* byte 89 */  ,
      (char)90 /* byte 90 */  ,
      (char)91 /* byte 91 */  ,
      (char)92 /* byte 92 */  ,
      (char)93 /* byte 93 */  ,
      (char)94 /* byte 94 */  ,
      (char)95 /* byte 95 */  ,
      (char)96 /* byte 96 */  ,
      (char)97 /* byte 97 */  ,
      (char)98 /* byte 98 */  ,
      (char)99 /* byte 99 */  ,
      (char)100 /* byte 100 */  ,
      (char)101 /* byte 101 */  ,
      (char)102 /* byte 102 */  ,
      (char)103 /* byte 103 */  ,
      (char)104 /* byte 104 */  ,
      (char)105 /* byte 105 */  ,
      (char)106 /* byte 106 */  ,
      (char)107 /* byte 107 */  ,
      (char)108 /* byte 108 */  ,
      (char)109 /* byte 109 */  ,
      (char)110 /* byte 110 */  ,
      (char)111 /* byte 111 */  ,
      (char)112 /* byte 112 */  ,
      (char)113 /* byte 113 */  ,
      (char)114 /* byte 114 */  ,
      (char)115 /* byte 115 */  ,
      (char)116 /* byte 116 */  ,
      (char)117 /* byte 117 */  ,
      (char)118 /* byte 118 */  ,
      (char)119 /* byte 119 */  ,
      (char)120 /* byte 120 */  ,
      (char)121 /* byte 121 */  ,
      (char)122 /* byte 122 */  ,
      (char)123 /* byte 123 */  ,
      (char)124 /* byte 124 */  ,
      (char)125 /* byte 125 */  ,
      (char)126 /* byte 126 */  ,
      (char)127 /* byte 127 */  ,
      (char)1026 /* byte 128 */  ,
      (char)1027 /* byte 129 */  ,
      (char)8218 /* byte 130 */  ,
      (char)1107 /* byte 131 */  ,
      (char)8222 /* byte 132 */  ,
      (char)8230 /* byte 133 */  ,
      (char)8224 /* byte 134 */  ,
      (char)8225 /* byte 135 */  ,
      (char)8364 /* byte 136 */  ,
      (char)8240 /* byte 137 */  ,
      (char)1033 /* byte 138 */  ,
      (char)8249 /* byte 139 */  ,
      (char)1034 /* byte 140 */  ,
      (char)1036 /* byte 141 */  ,
      (char)1035 /* byte 142 */  ,
      (char)1039 /* byte 143 */  ,
      (char)1106 /* byte 144 */  ,
      (char)8216 /* byte 145 */  ,
      (char)8217 /* byte 146 */  ,
      (char)8220 /* byte 147 */  ,
      (char)8221 /* byte 148 */  ,
      (char)8226 /* byte 149 */  ,
      (char)8211 /* byte 150 */  ,
      (char)8212 /* byte 151 */  ,
      (char)152 /* byte 152 */  ,
      (char)8482 /* byte 153 */  ,
      (char)1113 /* byte 154 */  ,
      (char)8250 /* byte 155 */  ,
      (char)1114 /* byte 156 */  ,
      (char)1116 /* byte 157 */  ,
      (char)1115 /* byte 158 */  ,
      (char)1119 /* byte 159 */  ,
      (char)160 /* byte 160 */  ,
      (char)1038 /* byte 161 */  ,
      (char)1118 /* byte 162 */  ,
      (char)1032 /* byte 163 */  ,
      (char)164 /* byte 164 */  ,
      (char)1168 /* byte 165 */  ,
      (char)166 /* byte 166 */  ,
      (char)167 /* byte 167 */  ,
      (char)1025 /* byte 168 */  ,
      (char)169 /* byte 169 */  ,
      (char)1028 /* byte 170 */  ,
      (char)171 /* byte 171 */  ,
      (char)172 /* byte 172 */  ,
      (char)173 /* byte 173 */  ,
      (char)174 /* byte 174 */  ,
      (char)1031 /* byte 175 */  ,
      (char)176 /* byte 176 */  ,
      (char)177 /* byte 177 */  ,
      (char)1030 /* byte 178 */  ,
      (char)1110 /* byte 179 */  ,
      (char)1169 /* byte 180 */  ,
      (char)181 /* byte 181 */  ,
      (char)182 /* byte 182 */  ,
      (char)183 /* byte 183 */  ,
      (char)1105 /* byte 184 */  ,
      (char)8470 /* byte 185 */  ,
      (char)1108 /* byte 186 */  ,
      (char)187 /* byte 187 */  ,
      (char)1112 /* byte 188 */  ,
      (char)1029 /* byte 189 */  ,
      (char)1109 /* byte 190 */  ,
      (char)1111 /* byte 191 */  ,
      (char)1040 /* byte 192 */  ,
      (char)1041 /* byte 193 */  ,
      (char)1042 /* byte 194 */  ,
      (char)1043 /* byte 195 */  ,
      (char)1044 /* byte 196 */  ,
      (char)1045 /* byte 197 */  ,
      (char)1046 /* byte 198 */  ,
      (char)1047 /* byte 199 */  ,
      (char)1048 /* byte 200 */  ,
      (char)1049 /* byte 201 */  ,
      (char)1050 /* byte 202 */  ,
      (char)1051 /* byte 203 */  ,
      (char)1052 /* byte 204 */  ,
      (char)1053 /* byte 205 */  ,
      (char)1054 /* byte 206 */  ,
      (char)1055 /* byte 207 */  ,
      (char)1056 /* byte 208 */  ,
      (char)1057 /* byte 209 */  ,
      (char)1058 /* byte 210 */  ,
      (char)1059 /* byte 211 */  ,
      (char)1060 /* byte 212 */  ,
      (char)1061 /* byte 213 */  ,
      (char)1062 /* byte 214 */  ,
      (char)1063 /* byte 215 */  ,
      (char)1064 /* byte 216 */  ,
      (char)1065 /* byte 217 */  ,
      (char)1066 /* byte 218 */  ,
      (char)1067 /* byte 219 */  ,
      (char)1068 /* byte 220 */  ,
      (char)1069 /* byte 221 */  ,
      (char)1070 /* byte 222 */  ,
      (char)1071 /* byte 223 */  ,
      (char)1072 /* byte 224 */  ,
      (char)1073 /* byte 225 */  ,
      (char)1074 /* byte 226 */  ,
      (char)1075 /* byte 227 */  ,
      (char)1076 /* byte 228 */  ,
      (char)1077 /* byte 229 */  ,
      (char)1078 /* byte 230 */  ,
      (char)1079 /* byte 231 */  ,
      (char)1080 /* byte 232 */  ,
      (char)1081 /* byte 233 */  ,
      (char)1082 /* byte 234 */  ,
      (char)1083 /* byte 235 */  ,
      (char)1084 /* byte 236 */  ,
      (char)1085 /* byte 237 */  ,
      (char)1086 /* byte 238 */  ,
      (char)1087 /* byte 239 */  ,
      (char)1088 /* byte 240 */  ,
      (char)1089 /* byte 241 */  ,
      (char)1090 /* byte 242 */  ,
      (char)1091 /* byte 243 */  ,
      (char)1092 /* byte 244 */  ,
      (char)1093 /* byte 245 */  ,
      (char)1094 /* byte 246 */  ,
      (char)1095 /* byte 247 */  ,
      (char)1096 /* byte 248 */  ,
      (char)1097 /* byte 249 */  ,
      (char)1098 /* byte 250 */  ,
      (char)1099 /* byte 251 */  ,
      (char)1100 /* byte 252 */  ,
      (char)1101 /* byte 253 */  ,
      (char)1102 /* byte 254 */  ,
      (char)1103 /* byte 255 */
        };

        #endregion


        #region Byte Lookup Dictionary

        /// <summary>
        /// This dictionary is used to resolve byte values for a given character.
        /// </summary>
        private static Dictionary<char, byte> charToByte = new Dictionary<char, byte>
    {
      { (char)0, 0 },
      { (char)1, 1 },
      { (char)2, 2 },
      { (char)3, 3 },
      { (char)4, 4 },
      { (char)5, 5 },
      { (char)6, 6 },
      { (char)7, 7 },
      { (char)8, 8 },
      { (char)9, 9 },
      { (char)10, 10 },
      { (char)11, 11 },
      { (char)12, 12 },
      { (char)13, 13 },
      { (char)14, 14 },
      { (char)15, 15 },
      { (char)16, 16 },
      { (char)17, 17 },
      { (char)18, 18 },
      { (char)19, 19 },
      { (char)20, 20 },
      { (char)21, 21 },
      { (char)22, 22 },
      { (char)23, 23 },
      { (char)24, 24 },
      { (char)25, 25 },
      { (char)26, 26 },
      { (char)27, 27 },
      { (char)28, 28 },
      { (char)29, 29 },
      { (char)30, 30 },
      { (char)31, 31 },
      { (char)32, 32 },
      { (char)33, 33 },
      { (char)34, 34 },
      { (char)35, 35 },
      { (char)36, 36 },
      { (char)37, 37 },
      { (char)38, 38 },
      { (char)39, 39 },
      { (char)40, 40 },
      { (char)41, 41 },
      { (char)42, 42 },
      { (char)43, 43 },
      { (char)44, 44 },
      { (char)45, 45 },
      { (char)46, 46 },
      { (char)47, 47 },
      { (char)48, 48 },
      { (char)49, 49 },
      { (char)50, 50 },
      { (char)51, 51 },
      { (char)52, 52 },
      { (char)53, 53 },
      { (char)54, 54 },
      { (char)55, 55 },
      { (char)56, 56 },
      { (char)57, 57 },
      { (char)58, 58 },
      { (char)59, 59 },
      { (char)60, 60 },
      { (char)61, 61 },
      { (char)62, 62 },
      { (char)63, 63 },
      { (char)64, 64 },
      { (char)65, 65 },
      { (char)66, 66 },
      { (char)67, 67 },
      { (char)68, 68 },
      { (char)69, 69 },
      { (char)70, 70 },
      { (char)71, 71 },
      { (char)72, 72 },
      { (char)73, 73 },
      { (char)74, 74 },
      { (char)75, 75 },
      { (char)76, 76 },
      { (char)77, 77 },
      { (char)78, 78 },
      { (char)79, 79 },
      { (char)80, 80 },
      { (char)81, 81 },
      { (char)82, 82 },
      { (char)83, 83 },
      { (char)84, 84 },
      { (char)85, 85 },
      { (char)86, 86 },
      { (char)87, 87 },
      { (char)88, 88 },
      { (char)89, 89 },
      { (char)90, 90 },
      { (char)91, 91 },
      { (char)92, 92 },
      { (char)93, 93 },
      { (char)94, 94 },
      { (char)95, 95 },
      { (char)96, 96 },
      { (char)97, 97 },
      { (char)98, 98 },
      { (char)99, 99 },
      { (char)100, 100 },
      { (char)101, 101 },
      { (char)102, 102 },
      { (char)103, 103 },
      { (char)104, 104 },
      { (char)105, 105 },
      { (char)106, 106 },
      { (char)107, 107 },
      { (char)108, 108 },
      { (char)109, 109 },
      { (char)110, 110 },
      { (char)111, 111 },
      { (char)112, 112 },
      { (char)113, 113 },
      { (char)114, 114 },
      { (char)115, 115 },
      { (char)116, 116 },
      { (char)117, 117 },
      { (char)118, 118 },
      { (char)119, 119 },
      { (char)120, 120 },
      { (char)121, 121 },
      { (char)122, 122 },
      { (char)123, 123 },
      { (char)124, 124 },
      { (char)125, 125 },
      { (char)126, 126 },
      { (char)127, 127 },
      { (char)1026, 128 },
      { (char)1027, 129 },
      { (char)8218, 130 },
      { (char)1107, 131 },
      { (char)8222, 132 },
      { (char)8230, 133 },
      { (char)8224, 134 },
      { (char)8225, 135 },
      { (char)8364, 136 },
      { (char)8240, 137 },
      { (char)1033, 138 },
      { (char)8249, 139 },
      { (char)1034, 140 },
      { (char)1036, 141 },
      { (char)1035, 142 },
      { (char)1039, 143 },
      { (char)1106, 144 },
      { (char)8216, 145 },
      { (char)8217, 146 },
      { (char)8220, 147 },
      { (char)8221, 148 },
      { (char)8226, 149 },
      { (char)8211, 150 },
      { (char)8212, 151 },
      { (char)152, 152 },
      { (char)8482, 153 },
      { (char)1113, 154 },
      { (char)8250, 155 },
      { (char)1114, 156 },
      { (char)1116, 157 },
      { (char)1115, 158 },
      { (char)1119, 159 },
      { (char)160, 160 },
      { (char)1038, 161 },
      { (char)1118, 162 },
      { (char)1032, 163 },
      { (char)164, 164 },
      { (char)1168, 165 },
      { (char)166, 166 },
      { (char)167, 167 },
      { (char)1025, 168 },
      { (char)169, 169 },
      { (char)1028, 170 },
      { (char)171, 171 },
      { (char)172, 172 },
      { (char)173, 173 },
      { (char)174, 174 },
      { (char)1031, 175 },
      { (char)176, 176 },
      { (char)177, 177 },
      { (char)1030, 178 },
      { (char)1110, 179 },
      { (char)1169, 180 },
      { (char)181, 181 },
      { (char)182, 182 },
      { (char)183, 183 },
      { (char)1105, 184 },
      { (char)8470, 185 },
      { (char)1108, 186 },
      { (char)187, 187 },
      { (char)1112, 188 },
      { (char)1029, 189 },
      { (char)1109, 190 },
      { (char)1111, 191 },
      { (char)1040, 192 },
      { (char)1041, 193 },
      { (char)1042, 194 },
      { (char)1043, 195 },
      { (char)1044, 196 },
      { (char)1045, 197 },
      { (char)1046, 198 },
      { (char)1047, 199 },
      { (char)1048, 200 },
      { (char)1049, 201 },
      { (char)1050, 202 },
      { (char)1051, 203 },
      { (char)1052, 204 },
      { (char)1053, 205 },
      { (char)1054, 206 },
      { (char)1055, 207 },
      { (char)1056, 208 },
      { (char)1057, 209 },
      { (char)1058, 210 },
      { (char)1059, 211 },
      { (char)1060, 212 },
      { (char)1061, 213 },
      { (char)1062, 214 },
      { (char)1063, 215 },
      { (char)1064, 216 },
      { (char)1065, 217 },
      { (char)1066, 218 },
      { (char)1067, 219 },
      { (char)1068, 220 },
      { (char)1069, 221 },
      { (char)1070, 222 },
      { (char)1071, 223 },
      { (char)1072, 224 },
      { (char)1073, 225 },
      { (char)1074, 226 },
      { (char)1075, 227 },
      { (char)1076, 228 },
      { (char)1077, 229 },
      { (char)1078, 230 },
      { (char)1079, 231 },
      { (char)1080, 232 },
      { (char)1081, 233 },
      { (char)1082, 234 },
      { (char)1083, 235 },
      { (char)1084, 236 },
      { (char)1085, 237 },
      { (char)1086, 238 },
      { (char)1087, 239 },
      { (char)1088, 240 },
      { (char)1089, 241 },
      { (char)1090, 242 },
      { (char)1091, 243 },
      { (char)1092, 244 },
      { (char)1093, 245 },
      { (char)1094, 246 },
      { (char)1095, 247 },
      { (char)1096, 248 },
      { (char)1097, 249 },
      { (char)1098, 250 },
      { (char)1099, 251 },
      { (char)1100, 252 },
      { (char)1101, 253 },
      { (char)1102, 254 },
      { (char)1103, 255 }
    };

        #endregion
    }
}
