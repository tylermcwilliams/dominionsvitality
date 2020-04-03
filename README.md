# Dominions, Vitality

A vintage story mod that aims at adding bodyheat and thirst.

## Todo

- Add vitals attributes as tree

## Bodyheat

1

- Bodyheat goes down every 10 seconds.
- BehaviorHeatEntities runs `BehaviorBodyheat.HeatEntity(int amount)` for players within range. - Campfire - Forge - Charcoal pit

## Thirst

Thirst goes down every 10 seconds. Currently attaches to MouseEvent in `Core.cs` in Client Events. Will move to behavior class.
