using System.Collections.Generic;

namespace CommonModel.Models
{
    public class BaseResult
    {
        public BaseResult()
        {
            ResultCodes = new List<Result>();
        }

        public bool IsSuccess { get; set; }

        public IEnumerable<Result> ResultCodes { get; set; }
    }
}
