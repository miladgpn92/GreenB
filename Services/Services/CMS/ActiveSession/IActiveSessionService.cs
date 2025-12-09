using Common;
using Microsoft.AspNetCore.Http;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.ActiveSession
{
	public interface IActiveSessionService
	{
        ResponseModel AddActiveSession(int UserId , HttpContext context,string Jwt);

		Task<ResponseModel<List<ActiveSessionDto>>> GetSession(int UserId, HttpContext context, CancellationToken cancellationToken);

        ResponseModel RemoveActiveSession(int SessionId);
	}
}
