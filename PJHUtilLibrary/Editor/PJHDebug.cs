using System;
using System.Diagnostics;
using UnityEngine;

namespace PJH.Utility
{
    public struct ReplaceMode
    {
        public ReplaceModeType mode;
        public int nthIndex;

        public ReplaceMode(ReplaceModeType mode, int nth = 1)
        {
            this.mode = mode;
            nthIndex = Math.Max(1, nth);
        }

        public static ReplaceMode All => new ReplaceMode(ReplaceModeType.ReplaceAll);
        public static ReplaceMode First => new ReplaceMode(ReplaceModeType.ReplaceFirst);
        public static ReplaceMode Nth(int n) => new ReplaceMode(ReplaceModeType.ReplaceNth, n);
    }

    public enum ReplaceModeType
    {
        ReplaceAll,
        ReplaceFirst,
        ReplaceNth
    }

    public static class PJHDebug
    {
        private const string MARK_PREFIX = "[Mark]";

        #region 기본 로그

        [Conditional("ENABLE_LOG")]
        public static void Log(object message, UnityEngine.Object context = null, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false)
        {
            UnityEngine.Debug.Log(BuildMessage(message, prefixNumber, tag, showTimestamp), context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogWarning(object message, UnityEngine.Object context = null, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false)
        {
            UnityEngine.Debug.LogWarning(BuildMessage(message, prefixNumber, tag, showTimestamp), context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogError(object message, UnityEngine.Object context = null, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false)
        {
            UnityEngine.Debug.LogError(BuildMessage(message, prefixNumber, tag, showTimestamp), context);
        }

        #endregion

        #region 색상 로그 (부분/전체 메시지 지원)

        [Conditional("ENABLE_LOG")]
        public static void LogColorPart(string message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, params (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            string output = ApplyColors(BuildMessage(message, prefixNumber, tag, showTimestamp), colorInfos);
            UnityEngine.Debug.Log(output);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorWarningPart(string message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, params (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            string output = ApplyColors(BuildMessage(message, prefixNumber, tag, showTimestamp), colorInfos);
            UnityEngine.Debug.LogWarning(output);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorErrorPart(string message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, params (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            string output = ApplyColors(BuildMessage(message, prefixNumber, tag, showTimestamp), colorInfos);
            UnityEngine.Debug.LogError(output);
        }

        // --- 전체 메시지 색상 오버로드 ---
        [Conditional("ENABLE_LOG")]
        public static void LogColorPart(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            LogColorPart(message, prefixNumber, tag, showTimestamp, (message, color, ReplaceMode.All));
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorWarningPart(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            LogColorWarningPart(message, prefixNumber, tag, showTimestamp, (message, color, ReplaceMode.All));
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorErrorPart(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            LogColorErrorPart(message, prefixNumber, tag, showTimestamp, (message, color, ReplaceMode.All));
        }

        #endregion

        #region Format 로그

        [Conditional("ENABLE_LOG")]
        public static void LogFormat(UnityEngine.Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params object[] args)
        {
            string message = string.Format(format, args);
            UnityEngine.Debug.Log(BuildMessage(message, prefixNumber, tag, showTimestamp), context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogWarningFormat(UnityEngine.Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params object[] args)
        {
            string message = string.Format(format, args);
            UnityEngine.Debug.LogWarning(BuildMessage(message, prefixNumber, tag, showTimestamp), context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogErrorFormat(UnityEngine.Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params object[] args)
        {
            string message = string.Format(format, args);
            UnityEngine.Debug.LogError(BuildMessage(message, prefixNumber, tag, showTimestamp), context);
        }

        #endregion

        #region ColorFormat 로그

        [Conditional("ENABLE_LOG")]
        public static void LogColorFormat(UnityEngine.Object context, Color color, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false, params object[] args)
        {
            string message = string.Format(format, args);
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
            UnityEngine.Debug.Log(BuildMessage(coloredMessage, prefixNumber, tag, showTimestamp), context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorWarningFormat(UnityEngine.Object context, Color color, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false, params object[] args)
        {
            string message = string.Format(format, args);
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
            UnityEngine.Debug.LogWarning(BuildMessage(coloredMessage, prefixNumber, tag, showTimestamp), context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorErrorFormat(UnityEngine.Object context, Color color, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false, params object[] args)
        {
            string message = string.Format(format, args);
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
            UnityEngine.Debug.LogError(BuildMessage(coloredMessage, prefixNumber, tag, showTimestamp), context);
        }

        #endregion

        #region Assert

        [Conditional("ENABLE_LOG")]
        public static void Assert(bool condition, string message = "", UnityEngine.Object context = null)
        {
            if (!condition)
            {
                UnityEngine.Debug.Assert(false, BuildMessage(message, null, "Assert", true), context);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
        {
            if (!condition)
            {
                string message = string.Format(format, args);
                UnityEngine.Debug.Assert(false, BuildMessage(message, null, "Assert", true), context);
            }
        }

        #endregion

        #region 내부 유틸

        private static string BuildMessage(object message, int? prefixNumber, string tag, bool showTimestamp)
        {
            string numberPrefix = prefixNumber.HasValue ? $"#{prefixNumber.Value:000} " : "";
            string tagPrefix = !string.IsNullOrEmpty(tag) ? $"[{tag}] " : "";
            string timestamp = showTimestamp ? $"[{DateTime.Now:HH:mm:ss.fff}] " : "";
            return $"{MARK_PREFIX} {numberPrefix}{tagPrefix}{timestamp}{message}";
        }

        private static string ApplyColors(string text, (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            if (string.IsNullOrEmpty(text) || colorInfos == null) return text;

            foreach (var (part, color, mode) in colorInfos)
            {
                if (string.IsNullOrEmpty(part)) continue;
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string coloredPart = $"<color=#{colorHex}>{part}</color>";

                switch (mode.mode)
                {
                    case ReplaceModeType.ReplaceAll:
                        text = text.Replace(part, coloredPart);
                        break;
                    case ReplaceModeType.ReplaceFirst:
                        int index = text.IndexOf(part, StringComparison.Ordinal);
                        if (index >= 0)
                            text = text.Substring(0, index) + coloredPart + text.Substring(index + part.Length);
                        break;
                    case ReplaceModeType.ReplaceNth:
                        int occurrence = 0;
                        int start = 0;
                        while ((index = text.IndexOf(part, start, StringComparison.Ordinal)) >= 0)
                        {
                            occurrence++;
                            if (occurrence == mode.nthIndex)
                            {
                                text = text.Substring(0, index) + coloredPart + text.Substring(index + part.Length);
                                break;
                            }
                            start = index + part.Length;
                        }
                        break;
                }
            }

            return text;
        }

        #endregion
    }
}
