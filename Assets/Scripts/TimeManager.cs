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
                EastWestDegrees = (float)((value.TotalHours - 6) * HoursToDegreesMultiplier);
            }
        }
        
        public float TimeHours
        {
            get => (float)Time.TotalHours;
            set => Time = TimeSpan.FromHours(value);
        }

        public float TimeDegrees
        {
            get => EastWestDegrees;
            set => Time = DetermineTime(value);
        }
        
        private Vector3 _eulers;

        /// <summary>Internal utility method</summary>
        /// <remarks>Use <see cref="TimeDegrees"/> if you also want <see cref="Time"/> to be updated</remarks>
        private float EastWestDegrees
        {
            get => _eulers.x;
            set
            {
                _eulers.x = value;
                transform.eulerAngles = _eulers;
            }
        }

        private TimeSpan DetermineTime(float degrees)
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
