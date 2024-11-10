namespace Api.Utils.DTOs 
{
    public class OptimizationOutput
    {
        public double[] BatteryPowers { get; set; }
        public double[] BatteryEnergy { get; set; }
        public double MaxPower { get; set; }
        public double MaxPowerPH { get; set; }

        // default constructor
        public OptimizationOutput()
        { 
            BatteryPowers = [];
            BatteryEnergy = [];
            MaxPower = 0;
            MaxPowerPH = 0;
        }
        // constructor with parameters
        public OptimizationOutput(double[] batteryPowers, double[] batteryEnergy, double maxPower, double maxPowerPH)
        {
            BatteryPowers = batteryPowers;
            BatteryEnergy = batteryEnergy;
            MaxPower = maxPower;
            MaxPowerPH = maxPowerPH;
        }

    }
}