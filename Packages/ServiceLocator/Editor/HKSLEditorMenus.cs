using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hik.ServiceLocator.Editor
{
    public class HKSLEditorMenus
    {
        [MenuItem("Hik/ServiceLocator/Create Root", false, 200)]
        private static void CreateServiceLocatorRoot()
        {
            GameObject rootGameObject = GetNewServiceLocatorRoot();
            string filePath = GetServiceLocatorRootFilepath();
            Directory.CreateDirectory(GetDirectoryPath(GetServiceLocatorRootFilepath()));
            PrefabUtility.SaveAsPrefabAsset(rootGameObject, GetServiceLocatorRootFilepath());
            UnityEngine.GameObject.DestroyImmediate(rootGameObject);
        }

        [MenuItem("Hik/ServiceLocator/Create Root", true)]
        private static bool CreateServiceLocatorRootValidator()
        {
            GameObject rootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetServiceLocatorRootFilepath());
            return rootPrefab == null;
        }

        private static string GetServiceLocatorRootFilepath()
        {
            return string.Format("Assets/Resources/{0}.prefab", ServiceLocatorRoot.RESOURCES_ROOT_NAME);
        }

        private static GameObject GetNewServiceLocatorRoot()
        {
            GameObject rootGameObject = new GameObject(ServiceLocatorRoot.RESOURCES_ROOT_NAME);
            rootGameObject.AddComponent<ServiceLocatorRoot>();
            return rootGameObject;
        }

        private static string GetDirectoryPath(string fullPath_)
        {
            return fullPath_.Substring(0, fullPath_.LastIndexOf("/"));
        }
    }
}