using System;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJH.Utility
{
    public static class Debug
    {
        private static string BuildPrefix(int? prefixNumber, string tag, bool showTimestamp)
        {
            string ts = showTimestamp ? $"[{System.DateTime.Now:HH:mm:ss}] " : "";
            string t = string.IsNullOrEmpty(tag) ? "" : $"[{tag}] ";
            string n = prefixNumber.HasValue ? $"{prefixNumber.Value}. " : "";
            return ts + t + n;
        }

        #region Properties

        public static ILogger logger => UnityEngine.Debug.unityLogger;
        public static ILogger unityLogger => UnityEngine.Debug.unityLogger;

        public static bool developerConsoleVisible
        {
            get => UnityEngine.Debug.developerConsoleVisible;
            set => UnityEngine.Debug.developerConsoleVisible = value;
        }

        public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

        #endregion

        #region Mark

        /// <summary> 메소드 호출 전파 추적용 메소드 </summary>
        [Conditional("UNITY_EDITOR")]
        public static void Mark(
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0
        )
        {
            int begin = sourceFilePath.LastIndexOf(@"\");
            int end = sourceFilePath.LastIndexOf(@".cs");
            string className = sourceFilePath.Substring(begin + 1, end - begin - 1);

            UnityEngine.Debug.Log($"[Mark] {className}.{memberName}, {sourceLineNumber}");
        }


#if UNITY_EDITOR
	 / / Handle the callback function opened by the asset
	[UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
	static bool OnOpenAsset(int instance, int line)
	{
		 // Custom function, used to get the stacktrace in the log, defined later.
		string stack_trace = GetStackTrace();
		 // Use stacktrace to locate whether it is our custom log. My log has special text [FoxLog], which is well recognized.
		 If (!string.IsNullOrEmpty(stack_trace)) // can customize the label to be added here; the original code is confusing and does not need to be modified, you need to locate it yourself;
		{
			string strLower = stack_trace.ToLower();
			if (strLower.Contains("[foxlog]"))
			{
				Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
				string pathline = "";
				if (matches.Success)
				{
					pathline = matches.Groups[1].Value;
					 Matches = matches.NextMatch(); // Raise another layer up to enter;
					if (matches.Success)
					{
						pathline = matches.Groups[1].Value;
						pathline = pathline.Replace(" ", "");

						int split_index = pathline.LastIndexOf(":");
						string path = pathline.Substring(0, split_index);
						line = Convert.ToInt32(pathline.Substring(split_index + 1));
						string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
						fullpath = fullpath + path;
						string strPath = fullpath.Replace('/', '\\');
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(strPath, line);
					}
					else
					{
						Debug.LogError("DebugCodeLocation OnOpenAsset, Error StackTrace");
					}

					matches = matches.NextMatch();
				}
				return true;
			}
		}
		return false;
	}

	static string GetStackTrace()
	{
		 // Find the assembly of UnityEditor.EditorWindow
		var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
		if (assembly_unity_editor == null) return null;

		 // Find the class UnityEditor.ConsoleWindow
		var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
		if (type_console_window == null) return null;
		 // Find the member ms_ConsoleWindow in UnityEditor.ConsoleWindow
		var field_console_window =
 type_console_window.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		if (field_console_window == null) return null;
		 / / Get the value of ms_ConsoleWindow
		var instance_console_window = field_console_window.GetValue(null);
		if (instance_console_window == null) return null;

		 / / If the focus window of the console window, get the stacktrace
		if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
		{
			 / / Get the class ListViewState through the assembly
			var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
			if (type_list_view_state == null) return null;

			 / / Find the member m_ListView in the class UnityEditor.ConsoleWindow
			var field_list_view =
 type_console_window.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			if (field_list_view == null) return null;

			 / / Get the value of m_ListView
			var value_list_view = field_list_view.GetValue(instance_console_window);
			if (value_list_view == null) return null;

			 // Find the member m_ActiveText in the class UnityEditor.ConsoleWindow
			var field_active_text =
 type_console_window.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			if (field_active_text == null) return null;

			 / / Get the value of m_ActiveText, is the stacktrace we need
			string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
			return value_active_text;
		}

		return null;
	}
}
#endif

        #endregion

        #region Assert

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message, Object context)
            => UnityEngine.Debug.Assert(condition, message, context);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
            => UnityEngine.Debug.Assert(condition);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message, Object context)
            => UnityEngine.Debug.Assert(condition, message, context);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
            => UnityEngine.Debug.Assert(condition, message);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message)
            => UnityEngine.Debug.Assert(condition, message);

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, Object context)
            => UnityEngine.Debug.Assert(condition, context);


        [Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
            => UnityEngine.Debug.AssertFormat(condition, context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, string format, params object[] args)
            => UnityEngine.Debug.AssertFormat(condition, format, args);

        #endregion

        #region Log

        [Conditional("UNITY_EDITOR")]
        public static void Log(object message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"{prefix}{message}";
            UnityEngine.Debug.Log(output);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"{prefix}{message}";
            UnityEngine.Debug.Log(output, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(string format, params object[] args)
            => UnityEngine.Debug.LogFormat(format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format,
            params object[] args)
            => UnityEngine.Debug.LogFormat(logType, logOptions, context, format, args);

        #endregion

        #region LogColor

        [Conditional("UNITY_EDITOR")]
        public static void LogColor(object message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.Log(output);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColor(object message, Object context, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.Log(output, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorPart(string message, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + message;

            foreach (var (part, color) in colorInfos)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string coloredPart = $"<color=#{colorHex}>{part}</color>";
                fullMessage = fullMessage.Replace(part, coloredPart);
            }

            UnityEngine.Debug.Log(fullMessage);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorPart(string message, Object context, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + message;

            foreach (var (part, color) in colorInfos)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string coloredPart = $"<color=#{colorHex}>{part}</color>";
                fullMessage = fullMessage.Replace(part, coloredPart);
            }

            UnityEngine.Debug.Log(fullMessage, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorFormat(string format, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogFormat(output, args);
        }

        public static void LogColorFormat(Object context, string format, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogFormat(context, output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorFormat(LogType logType, LogOption logOptions, Object context, string format,
            Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogFormat(logType, logOptions, context, output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorFormatPart(string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false,
            (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string message = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    message = message.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogFormat(message, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorFormatPart(Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false,
            (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string message = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    message = message.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogFormat(context, message, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorFormatPart(LogType logType, LogOption logOptions, Object context, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false,
            (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string message = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    message = message.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogFormat(logType, logOptions, context, message, args);
        }

        #endregion

        #region LogAssertion

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message, Object context)
            => UnityEngine.Debug.LogAssertion(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message)
            => UnityEngine.Debug.LogAssertion(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogAssertionFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(string format, params object[] args)
            => UnityEngine.Debug.LogAssertionFormat(format, args);

        #endregion

        #region LogColorAssertion

        [Conditional("UNITY_EDITOR")]
        public static void LogColorAssertion(object message, Object context, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.LogAssertion(output, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorAssertion(object message, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.LogAssertion(output);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorAssertionParts(string message, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + message;

            foreach (var (part, color) in colorInfos)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string coloredPart = $"<color=#{colorHex}>{part}</color>";
                fullMessage = fullMessage.Replace(part, coloredPart);
            }

            UnityEngine.Debug.LogAssertion(fullMessage);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorAssertionParts(Object context, string message, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + message;

            foreach (var (part, color) in colorInfos)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string coloredPart = $"<color=#{colorHex}>{part}</color>";
                fullMessage = fullMessage.Replace(part, coloredPart);
            }

            UnityEngine.Debug.LogAssertion(fullMessage, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormatColor(Object context, string format, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogAssertionFormat(context, output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormatColor(string format, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogAssertionFormat(output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormatColorParts(Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false,
            (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + string.Format(format, args);

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    fullMessage = fullMessage.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogAssertion(fullMessage, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormatColorParts(string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false,
            (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + string.Format(format, args);

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    fullMessage = fullMessage.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogAssertion(fullMessage);
        }

        #endregion

        #region LogWarning

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message, Object context)
            => UnityEngine.Debug.LogWarning(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
            => UnityEngine.Debug.LogWarning(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogWarningFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(string format, params object[] args)
            => UnityEngine.Debug.LogWarningFormat(format, args);

        #endregion

        #region LogColorWarning

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarning(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.LogWarning(output);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarning(string message, Object context, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.LogWarning(output, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarningPart(string message, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    prefix = prefix.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogWarning(prefix);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarningPart(string message, Object context, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    prefix = prefix.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogWarning(prefix, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarningFormat(string format, Object context, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogWarningFormat(context, output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarningFormat(string format, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogWarningFormat(output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarningFormatPart(string format, Object context, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    output = output.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogWarningFormat(context, output);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorWarningFormatPart(string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    output = output.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogWarningFormat(output);
        }

        #endregion

        #region LogError

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message, Object context)
            => UnityEngine.Debug.LogError(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
            => UnityEngine.Debug.LogError(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(Object context, string format, params object[] args)
            => UnityEngine.Debug.LogErrorFormat(context, format, args);

        [Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(string format, params object[] args)
            => UnityEngine.Debug.LogErrorFormat(format, args);

        #endregion

        #region LogColorError

        [Conditional("UNITY_EDITOR")]
        public static void LogColorError(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.LogError(output);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorError(string message, Object context, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{message}</color>";
            UnityEngine.Debug.LogError(output, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorErrorFormat(string format, Object context, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogErrorFormat(context, output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorErrorFormat(string format, Color color, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string output = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{prefix}{format}</color>";
            UnityEngine.Debug.LogErrorFormat(output, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorErrorPart(string message, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + message;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    fullMessage = fullMessage.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogError(fullMessage);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorErrorPart(string message, Object context, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, params (string part, Color color)[] colorInfos)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + message;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    fullMessage = fullMessage.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogError(fullMessage, context);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorErrorFormatPart(string format, Object context, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    fullMessage = fullMessage.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogErrorFormat(context, fullMessage, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogColorErrorFormatPart(string format, int? prefixNumber = null,
            string tag = null,
            bool showTimestamp = false, (string part, Color color)[] colorInfos = null, params object[] args)
        {
            string prefix = BuildPrefix(prefixNumber, tag, showTimestamp);
            string fullMessage = prefix + format;

            if (colorInfos != null)
            {
                foreach (var (part, color) in colorInfos)
                {
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    string coloredPart = $"<color=#{colorHex}>{part}</color>";
                    fullMessage = fullMessage.Replace(part, coloredPart);
                }
            }

            UnityEngine.Debug.LogErrorFormat(fullMessage, args);
        }

        #endregion

        #region LogException

        [Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception)
            => UnityEngine.Debug.LogException(exception);

        [Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception, Object context)
            => UnityEngine.Debug.LogException(exception, context);

        #endregion

        #region DrawLine

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end)
            => UnityEngine.Debug.DrawLine(start, end);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
            => UnityEngine.Debug.DrawLine(start, end, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
            => UnityEngine.Debug.DrawLine(start, end, color, duration);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
            => UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);

        #endregion

        #region DrawRay

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
            => UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
            => UnityEngine.Debug.DrawRay(start, dir, color, duration);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color)
            => UnityEngine.Debug.DrawRay(start, dir, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir)
            => UnityEngine.Debug.DrawRay(start, dir);

        #endregion

        #region Etc

        [Conditional("UNITY_EDITOR")]
        public static void Break()
            => UnityEngine.Debug.Break();

        [Conditional("UNITY_EDITOR")]
        public static void DebugBreak()
            => UnityEngine.Debug.DebugBreak();

        [Conditional("UNITY_EDITOR")]
        public static void ClearDeveloperConsole()
            => UnityEngine.Debug.ClearDeveloperConsole();

        #endregion
    }
}