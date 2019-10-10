using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPClassLibrary.Helpers
{
    public sealed class Md5 : IDisposable
    {
        private const byte S11 = 7;

        private const byte S12 = 12;

        private const byte S13 = 17;

        private const byte S14 = 22;

        private const byte S21 = 5;

        private const byte S22 = 9;

        private const byte S23 = 14;

        private const byte S24 = 20;

        private const byte S31 = 4;

        private const byte S32 = 11;

        private const byte S33 = 16;

        private const byte S34 = 23;

        private const byte S41 = 6;

        private const byte S42 = 10;

        private const byte S43 = 15;

        private const byte S44 = 21;

        private static byte[] _padding;

        private readonly uint[] _state = new uint[4];

        private readonly uint[] _count = new uint[2];

        private readonly byte[] _buffer = new byte[64];

        private byte[] _hashValue;

        private const int HashSizeValue = 128;

        public bool CanReuseTransform => true;

        public bool CanTransformMultipleBlocks => true;

        public byte[] Hash
        {
            get
            {
                if (this.State1 != 0)
                {
                    throw new InvalidOperationException();
                }
                return (byte[])this._hashValue.Clone();
            }
        }

        public int HashSize => HashSizeValue;

        public int InputBlockSize => 1;

        public int OutputBlockSize => 1;

        public int State1 { get; set; }

        public static Md5 Create(string hashName)
        {
            if (hashName == "MD5")
            {
                return new Md5();
            }
            throw new NotSupportedException();
        }

        public static string GetMd5String(string source)
        {
            var mD = Md5.Create();
            var uTf8Encoding = new UTF8Encoding();
            var bytes = uTf8Encoding.GetBytes(source);
            var array = mD.ComputeHash(bytes);
            var stringBuilder = new StringBuilder();
            var array2 = array;
            foreach (var b in array2)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static Md5 Create()
        {
            return new Md5();
        }

        private static uint F(uint x, uint y, uint z)
        {
            return (x & y) | (~x & z);
        }

        private static uint G(uint x, uint y, uint z)
        {
            return (x & z) | (y & ~z);
        }

        private static uint H(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private static uint I(uint x, uint y, uint z)
        {
            return y ^ (x | ~z);
        }

        private static uint ROTATE_LEFT(uint x, byte n)
        {
            return x << n | x >> (int)(32 - n);
        }

        private static void Ff(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += F(b, c, d) + x + ac;
            a = ROTATE_LEFT(a, s);
            a += b;
        }

        private static void Gg(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += G(b, c, d) + x + ac;
            a = ROTATE_LEFT(a, s);
            a += b;
        }

        private static void Hh(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += H(b, c, d) + x + ac;
            a = ROTATE_LEFT(a, s);
            a += b;
        }

        private static void Ii(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += I(b, c, d) + x + ac;
            a = ROTATE_LEFT(a, s);
            a += b;
        }

        internal Md5()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            this._count[0] = (this._count[1] = 0u);
            this._state[0] = 1732584193u;
            this._state[1] = 4023233417u;
            this._state[2] = 2562383102u;
            this._state[3] = 271733878u;
        }

        private void HashCore(byte[] input, int offset, int count)
        {
            var num = (int)(this._count[0] >> 3 & 63u);
            if ((this._count[0] += (uint)((uint)count << 3)) < (uint)((uint)count << 3))
            {
                this._count[1] += 1u;
            }
            _count[1] += (uint)count >> 29;
            var num2 = 64 - num;
            int num3;
            if (count >= num2)
            {
                Buffer.BlockCopy(input, offset, this._buffer, num, num2);
                this.Transform(this._buffer, 0);
                num3 = num2;
                while (num3 + 63 < count)
                {
                    this.Transform(input, offset + num3);
                    num3 += 64;
                }
                num = 0;
            }
            else
            {
                num3 = 0;
            }
            Buffer.BlockCopy(input, offset + num3, this._buffer, num, count - num3);
        }

        private byte[] HashFinal()
        {
            var array = new byte[16];
            var array2 = new byte[8];
            Encode(array2, 0, this._count, 0, 8);
            var num = (int)(this._count[0] >> 3 & 63u);
            var num2 = (num < 56) ? (56 - num) : (120 - num);
            HashCore(Md5._padding, 0, num2);
            HashCore(array2, 0, 8);
            Encode(array, 0, this._state, 0, 16);
            _count[0] = (this._count[1] = 0u);
            _state[0] = 0u;
            _state[1] = 0u;
            _state[2] = 0u;
            _state[3] = 0u;
            Initialize();
            return array;
        }

        private void Transform(byte[] block, int offset)
        {
            var num = this._state[0];
            var num2 = this._state[1];
            var num3 = this._state[2];
            var num4 = this._state[3];
            var array = new uint[16];
            Decode(array, 0, block, offset, 64);
            Ff(ref num, num2, num3, num4, array[0], 7, 3614090360u);
            Ff(ref num4, num, num2, num3, array[1], 12, 3905402710u);
            Ff(ref num3, num4, num, num2, array[2], 17, 606105819u);
            Ff(ref num2, num3, num4, num, array[3], 22, 3250441966u);
            Ff(ref num, num2, num3, num4, array[4], 7, 4118548399u);
            Ff(ref num4, num, num2, num3, array[5], 12, 1200080426u);
            Ff(ref num3, num4, num, num2, array[6], 17, 2821735955u);
            Ff(ref num2, num3, num4, num, array[7], 22, 4249261313u);
            Ff(ref num, num2, num3, num4, array[8], 7, 1770035416u);
            Ff(ref num4, num, num2, num3, array[9], 12, 2336552879u);
            Ff(ref num3, num4, num, num2, array[10], 17, 4294925233u);
            Ff(ref num2, num3, num4, num, array[11], 22, 2304563134u);
            Ff(ref num, num2, num3, num4, array[12], 7, 1804603682u);
            Ff(ref num4, num, num2, num3, array[13], 12, 4254626195u);
            Ff(ref num3, num4, num, num2, array[14], 17, 2792965006u);
            Ff(ref num2, num3, num4, num, array[15], 22, 1236535329u);
            Gg(ref num, num2, num3, num4, array[1], 5, 4129170786u);
            Gg(ref num4, num, num2, num3, array[6], 9, 3225465664u);
            Gg(ref num3, num4, num, num2, array[11], 14, 643717713u);
            Gg(ref num2, num3, num4, num, array[0], 20, 3921069994u);
            Gg(ref num, num2, num3, num4, array[5], 5, 3593408605u);
            Gg(ref num4, num, num2, num3, array[10], 9, 38016083u);
            Gg(ref num3, num4, num, num2, array[15], 14, 3634488961u);
            Gg(ref num2, num3, num4, num, array[4], 20, 3889429448u);
            Gg(ref num, num2, num3, num4, array[9], 5, 568446438u);
            Gg(ref num4, num, num2, num3, array[14], 9, 3275163606u);
            Gg(ref num3, num4, num, num2, array[3], 14, 4107603335u);
            Gg(ref num2, num3, num4, num, array[8], 20, 1163531501u);
            Gg(ref num, num2, num3, num4, array[13], 5, 2850285829u);
            Gg(ref num4, num, num2, num3, array[2], 9, 4243563512u);
            Gg(ref num3, num4, num, num2, array[7], 14, 1735328473u);
            Gg(ref num2, num3, num4, num, array[12], 20, 2368359562u);
            Hh(ref num, num2, num3, num4, array[5], 4, 4294588738u);
            Hh(ref num4, num, num2, num3, array[8], 11, 2272392833u);
            Hh(ref num3, num4, num, num2, array[11], 16, 1839030562u);
            Hh(ref num2, num3, num4, num, array[14], 23, 4259657740u);
            Hh(ref num, num2, num3, num4, array[1], 4, 2763975236u);
            Hh(ref num4, num, num2, num3, array[4], 11, 1272893353u);
            Hh(ref num3, num4, num, num2, array[7], 16, 4139469664u);
            Hh(ref num2, num3, num4, num, array[10], 23, 3200236656u);
            Hh(ref num, num2, num3, num4, array[13], 4, 681279174u);
            Hh(ref num4, num, num2, num3, array[0], 11, 3936430074u);
            Hh(ref num3, num4, num, num2, array[3], 16, 3572445317u);
            Hh(ref num2, num3, num4, num, array[6], 23, 76029189u);
            Hh(ref num, num2, num3, num4, array[9], 4, 3654602809u);
            Hh(ref num4, num, num2, num3, array[12], 11, 3873151461u);
            Hh(ref num3, num4, num, num2, array[15], 16, 530742520u);
            Hh(ref num2, num3, num4, num, array[2], 23, 3299628645u);
            Ii(ref num, num2, num3, num4, array[0], 6, 4096336452u);
            Ii(ref num4, num, num2, num3, array[7], 10, 1126891415u);
            Ii(ref num3, num4, num, num2, array[14], 15, 2878612391u);
            Ii(ref num2, num3, num4, num, array[5], 21, 4237533241u);
            Ii(ref num, num2, num3, num4, array[12], 6, 1700485571u);
            Ii(ref num4, num, num2, num3, array[3], 10, 2399980690u);
            Ii(ref num3, num4, num, num2, array[10], 15, 4293915773u);
            Ii(ref num2, num3, num4, num, array[1], 21, 2240044497u);
            Ii(ref num, num2, num3, num4, array[8], 6, 1873313359u);
            Ii(ref num4, num, num2, num3, array[15], 10, 4264355552u);
            Ii(ref num3, num4, num, num2, array[6], 15, 2734768916u);
            Ii(ref num2, num3, num4, num, array[13], 21, 1309151649u);
            Ii(ref num, num2, num3, num4, array[4], 6, 4149444226u);
            Ii(ref num4, num, num2, num3, array[11], 10, 3174756917u);
            Ii(ref num3, num4, num, num2, array[2], 15, 718787259u);
            Ii(ref num2, num3, num4, num, array[9], 21, 3951481745u);
            _state[0] += num;
            _state[1] += num2;
            _state[2] += num3;
            _state[3] += num4;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0u;
            }
        }

        private static void Encode(IList<byte> output, int outputOffset, IReadOnlyList<uint> input, int inputOffset, int count)
        {
            var num = outputOffset + count;
            var num2 = inputOffset;
            for (var i = outputOffset; i < num; i += 4)
            {
                output[i] = (byte)(input[num2] & 255u);
                output[i + 1] = (byte)(input[num2] >> 8 & 255u);
                output[i + 2] = (byte)(input[num2] >> 16 & 255u);
                output[i + 3] = (byte)(input[num2] >> 24 & 255u);
                num2++;
            }
        }

        private static void Decode(IList<uint> output, int outputOffset, IReadOnlyList<byte> input, int inputOffset, int count)
        {
            var num = inputOffset + count;
            var num2 = outputOffset;
            for (var i = inputOffset; i < num; i += 4)
            {
                output[num2] = (uint)(input[i] | input[i + 1] << 8 | input[i + 2] << 16 | input[i + 3] << 24);
                num2++;
            }
        }

        public void Clear()
        {
            this.Dispose(true);
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            return this.ComputeHash(buffer, 0, buffer.Length);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            this.Initialize();
            this.HashCore(buffer, offset, count);
            this._hashValue = this.HashFinal();
            return (byte[])this._hashValue.Clone();
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            this.Initialize();
            var input = new byte[4096];
            int num;
            while (0 < (num = inputStream.Read(input, 0, 4096)))
            {
                this.HashCore(input, 0, num);
            }
            this._hashValue = this.HashFinal();
            return (byte[])this._hashValue.Clone();
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (inputBuffer == null)
            {
                throw new ArgumentNullException(nameof(inputBuffer));
            }
            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset));
            }
            if (inputCount < 0 || inputCount > inputBuffer.Length)
            {
                throw new ArgumentException("inputCount");
            }
            if (inputBuffer.Length - inputCount < inputOffset)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset));
            }
            if (this.State1 == 0)
            {
                this.Initialize();
                this.State1 = 1;
            }
            this.HashCore(inputBuffer, inputOffset, inputCount);
            if (inputBuffer != outputBuffer || inputOffset != outputOffset)
            {
                Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
            }
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null)
            {
                throw new ArgumentNullException(nameof(inputBuffer));
            }
            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset));
            }
            if (inputCount < 0 || inputCount > inputBuffer.Length)
            {
                throw new ArgumentException("inputCount");
            }
            if (inputBuffer.Length - inputCount < inputOffset)
            {
                throw new ArgumentOutOfRangeException(nameof(inputOffset));
            }
            if (this.State1 == 0)
            {
                this.Initialize();
            }
            this.HashCore(inputBuffer, inputOffset, inputCount);
            this._hashValue = this.HashFinal();
            var array = new byte[inputCount];
            Buffer.BlockCopy(inputBuffer, inputOffset, array, 0, inputCount);
            this.State1 = 0;
            return array;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                this.Initialize();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        static Md5()
        {
            // Note: this type is marked as 'beforefieldinit'.
            var array = new byte[64];
            array[0] = 128;
            _padding = array;
        }
    }
}
