using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IProjectQuestionsRepository
    {
        IEnumerable<ProjectQuestions> GetAll(int? projectID = null, int? questionID = null);
        IEnumerable<ProjectQuestions> GetAllByProjectID(int? projectID = null, int? questionID = null);
        int AddNewProjectQuestions(ProjectQuestions projectQuestions);
        int UpdateProjectQuestions(ProjectQuestions projectQuestions);
        int DeleteProjectQuestions(int projectID, int questionID);
    }
}
