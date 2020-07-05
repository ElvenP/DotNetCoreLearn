namespace Admin.Core.Common.Configs
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public class AppConfig
    {

        /// <summary>
        /// Api地址，默认 http://*:48352
        /// </summary>
        public string Urls { get; set; } = "http://*:48352";
        /// <summary>
        /// Swagger文档
        /// </summary>
        public bool Swagger { get; set; } = false;


        /// <summary>
        /// Aop配置
        /// </summary>
        public AopConfig Aop { get; set; } = new AopConfig();
    }
}