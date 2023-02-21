using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.GL;
using RCGV.GV.Attribute.Selector;

namespace RCGV.GV
{
	public class GVPeriodMaint : PXGraph<GVPeriodMaint>
	{
		#region Buttons
		//public PXCancel<GVPeriod> Cancel;
		//public PXCopyPasteAction<GVPeriod> CopyPaste;
		//public PXDelete<GVPeriod> Delete;
		//public PXInsert<GVPeriod> Insert;
		public PXSave<GVPeriod> Save;
		//public PXFirst<GVTaxMapping> First;
		//public PXLast<GVTaxMapping> Last;
		//public PXNext<GVTaxMapping> Next;
		//public PXPrevious<GVTaxMapping> Previous;
		#endregion

		#region Selects

		[PXCopyPasteHiddenView]
		public PXFilter<GVPeriodSettingsFilter> PeriodSettings;
		[PXFilterable]
		public PXSelect<GVPeriod> GVPeriods;
		//public PXSelect<GVPeriod, 
		//	Where<GVPeriod.periodYear, 
		//		Equal<Current<FinYear.year>>>, 
		//		OrderBy<Asc<GVPeriod.periodYear>>> GVPeriods;
		#endregion

		#region Actions
		//public PXSave<GVPeriod> Save;

		public PXAction<GVPeriod> AutoFill;
		[PXButton(Tooltip = "Auto fill periods")]
		[PXUIField(DisplayName = "Generate Periods", MapEnableRights = PXCacheRights.Select)]
		public virtual IEnumerable autoFill(PXAdapter adapter)
		{

			if (this.PeriodSettings.AskExt(true) == WebDialogResult.OK)
			{
				var periodFilter = (GVPeriodSettingsFilter)this.PeriodSettings.Cache.Current;
				if (String.IsNullOrEmpty(periodFilter.RegistrationCD))
				{
					throw new PXException("Registration ID is invalid.");
				}

				if (String.IsNullOrEmpty(periodFilter.PeriodYear) ||
					periodFilter.PeriodYear.Length != 4)
				{
					throw new PXException("Period Year is invalid.");
				}
				//mark by louis 20220526 不需要檢核產生的Period是今年
				/**
				if(Int32.Parse(periodFilter.PeriodYear)<=System.DateTime.Now.Year)
				{
					throw new Exception(string.Format("Please enter the year after {0}!", System.DateTime.Today.Year.ToString()));
				}**/

				foreach (GVPeriod period in
							PXSelectReadonly<GVPeriod,
								Where<GVPeriod.registrationCD, Equal<Required<GVPeriod.registrationCD>>,
									And<GVPeriod.periodYear, Equal<Required<GVPeriod.periodYear>>>>>
								.Select(this, periodFilter.RegistrationCD, periodFilter.PeriodYear))
				{
					throw new PXException("Period Year exist can't regenerate.");
				}

				PXLongOperation.StartOperation(this, delegate()
				{
					CreatePeriod(periodFilter);
				});
				
			}
			return adapter.Get();
		}

		public static void CreatePeriod(GVPeriodSettingsFilter periodFilter)
		{
			GVPeriodMaint graph = PXGraph.CreateInstance<GVPeriodMaint>();
			graph.PeriodSettings.Current = periodFilter;
			for (int i = 1; i < 13; i++)
			{
				GVPeriod period = new GVPeriod();
				period.RegistrationCD = periodFilter.RegistrationCD;
				period.PeriodYear = Int32.Parse(periodFilter.PeriodYear);
				period.PeriodMonth = i;

				graph.GVPeriods.Update(period);
			}
			graph.Persist();
			
			//// TODO: generate Period
			//foreach (GVLookupCodeValue lookupValue in
			//			PXSelectReadonly2<GVLookupCodeValue,
			//				InnerJoin<GVLookupCodeType,
			//					On<GVLookupCodeValue.lookupCodeTypeID, Equal<GVLookupCodeType.lookupCodeTypeID>>>,
			//				Where<GVLookupCodeType.lookupCodeType, Equal<Required<GVLookupCodeType.lookupCodeType>>>,
			//				OrderBy<Asc<GVLookupCodeValue.lookupCode>>>
			//				.Select(this, "DeclarePeriod"))
			//{
			//	GVPeriod period = new GVPeriod();
			//	period.RegistrationCD = periodFilter.RegistrationCD;
			//	period.PeriodYear = Int32.Parse(periodFilter.PeriodYear);
			//	period.PeriodMonth = Int32.Parse(lookupValue.LookupCode);

			//	graph.GVPeriods.Update(period);
			//}
		}
		#endregion

