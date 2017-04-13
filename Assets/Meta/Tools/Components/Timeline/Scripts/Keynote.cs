using System;
using UnityEngine;

namespace Meta.Tools
{
    [Serializable]
    public enum OnKeynoteStartAction
    {
        None = 0,
        Delay,
        Pause,
        Loop
    }

    /*[Serializable]
    public enum AllowedKeyCodes
    {
        None = 0,
        Enter,
        Space,
        Backspace
    }*/

    [Serializable]
    public enum KeynoteDragingSide
    {
        LeftBorder = 0,
        Body,
        RightBorder
    }

    [Serializable]
    public enum ConditionType
    {
        None = 0,
        VoiceCommand,
        Button,
        Timer,
        KeyCode
    }

    [Serializable]
    public class ConditionInfo
    {
        public float Delay = 0f;
        public KeyCode KeyCode = KeyCode.None;
    }

    [Serializable]
    public class Keynote
    {
        #region Public fields
#if UNITY_EDITOR
        public static float ClosedKeynoteWidth = /*16f*/0f;
#endif

        public OnKeynoteStartAction OnKeynoteStartAction = OnKeynoteStartAction.None;
        public float DelayOnStart = 1f;
        public bool LoopedOnStart = true;
        public bool PauseOnStart = true;

        //public Action<Keynote> _conditionComplied;
        //public Action<Keynote> _pauseActionDone;
        //public Action<Keynote> _nextActionDone;
        public Action _keynoteStartReached;
        public Action _keynoteEndReached;
        public Action _keynoteEnded;

        public float Timing = 0f;

        public string Name = "";
        public string Description = "";

        /*public ConditionType LaunchingConditionType = ConditionType.Timer;
        public ConditionInfo LaunchingConditionInfo = new ConditionInfo() { Delay = 0f };

        public ConditionType PauseActionType = ConditionType.KeyCode;
        public ConditionInfo PauseActionInfo;

        public ConditionType NextActionType = ConditionType.KeyCode;
        public ConditionInfo NextActionInfo;*/

        //public bool Looped = false;

#if UNITY_EDITOR
        //public Color BackgroundColor = new Color(254f / 255f, 254f / 255f, 153f / 255f);
        public Color BackgroundColor = new Color(18 / 255f, 21f / 255f, 23f / 255f);
        public bool Opened = false;
        public bool BookmarkSelected = false;
        public Rect BookmarkRect;
        public Rect BodyRect;
        public Rect LeftBorderRect;
        public Rect RightBorderRect;
        public Vector2 StartDragingPosition;
        public float PreviousTiming = 0f;
        public float PreviousWidth = 0f;
        public KeynoteDragingSide KeynoteDragingSide;
#endif
#endregion


#if UNITY_EDITOR
        public float Width
        {
            get
            {
                /*if (Opened)
                {
                    return _width;
                }
                else
                {
                    return ClosedKeynoteWidth;
                }*/
                return ClosedKeynoteWidth;
            }
            set
            {
                _width = value;
            }
        }
#endif

        #region Serialized Fields
        [SerializeField]
        private float _width = 160f;
        #endregion

        #region Private Fields
        //private bool _onKeynoteMoment;
        //private float _counter;
        #endregion

        #region Main Methods
        /// <summary>
        /// Invoked when particular keynote is reached. Usually it used to initialize conditions.
        /// </summary>
        public void ReachedKeynoteMoment()
        {
            //_onKeynoteMoment = true;

            if (_keynoteStartReached != null)
            {
                _keynoteStartReached.Invoke();
            }
            /*switch (LaunchingConditionType)
            {
                case ConditionType.Timer:
                    _counter = 0f;
                    break;
            }*/
        }

        public void ReachedEndOfKeynote()
        {
            if (_keynoteEndReached != null)
            {
                _keynoteEndReached.Invoke();
            }
        }

        public void Ended()
        {
            if (_keynoteEnded != null)
            {
                _keynoteEnded.Invoke();
            }
        }

        /*/// <summary>
        /// Invoked when particular condition was skipped until it's condition was complied.
        /// </summary>
        public void SkipLaunchingCondition()
        {
            //_onKeynoteMoment = false;
        }*/

        /*/// <summary>
        /// Method where Keynote proceses it's logic
        /// </summary>
        /// <param name="deltaTime">Delta Time</param>
        /// <param name="keyCode">KeyCode for editor</param>
        public void Update(float deltaTime, KeyCode keyCode = KeyCode.None)
        {
            if (_onKeynoteMoment)
            {
                switch (LaunchingConditionType)
                {
                    case ConditionType.Timer:
                        _counter += deltaTime;
                        if (_counter >= LaunchingConditionInfo.Delay)
                        {
                            _onKeynoteMoment = false;
                            if (_conditionComplied != null)
                            {
                                _conditionComplied.Invoke(this);
                            }
                        }
                        break;
                    case ConditionType.KeyCode:
                        if (keyCode == KeyCode.None)
                        {
                            if (Input.GetKeyUp(getKeyCode(LaunchingConditionInfo.KeyCode)))
                            {
                                _onKeynoteMoment = false;

                                if (_conditionComplied != null)
                                {
                                    _conditionComplied.Invoke(this);
                                }
                            }
                        }
                        else
                        {
                            if (keyCode == getKeyCode(LaunchingConditionInfo.KeyCode))
                            {
                                _onKeynoteMoment = false;

                                if (_conditionComplied != null)
                                {
                                    _conditionComplied.Invoke(this);
                                }
                            }
                        }
                        break;
                }
            }


            switch (PauseActionType)
            {
                case ConditionType.KeyCode:
                    if (keyCode == KeyCode.None)
                    {
                        if (Input.GetKeyUp(getKeyCode(PauseActionInfo.KeyCode)))
                        {
                            if (_pauseActionDone != null)
                            {
                                _pauseActionDone.Invoke(this);
                            }
                        }
                    }
                    else
                    {
                        if (keyCode == getKeyCode(PauseActionInfo.KeyCode))
                        {
                            if (_pauseActionDone != null)
                            {
                                _pauseActionDone.Invoke(this);
                            }
                        }
                    }
                    break;
            }

            switch (NextActionType)
            {
                case ConditionType.KeyCode:
                    if (keyCode == KeyCode.None)
                    {
                        if (Input.GetKeyUp(getKeyCode(NextActionInfo.KeyCode)))
                        {
                            if (_nextActionDone != null)
                            {
                                _nextActionDone.Invoke(this);
                            }
                        }
                    }
                    else
                    {

                        if (keyCode == getKeyCode(NextActionInfo.KeyCode))
                        {
                            if (_nextActionDone != null)
                            {
                                _nextActionDone.Invoke(this);
                            }
                        }
                    }
                    break;
            }
        }*/
#endregion

        #region Utility Methods
        /*private KeyCode getKeyCode(AllowedKeyCodes allowedCode)
        {
            switch (allowedCode)
            {
                case AllowedKeyCodes.Enter:
                    return KeyCode.Return;
                case AllowedKeyCodes.Space:
                    return KeyCode.Space;
                case AllowedKeyCodes.Backspace:
                    return KeyCode.Backspace;
            }

            return KeyCode.None;
        }*/
        #endregion
    }
}
