# Instance

## What is an instance?

An instance is an entity that appears on a map. Examples of entities are the player, an enemy, or a treasure chest.

An entity serves as a way to separate the general information about an entity's positional data on a map and that entity's specific information that doesn't relate to the current map.

For example, the player has information about their stats, skills, inventory, and unlocked recipes. This kind of information would go in a class like PlayerInfo.

On the other hand, a player can also appear on a map and needs information about their position and depth. Therefore, this information can be stored in a PlayerInstance class.

For example:
```cs
var playerInfo = new PlayerInfo();
var playerInstance = new
{
    Info = playerInfo,
    Position = new Point(100, 200),
    Depth = 200 + 32
};
```

In the example above, the player info might be passed from a global instance of the player, whereas the position of the player on the current map might come from a more localized origin, such as a loading zone on a specific map. It is useful to keep these concepts separate.