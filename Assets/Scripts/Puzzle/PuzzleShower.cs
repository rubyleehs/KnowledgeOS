﻿using System.Collections;
using System;
using UnityEngine;
using System.Linq;

public class PuzzleShower : MonoBehaviour
{
    public GameObject cellGO;

    public float tileXDelta;
    public static int[][] puzzle;
    public Transform elementParent;
    public float tileMaxRadiusFromCenter;

    private Vector2 tileDelta;
    private new Transform transform;
    private Transform[][] board;

    public void Initialize()
    {
        tileDelta = new Vector2(tileXDelta, tileXDelta * Mathf.Cos(30 * Mathf.Deg2Rad));
        transform = GetComponent<Transform>();
    }

    public void SetPuzzle(Puzzle puzzle)
    {
        Clear();
        DeserializePuzzleString(puzzle.code, true);
        ShowPuzzle(puzzle.go);
    }

    protected void DeserializePuzzleString(string s, bool flipY)
    {
        //top to bottom
        //start with offset , then ids seperated by "," and end with '\n'
        //no space
        //s = s.Replace(" ", "");
        string[] sa = s.Split('\n');
        puzzle = new int[sa.Length][];
        for (int i = 0; i < sa.Length; i++)
        {
            string[] ss = sa[i].Split(' ');
            //for (int si = 0; si < ss.Length; si++) { Debug.Log(ss[si]); }

            puzzle[i] = ss.Select(int.Parse).ToArray();
        }
        if (flipY) Array.Reverse(puzzle);
    }

    protected void Clear()
    {
        if (board == null) return;

        for (int y = 0; y < board.Length; y++)
        {
            for (int x = 0; x < board[y].Length; x++)
            {
                if (board[y][x] == null) continue;
                //board[y][x].gameObject.SetActive(false);
                Destroy(board[y][x].gameObject); //pool me!;
            }
        }
    }

    protected void ShowPuzzle(GameObject go)
    {
        elementParent.localPosition = Vector3.zero;
        elementParent.localScale = Vector3.one;

        if (go != null)
        {
            board = new Transform[1][];
            board[0] = new Transform[1];
            board[0][0] = Instantiate(go, transform.position, Quaternion.identity, elementParent).transform;
            //board[0][0].GetComponent<SpriteRenderer>().color = Color.white;
            //board[0][0].GetComponent<SpriteRenderer>().sprite = sprite;
            return;
        }

        Vector2[][] tilePos = new Vector2[puzzle.Length][];
        Vector2 tileXPosRange = Vector2.zero;
        Vector2 tileYPosRange = Vector2.zero;

        for (int dy = 0; dy < puzzle.Length; dy++)
        {
            tilePos[dy] = new Vector2[puzzle[dy].Length - 1];
            for (int dx = 1; dx < puzzle[dy].Length; dx++)
            {
                float y = (dy - puzzle.Length * 0.5f + 0.5f) * tileDelta.y;
                float x = (dx - 1 + puzzle[dy][0] - ((dy + 1) % 2 + 1) * 0.5f) * tileDelta.x;
                tilePos[dy][dx - 1] = new Vector2(x, y);
                tileXPosRange.x = Mathf.Min(tileXPosRange.x, x);
                tileYPosRange.x = Mathf.Min(tileYPosRange.x, y);
                tileXPosRange.y = Mathf.Max(tileXPosRange.y, x);
                tileYPosRange.y = Mathf.Max(tileYPosRange.y, y);
            }
        }
        Vector2 c = new Vector2((tileXPosRange.x + tileXPosRange.y) * 0.5f, (tileYPosRange.x + tileYPosRange.y) * 0.5f);
        float furthestSqrDistFromCenter = 0;
        board = new Transform[tilePos.Length][];
        for (int dy = 0; dy < tilePos.Length; dy++)
        {
            board[dy] = new Transform[tilePos[dy].Length];
            for (int dx = 0; dx < tilePos[dy].Length; dx++)
            {
                tilePos[dy][dx] -= c;
                furthestSqrDistFromCenter = Mathf.Max(furthestSqrDistFromCenter, Mathf.Pow(tilePos[dy][dx].x, 2) + Mathf.Pow(tilePos[dy][dx].y, 2));
                if (puzzle[dy][dx + 1] != -2) board[dy][dx] = Instantiate(cellGO, (Vector3)tilePos[dy][dx] + transform.position, Quaternion.identity, elementParent).transform;

                if (puzzle[dy][dx + 1] >= 0)
                {
                    board[dy][dx].GetChild(0).GetComponent<SpriteRenderer>().material = GridElementManager.elements[puzzle[dy][dx + 1]].material;
                    board[dy][dx].GetChild(0).GetComponent<SpriteRenderer>().sprite = GridElementManager.elements[puzzle[dy][dx + 1]].sprite;
                }
            }
        }
        if (furthestSqrDistFromCenter > tileMaxRadiusFromCenter * tileMaxRadiusFromCenter)
        {
            elementParent.localScale = Vector3.one * (tileMaxRadiusFromCenter / Mathf.Sqrt(furthestSqrDistFromCenter));
        }
    }
}