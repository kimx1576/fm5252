using System;

namespace MonteCarloOptionPricer
{
    public class OptionParameters
    {
        public double SpotPrice;
        public double StrikePrice;
        public double RiskFreeRate;
        public double Volatility;
        public double TimeToExpiry;
        public int NumSteps;
        public int NumSimulations;
    }
}