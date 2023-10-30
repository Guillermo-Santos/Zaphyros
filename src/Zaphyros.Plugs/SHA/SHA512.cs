﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Zaphyros.Plugs.SHA
{
    public static class CustomSHA512
    {
        private const int BlockSize = 128;
        private const int DigestSize = 64;

        private static readonly ulong[] K =
        {
            0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc,
            0x3956c25bf348b538, 0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118,
            0xd807aa98a3030242, 0x12835b0145706fbe, 0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2,
            0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 0xc19bf174cf692694,
            0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
            0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5,
            0x983e5152ee66dfab, 0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4,
            0xc6e00bf33da88fc2, 0xd5a79147930aa725, 0x06ca6351e003826f, 0x142929670a0e6e70,
            0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 0x53380d139d95b3df,
            0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
            0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30,
            0xd192e819d6ef5218, 0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8,
            0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8,
            0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 0x682e6ff3d6b2b8a3,
            0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
            0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b,
            0xca273eceea26619c, 0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178,
            0x06f067aa72176fba, 0x0a637dc5a2c898a6, 0x113f9804bef90dae, 0x1b710b35131c471b,
            0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 0x431d67c49c100d4c,
            0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817
        };


        public static byte[] ComputeHash(byte[] data)
        {
            var hState = new ulong[8]
            {
                0x6a09e667f3bcc908, 0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, 0xa54ff53a5f1d36f1,
                0x510e527fade682d1, 0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179
            };

            data = AppendPaddingAndLength(data);

            for (int i = 0; i < data.Length; i += BlockSize)
            {
                ulong[] w = new ulong[80];
                for (int t = 0; t < 16; t++)
                    w[t] = ConvertBytesToUInt64(data, i + t * 8);

                for (int t = 16; t < 80; t++)
                    w[t] = Sigma1(w[t - 2]) + w[t - 7] + Sigma0(w[t - 15]) + w[t - 16];

                ulong a = hState[0], b = hState[1], c = hState[2], d = hState[3], e = hState[4], f = hState[5], g = hState[6], h = hState[7];

                for (int t = 0; t < 80; t++)
                {
                    ulong t1 = h + Sum1(e) + Ch(e, f, g) + K[t] + w[t];
                    ulong t2 = Sum0(a) + Maj(a, b, c);
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
                int destIndex = i * 8;
                hashBytes[destIndex] = (byte)(hState[i] >> 56);
                hashBytes[destIndex + 1] = (byte)(hState[i] >> 48);
                hashBytes[destIndex + 2] = (byte)(hState[i] >> 40);
                hashBytes[destIndex + 3] = (byte)(hState[i] >> 32);
                hashBytes[destIndex + 4] = (byte)(hState[i] >> 24);
                hashBytes[destIndex + 5] = (byte)(hState[i] >> 16);
                hashBytes[destIndex + 6] = (byte)(hState[i] >> 8);
                hashBytes[destIndex + 7] = (byte)hState[i];
            }

            return hashBytes;
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

        // Helper methods for SHA-512 operations (similar to SHA-384)

        private static ulong Sigma0(ulong x)
        {
            return (x >> 1 | x << 63) ^ (x >> 8 | x << 56) ^ x >> 7;
        }

        private static ulong Sigma1(ulong x)
        {
            return (x >> 19 | x << 45) ^ (x >> 61 | x << 3) ^ x >> 6;
        }

        private static ulong Sum0(ulong x)
        {
            return (x >> 28 | x << 36) ^ (x >> 34 | x << 30) ^ (x >> 39 | x << 25);
        }

        private static ulong Sum1(ulong x)
        {
            return (x >> 14 | x << 50) ^ (x >> 18 | x << 46) ^ (x >> 41 | x << 23);
        }

        private static ulong Ch(ulong x, ulong y, ulong z)
        {
            return x & y ^ ~x & z;
        }

        private static ulong Maj(ulong x, ulong y, ulong z)
        {
            return x & y ^ x & z ^ y & z;
        }

        private static ulong ConvertBytesToUInt64(byte[] bytes, int startIndex)
        {
            ulong result = 0;
            for (int i = 0; i < 8; i++)
            {
                result <<= 8;
                result |= bytes[startIndex + i];
            }
            return result;
        }

        // ... (ComputeHash and AppendPaddingAndLength methods as previously shown)

        // Helper method to encode a ulong value to a byte array in big-endian format
        private static byte[] EncodeUInt64ToBytes(ulong value)
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                result[7 - i] = (byte)(value >> i * 8);
            }
            return result;
        }

        // Helper method to convert a byte array to its hexadecimal string representation
        private static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

    }
}

