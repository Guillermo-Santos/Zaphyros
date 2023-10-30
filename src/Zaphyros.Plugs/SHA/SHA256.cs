namespace Zaphyros.Plugs.SHA
{
    public static class CustomSHA256
    {

        private const int BlockSize = 64;
        private const int DigestSize = 32;

        private static readonly uint[] K =
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
            0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
            0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
            0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
            0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
            0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
            0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
            0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
            0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        public static byte[] ComputeHash(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            data = AppendPaddingAndLength(data);

            var hState = new uint[8]
            {
                0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
                0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
            };

            for (int i = 0; i < data.Length; i += BlockSize)
            {
                uint[] w = new uint[64];
                for (int t = 0; t < 16; t++)
                    w[t] = ConvertBytesToUInt32(data, i + t * 4);

                for (int t = 16; t < 64; t++)
                    w[t] = Sigma1(w[t - 2]) + w[t - 7] + Sigma0(w[t - 15]) + w[t - 16];

                uint a = hState[0], b = hState[1], c = hState[2], d = hState[3], e = hState[4], f = hState[5], g = hState[6], h = hState[7];

                for (int t = 0; t < 64; t++)
                {
                    uint t1 = h + Sum1(e) + Ch(e, f, g) + K[t] + w[t];
                    uint t2 = Sum0(a) + Maj(a, b, c);
                    h = g;
                    g = f;
                    f = e;
                    e = d + t1;
                    d = c;
                    c = b;
                    b = a;
                    a = t1 + t2;
                }

                hState[0] += a;
                hState[1] += b;
                hState[2] += c;
                hState[3] += d;
                hState[4] += e;
                hState[5] += f;
                hState[6] += g;
                hState[7] += h;
            }

            byte[] hashBytes = new byte[DigestSize];
            for (int i = 0; i < 8; i++)
            {
                int destIndex = i * 4;
                hashBytes[destIndex] = (byte)(hState[i] >> 24);
                hashBytes[destIndex + 1] = (byte)(hState[i] >> 16);
                hashBytes[destIndex + 2] = (byte)(hState[i] >> 8);
                hashBytes[destIndex + 3] = (byte)hState[i];
            }

            return hashBytes;
        }

        // ... (Constants and Helper Functions)

        private static uint Ch(uint x, uint y, uint z) => x & y ^ ~x & z;

        private static uint Maj(uint x, uint y, uint z) => x & y ^ x & z ^ y & z;

        private static uint Sum0(uint x) => RotateRight(x, 2) ^ RotateRight(x, 13) ^ RotateRight(x, 22);

        private static uint Sum1(uint x) => RotateRight(x, 6) ^ RotateRight(x, 11) ^ RotateRight(x, 25);

        private static uint Sigma0(uint x) => RotateRight(x, 7) ^ RotateRight(x, 18) ^ x >> 3;

        private static uint Sigma1(uint x) => RotateRight(x, 17) ^ RotateRight(x, 19) ^ x >> 10;

        private static uint RotateRight(uint value, int count) => value >> count | value << 32 - count;

        private static uint ConvertBytesToUInt32(byte[] data, int startIndex)
        {
            return (uint)data[startIndex] << 24 | (uint)data[startIndex + 1] << 16 |
                   (uint)data[startIndex + 2] << 8 | data[startIndex + 3];
        }

        private static byte[] AppendPaddingAndLength(byte[] data)
        {
            ulong messageLength = (ulong)data.Length * 8;

            int paddingSize = BlockSize - (data.Length + 1 + 8) % BlockSize;
            int paddedDataLength = data.Length + 1 + paddingSize + 8;

            // Create a new byte array to store the padded data and length
            byte[] paddedData = new byte[paddedDataLength];

            // Copy the original data into the padded data array
            Array.Copy(data, paddedData, data.Length);

            // Append a single 1 bit
            paddedData[data.Length] = 0x80;

            // Append zeros for padding
            for (int i = data.Length + 1; i < paddedDataLength - 8; i++)
            {
                paddedData[i] = 0;
            }

            // Append the message length in bits in big-endian format
            for (int i = 0; i < 8; i++)
            {
                paddedData[paddedDataLength - 8 + i] = (byte)(messageLength >> 56 - i * 8);
            }

            return paddedData;
        }

    }
}

