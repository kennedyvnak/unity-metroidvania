# Metroidvania versions changes

## 1.2.6-a.2

- Upgrade Unity to 2021.3.3f1
- Add Unity Addressables system
- Add Entities (enemies) system
- Fix code comments and typos
- Add singleton ScriptableObject behaviour called ScriptableSingleton\<T>

----

- Add CHANGELOG.md
- Add EntityTarget component
- Change collision layer matrix
- Update project settings
- Add player layer
- Add object to handle gizmos colors and struct to handle gizmos drawing
- Add attribute to handle ScriptableObjects singletons in resources
- Add Editor class to handle project validation
- Add ScriptableObject to handle a entity instance
- Add ScriptableObject to handle all entities

## 1.3.0-a.1

- Add Audio system
- Add menus (main title, pause and options (settings management))
- Add Unity Localization system
- Add TextMesh Pro font
- Update project structure

----

- Add UI Fields
- Change sorting layers
- Add texture importer
- Add RangedFloat/Int fields with custom editor drawer
- Removed default TextMesh Pro assets

## 1.3.1-a.2

- Add Serialization system
- Improve Main Menu
- Add a custom Debugger called GameDebugger
- Add UI utility class
- Create UI table entries for all labels in main menu
- Add DOTween package

----

- Improve markdown files structure
- Add Mein Menu to gameplay transition
- Add logs on project validator
- Add UI units prefabs
- Renamed scene 'SampleScene' to 'Level0'

## 1.3.1-a.5

- Add a .save file importer and editor
- Add default game data asset

----

- DataHandler encryption now in public
- Set Game Debugger to non editor-only
- Improve files formatting

## 1.3.2-a.4

- Add GameManager class for handle pause
- Add pause menu
- Add placeholder menu scene transition
- Add support for menus in gameplay
- Add support to return (ESC) key

----

- Add Gameplay Asset Group
- Update "Base Scene" sorting layers (player, ground and undead executioner)
- Update Input actions
- Add "MainCanvas" tag

## 1.3.2-a.6

- Add credits screen
- Add return key perform in main menu
- Fix members names
- Add text hyperlinks support

----

- Add CameraUtility class
- Update TextMesh Pro settings pats

## 1.3.4-a.10

- Upgrade all project to unity addressables
- Add Scene control
- Update project folders and scenes hierarchy structure
- Normalize files names
- Create scriptable objects to handle events
- Add menu handling to inputActions

----

- Rename "Plugins" folder to "Addons"
- Add GameInitializer
- Set GameManager as MonoBehaviour
- Set DataManager as ScriptableObject
- Change ScriptableSingleton instance load behaviour
- Fix some typo
- Create player prefab
- Rename "Textures/Enemies" to "Textures/Entities"

## 1.3.4-a.12

- Add project validator window
- Add scriptable singletons validator
- Change GameDebugger path

## 1.3.5-a.2

- Add channels event tracking system
- Add handler to log event tracking in a file

----

- Add game initialization in editor

## 1.4.0 (1.4 will focus on improving player behaviours)

- Update playerstates.cs file structure
- Add dynamic player collision
- Improve crouch states
- Improve jump

> 1.4.0.1

- Add metadata and modules to states
- Add previous state param to PlayerState.Enter
- Improve crouch states
- Add validator state to player state machine

> 1.4.0.2

- Improve player attacks
- Update animator play mode (changed from string to hash)
- Add cooldown module

## 1.4.1

- Add wall slide/jump states
- Improve jump
- Update hand collision check
- Improve code readability
- Improve player input
- Remove unused player assets

## 1.4.2

- Add player particles
- Update wall detection
- Add fall distance detection

> 1.4.2.1

- Improve input
- Add gamepad support
- Update options screen
- Add animations in UI behaviours
- Update toggle field checkmark display
- Improve keyboard/gamepad UI navigation

## 1.4.3

>- Features

- Add player health bar
- Add Fade Screen
- Add game over
- Add delete save button in save menu
- Add runtime fields (scriptable object that contains a variable)
- Add scene templates
- Add player safe points (with custom editor)
- Add Serializable GUID struct
- Add Singleton Pattern

>- Fixes

- Fix void event channels tracking
- Fix player particles sorting layer
- Fix UI is not disabled when fading a group

>- Changes

- Rename scripts folder (_Scripts to Scripts)
- Change data save mode
- Change damage type from int to float
- Change game assets initialization to a generic method
- Change vertical selection group (now it just override the up/down selections)
- Improve camera follow
- Improve scene management
- DataManager now is a singleton
- Run code cleanup
