using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Helpers
{
    public enum EnumTypeOfCampo
    {
        Normale = 0,
        AutocompletamentoCsv = 1,
        AutocompletamentoDbSqlite = 2,
        AutocompletamentoDbSql = 3
    }
}
