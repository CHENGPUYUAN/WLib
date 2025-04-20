using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WLib.Samples.WinForm
{

    // Optional: If UI updates are required, use this pattern instead:
    // Example for updating progressBarTextLabel (if exists):
    // progressBarTextLabel.InvokeIfRequired(() => progressBarTextLabel.Text = $"开始处理{name}");

    // Add extension method for Control.Invoke pattern (if needed):
    public static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
