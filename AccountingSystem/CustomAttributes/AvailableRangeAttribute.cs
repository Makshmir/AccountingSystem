using System;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.CustomAttributes
{
    public class AvailableRangeAttribute:RangeAttribute
    {
        public AvailableRangeAttribute() : base(typeof(double), "0", "10000")
        { }
    }
}
