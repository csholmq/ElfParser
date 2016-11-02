using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfParser.Dwarf
{
    class LEB128
    {
        public static ulong ReadUnsigned(List<byte> data, ref int index)
        {
            var input = new List<byte>();
            byte chunk;

            do
            {
                chunk = data[index];
                index++;

                input.Add(chunk);
            } while ((chunk & 0x80) > 0);

            return LEB128.DecodeUnsigned(input.ToArray());
        }

        public static long ReadSigned(List<byte> data, ref int index)
        {
            var input = new List<byte>();
            byte chunk;

            do
            {
                chunk = data[index];
                index++;

                input.Add(chunk);
            } while ((chunk & 0x80) > 0);

            return LEB128.DecodeSigned(input.ToArray());
        }

        static byte[] EncodeUnsigned(ulong input)
        {
            var output = new List<byte>();

            while (input != 0)
            {
                var chunk = (byte)(input & 0x7F);
                input >>= 7;

                if (input != 0)
                {
                    chunk |= 0x80;
                }

                output.Add(chunk);
            }

            return output.ToArray();
        }

        static byte[] EncodeSigned(long input)
        {
            var output = new List<byte>();
            var more = true;

            while (more)
            {
                var chunk = (byte)(input & 0x7F);
                input >>= 7;

                // Sign bit of byte is 2nd high order bit (0x40)
                if ((input == 0 && (chunk & 0x40) == 0) ||
                    (input == -1 && (chunk & 0x40) > 0))
                {
                    more = false;
                }
                else
                {
                    chunk |= 0x80;
                }

                output.Add(chunk);
            }

            return output.ToArray();
        }

        static ulong DecodeUnsigned(byte[] input)
        {
            ulong output = 0;
            var shift = 0;

            for (int i = 0; i < input.Length; i++)
            {
                var chunk = input[i];
                output |= ((ulong)chunk & 0x7F) << shift;
                shift += 7;

                if ((chunk & 0x80) == 0) { break; }
            }

            return output;
        }

        static long DecodeSigned(byte[] input)
        {
            long output = 0;
            var shift = 0;
            var size = 64;
            byte chunk = 0;

            for (int i = 0; i < input.Length; i++)
            {
                chunk = input[i];
                output |= ((long)chunk & 0x7F) << shift;
                shift += 7;

                if ((chunk & 0x80) == 0) { break; }
            }

            // Sign bit of byte is 2nd high order bit (0x40)
            if ((shift < size) && ((chunk & 0x40) != 0))
            {
                // Sign extend
                output |= -((long)1 << shift);
            }

            return output;
        }
    }
}
