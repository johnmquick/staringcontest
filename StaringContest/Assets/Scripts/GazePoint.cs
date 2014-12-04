/*
GazePoint
Tracks collisions and states for the player's gaze position.
*/

/*
Copyright 2014 John M. Quick

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met: Redistributions of 
source code must retain the above copyright notice, this list of conditions and 
the following disclaimer. Redistributions in binary form must reproduce the above 
copyright notice, this list of conditions and the following disclaimer in the 
documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using UnityEngine;
using System.Collections;

public class GazePoint : MonoBehaviour {

    //properties
    public bool hasEngaged; //whether player engaged with eye
    public bool hasDisengaged; //whether player disengaged with eye

	//init
	void Start () {

        //properties
        hasEngaged = false; //has not yet engaged
        hasDisengaged = false; //has not finished yet
	
	}

    //check collisions
    void OnCollisionEnter2D(Collision2D theCollision) {
        
        //check tag for collision object
        //eye
        if (theCollision.gameObject.tag == "Pupil") {

            Debug.Log("[GazePoint] Player engaged with eye");

            //toggle flag
            hasEngaged = true;

        }

    }

    void OnCollisionStay2D(Collision2D theCollision) {

        //check tag for collision object
        //eye
        if (theCollision.gameObject.tag == "Pupil") {

            //Debug.Log("[GazePoint] Player maintained engagement with eye");

        }

    } 

    void OnCollisionExit2D(Collision2D theCollision) {

        //check tag for collision object
        //eye
        if (theCollision.gameObject.tag == "Pupil") {

            Debug.Log("[GazePoint] Player disengaged with eye");

            //toggle flag
            hasDisengaged = true;

            //disable collider
            gameObject.collider2D.enabled = false;

        }

    } 

} //end class