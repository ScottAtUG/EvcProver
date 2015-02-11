using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.InstrumentCommunication
{
    public static class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(string input)
        {
            ushort crc = 0;
            var inputHex = StringToHex(input);
            
            foreach (var b in HexToBytes(inputHex))
            {
                byte index = (byte)(crc ^ b);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        private static string StringToHex(string input)
        {
            string output = string.Empty;
            var values = input.ToCharArray();
            return values.Aggregate(output, (current, letter) => current + (current + "" + String.Format("{0:X}", Convert.ToInt32(letter))));
        }

        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return result;
        }

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
}
