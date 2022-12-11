# Metroidvania versions changes

## 1.5

### Added

- Added "Characters" addressable group
- Added "Archer" character raw assets

### Changed

- Abstract characters behaviour
- Renamed Player assets to Character/Knight assets
- Improved Knight collision properties

----

## 1.4.4.1 ([90def17](https://github.com/kennedyvnak/metroidvania/commit/90def1762509b34c5276ab30b35dc0a21f1c76f6))

### Added

- Added 'The Pale Moonlight' environment
  - Graves, Statues, Tombs, Trees and Bushes
  - Ground tileset (with rule tile and normal maps)
- Added parallax effect
- Added player normal map and emission light
- Added stars particles
- Added window foliage shader and glow sprite shader
- Added VFX package
- Added ground and dense Fog effect

### Changed

- Renamed all assets to add their type suffix _[Data, Scene, Event...]
- Removed audio assets
- Reseted prefabs assets positions
- Improve camera settings
- Improve skeleton behaviour code readability
- Integrated pathfinder renderer directly in component

### Fixed

- Fixed player jump cancel / removed jump ground delay
- Fixed most of skeleton bugs
- Fixed safe areas gizmos color

## 1.4.4 ([4c5225b](https://github.com/kennedyvnak/metroidvania/commit/4c5225b437e595478705e02d1ef3ae2394a9cb16) to [61ca197](https://github.com/kennedyvnak/metroidvania/commit/61ca1974384f25df401d82b289c475f286a149e2))

### Added

- Added first IA (Skeleton)
- Added Entity Target Finder component
- Added post processing
- Added skeleton animations and materials
- Added glow shader
- Added screenshot capturer

### Changed

- Updated 'Dependencies' Addressable group
- Updated Player attack combo mode
- Increased player speed and first player attack force
- Changed gizmosColor singleton structure
- Changed PathfinderRenderer.DrawGizmos storage mode
- Changed Pathfinder behaviour to singleton
- Updated Path start and end points

----

## 1.4.3 ([4d9dc60](https://github.com/kennedyvnak/Metroidvania/commit/4d9dc6036db22e18ac916eff7a2bc071cc7b3242) to [475870d](https://github.com/kennedyvnak/Metroidvania/commit/475870da3667f15e3bea7f9c71e2385b0c061c30))

### Added

- Added pathfinding
- Added player health bar
- Added Fade Screen
- Added game over
- Added delete save button in save menu
- Added runtime fields (scriptable object that contains a variable)
- Added scene templates
- Added player safe points (with custom editor)
- Added Serializable GUID struct
- Added Singleton Pattern
- Added player fake walk state

### Fixed

- Fixed void event channels tracking
- Fixed player particles sorting layer
- Fixed UI is not disabled when fading a group

### Changed

- Renamed scripts folder (_Scripts to Scripts)
- Changed data save mode
- Changed damage type from int to float
- Changed game assets initialization to a generic method
- Changed vertical selection group (now it just override the up/down selections)
- Improved camera follow
- Improved scene management
- DataManager now is a singleton
- Run code cleanup

----

## 1.4.2.1 ([a8d32eb](https://github.com/kennedyvnak/Metroidvania/commit/a8d32eb9f4efe7df6a0f7cbc31cfeb4ffb857cc2) [7e8e0e5](https://github.com/kennedyvnak/Metroidvania/commit/7e8e0e5077afe76d0969388e8e4720729132d083))

### Added

- Added gamepad support
- Added animations in UI behaviours

### Changed

- Improved input
- Updated options screen
- Updated toggle field checkmark display
- Improved keyboard/gamepad UI navigation

## 1.4.2 ([dc8a166](https://github.com/kennedyvnak/Metroidvania/commit/dc8a166de14d52eb796e859fe6fa58cb33cfacfb) to [9c685c0](https://github.com/kennedyvnak/Metroidvania/commit/9c685c028395a8e408362c98e08100f09a686206))

### Added

- Added player particles
- Added fall distance detection

### Changed

- Updated wall detection

----

## 1.4.1 ([68d9fc8](https://github.com/kennedyvnak/Metroidvania/commit/68d9fc84d046b6d350eae589c5e5e5d31d76cf10) to [db68e7a](https://github.com/kennedyvnak/Metroidvania/commit/db68e7a55371d1abf25d69d2f501f59e97605c6c))

### Added

- Added wall slide/jump states

### Changed

- Improved jump
- Updated hand collision check
- Improved code readability
- Improved player input
- Removed unused player assets

----

## 1.4.0.2 ([b93c28f](https://github.com/kennedyvnak/Metroidvania/commit/b93c28f31a96ae9be44b6aa37e97c6da0a50fed3) to [daf6535](https://github.com/kennedyvnak/Metroidvania/commit/daf65352dd4ffb9aa3d61effe273589ef5513402))

### Added

- Added player cooldown module

### Changed

- Improved player attacks
- Updated animator play mode (changed from string to hash)

## 1.4.0.1 ([73e6334](https://github.com/kennedyvnak/Metroidvania/commit/73e63342f5a0be124d9b2b713647f758661f8e4c) to [0310dbb](https://github.com/kennedyvnak/Metroidvania/commit/0310dbb8b783e8b8296d6c0db3c7177dc3c9f8d8))

### Added

- Added metadata and modules to states
- Added previous state param to PlayerState.Enter
- Added validator state to player state machine

### Changed

- Improved crouch states

