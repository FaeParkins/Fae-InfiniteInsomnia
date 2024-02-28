using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteInsomnia
{
    public sealed class ModConfig
    {
        private int endOfDayTime = 2500;
        private int staminaReducedTo = 100;

        /// <summary>
        /// The time at which the mod will lock time
        /// The later constraint is present due to the way stardew seems to perform end of day checks
        /// </summary>
        /// <remarks>Constrained between 600 (6am) and 2540 (2:40am)</remarks>
        public int EndOfDayTime
        {
            get => endOfDayTime;
            set
            {
                if (value < 600)
                    endOfDayTime = 600;
                else if (value > 2540)
                    endOfDayTime = 2540;
                else
                    endOfDayTime = value;
            }
        }

        /// <summary>
        /// The amount stamina will be reduced to when staying up past the end of the day
        /// Any lower than 100 will result in graphical errors
        /// </summary>
        /// <remarks>Constrained to always be above 100</remarks>
        public int StaminaReducedTo
        {
            get => staminaReducedTo;
            set
            {
                if (value < 100)
                    staminaReducedTo = 100;
                else
                    staminaReducedTo = value;
            }
        }

        /// <summary>
        /// When you stay up late stardew reduces stamina regen from sleep, this will turn that off
        /// </summary>
        public bool FullStaminaAfterSleep { get; set; } = false;
    }
}
