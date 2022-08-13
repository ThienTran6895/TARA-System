using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ProjectsDTO : Projects
    {
        public ProjectsDTO()
        {
            AvailableCampaign = new List<SelectListItem>();
            AvailableSource = new List<SelectListItem>();
            AvailableQuestions = new List<QuestionDTO>();
        }

        public List<SelectListItem> AvailableCampaign { get; set; }
        public List<SelectListItem> AvailableSource { get; set; }
        public List<QuestionDTO> AvailableQuestions { get; set; }

        [DisplayName("Nguồn ")]
        public int SearchSourceID1 { get; set; }

        [DisplayName("Nguồn ")]
        public int SearchSourceID2 { get; set; }

        [DisplayName("Thứ tự hiển thị ")]
        public int DisplayOrder { get; set; }
    }
}
