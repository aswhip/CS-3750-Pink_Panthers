namespace Pink_Panthers_Project.Models
{
    public class RGB
    {
        public RGB(int red, int green, int blue, int alpha)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        public int red {  get; set; }
        public int green { get; set; }
        public int blue { get; set; }
        public int alpha { get; set; }
    }
}
