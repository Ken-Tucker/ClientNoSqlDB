﻿using System;

namespace ClientNoSqlDB
{
  interface ITransactionScope : IDisposable
  {
    IDbTableReader GetReader(DbTable table);
    IDbTableWriter GetWriter(DbTable table, bool autoReload = true);
    void AddRef();
    void Modified(DbTable table, bool crop = false);
  }
}
