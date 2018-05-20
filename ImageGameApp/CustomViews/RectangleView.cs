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

        private Rect _rect;

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

        public RectangleView(Context context, float centerX, float centerY, double startWidth, double startHeight)
            : this(context)
        {
            _rect = new Rect(
                (int)(centerX - startWidth / 2),
                (int)(centerY - startHeight / 2),
                (int)(centerX + startWidth / 2),
                (int)(centerY + startHeight / 2));
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
            canvas.DrawRect(_rect, _paint);
        }

        public void MoveCenter(float newCenterX, float newCenterY)
        {
            _rect.Inset((int)(newCenterX - _rect.ExactCenterX()),
                (int)(newCenterY - _rect.ExactCenterY()));

            this.Invalidate();
            this.RequestLayout();
        }

        public void UpdateSize(int diffWidth, int diffHeight)
        {
            _rect.Offset(diffWidth, diffHeight);
            this.Invalidate();
            this.RequestLayout();
        }
    }
}