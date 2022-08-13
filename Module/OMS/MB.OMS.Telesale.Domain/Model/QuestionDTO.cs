using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Common.Helpers;
namespace MB.OMS.Telesale.Domain.Model
{
    public class QuestionDTO : Question
    {
        public QuestionDTO()
        {
            AvailableSurveys = new List<SurveyDTO>();
        }
        public string TieuDeNgan
        {
            get { return CommonHelper.CutString(50,Name); }
            set { ;}
        }
        
        public List<SurveyDTO> AvailableSurveys { get; set; }

        [DisplayName("Thứ tự hiển thị ")]
        public int DisplayOrder { get; set; }

    }
}
