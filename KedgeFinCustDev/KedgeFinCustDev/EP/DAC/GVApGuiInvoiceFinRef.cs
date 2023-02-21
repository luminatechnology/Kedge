using PX.Data;
using PX.Objects.AP;
using static EP.Util.EPStringList;

namespace PX.Objects.EP
{
    public class GVApGuiInvoiceFinRef : GVApGuiInvoiceRef
    {
        #region GuiInvoiceNbr
        public new abstract class guiInvoiceNbr : PX.Data.IBqlField { }
        protected new string _GuiInvoiceNbr;
        [PXDBString(10, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "GuiInvoiceNbr")]
        public new virtual string GuiInvoiceNbr
        {
            get
            {
                return this._GuiInvoiceNbr;
            }
            set
            {
                this._GuiInvoiceNbr = value;
            }
        }
        #endregion

        #region EPRefNbr
        [PXDBString()]
        [PXUIField(DisplayName = "EP Ref. Nbr.", Visible = false)]
        [PXDBDefault(typeof(EPExpenseClaim.refNbr))]
        [PXParent(typeof(Select<EPExpenseClaim, Where<EPExpenseClaim.refNbr, Equal<Current<GVApGuiInvoiceFinRef.ePRefNbr>>>>))]
        public override string EPRefNbr { get; set; }
        //public abstract class ePRefNbr : PX.Data.BQL.BqlString.Field<ePRefNbr> { }
        #endregion

        #region RefNbr
        [PXDBString()]
        [PXUIField(DisplayName = "RefNbr")]
        public override string RefNbr { get; set; }
        #endregion

        #region GuiInvoiceID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(Visible = false)]
        public override int? GuiInvoiceID { get; set; }
        #endregion

        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        #region DocType
        [PXDBString(3, IsFixed = true)]
        [PXDBDefault(typeof(EPExpenseClaimExt.usrDocType))]
        [EPDocType()]
        [PXUIField(DisplayName = "Doc. Type", Visibility = PXUIVisibility.SelectorVisible)]
        public override string DocType { get; set; }
        #endregion
    }
}
