using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Globalization;

namespace NormalVectorEstimation
{
    class Program
    {
        private static List<ColorPoint3D> points = new List<ColorPoint3D>();
        private static Dictionary<ColorPoint3D, ColorPoint3D> normals = new Dictionary<ColorPoint3D, ColorPoint3D>();

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

            points = ReadFromFile("../../../cube.off");
            Dictionary<ColorPoint3D, List<ColorPoint3D>> neighbours = GetNeighbours(points);

            //Ransac(neighbours);
            Pca(neighbours);
            //LinearRegression();

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
            //Draw1();
            DrawNormals();
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

        private static List<ColorPoint3D> ReadFromFile(string fileName)
        {
            string[] rows = System.IO.File.ReadAllLines(fileName);
            string row = rows[1];
            string[] tokens = row.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int numberOfInputPoints = Int32.Parse(tokens[0]);
            List<ColorPoint3D> input = new List<ColorPoint3D>(numberOfInputPoints);

            //CultureInfo ci = CultureInfo.InvariantCulture;
            CultureInfo ci = CultureInfo.CurrentCulture;

            Object lockObject = new Object();

            Parallel.For(0, numberOfInputPoints, i =>
            {
                //for (int i = 0; i < numberOfInputPoints; i++) {
                string[] tokensLocal = rows[i + 2].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                float x = float.Parse(tokensLocal[0], ci);
                float y = float.Parse(tokensLocal[1], ci);
                float z = float.Parse(tokensLocal[2], ci);

                int r = int.Parse(tokensLocal[3], ci);
                int g = int.Parse(tokensLocal[4], ci);
                int b = int.Parse(tokensLocal[5], ci);

                ColorPoint3D point = new ColorPoint3D(x, y, z, r, g, b);
                lock (lockObject)
                {
                    input.Add(point);
                }
                //}
            });


            return input;
        }

        private static Dictionary<ColorPoint3D, List<ColorPoint3D>> GetNeighbours(List<ColorPoint3D> points)
        {
            Dictionary<ColorPoint3D, List<ColorPoint3D>> neighbourhood = new Dictionary<ColorPoint3D, List<ColorPoint3D>>();

            int density = 70;
            float epsilon = 0.1f;
            for (int i = 0; i < points.Count; i += density)
            {
                ColorPoint3D point = points[i];
                List<ColorPoint3D> neighbours = new List<ColorPoint3D>();
                foreach (ColorPoint3D p in points)
                {
                    if (SquareDistance(point, p) < epsilon)
                    {
                        neighbours.Add(p);
                    }
                }                

                neighbourhood.Add(point, neighbours);
            }

            Console.WriteLine("Celkom " + neighbourhood.Count + " - " + points.Count);

            return neighbourhood;
        }

        private static float SquareDistance(ColorPoint3D a, ColorPoint3D b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z);
        }

        private static void Ransac(Dictionary<ColorPoint3D, List<ColorPoint3D>> neighbourhood)
        {
            normals = new Dictionary<ColorPoint3D, ColorPoint3D>();

            foreach (KeyValuePair<ColorPoint3D, List<ColorPoint3D>> pair in neighbourhood)
            {
                ColorPoint3D normal = RansacNormal(pair.Key, pair.Value);
                normals.Add(pair.Key, normal);
            }
        }

        private static ColorPoint3D RansacNormal(ColorPoint3D p, List<ColorPoint3D> n)
        {
            float epsilon = 0.01f;
            Random rand = new Random();

            ColorPoint3D bestNormal = p;
            int maxInliers = 0;

            for (int k = 0; k < 300; k++)
            {
                ColorPoint3D a = n[rand.Next(n.Count)];
                ColorPoint3D b = n[rand.Next(n.Count)];
                ColorPoint3D c = n[rand.Next(n.Count)];

                ColorPoint3D u = b - a;
                ColorPoint3D v = c - a;

                ColorPoint3D normal = ColorPoint3D.crossProduct(u, v);

                float d = -(normal.X * a.X + normal.Y * a.Y + normal.Z * a.Z);
                int inliers = 0;

                foreach (ColorPoint3D p1 in n)
                {
                    if (PointToPlaneDistance(p1, normal, d) < epsilon)
                    {
                        inliers++;
                    }
                }

                if (inliers > maxInliers)
                {
                    maxInliers = inliers;
                    bestNormal = normal;
                }
            }            

            return bestNormal;
        }

        private static float PointToPlaneDistance(ColorPoint3D p, ColorPoint3D n, float d)
        {
            return Math.Abs(n.X * p.X + n.Y * p.Y + n.Z * p.Z + d) / (float)(Math.Sqrt(n.X * n.X + n.Y * n.Y + n.Z * n.Z));
        }

        private static void Pca(Dictionary<ColorPoint3D, List<ColorPoint3D>> neighbourhood)
        {

        }

        private static void LinearRegression(Dictionary<ColorPoint3D, List<ColorPoint3D>> neighbourhood)
        {

        }

        private static void DrawNormals()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Cyan);
            foreach (KeyValuePair<ColorPoint3D, ColorPoint3D> pair in normals)
            {
                ColorPoint3D n = pair.Value + pair.Key;
                GL.Vertex3(pair.Key.X, pair.Key.Y, pair.Key.Z);
                GL.Vertex3(n.X, n.Y, n.Z);
            }
            GL.End();
        }
    }
}
