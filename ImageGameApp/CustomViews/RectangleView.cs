namespace ImageGameApp.CustomViews
{
    using System;
    using Android.Content;
    using Android.Graphics;
    using Android.Runtime;
    using Android.Util;
    using Android.Views;
    using ImageGameApp.Shared;

    [Register("imagegameapp.customviews.RectangleView")]
    public class RectangleView : View
    {
        private Paint _paint;
        private readonly Rect _bounderies;

        #region Overridden Ctors

        protected RectangleView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            this.Init();
        }

        public RectangleView(Context context) : base(context)
        {
            this.Init();
        }

        public RectangleView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.Init();
        }

        public RectangleView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            this.Init();
        }

        public RectangleView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            this.Init();
        }

        #endregion

        public RectangleView(Context context, Rect initialRect, Rect bounderies)
            : this(context)
        {
            this.InternalRect = initialRect;
            _bounderies = bounderies;
        }

        private void Init()
        {
            _paint = new Paint(PaintFlags.AntiAlias)
            {
                Color = Color.Red,
                StrokeWidth = 5,
            };

            _paint.SetStyle(Paint.Style.Stroke);
        }

        public bool InRect(float x, float y)
        {
            return this.InternalRect.Contains((int)x, (int)y);
        }

        public Rect InternalRect { get; private set; }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.DrawRect(this.InternalRect, _paint);
        }

        public void MoveCenter(float newCenterX, float newCenterY)
        {
            var rect = CreateRect(newCenterX, newCenterY, this.InternalRect.Width(), this.InternalRect.Height());
            this.UpdateIfValid(rect);
        }

        public void UpdateSize(float newWidth, float newHeight)
        {
            var rect = CreateRect(this.InternalRect.ExactCenterX(), this.InternalRect.ExactCenterY(), newWidth, newHeight);

            this.UpdateIfValid(rect);
        }

        private void UpdateIfValid(Rect rect)
        {
            if (this.ValidRect(rect))
            {
                this.InternalRect = rect;

                this.Invalidate();
                this.RequestLayout();
            }
        }

        private bool ValidRect(Rect rect)
        {
            return rect.Top >= _bounderies.Top
                   && rect.Left >= _bounderies.Left
                   && rect.Right <= _bounderies.Right
                   && rect.Bottom <= _bounderies.Bottom
                   && rect.Bottom >= rect.Top
                   && rect.Right >= rect.Left;
        }

        public static Rect CreateRect(float x, float y, double width, double height)
        {
            return new Rect(
                (int)(x - width / 2),
                (int)(y - height / 2),
                (int)(x + width / 2),
                (int)(y + height / 2));
        }
    }
}