﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BoardOS : MonoBehaviour //Used for interactions within the board.
{
    [Header("Refrences")]
    private static HexBoard hexBoard;
    public HexBoard I_hexBoard;

    public static BoardVisualUpdateSequencer bvus;
    public BoardVisualUpdateSequencer I_bvus;

    [Header("Board Gen")]
    public int boardRadius;
    public float tileXDelta;
    public Vector2 centerPos;

    public static HashSet<HexBoardTile> endTurnEffectsTiles;
    public static HashSet<HexBoardTile> endTurnEffectsTilesQueue;

    private static List<HexBoardTile> endTurnUpdateQueue;
    private static List<int> endTurnUpdateQueueId;

    private void Awake()
    {
        hexBoard = I_hexBoard;
        bvus = I_bvus;
        endTurnEffectsTiles = new HashSet<HexBoardTile>();
        endTurnEffectsTilesQueue = new HashSet<HexBoardTile>();
        hexBoard.Initialize(boardRadius, tileXDelta, centerPos);
        hexBoard.CreateGrid();
        endTurnUpdateQueue = new List<HexBoardTile>();
        endTurnUpdateQueueId = new List<int>();
    }

    public static bool TryUpdateElement(Vector2 worldPos, int id, bool runUpdated)
    {
        HexBoardTile ht = hexBoard.WorldPosToGrid(worldPos) as HexBoardTile;
        return TryUpdateElement(ht, id, runUpdated);
    }
    public static bool TryUpdateElement(HexBoardTile ht, int id, bool runUpdated)
    {
        if (ht == null || ht.ReadElementID() != -1) return false;
        else
        {
            ht.UpdateElement(id);
            if (runUpdated) Run(ht);
            return true;
        }
    }

    public static void ForceChange(Vector2Int index, int id, bool lightUp, bool runUpdated)
    {
        HexBoardTile ht = hexBoard.grid[index.y][index.x];
        ht.UpdateElement(id);
        ht.UpdateVisuals(lightUp);
        if (runUpdated) Run(ht);
    }

    public static void Run(HexBoardTile tile)
    {
        if (tile == null) return;
        if (tile.ReadElementID() >= 0)
        {
            int elementUsed = tile.ReadElementID();
            bvus.ForceComplete(true);
            tile.UpdateVisuals(true);
            GridElementManager.elements[elementUsed].Run(tile);
        }
        EndTurn();
    }

    public static void EndTurn()
    {
        for (int i = 0; i < endTurnEffectsTiles.Count; i++)
        {
            HexBoardTile ht = endTurnEffectsTiles.ElementAt(i);
            if (ht.ReadElementID() != -1) GridElementManager.elements[ht.ReadElementID()].OnTurnEnd(ht);
            else
            {
                //endTurnEffectsTiles.Remove(endTurnEffectsTiles.ElementAt(i));
                i--;
                //Debug.Log("Invalid End Turn Effect!");
            }
        }

        for (int i = 0; i < endTurnUpdateQueue.Count; i++) endTurnUpdateQueue[i].UpdateElement(endTurnUpdateQueueId[i]);
        BoardVisualUpdateSequencer.AddToQueue(endTurnUpdateQueue);

        for (int i = 0; i < endTurnEffectsTilesQueue.Count; i++)
        {
            endTurnEffectsTiles.Add(endTurnEffectsTilesQueue.ElementAt(i));
        }
        endTurnEffectsTilesQueue.Clear();
        endTurnUpdateQueue.Clear();
        endTurnUpdateQueueId.Clear();


    }

    public static void AddToEndTurnUpdateQueue(HexBoardTile tile, int id)
    {
        endTurnUpdateQueue.Add(tile);
        endTurnUpdateQueueId.Add(id);
    }

    public static bool FindElementPattern(int[][] pattern)
    {
        HexBoardTile runtile;
        Vector2Int patternIndex;
        for (int y = 0; y < hexBoard.grid.Length; y++)
        {
            for (int x = 0; x < hexBoard.grid[y].Length; x++)
            {
                patternIndex = Vector2Int.right;
                runtile = hexBoard.grid[y][x];
                if (pattern[patternIndex.y][patternIndex.x] == -2) continue;
                int failsafe = 0;
                while (failsafe < 200 && runtile != null && (runtile.ReadElementID() == pattern[patternIndex.y][patternIndex.x] || (pattern[patternIndex.y][patternIndex.x] == -2 && runtile.ReadElementID() == -1)))
                {
                    failsafe++;
                    if (patternIndex.x < pattern[patternIndex.y].Length - 1)//
                    {
                        patternIndex.x++;
                        runtile = runtile.adjTiles[(int)Hexinal.E];
                        
                        /*
                        if (pattern[patternIndex.y][patternIndex.x] >= -1)
                        {
                            runtile.UpdateElement(pattern[patternIndex.y][patternIndex.x]);
                            runtile.UpdateVisuals(true);
                        }
                        */
                    }
                    else if (patternIndex.y < pattern.Length - 1)
                    {
                        patternIndex.y++;
                        patternIndex.x = 1;

                        int d = pattern[patternIndex.y][0] - pattern[patternIndex.y - 1][0] - (pattern[patternIndex.y - 1].Length -2) + 1;
                        //Debug.Log(pattern[patternIndex.y][0] + " - " + pattern[patternIndex.y - 1][0] + " - " + (pattern[patternIndex.y - 1].Length - 2));
                        runtile = runtile.adjTiles[(int)Hexinal.NW];
                        if (patternIndex.y % 2 == 0) d--;
                        //Debug.Log(d);
                        if (d >= 0)
                        {
                            for (int i = 0; i < d; i++)
                            {
                                if (runtile == null) break;
                                runtile = runtile.adjTiles[(int)Hexinal.E];
                            }
                        }
                        else
                        {
                            for (int i = d; i < 0; i++)
                            {
                                if (runtile == null) break;
                                runtile = runtile.adjTiles[(int)Hexinal.W];
                            }
                        }
                        
                        /*
                        if (pattern[patternIndex.y][patternIndex.x] >= -1)
                        {
                            runtile.UpdateElement(pattern[patternIndex.y][patternIndex.x]);
                            runtile.UpdateVisuals(true);
                        }
                        */      
                    }
                    else return true;
                }
            }
        }
        return false;
    }

    public static int GetTileID(Vector2Int index)
    {
        return hexBoard.grid[index.y][index.x].ReadElementID();
    }
}
