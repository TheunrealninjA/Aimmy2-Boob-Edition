using Accord.Statistics.Running;
using Aimmy2.WinformsReplacement;

namespace Aimmy2.AILogic
{
    internal class KalmanPrediction
    {
        public struct Detection
        {
            public int X;
            public int Y;
            public DateTime Timestamp;
        }

        private readonly KalmanFilter2D kalmanFilter = new KalmanFilter2D();
        private long lastFilterUpdateTicks = DateTime.UtcNow.Ticks;

        public void UpdateKalmanFilter(Detection detection)
        {
            kalmanFilter.Push(detection.X, detection.Y);
            lastFilterUpdateTicks = DateTime.UtcNow.Ticks;
        }

        public Detection GetKalmanPosition()
        {
            double timeStep = (DateTime.UtcNow.Ticks - lastFilterUpdateTicks) / TimeSpan.TicksPerSecond;

            double predictedX = kalmanFilter.X + kalmanFilter.XAxisVelocity * timeStep;
            double predictedY = kalmanFilter.Y + kalmanFilter.YAxisVelocity * timeStep;

            return new Detection { X = (int)predictedX, Y = (int)predictedY };
        }
    }


    internal class WiseTheFoxPrediction
    { // Proof of Concept Prediction as written by @wisethef0x
        public struct WTFDetection
        {
            public int X;
            public int Y;
            public DateTime Timestamp;
        }

        private DateTime lastUpdateTime;
        private const double alpha = 0.5; // Smoothing factor, adjust as necessary

        private double emaX;
        private double emaY;

        public void UpdateDetection(WTFDetection detection)
        {
            if (lastUpdateTime == DateTime.MinValue)
            {
                emaX = detection.X;
                emaY = detection.Y;
            }
            else
            {
                emaX = alpha * detection.X + (1 - alpha) * emaX;
                emaY = alpha * detection.Y + (1 - alpha) * emaY;
            }

            lastUpdateTime = DateTime.UtcNow;
        }

        public WTFDetection GetEstimatedPosition()
        {
            return new WTFDetection { X = (int)emaX, Y = (int)emaY };
        }
    }

    internal class ShalloePredictionV2
    {
        public static List<int> xValues = [];
        public static List<int> yValues = [];
        private static int currentIndex = 0;
        private static int sumX = 0;
        private static int sumY = 0;
        public static int AmountCount = 2;

        public static void AddValues(int x, int y)
        {
            sumX -= xValues[currentIndex];
            sumY -= yValues[currentIndex];

            xValues[currentIndex] = x;
            yValues[currentIndex] = y;

            sumX += x;
            sumY += y;

            currentIndex = (currentIndex + 1) % AmountCount;
        }

        public static int GetSPX()
        {
            return (int)(sumX / (double)AmountCount + WinAPICaller.GetCursorPosition().X);
        }

        public static int GetSPY()
        {
            return (int)(sumY / (double)AmountCount + WinAPICaller.GetCursorPosition().Y);
        }
    }

    //internal class HoodPredict
    //{
    //    public static List<int> xValues = [];
    //    public static List<int> yValues = [];

    //    public static int AmountCount = 2;

    //    public static int GetHPX(int CurrentX, int PrevX)
    //    {
    //        int CurrentTime = DateTime.Now.Millisecond;
    //        return 1;
    //    }

    //    public static int GetSPY()
    //    {
    //        //Debug.WriteLine((int)Queryable.Average(yValues.AsQueryable()));
    //        return (int)(((Queryable.Average(yValues.AsQueryable()) * AmountCount) + WinAPICaller.GetCursorPosition().Y));
    //    }
    //}
}