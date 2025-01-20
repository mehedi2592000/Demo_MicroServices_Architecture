using System.Diagnostics.Metrics;

namespace api_Product.OpenTelematry
{
    public class DiagnosticsConfig
    {

        public const string ServiceName = "BasicOpenTelemetry.API";

        public static Meter Meter = new(ServiceName);

        //Metric to track the number of students
        public static Counter<int> StudentCounter = Meter.CreateCounter<int>("students.count");
    }
}
