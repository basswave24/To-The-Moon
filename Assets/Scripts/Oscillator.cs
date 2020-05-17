using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is used for objects moving */ 
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);
    [SerializeField] float period = 2f; //can be changed in unity for faster/slower movement 


    float movementFactor; //0 for not moved, 1 for fully moved


    Vector3 startingPos;
	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if (period <= Mathf.Epsilon) {
            return;
        } //Mathf.Epsilon == "The smallest value that a float can have different from zero." 

        float cycles = Time.time / period; //grows continually from 0

        const float tau = Mathf.PI * 2; //tau is 2pi
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f; //goes from 0 to 1

        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
    }  //Source : https://en.wikipedia.org/wiki/Sine 
}
