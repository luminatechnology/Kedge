using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using RCGV.GV.SYS;
namespace RCGV.GV
{
	public class GVGuiNumberMaint : GVBaseGraph<GVGuiNumberMaint, GVGuiNumber>
	{

        public PXSelect<GVGuiNumber> CodeMaster;
		[PXViewName("CodeDetails")]
		public PXSelect<GVGuiNumberDetail,
							Where<GVGuiNumberDetail.guiNumberID,
							Equal<Optional<GVGuiNumber.guiNumberID>>>> CodeDetails;

		public PXSelect<GVGuiWord> CheckGVGuiWord;
        //public new PXDelete<GVGuiNumber> Delete;
        /*
               protected virtual void GVGuiNumber_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
               {
                   List<int> list = new List<int>();

                   if (CodeDetails.Select().Count > 0)
                   {

                       foreach (GVGuiNumberDetail  detail in    CodeDetails.Select()) {
                           if (detail.GuiNumberDetailID != null) {
                               list.Add((int)detail.GuiNumberDetailID);
                           }
                       }
                       PXResultset<GVGuiBook> setNumber = PXSelect<GVGuiBook,
                                  Where<GVGuiBook.guiNumberDetailID,
                                      In<Required<GVGuiBook.guiNumberDetailID>>>>
                                          .Select(this, list.ToArray());
                       int a = setNumber.Count;
                   }


               }*/
        /*
        public PXAction<GVGuiNumber> InsertRow;
        [PXButton]
        [PXUIField(DisplayName = "Insert",Enabled =true)]
        public void insertRow()
        {
            CodeDetails.Insert();
        }*/
        public PXAction<GVGuiNumber> insertRow;
        [PXUIField(DisplayName = "Insert", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable InsertRow(PXAdapter adapter)
        {
            CodeDetails.Insert();
            return adapter.Get();
        }

        protected virtual void GVGuiNumber_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            //if (!e.ExternalCall) return;
            GVGuiNumber row = (GVGuiNumber)e.Row;
            if (CodeMaster.Current == null)
            {
                return;
            }
            if (row.Hold == true)
            {
                PXUIFieldAttribute.SetEnabled(CodeMaster.Cache, CodeMaster.Current, true);
                //PXUIFieldAttribute.SetEnabled<GVGuiNumber.hold>(CodeMaster.Cache, CodeMaster.Current, true);
                CodeDetails.AllowInsert = CodeDetails.AllowUpdate = CodeDetails.AllowDelete = true;
                //CodeMaster.AllowDelete = true;
                Delete.SetEnabled(true);
                if (row.GuiNumberID < 0)
                {
                    //row.DeclareYear = DateTime.Now.Year;
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.declareYear>(sender, row, true);
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.guiWordID>(sender, row, true);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.declareYear>(sender, row, false);
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.guiWordID>(sender, row, false);

                }
                PXEntryStatus status = CodeMaster.Cache.GetStatus(row);
                //Hold啟用後暫時不用了
                //enableAll(row);
                if (CodeDetails.Select().Count > 0)
                {
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.applyStartNumber>(CodeMaster.Cache, CodeMaster.Current, false);
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.applyEndNumber>(CodeMaster.Cache, CodeMaster.Current, false);
                }
                else
                {

                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.applyStartNumber>(CodeMaster.Cache, CodeMaster.Current, true);
                    PXUIFieldAttribute.SetEnabled<GVGuiNumber.applyEndNumber>(CodeMaster.Cache, CodeMaster.Current, true);

                }


                //調整插入功能
                //Check Data .If true Can be Insert.
                String applyStartNumber = CodeMaster.Current.ApplyStartNumber;
                String applyEndNumber = CodeMaster.Current.ApplyEndNumber;
                //bool check = true;
                bool disable = !(applyStartNumber == null || applyEndNumber == null) && check50DistanceStr(applyStartNumber, applyEndNumber)
                            && checkAllLine();
                CodeDetails.AllowInsert = disable;
                insertRow.SetEnabled(disable);
                
            }
            else {

                PXUIFieldAttribute.SetEnabled(CodeMaster.Cache, CodeMaster.Current, false);
                //CodeMaster.AllowDelete = false;
                Delete.SetEnabled(false);
                bool isUse = checkIsUse();
                PXUIFieldAttribute.SetEnabled<GVGuiNumber.hold>(CodeMaster.Cache, CodeMaster.Current, !isUse);
                CodeDetails.AllowInsert = CodeDetails.AllowUpdate = CodeDetails.AllowDelete = false;
                CodeDetails.AllowInsert = false;
                insertRow.SetEnabled(false);
            }
        }
        public bool checkIsUse() {

            bool? isUse = CodeMaster.Current.IsUse;
            if (CodeMaster.Current.GuiNumberID < 0)
            {
                return false;
            }

            if (isUse != null)
            {
                return isUse.Value;

            }
            else {

                if (CodeMaster.Current.GuiNumberID > 0 && CodeDetails.Select().Count > 0)
                {
                    List<int> list = new List<int>();
                    foreach (GVGuiNumberDetail detail in CodeDetails.Select())
                    {
                        if (detail.GuiNumberDetailID != null)
                        {
                            list.Add((int)detail.GuiNumberDetailID);
                        }
                    }
                    PXResultset<GVGuiBook> setNumber = PXSelect<GVGuiBook,
                               Where<GVGuiBook.guiNumberDetailID,
                                   In<Required<GVGuiBook.guiNumberDetailID>>>>
                                       .Select(this, list.ToArray());
                    int a = setNumber.Count;
                    if (a > 0)
                    {
                        isUse = true;
                    }
                    else
                    {
                        isUse = false;
                    }
                    CodeMaster.Current.IsUse = isUse;
                    return isUse.Value;
                }
                else {
                    CodeMaster.Current.IsUse = false;
                    return false;
                }
                
            }
            return false;

        }


        private Boolean checkAllLineEnable() {
            Boolean anyProblem = false;
            foreach (GVGuiNumberDetail det in CodeDetails.Select())
            {
                if (checkLineEX(det) == false) anyProblem = true;
            }
            return anyProblem;
        }
        private void enableAll(GVGuiNumber row) {
            PXEntryStatus status = CodeMaster.Cache.GetStatus(row);
            bool enable = true;
            if (!(status == PXEntryStatus.Inserted))
            {
                if (row.GuiNumberID > 0 && CodeDetails.Select().Count > 0)
                {
                    List<int> list = new List<int>();
                    foreach (GVGuiNumberDetail detail in CodeDetails.Select())
                    {
                        if (detail.GuiNumberDetailID != null)
                        {
                            list.Add((int)detail.GuiNumberDetailID);
                        }
                    }
                    PXResultset<GVGuiBook> setNumber = PXSelect<GVGuiBook,
                               Where<GVGuiBook.guiNumberDetailID,
                                   In<Required<GVGuiBook.guiNumberDetailID>>>>
                                       .Select(this, list.ToArray());
                    int a = setNumber.Count;
                    if (a > 0)
                    {
                        enable = false;
                    }
                    else
                    {
                        enable = true;
                    }
                }
            }
            else
            {
                enable = true;
            }
            CodeDetails.AllowInsert = enable;
            if (enable)
            {
                //Check Data .If true Can be Insert.
                String applyStartNumber = CodeMaster.Current.ApplyStartNumber;
                String applyEndNumber = CodeMaster.Current.ApplyEndNumber;
                //bool check = true;
                bool disable = !(applyStartNumber == null || applyEndNumber == null) && check50DistanceStr(applyStartNumber, applyEndNumber)
                            && checkAllLine();
                CodeDetails.AllowInsert = disable;
                insertRow.SetEnabled(disable);
            }

            CodeMaster.AllowUpdate = CodeMaster.AllowDelete = enable;
            CodeDetails.AllowUpdate = CodeDetails.AllowDelete = enable;

        }

        protected virtual void GVGuiNumber_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            GVGuiNumber row = (GVGuiNumber)e.Row;
            if (CodeDetails.Select().Count > 0)
            {
                throw new PXSetPropertyException("GuiNumber Detail is existed, can not be deleted!", PXErrorLevel.Error);
            }
          
        }
        protected virtual void GVGuiNumber_GuiWordID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVGuiNumber row = (GVGuiNumber)e.Row;
            GVGuiWord gvGuiWord = PXSelectorAttribute.Select<GVGuiNumber.guiWordID>(sender, row) as GVGuiWord;
            row.DeclarePeriod = gvGuiWord.DeclarePeriod;

        }
        protected virtual void GVGuiNumberDetail_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            GVGuiNumberDetail row = (GVGuiNumberDetail)e.Row;

