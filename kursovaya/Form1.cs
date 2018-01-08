using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kursovaya
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double step;
        double epsilon = 0.00001, w = 0.6, Kizm = 1, Kus = 5, Tus = 0.06, Kdv = 5, Tdv = 0.017, ip = 40, tp = 0.5, sigma = 40;
        double T1, T2, T3, T4, T5, T6 = 1;
        double x1, x2, x3, x4, x5, x6;
        double Q2;
        List<double> X = new List<double>();

        private void Form1_Load(object sender, EventArgs e)
        {

            CreateVector();
            RausStabilityCheck(X);

            //epsilon = Translation(epsilon);
            w = Translation(w);
            Kdv = Translation(Kdv);
            double Kdop = w / epsilon;
            double Kus_strih = (Kdop * ip) / (Kizm * Kus * Kdv);

            

            Change(0, X);
            Change(1, X);
            Change(2, X);
            Change(3, X);
            Change(4, X);
            Change(5, X);

            RausStabilityCheck(X);
            
            
            answer.Text = Convert.ToString(Q(X));
        }


        /// <summary>
        /// Checks if system is stable according to Raus
        /// </summary>
        /// <param name="X"> coefficients of characteristic polynomial</param>
        private bool RausStabilityCheck(List<double> X)
        {
            foreach (double a in X)
                System.Console.Write("{0:0.000} ", a);
            System.Console.WriteLine(" \n ");


            int size = X.Count();
            if (size <= 0)
            {
                return true;
            }
            //Number of coefficients must be even
            int columns = (int) Math.Ceiling(size / 2.0);
            if (columns * 2 > size)
            {
                X.Add(0);
                size = X.Count(); // update size
            }

            //two initial rows
            List<List<double>> RausTable = new List<List<double>>();
            RausTable.Add( new List<double>() );
            RausTable.Add( new List<double>() );
            for (int k = 0; k < columns; k++)
            {
                RausTable.ElementAt(0).Add(X.ElementAt(k * 2 + 0 )); // even elements
                RausTable.ElementAt(1).Add(X.ElementAt(k * 2 + 1 )); // odd elements
            }

            //Raus itself
            // i - row index
            // k - column index
            for (int i = 2; i < size; i++)
            {
                RausTable.Add(new List<double>());
                double C1i2 = RausTable.ElementAt(i-2).ElementAt(0); //C1,i-2; subscripts: Ccolumn,row
                double C1i1 = RausTable.ElementAt(i-1).ElementAt(0); //C1,i-1
                double Ri =  C1i2 / C1i1 ; // C1,i-2 / C1,i-1
                for (int k = 0; k < columns; k++)
                {
                    double Ck1i2 = 0 ;
                    if ( (k+1) < RausTable.ElementAt(i-2).Count() ) // elementAtOrDefault
                    {
                        Ck1i2 =  RausTable.ElementAt(i-2).ElementAt(k+1);
                    }

                    double Ck1i1 = 0 ;
                    if ( (k+1) < RausTable.ElementAt(i-1).Count() )
                    {
                        Ck1i1 =  RausTable.ElementAt(i-1).ElementAt(k+1);
                    }

                    double Cki = Ck1i2 - Ri * Ck1i1;
                    RausTable.ElementAt(i).Add(Cki);
                }
            }

            //print table
            foreach (List<double> row in RausTable)
            {
                foreach (double a in row )
                    System.Console.Write("{0,10:00000.000} ", a);
                System.Console.WriteLine();
            }

            //check if first column is positive
            foreach (List<double> row in RausTable)
            {
                if (row.ElementAt(0) <= 0)
                {
                    System.Console.WriteLine("UNSTABLE");
                    return false;
                }
            }

            System.Console.WriteLine("STABLE");
            return true;
        }


        private void CreateVector()
        {
            Random rnd = new Random();
            for (int i = 1; i <= 6; i++)
                X.Add(rnd.Next(1, 100));
        }

        private void Change(int i, List<double> X)
        {
            double step_up = X[i], step_down = X[i];
            double delta = 0.001;
            step_up = step_up + delta;
            step_down = step_down - delta;
            double Q0 = Q(X);
            X[i] = step_up;
            double Q1 = Q(X);
            if (Q1 < Q0)
            {
                CalculationPlus(Q0, Q1, X[i], delta, X, i);
            }
            else
            {
                X[i] = step_down;
                Q1 = Q(X);
                if (Q1 < Q0)
                {
                    CalculationMinus(Q0, Q1, X[i], delta, X, i);
                }
            }
        }

        private void CalculationPlus(double Q0, double Q1, double x, double delta, List<double> X, int i)
        {
            while (Q0 - Q1 > epsilon && x < 100)
            {
                x = x + delta;
                X[i] = x;
                Q0 = Q1;
                Q1 = Q(X);
            }
            step = x;
        }

        private void CalculationMinus(double Q0, double Q1, double x, double delta, List<double> X, int i)
        {
            while (Q0 - Q1 > epsilon && x > 1)
            {
                x = x - delta;
                X[i] = x;
                Q0 = Q1;
                Q1 = Q(X);
            }
            step = x;
        }

        private double Translation (double variable)
        {
            return variable * 180 / Math.PI;
        }

        double Q(List<double> X)
        {
            return X[0] - X[1] * X[2] + X[3] * X[4] / X[5];
        }

        private void answer_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
