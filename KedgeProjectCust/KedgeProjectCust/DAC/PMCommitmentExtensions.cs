using PX.Data;
using PX.Objects.CR;
using PX.Objects.PO;

namespace PX.Objects.PM
{
    public class PMCommitmentExt : PXCacheExtension<PX.Objects.PM.PMCommitment>
    {
        #region UsrVendShortName
        [PXString(255)]
        [PXUIField(DisplayName = "Vendor Name")]
        [PXDBScalar(typeof(Search2<Contact.fullName,
                                   InnerJoin<POOrder, On<POOrder.vendorID, Equal<Contact.bAccountID>,
                                             And<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>>,
                                   Where<POOrder.noteID, Equal<PMCommitment.refNoteID>>>))]
        public virtual string UsrVendShortName { get; set; }
        public abstract class usrVendShortName : PX.Data.BQL.BqlString.Field<usrVendShortName> { }
        #endregion
    }
}