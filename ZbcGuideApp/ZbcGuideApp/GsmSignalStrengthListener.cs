using System;
using System.Collections.Generic;
using System.Text;
using Android.Telephony;
namespace ZbcGuideApp
{
    class GsmSignalStrengthListener : PhoneStateListener
    {
        public delegate void SignalStrengthChangedDelegate(int strength);

        public event SignalStrengthChangedDelegate SignalStrengthChanged;

        public override void OnSignalStrengthsChanged(SignalStrength newSignalStrength)
        {
            if (newSignalStrength.IsGsm)
            {
                if (SignalStrengthChanged != null)
                {
                    SignalStrengthChanged(newSignalStrength.GsmSignalStrength);
                }
            }
        }
    }
}
