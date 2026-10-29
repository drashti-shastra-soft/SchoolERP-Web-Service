using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public enum HomeworkSubmissionStatus
    {
         /// <summary>
         /// Homework not submitted yet.
         /// </summary>
        Pending = 1,

        /// <summary>
        /// Homework submitted by student and awaiting evaluation.
        /// </summary>
        Submitted = 2,

        /// <summary>
        /// Homework evaluated by teacher.
        /// </summary>
        Evaluated = 3,

        /// <summary>
        /// Homework submitted after the submission deadline.
        /// </summary>
        Late = 4
    }
}