## 1.4.0 ([c552450](https://github.com/kennedyvnak/Metroidvania/commit/c552450a84082d8da5c29e485cd86c443447ccde) to [f78ec54](https://github.com/kennedyvnak/Metroidvania/commit/f78ec54c5e727aad4c780eb751fba0a809186275))

> 1.4 will focus on improving player behaviours

### Added

- Added dynamic player collision

### Changed

- Updated playerstates.cs file structure
- Improved crouch states
- Improved jump

----

## 1.3.5-a.2 ([2dd4001](https://github.com/kennedyvnak/Metroidvania/commit/2dd4001dbd6d170f002511626b41e1d5b0460571) to [51af82d](https://github.com/kennedyvnak/Metroidvania/commit/51af82d837dd5bac0b31d46c8946f4a20d358747))

### Added

- Added channels event tracking system
- Added handler to log event tracking in a file
- Added game initialization in editor

----

## 1.3.4-a.12 ([2144e83](https://github.com/kennedyvnak/Metroidvania/commit/2144e83d7208314989c0960990db642cda2877f5) to [502db0c](https://github.com/kennedyvnak/Metroidvania/commit/502db0c248dd43706f5585802ce4d33daeaccffd))

### Added

- Added project validator window
- Added scriptable singletons validator view

### Changed

- Changed GameDebugger singleton path

## 1.3.4-a.10 ([cc766d0](https://github.com/kennedyvnak/Metroidvania/commit/cc766d06207bfdd2261408d2533cdba7c8a17fb6))

### Added

- Added Scene control
- Added GameInitializer
- Added menu handling to inputActions
- Created scriptable objects to handle events
- Created player prefab

### Fixed

- Fixed some typo

### Changed

- Upgraded all project to unity addressables
- Updated project folders and scenes hierarchy structure
- Normalized files names
- Renamed "Plugins" folder to "Addons"
- Set GameManager as MonoBehaviour
- Set DataManager as ScriptableObject
- Changed ScriptableSingleton instance load behaviour
- Renamed "Textures/Enemies" to "Textures/Entities"

----

## 1.3.2-a.6 ([829daf9](https://github.com/kennedyvnak/Metroidvania/commit/829daf9b50f049f2daaa1c8e2e710526d3cab3d6))

### Added

- Added credits screen
- Added return key perform in main menu
- Added CameraUtility class
- Added text hyperlinks support

### Fixed

- Fixed members names

### Changed

- Updated TextMesh Pro settings paths

## 1.3.2-a.4 ([a774918](https://github.com/kennedyvnak/Metroidvania/commit/a774918ad8c29ff138a87e03f11ad78f8d7c8fd9))

### Added

- Added GameManager class for handle pause
- Added pause menu
- Added placeholder menu scene transition
- Added support for menus in gameplay
- Added support to return (ESC) key
- Added Gameplay Asset Group
- Added "MainCanvas" tag

### Changed

- Updated "Base Scene" sorting layers (player, ground and undead executioner)
- Updated Input actions

----

## 1.3.1-a.5 ([7ed2a3a](https://github.com/kennedyvnak/Metroidvania/commit/7ed2a3ae29b6647b163dcff2d103e698bf348a65) to [cc44635](https://github.com/kennedyvnak/Metroidvania/commit/cc44635d2dffedb899e9a60c34b323f3a9fcd5ae))

### Added

- Added a .save file importer and editor
- Added default game data asset

### Changed

- Changed DataHandler encryption method public
- Allowed Game Debugger to run in builds
- Improved c# code formatting

## 1.3.1-a.2 ([6cb68d2](https://github.com/kennedyvnak/Metroidvania/commit/6cb68d2bcdb19b53782e40a81dcb7fcef5f81c22))

### Added

- Added Serialization system
- Added custom Debugger
- Added UI utility class
- Added DOTween package
- Added Mein Menu to gameplay transition
- Added logs on project validator
- Added UI units prefabs
- Create UI table entries for all labels in UI

### Changed

- Improved Main Menu
- Improved markdown files structure
- Renamed scene 'SampleScene' to 'Level0'

----

## 1.3.0-a.1 ([9897037](https://github.com/kennedyvnak/Metroidvania/commit/98970376b4639eee4c7e4963b550e084ee1eed28))

### Added

- Added Audio system base
- Added menus (main title, pause and options (settings management))
- Added Unity Localization system
- Added TextMesh Pro font
- Added UI Fields (buttons, sliders...)
- Added texture importer preset
- Added RangedFloat/Int fields with custom editor drawer

### Changed

- Updated project structure
- Updated sorting layers
- Removed default TextMesh Pro assets

----

## 1.2.6-a.2 ([571e1e9](https://github.com/kennedyvnak/Metroidvania/commit/571e1e9212d0a37a8abb8edaad14b67cd37b2b56) to [6c86c95](https://github.com/kennedyvnak/Metroidvania/commit/6c86c9589262491db59be6433d478bd23ed6b667))

> First logged version

### Added

- Added CHANGELOG.md
- Added Unity Addressables system
- Added Entities (enemies) system
- Added EntityTarget component
- Added singleton ScriptableObject behaviour
- Added player collision layer
- Added object to handle gizmos colors and struct to handle gizmos drawing
- Added attribute to handle ScriptableObjects singletons in resources
- Added Editor class to handle project validation
- Added ScriptableObject to handle a entity instance
- Added ScriptableObject to handle all entities

### Fixed

- Fixed code comments and typos

### Changed

- Upgraded Unity to 2021.3.3f1
- Updated collision layer matrix
- Updated project settings
