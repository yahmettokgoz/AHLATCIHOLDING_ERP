namespace HoldingERP.WebUI.Models
{
    public class RoleSelection
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class EditRolesViewModel
    {
        public EditRolesViewModel()
        {
            Roles = new List<RoleSelection>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleSelection> Roles { get; set; }
    }
}
