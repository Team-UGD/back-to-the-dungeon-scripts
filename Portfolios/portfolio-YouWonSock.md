# Back to the Dungeon

## Developer Info
* 이름 : 유원석(You Won Sock)
* GitHub : https://github.com/youwonsock
* Mail : qazwsx233434@gmail.com

## Our Game
### Game trailer - Youtube

[![Back to The Dungeon Trailer](https://img.youtube.com/vi/hy_my0OQddc/0.jpg)](https://www.youtube.com/watch?v=hy_my0OQddc) 

### Downloads

* [itch.io](https://devslem.itch.io/back-to-the-dungeon)

### Genres

2D platformer shooting

<b><h2>Platforms</h2></b>

<p>
<img src="https://upload.wikimedia.org/wikipedia/commons/c/c7/Windows_logo_-_2012.png" height="30">
</p>

### Development kits

<p>
<img src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/19/Unity_Technologies_logo.svg/1280px-Unity_Technologies_logo.svg.png" height="40">
</p>

<b><h2>Periods</h2></b>

* 2021-08 ~ 2022-04 (about 9 months) - main development and build for the pc version
* 2022-04 ~ now - build for the mobile version

<b><h2>Contribution</h2></b> 


### Trailer
[![Back to The Dungeon Trailer](https://img.youtube.com/vi/hy_my0OQddc/0.jpg)](https://www.youtube.com/watch?v=hy_my0OQddc) 

### Contribution

### Enemy
* #### Entities
  * Astronaut
    * SelfExplosion
    * Bomb
      
    ![is](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/af955e55-ad8b-49af-8d2a-b58260acd575)
    자폭 몬스터로 플레이어 발견 시 빠른 속도로 접근합니다.  
    EnemyDetection에서 Target을 감지하고 있는경우 사망 시 그 자리에 폭탄을 생성하며,  
    감지한 Target이 없는 경우 폭탄을 생성하지 않습니다.
      
  * Beez
    * SpreadSkill  
      
    ![bee](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/6681e588-0dc5-4f4e-a7e6-c83a2db97445)
    비행 몬스터로 크기가 작으며 낮은 체력을 가지고 있습니다.  
    기본 원거리 공격과 SpreadSkill을 사용합니다.
    
  * Squirrel
    * DashSkill.cs - class
    ![다람이](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/f1ac8c76-9fa4-4217-ae65-296959d63bb6)
    돌진 스킬을 사용하는 몬스터입니다.  
    돌진 패턴만을 가지고 있으며 높은 데미지를 줍니다.  
    인스팩터에서 bool값을 이용하여 돌진 시 낭떠러지에서 떨어질지 반대로 돌진할지 설정해줄 수 있습니다.
    
* #### ETC
  * Enemy Health Bar
    ![Spread](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/d6353317-ddb8-4127-be28-91e6eeaca546)
    일반 몬스터들의 체력을 표시해주는 HealthBar입니다.

  * Boss Health Bar
    ![BossHealth-min](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/5b955525-8ff5-433c-bb73-37ad0605d0b6)
    특수 몬스터의 체력을 표시해주는 HealthBar입니다.

### Enemy Skills
* #### Boss Skill
  * BossSpreadSkill.cs - class
    ![Spread](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/2a434c60-2f95-43e2-851b-e1009d5b7356)
    SpreadSkill의 강화형으로 4방향으로 발사하면서 회전합니다.
    
### Item
* #### Field Item
  * Coin
    ![coin](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/35c17f3a-fcda-4e80-8e3e-43a7439da265)  
    
  * HealPotion
    ![potion](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/5b80709d-9164-4440-92e2-0311a757bf1a)  
    
  * Invincible
    ![invin](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/5db34b85-9067-47ca-9979-d1d431f310ef)  
    
* #### Store Item
  * IncreaseMaxHealth
    ![max health](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/0af8e66f-14c3-43fc-828e-a7271f78e617)  

  * IncreaseStamina
    ![max Ste](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/38aae36f-9eaf-47f4-8370-1f9b42ce6641)  

  * Resurrection
    ![re](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/ef0f4ed8-3001-4e64-a24b-84ae5f0bf6a7)

### Manager
* ETC
  * ItemManager
    아이템 생성을 위한 클래스로 Stage내에 존재하는 몬스터들의 OnDeath 이벤트에  
    확률에 따라 아이템을 생성하는 메서드 OnEnemyDeath를 등록합니다. 
    
  * UIManager
    UI를 관리하는 Singleton오브젝트로 PlayerUI, GameObjectUI, PauseUI등의 Ui처리를 위한 매니저입니다.
    
  * SaveManager
    세이브 데이터 관리

### Other Objects
* #### Interation Objects  
  * Door and Switch  
  ![door](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/9ceaff02-a56d-43d0-9d1c-c5c56e06ee61)
  
* #### Trap
  * BossEventTrigger
    ![bosseven](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/883a19e0-751d-4b48-bad4-83449798741d)  

### Player
  * 체력 및 사망처리
    
### UI
  * QuitButton
  * SettingButton
  ![ui](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/496ab77b-bb31-4883-92ba-72469a7a71b8)

### Weapon
* #### PlayerWeapon
  * Weapon
    * AssaultRifle
      ![ak](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/6a0058a7-3b47-44e7-879f-2df2810fc0c5)
  
    * AutoShutGun
      ![autoshot](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/96a8bb8f-9965-4c94-a336-df6dc9271d83)
  
    * AWP
      ![awp](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/c04c358e-68c8-409f-8a91-cd7ed655fcd8)
  
    * BurstRifle
      ![m16](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/1dcd52a1-3564-48a3-b75e-71318532b2db)
  
    * Cannon
      ![cannon](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/010f417c-de69-4069-b4e7-7f4422dbd1ae)
  
    * Minigun
      ![minigun](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/d6524d28-41b7-450d-8fdb-8705eb08fdba)
  
    * Pistol
      ![HandGun](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/deb734f9-4563-4fb0-b966-5982ec287998)
  
    * ShotGun
      ![shotgun](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/4f7bb0ca-a397-4b84-9ac7-686b069c0f6b)
  
    * Smg
      ![smg](https://github.com/youwonsock/back-to-the-dungeon-scripts/assets/46276141/8c0f99a0-b5e5-4811-804b-df52adc9665d)  

