using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EFT.Interactive;
using Phobos.Diag;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Phobos.Navigation;

public struct Cell()
{
    public readonly List<Location> Locations = [];
    public int Congestion = 0;

    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Locations.Count > 0;
    }
}

public class LocationGrid
{
    private const int MinCells = 3;
    private const float MaxCellSize = 50f;
    
    private readonly Cell[,] _cells;
    private readonly float _cellSize;
    private readonly Vector2Int _gridSize;
    private readonly Vector2 _worldOffset; // Bottom-left corner in world space

    private static int _idCounter;

    public LocationGrid()
    {
        var locations = Collect();
        Shuffle(locations);

        // Calculate bounds from positions
        var worldMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var worldMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        for (var i = 0; i < locations.Count; i++)
        {
            var pos = locations[i].Position;
            
            worldMin.x = Mathf.Min(worldMin.x, pos.x);
            worldMin.z = Mathf.Min(worldMin.z, pos.z);
            worldMax.x = Mathf.Max(worldMax.x, pos.x);
            worldMax.z = Mathf.Max(worldMax.z, pos.z);
        }
        
        // Add padding to bounds
        worldMin.x -= 10f;
        worldMin.z -= 10f;
        worldMax.x += 10f;
        worldMax.z += 10f;

        _worldOffset = new Vector2(worldMin.x, worldMin.z);
        
        var worldWidth = worldMax.x - worldMin.x;
        var worldHeight = worldMax.z - worldMin.z;
        
        // Calculate cell size that gives us at least minCells cells
        var cellSizeFromWidth = worldWidth / MinCells;
        var cellSizeFromHeight = worldHeight / MinCells;
        
        // Take the minimum of the three constraints
        _cellSize = Mathf.Min(MaxCellSize, Mathf.Max(cellSizeFromWidth, cellSizeFromHeight));
        
        // Calculate resulting grid dimensions
        var cols = Mathf.CeilToInt(worldWidth / _cellSize);
        var rows = Mathf.CeilToInt(worldHeight / _cellSize);
        
        _gridSize = new Vector2Int(cols, rows);
        _cells = new Cell[cols, rows];

        var searchRadius = Math.Max(worldWidth, worldHeight) / 2f;

        DebugLog.Write($"Location grid cell size: {_gridSize}, radius: {_cellSize:F1}, locations: {locations.Count}");
        DebugLog.Write($"Location grid world bounds: [{worldMin.x:F0},{worldMin.z:F0}] -> [{worldMax.x:F0},{worldMax.z:F0}]");
        DebugLog.Write($"Location grid world size: {worldWidth:F0}x{worldHeight:F0} search radius: {searchRadius}");

        // Initialize all cells
        for (var x = 0; x < cols; x++)
        {
            for (var y = 0; y < rows; y++)
            {
                _cells[x, y] = new Cell();
            }
        }

        for (var i = 0; i < locations.Count; i++)
        {
            var location = locations[i];
            var coords = WorldToCell(location.Position);
            DebugLog.Write($"Adding {location} to cell at [{coords.x}, {coords.y}]");
            _cells[coords.x, coords.y].Locations.Add(location);
        }

        // Loop through all the cells and check if a path can be found from the cell center or BSG locations to a neighboring cell location.
        for (var x = 0; x < _gridSize.x; x++)
        {
            for (var y = 0; y < _gridSize.y; y++)
            {
                var cellCoords = new Vector2Int(x, y);
                ref var cell = ref _cells[cellCoords.x, cellCoords.y];

                // If we already have BSG issued locations, bail out
                if (cell.Locations.Count > 0)
                {
                    continue;
                }

                DebugLog.Write($"Cell at [{x}, {y}] has no BSG locations, attempting to create synthetic at cell center");

                // Try to find a navmesh position as close to the cell center as possible.
                var worldPos = CellToWorld(cellCoords);

                if (NavMesh.SamplePosition(worldPos, out var hit, searchRadius, NavMesh.AllAreas))
                {
                    var centerPos = hit.position;

                    if (WorldToCell(centerPos) == cellCoords)
                    {
                        DebugLog.Write($"Cell {cellCoords}: found center sample on navmesh at {centerPos}");
                        cell.Locations.Add(BuildSyntheticLocation(centerPos));
                        continue;
                    }
                }

                DebugLog.Write($"Cell {cellCoords}: no navmesh positions found");
            }
        }
    }

