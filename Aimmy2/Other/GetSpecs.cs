using System.Management;

namespace Aimmy2.Other
{
    internal class GetSpecs
    {
        // Reference: https://www.youtube.com/watch?v=rou471Evuzc
        // Nori
        public static string? GetSpecification(string HardwareClass, string Syntax)
        {
            try
            {
                ManagementObjectSearcher SpecsSearch = new("root\\CIMV2", "SELECT * FROM " + HardwareClass);

                if (SpecsSearch.Get().Count == 0 || SpecsSearch == null)
                {
                    return "Not Found";
                }

                foreach (ManagementObject MJ in SpecsSearch.Get().Cast<ManagementObject>())
                {
                    return Convert.ToString(MJ[Syntax])?.Trim();
                }
                return "Not Found";
            }
            catch (Exception e)
            {
                FileManager.LogError("Failed to get specs: " + e, true);
                return "Not Found";
            }
        }
    }
}