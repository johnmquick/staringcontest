/*
OpponentAnimator
Manages the opponent's states, actions, and animations.
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

public class OpponentAnimator : MonoBehaviour {

    //children objects
    private GameObject _eyelid;
    private GameObject _pupil;
    private GameObject _mouth;
    private GameObject _head;

    //animators
    private Animator _eyelidAnimator;
    private Animator _pupilAnimator;
    private Animator _mouthAnimator;
    private Animator _headAnimator;

    //actions
    public bool isActive; //whether or not currently taking action
    public float endTime; //end time for latest segment
    private float _startTime; //start time for segment
    private float _moveDuration; //duration of movement
    private Vector3 _moveStart; //start position for movement
    private Vector3 _moveTarget; //target position for movement
    private bool _isMoving; //whether currently moving

	//init
	void Start () {

        //objects
        _eyelid = GameObject.FindWithTag("Eyelid");
        _pupil = GameObject.FindWithTag("Pupil");
        _mouth = GameObject.FindWithTag("Mouth");
        _head = GameObject.FindWithTag("Head");

        //animators
        _eyelidAnimator = _eyelid.GetComponent<Animator>();
        _pupilAnimator = _pupil.GetComponent<Animator>();
        _mouthAnimator = _mouth.GetComponent<Animator>();
        _headAnimator = _head.GetComponent<Animator>();

        //actions
        isActive = true; //default/intro animations active
        endTime = Time.time; //update time
        _startTime = Time.time; //update time 
        _moveDuration = 1.0f;
        _moveStart = Vector3.zero; //default to center
        _moveTarget = Vector3.zero; //default to center
        _isMoving = false; //not moving
	
	}

    //update
    void Update() {

        //check movement
        if (_isMoving == true) {

            //move
            move();

        }

    }

    //update animation states
    public void changeAnimationStateTo(int theEyelidState, int thePupilState, int theMouthState, int theHeadState) {

        /*
        Possible animation states
        0000 = intro
        1010 = idle
        1031 = opponent wins
        1130 = move left
        1230 = move right
        1330 = roll
        2020 = close
        3010 = fake
        */

        //update animations
        //0 = default/intro; 1 = idle; 2 = close; 3 = fake
        _eyelidAnimator.SetInteger("stateId", theEyelidState);
        //0 = default/idle; 1 = move left; 2 = move right; 3 = roll
        _pupilAnimator.SetInteger("stateId", thePupilState);
        //0 = default/intro; 1 = idle; 2 = close; 3 = action
        _mouthAnimator.SetInteger("stateId", theMouthState);
        //0 = default/intro; 1 = idle; 2 = close; 3 = action
        _headAnimator.SetInteger("stateId", theHeadState);

        //check for idle
        if (theEyelidState == 1 && thePupilState == 0 && theMouthState == 1 && theHeadState == 0) {

            //toggle flag
            isActive = false; 

        }

        //otherwise, start action
        else {

            //toggle flag
            isActive = true;

            //update start time
            _startTime = Time.time;

        }

    }

    //select a new action to begin
    public void selectAction() {

        /* 
        Possible actions
        0 = move 
        1 = roll 
        2 = fake
        */

        //choose random action
        int randAct = Random.Range(0, 3);

        //check action
        switch (randAct) {

            //move
            case 0:

                //return to center position from left 
                if (gameObject.transform.position.x < 0) {

                    Debug.Log("[OpponentAnimator] Action: Move Center");

                    //update movement target
                    _moveTarget = Vector3.zero;

                    //update animations
                    //move right
                    changeAnimationStateTo(1, 2, 3, 0);

                }
                                
                //return to center position from right
                else if (gameObject.transform.position.x > 0) {

                    Debug.Log("[OpponentAnimator] Action: Move Center");

                    //update movement target
                    _moveTarget = Vector3.zero;

                    //update animations
                    //move left
                    changeAnimationStateTo(1, 1, 3, 0);
                }

                //currently at center point
                //randomly select the next target
                else {

                    /* 
                    Possible directions
                    0 = left
                    1 = right 
                    */

                    //update direction
                    int dir = Random.Range(0, 2);

                    //left
                    if (dir <= 0) {

                        Debug.Log("[OpponentAnimator] Action: Move Left");

                        //update movement target
                        _moveTarget = new Vector3(-0.5f * Screen.width / StateManager.PIXELS_TO_UNITS + 0.5f * _head.renderer.bounds.size.x, gameObject.transform.position.y, gameObject.transform.position.z);

                        //update animations
                        //move left
                        changeAnimationStateTo(1, 1, 3, 0);

                    }
                    
                    //right
                    else if (dir >= 1) {

                        Debug.Log("[OpponentAnimator] Action: Move Right");

                        //update movement target
                        _moveTarget = new Vector3(0.5f * Screen.width / StateManager.PIXELS_TO_UNITS - 0.5f * _head.renderer.bounds.size.x, gameObject.transform.position.y, gameObject.transform.position.z);

                        //update animations
                        //move right
                        changeAnimationStateTo(1, 2, 3, 0);

                    }
                    
                }

                //update start position
                _moveStart = gameObject.transform.position;

                //start movement
                _isMoving = true;

                //audio
                AudioManager.Instance.playMove();

                break; 

            //roll
            case 1:

                Debug.Log("[OpponentAnimator] Action: Roll");

                //update animations
                changeAnimationStateTo(1, 3, 3, 0);

                //end action after delay
                Invoke("endAction", 1.0f);

                //audio
                AudioManager.Instance.playRoll();

                break; 

            //fake
            case 2:

                Debug.Log("[OpponentAnimator] Action: Fake");

                //update animations
                changeAnimationStateTo(3, 0, 2, 0);

                //end action after delay
                Invoke("endAction", 1.0f);

                //audio
                AudioManager.Instance.playClipAfterDelay("playFake", 0.8f);

                break; 

            default:
                Debug.Log("[OpponentAnimator] Error: action not recognized");
                break;

        }

        //update start time
        _startTime = Time.time;

        //toggle flag
        isActive = true;

    }

    //end action
    public void endAction() {

        Debug.Log("[OpponentAnimator] Action ended - transition to idle");

        //toggle active
        isActive = false;

        //stop movement
        _isMoving = false;

        //update end time
        endTime = Time.time;

        //update animations
        //idle
        changeAnimationStateTo(1, 0, 1, 0);

    }
	
	//move
    public void move() {

        //calculate percent complete
        float pctComplete = (Time.time - _startTime) / _moveDuration;
        
        //position
        gameObject.transform.position = Vector3.Lerp(_moveStart, _moveTarget, pctComplete);

        //check for end of animation
        if (pctComplete >= 1.0f) {

            //set to target position
            gameObject.transform.position = _moveTarget;

            //end action
            endAction();

        }

    }

} //end class
