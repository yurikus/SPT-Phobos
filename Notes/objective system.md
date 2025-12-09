!!! NOTES !!!
The objective system will run ~1x a second or so.

# List of prioritized objectives
Only one activity can run at a time. We have a list of activities in order of their priorities that we check repeatedly.

We have the following objectives:

1. Assist
2. Quest
3. Guard

## IsActive = false
1. If we have a current objective, unregister from it and set it to null
2. If quest objective is not failed, reset to suspended. We have to leave at failed so that there's a stable final outcome of completed or failed 
3. Short circuit execution

## IsActive = true
* Iterate through the objectives, grab the first one whose ShouldBegin == true and activate it
  * Assist:
    * Activate if there is a squaddie in combat
    * Set strategic objective status to suspended (if not failed already)
    * Turns off if the squaddie dies or is no longer in combat
  * Strategic:
    * Turns on if there is no current objective and the strategic objective status is suspended 
    * Turns off if completed or failed
    * If failed, it also pauses Phobos
  * Guard:
    * Activate if strategic objective is completed
* If all actors failed or completed their Strategic:
  * Generate a new squad Strategic objective
  * Assign it to each squaddie
  * Reset the strategic objective statuses to suspended
  * Unpause all actors.

## Pros/Cons
- Pros: Simple
- Cons: Less scalable


# Plan queue
Only one activity can run at a time. Each actor starts out with an empty plan.

* The squad generates a new strategic objective
* Default plan: Strategic, Guard

We need a Plan class that will be basically a sorted dict internally, but with a pre-baked current and next field for O(1) access to these.

Something like:

```csharp
public class Actor
{
    // Move these to the actor
    public VisitLocation VisitLocation; // Was Strategic
    public Assist Assist;
    public Guard Guard;
}

public class Strategy // Was Objective
{
    public Plan Plan;
}

public class Plan
{
    public PlanStatus Status;
    public Objective Current;
    public Objective Strategic;
    public List<Objective> Tactical;
    public SortedList<int, Objective> Backlog;
}

public enum ObjectiveType
{
    Strategic,
    Tactical
}

public abstract class Objective
{
    public static int TypeId;  // Used for indexing into the array of systems to get this objective type
    public ObjectiveStatus Status;
}

public class SquadStrategy
{
    // Fills in a plan instance for an actor
}
```


## IsActive = false
* If we have a plan, clear it out and unregister the actor from the current objective.
* Short circuit execution

## IsActive = true
* ExecutePlan()
  * If the plan failed:
    * Clear out the plan, pause the actor and bail out
  * If the plan is empty:
    * Generate a new plan from the SquadStrategy
  * If the plan is not empty (has active objective or >0 in the backlog):
    * Check if we have a current objective, if not:
      * Grab the next objective, set it as current and register the actor with it
    * If yes:
      * Check the status of the current objective
        * If failed, we empty out the plan, set it to failed and pause the actor
        * If completed, move on to the next objective, if there are none left, set the plan to succeeded.
* UpdatePlan()
  * Check the tactical objectives for interrupt
    * If we get interrupted
      * If the current objective is strategic: push the objective back into the backlog and reset it's status to suspended
      * Set the tactical objective as the current.
  * Do the squad level plan updating:
    * Check if everyone failed/completed the current plan

## Interrupts
* Assist:
  * Activate if there is a squaddie in combat
  * Sets status to completed if the squaddie dies or is no longer in combat
* Guard:
  * Activate if strategic objective is completed

## Pros/Cons
* Pros: we can seamlessly implement squad level strategic plans. E.g. VisitLocation, which when picked will pick a location and generate a plan itself.
* Complex