using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PJH.Utility.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PJH.Utility
{
    public static class AddressableManager
    {
        public static bool showDebugLog = true;
        public static bool isLoaded;
        public static Action OnLoaded;
        private static Dictionary<string, Object> _resources = new Dictionary<string, Object>();

        private static Dictionary<string, AsyncOperationHandle> _handles =
            new Dictionary<string, AsyncOperationHandle>();

        #region load reousources

        public static T Load<T>(string key) where T : Object
        {
            if (_resources.TryGetValue(key, out Object resource))
            {
                var result = resource as T;
                if (showDebugLog)
                    Debug.Log(result);
                if (result == null)
                    if (resource is GameObject go)
                    {
                        return go.GetComponent<T>();
                    }

                return result;
            }

            return null;
        }

        public static T Instantiate<T>(string key, Transform parent = null) where T : Component
        {
            GameObject prefab = Load<GameObject>(key);

            if (!prefab)
            {
                if (showDebugLog)
                    Debug.LogError($"Failed to load prefab : {key}");
                return null;
            }

            var prefabInstantiate = Object.Instantiate(prefab, parent);
            T go = prefabInstantiate.GetOrAdd<T>();
            go.gameObject.name = prefab.name;
            return go;
        }

        public static GameObject Instantiate(string key, Transform parent = null)
        {
            GameObject prefab = Load<GameObject>(key);

            if (!prefab)
            {
                if (showDebugLog)
                    Debug.LogError($"Failed to load prefab : {key}");
                return null;
            }

            GameObject go = Object.Instantiate(prefab, parent);
            go.gameObject.name = prefab.name;
            return go;
        }

        #endregion

        #region addressable

        private static async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            if (_resources.TryGetValue(key, out Object resource))
            {
                return resource as T;
            }

            string loadKey = key;
            if (key.Contains(".sprite"))
                loadKey = $"{key}[{key.Replace(".sprite", "")}]";

            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
            await asyncOperation.Task;

            T result = asyncOperation.Result;
            _resources.TryAdd(key, result);
            _handles.TryAdd(key, asyncOperation);
            return result;
        }


        public static async UniTask LoadALlAsync<T>(string label, Action<string, int, int> callBack = null)
            where T : Object
        {
            bool downloadSuccess = await DownloadDependenciesAsync(label);
            if (!downloadSuccess) return;
            var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
            await opHandle.Task;

            int loadCount = 0;
            int totalCount = opHandle.Result.Count;

            foreach (var result in opHandle.Result)
            {
                bool isContainsDotSprite = result.PrimaryKey.Contains(".sprite");
                if (isContainsDotSprite)
                {
                    await LoadAsync<Sprite>(result.PrimaryKey);
                    loadCount++;
                    callBack?.Invoke(result.PrimaryKey, loadCount, totalCount);
                }
                else
                {
                    await LoadAsync<T>(result.PrimaryKey);
                    loadCount++;
                    callBack?.Invoke(result.PrimaryKey, loadCount, totalCount);
                }
            }

            isLoaded = true;
            OnLoaded?.Invoke();
        }

        public static async UniTask<bool> DownloadDependenciesAsync(object label)
        {
            var getDownloadHandle = Addressables.GetDownloadSizeAsync(label);
            await getDownloadHandle.Task;
            if (getDownloadHandle.Status == AsyncOperationStatus.Succeeded && getDownloadHandle.Result > 0)
            {
                var downloadHandle = Addressables.DownloadDependenciesAsync(label, true);
                await downloadHandle.Task;
                if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
                    return false;
            }

            return true;
        }

        #endregion

        #region scene

        public static async UniTask<SceneInstance> LoadSceneAsync(string key,
            LoadSceneMode sceneMode = LoadSceneMode.Single)
        {
            return await Addressables.LoadSceneAsync(key, sceneMode);
        }

        public static async UniTask<SceneInstance> UnloadSceneAsync(
            AsyncOperationHandle<SceneInstance> sceneInstanceHandle)
        {
            return await Addressables.UnloadSceneAsync(sceneInstanceHandle);
        }

        #endregion
    }
}