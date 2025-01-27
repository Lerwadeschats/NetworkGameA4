using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace Protocols
{
    public static class Protocole
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
            int htoni = IPAddress.HostToNetworkOrder((int)value);
            byte[] ar = BitConverter.GetBytes(htoni);
            byteArray.AddRange(ar);
        }
        public static void Serialize_int32(ref List<byte> byteArray, short value)
        {
            Serialize_Uint32(ref byteArray, (ushort)value);
        }
        public static void Seriailize_f(ref List<byte> byteArray, float value)
        {
            int htonf = IPAddress.HostToNetworkOrder((int)value);
            byte[] ar = BitConverter.GetBytes(htonf);
            byteArray.AddRange(ar);
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
            short value;
            byte[] ar = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                ar[i] = byteArray[offset + i];
            }
            value = BitConverter.ToInt16(ar);
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
            int value;
            byte[] ar = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                ar[i] = byteArray[offset + i];
            }
            value = BitConverter.ToInt32(ar);
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
                int j = i * 2;
                str[i] = (char)byteArray[j + offset];
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
        #region OPCODES
        enum Opcode : byte
        {
            C_PlayerName,
            C_PlayerInputs,
            S_GameData,
            S_WorldInit,
            S_PlayerDeath,
            S_PlayerList,
            S_PlayersPosition,
        };
        #endregion
        #region packets
        struct PlayerInputsPacket
        {
            static Opcode opcode = Opcode.C_PlayerInputs;

            //PlayerInputs inputs;

            void Serialize(List<byte> byteArray)
            {
                byte inputByte = 0;
                /* if (inputs.moveLeft)
                     inputByte |= 1 << 0;

                 if (inputs.moveRight)
                     inputByte |= 1 << 1;

                 if (inputs.jump)
                     inputByte |= 1 << 2;*/
                Serialize_Uint8(ref byteArray, inputByte);
            }
            static PlayerInputsPacket Deserialize(List<byte> byteArray, int offset)
            {
                byte inputByte = Deserialize_Uint8(byteArray, ref offset);

                PlayerInputsPacket packet;
                /*packet.inputs.moveLeft = (inputByte & (1 << 0)) != 0;
                packet.inputs.moveRight = (inputByte & (1 << 1)) != 0;
                packet.inputs.jump = (inputByte & (1 << 2)) != 0;*/
                return packet;
            }
        }
        #endregion
    }
}

