﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDK.Windowing;

public class MouseWheelEventArgs : EventArgs
{
    public float x;
    public float y;
    
    public MouseWheelEventArgs(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

public delegate void MouseWheelEventHandler(object sender, MouseWheelEventArgs e);
