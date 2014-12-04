/*
SummaryManager
Manages the summary scene.
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
using System; //for Math

//manages summary of data presented to player
public class SummaryManager : MonoBehaviour {

    //properties
    private string _summaryText; //the text to be presented to the player

    //GUI
    public GUISkin guiSkin; //custom skin

    //init
    void Start() {
	
		//show mouse cursor
        Screen.showCursor = true;
        
        //summarize the data
        summarizeData();

        //state manager
        if (StateManager.Instance != null) {

            //update camera
            StateManager.Instance.updateCamOrthographicSize();

            //transition into scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
        }

    } //end function

    //summarize the data from the score manager instance so it can be reported to the player
    private void summarizeData() {
        
        //verify that the score manager instance exists
        if (ScoreManager.Instance != null) {

            //get the data from the score manager instance
            TimeSpan duration = TimeSpan.FromSeconds(ScoreManager.Instance.duration);

            //format strings
            string durationStr = String.Format("{0:D1}:{1:D2}.{2:D3}", duration.Minutes, duration.Seconds, duration.Milliseconds);

            //prepare the final summary text
            _summaryText =
                "Contest Summary \n"
                + "Round Time: " + durationStr + "\n" 
                + "Total Wins: " + ScoreManager.Instance.numWins;

        } //end outer if

        //error
        else {
            Debug.Log("[SummaryManager] Score Manager Instance not found - cannot summarize data");
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

        //continue button
        Rect btnContinueRect = new Rect(btnX, btnY, btnW, btnH);
        string btnContinueText = "Continue";

        //menu button
        float btnMenuY = btnY + btnH + btnBuffer;
        Rect btnMenuRect = new Rect(btnX, btnMenuY, btnW, btnH);
        string btnMenuText = "Main Menu";

        //create text
        float summaryW = 500;
        float summaryH = 120;
        float summaryX = Screen.width / 2 - summaryW / 2;
        float summaryY = btnY - summaryH;
        Rect summaryRect = new Rect(summaryX, summaryY, summaryW, summaryH);
        GUI.Label(summaryRect, _summaryText, GUI.skin.customStyles[0]);

        //continue button pressed
        if (GUI.Button(btnContinueRect, btnContinueText)) {

            //go to next level
            Debug.Log("Load Next Level");

            //transition to next scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
            StateManager.Instance.switchSceneAfterDelay("Game", theTransition.duration);

            //audio
            AudioManager.Instance.playBtnClick(); //sfx

        }

        //menu button pressed
        if (GUI.Button(btnMenuRect, btnMenuText)) {
            
            //return to main menu
            Debug.Log("Load Main Menu");

            //reset entire game score
            ScoreManager.Instance.resetScore();
            
            //transition to next scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
            StateManager.Instance.switchSceneAfterDelay("Menu", theTransition.duration);
            
            //audio
            AudioManager.Instance.playBtnClick(); //sfx

        }

    } //end function

} //end class
