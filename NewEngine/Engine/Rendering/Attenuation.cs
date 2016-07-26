namespace NewEngine.Engine.Rendering {
    public class Attenuation {
        public Attenuation(float constant, float linear, float exponent) {
            Constant = constant;
            Linear = linear;
            Exponent = exponent;
        }

        public float Constant { get; set; }

        public float Linear { get; set; }

        public float Exponent { get; set; }
    }
}