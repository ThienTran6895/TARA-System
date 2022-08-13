namespace MB.Web.Core
{
    public class MBInnerIcon 
    {
        public string Icon { get; set; }
        public bool IsBefore { get; set; }
        public string Icon2 { get; set; }
        public bool IsBeforeAfter { get; set; }
        public MBInnerIcon(string icon, bool isBefore = true, string icon2 = null, bool isBeforeAfter = false)
        {
            this.Icon = icon;
            this.IsBefore = isBefore;
            this.Icon2 = icon2;
            this.IsBeforeAfter = isBeforeAfter;
        }
    }
}