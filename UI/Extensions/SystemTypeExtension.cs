using System;
using System.Windows.Markup;

namespace Nonogram.UI.Extensions
{
    internal class SystemTypeExtension : MarkupExtension
    {
        private object _parameter;
        public bool Bool { set => _parameter = value; }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _parameter;
        }
    }
}