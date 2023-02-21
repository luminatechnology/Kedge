using System;
using PX.Data;
using PX.Objects.CR;
using RCGV.GV.DAC;
using RCGV.GV.SYS;

namespace RCGV.GV
{
	public class GVRegistrationMaint : GVBaseViewGraph<GVRegistrationMaint>
	{
        #region Action Bar
        public PXSave<GVRegistration> Save;
        public PXCancel<GVRegistration> Cancel;
        public PXInsert<GVRegistration> Insert;
        public PXCopyPasteAction<GVRegistration> CopyPaste;
        public PXDelete<GVRegistration> Delete;
        public PXFirst<GVRegistration> First;
        public PXPrevious<GVRegistration> Previous;
        public PXNext<GVRegistration> Next;
        public PXLast<GVRegistration> Last;
        #endregion

        #region Select
        public PXSelect<GVRegistration> Registrations;
        public PXSelectJoin<GVRegistrationBranch,
            InnerJoin<BAccount,
            On<BAccount.bAccountID, Equal<GVRegistrationBranch.bAccountID>>,
            LeftJoin<GVRegistration,
            On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>>>,
            Where<GVRegistrationBranch.registrationID, Equal<Current<GVRegistration.registrationID>>>> baccounts;
        public PXSelect<BAccount> ba;
        #endregion
        
        #region Registration Verify
        //CD驗證長度
        protected virtual void GVRegistration_RegistrationCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			GVRegistration row = (GVRegistration)e.Row;
            if (row == null)
            {
                return;
            }
            if(row.RegistrationCD != null)
            {
                setCheckThread(checkRegistrationCD_M(sender, row, (String)row.RegistrationCD));
            }
        }
           
        public bool checkRegistrationCD_M(PXCache sender, GVRegistration row, String newValue)
        {
            string RegistrationCDs = (string)newValue;
            if (row.RegistrationCD == null || RegistrationCDs == null)
            {
                sender.RaiseExceptionHandling<GVRegistration.registrationCD>(
                        row, newValue,
                          new PXSetPropertyException(
                          "RegistrationCD cannot be null",
                          PXErrorLevel.Error));
                return true;
            }
            if (RegistrationCDs.Equals(row.RegistrationCD))
            {
                return false;
            }
            int intRegistrationCDs = RegistrationCDs.Length;
            if (intRegistrationCDs < 9)
            {

                sender.RaiseExceptionHandling<GVRegistration.registrationCD>(
                        row, newValue,
                          new PXSetPropertyException(
                          "RegistrationCD is too short!",
                          PXErrorLevel.Error));
                return true;
            }
            else if (intRegistrationCDs > 9)
            {

                sender.RaiseExceptionHandling<GVRegistration.registrationCD>(
                        row, newValue,
                          new PXSetPropertyException(
                          "RegistrationCD is too long!",
                          PXErrorLevel.Error));
                return true;
            }
            return false;
        }
 
        //統編驗證長度
        protected virtual void GVRegistration_GovUniformNumber_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			GVRegistration row = (GVRegistration)e.Row;
            
