using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CopySessionAPIController : ControllerBase
    {
        private readonly IStudentInformationService _studentService;
        private readonly ISubjectService _subjectService;
        private readonly IClassService _classService;
        private readonly IHostelService _hostelService;
        private readonly IHostelTypeService _hostelTypeService;
        private readonly IHolidayTypeService _holidayTypeService;
        private readonly IHolidayService _holidayService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        public CopySessionAPIController(IStudentInformationService studentService,
            ISubjectService subjectService, IClassService classService, IHostelService hostelService,
            IHostelTypeService hostelTypeService, IHolidayTypeService holidayTypeService, 
            IHolidayService holidayService, ICountryService countryService, IStateService stateService)
        {
            _studentService = studentService;
            _subjectService = subjectService;
            _classService = classService;
            _hostelService = hostelService;
            _hostelTypeService = hostelTypeService;
            _holidayTypeService = holidayTypeService;
            _holidayService = holidayService;
            _countryService = countryService;
            _stateService = stateService;
        }

        [HttpGet("Student")]
        public async Task<IActionResult> GetStudentList(int sessionId, int companyId)
        {
            try
            {
                var data = await _studentService.GetStudentCopyList(companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message=ex.Message, });
            }           
        }


        // ── COPY STUDENTS TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) students from one session to another.
        /// POST /CopySession/CopyStudents
        /// </summary>
        [HttpPost("CopyStudents")]
        public async Task<IActionResult> CopyStudents([FromBody] CopyRequest request)
        {
            try
            {
               
                // ── Execute copy ──────────────────────────────────────────
                var result = await _studentService.CopyStudentsToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {                
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }



        // ── COPY Subject TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Subject from one session to another.
        /// POST /CopySession/CopySubject
        /// </summary>
        [HttpPost("CopySubject")]
        public async Task<IActionResult> CopySubject([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _subjectService.CopySubjectToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY STUDENTS TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) students from one session to another.
        /// POST /CopySession/CopyStudents
        /// </summary>
        [HttpPost("CopyClasss")]
        public async Task<IActionResult> CopyClasss([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _classService.CopyClassToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }



        // ── COPY Student House TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies all StudentHouse from one session to another.
        /// POST /CopySession/CopyStudentHouse
        /// </summary>
        [HttpPost("CopyStudentHouse")]
        public async Task<IActionResult> CopyStudentHouse([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _classService.CopyStudentHouseToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }


        // ── COPY Room Types TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Room Types from one session to another.
        /// POST /CopySession/CopyRoomTypes
        /// </summary>
        [HttpPost("CopyRoomTypes")]
        public async Task<IActionResult> CopyRoomTypes([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _hostelService.CopyRoomTypeToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY Room Types TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Room Types from one session to another.
        /// POST /CopySession/CopyRoomTypes
        /// </summary>
        [HttpPost("CopyHostels")]
        public async Task<IActionResult> CopyHostels([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _hostelService.CopyHotelToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }


        // ── COPY Hostel Type TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Hostel Type from one session to another.
        /// POST /CopySession/CopyHostelType
        /// </summary>
        [HttpPost("CopyHostelType")]
        public async Task<IActionResult> CopyHostelType([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _hostelTypeService.CopyHostelTypeToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY Hostel Type TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Hostel Type from one session to another.
        /// POST /CopySession/CopyHostelType
        /// </summary>
        [HttpPost("CopyHolidayType")]
        public async Task<IActionResult> CopyHolidayType([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _holidayTypeService.CopyHolidayTypeToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY Hostel Type TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Hostel Type from one session to another.
        /// POST /CopySession/CopyHostelType
        /// </summary>
        [HttpPost("CopyHolidays")]
        public async Task<IActionResult> CopyHolidays([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _holidayService.CopyHolidayToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY Hostel Type TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Hostel Type from one session to another.
        /// POST /CopySession/CopyHostelType
        /// </summary>
        [HttpPost("CopyCountrs")]
        public async Task<IActionResult> CopyCountrs([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _countryService.CopyCountryToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY States Type TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) States Type from one session to another.
        /// POST /CopySession/CopyStates
        /// </summary>
        [HttpPost("CopyStates")]
        public async Task<IActionResult> CopyStates([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _stateService.CopyStateToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        // ── COPY Hostel Type TO SESSION ──────────────────────────────────────
        /// <summary>
        /// Copies selected (or all) Hostel Type from one session to another.
        /// POST /CopySession/CopyHostelType
        /// </summary>
        [HttpPost("CopyRoomCoolingType")]
        public async Task<IActionResult> CopyRoomCoolingType([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _hostelService.CopyRoomCoolingTypeToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }
    }
}
