# Back to The Dungeon

It's the first project of **Team UGD**. We've been interested in the development of the game system more than the game itself. So the game quality, especially with regards to assets(e.g. graphics, motions, level design, ui, etc), may be rather low but it's really happy for us to implement most of game systems by our own efforts. We share scripts here used for "Back to The Dungeon" project.

## Game overview

### Game trailer - Youtube

[![Back to The Dungeon Trailer](https://img.youtube.com/vi/hy_my0OQddc/0.jpg)](https://www.youtube.com/watch?v=hy_my0OQddc) 


## Scripts overview

### Environment

* Unity Editor 2020.3.14f1

### External Library

> Note: If you wanna use our scripts, you must import external libraries below.

* [A* Pathfinding](https://arongranberg.com/astar/) - version: 4.2.15

### Implemented systems

We've implemented most of game systems using only basic libraries provided by C# and Unity. [A* Pathfinding](https://arongranberg.com/astar/) as external library has been only used.

#### Entity

* Health
* Damage

#### Player

* Input
* Movement
* Attack(shooting)

#### Enemy

* Target Detection
* Movement
* Pathfinding(We use [A* Pathfinding](https://arongranberg.com/astar/) asset and implement our own pathfinding logic by using its API in the 2D platformer game.)
* Attack(by using skills)

#### Weapon

* Guns
* Bullets
* Melee Weapon(only used for enemies)

#### Skill

We've implemented a lot of skills. Skill is only used by enemies.  

#### Item

It's only used for the player.

* Item
* Item Spawner

#### UI

* HUD
* etc(e.g. Store)

#### Manager

* Game Manager
* UI Manager

#### Game Object

We've implemented a lot of interactive game objects like portal, trap, etc.

#### Save System

* Manager(only serves API)
* Listener(interface for synchronization)

#### Utility

* Physics
* Math
* Singleton
* Attributes(for unity editor)

### Feedback

* Should have planned the system thoroughly in advance
* Poor exception control
* Lack of modularization 
* Lack of using unity basic components
* Poor skill system
* Lack of polymorphism
* Lack of interfaces
* Lack of events
* Poor weapon system
* Lack of code documentation
* Didn't use asynchronous programming with `async` and `await`
* Should have considered the extension of input devices
* Poor management of scripts
* Poor management of log messages
* Poor management of directories, project structure
* Bad git version management
* Poor organization of objects in unity scene

<!-- ## 스크립트 개요  

작성한 스크립트가 아래보다 더 많이 추가되어 더이상 기록하지는 못했음.

### 작성할 스크립트 분류 개요  

* [**Utility**](#utility)
* [**Manager**](#manager)
* [**Interface**](#interface)
* [**Entity**](#entity)
* [**Player**](#player)
* [**Enemy**](#enemy)
* [**Enemy Entity**](#enemy-entity)
* [**Enemy Skills**](#enemy-skills)
* [**Weapon**](#weapon)
* [**Item**](#item)
* [**Other Objects**](#other-objects)

#### Utility  
BezierMoveTool : IList\<BezierPath2> - class  [`kgmslem`](https://github.com/kgmslem)  
BezierMoveToolEditor : Editor - class  [`kgmslem`](https://github.com/kgmslem)  
ExtensionMethods - static class  [`kgmslem`](https://github.com/kgmslem)  
MoveToolAttribute : PropertyAttribute - class  [`kgmslem`](https://github.com/kgmslem)  
MoveToolAvailableAttribute : PropertyAttribute - class  [`kgmslem`](https://github.com/kgmslem)  
MoveToolDrawer : PropertyDrawer - class  [`kgmslem`](https://github.com/kgmslem)  
MoveToolEditor : Editor - class  [`kgmslem`](https://github.com/kgmslem)  
PhysicsUtility - static class  [`kgmslem`](https://github.com/kgmslem)  
ReflectionExtension - static class  [`kgmslem`](https://github.com/kgmslem)  
SaveSystem - static class  [`kgmslem`](https://github.com/kgmslem)  
ScenePopupAttribute : PropertyAttribute - class  [`kgmslem`](https://github.com/kgmslem)  
ScenePopupDrawer : PropertyDrawer - class  [`kgmslem`](https://github.com/kgmslem)  
SerializableDictionary : Dictionary - class  [`kgmslem`](https://github.com/kgmslem)  
Singleton - abstract class  [`kgmslem`](https://github.com/kgmslem)  

#### Manager  
FixedResolution - class  [`kgmslem`](https://github.com/kgmslem)  
GameManager : Singleton - class  [`kgmslem`](https://github.com/kgmslem)  
UIManager : Singleton - class  [`youwonsock`](https://github.com/youwonsock)  
ItemManager - class  [`youwonsock`](https://github.com/youwonsock)  

#### Interface
IAttackTime - interface  [`kgmslem`](https://github.com/kgmslem)  
IFade - interface  [`kgmslem`](https://github.com/kgmslem)  
ISkillFirePosition - interface  [`kgmslem`](https://github.com/kgmslem)  
IStrikingPower - interface  [`kgmslem`](https://github.com/kgmslem)  

#### Entity
Attacker - abstract class  [`kgmslem`](https://github.com/kgmslem)  
Enemy : Entity, IStrikingPower - abstract class  [`kgmslem`](https://github.com/kgmslem)  
Entity - abstract class  [`kgmslem`](https://github.com/kgmslem)  

#### Player
Hero : Entity - class  [`youwonsock`](https://github.com/youwonsock)  
PlayerInput - class  [`kgmslem`](https://github.com/kgmslem)  
PlayerMovement - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
PlayerShooter - class  [`kgmslem`](https://github.com/kgmslem)  [`youwonsock`](https://github.com/youwonsock)  
PlayerSingleton : Singleton - class  [`kgmslem`](https://github.com/kgmslem)  

#### Enemy 
AttackTrap : IStrikingPower - class  [`kgmslem`](https://github.com/kgmslem)  
EnemyAttacker - class  [`kgmslem`](https://github.com/kgmslem)  
EnemyAttackerEditor : Editor - class  [`kgmslem`](https://github.com/kgmslem)  
EnemyDetection - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
EnemyPathfinder - class  [`kgmslem`](https://github.com/kgmslem)  
EnemyPathfinderEditor : Editor - class  [`kgmslem`](https://github.com/kgmslem)  
EnemySkillCondition - class  [`kgmslem`](https://github.com/kgmslem)  
EnemyHealthBar - class  [`youwonsock`](https://github.com/youwonsock)  
FlyBasicMovement - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
FlyFollowState - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
FlyReadyState - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
WalkBasicMovement - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
WalkFollowState - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
WalkReadyState - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  

#### Enemy Entity
Beez : Enemy - class  [`jihyeong4565`](https://github.com/jihyeong4565)  
Boss : Enemy - class  [`kgmslem`](https://github.com/kgmslem)  
BringerOfDeath : Enemy - class  [`kgmslem`](https://github.com/kgmslem)  
FlyingEye : Enemy - class  [`kgmslem`](https://github.com/kgmslem)  
Ninja : Enemy - class  [`kgmslem`](https://github.com/kgmslem)  
Squirrel : Enemy - class  [`youwonsock`](https://github.com/youwonsock)  
Wizard : Enemy - class  [`kgmslem`](https://github.com/kgmslem)  
Zombie : Enemy - class [`kgmslem`](https://github.com/kgmslem)  

#### Enemy Skills  
Assassination : EnemySkill - class  [`kgmslem`](https://github.com/kgmslem)  
BigBallSkill : EnemySkill - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
BodyStatBuff : EnemySkill - class  [`kgmslem`](https://github.com/kgmslem)  
Bomb - class  [`youwonsock`](https://github.com/youwonsock)  
BossLaser : EnemySkill - class  [`youwonsock`](https://github.com/youwonsock)  
BossSmashSkill : EnemySkill - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
BossSpreadSkill : EnemySkill - class  [`youwonsock`](https://github.com/youwonsock)  
CloseAttackSkill : EnemySkill - class  [`kgmslem`](https://github.com/kgmslem)  
DashSkill : EnemySkill - class  [`youwonsock`](https://github.com/youwonsock)  
DoubleSwordSwing : SwordSwing - class  [`kgmslem`](https://github.com/kgmslem)  
EnemySkill : ScriptableObject - abstract class  [`kgmslem`](https://github.com/kgmslem)  
GrabSkill : EnemySkill - class  [`kgmslem`](https://github.com/kgmslem)  
NinjaSequentialShuriken : SequentialProjectileFire - class  [`kgmslem`](https://github.com/kgmslem)  
RangedAutoAttack : EnemySkill, ISkillFirePosition - class  [`kgmslem`](https://github.com/kgmslem)    
SelfExplosion - class  [`youwonsock`](https://github.com/youwonsock)  
SequentialProjectileFire : EnemySkill, ISkillFirePosition - class  [`kgmslem`](https://github.com/kgmslem)  
SickleGrab : EnemySkill - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
SingleSwordSwing : SwordSwing - class  [`kgmslem`](https://github.com/kgmslem)  
SpellSkill : EnemySkill - class  [`kgmslem`](https://github.com/kgmslem)  
SpreadSkill : EnemySkill - class  [`youwonsock`](https://github.com/youwonsock)   
SwordSwing : EnemySkill - class  [`kgmslem`](https://github.com/kgmslem)  
ThrowBoomerang : EnemySkill, ISkillFirePosition  [`kgmslem`](https://github.com/kgmslem)  
ThrowRotatedSword : EnemySkill, ISkillFirePosition  [`kgmslem`](https://github.com/kgmslem)  
TripleShuriken : EnemySkill, ISkillFirePosition - class  [`kgmslem`](https://github.com/kgmslem)  

#### Weapon  
AssaultRifle : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
AutoShotGun : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
AWP : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
Ball : Entity - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)   
BossHand - class  [`kgmslem`](https://github.com/kgmslem)  
BringerOfDeathSpell - class  [`kgmslem`](https://github.com/kgmslem)  
BringerOfDeathSword : MeleeWeapon - class  [`kgmslem`](https://github.com/kgmslem)  
Bullet - class  [`gisu1102`](https://github.com/gisu1102)   
BurstRifle : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
Cannon : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
Explosion - class  [`youwonsock`](https://github.com/youwonsock)  
ExplosionBullet : Bullet - class  [`youwonsock`](https://github.com/youwonsock)  
GrabbingSickle - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
MeleeWeapon - abstract class  [`kgmslem`](https://github.com/kgmslem)  
Minigun : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
PumpShotGun : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
Pistol : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
ReapingHook : MeleeWeapon, IAttackTime, IFade - class  [`kgmslem`](https://github.com/kgmslem)  
Smg : Weapon - class  [`youwonsock`](https://github.com/youwonsock)  
Weapon - abstract class  [`youwonsock`](https://github.com/youwonsock)  
WeaponChangeInfo : ScriptableObject - class  [`kgmslem`](https://github.com/kgmslem)  


#### Item  
Item - abstract class  [`youwonsock`](https://github.com/youwonsock)  
Coin : Item - class  [`youwonsock`](https://github.com/youwonsock)  
HealPotion : Item - class  [`youwonsock`](https://github.com/youwonsock)  

#### Other Objects  
AlwaysUseablePortal - class  [`youwonsock`](https://github.com/youwonsock)  
BgmPlayer - class  [`kgmslem`](https://github.com/kgmslem)  
ChapterClear - class  [`kgmslem`](https://github.com/kgmslem)  
DeadZone - class  [`kgmslem`](https://github.com/kgmslem)  
DisableOnEntityDeath - class  [`kgmslem`](https://github.com/kgmslem)  
DownPlatform - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
EndingCredit - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
EnemyStealthZone - class  [`kgmslem`](https://github.com/kgmslem)  
FallingObject - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
FullScreenBackground - class  [`kgmslem`](https://github.com/kgmslem)  
GameObjectGenerator - class  [`kgmslem`](https://github.com/kgmslem)  
LinearMovableObject - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
MovableGroundTrap - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
MovingGround - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
MoveToCustomPoint - class  [`youwonsock`](https://github.com/youwonsock)  
NonLinearMovableObject - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
PassableObject - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo) 
PlayerRestrictionArea - class  [`kgmslem`](https://github.com/kgmslem)   
Portal - class  [`jihyeong4565`](https://github.com/jihyeong4565), [`kgmslem`](https://github.com/kgmslem)  
RandomPosition - class  [`kgmslem`](https://github.com/kgmslem)  
RandomSpawner - class  [`kgmslem`](https://github.com/kgmslem)    
Rotator - class  [`kgmslem`](https://github.com/kgmslem)  
RecordBoard - class  [`KoHyeonSeo`](https://github.com/KoHyeonSeo)  
SaveArea - class  [`kgmslem`](https://github.com/kgmslem)  
Stage8DissolveEventTrigger - class  [`kgmslem`](https://github.com/kgmslem)  
Stage8FlameEventTrigger - class  [`kgmslem`](https://github.com/kgmslem)  
Stage8PortalEventTrigger - class  [`kgmslem`](https://github.com/kgmslem)  
Store - class  [`kgmslem`](https://github.com/kgmslem)  
StoreEditor : Editor - class  [`kgmslem`](https://github.com/kgmslem)  
StoreItemCountControl - class  [`kgmslem`](https://github.com/kgmslem)  
StoreItemSlot - class  [`kgmslem`](https://github.com/kgmslem)  
StoreUI - class  [`kgmslem`](https://github.com/kgmslem)  
TextMeshController - class  [`youwonsock`](https://github.com/youwonsock)  
UpdatePathfinderGraph - class  [`kgmslem`](https://github.com/kgmslem)  
 -->
