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
        [BurstCompile(FloatMode = FloatMode.Fast)]
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

        [BurstCompile(FloatMode = FloatMode.Fast)]
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

        [BurstCompile(FloatMode = FloatMode.Fast)]
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

        /// <summary>Installs the Burst loops before anything else runs (the menus, a game, training).</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Install()
        {
            PolicyNet.FastMath = new BurstPolicyMath();
            Debug.Log("[PokemonTCG] The Neural AI's network runs with Burst.");
        }
    }
}
