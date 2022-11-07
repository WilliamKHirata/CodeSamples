using UnityEngine;

namespace Hik.ServiceLocator
{
    [DefaultExecutionOrder(-9996)]
    public class ServiceLocatorMono : ServiceLocatorBase
    {
        internal override ServiceLocatorBase GetParent()
        {
            if(_parent != null)
            {
                return _parent;
            }

            ServiceLocatorBase parent = null;
            
            if(gameObject.transform.parent != null)
            {
                parent = gameObject.transform.parent.GetComponentInParent<ServiceLocatorMono>();
            }

            if(parent == null)
            {
                parent = ServiceLocatorSceneLoaderObserver.GetInstance().GetServiceLocatorForScene(gameObject.scene);
            }

            if(parent == null)
            {
                parent = ServiceLocatorRoot.GetInstance();
            }

            return parent;
        }
    }
}