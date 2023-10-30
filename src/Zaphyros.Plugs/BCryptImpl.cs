using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using IL2CPU.API.Attribs;
using System.Runtime.CompilerServices;
using Console = System.Console;
using static System.Environment;
using Zaphyros.Plugs.SHA;
using Zaphyros.Core.Security;
using Cosmos.Core.Memory;
using System.Runtime.InteropServices;

namespace Zaphyros.Plugs
{

    [Plug(Target = typeof(System.Runtime.DependentHandle))]
    public sealed class DependentHandleImpl
    {
        /*
        public static object? InternalGetTargetAndDependent(IntPtr handle)
        {
            if (handle == IntPtr.Zero) { return null; }

            return GCHandle.FromIntPtr(handle).Target;
        }*/

        [PlugMethod(Signature = "System_Object__System_Runtime_DependentHandle_InternalGetTargetAndDependent_System_IntPtr___System_Object_")]
        public static object? InternalGetTargetAndDependent(IntPtr handle, out object? dependant)
        {
            var GCHndl = GCHandle.FromIntPtr(handle);

            var targ = (DependaatHndl?)GCHndl.Target;

            if (targ.HasValue)
            {
                dependant = targ.Value.Item2?.Target;
                return targ.Value.Item1?.Target;
            }

            dependant = null;
            return null;
        }

        [PlugMethod(Signature = "System_IntPtr__System_Runtime_DependentHandle_InternalInitialize_System_Object__System_Object_")]
        public static IntPtr InternalInitialize(object? target, object? dependent)
        {
            /*
            var handl = new GCHandle
            {
                Target = new DependaatHndl(GCHandle.Alloc(target), GCHandle.Alloc(dependent))
            };*/

            return IntPtr.Zero;
            //return GCHandle.ToIntPtr(handl);
        }

        public static void InternalFree(IntPtr handle)
        {
            GCHandle.FromIntPtr(handle).Free();
        }

    }
    [Plug(Target = typeof(BCrypt.Net.BCrypt))]
    public sealed class BCryptImpl
    {
        private static readonly Encoding SafeUTF8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        private static readonly char[] Base64Code = new char[64]
        {
            '.', '/', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
            'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b',
            'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
            'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9'
        };
        private static ISeedProvider? seedProvider;
        public static ISeedProvider SeedProvider { get => seedProvider ??= new SeedProvider(); set => seedProvider = value; }
        private static Random Random => new(SeedProvider.GetSeed());

