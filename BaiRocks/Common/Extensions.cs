using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiRocs.Common
{
    public static class Extend
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static double StandardDeviation(this IEnumerable<double> values, double sample)
        {
            double avg = values.Average();
            double stdDev = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

            var dev = sample - avg;
            return dev / stdDev;
        }
    }

   

    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">the control for which the update is required</param>
        /// <param name="action">action to be performed on the control</param>
        public static void InvokeOnUiThreadIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }


    }

}
