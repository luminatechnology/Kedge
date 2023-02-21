using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using Fin.DAC;

namespace Fin.Graph
{
    public class TWNWHTInquiry : PXGraph<TWNWHTInquiry>
    {
        #region Select & Features
        public PXSavePerRow<TWNWHTTran> Save;
        public PXCancel<TWNWHTTran> Cancel;

        [PXFilterable]
        public SelectFrom<TWNWHTTran>.View WHTTran;
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<TWNWHTTran> e)
        {
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.refNbr>  (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.batchNbr>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.tranDate>(e.Cache, e.Row, false);
        }
        #endregion
    }
}