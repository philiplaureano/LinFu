﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface ICustomTypeDescriptor
    {
        ICustomTypeConverter GetConverter(Type targetType);
    }
}