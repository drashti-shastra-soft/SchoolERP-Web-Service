using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IPaperMarksDetailsService
    {
        (bool success, string message) UpsertPaperMarksDetails(PaperMarksDetailsSaveRequest request);
        PaperMarksDetailsViewModel? GetPaperMarksDetailsByID(int paperMarksDetailID);
        (bool success, string message) DeletePaperMarksDetails(List<int> paperMarksDetailID, int userId, string ipAddress);
        (bool success, string message) CopyPaperMarksDetails(PaperMarksDetailsCopyRequest request);
    }
}
