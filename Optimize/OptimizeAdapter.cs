using MathWorks.MATLAB.Runtime;
using Optimize;
using Api.Models;
using Api.Utils.DTOs;

public interface IOptimize
{
    void Optimize(Grid grid, List<HistoricalData> historicalDataList, out OptimizationOutput output);
}

namespace Api.Optimize
{
    public class OptimizeAdapter : IOptimize
    {
        // executes malab optimization function
        public void Optimize(Grid grid, List<HistoricalData> historicalDataList, out OptimizationOutput output)
        {   // executes malab function
            string ctfPath = @"Optimize/Optimize.ctf";
            using (dynamic matlab = MATLABRuntime.StartMATLAB(ctfPath))
            {   // Optimizer input variables
                bool[] peakHours = historicalDataList.Select(data => // peak hours binary array
                        (data.Timestamp.Hour >=7 && data.Timestamp.Hour <=22) ? true : false
                    ).ToArray();
                double[] demandPowers = historicalDataList.Select(data => data.DemandPower).ToArray();
                // Reverse the arrays to recover the original order
                Array.Reverse(peakHours);
                Array.Reverse(demandPowers);
                // Optimizer output variables
                dynamic solPb;
                dynamic solEb;
                dynamic solPgMax1;
                dynamic solPgMax2;
                dynamic solPgMaxPH;
                MATLABFunctions.optimize(
                    matlab,
                    grid.DeltaT,
                    grid.EnergyCost,
                    grid.PowerCost,
                    grid.PowerCostPH,
                    grid.UnservedEnergyCost,
                    grid.MaxEnergy,
                    grid.MaxSOC,
                    grid.MinSOC,
                    grid.InitialEnergy,
                    grid.PowerLimit,
                    demandPowers,
                    grid.MinBatteryPower,
                    grid.MaxBatteryPower,
                    peakHours,
                    grid.MaxPower1,
                    grid.MaxPower2,
                    grid.MonthMaxPowerPH,
                    grid.ChargeEfficiency,
                    1/grid.DischargeEfficiency,
                    out solPb,
                    out solEb,
                    out solPgMax1,
                    out solPgMax2,
                    out solPgMaxPH
                );
                // Convertion to C# types
                solPb = (double[]) solPb;
                solEb = (double[]) solEb;
                solPgMax1 = (double) solPgMax1;
                solPgMax2 = (double) solPgMax2;
                solPgMaxPH = (double) solPgMaxPH;
                output = new OptimizationOutput(solPb, solEb, solPgMax1, solPgMax2, solPgMaxPH);
            }
        }
    }
}