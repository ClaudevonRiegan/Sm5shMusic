﻿using System.Collections.Generic;

namespace Sm5sh.ResourceProviders.Prc.Helpers
{
    public class PcrFilterStruct<T>
    {
        public PrcHash40 Id { get; set; }

        public List<T> Values { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

}
