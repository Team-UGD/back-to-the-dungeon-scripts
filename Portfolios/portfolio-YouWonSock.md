# Back to the Dungeon

개발 기간 : 2021~2022
개발 팀 : [Team UGD](https://github.com/orgs/Team-UGD/teams)

### Developer Info
이름 : 유원석(You Won Sock)
연락처 : qazwsx233434@gmail.com
GitHub : https://github.com/youwonsock

### Game Info

**Genres :** 2D platformer shooting
**Platforms :** Windows
**Development kits :** Unity
**Downloads :** [itch.io](https://devslem.itch.io/back-to-the-dungeon)

### Trailer
[![Back to The Dungeon Trailer](https://img.youtube.com/vi/hy_my0OQddc/0.jpg)](https://www.youtube.com/watch?v=hy_my0OQddc) 

### Contribution

### Enemy
* #### EnemyMovement
  * DashState.cs - class
* #### Entities
  * Astronaut.cs - class
    > 자폭 몬스터로 플레이어 발견 시 빠른 속도로 접근하며 
      EnemyDetection에서 Target을 감지하고 있는경우 사망 시 그 자리에 폭탄을 생성합니다.
      감지한 Target이 없으면 폭탄을 생성하지 않습니다.
      
  * Beez.cs - class
    > 비행 몬스터로 크기가 작으며 낮은 체력을 가지고 있습니다.
      짧은 사거리를 가지고 있습니다.
  * Squirrel.cs - class
    > 돌진 스킬을 사용하는 몬스터입니다. 
      돌진 패턴만을 가지고 있으며 높은 데미지를 줍니다.
      인스팩터에서 bool값을 이용하여 돌진 시 낭떠러지에서 떨어질지 
      반대로 돌진할지 설정해줄 수 있습니다.
* #### ETC
  * EnemyHealthBar.cs - class
    > 일반 몬스터들의 체력을 표시해주는 HealthBar에 추가되는 컴포넌트로 
      부모 오브젝트의 스케일값이 바뀌더라도 방향을 유지하기 위해 
      Update에서 스케일값을 조정해줍니다. 

### Enemy Skills
* #### Projectile Skill
  * SpreadSkill.cs - class
    > 범위 공격으로 몬스터의 현재 위치를 기준으로 8방향을 공격합니다.
      기본 공격과 같은 방식으로 동작하지만, 미리 정의해둔 방향으로 8번 반복합니다.
* #### Boss Skill
  * BossSpreadSkill.cs - class
    > SpreadSkill의 강화형으로 상하좌우 4방향으로 발사하면서 회전합니다.
      반복문을 통해 탄환을 발사하면서 발사하는 방향을 바꿔주어 회전시킵니다. 
* #### Melee Skill
  * Bomb.cs - class
    > 자폭 몬스터가 사망 시 생성되는 폭탄으로 지정한 시간 뒤 Destory되며
     SelfExplosion Prefab을 생성하는 방식으로 구현하였습니다. 
  * DashSkill.cs - class
    > 돌진 몬스터가 사용하는 돌진 스킬로 스킬 사용 시점에 플레이어가 있는 방향으로 돌진합니다.
      TriggerSkill의 매개변수로 전달된 정보를 이용하여 플레이어가 있는 위치를 얻습니다.
  * SelfExplosion.cs - class
    > 자폭 몬스터가 플레이어에게 닿았을 경우와 폭탄 폭발 시 생성되는 SelfExplosion Prefab을
      생성하기 위한 컴포넌트입니다.
     
### Item
* #### Field Item
  * Coin.cs - class
    > GamaManager에서 관리 중인 인게임 재화 Gold를 지정해둔 값만큼 증가시킵니다.
  * HealPotion.cs - class
    > Hero.cs에서 관리 중인 Health을 증가시킵니다. 
  * Invincible.cs - class
    > 무적 아이템으로 획득 시 player의 layer를 변경해줍니다. 
* #### Store Item
  * IncreaseMaxHealth.cs - class
    > 구매 시 Hero.cs에서 관리 중인 MaxHealth를 증가시킵니다.
  * IncreaseStamina.cs - class
    > 구매 시 Hero.cs에서 관리 중인 MaxStamina를 증가시킵니다.
  * Resurrection.cs - class
    > 구매 시 Hero.cs에서 관리 중인 Resurrection Chance를 True로 바꾸어 1회 부활이 가능합니다.
* #### ETC
  * Item.cs - abstract class
    > 모든 아이템의 추상 클래스로 아이템 획득 시 실행되는 추상 메서드 GetItem()과
      OnCollisionEnter2D가 정의되어있습니다.
### Manager
* ETC
  * ItemManager.cs - class
    > 아이템 생성을 위한 클래스로 Stage내에 존재하는 몬스터들의 OnDeath 이벤트에 
      확률에 따라 아이템을 생성하는 메서드 OnEnemyDeath를 등록합니다. 
  * UIManager.cs : Singleton - class
    > UI를 관리하는 Singleton오브젝트로 PlayerUI, GameObjectUI, PauseUI등의 Ui처리를 위한 
      메서드, 자료구조등을 관리하는 메서드입니다. 

### Other Objects
* #### Interation Objects
  * Door.cs - class
  * Switch.cs - class
* #### Portal
  * AlwaysUseablePortal.cs - class
* #### Trap
  * BossEventTrigger.cs - class
  * MoveToCustomPoint.cs - class

### Player
  * Hero.cs - class

### Save System
  * SaveManager.cs - class

### UI
  * QuitButton.cs - class
  * SettingButton.cs - class

### Weapon
* #### PlayerWeapon
  * AssaultRifle.cs - class
  * AutoShutGun.cs - class
  * AWP.cs - class
  * BurstRifle.cs - class
  * Cannon.cs - class
  * Minigun.cs - class
  * Pistol.cs - class
  * PubpShotGun.cs - class
  * Smg.cs - class
* #### ETC
  * Bullet.cs - class
  * Explosion.cs - class
  * ExplosionBullet.cs - class
  * Weapon.cs - abstract class 