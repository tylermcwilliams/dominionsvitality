# Dominions, Vitality

A vintage story mod that aims at adding bodyheat and thirst.

### Bodyheat Dyanmics

Bodyheat will tend toward the climate's scaledTemp, by adding modifiers to the rate of change.
Change will occur every second. Fireplace and Forge are active heatsources that call `ChangeTemperature(int amount)`;

`OnGameTick` will cause a change in bodyheat toward climate that the player is in
`COOL BY tempChange`
Static modifiers:
- `tempChange = getTemp` Grab the scaled temperature of the block
- `tempChange -= (saturation * 0.01)` Make the change warmer based on saturation
- `if(inWater) tempChange += 5`
- `if(night) +5 else -5` 
- `tempChange -= clothingHeat`


 

### Bodyheat Requirements

Bodyheat will be measured as an int stored on the Entity. The int will directly correspond to Celsius. 
COLD KILLS
KEEP HEAT UP

< 32 = Hazard

- Temperature is updated every 10 seconds
- Update looks at Environment temperature
- Env temperature modifiers: Water, Fire, Altitude, Time

- Water cools
- Proximity to fire heats
- Day heats
- Night cools

- If below 32, damage

