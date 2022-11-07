using UnityEngine;

namespace Hik.ServiceLocator
{
    public abstract class ServiceInstallerAbstract : IServiceInstaller
    {
        internal bool _didInstalled = false;
        public void DoInstall(ServiceLocatorBase serviceLocator_)
        {
            if(_didInstalled)
            {
#if UNITY_EDITOR
                Debug.LogError("ServiceInstallerAbstract: trying to install an already installed installer");
#endif
                return; 
            }

            InstallServices(serviceLocator_);

            _didInstalled = true;
        }

        public abstract void InstallServices(ServiceLocatorBase serviceLocator_);
    }
}