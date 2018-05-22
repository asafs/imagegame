namespace ImageGameApp.CustomViews
{
    using System;
    using ImageGameApp.Shared;
    using Android.Content;
    using Android.Runtime;
    using Android.Util;
    using Android.Views;
    using Android.Graphics;

    [Register("imagegameapp.customviews.LineView")]
    public class LineView : View
    {
        private Paint _paint;
        private readonly SimplePoint _start;
        private SimplePoint _end;

        #region Overridden Ctors

        protected LineView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            this.Init();
        }

        public LineView(Context context) : base(context)
        {
            this.Init();
        }

        public LineView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.Init();
        }

        public LineView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            this.Init();
        }

        public LineView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            this.Init();
        }

        #endregion

        public LineView(Context context, float startX, float startY) : this(context)
        {
            _start = new SimplePoint(startX, startY);
            _end = null;
        }

        private void Init()
        {
            _paint = new Paint(PaintFlags.AntiAlias)
            {
                Color = Color.Red,
                StrokeWidth = 5
            };
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (_end != null)
            {
                canvas.DrawLine((float)_start.X, (float)_start.Y, (float)_end.X, (float)_end.Y, _paint);
            }            
        }

        public void UpdateEndPoint(float x, float y)
        {
            _end = new SimplePoint(x, y);
            this.Invalidate();
            this.RequestLayout();
        }


    }

}