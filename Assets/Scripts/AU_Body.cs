using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AU_Body : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;

    void OnEnable()
    {
        if (AU_PlayerMovement.allBodies != null)
        {
            AU_PlayerMovement.allBodies.Add(transform);
        }
    }
    public void SetColor(Color newColor)
    {
        bodySprite.color = newColor;
    }
    public void Report()
    {
        Debug.Log("Body Reported");
        Destroy(gameObject);
    }
}