    public Location RequestNear(Vector3 worldPos, Location previous)
    {
        var requestCoords = WorldToCell(worldPos);
        
        for (var dx = -1; dx <= 1; dx++)
        {
            for (var dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                var coords = new Vector2Int(requestCoords.x + dx, requestCoords.y + dy);
                
                if (!IsValidCell(coords))
                    continue;
            }
        }

        var currentCell = _cells[requestCoords.x, requestCoords.y];

        // We can't go to any neighboring cell for some reason, grab something from the current cell. 
        return currentCell.Locations.Count > 0 ? currentCell.Locations[Random.Range(0, currentCell.Locations.Count)] : null;
    }

    public void Return(Location location)
    {
        var coords = WorldToCell(location.Position);
        ref var cell = ref _cells[coords.x, coords.y];
        cell.Congestion--;

        if (cell.Congestion >= 0) return;

        cell.Congestion = 0;
        DebugLog.Write($"Returning {location} to the pool resulted in negative congestion");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < _gridSize.x && cell.y >= 0 && cell.y < _gridSize.y && _cells[cell.x, cell.y].IsValid;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector3 CellToWorld(Vector2Int cell)
    {
        var x = _worldOffset.x + (cell.x + 0.5f) * _cellSize;
        var z = _worldOffset.y + (cell.y + 0.5f) * _cellSize;
        
        return new Vector3(x, 0, z);
    }

    private Vector2Int WorldToCell(Vector3 worldPos)
    {
        var x = Mathf.FloorToInt((worldPos.x - _worldOffset.x) / _cellSize);
        var y = Mathf.FloorToInt((worldPos.z - _worldOffset.y) / _cellSize);
        
        return new Vector2Int(x, y);
    }
    
    private static void Shuffle(List<Location> objectives)
    {
        // Fisher-Yates in-place shuffle
        for (var i = 0; i < objectives.Count; i++)
        {
            var randomIndex = Random.Range(i, objectives.Count);
            (objectives[i], objectives[randomIndex]) = (objectives[randomIndex], objectives[i]);
        }
    }

    private static List<Location> Collect()
    {
        var collection = new List<Location>();

        DebugLog.Write("Collecting quests POIs");

        _idCounter = 0;

        foreach (var trigger in Object.FindObjectsOfType<TriggerWithId>())
        {
            if (trigger.transform == null)
                continue;

            ValidateAndAddLocation(collection, LocationCategory.Quest, trigger.name, trigger.transform.position);
        }

        foreach (var container in Object.FindObjectsOfType<LootableContainer>())
        {
            if (container.transform == null || !container.enabled || container.Template == null)
                continue;

            ValidateAndAddLocation(collection, LocationCategory.ContainerLoot, container.name, container.transform.position);
        }

        DebugLog.Write($"Collected {collection.Count} points of interest");

        return collection;
    }

    private static void ValidateAndAddLocation(List<Location> collection, LocationCategory category, string name, Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out var target, 2f, NavMesh.AllAreas))
        {
            var objective = new Location(_idCounter, category, name, target.position);
            collection.Add(objective);
            _idCounter++;
        }
        else
        {
            DebugLog.Write($"Skipping Location({category}, {name}, {position}), too far from navmesh");
        }
    }

    private static Location BuildSyntheticLocation(Vector3 position)
    {
        var location = new Location(_idCounter, LocationCategory.Synthetic, $"Synthetic_{_idCounter}", position);
        _idCounter++;
        return location;
    }
}