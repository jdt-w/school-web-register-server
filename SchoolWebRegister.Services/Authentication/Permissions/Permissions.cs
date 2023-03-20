namespace SchoolWebRegister.Domain.Permissions
{
    [Flags]
    public enum Permissions
    {
        Read = 0x00,
        Create = 0x01,
        Edit = 0x02,
        Delete = 0x04,
        Admin = (Read | Create | Edit | Delete)
    }
}
