using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private float floatMagnitude = 0.5f;
    [SerializeField] private float floatSpeed = 1.75f; 
    [field:Space(10)]
    [field:SerializeField] public GameObject objectInThisGridSpace {get; private set;}
    [field:SerializeField] public bool isOccupied {get; private set;}

    private int posX;
    private int posY;
    
    private Vector3 initialPosition;
    private int columnId;

    private void Start() 
    {
        initialPosition = transform.position;
        StartCoroutine(IdleFloatingAnimation());
    }

    public void SetColumn(int column)
    {
        columnId = column;
    }

    private IEnumerator IdleFloatingAnimation()
    {
        float offset = 0f;

        while (true)
        {
            float phase = columnId * 0.5f; 
            float sineWave = Mathf.Sin((Time.time + phase) * floatSpeed);
            float yOffset = sineWave * floatMagnitude;

            offset = Mathf.Lerp(offset, yOffset, Time.deltaTime * floatSpeed * 0.5f);

            Vector3 newPosition = initialPosition + new Vector3(0, offset, 0);
            transform.position = newPosition;

            yield return null; 
        }
    }
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