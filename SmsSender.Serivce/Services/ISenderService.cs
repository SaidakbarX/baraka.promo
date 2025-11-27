using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsSender.Serivce.Services
{
    public interface ISenderService
    {
        Task SendData();
        Task<bool> SendPushData();
    }
}
