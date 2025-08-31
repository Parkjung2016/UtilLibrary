﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PJH.Utility.Editor
{
    [InitializeOnLoad]
    public static class PJHDebugToggle
    {
        private const string SYMBOL = "ENABLE_LOG";
        private const string MENU_PATH = "PJH/Toggle Debug Logs";

        static PJHDebugToggle()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);
            UpdateMenuCheck(); // 에디터 로드 시 체크 상태 갱신
        }

        private static void OnBuildPlayer(BuildPlayerOptions options)
        {
            bool enableLogs = EditorUtility.DisplayDialog(
                "Enable Debug Logs",
                "빌드에서 PJHDebug 로그를 활성화하시겠습니까?",
                "Yes",
                "No"
            );

            SetSymbolEnabled(enableLogs);

            AssetDatabase.Refresh();

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == BuildResult.Succeeded)
                Debug.Log("빌드 성공");
            else
                Debug.LogError("빌드 실패");
        }

        [MenuItem(MENU_PATH)]
        private static void ToggleLogs()
        {
            bool isEnabled = IsSymbolEnabled();
            SetSymbolEnabled(!isEnabled);
            Debug.Log($"PJHDebug: Logs {(isEnabled ? "disabled" : "enabled")}");
            AssetDatabase.Refresh();
        }

        [MenuItem(MENU_PATH, true)]
        private static bool ToggleLogsValidate()
        {
            UpdateMenuCheck();
            return true;
        }

        private static bool IsSymbolEnabled()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            string[] symbolArray = symbols.Split(';');
            return System.Array.Exists(symbolArray, s => s == SYMBOL);
        }

        private static void SetSymbolEnabled(bool enabled)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            string[] symbolArray = symbols.Split(';');

            if (enabled && !IsSymbolEnabled())
                symbols = string.Join(";", symbolArray) + ";" + SYMBOL;
            else if (!enabled && IsSymbolEnabled())
                symbols = string.Join(";", System.Array.FindAll(symbolArray, s => s != SYMBOL));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);

            UpdateMenuCheck(); // 심볼 변경 후 메뉴 체크 상태 갱신
        }

        private static void UpdateMenuCheck()
        {
            Menu.SetChecked(MENU_PATH, IsSymbolEnabled());
        }
    }
}
#endif