# State

## What is a state?

A state is a class that can be used to implement a specific scenario in an application.

For example, the main menu of a game is an entirely different scenario than when a player is fighting a boss.
States can be created by inheriting the IState interface and implementing the Update and Render methods.
When defined conditions are met, the current state can request that the state be transitioned into another state,
such as when the player selects "Start Game" on the main menu.