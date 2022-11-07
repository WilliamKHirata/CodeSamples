using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hik.ServiceLocator
{
    public abstract class ServiceLocatorBase : MonoBehaviour
    {
        internal static object defaultIdObject = new object();

        [SerializeField] internal List<ScriptableObject> _scriptables = new List<ScriptableObject>();
        [SerializeField] internal List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();
        [SerializeField] internal ServiceLocatorBase _parent = null;

        internal List<IServiceInstaller> _installers = new List<IServiceInstaller>();
        internal Dictionary<Type, Dictionary<object, object>> _entries = new Dictionary<Type, Dictionary<object, object>>();

        internal void Awake()
        {
            DoInstalls();
        }

        internal virtual void DoInstalls()
        {
            HashSet<IServiceInstaller> installersToExecute = new HashSet<IServiceInstaller>();
            MonoInstaller[] monoInstallers = GetComponents<MonoInstaller>();

            foreach (IServiceInstaller it in _installers)
            {
                installersToExecute.Add(it);
            }

            foreach (MonoInstaller it in monoInstallers)
            {
                installersToExecute.Add(it);
            }

            foreach (MonoInstaller it in _monoInstallers)
            {
                installersToExecute.Add(it);
            }

            foreach (IServiceInstaller it in installersToExecute)
            {
                it?.DoInstall(this);
            }

            foreach(ScriptableObject it in _scriptables)
            {
                if(it != null)
                {
                    SetElement(it.GetType(), it, defaultIdObject);
                }
            }
        }

        public ServiceLocatorBase AddInstaller(IServiceInstaller installer_, bool doInstall_ = false)
        {
            _installers.Add(installer_);
            if(doInstall_)
            {
                installer_?.DoInstall(this);
            }
            return this;
        }

        public void SetParent(ServiceLocatorBase parent_)
        {
            _parent = parent_;
        }

        public bool Contains<T>(Type type_, object id_ = null) where T : class
        {
            if (id_ == null) { id_ = defaultIdObject; }

            Dictionary<object, object> dictServices;

            if(_entries.TryGetValue(type_, out dictServices))
            {
                return dictServices.ContainsKey(id_);
            }

            return false;
        }

        public void Set<T>(object service_, object id_ = null) where T : class
        {
            if(id_ == null) { id_ = defaultIdObject; }
            SetElement(typeof(T), service_, id_);
        }

        public void Remove<T>(object id_ = null) where T : class
        {
            if(id_ == null) { id_ = defaultIdObject; }
            RemoveElement(typeof(T), id_);
        }

        public void RemoveAll<T>(object id_ = null) where T : class
        {
            if(id_ == null) { id_ = defaultIdObject; }
            RemoveElementCascade(typeof(T), id_);
        }

        internal T Find<T>(Type type_, object id_ = null) where T : class
        {
            if (id_ == null) { id_ = defaultIdObject; }

            Dictionary<object, object> dictServices;
            if (_entries.TryGetValue(type_, out dictServices))
            {
                if(dictServices.ContainsKey(id_))
                {
                    return (T)dictServices[id_];
                }
            }

            if(GetParent() == null)
            {
                return null;
            }

            return GetParent().Find<T>(type_, id_);
        }

        internal void SetElement(Type type_, object service_, object id_ = null)
        {
            if (id_ == null) { id_ = defaultIdObject; }

            Assert.IsTrue(type_.IsClass);

            Dictionary<object, object> dictServices;

            if(!_entries.TryGetValue(type_, out dictServices))
            {
                dictServices = new Dictionary<object, object>();
                _entries.Add(type_, dictServices);
            }

            Assert.IsFalse(dictServices.ContainsKey(id_));

            dictServices.Add(id_, service_);
        }

        internal void RemoveElement(Type type_, object id_ = null)
        {
            if (id_ == null) { id_ = defaultIdObject; }

            Dictionary<object, object> dictServices;

            if(!_entries.TryGetValue(type_, out dictServices))
            {
                return;
            }

            if(!dictServices.ContainsKey(id_))
            {
                return;
            }

            dictServices.Remove(id_);
        }

        internal void RemoveElementCascade(Type type_, object id_)
        {
            if (id_ == null) { id_ = defaultIdObject; }

            RemoveElement(type_, id_);
            ServiceLocatorBase parent = GetParent();
            if(parent != null)
            {
                RemoveElementCascade(type_, id_);
            }
        }


        internal abstract ServiceLocatorBase GetParent();
    }
}