using System;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core.Models
{
    public class JoinServiceRequest : ServiceRequest
    {
        public JoinServiceRequest() { }

        public JoinServiceRequest(ServiceRequest serviceRequest)
        {
            ArgumentNullException.ThrowIfNull(serviceRequest);
            ArgumentNullException.ThrowIfNull(serviceRequest.BackendServiceArguments);

            ServiceId = serviceRequest.ServiceId;
            BackendId = serviceRequest.BackendId;
            PlayerId = serviceRequest.PlayerId;

            BackendServiceArguments = serviceRequest.BackendServiceArguments.ToObject<JoinBackendArguments>();
        }

        public new JoinBackendArguments BackendServiceArguments { get; set; }
    }
}
