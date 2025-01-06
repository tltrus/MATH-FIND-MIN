using System.Windows;
using System.Windows.Media;


namespace _Chart2D
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timerMain;
        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        FunctionXY Func3D;
        PSO PSO;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = g.Width;
            height = g.Height;
            axis = new Axis(width, height);

            Init();
        }

        void Init()
        {
            Func<double, double, double> func = (x, y) => 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            Func3D = new FunctionXY(width, height);
            Func3D.SetFunc(func);

            rtbConsole.Clear();
            rtbConsole.AppendText("\rBegin Particle Swarm Optimization demonstration\n");
            rtbConsole.AppendText("\rObjective function to minimize has dimension = 2");
            rtbConsole.AppendText("\rObjective function is f(x) = 3 * (1 - x)^2 * Exp(-x * x - (y + 1)^2) - 10 * (0.2 * x - x^3) - y^5 * Exp(-x * x - y * y) - 1 / 3 * Exp(-(x + 1)^2 - y * y)");

            PSO = new PSO((x) => 3 * Math.Pow((1 - x[0]), 2) * Math.Exp(-x[0] * x[0] - (x[1] + 1) * (x[1] + 1)) - 10 * (0.2 * x[0] - Math.Pow(x[0], 3) - Math.Pow(x[1], 5)) * Math.Exp(-x[0] * x[0] - x[1] * x[1]) - 1 / 3 * Math.Exp(-(x[0] + 1) * (x[0] + 1) - x[1] * x[1]));
            PSO.Init(Func3D.Xmin, Func3D.Xmax);

            rtbConsole.AppendText("\r\rRange for all x values is " + PSO.minX + " <= x <= " + PSO.maxX);
            rtbConsole.AppendText("\rNumber particles in swarm = " + PSO.numberParticles);

            rtbConsole.AppendText("\r\rInitializing swarm with random positions/solutions");


            Control();
            Drawing();

            timerMain = new System.Windows.Threading.DispatcherTimer();
            timerMain.Tick += new EventHandler(timerMainTick);
            timerMain.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var contour_num = int.Parse(tbCnum.Text);
            Func3D.SetNumberContours(contour_num);

            Drawing();
        }
        private void cbDrawFunc_Click(object sender, RoutedEventArgs e) => Drawing();
        private void cbDrawContour_Click(object sender, RoutedEventArgs e) => Drawing();
        private void Control()
        {
            var contour_num = int.Parse(tbCnum.Text);
            Func3D.SetNumberContours(contour_num);

            Func3D.Calculation();
            PSO.Clculation();
        }
        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                if (cbDrawFunc.IsChecked == true) Func3D.DrawFunc(dc);
                if (cbDrawContour.IsChecked == true) Func3D.DrawContour(dc);

                axis.Draw(dc, visual);

                PSO.Drawing(dc, width, height, Func3D.Xmin, Func3D.Xmax, Func3D.Ymin, Func3D.Ymax);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!timerMain.IsEnabled)
            {
                Init();
                timerMain.Start();
                btnStart.Content = "Stop timer";
                rtbConsole.AppendText("\r\rIn process ..........");
            } else
            {
                timerMain.Stop();
                btnStart.Content = "Start timer";
                rtbConsole.AppendText("\r\rProcess stopped");
            }
           
        }

        private void timerMainTick(object sender, EventArgs e)
        {
            Control();
            Drawing();
        }
    }
}