using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharptamer
{
    namespace Utils
    {
        class Hash
        {
            private static ulong rot(ulong x, int k)
            {
                return (x << k) | (x >> (32 - k));
            }

            private static void mix(ref ulong first, ref ulong second, ref 
ulong third)
            {
                first -= third;
                first ^= rot(third, 4);
                third += second;

                second -= first;
                second ^= rot(first, 6);
                first += third;

                third -= second;
                third ^= rot(second, 8);
                second += first;

                first -= third;
                first ^= rot(third, 16);
                third += second;

                second -= first;
                second ^= rot(first, 19);
                first += third;

                third -= second;
                third ^= rot(second, 4);
                second += first;
            }

            private static void finalise(ref ulong first, ref ulong second, 
ref 
ulong third)
            {
                third ^= second;
                third -= rot(second, 14);

                first ^= third;
                first -= rot(third, 11);

                second ^= first;
                second -= rot(first, 25);

                third ^= second;
                third -= rot(second, 16);

                first ^= third;
                first -= rot(third, 4);

                second ^= first;
                second -= rot(first, 14);

                third ^= second;
                third -= rot(second, 24);
            }

            public static int hashcode(ulong[] vals)
            {
                ulong first = 0, second=0,third =0;
                for (int i = 0; i < vals.Length; i += 3)
                {
                    if (i + 3 >= vals.Length)
                    {
                        first += vals[i];
                        if (i + 1 < vals.Length)
                        {
                            second += vals[i + 1];
                        }
                        if (i + 2 < vals.Length)
                        {
                            third += vals[i + 2];
                        }
                        finalise(ref first,ref second, ref third);
                    }
                    else
                    {
                        first += vals[i];
                        second += vals[i + 1];
                        third += vals[i + 2];
                        mix(ref first,ref second, ref third);
                    }
                }

                return (int)third;
            }
        }
    }
}
