<b><h1>Back To The Dungeon Portfolio</h1></b> <br>
Hi! I'm Hyeonseo. I'm happy to start our first project when i was 21. I was really satisfied to come up with a fresh new idea and make that into my code on my own. We have had a lot of success and learned a lot while falling down, getting back on out feet. I will use this experience as a springboard and go further. <br>

<b><h3>Developer</h3></b>
- Name: KoHyeonSeo
- Github: https://github.com/KoHyeonSeo
- Contact: <a href=mailto:rhgustj01@naver.com><img src="https://img.shields.io/badge/-Naver-brightgreen?style=flat-square&logo=Naver&logoColor=white&link=mailto:rhgustj01@naver.com"
style="height : auto; margin-left : 10px; margin-right : 10px;"/> or </a>
<a href=mailto:rhgustj310@gmail.com><img src="https://img.shields.io/badge/Gmail-d14836?style=flat-square&logo=Gmail&logoColor=white&link=mailto:rhgustj310@gmail.com"
style="height : auto; margin-left : 10px; margin-right : 10px;"/>
</a>

<b><h2>Our Game</h2></b>
### Game trailer - Youtube

[![Back to The Dungeon Trailer](https://img.youtube.com/vi/hy_my0OQddc/0.jpg)](https://www.youtube.com/watch?v=hy_my0OQddc) 

<b><h2>Downloads</h2></b>

* [itch.io](https://devslem.itch.io/back-to-the-dungeon)

<b><h2>Genres</h2></b>

2D platformer shooting

<b><h2>Platforms</h2></b>

<p>
<img src="https://upload.wikimedia.org/wikipedia/commons/c/c7/Windows_logo_-_2012.png" height="30">
<!--<img src="https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Android_logo_2019_%28stacked%29.svg/640px-Android_logo_2019_%28stacked%29.svg.png" width="40">-->
</p>

<b><h2>Development kits</h2></b>

We've used **C#** and **Unity** game engine.

<p>
<img src="https://w.namu.la/s/a5c8b52bd00f38f3430dd7540867240527fd91e023abc9ff5afc7612faaf0ff3d089ebc7d17fd742323e15a32383753a3777de02ec664a6e15b0e92847220dc47f2be0a379d83dfb0a437a75ee6b2f63e63bbc1106ffb05877c5ccac54f45b22" height="40">
<img src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/19/Unity_Technologies_logo.svg/1280px-Unity_Technologies_logo.svg.png" height="40">
</p>

<b><h2>Periods</h2></b>

* 2021-08 ~ 2022-04 (about 9 months) - main development and build for the pc version
* 2022-04 ~ now - build for the mobile version

<b><h2>Contribution</h2></b> 
<b><h3>Enemy Detection</h3><b>
- I built AI with Unity function. That is Enemy Detection.
- The enemy can detect player, cliffs, and walls.
- Even if the enemy detect the player, you do not chase if there is a wall in between. 
- Also, the enemy does not give up chasing for a certain period of time, even if the player hides behind the wall.
 > ![EnemyDetection_detect the cliffs](https://user-images.githubusercontent.com/76097749/166153565-8c466644-9ae3-4ffc-9b0b-99710e6e94a3.gif)
 > ![EnemyDetection_PlayerDetection](https://user-images.githubusercontent.com/76097749/166153214-cab7e055-1244-4baf-8149-b3c2458b4b1c.gif)

<b><h3>Movement</h3><b>
- PlayerMovement
  > - The player can use long jumps and double jumps.
  > - The player can also move from side to side and jump down.

- EnemyMovement
  > - I used the FSM.
  > - I built the basic movement of the flying enemy, the basic movement of the walking enemy, and the chasing movement of the enemy.
  >> ![BasicMovement](https://user-images.githubusercontent.com/76097749/166153230-8ac301f6-f8ce-49ae-955b-ec5680e276ab.gif)


<b><h3>Trap</h3><b>
- Falling Platform
  > ![Trap_FallingGround](https://user-images.githubusercontent.com/76097749/166153251-ab4d2b63-a9ce-4168-b7c4-a949bfb5e080.gif)

- Passable Platform
  > ![passableTile](https://user-images.githubusercontent.com/76097749/166153258-2595f988-42cf-4a49-acc1-0cdcc6e2d98f.gif)

- Moving Ground Trap
  > ![Moving Ground](https://user-images.githubusercontent.com/76097749/166153262-a2b9f137-4aae-42f5-a638-86029fa358ec.gif)

- NonLinear Movable Spike
  > ![Trap_NonLinearMovableSpike](https://user-images.githubusercontent.com/76097749/166153265-7bfa13f7-5203-4dde-93b7-1b6b1a2be206.gif)

- Linear Movable Spike
  > ![Linear](https://user-images.githubusercontent.com/76097749/166153273-718b9402-2db0-499a-9c51-f793f4b05836.gif)

<b><h3>Utility</h3><b>
- I studied the movement of soft objects.
- I contributed to smooth object movement by solving the equation of the Bezier curve by coding and adding it to the Utility.
- My vezier formula is incorporated into many elements of the game.
  
<b><h3>Enemy Skill</h3><b>
- Big Ball Skill of Wizard
  > ![BigBallSkill](https://user-images.githubusercontent.com/76097749/166153277-43241b92-afdc-44e0-a548-1768d33c7f73.gif)

- Bounce Ball Skill of Boss
  > ![BounceBall](https://user-images.githubusercontent.com/76097749/166153280-db5c04b8-23ec-49ba-bd83-60f6dfa58776.gif)

- Sickle Grab Skill of Boss
  > ![SickleGrab](https://user-images.githubusercontent.com/76097749/166153282-09248cf3-d5a7-486f-a8cd-cac9819b0d19.gif)

- Smash Skill of Boss
  > ![BossSmash](https://user-images.githubusercontent.com/76097749/166153286-e15ffb6a-2a1b-44db-a344-54a79344a35d.gif)
  
<b><h3>UI</h3><b>
- Ending Credit
  > ![Ending](https://user-images.githubusercontent.com/76097749/166153297-2ee1fb99-3c94-4523-87d7-9cb61f18b4bc.gif)
  
- LevelSetting
  > I used "dispose pattern" to manage the data, and I used "using" to call the data in a temporary space and delete it automatically.
  >> ![UI_LevelSetting](https://user-images.githubusercontent.com/76097749/166153304-36d5a690-63e4-427a-9f72-f8efda3a1a97.gif)

