using Hik.ServiceLocator;

public class SceneTestsMonoInstaller : MonoInstaller
{
    public TestSuiteDataExample dataExample;
    public override void InstallServices(ServiceLocatorBase serviceLocator_)
    {
        serviceLocator_.Set<TestSuiteDataExample>(dataExample);
    }
}
