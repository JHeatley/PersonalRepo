using Unity.Burst;
using PokemonTCG.AI;
using UnityEngine;

namespace PokemonTCG.Burst
{
    /// <summary>
    /// The Neural AI's inner loops compiled by Unity's Burst compiler, which turns them into machine code that uses the CPU's
    /// vector instructions (plain C# in Unity can't). This assembly only builds when the Burst package is installed
    /// (com.unity.burst 1.6 or later, see PokemonTCG.Burst.asmdef); without it the network uses its plain C# loops, which give
    /// the same numbers, only slower.
    /// </summary>
    [BurstCompile]
    public static unsafe class PolicyKernels
    {
        [BurstCompile(FloatMode = FloatMode.Fast, CompileSynchronously = true)]
        public static void MatVecAcc([NoAlias] float* w, int stride, int rows, int cols, [NoAlias] float* x, [NoAlias] float* y)
        {
            for (int o = 0; o < rows; o++)
            {
                float* r = w + (long)o * stride;
                float sum = 0f;
                for (int i = 0; i < cols; i++) sum += r[i] * x[i];
                y[o] += sum;
            }
        }

        [BurstCompile(FloatMode = FloatMode.Fast, CompileSynchronously = true)]
        public static void MatTVecAcc([NoAlias] float* w, int stride, int rows, int cols, [NoAlias] float* dy, [NoAlias] float* dx)
        {
            for (int o = 0; o < rows; o++)
            {
                float d = dy[o];
                if (d == 0f) continue;
                float* r = w + (long)o * stride;
                for (int i = 0; i < cols; i++) dx[i] += d * r[i];
            }
        }

        [BurstCompile(FloatMode = FloatMode.Fast, CompileSynchronously = true)]
        public static void OuterAcc([NoAlias] float* gw, int stride, int rows, int cols, [NoAlias] float* dy, [NoAlias] float* x)
        {
            for (int o = 0; o < rows; o++)
            {
                float d = dy[o];
                if (d == 0f) continue;
                float* g = gw + (long)o * stride;
                for (int i = 0; i < cols; i++) g[i] += d * x[i];
            }
        }
    }

    /// <summary>Hands the arrays to the Burst kernels (pinned for the duration of the call).</summary>
    public sealed unsafe class BurstPolicyMath : IPolicyMath
    {
        public void MatVecAcc(float[] w, int wo, int stride, int rows, int cols, float[] x, int xo, float[] y, int yo)
        {
            if (rows <= 0 || cols <= 0) return;
            fixed (float* pw = w)
            fixed (float* px = x)
            fixed (float* py = y)
                PolicyKernels.MatVecAcc(pw + wo, stride, rows, cols, px + xo, py + yo);
        }

        public void MatTVecAcc(float[] w, int wo, int stride, int rows, int cols, float[] dy, int dyo, float[] dx, int dxo)
        {
            if (rows <= 0 || cols <= 0) return;
            fixed (float* pw = w)
            fixed (float* pdy = dy)
            fixed (float* pdx = dx)
                PolicyKernels.MatTVecAcc(pw + wo, stride, rows, cols, pdy + dyo, pdx + dxo);
        }

        public void OuterAcc(float[] gw, int wo, int stride, int rows, int cols, float[] dy, int dyo, float[] x, int xo)
        {
            if (rows <= 0 || cols <= 0) return;
            fixed (float* pg = gw)
            fixed (float* pdy = dy)
            fixed (float* px = x)
                PolicyKernels.OuterAcc(pg + wo, stride, rows, cols, pdy + dyo, px + xo);
        }

        /// <summary>
        /// Installs the Burst loops before anything else runs (the menus, a game, training), once they've shown they give the
        /// same numbers as the plain C# loops on random matrices. If they don't, the network keeps the C# loops.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Install()
        {
            PolicyNet.FastMath = null;
            string problem = SelfTest(new BurstPolicyMath());
            if (problem != null)
            {
                Debug.LogWarning("[PokemonTCG] Burst gave different numbers from the C# loops (" + problem + "); the Neural AI uses the C# loops.");
                return;
            }
            PolicyNet.FastMath = new BurstPolicyMath();
            Debug.Log("[PokemonTCG] The Neural AI's network runs with Burst (checked against the C# loops).");
        }

        /// <summary>Compares the loops with the C# ones on random matrices of the network's sizes; null when they agree.</summary>
        public static string SelfTest(IPolicyMath fast)
        {
            var rng = new System.Random(12345);
            var managed = ManagedPolicyMath.Instance;
            int[][] shapes = { new[] { 96, 122, 122 }, new[] { 384, 384, 384 }, new[] { 384, 96, 2260 }, new[] { 128, 198, 198 }, new[] { 1, 192, 192 } };
            foreach (var shape in shapes)
            {
                int rows = shape[0], cols = shape[1], stride = shape[2];
                int wo = rng.Next(0, 5), xo = rng.Next(0, 5), yo = rng.Next(0, 5);
                var w = Random(rng, wo + rows * stride);
                var x = Random(rng, xo + cols);
                var y = Random(rng, yo + rows);
                var y1 = (float[])y.Clone();
                var y2 = (float[])y.Clone();
                fast.MatVecAcc(w, wo, stride, rows, cols, x, xo, y1, yo);
                managed.MatVecAcc(w, wo, stride, rows, cols, x, xo, y2, yo);
                string bad = Compare(y1, y2, "matrix x vector");
                if (bad != null) return bad;
                var dx1 = Random(rng, xo + cols);
                var dx2 = (float[])dx1.Clone();
                fast.MatTVecAcc(w, wo, stride, rows, cols, y, yo, dx1, xo);
                managed.MatTVecAcc(w, wo, stride, rows, cols, y, yo, dx2, xo);
                bad = Compare(dx1, dx2, "transposed");
                if (bad != null) return bad;
                var g1 = Random(rng, wo + rows * stride);
                var g2 = (float[])g1.Clone();
                fast.OuterAcc(g1, wo, stride, rows, cols, y, yo, x, xo);
                managed.OuterAcc(g2, wo, stride, rows, cols, y, yo, x, xo);
                bad = Compare(g1, g2, "outer product");
                if (bad != null) return bad;
            }
            return null;
        }

        private static float[] Random(System.Random rng, int n)
        {
            var a = new float[n];
            for (int i = 0; i < n; i++) a[i] = (float)(rng.NextDouble() * 2 - 1);
            return a;
        }

        private static string Compare(float[] a, float[] b, string what)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (float.IsNaN(a[i]) || float.IsInfinity(a[i])) return what + ": not a number at " + i;
                if (System.Math.Abs(a[i] - b[i]) > 1e-3f * (1 + System.Math.Abs(b[i]))) return what + ": " + a[i] + " instead of " + b[i] + " at " + i;
            }
            return null;
        }
    }
}
