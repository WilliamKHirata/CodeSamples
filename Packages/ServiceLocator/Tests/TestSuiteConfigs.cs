using Hik.ServiceLocator;
using System;
using UnityEngine;

public class TestSuiteInstaller : IServiceInstaller
{
    private Action<ServiceLocatorBase> __installAction;
    public TestSuiteInstaller(Action<ServiceLocatorBase> installAction_)
    {
        __installAction = installAction_;
    }

    public void DoInstall(ServiceLocatorBase serviceLocator)
    {
        __installAction?.Invoke(serviceLocator);
    }
}

[Serializable]
public class TestSuiteDataExample
{
    public string myString;
    public int intVal;

    public void LogValues()
    {
        Debug.Log(JsonUtility.ToJson(this));
    }
}