using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Meta.Tools.Editor
{
    /// <summary>
    /// This View is a main working View, consisting of a number of subviews, sharing that working area 
    /// in a different and changing ways.
    /// </summary>
    public class WorkView : TracksViewBase
    {
        #region Private Variables
        //Views
        private TracksSubview _tracksSubview;
        private TimelineSubview _timelineSubview;
        private ChaptersSubview _chaptersSubview;
        
        private VoidView _voidView;
        private TimeSliderView _timeSliderView;
        private LinesView _linesView;

        //Field
        private Rect _field = new Rect();
        
        private bool _pressedLMB;
        private bool _pressedRMB;
        
        private Vector2 _initialMousePos;
        private Vector2 _initialFieldPos;

        public float EndGlobalPosition;
        private bool _timeSliderIsVisible = false;

        public float LocalSliderPosition = 0f;
        public float StartGlobalPositionTiming = 0f;
        public float EndGlobalPositionTiming = 0f;
        #endregion

        #region Subclasses
        public class Subview
        {
            public Rect RealRect;
            public float Timing0;
            public float Timing1;
            public float ChapterTiming0;
            public float ChapterTiming1;
            public int InstanceID;

            public void UpdateInstanceID(int id = 0)
            {
                if (id == 0)
                {
                    InstanceID = (RealRect.ToString() + Timing0.ToString() + Timing1.ToString()).GetHashCode();
                }
                else
                {
                    InstanceID = id;
                }
            }
        }
        
        private class KeynoteQuery
        {
            //Reference to a Keynote
            public Keynote Keynote;
            //Area in the field where it must be drawed 
            public float LocalBeginning;
            public float LocalEnding;
            //Area in the target area
            public float GlobalBeginning;
            public float GlobalEnding;
        }
        
        private class TrackQuery
        {
            public float BeginningTiming;
            public float EndingTiming;

            public float GlobalBeginning;
            public float GlobalEnding;
            public float LocalBeginning;
            public float LocalEnding;

            public Rect Area = new Rect();
        }
        #endregion

        #region Constructor
        public WorkView()
        {
            createViews();

            _field.x = 0f;
            _field.y = 0f;
        }
        #endregion

        #region Main Methods
        protected override void Draw()
        {
            base.Draw();

            //Debug.Log("work: _field.y = " + _field.y);

            /*
             * Because tracks and timeline may be separated by blocks of keynotes of arbitrary width, we must render blocks separately, one after another.
             * That views are called subviews. Other type of views here are simple, non-interrupted views, wich are rendered single time.
             * Order of drawing: tracks -> timeline -> chapters -> void -> timeslider -> windows -> lines
             */

            if (_tracksSubview == null || _timelineSubview == null || _chaptersSubview == null ||
                _voidView == null || _timeSliderView == null || _linesView == null)
            {
                createViews();
                return;
            }

            drawSubviews();

            if (_timeSliderIsVisible)
            {
                _timeSliderView.DrawTimeSlider();
            }

            _linesView.Draw(new Rect(0f, 0f, _width, _height),
                    _rect,
                    _offset,
                    _field,
                    _rect,
                    _timeline,
                    _serializedObject,
                    _timelineWindow,
                    this);
        }

        public override void ProcessEvents(Event e)
        {
            base.ProcessEvents(e);

            _timeSliderView.ProcessEvents(e);
            _chaptersSubview.ProcessEvents(e);
            _tracksSubview.ProcessEvents(e);
            _timelineSubview.ProcessEvents(e);
            
            _linesView.ProcessEvents(e);
        }

        public override void SetNewFieldPosition(Vector2 fieldPosition)
        {
            _field.x = fieldPosition.x;
            _field.y = fieldPosition.y;
        }
        #endregion

        #region Utility Methods
        #region Navigation
        public void NavigationStart(Vector2 mousePosition)
        {
            _initialMousePos = mousePosition;
            _initialFieldPos.x = _field.x;
            _initialFieldPos.y = _field.y;

            _tracksView.FieldManuallyControlled = true;
        }

        public void NavigationXStart(float xValue)
        {
            _initialMousePos.x = xValue;
            _initialFieldPos.x = _field.x;

            _tracksView.FieldManuallyControlled = true;
        }

        public void Navigate(Vector2 mousePosition)
        {
            _tracksView.SetTargetFieldPosition(new Vector2(_initialFieldPos.x + mousePosition.x - _initialMousePos.x, _initialFieldPos.y + mousePosition.y - _initialMousePos.y));
        }

        public void NavigateX(float xValue)
        {
            _tracksView.SetTargetFieldX(_initialFieldPos.x + xValue - _initialMousePos.x);
        }

        private float getTimesPosition(float time, Subview subview)
        {
            float position = subview.RealRect.width * ((time - subview.Timing0) / (subview.Timing1 - subview.Timing0));

            return position;
        }

        public void ProcessClickOnTimeline(float mouseX)
        {
            float pos = mouseX - _field.x;
            float time = TimelineWindow.GetPositionsTiming(pos);
            _timeline.GoToTiming(time, Timeline.AnimationMode.IgnoreKeynotes);
        }
        #endregion

        #region Lines
        public void DrawLineBetween(Vector2 point0, Vector2 point1, Color color)
        {
            _linesView.DrawLine(color, point0, point1, new Vector2(0, (_tracksView.TimelinePercentage + _tracksView.TimelineBarPercentage) * _rect.height));
        }
        #endregion

        private void createViews()
        {
            _tracksSubview = new TracksSubview();
            _timelineSubview = new TimelineSubview();
            _chaptersSubview = new ChaptersSubview();

            _voidView = new VoidView();
            _timeSliderView = new TimeSliderView();
            _linesView = new LinesView();
        }

        private void drawSubviews()
        {
            /*
             * In that function we get our viewrect and define what parts need to be displayed in that rect. And then display them.
             */

            List<KeynoteQuery> keynoteQueries = new List<KeynoteQuery>();
            List<TrackQuery> trackQueries = new List<TrackQuery>();

            //Evaluating rendered Keynotes
            float position = 0f;
            float lastKeynoteTiming = 0f;
            KeynoteQuery keynoteQueryBeforeBeginning = null;
            //bool localSliderPositionDefined = false;
            for (int i = 0; i < _timeline.Keynotes.Count; i++)
            {
                position += (_timeline.Keynotes[i].Timing - lastKeynoteTiming) * _timeline.PixelPerSecond;
                /*if (!localSliderPositionDefined && _timeline.CurrentTime < _timeline.Keynotes[i].Timing)
                {
                    localSliderPositionDefined = true;
                    LocalSliderPosition = position - (_timeline.Keynotes[i].Timing - _timeline.CurrentTime) * _timeline.PixelPerSecond;
                }*/
                float keyNotesTimingsPosition = position;
                position += _timeline.Keynotes[i].Width;
                lastKeynoteTiming = _timeline.Keynotes[i].Timing;

                KeynoteQuery keynoteQuery = new KeynoteQuery();
                keynoteQuery.Keynote = _timeline.Keynotes[i];
                keynoteQuery.LocalBeginning = keyNotesTimingsPosition;
                keynoteQuery.LocalEnding = keyNotesTimingsPosition + _timeline.Keynotes[i].Width;
                keynoteQuery.GlobalBeginning = _field.x + keynoteQuery.LocalBeginning;
                keynoteQuery.GlobalEnding = keynoteQuery.GlobalBeginning + _timeline.Keynotes[i].Width;

                if (keynoteQuery.GlobalEnding <= 0f)
                {
                    keynoteQueryBeforeBeginning = keynoteQuery;
                    continue;
                }
                else if (keynoteQuery.GlobalBeginning > _width)
                {
                    break;
                }

                keynoteQueries.Add(keynoteQuery);
            }

            /*if (!localSliderPositionDefined)
            {
                if (_timeline.Keynotes != null && _timeline.Keynotes.Count > 0)
                {
                    localSliderPositionDefined = true;
                    LocalSliderPosition = position + (_timeline.CurrentTime - _timeline.Keynotes[_timeline.Keynotes.Count - 1].Timing) * _timeline.PixelPerSecond;
                }
                else
                {
                    localSliderPositionDefined = true;
                    LocalSliderPosition = _timeline.CurrentTime * _timeline.PixelPerSecond;
                }
            }*/

            LocalSliderPosition = TimelineWindow.GetTimingsPosition(_timeline.CurrentTime);

            //Debug.Log("keynoteQueries.Count = " + keynoteQueries.Count);
            //Evaluating rendered Tracks
            for (int i = 0; i < keynoteQueries.Count; i++)
            {
                if (i == 0 && keynoteQueries[i].GlobalBeginning > 0f)
                {
                    //case of first keynote located after beginning of drawing area
                    TrackQuery trackQuerie = new TrackQuery();

                    if (_field.x <= 0f)
                    {
                        trackQuerie.GlobalBeginning = 0f;
                        if (keynoteQueryBeforeBeginning == null)
                        {
                            trackQuerie.BeginningTiming = (_field.x * -1f) / _timeline.PixelPerSecond;
                        }
                        else
                        {
                            trackQuerie.BeginningTiming = keynoteQueryBeforeBeginning.Keynote.Timing + (keynoteQueryBeforeBeginning.GlobalEnding * -1f) / _timeline.PixelPerSecond;
                        }
                    }
                    else
                    {
                        trackQuerie.GlobalBeginning = _field.x;
                        trackQuerie.BeginningTiming = 0f;
                    }
                    
                    trackQuerie.GlobalEnding = keynoteQueries[i].GlobalBeginning;
                    trackQuerie.EndingTiming = keynoteQueries[i].Keynote.Timing;
                    trackQuerie.LocalBeginning = trackQuerie.GlobalBeginning - _field.x;
                    trackQuerie.LocalEnding = trackQuerie.GlobalEnding - _field.x;

                    trackQueries.Add(trackQuerie);
                }

                if (i == keynoteQueries.Count - 1)
                {
                    //case of last keynote
                    if (keynoteQueries[i].GlobalEnding >= _width)
                    {
                        EndGlobalPosition = _width;
                        break;
                    }
                    
                    TrackQuery trackQuerie = new TrackQuery();

                    trackQuerie.GlobalBeginning = keynoteQueries[i].GlobalEnding;
                    trackQuerie.GlobalEnding = _width;
                    trackQuerie.BeginningTiming = keynoteQueries[i].Keynote.Timing;
                    trackQuerie.EndingTiming = trackQuerie.BeginningTiming + (trackQuerie.GlobalEnding - trackQuerie.GlobalBeginning) / _timeline.PixelPerSecond;
                    if (trackQuerie.EndingTiming > _timeline.Duration)
                    {
                        trackQuerie.EndingTiming = _timeline.Duration;
                        trackQuerie.GlobalEnding = trackQuerie.GlobalBeginning + (trackQuerie.EndingTiming - trackQuerie.BeginningTiming) * _timeline.PixelPerSecond;
                    }
                    trackQuerie.LocalBeginning = trackQuerie.GlobalBeginning - _field.x;
                    trackQuerie.LocalEnding = trackQuerie.GlobalEnding - _field.x;

                    //Debug.Log("Setting EndGlobalPosition");
                    //Debug.Log("" + i.ToString()  + ": trackQuerie.GlobalEnding trackQuerie.GlobalEnding");
                    EndGlobalPosition = trackQuerie.GlobalEnding;

                    trackQueries.Add(trackQuerie);
                }
                else
                {
                    //default case, when track area begins at the end of one keynote and ends on the beginning of other
                    TrackQuery trackQuerie = new TrackQuery();
                    
                    trackQuerie.GlobalBeginning = keynoteQueries[i].GlobalEnding;
                    trackQuerie.GlobalEnding = keynoteQueries[i + 1].GlobalBeginning;
                    trackQuerie.BeginningTiming = keynoteQueries[i].Keynote.Timing;
                    trackQuerie.EndingTiming = keynoteQueries[i + 1].Keynote.Timing;

                    trackQuerie.LocalBeginning = trackQuerie.GlobalBeginning - _field.x;
                    trackQuerie.LocalEnding = trackQuerie.GlobalEnding - _field.x;

                    trackQueries.Add(trackQuerie);
                }
            }

            if (trackQueries.Count > 0)
            {
                StartGlobalPositionTiming = trackQueries[0].BeginningTiming;
                EndGlobalPositionTiming = trackQueries[trackQueries.Count - 1].EndingTiming;
            }

            if (keynoteQueries.Count == 0)
            {
                //that case means that no keynotes is rendered in the view area and we must add whole area as track area

                TrackQuery trackQuerie = new TrackQuery();

                if (_field.x <= 0f)
                {
                    trackQuerie.GlobalBeginning = 0f;
                    if (keynoteQueryBeforeBeginning == null)
                    {
                        trackQuerie.BeginningTiming = (_field.x * -1f) / _timeline.PixelPerSecond;
                    }
                    else
                    {
                        trackQuerie.BeginningTiming = keynoteQueryBeforeBeginning.Keynote.Timing + (keynoteQueryBeforeBeginning.GlobalEnding * -1f) / _timeline.PixelPerSecond;
                    }
                }
                else
                {
                    trackQuerie.GlobalBeginning = _field.x;
                    trackQuerie.BeginningTiming = 0f;
                }
                trackQuerie.GlobalEnding = _width;
                trackQuerie.EndingTiming = trackQuerie.BeginningTiming + (trackQuerie.GlobalEnding - trackQuerie.GlobalBeginning) / _timeline.PixelPerSecond;
                if (trackQuerie.EndingTiming > _timeline.Duration)
                {
                    trackQuerie.EndingTiming = _timeline.Duration;
                    trackQuerie.GlobalEnding = trackQuerie.GlobalBeginning + (trackQuerie.EndingTiming - trackQuerie.BeginningTiming) * _timeline.PixelPerSecond;
                }
                trackQuerie.LocalBeginning = trackQuerie.GlobalBeginning - _field.x;
                trackQuerie.LocalEnding = trackQuerie.GlobalEnding - _field.x;

                EndGlobalPosition = trackQuerie.GlobalEnding;

                trackQueries.Add(trackQuerie);
            }

            Rect timelineRect = GetRelativeRect(_rect, new Rect(0, 0, 0, 1f - _tracksView.TimelinePercentage));
            Rect tracksRect = GetRelativeRect(_rect, new Rect(0, _tracksView.TimelinePercentage + _tracksView.TimelineBarPercentage, 0, 0f));
            Rect keynotesRect = GetRelativeRect(_rect, new Rect(0, _tracksView.TimelinePercentage, 0, 0f));

            _tracksSubview.ClearSubviews();
            _timelineSubview.ClearSubviews();
            _chaptersSubview.ClearSubviews();
            _timeSliderIsVisible = false;
            
            //drawing all subtracks first
            for (int i = 0; i < trackQueries.Count; i++)
            {
                float[] keynoteRange = _timeline.GetKeynoteRangeByTime(trackQueries[i].BeginningTiming);

                _tracksSubview.Draw(new Rect(trackQueries[i].LocalBeginning, 0f, trackQueries[i].LocalEnding - trackQueries[i].LocalBeginning, tracksRect.height),
                    new Rect(trackQueries[i].GlobalBeginning, tracksRect.y, trackQueries[i].GlobalEnding - trackQueries[i].GlobalBeginning, tracksRect.height),
                    _offset,
                    _field,
                    tracksRect,
                    _timeline,
                    _serializedObject,
                    _timelineWindow,
                    this,
                    trackQueries[i].BeginningTiming,
                    trackQueries[i].EndingTiming,
                    keynoteRange[0],
                    keynoteRange[1]);

                if (trackQueries[i].BeginningTiming <= _timeline.CurrentTime && _timeline.CurrentTime < trackQueries[i].EndingTiming)
                {
                    _timeSliderView.Draw(new Rect(trackQueries[i].LocalBeginning, 0f, trackQueries[i].LocalEnding - trackQueries[i].LocalBeginning, tracksRect.height),
                        new Rect(trackQueries[i].GlobalBeginning, tracksRect.y, trackQueries[i].GlobalEnding - trackQueries[i].GlobalBeginning, tracksRect.height),
                        _offset,
                        _field,
                        keynotesRect,
                        _timeline,
                        _serializedObject,
                        _timelineWindow,
                        this,
                        trackQueries[i].BeginningTiming,
                        trackQueries[i].EndingTiming);

                    _timeSliderIsVisible = true;
                }
            }

            //Debug.Log("trackQueries.Count = " + trackQueries.Count);
            //drawing all subtimelines second
            for (int i = 0; i < trackQueries.Count; i++)
            {
                _timelineSubview.Draw(new Rect(trackQueries[i].LocalBeginning, 0f, trackQueries[i].LocalEnding - trackQueries[i].LocalBeginning, timelineRect.height),
                    new Rect(trackQueries[i].GlobalBeginning, timelineRect.y, trackQueries[i].GlobalEnding - trackQueries[i].GlobalBeginning, timelineRect.height),
                    _offset,
                    _field,
                    timelineRect,
                    _timeline,
                    _serializedObject,
                    _timelineWindow,
                    this,
                    trackQueries[i].BeginningTiming,
                    trackQueries[i].EndingTiming);
            }

            //Debug.Log("keynoteQueries.Count = " + trackQueries.Count);
            //drawing all keynotes last
            for (int i = 0; i < keynoteQueries.Count; i++)
            {
                _chaptersSubview.Draw(new Rect(keynoteQueries[i].LocalBeginning, 0f, keynoteQueries[i].LocalEnding - keynoteQueries[i].LocalBeginning, _height),
                    new Rect(keynoteQueries[i].GlobalBeginning, keynotesRect.y, keynoteQueries[i].GlobalEnding - keynoteQueries[i].GlobalBeginning, keynotesRect.height),
                    _offset,
                    _field,
                    keynotesRect,
                    _timeline,
                    _serializedObject,
                    _timelineWindow,
                    this);
                _chaptersSubview.DrawThinChapter(keynoteQueries[i].Keynote);
                //_chaptersSubview.DrawKeynote(keynoteQueries[i].Keynote);
            }
        }
        #endregion
    }
}
