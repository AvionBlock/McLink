using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MCLink.Interfaces;

//Imported from https://github.com/RevenantX/LiteNetLib/blob/master/LiteNetLib/Utils/NetDataReader.cs. Modified for use with MCLink

namespace MCLink.Utils
{
    public class NetDataReader
    {
        private byte[]? _data;
        private int _position;
        private int _dataSize;
        private int _offset;

        public byte[] RawData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(_data == null)
                    throw new InvalidOperationException("Data is null!");
                return _data;
            }
        }
        public int RawDataSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dataSize;
        }
        public int UserDataOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _offset;
        }
        public int UserDataSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dataSize - _offset;
        }
        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data == null;
        }
        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }
        public bool EndOfData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position == _dataSize;
        }
        public int AvailableBytes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dataSize - _position;
        }

        public void SkipBytes(int count)
        {
            _position += count;
        }

        public void SetPosition(int position)
        {
            _position = position;
        }

        public void SetSource(NetDataWriter dataWriter)
        {
            _data = dataWriter.Data;
            _position = 0;
            _offset = 0;
            _dataSize = dataWriter.Length;
        }

        public void SetSource(byte[] source)
        {
            _data = source;
            _position = 0;
            _offset = 0;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int maxSize)
        {
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = maxSize;
        }

        public NetDataReader() {}

        public NetDataReader(NetDataWriter writer)
        {
            SetSource(writer);
        }

        public NetDataReader(byte[] source)
        {
            SetSource(source);
        }

        public NetDataReader(byte[] source, int offset, int maxSize)
        {
            SetSource(source, offset, maxSize);
        }

        #region GetMethods

        public void Get<T>(out T result) where T : struct, IMcLinkSerializable
        {
            result = default(T);
            result.Deserialize(this);
        }

        public void Get<T>(out T result, Func<T> constructor) where T : class, IMcLinkSerializable
        {
            result = constructor();
            result.Deserialize(this);
        }

        public void Get(out byte result)
        {
            result = GetByte();
        }

        public void Get(out sbyte result)
        {
            result = (sbyte)GetByte();
        }

        public void Get(out bool result)
        {
            result = GetBool();
        }

        public void Get(out char result)
        {
            result = GetChar();
        }

        public void Get(out ushort result)
        {
            result = GetUShort();
        }

        public void Get(out short result)
        {
            result = GetShort();
        }

        public void Get(out ulong result)
        {
            result = GetULong();
        }

        public void Get(out long result)
        {
            result = GetLong();
        }

        public void Get(out uint result)
        {
            result = GetUInt();
        }

        public void Get(out int result)
        {
            result = GetInt();
        }

        public void Get(out double result)
        {
            result = GetDouble();
        }

        public void Get(out float result)
        {
            result = GetFloat();
        }

        public void Get(out string result)
        {
            result = GetString();
        }

        public void Get(out string result, int maxLength)
        {
            result = GetString(maxLength);
        }
        
        public void Get(out Guid result)
        {
            result = GetGuid();
        }

        public byte GetByte()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var res = _data[_position];
            _position++;
            return res;
        }

        public sbyte GetSByte()
        {
            return (sbyte)GetByte();
        }

        public T[] GetArray<T>(ushort size)
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var length = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var result = new T[length];
            length *= size;
            Buffer.BlockCopy(_data, _position, result, 0, length);
            _position += length;
            return result;
        }

        public T[] GetArray<T>() where T : IMcLinkSerializable, new()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var length = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
                var item = new T();
                item.Deserialize(this);
                result[i] = item;
            }
            return result;
        }
        
        public T[] GetArray<T>(Func<T> constructor) where T : class, IMcLinkSerializable
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var length = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var result = new T[length];
            for (var i = 0; i < length; i++)
                Get(out result[i], constructor);
            return result;
        }
        
        public bool[] GetBoolArray()
        {
            return GetArray<bool>(1);
        }

        public ushort[] GetUShortArray()
        {
            return GetArray<ushort>(2);
        }

        public short[] GetShortArray()
        {
            return GetArray<short>(2);
        }

        public int[] GetIntArray()
        {
            return GetArray<int>(4);
        }

        public uint[] GetUIntArray()
        {
            return GetArray<uint>(4);
        }

        public float[] GetFloatArray()
        {
            return GetArray<float>(4);
        }

        public double[] GetDoubleArray()
        {
            return GetArray<double>(8);
        }

        public long[] GetLongArray()
        {
            return GetArray<long>(8);
        }

        public ulong[] GetULongArray()
        {
            return GetArray<ulong>(8);
        }

        public string[] GetStringArray()
        {
            var length = GetUShort();
            var arr = new string[length];
            for (var i = 0; i < length; i++)
            {
                arr[i] = GetString();
            }
            return arr;
        }

        /// <summary>
        /// Note that "maxStringLength" only limits the number of characters in a string, not its size in bytes.
        /// Strings that exceed this parameter are returned as empty
        /// </summary>
        public string[] GetStringArray(int maxStringLength)
        {
            var length = GetUShort();
            var arr = new string[length];
            for (var i = 0; i < length; i++)
            {
                arr[i] = GetString(maxStringLength);
            }
            return arr;
        }

        public bool GetBool()
        {
            return GetByte() == 1;
        }

        public char GetChar()
        {
            return (char)GetUShort();
        }

        public ushort GetUShort()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }

        public short GetShort()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public long GetLong()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }

        public ulong GetULong()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToUInt64(_data, _position);
            _position += 8;
            return result;
        }

        public int GetInt()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }

        public uint GetUInt()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public float GetFloat()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToSingle(_data, _position);
            _position += 4;
            return result;
        }

        public double GetDouble()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var result = BitConverter.ToDouble(_data, _position);
            _position += 8;
            return result;
        }

        /// <summary>
        /// Note that "maxLength" only limits the number of characters in a string, not its size in bytes.
        /// </summary>
        /// <returns>"string.Empty" if value > "maxLength"</returns>
        public string GetString(int maxLength)
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var size = GetUShort();
            if (size == 0)
                return string.Empty;
            
            var actualSize = size - 1;
            var result = maxLength > 0 && NetDataWriter.Utf8Encoding.Value.GetCharCount(_data, _position, actualSize) > maxLength ?
                string.Empty :
                NetDataWriter.Utf8Encoding.Value.GetString(_data, _position, actualSize);
            _position += actualSize;
            return result;
        }

        public string GetString()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var size = GetUShort();
            if (size == 0)
                return string.Empty;
            
            var actualSize = size - 1;
            var result = NetDataWriter.Utf8Encoding.Value.GetString(_data, _position, actualSize);
            _position += actualSize;
            return result;
        }

        public string GetLargeString()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var size = GetInt();
            if (size <= 0)
                return string.Empty;
            var result = NetDataWriter.Utf8Encoding.Value.GetString(_data, _position, size);
            _position += size;
            return result;
        }
        
        public Guid GetGuid()
        {
            var result =  new Guid(_data.AsSpan(_position, 16));
            _position += 16;
            return result;
        }

        public ArraySegment<byte> GetBytesSegment(int count)
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var segment = new ArraySegment<byte>(_data, _position, count);
            _position += count;
            return segment;
        }

        public ArraySegment<byte> GetRemainingBytesSegment()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var segment = new ArraySegment<byte>(_data, _position, AvailableBytes);
            _position = _data.Length;
            return segment;
        }

        public T Get<T>() where T : struct, IMcLinkSerializable
        {
            var obj = default(T);
            obj.Deserialize(this);
            return obj;
        }

        public T Get<T>(Func<T> constructor) where T : class, IMcLinkSerializable
        {
            var obj = constructor();
            obj.Deserialize(this);
            return obj;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> GetRemainingBytesSpan()
        {
            return new ReadOnlySpan<byte>(_data, _position, _dataSize - _position);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> GetRemainingBytesMemory()
        {
            return new ReadOnlyMemory<byte>(_data, _position, _dataSize - _position);
        }

        public byte[] GetRemainingBytes()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var outgoingData = new byte[AvailableBytes];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, AvailableBytes);
            _position = _data.Length;
            return outgoingData;
        }

        public void GetBytes(byte[] destination, int start, int count)
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            Buffer.BlockCopy(_data, _position, destination, start, count);
            _position += count;
        }

        public void GetBytes(byte[] destination, int count)
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            Buffer.BlockCopy(_data, _position, destination, 0, count);
            _position += count;
        }

        public sbyte[] GetSBytesWithLength()
        {
            return GetArray<sbyte>(1);
        }

        public byte[] GetBytesWithLength()
        {
            return GetArray<byte>(1);
        }
        #endregion

        #region PeekMethods

        public byte PeekByte()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return _data[_position];
        }

        public sbyte PeekSByte()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return (sbyte)_data[_position];
        }

        public bool PeekBool()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return _data[_position] == 1;
        }

        public char PeekChar()
        {
            return (char)PeekUShort();
        }

        public ushort PeekUShort()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToUInt16(_data, _position);
        }

        public short PeekShort()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToInt16(_data, _position);
        }

        public long PeekLong()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToInt64(_data, _position);
        }

        public ulong PeekULong()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToUInt64(_data, _position);
        }

        public int PeekInt()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToInt32(_data, _position);
        }

        public uint PeekUInt()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToUInt32(_data, _position);
        }

        public float PeekFloat()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToSingle(_data, _position);
        }

        public double PeekDouble()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            return BitConverter.ToDouble(_data, _position);
        }

        /// <summary>
        /// Note that "maxLength" only limits the number of characters in a string, not its size in bytes.
        /// </summary>
        public string PeekString(int maxLength)
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var size = PeekUShort();
            if (size == 0)
                return string.Empty;
            
            var actualSize = size - 1;
            return (maxLength > 0 && NetDataWriter.Utf8Encoding.Value.GetCharCount(_data, _position + 2, actualSize) > maxLength) ?
                string.Empty :
                NetDataWriter.Utf8Encoding.Value.GetString(_data, _position + 2, actualSize);
        }

        public string PeekString()
        {
            if(_data == null)
                throw new InvalidOperationException("Data is null!");
            
            var size = PeekUShort();
            if (size == 0)
                return string.Empty;

            var actualSize = size - 1;
            return NetDataWriter.Utf8Encoding.Value.GetString(_data, _position + 2, actualSize);
        }
        #endregion

        #region TryGetMethods
        public bool TryGetByte(out byte result)
        {
            if (AvailableBytes >= 1)
            {
                result = GetByte();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetSByte(out sbyte result)
        {
            if (AvailableBytes >= 1)
            {
                result = GetSByte();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetBool(out bool result)
        {
            if (AvailableBytes >= 1)
            {
                result = GetBool();
                return true;
            }
            result = false;
            return false;
        }

        public bool TryGetChar(out char result)
        {
            if (!TryGetUShort(out var uShortValue))
            {
                result = '\0';
                return false;
            }
            result = (char)uShortValue;
            return true;
        }

        public bool TryGetShort(out short result)
        {
            if (AvailableBytes >= 2)
            {
                result = GetShort();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetUShort(out ushort result)
        {
            if (AvailableBytes >= 2)
            {
                result = GetUShort();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetInt(out int result)
        {
            if (AvailableBytes >= 4)
            {
                result = GetInt();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetUInt(out uint result)
        {
            if (AvailableBytes >= 4)
            {
                result = GetUInt();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetLong(out long result)
        {
            if (AvailableBytes >= 8)
            {
                result = GetLong();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetULong(out ulong result)
        {
            if (AvailableBytes >= 8)
            {
                result = GetULong();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetFloat(out float result)
        {
            if (AvailableBytes >= 4)
            {
                result = GetFloat();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetDouble(out double result)
        {
            if (AvailableBytes >= 8)
            {
                result = GetDouble();
                return true;
            }
            result = 0;
            return false;
        }
        
        public bool TryGetString([NotNullWhen(true)] out string? result)
        {
            if (AvailableBytes >= 2)
            {
                var strSize = PeekUShort();
                if (AvailableBytes >= strSize + 1)
                {
                    result = GetString();
                    return true;
                }
            }
            result = null;
            return false;
        }

        public bool TryGetStringArray(out string[]? result)
        {
            if (!TryGetUShort(out var strArrayLength)) {
                result = null;
                return false;
            }

            result = new string[strArrayLength];
            for (var i = 0; i < strArrayLength; i++)
            {
                if (TryGetString(out result[i])) continue;
                result = null;
                return false;
            }

            return true;
        }

        public bool TryGetBytesWithLength(out byte[]? result)
        {
            var length = PeekUShort();
            if (AvailableBytes >= 2)
            {
                if (AvailableBytes >= 2 + length)
                {
                    result = GetBytesWithLength();
                    return true;
                }
            }
            result = null;
            return false;
        }
        #endregion

        public void Clear()
        {
            _position = 0;
            _dataSize = 0;
            _data = null;
        }
    }
}