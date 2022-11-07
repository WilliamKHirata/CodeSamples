using UnityEngine;

namespace Hik.ServiceLocator
{
    public abstract class MonoInstaller : MonoBehaviour, IServiceInstaller
    {
        internal bool _didInstalled = false;
        public void DoInstall(ServiceLocatorBase serviceLocator_)
        {
            if(_didInstalled)
            {
#if UNITY_EDITOR
                Debug.LogError("MonoInstaller: trying to install an already installed installer", this);
#endif
                return; 
            }

            InstallServices(serviceLocator_);

            _didInstalled = true;
        }

        public abstract void InstallServices(ServiceLocatorBase serviceLocator_);
    }
}