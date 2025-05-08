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

