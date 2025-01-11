using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Z3.Utils
{
    [Flags]
    public enum LogLevel
    {
        Log = 1,
        Warning = 2,
        Error = 4,

        All = 7,
    }

    [Serializable]
    public class LogConfig
    {
        [field: SerializeField] public LogLevel LogLevel { get; private set; } = LogLevel.All;
        [field: SerializeField] public List<string> MandatoryTags { get; private set; } = new();
        [field: SerializeField] public List<string> IgnoredTags { get; private set; } = new();
        [field: SerializeField] public List<string> MandatoryNamespaces { get; private set; } = new();
        [field: SerializeField] public List<string> IgnoredNamespaces { get; private set; } = new();
    }

    /// <summary>
    /// Global logger with filters to facilitate problem identification
    /// </summary>
    /// <remarks>
    /// It is recommended in PlayerSettings to set Logging and Warning to None to avoid callstack appearing in builds.
    /// </remarks>
    [CreateAssetMenu(menuName = Z3Path.ScriptableObjects + "Logger", fileName = "New" + nameof(DebugLogger))]
    public class DebugLogger : ScriptableObject
    {
        [SerializeField] private LogConfig editorLogConfig;
        [SerializeField] private LogConfig runtimeLogConfig;

        private static LogConfig logConfig;

        public void Init()
        {
#if UNITY_EDITOR
            logConfig = editorLogConfig;
#else
            logConfig = runtimeLogConfig;
#endif
        }

        public static void Error(object from, object message, Object objectContext = null)
        {
            SendLog(from, message, objectContext, LogLevel.Error, Debug.LogError);
        }

        public static void Warning(object from, object message, Object objectContext = null)
        {
            SendLog(from, message, objectContext, LogLevel.Warning, Debug.LogWarning);
        }

        public static void Log(object from, object message, Object objectContext = null)
        {
            SendLog(from, message, objectContext, LogLevel.Log, Debug.Log);
        }

        private static void SendLog(object from, object message, Object objectContext, LogLevel logLevel, Action<object, Object> method)
        {
            Type type = from?.GetType();

            if (!ShouldLog(type, logLevel))
                return;

            if (!objectContext && from is Object obj)
            {
                objectContext = obj;
            }

            method($"<b>{type?.Name}:</b> {message}", objectContext);
        }

        private static bool ShouldLog(Type type, LogLevel logLevel)
        {
            bool active = logConfig.LogLevel.HasFlag(logLevel);

            if (type == null)
                return active;

            string namespaceTag = type.Namespace;
            string classTag = type.Name;

            if (!active)
            {
                // Check forced namespace
                if (logConfig.MandatoryNamespaces.Any(n => n.StartsWith(namespaceTag)))
                    return true;

                // Check forced tags
                if (logConfig.MandatoryTags.Contains(classTag))
                    return true;

                return false;
            }

            // Check ignored namespace
            if (logConfig.IgnoredNamespaces.Any(n => n.StartsWith(namespaceTag)))
                return false;

            // Check ignored tags
            if (logConfig.IgnoredTags.Contains(classTag))
                return false;

            return true;
        }
    }
}