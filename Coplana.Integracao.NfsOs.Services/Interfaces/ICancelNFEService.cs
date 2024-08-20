using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Interfaces
{
    
    public interface ICancelNFEService
    {
        Task<bool> ProcessAsync();
    }
}
