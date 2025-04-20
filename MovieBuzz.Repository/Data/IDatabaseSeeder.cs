using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Data
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }
}
