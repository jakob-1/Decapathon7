using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Piece : MonoBehaviour {

    public int value = 1;
    public bool beenChecked = false;
    public int x;
    public int y;


    public void assignXY(int X, int Y)
    {
        x = X;
        y = Y;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
