using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStudioSystem.Controllers
{
    [Authorize]
    public class ControllerBase : Controller
    {
 
    }
}
