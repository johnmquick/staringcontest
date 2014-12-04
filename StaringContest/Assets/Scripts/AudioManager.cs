/*
AudioManager
Manages the background music and sound effects for the application.
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

//manages all audio for application, including music and sound effects
//uses singleton instance to manage all scenes

public class AudioManager : MonoBehaviour {
    //singleton properties
    private static AudioManager _Instance; //the singleton instance of the class

    //constants
    private const string TAG_AUDIO = "AudioManager"; //tag for audio manager object
    private const float SFX_VOL_MAX = 1.0f; //max volume for sfx
    private const float BGM_VOL_MAX = 0.8f; //max volume for bgm
    private const float BGM_VOL_MIN = 0.0f; //min volume for bgm

    //properties
    public float bgmFadeDuration; //how long, in seconds, the fade in/out effects should last
    private bool _bgmIsFadingIn; //whether currently fading in or out
    private bool _bgmIsHoldFade; //whether to hold the fading
    private float _bgmFadeStartTime; //start time for latest bgm fade in/out effect
    private float _bgmVolume; //the current bgm volume

    //objects
    //bgm
    public AudioClip bgm; //the background music
    private AudioSource _bgmSource; //audio source for music

    //sfx
    private AudioSource _sfxSource; //audio source for sound effects
    public AudioClip sfxBtn; //button click
    public AudioClip sfxFake; //fake action
    public AudioClip sfxLoss; //player loses
    public AudioClip sfxMove; //move action 
    public AudioClip sfxRoll; //roll action 
    public AudioClip sfxSnoreStart; //start of snore 
    public AudioClip sfxSnoreEnd; //end of snore
    public AudioClip sfxWin; //player wins


    //create instance via getter
    //access AudioManager.Instance from other classes
    public static AudioManager Instance {
        get {
            //check for existing instance
            //if no instance
            if (_Instance == null) {
                //set instance to existing game object
                if (GameObject.FindWithTag(TAG_AUDIO) != null) {
                    _Instance = GameObject.FindWithTag(TAG_AUDIO).GetComponent<AudioManager>();
                }
                //otherwise, create game object
                else {
                    //create new game object
                    GameObject AudioManagerObj = new GameObject();
                    AudioManagerObj.name = "Audio Manager";

                    //create instance
                    _Instance = AudioManagerObj.AddComponent<AudioManager>();
                    
                }

                //add audio sources
                //bgm
                _Instance._bgmSource = _Instance.gameObject.AddComponent<AudioSource>(); //add new audio source
                _Instance._bgmSource.playOnAwake = false; //set play on awake
                _Instance._bgmSource.loop = true; //set looping
                _Instance._bgmSource.volume = BGM_VOL_MIN; //set volume
                _Instance._bgmSource.priority = 0; //set highest priority

                //sfx
                _Instance._sfxSource = _Instance.gameObject.AddComponent<AudioSource>(); //audio source
                _Instance._sfxSource.playOnAwake = false; //set play on awake
                _Instance._sfxSource.loop = false; //set looping
                _Instance._sfxSource.volume = SFX_VOL_MAX; //set volume

            } //end outer if

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
    public void Start() {
        //ensure audio clips are defined in inspector
        //error
        if (bgm == null) {
            
            Debug.Log("[AudioManager] Error: Missing audio clips - define in inspector");
            
            //disable script
            this.enabled = false;

        }

        //proceed
        else {

            //set clip
            _bgmSource.clip = bgm; 

            //assume min volume
            _bgmVolume = BGM_VOL_MIN;

            //set fade
            _bgmIsFadingIn = false; 

            //set hold
            _bgmIsHoldFade = true;

            //establish the start time
            _bgmFadeStartTime = Time.time;

        }

    } //end function

    //update
    void Update() {

        //only update volume if not on hold
        if (_bgmIsHoldFade == false) {

            //fade in
            if (_bgmIsFadingIn == true) {
                bgmFadeIn();
            }

            //fade out
            if (_bgmIsFadingIn == false) {
                bgmFadeOut();
            }

            //update playback volume
            _bgmSource.volume = _bgmVolume;

        } //end outer if

    } //end function

    //fade in the volume
    private void bgmFadeIn() {

        //calculate the time completed thus far
        float cumulativeTime = Time.time - _bgmFadeStartTime; //cumulative time completed
        float pctTime = Mathf.Clamp(cumulativeTime / bgmFadeDuration, 0.0f, 1.0f); //percentage time completed

        //volume is less than max
        if (_bgmVolume < BGM_VOL_MAX) {

            //update
            _bgmVolume = pctTime;

        }

        //volume has reached max
        else {

            //set to max
            _bgmVolume = BGM_VOL_MAX;

            //hold
            _bgmIsHoldFade = true;

        }

    } //end function

    //fade out the volume
    private void bgmFadeOut() {

        //calculate the time completed thus far
        float cumulativeTime = Time.time - _bgmFadeStartTime; //cumulative time completed
        float pctTime = 1.0f - Mathf.Clamp(cumulativeTime / bgmFadeDuration, 0.0f, 1.0f); //percentage time completed

        //volume is greater than min
        if (_bgmVolume > BGM_VOL_MIN) {

            //update
            _bgmVolume = pctTime;

        }

        //volume has reached min
        else {

            //set to min
            _bgmVolume = BGM_VOL_MIN;

            //hold
            _bgmIsHoldFade = true;

        }

    } //end function

    //toggle fade
    //call before triggering transition
    public void toggleFade() {

        //switch fade effect to prepare for next segment
        _bgmIsFadingIn = !_bgmIsFadingIn;

        //turn off hold
        _bgmIsHoldFade = false;

        //reset time
        _bgmFadeStartTime = Time.time;

    } //end function

    //sfx playback functions
    public void playClipAfterDelay(string theClip, float theDelay) {

        Invoke(theClip, theDelay); 

    }
    public void playBtnClick() {

        _sfxSource.clip = sfxBtn;
        _sfxSource.Play();
    }

    public void playFake() {

        _sfxSource.clip = sfxFake;
        _sfxSource.Play();
    }
    public void playLoss() {

        _sfxSource.clip = sfxLoss;
        _sfxSource.Play();
    }
    public void playMove() {

        _sfxSource.clip = sfxMove;
        _sfxSource.Play();
    }
    public void playRoll() {

        _sfxSource.clip = sfxRoll;
        _sfxSource.Play();
    }
    public void playSnoreStart() {

        _sfxSource.clip = sfxSnoreStart;
        _sfxSource.Play();
        Invoke("playSnoreEnd", 1.0f);
    }
    public void playSnoreEnd() {

        _sfxSource.clip = sfxSnoreEnd;
        _sfxSource.Play();

    }
    public void playWin() {

        _sfxSource.clip = sfxWin;
        _sfxSource.Play();
    }

    public void stopAllSfx() {
        _sfxSource.Stop();
    }

    //setters and getters
    public AudioSource bgmSource {
        get { return _bgmSource; }
        set { _bgmSource = value; }
    }

    public bool bgmIsFadingIn {
        get { return _bgmIsFadingIn; }
        set { _bgmIsFadingIn = value; }
    }
    public bool bgmIsHoldFade {
        get { return _bgmIsHoldFade; }
        set { _bgmIsHoldFade = value; }
    }
    
}