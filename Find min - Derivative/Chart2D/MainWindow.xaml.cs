using System;
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

        delegate double delegateFunc(double val);
        delegateFunc Func;
        double x_;
        Point gradpoint = new Point();


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


            timerMain = new System.Windows.Threading.DispatcherTimer();
            timerMain.Tick += new EventHandler(timerMainTick);
            timerMain.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timerMain.Start();

            timerGradient = new System.Windows.Threading.DispatcherTimer();
            timerGradient.Tick += new EventHandler(timerGradientTick);
            timerGradient.Interval = new TimeSpan(0, 0, 0, 0, 100);

            Func = Quadr;
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
        private void FuncDraw(DrawingContext dc, double start, double end, double step, delegateFunc func, Brush color)
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

        private double PartialGradient(delegateFunc func, double x)
        {
            // Gradient : [F(x+Offs) - F(x)] / h
            double Fx = func(x);

            x += 0.1;
            double Fx_plus_offs = func(x);

            double gradient = (Fx_plus_offs - Fx) / 0.1;

            return gradient;
        }


        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                // axis drawing
                axis.Draw(dc, visual);

                axis.SetFactor(factor);

                // func
                FuncDraw(dc, -5, 5, 0.1, Func, Brushes.Yellow);

                // gradient pos
                dc.DrawEllipse(null, new Pen(Brushes.Red, 2), gradpoint, 2, 2);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) => Reset();
        private void Reset()
        {
            if (timerGradient != null) timerGradient.Stop();
            gradpoint = new Point(0, 0);
            x_ = 1;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            x_ = 1;
            timerGradient.Start();
        }

        private void rdQuadr_Checked(object sender, RoutedEventArgs e)
        {
            Func = Quadr;
            Reset();
        }

        private void rdSin_Checked(object sender, RoutedEventArgs e)
        {
            Func = Sin;
            Reset();
        }

        private void timerMainTick(object sender, EventArgs e) => Drawing();

        private void timerGradientTick(object sender, EventArgs e)
        {
            double gradient = PartialGradient(Func, x_);
            x_ -= 0.008 * gradient;

            gradpoint.X = axis.Xto(x_);
            gradpoint.Y = axis.Yto(Func(x_));

            if (gradient < 0.1) timerGradient.Stop();
        }
    }
}