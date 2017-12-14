using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrowdSpark.Common;

namespace CrowdSpark.App.Models
{
    public interface ISparkAPI : IDisposable
    {
        Task<IReadOnlyCollection<SparkDTO>> GetSparks();
    }
}