        public static byte[] EnhancedHash(byte[] inputBytes, char bcryptMinorRevision, HashType hashType)
        {
            try
            {
                switch (hashType)
                {
                    case HashType.SHA256:
                        var hash = new Sha256();
                        hash.AddData(inputBytes, 0, (uint)inputBytes.Length);
                        inputBytes = SafeUTF8.GetBytes(Convert.ToBase64String(hash.GetHash()) + (bcryptMinorRevision >= 'a' ? "\0" : ""));
                        //inputBytes = SafeUTF8.GetBytes(Convert.ToBase64String(CustomSHA256.ComputeHash(inputBytes)) + ((bcryptMinorRevision >= 'a') ? "\0" : ""));
                        break;
                    case HashType.SHA384:
                        inputBytes = SafeUTF8.GetBytes(Convert.ToBase64String(CustomSHA384.ComputeHash(inputBytes)) + (bcryptMinorRevision >= 'a' ? "\0" : ""));
                        break;
                    case HashType.SHA512:
                        inputBytes = SafeUTF8.GetBytes(Convert.ToBase64String(CustomSHA512.ComputeHash(inputBytes)) + (bcryptMinorRevision >= 'a' ? "\0" : ""));
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Pre-hashing failed, using raw data");
            }

            return inputBytes;
        }

        public static string GenerateSalt(int workFactor, char bcryptMinorRevision = 'a')
        {
            if (workFactor < 4 || workFactor > 31)
            {
                object actualValue = workFactor;
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new(49, 2);
                defaultInterpolatedStringHandler.AppendLiteral("The work factor must be between ");
                defaultInterpolatedStringHandler.AppendFormatted((short)4);
                defaultInterpolatedStringHandler.AppendLiteral(" and ");
                defaultInterpolatedStringHandler.AppendFormatted((short)31);
                defaultInterpolatedStringHandler.AppendLiteral(" (inclusive)");
                throw new ArgumentOutOfRangeException(nameof(workFactor), actualValue, defaultInterpolatedStringHandler.ToStringAndClear());
            }

            if (bcryptMinorRevision != 'a' && bcryptMinorRevision != 'b' && bcryptMinorRevision != 'x' && bcryptMinorRevision != 'y')
            {
                throw new ArgumentException("BCrypt Revision should be a, b, x or y", "bcryptMinorRevision");
            }

            byte[] array = new byte[16];
            Console.WriteLine("gs 1");
            Random.NextBytes(array);

            for (int i = 0; i < array.Length; i++)
            {
                byte b = array[Random.Next(0, array.Length - 1)];
                array[i] = (byte)((b * b ^ b >> b) - 1);
            }

            Console.WriteLine("gs 2");
            var stringBuilder = new StringBuilder(29);
            stringBuilder.Append("$2").Append(bcryptMinorRevision).Append('$')
                .Append(workFactor.ToString("D2"))
                .Append('$');
            stringBuilder.Append(EncodeBase64(array, array.Length));
            return stringBuilder.ToString();
        }

        static void NextBytes(byte[] array)
        {
            var offset = Random.Next(10_000, 50_000);
            byte[] bytes = Fibonacci.GetSingleton().Skip(offset).First(); // Buffer for generating random ulongs

            for (int i = 0; i < bytes.Length; i++)
            {
                array[i % 16] ^= bytes[i];
            }
        }
        public static char[] EncodeBase64(byte[] byteArray, int length)
        {
            if (length <= 0 || length > byteArray.Length)
            {
                throw new ArgumentException("Invalid length", "length");
            }

            char[] array = new char[(int)Math.Ceiling(length * 4.0 / 3.0)];
            int num = 0;
            int num2 = 0;
            while (num2 < length)
            {
                int num3 = byteArray[num2++] & 0xFF;
                array[num++] = Base64Code[num3 >> 2 & 0x3F];
                num3 = (num3 & 3) << 4;
                if (num2 >= length)
                {
                    array[num++] = Base64Code[num3 & 0x3F];
                    break;
                }

                int num4 = byteArray[num2++] & 0xFF;
                num3 |= num4 >> 4 & 0xF;
                array[num++] = Base64Code[num3 & 0x3F];
                num3 = (num4 & 0xF) << 2;
                if (num2 >= length)
                {
                    array[num++] = Base64Code[num3 & 0x3F];
                    break;
                }

                num4 = byteArray[num2++] & 0xFF;
                num3 |= num4 >> 6 & 3;
                array[num++] = Base64Code[num3 & 0x3F];
                array[num++] = Base64Code[num4 & 0x3F];
            }

            return array;
        }
    }

    static class Fibonacci
    {
        static BigInteger current = 1, next = 1;

        public static IEnumerable<byte[]> GetSingleton()
        {
            while (true)
            {
                yield return current.ToByteArray();
                BigInteger newNext = current + next;
                current = next;
                next = newNext;
            }
        }

        public static IEnumerable<byte[]> GetTransient()
        {
            BigInteger current = 1, next = 1;

            while (true)
            {
                yield return current.ToByteArray();
                BigInteger newNext = current + next;
                current = next;
                next = newNext;
            }
        }

        public static void Reset()
        {
            current = 1;
            next = 1;
        }
    }

    internal record struct DependaatHndl(GCHandle? Item1, GCHandle? Item2)
    {
        public static implicit operator (GCHandle?, GCHandle?)(DependaatHndl value)
        {
            return (value.Item1, value.Item2);
        }

        public static implicit operator DependaatHndl((GCHandle?, GCHandle?) value)
        {
            return new DependaatHndl(value.Item1, value.Item2);
        }
    }
}