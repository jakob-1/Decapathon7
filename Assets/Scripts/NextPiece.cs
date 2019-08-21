using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPiece : MonoBehaviour {

    public Sprite sprite;
    public int value;
    public Sprite[] sprites;
    public GameObject piecePrefab;


    public void newPiece(int newValue)
    {
        sprite = sprites[newValue];
        value = newValue;
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public int getValue()
    {
        return value;
    }
}
