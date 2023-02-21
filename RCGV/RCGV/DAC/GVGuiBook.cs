using System;
using PX.Data;
using PX.Objects.TX;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.SO;
using RCGV.GV.Util;
using RCGV.GV.Attribute.Selector;
using System.Collections;

namespace RCGV.GV.DAC
{

	[Serializable]
	public class GVGuiBook : IBqlTable
	{
		#region GuiBookID
		public abstract class guiBookID : PX.Data.IBqlField
		{
		}
		protected int? _GuiBookID;
		[PXDBIdentity]
		public virtual int? GuiBookID
		{
			get
			{
				return this._GuiBookID;
			}
			set
			{
				this._GuiBookID = value;
			}
		}
		#endregion
		#region GuiBookCD
		public abstract class guiBookCD : PX.Data.IBqlField
		{
		}
		protected string _GuiBookCD;
		[PXDBString(240, IsKey = true, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "GuiBook ID", Required = true)]
		[PXSelector(
			typeof(Search<GVGuiBook.guiBookCD>),
			typeof(GVGuiBook.guiBookCD),
			typeof(GVGuiBook.guiBookDesc),
			typeof(GVGuiBook.guiWordCD),
			DescriptionField=typeof(GVGuiBook.guiBookDesc)
		)]
		public virtual string GuiBookCD
		{
			get
			{
				return this._GuiBookCD;
			}
			set
			{
				this._GuiBookCD = value;
			}
		}
		#endregion
		#region RegistrationCD
		public abstract class registrationCD : PX.Data.IBqlField
		{
		}
        protected string _RegistrationCD;
        [PXDBString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Registration CD", Required = true)]
        [RegistrationCDAttribute()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string RegistrationCD
		{
			get
			{
				return this._RegistrationCD;
			}
			set
			{
				this._RegistrationCD = value;
			}
		}
		#endregion
		#region GovUniformNumber
		public abstract class govUniformNumber : PX.Data.IBqlField
		{
		}
		protected string _GovUniformNumber;
		[PXDBString(10)]
		[PXUIField(DisplayName = "Gov Uniform Number", IsReadOnly = true)]
		public virtual string GovUniformNumber
		{
			get
			{
				return this._GovUniformNumber;
			}
			set
			{
				this._GovUniformNumber = value;
			}
		}
        #endregion
        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual Boolean? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion
        #region GuiNumberDetailID
        public abstract class guiNumberDetailID : PX.Data.IBqlField
		{
		}
		protected int? _GuiNumberDetailID;
		[PXDBInt()]
		[PXDefault()]
		[PXUIField(DisplayName = "Gui Word", Required = true)]
        /*
            [PXSelector(
			    typeof(Search2<GVGuiNumberDetail.guiNumberDetailID,
				    InnerJoin<GVGuiNumber, On<GVGuiNumberDetail.guiNumberID, Equal<GVGuiNumberDetail.guiNumberID>>>,
				    Where<GVGuiNumber.registrationCD, Equal<Current<GVGuiBook.registrationCD>>, 
                        And<GVGuiNumber.declareYear, Equal<Current<GVGuiBook.declareYear>>>>, 
                    OrderBy<Desc<GVGuiNumber.declareYear, Desc<GVGuiNumber.declarePeriod, 
                        Asc<GVGuiNumberDetail.guiWordCD, Desc<GVGuiNumberDetail.guiNumberDetailID>>>>>>),
				    new Type[] {
					    typeof(GVGuiNumber.declareYear),
					    typeof(GVGuiNumber.declarePeriod),
					    typeof(GVGuiNumberDetail.guiWordCD),
                        typeof(GVGuiNumberDetail.guiNumberDetailID),
                        typeof(GVGuiNumberDetail.startNumber),
                        typeof(GVGuiNumberDetail.endNumber),
				    },
				DescriptionField = typeof(GVGuiNumberDetail.guiWordCD)
		)]*/

         
        [PXSelector(
			typeof(Search2<GVGuiNumberDetail.guiNumberDetailID,
				InnerJoin<GVGuiNumber, On<GVGuiNumberDetail.guiNumberID, Equal<GVGuiNumber.guiNumberID>>>,
				Where<GVGuiNumber.registrationCD, Equal<Current<GVGuiBook.registrationCD>>, 
                    And<GVGuiNumber.declareYear, Equal<Current<GVGuiBook.declareYear>>,
                        And<Where<GVGuiNumber.hold,IsNull,Or<GVGuiNumber.hold,NotEqual<True>>>>>>, 
                OrderBy<Desc<GVGuiNumber.declareYear, Desc<GVGuiNumber.declarePeriod, 
                    Asc<GVGuiNumberDetail.guiWordCD, Desc<GVGuiNumberDetail.guiNumberDetailID>>>>>>),
				new Type[] {
					typeof(GVGuiNumber.declareYear),
					typeof(GVGuiNumber.declarePeriod),
					typeof(GVGuiNumberDetail.guiWordCD),
                    typeof(GVGuiNumberDetail.guiNumberDetailID),
                    typeof(GVGuiNumberDetail.startNumber),
                    typeof(GVGuiNumberDetail.endNumber),
				},
				DescriptionField = typeof(GVGuiNumberDetail.guiWordCD)
		)]
		public virtual int? GuiNumberDetailID
		{
			get
			{
				return this._GuiNumberDetailID;
			}
			set
			{
				this._GuiNumberDetailID = value;
			}
		}
		#endregion
		#region GuiWordCD
		public abstract class guiWordCD : PX.Data.IBqlField
		{
		}
		protected string _GuiWordCD;
		[PXDBString(2)]
		[PXUIField(DisplayName = "Gui Word ID", Required = true, IsReadOnly = true)]
		public virtual string GuiWordCD
		{
			get
			{
				return this._GuiWordCD;
			}
			set
			{
				this._GuiWordCD = value;
			}
		}
		#endregion
		#region GuiNumberRange
		public abstract class guiNumberRange : PX.Data.IBqlField
		{
		}
		protected string _GuiNumberRange;
		[PXString()]
		[PXUIField(DisplayName = "Number Range", IsReadOnly = true)]
		public virtual string GuiNumberRange
		{
			get
			{
				return this._GuiNumberRange;
			}
			set
			{
				this._GuiNumberRange = value;
			}
		}
		#endregion
		#region DeclareYear
		public abstract class declareYear : PX.Data.IBqlField
		{
		}
		protected int? _DeclareYear;
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Declare Year", Required = true, Visibility = PXUIVisibility.Visible)]
		public virtual int? DeclareYear
		{
			get { return this._DeclareYear; }
			set { this._DeclareYear = value; }
		}
        #endregion
        #region DeclarePeriod
        public abstract class declarePeriod : PX.Data.IBqlField
        {
        }
        protected string _DeclarePeriod;
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Declare Period", Required = true, IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVList.GVDeclarePeriod]
        public virtual string DeclarePeriod
        {
            get
            {
                return this._DeclarePeriod;
            }
            set
            {
                this._DeclarePeriod = value;
            }
        }
        #endregion
        #region GuiType
        public abstract class guiType : PX.Data.IBqlField
		{
		}
		protected String _GuiType;
		[PXDBString(2)]
		[PXUIField(DisplayName = "Gui Type", Visibility = PXUIVisibility.Visible, Required = true)]
	    [GVList.GVGuiType.ARInvoice]
		public virtual String GuiType
		{
			get
			{
				return this._GuiType;
			}
			set
			{
				this._GuiType = value;
			}
		}
		#endregion
		#region GuiBookDesc
		public abstract class guiBookDesc : PX.Data.IBqlField
		{
		}
		protected string _GuiBookDesc;
		[PXDBString(IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Gui Book Desc")]

		public virtual string GuiBookDesc
		{
			get
			{
				return this._GuiBookDesc;
			}
			set
			{
				this._GuiBookDesc = value;
			}
		}
		#endregion
		#region StartNum
		public abstract class startNum : PX.Data.IBqlField
		{
		}
		protected string _StartNum;
		[PXDBString(8, InputMask = "########")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Start Number", IsReadOnly = true)]
		public virtual string StartNum
		{
			get
			{
				return this._StartNum;
			}
			set
			{
				this._StartNum = value;
			}
		}
		#endregion
		#region EndNum
		public abstract class endNum : PX.Data.IBqlField
		{
		}
		protected string _EndNum;
		[PXDBString(8, InputMask = "########")]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "End Number", Required = true)]
		public virtual string EndNum
		{
			get
			{
				return this._EndNum;
			}
			set
			{
				this._EndNum = value;
			}
		}
		#endregion
		#region CurrentNum
		public abstract class currentNum : PX.Data.IBqlField
		{
		}
		protected string _CurrentNum;
		[PXDBString(8, InputMask = "########")]
		[PXUIField(DisplayName = "Current Number", IsReadOnly = true)]
		public virtual string CurrentNum
		{
			get
			{
				return this._CurrentNum;
			}
			set
			{
				this._CurrentNum = value;
			}
		}
		#endregion
		#region RemainCount
		public abstract class remainCount : PX.Data.IBqlField
		{
		}
		protected int? _RemainCount;
		[PXInt]
		[PXUIField(DisplayName = "Remain Count", IsReadOnly = true)]
		public virtual int? RemainCount
		{
			get
			{
				return this._RemainCount;
			}
			set
			{
				this._RemainCount = value;
			}
		}
		#endregion
		#region CurrentGuiDate
		public abstract class currentGuiDate : IBqlField { }
		[PXDBDate]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Current Gui Date", IsReadOnly = true)]
		public virtual DateTime? CurrentGuiDate { get; set; }
		#endregion
		#region IsLock
		public abstract class isLock : PX.Data.IBqlField
		{
		}
		protected bool? _IsLock;
		[PXDBBool()]
        [PXDefault(false)]
		[PXUIField(DisplayName = "IsLock")]
		public virtual bool? IsLock
		{
			get
			{
				return this._IsLock;
			}
			set
			{
				this._IsLock = value;
			}
		}
		#endregion
		#region LockBy
		[PXDBGuid]
		public virtual Guid? LockBy { get; set; }
		public abstract class lockBy : IBqlField { }
		#endregion
		#region MaxEndNum
		public abstract class maxEndNum : PX.Data.IBqlField
		{
		}
		protected string _MaxEndNum;
		[PXString(8)]
		[PXUIField(DisplayName = "Max End Number", Visible = false)]
		public virtual string MaxEndNum
		{
			get
			{
				return this._MaxEndNum;
			}
			set
			{
				this._MaxEndNum = value;
			}
		}
        #endregion
        #region StartMonth
        public abstract class startMonth : PX.Data.IBqlField
        {
        }
        protected int? _StartMonth;
        [PXDBInt()]
        [PXUIField(DisplayName = "StartMonth", Required = true)]
        public virtual int? StartMonth
        {
            get
            {
                return this._StartMonth;
            }
            set
            {
                this._StartMonth = value;
            }
        }
        #endregion
        #region EndMonth
        public abstract class endMonth : PX.Data.IBqlField
        {
        }
        protected int? _EndMonth;
        [PXDBInt()]
        [PXUIField(DisplayName = "End Month", Required = true)]
        public virtual int? EndMonth
        {
            get
            {
                return this._EndMonth;
            }
            set
            {
                this._EndMonth = value;
            }
        }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }
		public abstract class createdByID : IBqlField { }
		#endregion
		#region CreatedByScreenID
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID { get; set; }
		public abstract class createdByScreenID : IBqlField { }
		#endregion
		#region CreatedDateTime
		//[PXDBDate()]
		//[PXUIField(DisplayName = "Created Date Time")]
		//public virtual DateTime? CreatedDateTime { get; set; }
		//public abstract class createdDateTime : IBqlField { }
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		public abstract class lastModifiedByID : IBqlField { }
		#endregion
		#region LastModifiedByScreenID
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }
		public abstract class lastModifiedByScreenID : IBqlField { }
		#endregion
		#region LastModifiedDateTime

		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#region NoteID

		public abstract class noteID : IBqlField { }
		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
		#region tstamp

		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
	}
}
