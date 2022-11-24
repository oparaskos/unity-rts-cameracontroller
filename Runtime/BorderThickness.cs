using System;

namespace RTSCameraController.Runtime
{
    [Serializable]
    public struct BorderThickness
    {

        public BorderThickness(float Top, float Right, float Bottom, float Left)
        {
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
            this.Left = Left;
        }
        public static BorderThickness zero()
        {
            return new BorderThickness(0, 0, 0, 0);
        }

        public static BorderThickness one()
        {
            return new BorderThickness(1, 1, 1, 1);
        }

        public static BorderThickness operator *(BorderThickness a, float d)
        {
            return new BorderThickness(a.Top * d, a.Right * d, a.Bottom * d, a.Left * d);
        }


        public float Top;
        public float Right;
        public float Bottom;
        public float Left;
    }

}