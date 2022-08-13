using MB.Web.Core.Validations;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CisCallResult
    {
        public int project { get; set; }
        public string customer_id { get; set; }
        public string phone { get; set; }
        public string status { get; set; }
        public string statusmsg { get; set; }
        public string secretkey { get; set; }
    }
    public class CisResponseResult
    {
        public int resultcode { get; set; }
        public string message { get; set; }
    }

    public class CisCallRequest
    {
        public CisAuthentication authentication { get; set; }
        public CisCallData callData { get; set; }
    }

    public class CisAuthentication
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class CisCallData
    {
        public string projectId { get; set; }
        public string expectCallTime { get; set; }
        public string appId { get; set; }
        public string phoneNumber { get; set; }
        public string customerId { get; set; }
        public string phoneType { get; set; }
    }

    public static class CisCallStatus
    {
        public static string ANSWERED { get { return "ANSWERED"; } }
        public static string NOANSWER { get {return "NO ANSWER"; } }
        public static string BUSY { get { return "BUSY"; } }
        public static string FAILED { get { return "FAILED"; }} 
    }

    public class CisCallModel
    {
        public string ID_WORKLIST { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string EXTENSION { get; set; }
        public string CALL_RESULT { get; set; }
        public string CALL_TIME { get; set; }
        public string END_TIME { get; set; }
        public string DURATION { get; set; }
        public string TIME_IN_QUEUE { get; set; }
        public string RINGING_TIME { get; set; }
        public string TALK_TIME { get; set; }
        public string PATH_MP3 { get; set; }
    }
}
