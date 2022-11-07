using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hik.ServiceLocator
{
    public static class HKSL
    {
        private const string DONTDESTROY_SCENE_NAME = "DontDestroyOnLoad";

        private static Type s_monoType = typeof(MonoBehaviour);
        private static Type s_gameObjectType = typeof(GameObject);

        public static void Set<T>(object start_, HKSLT type_, object service_, object id_ = null) where T : class
        {
            if(id_ == null) { id_ = ServiceLocatorBase.defaultIdObject; }
            ServiceLocatorBase serviceLocator = GetServiceLocator(start_, type_);
            serviceLocator.SetElement(typeof(T), service_, id_);
        }

        public static T GetFromRoot<T>(object id_ = null) where T : class
        {
            if (id_ == null) { id_ = ServiceLocatorBase.defaultIdObject; }
            ServiceLocatorBase serviceLocator = GetRoot();
            return serviceLocator.Find<T>(typeof(T), id_);
        }

        public static T Get<T>(object start_, HKSLT type_, object id_ = null) where T : class
        {
            if(id_ == null) { id_ = ServiceLocatorBase.defaultIdObject; }
            ServiceLocatorBase serviceLocator = GetServiceLocator(start_, type_);
            return serviceLocator.Find<T>(typeof(T), id_);
        }

        public static void Delete<T>(object start_, HKSLT type_, object id_ = null) where T : class
        {
            if(id_ == null) { id_ = ServiceLocatorBase.defaultIdObject; }
            ServiceLocatorBase serviceLocator = GetServiceLocator(start_, type_);
            serviceLocator.RemoveElement(typeof(T), id_);
        }

        public static void DeleteAll<T>(object start_, HKSLT type_, object id_ = null) where T : class
        {
            if(id_ == null) { id_ = ServiceLocatorBase.defaultIdObject; }
            ServiceLocatorBase serviceLocator = GetServiceLocator(start_, type_);
            serviceLocator.RemoveElementCascade(typeof(T), id_);
        }

        public static Guid GenerateNewGuidForScene(string sceneName_)
        {
            return ServiceLocatorSceneLoaderObserver.GetInstance().GenerateNewGuidForScene(sceneName_);
        }

        public static void AddInstallActionForScenePreInstaller(Guid sceneGuid_, Action<ServiceLocatorBase> installer_)
        {
            ServiceLocatorSceneLoaderObserver.GetInstance().AddInstallActionForSceneInstaller(sceneGuid_, installer_);
        }

        public static void SetParentForScenePreInstaller(Guid sceneGuid_, ServiceLocatorBase parent_)
        {
            ServiceLocatorSceneLoaderObserver.GetInstance().SetParentForSceneInstaller(sceneGuid_, parent_);
        }

        public static ServiceLocatorBase GetSceneServiceLocator(Scene scene_)
        {
            return GetScene(scene_);
        }

        public static ServiceLocatorBase GetSceneServiceLocator(Guid sceneGuid_)
        {
            return ServiceLocatorSceneLoaderObserver.GetInstance().GetServiceLocatorForGuid(sceneGuid_);
        }

        public static List<ServiceLocatorScene> GetSceneServiceLocators(string sceneName_)
        {
            return ServiceLocatorSceneLoaderObserver.GetInstance().GetServiceLocatorsForSceneName(sceneName_);
        }

        public static ServiceLocatorBase GetServiceLocator(object start_, HKSLT type_)
        {
            GameObject monoGameObject = null;
            if(start_ == null)
            {
                type_ = HKSLT.Root;
            }
            else if(s_monoType.IsAssignableFrom(start_.GetType()))
            {
                monoGameObject = ((MonoBehaviour) start_).gameObject;
            }
            else if(s_gameObjectType.IsAssignableFrom(start_.GetType()))
            {
                monoGameObject = (GameObject) start_;
            }

            switch(type_)
            {
                case HKSLT.Scene:
                    if (monoGameObject == null)
                    {
                        return GetRoot();
                    }
                    ServiceLocatorBase serviceLocatorFromScene = GetScene(monoGameObject.scene);
                    if(serviceLocatorFromScene != null)
                    {
                        return serviceLocatorFromScene;
                    }
                    goto case HKSLT.Root;
                case HKSLT.GameObject:
                    if(monoGameObject == null)
                    {
                        return GetRoot();
                    }

                    ServiceLocatorBase serviceLocatorFromMono = GetFromMono(monoGameObject);
                    if(serviceLocatorFromMono != null)
                    {
                        return serviceLocatorFromMono;
                    }
                    goto case HKSLT.Root;
                case HKSLT.Root:
                    return GetRoot();
            }
            return GetRoot();
        }

        private static ServiceLocatorRoot GetRoot()
        {
            return ServiceLocatorRoot.GetInstance();
        }

        private static ServiceLocatorScene GetScene(Scene scene_)
        {
            if(scene_.name == DONTDESTROY_SCENE_NAME)
            {
                return null;
            }
            
            return ServiceLocatorSceneLoaderObserver.GetInstance().GetServiceLocatorForScene(scene_);
        }

        private static ServiceLocatorBase GetFromMono(GameObject gameObject_)
        {
            ServiceLocatorBase serviceLocator = gameObject_.GetComponent<ServiceLocatorMono>();

            if(serviceLocator != null)
            {
                return serviceLocator;
            }

            if(gameObject_.transform.parent != null)
            {
                serviceLocator = gameObject_.transform.parent.GetComponentInParent<ServiceLocatorMono>();
            }

            if(serviceLocator != null)
            {
                return serviceLocator;
            }

            return GetScene(gameObject_.scene);
        }
    }

    public enum HKSLT
    {
        Root,
        Scene,
        GameObject
    }
}