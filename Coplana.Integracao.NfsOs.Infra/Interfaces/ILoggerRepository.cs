using Coplana.Integracao.NfsOs.Domain.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Infra.Interfaces
{
    public interface ILoggerRepository
    {
        Task Logger(LogIntegration logIntegration);
    }
}
