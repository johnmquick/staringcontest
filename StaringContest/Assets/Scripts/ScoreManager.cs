/*
ScoreManager
Manages the player's score throughout the application.
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

//manages score tracking for the game
//uses singleton instance to manage all scoring across game
public class ScoreManager : MonoBehaviour {
    
    //singleton properties
    private static ScoreManager _Instance; //the singleton instance of the class

    //properties
    public float duration; //total time it took to complete the latest level
    public int numWins; //total number of victories

    //create instance via getter
    //access ScoreManager.Instance from other classes
    public static ScoreManager Instance {
        get {
           
            //check for existing instance
            //if no instance
            if (_Instance == null) {

                //create game object
                GameObject ScoreManagerObj = new GameObject();
                ScoreManagerObj.name = "Score Manager";

                //create instance
                _Instance = ScoreManagerObj.AddComponent<ScoreManager>();

            }

            //return the instance
            return _Instance;

        } //end get
    } //end accessor

    //awake
    void Awake() {
        
        //prevent this script from being destroyed when application switches scenes
        DontDestroyOnLoad(this);

    } //end function
    
	//init
	void Start () {
	    
        //properties
        duration = 0.0f;
        numWins = 0;

	} //end function
	
    //reset scoring variables
    //call when a new session is started
    public void resetScore() {
        Debug.Log("[ScoreManager] Score Reset");

        //properties
        duration = 0.0f;
        numWins = 0;

    } //end function

} //end class
