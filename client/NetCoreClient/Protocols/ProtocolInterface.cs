using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCoreClient.Protocols
{
    interface ProtocolInterface
    {
        Task Send(string data);  // Modifica per supportare metodi asincroni
    }
}
