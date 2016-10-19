using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    
    public class ValueChangedEventArgs : EventArgs
    {
        public double Value { get; set; }
        public ValueChangedEventArgs(double value)
        {
            Value = value;
        }
    }
}
