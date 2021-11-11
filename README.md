# MiniRTS-Tutorial

There is a blogpost about this project, which can be found at [https://cxyda.github.io/](https://cxyda.github.io/MiniRTS-introduction/)

This project is about how to implement common game systems used in RTS games. I will also implement multiplayer support. The network architecture will be a lockstep simulation, which is quite common for games in the RTS genre.

I’m going to focus on a good architecture which should allow you to learn from it and extend it to a big project without suffering from a fast but hacky implementation. This of course comes with a drawback. Some implementations might look cumbersome at first or even like overkill for such a small project, but the goal of this series is not to get basic systems working but to create a solid foundation for any RTS project. I assume you have at least basic knowledge about Unity, so this series will not explain you how to do basic thing in Unity. If you are stuck on some steps, I’m sure google can help you out. Otherwise, feel free to drop me some lines.

There won’t be any fancy graphics, so don’t be disappointed that it looks bad at the end. I’ll also keep in mind that you might want to replace my coder artwork with your own to start the next big RTS hit.

This project will use Extenject (formerly know as Zenject) for Dependency Injection.


### Content of this series (might change on the go)

- [x] v0.1 [#1 Basic project setup](https://github.com/Cxyda/MiniRTS-Tutorial/tree/0.1)
- [x] v0.2 [#2 Selection and Input handling](https://github.com/Cxyda/MiniRTS-Tutorial/tree/0.2)
- [x] v0.3 [#3 Building Placement](https://github.com/Cxyda/MiniRTS-Tutorial/tree/0.3)
- [ ] \#4 Factories and Unit production
- [ ] \#5 UI and Asset Management
- [ ] \#6 Units and Resource Gathering
- [ ] \#7 Camera Controls
- [ ] \#8 Battle
- [ ] \#9 Fog od War
- [ ] \#10 Lockstep Simulation
- [ ] \#11 Networking
- [ ] \#12 TechTrees


### Technology used

1. Unity3D 2021.1.15f1
2. Extenject (Zenject) framework
3. Photon Engine
