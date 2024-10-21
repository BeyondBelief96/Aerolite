using Microsoft.Xna.Framework;

namespace AeroliteMonoGraphics.Graphics
{
    public class Camera
    {
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        private const int MinZoom = 1;
        private const int MaxZoom = 64;

        public Camera(int screenWidth, int screenHeight)
        {
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
            Position = Vector2.Zero;
            Zoom = 1.0f;
            Rotation = 0.0f;
        }

        public Vector2 Position { get; private set; }

        public Rectangle Bounds { get; private set; }

        public float Zoom { get; private set; }

        public float Rotation { get; private set; }

        public Matrix GetTransformation()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1.0f)) *
                   Matrix.CreateTranslation(new Vector3(_screenWidth * 0.5f, _screenHeight * 0.5f, 0.0f));
        }

        public void Move(Vector2 delta)
        {
            Position += delta;
            UpdateBounds();
        }

        public void MoveTo(Vector2 position)
        {
            Position = position;
            UpdateBounds();
        }

        public void ZoomIn(float factor)
        {
            Zoom = MathHelper.Clamp(Zoom * factor, MinZoom, MaxZoom);
            UpdateBounds();
        }

        public void ZoomOut(float factor)
        {
            Zoom = MathHelper.Clamp(Zoom / factor, MinZoom, MaxZoom);
            UpdateBounds();
        }

        public void SetZoom(float zoom)
        {
            Zoom = MathHelper.Clamp(zoom, MinZoom, MaxZoom);
            UpdateBounds();
        }

        public void Rotate(float angle)
        {
            Rotation += angle;
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            float viewWidth = _screenWidth / Zoom;
            float viewHeight = _screenHeight / Zoom;
            Bounds = new Rectangle(
                (int)(Position.X - viewWidth / 2),
                (int)(Position.Y - viewHeight / 2),
                (int)viewWidth,
                (int)viewHeight);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(GetTransformation()));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, GetTransformation());
        }

    }
}
