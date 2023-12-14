using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [field:SerializeField] public GameObject objectInThisGridSpace {get; private set;}
    [field:SerializeField] public bool isOccupied {get; private set;}

    private int posX;
    private int posY;

    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject != null)
        {
            if (other.gameObject.layer != 6)
            {
                isOccupied = true;
                objectInThisGridSpace = other.gameObject;
            }
        }
    }
}