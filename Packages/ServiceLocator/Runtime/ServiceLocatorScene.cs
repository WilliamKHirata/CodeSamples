using System;
using UnityEngine;

namespace Hik.ServiceLocator
{
    [DefaultExecutionOrder(-9997)]
    public class ServiceLocatorScene : ServiceLocatorBase
    {
        internal Guid _sceneGuid = Guid.Empty;

        new void Awake()
        {
            ServiceLocatorSceneLoaderObserver.GetInstance().PopulateServiceLocatorWithQueuedOrNewGuid(this);
            ServiceLocatorSceneLoaderObserver.GetInstance().RegisterServiceLocator(this);
            SceneInstaller sceneInstaller = ServiceLocatorSceneLoaderObserver.GetInstance().GetScenePreInstaller(_sceneGuid);
            if(sceneInstaller != null)
            {
                AddInstaller(sceneInstaller);
                ServiceLocatorSceneLoaderObserver.GetInstance().RemoveScenePreInstaller(_sceneGuid);
            }
            base.Awake();
        }

        internal override ServiceLocatorBase GetParent()
        {
            if(_parent != null)
            {
                return _parent;
            }
            
            return ServiceLocatorRoot.GetInstance();
        }

        internal void SetGuid(Guid guid_)
        {
            _sceneGuid = guid_;
        }

        internal Guid GetGuid()
        {
            return _sceneGuid;
        }
    }
}