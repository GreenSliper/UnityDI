# UnityDI
Lightweight unity DI system. Includes field-injection, constructor-injection, recursive injection

Samples for injection classes into container are shown in Services.cs. The best way is to use this class on one of 
your gameobjects in the main scene and confingure services in the related method. You can clear all the dependencies
and re-inject them on the other scenes (if needed) or use objects transferred between scenes (or use transient-only DI).
When injecting dependencies you can define object factory or use auto-construction (with no arguments). Auto-construction
constructors are called by DIConstruction using recursive constructor calls through reflection. Circular references 
are avoided if possible.
Now only transient and singleton lifetimes are implemented, but you are free to add other ones, if needed. See 
DIContainer for details.

Injection can be done via 2 ways:
- constructor injection
- field injection

For monobehaviours field injection is a better option (their constructors are not called in runtime). To use field
injection you need 2 steps:
- Use [DInject] attribute on interface fields
- Inherit from DIMono class OR call DI.InjectFields(this) on Start/Awake
See DI.cs, DIContainer.cs and DIMono.cs for details.

For classes, created manually (by your code) constructor injection is a better option. Just add the needed classes
in the DI container, then use DI.GetType<T>() / field injection.

For serialized (in the inspector, for example) class instances you can inherit from DIClass & use field injection,
while the constructor can be called multiple times for some reason. This way adds the instances to injection queue,
and the fields are injected right after all the services are configured.
