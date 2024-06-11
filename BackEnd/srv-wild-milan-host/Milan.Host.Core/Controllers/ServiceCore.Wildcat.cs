using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Milan.Common.Logging;
using ProductMadness.Phoenix.Api.Contracts;
using ProductMadness.Phoenix.Api.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wildcat.Milan.Host.Core.Utilities;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core.Controllers
{
    public partial class ServiceCore
    {
        public const string ERROR_CODE_INSUFFICIENT_BALANCE = "insufficient_balance";
        public const string ERROR_CODE_MAX_BET_LEVEL = "max_bet_level";

        /// <summary>
        /// Overriding the generic problem handling from Microsoft.AspNetCore.Mvc.ControllerBase
        /// to return the Product Madness Phoenix error message format.
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="instance"></param>
        /// <param name="statusCode"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override ObjectResult Problem(
            string? detail = null,
            string? instance = null,
            int? statusCode = null,
            string? title = null,
            string? type = null)
        {
            return this.Problem(detail, title, statusCode, null);
        }

        /// <summary>
        /// Overriding the generic problem handling from Microsoft.AspNetCore.Mvc.ControllerBase
        /// to return the Product Madness Phoenix error message format.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorCode">Human-readable error code</param>
        /// <param name="statusCode">HTTP error status code</param>
        /// <param name="request">Incoming API request to be serialized in the error model if desired</param>
        /// <returns></returns>
        protected ObjectResult Problem(
            string? errorMessage = null,
            int? statusCode = null,
            object? request = null)
        {
            return this.Problem(errorMessage, ErrorCodes.VALIDATION_ERROR, statusCode, request);
        }

        /// <summary>
        /// Overriding the generic problem handling from Microsoft.AspNetCore.Mvc.ControllerBase
        /// to return the Product Madness Phoenix error message format.
        /// </summary>
        /// <param name="validationResult">FluentValidation validation result. Will use FIRST error in error contract.</param>
        /// <param name="request">Incoming API request to be serialized in the error model if desired</param>
        /// <returns></returns>
        protected ObjectResult Problem(
            ValidationResult validationResult,
            object? request = null)
        {
            var firstError = validationResult.Errors.FirstOrDefault();

            return this.Problem(
                errorMessage: firstError.ErrorMessage
                , errorCode: ErrorCodes.VALIDATION_ERROR
                , statusCode: StatusCodes.Status422UnprocessableEntity
                , request: request
                , firstError?.PropertyName);
        }

        /// <summary>
        /// Overriding the generic problem handling from Microsoft.AspNetCore.Mvc.ControllerBase
        /// to return the Product Madness Phoenix error message format.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorCode">Human-readable error code</param>
        /// <param name="statusCode">HTTP error status code</param>
        /// <param name="request">Incoming API request to be serialized in the error model if desired</param>
        /// <returns></returns>
        private ObjectResult Problem(
            string errorMessage,
            string errorCode,
            int? statusCode = null,
            object? request = null,
            string propertyName = null)
        {
            var errorContract = new ErrorContract(
                applicationName: this.AppConfiguration.GetSpringApplicationName()
                , errorCode: errorCode
                , errorMessage: errorMessage,
                new
                {
                    path = Request.Path,
                    method = Request.Method,
                    request,
                    property_name = propertyName ?? null
                });

            return new ObjectResult(errorContract)
            {
                StatusCode = statusCode ?? 500
            };
        }

        /// <summary>
        /// Returns information about the hosted slot machine, it's services and variations.
        /// </summary>
        [HttpGet("~/internal/v1/slots")]
        public ActionResult<GameVersionModel> GetGame()
        {
            return Ok(_gameVersionDetails);
        }

        [HttpPost("~/internal/v1/slots")]
        public async Task<ActionResult<ServiceResponse>> InvokeSlots([FromBody] ServiceRequest request)
        {
            EnrichNewRelicTransaction(request);
            return await ProcessRequest(request);
        }

        [HttpPost("~/internal/v1/slots/spin")]
        public async Task<ActionResult<SpinResponse>> Spin([FromBody] SpinRequest request)
        {
            try
            {
                return await SpinInternal(request);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError<ServiceCore>($"***ERROR***", ex);
                throw;
            }
        }

        [HttpGet("~/internal/v1/jackpots/{gameAccountId}")]
        public async Task<ActionResult<GetJackpotsResponseModel>> GetJackpots(
        [FromRoute] string gameAccountId, CancellationToken cancellationToken)
        {
            var requestModel = new GetJackpotsRequestModel()
            {
                PlayerId = gameAccountId,
                GameIds = new string[0],
                TableId = null
            };

            return await GetJackpotValues(requestModel, cancellationToken);
        }
    }
}