# TODO:
1. Add robust path corner handling from SAIN
2. Hook up objectives again
3. Squad level objective handling & timer on switching to the next objective.
   a. Add a Timer class
   b. Simple guarding
4. Proper door handling
   a. Disable collisions between doors on the path and the agent. Can probably do in a lightweight way - when a door is first detected, we check a cache if it's already handled.
   b. Sequence: when distance is less than 2m, pause movement, open door, start timer, start moving.
5. Add stuck bot detection to movement.
   a. If we are within 2m of the target, just assume we reached it.
   b. Otherwise fail the movement
5. Proper guarding