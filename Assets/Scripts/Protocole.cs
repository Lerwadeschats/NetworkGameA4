using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace Protocols
{
    public static class Protocole
    {
        #region Base Serialize

        static void Serialize_Uint8(List<byte> byteArray, byte value)
        {
            byteArray.Add(value);
        }
        static void Serialize_Int8(List<byte> byteArray, sbyte value)
        {
            Serialize_Uint8(byteArray, (byte)value);
        }
        static void Serialize_Uint16(List<byte> byteArray, ushort value)
        {
            short htons = IPAddress.HostToNetworkOrder((short)value);
            byte[] ar = BitConverter.GetBytes(htons);
            byteArray.AddRange(ar);
        }
        static void Serialize_int16(List<byte> byteArray, short value)
        {
            Serialize_Uint16(byteArray, (ushort)value);
        }
        static void Serialize_Uint32(List<byte> byteArray, uint value)
        {
            int htoni = IPAddress.HostToNetworkOrder((int)value);
            byte[] ar = BitConverter.GetBytes(htoni);
            byteArray.AddRange(ar);
        }
        static void Serialize_int32(List<byte> byteArray, short value)
        {
            Serialize_Uint32(byteArray, (ushort)value);
        }
        static void Seriailize_f(List<byte> byteArray, float value)
        {
            int htonf = IPAddress.HostToNetworkOrder((int)value);
            byte[] ar = BitConverter.GetBytes(htonf);
            byteArray.AddRange(ar);
        }
        static void Serialize_str(List<byte> byteArray, string value)
        {
            Serialize_Uint32(byteArray, (uint)value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                byte[] ar = BitConverter.GetBytes(value[i]);
                byteArray.AddRange(ar);
            }
        }
        static void Serialize_color(List<byte> byteArray, Color value)
        {
            Seriailize_f(byteArray, value.r);
            Seriailize_f(byteArray, value.g);
            Seriailize_f(byteArray, value.b);
            Seriailize_f(byteArray, value.a);
        }
        #endregion
        #region Base Deserialize
        static byte Deserialize_Uint8(List<byte> byteArray, int offset)
        {
            byte value = byteArray[offset];
            offset++;
            return value;
        }
        static sbyte Deserialize_int8(List<byte> byteArray, int offset)
        {
            byte val = Deserialize_Uint8(byteArray, offset);
            return unchecked((sbyte)val);
        }
        static ushort Deserialize_Uint16(List<byte> byteArray, int offset)
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
        static short Deserialize_int16(List<byte> byteArray, int offset)
        {
            ushort value = Deserialize_Uint16(byteArray, offset);
            return unchecked((short)value);
        }
        static uint Deserialize_Uint32(List<byte> byteArray, int offset)
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
        static int Deserialize_int32(List<byte> byteArray, int offset)
        {
            uint value = Deserialize_Uint32(byteArray, offset);
            return unchecked((int)value);
        }
        static float Deserialize_f(List<byte> byteArray, int offset)
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
        static string Deserialize_str(List<byte> byteArray, int offset)
        {
            uint length = Deserialize_Uint32(byteArray, offset);
            char[] str = new char[length];
            for (int i = 0; i < length; i++)
            {
                str[i] = (char)byteArray[i + offset];
            }
            string val = str.ToString();
            offset += (int)length;
            return val;
        }
        static Color Deserialize_color(List<byte> byteArray, int offset)
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
}

