using InsertInto.ModelComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateCore.EventManagement;
using UltimateCore.LRI;

namespace InsertInto.Contracts
{
    public class ReturnDTPContract : IContract<bool>
    {
        public ReturnDTPContract(DTP dtp)
        {
            DTP = dtp;
        }

        public DTP DTP { get; }
        public Result<bool> Result { get; set; }
    }
}
