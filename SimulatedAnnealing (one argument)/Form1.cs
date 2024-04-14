using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chart
{
    // Based on Roman Shamin solution: https://www.youtube.com/watch?v=R3XawKalzgo
    public partial class Form1 : Form
    {
        Random rnd = new Random();
        double X = 3;
        double Y = 0;

        double T = 100; // Temperature
        double alpha = 0.98;
        double dY = 0;

        double Xnew = 0, Ynew = 0;
        double p = 0.5;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            double x = 0;
            for (x = -4; x < 4; x += 0.01)
            {
                double y = Func(x);
                chart1.Series[0].Points.AddXY(x, y);
            }

            chart1.Series[1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            chart1.Series[1].Points.AddXY(X, Func(X));
            chart1.Series[1].Points[0].Color = Color.Red;
        }

        private double Func(double x) => (x * x * (2 + Math.Cos(8 * x)));

        private void Static_Annealing()
        {
            double alpha = 0.99;
            double dY = 0;

            double Xnew = 0, Ynew = 0;
            double p = 0.5;

            while (T > 0.005)
            {
                if (rnd.NextDouble() < 0.5)
                    Xnew = X + 0.1;
                else
                    Xnew = X - 0.1;

                Y = Func(X);
                Ynew = Func(Xnew);
                dY = Ynew - Y;

                if (dY <= 0) // new solution better than old one
                {
                    X = Xnew; // particle goes to new point Xnew
                }
                else // new solution worse than old one (Ynew - Y) > 0
                {
                    p = Math.Exp(-dY / T);
                    if (rnd.NextDouble() < p)
                        X = Xnew;

                }
                T = alpha * T;
            }

            chart1.Series[1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            chart1.Series[1].Points.AddXY(X, Func(X));
            chart1.Series[1].Points[1].Color = Color.Green;
        }
        private void Dynamic_Annealin()
        {
            chart1.Series[1].Points.Clear();
            if (T > 0.001)
            {
                if (rnd.NextDouble() < 0.5)
                    Xnew = X + 0.1;
                else
                    Xnew = X - 0.1;

                Y = Func(X);
                Ynew = Func(Xnew);
                dY = Ynew - Y;

                if (dY <= 0) // new solution better than old one
                {
                    X = Xnew; // particle goes to new point Xnew
                }
                else // new solution worse than old one (Ynew - Y) > 0
                {
                    p = Math.Exp(-dY / T);
                    if (rnd.NextDouble() < p)
                        X = Xnew;

                }
                T = alpha * T;
            }

            chart1.Series[1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            chart1.Series[1].Points.AddXY(X, Func(X));
            chart1.Series[1].Points[0].Color = Color.Red;

            lbTemp.Text = T.ToString("#.###");
        }

        private void timer1_Tick(object sender, EventArgs e) => Dynamic_Annealin();

        private void btnStaticStart_Click(object sender, EventArgs e) => Static_Annealing();

        private void btnReset_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            chart1.Series[1].Points.Clear();
            X = 3;
            Y = 0;
            T = 100;
            lbTemp.Text = "0";

            chart1.Series[1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            chart1.Series[1].Points.AddXY(X, Func(X));
        }

        private void btnDynamicStart_Click(object sender, EventArgs e) => timer1.Start();
    }
}
