namespace ImageGameApp.CustomViews
{
    using System;
    using Android.Content;
    using Android.Graphics;
    using Android.Runtime;
    using Android.Util;
    using Android.Views;

    [Register("imagegameapp.customviews.DrawView")]
    public class DrawView : View
    {
        private readonly Paint _paint = new Paint();

        protected DrawView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            this.Init();
        }

        public DrawView(Context context) : base(context)
        {
            this.Init();
        }

        public DrawView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.Init();
        }

        public DrawView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            this.Init();
        }

        public DrawView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            this.Init();
        }

        private void Init()
        {
            _paint.Color = Color.Red;
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.DrawLine(0, 0, 20, 20, _paint);
            canvas.DrawLine(20, 0, 0, 20, _paint);
        }
    }
}