            setCheckThread(checkGovUniformNumber_M(sender, row, (String)row.GovUniformNumber));
        }
        public bool checkGovUniformNumber_M(PXCache sender, GVRegistration row, String newValue)
        {
            
            string GovUniformNumbers = (string)newValue;
            int intGovUniformNumbers = GovUniformNumbers.Length;
            if (row.GovUniformNumber == null && GovUniformNumbers == null)
            {
                return false;                 
            }
            
            if (intGovUniformNumbers < 8)
            {
                sender.RaiseExceptionHandling<GVRegistration.govUniformNumber>(
                        row, newValue,
                          new PXSetPropertyException(
                          "GovUniformNumber too short",
                          PXErrorLevel.Error));
                return true;
            }
            else if (intGovUniformNumbers > 8)
            {

                sender.RaiseExceptionHandling<GVRegistration.govUniformNumber>(
                        row, newValue,
                          new PXSetPropertyException(
                          "GovUniformNumber too long",
                          PXErrorLevel.Error));
                return true;
            }
            return false;
        }

        //如果DeclarationPayCode=1 or 2時, ParentRegistrationCD為必填
        public bool checkDeclarationPayCodeCD_M(PXCache sender, GVRegistration row, String newValue)
        { 
            if (newValue == "1" || newValue == "2")
            {
                if (row.ParentRegistrationCD == null)
                {
                    sender.RaiseExceptionHandling<GVRegistration.parentRegistrationCD>(
                        row, row.ParentRegistrationCD,
                          new PXSetPropertyException(
                          "ParentRegistrationCD cannot be null",
                          PXErrorLevel.Error));
                    return true;
                }               
            }
            return false;
        }

        #endregion

        #region Branch Verify
        protected virtual void GVRegistrationBranch_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVRegistrationBranch row = (GVRegistrationBranch)e.Row;
            setCheckThread(checkBAccountID_M(sender, row, (int?)e.NewValue));
        }

        public bool checkBAccountID_M(PXCache sender, GVRegistrationBranch row, int? newValue)
        {
            if (row == null || newValue == null)
            {
                return false;
            }
            // no modify
            if (newValue.Equals(row.BAccountID) )
            {
                return false;
            }
            PXResultset<GVRegistrationBranch> set = PXSelect<GVRegistrationBranch,
                       Where<GVRegistrationBranch.bAccountID,
                           Equal<Required<GVRegistrationBranch.bAccountID>>>>
                               .Select(this, newValue/*, row.BAccountID*/);

            foreach (GVRegistrationBranch line in set)
            {
                if (line.BAccountID == null) { continue; }

                if (line.BAccountID.Equals(newValue))
                {
                    if (line.RegistrationID.Equals(row.RegistrationID)) { continue; }

                    sender.RaiseExceptionHandling<GVRegistrationBranch.bAccountID>(row, BAccount.PK.Find(sender.Graph, line.BAccountID.Value).AcctCD,
                                                                                   new PXSetPropertyException("AcctCD is duplicate.", PXErrorLevel.Error));
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Event
        protected virtual void GVRegistrationBranch_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			GVRegistrationBranch row = (GVRegistrationBranch)e.Row;
			if (row.RegistrationID == null)
			{
				GVRegistrationBranch line = baccounts.Current;
				GVRegistration form = Registrations.Current;
				line.RegistrationID = form.RegistrationID;
			}
		}
        protected virtual void GVRegistrationBranch_BAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			GVRegistrationBranch row = (GVRegistrationBranch)e.Row;
			BAccount baccount = null;
			if (row.BAccountID != null)
			{
				baccount = PXSelectorAttribute.Select<GVRegistrationBranch.bAccountID>(
						sender, row) as BAccount;
				if (baccount != null)
				{
					row.BAccountID = baccount.BAccountID;

				}
			}

		}
    
        protected virtual void GVRegistration_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            GVRegistration row = (GVRegistration)e.Row; 
            setCheckThread(checkRegistrationCD_M(sender, row, row.RegistrationCD));
            setCheckThread(checkGovUniformNumber_M(sender, row, row.GovUniformNumber));   
            //setCheckThread(checkParentRegistrationCD_M(sender, row, row.ParentRegistrationCD));
            setCheckThread(checkDeclarationPayCodeCD_M(sender, row, row.DeclarationPayCode));
            //2020/04/09 刪掉
            //setCheckThread(checkEffectiveDate_M(sender, row, row.EffectiveDate));
            //setCheckThread(checkExpirationDate_M(sender, row, row.ExpirationDate));
            e.Cancel = getCheckThread();
        }
        protected virtual void GVRegistrationBranch_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            GVRegistrationBranch row = (GVRegistrationBranch)e.Row;
            setCheckThread(checkBAccountID_M(sender, row, row.BAccountID));            
            e.Cancel = getCheckThread();
        }
        #endregion

        #region Method
        public override void Persist()
        {
            GVRegistration row = Registrations.Current;
            setCheckThread(checkRegistrationCD_M(Registrations.Cache, row, row.RegistrationCD));
            setCheckThread(checkGovUniformNumber_M(Registrations.Cache, row, row.GovUniformNumber));
            setCheckThread(checkDeclarationPayCodeCD_M(Registrations.Cache, row, row.DeclarationPayCode));

            foreach (GVRegistrationBranch details in baccounts.Cache.Inserted)
            {
                setCheckThread(checkBAccountID_M(baccounts.Cache, details, details.BAccountID));
            }
            foreach (GVRegistrationBranch details in baccounts.Cache.Updated)
            {
                setCheckThread(checkBAccountID_M(baccounts.Cache, details, details.BAccountID));
            }
           
            if (getCheckThread())
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
                base.Persist();
        }
        #endregion
    }
}


