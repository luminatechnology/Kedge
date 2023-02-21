using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.AP;

namespace Kedge
{
    /**
     *===2021/07/06 :口頭 === Althea
     *VoucehrL 加上orderby CD DESC/ ItemNO ASC
     *
     *===2021/11/09 : 0012265 === Althea
     *Modify View 改為用setReadOnly
     *
     *===2021/11/22 : 0012269 === Althea
     *Rowselected 塞CD金額時改為若為空值再塞
     **/
    public class KGVoucherMaint : PXGraph<KGVoucherMaint>
    {


        #region Select
        public PXSave<KGVoucherH> Save;

        //2021/11/09 Modify Mantis: 0012265 開放digest欄位修改(FIN)
        public PXSelectReadonly2<KGVoucherH,
            InnerJoin<APRegister,
                On<APRegister.refNbr,Equal<KGVoucherH.refNbr>>>> VoucherH;

        public PXSelect<KGVoucherL,
                  Where<KGVoucherL.voucherHeaderID,
                    Equal<Current<KGVoucherH.voucherHeaderID>>>,
                  OrderBy<Desc<KGVoucherL.cd,Asc<KGVoucherL.itemNo>>>> VoucherL;

        #endregion

        #region Event
        protected virtual void KGVoucherH_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGVoucherH row = VoucherH.Current;
            if (row == null) return;
            setUI(row);
            
             KGVoucherL CAmt = PXSelectGroupBy<KGVoucherL,
                Where<KGVoucherL.cd, Equal<word.c>,
                And<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>>>,
                Aggregate<GroupBy<KGVoucherL.voucherHeaderID, Sum<KGVoucherL.amt>>>>.
                Select(this, row.VoucherHeaderID);
            if (CAmt != null && (row.CAmt == null || row.CAmt == 0m)) row.CAmt = CAmt.Amt;
            
            KGVoucherL DAmt = PXSelectGroupBy<KGVoucherL,
                Where<KGVoucherL.cd, Equal<word.d>,
                And<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>>>,
                Aggregate<GroupBy<KGVoucherL.voucherHeaderID, Sum<KGVoucherL.amt>>>>.
                Select(this, row.VoucherHeaderID);
            if (DAmt != null && (row.DAmt == null || row.DAmt == 0m)) row.DAmt = DAmt.Amt;
            
        }
        #endregion

        #region Method
        private void setUI(KGVoucherH row)
        {
            PXUIFieldAttribute.SetReadOnly(VoucherL.Cache, null);
            VoucherL.AllowDelete = false;
            VoucherL.AllowInsert = false;
        }
        #endregion

    }
}