using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kedge.DAC
{
    public interface IKGSelfInspectionLine
    {
       
        int? SelfInspectionID { get; }
        int? InventoryID { get; }
        string InvDesc { get; }
        string TestResult { get; }
    }
}
