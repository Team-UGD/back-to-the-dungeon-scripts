# Back to The Dungeon

We share scripts used for "Back to The Dungeon" project.

## Game overview

### Game trailer - Youtube

[![Back to The Dungeon Trailer](https://img.youtube.com/vi/hy_my0OQddc/0.jpg)](https://www.youtube.com/watch?v=hy_my0OQddc) 


## Scripts overview

### External Library

> Note: If you wanna use our scripts, you must import external libraries below.

* [A* Pathfinding](https://arongranberg.com/astar/)

### Implemented systems

We've implemented almost systems using only basic libraries provided by C# and Unity. [A* Pathfinding](https://arongranberg.com/astar/) as external library has been only used.

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

It's only used by enemies.  

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

We implement a lot of interactive game objects like portal, trap, etc.

#### Save System

* Manager(only serves API)
* Listener(interface for synchronization)

#### Utility

* Physics
* Math
* Singleton
* Attributes(for unity editor)

### Feedback

개선해야할 점들을 기록함.

* 예외 처리 더 사전에 처리할 필요 있었음.
* 모듈화를 조금 더 했어야함.
* 유니티 컴포넌트를 조금 더 활용을 할 필요 있었음.
* 스킬 시스템이 빈약했음.
* 인터페이스 너무 부족함.
* 추상멤버들을 더 많이 정의해 다형성을 적극적으로 활용했어야함.
* 이벤트를 더 적극적으로 사용할 필요 있었음.
* 무기 시스템이 빈약했음.
* 코드 문서화가 덜 됐음.
* 비동기 프로그래밍 사용을 해봤었으면 좋았을 듯.
* 인풋 장치 확장이 용이하지 못했음.
* 코드 관련 패키지들을 미리 고려할 필요가 있었음.
* 설계를 제대로 할 필요 있었음.
* 코드 유지 보수가 용이하지 못했음.
* 함수 기능을 조금 더 세분화할 필요 있었음.
* 디버그 로그 메세지 형식 통일 필요했었음.
* 깃 버전 관리를 개판으로 했음.
* 깃 ignore를 통해 파일 걸러낼 필요 있었음.
* 디렉토리 관리를 개판으로 함.
* Scene 내 오브젝트 조직화가 잘 이루어지지 못했음.

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
