using System;
using System.Collections.Generic;

namespace MonteCarloOptions
{
    public enum SimulationType
    {
        Random,
        VanDerCorput
    }

    public class OptionInput
    {
        public double S;
        public double K;
        public double T;
        public double R;
        public double Sigma;
        public bool IsCall;
    }

    public class SimResult
    {
        public double Price;
        public double StandardError;
        public double Delta;
        public double Gamma;
        public double Vega;
        public double Rho;
        public double Theta;
    }

    public class PathValue
    {
        public double Price;
        public double Up;
        public double Down;
        public double VolUp;
        public double VolDown;
        public double RateUp;
        public double RateDown;
        public double TimeDown;
    }

    public class MonteCarlo
    {
        private Random rand = new Random();

        public SimResult Run(OptionInput option, int numberOfSims, bool useAntithetic, SimulationType type, int vdcBase)
        {
            int n = numberOfSims;

            // Cut simulations in half if we are doing antithetic pairs
            if (useAntithetic && type == SimulationType.Random)
            {
                n = numberOfSims / 2;
            }

            // Bump amounts for Greeks
            double dS = option.S * 0.01;
            double dSigma = 0.01;
            double dR = 0.0001;
            double dT = 1.0 / 365.0;

            if (option.T - dT <= 0)
            {
                dT = option.T / 2.0;
            }

            List<double> priceList = new List<double>();

            double priceSum = 0.0;
            double upSum = 0.0;
            double downSum = 0.0;
            double volUpSum = 0.0;
            double volDownSum = 0.0;
            double rateUpSum = 0.0;
            double rateDownSum = 0.0;
            double timeDownSum = 0.0;

            for (int i = 0; i < n; i++)
            {
                double z;

                // Figure out our random variable depending on the type
                if (type == SimulationType.VanDerCorput)
                {
                    double u = VanDerCorput(i + 1, vdcBase);

                    // Prevent crashing at the edges
                    if (u <= 0.0) u = 0.000001;
                    if (u >= 1.0) u = 0.999999;

                    z = InverseNormal(u);
                }
                else
                {
                    double u = rand.NextDouble();
                    if (u <= 0.0) u = 0.000001;
                    
                    z = InverseNormal(u);
                }

                // Get base values
                PathValue first = GetPathValues(option, z, dS, dSigma, dR, dT);

                double price = first.Price;
                double up = first.Up;
                double down = first.Down;
                double volUp = first.VolUp;
                double volDown = first.VolDown;
                double rateUp = first.RateUp;
                double rateDown = first.RateDown;
                double timeDown = first.TimeDown;

                // Handle antithetic pairs if turned on
                if (useAntithetic && type == SimulationType.Random)
                {
                    PathValue second = GetPathValues(option, -z, dS, dSigma, dR, dT);

                    // Average the normal path and the negative path
                    price = (first.Price + second.Price) / 2.0;
                    up = (first.Up + second.Up) / 2.0;
                    down = (first.Down + second.Down) / 2.0;
                    volUp = (first.VolUp + second.VolUp) / 2.0;
                    volDown = (first.VolDown + second.VolDown) / 2.0;
                    rateUp = (first.RateUp + second.RateUp) / 2.0;
                    rateDown = (first.RateDown + second.RateDown) / 2.0;
                    timeDown = (first.TimeDown + second.TimeDown) / 2.0;
                }

                priceList.Add(price);

                priceSum += price;
                upSum += up;
                downSum += down;
                volUpSum += volUp;
                volDownSum += volDown;
                rateUpSum += rateUp;
                rateDownSum += rateDown;
                timeDownSum += timeDown;
            }

            // Calculate averages
            double priceAvg = priceSum / n;
            double upAvg = upSum / n;
            double downAvg = downSum / n;
            double volUpAvg = volUpSum / n;
            double volDownAvg = volDownSum / n;
            double rateUpAvg = rateUpSum / n;
            double rateDownAvg = rateDownSum / n;
            double timeDownAvg = timeDownSum / n;

            SimResult result = new SimResult();
            result.Price = priceAvg;

            // Only do standard error if it's not Van der Corput
            if (type == SimulationType.VanDerCorput)
            {
                result.StandardError = 0.0;
            }
            else
            {
                result.StandardError = StandardError(priceList);
            }

            // Calculate Greeks
            result.Delta = (upAvg - downAvg) / (2.0 * dS);
            result.Gamma = (upAvg - 2.0 * priceAvg + downAvg) / (dS * dS);
            result.Vega = (volUpAvg - volDownAvg) / (2.0 * dSigma);
            result.Rho = (rateUpAvg - rateDownAvg) / (2.0 * dR);
            result.Theta = (timeDownAvg - priceAvg) / dT;

            return result;
        }

        private PathValue GetPathValues(OptionInput option, double z, double dS, double dSigma, double dR, double dT)
        {
            PathValue value = new PathValue();

            value.Price = OptionValue(option.S, option.K, option.R, option.Sigma, option.T, option.IsCall, z);

            value.Up = OptionValue(option.S + dS, option.K, option.R, option.Sigma, option.T, option.IsCall, z);
            value.Down = OptionValue(option.S - dS, option.K, option.R, option.Sigma, option.T, option.IsCall, z);

            value.VolUp = OptionValue(option.S, option.K, option.R, option.Sigma + dSigma, option.T, option.IsCall, z);
            value.VolDown = OptionValue(option.S, option.K, option.R, option.Sigma - dSigma, option.T, option.IsCall, z);

            value.RateUp = OptionValue(option.S, option.K, option.R + dR, option.Sigma, option.T, option.IsCall, z);
            value.RateDown = OptionValue(option.S, option.K, option.R - dR, option.Sigma, option.T, option.IsCall, z);

            value.TimeDown = OptionValue(option.S, option.K, option.R, option.Sigma, option.T - dT, option.IsCall, z);

            return value;
        }

