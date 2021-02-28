using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class RulerGrid : RulerBase
    {
        public RulerGrid()
        {
            this.DefaultStyleKey = typeof(RulerGrid);
            //this.RulerMinimumHeight = 30;
        }
    }
}
