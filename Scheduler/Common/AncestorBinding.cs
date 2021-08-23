using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Scheduler
{
    internal class AncestorBinding : MarkupExtension
    {
        public string Path { get; set; }
        public Type AncestorType { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Path) || AncestorType == null)
            {
                throw new ArgumentNullException();
            }

            return new Binding(Path)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = this.AncestorType
                }
            };
        }
    }
}
