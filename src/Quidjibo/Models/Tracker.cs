namespace Quidjibo.Models
{
    public sealed class Tracker
    {
        public int Value { get; }

        public string Text { get; }

        public Tracker(int value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}