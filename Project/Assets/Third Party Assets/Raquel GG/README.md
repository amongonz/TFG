# README
Unity version required: 2021.3.11
Dependencies from Unity Registry:
- Cinemachine 2.8.9
- Input System 1.4.3
- TextMeshPro 3.0.6

## Import
1. Open an Unity Project
2. Asset>Import Package>Custom package
3. If not import dependencies automatically:
	1. Open Asset Manager>Unity Registry
	2. Import cinemachine, inputsystem and textmeshpro

## Asset Content
- Animations: animationController, avatar and animation clips
- DemoScene: demo to try the character.
- Input: InputAction
- Materials
- Models
- Prefabs
- Scripts
- SFX
- Terrain: for the demo
- Textures

## Prefabs Scheme
```
Character
|_ Ninja
|	|_ Models
|	|_ Rig
|
|_ WeaponManager
|	|_ Katana
|	|_ Kunai
|
|_ CameraManager
|	|_ 3rdPersonCam
|	|_ TopCam
|
|_ SoundManager
|_ CanvasHUD
```
### Ninja
Character model.

Components:
- CharacterController
- Animator
- PlayerController.cs
- Health.cs
- Stamina.cs
- Input.cs
- PlayerInput: InputActions asset in Actions option

### Katana and Kunai
Weapon models.

Components:
- Capsule Collider: Trigger option enable.
- Weapon.cs

### WeaponManager
Contain the katana and kunai prefabs.

Components:
WeaponManager.cs

### TopCam and 3rdPersonCam
Cinemachine Virtual Cameras that are configurate.

Components:
- CinemachineVirtualCamera: Add the ninja in Folloy and LookAt options.
- CinemachineInputProvider

### CameraManager
Have the TopCam, 3rdPersonCam and the main camera. The main camera have the component CinemachineBrain

Components:
- CameraManager.cs

### SoundManager
One game object for each audio source.

Components:
- SoundManager.cs

### CavasHUD
Two text to show the hp and stamina for the player.

### Character
Contain all the other prefabs.

## Scripts Scheme
```
Animator
|_ attackState.cs
|_ dodgeState.cs
|_ jumpState.cs
|_ reactState.cs
|_ specialAttackState.cs

Camera
|_ CameraManager.cs

Enemy
|_ aiController.cs

Physics
|_ Physics.cs

Player
|_ Health.cs
|_ Input.cs
|_ PlayerController.cs
|_ Stamina.cs

Sound
|_ SoundManager.cs

Weapon
|_ WeaponManager.cs
|_ Weapon.cs
```