using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.Common.Abstractions;
using PX.Objects.Common.MigrationMode;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using RCGV.GV.DAC;

namespace PX.Objects.AP
{
    /***
     * =====2021-03-08: 11972======Alton
     * APRegister新增一個欄位, UsrApGuiCmInvoiceNbr, nvarchar(15)
     * **/
    public class APRegisterGVExt : PXCacheExtension<PX.Objects.AP.APRegister>
    {
        #region UsrApGuiInvoiceNbr
        [PXDBString(255)]
        [PXUIField(DisplayName = "ApGuiInvoiceNbr", Enabled = false)]

        public virtual string UsrApGuiInvoiceNbr { get; set; }
        public abstract class usrApGuiInvoiceNbr : IBqlField { }
        #endregion

        #region SalesAmtTotal
        public abstract class salesAmtTotal : PX.Data.IBqlField
        {
        }
        protected decimal? _SalesAmtTotal;
        [PXDecimal(2)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "SalesAmtTotal", Enabled = false, Required = true)]
        [PXDBScalar(typeof(Search4<GVApGuiInvoiceDetail.salesAmt,
            Aggregate<GroupBy<GVApGuiInvoiceDetail.apRefNbr, Sum<GVApGuiInvoiceDetail.salesAmt>>>>))]
        public virtual decimal? SalesAmtTotal
        {
            get
            {
                return this._SalesAmtTotal;
            }
            set
            {
                this._SalesAmtTotal = value;
            }
        }
        #endregion

        #region UsrApGuiCmInvoiceNbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "ApGuiCmInvoiceNbr")]
        public virtual string UsrApGuiCmInvoiceNbr { get; set; }
        public abstract class usrApGuiCmInvoiceNbr : PX.Data.BQL.BqlString.Field<usrApGuiCmInvoiceNbr> { }
        #endregion
    }
}