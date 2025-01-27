using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class Protocole : MonoBehaviour
{
    #region Base Serialize

        public static void Serialize_Uint8(ref List<byte> byteArray, byte value)
        {
            byteArray.Add(value);
        }
        public static void Serialize_Int8(ref List<byte> byteArray, sbyte value)
        {
            Serialize_Uint8(ref byteArray, (byte)value);
        }
        public static void Serialize_Uint16(ref List<byte> byteArray, ushort value)
        {
            short htons = IPAddress.HostToNetworkOrder((short)value);
            byte[] ar = BitConverter.GetBytes(htons);
            byteArray.AddRange(ar);
        }
        public static void Serialize_int16(ref List<byte> byteArray, short value)
        {
            Serialize_Uint16(ref byteArray, (ushort)value);
        }
        public static void Serialize_Uint32(ref List<byte> byteArray, uint value)
        {
            ar[i] = byteArray[offset + i];
        }
        public static void Serialize_int32(ref List<byte> byteArray, short value)
        {
            Serialize_Uint32(ref byteArray, (ushort)value);
        }
        public static void Seriailize_f(ref List<byte> byteArray, float value)
        {
            str[i] = (char)byteArray[i + offset];
        }
        public static void Serialize_str(ref List<byte> byteArray, string value)
        {
            Serialize_Uint32(ref byteArray, (uint)value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                byte[] ar = BitConverter.GetBytes(value[i]);
                byteArray.AddRange(ar);
            }
        }
        public static void Serialize_color(ref List<byte> byteArray, Color value)
        {
            Seriailize_f(ref byteArray, value.r);
            Seriailize_f(ref byteArray, value.g);
            Seriailize_f(ref byteArray, value.b);
            Seriailize_f(ref byteArray, value.a);
        }
        #endregion
        #region Base Deserialize
        public static byte Deserialize_Uint8(List<byte> byteArray, ref int offset)
        {
            byte value = byteArray[offset];
            offset++;
            return value;
        }
        public static sbyte Deserialize_int8(List<byte> byteArray, ref int offset)
        {
            byte val = Deserialize_Uint8(byteArray, ref offset);
            return unchecked((sbyte)val);
        }
        public static ushort Deserialize_Uint16(List<byte> byteArray, ref int offset)
        {
            ushort value;
            byte[] ar = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                ar[i] = byteArray[offset + i];
            }
            value = BitConverter.ToUInt16(ar);
            ushort ntohs = (ushort)IPAddress.NetworkToHostOrder(value);
            offset += 2;
            return ntohs;
        }
        public static short Deserialize_int16(List<byte> byteArray, ref int offset)
        {
            ushort value = Deserialize_Uint16(byteArray, ref offset);
            return unchecked((short)value);
        }
        public static uint Deserialize_Uint32(List<byte> byteArray, ref int offset)
        {
            uint value;
            byte[] ar = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                ar[i] = byteArray[offset + i];
            }
            value = BitConverter.ToUInt32(ar);
            uint ntohi = (uint)IPAddress.NetworkToHostOrder(value);
            offset += 4;
            return ntohi;
        }
        public static int Deserialize_int32(List<byte> byteArray, ref int offset)
        {
            uint value = Deserialize_Uint32(byteArray, ref offset);
            return unchecked((int)value);
        }
        public static float Deserialize_f(List<byte> byteArray, ref int offset)
        {
            int val;
            byte[] ar = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                ar[i] = byteArray[offset + i];
            }
            val = BitConverter.ToInt32(ar);
            float ntohf = (float)IPAddress.NetworkToHostOrder(val);
            offset += 4;
            return ntohf;
        }
        public static string Deserialize_str(List<byte> byteArray, ref int offset)
        {
            uint length = Deserialize_Uint32(byteArray, ref offset);
            char[] str = new char[length];
            for (int i = 0; i < length; i++)
            {
                str[i] = (char)byteArray[i + offset];
            }
            string val = new string(str);
            offset += (int)length;
            return val;
        }
        public static Color Deserialize_color(List<byte> byteArray, ref int offset)
        {
            Color c;
            c.r = Deserialize_f(byteArray, ref offset);
            c.g = Deserialize_f(byteArray, ref offset);
            c.b = Deserialize_f(byteArray, ref offset);
            c.a = Deserialize_f(byteArray, ref offset);
            return c;
        }
        #endregion
    }
    Color Deserialize_color(List<byte> byteArray,int offset)
    {
        Color c;
        c.r = Deserialize_f(byteArray, offset);
        c.g = Deserialize_f(byteArray, offset);
        c.b = Deserialize_f(byteArray, offset);
        c.a = Deserialize_f(byteArray, offset);
        return c;
    }
    #endregion
}
