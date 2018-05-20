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
        private LineView _currentLineView;
        private RectangleView _currentRectangleView;

        private bool _inAddSectionMode;

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
            var imageView = (ImageView) sender;
            TouchLocation currentPoint = TouchLocation.FromMotion(imageView, touchEventArgs.Event);
            _label.Text = currentPoint.ToString();

            if (!_inAddSectionMode)
            {
                return;
            }

            switch (touchEventArgs.Event.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    

                    break;

                case MotionEventActions.Move:
                    _currentLineView.UpdateEndPoint(touchEventArgs.Event.RawX, touchEventArgs.Event.RawY);
                    break;

                case MotionEventActions.Up:
                    if (_currentRectangleView == null)
                    {
                        // default rect size is 5% of image
                        double defaultWidth = imageView.Width * 0.05;
                        double defaultHeight = imageView.Height * 0.05;
                        _currentRectangleView = new RectangleView(this,
                            currentPoint.ScreenX,
                            currentPoint.ScreenY,
                            defaultWidth,
                            defaultHeight);

                        _imageLayout.AddView(_currentRectangleView);
                    }

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
            float x = motionEvent.RawX - view.Left;
            float y = motionEvent.RawY - view.Top;

            return new TouchLocation
            {
                InView = x < view.Width && x > 0 && y < view.Height && y > 0,
                ScreenX = motionEvent.RawX,
                ScreenY = motionEvent.RawY,
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

