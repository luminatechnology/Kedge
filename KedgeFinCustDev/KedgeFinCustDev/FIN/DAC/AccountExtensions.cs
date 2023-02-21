using PX.Data;

namespace PX.Objects.GL
{
    public class AccountExt : PXCacheExtension<PX.Objects.GL.Account>
    {
        #region UsrAccountGroup
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Account Category")]
        public virtual string UsrAccountGroup { get; set; }
        public abstract class usrAccountGroup : PX.Data.BQL.BqlString.Field<usrAccountGroup> { }
        #endregion
    }
}