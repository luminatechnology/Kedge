using System;
using PX.Data;
using Kedge.DAC;
using System.Collections.Generic;
using static PX.Data.PXImportAttribute;
using System.Collections;
using PX.Objects.AP;
using PX.Objects.IN;

namespace Kedge
{
    /*======2021/03/05 0011966 Edit By Althea=====
     * 1. �쥻�W�Ǯ�,�ˬd��@�������Ӥ��s�b���~�N�|���@�ӿ��~�T��,
     *    �אּ�@���ˬd��������@�ӿ��~�T��.
     * 2. �u�ƽs���]�@��
     * 
     */
    public class KGVendorPriceEntry : PXGraph<KGVendorPriceEntry>, IPXPrepareItems
    {
        [PXImport(typeof(KGVendorPrice))]
        public PXSelect<KGVendorPrice,
            Where<KGVendorPrice.isDelete,NotEqual<True>,
                Or<KGVendorPrice.isDelete,IsNull>>,
            OrderBy<Asc<KGVendorPrice.inventoryID, 
            Asc<KGVendorPrice.areaCode, Desc<KGVendorPrice.effectiveDate>>>>> VendorPrices;
        public PXSave<KGVendorPrice> Save;
        public PXCancel<KGVendorPrice> Cancel;
        /*
        protected virtual void KGVendorPrice_RowInserting(PXCache sender, PXRowInsertingEventArgs e) {
            KGVendorPrice kgVendorPrice = (KGVendorPrice)e.Row;
            kgVendorPrice.VendorPriceCD = DateTime.Now.ToLongTimeString();
        }*/

        protected virtual void KGVendorPrice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGVendorPrice master = VendorPrices.Current;
            VendorPrices.AllowDelete = false;
        }

        #region Import
        bool preImport = true;
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            //2021/03/05 Mantis:0011966 By Althea
            List<string> VendorErrorMsgList = new List<string>();
            List<string> InventoryErrorMsgList = new List<string>();
            foreach (DictionaryEntry item in keys)
            {
                if (keys.Contains("VendorID"))
                {
                    string venderCD = keys["VendorID"].ToString();
                    Vendor vendor
                         = PXSelect<Vendor,
                        Where<Vendor.acctCD, Equal<Required<Vendor.acctCD>>>>
                        .Select(new PXGraph(), venderCD);
                    if (vendor == null)
                    {
                        VendorErrorMsgList.Add(venderCD);
                        //throw new Exception(String.Format("{0},�������Ӥ��s�b!", venderCD));
                    }

                }
                if (keys.Contains("InventoryID"))
                {
                    string InventoryCD = keys["InventoryID"].ToString();
                    InventoryItem inventoryItem
                         = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>
                        .Select(new PXGraph(), InventoryCD);
                    if (inventoryItem == null)
                    {
                        InventoryErrorMsgList.Add(InventoryCD);
                        //throw new Exception(String.Format("{0},���u�ƽs�����s�b!", InventoryCD));
                    }
                }
            }
            if(VendorErrorMsgList.Count != 0)
            {
                string Error = string.Join(",", VendorErrorMsgList);
                throw new Exception(String.Format("{0},�������Ӥ��s�b!", Error));
            }
            if(InventoryErrorMsgList.Count != 0)
            {
                string Error = string.Join(",", VendorErrorMsgList);
                throw new Exception(String.Format("{0},���u�ƽs�����s�b!", Error));
            }
            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return true;
        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return true;
        }

        public void PrepareItems(string viewName, IEnumerable items)
        {
        }
        #endregion

        #region Button
        #region ����
        public PXAction<KGVendorPrice> ProcessDelete;
        [PXButton(CommitChanges = true, Tooltip = "�i�Ŀ�h���R��")]
        [PXUIField( DisplayName = "�R��")]
        protected IEnumerable processDelete(PXAdapter adapter)
        {
            KGVendorPrice master = VendorPrices.Current;
            foreach (KGVendorPrice vendorPrice in VendorPrices.Cache.Cached)
            {
                if (vendorPrice.Selected == true)
                {
                    vendorPrice.IsDelete = true;
                }
                //VendorPrices.Update(vendorPrice);
            }
            return adapter.Get();
        }


        #endregion
        #endregion
        public override void Persist()
        {
            
           
            KGVendorPrice master = VendorPrices.Current;

            //�N��R��
            if (master == null)
            {
                base.Persist();
            }
            else
            {
                if (!beforeSaveCheck())
                {
                    return;
                }
                initValue();
                base.Persist();

            }
        }
        public void initValue() {
            Dictionary<String, KGVendorPrice> kgVendorPriceDic = new Dictionary<String, KGVendorPrice>();
            foreach (KGVendorPrice kgVendorPrice in VendorPrices.Select()) {
                string key = kgVendorPrice.InventoryID + "-" + kgVendorPrice.VendorID + "-" + kgVendorPrice.AreaCode + "-" + kgVendorPrice.Uom+ "-" + kgVendorPrice.Item;

                if (kgVendorPriceDic.ContainsKey(key)) {
                    KGVendorPrice newKgVendorPrice = kgVendorPriceDic[key];
                    kgVendorPriceDic[key] = kgVendorPrice;
                    if (kgVendorPrice.ExpirationDate == null)
                    {
                        //��@�Ѷ�^
                        DateTime day = ((DateTime)newKgVendorPrice.EffectiveDate).AddDays(-1);
                        kgVendorPrice.ExpirationDate = day;
                        VendorPrices.Update(kgVendorPrice);
                    }
                }
                else {
                    kgVendorPriceDic.Add(key, kgVendorPrice);
                }
            }
        }
        public bool beforeSaveCheck() {
            bool check = true;
            return check;
        }

    }
}