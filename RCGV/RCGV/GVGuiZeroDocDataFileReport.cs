using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace RCGV.GV
{
    public class GVGuiZeroDocDataFileReport : PXGraph<GVGuiZeroDocDataFileReport>
    {
        public  String fileName = AppDomain.CurrentDomain.BaseDirectory+"\\營業人零稅率銷售額資料檔.txt";

        public PXFilter<GVGuiZeroDocFilter> Filter;
        public PXAction<GVGuiZeroDocFilter> exporttxt;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Expoert TXT.")]
        public virtual IEnumerable Exporttxt(PXAdapter adapter)
        {


            GVGuiZeroDocFilter filter = Filter.Current;
            //throw new PXSetPropertyException(fileName);

            PXSelectBase <GVArGuiInvoice> set   = new PXSelectJoin<GVArGuiInvoice, InnerJoin<GVArGuiZeroDocLine,
                                    On<GVArGuiZeroDocLine.guiInvoiceNbr,
                                    Equal<GVArGuiInvoice.guiInvoiceNbr>>
                                    ,
                                    InnerJoin<GVArGuiZeroDoc,
                                    On<GVArGuiZeroDoc.zeroDocID,
                                    Equal<GVArGuiZeroDocLine.zeroDocID>>,
                                    InnerJoin<GVRegistration,
                                    On<GVRegistration.registrationCD,
                                    Equal<GVArGuiZeroDoc.registrationCD>>
                                    >>>,
                            Where<GVArGuiInvoice.declareYear,
                                Equal<Required<GVArGuiInvoice.declareYear>>,
                                And<GVArGuiInvoice.declareMonth, Equal<Required<GVArGuiInvoice.declareMonth>>>>>(this);

            //寫資料入檔案
            createFile();
            using (StreamWriter sw = new StreamWriter(@fileName))
            {
                //sw.WriteLine("123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789");
                foreach (PXResult<GVArGuiInvoice, GVArGuiZeroDocLine, GVArGuiZeroDoc, GVRegistration> record in set.Select(filter.DeclareYear, filter.DeclareMonth))
                {
                    
                    GVArGuiInvoice gvArGuiInvoice = record;
                    GVArGuiZeroDocLine gvArGuiZeroDocLine = record;
                    GVArGuiZeroDoc gvArGuiZeroDoc = record;
                    GVRegistration gvRegistration = record;
                    //throw new PXSetPropertyException(gvArGuiInvoice.GuiInvoiceID + "-" + gvArGuiZeroDocLine.ZeroDocLineID + "-" + gvArGuiZeroDoc.ZeroDocID);
                    sw.WriteLine(getZeroDocData(gvArGuiInvoice, gvArGuiZeroDocLine, gvArGuiZeroDoc, gvRegistration));
                }
            }
            downLoadFile();
            return adapter.Get();
        }
        public String getZeroDocData(GVArGuiInvoice gvArGuiInvoice, GVArGuiZeroDocLine gvArGuiZeroDocLine, GVArGuiZeroDoc gvArGuiZeroDoc, GVRegistration gvRegistration) {
            //銷售人統一編號1~8
            String GovUniformNumber_1_8 = String.Format("{0:00000000}", gvRegistration.GovUniformNumber);
            String City9_9 = gvRegistration.TaxCityCode;
            String GovUniformNumber_10_18 = String.Format("{0:000000000}", gvRegistration.RegistrationCD);
            String BelongYearMonth_19_23 = "" + String.Format("{0:000}", (gvArGuiInvoice.DeclareYear - 1911)) + String.Format("{0:00}", gvArGuiInvoice.DeclareMonth);
            String BelongYearMonth_24_28 = null;
            if (gvArGuiZeroDocLine.InvoiceDate == null)
            {
                BelongYearMonth_24_28 = "     ";
            }
            else {
                DateTime InvoiceDate = (DateTime)gvArGuiZeroDocLine.InvoiceDate;
                BelongYearMonth_24_28 = "" + String.Format("{0:000}", (InvoiceDate.Year - 1911)) + String.Format("{0:00}", InvoiceDate.Month);
            }

            String GuiInvoiceNbr_29_38 = null;
            if (gvArGuiZeroDocLine.GuiInvoiceNbr == null)
            {
                GuiInvoiceNbr_29_38 = "          ";
            }
            else {
                GuiInvoiceNbr_29_38 = String.Format("{0:0000000000}", gvArGuiZeroDocLine.GuiInvoiceNbr);
            }

            String CustUniformNumber_39_46 = null;
            if (gvArGuiInvoice.CustUniformNumber == null)
            {
                CustUniformNumber_39_46 = "        ";
            }
            else {
                CustUniformNumber_39_46 = String.Format("{0:00000000}", Convert.ToInt32(gvArGuiInvoice.CustUniformNumber));
            }
            String ZeroTaxrFomat_47_47 = gvArGuiZeroDoc.SalesType;
            String DataType_48_48 = gvArGuiZeroDoc.DataType;
            String DocTypeCode_49_50 = gvArGuiZeroDoc.DocTypeCode;
            String DocNbr_51_64 = String.Format("{0:00000000000000}", Convert.ToInt32(gvArGuiZeroDoc.DocNbr));
            String SalesAmt_65_76 = String.Format("{0:000000000000}", gvArGuiZeroDoc.SalesAmt);
            DateTime DocDate = (DateTime)gvArGuiZeroDoc.DocDate;
            String OutputDate_77_83 = String.Format("{0:000}", (DocDate.Year - 1911)) + String.Format("{0:00}", DocDate.Month)+ String.Format("{0:00}", DocDate.Day);
            //輸出或結匯 docDate
            return GovUniformNumber_1_8 + City9_9 + GovUniformNumber_10_18 + BelongYearMonth_19_23 +
                BelongYearMonth_24_28 + GuiInvoiceNbr_29_38 +CustUniformNumber_39_46+ ZeroTaxrFomat_47_47+ DataType_48_48+
                DocTypeCode_49_50 + DocNbr_51_64+ SalesAmt_65_76+ OutputDate_77_83;
        }

        public void createFile() {
            //建立一個空白檔案
            FileStream fileStream = new FileStream(@fileName, FileMode.Create);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
        }
        public void downLoadFile() {
            //開檔案
            FileStream fileStream1 = new FileStream(@fileName, FileMode.Open);
            byte[] bytes1 = new byte[fileStream1.Length];
            //算位元
            using (BinaryReader br = new BinaryReader(fileStream1))
            {
                bytes1 = br.ReadBytes((int)fileStream1.Length);
            }
            fileStream1.Close();
            throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(),
                                                   @fileName,
                                                   null,
                                                   bytes1), true);
        }


        [Serializable]
        public class GVGuiZeroDocFilter : IBqlTable
        {
            #region DeclareYear
            public abstract class declareYear : PX.Data.IBqlField
            {
            }
            protected int? _DeclareYear;
            [PXInt()]
            [PXUIField(DisplayName = "DeclareYear")]
            public virtual int? DeclareYear
            {
                get
                {
                    return this._DeclareYear;
                }
                set
                {
                    this._DeclareYear = value;
                }
            }
            #endregion
            #region DeclareMonth
            public abstract class declareMonth : PX.Data.IBqlField
            {
            }
            protected int? _DeclareMonth;
            [PXInt()]
            [PXUIField(DisplayName = "DeclareMonth")]
            public virtual int? DeclareMonth
            {
                get
                {
                    return this._DeclareMonth;
                }
                set
                {
                    this._DeclareMonth = value;
                }
            }
            #endregion
        }
    }
}