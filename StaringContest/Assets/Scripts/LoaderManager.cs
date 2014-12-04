/*
LoaderManager
Manages the application loader to establish singleton objects.
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

public class LoaderManager:MonoBehaviour {
    //awake
    //called before start function
    void Awake() {

        //audio manager
        //initialize since this is the first scene
        if (AudioManager.Instance != null) { 
        
            //play sound effect
            AudioManager.Instance.playLoss();
        
        }

        //state manager
        if (StateManager.Instance != null) {

            //update camera
            StateManager.Instance.updateCamOrthographicSize();

            //transition into scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
        }

    } //end function

    //init
    void Start() {

        //transition after delay
        Invoke("transition", 3.0f);

    } //end function

    //transition
    private void transition() {

        //proceed to main menu scene
        Debug.Log("[LoaderManager] Load Main Menu");

        //transition to next scene
        TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
        theTransition.toggleFade();
        StateManager.Instance.switchSceneAfterDelay("Menu", theTransition.duration);

    } //end function

} //end class
