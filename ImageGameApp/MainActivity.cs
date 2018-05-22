using Android.App;
using Android.Widget;
using Android.OS;

namespace ImageGameApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.InputMethodServices;
    using Android.Util;
    using Android.Views;
    using Com.Lilarcor.Cheeseknife;
    using ImageGameApp.CustomViews;

    [Activity(Label = "ImageGameApp", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : Activity
    {
        private TextView _label;
        private RelativeLayout _imageLayout;

        private List<TouchLocation> _endPoints = new List<TouchLocation>();
        private RectangleView _currentRectangleView;


        private ScaleGestureDetector _scaleGestureDetector;

        private bool _inAddSectionMode;
        private bool _inMoveState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.HideSystemUi();

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.Main);

            _label = this.FindViewById<TextView>(Resource.Id.DebugLabel);
            _imageLayout = this.FindViewById<RelativeLayout>(Resource.Id.ImageLayout);

            this.FindViewById<ImageView>(Resource.Id.ImageView).Touch += this.ImageViewOnTouch;
            this.FindViewById<Button>(Resource.Id.ManageSectionAdd).Click += this.AddSectionButtonOnClick;

            _inAddSectionMode = false;

        }

        private void UpdateRectSize(float newWidth, float newHeight)
        {
            _label.Text = $"Diff [{newWidth},{newHeight}]";

            _currentRectangleView?.UpdateSize((int)newWidth, (int)newHeight);
        }

        private class MyOnScaleGestureListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            private readonly Action<float, float> _updateOnSpan;

            private float _initialSpanX;
            private float _initialSpanY;

            private readonly Rect _startRect;

            public MyOnScaleGestureListener(Action<float, float> updateOnSpan, Rect startRect)
            {
                _updateOnSpan = updateOnSpan;
                _startRect = startRect;
            }

            public override bool OnScaleBegin(ScaleGestureDetector detector)
            {
                _initialSpanX = detector.CurrentSpanX;
                _initialSpanY = detector.CurrentSpanY;
                return base.OnScaleBegin(detector);
            }

            public override bool OnScale(ScaleGestureDetector detector)
            {
                if (_initialSpanX > 0)
                {
                    float diffx = (detector.CurrentSpanX - _initialSpanX);
                    float diffy = (detector.CurrentSpanY - _initialSpanY);

                    _updateOnSpan(_startRect.Width() + diffx, _startRect.Height() + diffy);
                }
                else
                {
                    _initialSpanX = detector.CurrentSpanX;
                    _initialSpanY = detector.CurrentSpanY;
                }
                
                
                return base.OnScale(detector);
            }
        }
        
        private void HideSystemUi()
        {
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);

            View decorView = this.Window.DecorView;
            decorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.HideNavigation
                    | SystemUiFlags.Fullscreen
                    | SystemUiFlags.LayoutFullscreen
                    | SystemUiFlags.LayoutHideNavigation
                    | SystemUiFlags.LayoutStable
                    | SystemUiFlags.ImmersiveSticky);
        }

        private void ImageViewOnTouch(object sender, View.TouchEventArgs touchEventArgs)
        {
            MotionEvent motionEvent = touchEventArgs.Event;
            
            var imageView = (ImageView) sender;

            TouchLocation currentPoint = TouchLocation.FromMotion(imageView, motionEvent);
            
            switch (motionEvent.ActionMasked)
            {
                case MotionEventActions.Down:
                    if (_currentRectangleView != null)
                    {
                        _inMoveState = motionEvent.PointerCount == 1
                                       && _currentRectangleView.InRect(currentPoint.ScreenX, currentPoint.ScreenY);

                        var scaleListener = new MyOnScaleGestureListener(this.UpdateRectSize, _currentRectangleView.InternalRect);
                        _scaleGestureDetector = new ScaleGestureDetector(this, scaleListener);
                    }

                    break;

                case MotionEventActions.Move:
                    if (_currentRectangleView != null)
                    {
                        if (_inMoveState)
                        {
                            _currentRectangleView?.MoveCenter(touchEventArgs.Event.RawX, touchEventArgs.Event.RawY);
                            _label.Text = $"Moving center to {currentPoint}";
                        }
                        else
                        {
                            _scaleGestureDetector.OnTouchEvent(motionEvent);
                        }
                    }
                        
                    break;

                case MotionEventActions.Up:
                    if (_inAddSectionMode && _currentRectangleView == null)
                    {
                        var drawingRect = new Rect();
                        imageView.GetDrawingRect(drawingRect);

                        double defaultWidth = imageView.Width * 0.10;
                        double defaultHeight = imageView.Height * 0.10;
                        _currentRectangleView = new RectangleView(this,
                            RectangleView.CreateRect(
                            currentPoint.ScreenX,
                            currentPoint.ScreenY,
                            defaultWidth,
                            defaultHeight),
                            drawingRect);

                        _imageLayout.AddView(_currentRectangleView);
                    }

                    _inMoveState = false;
                    break;
            }

        }

        private void AddSectionButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (_inAddSectionMode)
            {
                _currentRectangleView = null;
            }

            ((Button)sender).Text = _inAddSectionMode ? "Add Section" : "Done Edit";
            _inAddSectionMode = !_inAddSectionMode;
        }

        private string LocationsToString()
        {
            return string.Join(",", _endPoints.Select(p => p.ToString()));
        }
    }

    public class TouchLocation
    {
        public bool InView { get; set; }

        public float PercentX { get; set; }
        public float PercentY { get; set; }

        public float ScreenX { get; set; }
        public float ScreenY { get; set; }

        public float ViewX { get; set; }
        public float ViewY { get; set; }

        public static TouchLocation FromMotion(View view, MotionEvent motionEvent)
        {

            float rawX = motionEvent.RawX;
            float rawY = motionEvent.RawY;

            float x = rawX - view.Left;
            float y = rawY - view.Top;

            return new TouchLocation
            {
                InView = x < view.Width && x > 0 && y < view.Height && y > 0,
                ScreenX = rawX,
                ScreenY = rawY,
                ViewX = x,
                ViewY = y,
                PercentX = x / view.Width * 100,
                PercentY = y / view.Height * 100
            };
        }

        public override string ToString()
        {
            string inView = this.InView ? "t" : "f";
            return $"{inView}[{this.PercentX},{this.PercentY}]";
        }
    }
}

