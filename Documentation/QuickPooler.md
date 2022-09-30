# Documentation

## ObjectPooler

### Public methods

#### Pop

Retrieves an object from the pool.

#### Add

Add a new object to the pool.

#### Return

Return to the pool an object that belongs to it.

### Actions

#### OnExpand

Triggers when the pool expands itself.

## PoolableMonobehaviour

### Public methods

#### Tick

For a poolable object that has its deactivation driven by "ticks" this method
decrements a tick. When there are no ticks left, the object is deactivated.

#### StartDeactivation

If a game object is deactivated immediately, stuff like visual effects, sound
effects and animations would not be activated. This method starts a process of
deactivation, which has its duration customizable to only when the time is
passed to finally disable the game object and return it to the pool.

An overload of the method let you override the object's original deactivation time.

#### DeactivateImmediate

Deactivates the object immediately.

### Actions

#### OnInitialize

Triggered when the object is initialized by the pool.

#### OnActivate

Triggered when the object is activated by the pool.

#### OnDeactivationProgrammed

Triggered when the object has its deactivation process initiated.

#### OnDeactivation

Triggered when the object has its game object deactivated.
