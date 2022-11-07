using System;
using System.Collections.Generic;

namespace Hik.ServiceLocator
{
    public class SceneInstaller : ServiceInstallerAbstract
    {
        private static Type s_sceneType = typeof(ServiceLocatorScene);
        private List<Action<ServiceLocatorBase>> _installActions;
        private ServiceLocatorBase _parentServiceLocator;

        internal SceneInstaller()
        {
            _installActions = new List<Action<ServiceLocatorBase>>();
        }

        public override void InstallServices(ServiceLocatorBase serviceLocator_)
        {
            if(_parentServiceLocator != null && s_sceneType.IsAssignableFrom(serviceLocator_.GetType()))
            {
                ((ServiceLocatorScene)serviceLocator_)._parent = _parentServiceLocator;
            }
            foreach(Action<ServiceLocatorBase> it in _installActions)
            {
                it?.Invoke(serviceLocator_);
            }
        }

        internal void AddInstallAction(Action<ServiceLocatorBase> installAction_)
        {
            if(!_installActions.Contains(installAction_))
            {
                _installActions.Add(installAction_);
            }
        }

        internal void SetParent(ServiceLocatorBase serviceLocator_)
        {
            _parentServiceLocator = serviceLocator_;
        }

        internal List<Action<ServiceLocatorBase>> GetInstallActions()
        {
            return _installActions;
        }

        internal ServiceLocatorBase GetParentServiceLocator()
        {
            return _parentServiceLocator;
        }
    }
}