using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace _Chart2D
{
    /*
     * Based on article "Artificial Intelligence - Particle Swarm Optimization" of microsoft MSDN Magazine
     * https://learn.microsoft.com/en-us/archive/msdn-magazine/2011/august/artificial-intelligence-particle-swarm-optimization
     */
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timerMain;
        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        Quadr3D Quadr3D;
        int factor = 30;
        PSO PSO;

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

            Func<double[], double> sampleFunc = (x) => 3.0 + (x[0] * x[0]) + (x[1] * x[1]);

            PSO = new PSO(sampleFunc);
            PSO.Init(-10, 10);
            Quadr3D = new Quadr3D((x, y) => 3.0 + (x * x) + (y * y));
            Quadr3D.Init(-10, 10, -10, 10, 0.1);
            Quadr3D.Calculation(axis);
            Quadr3D.CalMinMaxZ();

            timerMain = new System.Windows.Threading.DispatcherTimer();
            timerMain.Tick += new EventHandler(timerMainTick);
            timerMain.Interval = new TimeSpan(0, 0, 0, 0, 50);
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


        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                Quadr3D.Calculation(axis);
                Quadr3D.Draw(dc);

                // axis drawing
                axis.Draw(dc, visual);

                axis.SetFactor(factor);

                PSO.Clculation();
                PSO.Drawing(dc, axis);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) => Reset();
        private void Reset()
        {
            timerMain.Stop();
            Init();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e) => timerMain.Start();
        private void timerMainTick(object sender, EventArgs e) => Drawing();
    }
}