/*
 * Copyright (c) 2010 Yuri K. Schlesner
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace Zaphyros.Plugs.SHA
{
    internal class Sha256
    {
        private static readonly uint[] K = new uint[64] {
            0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5, 0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3, 0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC, 0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7, 0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13, 0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3, 0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5, 0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208, 0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2
        };

        private static uint ROTL(uint x, byte n)
        {
            if (n >= 32) throw new ArgumentException("n");
            return x << n | x >> 32 - n;
        }

        private static uint ROTR(uint x, byte n)
        {
            if (n >= 32) throw new ArgumentException("n");
            return x >> n | x << 32 - n;
        }

        private static uint Ch(uint x, uint y, uint z)
        {
            return x & y ^ ~x & z;
        }

        private static uint Maj(uint x, uint y, uint z)
        {
            return x & y ^ x & z ^ y & z;
        }

        private static uint Sigma0(uint x)
        {
            return ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22);
        }

        private static uint Sigma1(uint x)
        {
            return ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25);
        }

        private static uint sigma0(uint x)
        {
            return ROTR(x, 7) ^ ROTR(x, 18) ^ x >> 3;
        }

        private static uint sigma1(uint x)
        {
            return ROTR(x, 17) ^ ROTR(x, 19) ^ x >> 10;
        }


        private uint[] H = new uint[8] {
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
        };

        private byte[] pendingBlock = new byte[64];
        private uint pendingBlockOff = 0;
        private uint[] uintBuffer = new uint[16];

        private ulong bitsProcessed = 0;

        private bool closed = false;

        private void ProcessBlock(uint[] M)
        {
            if (M.Length != 16) throw new ArgumentException("M");

            // 1. Prepare the message schedule (W[t]):
            uint[] W = new uint[64];
            for (int t = 0; t < 16; ++t)
            {
                W[t] = M[t];
            }

            for (int t = 16; t < 64; ++t)
            {
                W[t] = sigma1(W[t - 2]) + W[t - 7] + sigma0(W[t - 15]) + W[t - 16];
            }

            // 2. Initialize the eight working variables with the (i-1)-st hash value:
            uint a = H[0],
                   b = H[1],
                   c = H[2],
                   d = H[3],
                   e = H[4],
                   f = H[5],
                   g = H[6],
                   h = H[7];

            // 3. For t=0 to 63:
            for (int t = 0; t < 64; ++t)
            {
                uint T1 = h + Sigma1(e) + Ch(e, f, g) + K[t] + W[t];
                uint T2 = Sigma0(a) + Maj(a, b, c);
                h = g;
                g = f;
                f = e;
                e = d + T1;
                d = c;
                c = b;
                b = a;
                a = T1 + T2;
            }

            // 4. Compute the intermediate hash value H:
            H[0] = a + H[0];
            H[1] = b + H[1];
            H[2] = c + H[2];
            H[3] = d + H[3];
            H[4] = e + H[4];
            H[5] = f + H[5];
            H[6] = g + H[6];
            H[7] = h + H[7];
        }

        internal void AddData(byte[] data, uint offset, uint len)
        {
            if (closed)
                throw new InvalidOperationException("Adding data to a closed hasher.");

            if (len == 0)
                return;

            bitsProcessed += len * 8;

            while (len > 0)
            {
                uint amount_to_copy;

                if (len < 64)
                {
                    if (pendingBlockOff + len > 64)
                        amount_to_copy = 64 - pendingBlockOff;
                    else
                        amount_to_copy = len;
                }
                else
                {
                    amount_to_copy = 64 - pendingBlockOff;
                }

                Array.Copy(data, offset, pendingBlock, pendingBlockOff, amount_to_copy);
                len -= amount_to_copy;
                offset += amount_to_copy;
                pendingBlockOff += amount_to_copy;

                if (pendingBlockOff == 64)
                {
                    toUintArray(pendingBlock, uintBuffer);
                    ProcessBlock(uintBuffer);
                    pendingBlockOff = 0;
                }
            }
        }

        internal byte[] GetHash()
        {
            return toByteArray(GetHashUint());
        }

        internal uint[] GetHashUint()
        {
            if (!closed)
            {
                ulong size_temp = bitsProcessed;

                AddData(new byte[1] { 0x80 }, 0, 1);

                uint available_space = 64 - pendingBlockOff;

                if (available_space < 8)
                    available_space += 64;

                // 0-initialized
                byte[] padding = new byte[available_space];
                // Insert length ulong
                for (uint i = 1; i <= 8; ++i)
                {
                    padding[padding.Length - i] = (byte)size_temp;
                    size_temp >>= 8;
                }

                AddData(padding, 0u, (uint)padding.Length);

                if (pendingBlockOff != 0) throw new Exception("Pending block offset should be 0.");

                closed = true;
            }

            return H;
        }

        private static void toUintArray(byte[] src, uint[] dest)
        {
            for (uint i = 0, j = 0; i < dest.Length; ++i, j += 4)
            {
                dest[i] = (uint)src[j + 0] << 24 | (uint)src[j + 1] << 16 | (uint)src[j + 2] << 8 | src[j + 3];
            }
        }

        private static byte[] toByteArray(uint[] src)
        {
            byte[] dest = new byte[src.Length * 4];
            int pos = 0;

            for (int i = 0; i < src.Length; ++i)
            {
                dest[pos++] = (byte)(src[i] >> 24);
                dest[pos++] = (byte)(src[i] >> 16);
                dest[pos++] = (byte)(src[i] >> 8);
                dest[pos++] = (byte)src[i];
            }

            return dest;
        }
    }
}