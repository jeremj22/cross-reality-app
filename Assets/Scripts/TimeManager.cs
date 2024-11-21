using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace Assets.Scripts
{
    public class TimeManager : MonoBehaviour
    {
        const double DegreeToHoursMultiplier = 1d / 360 * 24;
        const double HoursToDegreesMultiplier = 1d / DegreeToHoursMultiplier;

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set
            {
                _time = value;
                EastWestDegrees = (value.TotalHours - 6) * HoursToDegreesMultiplier;
            }
        }

        public double TimeHours
        {
            get => Time.TotalHours;
            set => Time = TimeSpan.FromHours(value);
        }

        public double TimeDegrees
        {
            get => EastWestDegrees;
            set => Time = DetermineTime(value);
        }

        private Vector3 _eulers;

        /// <summary>Internal utility method. Use <see cref="TimeDegrees"/></summary>
        private double EastWestDegrees
        {
            get => _eulers.x;
            set
            {
                _eulers.x = (float)value;
                transform.eulerAngles = _eulers;
            }
        }

        private TimeSpan DetermineTime(double degrees)
        {
            double hours = degrees * DegreeToHoursMultiplier + 6;

            return TimeSpan.FromHours(hours);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TimeSpan DetermineTime() => DetermineTime(EastWestDegrees);

        void Awake()
        {
            _eulers = transform.eulerAngles;

            Time = DetermineTime();
            Debug.Log($"Current Unity time: {Time}");
        }
    }
}
