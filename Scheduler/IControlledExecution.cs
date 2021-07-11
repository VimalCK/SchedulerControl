﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    internal interface IControlledExecution
    {
        bool IsEnabled { get; }

        void Enable();
        void Disable();
    }
}