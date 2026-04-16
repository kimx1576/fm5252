using System;

namespace RandomNormalProject
{
    class Program
    {
        static Random rng = new Random();

        static void Main(string[] args)
        {
            bool keepRunning = true;

            Console.WriteLine("FM5252 Project 4");
            Console.WriteLine();

            while (keepRunning)
            {
                Console.WriteLine("1 - Sum Twelve");
                Console.WriteLine("2 - Box Muller");
                Console.WriteLine("3 - Polar Rejection");
                Console.WriteLine("4 - Correlated pairs");
                Console.WriteLine("5 - Quit");
                Console.Write("> ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    GenerateValues("sum");
                }
                else if (choice == "2")
                {
                    GenerateValues("box");
                }
                else if (choice == "3")
                {
                    GenerateValues("polar");
                }
                else if (choice == "4")
                {
                    GenerateCorrelated();
                }
                else if (choice == "5")
                {
                    keepRunning = false;
                }
                else
                {
                    Console.WriteLine("Try again.");
                }

                Console.WriteLine();
            }
        }

        static void GenerateValues(string method)
        {
            Console.Write("How many? ");
            int n;
            if (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.WriteLine("Enter a positive number.");
                return;
            }

            for (int i = 0; i < n; i++)
            {
                double z = 0.0;

                if (method == "sum")
                    z = SumTwelve();
                else if (method == "box")
                    z = BoxMuller();
                else if (method == "polar")
                    z = PolarRejection();

                Console.WriteLine(z);
            }
        }

        static void GenerateCorrelated()
        {
            Console.Write("Enter rho (-1 to 1): ");
            double rho;
            if (!double.TryParse(Console.ReadLine(), out rho))
            {
                Console.WriteLine("Invalid.");
                return;
            }
            if (rho < -1.0 || rho > 1.0)
            {
                Console.WriteLine("Must be between -1 and 1.");
                return;
            }

            Console.Write("How many pairs? ");
            int n;
            if (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.WriteLine("Enter a positive number.");
                return;
            }

            Console.WriteLine("Method for normals? 1=Sum12  2=BoxMuller  3=Polar");
            Console.Write("> ");
            string m = Console.ReadLine();

            for (int i = 0; i < n; i++)
            {
                double e1 = 0.0;
                double e2 = 0.0;

                if (m == "1")
                {
                    e1 = SumTwelve();
                    e2 = SumTwelve();
                }
                else if (m == "2")
                {
                    e1 = BoxMuller();
                    e2 = BoxMuller();
                }
                else if (m == "3")
                {
                    e1 = PolarRejection();
                    e2 = PolarRejection();
                }
                else
                {
                    Console.WriteLine("Bad choice.");
                    return;
                }

                double z1 = e1;
                double z2 = rho * e1 + Math.Sqrt(1.0 - rho * rho) * e2;

                Console.WriteLine("(" + z1 + ", " + z2 + ")");
            }
        }

        static double SumTwelve()
        {
            double sum = 0.0;
            for (int i = 0; i < 12; i++)
            {
                sum += rng.NextDouble();
            }
            return sum - 6.0;
        }

        static double BoxMuller()
        {
            double u1 = rng.NextDouble();
            double u2 = rng.NextDouble();

            if (u1 == 0.0)
                u1 = 0.0000001;

            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return z;
        }

        static double PolarRejection()
        {
            double x1, x2, w;

            do
            {
                x1 = 2.0 * rng.NextDouble() - 1.0;
                x2 = 2.0 * rng.NextDouble() - 1.0;
                w = x1 * x1 + x2 * x2;
            }
            while (w >= 1.0 || w == 0.0);

            double c = Math.Sqrt((-2.0 * Math.Log(w)) / w);
            return x1 * c;
        }
    }
}