		#region GVPeriod Events
		protected virtual void GVPeriod_PeriodYear_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVPeriod row = (GVPeriod)e.Row;
			if (e.NewValue == null) return;

			if (e.NewValue.ToString().Length != 4)
			{
				sender.RaiseExceptionHandling<GVPeriod.periodYear>(
										row, e.NewValue,
												new PXSetPropertyException("Period Year format incorrect. ", PXErrorLevel.Error));
				e.Cancel = true;
			}
		}

		protected virtual void GVPeriod_PeriodMonth_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVPeriod row = (GVPeriod)e.Row;
			if (e.NewValue == null) return;

			int month = (int)e.NewValue;
			if (month < 1 || month > 12)
			{
				sender.RaiseExceptionHandling<GVPeriod.periodMonth>(
										row, e.NewValue,
												new PXSetPropertyException("Period Month range incorret.", PXErrorLevel.Error));
				e.Cancel = true;
			}
		}

        protected virtual void GVPeriod_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            GVPeriod row = e.Row as GVPeriod;
            PXUIFieldAttribute.SetReadOnly<GVPeriod.registrationCD>(sender, null, true);
			GVPeriods.AllowInsert = false;
        }

		protected virtual void GVPeriod_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			GVPeriod row = (GVPeriod)e.NewRow;

			if (row.RegistrationCD == null ||
				row.PeriodYear == null ||
				row.PeriodMonth == null)
			{
				//throw new PXSetPropertyException("尚未完成");
				return;
			}

			int year = (int)row.PeriodYear;
			if (year.ToString().Length != 4)
			{
				sender.RaiseExceptionHandling<GVPeriod.periodYear>(
										row, year,
										new PXSetPropertyException("Period Year format incorrect. ", PXErrorLevel.Error));
				e.Cancel = true;
			}

			int month = (int)row.PeriodMonth;
			if (month < 1 || month > 12)
			{
				sender.RaiseExceptionHandling<GVPeriod.periodMonth>(
										row, month,
										new PXSetPropertyException("Period Month range incorret.", PXErrorLevel.Error));
				e.Cancel = true;
			}

			// 不需要檢查,因為 IsKey = true 會找出相同的進行編輯覆蓋
			//GVPeriod period = PXSelectReadonly<GVPeriod,
			//		Where<GVPeriod.registrationCD, Equal<Required<GVPeriod.registrationCD>>,
			//			And<GVPeriod.periodYear, Equal<Required<GVPeriod.periodYear>>,
			//			And<GVPeriod.periodMonth, Equal<Required<GVPeriod.periodMonth>>>>>>
			//		.Select(this, row.RegistrationCD, row.PeriodYear, row.PeriodMonth);

			//// duplicate record
			//if (period != null)
			//{
			//	sender.RaiseExceptionHandling<GVPeriod.periodMonth>(
			//							row, row.PeriodMonth,
			//									new PXSetPropertyException("Period Month has been taken.", PXErrorLevel.Error));
			//	e.Cancel = true;
				
			//}
		}

		#endregion
	}

	[Serializable]
	public class GVPeriodSettingsFilter : IBqlTable
	{
        #region RegistrationCD
        public abstract class registrationCD : PX.Data.IBqlField
        {
        }
        protected string _RegistrationCD;
        [PXDBString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Registration CD", Required = true)]
        [RegistrationCDAttribute()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string RegistrationCD
        {
            get
            {
                return this._RegistrationCD;
            }
            set
            {
                this._RegistrationCD = value;
            }
        }
        #endregion
        #region Period Year
        public abstract class periodYear : PX.Data.IBqlField
		{
		}
		protected string _PeriodYear;
		[PXString(4, IsUnicode = true, InputMask = "####")]
		[PXDefault()]
		[PXUIField(DisplayName = "Period Year", Required = true)]
		public virtual string PeriodYear
		{
			get
			{
				return this._PeriodYear;
			}
			set
			{
				this._PeriodYear = value;
			}
		}
		#endregion
	}
}