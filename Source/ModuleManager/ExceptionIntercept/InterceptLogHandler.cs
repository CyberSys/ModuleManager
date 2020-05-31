﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModuleManager.UnityLogHandle
{
    class InterceptLogHandler : ILogHandler
    {
        private readonly ILogHandler baseLogHandler;
        private readonly List<Assembly> brokenAssemblies = new List<Assembly>();
        private readonly int gamePathLength;

        public static string Warnings { get; private set; } = "";

        public InterceptLogHandler()
        {
            baseLogHandler = Debug.logger.logHandler;
            Debug.logger.logHandler = this;
            gamePathLength = Path.GetFullPath(KSPUtil.ApplicationRootPath).Length;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            baseLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            try
            {
                baseLogHandler.LogException(exception, context);
            }
            catch (Exception e)
            {
                Logging.ModLogger.LOG.trace("**GOTCHA** {0}", e.GetType().ToString());
                if (null != context)
                    Logging.ModLogger.LOG.trace("context type {0}, toString {1}", context.GetType(), context.ToString());
                Logging.ModLogger.LOG.error(this, exception);
                Logging.ModLogger.LOG.error(this, e);
                return;
            }

            if (exception is ReflectionTypeLoadException ex)
            {
                string message = "Intercepted a ReflectionTypeLoadException. List of broken DLLs:\n";
                try
                {
                    var assemblies = ex.Types.Where(x => x != null).Select(x => x.Assembly).Distinct();
                    foreach (Assembly assembly in assemblies)
                    {
                        if (string.IsNullOrEmpty(Warnings))
                        {
                            Warnings = "Add'On(s) DLL that have failed to be dynamically linked on loading\n";
                        }
                        string modInfo = assembly.GetName().Name + " " + assembly.GetName().Version + " " +
                                         assembly.Location.Remove(0, gamePathLength) + "\n";
                        if (!brokenAssemblies.Contains(assembly))
                        {
                            brokenAssemblies.Add(assembly);
                            Warnings += modInfo;
                        }
                        message += modInfo;
                    }
                }
                catch (Exception e)
                {
                    message += "Exception " + e.GetType().Name + " while handling the exception...";
                }
                Logging.ModLogger.LOG.error("**FATAL** {0}", message);
                GUI.ShowStopperAlertBox.Show(message);
            }
        }
    }
}