        private double OptionValue(double s, double k, double r, double sigma, double t, bool isCall, double z)
        {
            double st = s * Math.Exp((r - 0.5 * sigma * sigma) * t + sigma * Math.Sqrt(t) * z);

            double payoff;
            if (isCall)
            {
                payoff = Math.Max(st - k, 0.0);
            }
            else
            {
                payoff = Math.Max(k - st, 0.0);
            }

            return Math.Exp(-r * t) * payoff;
        }

        private double StandardError(List<double> values)
        {
            double avg = 0.0;
            for (int i = 0; i < values.Count; i++)
            {
                avg += values[i];
            }
            avg = avg / values.Count;

            double sum = 0.0;
            for (int i = 0; i < values.Count; i++)
            {
                double diff = values[i] - avg;
                sum += diff * diff;
            }

            double variance = sum / (values.Count - 1);
            return Math.Sqrt(variance) / Math.Sqrt(values.Count);
        }

        private double VanDerCorput(int index, int baseValue)
        {
            double result = 0.0;
            double fraction = 1.0 / baseValue;

            while (index > 0)
            {
                int remainder = index % baseValue;
                result += remainder * fraction;

                index = index / baseValue;
                fraction = fraction / baseValue;
            }

            return result;
        }

        private double InverseNormal(double p)
        {
            double c0 = 2.515517;
            double c1 = 0.802853;
            double c2 = 0.010328;
            double d1 = 1.432788;
            double d2 = 0.189269;
            double d3 = 0.001308;

            double t;
            if (p < 0.5)
            {
                t = Math.Sqrt(-2.0 * Math.Log(p));
            }
            else
            {
                t = Math.Sqrt(-2.0 * Math.Log(1.0 - p));
            }

            double top = c0 + c1 * t + c2 * t * t;
            double bottom = 1.0 + d1 * t + d2 * t * t + d3 * t * t * t;

            double z = t - top / bottom;

            if (p < 0.5)
            {
                return -z;
            }
            else
            {
                return z;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            MonteCarlo mc = new MonteCarlo();
            bool running = true;

            while (running)
            {
                Console.WriteLine("Project 6");
                Console.WriteLine("1 - Random Monte Carlo");
                Console.WriteLine("2 - Van der Corput");
                Console.WriteLine("3 - Quit");
                Console.Write("> ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    OptionInput option = ReadOption();
                    int sims = ReadInt("Number of simulations: ");

                    Console.Write("Use antithetic? y/n: ");
                    string antiChoice = Console.ReadLine().ToLower();

                    bool useAntithetic = (antiChoice == "y");

                    SimResult result = mc.Run(option, sims, useAntithetic, SimulationType.Random, 2);
                    PrintResult(result, true);
                }
                else if (choice == "2")
                {
                    OptionInput option = ReadOption();
                    int baseValue = ReadInt("Enter Van der Corput base: ");

                    if (baseValue < 2)
                    {
                        Console.WriteLine("Base has to be at least 2.");
                    }
                    else
                    {
                        int sims = 10000;
                        SimResult result = mc.Run(option, sims, false, SimulationType.VanDerCorput, baseValue);
                        PrintResult(result, false);
                    }
                }
                else if (choice == "3")
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("Please choose 1, 2, or 3.");
                }

                Console.WriteLine();
            }
        }

        static OptionInput ReadOption()
        {
            OptionInput option = new OptionInput();

            option.S = ReadDouble("Stock price: ");
            option.K = ReadDouble("Strike: ");
            option.T = ReadDouble("Time to maturity: ");
            option.R = ReadDouble("Risk-free rate: ");
            option.Sigma = ReadDouble("Volatility: ");

            Console.Write("Call or put? c/p: ");
            string cp = Console.ReadLine().ToLower();

            option.IsCall = (cp == "c");

            return option;
        }

        static void PrintResult(SimResult result, bool showStandardError)
        {
            Console.WriteLine("\nResults:");
            Console.WriteLine("Price: " + result.Price.ToString("F4"));

            if (showStandardError)
            {
                Console.WriteLine("Standard Error: " + result.StandardError.ToString("F6"));
            }
            else
            {
                Console.WriteLine("Standard Error: not used for Van der Corput");
            }

            Console.WriteLine("Delta: " + result.Delta.ToString("F4"));
            Console.WriteLine("Gamma: " + result.Gamma.ToString("F4"));
            Console.WriteLine("Vega: " + result.Vega.ToString("F4"));
            Console.WriteLine("Rho: " + result.Rho.ToString("F4"));
            Console.WriteLine("Theta: " + result.Theta.ToString("F4"));
        }

        static int ReadInt(string message)
        {
            Console.Write(message);
            int value;
            int.TryParse(Console.ReadLine(), out value);
            return value;
        }

        static double ReadDouble(string message)
        {
            Console.Write(message);
            double value;
            double.TryParse(Console.ReadLine(), out value);
            return value;
        }
    }
}