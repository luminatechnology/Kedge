using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using PX.Data;
using PX.Objects.CS.DAC;
using PX.SM;
using PX.EP;

namespace KG
{
    public class KGLoginHomeEntry : PXGraph<KGLoginHomeEntry>
    {
 
        public PXFilter<FilterFoBranch> MasterView;
        //public PXSelect<FilterFoBranch> MasterView;

        #region �����q�T�{
        public PXAction<FilterFoBranch> ConfirmBranchAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�T�{�n�J��@���q")]
        protected IEnumerable confirmBranchAction(PXAdapter adapter)
        {
            if (MasterView.Current.LoginBranchID != null)
            {
                AccessUsers accessUsers = CreateInstance<AccessUsers>();
                accessUsers.UserList.Current = Users.PK.Find(this, this.Accessinfo.UserID);

                List<OrganizationBAccount> companies =
                            PXSelect<OrganizationBAccount>.Select(this).RowCast<OrganizationBAccount>().ToList();

                foreach (EPLoginTypeAllowsRole allowedRole in accessUsers.AllowedRoles.Select())
                {
                    if (companies.Exists(e => e.AcctCD.Trim() == allowedRole.Rolename))
                    {
                        allowedRole.Selected = companies.Exists(e => e.AcctCD.Trim() == allowedRole.Rolename
                                                                && e.BAccountID == MasterView.Current.LoginBranchID);
                        accessUsers.AllowedRoles.Update(allowedRole);
                    }
                }
                accessUsers.Save.Press();
                accessUsers.LoginAsUser.Press();
                PX.Data.Redirector.Refresh(HttpContext.Current);
            }else {
                throw new PXSetPropertyException<FilterFoBranch.loginBranchID>("�п�ܭn�n�J�����q");
            }
            
            return adapter.Get();
        }
        #endregion

        #region �n�J�ҥH���q
        public PXAction<FilterFoBranch> ALLBranchAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�n�J�Ҧ����q")]
        protected IEnumerable aLLBranchAction(PXAdapter adapter)
        {

            AccessUsers accessUsers = CreateInstance<AccessUsers>();
            accessUsers.UserList.Current = Users.PK.Find(this, this.Accessinfo.UserID);
            
            List<OrganizationBAccount> companies =
                        PXSelect<OrganizationBAccount>.Select(this).RowCast<OrganizationBAccount>().ToList();

            foreach (EPLoginTypeAllowsRole allowedRole in accessUsers.AllowedRoles.Select())
            {
                if (companies.Exists(e => e.AcctCD.Trim() == allowedRole.Rolename))
                {
                    allowedRole.Selected = true;
                    accessUsers.AllowedRoles.Update(allowedRole);
                }
            }
            accessUsers.Save.Press();
            PX.Data.Redirector.Refresh(HttpContext.Current);
            return adapter.Get();
        }
        #endregion

        

        #region FilterFoBranch Table
        [Serializable]
        //public class FilterFoBranch : OrganizationBAccount
        public class FilterFoBranch : IBqlTable
        {
            #region LoginBranchID
            [PXInt()]
            [PXUIField(DisplayName = "LoginBranchID")]
            [PXSelector(typeof(Search<OrganizationBAccount.bAccountID>),
                        typeof(OrganizationBAccount.acctCD),
                        typeof(OrganizationBAccount.acctName),
                      SubstituteKey = typeof(OrganizationBAccount.acctCD), 
                      DescriptionField = typeof(OrganizationBAccount.acctName),
                      ValidateValue = false)]
            [PXDefault(typeof(Search<PX.Objects.GL.Branch.bAccountID, 
                               Where<PX.Objects.GL.Branch.branchID, 
                               Equal<Current<AccessInfo.branchID>>>>), 
                       PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? LoginBranchID { get; set; }
            public abstract class loginBranchID : IBqlField { }
            #endregion
        }
        #endregion

    }
}