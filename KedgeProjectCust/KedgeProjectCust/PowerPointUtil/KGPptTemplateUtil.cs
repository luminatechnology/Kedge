using PptxTemplater;
using PX.Data;
using System;
using System.IO;

namespace Kedge.Util
{
    public class KGPptTemplateUtil
    {
        private MemoryStream ms = null;
        private Pptx pptx= null;
        private PptxSlide currentSlide = null;

        /**
         * <summary>
         * PowerPoint範本工具(範本必須是.pptx)
         * </summary>
         * **/
        public KGPptTemplateUtil(byte[] templatePptBytes) {
            //將範本bytes轉換為MemoryStream
            MemoryStream tempMs = new MemoryStream(templatePptBytes);
            //複製範本的MemoryStream
            ms = new MemoryStream();
            tempMs.CopyTo(ms);
            tempMs.Close();
            pptx = new Pptx(ms,FileAccess.ReadWrite);
        }

        /**
         * <summary>
         * 取得最後一頁投影片範本，產生新的投影片
         * </summary>
         * **/
        public void NewPage() {
            //取得最後一頁
            PptxSlide lastSlide = pptx.GetSlide(pptx.SlidesCount() - 1);
            PptxSlide newSlide = lastSlide.Clone();//複製slide
            PptxSlide.InsertAfter(newSlide, lastSlide);//插入新slide
            //取得倒數第二頁(新)
            currentSlide = pptx.GetSlide(pptx.SlidesCount() - 2);
        }

        /**
         * <summary>
         * 取代當前投影片tag文字
         * </summary>
         * **/
        public void ReplaceTextTag(String tagName , String value) {
            if (tagName != null) {
                currentSlide.ReplaceTag(tagName, value, PptxTemplater.PptxSlide.ReplacementType.Global);
            }
        }

        /**
         * <summary>
         * 取代當前投影片Image(透過替代文字)
         * </summary>
         * **/
        public void ReplaceImageTag(String tagName, byte[] bytes, String contentType)
        {
            if (tagName != null)
            {
                currentSlide.ReplacePicture(tagName, bytes, contentType);
            }
        }

        /**
         * <summary>
         * 產生完成，回傳產生後的bytes
         * </summary>
         * **/
        public byte[] Complete() {
            //把最後一張投影片刪除，index 從0算起 
            PptxSlide last = pptx.GetSlide(pptx.SlidesCount() - 1);
            last.Remove();
            //關檔
            pptx.Close();
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }

        #region
        public static PX.SM.UploadFileRevision GetUploadFileRevision(PXGraph graph,Guid? noteID)
        {
            return PXSelectJoin<PX.SM.UploadFileRevision,
                    InnerJoin<NoteDoc, On<NoteDoc.fileID, Equal<PX.SM.UploadFileRevision.fileID>>>
                    , Where<NoteDoc.noteID, Equal<Required<NoteDoc.noteID>>>>.Select(graph, noteID);
        }

        #endregion
    }
}
