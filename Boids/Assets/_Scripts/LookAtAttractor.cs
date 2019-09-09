using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtAttractor : MonoBehaviour {

	
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Attractor.POS);
	}
}
