using System;

namespace FalcaPOS.Common.Attributes
{
    /// <summary>
    /// Ignore property attribute.    
    /// Use it to filter properties that are not part of export to excel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorePropertyAttribute : Attribute
    {
    }
}
