
namespace github_to_lametric.Models
{
    public class Frame
    {
        public Frame(int index, string icon, string text)
        {
            this.index = index;
            this.icon = icon;
            this.text = text;
        }
        
        public Frame()
        {
            
        }

        public string text { get; set; }
        public string icon { get; set; }
        public int index { get; set; }
    }
}
