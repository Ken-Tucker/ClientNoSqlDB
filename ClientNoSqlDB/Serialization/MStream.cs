using System.IO;

namespace ClientNoSqlDB
{
  sealed class MStream : MemoryStream
  {
    public MStream() { }

    public MStream(int capacity) : base(capacity) { }

    public MStream(byte[] data) : base(data) { }

    public MStream(byte[] data, bool writable) : base(data, writable) { }


    public byte[] GetBuffer() 
    {
      return ToArray();
    }

  }
}
