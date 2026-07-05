using Arquanix.Application.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArquanixApi.Controllers;


public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult HandleFailure(ServiceResult result)
    {
        if (result.Status == ServiceResultStatus.NotFound)
        {
            return NotFound();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return ValidationProblem(ModelState);
    }
}
