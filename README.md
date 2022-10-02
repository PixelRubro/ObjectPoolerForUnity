# Quick Pooler

## What is this?

This is an asset created to easily pool objects to avoid garbage collection. Instead of creating and destroying objects, you will be activating and deactivating the same objects, recycling them and avoiding performance loss.

## How do I use this?

- Add the component ObjectPooler to the game object that will spawn the pooled objects
- Create a script extending the class PoolableMonobehaviour and create a prefab with it
- Drag the prefab to the field "Poolable Object Prefab" of the ObjectPooler component
- Use the pool's method Pop to retrieve a copy of the prefab for use
- Call StartDeactivation or DeactivateImmediate on the pooled object to return the item to the pool

## How the pool actually works?

The pool automatically instantiates a number (set in the inspector) of copies of the provided prefab and registers them to a dictionary assigning ids to them. In the process of instiation of the prefabs, each object is subscribed to a callback that returns it back to the pool when it is deactivated.

When there is a request for an object using the method Pop, the pool simply pops its stack and provide the object immediately. If there are no objects in the stack, the pool will attempt to expand itself if the isExpansible field is set to true. If that's not the case, it will return null.

It's possible to add new objects to the pool using the method Add, but an exception will be thrown if you are trying to add an object that already belongs to the pool. To return a object to the pool, you can use the method Return, but the ideal way is to call StartDeactivation or StartDeactivation on the pooled object being deactivated. Objects added or returned to the pool will be deactivated before pushed onto the stack.

The operations to retrieve an object from the pool and to return an object to the pool have both a complexity of O(1).

## Why can't I open the demo scene?

This is a known limitation of the Unity team. Until they look at it, you will have to drag the scene file into somewhere in your "Assets" folder.

## There are any events fired along the flow of the asset?

Yes, both the pool and the poolable object have Actions that you can subscribe methods to. They always start with the prefix "On".

## Why call the StartDeactivation method?

If a game object is deactivated immediately, stuff like visual effects, sound effects and animations would not be activated, so this process starts a countdown to any additional events to be fired.

## Anything else to look out for?

- When extending the PoolableMonobehaviour class, watch out when you need to use the Unity events Start, OnEnable and OnDisable. As they are used by the base class, you will need to override them and call the super version.

- If you are not sure how to proceed, the samples folder have pretty clear implementations of topics discussed.
