﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTile : HexTile
{
    public InventoryTile(Transform transform, Vector2Int gridPos, int elementID)
    {
        this.transform = transform;
        this.gridPos = gridPos;
        this.elementID = elementID;
        this.spriteRenderer = transform.GetComponent<SpriteRenderer>();
        this.elementSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if(elementID >= 0 && elementID < GridElementManager.elements.Length) elementSpriteRenderer.sprite = GridElementManager.elements[elementID].sprite;
        ToggleOccupancy(true);
    }

    public void ToggleOccupancy(bool isOccupied)
    {
        if (elementID < 0 || elementID >= GridElementManager.elements.Length) return;
        if (isOccupied) elementSpriteRenderer.material = GridElementManager.elements[elementID].material;
        else elementSpriteRenderer.material = InventoryOS.usedMat;
    }
}
