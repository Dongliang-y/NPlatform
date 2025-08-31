namespace NPlatform
{
    public static class EnvironmentHelper
    {
        public static string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                   ?? "Production"; // 默认值
        }

        public static bool IsDevelopment()
        {
            return string.Equals(GetEnvironment(), "Development", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsStage()
        {
            return string.Equals(GetEnvironment(), "Stage", StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsProduction()
        {
            return string.Equals(GetEnvironment(), "Production", StringComparison.OrdinalIgnoreCase);
        }
    }
}
