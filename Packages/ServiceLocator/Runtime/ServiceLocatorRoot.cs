using System;
using UnityEngine;

namespace Hik.ServiceLocator
{
    [DefaultExecutionOrder(-9998)]
    public class ServiceLocatorRoot : ServiceLocatorBase
    {
        public const string RESOURCES_ROOT_NAME = "ServiceLocatorRoot";

        private static ServiceLocatorRoot _root;

        new void Awake()
        {
            if(_root != null)
            {
                Destroy(gameObject);
            }
            gameObject.transform.parent = null;

            DontDestroyOnLoad(gameObject);
            _root = this;
            ServiceLocatorSceneLoaderObserver.Initialize();

            base.Awake();
        }

        internal override ServiceLocatorBase GetParent()
        {
            return null;
        }

        public static ServiceLocatorRoot GetInstance()
        {
            if(_root == null)
            {
                LoadDefaultRoot();
            }

            return _root;
        }

        public static ServiceLocatorRoot GetCleanInstance()
        {
            GameObject gameObject = new GameObject();
            gameObject.name = RESOURCES_ROOT_NAME;
            ServiceLocatorRoot locatorRoot = gameObject.AddComponent<ServiceLocatorRoot>();
            _root = locatorRoot;
            return _root;
        }

        public static void Remove()
        {
            if (_root == null)
            {
                return;
            }

            ServiceLocatorSceneLoaderObserver.Remove();
            Destroy(_root.gameObject);
        }

        private static void LoadDefaultRoot()
        {
            GameObject serviceLocatorRootGameObject = Resources.Load<GameObject>(typeof(ServiceLocatorRoot).Name);
            Instantiate(serviceLocatorRootGameObject).name = RESOURCES_ROOT_NAME;
        }
    }
}