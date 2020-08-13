using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.ViewModels.Home
{
    public class OperationResult<TEntity>
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public TEntity Entity { get; set; }
    }
}
