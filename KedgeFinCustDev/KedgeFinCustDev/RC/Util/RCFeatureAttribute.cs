using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC.Util
{
    public class RCFeatureAttribute : FeatureAttribute
    {
        public RCFeatureAttribute(bool defValue)
        : base(defValue, null, null)
        {
        }

        public RCFeatureAttribute(bool defValue, Type parent)
            : base(defValue, parent, null)
        {
        }

        public RCFeatureAttribute(Type parent)
            : base(parent, null)
        {
        }

        public RCFeatureAttribute(Type parent, Type checkUsage)
            : base(checkUsage)
        {
        }

        public RCFeatureAttribute(bool defValue, Type parent, Type checkUsage)
            : base(defValue, parent, checkUsage)
        {
        }

        protected override void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            bool? currentValue = (bool?)sender.GetValue(e.Row, Parent.Name);
            bool? oldValue = (bool?)sender.GetValue(e.OldRow, Parent.Name);
            object status = sender.GetValue(e.Row, "Status");
            if ((status != null && status is int? && (int)status != 0) || currentValue == oldValue)
            {
                return;
            }
            if (currentValue == true)
            {
                object value = sender.GetValue(e.Row, _FieldName);
                if (value == null || !(value is bool?) || (bool?)value != true)
                {
                    sender.SetDefaultExt(e.Row, _FieldName);
                }
                if (SyncToParent)
                {
                    sender.SetValueExt(e.Row, _FieldName, true);
                }
            }
            else
            {
                sender.SetValueExt(e.Row, _FieldName, false);
            }
        }
    }
}
