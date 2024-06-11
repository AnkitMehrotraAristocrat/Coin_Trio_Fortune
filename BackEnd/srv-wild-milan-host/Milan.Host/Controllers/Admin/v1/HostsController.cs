using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Interfaces.Utilities;
using ProductMadness.Phoenix.Api.Controllers;
using Wildcat.Milan.Host.Core.Utilities;
using Wildcat.Milan.Host.Utilities;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Controllers.Admin.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("admin/v{version:apiVersion}/slots")]
    public class HostsController : AdminControllerBase
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IBackend _backend;
        private readonly IHostVersionHelper _hostVersionHelper;

        public HostsController(ILogger<HostsController> logger, IConfigurationManager configurationManager, IBackend backend, IHostVersionHelper hostVersionHelper) : base(logger)
        {
            _configurationManager = configurationManager;
            _backend = backend;
            _hostVersionHelper = hostVersionHelper;
        }

        /// <summary>
        /// Returns information about the hosted slot machine, it's services and variations.
        /// </summary>
        [HttpGet("~/admin/v1/slots")]
        public ActionResult<GameVersionModel> GetGame()
        {
            var hostedGame = _backend.GetGameVersionDetails(_configurationManager, _hostVersionHelper);

            if (hostedGame != null)
            {
                return Ok(hostedGame);
            }

            return NotFound();
        }
    }
}
