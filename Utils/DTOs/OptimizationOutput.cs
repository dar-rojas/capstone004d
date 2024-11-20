namespace Api.Utils.DTOs 
{
    public class OptimizationOutput
    {
        public double[] BatteryPowers { get; set; }
        public double[] BatteryEnergy { get; set; }
        public double MaxPower1 { get; set; }
        public double MaxPower2 { get; set; }
        public double MaxPowerPH { get; set; }

        // default constructor
        public OptimizationOutput()
        { 
            BatteryPowers = [];
            BatteryEnergy = [];
            MaxPower1 = 0;
            MaxPower2 = 0;
            MaxPowerPH = 0;
        }
        // constructor with parameters
        public OptimizationOutput(double[] batteryPowers, double[] batteryEnergy, double maxPower1, double maxPower2, double maxPowerPH)
        {
            BatteryPowers = batteryPowers;
            BatteryEnergy = batteryEnergy;
            MaxPower1 = maxPower1;
            MaxPower2 = maxPower2;
            MaxPowerPH = maxPowerPH;
        }

    }
}