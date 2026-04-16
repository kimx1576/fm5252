using System;

namespace MonteCarloOptionPricer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Monte Carlo European Option Pricer ===");
            Console.WriteLine();

            Console.Write("Spot price: ");
            double spot = double.Parse(Console.ReadLine());

            Console.Write("Strike price: ");
            double strike = double.Parse(Console.ReadLine());

            Console.Write("Risk-free rate (e.g. 0.05): ");
            double rate = double.Parse(Console.ReadLine());

            Console.Write("Volatility (e.g. 0.20): ");
            double vol = double.Parse(Console.ReadLine());

            Console.Write("Time to expiry (years): ");
            double expiry = double.Parse(Console.ReadLine());

            Console.Write("Number of time steps: ");
            int steps = int.Parse(Console.ReadLine());

            Console.Write("Number of simulations: ");
            int sims = int.Parse(Console.ReadLine());

            OptionParameters p = new OptionParameters();
            p.SpotPrice = spot;
            p.StrikePrice = strike;
            p.RiskFreeRate = rate;
            p.Volatility = vol;
            p.TimeToExpiry = expiry;
            p.NumSteps = steps;
            p.NumSimulations = sims;

            Console.WriteLine();
            Console.WriteLine("Running...");

            MonteCarloEngine engine = new MonteCarloEngine();
            OptionResult res = engine.Run(p);

            Console.WriteLine();
            Console.WriteLine("--- Prices ---");
            Console.WriteLine("Call: " + res.CallPrice.ToString("F4") + "  (SE: " + res.CallSE.ToString("F6") + ")");
            Console.WriteLine("Put:  " + res.PutPrice.ToString("F4") + "  (SE: " + res.PutSE.ToString("F6") + ")");
            Console.WriteLine();
            Console.WriteLine("--- Greeks ---");
            Console.WriteLine("Call Delta: " + res.CallDelta.ToString("F6"));
            Console.WriteLine("Put Delta:  " + res.PutDelta.ToString("F6"));
            Console.WriteLine("Gamma:      " + res.Gamma.ToString("F6"));
            Console.WriteLine("Vega:       " + res.Vega.ToString("F6"));
            Console.WriteLine("Call Theta: " + res.CallTheta.ToString("F6"));
            Console.WriteLine("Put Theta:  " + res.PutTheta.ToString("F6"));
            Console.WriteLine("Call Rho:   " + res.CallRho.ToString("F6"));
            Console.WriteLine("Put Rho:    " + res.PutRho.ToString("F6"));
        }
    }
}