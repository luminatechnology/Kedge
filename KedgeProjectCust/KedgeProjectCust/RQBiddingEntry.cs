using PX.Data;

namespace PX.Objects.RQ
{
    public class RQBiddingEntry_Extension : PXGraphExtension<RQBiddingEntry>
    {
        /*protected void RQBiddingVendor_ReqNbr_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
          {
              foreach (RQRequisitionLineBidding line in Base.Lines.Select()) 
              {
                  bool change = false;
                  if (line.MinQty == null) {
                      change = true;
                      line.MinQty = line.OrderQty;
                  }
                  if (line.QuoteQty == null) {
                      change = true;
                      line.QuoteQty = line.OrderQty;
                  }
                  if (change) {
                      Base.Lines.Update(line);
                  }
              }
          }*/
        /*
        protected void RQRequisitionLineBidding_RowPersisting(PXCache cache, PXRowPersistingEventArgs e, PXRowPersisting InvokeBaseHandler)
        {           
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(cache, e);
            var row = (RQRequisitionLineBidding)e.Row;

        }*/
        /*
        protected virtual void RQRequisitionLineBidding_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            RQRequisitionLineBidding line = (RQRequisitionLineBidding)e.Row;
            String a = "1";
            e.Cancel = false;
        }*/
        //protected virtual void RQRequisitionLineBidding_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        //{           
        //    RQRequisitionLineBidding line = (RQRequisitionLineBidding)e.Row;
        //    //bool change = false;
        //    if (line.MinQty == null)
        //    {
        //       //change = true;
        //        line.MinQty = line.OrderQty;
        //    }
        //    if (line.QuoteQty == null)
        //    {
        //        //change = true;
        //        line.QuoteQty = line.OrderQty;
        //    }

        //    //Base.Lines.Cache.IsDirty = true;
        //    /*
        //    if (change)
        //    {
        //        Base.Lines.Cache.Update(line);
        //    }*/
        //}

        /// <summary>
        /// Using delegate method to bring line default value with the requirement.
        /// </summary>
        /// <param name="rqLineBidding"></param>
        /// <param name="bidding"></param>
        public delegate void FillRequisitionLineBiddingPropertiesInViewDelegateDelegate(RQRequisitionLineBidding rqLineBidding, RQBidding bidding);
        [PXOverride]
        public void FillRequisitionLineBiddingPropertiesInViewDelegate(RQRequisitionLineBidding rqLineBidding, RQBidding bidding, 
                                                                       FillRequisitionLineBiddingPropertiesInViewDelegateDelegate baseMethod)
        {
            baseMethod(rqLineBidding, bidding);

            rqLineBidding.QuoteQty = rqLineBidding.OrderQty;
        }
    }
}