            // Checking whether the deletion has been initiated from the UI
            if (!e.ExternalCall) return;
            GVGuiNumberDetail data = row;
            if (CodeDetails.Select().Count > 0)
            {
                data = CodeDetails.Select()[CodeDetails.Select().Count - 1];
            }
            //Last One Record -Y
            if (row.LineNbr >= data.LineNbr)
            {
                //Is row in the DataBase?
                if (sender.GetStatus(row) == PXEntryStatus.InsertedDeleted)
                {
                    if (CodeDetails.Ask("Confirm Delete", "Are you sure?", MessageButtons.YesNo) != WebDialogResult.Yes)
                    {
                        CodeDetails.Cache.IsDirty = true;
                        e.Cancel = true;
                    }
                }
                else
                {
                    PXResultset<GVGuiBook> setNumber = PXSelect<GVGuiBook,
                            Where<GVGuiBook.guiNumberDetailID,
                                Equal<Required<GVGuiBook.guiNumberDetailID>>>>
                                    .Select(this, row.GuiNumberDetailID);
                    //IS GUINumberDtailID is  aleady  used by Guibook ?
                    if (setNumber.Count > 0)
                    {
                        CodeDetails.Ask("ERROR", "GUINumberDtailID is  aleady  used by Guibook  , can not be deleted!", MessageButtons.OK);
                        CodeDetails.Cache.IsDirty = true;
                        e.Cancel = true;
                    }
                    else
                    {
                        if (CodeDetails.Ask("Confirm Delete", "Are you sure?", MessageButtons.YesNo) != WebDialogResult.Yes)
                        {
                            CodeDetails.Cache.IsDirty = true;
                            e.Cancel = true;
                        }
                    }
                }
            }
            //Last One Record -N
            else
            {
                CodeDetails.Ask("ERROR", "This is not last one  , can not be deleted!", MessageButtons.OK);
                e.Cancel = true;
            }
            
            

        }
        protected virtual void GVGuiNumberDetail_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			GVGuiNumberDetail line = (GVGuiNumberDetail)e.Row;
			if (CodeDetails.Select().Count == 0)
			{
				line.StartNumber = CodeMaster.Current.ApplyStartNumber;
			}
			else {
                string maxNumber = CodeDetails.Select().RowCast<GVGuiNumberDetail>().ToList().Max(d => d.EndNumber);
                //檢查最後的Line是否大於Master的End Number
				//bool check= checkLineGuinumber(data);
				//line.LineID = data.LineID + 1;
				if (maxNumber != null)
				{
					int endNum = int.Parse(maxNumber);
					endNum = endNum + 1;
					String endNumStr = Convert.ToString(endNum);
					endNumStr = endNumStr.PadLeft(8, '0');
					//input
					line.StartNumber = endNumStr;
				}
				else {

				}
			}
          
		}
		protected virtual void GVGuiNumber_ApplyStartNumber_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVGuiNumber row = (GVGuiNumber)e.Row;
			if (row == null || e.NewValue == null)
			{
				return;
			}
            ckeckApplyStartNumber_M(sender, row, (String)e.NewValue);
		}
        public     String err50Message = "End Number have to bigger than StartNumber 50. And EndNumber and StarNumber is Multiple of 50";
        public bool ckeckApplyStartNumber_M(PXCache sender, GVGuiNumber row, String newValue)
		{
			String applyStartNumber = newValue;
			String applyEndNumber = row.ApplyEndNumber;
			if (newValue == null)
			{

					sender.RaiseExceptionHandling<GVGuiNumber.applyStartNumber>(
									row, newValue,
											new PXSetPropertyException("ApplyStartNumber can't Null.", PXErrorLevel.Error));

				return true;
			}
			if (applyEndNumber == null)
			{
				return true;
			}
            int startNumber = int.Parse(applyStartNumber);
			int endNumber = int.Parse(applyEndNumber);
            if (!check50Distance(startNumber ,endNumber))
			{
               

                sender.RaiseExceptionHandling<GVGuiNumber.applyStartNumber>(
											CodeMaster.Current, applyStartNumber,
													new PXSetPropertyException(err50Message, PXErrorLevel.Error));
					sender.RaiseExceptionHandling<GVGuiNumber.applyEndNumber>(
										CodeMaster.Current, applyEndNumber,
												new PXSetPropertyException(err50Message, PXErrorLevel.Error));
				return true;
			}
            if (checkIsExist(row.GuiNumberID, row.GuiWordID, row.ApplyStartNumber, row.ApplyEndNumber)) {
                return true;
            }
			return false;
		}
        public bool check50DistanceStr(String applyStartNumber, String applyEndNumber)
        {
            if (applyStartNumber == null || applyEndNumber == null) {
                return false;
            }
            int startNumber = int.Parse(applyStartNumber);
            int endNumber = int.Parse(applyEndNumber);
            return check50Distance(startNumber, endNumber);
        }
        public bool check50Distance(int startNumber, int endNumber) {

            int check = (endNumber - startNumber + 1) % 50;
            int check2 = endNumber - startNumber+1 ;
            if (check == 0 && check2>=50)
            {
                return true;
            }
            else {
                return false;
            }
        }

        public bool checkIsExist(int? guiNumberID,  int? guiWordID, String startNumber, String endNumber)
        {
            
            PXResultset<GVGuiNumber> gvGuiNumberStartNumber= PXSelect<GVGuiNumber,
                                  Where<GVGuiNumber.guiWordID,Equal< Required <GVGuiNumber.guiWordID>>,
                                  And<GVGuiNumber.guiNumberID,NotEqual<Required<GVGuiNumber.guiNumberID>>,
                                  And<Where<GVGuiNumber.applyStartNumber,LessEqual<Required<GVGuiNumber.applyStartNumber>>, 
                                  And <GVGuiNumber.applyEndNumber, GreaterEqual < Required < GVGuiNumber.applyStartNumber >>>>>>>>
                                          .Select(this, guiWordID, guiNumberID, startNumber, startNumber);
            /*
            PXResultset<GVGuiNumber> gvGuiNumberStartNumber = PXSelect<GVGuiNumber,
                        Where<GVGuiNumber.applyStartNumber,GreaterEqual<Required<GVGuiNumber.applyStartNumber>>>>
                                .Select(this, startNumber);*/

            GVGuiNumber gvGuiNumber = CodeMaster.Current;
            if (gvGuiNumberStartNumber.Count>0) {
                CodeMaster.Cache.RaiseExceptionHandling<GVGuiNumber.applyStartNumber>(
                                          CodeMaster.Current, gvGuiNumber.ApplyStartNumber,
                                                  new PXSetPropertyException("該號碼已經存在其他發票號碼維護之中", PXErrorLevel.Error));
                return true;
            }
            
            PXResultset<GVGuiNumber> gvGuiNumberEndNumber = PXSelect<GVGuiNumber,
                                  Where<GVGuiNumber.guiWordID, Equal<Required<GVGuiNumber.guiWordID>>,
                                  And<GVGuiNumber.guiNumberID, NotEqual<Required<GVGuiNumber.guiNumberID>>,
                                  And<Where<GVGuiNumber.applyStartNumber, LessEqual<Required<GVGuiNumber.applyEndNumber>>,
                                  And<GVGuiNumber.applyEndNumber, GreaterEqual<Required<GVGuiNumber.applyEndNumber>>>>>>>>
                                          .Select(this, guiWordID, guiNumberID, endNumber, endNumber);
            if (gvGuiNumberEndNumber.Count > 0)
            {
                CodeMaster.Cache.RaiseExceptionHandling<GVGuiNumber.applyEndNumber>(
                               CodeMaster.Current, gvGuiNumber.ApplyEndNumber,
                                       new PXSetPropertyException("該號碼已經存在其他發票號碼維護之中", PXErrorLevel.Error));
                return true;
            }

            return false;
        }


        protected virtual void GVGuiNumber_ApplyEndNumber_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVGuiNumber row = (GVGuiNumber)e.Row;
			if (row == null || e.NewValue == null)
			{
				return;
			}
            checkApplyEndNumber_M(sender, row, (String)e.NewValue);
		}
		public bool checkApplyEndNumber_M(PXCache sender, GVGuiNumber row, String newValue)
		{
			String applyStartNumber = row.ApplyStartNumber;
			String applyEndNumber = newValue;
			if (newValue == null)
			{

					sender.RaiseExceptionHandling<GVGuiNumber.applyEndNumber>(
									row, newValue,
											new PXSetPropertyException("ApplyEndNumber can't Null.", PXErrorLevel.Error));

				return true;
			}
            /*
			if (!ckeckEndGuiNumber(applyEndNumber))
			{
				sender.RaiseExceptionHandling<GVGuiNumber.applyEndNumber>(
									CodeMaster.Current, applyEndNumber,
											new PXSetPropertyException("Last Two number need 50 or 00. ", PXErrorLevel.Error));
				return true;
			}*/
			if (applyStartNumber == null)
			{
				return true;
			}
			int startNumber = int.Parse(applyStartNumber);
			int endNumber = int.Parse(applyEndNumber);
            if (!check50Distance(startNumber, endNumber))
            {
                sender.RaiseExceptionHandling<GVGuiNumber.applyStartNumber>(
                                            CodeMaster.Current, applyStartNumber,
                                                    new PXSetPropertyException(err50Message, PXErrorLevel.Error));
                sender.RaiseExceptionHandling<GVGuiNumber.applyEndNumber>(
                                    CodeMaster.Current, applyEndNumber,
                                            new PXSetPropertyException(err50Message, PXErrorLevel.Error));
              
                return true;
            }

            return false;
		}
		public bool checkLineGuinumber(GVGuiNumberDetail line) {
			String applyEndNumber = CodeMaster.Current.ApplyEndNumber;
			int applyEndNum = int.Parse(applyEndNumber);
			int startNum = int.Parse(line.StartNumber);
			if (line.EndNumber == null) {
				return false;
			}
			int endNum = int.Parse(line.EndNumber);

			if (!ckeckEndGuiNumber(line.EndNumber))
			{
               
                return false;
			}

			if (startNum > endNum || endNum > applyEndNum)
			{
				return false;
			}
            if (!check50Distance(startNum, endNum))
            {
                return false;
            }
            return true;
		}
        public bool checkAllLine()
        {
            bool enable = true;
            foreach (GVGuiNumberDetail detail in CodeDetails.Select()) {
                enable = enable && checkLine(detail);
            }
            return enable;
        }
        public bool checkLineEX(GVGuiNumberDetail data) {
            String applyStartNumber = CodeMaster.Current.ApplyStartNumber;
            String applyEndNumber = CodeMaster.Current.ApplyEndNumber;
            //bool check = true;
            bool disable = !(applyStartNumber == null || applyEndNumber == null) && check50DistanceStr(applyStartNumber, applyEndNumber)
                        && checkLine(data);
            return disable;
        }
        public bool checkLine(GVGuiNumberDetail data)
        {
            if (CodeDetails.Select().Count == 0) {
                return true;
            }
            if (data.EndNumber == null)
            {
                return false;
            }
            //檢查最後的Line是否大於Master的End Number
            String applyEndNumber = CodeMaster.Current.ApplyEndNumber;
            int applyEndNum = int.Parse(applyEndNumber);
            int startNum = int.Parse(data.StartNumber);
            bool check = true;
            if (data.EndNumber == null)
            {
                check = false;
            }

            int endNum = int.Parse(data.EndNumber);
            if (startNum >= applyEndNum || endNum > applyEndNum)
            {
                check = false;
            }
            //CodeDetails.AllowInsert = check;
            if (!check50Distance(startNum, endNum))
            {
                
                return false;
            }
            /*
            CodeDetails.Cache.RaiseExceptionHandling<GVGuiNumberDetail.endNumber>(
                                   row, endNumber,
                                           new PXSetPropertyException("Last Two number need 50 or 00.", PXErrorLevel.Error));*/
            return check;

        }


        protected virtual void GVGuiNumberDetail_EndNumber_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVGuiNumberDetail row = (GVGuiNumberDetail)e.Row;
			if (row == null )
			{
				return;
			}
			checkEndNumber_D(sender, row, (String)e.NewValue);
            

        }
		public bool checkEndNumber_D(PXCache sender,GVGuiNumberDetail row,String newValue) {
			String startNumber = row.StartNumber;
			String endNumber = newValue;
			if (newValue ==null)
			{
					sender.RaiseExceptionHandling<GVGuiNumberDetail.endNumber>(
									row, endNumber,
											new PXSetPropertyException("EndNumber can't Null.", PXErrorLevel.Error));
                //CodeDetails.AllowInsert = false;
                return true;
			}
            /*
			if (!ckeckEndGuiNumber(endNumber))
			{
					sender.RaiseExceptionHandling<GVGuiNumberDetail.endNumber>(
									row, endNumber,
											new PXSetPropertyException("Last Two number need 50 or 00.", PXErrorLevel.Error));
				return true;
			}*/
			int startNum = int.Parse(startNumber);
			int endNum = int.Parse(endNumber);
			String applyEndNumber = CodeMaster.Current.ApplyEndNumber;
			int applyEndNum = int.Parse(applyEndNumber);
			if (startNum > endNum || endNum > applyEndNum)
			{
				sender.RaiseExceptionHandling<GVGuiNumberDetail.endNumber>(
									row, endNumber,
				new PXSetPropertyException("End Number have to bigger than StartNumber 50 " +
									"And ApplyEndNumber have to bigger than EndNumber. ", PXErrorLevel.Error));
                //CodeDetails.AllowInsert = false;
                return true;
			}
            if (!check50Distance(startNum, endNum))
            {
                sender.RaiseExceptionHandling<GVGuiNumberDetail.endNumber>(
                                            row, endNumber,
                                                    new PXSetPropertyException(err50Message, PXErrorLevel.Error));

               //CodeDetails.AllowInsert = false;
                return true;
            }
            return false;
		}


		public String formatGuiNumber(String guiNumber)
		{
			if (!guiNumber[7].Equals('1'))
			{
				guiNumber = guiNumber.Remove(7, 1);
				guiNumber = guiNumber.Insert(7, "1");

				if (!guiNumber[6].Equals('0'))
				{
					guiNumber = guiNumber.Remove(6, 1);
					guiNumber = guiNumber.Insert(6, "5");
				}
			}
			return guiNumber;
		}
		public bool ckeckStartGuiNumber(String guiNumber) {
			try
			{
				if (!guiNumber[7].Equals('1') || !(guiNumber[6].Equals('0') || guiNumber[6].Equals('5')))
				{
					return false;
				}
			}
			catch (IndexOutOfRangeException e) {
			
				return false;
			}
			return true;
		}
		public bool ckeckEndGuiNumber(String guiNumber)
		{
			try
			{
				if (!guiNumber[7].Equals('0') || !(guiNumber[6].Equals('0') || guiNumber[6].Equals('5')))
				{
					return false;
				}
			}
			catch (IndexOutOfRangeException e)
			{
				return false;
			}
			return true;
		}
        /*
        public PXAction<GVGuiNumber> gVGuiNumberDetailEnable;
        [PXUIField(DisplayName = "GVGuiNumberDetail", MapEnableRights = PXCacheRights.Select,
                          MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable GVGuiNumberDetailEnable(PXAdapter adapter)
        {
            int count= CodeDetails.Select().Count;
            int currentCount = 0;
            foreach (GVGuiNumberDetail detail in CodeDetails.Select()) {
          
                currentCount = currentCount + 1;
                if (currentCount != count)
                {
                    PXUIFieldAttribute.SetEnabled<GVGuiNumberDetail.endNumber>(CodeDetails.Cache, detail, false);
                }
                else {

                }
                PXUIFieldAttribute.SetEnabled<GVGuiNumberDetail.endNumber>(CodeDetails.Cache, detail, false);
            }
            return   adapter.Get();
        }*/
        public virtual void GVGuiNumberDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            GVGuiNumberDetail row = (GVGuiNumberDetail)e.Row;

        }
        protected virtual void GVGuiNumber_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
            GVGuiNumber master = CodeMaster.Current;
            PXEntryStatus status = CodeMaster.Cache.GetStatus(master);
            if (status != PXEntryStatus.Deleted && master != null)
            {
                e.Cancel = getCheckThread();
            }
		}
		protected virtual void 	GVGuiNumberDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
            GVGuiNumber master = CodeMaster.Current;
            PXEntryStatus status = CodeMaster.Cache.GetStatus(master);
            if (status != PXEntryStatus.Deleted  && master != null)
            {
                e.Cancel = getCheckThread();
            }
        }
		public bool checkWordID_M(PXCache sender,GVGuiNumber row) {
			if(row.DeclarePeriod==null  ){
					sender.RaiseExceptionHandling<GVGuiNumber.declarePeriod>(
									CodeMaster.Current, row.DeclarePeriod,
											new PXSetPropertyException("DeclarePeriod  can't be  null. ", PXErrorLevel.Error));
				return true;
			}
			if (row.DeclareYear == null)
			{
					sender.RaiseExceptionHandling<GVGuiNumber.declareYear>(
									CodeMaster.Current, row.DeclareYear,
											new PXSetPropertyException("DeclareYear  can't be  null. ", PXErrorLevel.Error));
				return true;
			}
			if (row.GuiWordID == null)
			{
				sender.RaiseExceptionHandling<GVGuiNumber.guiWordID>(
								row, row.GuiWordID,
										new PXSetPropertyException("GuiWordID can't Null.", PXErrorLevel.Error));

				return true;
			}
			if(row.GuiWordID != null){
				CheckGVGuiWord.Current = CheckGVGuiWord.Search<GVGuiWord.guiWordID>(row.GuiWordID);
				GVGuiWord checkGVGuiNumber = CheckGVGuiWord.Current;
				if (	!(row.DeclarePeriod.Equals(checkGVGuiNumber.DeclarePeriod) &&
						row.DeclareYear.Equals(checkGVGuiNumber.DeclareYear))  ){
					sender.RaiseExceptionHandling<GVGuiNumber.declarePeriod>(
									CodeMaster.Current, row.DeclarePeriod,
											new PXSetPropertyException("DeclarePeriod  is not mapping GuiWorfID. "
												, PXErrorLevel.Error));
					sender.RaiseExceptionHandling<GVGuiNumber.declareYear>(
									CodeMaster.Current, row.DeclareYear,
											new PXSetPropertyException("DeclareYear  is not mapping GuiWorfID. "
												, PXErrorLevel.Error));
                    return true;
                }
			}
			return false;
		}
        public override void Persist()
        {

            GVGuiNumber master = CodeMaster.Current;
            PXEntryStatus status = CodeMaster.Cache.GetStatus(master);
            if (master == null) {
                base.Persist();
                return;
            }

            setCheckThread(ckeckApplyStartNumber_M(CodeMaster.Cache, master, master.ApplyStartNumber));
            setCheckThread(checkApplyEndNumber_M(CodeMaster.Cache, master, master.ApplyEndNumber));
            setCheckThread(checkWordID_M(CodeMaster.Cache, master));
            if (getCheckThread())
            {
                return;
            }


            if (CodeDetails.Select().Count == 0)
            {
                throw new PXSetPropertyException("At Least One Detail Line.", PXErrorLevel.Error);
            }

            foreach (GVGuiNumberDetail row in CodeDetails.Select())
            {
                setCheckThread(checkEndNumber_D(CodeDetails.Cache, row, row.EndNumber));
            }

            //20200414
            int lastStartNum = 0;
            int lastEndNum = 0;
            PXView view = new PXView(this, true, CodeDetails.View.BqlSelect);
            view.OrderByNew<OrderBy<Asc<GVGuiNumberDetail.lineNbr>>>();
            foreach (GVGuiNumberDetail row in view.SelectMulti())
            {
                int currentStartNum = 0;
                int currentEndNum = 0;
                if (row.EndNumber != null ) {
                    currentStartNum = int.Parse(row.StartNumber);
                    currentEndNum = int.Parse(row.EndNumber);
                    if (lastEndNum != 0  && (lastEndNum + 1) != currentStartNum) {
                        CodeDetails.Cache.RaiseExceptionHandling<GVGuiNumberDetail.startNumber>(
                                    row, row.StartNumber, new PXSetPropertyException("沒有連號請刪除此筆資料", PXErrorLevel.RowError));
                        setCheckThread(false);
                    }
                }
                lastStartNum = currentStartNum;
                lastEndNum = currentEndNum;
            }

            
            if (getCheckThread())
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            } else{
                base.Persist();
            }

        }
    }
}