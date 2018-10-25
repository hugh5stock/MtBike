using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Utils
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public string MsgCode { get; set; }
        public string Message { get; set; }
        public object ResObject { get; set; }
        public ResultModel()
        {
            IsSuccess = true;
            MsgCode = ResPrompt.Success;
            //Message = "";
            //ResObject = "";
        }
    }




    public class ResultModel<T>
    {
        public bool IsSuccess { get; set; }

        public string MsgCode { get; set; }

        public string Message { get; set; }

        public T ResObject { get; set; }

        public ResultModel()
        {
            IsSuccess = true;
            MsgCode = ResPrompt.Success;
            Message = string.Empty;
        }
    }

    public class UMUploadResultModel
    {
        public string state { get; set; }

        public string url { get; set; }

        public string title { get; set; }

        public string original { get; set; }

        public string error { get; set; }
    }
}
