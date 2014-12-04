/*
GameManager
Manages the gameplay states and objects for the main game loop.
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

//eye tracking
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

//manages the main game loop
public class GameManager : MonoBehaviour {

    //properties
    private bool _isLevelStarted; //whether player has made eye contact in order to begin the level
    private bool _isGameOver; //whether the game has ended
    private float _winDuration; //time, in seconds, at which player will win contest
    private float _levelDuration; //time, in seconds, the level lasted
    private GameObject _gazePoint; //represents where the player is looking
    public float startTime; //time the level started
    private OpponentAnimator _opponentAnimator; //the opponent

    //eye tracking
    private EyeXHost _host; //eye tracking host
    private IEyeXDataProvider<EyeXGazePoint> _gazeData; //for retrieving gaze points

	//awake
    void Awake () {

        //hide mouse cursor
        Screen.showCursor = false;
    }

	//init
	void Start () {

        //reset level
        resetLevel();

        //opponent
        _opponentAnimator = GameObject.FindWithTag("Opponent").GetComponent<OpponentAnimator>();

        //score manager
        if (ScoreManager.Instance != null) {

            //determine level difficulty based on total wins
            //5 seconds plus another 3-5 per previous win
            _winDuration = 5.0f + Random.Range(3.0f, 5.0f) * ScoreManager.Instance.numWins;

        }
        
        //state manager
        if (StateManager.Instance != null) {

            //update camera
            StateManager.Instance.updateCamOrthographicSize();

            //transition into scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
        }

        //eye tracking
        //listen for gaze data
        if (StateManager.Instance.mouseModeOn == false) {

            //init components
            _host = EyeXHost.GetInstance();
            _gazeData = _host.GetGazePointDataProvider(GazePointDataMode.LightlyFiltered);

            //listen for gaze data
            EyeXHost.GetInstance().GetGazePointDataProvider(GazePointDataMode.LightlyFiltered).Start();

        }
        
		//enable collisions after delay
        //allow for transitions, animations, etc.
        //sleeping to awake animation sequence is 5 seconds
        Invoke("enableCollisions", 5.0f);

        //audio
        //for sleeping to awake animation sequence 
        //audio
        AudioManager.Instance.playSnoreStart();
        AudioManager.Instance.playClipAfterDelay("playSnoreStart", 2.0f);        
	} 
	
	//update
	void Update () {

        //check whether player has engaged and level needs to be started
        if (_gazePoint.GetComponent<GazePoint>().hasEngaged == true && _isLevelStarted == false) {

            //toggle flag
            _isLevelStarted = true; 

            //reset start time
            startTime = Time.time;

            //update end time for opponent animator
            _opponentAnimator.endTime = Time.time;

            //update animations
            //set opponent to idle
            _opponentAnimator.changeAnimationStateTo(1, 0, 1, 0);

        }
        
        //only check state if level has started
        if (_isLevelStarted == true) {
            
            //check the game state and update as needed
            checkGameState();

            //check opponent activity
            /*
            Begin a new action if the opponent is idle, 
            the time since the last action ended has 
            been long enough, and the game is not over.
            */
            if (_isGameOver == false && 
                _opponentAnimator.isActive == false &&
                Time.time - _opponentAnimator.endTime >= Random.Range(3.0f, 5.0f)) {

                //begin action
                _opponentAnimator.selectAction();

            }

        }

        //mouse mode
        if (StateManager.Instance.mouseModeOn == true) {

            //show mouse cursor
            Screen.showCursor = true;

            //get current cursor position
            float cursorX = Input.mousePosition.x;
            float cursorY = Input.mousePosition.y;
            float cursorZ = _gazePoint.transform.position.z - Camera.main.transform.position.z;

            //convert cursor position to world position vector
            Vector3 cursorPos = new Vector3(cursorX, cursorY, cursorZ);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(cursorPos);

            //set gaze point to mouse cursor position
            _gazePoint.transform.position = worldPos;

            //Debug.Log("[GameManager] Mouse cursor positioned at (" + worldPos.x + ", " + worldPos.y + ")");
        }

        //eye tracking
        else if (StateManager.Instance.mouseModeOn == false) {

            //get gaze data
            IEyeXDataProvider<EyeXGazePoint> gazeData = EyeXHost.GetInstance().GetGazePointDataProvider(GazePointDataMode.LightlyFiltered);

            //convert eye position to world position vector
            Vector2 eyePos = gazeData.Last.Viewport;

            //check gaze state
            EyeXEngineStateValue<UserPresence> gazeState = _host.UserPresence;

            //check whether gaze point is valid and user is present
            if (gazeState.ToString().Equals("Present") && 
                eyePos.x >= 0.0f && eyePos.x <= 1.0f && eyePos.y >= 0.0f && eyePos.y <= 1.0f) {

                Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(eyePos.x, eyePos.y, 10));

                //set gaze point to mouse cursor position
                _gazePoint.transform.position = worldPos;

                Debug.Log("[GameManager] Eye position: " + worldPos);

            }

            else if (gazeState.ToString().Equals("NotPresent") && _isLevelStarted == true) {

                Debug.Log("[GameManager] User not present");

                //disable collisions
                _gazePoint.collider2D.enabled = false;

                //player disengaged
                _gazePoint.GetComponent<GazePoint>().hasDisengaged = true;

            }

        }
	} 

    //reset level
    public void resetLevel() {

        //properties
        _gazePoint = GameObject.FindWithTag("GazePoint"); //retrieve from scene
        _isGameOver = false; //level has not ended
        _isLevelStarted = false; //level has not yet begun
        _levelDuration = 0.0f; //level has not yet begun
        _gazePoint.collider2D.enabled = false; //disable collisions until level begins

    }

    //end level
    public void endLevel() {

        //game over
        if (_isGameOver == true) {

            Debug.Log("[GameManager] Ending Level");

            //update score
            ScoreManager.Instance.duration = _levelDuration;

            //transition to next scene
            TransitionFade theTransition = StateManager.Instance.gameObject.GetComponent<TransitionFade>();
            theTransition.toggleFade();
            StateManager.Instance.switchSceneAfterDelay("Summary", theTransition.duration);

        }

    }

    //enable collisions
    public void enableCollisions() {

        UnityEngine.Debug.Log("[GameManager] Collisions Enabled");

        //enable collisions
        _gazePoint.collider2D.enabled = true;

        //audio
        AudioManager.Instance.playLoss();
        
    }
    
    //check the game state and update as needed
    //used to manage levels, waves, and other content experienced during gameplay
    public void checkGameState() {

        //check whether player has disengaged
        bool hasDisengaged = _gazePoint.GetComponent<GazePoint>().hasDisengaged;

        //determine time elapsed thus far in round
        _levelDuration = Time.time - startTime;

        //check whether opponent has been defeated
        if (_levelDuration >= _winDuration && 
            _opponentAnimator.isActive == false && 
            hasDisengaged == false) {

            //player wins
            playerWin();

        }

        //otherwise, check if player disengaged
        else if (hasDisengaged == true && _opponentAnimator.isActive == false) {

            //player loses
            playerLoss();

        }
                
    }

    //player win
    private void playerWin() {

        Debug.Log("[GameManager] Player Wins!");

        //disable collisions
        _gazePoint.collider2D.enabled = false;

        //toggle flag
        _isGameOver = true;

        //increment wins
        ScoreManager.Instance.numWins++;

        //update animations
        _opponentAnimator.changeAnimationStateTo(2, 0, 2, 0);

        //audio
        AudioManager.Instance.playClipAfterDelay("playWin", 1.0f);

        //end level after delay
        //level end animation is 2 seconds
        Invoke("endLevel", 2.0f);

    }

    //player loss
    private void playerLoss() {

        Debug.Log("[GameManager] Player Loses");

        //disable collisions
        _gazePoint.collider2D.enabled = false;

        //toggle flag
        _isGameOver = true;

        //update animations
        _opponentAnimator.changeAnimationStateTo(1, 0, 3, 1);

        //audio
        AudioManager.Instance.playLoss();

        //end level after delay
        //level end animation is 2 seconds
        Invoke("endLevel", 2.0f);

    }

    //exit
    void OnApplicationQuit() {

        //eye tracking
        //stop listener
        if (_gazeData != null) {
            _gazeData.Stop();
        }

    } 

} //end class