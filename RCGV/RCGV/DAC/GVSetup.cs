﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	using RCGV.GV.Descriptor;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(GVSetupMaint))]
	[PXCacheName("Preferences")]
	public class GVSetup : PX.Data.IBqlTable
	{
		#region ZeroDocCDNbr
		public abstract class zeroDocCDNbr : PX.Data.IBqlField
		{
		}
		protected string _ZeroDocCDNbr;
		[PXDBString(9, IsUnicode = true)]
		[PXDefault("000000000")]
		[PXUIField(DisplayName = "ZeroDoc CD Nbr")]
		public virtual string ZeroDocCDNbr
		{
			get
			{
				return this._ZeroDocCDNbr;
			}
			set
			{
				this._ZeroDocCDNbr = value;
			}
		}
		#endregion
		#region GuiCmInvoiceNbr
		public abstract class guiCmInvoiceNbr : PX.Data.IBqlField
		{
		}
		protected string _GuiCmInvoiceNbr;
		[PXDBString(9, IsUnicode = true)]
		[PXDefault("000000000")]
		[PXUIField(DisplayName = "Gui Cm Invoice Nbr")]
		public virtual string GuiCMInvoiceNbr
		{
			get
			{
				return this._GuiCmInvoiceNbr;
			}
			set
			{
				this._GuiCmInvoiceNbr = value;
			}
		}
        #endregion
        #region GuiApCmInvoiceNbr
        public abstract class guiApCmInvoiceNbr : PX.Data.IBqlField
        {
        }
        protected string _GuiApCmInvoiceNbr;
        [PXDBString(8, IsUnicode = true)]
        [PXDefault("00000000")]
        [PXUIField(DisplayName = "Gui AP Cm Invoice Nbr")]
        public virtual string GuiApCmInvoiceNbr
        {
            get
            {
                return this._GuiApCmInvoiceNbr;
            }
            set
            {
                this._GuiApCmInvoiceNbr = value;
            }
        }
        #endregion
        #region AutoNumbering
        public abstract class autoNumbering : PX.Data.IBqlField
		{
		}
		protected bool? _AutoNumbering;
		[PXDBBool()]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Auto Numbering")]
		public virtual bool? AutoNumbering
		{
			get
			{
				return this._AutoNumbering;
			}
			set
			{
				this._AutoNumbering = value;
			}
		}
		#endregion
	}
}
