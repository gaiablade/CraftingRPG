# Droppable

## What is a droppable?

A droppable is an object that can be dropped from a slain enemy. These can includes things like items and recipes.

Enemies must be able to include many kinds of drops inside of their drop-tables, yet the functionality of drops can be entirely different. Therefore, an interface called IDroppable is used to give different drops unique behaviour.

The ItemDrop and RecipeDrop classes are examples of its use. Both inherit from the IDroppable interface and implement the OnObtain method. The ItemDrop class uses this method to access the player's inventory and increase the quantity of an item. The RecipeDrop class instead accesses the player's recipe book and checks if the recipe has been unlocked. If not, it adds it to the player's recipe book.