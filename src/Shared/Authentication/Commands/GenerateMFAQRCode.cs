using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Shared.Authentication.Commands
{
    public sealed record GenerateMFAQRCode : IRequest<string>
    {
        public int UserId { get; set; }
    }
}
