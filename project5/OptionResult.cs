using System;

namespace MonteCarloOptionPricer
{
    public class OptionResult
    {
        public double CallPrice;
        public double PutPrice;

        public double CallDelta;
        public double PutDelta;
        public double Gamma;
        public double Vega;
        public double CallTheta;
        public double PutTheta;
        public double CallRho;
        public double PutRho;

        public double CallSE;
        public double PutSE;
    }
}