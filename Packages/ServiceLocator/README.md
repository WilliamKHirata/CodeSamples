Why to use:

As projects grows, the amount of scripts, data and services increases to a point that it becomes difficult to include/get without a use of a singleton or a hackish way to move data from an object to another one

To help managing that the Service Locator module was created with Unity workflow in mind

Which includes the following features:
- Locate objects by cascading context
	Cascading context can start from Root, Scene or Mono
	When looking up for service, it will go through the following order: Mono -> Scene -> Root
- Register anything that inherits from object
- Register by context and id
- Register multiple of same type by using id

Setup:
On toolbar click on Hik -> Service Locator -> Create Root

Configuring service locator:

===== Taxonomy =====
ServiceLocators are separated on 3 following types(Component name on parenthesis):
Base (ServiceLocatorBase)
	Base for all 3 ServiceLocator classes described below
Root (ServiceLocatorRoot)
	Only one should exists and lies on Resources folder named as ServiceLocatorRoot
Scene (ServiceLocatorScene)
	Can be put on any root GameObject on scene and should have only one instance per scene
Mono (ServiceLocatorMono)
	Can have any amount of this type and should be added on monobehaviours

===== ServiceLocator configurations =====
Scriptables
	ScriptableObjects to be set automatically on ServiceLocator
Mono Installers
	List of MonoInstallers to be used when ServiceLocator do installs
Parent
	Parent used for object lookup cascade

===== MonoInstallers =====
Abstract classes used to do installs on ServiceLocators, the same instance can't be installed more than once

To use it, create a class that extends MonoInstaller and add on a GameObject with a ServiceLocator or link it manually on ServiceLocator's MonoInstallers field
MonoInstallers should implement the abstract method InstallServices(ServiceLocatorBase serviceLocator_)
The common methods used on ServiceLocatorBase install are:
 Set<T>(object service_, object id_ = null)
	Register a value(service_) on current ServiceLocator with type T and id(id_). If id isn't passed then it will fallback for the default value which is controlled internally
 Remove<T>(object id_ = null)
	Remove a value T from current ServiceLocator with id(id_). If id isn't passed then it will fallback for the default value which is controlled internally
 RemoveAll<T>(object id_ = null)
	Cascade remove all occurrences of type T with id(id_). Note that it will remove from all lookup hierarchy until root

===== Read/Write/Prepare services =====
To use ServiceLocator capabilities it's suggested to be done via the static class HKSL where the methods works as described below

== Lookup ==
Lookup for ServiceLocators occurs by the following:

Lookup for a ServiceLocator instance which will go through the following path by starting object
	If start object is a MonoBehaviour or GameObject will search on all hierarchy starting from object for a ServiceLocator instance and add the value on this service locator, if none found then it will fallback to root
		It will go through all parenting hierarchy until parent doesnt exists then it will fall to root
	If start object is null or non-MonoBehaviour/non-GameObject it will lookup directly on ServiceLocator root

Which lookup can be modified by HKSLT enum
	HKSLT had 3 types as follow:
		Root - Lookup directly on Root
        Scene - Lookup for Scene ServiceLocator
        GameObject - Lookup for MonoBehaviour, 
	Note that any if non-mono or non-gameobject will fallback to root

void Set<T>(object start_, HKSLT type_, object service_, object id_ = null)
	Set service_ of type T on ServiceLocator

T Get<T>(object id_ = null)
	Get service on Root of type T by id

T Get<T>(object start_, HKSLT type_, object id_ = null)
	Get service of type T by id on ServiceLocator found by start_ and type_

void Delete<T>(object start_, HKSLT type_, object id_ = null)
	Delete service of type T by id on ServiceLocator found by start_ and type_

void DeleteAll<T>(object start_, HKSLT type_, object id_ = null)
	Delete all service of type T by id on all ServiceLocators found by start_ and type_

Guid GenerateNewGuidForScene(string sceneName_)
	Create guid for sceneName_ (Read "Setting Up Scenes" section for better understanding)

void AddInstallActionForScenePreInstaller(Guid sceneGuid_, Action<ServiceLocatorBase> installer_)
	Add install action for non initialized scene with Guid sceneGuid_ (Read "Setting Up Scenes" section for better understanding)

void SetParentForScenePreInstaller(Guid sceneGuid_, ServiceLocatorBase parent_)
	Set parent to be put on non initialized scene with Guid sceneGuid_ when initialized (Read "Setting Up Scenes" section for better understanding)

ServiceLocatorBase GetSceneServiceLocator(Scene scene_)
	Get ServiceLocatorScene by Scene scene_

ServiceLocatorBase GetSceneServiceLocator(Guid sceneGuid_)
	Get ServiceLocatorScene by Guid sceneGuid_

List<ServiceLocatorScene> GetSceneServiceLocators(string sceneName_)
	Get all ServiceLocatorScenes that has scene name as sceneName_

ServiceLocatorBase GetServiceLocator(object start_, HKSLT type_)
	Get ServiceLocator found by start_ and type_


== Setting Up Scenes ==
ServiceLocator module enabled the scene data configuration when loading new scenes

On section "Lookup" there's 3 common methods used for the scene configuration
Guid GenerateNewGuidForScene(string sceneName_)
void AddInstallActionForScenePreInstaller(Guid sceneGuid_, Action<ServiceLocatorBase> installer_)
void SetParentForScenePreInstaller(Guid sceneGuid_, ServiceLocatorBase parent_)

GenerateNewGuidForScene
	Will generate a Guid for the specified scene name and create internally on system a SceneInstaller which can be modified with the 2 other methods
	When a scene with same name used on GenerateNewGuidForScene is created, the ServiceLocatorScene will check if there's any queued Guid for the scene name, if there's one it will use that one for the scene and get the SceneInstaller associated with that Guid

AddInstallActionForScenePreInstaller
	After a Guid is generated for a scene, that Guid received can be used to add install actions the same way it's done when installing by components

SetParentForScenePreInstaller
	After a Guid is generated for a scene, that Guid received can be used to set the parent that the scene will have when looking up for objects