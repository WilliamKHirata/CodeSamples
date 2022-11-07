using Hik.ServiceLocator;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

internal class TestSuiteHelper
{
    public ServiceLocatorRoot serviceLocatorRoot;

    public TestSuiteHelper()
    {
        serviceLocatorRoot = ServiceLocatorRoot.GetCleanInstance();
    }

    public void Clear()
    {
        UnityEngine.Object.Destroy(serviceLocatorRoot.gameObject);
    }

    public AsyncOperation UnloadSceneAsync(string sceneName_)
    {
        return SceneManager.UnloadSceneAsync(sceneName_);
    }

    public void LoadScene(string sceneName_)
    {
        EditorSceneManager.LoadSceneInPlayMode(GetAssetInPackage($"Tests/Scenes/{sceneName_}.unity"), new LoadSceneParameters()
        {
            loadSceneMode = LoadSceneMode.Additive
        });
    }

    public AsyncOperation LoadSceneAsync(string sceneName_)
    {
        AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(GetAssetInPackage($"Tests/Scenes/{sceneName_}.unity"), new LoadSceneParameters()
        {
            loadSceneMode = LoadSceneMode.Additive
        });
        return operation;
    }

    public IEnumerator LoadSceneAndGetObjects(string sceneName_, Action<SceneObjects> sceneObjectsCallback)
    {
        yield return null;

        AsyncOperation operation = LoadSceneAsync(sceneName_);
        operation.allowSceneActivation = true;

        yield return new WaitUntil(() => operation.isDone);
        Scene scene = GetScene(sceneName_);
        yield return new WaitUntil(() => scene.isLoaded);

        SceneObjects sceneObjectsInstance = null;
        foreach (GameObject it in scene.GetRootGameObjects())
        {
            sceneObjectsInstance = it.GetComponent<SceneObjects>();
            if (sceneObjectsInstance != null)
            {
                break;
            }
        }
        Assert.IsNotNull(sceneObjectsInstance);
        sceneObjectsCallback?.Invoke(sceneObjectsInstance);
    }

    public Scene GetScene(string sceneName_)
    {
        return SceneManager.GetSceneByName(sceneName_);
    }

    private string GetAssetInPackage(string path)
    {
        return $"Packages/com.hik.servicelocator/{path}";
    }
}
