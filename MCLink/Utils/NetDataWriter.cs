using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MCLink.Interfaces;

//Imported from https://github.com/RevenantX/LiteNetLib/blob/master/LiteNetLib/Utils/NetDataWriter.cs. Modified for use with MCLink

namespace MCLink.Utils
{
    public class NetDataWriter
    {
        private byte[] _data;
        private int _position;
        private const int InitialSize = 64;
        private readonly bool _autoResize;

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data.Length;
        }
        public byte[] Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data;
        }
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }
        
        public ReadOnlySpan<byte> AsReadOnlySpan()
        {
            return new ReadOnlySpan<byte>(_data, 0, _position);
        }

        public static readonly ThreadLocal<UTF8Encoding> Utf8Encoding = new ThreadLocal<UTF8Encoding>(() => new UTF8Encoding(false, true));

        public NetDataWriter() : this(true) {}

        public NetDataWriter(bool autoResize, int initialSize = InitialSize)
        {
            _data = new byte[initialSize];
            _autoResize = autoResize;
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="copy">Copy array to new location or use existing</param>
        public static NetDataWriter FromBytes(byte[] bytes, bool copy)
        {
            if (!copy) return new NetDataWriter(true, 0) { _data = bytes, _position = bytes.Length };
            var netDataWriter = new NetDataWriter(true, bytes.Length);
            netDataWriter.Put(bytes);
            return netDataWriter;
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray (always copied data)
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="offset">Offset of array</param>
        /// <param name="length">Length of array</param>
        public static NetDataWriter FromBytes(byte[] bytes, int offset, int length)
        {
            var netDataWriter = new NetDataWriter(true, bytes.Length);
            netDataWriter.Put(bytes, offset, length);
            return netDataWriter;
        }

        public static NetDataWriter FromString(string value)
        {
            var netDataWriter = new NetDataWriter();
            netDataWriter.Put(value);
            return netDataWriter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeIfNeed(int newSize)
        {
            if (_data.Length < newSize)
            {
                Array.Resize(ref _data, Math.Max(newSize, _data.Length * 2));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureFit(int additionalSize)
        {
            if (_data.Length < _position + additionalSize)
            {
                Array.Resize(ref _data, Math.Max(_position + additionalSize, _data.Length * 2));
            }
        }

        public void Reset(int size)
        {
            ResizeIfNeed(size);
            _position = 0;
        }

        public void Reset()
        {
            _position = 0;
        }

        public byte[] CopyData()
        {
            var resultData = new byte[_position];
            Buffer.BlockCopy(_data, 0, resultData, 0, _position);
            return resultData;
        }

        /// <summary>
        /// Sets position of NetDataWriter to rewrite previous values
        /// </summary>
        /// <param name="position">new byte position</param>
        /// <returns>previous position of data writer</returns>
        public int SetPosition(int position)
        {
            var prevPosition = _position;
            _position = position;
            return prevPosition;
        }

        public void Put(float value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(double value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(long value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(ulong value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(int value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(uint value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(char value)
        {
            Put((ushort)value);
        }

        public void Put(ushort value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(short value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(sbyte value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = (byte)value;
            _position++;
        }

        public void Put(byte value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = value;
            _position++;
        }

        public void Put(Guid value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 16);
            value.TryWriteBytes(_data.AsSpan(_position));
            _position += 16;
        }

        public void Put(byte[] data, int offset, int length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        public void Put(byte[] data)
        {
            if (_autoResize)
                ResizeIfNeed(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }

        public void PutSBytesWithLength(sbyte[] data, int offset, ushort length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2 + length);
            FastBitConverter.GetBytes(_data, _position, length);
            Buffer.BlockCopy(data, offset, _data, _position + 2, length);
            _position += 2 + length;
        }

        public void PutSBytesWithLength(sbyte[] data)
        {
            PutArray(data, 1);
        }

        public void PutBytesWithLength(byte[] data, int offset, ushort length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2 + length);
            FastBitConverter.GetBytes(_data, _position, length);
            Buffer.BlockCopy(data, offset, _data, _position + 2, length);
            _position += 2 + length;
        }

        public void PutBytesWithLength(byte[] data)
        {
            PutArray(data, 1);
        }

        public void Put(bool value)
        {
            Put((byte)(value ? 1 : 0));
        }

        public void PutArray(Array arr, int sz)
        {
            var length = (ushort)arr.Length;
            sz *= length;
            if (_autoResize)
                ResizeIfNeed(_position + sz + 2);
            FastBitConverter.GetBytes(_data, _position, length);
            Buffer.BlockCopy(arr, 0, _data, _position + 2, sz);
            _position += sz + 2;
        }

        public void PutArray(float[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(double[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(long[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(ulong[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(int[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(uint[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(ushort[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(short[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(bool[] value)
        {
            PutArray(value, 1);
        }

        public void PutArray(string[] value)
        {
            var strArrayLength = (ushort)value.Length;
            Put(strArrayLength);
            for (var i = 0; i < strArrayLength; i++)
                Put(value[i]);
        }

        public void PutArray(string[] value, int strMaxLength)
        {
            var strArrayLength = (ushort)value.Length;
            Put(strArrayLength);
            for (var i = 0; i < strArrayLength; i++)
                Put(value[i], strMaxLength);
        }

        public void PutArray<T>(T[] value) where T : IMcLinkSerializable, new()
        {
            var strArrayLength = (ushort)value.Length;
            Put(strArrayLength);
            for (var i = 0; i < strArrayLength; i++)
                value[i].Serialize(this);
        }

        public void Put(IPEndPoint endPoint)
        {
            Put(endPoint.Address.ToString());
            Put(endPoint.Port);
        }

        public void PutLargeString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }
            var size = Utf8Encoding.Value.GetByteCount(value);
            if (size == 0)
            {
                Put(0);
                return;
            }
            Put(size);
            if (_autoResize)
                ResizeIfNeed(_position + size);
            Utf8Encoding.Value.GetBytes(value, 0, size, _data, _position);
            _position += size;
        }

        public void Put(string value)
        {
            Put(value, 0);
        }

        /// <summary>
        /// Note that "maxLength" only limits the number of characters in a string, not its size in bytes.
        /// </summary>
        public void Put(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                Put((ushort)0);
                return;
            }

            var length = maxLength > 0 && value.Length > maxLength ? maxLength : value.Length;
            var maxSize = Utf8Encoding.Value.GetMaxByteCount(length);
            if (_autoResize)
                ResizeIfNeed(_position + maxSize + sizeof(ushort));
            var size = Utf8Encoding.Value.GetBytes(value, 0, length, _data, _position + sizeof(ushort));
            if (size == 0)
            {
                Put((ushort)0);
                return;
            }
            Put(checked((ushort)(size + 1)));
            _position += size;
        }

        public void Put<T>(T obj) where T : IMcLinkSerializable
        {
            obj.Serialize(this);
        }
    }
}