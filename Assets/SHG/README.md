## Tile Map generator class
```mermaid
classDiagram

  TileMapGenerator "x" --* "*" MapWalker: has
  TileMapGenerator "x" --> "*" Tile: use
  MapWalker "x" --* "1" Direction: has

  class TileMapGenerator {
    +float ChanceToCreateWalker
    +float ChanceToRedirect
    +float ChanceToRemoveWalker
    +Vector2Int MapSize
    +Vector2Int StartPoint

    -TwoDimentionalArray~Tile~ tiles
    -List~MapWalker~ walkers
    -void Generate()
    -void CreateFloors()
    -void RandomlyRemoveWalkers()
    -void RandomlyRedirectWalkers()
    -void RandomlyCreateWalkers()
    -Walker AwakeWalker(Vector2Int pos)
    -void ProgressWalkers()
    -void FillWalls()
  }

  class Tile {
    <<enumeration>>
    Empty
    Floor
    Wall
  }

  class MapWalker {
    +Direction Dir
    +Vector2Int Position
    +bool IsActive
    +void Redirect()
  }

  class Direction {
    <<enumeration>>
    Up
    Down
    Left
    Right
  }
```

> [알고리즘](https://bartshin.github.io/TIL/unity/2025.04.28_Resources%2CTile_map_generate/#tile-map-generate)   
## Manager class
```mermaid
classDiagram

  GameManager "x" --* "1" GameState
  GameManager "x" --> "1" StageManager: use
  GameManager "x" --> "1" UIManager: use
  GameManager "x" --> "1" SceneManager: use
  StageManager "x" --> "*" Monster: has
  UIManager "x" --* "1" PopupUI: has
  UIManager "x" --* "1" CombatUI: has
  CombatUI "1" --* "*" UIComponent: has
  SceneManager "x" --> "1" SceneName: use

  GameManager --<| Singleton
  UIManager --<| Singleton
  SceneManager --<| Singleton
  StageManager --<| Singleton
  PrefabObjectPool --<| Singleton
  
  class GameManager {
    GameState State;
    +bool IsPlaying
    +event OnGameStart
    +event OnGamePaused
    +event OnGameResumed

    +void SetPlayerCharacter(PlayerCharacter player)
  }

  class PlayerCharacter {
    +event OnDie
    +event OnLevelUp
  }

  class GameState {
    <<enumeration>>
    Title
    InCombat
    SelectItem
  }

  class StageManager {
    +int CurrentStage
    +event OnStageClear 
    +ID RegisterMonster(Monster monster)

    -Set~Monster~ CurrentStageMonsters
    -void GoToNextStage()
  }

  class Monster {
    +ID Id
    +event Action<ID> OnDie
  }

  class SceneManager {
    +void StartLoadScene()
    +void MoveToNextScene()
    +event Action<SceneName> OnSceneLoaded
  }

  note for SceneName "미리 Resources에 넣어둔 파일 이름"
  class SceneName {
    <<enumeration>>
    FirstScene
  }
  
  class UIManager {
    +Nullable~PopupUI~ CurrentPopup
    CombatUI CombatWindow
  }

  class PopupUI {
    <<enumeration>>
    Loading
    ItemSelect
    LevelUp
    StageEnd
  }

  class CombatUI {
    +UIComponent CharacterHpUI
    +UIComponent SkillListUI
  }

  class UIComponent {
    <<interface>>
  }

  class Singleton~T~ {
    +Singleton~T~ Shared$
    -void CreateInstnace()
  }

  class PrefabObjectPool {
    +void SetConfig(string prefabName, string path, int poolSize)

    +GameObject GetPooledObject(string prefabName)
    +void ReturnObject(GameObject pooledObject, string prefabName)
  }
```

### Game Manager
- 다른 매니저를 연결 시키고 게임의 상태를 관리하는 역할  
### Stage Manager
- 현재 스테이지 상태를 관리하고 스테이지를 진행시키는 역할  
### Scene Manager
- 상황에 맞는 scene을 로딩하고 전환하는 역할  
### UI Manager
- 게임 상태를 보여주고 사용자가 캐릭터, 아이템 등을 변경할 수 있게 하는 역할   
#### CombatUI
- 전투 플레이중 사용자에게 제공되는 UI   
#### PrefabObjectPool 
- Prefab에 따른 Object pool을 관리하는 역할   
- Resources 디렉토리에 넣어둔 prefab들을 생성할 수 있다   

---

## 게임 컨셉
<table>
<tr>
<td>
<a href="https://ko.wikipedia.org/wiki/%EB%B1%80%ED%8C%8C%EC%9D%B4%EC%96%B4_%EC%84%9C%EB%B0%94%EC%9D%B4%EB%B2%84%EC%A6%88">뱀파이어 서바이벌</a> <br/>    
플레이어는 2021년 이탈리아의 시골에서 괴물 군단의 보스 비스콘테 드라큘로에 대항하는 벨파에제 가문과 다른 영웅 생존자들을 조종한다. 플레이어 캐릭터는 적을 향해 자동으로 공격하며 사방에서 끊임없이 몰려오는 괴물들을 물리치며 30분동안 생존하는 것이 목표이다. 플레이 성과에 따라 무기, 캐릭터, 유물들이 해금된다.
</td>
<td>
<img src="https://upload.wikimedia.org/wikipedia/en/e/e6/Vampire_Survivors_key_art.jpg" style="min-width:140px;aspect-ratio:16/9">
</td>
</tr>
<tr>
<td>
<a href="https://play.google.com/store/apps/details?id=com.xq.archeroii&hl=ko&pli=1">궁수의 전설2 </a><br/>
로그라이크 체험 2.0: 독특한 스킬 희귀도 설정 및 다양한 스킬 선택 기회!
전투 체험 2.0: 더욱 빠르고 더욱 재밌게!
 스테이지 설계 2.0: 클래식한 스테이지와 신규 카운트다운 서바이버 콘텐츠 함께 등장!
대결 시스템 2.0: 한판의 승부? NO! 3판 2승제입니다. 진정한 용사는 실패 속에서 성공의 비법을 얻게 됩니다!
핵잼 던전 2.0: 보스 봉인전, 시련 타워, 골드 동굴 등 던전에서 보상을 많이 받아 가세요!
</td>
<td>
<img src= "https://i.ytimg.com/vi/7erz6d5yybM/maxresdefault.jpg"style="min-width:140px;aspect-ratio:16/9">
</td>
</tr>
</table>

## 구현 파트
### 필수
1. 플레이어
  - 이동
  - 공격
  - 스탯
  - 인벤토리
2. 몬스터
3. 맵
4. 스테이지 (스테이지가 끝나면 로딩후 다른 스테이지로 이동)

### 추가
1. 스킬 (플레이어를 강화)
2. 아이템 (스테이지 중간에 나오고 플레이어를 강화)
  - 레벨업, 스테이지 클리어시 획득
3. 스킬 창
4. 미로
5. 아이템 인벤토리
6. 디자인

## 구현 배분
- 신희관: 게임 전체 싱글톤, 게임 흐름
- 이정주: 몬스터 이동, 공격
- 송재오: 플레이어 이동, 스탯
- 이세준: 아이템, 인벤토리
- 최연호: 플레이어 공격


