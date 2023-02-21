using PX.Data;
using System;
using System.IO;
using System.Text;

namespace RC.Util
{
    public class RCEleTxtFileUtil
    {
        String fileName;
        MemoryStream memoryStream;
        StreamWriter sw;
        String encoding;
        public String ENCODING { get { return encoding; } }
        public String FILENAME { get { return fileName; } }


        public RCEleTxtFileUtil(String fileName, String encoding)
        {
            this.fileName = fileName + ".txt";
            this.memoryStream = new MemoryStream();
            this.encoding = encoding;
            this.sw = new StreamWriter(this.memoryStream, System.Text.Encoding.GetEncoding(encoding));
        }

        public virtual void InputLine(String str)
        {
            sw.WriteLine(str);
        }

        public virtual void Dowload()
        {
            this.sw.Flush();
            this.sw.Close();
            byte[] bytes = this.memoryStream.ToArray();
            this.memoryStream.Close();
            throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(),
                                                           this.fileName,
                                                           null, bytes
                                                           ), true);
        }

        public byte[] Close()
        {
            this.sw.Flush();
            this.sw.Close();
            byte[] bytes = this.memoryStream.ToArray();
            this.memoryStream.Close();
            return bytes;
        }


        /// <summary>
        /// 字串處理<br/>
        /// 
        /// </summary>
        /// <param name="str">代處理字串</param>
        /// <param name="lenght">長度限制</param>
        /// <param name="isLeft">True:左補空白 ; False:右補空白</param>
        /// <param name="paddingChar">填補字串</param>
        /// <returns></returns>
        public static String GetDataStrByLenght(String str, int lenght, bool isLeft, char paddingChar)
        {
            String data = str ?? "";
            data = data.Trim();
            data = data.Substring(0, lenght > data.Length ? data.Length : lenght);
            return isLeft ? data.PadLeft(lenght, paddingChar) : data.PadRight(lenght, paddingChar);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">代處理字串</param>
        /// <param name="lenght">長度限制</param>
        /// <param name="isLeft">True:左補空白 ; False:右補空白</param>
        /// <param name="_encoding">編碼</param>
        /// <returns></returns>
        public static String GetDataStrByByte(String str, int lenght, bool isLeft, String _encoding)
        {
            return GetDataStrByByte(str, lenght, isLeft, ' ', _encoding);
        }

        /// <summary>
        /// 字串處理<br/>
        /// 
        /// </summary>
        /// <param name="str">代處理字串</param>
        /// <param name="lenght">長度限制</param>
        /// <param name="isLeft">True:左補空白 ; False:右補空白</param>
        /// <param name="paddingChar">填補字串</param>
        /// <param name="_encoding">編碼</param>
        /// <returns></returns>
        public static String GetDataStrByByte(String str, int lenght, bool isLeft, char paddingChar, String _encoding)
        {
            Encoding encoding = System.Text.Encoding.GetEncoding(_encoding);
            String data = str ?? "";
            data = data.Trim();
            byte[] bytes = encoding.GetBytes(data.ToString());
            if (bytes.Length > lenght)
            {
                return encoding.GetString(bytes, 0, lenght);
            }
            else
            {
                data = isLeft ? data.PadLeft(lenght, paddingChar) : data.PadRight(lenght, paddingChar);
                bytes = encoding.GetBytes(data.ToString());
                return encoding.GetString(bytes, 0, lenght);
            }
        }

    }
}
