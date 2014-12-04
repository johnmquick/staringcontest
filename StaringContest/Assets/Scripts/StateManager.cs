/*
StateManager
Manages scene loading and switching for the application.
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

//manages switching between scenes
//uses singleton instance to manage all scenes
public class StateManager : MonoBehaviour {
    //singleton properties
    private static StateManager _Instance; //the singleton instance of the class

    //constants
    //pixels to world units conversion for Unity
    //should match setting used for sprites in Textures folder
    public const float PIXELS_TO_UNITS = 100.0f;

    //properties
    private string _nextScene; //the next scene to switch to; set by the current scene
    public bool mouseModeOn; //whether using mouse or eye tracker for input

    //create instance via getter
    //access StateManager.Instance from other classes
    public static StateManager Instance {
        get {
            //check for existing instance
            //if no instance
            if (_Instance == null) {
                //create game object
                GameObject StateManagerObj = new GameObject();
                StateManagerObj.name = "State Manager";

                //create instance
                _Instance = StateManagerObj.AddComponent<StateManager>();

                //add scripts
                StateManagerObj.AddComponent<TransitionFade>(); //add new transition script
            }

            //return the instance
            return _Instance;
        } //end get
    } //end accessor

    //awake
    void Awake() {

        //prevent this script from being destroyed when application switches scenes
        DontDestroyOnLoad(this);

        //properties
        mouseModeOn = false; //set default mode

    } //end function

    //switch scene after delay
    public void switchSceneAfterDelay(string theScene, float theDelay) {
        //set the next scene
        _nextScene = theScene;

        //invoke the switch after the given delay
        //used to allow transition to occur before switch
        Invoke("switchScene", theDelay);
    }

    //switch scene
    private void switchScene() {
        
        //audio
        //stop any outstanding sound effects
        AudioManager.Instance.stopAllSfx();

        //load next scene
        Application.LoadLevel(_nextScene);
    }

    //update camera orthographic size
    //used after transitioning between scenes
    public void updateCamOrthographicSize() {

        //set the camera's size based on the current resolution and pixels to units ratio
        Camera.main.orthographicSize = Screen.height / PIXELS_TO_UNITS / 2; //half of the current window size

    }
    
} //end class
