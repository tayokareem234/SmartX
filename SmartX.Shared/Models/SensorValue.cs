namespace SmartX.Shared.Models
{
    public struct SensorValue
    {
        public double Value { get; }

        public string Unit { get; }

        public SensorValue(double value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        public static SensorValue operator +(
            SensorValue first,
            SensorValue second)
        {
            if (first.Unit != second.Unit)
            {
                throw new InvalidOperationException(
                    "Sensor values must use the same unit."
                );
            }

            return new SensorValue(
                first.Value + second.Value,
                first.Unit
            );
        }

        public static double operator -(
            SensorValue first,
            SensorValue second)
        {
            if (first.Unit != second.Unit)
            {
                throw new InvalidOperationException(
                    "Sensor values must use the same unit."
                );
            }

            return first.Value - second.Value;
        }

        public static bool operator >(
            SensorValue first,
            SensorValue second)
        {
            return first.Value > second.Value;
        }

        public static bool operator <(
            SensorValue first,
            SensorValue second)
        {
            return first.Value < second.Value;
        }

        public override string ToString()
        {
            return $"{Value} {Unit}";
        }
    }
}