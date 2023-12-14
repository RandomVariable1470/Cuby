using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameGrid grid;
    [SerializeField] private LayerMask layerMask;

    private void Update() 
    {
        GridCell gridCell = IsOverAGridSpace();

        if (gridCell != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gridCell.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            }
        }
    }

    private GridCell IsOverAGridSpace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, layerMask))
        {
            return raycastHit.transform.GetComponent<GridCell>();
        }
        else
        {
            return null;
        }
    }
}