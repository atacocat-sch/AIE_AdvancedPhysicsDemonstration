using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public static class Util
    {
        public static int Wrap (int i, int l)
        {
            return i > 0 ? i % l : l + (i % l);
        }
    }
}
