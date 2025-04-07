using System.Globalization;
using System.Windows;
using System.Windows.Media;
using static _Chart2D.FireflyOptimization;


namespace _Chart2D
{
    // Test Run - Optimization using the firefly algorithm
    // https://learn.microsoft.com/ru-ru/archive/msdn-magazine/2015/june/test-run-firefly-algorithm-optimization
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        FireflyOptimization FireflyOptimization { get; set; }
        double bestX, bestY;

        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        FunctionXY Func3D;
        bool showBestPosition;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = g.Width;
            height = g.Height;
            axis = new Axis(width, height);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerMainTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);

            Init();
        }

        void Init()
        {
            showBestPosition = false;
            rtbConsole.Clear();

            rtbConsole.AppendText("\rBegin firefly algorithm optimization demo.");
            rtbConsole.AppendText("\r\rGoal is to solve the Michalewicz benchmark function.");
            rtbConsole.AppendText("\rx = 2.2029 1.5707.");

            FireflyOptimization = new FireflyOptimization();
            FireflyOptimization.TimerNotify += () => { timer.Stop(); };
            FireflyOptimization.BestPositionNotify += (array) => {
                if (array.Length != 2) return;

                bestX = array[0];
                bestY = array[1];

                showBestPosition = true;

                rtbConsole.AppendText("\r\rBest solution found: " + bestX.ToString("F4") + " " + bestY.ToString("F4"));
                rtbConsole.AppendText("\rValue of function at best position = " + FireflyOptimization.z.ToString("F4"));
                rtbConsole.AppendText("\rError at best position = " + FireflyOptimization.error.ToString("F4"));
                rtbConsole.AppendText("\r\r******* END *******");
            };

            rtbConsole.AppendText("\r\rSetting numFireflies        = " + FireflyOptimization.numFireflies);
            rtbConsole.AppendText("\rSetting problem dim         = " + FireflyOptimization.dim);
            rtbConsole.AppendText("\rSetting maxEpochs           = " + FireflyOptimization.maxEpochs);
            rtbConsole.AppendText("\rSetting initialization seed = " + FireflyOptimization.seed);


            //Func<double, double, double> func1 = (x, y) => 5.0 * (x * x) + (y * y);
            //Func<double, double, double> func2 = (x, y) => 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            //Func<double, double, double> func3 = (x, y) => ((x * x) - (10 * Math.Cos(2 * Math.PI * x)) + 10)+ ((y * y) - (10 * Math.Cos(2 * Math.PI * y)) + 10);
            //Func<double, double, double> func4 = (x, y) => (x - 1) * (x - 1) + (y - 1) * (y - 1) - (x * y);
            //Func<double, double, double> func5 = (x, y) => (-x * Math.Sin(Math.Sqrt(Math.Abs(x)))) + (-y * Math.Sin(Math.Sqrt(Math.Abs(y))));
            
            // Michalewicz benchmark function
            Func<double, double, double> func6 = (x, y) => -1 * ((Math.Sin(x) * Math.Pow(Math.Sin((1 * x * x) / Math.PI), 20) + (Math.Sin(y) * Math.Pow(Math.Sin((2 * y * y) / Math.PI), 20))));

            Func3D = new FunctionXY(width, height, 20, -4, 4, -4, 4);
            Func3D.SetFunc(func6);

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

                // Draw points
                for (int i = 0; i < FireflyOptimization.swarm.Length; ++i)
                {
                    var X = FireflyOptimization.swarm[i].position[0]; // X
                    var Y = FireflyOptimization.swarm[i].position[1]; // Y

                    var point = new Point(X, Y);
                    var normalize = Tools.Normalize(point, width, height, -4, 4, -4, 4);

                    dc.DrawEllipse(Brushes.Red, null, normalize, 4, 4);
                }

                // Best solution
                if (showBestPosition)
                {
                    var p = new Point(bestX, bestY);
                    var norm = Tools.Normalize(p, width, height, -4, 4, -4, 4);
                    dc.DrawEllipse(Brushes.WhiteSmoke, null, norm, 5, 5);
                }

                axis.Draw(dc, visual);

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

            FireflyOptimization.Calculation();
            lbEpoch.Content = FireflyOptimization.epoch;

            Drawing();
        }
    }
}