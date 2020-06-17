using System;

namespace Admin.Core.Common.Helpers
{

    /// <summary>
    /// 数据类型转换
    /// </summary>
    public static class UtilConvert
    {
        public static long ToLong(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0L;

            long.TryParse(s.ToString(), out var result);
            return result;
        }
    }
}