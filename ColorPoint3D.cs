using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormalVectorEstimation
{
    public class ColorPoint3D
    {
        private float x;
        private float y;
        private float z;
        private int r;
        private int g;
        private int b;

        public ColorPoint3D(float x, float y, float z, int r, int g, int b)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Z { get => z; set => z = value; }
        public int R { get => r; set => r = value; }
        public int G { get => g; set => g = value; }
        public int B { get => b; set => b = value; }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + "; " + R + ", " + G + ", " + B + ")";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ColorPoint3D point = obj as ColorPoint3D;
            if (point == null)
            {
                return false;
            }

            return (this.X == point.X)
            && (this.Y == point.Y)
            && (this.Z == point.Z)
            && (this.R == point.R)
            && (this.G == point.G)
            && (this.B == point.B);
        }

        public override int GetHashCode()
        {
            int x = (int)this.X;
            int y = (int)this.Y;
            int z = (int)this.Z;
            return x ^ y ^ z ^ r ^ g ^ b;
        }

        public static ColorPoint3D operator +(ColorPoint3D u, ColorPoint3D v)
        {
            return new ColorPoint3D(
                u.X + v.X,
                u.Y + v.Y,
                u.Z + v.Z,
                0, 0, 0
            );
        }

        public static ColorPoint3D operator -(ColorPoint3D u, ColorPoint3D v)
        {
            return new ColorPoint3D(
                u.X - v.X,
                u.Y - v.Y,
                u.Z - v.Z,
                0, 0, 0
            );
        }

        public static ColorPoint3D crossProduct(ColorPoint3D u, ColorPoint3D v)
        {
            return new ColorPoint3D(
                u.Y * v.Z - u.Z * v.Y,
                u.Z * v.X - u.X * v.Z,
                u.X * v.Y - u.Y * v.X,
                0, 0, 0
            );
        }
    }
}
