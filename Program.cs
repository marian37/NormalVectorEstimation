using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormalVectorEstimation
{
    class Program
    {
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

            Console.ReadLine();
        }
    }
}
