using Microsoft.AspNetCore.Mvc;
using Web.core.Attributes;

namespace Web.core.Controllers
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [Route("api/[area]/[controller]/[action]")]
    [ApiController]
    [Permission]
    public class BaseController: ControllerBase
    {
        
    }
}