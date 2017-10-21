﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            SharpFace.Windows.Native.Init();

            TestBase t =
                //new LandmarkTestVid();
                new LandmarkWrapperTest();
            var ret = t.Run();

            Console.Write($"======== Test Finished {ret} =======");
            Console.Read();
        }
    }
}
