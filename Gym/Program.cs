using Phobos.Helpers;
using UnityEngine;

namespace Gym;

public static class Program
{
    public static void Main()
    {
        var history = new PositionHistory(10);

        // Move with constant velocity
        for (var i = 0; i < 15; i++)
        {
            var position = new Vector3(i, 0, 0);
            history.Update(position);
    
            // Always query from CURRENT position (not i)
            var distSqr = history.GetDistanceSqr();
            var dist = Mathf.Sqrt(distSqr);
            
            Console.WriteLine($"Update {i+1}: Distance = {dist:F1}, DistSqr = {distSqr:F1}");
        }
        
        history.Reset();
        
        // Move with constant velocity
        for (var i = 0; i < 15; i++)
        {
            var position = new Vector3(i, 0, 0);
            history.Update(position);
    
            // Always query from CURRENT position (not i)
            var distSqr = history.GetDistanceSqr();
            var dist = Mathf.Sqrt(distSqr);
            
            Console.WriteLine($"Update {i+1}: Distance = {dist:F1}, DistSqr = {distSqr:F1}");
        }
    }
}