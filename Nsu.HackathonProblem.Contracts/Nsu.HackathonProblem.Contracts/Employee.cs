﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsu.HackathonProblem.Contracts
{
    public record Employee(int Id, string Name, List<Employee> Wishlist);
}
