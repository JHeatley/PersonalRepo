namespace PokemonTCG.AI
{
    /// <summary>
    /// The policy network's inner loops (nearly all of its time). PolicyNet runs them through PolicyNet.FastMath when something
    /// faster is installed (Unity's Burst compiler, see Assets/PokemonTCG/Burst), otherwise through plain C# (ManagedPolicyMath).
    /// Every version must give the same numbers. Matrices are row-major float arrays: row o of a matrix starting at wo with
    /// row length stride is w[wo + o * stride ...].
    /// </summary>
    public interface IPolicyMath
    {
        /// <summary>y[yo + o] += sum_i w[wo + o * stride + i] * x[xo + i] for o &lt; rows, i &lt; cols.</summary>
        void MatVecAcc(float[] w, int wo, int stride, int rows, int cols, float[] x, int xo, float[] y, int yo);

        /// <summary>dx[dxo + i] += sum_o dy[dyo + o] * w[wo + o * stride + i] for o &lt; rows, i &lt; cols.</summary>
        void MatTVecAcc(float[] w, int wo, int stride, int rows, int cols, float[] dy, int dyo, float[] dx, int dxo);

        /// <summary>gw[wo + o * stride + i] += dy[dyo + o] * x[xo + i] for o &lt; rows, i &lt; cols.</summary>
        void OuterAcc(float[] gw, int wo, int stride, int rows, int cols, float[] dy, int dyo, float[] x, int xo);
    }

    /// <summary>The loops in plain C# (with System.Numerics vectors where the runtime has them).</summary>
    public sealed class ManagedPolicyMath : IPolicyMath
    {
        public static readonly ManagedPolicyMath Instance = new ManagedPolicyMath();

        public void MatVecAcc(float[] w, int wo, int stride, int rows, int cols, float[] x, int xo, float[] y, int yo)
        {
            for (int o = 0; o < rows; o++) y[yo + o] += PolicyNet.Dot(w, wo + o * stride, x, xo, cols);
        }

        public void MatTVecAcc(float[] w, int wo, int stride, int rows, int cols, float[] dy, int dyo, float[] dx, int dxo)
        {
            for (int o = 0; o < rows; o++)
            {
                float d = dy[dyo + o];
                if (d != 0f) PolicyNet.Axpy(d, w, wo + o * stride, dx, dxo, cols);
            }
        }

        public void OuterAcc(float[] gw, int wo, int stride, int rows, int cols, float[] dy, int dyo, float[] x, int xo)
        {
            for (int o = 0; o < rows; o++)
            {
                float d = dy[dyo + o];
                if (d != 0f) PolicyNet.Axpy(d, x, xo, gw, wo + o * stride, cols);
            }
        }
    }
}
