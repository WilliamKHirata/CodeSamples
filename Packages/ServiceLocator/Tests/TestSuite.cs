using System.Collections;
using Hik.ServiceLocator;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

internal class TestSuite
{
    private const string SceneWithMonoInstallerFromSameObject = "SceneWithMonoInstallerFromSameObject"; //val starts on 101
    private const string SceneWithMonoInstallerFromAnotherObject = "SceneWithMonoInstallerFromAnotherObject"; //val starts on 201
    private const string SceneWithMultipleMonoInstallers = "SceneWithMultipleMonoInstallers"; //val starts on 301
    private const string SceneWithSceneInstaller = "SceneWithSceneInstaller"; //val starts on 401
    private const string SceneWithMonoAndSceneInstaller = "SceneWithMonoAndSceneInstaller"; //val starts on 501

    private const string RootString = "FromRoot";
    private const int RootInt = 1337;

    private TestSuiteHelper testSuiteHelper;
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        testSuiteHelper = new TestSuiteHelper();
        yield return new WaitForEndOfFrame();
        testSuiteHelper.serviceLocatorRoot.AddInstaller(new TestSuiteInstaller((installer) =>
        {
            installer.Set<TestSuiteDataExample>(new TestSuiteDataExample
            {
                myString = RootString,
                intVal = RootInt
            });
        }), true);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        testSuiteHelper.Clear();
        testSuiteHelper = null;
        yield return null;
    }

    [UnityTest]
    public IEnumerator SLTest_NoLocatorFromRoot()
    {
        yield return null;
        Assert.AreEqual(HKSL.GetFromRoot<TestSuiteDataExample>().intVal, RootInt);
    }

    [UnityTest]
    public IEnumerator SLTest_MonoInstallerFromSameObject()
    {
        string sceneName = SceneWithMonoInstallerFromSameObject;

        SceneObjects sceneObjects = null;

        yield return testSuiteHelper.LoadSceneAndGetObjects(sceneName, (value) => sceneObjects = value);

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono1, HKSLT.GameObject).intVal, 101);

        testSuiteHelper.UnloadSceneAsync(sceneName);
    }

    [UnityTest]
    public IEnumerator SLTest_MonoInstallerFromAnotherObject()
    {
        string sceneName = SceneWithMonoInstallerFromAnotherObject;

        SceneObjects sceneObjects = null;

        yield return testSuiteHelper.LoadSceneAndGetObjects(sceneName, (value) => sceneObjects = value);

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono1, HKSLT.GameObject).intVal, 201);

        testSuiteHelper.UnloadSceneAsync(sceneName);
    }

    [UnityTest]
    public IEnumerator SLTest_MonoInstallerMultiple()
    {
        string sceneName = SceneWithMultipleMonoInstallers;

        SceneObjects sceneObjects = null;

        yield return testSuiteHelper.LoadSceneAndGetObjects(sceneName, (value) => sceneObjects = value);

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono1, HKSLT.GameObject).intVal, 301);
        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono2, HKSLT.GameObject).intVal, 302);

        sceneObjects.serviceLocatorMono2.Remove<TestSuiteDataExample>();

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono2, HKSLT.GameObject).intVal, 301);

        testSuiteHelper.UnloadSceneAsync(sceneName);
    }

    [UnityTest]
    public IEnumerator SLTest_SceneInstaller()
    {
        string sceneName = SceneWithSceneInstaller;

        SceneObjects sceneObjects = null;

        yield return testSuiteHelper.LoadSceneAndGetObjects(sceneName, (value) => sceneObjects = value);

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.gameObject, HKSLT.Scene).intVal, 401);

        testSuiteHelper.UnloadSceneAsync(sceneName);
    }

    [UnityTest]
    public IEnumerator SLTest_MonoAndSceneInstaller()
    {
        string sceneName = SceneWithMonoAndSceneInstaller;

        SceneObjects sceneObjects = null;

        yield return testSuiteHelper.LoadSceneAndGetObjects(sceneName, (value) => sceneObjects = value);

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono1, HKSLT.Scene).intVal, 501);
        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono2, HKSLT.Scene).intVal, 501);

        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono1, HKSLT.GameObject).intVal, 502);
        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono2, HKSLT.GameObject).intVal, 503);

        sceneObjects.serviceLocatorMono2.Remove<TestSuiteDataExample>();
        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono2, HKSLT.GameObject).intVal, 502);

        sceneObjects.serviceLocatorMono1.Remove<TestSuiteDataExample>();
        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono1, HKSLT.GameObject).intVal, 501);
        Assert.AreEqual(HKSL.Get<TestSuiteDataExample>(sceneObjects.serviceLocatorMono2, HKSLT.GameObject).intVal, 501);

        testSuiteHelper.UnloadSceneAsync(sceneName);
    }
}
