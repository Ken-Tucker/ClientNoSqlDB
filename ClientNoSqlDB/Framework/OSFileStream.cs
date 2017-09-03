
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ClientNoSqlDB
{

  [Flags]
  enum FileOptions : uint
  {
    /// <summary>
    /// None attribute.
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// Read only attribute.
    /// </summary>
    ReadOnly = 0x00000001,

    /// <summary>
    /// Hidden attribute.
    /// </summary>
    Hidden = 0x00000002,

    /// <summary>
    /// System attribute.
    /// </summary>
    System = 0x00000004,

    /// <summary>
    /// Directory attribute.
    /// </summary>
    Directory = 0x00000010,

    /// <summary>
    /// Archive attribute.
    /// </summary>
    Archive = 0x00000020,

    /// <summary>
    /// Device attribute.
    /// </summary>
    Device = 0x00000040,

    /// <summary>
    /// Normal attribute.
    /// </summary>
    Normal = 0x00000080,

    /// <summary>
    /// Temporary attribute.
    /// </summary>
    Temporary = 0x00000100,

    /// <summary>
    /// Sparse file attribute.
    /// </summary>
    SparseFile = 0x00000200,

    /// <summary>
    /// ReparsePoint attribute.
    /// </summary>
    ReparsePoint = 0x00000400,

    /// <summary>
    /// Compressed attribute.
    /// </summary>
    Compressed = 0x00000800,

    /// <summary>
    /// Offline attribute.
    /// </summary>
    Offline = 0x00001000,

    /// <summary>
    /// Not content indexed attribute.
    /// </summary>
    NotContentIndexed = 0x00002000,

    /// <summary>
    /// Encrypted attribute.
    /// </summary>
    Encrypted = 0x00004000,

    /// <summary>
    /// Write through attribute.
    /// </summary>
    Write_Through = 0x80000000,

    /// <summary>
    /// Overlapped attribute.
    /// </summary>
    Overlapped = 0x40000000,

    /// <summary>
    /// No buffering attribute.
    /// </summary>
    NoBuffering = 0x20000000,

    /// <summary>
    /// Random access attribute.
    /// </summary>
    RandomAccess = 0x10000000,

    /// <summary>
    /// Sequential scan attribute.
    /// </summary>
    SequentialScan = 0x08000000,

    /// <summary>
    /// Delete on close attribute.
    /// </summary>
    DeleteOnClose = 0x04000000,

    /// <summary>
    /// Backup semantics attribute.
    /// </summary>
    BackupSemantics = 0x02000000,

    /// <summary>
    /// Post semantics attribute.
    /// </summary>
    PosixSemantics = 0x01000000,

    /// <summary>
    /// Open reparse point attribute.
    /// </summary>
    OpenReparsePoint = 0x00200000,

    /// <summary>
    /// Open no recall attribute.
    /// </summary>
    OpenNoRecall = 0x00100000,

    /// <summary>
    /// First pipe instance attribute.
    /// </summary>
    FirstPipeInstance = 0x00080000
  }



  class OSFileStream : Stream
  {
    IntPtr _handle;
    bool _canRead, _canWrite, _canSeek;
    long _position;

    public OSFileStream(string fileName, FileMode fileMode, FileAccess access, FileShare share = FileShare.Read)
    {

      _handle = WinApi.CreateFile(fileName, access, share, IntPtr.Zero, fileMode, FileOptions.None, IntPtr.Zero);

      if (_handle == new IntPtr(-1))
        throw new IOException(string.Format(CultureInfo.InvariantCulture, "Unable to open file {0}", fileName), Marshal.GetLastWin32Error());

      _canRead = 0 != (access & FileAccess.Read);
      _canWrite = 0 != (access & FileAccess.Write);
      _canSeek = true;
    }

    public override void Flush()
    {
      if (!WinApi.FlushFileBuffers(_handle))
        throw new IOException("Unable to flush stream", Marshal.GetLastWin32Error());
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long newPosition;

      if (!WinApi.SetFilePointerEx(_handle, offset, out newPosition, origin))
        throw new IOException("Unable to seek to this position", Marshal.GetLastWin32Error());

      _position = newPosition;
      return _position;
    }

    public override void SetLength(long value)
    {
      long newPosition;

      if (!WinApi.SetFilePointerEx(_handle, value, out newPosition, SeekOrigin.Begin))
        throw new IOException("Unable to seek to this position", Marshal.GetLastWin32Error());

      if (!WinApi.SetEndOfFile(_handle))
        throw new IOException("Unable to set the new length", Marshal.GetLastWin32Error());

      if (_position < value)
      {
        Seek(_position, SeekOrigin.Begin);
      }
      else
      {
        Seek(0, SeekOrigin.End);
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      unsafe
      {
        fixed (void* pbuffer = buffer)
          return Read((IntPtr)pbuffer, offset, count);
      }
    }

    public int Read(IntPtr buffer, int offset, int count)
    {
      if (buffer == IntPtr.Zero)
        throw new ArgumentNullException("buffer");

      int numberOfBytesRead;
      unsafe
      {
        void* pbuffer = (byte*)buffer + offset;
        {
          if (!WinApi.ReadFile(_handle, (IntPtr)pbuffer, count, out numberOfBytesRead, IntPtr.Zero))
            throw new IOException("Unable to read from file", Marshal.GetLastWin32Error());
        }
        _position += numberOfBytesRead;
      }
      return numberOfBytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException("buffer");

      unsafe
      {
        fixed (void* pbuffer = buffer)
          Write((IntPtr)pbuffer, offset, count);
      }
    }

    public void Write(IntPtr buffer, int offset, int count)
    {
      if (buffer == IntPtr.Zero)
        throw new ArgumentNullException("buffer");

      int numberOfBytesWritten;
      unsafe
      {
        void* pbuffer = (byte*)buffer + offset;
        {
          if (!WinApi.WriteFile(_handle, (IntPtr)pbuffer, count, out numberOfBytesWritten, IntPtr.Zero))
            throw new IOException("Unable to write to file", Marshal.GetLastWin32Error());
        }
        _position += numberOfBytesWritten;
      }
    }

    public override bool CanRead
    {
      get
      {
        return _canRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return _canSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return _canWrite;
      }
    }

    public override long Length
    {
      get
      {
        long length;

        if (!WinApi.GetFileSizeEx(_handle, out length))
          throw new IOException("Unable to get file length", Marshal.GetLastWin32Error());

        return length;
      }
    }

    public override long Position
    {
      get
      {
        return _position;
      }
      set
      {
        Seek(value, SeekOrigin.Begin);
        _position = value;
      }
    }

    protected override void Dispose(bool disposing)
    {
      WinApi.CloseHandle(_handle);
      _handle = IntPtr.Zero;
      base.Dispose(disposing);
    }
  }

  static class OSFile
  {
    public static byte[] ReadAllBytes(string path)
    {
      using (var stream = new OSFileStream(path, FileMode.Open, FileAccess.Read))
      {
        var offset = 0;
        var length = stream.Length;

        if (length > int.MaxValue)
          throw new IOException("File too long");

        var count = (int)length;
        var buffer = new byte[count];

        while (count > 0)
        {
          int read = stream.Read(buffer, offset, count);
          if (read == 0)
            throw new EndOfStreamException();

          offset += read;
          count -= read;
        }

        return buffer;
      }
    }

    public static string ReadAllText(string path)
    {
      return ReadAllText(path, Encoding.UTF8);
    }

    public static string ReadAllText(string path, Encoding encoding)
    {
      using (var stream = new OSFileStream(path, FileMode.Open, FileAccess.Read))
      using (var reader = new StreamReader(stream, encoding, true, 0x400))
        return reader.ReadToEnd();
    }


  }



  static class WinApi
  {

    const string WinBaseDll = "Kernel32.dll";
    const string FileApiDll = WinBaseDll;
    const string HandleApiDll = WinBaseDll;




    [DllImport(FileApiDll, SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern IntPtr CreateFile(string fileName, FileAccess desiredAccess, FileShare shareMode, IntPtr securityAttributes, FileMode mode, FileOptions flagsAndOptions, IntPtr templateFile);


    [DllImport(HandleApiDll, SetLastError = true)]
    public static extern bool CloseHandle(IntPtr handle);


    [DllImport(FileApiDll, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool ReadFile(IntPtr fileHandle, IntPtr buffer, int numberOfBytesToRead, out int numberOfBytesRead, IntPtr overlapped);


    [DllImport(FileApiDll, SetLastError = true)]
    public static extern bool FlushFileBuffers(IntPtr hFile);


    [DllImport(FileApiDll, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool WriteFile(IntPtr fileHandle, IntPtr buffer, int numberOfBytesToRead, out int numberOfBytesRead, IntPtr overlapped);


    [DllImport(FileApiDll, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetFilePointerEx(IntPtr handle, long distanceToMove, out long distanceToMoveHigh, SeekOrigin seekOrigin);


    [DllImport(FileApiDll, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetEndOfFile(IntPtr handle);


    [DllImport(FileApiDll, EntryPoint = "GetFileSizeEx", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool GetFileSizeEx(IntPtr handle, out long fileSize);

  }
}
