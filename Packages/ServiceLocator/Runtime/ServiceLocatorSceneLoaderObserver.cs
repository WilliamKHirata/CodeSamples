using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hik.ServiceLocator
{
    [DefaultExecutionOrder(-9999)]
    internal class ServiceLocatorSceneLoaderObserver
    {
        internal static ServiceLocatorSceneLoaderObserver _instance;

        private Dictionary<Scene, ServiceLocatorScene> _serviceLocators;
        internal Dictionary<Guid, SceneInstaller> _scenePreInstaller = new Dictionary<Guid, SceneInstaller>();
        internal Dictionary<string, List<Guid>> _queuedGuids = new Dictionary<string, List<Guid>>();

        private object _lockObjectSceneLocator = new object();
        private object _lockObjectGuid = new object();
        private object _lockObjectScenePreInstaller = new object();

        internal static void Initialize()
        {
            if (_instance != null)
            {
                return;
            }

            _instance = new ServiceLocatorSceneLoaderObserver();
        }

        internal static void Remove()
        {
            if(_instance == null)
            {
                return;
            }

            SceneManager.sceneUnloaded -= _instance.OnSceneUnloaded;
            _instance = null;
        }

        internal static ServiceLocatorSceneLoaderObserver GetInstance()
        {
            if(_instance == null)
            {
                ServiceLocatorRoot.GetInstance();
            }
            return _instance;
        }

        internal ServiceLocatorSceneLoaderObserver()
        {
            _serviceLocators = new Dictionary<Scene, ServiceLocatorScene>();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        internal ServiceLocatorScene GetServiceLocatorForScene(Scene scene_)
        {
            lock (_lockObjectSceneLocator)
            {
                if (_serviceLocators.ContainsKey(scene_))
                {
                    return _serviceLocators[scene_];
                }
                return null;
            }
        }

        internal ServiceLocatorScene GetServiceLocatorForGuid(Guid guid_)
        {
            lock (_lockObjectSceneLocator)
            {
                ServiceLocatorScene serviceLocator = _serviceLocators.Where(x => x.Value.GetGuid() == guid_).Select(x => x.Value).FirstOrDefault();
                return serviceLocator;
            }
        }

        internal List<ServiceLocatorScene> GetServiceLocatorsForSceneName(string sceneName_)
        {
            lock (_lockObjectSceneLocator)
            {
                List<ServiceLocatorScene> serviceLocators = new List<ServiceLocatorScene>();
                foreach (KeyValuePair<Scene, ServiceLocatorScene> kv in _serviceLocators)
                {
                    if (kv.Key.name == sceneName_)
                    {
                        serviceLocators.Add(kv.Value);
                    }
                }

                return serviceLocators;
            }
        }

        internal void AddInstallActionForSceneInstaller(Guid sceneGuid_, Action<ServiceLocatorBase> installAction_)
        {
            SceneInstaller sceneInstaller = GetScenePreInstaller(sceneGuid_);
            if(sceneInstaller == null)
            {
                throw new Exception("ServiceLocatorSceneLoaderObserver: AddInstallActionForSceneInstaller trying to add action for non-existent sceneInstaller");
            }
            sceneInstaller.AddInstallAction(installAction_);
        }

        internal void SetParentForSceneInstaller(Guid sceneGuid_, ServiceLocatorBase serviceLocator_)
        {
            SceneInstaller sceneInstaller = GetScenePreInstaller(sceneGuid_);
            if (sceneInstaller == null)
            {
                throw new Exception("ServiceLocatorSceneLoaderObserver: SetParentForSceneInstaller trying to add action for non-existent sceneInstaller");
            }
            sceneInstaller.SetParent(serviceLocator_);
        }

        internal Guid GenerateNewGuidForScene(string sceneName_)
        {
            lock (_lockObjectGuid)
            {
                List<Guid> sceneQueuedGuids;
                if (!_queuedGuids.TryGetValue(sceneName_, out sceneQueuedGuids))
                {
                    sceneQueuedGuids = new List<Guid>();
                    _queuedGuids.Add(sceneName_, sceneQueuedGuids);
                }

                Guid createdGuid = Guid.NewGuid();
                CreateScenePreInstaller(createdGuid);
                sceneQueuedGuids.Add(createdGuid);
                return createdGuid;
            }
        }

        internal void CreateScenePreInstaller(Guid sceneGuid_)
        {
            lock (_lockObjectScenePreInstaller)
            {
                if (sceneGuid_ == Guid.Empty)
                {
                    throw new Exception("ServiceLocatorSceneLoaderObserver: CreateScenePreInstaller trying to create pre installer with empty Guid");
                }

                if (_scenePreInstaller.ContainsKey(sceneGuid_))
                {
                    throw new Exception("ServiceLocatorSceneLoaderObserver: CreateScenePreInstaller trying to create pre installer with existent Guid");
                }

                SceneInstaller sceneInstaller = new SceneInstaller();
                _scenePreInstaller.Add(sceneGuid_, sceneInstaller);
            }
        }

        internal SceneInstaller GetScenePreInstaller(Guid sceneGuid_)
        {
            lock (_lockObjectScenePreInstaller)
            {
                if (sceneGuid_ == Guid.Empty)
                {
                    throw new Exception("ServiceLocatorSceneLoaderObserver: GetScenePreInstaller trying to get pre installer with empty Guid");
                }

                SceneInstaller sceneInstaller = null;

                if (!_scenePreInstaller.TryGetValue(sceneGuid_, out sceneInstaller))
                {
                    return null;
                }

                return sceneInstaller;
            }
        }

        internal void RemoveScenePreInstaller(Guid sceneGuid_)
        {
            lock (_lockObjectScenePreInstaller)
            {
                if (_scenePreInstaller.ContainsKey(sceneGuid_))
                {
                    _scenePreInstaller.Remove(sceneGuid_);
                }
            }
        }

        internal void RegisterServiceLocator(ServiceLocatorScene serviceLocator_)
        {
            lock (_lockObjectSceneLocator)
            {
                _serviceLocators.Add(serviceLocator_.gameObject.scene, serviceLocator_);
            }
        }

        internal void RemoveServiceLocator(Scene scene_)
        {
            lock (_lockObjectSceneLocator)
            {
                if (_serviceLocators.ContainsKey(scene_))
                {
                    if (_serviceLocators[scene_] != null)
                    {
                        UnityEngine.Object.Destroy(_serviceLocators[scene_].gameObject);
                    }
                    _serviceLocators.Remove(scene_);
                }
            }
        }

        internal void PopulateServiceLocatorWithQueuedOrNewGuid(ServiceLocatorScene serviceLocatorScene_)
        {
            lock (_lockObjectGuid)
            {
                string sceneName = serviceLocatorScene_.gameObject.scene.name;

                Guid queuedOrNewGuid;
                if (_queuedGuids.TryGetValue(sceneName, out List<Guid> sceneQueuedGuids) && sceneQueuedGuids.Count > 0)
                {
                    Guid sceneGuid = sceneQueuedGuids[0];
                    sceneQueuedGuids.RemoveAt(0);
                    queuedOrNewGuid = sceneGuid;
                }
                else
                {
                    queuedOrNewGuid = Guid.NewGuid();
                }

                serviceLocatorScene_.SetGuid(queuedOrNewGuid);
            }
        }

        private void OnSceneUnloaded(Scene scene_)
        {
            RemoveServiceLocator(scene_);
        }
    }
}