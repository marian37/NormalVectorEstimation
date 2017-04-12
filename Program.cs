using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace NormalVectorEstimation
{
    class Program
    {
        private static List<ColorPoint3D> points = new List<ColorPoint3D>();

        static void Main(string[] args)
        {
            ColorPoint3D a = new ColorPoint3D(0, 0, 0, 255, 0, 0);
            ColorPoint3D b = new ColorPoint3D(1, 0, 0, 0, 255, 0);
            ColorPoint3D c = new ColorPoint3D(0, 1, 0, 0, 0, 255);

            ColorPoint3D u = b - a;
            ColorPoint3D v = c - a;
            Console.WriteLine(u);
            Console.WriteLine(v);

            ColorPoint3D n = ColorPoint3D.crossProduct(u, v);
            Console.WriteLine("Normal: " + n);
            
            points.Add(a);
            points.Add(b);
            points.Add(c);            

            points.Add(u);
            points.Add(v);

            points.Add(n);

            try
            {
                Visualization visualization = new Visualization(950, 950, points);
                visualization.ShowDialog();                
            }            
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
            }            
        }

        public static void Draw()
        {
            Draw1();
        }

        private static void Draw1()
        {
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < 3; i++)
            {
                Color c = Color.FromArgb(points[i].R, points[i].G, points[i].B);
                GL.Color3(c);
                GL.Vertex3(points[i].X, points[i].Y, points[i].Z);
            }
            GL.End();

            ColorPoint3D a = points[0];
            ColorPoint3D u = points[3];
            ColorPoint3D v = points[4];
            ColorPoint3D n = points[5];

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Yellow);
            GL.Vertex3(a.X, a.Y, a.Z);
            GL.Vertex3(u.X, u.Y, u.Z);
            GL.Color3(Color.Orange);
            GL.Vertex3(a.X, a.Y, a.Z);
            GL.Vertex3(v.X, v.Y, v.Z);
            GL.Color3(Color.Magenta);
            GL.Vertex3(a.X, a.Y, a.Z);
            GL.Vertex3(n.X, n.Y, n.Z);
            GL.End();
        }
    }
}
