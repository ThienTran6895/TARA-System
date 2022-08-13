namespace MB.Common.Resource
{
    public class ResxControls
    {
        public static dynamic Resources
        {
            get
            {
                return MBResourceManager.LoadResource("ResxControls");
            }
        }
    }
}
