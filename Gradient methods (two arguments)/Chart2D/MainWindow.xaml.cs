using System.Globalization;
using System.Windows;
using System.Windows.Media;
using _Chart2D.Classes;


namespace _Chart2D
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        FunctionXY Func3D;
        MethodCoordinateDescent CoordinateDescent;
        MethodGradientDescentСonstantStep GradDescСonstantStep;
        MethodGradientDescentSplittingStep GradDescSplittingStep;
        MethodGradientDescentSteepestStep GradDescSteepestStep;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = g.Width;
            height = g.Height;
            axis = new Axis(width, height);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerMainTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            Init();
        }

        void Init()
        {
            rtbConsole.Document.Blocks.Clear();

            Func<double, double, double> func1 = (x, y) => 5.0 * (x * x) + (y * y);
            Func<double, double, double> func2 = (x, y) => 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            Func<double, double, double> func3 = (x, y) => ((x * x) - (10 * Math.Cos(2 * Math.PI * x)) + 10)+ ((y * y) - (10 * Math.Cos(2 * Math.PI * y)) + 10);
            Func<double, double, double> func4 = (x, y) => (x - 1) * (x - 1) + (y - 1) * (y - 1) - (x * y);

            Func3D = new FunctionXY(width, height, 10, -10, 10, -10, 10);
            Func3D.SetFunc(func1);

            CoordinateDescent = new MethodCoordinateDescent(9, -9, Brushes.WhiteSmoke);
            //CoordinateDescent.SetFunc((x) => (x[0] - 1) * (x[0] - 1) + (x[1] - 1) * (x[1] - 1) - x[0] * x[1]);
            CoordinateDescent.SetFunc((x) => 5.0 * x[0] * x[0] + x[1] * x[1]);
            CoordinateDescent.InfoNotify += (x, iter) => {
                rtbConsole.AppendText("\rМЕТОД ПОКООРДИНАТНОГО СПУСКА / COORDINATE-BY-COORDINATE DESCENT METHOD", "White");
                rtbConsole.AppendText("\rКоличество итераций / Iterations: " + iter, "White");
                rtbConsole.AppendText("\rx1 = " + x[0].ToString("F4", CultureInfo.InvariantCulture) + "\rx2 = " + x[1].ToString("F4", CultureInfo.InvariantCulture), "White");
                rtbConsole.AppendText("\r------------------------------", "White");
            };

            GradDescСonstantStep = new MethodGradientDescentСonstantStep(-9, -9, Brushes.Red);
            GradDescСonstantStep.SetFunc((x) => 5.0 * x[0] * x[0] + x[1] * x[1]);
            GradDescСonstantStep.InfoNotify += (x, iter) =>
            {
                rtbConsole.AppendText("\rМЕТОД ГРАДИЕНТНОГО СПУСКА С ПОСТОЯННЫМ ШАГОМ / CONSTANT STEP GRADIENT DESCENT METHOD", "Red");
                rtbConsole.AppendText("\rКоличество итераций / Iterations: " + iter, "Red");
                rtbConsole.AppendText("\rx1 = " + x[0].ToString("F4", CultureInfo.InvariantCulture) + "\rx2 = " + x[1].ToString("F4", CultureInfo.InvariantCulture), "Red");
                rtbConsole.AppendText("\r------------------------------", "Red");
            };

            GradDescSplittingStep = new MethodGradientDescentSplittingStep(-9, 9, Brushes.Yellow);
            GradDescSplittingStep.SetFunc((x) => 5.0 * x[0] * x[0] + x[1] * x[1]);
            GradDescSplittingStep.InfoNotify += (x, iter) =>
            {
                rtbConsole.AppendText("\rМЕТОД ГРАДИЕНТНОГО СПУСКА С ДРОБЛЕНИЕМ ШАГА / GRADIENT DESCENT METHOD WITH STEP SPLITTING", "Yellow");
                rtbConsole.AppendText("\rКоличество итераций / Iterations: " + iter, "Yellow");
                rtbConsole.AppendText("\rx1 = " + x[0].ToString("F4", CultureInfo.InvariantCulture) + "\rx2 = " + x[1].ToString("F4", CultureInfo.InvariantCulture), "Yellow");
                rtbConsole.AppendText("\r------------------------------", "Yellow");
            };

            GradDescSteepestStep = new MethodGradientDescentSteepestStep(9, 9, Brushes.Pink);
            GradDescSteepestStep.SetFunc((x) => 5.0 * x[0] * x[0] + x[1] * x[1]);
            GradDescSteepestStep.InfoNotify += (x, iter) =>
            {
                rtbConsole.AppendText("\rМЕТОД НАИСКОРЕЙШЕГО СПУСКА / THE STEEPEST DESCENT METHOD", "Pink");
                rtbConsole.AppendText("\rКоличество итераций / Iterations: " + iter, "Pink");
                rtbConsole.AppendText("\rx1 = " + x[0].ToString("F4", CultureInfo.InvariantCulture) + "\rx2 = " + x[1].ToString("F4", CultureInfo.InvariantCulture), "Pink");
                rtbConsole.AppendText("\r------------------------------", "Pink");
            };

            Func3DControl();
            Drawing();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var contour_num = int.Parse(tbCnum.Text);
            Func3D.SetNumberContours(contour_num);

            Drawing();
        }
        private void cbDrawFunc_Click(object sender, RoutedEventArgs e) => Drawing();
        private void cbDrawContour_Click(object sender, RoutedEventArgs e) => Drawing();
        private void Func3DControl()
        {
            var contour_num = int.Parse(tbCnum.Text);
            Func3D.SetNumberContours(contour_num);
            Func3D.Calculation();
        }
        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                if (cbDrawFunc.IsChecked == true) Func3D.DrawFunc(dc);
                if (cbDrawContour.IsChecked == true) Func3D.DrawContour(dc);

                axis.Draw(dc, visual);

                CoordinateDescent.Drawing(dc, width, height, Func3D.Xmin, Func3D.Xmax, Func3D.Ymin, Func3D.Ymax);
                GradDescСonstantStep.Drawing(dc, width, height, Func3D.Xmin, Func3D.Xmax, Func3D.Ymin, Func3D.Ymax);
                GradDescSplittingStep.Drawing(dc, width, height, Func3D.Xmin, Func3D.Xmax, Func3D.Ymin, Func3D.Ymax);
                GradDescSteepestStep.Drawing(dc, width, height, Func3D.Xmin, Func3D.Xmax, Func3D.Ymin, Func3D.Ymax);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {


            if (!timer.IsEnabled)
            {
                Init();
                timer.Start();
                btnStart.Content = "Stop timer";
            } else
            {
                timer.Stop();
                btnStart.Content = "Start timer";
            }
        }

        private void timerMainTick(object sender, EventArgs e)
        {
            Func3DControl();

            if (!CoordinateDescent.isFinished)
                CoordinateDescent.Calculation();

            if (!GradDescСonstantStep.isFinished)
                GradDescСonstantStep.Calculation();

            if (!GradDescSplittingStep.isFinished)
                GradDescSplittingStep.Calculation();

            if (!GradDescSteepestStep.isFinished)
                GradDescSteepestStep.Calculation();

            Drawing();

            if (CoordinateDescent.isFinished && GradDescСonstantStep.isFinished &&
                GradDescSplittingStep.isFinished && GradDescSteepestStep.isFinished)
            {
                btnStart.Content = "Start timer";
                timer.Stop();
            }
        }
    }
}