using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing organization or campus details.
    /// </summary>
    public interface IOrganisationService
    {
        /// <summary>
        /// Gets a list of all organizations registered in the system.
        /// </summary>
        List<OrganisationViewModel> GetAllOrganisations(bool includeDeleted = false);

        /// <summary>
        /// Finds the details of a specific organization using its ID.
        /// </summary>
        OrganisationViewModel? GetOrganisationByID(int organisationID);

        /// <summary>
        /// Adds a new organization or updates an existing one with detailed information (like address, contact, and settings).
        /// </summary>
        (bool success, string message) UpsertOrganisation(OrganisationUpsertRequest request, int userId);

        /// <summary>
        /// Removes an organization from the system.
        /// </summary>
        (bool success, string message) DeleteOrganisation(int organisationID, int userId);

        /// <summary>
        /// Turns an organization's active status on or off.
        /// </summary>
        (bool success, string message) ToggleOrganisationStatus(int organisationID, bool isActive, int userId);
    }
}
