﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public class MiItemValue : ItemValue
    {
        public MiItemValue(ItemMetadata itemMetadata, string value, string checkSum) : base(itemMetadata, value)
        {
            Checksum = checkSum;
        }

        public string Checksum { get; }
    }
}
