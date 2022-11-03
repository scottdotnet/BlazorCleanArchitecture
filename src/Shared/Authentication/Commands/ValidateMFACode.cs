using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Shared.Authentication.Commands
{
    public sealed record ValidateMFACode : IRequest<bool>
    {
        public int UserId { get; set; }
        public int MFACode { get; set; }
    }
}
