using System;

namespace Service.Commons
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SortDirectionAttribute : Attribute
    {
    }
}
