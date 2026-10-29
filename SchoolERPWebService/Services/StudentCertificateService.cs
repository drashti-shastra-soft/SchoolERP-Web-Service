using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class StudentCertificateService: IStudentCertificateService
    {
        private readonly IConfiguration _configuration;
        public StudentCertificateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all student certificates for a given company and session.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <param name="sessionId">The unique identifier of the session.</param>
        /// <returns>A list of <see cref="StudentCertificateViewModel"/> containing certificate details.</returns>
        public List<StudentCertificateViewModel> GetAll(int companyId, int sessionId)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
            var p = new DynamicParameters();
            p.Add("@CompanyID", companyId);
            p.Add("@SessionID", sessionId);

            return conn.Query<StudentCertificateViewModel>(
                "sp_StudentCertificate_GetAll",
                p,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Retrieves a StudentCertificate record by its primary key.
        /// Uses Dapper's automatic column-to-property mapping — no manual field access needed.
        /// Returns null if no record is found.
        /// </summary>
        /// <param name="id">The CertificateID to look up.</param>
        /// <returns>A <see cref="StudentCertificateViewModel"/> or null.</returns>
        public StudentCertificateViewModel GetByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<StudentCertificateViewModel>(
                "sp_StudentCertificate_GetByID",
                new { CertificateID = id },
                commandType: CommandType.StoredProcedure
            );
        }

        //public (int Result, string Message) Upsert(StudentCertificateUpsertRequest request, int userId, int companyId, int sessionId)
        //{
        //    // Preserve existing background image if updating without a new one
        //    if (request.CertificateID > 0 && (request.BackgroundImage == null || request.BackgroundImage.Length == 0))
        //    {
        //        var existing = GetByID(request.CertificateID);
        //        if (existing?.BackgroundImage != null)
        //        {
        //            request.BackgroundImage = existing.BackgroundImage;
        //            request.BackgroundImageType = existing.BackgroundImageType;
        //            request.BackgroundImageName = existing.BackgroundImageName;
        //        }
        //    }

        //    using var conn = new SqlConnection(
        //        _configuration.GetConnectionString("DefaultConnection"));

        //    // Dapper maps anonymous object properties → SP parameters automatically
        //    var result = conn.QueryFirstOrDefault(
        //        "sp_StudentCertificate_Upsert",
        //        new
        //        {
        //            request.CertificateID,
        //            request.CertificateName,
        //            request.HeaderLeftText,
        //            request.HeaderCenterText,
        //            request.HeaderRightText,
        //            request.BodyText,
        //            request.FooterLeftText,
        //            request.FooterCenterText,
        //            request.FooterRightText,
        //            request.HeaderHeight,
        //            request.FooterHeight,
        //            request.BodyHeight,
        //            request.BodyWidth,
        //            request.EnableStudentPhoto,
        //            request.BackgroundImage,
        //            request.BackgroundImageType,
        //            request.BackgroundImageName,
        //            request.IsActive,
        //            UserId = userId,
        //            CompanyID = companyId,
        //            SessionID = sessionId
        //        },
        //        commandType: CommandType.StoredProcedure
        //    );

        //    // QueryFirstOrDefault returns a dynamic row — no foreach, no DataTable, direct property access
        //    return result is not null
        //        ? ((int)result.Result, (string)(result.Message ?? ""))
        //        : (0, "Failed to upsert certificate template.");
        //}


        public (bool success, string message) Upsert(StudentCertificateUpsertRequest request, int userId, int companyId, int sessionId)
        {
            try
            {
                // Convert base64 text to bytes, because the SP column is VARBINARY(MAX).
                // If no new image was chosen, this stays null and the SP keeps the old image.
                byte[]? imageBytes = (request.BackgroundImage != null && request.BackgroundImage.Length > 0)
                        ? request.BackgroundImage
                        : null;

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CertificateID", request.CertificateID);
                parameters.Add("@CertificateName", request.CertificateName);
                parameters.Add("@HeaderLeftText", request.HeaderLeftText);
                parameters.Add("@HeaderCenterText", request.HeaderCenterText);
                parameters.Add("@HeaderRightText", request.HeaderRightText);
                parameters.Add("@BodyText", request.BodyText);
                parameters.Add("@FooterLeftText", request.FooterLeftText);
                parameters.Add("@FooterCenterText", request.FooterCenterText);
                parameters.Add("@FooterRightText", request.FooterRightText);
                parameters.Add("@HeaderHeight", request.HeaderHeight);
                parameters.Add("@FooterHeight", request.FooterHeight);
                parameters.Add("@BodyHeight", request.BodyHeight);
                parameters.Add("@BodyWidth", request.BodyWidth);
                parameters.Add("@EnableStudentPhoto", request.EnableStudentPhoto);
                parameters.Add("@BackgroundImage", imageBytes, DbType.Binary, size: -1);
                parameters.Add("@BackgroundImageType", request.BackgroundImageType);
                parameters.Add("@BackgroundImageName", request.BackgroundImageName);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", userId);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_StudentCertificate_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        //public (int Result, string Message) Delete(List<int> id, int userId)
        //{
        //    if (id == null || !id.Any())
        //    {
        //        return (0, "No students selected for deletion.");
        //    }

        //    using var conn = new SqlConnection(
        //        _configuration.GetConnectionString("DefaultConnection"));

        //    string certificateIDs = string.Join(",", id);

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@CertificateID", certificateIDs);
        //    parameters.Add("@UserId", userId);

        //    var result = conn.QueryFirstOrDefault(
        //        "sp_StudentCertificate_Delete",
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    if (result != null)
        //    {
        //        return (
        //            (int)result.Result,
        //            (string)result.Message ?? "Failed to delete certificate template."
        //        );
        //    }

        //    return (0, "Failed to delete certificate template.");
        //}

        public (bool success, string message) Delete(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No certificates selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string certificateIDs = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@CertificateID", certificateIDs);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_StudentCertificate_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool success, string message) ToggleStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CertificateID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_StudentCertificate_ToggleStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public string GenerateCertificate(int studentId, int certificateId, int companyId, int sessionId)
        {
            var template = GetByID(certificateId);
            if (template == null) return "Template not found";

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var row = conn.QueryFirstOrDefault("SP_STUDENT_DETAILS_GET",
                new { STUDENTID = studentId, COMPANYID = companyId, SESSIONID = sessionId },
                commandType: CommandType.StoredProcedure);

            if (row == null) return "Student not found";

            // Cast to IDictionary for dynamic column access
            var data = (IDictionary<string, object>)row;

            // Helper: safely get string value from dynamic row
            string GetVal(string col)
            {
                var key = data.Keys.FirstOrDefault(k => string.Equals(k, col, StringComparison.OrdinalIgnoreCase));
                return key != null ? data[key]?.ToString() ?? "" : "";
            }

            // Helper: safely parse date from dynamic row
            string GetDate(string col)
            {
                var key = data.Keys.FirstOrDefault(k => string.Equals(k, col, StringComparison.OrdinalIgnoreCase));
                if (key == null || data[key] == null) return "";
                return data[key] is DateTime dt ? dt.ToString("dd-MM-yyyy") : "";
            }

            string body = template.BodyText ?? "";

            // Build placeholder → value map (no foreach needed; Replace handles all keys inline)
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["[name]"] = $"{GetVal("FIRSTNAME")} {GetVal("MIDDLENAME")} {GetVal("LASTNAME")}".Trim().Replace("  ", " "),
                ["[dob]"] = GetDate("DOB"),
                ["[present_address]"] = GetVal("CURRENTADDRESS"),
                ["[guardian]"] = GetVal("GUARDIANNAME"),
                ["[admission_no]"] = GetVal("ADMISSIONNO"),
                ["[roll_no]"] = GetVal("ROLLNO"),
                ["[class]"] = GetVal("ClassName"),
                ["[section]"] = GetVal("SectionName"),
                ["[gender]"] = GetVal("GENDER"),
                ["[admission_date]"] = GetDate("ADMISSIONDATE"),
                ["[father_name]"] = GetVal("FATHERNAME"),
                ["[mother_name]"] = GetVal("MOTHERNAME"),
                ["[phone]"] = GetVal("MOBILENO"),
                ["[email]"] = GetVal("EMAIL"),
                ["[religion]"] = GetVal("RELIGION"),
                ["[category]"] = GetVal("StudentCategoryName"),
                ["[cast]"] = GetVal("CASTE"),
                ["[created_at]"] = DateTime.Now.ToString("dd-MM-yyyy")
            };

            // Apply all placeholder replacements — map.Aggregate replaces foreach + body.Replace loop
            body = map.Aggregate(body, (current, kv) =>
                current.Replace(kv.Key, kv.Value, StringComparison.OrdinalIgnoreCase));

            // Handle student photo placeholder
            string studentPhotoHtml = "";
            if (template.EnableStudentPhoto)
            {
                var photoKey = data.Keys.FirstOrDefault(k => string.Equals(k, "STUDENTPHOTO", StringComparison.OrdinalIgnoreCase));
                var typeKey = data.Keys.FirstOrDefault(k => string.Equals(k, "STUDENTPHOTOTYPE", StringComparison.OrdinalIgnoreCase));

                byte[] photo = photoKey != null && data[photoKey] is byte[] b ? b : null;
                string photoType = typeKey != null ? data[typeKey]?.ToString() ?? "image/png" : "image/png";

                if (photo is { Length: > 0 })
                {
                    string base64 = Convert.ToBase64String(photo);
                    studentPhotoHtml = $"<img src=\"data:{photoType};base64,{base64}\" style=\"width:100px;height:auto;border:1px solid #ccc;\" alt=\"Student Photo\" />";
                }
            }

            body = body.Replace("[student_photo]", studentPhotoHtml, StringComparison.OrdinalIgnoreCase);

            return body;
        }
    }
}
