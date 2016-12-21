using System;

namespace Splix.Net.Test
{
    class console {
		public static void log(dynamic str) {
            Console.WriteLine(str);
        }
	}
    class Assert {
        public static void equal(dynamic a, dynamic b){
            equal(a, b, "");
        }
        public static void equal(dynamic a, dynamic b, string description){
            if (a != b) {
                Console.WriteLine("  error {2}: {0} != {1}", a, b, description);
                Environment.Exit(1);
            }
        }
    }
}