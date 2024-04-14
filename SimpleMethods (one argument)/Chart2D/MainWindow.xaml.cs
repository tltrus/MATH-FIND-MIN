using System;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _Chart2D
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timerGradient, timerMain;
        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        int factor = 50;

        Func<double, double> Func;

        BisectionMethod BisectionMethod;
        Golden_SectionMethod GoldenSectionMethod;


        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            width = g.Width;
            height = g.Height;

            visual = new DrawingVisual();

            axis = new Axis(width, height);

            InitPoints();

            Func = Quadr;

            timerMain = new System.Windows.Threading.DispatcherTimer();
            timerMain.Tick += new EventHandler(timerMainTick);
            timerMain.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timerMain.Start();

            timerGradient = new System.Windows.Threading.DispatcherTimer();
            timerGradient.Tick += new EventHandler(timerGradientTick);
            timerGradient.Interval = new TimeSpan(0, 0, 0, 0, 700);
        }

        private void InitPoints()
        {
            BisectionMethod = new BisectionMethod();
            GoldenSectionMethod = new Golden_SectionMethod();

            BisectionMethod.SetPoint(2, Func(2));
            BisectionMethod.SetSection(-6, 4);

            GoldenSectionMethod.SetPoint(1.5, Func(1.5));
            GoldenSectionMethod.SetSection(-4, 4);

        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                factor -= 1;
            }
            else
            {
                // increasing
                factor += 1;
            }

            if (factor <= 0) factor = 1;
            if (factor >= 58) factor = 58;
        }
        private void FuncDraw(DrawingContext dc, double start, double end, double step, Func<double, double> func, Brush color)
        {
            double y = 0;
            PointCollection points = new PointCollection();

            for (double i = start; i < end; i += step)
            {
                double x = i;
                y = func(i);
                points.Add(new Point(axis.Xto(x), axis.Yto(y)));
            }

            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                Point startpoint = points[0];
                points.RemoveAt(0);
                geometryContext.BeginFigure(startpoint, false, false);
                geometryContext.PolyLineTo(points, true, true);
            }
            dc.DrawGeometry(null, new Pen(color, 1), streamGeometry);
        }

        private double Quadr(double v) => v * v - 2;
        private double Sin(double v) => Math.Sin(v);
        private double SuperFunc(double x) => (x * x * (2 + Math.Cos(8 * x)));

        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                // axis drawing
                axis.Draw(dc, visual);

                axis.SetFactor(factor);

                // func
                FuncDraw(dc, -4, 4, 0.1, Func, Brushes.Yellow);

                // Bisection Method
                var x = axis.Xto(BisectionMethod.Point.X);
                var y = axis.Yto(BisectionMethod.Point.Y);
                dc.DrawEllipse(Brushes.Red, null, new Point(x, y), 4, 4);

                // Golden-Section Method
                x = axis.Xto(GoldenSectionMethod.Point.X);
                y = axis.Yto(GoldenSectionMethod.Point.Y);
                dc.DrawEllipse(Brushes.Blue, null, new Point(x, y), 4, 4);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) => Reset();
        private void Reset()
        {
            if (timerGradient != null) timerGradient.Stop();
            InitPoints();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            timerGradient.Start();
        }

        private void rdQuadr_Checked(object sender, RoutedEventArgs e)
        {
            Func = Quadr;
            InitPoints();
        }

        private void rdSin_Checked(object sender, RoutedEventArgs e)
        {
            Func = Sin;
            InitPoints();
        }

        private void timerMainTick(object sender, EventArgs e) => Drawing();

        private void timerGradientTick(object sender, EventArgs e)
        {
            BisectionMethod.Calculation(Func);
            GoldenSectionMethod.Calculation(Func);
        }
    }
}