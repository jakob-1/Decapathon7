using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveDown : MonoBehaviour {

    public float distance = 1;
    public float moveRate = 1;
    private float moveTimer;

	// Use this for initialization
	void Start () {
        
        moveTimer = Time.time + moveRate;
    }
	
	// Update is called once per frame
	void Update () {
		if (moveTimer < Time.time)
        {
            transform.position += -distance * transform.up;
            moveTimer = Time.time + moveRate;
        }
	}
}
