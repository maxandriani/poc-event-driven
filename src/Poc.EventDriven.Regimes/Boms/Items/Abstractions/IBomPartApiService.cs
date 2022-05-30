using Poc.EventDriven.Services.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Boms.Items.Abstractions;

public interface IBomPartApiService : ICrudService<BomPartDto, GetBomPartByKey, SearchBomPartRequest, CreateUpdateBomPartDto>
{
}
