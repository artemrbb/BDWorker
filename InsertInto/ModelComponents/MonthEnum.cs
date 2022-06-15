using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertInto.ModelComponents
{
    public enum MonthEnum
    {
        None = 0,
        [Description("januaryTable")]
        JanuaryTable = 1,
        [Description("februaryTable")]
        FebruaryTable = 2,
        [Description("marchTable")]
        MarchTable = 3,
        [Description("aprilTable")]
        AprilTable = 4,
        [Description("mayTable")]
        MayTable = 5,
        [Description("juneTable")]
        JuneTable = 6,
        [Description("julyTable")]
        JulyTable = 7,
        [Description("augustTable")]
        AugustTable = 8,
        [Description("septemberTable")]
        SeptemberTable = 9,
        [Description("octoberTable")]
        OctoberTable = 10,
        [Description("novemberTable")]
        NovemberTable = 11,
        [Description("decemberTable")]
        DecemberTable = 12
    }
}
