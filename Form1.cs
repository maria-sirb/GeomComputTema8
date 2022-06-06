using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeomComputTema8
{
    
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.LimeGreen, 1);
            StreamReader sr = new StreamReader(@"..\..\TextFile1.txt");
            Polygon pol = new Polygon(g, sr);
            List<Point> pointsList = new List<Point>(pol.PointsList);
            Stack<Point> eliminated = new Stack<Point>();
            List<Tuple<Point, Point, Point>> trianglesList = new List<Tuple<Point, Point, Point>>();
            while(pointsList.Count > 3)
            {
                for(int i = 0; i < pointsList.Count; i++)
                {
                    Point p1 = new Point();
                    Point p2 = new Point();
                    Point p3 = new Point();
                    p1 = pointsList[i];
                   
                    if (i == pointsList.Count - 2)
                    {
                        p2 = pointsList[i + 1];
                        p3 = pointsList[0];
                    }
                    else if(i == pointsList.Count - 1)
                    {
                        p2 = pointsList[0];
                        p3 = pointsList[1];
                    }
                    else
                    {
                        p2 = pointsList[i + 1];
                        p3 = pointsList[i + 2];
                    }
                    if(IsDiagonal(pointsList, p1, p3))
                    {
                        g.DrawLine(p, p1.X, p1.Y, p3.X, p3.Y);
                        Tuple<Point, Point, Point> triangle;
                        triangle = Tuple.Create(p1, p2, p3);
                        trianglesList.Add(triangle);
                        eliminated.Push(p2);
                        pointsList.Remove(p2);
                        break;
                    }
                }
            }
            
            //tricolorare
            List<Color> pointsColors = new List<Color>();
            Pen pinkPen = new Pen(Color.Fuchsia, 4);
            Pen orangePen = new Pen(Color.Orange, 4);
            Pen redPen = new Pen(Color.Red, 4);
            g.DrawEllipse(pinkPen, pointsList[0].X, pointsList[0].Y, 3, 3);
            pointsColors.Add(Color.Fuchsia);
            g.DrawEllipse(orangePen, pointsList[1].X, pointsList[1].Y, 3, 3);
            pointsColors.Add(Color.Orange);
            g.DrawEllipse(redPen, pointsList[2].X, pointsList[2].Y, 3, 3);
            pointsColors.Add(Color.Red);
            g.DrawEllipse(Pens.Black, eliminated.Peek().X, eliminated.Peek().Y, 6, 6);
            while (eliminated.Count > 0)
            {
               
                Dictionary<Color, bool> colors = new Dictionary<Color, bool>();
                colors.Add(Color.Fuchsia, false);
                colors.Add(Color.Red, false);
                colors.Add(Color.Orange, false);
                Tuple<Point, Point, Point> triangle;
                triangle = FindTriangle(pointsList, trianglesList, eliminated.Peek());
                Point p1 = triangle.Item1;
                Point p2 = triangle.Item2;
                colors[pointsColors[pointsList.FindIndex(s => s == p1)]] = true;
                colors[pointsColors[pointsList.FindIndex(s => s == p2)]] = true;
                foreach (KeyValuePair<Color, bool> kvp in colors)
                {
                    if(kvp.Value == false)
                    {
                        Pen pen = new Pen(kvp.Key, 4);
                        g.DrawEllipse(pen, eliminated.Peek().X, eliminated.Peek().Y, 3, 3);
                        pointsColors.Add(kvp.Key);
                        break;
                    }
                }
                pointsList.Add(eliminated.Pop());
                
            }

            //arie
            float area = 0;
            Random rnd = new Random();
            Point p0 = new Point(rnd.Next(0, 709), rnd.Next(0, 447));
            int j = 0, k = 1;
            while(j < pointsList.Count)
            {
                area += Area(p0, pointsList[j], pointsList[k]);
                if(k == pointsList.Count - 1)
                {
                    k = 0;
                }
                else
                {
                    k++;
                }
                j++;
            }
            listBox1.Items.Add(area);
            //listBox1.Show();




        }
       private static Tuple<Point, Point, Point> FindTriangle(List<Point> l, List<Tuple<Point, Point, Point>> t, Point a)
       {
            Tuple<Point, Point, Point> noTriangle;
            Point p = new Point(0, 0);
            noTriangle = Tuple.Create(p, p, p);
            for(int i = 0; i < l.Count; i++)
            {
                for(int j = i + 1; j < l.Count; j++)
                {
                    Tuple<Point, Point, Point> triangle;
                    triangle = Tuple.Create(l[i], l[j], a);
                    for(int k = 0; k < t.Count; k++)
                    {
                        if (IsEqual(triangle, t[k]))
                            return triangle;
                    }

                }
            }
            return noTriangle;
            
       }
        private static bool IsEqual(Tuple<Point, Point, Point> t1, Tuple<Point, Point, Point> t2)
        {
            bool foundItem1 = false;
            bool foundItem2 = false;
            bool foundItem3 = false;
            if (t2.Item1 == t1.Item1 ||
                t2.Item2 == t1.Item1 ||
                t2.Item3 == t1.Item1)
                foundItem1 = true;
            if (t2.Item1 == t1.Item2 ||
               t2.Item2 == t1.Item2 ||
               t2.Item3 == t1.Item2)
                foundItem2 = true;
            if (t2.Item1 == t1.Item3 ||
               t2.Item2 == t1.Item3 ||
               t2.Item3 == t1.Item3)
                foundItem3 = true;
            return foundItem1 && foundItem2 && foundItem3;

        }
        /// <summary>
        /// Computes the area of a triangle
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        private static float Area(Point A, Point B, Point C)
        {
            return (float)0.5 * Math.Abs(A.X * B.Y - B.X * A.Y + B.X * C.Y - C.X * B.Y + C.X * A.Y - A.X * C.Y);
        }
        /// <summary>
        /// Checks if three points turn left or right
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns>0 for collinear, 1 for rigth turn, 2 for left turn</returns>
        private static int Orientation(Point A, Point B, Point C)
        {
            int value = (B.Y - A.Y) * (C.X - B.X) - (C.Y - B.Y) * (B.X - A.X);
            if (value == 0)
            {
                return 0;  //collinear points
            }
            else
            {
                return value > 0 ? 1 : 2;  //clockwise(right) / counterclockwise(left) orientation
            }
        }
        /// <summary>
        /// Checks if two line segments intersect
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="Q1"></param>
        /// <param name="P2"></param>
        /// <param name="Q2"></param>
        /// <returns>true / false</returns>
        private static bool DoIntersect(Point P1, Point Q1, Point P2, Point Q2)
        {
            int o1 = Orientation(P1, Q1, P2);
            int o2 = Orientation(P1, Q1, Q2);
            int o3 = Orientation(P2, Q2, P1);
            int o4 = Orientation(P2, Q2, Q1);

            if (o1 != o2 && o3 != o4)
                return true;
            else return false;
        }
        /// <summary>
        /// Checks if a line segment P1P2 intersects the sides of a polygon(list)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns>true / false</returns>
        private static bool IntersectsSides(List<Point> l, Point P1, Point P2)
        {
            for (int i = 0; i < l.Count - 1; i++)
            {
                if (l[i] != P1 && l[i] != P2 && l[i + 1] != P1 && l[i + 1] != P2)
                {
                    if (DoIntersect(P1, P2, l[i], l[i + 1]))
                        return true;

                }
            }
            return false;
        }
        /// <summary>
        /// checks if a line segment is inside a polygon(list)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="s"></param>
        /// <returns>true / false</returns>
        private static bool IsInside(List<Point> l, Point A, Point B)
        {
            //if segment doesn't intersect sides of the polygon  =>it's enough to check
            //if the middle is inside 
            Point middle = new Point();
            middle.X = (B.X + A.X) / 2;
            middle.Y = (B.Y + A.Y) / 2;
            Point extreme = new Point(800, middle.Y);
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % l.Count;
                if (DoIntersect(l[i], l[next], middle, extreme))
                {
                    count++;
                }
                i = next;
            } while (i != 0);
            return count % 2 == 1;

        }
        /// <summary>
        /// Checks if a line segment is a diagonal in polygon 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static bool IsDiagonal(List<Point> l, Point A, Point B)
        {
            return !IntersectsSides(l, A, B) && IsInside(l, A, B);
        }
    }
}
