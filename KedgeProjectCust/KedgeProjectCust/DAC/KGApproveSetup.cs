using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGApproveSetup : IBqlTable
    {
        #region ApproveSetupID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "ApproveSetupID", Required = true)]  
        public virtual int? ApproveSetupID { get; set; }
        public abstract class approveSetupID : PX.Data.BQL.BqlInt.Field<approveSetupID> { }
        #endregion

        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project", Required =true)]   
        [PXSelector(typeof(Search<PMProject.contractID, 
                Where<PMProject.baseType, Equal<CTPRType.project>,        
                 //And<PMProject.nonProject, Equal<false>, 
                 And<Match<Current<AccessInfo.userName>>>>>)
                , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false,
            DescriptionField =typeof(PMProject.description))]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        //Where<PMProject.contractID,
        //            NotIn2<Search<KGApproveSetup.contractID,
        //                    Where<PMProject.contractID,
        //                        Equal<KGApproveSetup.contractID>>>>,
        //         And<

        #region SetupID
        [PXDBInt()]
        [PXUIField(DisplayName = "Setup ID")]
        [PXDBDefault(typeof(KGSetUp.setupID))]
        [PXParent(typeof(Select<KGSetUp,
                            Where<KGSetUp.setupID,
                            Equal<Current<KGApproveSetup.setupID>>>>))]
        public virtual int? SetupID { get; set; }
        public abstract class setupID : PX.Data.BQL.BqlInt.Field<setupID> { }
        #endregion

        #region ApprovalSite
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Approval Site",Required =true)]
        [PXStringList(
            new string[] {"0","1" },
            new string[] { "測試機", "正式機" }
            )]
        public virtual string ApprovalSite { get; set; }
        public abstract class approvalSite : PX.Data.BQL.BqlString.Field<approvalSite> { }
        #endregion

        #region IsAutoApproved
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Auto Approved",Required =true)]
        [PXDefault(false)]
        public virtual bool? IsAutoApproved { get; set; }
        public abstract class isAutoApproved : PX.Data.BQL.BqlBool.Field<isAutoApproved> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch(typeof(KGSetUp.branchID))]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/
    }
}