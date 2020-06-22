using System;
using System.IO;
using System.Text;

namespace Admin.Core.Common.Helpers
{
     public class FileHelper : IDisposable
    {
        private bool _alreadyDispose;

        ~FileHelper()
        {
            Dispose();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDispose) return;
            _alreadyDispose = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="strings">文件内容</param>
        public static void WriteFile(string path, string strings)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            StreamWriter streamWriter = new StreamWriter(path, false);
            streamWriter.Write(strings);
            streamWriter.Close();
            streamWriter.Dispose();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="strings">文件内容</param>
        /// <param name="encode">编码格式</param>
        public static void WriteFile(string path, string strings, Encoding encode)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            StreamWriter streamWriter = new StreamWriter(path, false, encode);
            streamWriter.Write(strings);
            streamWriter.Close();
            streamWriter.Dispose();
        }
        #endregion

        #region 读文件
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            string s;
            if (!File.Exists(path))
                s = "不存在相应的目录";
            else
            {
                StreamReader streamReader = new StreamReader(path);
                s = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
            }

            return s;
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encode">编码格式</param>
        /// <returns></returns>
        public static string ReadFile(string path, Encoding encode)
        {
            string s;
            if (!File.Exists(path))
                s = "不存在相应的目录";
            else
            {
                StreamReader streamReader = new StreamReader(path, encode);
                s = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
            }

            return s;
        }
        #endregion
    }
}