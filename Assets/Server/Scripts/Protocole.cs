using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Reflection;

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
        public static void Serialize_int32(ref List<byte> byteArray, int value)
        {
            Serialize_Uint32(ref byteArray, (uint)value);
        }
        public static void Serialize_Uint64(ref List<byte> byteArray, ulong value)
        {
            long htonl = IPAddress.HostToNetworkOrder((long)value);
            byte[] ar = BitConverter.GetBytes(htonl);
            byteArray.AddRange(ar);
        }
        public static void Serialize_int64(ref List<byte> byteArray, long value)
        {
            Serialize_Uint64(ref byteArray, (ulong)value);
        }
        public static void Serialize_f(ref List<byte> byteArray, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            byteArray.AddRange(bytes);
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
            Serialize_f(ref byteArray, value.r);
            Serialize_f(ref byteArray, value.g);
            Serialize_f(ref byteArray, value.b);
            Serialize_f(ref byteArray, value.a);
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
        public static ulong Deserialize_Uint64(List<byte> byteArray, ref int offset)
        {
            long value;
            byte[] ar = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                ar[i] = byteArray[offset + i];
            }
            value = BitConverter.ToInt64(ar);
            ulong ntohl = (ulong)IPAddress.NetworkToHostOrder(value);
            offset += 8;
            return ntohl;
        }
        public static long Deserialize_int64(List<byte> byteArray, ref int offset)
        {
            ulong value = Deserialize_Uint64(byteArray, ref offset);
            return unchecked((long)value);
        }
        public static float Deserialize_f(List<byte> byteArray, ref int offset)
        {
            float val;
            byte[] ar = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                ar[i] = byteArray[offset + i];
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ar);
            }
            val = BitConverter.ToSingle(ar);
            offset += sizeof(Single);
            return val;
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
        public enum Opcode : byte
        {
            C_PlayerName,
            C_PlayerInputs,
            S_PlayerNames,
            S_GameData,
            S_WorldInit,
            S_PlayerDeath,
            S_PlayerList,
            S_PlayersPosition,
            S_PlayerStats
        };
        #endregion
        #region packets
        public struct PlayerNamePacket
        {
            static Opcode opcode = Opcode.C_PlayerName;

            public string name;

            public void Serialize(ref List<byte> byteArray)
            {
                Serialize_Uint8(ref byteArray, (byte)opcode);
                Serialize_str(ref byteArray, name);

            }
            public static PlayerNamePacket Deserialize(List<byte> byteArray, int offset)
            {
                PlayerNamePacket packet;
                packet.name = Deserialize_str(byteArray, ref offset);
                return packet;
            }
        };

        public struct GameDataPacket
        {
            static Opcode opcode = Opcode.S_GameData;

            public uint playerIndex;

            public void Serialize(ref List<byte> byteArray)
            {
                Serialize_Uint8(ref byteArray, (byte)opcode);
                Serialize_Uint32(ref byteArray, playerIndex);

            }
            public static GameDataPacket Deserialize(List<byte> byteArray, int offset)
            {
                GameDataPacket packet;
                packet.playerIndex = Deserialize_Uint32(byteArray, ref offset);
                return packet;
            }

        }

        public struct ListPlayersNamePacket
        {
            static Opcode opcode = Opcode.C_PlayerName;
            public List<string> names;

            public void Serialize(ref List<byte> byteArray)
            {
                Serialize_Uint8(ref byteArray, (byte)opcode);
                Serialize_int32(ref byteArray, names.Count);
                foreach (string name in names)
                {
                    Serialize_str(ref byteArray, name);
                }
            }
            public static ListPlayersNamePacket Deserialize(List<byte> byteArray, int offset)
            {
                ListPlayersNamePacket packet;
                packet.names = new List<string>();
                string[] nameArray = new string[Deserialize_int32(byteArray, ref offset)];
                for (int i = 0; i < nameArray.Length; i++)
                {
                    nameArray[i] = Deserialize_str(byteArray, ref offset);
                }
                packet.names.AddRange(nameArray);
                return packet;
            }
        };
        public struct PlayerInputsPacket
        {
            static Opcode opcode = Opcode.C_PlayerInputs;

            public PlayerInputs inputs;

            public void Serialize(ref List<byte> byteArray)
            {
                Serialize_Uint8(ref byteArray, (byte)opcode);
                byte inputByte = 0;
                if (inputs.moveLeft)
                    inputByte |= 1 << 0;

                if (inputs.moveRight)
                    inputByte |= 1 << 1;
                if (inputs.jump)
                    inputByte |= 1 << 2;
                if (inputs.attack)
                    inputByte |= 1 << 3;
                if (inputs.dash)
                    inputByte |= 1 << 4;
                if (inputs.block)
                    inputByte |= 1 << 5;
                Serialize_Uint8(ref byteArray, inputByte);
            }
            public static PlayerInputsPacket Deserialize(List<byte> byteArray, int offset)
            {
                byte inputByte = Deserialize_Uint8(byteArray, ref offset);

                PlayerInputsPacket packet;
                packet.inputs = new PlayerInputs();
                packet.inputs.moveLeft = (inputByte & (1 << 0)) != 0;
                packet.inputs.moveRight = (inputByte & (1 << 1)) != 0;
                packet.inputs.jump = (inputByte & (1 << 2)) != 0;
                packet.inputs.attack = (inputByte & (1 << 3)) != 0;
                packet.inputs.dash = (inputByte & (1 << 4)) != 0;
                packet.inputs.block = (inputByte & (1 << 5)) != 0;
                return packet;
            }
        }
        public struct WorldInitPacket
        {
            static Opcode opcode = Opcode.S_WorldInit;
            public ulong seed;

            public void Serialize(ref List<byte> byteArray)
            {
                Serialize_Uint8(ref byteArray, (byte)opcode);
                Serialize_Uint64(ref byteArray, seed);
            }
            public static WorldInitPacket Deserialize(List<byte> byteArray, int offset)
            {
                WorldInitPacket packet;
                packet.seed = Deserialize_Uint64(byteArray, ref offset);
                return packet;
            }
        }
        public struct PlayerPositionPacket
        {
            static Opcode opcode = Opcode.S_PlayersPosition;

            public struct PlayerData
            {
                public byte playerIndex;
                public Vector2 position;
                public Vector2 velocity;
                public PlayerInputs inputs;
            };

            public List<PlayerData> players;
            byte positionIndex;
            public void Serialize(ref List<byte> byteArray)
            {
                Serialize_Uint8(ref byteArray, (byte)opcode);
                Serialize_int32(ref byteArray, players.Count);
                foreach (PlayerData player in players)
                {
                    Serialize_Uint8(ref byteArray, player.playerIndex);
                    Serialize_f(ref byteArray, player.position.x);
                    Serialize_f(ref byteArray, player.position.y);
                    Serialize_f(ref byteArray, player.velocity.x);
                    Serialize_f(ref byteArray, player.position.y);
                    Serialize_Uint8(ref byteArray, (byte)opcode);
                    byte inputByte = 0;
                    if (player.inputs.moveLeft)
                        inputByte |= 1 << 0;

                    if (player.inputs.moveRight)
                        inputByte |= 1 << 1;
                    if (player.inputs.jump)
                        inputByte |= 1 << 2;
                    if (player.inputs.attack)
                        inputByte |= 1 << 3;
                    if (player.inputs.dash)
                        inputByte |= 1 << 4;
                    if (player.inputs.block)
                        inputByte |= 1 << 5;
                    Serialize_Uint8(ref byteArray, inputByte);
                }
                Serialize_Uint8(ref byteArray, positionIndex);
            }
            public static PlayerPositionPacket Deserialize(List<byte> byteArray, int offset)
            {
                PlayerPositionPacket packet;
                packet.players = new List<PlayerData>();
                PlayerData[] playersArray = new PlayerData[Deserialize_int32(byteArray, ref offset)];
                for (int i = 0; i < playersArray.Length; i++)
                {
                    playersArray[i].playerIndex = Deserialize_Uint8(byteArray, ref offset);
                    playersArray[i].position.x = Deserialize_f(byteArray, ref offset);
                    playersArray[i].position.y = Deserialize_f(byteArray, ref offset);
                    playersArray[i].velocity.x = Deserialize_f(byteArray, ref offset);
                    playersArray[i].velocity.y = Deserialize_f(byteArray, ref offset);
                    byte inputByte = Deserialize_Uint8(byteArray, ref offset);
                    playersArray[i].inputs = new PlayerInputs();
                    playersArray[i].inputs.moveLeft = (inputByte & (1 << 0)) != 0;
                    playersArray[i].inputs.moveRight = (inputByte & (1 << 1)) != 0;
                    playersArray[i].inputs.jump = (inputByte & (1 << 2)) != 0;
                    playersArray[i].inputs.attack = (inputByte & (1 << 3)) != 0;
                    playersArray[i].inputs.dash = (inputByte & (1 << 4)) != 0;
                    playersArray[i].inputs.block = (inputByte & (1 << 5)) != 0;
                }
                packet.players.AddRange(playersArray);
                packet.positionIndex = Deserialize_Uint8(byteArray, ref offset);
                return packet;
            }
            public struct PlayerStatsPacket
            {
                static Opcode opcode = Opcode.S_PlayerStats;
                List<PlayerStatData> playerStatsList;
                struct PlayerStatData
                {
                    public byte playerIndex;
                    public Player.PlayerStats playerStats;
                }
                byte statsindex;

                public void Serialize(ref List<byte> byteArray)
                {
                    Serialize_Uint8(ref byteArray, (byte)opcode);
                    Serialize_int32(ref byteArray, playerStatsList.Count);
                    foreach (PlayerStatData player in playerStatsList)
                    {
                        Serialize_Uint8(ref byteArray, player.playerIndex);
                        Serialize_f(ref byteArray, player.playerStats.hpValue);
                        Serialize_f(ref byteArray, player.playerStats.maxHpValue);
                        Serialize_f(ref byteArray, player.playerStats.speed);
                        Serialize_f(ref byteArray, player.playerStats.attackValue);
                    }
                    Serialize_Uint8(ref byteArray, statsindex);
                }
                public static PlayerStatsPacket Deserialize(List<byte> byteArray, int offset)
                {
                    PlayerStatsPacket packet;
                    packet.playerStatsList = new List<PlayerStatData>();
                    PlayerStatData[] playersArray = new PlayerStatData[Deserialize_int32(byteArray, ref offset)];
                    for (int i = 0; i < playersArray.Length; i++)
                    {
                        playersArray[i].playerIndex = Deserialize_Uint8(byteArray, ref offset);
                        playersArray[i].playerStats.hpValue = Deserialize_f(byteArray, ref offset);
                        playersArray[i].playerStats.maxHpValue = Deserialize_f(byteArray, ref offset);
                        playersArray[i].playerStats.speed = Deserialize_f(byteArray, ref offset);
                        playersArray[i].playerStats.attackValue = Deserialize_f(byteArray, ref offset);
                        //inputs
                    }
                    packet.playerStatsList.AddRange(playersArray);
                    packet.statsindex = Deserialize_Uint8(byteArray, ref offset);
                    return packet;
                }
            }
        }
    }
    #endregion
 }