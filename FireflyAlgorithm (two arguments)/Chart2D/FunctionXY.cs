using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _Chart2D
{
    internal class FunctionXY
    {
        double width, height;
        Brush color;
        Func<double, double, double> F;

        public double Xmin, Xmax, Ymin, Ymax;
        double Zmin, Zmax;

        double XSpacing = 0.1;
        double YSpacing = 0.1;

        int XNumber, YNumber;

        int NumberContours;
        double[] zlevels;

        int size;

        Point3D[,] pts;

        public FunctionXY(double width, double height, int contours = 5, double Xmin = -10, double Xmax = 10, double Ymin = -10, double Ymax = 10)
        {
            this.width = width;
            this.height = height;
            NumberContours = contours;

            color = Brushes.Red;

            this.Xmin = Xmin;
            this.Xmax = Xmax;
            this.Ymin = Ymin;
            this.Ymax = Ymax;

            XNumber = Convert.ToInt16((Xmax - Xmin) / XSpacing) + 1;
            YNumber = Convert.ToInt16((Ymax - Ymin) / YSpacing) + 1;

            pts = new Point3D[XNumber, YNumber];

            size = (int)width / XNumber + 1;
        }

        public void SetFunc(Func<double, double, double> func) => F = func;
        public void SetNumberContours(int value) => NumberContours = value;

        private double MinZ()
        {
            double zmin = 0;
            for (int i = 0; i < pts.GetLength(0); i++)
            {
                for (int j = 0; j < pts.GetLength(1); j++)
                {
                    zmin = Math.Min(zmin, pts[i, j].Z);
                }
            }
            return zmin;
        }
        private double MaxZ()
        {
            double zmax = 0;
            for (int i = 0; i < pts.GetLength(0); i++)
            {
                for (int j = 0; j < pts.GetLength(1); j++)
                {
                    zmax = Math.Max(zmax, pts[i, j].Z);
                }
            }
            return zmax;
        }

        public void Calculation()
        {
            for (int i = 0; i < YNumber; ++i)
            {
                for (int j = 0; j < XNumber; ++j)
                {
                    double x = Xmin + i * XSpacing;
                    double y = Ymin + j * YSpacing;
                    double z = F(x, y);
                    pts[i, j] = new Point3D(x, y, z);
                }
            }
            Zmin = MinZ();
            Zmax = MaxZ();
        }

        public void DrawFunc(DrawingContext dc)
        {
            for (int i = 0; i < pts.GetLength(0); i++)
            {
                for (int j = 0; j < pts.GetLength(1); j++)
                {
                    var value = pts[i, j].Z;
                    var rgb = (byte)Tools.Map(value, Zmin, Zmax, 0, 255);
                    Brush brush = new SolidColorBrush(Color.FromRgb(0, 0, rgb));

                    var x = pts[i, j].X;
                    var y = pts[i, j].Y;
                    Point p2D = new Point(x, y);
                    Point norm = Tools.Normalize(p2D, width, height, Xmin, Xmax, Ymin, Ymax);

                    Rect rect = new Rect()
                    {
                        X = norm.X,
                        Y = norm.Y,
                        Width = size,
                        Height = size
                    };

                    dc.DrawRectangle(brush, null, rect);
                }
            }
        }
        public void DrawContour(DrawingContext dc)
        {
            Point[] pta = new Point[2];
            zlevels = new double[NumberContours];

            for (int i = 0; i < NumberContours; i++)
            {
                zlevels[i] = Zmin + i * (Zmax - Zmin) / (NumberContours - 1);
            }

            int i0, i1, i2, j0, j1, j2;
            double zratio = 1;

            // Draw contour on the XY plane:
            for (int i = 0; i < pts.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < pts.GetLength(1) - 1; j++)
                {
                    for (int k = 0; k < NumberContours; k++)
                    {
                        double hue = 0;
                        // Left triangle:
                        i0 = i;
                        j0 = j;
                        i1 = i;
                        j1 = j + 1;
                        i2 = i + 1;
                        j2 = j + 1;
                        if ((zlevels[k] >= pts[i0, j0].Z && zlevels[k] < pts[i1, j1].Z ||
                            zlevels[k] < pts[i0, j0].Z && zlevels[k] >= pts[i1, j1].Z) &&
                            (zlevels[k] >= pts[i1, j1].Z && zlevels[k] <
                            pts[i2, j2].Z || zlevels[k] < pts[i1, j1].Z && zlevels[k] >= pts[i2, j2].Z))
                        {
                            zratio = (zlevels[k] - pts[i0, j0].Z) /
                                (pts[i1, j1].Z - pts[i0, j0].Z);
                            pta[0] = Tools.Normalize(new Point(pts[i0, j0].X, (1 - zratio) * pts[i0, j0].Y + zratio * pts[i1, j1].Y), width, height, Xmin, Xmax, Ymin, Ymax);
                            zratio = (zlevels[k] - pts[i1, j1].Z) / (pts[i2, j2].Z - pts[i1, j1].Z);
                            pta[1] = Tools.Normalize(new Point((1 - zratio) * pts[i1, j1].X + zratio * pts[i2, j2].X, pts[i1, j1].Y), width, height, Xmin, Xmax, Ymin, Ymax);

                            DrawLine(dc, pta[0], pta[1], k);
                        }
                        else if ((zlevels[k] >= pts[i0, j0].Z && zlevels[k]
                            < pts[i2, j2].Z || zlevels[k] < pts[i0, j0].Z
                            && zlevels[k] >= pts[i2, j2].Z) &&
                                (zlevels[k] >= pts[i1, j1].Z && zlevels[k] <
                            pts[i2, j2].Z || zlevels[k] < pts[i1, j1].Z
                            && zlevels[k] >= pts[i2, j2].Z))
                        {
                            zratio = (zlevels[k] - pts[i0, j0].Z) /
                                (pts[i2, j2].Z - pts[i0, j0].Z);
                            pta[0] = Tools.Normalize(new Point((1 - zratio) * pts[i0, j0].X + zratio * pts[i2, j2].X, (1 - zratio) * pts[i0, j0].Y + zratio * pts[i2, j2].Y), width, height, Xmin, Xmax, Ymin, Ymax);
                            zratio = (zlevels[k] - pts[i1, j1].Z) / (pts[i2, j2].Z - pts[i1, j1].Z);
                            pta[1] = Tools.Normalize(new Point((1 - zratio) * pts[i1, j1].X + zratio * pts[i2, j2].X, pts[i1, j1].Y), width, height, Xmin, Xmax, Ymin, Ymax);

                            DrawLine(dc, pta[0], pta[1], k);
                        }
                        else if ((zlevels[k] >= pts[i0, j0].Z && zlevels[k]
                         < pts[i1, j1].Z || zlevels[k] < pts[i0, j0].Z
                         && zlevels[k] >= pts[i1, j1].Z) &&
                            (zlevels[k] >= pts[i0, j0].Z && zlevels[k] <
                         pts[i2, j2].Z || zlevels[k] < pts[i0, j0].Z &&
                         zlevels[k] >= pts[i2, j2].Z))
                        {
                            zratio = (zlevels[k] - pts[i0, j0].Z) /
                                (pts[i1, j1].Z - pts[i0, j0].Z);
                            pta[0] = Tools.Normalize(new Point(pts[i0, j0].X, (1 - zratio) * pts[i0, j0].Y + zratio * pts[i1, j1].Y), width, height, Xmin, Xmax, Ymin, Ymax);
                            zratio = (zlevels[k] - pts[i0, j0].Z) / (pts[i2, j2].Z - pts[i0, j0].Z);
                            pta[1] = Tools.Normalize(new Point(pts[i0, j0].X * (1 - zratio) + pts[i2, j2].X * zratio, pts[i0, j0].Y * (1 - zratio) + pts[i2, j2].Y * zratio), width, height, Xmin, Xmax, Ymin, Ymax);
                            
                            DrawLine(dc, pta[0], pta[1], k);
                        }

                        // right triangle:
                        i0 = i;
                        j0 = j;
                        i1 = i + 1;
                        j1 = j;
                        i2 = i + 1;
                        j2 = j + 1;
                        if ((zlevels[k] >= pts[i0, j0].Z && zlevels[k] <
                            pts[i1, j1].Z || zlevels[k] < pts[i0, j0].Z
                            && zlevels[k] >= pts[i1, j1].Z) &&
                                (zlevels[k] >= pts[i1, j1].Z && zlevels[k]
                            < pts[i2, j2].Z || zlevels[k] < pts[i1, j1].Z
                            && zlevels[k] >= pts[i2, j2].Z))
                        {
                            zratio = (zlevels[k] - pts[i0, j0].Z) /
                                (pts[i1, j1].Z - pts[i0, j0].Z);
                            pta[0] = Tools.Normalize(new Point(pts[i0, j0].X * (1 - zratio) + pts[i1, j1].X * zratio, pts[i0, j0].Y), width, height, Xmin, Xmax, Ymin, Ymax);
                            zratio = (zlevels[k] - pts[i1, j1].Z) / (pts[i2, j2].Z - pts[i1, j1].Z);
                            pta[1] = Tools.Normalize(new Point(pts[i1, j1].X, pts[i1, j1].Y * (1 - zratio) + pts[i2, j2].Y * zratio), width, height, Xmin, Xmax, Ymin, Ymax);

                            DrawLine(dc, pta[0], pta[1], k);
                        }
                        else if ((zlevels[k] >= pts[i0, j0].Z && zlevels[k]
                            < pts[i2, j2].Z || zlevels[k] < pts[i0, j0].Z
                            && zlevels[k] >= pts[i2, j2].Z) &&
                                (zlevels[k] >= pts[i1, j1].Z && zlevels[k] <
                            pts[i2, j2].Z || zlevels[k] < pts[i1, j1].Z
                            && zlevels[k] >= pts[i2, j2].Z))
                        {
                            zratio = (zlevels[k] - pts[i0, j0].Z) /
                                (pts[i2, j2].Z - pts[i0, j0].Z);
                            pta[0] = Tools.Normalize(new Point(pts[i0, j0].X * (1 - zratio) + pts[i2, j2].X * zratio, pts[i0, j0].Y * (1 - zratio) + pts[i2, j2].Y * zratio), width, height, Xmin, Xmax, Ymin, Ymax);
                            zratio = (zlevels[k] - pts[i1, j1].Z) / (pts[i2, j2].Z - pts[i1, j1].Z);
                            pta[1] = Tools.Normalize(new Point(pts[i1, j1].X, pts[i1, j1].Y * (1 - zratio) + pts[i2, j2].Y * zratio), width, height, Xmin, Xmax, Ymin, Ymax);

                            DrawLine(dc, pta[0], pta[1], k);
                        }
                        else if ((zlevels[k] >= pts[i0, j0].Z && zlevels[k]
                            < pts[i1, j1].Z || zlevels[k] < pts[i0, j0].Z
                            && zlevels[k] >= pts[i1, j1].Z) &&
                                (zlevels[k] >= pts[i0, j0].Z && zlevels[k] <
                            pts[i2, j2].Z || zlevels[k] < pts[i0, j0].Z
                            && zlevels[k] >= pts[i2, j2].Z))
                        {
                            zratio = (zlevels[k] - pts[i0, j0].Z) /
                                (pts[i1, j1].Z - pts[i0, j0].Z);
                            pta[0] = Tools.Normalize(new Point(pts[i0, j0].X * (1 - zratio) + pts[i1, j1].X * zratio, pts[i0, j0].Y), width, height, Xmin, Xmax, Ymin, Ymax);
                            zratio = (zlevels[k] - pts[i0, j0].Z) / (pts[i2, j2].Z - pts[i0, j0].Z);
                            pta[1] = Tools.Normalize(new Point(pts[i0, j0].X * (1 - zratio) + pts[i2, j2].X * zratio, pts[i0, j0].Y * (1 - zratio) + pts[i2, j2].Y * zratio), width, height, Xmin, Xmax, Ymin, Ymax);

                            DrawLine(dc, pta[0], pta[1], k);
                        }
                    }
                }
            }

            DrawHeatBar(dc);
        }
        private void DrawLine(DrawingContext dc, Point p0, Point p1, int k)
        {
            var hue = Tools.Map(k, 0, NumberContours, 240, 0);
            Color color = Tools.HsvToRgb((float)hue, 1f, 1f);
            Brush brush = new SolidColorBrush(color);
            Pen pen = new Pen(brush, 2);
            dc.DrawLine(pen, p0, p1);
        }

        private void DrawHeatBar(DrawingContext dc)
        {
            for (int i = 0; i < NumberContours; i++)
            {
                var hue = Tools.Map(i, 0, NumberContours, 0, 240);

                Color color = Tools.HsvToRgb((float)hue, 1f, 1f);
                Brush brush = new SolidColorBrush(color);
                var x = 30;
                var y = 30 + i * 5;

                Rect rect = new Rect()
                {
                    X = x,
                    Y = y,
                    Width = 10,
                    Height = 10
                };
                dc.DrawRectangle(brush, null, rect);

            }
        }
    }
}
