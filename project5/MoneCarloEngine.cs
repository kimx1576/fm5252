using System;

namespace MonteCarloOptionPricer
{
    public class MonteCarloEngine
    {
        private Random rng = new Random();

        // runs the full simulation and computes greeks
        public OptionResult Run(OptionParameters p)
        {
            OptionResult result = new OptionResult();

            // first get the base prices
            double callSum = 0;
            double putSum = 0;
            double callSqSum = 0;
            double putSqSum = 0;
            SimulatePaths(p.SpotPrice, p.StrikePrice, p.RiskFreeRate, p.Volatility,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref callSum, ref putSum, ref callSqSum, ref putSqSum);

            result.CallPrice = callSum / p.NumSimulations;
            result.PutPrice = putSum / p.NumSimulations;

            // standard error
            double callVar = (callSqSum / p.NumSimulations) - (result.CallPrice * result.CallPrice);
            double putVar = (putSqSum / p.NumSimulations) - (result.PutPrice * result.PutPrice);
            result.CallSE = Math.Sqrt(callVar / p.NumSimulations);
            result.PutSE = Math.Sqrt(putVar / p.NumSimulations);

            // ----- Greeks -----
            // Delta: bump spot price up and down by 1%
            double dS = p.SpotPrice * 0.01;

            double callUpSum = 0, putUpSum = 0, dummy1 = 0, dummy2 = 0;
            SimulatePaths(p.SpotPrice + dS, p.StrikePrice, p.RiskFreeRate, p.Volatility,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref callUpSum, ref putUpSum, ref dummy1, ref dummy2);
            double callUp = callUpSum / p.NumSimulations;
            double putUp = putUpSum / p.NumSimulations;

            double callDnSum = 0, putDnSum = 0;
            dummy1 = 0; dummy2 = 0;
            SimulatePaths(p.SpotPrice - dS, p.StrikePrice, p.RiskFreeRate, p.Volatility,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref callDnSum, ref putDnSum, ref dummy1, ref dummy2);
            double callDn = callDnSum / p.NumSimulations;
            double putDn = putDnSum / p.NumSimulations;

            result.CallDelta = (callUp - callDn) / (2.0 * dS);
            result.PutDelta = (putUp - putDn) / (2.0 * dS);

            // Gamma
            result.Gamma = (callUp - 2.0 * result.CallPrice + callDn) / (dS * dS);

            // Vega: bump volatility
            double dVol = 0.01;

            double cVolUp = 0, pVolUp = 0;
            dummy1 = 0; dummy2 = 0;
            SimulatePaths(p.SpotPrice, p.StrikePrice, p.RiskFreeRate, p.Volatility + dVol,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref cVolUp, ref pVolUp, ref dummy1, ref dummy2);

            double cVolDn = 0, pVolDn = 0;
            dummy1 = 0; dummy2 = 0;
            SimulatePaths(p.SpotPrice, p.StrikePrice, p.RiskFreeRate, p.Volatility - dVol,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref cVolDn, ref pVolDn, ref dummy1, ref dummy2);

            result.Vega = (cVolUp / p.NumSimulations - cVolDn / p.NumSimulations) / (2.0 * dVol);

            // Theta: bump time down by one day
            double dT = 1.0 / 365.0;
            double timeDown = p.TimeToExpiry - dT;
            if (timeDown < 0) timeDown = 0.0001;

            double cTimeDn = 0, pTimeDn = 0;
            dummy1 = 0; dummy2 = 0;
            SimulatePaths(p.SpotPrice, p.StrikePrice, p.RiskFreeRate, p.Volatility,
                timeDown, p.NumSteps, p.NumSimulations,
                ref cTimeDn, ref pTimeDn, ref dummy1, ref dummy2);

            result.CallTheta = (cTimeDn / p.NumSimulations - result.CallPrice) / dT;
            result.PutTheta = (pTimeDn / p.NumSimulations - result.PutPrice) / dT;

            // Rho: bump interest rate
            double dR = 0.001;

            double cRateUp = 0, pRateUp = 0;
            dummy1 = 0; dummy2 = 0;
            SimulatePaths(p.SpotPrice, p.StrikePrice, p.RiskFreeRate + dR, p.Volatility,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref cRateUp, ref pRateUp, ref dummy1, ref dummy2);

            double cRateDn = 0, pRateDn = 0;
            dummy1 = 0; dummy2 = 0;
            SimulatePaths(p.SpotPrice, p.StrikePrice, p.RiskFreeRate - dR, p.Volatility,
                p.TimeToExpiry, p.NumSteps, p.NumSimulations,
                ref cRateDn, ref pRateDn, ref dummy1, ref dummy2);

            result.CallRho = (cRateUp / p.NumSimulations - cRateDn / p.NumSimulations) / (2.0 * dR);
            result.PutRho = (pRateUp / p.NumSimulations - pRateDn / p.NumSimulations) / (2.0 * dR);

            return result;
        }


        // simulates all paths and accumulates payoff sums
        private void SimulatePaths(double S, double K, double r, double sigma, double T,
            int steps, int sims,
            ref double callSum, ref double putSum,
            ref double callSqSum, ref double putSqSum)
        {
            double dt = T / steps;
            double disc = Math.Exp(-r * T);

            for (int i = 0; i < sims; ++i)
            {
                double price = S;

                // GBM step by step
                for (int j = 0; j < steps; ++j)
                {
                    double z = BoxMuller();
                    price = price * Math.Exp((r - 0.5 * sigma * sigma) * dt
                        + sigma * Math.Sqrt(dt) * z);
                }

                double cPayoff = disc * Math.Max(price - K, 0);
                double pPayoff = disc * Math.Max(K - price, 0);

                callSum += cPayoff;
                putSum += pPayoff;
                callSqSum += cPayoff * cPayoff;
                putSqSum += pPayoff * pPayoff;
            }
        }


        // box-muller transform to get a standard normal random
        private double BoxMuller()
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = rng.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }
    }
}