using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGFinConnectSetup : IBqlTable
    {
        #region ConnectSite
        [PXDBString(1, IsKey = true, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Connect Site")]
        public virtual string ConnectSite { get; set; }
        public abstract class connectSite : PX.Data.BQL.BqlString.Field<connectSite> { }
        #endregion

        #region Host
        [PXDBString(120, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Host")]
        public virtual string Host { get; set; }
        public abstract class host : PX.Data.BQL.BqlString.Field<host> { }
        #endregion

        #region Sid
        [PXDBString(60, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Sid")]
        public virtual string Sid { get; set; }
        public abstract class sid : PX.Data.BQL.BqlString.Field<sid> { }
        #endregion

        #region Port
        [PXDBInt()]
        [PXUIField(DisplayName = "Port")]
        public virtual int? Port { get; set; }
        public abstract class port : PX.Data.BQL.BqlInt.Field<port> { }
        #endregion

        #region UserName
        [PXDBString(60, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "User Name")]
        public virtual string UserName { get; set; }
        public abstract class userName : PX.Data.BQL.BqlString.Field<userName> { }
        #endregion

        #region UserPwd
        [PXDBString(60, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "User Pwd")]
        public virtual string UserPwd { get; set; }
        public abstract class userPwd : PX.Data.BQL.BqlString.Field<userPwd> { }
        #endregion
    }
}