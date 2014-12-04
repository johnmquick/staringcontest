/*
MainMenuManager
Manages the main menu.
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

//manages the application states from the main menu
public class MainMenuManager : MonoBehaviour {

    //GUI
    public GUISkin guiSkin; //custom skin

	//init
	void Start () {
        
        //show mouse cursor
        Screen.showCursor = true;

        //state manager
        if (StateManager.Instance != null) {

            //update camera
            StateManager.Instance.updateCamOrthographicSize();

            //transition into scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
        }

        //audio
        if (AudioManager.Instance != null) {
            AudioManager.Instance.bgmSource.time = 0; //reset playback to beginning
            AudioManager.Instance.bgmIsFadingIn = true; //remove hold
            AudioManager.Instance.bgmIsHoldFade = false; //remove hold
            AudioManager.Instance.bgmSource.Play(); //play
        }
	
	} //end function

    //GUI
    void OnGUI() {
	
		//gui settings
        GUI.skin = guiSkin; //custom skin

        //create buttons
        int btnW = 240;
        int btnH = 80;
        int btnBuffer = 10;
        int btnOffset = 50;
        int btnNum = 2;
        float btnX = Screen.width / 2 - btnW / 2;
        float btnY = Screen.height - (btnNum * (btnH + btnBuffer)) - btnOffset; //aligned to bottom of screen

        //play button
        Rect btnPlayRect = new Rect(btnX, btnY, btnW, btnH);
        string btnPlayText = "Play";

        //mode button
        float btnModeY = btnY + btnH + btnBuffer;
        Rect btnModeRect = new Rect(btnX, btnModeY, btnW, btnH);
        string btnModeText = "";

        //update mode button text
        //mouse mode
        if (StateManager.Instance.mouseModeOn == true) {

            //mouse mode
            btnModeText = "Controls: Mouse";

        }

        //eye mode
        else if (StateManager.Instance.mouseModeOn == false) {

            //eye mode
            btnModeText = "Controls: Eye";

        }
        
        //quit button
        float btnQuitW = 100;
        float btnQuitH = 50;
        float btnQuitX = Screen.width - btnQuitW - btnBuffer; //aligned to bottom-right of screen
        float btnQuitY = Screen.height - btnQuitH - btnBuffer;
        Rect btnQuitRect = new Rect(btnQuitX, btnQuitY, btnQuitW, btnQuitH);
        string btnQuitText = "Quit";
        
        //play button pressed
        if (GUI.Button(btnPlayRect, btnPlayText)) {
            //proceed to game scene
            Debug.Log("Load Game");
            //transition to next scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
            StateManager.Instance.switchSceneAfterDelay("Game", theTransition.duration);
            
            //audio
            AudioManager.Instance.playBtnClick(); //sfx

        }
        
        //mode button pressed
        if (GUI.Button(btnModeRect, btnModeText)) {
            
            //toggle mode
            StateManager.Instance.mouseModeOn = !StateManager.Instance.mouseModeOn;

            Debug.Log("Mouse controls updated to: " + StateManager.Instance.mouseModeOn);

            //audio
            AudioManager.Instance.playBtnClick();

        }
        
        //quit button pressed
        if (GUI.Button(btnQuitRect, btnQuitText)) {

            //quit application
            Debug.Log("Quit Application");
            Application.Quit();

        }
        
    } //end function

} //end class
