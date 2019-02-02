﻿using System;
using System.IO;
using UnityEngine;

namespace ModuleManager.Logging
{
    public class StreamLogger : IBasicLogger, IDisposable
    {
        private const string DATETIME_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss.fff";

        private readonly Stream stream;
        private readonly StreamWriter streamWriter;
        private bool disposed = false;
        private readonly ModLogger modLogger;

        public StreamLogger(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new ArgumentException("must be writable", nameof(stream));
            streamWriter = new StreamWriter(stream);
            this.modLogger = new ModLogger();
        }

        public void Log(LogType logType, string message)
        {
            this.modLogger.Log(logType, message);
            if (disposed) throw new InvalidOperationException("Object has already been disposed");

            string prefix;
            if (logType == LogType.Log)
                prefix = "LOG";
            else if (logType == LogType.Warning)
                prefix = "WRN";
            else if (logType == LogType.Error)
                prefix = "ERR";
            else if (logType == LogType.Assert)
                prefix = "AST";
            else if (logType == LogType.Exception)
                prefix = "EXC";
            else
                prefix = "UNK";

            streamWriter.WriteLine("[{0} {1}] {2}", prefix, DateTime.Now.ToString(DATETIME_FORMAT_STRING), message);
        }

        public void Exception(string message, Exception exception)
        {
            Log(LogType.Exception, exception?.ToString() ?? "<null exception>");
        }

        public void Dispose()
        {
            // Flushes and closes the StreamWriter and the underlying stream
            streamWriter.Close();

            disposed = true;
        }
    }
}
