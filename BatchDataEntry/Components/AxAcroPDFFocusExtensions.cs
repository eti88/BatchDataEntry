using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxAcroPDFLib;

namespace BatchDataEntry.Components
{
    static class AxAcroPDFFocusExtensions
    {
        struct TimerTag
        {
            public TimerTag(AxAcroPDF control, object controlTag)
            {
                Control = control;
                ControlTag = controlTag;
            }

            public object ControlTag;
            public AxAcroPDF Control;
        }

        /// <summary>
        /// Prevent Adobe AxAcroPDF from stealing focus when you change the src. 
        /// The control will be disabled for 250 ms
        /// /// <param name="pdfControl">The acrobat ActiveX control</param>
        /// </summary>
        public static void SuspendStealFocus(this AxAcroPDF pdfControl)
        {
            SuspendStealFocus(pdfControl, 250);
        }

        /// <summary>
        /// Prevent Adobe AxAcroPDF from stealing focus when you change the src. 
        /// </summary>
        /// <param name="pdfControl">The acrobat ActiveX control</param>
        /// <param name="timeSpan">Time the ActiveX will be inaccessible (and not grabbing focus)</param>
        public static void SuspendStealFocus(this AxAcroPDF pdfControl, TimeSpan timeSpan)
        {
            SuspendStealFocus(pdfControl, (int) timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// Prevent Adobe AxAcroPDF from stealing focus when you change the src. 
        /// </summary>
        /// <param name="pdfControl">The acrobat ActiveX control</param>
        //// <param name="timeoutInMilliSeconds">Number of milliseconds the ActiveX will be inaccessible (and not grabbing focus)</param>
        public static void SuspendStealFocus(this AxAcroPDF pdfControl, int timeoutInMilliSeconds)
        {
            pdfControl.Enabled = false;

            Timer t = new Timer();
            t.Interval = timeoutInMilliSeconds;
            t.Tick += t_Tick;
            t.Start();

            pdfControl.Tag = Guid.NewGuid();
            t.Tag = new TimerTag(pdfControl, pdfControl.Tag);
        }

        static void t_Tick(object sender, EventArgs e)
        {
            var timer = (Timer) sender;
            timer.Stop();
            timer.Dispose();

            TimerTag t = (TimerTag) timer.Tag;
            if (ReferenceEquals(t.Control.Tag, t.ControlTag))
            {
                t.Control.Enabled = true;
            }
        }
    }
}
