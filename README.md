
# Blast Bender

<p align="center">
  <img src="Assets/Documentation/Images/logo.png?raw=true" alt="Blast Bender" width="217px" height="93px"/>
</p>

<p align="center">
  <img src="Assets/Documentation/Images/1.png?raw=true" alt="Blast Bender" width="350px" height="350px"/>
    <img src="Assets/Documentation/Images/2.png?raw=true" alt="Blast Bender" width="350px" height="350px"/>
</p>

## Introduction
Welcome! This repository is a clone inspired by popular blast games. This project aims to implement clean coding principles and use various design patterns to create dynamic game mechanics using C# and Unity.
## Dependencies
This project integrates numerous external libraries. Zenject, a robust framework, has been utilized for managing dependencies.This framework enhances the modularity and sustainability of the code, making significant contributions to the project's architecture. UniTask has been used for asynchronous operations, while Naughty Attributes and Toolbar Extender have been employed to enhance the Unity editor. On the other hand, DoTween is an indispensable component in this project.
- [Zenject](https://github.com/modesttree/Zenject)
- [DoTween](http://dotween.demigiant.com/)
- [UniTask](https://github.com/Cysharp/UniTask)
- [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes)
- [Toolbar Extender](https://github.com/marijnz/unity-toolbar-extender)

## Game Structure Overview

Successful games need more than good code and design; they also require well-thought-out levels. This project has two main parts:

- Main Game: Where players can play.
- Level Generator: Where new levels are created.

<img src="Assets/Documentation/Images/scenes.png?raw=true" alt="Zenject" width="200px" height="102px"/>

Scenes in the game are created only once and persist throughout the game. They are not created or deleted during gameplay. Each scene performs its specific function. Scenes do not open or close themselves; instead, objects are either pooled or toggled on and off.

When the game starts, the scenes load in the following order:

- For the Main Game: Loading Scene -> Menu Scene -> Game Scene
- For the Level Generator: Loading Scene -> Menu Scene -> Game Scene -> Level Generator Scene

If you do not want the current scene to be active when you run the game, and prefer starting from the Loading Scene, you should select 

- BlastBender(in the menu bar) -> LoadSceneOnPlay -> Loading Scene

This feature allows you to debug quickly without having to open the "Loading Scene" every time.

#### Board Data Structure

```mermaid
flowchart LR

A[IBoardItem] --> B[IBoardItem< TPoolItem >] 
B --> C[BaseBoardItem< TPoolItem >]

C--> G[SpaceArea]
C--> D[Bead]
C--> E[PowerUp]
C--> F[Obstacle]

E--> P1[Rocket]
E--> P2[Bomb]
E--> P3[Light Ball]

F--> O1[Box]
F--> O2[X1]
F--> O3[X2]
F--> O4[...]



```

<br />

The diagram here displays the class hierarchy. Each class is a Controller and manages its associated Item. The TPoolItem item serves as the View of the Item and is its physical form.

 I can store an IBoardData array for each level.To make an Item meaningful, I need to know its Row, Column, and the type of the Item.IBoardData contains all of these.As you can see, it is simple.

```csharp
namespace BoardItems.LevelData
{
    [CreateAssetMenu(fileName = "Levels", menuName = "Level")]
    public class LevelData : ScriptableObject
    {
        public int RowLength;
        public int ColumnLength;

        [SerializeReference] 
        public IBoardItem[] BoardItems;

        public Border.Border[] Border;
    }
}
```

Now let’s examine how Unity stores this data in YAML format.

```yaml
- rid: 7831880784201908428
      type: {class: Bead, ns: BoardItems.Bead, asm: Assembly-CSharp}
      data:
        _row: 0
        _column: 4
        _color: 1

- rid: 7831880784201908429
      type: {class: SpaceArea, ns: BoardItems.Void, asm: Assembly-CSharp}
      data:
        _row: 0
        _column: 5
```

Holding a reference with [SerializeReference] makes our work easier. There is color in Bead, but it is not necessary for SpaceArea.