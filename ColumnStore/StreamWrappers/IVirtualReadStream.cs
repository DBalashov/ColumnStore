using System;

namespace ColumnStore;

interface IVirtualReadStream : IDisposable
{
    byte[] GetBytes();
}