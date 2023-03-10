using System;
using System.Linq;
using PX.Data;
using System.Collections.Generic;
using RCGV.GV.DAC;
using PX.Objects.IN;

namespace RCGV.GV.Descriptor
{
	public class AutoNumberAttribute : PXEventSubscriberAttribute,
									   IPXFieldDefaultingSubscriber, IPXFieldVerifyingSubscriber,
									   IPXRowPersistingSubscriber, IPXRowPersistedSubscriber
	{
		public const string NewValue = "<NEW>";

		private bool _AutoNumbering;
		private Type _AutoNumberingField;
		private BqlCommand _LastNumberCommand;

		public virtual string Prefix { get; private set; }
		public static void SetPrefix<Field>(PXCache sender, object row, string prefix) where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attribute in sender.GetAttributes<Field>(row))
			{
				if (attribute is AutoNumberAttribute)
				{
					((AutoNumberAttribute)attribute).Prefix = prefix;
				}
			}
		}

		public virtual Type LastNumberField { get; private set; }
		public static void SetLastNumberField<Field>(PXCache sender, object row, Type lastNumberField)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attribute in sender.GetAttributes<Field>(row))
			{
				if (attribute is AutoNumberAttribute)
				{
					AutoNumberAttribute attr = (AutoNumberAttribute)attribute;
					attr.LastNumberField = lastNumberField;
					attr.CreateLastNumberCommand();
				}
			}
		}

		public AutoNumberAttribute(Type autoNumbering)
		{
			if (autoNumbering != null &&
				(typeof(IBqlSearch).IsAssignableFrom(autoNumbering) ||
				 typeof(IBqlField).IsAssignableFrom(autoNumbering) && autoNumbering.IsNested))
			{
				_AutoNumberingField = autoNumbering;
			}
			else
			{
				throw new PXArgumentException("autoNumbering");
			}
		}

		public AutoNumberAttribute(Type autoNumbering, Type lastNumberField)
			: this(autoNumbering)
		{
			LastNumberField = lastNumberField;
			CreateLastNumberCommand();
		}

		private void CreateLastNumberCommand()
		{
			_LastNumberCommand = null;

			if (LastNumberField != null)
			{
				if (typeof(IBqlSearch).IsAssignableFrom(LastNumberField))
					_LastNumberCommand = BqlCommand.CreateInstance(LastNumberField);
				else if (typeof(IBqlField).IsAssignableFrom(LastNumberField) && LastNumberField.IsNested)
					_LastNumberCommand = BqlCommand.CreateInstance(typeof(Search<>), LastNumberField);
			}

			if (_LastNumberCommand == null) throw new PXArgumentException("lastNumberField");
		}

		public override void CacheAttached(PXCache sender)
		{
			BqlCommand command = null;
			Type autoNumberingField = null;
			if (typeof(IBqlSearch).IsAssignableFrom(_AutoNumberingField))
			{
				command = BqlCommand.CreateInstance(_AutoNumberingField);
				autoNumberingField = ((IBqlSearch)command).GetField();
			}
			else
			{
				command = BqlCommand.CreateInstance(typeof(Search<>), _AutoNumberingField);
				autoNumberingField = _AutoNumberingField;
			}
			PXView view = new PXView(sender.Graph, true, command);
			object row = view.SelectSingle();
			if (row != null)
			{
				_AutoNumbering = (bool)view.Cache.GetValue(row, autoNumberingField.Name);
			}
		}

		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (_AutoNumbering)
			{
				e.NewValue = NewValue;
			}
		}

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_AutoNumbering && PXSelectorAttribute.Select(sender, e.Row, _FieldName, e.NewValue) == null)
			{
				e.NewValue = NewValue;
			}
		}

		protected virtual string GetNewNumber(PXCache sender, Type setupType)
		{
			if (_LastNumberCommand == null)
				CreateLastNumberCommand();
			PXView view = new PXView(sender.Graph, false, _LastNumberCommand);
			object row = view.SelectSingle();
			if (row == null) return null;

			string lastNumber = (string)view.Cache.GetValue(row, LastNumberField.Name);
			char[] symbols = lastNumber.ToCharArray();
			for (int i = symbols.Length - 1; i >= 0; i--)
			{
				if (!char.IsDigit(symbols[i])) break;

				if (symbols[i] < '9')
				{
					symbols[i]++;
					break;
				}
				symbols[i] = '0';
			}
			lastNumber = new string(symbols);

			view.Cache.SetValue(row, LastNumberField.Name, lastNumber);
			PXCache setupCache = sender.Graph.Caches[setupType];
			setupCache.Update(row);
			setupCache.PersistUpdated(row);

			if (!string.IsNullOrEmpty(Prefix))
			{
				lastNumber = Prefix + lastNumber;
			}
			return lastNumber;
		}

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert)
			{
				Type setupType = BqlCommand.GetItemType(_AutoNumberingField);
				string lastNumber = GetNewNumber(sender, setupType);
				if (lastNumber != null)
				{
					sender.SetValue(e.Row, _FieldOrdinal, lastNumber);
				}
			}
		}

		public virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert &&
				e.TranStatus == PXTranStatus.Aborted)
			{
				sender.SetValue(e.Row, _FieldOrdinal, NewValue);
				Type setupType = BqlCommand.GetItemType(_AutoNumberingField);
				sender.Graph.Caches[setupType].Clear();
			}
		}
	}

}
namespace RCGV.GV.Attribute.Selector
{
    public class RegistrationCDAttribute : PXSelectorAttribute
    {
        public RegistrationCDAttribute()
                : base(typeof(Search<GVRegistration.registrationCD, Where<GVRegistration.isActive,Equal<True>>>),
                typeof(GVRegistration.registrationCD),
                typeof(GVRegistration.govUniformNumber),
                typeof(GVRegistration.siteNameChinese))
        {
            SubstituteKey = typeof(GVRegistration.registrationCD);
            DescriptionField = typeof(GVRegistration.siteNameChinese);
        }

    }
}

namespace RCGV.GV.Attribute.DropDown
{
    public class UnitAttribute : PXStringListAttribute
    {
        public UnitAttribute(): base()
        {
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            PXGraph graph = new PXGraph();
            PXResultset<INUnit> set = PXSelectGroupByOrderBy<INUnit, Aggregate<GroupBy<INUnit.fromUnit>>,
                OrderBy<Asc<INUnit.fromUnit>>>.Select(graph);
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (INUnit inunit in set)
            {
                keys.Add(inunit.FromUnit);
                values.Add(inunit.FromUnit);
            }
            this._AllowedLabels = values.ToArray();
            this._AllowedValues = keys.ToArray();
        }
    }

    public class GuiTypeAttribute : PXStringListAttribute
    {
        String type;
        public const String Output = "O";
        public const String Input = "I";
        public GuiTypeAttribute(String type) : base()
        {
            this.type = type;
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            PXGraph graph = new PXGraph();
            PXResultset<GVGuiType> set = PXSelect<GVGuiType,

                        Where<GVGuiType.gvType,
                            Equal<Required<GVGuiType.gvType>>

                            >>.Select(graph, type);
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (GVGuiType line in set)
            {
                keys.Add(line.GuiTypeCD);
                values.Add(line.GuiTypeCD + "-" + line.GuiTypeDesc);
            }
            this._AllowedLabels = values.ToArray();
            this._AllowedValues = keys.ToArray() ;
        }
    }

}